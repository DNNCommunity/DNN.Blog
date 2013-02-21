Imports System.Collections
Imports System.Collections.Specialized
Imports System.Xml.Serialization
Imports System.Collections.Generic

Namespace BlogML.Xml
 <Serializable()> _
 <XmlRoot(ElementName:="blog", [Namespace]:="http://www.blogml.com/2006/09/BlogML")> _
 Public NotInheritable Class BlogMLBlog
  Private m_rootUrl As String
  Private m_title As String
  Private m_subTitle As String
  Private m_dateCreated As DateTime = DateTime.Now
  Private m_authors As New AuthorCollection()
  Private m_posts As New PostCollection()
  Private m_categories As New CategoryCollection()
  Private m_extendedproperties As New ExtendedPropertiesCollection()


  <XmlAttribute("root-url")> _
  Public Property RootUrl() As String
   Get
    Return Me.m_rootUrl
   End Get
   Set(value As String)
    Me.m_rootUrl = value
   End Set
  End Property

  <XmlElement("title")> _
  Public Property Title() As String
   Get
    Return Me.m_title
   End Get
   Set(value As String)
    Me.m_title = value
   End Set
  End Property

  <XmlElement("sub-title")> _
  Public Property SubTitle() As String
   Get
    Return Me.m_subTitle
   End Get
   Set(value As String)
    Me.m_subTitle = value
   End Set
  End Property

  <XmlAttribute("date-created", DataType:="dateTime")> _
  Public Property DateCreated() As DateTime
   Get
    Return Me.m_dateCreated
   End Get
   Set(value As DateTime)
    Me.m_dateCreated = value
   End Set
  End Property

  <XmlArray("extended-properties")> _
  <XmlArrayItem("property", GetType(Pair(Of String, String)))> _
  Public ReadOnly Property ExtendedProperties() As ExtendedPropertiesCollection
   Get
    Return Me.m_extendedproperties
   End Get
  End Property

  <XmlArray("authors")> _
  <XmlArrayItem("author", GetType(BlogMLAuthor))> _
  Public ReadOnly Property Authors() As AuthorCollection
   Get
    Return Me.m_authors
   End Get
  End Property


  <XmlArray("posts")> _
  <XmlArrayItem("post", GetType(BlogMLPost))> _
  Public ReadOnly Property Posts() As PostCollection
   Get
    Return Me.m_posts
   End Get
  End Property

  <XmlArray("categories")> _
  <XmlArrayItem("category", GetType(BlogMLCategory))> _
  Public ReadOnly Property Categories() As CategoryCollection
   Get
    Return Me.m_categories
   End Get
  End Property

  <Serializable()> _
  Public NotInheritable Class AuthorCollection
   Inherits List(Of BlogMLAuthor)
  End Class

  <Serializable()> _
  Public NotInheritable Class PostCollection
   Inherits List(Of BlogMLPost)
  End Class

  <Serializable()> _
  Public NotInheritable Class CategoryCollection
   Inherits List(Of BlogMLCategory)
  End Class

  <Serializable()> _
  Public NotInheritable Class ExtendedPropertiesCollection
   Inherits List(Of Pair(Of String, String))
  End Class
 End Class
End Namespace
