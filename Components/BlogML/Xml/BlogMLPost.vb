Imports System.Xml.Serialization

Namespace BlogML.Xml
 <Serializable>
 Public NotInheritable Class BlogMLPost
  Inherits BlogMLNode

  <XmlAttribute("post-url")>
  Public Property PostUrl As String

  <XmlAttribute("hasexcerpt")>
  Public Property HasExcerpt As Boolean = False

  <XmlAttribute("type")>
  Public Property PostType As New BlogPostTypes

  <XmlAttribute("views")>
  Public Property Views As UInt32 = 0

  <XmlAttribute("image", [Namespace]:="http://dnn-connect.org/blog/")>
  Public Property Image As String

  <XmlAttribute("allow-comments", [Namespace]:="http://dnn-connect.org/blog/")>
  Public Property AllowComments As Boolean = True

  <XmlAttribute("display-copyright", [Namespace]:="http://dnn-connect.org/blog/")>
  Public Property DisplayCopyright As Boolean = False

  <XmlAttribute("copyright", [Namespace]:="http://dnn-connect.org/blog/")>
  Public Property Copyright As String

  <XmlAttribute("locale", [Namespace]:="http://dnn-connect.org/blog/")>
  Public Property Locale As String

  <XmlElement("post-name")>
  Public Property PostName As String

  <XmlElement("content")>
  Public Property Content As New BlogMLContent

  <XmlElement("excerpt")>
  Public Property Excerpt As New BlogMLContent

  <XmlArray("authors")>
  <XmlArrayItem("author", GetType(BlogMLAuthorReference))>
  Public ReadOnly Property Authors As New AuthorReferenceCollection

  <XmlArray("categories")>
  <XmlArrayItem("category", GetType(BlogMLCategoryReference))>
  Public ReadOnly Property Categories As New CategoryReferenceCollection

  <XmlArray("comments")>
  <XmlArrayItem("comment", GetType(BlogMLComment))>
  Public ReadOnly Property Comments As New CommentCollection

  <XmlArray("trackbacks")>
  <XmlArrayItem("trackback", GetType(BlogMLTrackback))>
  Public ReadOnly Property Trackbacks As New TrackbackCollection

  <XmlArray("attachments")>
  <XmlArrayItem("attachment", GetType(BlogMLAttachment))>
  Public ReadOnly Property Attachments As New AttachmentCollection

  <Serializable>
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
    Dim item As New BlogMLAuthorReference
    item.Ref = authorID
    MyBase.Add(item)
    Return item
   End Function
  End Class

  <Serializable>
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

  <Serializable>
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

  <Serializable>
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
    Dim item As New BlogMLCategoryReference
    item.Ref = categoryID
    MyBase.Add(item)
    Return item
   End Function
  End Class

  <Serializable>
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
