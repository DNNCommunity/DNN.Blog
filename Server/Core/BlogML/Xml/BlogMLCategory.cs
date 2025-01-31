using System;
using System.Xml.Serialization;

namespace DotNetNuke.Modules.Blog.BlogML.Xml
{
  [Serializable]
  public sealed class BlogMLCategory : BlogMLNode
  {

    [XmlAttribute("description")]
    public string Description { get; set; }

    [XmlAttribute("parentref")]
    public string ParentRef { get; set; }

  }
}