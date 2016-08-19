Imports System.Xml.Serialization

Namespace BlogML.Xml
 <Serializable>
 <XmlRoot(ElementName:="blog", [Namespace]:="http://www.blogml.com/2006/09/BlogML")>
 Public NotInheritable Class BlogMLBlog

  <XmlAttribute("root-url")>
  Public Property RootUrl As String

  <XmlElement("title")>
  Public Property Title As String

  <XmlElement("sub-title")>
  Public Property SubTitle As String

  <XmlAttribute("date-created", DataType:="dateTime")>
  Public Property DateCreated As DateTime = DateTime.Now

  <XmlArray("extended-properties")>
  <XmlArrayItem("property", GetType(Pair(Of String, String)))>
  Public Property ExtendedProperties As New ExtendedPropertiesCollection

  <XmlArray("authors")>
  <XmlArrayItem("author", GetType(BlogMLAuthor))>
  Public Property Authors As New AuthorCollection

  <XmlArray("posts")>
  <XmlArrayItem("post", GetType(BlogMLPost))>
  Public Property Posts As New PostCollection

  <XmlArray("categories")>
  <XmlArrayItem("category", GetType(BlogMLCategory))>
  Public Property Categories As New CategoryCollection

  <Serializable>
  Public NotInheritable Class AuthorCollection
   Inherits List(Of BlogMLAuthor)
  End Class

  <Serializable>
  Public NotInheritable Class PostCollection
   Inherits List(Of BlogMLPost)
  End Class

  <Serializable>
  Public NotInheritable Class CategoryCollection
   Inherits List(Of BlogMLCategory)
  End Class

  <Serializable>
  Public NotInheritable Class ExtendedPropertiesCollection
   Inherits List(Of Pair(Of String, String))
  End Class
 End Class
End Namespace
