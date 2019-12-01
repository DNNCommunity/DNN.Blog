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

Imports DotNetNuke.Services.Social.Notifications
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Entities.Modules
Imports System.Linq
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Entities.Posts
Imports DotNetNuke.Modules.Blog.Security.Permissions
Imports DotNetNuke.Modules.Blog.Entities.Comments
Imports DotNetNuke.Modules.Blog.Integration.Integration
Imports DotNetNuke.Modules.Blog.Security.Security

Namespace Integration

 Public Class NotificationController

#Region " Integration Methods "
  ''' <summary>
  ''' This method will send a core notification to blog owners when a blog Post is pending publishing approval.
  ''' </summary>
  ''' <param name="objBlog"></param>
  ''' <param name="objPost"></param>
  ''' <param name="portalId"></param>
  ''' <param name="summary"></param>
  ''' <param name="title"></param>
  ''' <remarks></remarks>
  Public Shared Sub PostPendingApproval(objBlog As BlogInfo, objPost As PostInfo, portalId As Integer, summary As String, title As String)
   Dim notificationType As NotificationType = NotificationsController.Instance.GetNotificationType(NotificationPublishingTypeName)

   Dim notificationKey As New NotificationKey(ContentTypeName, objBlog.ModuleID, objPost.BlogID, objPost.ContentItemId, -1)
   Dim objNotification As New Notification

   objNotification.NotificationTypeID = notificationType.NotificationTypeId
   objNotification.Subject = title
   objNotification.Body = summary
   objNotification.IncludeDismissAction = False
   objNotification.SenderUserID = objPost.CreatedByUserID
   objNotification.Context = notificationKey.ToString

   Dim objOwner As UserInfo = UserController.GetUserById(portalId, objBlog.OwnerUserId)
   Dim colUsers As List(Of UserInfo) = BlogPermissionsController.GetUsersByBlogPermission(portalId, objBlog.BlogID, BlogPermissionTypes.APPROVE).Values.ToList
   If Not colUsers.Contains(objOwner) Then colUsers.Add(objOwner)

   NotificationsController.Instance.SendNotification(objNotification, portalId, Nothing, colUsers)

  End Sub

  ''' <summary>
  ''' Removes any notifications associated w/ a specific blog Post pending approval.
  ''' </summary>
  ''' <param name="blogId"></param>
  ''' <param name="PostId"></param>
  ''' <remarks></remarks>
  Public Shared Sub RemovePostPendingNotification(moduleId As Integer, blogId As Integer, PostId As Integer)
   Dim notificationType As NotificationType = NotificationsController.Instance.GetNotificationType(NotificationPublishingTypeName)
   Dim notificationKey As New NotificationKey(ContentTypeName, moduleId, blogId, PostId, -1)
   Dim objNotify As Notification = NotificationsController.Instance.GetNotificationByContext(notificationType.NotificationTypeId, notificationKey.ToString).SingleOrDefault
   If objNotify IsNot Nothing Then
    NotificationsController.Instance.DeleteAllNotificationRecipients(objNotify.NotificationID)
   End If
  End Sub

  ''' <summary>
  ''' This method will send a core notification to blog owners when a comment is pending approval.
  ''' </summary>
  ''' <param name="objComment"></param>
  ''' <param name="objBlog"></param>
  ''' <param name="objPost"></param>
  ''' <param name="portalId"></param>
  ''' <param name="summary"></param>
  ''' <param name="subject"></param>
  ''' <remarks></remarks>
  Public Shared Sub CommentPendingApproval(objComment As CommentInfo, objBlog As BlogInfo, objPost As PostInfo, portalId As Integer, summary As String, subject As String)
   Dim notificationType As NotificationType = NotificationsController.Instance.GetNotificationType(NotificationCommentApprovalTypeName)

   Dim notificationKey As New NotificationKey(ContentTypeName & NotificationCommentApprovalTypeName, objBlog.ModuleID, objBlog.BlogID, objPost.ContentItemId, objComment.CommentID)
   Dim objNotification As New Notification

   Dim recipientId As Integer
   If objBlog.PublishAsOwner Then
    recipientId = objBlog.OwnerUserId
   Else
    recipientId = objPost.CreatedByUserID
   End If

   objNotification.NotificationTypeID = notificationType.NotificationTypeId
   objNotification.Subject = subject
   objNotification.Body = summary
   objNotification.IncludeDismissAction = True
   objNotification.SenderUserID = objComment.CreatedByUserID
   objNotification.Context = notificationKey.ToString

   Dim objOwner As UserInfo = UserController.GetUserById(portalId, recipientId)
   Dim colUsers As Dictionary(Of String, UserInfo) = BlogPermissionsController.GetUsersByBlogPermission(portalId, objBlog.BlogID, BlogPermissionTypes.APPROVECOMMENT)
   If Not colUsers.ContainsKey(objOwner.Username) Then colUsers.Add(objOwner.Username, objOwner)
   AddNotifications(portalId, colUsers.Values.ToList, objNotification)

  End Sub

  Public Shared Sub ReportComment(objComment As CommentInfo, objBlog As BlogInfo, objPost As PostInfo, portalId As Integer, summary As String, subject As String)
   Dim notificationType As NotificationType = NotificationsController.Instance.GetNotificationType(NotificationCommentReportedTypeName)

   Dim notificationKey As New NotificationKey(ContentTypeName & NotificationCommentReportedTypeName, objBlog.ModuleID, objBlog.BlogID, objPost.ContentItemId, objComment.CommentID)
   Dim objNotification As New Notification

   Dim recipientId As Integer
   If objBlog.PublishAsOwner Then
    recipientId = objBlog.OwnerUserId
   Else
    recipientId = objPost.CreatedByUserID
   End If

   objNotification.NotificationTypeID = notificationType.NotificationTypeId
   objNotification.Subject = subject
   objNotification.Body = summary
   objNotification.IncludeDismissAction = True
   objNotification.SenderUserID = objComment.CreatedByUserID
   objNotification.Context = notificationKey.ToString

   Dim objOwner As UserInfo = UserController.GetUserById(portalId, recipientId)
   Dim colUsers As Dictionary(Of String, UserInfo) = BlogPermissionsController.GetUsersByBlogPermission(portalId, objBlog.BlogID, BlogPermissionTypes.APPROVECOMMENT)
   If Not colUsers.ContainsKey(objOwner.Username) Then colUsers.Add(objOwner.Username, objOwner)
   AddNotifications(portalId, colUsers.Values.ToList, objNotification)

  End Sub
  ''' <summary>
  ''' Removes any notifications associated w/ a specific blog comment pending approval.
  ''' </summary>
  ''' <param name="blogId"></param>
  ''' <param name="PostId"></param>
  ''' <remarks></remarks>
  Public Shared Sub RemoveCommentPendingNotification(moduleId As Integer, blogId As Integer, PostId As Integer, commentId As Integer)
   Dim notificationType As NotificationType = NotificationsController.Instance.GetNotificationType(NotificationCommentApprovalTypeName)
   Dim notificationKey As New NotificationKey(ContentTypeName & NotificationCommentApprovalTypeName, moduleId, blogId, PostId, commentId)
   Dim objNotify As Notification = NotificationsController.Instance.GetNotificationByContext(notificationType.NotificationTypeId, notificationKey.ToString).SingleOrDefault
   If objNotify IsNot Nothing Then
    NotificationsController.Instance.DeleteAllNotificationRecipients(objNotify.NotificationID)
   End If
  End Sub

  ''' <summary>
  ''' This method will send a core notification to blog owners when a comment is added (they can only dismiss this notification)
  ''' </summary>
  ''' <param name="objComment"></param>
  ''' <param name="objPost"></param>
  ''' <param name="objBlog"></param>
  ''' <param name="portalId"></param>
  ''' <param name="summary"></param>
  ''' <param name="subject"></param>
  ''' <remarks></remarks>
  Public Shared Sub CommentAdded(objComment As CommentInfo, objPost As PostInfo, objBlog As BlogInfo, portalId As Integer, summary As String, subject As String)
   Dim notificationType As NotificationType = NotificationsController.Instance.GetNotificationType(NotificationCommentAddedTypeName)

   Dim notificationKey As New NotificationKey(ContentTypeName & NotificationCommentAddedTypeName, objBlog.ModuleID, objBlog.BlogID, objPost.ContentItemId, objComment.CommentID)
   Dim objNotification As New Notification

   Dim recipientId As Integer
   If objBlog.PublishAsOwner Then
    recipientId = objBlog.OwnerUserId
   Else
    recipientId = objPost.CreatedByUserID
   End If

   objNotification.NotificationTypeID = notificationType.NotificationTypeId
   objNotification.Subject = subject
   objNotification.Body = summary
   objNotification.IncludeDismissAction = True
   objNotification.SenderUserID = objComment.CreatedByUserID
   objNotification.Context = notificationKey.ToString

   Dim objOwner As UserInfo = UserController.GetUserById(portalId, recipientId)
   Dim colUsers As New List(Of UserInfo)

   colUsers.Add(objOwner)

   AddNotifications(portalId, colUsers, objNotification)

  End Sub
#End Region

#Region " Private Methods "
  Private Shared Sub AddNotifications(portalId As Integer, colUsers As List(Of UserInfo), objNotification As Notification)
   If colUsers.Count > DotNetNuke.Services.Social.Messaging.Internal.InternalMessagingController.Instance.RecipientLimit(portalId) Then
    For Each u As UserInfo In colUsers
     Dim list As New List(Of UserInfo)
     list.Add(u)
     NotificationsController.Instance.SendNotification(objNotification, portalId, Nothing, list)
    Next
   Else
    NotificationsController.Instance.SendNotification(objNotification, portalId, Nothing, colUsers)
   End If
  End Sub
#End Region

#Region " Install Methods "
  ''' <summary>
  ''' This will create a notification type associated w/ the module and also handle the actions that must be associated with it.
  ''' </summary>
  ''' <remarks>This should only ever run once, during 5.0.0 install (via IUpgradeable)</remarks>
  Friend Shared Sub AddNotificationTypes()
   Dim actions As List(Of NotificationTypeAction) = New List(Of NotificationTypeAction)
   Dim deskModuleId As Integer = DesktopModuleController.GetDesktopModuleByFriendlyName("Blog").DesktopModuleID

   Dim objNotificationType As NotificationType = New NotificationType
   objNotificationType.Name = NotificationPublishingTypeName
   objNotificationType.Description = "Blog module post approval."
   objNotificationType.DesktopModuleId = deskModuleId

   If NotificationsController.Instance.GetNotificationType(objNotificationType.Name) Is Nothing Then
    Dim objAction As New NotificationTypeAction
    objAction.NameResourceKey = "ApprovePost"
    objAction.DescriptionResourceKey = "ApprovePost_Desc"
    objAction.APICall = "DesktopModules/Blog/API/NotificationService/ApprovePost"
    objAction.Order = 1
    actions.Add(objAction)

    objAction = New NotificationTypeAction
    objAction.NameResourceKey = "DeletePost"
    objAction.DescriptionResourceKey = "DeletePost_Desc"
    objAction.APICall = "DesktopModules/Blog/API/NotificationService/DeletePost"
    objAction.ConfirmResourceKey = "DeleteItem"
    objAction.Order = 3
    actions.Add(objAction)

    NotificationsController.Instance.CreateNotificationType(objNotificationType)
    NotificationsController.Instance.SetNotificationTypeActions(actions, objNotificationType.NotificationTypeId)
   End If

   objNotificationType = New NotificationType
   objNotificationType.Name = NotificationCommentApprovalTypeName
   objNotificationType.Description = "Blog module comment approval."
   objNotificationType.DesktopModuleId = deskModuleId

   If NotificationsController.Instance.GetNotificationType(objNotificationType.Name) Is Nothing Then
    actions.Clear()

    Dim objAction As New NotificationTypeAction
    objAction.NameResourceKey = "ApproveComment"
    objAction.DescriptionResourceKey = "ApproveComment_Desc"
    objAction.APICall = "DesktopModules/Blog/API/NotificationService/ApproveComment"
    objAction.Order = 1
    actions.Add(objAction)

    objAction = New NotificationTypeAction
    objAction.NameResourceKey = "DeleteComment"
    objAction.DescriptionResourceKey = "DeleteComment_Desc"
    objAction.APICall = "DesktopModules/Blog/API/NotificationService/DeleteComment"
    objAction.ConfirmResourceKey = "DeleteItem"
    objAction.Order = 3
    actions.Add(objAction)

    NotificationsController.Instance.CreateNotificationType(objNotificationType)
    NotificationsController.Instance.SetNotificationTypeActions(actions, objNotificationType.NotificationTypeId)
   End If

   objNotificationType = New NotificationType
   objNotificationType.Name = NotificationCommentReportedTypeName
   objNotificationType.Description = "Blog module comment reported."
   objNotificationType.DesktopModuleId = deskModuleId

   If NotificationsController.Instance.GetNotificationType(objNotificationType.Name) Is Nothing Then
    actions.Clear()

    Dim objAction As New NotificationTypeAction
    objAction.NameResourceKey = "DeleteComment"
    objAction.DescriptionResourceKey = "DeleteComment_Desc"
    objAction.APICall = "DesktopModules/Blog/API/Comments/Delete"
    objAction.ConfirmResourceKey = "DeleteItem"
    objAction.Order = 1
    actions.Add(objAction)

    NotificationsController.Instance.CreateNotificationType(objNotificationType)
    NotificationsController.Instance.SetNotificationTypeActions(actions, objNotificationType.NotificationTypeId)
   End If

   objNotificationType = New NotificationType
   objNotificationType.Name = NotificationCommentAddedTypeName
   objNotificationType.Description = "Blog module and comments being added."
   objNotificationType.DesktopModuleId = deskModuleId

   If NotificationsController.Instance.GetNotificationType(objNotificationType.Name) Is Nothing Then
    NotificationsController.Instance.CreateNotificationType(objNotificationType)
   End If
  End Sub

#End Region

 End Class

End Namespace