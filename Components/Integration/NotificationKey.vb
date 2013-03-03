
Namespace Integration
 Public Class NotificationKey

  Public ID As String = ""
  Public ModuleId As Integer = -1
  Public BlogId As Integer = -1
  Public ContentItemId As Integer = -1
  Public CommentId As Integer = -1

  Public Sub New(key As String)
   Dim keyParts() As String = key.Split(":"c)
   If keyParts.Length < 5 Then Exit Sub
   ID = keyParts(0)
   ModuleId = Integer.Parse(keyParts(1))
   BlogId = Integer.Parse(keyParts(2))
   ContentItemId = Integer.Parse(keyParts(3))
   CommentId = Integer.Parse(keyParts(4))
  End Sub

  Public Sub New(id As String, moduleId As Integer, blogId As Integer, contentItemId As Integer, commentId As Integer)
   Me.ID = id
   Me.ModuleId = moduleId
   Me.BlogId = blogId
   Me.ContentItemId = contentItemId
   Me.CommentId = commentId
  End Sub

  Public Shadows Function ToString() As String
   Return String.Format("{0}:{1}:{2}:{3}:{4}", ID, ModuleId, BlogId, ContentItemId, CommentId)
  End Function

 End Class
End Namespace
