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

Imports DotNetNuke.Services.Social.Notifications
Imports DotNetNuke.Modules.Blog.Components.Entities
Imports DotNetNuke.Entities.Users

Namespace Components.Integration

    Public Class Notifications

        Friend Sub EntryPendingApproval(ByVal objBlog As BlogInfo, ByVal objEntry As EntryInfo, ByVal portalId As Integer, ByVal subject As String)
            Dim notificationType As NotificationType = NotificationsController.Instance.GetNotificationType(Common.Constants.NotificationPublishingTypeName)

            Select Case objBlog.AuthorMode
                Case Common.Constants.AuthorMode.PersonalMode
                    ' should never happen
                Case Common.Constants.AuthorMode.GhostMode
                    Dim notificationKey As String = String.Format("{0}:{1}:{2}:{3}:{4}", Components.Common.Constants.ContentTypeName, objEntry.TabID, objEntry.ModuleID, objEntry.BlogID, objEntry.EntryID)
                    Dim objNotification As New Notification

                    objNotification.NotificationTypeID = notificationType.NotificationTypeId
                    objNotification.Subject = subject
                    objNotification.Body = objEntry.Title
                    objNotification.IncludeDismissAction = False
                    objNotification.SenderUserID = objEntry.CreatedUserId
                    objNotification.Context = notificationKey

                    Dim objOwner As UserInfo = UserController.GetUserById(portalId, objBlog.UserID)
                    Dim colUsers As New List(Of UserInfo)

                    colUsers.Add(objOwner)

                    NotificationsController.Instance.SendNotification(objNotification, portalId, Nothing, colUsers)
                Case Else
                    ' in blogger mode, we are not sending any notifications at this time
            End Select
        End Sub

    End Class

End Namespace