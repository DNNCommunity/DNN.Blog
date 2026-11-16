using System;
using DotNetNuke.ComponentModel.DataAnnotations;

namespace DotNetNuke.Modules.Blog.Core.Media
{
  public enum BlogImageState
  {
    /// <summary>Uploaded but not (yet) part of a saved post. Purged when unclaimed.</summary>
    Temp = 0,
    Attached = 1
  }

  [TableName("Blog_Images")]
  [PrimaryKey("ImageId", AutoIncrement = false)]
  public class BlogImageInfo
  {
    public Guid ImageId { get; set; }
    public int PortalId { get; set; }
    public int BlogId { get; set; }
    public int? PostId { get; set; }
    public int State { get; set; }
    public string OriginalFileName { get; set; }
    public string StoredFileName { get; set; }
    public string ContentType { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public long Bytes { get; set; }
    public string Alt { get; set; }
    public string Title { get; set; }
    public string Caption { get; set; }
    public string Hash { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime CreatedOn { get; set; }

    [IgnoreColumn]
    public bool IsTemp { get { return State == (int)BlogImageState.Temp; } }
  }
}
