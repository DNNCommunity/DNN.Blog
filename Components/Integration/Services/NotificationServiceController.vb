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
Imports DotNetNuke.Modules.Blog.Entities.Posts
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
  Private Property Post As PostInfo = Nothing
  Private Property CommentId As Integer = -1
  Private Property Comment As CommentInfo = Nothing

#End Region

#Region " Service Methods "
  <HttpPost()>
  <BlogAuthorizeAttribute(DotNetNuke.Modules.Blog.Services.SecurityAccessLevel.ApprovePost)>
  <ValidateAntiForgeryToken()>
  Public Function ApprovePost(postData As NotificationDTO) As HttpResponseMessage
   Dim notify As Notification = NotificationsController.Instance.GetNotification(postData.NotificationId)
   ParsePublishKey(notify.Context)
   If Blog Is Nothing Or Post Is Nothing Then
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   End If
   Post.Published = True
   PostsController.UpdatePost(Post, UserInfo.UserID)
   NotificationsController.Instance().DeleteNotification(postData.NotificationId)
   Return Request.CreateResponse(HttpStatusCode.OK, New With {.Result = "success"})
  End Function

  <HttpPost()>
  <BlogAuthorizeAttribute(DotNetNuke.Modules.Blog.Services.SecurityAccessLevel.EditPost)>
  <ValidateAntiForgeryToken()>
  Public Function DeletePost(postData As NotificationDTO) As HttpResponseMessage
   Dim notify As Notification = NotificationsController.Instance.GetNotification(postData.NotificationId)
   ParsePublishKey(notify.Context)
   If Blog Is Nothing Or Post Is Nothing Then
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   End If
   PostsController.DeletePost(ContentItemId)
   NotificationsController.Instance().DeleteNotification(postData.NotificationId)
   Return Request.CreateResponse(HttpStatusCode.OK, New With {.Result = "success"})
  End Function

  <HttpPost()>
  <BlogAuthorizeAttribute(DotNetNuke.Modules.Blog.Services.SecurityAccessLevel.ApproveComment)>
  <ValidateAntiForgeryToken()>
  Public Function ApproveComment(postData As NotificationDTO) As HttpResponseMessage
   Dim notify As Notification = NotificationsController.Instance.GetNotification(postData.NotificationId)
   ParseCommentKey(notify.Context)
   If Blog Is Nothing Or Post Is Nothing Or Comment Is Nothing Then
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   End If
   CommentsController.ApproveComment(BlogModuleId, BlogId, Comment)
   NotificationsController.Instance().DeleteNotification(postData.NotificationId)
   Return Request.CreateResponse(HttpStatusCode.OK, New With {.Result = "success"})
  End Function

  <HttpPost()>
  <BlogAuthorizeAttribute(DotNetNuke.Modules.Blog.Services.SecurityAccessLevel.ApproveComment)>
  <ValidateAntiForgeryToken()>
  Public Function DeleteComment(postData As NotificationDTO) As HttpResponseMessage
   Dim notify As Notification = NotificationsController.Instance.GetNotification(postData.NotificationId)
   ParseCommentKey(notify.Context)
   If Blog Is Nothing Or Post Is Nothing Or Comment Is Nothing Then
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   End If
   CommentsController.DeleteComment(BlogModuleId, BlogId, Comment)
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
   Blog = BlogsController.GetBlog(BlogId, UserInfo.UserID, Threading.Thread.CurrentThread.CurrentCulture.Name)
   Post = PostsController.GetPost(ContentItemId, BlogModuleId, Threading.Thread.CurrentThread.CurrentCulture.Name)
  End Sub

  Private Sub ParseCommentKey(key As String)
   Dim nKey As New NotificationKey(key)
   BlogModuleId = nKey.ModuleId
   BlogId = nKey.BlogId
   ContentItemId = nKey.ContentItemId
   CommentId = nKey.CommentId
   Blog = BlogsController.GetBlog(BlogId, UserInfo.UserID, Threading.Thread.CurrentThread.CurrentCulture.Name)
   Post = PostsController.GetPost(ContentItemId, BlogModuleId, Threading.Thread.CurrentThread.CurrentCulture.Name)
   Comment = CommentsController.GetComment(CommentId)
  End Sub
#End Region

 End Class

End Namespace