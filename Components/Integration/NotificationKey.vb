'
' DNN Connect - http://dnn-connect.org
' Copyright (c) 2015
' by DNN Connect
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'

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
