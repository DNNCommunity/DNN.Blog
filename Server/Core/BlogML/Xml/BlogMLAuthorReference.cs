using System;
using System.Xml.Serialization;

namespace DotNetNuke.Modules.Blog.Core.BlogML.Xml
{
  [Serializable]
  public sealed class BlogMLAuthorReference
  {

    [XmlAttribute("ref")]
    public string Ref { get; set; }

  }
}