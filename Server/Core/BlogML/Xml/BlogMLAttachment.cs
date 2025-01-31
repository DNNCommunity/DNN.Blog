using System;
using System.Xml.Serialization;

namespace DotNetNuke.Modules.Blog.BlogML.Xml
{
  [Serializable]
  public sealed class BlogMLAttachment
  {

    [XmlAttribute("embedded")]
    public bool Embedded { get; set; } = false;

    [XmlAttribute("url")]
    public string Url { get; set; }

    [XmlAttribute("path")]
    public string Path { get; set; }

    [XmlAttribute("mime-type")]
    public string MimeType { get; set; }

    [XmlText]
    public byte[] Data { get; set; }

  }
}