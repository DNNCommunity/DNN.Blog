using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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
    /// <summary>
    /// Pre-7 editors wrapped images in &lt;a href="guid.jpg"&gt; for Colorbox. After migration the
    /// &lt;img&gt; uses /Image/Get but the relative href remains and the lightbox 404s. Rewrite those
    /// anchors to the full-size image endpoint.
    /// </summary>
    private static readonly Regex LegacyLightboxAnchorRx = new Regex(
      @"<a\b(?<before>[^>]*?\bhref\s*=\s*)(?<q>[""'])(?<href>[^""']+)\k<q>(?<after>[^>]*)>(?<inner>.*?)</a>",
      RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex ImageFileHrefRx = new Regex(
      @"\.(?:gif|jpe?g|png|bmp|webp)(?:\?.*)?$",
      RegexOptions.IgnoreCase | RegexOptions.Compiled);

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
      var rendered = ImagePlaceholders.Render(html, (id, index, tag) =>
      {
        BlogImageInfo img;
        if (!map.TryGetValue(id, out img)) return ""; // image gone; do not emit a broken tag
        return RenderTag(img, index, tag);
      });
      return RewriteLegacyLightboxAnchors(rendered, map);
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

    /// <summary>
    /// Points leftover lightbox anchors at the full-size Image API URL so Colorbox can load them.
    /// </summary>
    internal static string RewriteLegacyLightboxAnchors(string html, IDictionary<Guid, BlogImageInfo> map = null)
    {
      if (string.IsNullOrEmpty(html) || html.IndexOf("<a", StringComparison.OrdinalIgnoreCase) < 0)
        return html;

      return LegacyLightboxAnchorRx.Replace(html, m =>
      {
        var href = WebUtility.HtmlDecode(m.Groups["href"].Value);
        if (!ImageFileHrefRx.IsMatch(href)) return m.Value;

        var isBareFile = href.IndexOf('/') < 0 && href.IndexOf('\\') < 0
          && href.IndexOf("://", StringComparison.Ordinal) < 0;
        var isBlogFiles = href.IndexOf("Blog/Files/", StringComparison.OrdinalIgnoreCase) >= 0;
        if (!isBareFile && !isBlogFiles) return m.Value;

        var id = ImagePlaceholders.GetGuidFromUrl(m.Groups["inner"].Value);
        if (!id.HasValue) return m.Value;

        string version = null;
        BlogImageInfo img;
        if (map != null && map.TryGetValue(id.Value, out img))
          version = ImageUrls.Version(img);

        var q = m.Groups["q"].Value;
        var full = ImageUrls.Build(id.Value, 0, version);
        return "<a" + m.Groups["before"].Value + q + full + q + m.Groups["after"].Value + ">"
          + m.Groups["inner"].Value + "</a>";
      });
    }
  }
}
