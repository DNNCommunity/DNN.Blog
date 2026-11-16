using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using DotNetNuke.Common;

namespace DotNetNuke.Modules.Blog.Core.Media
{
  public static class ImageUrls
  {
    /// <summary>Single place that knows where the image endpoint lives.</summary>
    public const string ServiceRoot = "~/DesktopModules/DotNetNuke/Blog/API/Image/Get";

    public static string Build(Guid id, int width = 0, string version = null)
    {
      var url = Globals.ResolveUrl(ServiceRoot) + "?id=" + id.ToString("D");
      if (width > 0) url += "&w=" + width;
      if (!string.IsNullOrEmpty(version)) url += "&v=" + version;
      return url;
    }

    public static string Version(BlogImageInfo img)
    {
      return string.IsNullOrEmpty(img.Hash) ? null : img.Hash.Substring(0, Math.Min(8, img.Hash.Length));
    }
  }

  /// <summary>Expands stored placeholders into the final img markup (no picture element yet).</summary>
  public static class ImageRenderer
  {
    public static string Render(string html, ImageService service = null)
    {
      if (string.IsNullOrEmpty(html) || html.IndexOf(ImagePlaceholders.PlaceholderAttribute, StringComparison.OrdinalIgnoreCase) < 0)
        return html;
      service = service ?? new ImageService();
      var ids = ImagePlaceholders.FindImageIds(html);
      var map = service.Get(ids).ToDictionary(i => i.ImageId);
      return Render(html, map);
    }

    public static string Render(string html, IDictionary<Guid, BlogImageInfo> map)
    {
      return ImagePlaceholders.Render(html, (id, index, tag) =>
      {
        BlogImageInfo img;
        if (!map.TryGetValue(id, out img)) return ""; // image gone; do not emit a broken tag
        return RenderTag(img, index, tag);
      });
    }

    public static string RenderTag(BlogImageInfo img, int index, HtmlImgTag tag)
    {
      var v = ImageUrls.Version(img);
      var widths = ImageSizes.WidthsFor(img.Width).ToList();
      if (!widths.Contains(img.Width)) widths.Add(img.Width);
      var defaultW = Math.Min(ImageSizes.DefaultWidth, img.Width);
      tag.Remove(ImagePlaceholders.PlaceholderAttribute);
      tag.Set("src", ImageUrls.Build(img.ImageId, defaultW == img.Width ? 0 : defaultW, v));
      if (widths.Count > 1)
      {
        tag.Set("srcset", string.Join(", ", widths.Select(w => ImageUrls.Build(img.ImageId, w == img.Width ? 0 : w, v) + " " + w + "w")));
        tag.Set("sizes", "(max-width: " + defaultW + "px) 100vw, " + defaultW + "px");
      }
      tag.Set("width", img.Width.ToString());
      tag.Set("height", img.Height.ToString());
      if (tag.Get("alt") == null) tag.Set("alt", img.Alt ?? "");
      if (tag.Get("title") == null && !string.IsNullOrEmpty(img.Title)) tag.Set("title", img.Title);
      if (index == 0)
      {
        tag.Set("fetchpriority", "high");
      }
      else
      {
        tag.Set("loading", "lazy");
      }
      tag.Set("decoding", "async");
      var style = tag.Get("style") ?? "";
      if (style.IndexOf("height", StringComparison.OrdinalIgnoreCase) < 0)
        tag.Set("style", (style.Length > 0 && !style.TrimEnd().EndsWith(";") ? style + ";" : style) + "max-width:100%;height:auto");
      return tag.ToString();
    }
  }
}
