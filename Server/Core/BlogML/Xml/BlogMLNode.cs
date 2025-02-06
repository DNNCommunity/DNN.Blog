using System;
using System.Xml.Serialization;

namespace DotNetNuke.Modules.Blog.Core.BlogML.Xml
{
  [Serializable]
  public abstract class BlogMLNode
  {

    [XmlAttribute("id")]
    public string ID { get; set; }

    [XmlElement("title")]
    public string Title { get; set; }

    [XmlAttribute("date-created", DataType = "dateTime")]
    public DateTime DateCreated { get; set; } = DateTime.Now;

    [XmlAttribute("date-modified", DataType = "dateTime")]
    public DateTime DateModified { get; set; } = DateTime.Now;

    [XmlAttribute("approved")]
    public bool Approved { get; set; } = true;

  }
}