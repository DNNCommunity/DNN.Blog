Imports System.Xml.Serialization
Imports System.Collections

Namespace BlogML.Xml
 <Serializable()> _
 Public NotInheritable Class BlogMLPost
  Inherits BlogMLNode
  Private m_postUrl As String
  Private m_hasexcerpt As Boolean = False
  Private m_content As New BlogMLContent()
  Private m_excerpt As New BlogMLContent()
  Private blogpostType As New BlogPostTypes()
  Private m_authors As AuthorReferenceCollection
  Private m_comments As CommentCollection
  Private m_trackbacks As TrackbackCollection
  Private m_categories As CategoryReferenceCollection
  Private m_attachments As AttachmentCollection
  Private postviews As UInt32 = 0
  Private m_postname As String

  <XmlAttribute("post-url")> _
  Public Property PostUrl() As String
   Get
    Return Me.m_postUrl
   End Get
   Set(value As String)
    Me.m_postUrl = value
   End Set
  End Property

  <XmlAttribute("hasexcerpt")> _
  Public Property HasExcerpt() As Boolean
   Get
    Return Me.m_hasexcerpt
   End Get
   Set(value As Boolean)
    Me.m_hasexcerpt = value
   End Set
  End Property

  <XmlAttribute("type")> _
  Public Property PostType() As BlogPostTypes
   Get
    Return Me.blogpostType
   End Get
   Set(value As BlogPostTypes)
    Me.blogpostType = value
   End Set
  End Property

  <XmlAttribute("views")> _
  Public Property Views() As UInt32
   Get
    Return Me.postviews
   End Get
   Set(value As UInt32)
    Me.postviews = value
   End Set
  End Property

  <XmlElement("post-name")> _
  Public Property PostName() As String
   Get
    Return Me.m_postname
   End Get
   Set(value As String)
    Me.m_postname = value
   End Set
  End Property

  <XmlElement("content")> _
  Public Property Content() As BlogMLContent
   Get
    Return Me.m_content
   End Get
   Set(value As BlogMLContent)
    Me.m_content = value
   End Set
  End Property

  <XmlElement("excerpt")> _
  Public Property Excerpt() As BlogMLContent
   Get
    Return Me.m_excerpt
   End Get
   Set(value As BlogMLContent)
    Me.m_excerpt = value
   End Set
  End Property

  <XmlArray("authors")> _
  <XmlArrayItem("author", GetType(BlogMLAuthorReference))> _
  Public ReadOnly Property Authors() As AuthorReferenceCollection
   Get
    If Me.m_authors Is Nothing Then
     Me.m_authors = New AuthorReferenceCollection()
    End If
    Return Me.m_authors
   End Get
  End Property

  <XmlArray("categories")> _
  <XmlArrayItem("category", GetType(BlogMLCategoryReference))> _
  Public ReadOnly Property Categories() As CategoryReferenceCollection
   Get
    If Me.m_categories Is Nothing Then
     Me.m_categories = New CategoryReferenceCollection()
    End If
    Return Me.m_categories
   End Get
  End Property

  <XmlArray("comments")> _
  <XmlArrayItem("comment", GetType(BlogMLComment))> _
  Public ReadOnly Property Comments() As CommentCollection
   Get
    If Me.m_comments Is Nothing Then
     Me.m_comments = New CommentCollection()
    End If
    Return Me.m_comments
   End Get
  End Property


  <XmlArray("trackbacks")> _
  <XmlArrayItem("trackback", GetType(BlogMLTrackback))> _
  Public ReadOnly Property Trackbacks() As TrackbackCollection
   Get
    If Me.m_trackbacks Is Nothing Then
     Me.m_trackbacks = New TrackbackCollection()
    End If
    Return Me.m_trackbacks
   End Get
  End Property

  <XmlArray("attachments")> _
  <XmlArrayItem("attachment", GetType(BlogMLAttachment))> _
  Public ReadOnly Property Attachments() As AttachmentCollection
   Get
    If Me.m_attachments Is Nothing Then
     Me.m_attachments = New AttachmentCollection()
    End If
    Return Me.m_attachments
   End Get
  End Property

  <Serializable()> _
  Public NotInheritable Class AuthorReferenceCollection
   Inherits ArrayList
   Default Public Shadows ReadOnly Property Item(index As Integer) As BlogMLAuthorReference
    Get
     Return TryCast(MyBase.Item(index), BlogMLAuthorReference)
    End Get
   End Property

   Public Overloads Sub Add(value As BlogMLAuthorReference)
    MyBase.Add(value)
   End Sub

   Public Overloads Function Add(authorID As String) As BlogMLAuthorReference
    Dim item As New BlogMLAuthorReference()
    item.Ref = authorID
    MyBase.Add(item)
    Return item
   End Function
  End Class

  <Serializable()> _
  Public NotInheritable Class CommentCollection
   Inherits ArrayList
   Default Public Shadows ReadOnly Property Item(index As Integer) As BlogMLComment
    Get
     Return TryCast(MyBase.Item(index), BlogMLComment)
    End Get
   End Property

   Public Overloads Sub Add(value As BlogMLComment)
    MyBase.Add(value)
   End Sub
  End Class

  <Serializable()> _
  Public NotInheritable Class TrackbackCollection
   Inherits ArrayList
   Default Public Shadows ReadOnly Property Item(index As Integer) As BlogMLTrackback
    Get
     Return TryCast(MyBase.Item(index), BlogMLTrackback)
    End Get
   End Property

   Public Overloads Sub Add(value As BlogMLTrackback)
    MyBase.Add(value)
   End Sub
  End Class

  <Serializable()> _
  Public NotInheritable Class CategoryReferenceCollection
   Inherits ArrayList
   Default Public Shadows ReadOnly Property Item(index As Integer) As BlogMLCategoryReference
    Get
     Return TryCast(MyBase.Item(index), BlogMLCategoryReference)
    End Get
   End Property

   Public Overloads Sub Add(value As BlogMLCategoryReference)
    MyBase.Add(value)
   End Sub

   Public Overloads Function Add(categoryID As String) As BlogMLCategoryReference
    Dim item As New BlogMLCategoryReference()
    item.Ref = categoryID
    MyBase.Add(item)
    Return item
   End Function
  End Class

  <Serializable()> _
  Public NotInheritable Class AttachmentCollection
   Inherits ArrayList
   Default Public Shadows ReadOnly Property Item(index As Integer) As BlogMLAttachment
    Get
     Return TryCast(MyBase.Item(index), BlogMLAttachment)
    End Get
   End Property

   Public Overloads Sub Add(value As BlogMLAttachment)
    MyBase.Add(value)
   End Sub
  End Class
 End Class
End Namespace
