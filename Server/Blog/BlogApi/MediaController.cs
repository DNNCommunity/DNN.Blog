using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Core.Media;
using DotNetNuke.Web.Api;
using Microsoft.Extensions.Logging;

namespace DotNetNuke.Modules.Blog.BlogApi
{
  public class MediaRouteMapper : IServiceRouteMapper
  {
    public void RegisterRoutes(IMapRoute mapRouteManager)
    {
      mapRouteManager.MapHttpRoute(
          "BlogApi",
          "BlogApiMediaUpload",
          "modules/{moduleId}/blogs/{blogId}/media",
          new { Controller = "Media", Action = "Upload" },
          new { moduleId = "-?\\d+", blogId = "-?\\d+", httpMethod = new HttpMethodConstraint(HttpMethod.Post) },
          new[] { "DotNetNuke.Modules.Blog.BlogApi" });
    }
  }

  public class MediaController : BlogApiController
  {
    private readonly ILogger<MediaController> _logger;
    private readonly MediaUploadService _uploadService = new MediaUploadService();

    public MediaController(ILogger<MediaController> logger)
    {
      _logger = logger;
    }

    public class UploadResultDto
    {
      public string Url { get; set; }
      public string FileName { get; set; }
    }

    [HttpPost]
    [ApiTokenAuthorize("BlogApi", "~/DesktopModules/MVC/Blog/App_LocalResources/SharedResources.resx", Web.Api.Auth.ApiTokens.Models.ApiTokenScope.User)]
    [Route("/api/BlogApi/modules/{moduleId}/blogs/{blogId}/media")]
    public async Task<HttpResponseMessage> Upload(int moduleId, int blogId)
    {
      _logger.LogDebug("BlogApi media upload: module {ModuleId}, blog {BlogId}, user {UserId}",
          moduleId, blogId, UserInfo.UserID);

      var ctx = BlogApiResourceHelper.Resolve(moduleId, blogId, PortalSettings, UserInfo);
      if (ctx == null)
        return Request.CreateResponse(HttpStatusCode.NotFound, "Module or blog not found");

      if (!BlogApiResourceHelper.UserCanEditBlog(ctx))
        return AccessViolation("You do not have permission to upload media to this blog");

      if (!Request.Content.IsMimeMultipartContent())
        return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);

      var provider = new MultipartMemoryStreamProvider();
      await Request.Content.ReadAsMultipartAsync(provider);

      int postId = 0;
      string originalName = null;
      Stream fileStream = null;

      foreach (var part in provider.Contents)
      {
        var name = (part.Headers.ContentDisposition.Name ?? "").Trim('"');
        if (string.Equals(name, "postId", StringComparison.OrdinalIgnoreCase))
        {
          int.TryParse(await part.ReadAsStringAsync(), out postId);
        }
        else if (string.Equals(name, "file", StringComparison.OrdinalIgnoreCase)
                 || part.Headers.ContentDisposition.FileName != null)
        {
          originalName = (part.Headers.ContentDisposition.FileName ?? "").Trim('"');
          if (originalName.IndexOf('\\') >= 0)
            originalName = Path.GetFileName(originalName);
          fileStream = await part.ReadAsStreamAsync();
        }
      }

      var result = _uploadService.Upload(
          fileStream,
          originalName,
          PortalSettings.PortalId,
          blogId,
          postId > 0 ? (int?)postId : null,
          UserInfo.UserID);

      switch (result.Status)
      {
        case MediaUploadResult.StatusKind.Success:
          _logger.LogInformation("BlogApi media uploaded {FileName} for blog {BlogId}", result.FileName, blogId);
          return Request.CreateResponse(HttpStatusCode.OK, new UploadResultDto
          {
            Url = result.Url,
            FileName = result.FileName
          });
        case MediaUploadResult.StatusKind.BadRequest:
          return Request.CreateResponse(HttpStatusCode.BadRequest, result.Message);
        case MediaUploadResult.StatusKind.Forbidden:
          return Request.CreateResponse(HttpStatusCode.Forbidden, result.Message);
        default:
          return Request.CreateResponse(HttpStatusCode.InternalServerError, result.Message);
      }
    }
  }
}
