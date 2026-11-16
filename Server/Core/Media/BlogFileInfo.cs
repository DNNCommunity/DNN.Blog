using System;
using DotNetNuke.ComponentModel.DataAnnotations;

namespace DotNetNuke.Modules.Blog.Core.Media
{
  public enum BlogFileState
  {
    Temp = 0,
    Attached = 1
  }

  [TableName("Blog_Files")]
  [PrimaryKey("FileId", AutoIncrement = false)]
  public class BlogFileInfo
  {
    public Guid FileId { get; set; }
    public int PortalId { get; set; }
    public int BlogId { get; set; }
    public int? PostId { get; set; }
    public int State { get; set; }
    public string OriginalFileName { get; set; }
    public string StoredFileName { get; set; }
    public string ContentType { get; set; }
    public long Bytes { get; set; }
    public string Hash { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime CreatedOn { get; set; }

    [IgnoreColumn]
    public bool IsTemp { get { return State == (int)BlogFileState.Temp; } }
  }
}
