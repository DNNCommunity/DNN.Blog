using System;
using System.Xml.Serialization;

namespace DotNetNuke.Modules.Blog.BlogML.Xml
{
  [Serializable]
  public sealed class BlogMLTrackback : BlogMLNode
  {

    [XmlAttribute("url")]
    public string Url { get; set; }

  }
}