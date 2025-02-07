using System;
using System.Xml.Serialization;

namespace DotNetNuke.Modules.Blog.Core.BlogML.Xml
{
  [Serializable]
  public sealed class BlogMLAuthor : BlogMLNode
  {

    [XmlAttribute("email")]
    public string Email { get; set; }

  }
}