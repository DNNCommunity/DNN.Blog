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
Imports DotNetNuke.Entities.Modules

Namespace Components.Integration

    Public Class Notifications

        ''' <summary>
        ''' This method will send a core notification to blog owners when a blog entry is pending publishing approval.
        ''' </summary>
        ''' <param name="objBlog"></param>
        ''' <param name="objEntry"></param>
        ''' <param name="portalId"></param>
        ''' <param name="subject"></param>
        ''' <remarks></remarks>
        Friend Sub EntryPendingApproval(ByVal objBlog As BlogInfo, ByVal objEntry As EntryInfo, ByVal portalId As Integer, ByVal subject As String)
            Dim notificationType As NotificationType = NotificationsController.Instance.GetNotificationType(Common.Constants.NotificationPublishingTypeName)

            Select Case objBlog.AuthorMode
                Case Common.Constants.AuthorMode.PersonalMode
                    ' should never happen
                Case Common.Constants.AuthorMode.GhostMode
                    Dim notificationKey As String = String.Format("{0}:{1}:{2}", Components.Common.Constants.ContentTypeName, objEntry.BlogID, objEntry.EntryID)
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

#Region "Install Methods"

        ''' <summary>
        ''' This will create a notification type associated w/ the module and also handle the actions that must be associated with it.
        ''' </summary>
        ''' <remarks>This should only ever run once, during 5.0.0 install (via IUpgradeable)</remarks>
        Friend Shared Sub AddNotificationTypes()
            Dim actions As List(Of NotificationTypeAction) = New List(Of NotificationTypeAction)
            Dim deskModuleId As Integer = DesktopModuleController.GetDesktopModuleByFriendlyName("Blog").DesktopModuleID

            Dim objNotificationType As NotificationType = New NotificationType
            objNotificationType.Name = Common.Constants.NotificationPublishingTypeName
            objNotificationType.Description = "Description"
            objNotificationType.DesktopModuleId = deskModuleId
            'objNotificationType.TimeToLive = TimeSpan.FromDays(7)

            If NotificationsController.Instance.GetNotificationType(objNotificationType.Name) Is Nothing Then
                Dim objAction As New NotificationTypeAction
                objAction.NameResourceKey = "Approve"
                objAction.DescriptionResourceKey = "ApprovePost"
                objAction.APICall = "DesktopModules/Blog/API/NotificationService.ashx/ApprovePost"
                objAction.Order = 1
                actions.Add(objAction)

                objAction = New NotificationTypeAction
                objAction.NameResourceKey = "Ignore"
                objAction.DescriptionResourceKey = "IngorePost"
                objAction.APICall = "DesktopModules/Blog/API/NotificationService.ashx/IngorePost"
                objAction.Order = 2
                actions.Add(objAction)

                NotificationsController.Instance.CreateNotificationType(objNotificationType)
                NotificationsController.Instance.SetNotificationTypeActions(actions, objNotificationType.NotificationTypeId)
            End If
        End Sub

#End Region

    End Class

End Namespace