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

namespace DotNetNuke.Modules.Blog.Api
{
    public class MediaController : BlogApiController
    {
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
            if (fileId < 1)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "fileId is required");

            var file = FileManager.Instance.GetFile(fileId);
            if (file == null)
                return Request.CreateResponse(HttpStatusCode.NotFound, "File not found");

            var folder = FolderManager.Instance.GetFolder(file.FolderId);
            if (folder == null || !FolderPermissionController.HasFolderPermission(folder.FolderPermissions, "READ"))
                return AccessViolation("You do not have permission to read this file");

            var url = FileManager.Instance.GetUrl(file);
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
            if (!Request.Content.IsMimeMultipartContent())
                return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);

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

            if (blogId < 1)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "blogId is required");
            if (fileStream == null || string.IsNullOrEmpty(originalName))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "file is required");

            var ext = Path.GetExtension(originalName);

            if (MediaFileService.IsImageExtension(ext))
            {
                try
                {
                    var img = new ImageService().AddImage(
                        fileStream,
                        originalName,
                        PortalSettings.PortalId,
                        blogId,
                        postId > 0 ? (int?)postId : null,
                        UserInfo.UserID);
                    var url = ImageUrls.Build(img.ImageId, 0, ImageUrls.Version(img));
                    return Request.CreateResponse(HttpStatusCode.OK, new UploadResultDto
                    {
                        Url = url,
                        FileName = img.OriginalFileName ?? img.StoredFileName
                    });
                }
                catch (ImageRejectedException ex)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, ex.Message);
                }
            }

            if (MediaFileService.IsAllowedFileExtension(ext))
            {
                try
                {
                    var file = new MediaFileService().AddFile(
                        fileStream,
                        originalName,
                        PortalSettings.PortalId,
                        blogId,
                        postId > 0 ? (int?)postId : null,
                        UserInfo.UserID);
                    var url = FileUrls.Build(file.FileId, FileUrls.Version(file));
                    return Request.CreateResponse(HttpStatusCode.OK, new UploadResultDto
                    {
                        Url = url,
                        FileName = file.OriginalFileName
                    });
                }
                catch (MediaRejectedException ex)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, ex.Message);
                }
                catch (Exception ex)
                {
                    Logger.Error("Media file upload failed for " + originalName, ex);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError,
                        "File upload failed: " + ex.Message);
                }
            }

            return Request.CreateResponse(HttpStatusCode.Forbidden, "This type of file is not allowed.");
        }
    }
}
