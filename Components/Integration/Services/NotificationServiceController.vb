'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2011
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
Option Strict On
Option Explicit On

Imports DotNetNuke.Modules.Blog.Common
Imports System.Globalization
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json
Imports System.Net
Imports System.Net.Http
Imports System.Web.Http
Imports DotNetNuke.Web.Api
Imports DotNetNuke.Services.Social.Notifications
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Entities.Entries
Imports DotNetNuke.Modules.Blog.Entities.Comments
Imports DotNetNuke.Modules.Blog.Security
Imports DotNetNuke.Modules.Blog.Integration
Imports DotNetNuke.Modules.Blog.Services

Namespace Integration.Services

 Public Class NotificationServiceController
  Inherits DnnApiController

  Public Class NotificationDTO
   Public Property NotificationId As Integer
  End Class

#Region " Private Members "

  Private Property BlogModuleId As Integer = -1
  Private Property BlogId As Integer = -1
  Private Property Blog As BlogInfo = Nothing
  Private Property ContentItemId As Integer = -1
  Private Property Entry As EntryInfo = Nothing
  Private Property CommentId As Integer = -1
  Private Property Comment As CommentInfo = Nothing
  Private Property Security As ContextSecurity = Nothing

#End Region

#Region " Service Methods "
  <HttpPost()>
  <BlogAuthorizeAttribute(DotNetNuke.Modules.Blog.Services.SecurityAccessLevel.ApprovePost)>
  <ValidateAntiForgeryToken()>
  Public Function ApproveEntry(postData As NotificationDTO) As HttpResponseMessage
   Dim notify As Notification = NotificationsController.Instance.GetNotification(postData.NotificationId)
   ParsePublishKey(notify.Context)
   If Blog Is Nothing Or Entry Is Nothing Then
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   End If
   If Security.CanApproveEntry Then
    Entry.Published = True
    EntriesController.UpdateEntry(Entry, UserInfo.UserID)
   End If
   NotificationsController.Instance().DeleteNotification(postData.NotificationId)
   Return Request.CreateResponse(HttpStatusCode.OK, New With {.Result = "success"})
  End Function

  <HttpPost()>
  <BlogAuthorizeAttribute(DotNetNuke.Modules.Blog.Services.SecurityAccessLevel.EditPost)>
  <ValidateAntiForgeryToken()>
  Public Function DeleteEntry(postData As NotificationDTO) As HttpResponseMessage
   Dim notify As Notification = NotificationsController.Instance.GetNotification(postData.NotificationId)
   ParsePublishKey(notify.Context)
   If Blog Is Nothing Or Entry Is Nothing Then
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   End If
   If Security.CanApproveEntry Then
    EntriesController.DeleteEntry(ContentItemId)
   End If
   NotificationsController.Instance().DeleteNotification(postData.NotificationId)
   Return Request.CreateResponse(HttpStatusCode.OK, New With {.Result = "success"})
  End Function

  <HttpPost()>
  <BlogAuthorizeAttribute(DotNetNuke.Modules.Blog.Services.SecurityAccessLevel.ApproveComment)>
  <ValidateAntiForgeryToken()>
  Public Function ApproveComment(postData As NotificationDTO) As HttpResponseMessage
   Dim notify As Notification = NotificationsController.Instance.GetNotification(postData.NotificationId)
   ParseCommentKey(notify.Context)
   If Blog Is Nothing Or Entry Is Nothing Or Comment Is Nothing Then
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   End If
   If Security.CanApproveComment Then
    CommentsController.ApproveComment(BlogModuleId, BlogId, Comment)
   End If
   NotificationsController.Instance().DeleteNotification(postData.NotificationId)
   Return Request.CreateResponse(HttpStatusCode.OK, New With {.Result = "success"})
  End Function

  <HttpPost()>
  <BlogAuthorizeAttribute(DotNetNuke.Modules.Blog.Services.SecurityAccessLevel.ApproveComment)>
  <ValidateAntiForgeryToken()>
  Public Function DeleteComment(postData As NotificationDTO) As HttpResponseMessage
   Dim notify As Notification = NotificationsController.Instance.GetNotification(postData.NotificationId)
   ParseCommentKey(notify.Context)
   If Blog Is Nothing Or Entry Is Nothing Or Comment Is Nothing Then
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   End If
   If Security.CanApproveComment Then
    CommentsController.DeleteComment(BlogModuleId, BlogId, Comment)
   End If
   NotificationsController.Instance().DeleteNotification(postData.NotificationId)
   Return Request.CreateResponse(HttpStatusCode.OK, New With {.Result = "success"})
  End Function
#End Region

#Region " Private Methods "
  Private Sub ParsePublishKey(key As String)
   Dim nKey As New NotificationKey(key)
   BlogModuleId = nKey.ModuleId
   BlogId = nKey.BlogId
   ContentItemId = nKey.ContentItemId
   Blog = BlogsController.GetBlog(BlogId, UserInfo.UserID)
   Entry = EntriesController.GetEntry(ContentItemId, BlogModuleId)
   Security = New ContextSecurity(BlogModuleId, -1, Blog, UserInfo)
  End Sub

  Private Sub ParseCommentKey(key As String)
   Dim nKey As New NotificationKey(key)
   BlogModuleId = nKey.ModuleId
   BlogId = nKey.BlogId
   ContentItemId = nKey.ContentItemId
   CommentId = nKey.CommentId
   Blog = BlogsController.GetBlog(BlogId, UserInfo.UserID)
   Entry = EntriesController.GetEntry(ContentItemId, BlogModuleId)
   Comment = CommentsController.GetComment(CommentId)
   Security = New ContextSecurity(BlogModuleId, -1, Blog, UserInfo)
  End Sub
#End Region

 End Class

End Namespace