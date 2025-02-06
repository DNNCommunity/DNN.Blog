using System;
using System.Xml.Serialization;

namespace DotNetNuke.Modules.Blog.Core.BlogML.Xml
{
  [Serializable]
  public sealed class BlogMLComment : BlogMLNode
  {
    private string m_userName;
    private string m_userEmail;
    private string m_userUrl;
    private BlogMLContent m_content = new BlogMLContent();

    [XmlAttribute("user-name")]
    public string UserName { get; set; }

    [XmlAttribute("user-url")]
    public string UserUrl { get; set; }

    [XmlAttribute("user-email")]
    public string UserEMail { get; set; }

    [XmlElement("content")]
    public BlogMLContent Content { get; set; }

  }
}