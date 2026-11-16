using System;
using System.IO;

namespace DotNetNuke.Modules.Blog.Core.Media
{
  public class MediaUploadResult
  {
    public enum StatusKind
    {
      Success,
      BadRequest,
      Forbidden,
      Error
    }

    public StatusKind Status { get; set; }
    public string Message { get; set; }
    public string Url { get; set; }
    public string FileName { get; set; }
  }

  public class MediaUploadService
  {
    public MediaUploadResult Upload(Stream fileStream, string originalName, int portalId, int blogId, int? postId, int userId)
    {
      if (blogId < 1)
        return new MediaUploadResult { Status = MediaUploadResult.StatusKind.BadRequest, Message = "blogId is required" };

      if (fileStream == null || string.IsNullOrEmpty(originalName))
        return new MediaUploadResult { Status = MediaUploadResult.StatusKind.BadRequest, Message = "file is required" };

      if (originalName.IndexOf('\\') >= 0)
        originalName = Path.GetFileName(originalName);

      var ext = Path.GetExtension(originalName);

      if (MediaFileService.IsImageExtension(ext))
      {
        try
        {
          var img = new ImageService().AddImage(
              fileStream,
              originalName,
              portalId,
              blogId,
              postId,
              userId);
          return new MediaUploadResult
          {
            Status = MediaUploadResult.StatusKind.Success,
            Url = ImageUrls.Build(img.ImageId, 0, ImageUrls.Version(img)),
            FileName = img.OriginalFileName ?? img.StoredFileName
          };
        }
        catch (ImageRejectedException ex)
        {
          return new MediaUploadResult { Status = MediaUploadResult.StatusKind.Forbidden, Message = ex.Message };
        }
      }

      if (MediaFileService.IsAllowedFileExtension(ext))
      {
        try
        {
          var file = new MediaFileService().AddFile(
              fileStream,
              originalName,
              portalId,
              blogId,
              postId,
              userId);
          return new MediaUploadResult
          {
            Status = MediaUploadResult.StatusKind.Success,
            Url = FileUrls.Build(file.FileId, FileUrls.Version(file)),
            FileName = file.OriginalFileName
          };
        }
        catch (MediaRejectedException ex)
        {
          return new MediaUploadResult { Status = MediaUploadResult.StatusKind.Forbidden, Message = ex.Message };
        }
        catch (Exception ex)
        {
          return new MediaUploadResult
          {
            Status = MediaUploadResult.StatusKind.Error,
            Message = "File upload failed: " + ex.Message
          };
        }
      }

      return new MediaUploadResult
      {
        Status = MediaUploadResult.StatusKind.Forbidden,
        Message = "This type of file is not allowed."
      };
    }
  }
}
