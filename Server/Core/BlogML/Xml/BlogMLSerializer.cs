using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace DotNetNuke.Modules.Blog.Core.BlogML.Xml
{
  public class BlogMLSerializer
  {
    private static readonly object syncRoot = new object();
    private static XmlSerializer m_serializer;

    public static XmlSerializer Serializer
    {
      get
      {
        lock (syncRoot)
        {
          if (m_serializer is null)
          {
            m_serializer = new XmlSerializer(typeof(BlogMLBlog));
          }
          return m_serializer;
        }
      }
    }

    public static XmlSerializerNamespaces Namespaces
    {
      get
      {
        var ns = new XmlSerializerNamespaces();
        ns.Add("dnn", "http://dnn-connect.org/blog/");
        return ns;
      }
    }

    public static BlogMLBlog Deserialize(Stream stream)
    {
      return Serializer.Deserialize(stream) as BlogMLBlog;
    }

    public static BlogMLBlog Deserialize(TextReader reader)
    {
      return Serializer.Deserialize(reader) as BlogMLBlog;
    }

    public static BlogMLBlog Deserialize(XmlReader reader)
    {
      return Serializer.Deserialize(reader) as BlogMLBlog;
    }

    public static void Serialize(Stream stream, BlogMLBlog blog)
    {
      Serializer.Serialize(stream, blog, Namespaces);
    }

    public static void Serialize(TextWriter writer, BlogMLBlog blog)
    {
      Serializer.Serialize(writer, blog, Namespaces);
    }

    public static void Serialize(XmlWriter writer, BlogMLBlog blog)
    {
      Serializer.Serialize(writer, blog, Namespaces);
    }
  }
}