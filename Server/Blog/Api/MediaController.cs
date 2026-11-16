using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Core.Media;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Web.Api;
using Microsoft.Extensions.Logging;

namespace DotNetNuke.Modules.Blog.Api
{
  public class MediaController : BlogApiController
  {
    private readonly ILogger<MediaController> _logger;
    private readonly MediaUploadService _uploadService = new MediaUploadService();

    public MediaController(ILogger<MediaController> logger) : base()
    {
      _logger = logger;
    }

    public class FileUrlDto
    {
      public string Url { get; set; }
      public string FileName { get; set; }
    }

    public class UploadResultDto
    {
      public string Url { get; set; }
      public string FileName { get; set; }
    }

    [HttpGet]
    [BlogAuthorize(SecurityAccessLevel.Edit)]
    public HttpResponseMessage FileUrl(int fileId)
    {
      _logger.LogDebug("FileUrl requested: fileId {FileId}, user {UserId}", fileId, UserInfo.UserID);

      if (fileId < 1)
      {
        _logger.LogWarning("FileUrl failed: invalid fileId {FileId}", fileId);
        return Request.CreateResponse(HttpStatusCode.BadRequest, "fileId is required");
      }

      var file = FileManager.Instance.GetFile(fileId);
      if (file == null)
      {
        _logger.LogWarning("FileUrl failed: file {FileId} not found", fileId);
        return Request.CreateResponse(HttpStatusCode.NotFound, "File not found");
      }

      var folder = FolderManager.Instance.GetFolder(file.FolderId);
      if (folder == null || !FolderPermissionController.HasFolderPermission(folder.FolderPermissions, "READ"))
      {
        _logger.LogWarning("FileUrl denied: user {UserId} cannot read file {FileId}", UserInfo.UserID, fileId);
        return AccessViolation("You do not have permission to read this file");
      }

      var url = FileManager.Instance.GetUrl(file);
      _logger.LogInformation("FileUrl resolved file {FileId} to {FileName}", fileId, file.FileName);
      return Request.CreateResponse(HttpStatusCode.OK, new FileUrlDto
      {
        Url = url,
        FileName = file.FileName
      });
    }

    [HttpPost]
    [BlogAuthorize(SecurityAccessLevel.Edit)]
    [ValidateAntiForgeryToken]
    public async Task<HttpResponseMessage> Upload()
    {
      _logger.LogDebug("Media upload started by user {UserId}", UserInfo.UserID);

      if (!Request.Content.IsMimeMultipartContent())
      {
        _logger.LogWarning("Media upload rejected: not multipart content");
        return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
      }

      var provider = new MultipartMemoryStreamProvider();
      await Request.Content.ReadAsMultipartAsync(provider);

      int blogId = 0;
      int postId = 0;
      string originalName = null;
      Stream fileStream = null;

      foreach (var part in provider.Contents)
      {
        var name = (part.Headers.ContentDisposition.Name ?? "").Trim('"');
        if (string.Equals(name, "blogId", StringComparison.OrdinalIgnoreCase))
        {
          int.TryParse(await part.ReadAsStringAsync(), out blogId);
        }
        else if (string.Equals(name, "postId", StringComparison.OrdinalIgnoreCase))
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

      _logger.LogDebug("Media upload parsed: blogId {BlogId}, postId {PostId}, fileName {FileName}",
          blogId, postId, originalName);

      var result = _uploadService.Upload(
          fileStream,
          originalName,
          PortalSettings.PortalId,
          blogId,
          postId > 0 ? (int?)postId : null,
          UserInfo.UserID);

      return ToHttpResponse(result, blogId, postId, originalName);
    }

    private HttpResponseMessage ToHttpResponse(MediaUploadResult result, int blogId, int postId, string originalName)
    {
      switch (result.Status)
      {
        case MediaUploadResult.StatusKind.Success:
          _logger.LogInformation("Media uploaded as {FileName} for blog {BlogId}, post {PostId}",
              result.FileName, blogId, postId);
          return Request.CreateResponse(HttpStatusCode.OK, new UploadResultDto
          {
            Url = result.Url,
            FileName = result.FileName
          });
        case MediaUploadResult.StatusKind.BadRequest:
          _logger.LogWarning("Media upload failed: {Message}", result.Message);
          return Request.CreateResponse(HttpStatusCode.BadRequest, result.Message);
        case MediaUploadResult.StatusKind.Forbidden:
          _logger.LogWarning("Media upload forbidden for {FileName}: {Message}", originalName, result.Message);
          return Request.CreateResponse(HttpStatusCode.Forbidden, result.Message);
        default:
          _logger.LogError("Media upload error for {FileName}: {Message}", originalName, result.Message);
          return Request.CreateResponse(HttpStatusCode.InternalServerError, result.Message);
      }
    }
  }
}
