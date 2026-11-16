using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Web.Api;
using DotNetNuke.Web.Api.Auth.ApiTokens;
using DotNetNuke.Web.Api.Auth.ApiTokens.Models;
using DotNetNuke.Web.Api.Auth.ApiTokens.Repositories;
using Microsoft.Extensions.Logging;

namespace DotNetNuke.Modules.Blog.Api
{
  public class ApiTokensController : BlogApiController
  {
    public const string BlogApiKey = "blogapi";

    private readonly ILogger<ApiTokensController> _logger;
    private readonly IApiTokenRepository _apiTokenRepository;
    private readonly IApiTokenController _apiTokenController;
    private readonly ApiTokenSettingsRepository _apiTokenSettingsRepository;

    public ApiTokensController(
      ILogger<ApiTokensController> logger,
      IApiTokenRepository apiTokenRepository,
      IApiTokenController apiTokenController,
      ApiTokenSettingsRepository apiTokenSettingsRepository) : base()
    {
      _logger = logger;
      _apiTokenRepository = apiTokenRepository;
      _apiTokenController = apiTokenController;
      _apiTokenSettingsRepository = apiTokenSettingsRepository;
    }

    public class ApiTokenDto
    {
      public int ApiTokenId { get; set; }
      public string TokenName { get; set; }
      public DateTime CreatedOnDate { get; set; }
      public DateTime ExpiresOn { get; set; }
      public DateTime? LastUsedOnDate { get; set; }
      public bool IsRevoked { get; set; }
      public bool IsExpired { get; set; }
      public string Keys { get; set; }
      public string Status { get; set; }
    }

    public class ApiTokenListDto
    {
      public IList<ApiTokenDto> Items { get; set; } = new List<ApiTokenDto>();
      public int Page { get; set; }
      public int PageSize { get; set; }
      public int Total { get; set; }
    }

    public class CreateApiTokenDto
    {
      public string TokenName { get; set; }
    }

    public class CreateApiTokenResultDto
    {
      public string Token { get; set; }
    }

    public class RevokeApiTokenDto
    {
      public int ApiTokenId { get; set; }
    }

    [HttpGet]
    [DnnAuthorize]
    public HttpResponseMessage List(int page = 0, int pageSize = 25)
    {
      if (!TryEnsureApiTokensAllowed(out var errorResponse))
        return errorResponse;

      if (page < 0) page = 0;
      if (pageSize < 1) pageSize = 25;
      if (pageSize > 100) pageSize = 100;

      var portalId = PortalSettings.PortalId;
      var userId = UserInfo.UserID;
      _logger.LogDebug("ApiTokens List: page {Page}, pageSize {PageSize}, user {UserId}", page, pageSize, userId);

      var tokens = _apiTokenRepository.GetApiTokens(
        ApiTokenScope.User,
        false,
        portalId,
        userId,
        ApiTokenFilter.All,
        BlogApiKey,
        page,
        pageSize);

      var now = DateTime.UtcNow;
      var items = tokens.Select(t => ToDto(t, now)).ToList();

      return Request.CreateResponse(HttpStatusCode.OK, new ApiTokenListDto
      {
        Items = items,
        Page = page,
        PageSize = pageSize,
        Total = tokens.TotalCount
      });
    }

    [HttpPost]
    [DnnAuthorize]
    [ValidateAntiForgeryToken]
    public HttpResponseMessage Create(CreateApiTokenDto data)
    {
      if (!TryEnsureApiTokensAllowed(out var errorResponse))
        return errorResponse;

      var name = data?.TokenName?.Trim();
      if (string.IsNullOrWhiteSpace(name))
        return Request.CreateResponse(HttpStatusCode.BadRequest, "Token name is required.");

      if (name.Length > 100)
        return Request.CreateResponse(HttpStatusCode.BadRequest, "Token name is too long.");

      var settings = ApiTokenSettings.GetSettings(_apiTokenSettingsRepository, PortalSettings.PortalId);
      var expiresOn = ComputeExpiration(settings.UserTokenTimespan);
      var portalId = PortalSettings.PortalId;
      var userId = UserInfo.UserID;

      _logger.LogInformation("Creating BlogApi token for user {UserId} on portal {PortalId}", userId, portalId);
      var token = _apiTokenController.CreateApiToken(
        portalId,
        name,
        ApiTokenScope.User,
        expiresOn,
        BlogApiKey,
        userId);

      return Request.CreateResponse(HttpStatusCode.OK, new CreateApiTokenResultDto { Token = token });
    }

    [HttpPost]
    [DnnAuthorize]
    [ValidateAntiForgeryToken]
    public HttpResponseMessage Revoke(RevokeApiTokenDto data)
    {
      if (!TryEnsureApiTokensAllowed(out var errorResponse))
        return errorResponse;

      if (data == null || data.ApiTokenId < 1)
        return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid token.");

      var token = _apiTokenController.GetApiToken(data.ApiTokenId);
      if (token == null)
        return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid token.");

      if (token.PortalId != PortalSettings.PortalId || token.CreatedByUserId != UserInfo.UserID)
        return AccessViolation("You have no access to this token.");

      if (token.Scope != ApiTokenScope.User)
        return AccessViolation("You have no access to this token.");

      _apiTokenController.RevokeOrDeleteApiToken(token, false, UserInfo.UserID);
      _logger.LogInformation("Revoked ApiToken {ApiTokenId} for user {UserId}", token.ApiTokenId, UserInfo.UserID);
      return Request.CreateResponse(HttpStatusCode.OK, true);
    }

    private bool TryEnsureApiTokensAllowed(out HttpResponseMessage errorResponse)
    {
      errorResponse = null;

      if (UserInfo == null || UserInfo.UserID < 1)
      {
        errorResponse = AccessViolation("Authentication required.");
        return false;
      }

      var settings = ApiTokenSettings.GetSettings(_apiTokenSettingsRepository, PortalSettings.PortalId);
      if (!settings.ApiTokensEnabled || !settings.AllowApiTokens)
      {
        errorResponse = Request.CreateResponse(HttpStatusCode.BadRequest, "API tokens are disabled.");
        return false;
      }

      return true;
    }

    private static DateTime ComputeExpiration(ApiTokenTimespan timespan)
    {
      var expirationTime = DateTime.Now;
      switch (timespan)
      {
        case ApiTokenTimespan.Days30:
          return expirationTime.AddDays(30);
        case ApiTokenTimespan.Days60:
          return expirationTime.AddDays(60);
        case ApiTokenTimespan.Days90:
          return expirationTime.AddDays(90);
        case ApiTokenTimespan.Days180:
          return expirationTime.AddDays(180);
        case ApiTokenTimespan.Years1:
          return expirationTime.AddYears(1);
        case ApiTokenTimespan.Years2:
          return expirationTime.AddYears(2);
        default:
          return expirationTime.AddDays(30);
      }
    }

    private static ApiTokenDto ToDto(ApiToken token, DateTime utcNow)
    {
      var isExpired = token.ExpiresOn <= utcNow;
      string status;
      if (token.IsRevoked)
        status = "Revoked";
      else if (isExpired)
        status = "Expired";
      else
        status = "Active";

      return new ApiTokenDto
      {
        ApiTokenId = token.ApiTokenId,
        TokenName = token.TokenName ?? string.Empty,
        CreatedOnDate = token.CreatedOnDate,
        ExpiresOn = token.ExpiresOn,
        LastUsedOnDate = token.LastUsedOnDate,
        IsRevoked = token.IsRevoked,
        IsExpired = isExpired,
        Keys = token.Keys ?? string.Empty,
        Status = status
      };
    }
  }
}
