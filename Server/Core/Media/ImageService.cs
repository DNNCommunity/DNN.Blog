using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using DotNetNuke.Data;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Portals;

namespace DotNetNuke.Modules.Blog.Core.Media
{
  public class ImageRejectedException : Exception
  {
    public ImageRejectedException(string message) : base(message) { }
  }

  public class PendingImageFile
  {
    public string FileName { get; set; }
    public Func<Stream> Open { get; set; }
  }

  /// <summary>
  /// Single entry point for storing and retrieving post images. Web UI, WLW, BlogML import and future APIs all go through here.
  /// </summary>
  public class ImageService
  {
    private readonly IImageResizer _resizer;

    public ImageService() : this(new SystemDrawingImageResizer()) { }

    public ImageService(IImageResizer resizer)
    {
      _resizer = resizer;
    }

    #region Paths
    private static string HomeMapPath(int portalId)
    {
      var p = PortalController.Instance.GetPortal(portalId);
      if (p == null) throw new InvalidOperationException("Unknown portal " + portalId);
      return p.HomeDirectoryMapPath;
    }

    public static string TempDirectory(int portalId, int blogId)
    {
      return string.Format(@"{0}Blog\Files\{1}\_temp_images\", HomeMapPath(portalId), blogId);
    }

    public static string PostDirectory(int portalId, int blogId, int postId)
    {
      return string.Format(@"{0}Blog\Files\{1}\{2}\", HomeMapPath(portalId), blogId, postId);
    }

    private static string CacheDirectory(int portalId, Guid id)
    {
      return string.Format(@"{0}Blog\Cache\{1:D}\", HomeMapPath(portalId), id);
    }

    public string GetOriginalPath(BlogImageInfo img)
    {
      var dir = img.PostId.HasValue && !img.IsTemp
        ? PostDirectory(img.PortalId, img.BlogId, img.PostId.Value)
        : TempDirectory(img.PortalId, img.BlogId);
      return Path.Combine(dir, img.StoredFileName);
    }
    #endregion

    #region Data
    private static IRepository<BlogImageInfo> Repo(IDataContext ctx)
    {
      return ctx.GetRepository<BlogImageInfo>();
    }

    public BlogImageInfo Get(Guid id)
    {
      using (var ctx = DataContext.Instance())
        return Repo(ctx).Find("WHERE ImageId = @0", id).FirstOrDefault();
    }

    public IList<BlogImageInfo> Get(IEnumerable<Guid> ids)
    {
      var list = ids.Distinct().ToList();
      var res = new List<BlogImageInfo>();
      using (var ctx = DataContext.Instance())
      {
        // chunked to stay well below the SQL parameter limit
        for (int i = 0; i < list.Count; i += 100)
        {
          var chunk = list.Skip(i).Take(100).ToList();
          var args = string.Join(",", chunk.Select((_, n) => "@" + n));
          res.AddRange(Repo(ctx).Find("WHERE ImageId IN (" + args + ")", chunk.Cast<object>().ToArray()));
        }
      }
      return res;
    }

    public IList<BlogImageInfo> GetForPost(int blogId, int postId)
    {
      using (var ctx = DataContext.Instance())
        return Repo(ctx).Find("WHERE BlogId = @0 AND PostId = @1", blogId, postId).ToList();
    }

    public void UpdateMetadata(Guid id, string alt, string title, string caption)
    {
      using (var ctx = DataContext.Instance())
      {
        var img = Repo(ctx).Find("WHERE ImageId = @0", id).FirstOrDefault();
        if (img == null) return;
        img.Alt = alt;
        img.Title = title;
        img.Caption = caption;
        Repo(ctx).Update(img);
      }
    }
    #endregion

    #region Ingestion
    /// <summary>
    /// Validates and stores an image. With no postId the image is held as temporary until a post claims it.
    /// Identical content already held for the same post (or temp area of the blog) is reused.
    /// </summary>
    public BlogImageInfo AddImage(Stream source, string fileName, int portalId, int blogId, int? postId, int userId)
    {
      var ext = Path.GetExtension(fileName ?? "").ToLowerInvariant();
      if (string.IsNullOrEmpty(ext) || !Host.AllowedExtensionWhitelist.IsAllowedExtension(ext))
        throw new ImageRejectedException("This type of file is not allowed.");

      var ms = new MemoryStream();
      source.CopyTo(ms);
      if (ms.Length == 0) throw new ImageRejectedException("The file is empty.");
      ms.Position = 0;

      var info = _resizer.Inspect(ms);
      if (info == null) throw new ImageRejectedException("The file is not a valid jpg, png or gif image.");

      string hash;
      ms.Position = 0;
      using (var sha = SHA256.Create())
        hash = BitConverter.ToString(sha.ComputeHash(ms)).Replace("-", "").ToLowerInvariant();

      var state = postId.HasValue ? BlogImageState.Attached : BlogImageState.Temp;
      using (var ctx = DataContext.Instance())
      {
        var existing = postId.HasValue
          ? Repo(ctx).Find("WHERE BlogId = @0 AND PostId = @1 AND Hash = @2", blogId, postId.Value, hash).FirstOrDefault()
          : Repo(ctx).Find("WHERE BlogId = @0 AND PostId IS NULL AND Hash = @1 AND CreatedByUserId = @2", blogId, hash, userId).FirstOrDefault();
        if (existing != null && File.Exists(GetOriginalPath(existing))) return existing;

        var id = Guid.NewGuid();
        var img = new BlogImageInfo
        {
          ImageId = id,
          PortalId = portalId,
          BlogId = blogId,
          PostId = postId,
          State = (int)state,
          OriginalFileName = Path.GetFileName(fileName).Length > 260 ? Path.GetFileName(fileName).Substring(0, 260) : Path.GetFileName(fileName),
          StoredFileName = id.ToString("D") + info.Extension,
          ContentType = info.ContentType,
          Width = info.Width,
          Height = info.Height,
          Bytes = ms.Length,
          Hash = hash,
          CreatedByUserId = userId,
          CreatedOn = DateTime.UtcNow
        };

        var path = GetOriginalPath(img);
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllBytes(path, ms.ToArray());
        try
        {
          Repo(ctx).Insert(img);
        }
        catch
        {
          TryDelete(path);
          throw;
        }
        return img;
      }
    }

    /// <summary>
    /// Turns submitted post HTML into stored form: browser-local images are saved, endpoint URLs become placeholders.
    /// Pending images are matched on the file name (without extension) of the uploaded files.
    /// </summary>
    public string ProcessSubmittedHtml(string html, int portalId, int blogId, int? postId, int userId, IDictionary<string, PendingImageFile> pending)
    {
      var created = new Dictionary<string, Guid?>(StringComparer.OrdinalIgnoreCase);
      return ImagePlaceholders.ToStorage(html, key =>
      {
        Guid? cached;
        if (created.TryGetValue(key, out cached)) return cached;
        PendingImageFile f;
        Guid? id = null;
        if (pending != null && pending.TryGetValue(key, out f))
        {
          try
          {
            using (var s = f.Open())
              id = AddImage(s, f.FileName, portalId, blogId, postId, userId).ImageId;
          }
          catch (ImageRejectedException)
          {
            id = null;
          }
        }
        created[key] = id;
        return id;
      });
    }

    public static IDictionary<string, PendingImageFile> IndexPending(IEnumerable<PendingImageFile> files)
    {
      var res = new Dictionary<string, PendingImageFile>(StringComparer.OrdinalIgnoreCase);
      foreach (var f in files)
        res[Path.GetFileNameWithoutExtension(f.FileName)] = f;
      return res;
    }
    #endregion

    #region Lifecycle
    /// <summary>
    /// After a post is saved: claims the temporary images it references (same blog, same uploader unless admin)
    /// and removes images that were attached to the post but are no longer referenced.
    /// </summary>
    public void SyncPostImages(int blogId, int postId, int userId, bool isAdmin, params string[] htmlParts)
    {
      var referenced = new HashSet<Guid>();
      foreach (var h in htmlParts)
        foreach (var id in ImagePlaceholders.FindImageIds(h)) referenced.Add(id);

      using (var ctx = DataContext.Instance())
      {
        var repo = Repo(ctx);
        foreach (var img in referenced.Count == 0 ? new List<BlogImageInfo>() : Get(referenced))
        {
          if (!img.IsTemp || img.BlogId != blogId) continue;
          if (img.CreatedByUserId != userId && !isAdmin) continue;
          var from = GetOriginalPath(img);
          img.PostId = postId;
          img.State = (int)BlogImageState.Attached;
          var to = GetOriginalPath(img);
          Directory.CreateDirectory(Path.GetDirectoryName(to));
          if (File.Exists(from) && !File.Exists(to)) File.Move(from, to);
          repo.Update(img);
        }

        foreach (var img in repo.Find("WHERE BlogId = @0 AND PostId = @1", blogId, postId).ToList())
        {
          if (!referenced.Contains(img.ImageId)) DeleteInternal(repo, img);
        }
      }
    }

    public void DeletePostImages(int blogId, int postId)
    {
      using (var ctx = DataContext.Instance())
      {
        var repo = Repo(ctx);
        foreach (var img in repo.Find("WHERE BlogId = @0 AND PostId = @1", blogId, postId).ToList())
          DeleteInternal(repo, img);
      }
    }

    /// <summary>Removes temporary images nobody claimed. Intended for a scheduled job.</summary>
    public int PurgeTemp(TimeSpan olderThan)
    {
      var cutoff = DateTime.UtcNow - olderThan;
      int n = 0;
      using (var ctx = DataContext.Instance())
      {
        var repo = Repo(ctx);
        foreach (var img in repo.Find("WHERE State = @0 AND CreatedOn < @1", (int)BlogImageState.Temp, cutoff).ToList())
        {
          DeleteInternal(repo, img);
          n++;
        }
      }
      return n;
    }

    private void DeleteInternal(IRepository<BlogImageInfo> repo, BlogImageInfo img)
    {
      TryDelete(GetOriginalPath(img));
      try { Directory.Delete(CacheDirectory(img.PortalId, img.ImageId), true); } catch { /* cache is disposable */ }
      repo.Delete(img);
    }

    private static void TryDelete(string path)
    {
      try { File.Delete(path); } catch { /* not critical */ }
    }
    #endregion

    #region Serving
    /// <summary>Returns the path of the file to serve for the requested variant, generating it on first use.</summary>
    public string GetVariantPath(BlogImageInfo img, int width, int height, bool crop)
    {
      var original = GetOriginalPath(img);
      if (!File.Exists(original)) return null;

      int w = ImageSizes.SnapWidth(width);
      int h = 0;
      if (crop)
      {
        var c = ImageSizes.SnapCrop(width, height);
        if (c != null) { w = c[0]; h = c[1]; } else crop = false;
      }
      if (w == 0) return original;
      if (!crop && w >= img.Width) return original;

      var ext = Path.GetExtension(img.StoredFileName);
      var variant = Path.Combine(CacheDirectory(img.PortalId, img.ImageId), string.Format("{0}x{1}{2}{3}", w, h, crop ? "c" : "", ext));
      if (File.Exists(variant)) return variant;

      var temp = variant + "." + Guid.NewGuid().ToString("N") + ".tmp";
      Directory.CreateDirectory(Path.GetDirectoryName(variant));
      _resizer.Resize(original, temp, w, h, crop);
      try
      {
        File.Move(temp, variant);
      }
      catch (IOException)
      {
        TryDelete(temp); // another request produced it first
      }
      return variant;
    }
    #endregion

    #region Legacy content
    private static readonly Regex LegacySrcRx = new Regex("<img\\b[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Converts &lt;img&gt; tags that point at files in the post's folder (the pre-7.0 format) into placeholders,
    /// registering those files as images. Returns the new HTML; unchanged when nothing matched.
    /// </summary>
    public string ImportLegacyImages(string html, int portalId, int blogId, int postId, int userId)
    {
      if (string.IsNullOrEmpty(html)) return html;
      var dir = PostDirectory(portalId, blogId, postId);
      var marker = string.Format("Blog/Files/{0}/{1}/", blogId, postId);
      return LegacySrcRx.Replace(html, m =>
      {
        var tag = HtmlImgTag.Parse(m.Value);
        var src = tag.Get("src");
        if (src == null) return m.Value;
        var idx = src.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (idx < 0) return m.Value;
        var name = Path.GetFileName(Uri.UnescapeDataString(src.Substring(idx + marker.Length).Split('?')[0]));
        if (string.IsNullOrEmpty(name)) return m.Value;
        var path = Path.Combine(dir, name);
        if (!File.Exists(path)) return m.Value;
        try
        {
          using (var fs = File.OpenRead(path))
          {
            var img = AddImage(fs, name, portalId, blogId, postId, userId);
            tag.Remove("src", "srcset", "sizes");
            tag.Set(ImagePlaceholders.PlaceholderAttribute, img.ImageId.ToString("D"));
            return tag.ToString();
          }
        }
        catch (ImageRejectedException)
        {
          return m.Value;
        }
      });
    }
    /// <summary>
    /// BlogML import: img tags whose src ends in one of the supplied file names become stored placeholders.
    /// The images are held as temporary until SyncPostImages claims them after the post is saved.
    /// </summary>
    public string ImportEmbeddedImages(string html, IDictionary<string, byte[]> files, int portalId, int blogId, int userId)
    {
      if (string.IsNullOrEmpty(html) || files == null || files.Count == 0) return html;
      var byName = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
      foreach (var kv in files) byName[Path.GetFileName(kv.Key)] = kv.Value;
      return LegacySrcRx.Replace(html, m =>
      {
        var tag = HtmlImgTag.Parse(m.Value);
        var src = tag.Get("src");
        if (string.IsNullOrEmpty(src)) return m.Value;
        byte[] data;
        var name = Path.GetFileName(Uri.UnescapeDataString(src.Split('?')[0]));
        if (string.IsNullOrEmpty(name) || !byName.TryGetValue(name, out data) || data == null) return m.Value;
        try
        {
          var img = AddImage(new MemoryStream(data), name, portalId, blogId, null, userId);
          tag.Remove("src", "srcset", "sizes");
          tag.Set(ImagePlaceholders.PlaceholderAttribute, img.ImageId.ToString("D"));
          return tag.ToString();
        }
        catch (ImageRejectedException)
        {
          return m.Value;
        }
      });
    }

    /// <summary>BlogML export: swaps placeholders for the stored file name and collects the image bytes as attachments.</summary>
    public string ExportEmbeddedImages(string html, IDictionary<string, byte[]> attachments)
    {
      if (string.IsNullOrEmpty(html)) return html;
      var map = Get(ImagePlaceholders.FindImageIds(html)).ToDictionary(i => i.ImageId);
      return ImagePlaceholders.Render(html, (id, index, tag) =>
      {
        BlogImageInfo img;
        if (!map.TryGetValue(id, out img)) return "";
        var path = GetOriginalPath(img);
        if (!File.Exists(path)) return "";
        if (!attachments.ContainsKey(img.StoredFileName)) attachments[img.StoredFileName] = File.ReadAllBytes(path);
        tag.Remove(ImagePlaceholders.PlaceholderAttribute);
        tag.Set("src", img.StoredFileName);
        return tag.ToString();
      });
    }
    #endregion
  }
}