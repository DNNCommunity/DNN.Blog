using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace DotNetNuke.Modules.Blog.Core.Media
{
  /// <summary>A parsed &lt;img&gt; tag with attribute order preserved.</summary>
  public class HtmlImgTag
  {
    private static readonly Regex AttrRx = new Regex(
      "(?<name>[a-zA-Z_:][-a-zA-Z0-9_:.]*)(?:\\s*=\\s*(?:\"(?<v>[^\"]*)\"|'(?<v>[^']*)'|(?<v>[^\\s\"'>/]+)))?",
      RegexOptions.Compiled);

    private readonly List<KeyValuePair<string, string>> _attrs = new List<KeyValuePair<string, string>>();

    public static HtmlImgTag Parse(string tag)
    {
      var res = new HtmlImgTag();
      var inner = tag.Trim();
      inner = inner.Substring(4).TrimEnd('>').TrimEnd('/'); // strip "<img" and the closing
      foreach (Match m in AttrRx.Matches(inner))
      {
        res._attrs.Add(new KeyValuePair<string, string>(
          m.Groups["name"].Value.ToLowerInvariant(),
          WebUtility.HtmlDecode(m.Groups["v"].Success ? m.Groups["v"].Value : "")));
      }
      return res;
    }

    public string Get(string name)
    {
      name = name.ToLowerInvariant();
      foreach (var a in _attrs)
        if (a.Key == name) return a.Value;
      return null;
    }

    public void Set(string name, string value)
    {
      name = name.ToLowerInvariant();
      for (int i = 0; i < _attrs.Count; i++)
      {
        if (_attrs[i].Key == name)
        {
          _attrs[i] = new KeyValuePair<string, string>(name, value);
          return;
        }
      }
      _attrs.Add(new KeyValuePair<string, string>(name, value));
    }

    public void Remove(params string[] names)
    {
      var set = new HashSet<string>(names.Select(n => n.ToLowerInvariant()));
      _attrs.RemoveAll(a => set.Contains(a.Key));
    }

    public override string ToString()
    {
      var sb = new StringBuilder("<img");
      foreach (var a in _attrs)
        sb.Append(' ').Append(a.Key).Append("=\"").Append(WebUtility.HtmlEncode(a.Value)).Append('"');
      return sb.Append(" />").ToString();
    }
  }

  /// <summary>
  /// Conversions between the three forms an image takes in post HTML:
  /// editor/submitted form (blob or endpoint URL), stored form (data-blog-image placeholder),
  /// and rendered form (full img with srcset).
  /// </summary>
  public static class ImagePlaceholders
  {
    public const string PlaceholderAttribute = "data-blog-image";
    public const string PendingAttribute = "data-blog-pending";

    private static readonly Regex ImgRx = new Regex("<img\\b[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex UrlIdRx = new Regex(
      "/Image/Get\\?(?:[^#]*&)?id=(?<id>[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})",
      RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly string[] TransientAttributes = { "src", "srcset", "sizes", "loading", "decoding", "fetchpriority", PendingAttribute };

    public static Guid? GetGuidFromUrl(string url)
    {
      if (string.IsNullOrEmpty(url)) return null;
      var m = UrlIdRx.Match(WebUtility.HtmlDecode(url));
      return m.Success ? Guid.Parse(m.Groups["id"].Value) : (Guid?)null;
    }

    /// <summary>Ids of all images referenced by placeholder or by endpoint URL.</summary>
    public static IList<Guid> FindImageIds(string html)
    {
      var res = new List<Guid>();
      if (string.IsNullOrEmpty(html)) return res;
      foreach (Match m in ImgRx.Matches(html))
      {
        var id = GetImageId(HtmlImgTag.Parse(m.Value));
        if (id.HasValue && !res.Contains(id.Value)) res.Add(id.Value);
      }
      return res;
    }

    private static Guid? GetImageId(HtmlImgTag tag)
    {
      Guid g;
      var p = tag.Get(PlaceholderAttribute);
      if (p != null && Guid.TryParse(p, out g)) return g;
      return GetGuidFromUrl(tag.Get("src"));
    }

    /// <summary>
    /// Converts submitted HTML into the stored form. Pending (browser-local) images are resolved through
    /// <paramref name="resolvePending"/>; a null result drops the tag.
    /// </summary>
    public static string ToStorage(string html, Func<string, Guid?> resolvePending)
    {
      if (string.IsNullOrEmpty(html)) return html;
      return ImgRx.Replace(html, m =>
      {
        var tag = HtmlImgTag.Parse(m.Value);
        var pending = tag.Get(PendingAttribute);
        Guid? id;
        if (!string.IsNullOrEmpty(pending))
        {
          id = resolvePending == null ? null : resolvePending(pending);
          if (!id.HasValue) return "";
        }
        else
        {
          id = GetImageId(tag);
          if (!id.HasValue) return m.Value; // external image, leave alone
        }
        tag.Remove(TransientAttributes);
        tag.Set(PlaceholderAttribute, id.Value.ToString("D"));
        return tag.ToString();
      });
    }

    /// <summary>Adds a src to each placeholder so the editor can show it.</summary>
    public static string ToEditor(string html, Func<Guid, string> urlFor)
    {
      if (string.IsNullOrEmpty(html)) return html;
      return ImgRx.Replace(html, m =>
      {
        var tag = HtmlImgTag.Parse(m.Value);
        var p = tag.Get(PlaceholderAttribute);
        Guid id;
        if (p == null || !Guid.TryParse(p, out id)) return m.Value;
        tag.Set("src", urlFor(id));
        return tag.ToString();
      });
    }

    /// <summary>Expands placeholders into the final markup. Unknown images are removed.</summary>
    public static string Render(string html, Func<Guid, int, HtmlImgTag, string> renderOne)
    {
      if (string.IsNullOrEmpty(html) || html.IndexOf(PlaceholderAttribute, StringComparison.OrdinalIgnoreCase) < 0)
        return html;
      int index = 0;
      return ImgRx.Replace(html, m =>
      {
        var tag = HtmlImgTag.Parse(m.Value);
        var p = tag.Get(PlaceholderAttribute);
        Guid id;
        if (p == null || !Guid.TryParse(p, out id)) return m.Value;
        return renderOne(id, index++, tag) ?? "";
      });
    }
  }
}
