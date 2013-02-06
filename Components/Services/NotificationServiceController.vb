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

Imports DotNetNuke.Modules.Blog.Components.Common
Imports DotNetNuke.Modules.Blog.Components.Controllers
Imports DotNetNuke.Web.Services
Imports System.Web.Mvc
Imports DotNetNuke.Services.Social.Notifications
Imports DotNetNuke.Modules.Blog.Components.Entities

Namespace Components.Services

    Public Class NotificationServiceController
        Inherits DnnController

#Region "Private Members"

        Private BlogId As Integer = -1
        Private EntryId As Integer = -1
        Private CommentId As Integer = -1

#End Region

        <DnnAuthorize()> _
        Public Function ApproveEntry(notificationId As Integer) As ActionResult
            Dim notify As Notification = NotificationsController.Instance.GetNotification(notificationId)
            ParsePublishKey(notify.Context)

            Dim cntBlog As New BlogController
            Dim objBlog As BlogInfo = cntBlog.GetBlog(BlogId)

            If objBlog Is Nothing Then
                Return Json(New With {.Result = "error"})
            End If

            If objBlog.AuthorMode = Constants.AuthorMode.PersonalMode Then
                ' this should never happen (only if they changed modes)
                Return Json(New With {.Result = "error"})
            ElseIf objBlog.AuthorMode = Constants.AuthorMode.GhostMode Then
                Dim isOwner As Boolean = objBlog.UserID = UserInfo.UserID

                ' NOTE: we need to allow more than just the owner (think of admin)
                If Not isOwner Then
                    Return Json(New With {.Result = "error"})
                End If

                Dim cntEntry As New EntryController
                Dim objEntry As EntryInfo = cntEntry.GetEntry(EntryId, PortalSettings.PortalId)

                If objEntry Is Nothing Then
                    Return Json(New With {.Result = "error"})
                End If

                objEntry.Published = True
                'CP TO DO: This shouldn't assume vocab = 1
                cntEntry.UpdateEntry(objEntry, objEntry.TabID, PortalSettings.PortalId, 1)
            Else
                ' blogger mode
                Dim isOwner As Boolean = objBlog.UserID = UserInfo.UserID
                Dim cntEntry As New EntryController
                Dim objEntry As EntryInfo = cntEntry.GetEntry(EntryId, PortalSettings.PortalId)

                If objEntry Is Nothing Then
                    Return Json(New With {.Result = "error"})
                End If

                Dim objSecurity As New ModuleSecurity(objEntry.ModuleID, objEntry.TabID)

                If objSecurity.CanAddEntry(isOwner, Constants.AuthorMode.BloggerMode) Then
                    objEntry.Published = True
                    'CP TO DO: This shouldn't assume vocab = 1
                    cntEntry.UpdateEntry(objEntry, objEntry.TabID, PortalSettings.PortalId, 1)
                Else
                    Return Json(New With {.Result = "error"})
                End If
            End If

            NotificationsController.Instance().DeleteNotification(notificationId)
            Return Json(New With {.Result = "success"})
        End Function

        <DnnAuthorize()> _
        Public Function DeleteEntry(notificationId As Integer) As ActionResult
            Dim notify As Notification = NotificationsController.Instance.GetNotification(notificationId)
            ParsePublishKey(notify.Context)
            Dim cntBlog As New BlogController
            Dim objBlog As BlogInfo = cntBlog.GetBlog(BlogId)

            If objBlog Is Nothing Then
                Return Json(New With {.Result = "error"})
            End If

            If objBlog.AuthorMode = Constants.AuthorMode.PersonalMode Then
                ' this should never happen (only if they changed modes)
                Return Json(New With {.Result = "error"})
            ElseIf objBlog.AuthorMode = Constants.AuthorMode.GhostMode Then
                Dim isOwner As Boolean = objBlog.UserID = UserInfo.UserID

                ' NOTE: we need to allow more than just the owner (think of admin)
                If Not isOwner Then
                    Return Json(New With {.Result = "error"})
                End If

                Dim cntEntry As New EntryController
                Dim objEntry As EntryInfo = cntEntry.GetEntry(EntryId, PortalSettings.PortalId)

                If objEntry Is Nothing Then
                    Return Json(New With {.Result = "error"})
                End If
                'CP TO DO: This shouldn't assume vocab = 1
                cntEntry.DeleteEntry(EntryId, objEntry.ContentItemId, objBlog.BlogID, PortalSettings.PortalId, 1)
            Else
                ' blogger mode
                Dim isOwner As Boolean = objBlog.UserID = UserInfo.UserID
                Dim cntEntry As New EntryController
                Dim objEntry As EntryInfo = cntEntry.GetEntry(EntryId, PortalSettings.PortalId)

                If objEntry Is Nothing Then
                    Return Json(New With {.Result = "error"})
                End If

                Dim objSecurity As New ModuleSecurity(objEntry.ModuleID, objEntry.TabID)

                If objSecurity.CanAddEntry(isOwner, Constants.AuthorMode.BloggerMode) Then
                    'CP TO DO: This shouldn't assume vocab = 1
                    cntEntry.DeleteEntry(EntryId, objEntry.ContentItemId, objBlog.BlogID, PortalSettings.PortalId, 1)
                Else
                    Return Json(New With {.Result = "error"})
                End If
            End If
            NotificationsController.Instance().DeleteNotification(notificationId)
            Return Json(New With {.Result = "success"})
        End Function

        <DnnAuthorize()> _
        Public Function ApproveComment(notificationId As Integer) As ActionResult
            Dim notify As Notification = NotificationsController.Instance.GetNotification(notificationId)
            ParseCommentKey(notify.Context)

            Dim cntBlog As New BlogController
            Dim objBlog As BlogInfo = cntBlog.GetBlog(BlogId)

            If objBlog Is Nothing Then
                Return Json(New With {.Result = "error"})
            End If

            Dim isOwner As Boolean = objBlog.UserID = UserInfo.UserID

            Dim cntEntry As New EntryController
            Dim objEntry As EntryInfo = cntEntry.GetEntry(EntryId, PortalSettings.PortalId)

            If objEntry Is Nothing Then
                Return Json(New With {.Result = "error"})
            End If

            Dim objSecurity As New ModuleSecurity(objEntry.ModuleID, objEntry.TabID)

            If Not objSecurity.CanApproveComment() Then
                Return Json(New With {.Result = "error"})
            End If

            Dim cntComment As New CommentController
            Dim objComment As CommentInfo = cntComment.GetComment(CommentId)

            If objComment Is Nothing Then
                Return Json(New With {.Result = "error"})
            End If

            objComment.Approved = True
            cntComment.UpdateComment(objComment)

            NotificationsController.Instance().DeleteNotification(notificationId)
            Return Json(New With {.Result = "success"})
        End Function

        <DnnAuthorize()> _
        Public Function DeleteComment(notificationId As Integer) As ActionResult
            Dim notify As Notification = NotificationsController.Instance.GetNotification(notificationId)
            ParseCommentKey(notify.Context)

            Dim cntBlog As New BlogController
            Dim objBlog As BlogInfo = cntBlog.GetBlog(BlogId)

            If objBlog Is Nothing Then
                Return Json(New With {.Result = "error"})
            End If

            Dim isOwner As Boolean = objBlog.UserID = UserInfo.UserID

            Dim cntEntry As New EntryController
            Dim objEntry As EntryInfo = cntEntry.GetEntry(EntryId, PortalSettings.PortalId)

            If objEntry Is Nothing Then
                Return Json(New With {.Result = "error"})
            End If

            Dim objSecurity As New ModuleSecurity(objEntry.ModuleID, objEntry.TabID)

            If Not objSecurity.CanApproveComment() Then
                Return Json(New With {.Result = "error"})
            End If

            Dim cntComment As New CommentController
            Dim objComment As CommentInfo = cntComment.GetComment(CommentId)

            If objComment Is Nothing Then
                Return Json(New With {.Result = "error"})
            End If

            cntComment.DeleteComment(CommentId)

            ' No journal call here because it should have never been added (since it wasn't approved)

            NotificationsController.Instance().DeleteNotification(notificationId)
            Return Json(New With {.Result = "success"})
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