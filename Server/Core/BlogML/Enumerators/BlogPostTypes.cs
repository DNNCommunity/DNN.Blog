
namespace DotNetNuke.Modules.Blog.Core.BlogML
{
  public enum BlogPostTypes : short
  {
    [System.Xml.Serialization.XmlEnum("normal")]
    Normal = 1,
    [System.Xml.Serialization.XmlEnum("article")]
    Article = 2
  }
}