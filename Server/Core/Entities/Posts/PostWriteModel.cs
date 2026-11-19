using System.Collections.Generic;
using System.IO;

namespace DotNetNuke.Modules.Blog.Core.Entities.Posts
{
  public class PostWriteModel
  {
    public int BlogId { get; set; } = -1;
    public int PostId { get; set; } = -1;
    public string Title { get; set; } = "";
    public string Summary { get; set; } = "";
    public string Content { get; set; } = "";
    public Dictionary<string, string> Titles { get; set; } = new Dictionary<string, string>();
    public Dictionary<string, string> Summaries { get; set; } = new Dictionary<string, string>();
    public Dictionary<string, string> Contents { get; set; } = new Dictionary<string, string>();
    public bool Published { get; set; } = true;
    public bool PublishNow { get; set; }
    public string PublishDate { get; set; } = "";
    public bool AllowComments { get; set; } = true;
    public bool DisplayCopyright { get; set; }
    public string Copyright { get; set; } = "";
    public string Locale { get; set; } = "";
    public string Tags { get; set; } = "";
    public List<int> SelectedCategoryIds { get; set; } = new List<int>();
    public string ImageKey { get; set; } = "";
    public bool RemoveImage { get; set; }

    /// <summary>Optional featured image file (MVC upload). BlogApi typically leaves this null.</summary>
    public Stream FeaturedImageStream { get; set; }
    public string FeaturedImageFileName { get; set; }

    /// <summary>HTML (0) or Markdown (1). Applied only when creating a post.</summary>
    public byte? ContentFormat { get; set; }
  }
}
