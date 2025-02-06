using System;
using System.Xml.Serialization;

namespace DotNetNuke.Modules.Blog.Core.BlogML.Xml
{
  [Serializable]
  public sealed class BlogMLCategoryReference
  {

    [XmlAttribute("ref")]
    public string Ref { get; set; }

  }
}