using System;
using System.Collections;
using System.Xml.Serialization;

namespace DotNetNuke.Modules.Blog.Core.BlogML.Xml
{
  [Serializable]
  public sealed class BlogMLPost : BlogMLNode
  {

    [XmlAttribute("post-url")]
    public string PostUrl { get; set; }

    [XmlAttribute("hasexcerpt")]
    public bool HasExcerpt { get; set; } = false;

    [XmlAttribute("type")]
    public BlogPostTypes PostType { get; set; } = new BlogPostTypes();

    [XmlAttribute("views")]
    public uint Views { get; set; } = 0U;

    [XmlAttribute("image", Namespace = "http://dnn-connect.org/blog/")]
    public string Image { get; set; }

    [XmlAttribute("allow-comments", Namespace = "http://dnn-connect.org/blog/")]
    public bool AllowComments { get; set; } = true;

    [XmlAttribute("display-copyright", Namespace = "http://dnn-connect.org/blog/")]
    public bool DisplayCopyright { get; set; } = false;

    [XmlAttribute("copyright", Namespace = "http://dnn-connect.org/blog/")]
    public string Copyright { get; set; }

    [XmlAttribute("locale", Namespace = "http://dnn-connect.org/blog/")]
    public string Locale { get; set; }

    [XmlElement("post-name")]
    public string PostName { get; set; }

    [XmlElement("content")]
    public BlogMLContent Content { get; set; } = new BlogMLContent();

    [XmlElement("excerpt")]
    public BlogMLContent Excerpt { get; set; } = new BlogMLContent();

    [XmlArray("authors")]
    [XmlArrayItem("author", typeof(BlogMLAuthorReference))]
    public AuthorReferenceCollection Authors { get; set; } = new AuthorReferenceCollection();

    [XmlArray("categories")]
    [XmlArrayItem("category", typeof(BlogMLCategoryReference))]
    public CategoryReferenceCollection Categories { get; set; } = new CategoryReferenceCollection();

    [XmlArray("comments")]
    [XmlArrayItem("comment", typeof(BlogMLComment))]
    public CommentCollection Comments { get; set; } = new CommentCollection();

    [XmlArray("trackbacks")]
    [XmlArrayItem("trackback", typeof(BlogMLTrackback))]
    public TrackbackCollection Trackbacks { get; set; } = new TrackbackCollection();

    [XmlArray("attachments")]
    [XmlArrayItem("attachment", typeof(BlogMLAttachment))]
    public AttachmentCollection Attachments { get; set; } = new AttachmentCollection();

    [Serializable]
    public sealed class AuthorReferenceCollection : ArrayList
    {
      public new BlogMLAuthorReference this[int index]
      {
        get
        {
          return base[index] as BlogMLAuthorReference;
        }
      }

      public void Add(BlogMLAuthorReference value)
      {
        base.Add(value);
      }

      public BlogMLAuthorReference Add(string authorID)
      {
        var item = new BlogMLAuthorReference();
        item.Ref = authorID;
        base.Add(item);
        return item;
      }
    }

    [Serializable]
    public sealed class CommentCollection : ArrayList
    {
      public new BlogMLComment this[int index]
      {
        get
        {
          return base[index] as BlogMLComment;
        }
      }

      public void Add(BlogMLComment value)
      {
        base.Add(value);
      }
    }

    [Serializable]
    public sealed class TrackbackCollection : ArrayList
    {
      public new BlogMLTrackback this[int index]
      {
        get
        {
          return base[index] as BlogMLTrackback;
        }
      }

      public void Add(BlogMLTrackback value)
      {
        base.Add(value);
      }
    }

    [Serializable]
    public sealed class CategoryReferenceCollection : ArrayList
    {
      public new BlogMLCategoryReference this[int index]
      {
        get
        {
          return base[index] as BlogMLCategoryReference;
        }
      }

      public void Add(BlogMLCategoryReference value)
      {
        base.Add(value);
      }

      public BlogMLCategoryReference Add(string categoryID)
      {
        var item = new BlogMLCategoryReference();
        item.Ref = categoryID;
        base.Add(item);
        return item;
      }
    }

    [Serializable]
    public sealed class AttachmentCollection : ArrayList
    {
      public new BlogMLAttachment this[int index]
      {
        get
        {
          return base[index] as BlogMLAttachment;
        }
      }

      public void Add(BlogMLAttachment value)
      {
        base.Add(value);
      }
    }
  }
}