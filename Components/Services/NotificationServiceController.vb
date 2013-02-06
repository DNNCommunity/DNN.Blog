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
Imports DotNetNuke.Modules.Blog.Controllers
Imports System.Globalization
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json
Imports System.Net
Imports System.Net.Http
Imports System.Web.Http
Imports DotNetNuke.Web.Api
Imports DotNetNuke.Services.Social.Notifications
Imports DotNetNuke.Modules.Blog.Entities

Namespace Services

 Public Class NotificationServiceController
  Inherits DnnApiController

#Region "Private Members"

  Private BlogId As Integer = -1
  Private EntryId As Integer = -1
  Private CommentId As Integer = -1

#End Region

  <DnnAuthorize()> _
  Public Function ApproveEntry(notificationId As Integer) As HttpResponseMessage
   Dim notify As Notification = NotificationsController.Instance.GetNotification(notificationId)
   ParsePublishKey(notify.Context)

   Dim objBlog As BlogInfo = BlogController.GetBlog(BlogId)

   If objBlog Is Nothing Then
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   End If

   If objBlog.AuthorMode = Constants.AuthorMode.PersonalMode Then
    ' this should never happen (only if they changed modes)
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   ElseIf objBlog.AuthorMode = Constants.AuthorMode.GhostMode Then
    Dim isOwner As Boolean = objBlog.UserID = UserInfo.UserID

    ' NOTE: we need to allow more than just the owner (think of admin)
    If Not isOwner Then
     Return Request.CreateResponse(HttpStatusCode.Unauthorized, New With {.Result = "error"})
    End If

    Dim objEntry As EntryInfo = EntryController.GetEntry(EntryId, PortalSettings.PortalId)

    If objEntry Is Nothing Then
     Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
    End If

    objEntry.Published = True
    'CP TO DO: This shouldn't assume vocab = 1
    EntryController.UpdateEntry(objEntry, objEntry.TabID, PortalSettings.PortalId, 1)
   Else
    ' blogger mode
    Dim isOwner As Boolean = objBlog.UserID = UserInfo.UserID
    Dim objEntry As EntryInfo = EntryController.GetEntry(EntryId, PortalSettings.PortalId)

    If objEntry Is Nothing Then
     Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
    End If

    Dim objSecurity As New ModuleSecurity(objEntry.ModuleID, objEntry.TabID, objBlog, UserInfo)

    If objSecurity.CanAddEntry Then
     objEntry.Published = True
     'CP TO DO: This shouldn't assume vocab = 1
     EntryController.UpdateEntry(objEntry, objEntry.TabID, PortalSettings.PortalId, 1)
    Else
     Return Request.CreateResponse(HttpStatusCode.Unauthorized, New With {.Result = "error"})
    End If
   End If

   NotificationsController.Instance().DeleteNotification(notificationId)
   Return Request.CreateResponse(HttpStatusCode.OK, New With {.Result = "success"})
  End Function

  <DnnAuthorize()> _
  Public Function DeleteEntry(notificationId As Integer) As HttpResponseMessage
   Dim notify As Notification = NotificationsController.Instance.GetNotification(notificationId)
   ParsePublishKey(notify.Context)
   Dim objBlog As BlogInfo = BlogController.GetBlog(BlogId)

   If objBlog Is Nothing Then
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   End If

   If objBlog.AuthorMode = Constants.AuthorMode.PersonalMode Then
    ' this should never happen (only if they changed modes)
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   ElseIf objBlog.AuthorMode = Constants.AuthorMode.GhostMode Then
    Dim isOwner As Boolean = objBlog.UserID = UserInfo.UserID

    ' NOTE: we need to allow more than just the owner (think of admin)
    If Not isOwner Then
     Return Request.CreateResponse(HttpStatusCode.Unauthorized, New With {.Result = "error"})
    End If

    Dim objEntry As EntryInfo = EntryController.GetEntry(EntryId, PortalSettings.PortalId)

    If objEntry Is Nothing Then
     Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
    End If
    'CP TO DO: This shouldn't assume vocab = 1
    EntryController.DeleteEntry(EntryId, objEntry.ContentItemId, objBlog.BlogID, PortalSettings.PortalId, 1)
   Else
    ' blogger mode
    Dim isOwner As Boolean = objBlog.UserID = UserInfo.UserID
    Dim objEntry As EntryInfo = EntryController.GetEntry(EntryId, PortalSettings.PortalId)

    If objEntry Is Nothing Then
     Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
    End If

    Dim objSecurity As New ModuleSecurity(objEntry.ModuleID, objEntry.TabID, objBlog, UserInfo)

    If objSecurity.CanAddEntry Then
     'CP TO DO: This shouldn't assume vocab = 1
     EntryController.DeleteEntry(EntryId, objEntry.ContentItemId, objBlog.BlogID, PortalSettings.PortalId, 1)
    Else
     Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
    End If
   End If
   NotificationsController.Instance().DeleteNotification(notificationId)
   Return Request.CreateResponse(HttpStatusCode.OK, New With {.Result = "success"})
  End Function

  <DnnAuthorize()> _
  Public Function ApproveComment(notificationId As Integer) As HttpResponseMessage
   Dim notify As Notification = NotificationsController.Instance.GetNotification(notificationId)
   ParseCommentKey(notify.Context)

   Dim objBlog As BlogInfo = BlogController.GetBlog(BlogId)

   If objBlog Is Nothing Then
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   End If

   Dim isOwner As Boolean = objBlog.UserID = UserInfo.UserID

   Dim objEntry As EntryInfo = EntryController.GetEntry(EntryId, PortalSettings.PortalId)

   If objEntry Is Nothing Then
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   End If

   Dim objSecurity As New ModuleSecurity(objEntry.ModuleID, objEntry.TabID, objBlog, UserInfo)

   If Not objSecurity.CanApproveComment() Then
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   End If

   Dim objComment As CommentInfo = CommentController.GetComment(CommentId)

   If objComment Is Nothing Then
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   End If

   objComment.Approved = True
   CommentController.UpdateComment(objComment)

   NotificationsController.Instance().DeleteNotification(notificationId)
   Return Request.CreateResponse(HttpStatusCode.OK, New With {.Result = "success"})
  End Function

  <DnnAuthorize()> _
  Public Function DeleteComment(notificationId As Integer) As HttpResponseMessage
   Dim notify As Notification = NotificationsController.Instance.GetNotification(notificationId)
   ParseCommentKey(notify.Context)

   Dim objBlog As BlogInfo = BlogController.GetBlog(BlogId)

   If objBlog Is Nothing Then
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   End If

   Dim isOwner As Boolean = objBlog.UserID = UserInfo.UserID

   Dim objEntry As EntryInfo = EntryController.GetEntry(EntryId, PortalSettings.PortalId)

   If objEntry Is Nothing Then
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   End If

   Dim objSecurity As New ModuleSecurity(objEntry.ModuleID, objEntry.TabID, objBlog, UserInfo)

   If Not objSecurity.CanApproveComment() Then
    Return Request.CreateResponse(HttpStatusCode.Unauthorized, New With {.Result = "error"})
   End If

   Dim objComment As CommentInfo = CommentController.GetComment(CommentId)

   If objComment Is Nothing Then
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   End If

   CommentController.DeleteComment(CommentId)

   ' No journal call here because it should have never been added (since it wasn't approved)

   NotificationsController.Instance().DeleteNotification(notificationId)
   Return Request.CreateResponse(HttpStatusCode.OK, New With {.Result = "success"})
  End Function

#Region "Private Methods"

  Private Sub ParsePublishKey(key As String)
   Dim keys() As String = key.Split(CChar(":"))
   ' 0 is content type string, to ensure unique key
   BlogId = Integer.Parse(keys(1))
   EntryId = Integer.Parse(keys(2))
  End Sub

  Private Sub ParseCommentKey(key As String)
   Dim keys() As String = key.Split(CChar(":"))
   ' 0 is content type string, to ensure unique key
   BlogId = Integer.Parse(keys(1))
   EntryId = Integer.Parse(keys(2))
   CommentId = Integer.Parse(keys(3))
  End Sub

#End Region

 End Class

End Namespace