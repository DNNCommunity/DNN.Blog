using System;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace DotNetNuke.Modules.Blog.BlogML.Xml
{
  [Serializable]
  public sealed class BlogMLContent
  {
    private bool Base64Encoded
    {
      get
      {
        return ContentType == ContentTypes.Base64;
      }
    }

    private bool HtmlEncoded
    {
      get
      {
        return ContentType == ContentTypes.Html || ContentType == ContentTypes.Xhtml;
      }
    }

    [XmlAttribute("type")]
    public ContentTypes ContentType
    {
      get
      {
        return m_ContentType;
      }
      set
      {
        m_ContentType = value;
      }
    }
    private ContentTypes m_ContentType = ContentTypes.Text;

    // Encoded Text
    [XmlText]
    public string Text
    {
      get
      {
        return m_Text;
      }
      set
      {
        m_Text = value;
      }
    }
    private string m_Text;

    [XmlIgnore]
    public string UncodedText
    {
      get
      {
        if (Base64Encoded)
        {
          byte[] byteArray = Convert.FromBase64String(Text);
          return Encoding.UTF8.GetString(byteArray);
        }
        if (HtmlEncoded)
        {
          return HttpUtility.HtmlDecode(Text);
        }
        return Text;
      }
    }

    public static BlogMLContent Create(string unencodedText, ContentTypes contentType)
    {
      var content = new BlogMLContent() { ContentType = contentType };
      if (content.Base64Encoded)
      {
        byte[] byteArray = Encoding.UTF8.GetBytes(unencodedText);
        content.Text = Convert.ToBase64String(byteArray);
      }
      else if (content.HtmlEncoded)
      {
        content.Text = HttpUtility.HtmlEncode(unencodedText);
      }
      else
      {
        content.Text = unencodedText;
      }
      return content;
    }
  }
}