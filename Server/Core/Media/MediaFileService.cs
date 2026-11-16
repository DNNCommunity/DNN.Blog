using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using DotNetNuke.Data;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Portals;

namespace DotNetNuke.Modules.Blog.Core.Media
{
  public class MediaRejectedException : Exception
  {
    public MediaRejectedException(string message) : base(message) { }
  }

  /// <summary>
  /// Stores non-image files uploaded from the rich editor.
  /// Allowed extensions (also must pass Host.AllowedExtensionWhitelist):
  /// pdf, doc, docx, xls, xlsx, ppt, pptx, txt, zip, csv
  /// </summary>
  public class MediaFileService
  {
    private static readonly HashSet<string> AllowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
      ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".zip", ".csv"
    };

    private static readonly Dictionary<string, string> ContentTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
      { ".pdf", "application/pdf" },
      { ".doc", "application/msword" },
      { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
      { ".xls", "application/vnd.ms-excel" },
      { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
      { ".ppt", "application/vnd.ms-powerpoint" },
      { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
      { ".txt", "text/plain" },
      { ".zip", "application/zip" },
      { ".csv", "text/csv" },
    };

    public static bool IsAllowedFileExtension(string extension)
    {
      if (string.IsNullOrEmpty(extension)) return false;
      if (!extension.StartsWith(".")) extension = "." + extension;
      return AllowedExtensions.Contains(extension)
        && Host.AllowedExtensionWhitelist.IsAllowedExtension(extension);
    }

    public static bool IsImageExtension(string extension)
    {
      if (string.IsNullOrEmpty(extension)) return false;
      if (!extension.StartsWith(".")) extension = "." + extension;
      var e = extension.ToLowerInvariant();
      return e == ".jpg" || e == ".jpeg" || e == ".png" || e == ".gif";
    }

    private static string HomeMapPath(int portalId)
    {
      var p = PortalController.Instance.GetPortal(portalId);
      if (p == null) throw new InvalidOperationException("Unknown portal " + portalId);
      return p.HomeDirectoryMapPath;
    }

    public static string TempDirectory(int portalId, int blogId)
    {
      return string.Format(@"{0}Blog\Files\{1}\_temp_files\", HomeMapPath(portalId), blogId);
    }

    public static string PostDirectory(int portalId, int blogId, int postId)
    {
      return string.Format(@"{0}Blog\Files\{1}\{2}\files\", HomeMapPath(portalId), blogId, postId);
    }

    public string GetPath(BlogFileInfo file)
    {
      var dir = file.PostId.HasValue && !file.IsTemp
        ? PostDirectory(file.PortalId, file.BlogId, file.PostId.Value)
        : TempDirectory(file.PortalId, file.BlogId);
      return Path.Combine(dir, file.StoredFileName);
    }

    private static IRepository<BlogFileInfo> Repo(IDataContext ctx)
    {
      return ctx.GetRepository<BlogFileInfo>();
    }

    public BlogFileInfo Get(Guid id)
    {
      using (var ctx = DataContext.Instance())
        return Repo(ctx).Find("WHERE FileId = @0", id).FirstOrDefault();
    }

    public BlogFileInfo AddFile(Stream source, string fileName, int portalId, int blogId, int? postId, int userId)
    {
      var ext = Path.GetExtension(fileName ?? "").ToLowerInvariant();
      if (!IsAllowedFileExtension(ext))
        throw new MediaRejectedException("This type of file is not allowed.");

      var ms = new MemoryStream();
      source.CopyTo(ms);
      if (ms.Length == 0) throw new MediaRejectedException("The file is empty.");
      ms.Position = 0;

      string hash;
      using (var sha = SHA256.Create())
        hash = BitConverter.ToString(sha.ComputeHash(ms)).Replace("-", "").ToLowerInvariant();
      ms.Position = 0;

      var state = postId.HasValue ? BlogFileState.Attached : BlogFileState.Temp;
      using (var ctx = DataContext.Instance())
      {
        var existing = postId.HasValue
          ? Repo(ctx).Find("WHERE BlogId = @0 AND PostId = @1 AND Hash = @2", blogId, postId.Value, hash).FirstOrDefault()
          : Repo(ctx).Find("WHERE BlogId = @0 AND PostId IS NULL AND Hash = @1 AND CreatedByUserId = @2", blogId, hash, userId).FirstOrDefault();
        if (existing != null && File.Exists(GetPath(existing))) return existing;

        var id = Guid.NewGuid();
        var original = Path.GetFileName(fileName);
        if (original.Length > 260) original = original.Substring(0, 260);
        string contentType;
        if (!ContentTypes.TryGetValue(ext, out contentType))
          contentType = "application/octet-stream";

        var file = new BlogFileInfo
        {
          FileId = id,
          PortalId = portalId,
          BlogId = blogId,
          PostId = postId,
          State = (int)state,
          OriginalFileName = original,
          StoredFileName = id.ToString("D") + ext,
          ContentType = contentType,
          Bytes = ms.Length,
          Hash = hash,
          CreatedByUserId = userId,
          CreatedOn = DateTime.UtcNow
        };

        var dir = postId.HasValue
          ? PostDirectory(portalId, blogId, postId.Value)
          : TempDirectory(portalId, blogId);
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, file.StoredFileName);
        using (var fs = File.Create(path))
          ms.CopyTo(fs);

        Repo(ctx).Insert(file);
        return file;
      }
    }
  }

  public static class FileUrls
  {
    public const string ServiceRoot = "~/DesktopModules/DotNetNuke/Blog/API/File/Get";

    public static string Build(Guid id, string version = null)
    {
      var url = DotNetNuke.Common.Globals.ResolveUrl(ServiceRoot) + "?id=" + id.ToString("D");
      if (!string.IsNullOrEmpty(version)) url += "&v=" + version;
      return url;
    }

    public static string Version(BlogFileInfo file)
    {
      return string.IsNullOrEmpty(file.Hash) ? null : file.Hash.Substring(0, Math.Min(8, file.Hash.Length));
    }
  }
}
