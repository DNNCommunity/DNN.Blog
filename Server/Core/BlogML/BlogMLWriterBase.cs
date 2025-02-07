using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using DotNetNuke.Modules.Blog.Core.BlogML.Xml;

namespace DotNetNuke.Modules.Blog.Core.BlogML
{

  public abstract class BlogMLWriterBase
  {
    private const string BlogMLNamespace = "http://www.blogml.com/2006/09/BlogML";

    protected XmlWriter Writer
    {
      get
      {
        return m_Writer;
      }
      private set
      {
        m_Writer = value;
      }
    }
    private XmlWriter m_Writer;

    public void Write(XmlWriter writer__1)
    {
      Writer = writer__1;

      // Write the XML delcaration. 
      writer__1.WriteStartDocument();

      try
      {
        InternalWriteBlog();
      }
      finally
      {
        Writer = null;
      }
    }

    protected abstract void InternalWriteBlog();

    #region WriteStartBlog


    protected void WriteStartBlog(string title, string subTitle, string rootUrl)
    {
      WriteStartBlog(title, ContentTypes.Text, subTitle, ContentTypes.Text, rootUrl, DateTime.Now);
    }

    protected void WriteStartBlog(string title, string subTitle, string rootUrl, DateTime dateCreated)
    {
      WriteStartBlog(title, ContentTypes.Text, subTitle, ContentTypes.Text, rootUrl, dateCreated);
    }

    protected void WriteStartBlog(string title, ContentTypes titleContentType, string subTitle, ContentTypes subTitleContentType, string rootUrl, DateTime dateCreated)
    {
      // WriteStartElement("blog");
      Writer.WriteStartElement("blog", BlogMLNamespace);
      // fixes bug in Work Item 2004
      WriteAttributeStringRequired("root-url", rootUrl);
      WriteAttributeString("date-created", FormatDateTime(dateCreated));

      // Write the default namespace, identified as xmlns with no prefix
      Writer.WriteAttributeString("xmlns", null, null, "http://www.blogml.com/2006/09/BlogML");
      Writer.WriteAttributeString("xmlns", "xs", null, "http://www.w3.org/2001/XMLSchema");

      WriteContent("title", BlogMLContent.Create(title, titleContentType));
      WriteContent("sub-title", BlogMLContent.Create(subTitle, subTitleContentType));
    }

    #endregion

    protected void WriteStartElement(string tag)
    {
      Writer.WriteStartElement(tag);
    }


    protected void WriteEndElement()
    {
      Writer.WriteEndElement();
    }


    #region Extended Properties

    protected void WriteStartExtendedProperties()
    {
      WriteStartElement("extended-properties");
    }

    protected void WriteExtendedProperty(string name, string value)
    {
      WriteStartElement("property");
      WriteAttributeString("name", name);
      WriteAttributeString("value", value);
      WriteEndElement();
    }

    #endregion


    protected void WriteAuthor(string id, string name, string email, DateTime dateCreated, DateTime dateModified, bool approved)
    {
      WriteStartElement("author");
      WriteNodeAttributes(id, dateCreated, dateModified, approved);
      WriteAttributeString("email", email);
      WriteContent("title", BlogMLContent.Create(name, ContentTypes.Text));
      WriteEndElement();
    }


    protected void WriteCategory(string id, string title, DateTime dateCreated, DateTime dateModified, bool approved, string description, string parentRef)
    {
      WriteCategory(id, title, ContentTypes.Text, dateCreated, dateModified, approved, description, parentRef);
    }


    protected void WriteCategory(string id, string title, ContentTypes titleContentType, DateTime dateCreated, DateTime dateModified, bool approved, string description, string parentRef)
    {
      WriteStartElement("category");
      WriteNodeAttributes(id, dateCreated, dateModified, approved);
      WriteAttributeString("description", description);
      WriteAttributeString("parentref", parentRef);
      WriteContent("title", BlogMLContent.Create(title, titleContentType));
      WriteEndElement();
    }

    protected void WriteAuthorReference(string refId)
    {
      WriteStartElement("author");
      WriteAttributeStringRequired("ref", refId);
      WriteEndElement();
    }

    protected void WriteCategoryReference(string refId)
    {
      WriteStartElement("category");
      WriteAttributeStringRequired("ref", refId);
      WriteEndElement();
    }

    protected void WriteStartAuthors()
    {
      WriteStartElement("authors");
    }

    protected void WriteStartCategories()
    {
      WriteStartElement("categories");
    }


    protected void WriteStartPosts()
    {
      WriteStartElement("posts");
    }


    protected void WriteStartTrackbacks()
    {
      WriteStartElement("trackbacks");
    }


    protected void WriteStartAttachments()
    {
      WriteStartElement("attachments");
    }


    protected void WriteNodeAttributes(string id, DateTime dateCreated, DateTime dateModified, bool approved)
    {
      WriteAttributeString("id", id);
      WriteAttributeString("date-created", FormatDateTime(dateCreated));
      WriteAttributeString("date-modified", FormatDateTime(dateModified));
      WriteAttributeString("approved", approved ? "true" : "false");
    }


    protected string FormatDateTime(DateTime date)
    {
      return date.ToUniversalTime().ToString("s");
    }

    protected void WriteStartPost(string id, string title, DateTime dateCreated, DateTime dateModified, bool approved, string content, string postUrl)
    {
      WriteStartPost(id, title, ContentTypes.Text, dateCreated, dateModified, approved, content, ContentTypes.Text, postUrl, 0U, false, null, ContentTypes.Text, BlogPostTypes.Normal, string.Empty);

    }

    protected void WriteStartPost(string id, string title, DateTime dateCreated, DateTime dateModified, bool approved, string content, string postUrl, uint views, BlogPostTypes blogpostType, string postName)
    {
      WriteStartPost(id, title, ContentTypes.Text, dateCreated, dateModified, approved, content, ContentTypes.Text, postUrl, views, false, null, ContentTypes.Text, blogpostType, postName);

    }

    protected void WriteStartPost(string id, string title, DateTime dateCreated, DateTime dateModified, bool approved, string content, string postUrl, uint views, string excerpt, BlogPostTypes blogpostType, string postName)
    {
      WriteStartPost(id, title, ContentTypes.Text, dateCreated, dateModified, approved, content, ContentTypes.Text, postUrl, views, true, excerpt, ContentTypes.Text, blogpostType, postName);

    }

    protected void WriteStartPost(string id, BlogMLContent title, DateTime dateCreated, DateTime dateModified, bool approved, BlogMLContent content, string postUrl, uint views, bool hasexcerpt, BlogMLContent excerpt, BlogPostTypes blogpostType, string postName)
    {
      WriteStartElement("post");
      WriteNodeAttributes(id, dateCreated, dateModified, approved);
      WriteAttributeString("post-url", postUrl);
      WriteAttributeStringRequired("type", blogpostType.ToString().ToLower());
      WriteAttributeStringRequired("hasexcerpt", hasexcerpt.ToString().ToLower());
      WriteAttributeStringRequired("views", views.ToString());
      WriteContent("title", title);
      WriteContent("content", content);
      if (postName != null)
      {
        WriteContent("post-name", BlogMLContent.Create(postName, ContentTypes.Text));
      }
      if (hasexcerpt)
      {
        WriteContent("excerpt", excerpt);
      }
    }

    protected void WriteStartPost(string id, string title, ContentTypes titleContentType, DateTime dateCreated, DateTime dateModified, bool approved, string content, ContentTypes postContentType, string postUrl, uint views, bool hasexcerpt, string excerpt, ContentTypes excerptContentType, BlogPostTypes blogpostType, string postName)

    {
      WriteStartElement("post");
      WriteNodeAttributes(id, dateCreated, dateModified, approved);
      WriteAttributeString("post-url", postUrl);
      WriteAttributeStringRequired("type", blogpostType.ToString().ToLower());
      WriteAttributeStringRequired("hasexcerpt", hasexcerpt.ToString().ToLower());
      WriteAttributeStringRequired("views", views.ToString());
      WriteContent("title", BlogMLContent.Create(title, titleContentType));
      WriteContent("content", BlogMLContent.Create(content, postContentType));
      if (postName != null)
      {
        WriteContent("post-name", BlogMLContent.Create(postName, ContentTypes.Text));
      }
      if (hasexcerpt)
      {
        WriteContent("excerpt", BlogMLContent.Create(excerpt, excerptContentType));
      }
    }

    protected void WriteStartComments()
    {
      WriteStartElement("comments");
    }


    protected void WriteComment(string id, string title, DateTime dateCreated, DateTime dateModified, bool approved, string userName, string userEmail, string userUrl, string content)
    {
      WriteComment(id, title, ContentTypes.Text, dateCreated, dateModified, approved, userName, userEmail, userUrl, content, ContentTypes.Text);
    }

    protected void WriteComment(string id, BlogMLContent title, DateTime dateCreated, DateTime dateModified, bool approved, string userName, string userEmail, string userUrl, BlogMLContent content)
    {
      WriteStartElement("comment");
      WriteNodeAttributes(id, dateCreated, dateModified, approved);
      WriteAttributeStringRequired("user-name", userName ?? "");
      WriteAttributeString("user-url", userUrl ?? "");
      WriteAttributeString("user-email", userEmail ?? "");
      WriteContent("title", title);
      WriteContent("content", content);
      WriteEndElement();
    }


    protected void WriteComment(string id, string title, ContentTypes titleContentType, DateTime dateCreated, DateTime dateModified, bool approved, string userName, string userEmail, string userUrl, string content, ContentTypes commentContentType)
    {
      WriteStartElement("comment");
      WriteNodeAttributes(id, dateCreated, dateModified, approved);
      WriteAttributeStringRequired("user-name", userName ?? "");
      WriteAttributeString("user-url", userUrl ?? "");
      WriteAttributeString("user-email", userEmail ?? "");
      WriteContent("title", BlogMLContent.Create(title, titleContentType));
      WriteContent("content", BlogMLContent.Create(content, commentContentType));
      WriteEndElement();
    }


    protected void WriteTrackback(string id, string title, DateTime dateCreated, DateTime dateModified, bool approved, string url)
    {
      WriteTrackback(id, title, ContentTypes.Text, dateCreated, dateModified, approved, url);
    }


    protected void WriteTrackback(string id, string title, ContentTypes titleContentType, DateTime dateCreated, DateTime dateModified, bool approved, string url)
    {
      WriteStartElement("trackback");
      WriteNodeAttributes(id, dateCreated, dateModified, approved);
      WriteAttributeStringRequired("url", url);
      WriteContent("title", BlogMLContent.Create(title, titleContentType));
      WriteEndElement();
    }


    protected void WriteAttributeStringRequired(string name, string value)
    {
      if (string.IsNullOrEmpty(value))
      {
        throw new ArgumentNullException("value", name);
      }
      Writer.WriteAttributeString(name, value);
    }


    protected void WriteAttributeString(string name, string value)
    {
      if (!string.IsNullOrEmpty(value))
      {
        Writer.WriteAttributeString(name, value);
      }
    }

    protected void WriteContent(string elementName, BlogMLContent content)
    {
      WriteStartElement(elementName);
      string contentType = (Enum.GetName(typeof(ContentTypes), content.ContentType) ?? "text").ToLowerInvariant();
      WriteAttributeString("type", contentType);
      Writer.WriteCData(content.Text ?? string.Empty);
      WriteEndElement();
    }

    protected void WriteAttachment(string externalUri, string mimeType, string fullUrl)
    {
      WriteAttachment(fullUrl, 0d, mimeType, externalUri, false, null);
    }

    protected void WriteAttachment(string embeddedUrl, string mimeType, Stream inputStream)
    {
      using (var reader = new BinaryReader(inputStream))
      {
        reader.BaseStream.Position = 0L;
        byte[] data = reader.ReadBytes((int)inputStream.Length);
        WriteAttachment(embeddedUrl, data.Length, mimeType, null, true, data);
      }
    }

    protected void WriteAttachment(string embeddedUrl, double size, string mimeType, string externalUri, bool embedded, byte[] data)
    {
      WriteStartElement("attachment");

      try
      {

        WriteAttributeStringRequired("url", embeddedUrl);

        if (size > 0d)
        {
          WriteAttributeStringRequired("size", size.ToString());
        }

        if (mimeType != null)
        {
          WriteAttributeStringRequired("mime-type", mimeType);
        }

        if (!string.IsNullOrEmpty(externalUri))
        {
          WriteAttributeStringRequired("external-uri", externalUri);
        }

        WriteAttributeString("embedded", embedded ? "true" : "false");

        if (embedded)
        {
          Writer.WriteBase64(data, 0, data.Length);
        }
      }
      finally
      {
        WriteEndElement();
      }

    }


    internal void CopyStream(Stream src, Stream dst)
    {
      var buf = new byte[4096];
      while (true)
      {
        int bytesRead = src.Read(buf, 0, buf.Length);

        // Read returns 0 when reached end of stream.
        if (bytesRead == 0)
        {
          break;
        }

        dst.Write(buf, 0, bytesRead);
      }
    }


    public sealed class SgmlUtil
    {
      public static bool IsRootUrlOf(string rootUrl, string url)
      {

        if (rootUrl is null)
        {
          throw new ArgumentNullException("rootUrl");
        }

        if (url is null)
        {
          throw new ArgumentNullException("url");
        }

        rootUrl = rootUrl.Trim().ToLower();
        url = url.Trim().ToLower();
        // is it a full path
        if (url.StartsWith("http://"))
        {
          return url.StartsWith(rootUrl);
        }

        // it's local
        return true;
      }

      public static string StripRootUrlPath(string rootUrl, string url)
      {
        if (url.StartsWith(rootUrl))
        {
          url = url.Remove(0, rootUrl.Length);
        }

        if (url.StartsWith("/"))
        {
          url.TrimStart('/');
        }

        return url;
      }

      public static string CleanAttachmentUrls(string content, string oldPath, string newPath)
      {
        oldPath = Regex.Escape(oldPath);

        content = Regex.Replace(content, oldPath, newPath, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
        return content;
      }

      public static string[] GetAttributeValues(string content, string tag, string attribute)
      {
        var srcrx = CreateAttributeRegex(attribute);
        var matches = CreateTagRegex(tag).Matches(content);
        var sources = new string[matches.Count];
        for (int i = 0, loopTo = sources.Length - 1; i <= loopTo; i++)
        {
          var m = srcrx.Match(matches[i].Value);
          sources[i] = m.Groups["Value"].Value;
        }
        return sources;
      }

      public static Regex CreateTagRegex(string name)
      {

        string pattern = @"<\s*{0}[^>]+>";

        pattern = string.Format(pattern, name);
        return new Regex(pattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
      }

      public static Regex CreateAttributeRegex(string name)
      {
        string pattern = @"{0}\s*=\s*['""]?\s*(?<Value>[^'"" ]+)";
        pattern = string.Format(pattern, name);
        return new Regex(pattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
      }

    }

  }

}