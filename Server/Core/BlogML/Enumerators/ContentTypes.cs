using System.Xml.Serialization;

namespace DotNetNuke.Modules.Blog.Core.BlogML
{
  public enum ContentTypes : short
  {
    [XmlEnum("html")]
    Html = 1,
    [XmlEnum("xhtml")]
    Xhtml = 2,
    [XmlEnum("text")]
    Text = 3,
    [XmlEnum("base64")]
    Base64 = 4
  }
}