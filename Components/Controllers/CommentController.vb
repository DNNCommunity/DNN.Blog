'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2012
' by DotNetNuke Corporation
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
Imports DotNetNuke.Modules.Blog.Providers.Data
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Modules.Blog.Entities

Namespace Controllers


 Public Class CommentController

  Public Shared Function GetComment(ByVal commentID As Integer) As CommentInfo

   Return CType(CBO.FillObject(DataProvider.Instance().GetComment(commentID), GetType(CommentInfo)), CommentInfo)

  End Function

  Public Shared Function ListComments(ByVal EntryID As Integer, ByVal ShowNonApproved As Boolean) As ArrayList

   Return CBO.FillCollection(DataProvider.Instance().GetCommentsByEntry(EntryID, ShowNonApproved), GetType(CommentInfo))

  End Function

  Public Shared Function ListCommentsByBlog(ByVal BlogID As Integer, ByVal ShowNonApproved As Boolean, ByVal MaximumComments As Integer) As List(Of CommentInfo)

   Return CBO.FillCollection(Of CommentInfo)(DataProvider.Instance().GetCommentsByBlog(BlogID, ShowNonApproved, MaximumComments))

  End Function

  Public Shared Function ListCommentsByPortal(ByVal PortalID As Integer, ByVal ShowNonApproved As Boolean, ByVal MaximumComments As Integer) As List(Of CommentInfo)

   Return CBO.FillCollection(Of CommentInfo)(DataProvider.Instance().GetCommentsByPortal(PortalID, ShowNonApproved, MaximumComments))

  End Function

  Public Shared Function AddComment(ByVal objComment As CommentInfo) As Integer

   Return CType(DataProvider.Instance().AddComment(objComment.EntryID, objComment.UserID, objComment.Title, objComment.Comment, objComment.Author, objComment.Approved, objComment.Website, objComment.Email, objComment.AddedDate), Integer)

  End Function

  Public Shared Sub UpdateComment(ByVal objComment As CommentInfo)

   DataProvider.Instance().UpdateComment(objComment.CommentID, objComment.EntryID, objComment.UserID, objComment.Title, objComment.Comment, objComment.Author, objComment.Approved, objComment.Website, objComment.Email, objComment.AddedDate)

  End Sub

  Public Shared Sub DeleteComment(ByVal commentID As Integer)

   DataProvider.Instance().DeleteComment(commentID)

  End Sub

  Public Shared Sub DeleteAllUnapproved(ByVal EntryID As Integer)

   DataProvider.Instance().DeleteAllUnapproved(EntryID)

  End Sub


 End Class
End Namespace