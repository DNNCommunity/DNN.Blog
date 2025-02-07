using System;
using System.Xml.Serialization;

namespace DotNetNuke.Modules.Blog.Core.BlogML.Xml
{
  [Serializable]
  [Obsolete("I don't think that we use this now that we are using Dictionary<K,V>")]
  public sealed class Meta
  {

    [XmlAttribute("type")]
    public string Type { get; set; }

    [XmlAttribute("value")]
    public string Value { get; set; }

  }
}