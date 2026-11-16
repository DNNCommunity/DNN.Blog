using DotNetNuke.Modules.Blog.Core.Media;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace DotNetNuke.Modules.Blog.Api
{
  public class FileController : Common.BlogApiController
  {
    private readonly ILogger<FileController> _logger;
    private static readonly MediaFileService Service = new MediaFileService();

    public FileController(ILogger<FileController> logger) : base()
    {
      _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public HttpResponseMessage Get(Guid id, string v = null)
    {
      _logger.LogDebug("File get requested: id {FileId}, version {Version}, portal {PortalId}",
          id, v, PortalSettings.PortalId);

      var file = Service.Get(id);
      if (file == null || file.PortalId != PortalSettings.PortalId)
      {
        _logger.LogDebug("File {FileId} not found or wrong portal", id);
        return Request.CreateResponse(HttpStatusCode.NotFound);
      }

      if (file.IsTemp)
      {
        var u = UserInfo;
        var allowed = u != null && u.UserID > 0 &&
            (u.UserID == file.CreatedByUserId || u.IsSuperUser || u.IsInRole(PortalSettings.AdministratorRoleName));
        if (!allowed)
        {
          _logger.LogWarning("Temp file {FileId} access denied for user {UserId}", id, u?.UserID ?? -1);
          return Request.CreateResponse(HttpStatusCode.NotFound);
        }
        _logger.LogDebug("Temp file {FileId} access granted to user {UserId}", id, u.UserID);
      }

      var path = Service.GetPath(file);
      if (string.IsNullOrEmpty(path) || !File.Exists(path))
      {
        _logger.LogWarning("File {FileId} path missing on disk: {Path}", id, path);
        return Request.CreateResponse(HttpStatusCode.NotFound);
      }

      var lastModified = File.GetLastWriteTimeUtc(path);
      lastModified = new DateTime(lastModified.Ticks - (lastModified.Ticks % TimeSpan.TicksPerSecond), DateTimeKind.Utc);
      var etag = "\"" + (file.Hash ?? "") + lastModified.Ticks + "\"";

      var cache = new CacheControlHeaderValue { Public = !file.IsTemp };
      if (file.IsTemp) { cache.Private = true; cache.NoCache = true; }
      else if (!string.IsNullOrEmpty(v)) cache.MaxAge = TimeSpan.FromDays(365);
      else cache.MaxAge = TimeSpan.FromHours(1);

      var inm = Request.Headers.IfNoneMatch;
      bool notModified = false;
      foreach (var t in inm) if (t.Tag == etag) notModified = true;

      HttpResponseMessage resp;
      if (notModified)
      {
        _logger.LogDebug("File {FileId} not modified (etag match)", id);
        resp = Request.CreateResponse(HttpStatusCode.NotModified);
      }
      else
      {
        _logger.LogInformation("Serving file {FileId} ({FileName})", id, file.OriginalFileName);
        resp = Request.CreateResponse(HttpStatusCode.OK);
        resp.Content = new StreamContent(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read));
        resp.Content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
        resp.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline")
        {
          FileName = file.OriginalFileName
        };
        resp.Content.Headers.LastModified = lastModified;
      }
      resp.Headers.CacheControl = cache;
      resp.Headers.ETag = new EntityTagHeaderValue(etag);
      resp.Headers.Add("X-Content-Type-Options", "nosniff");
      return resp;
    }
  }
}
