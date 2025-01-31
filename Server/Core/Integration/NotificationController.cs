using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Modules.Blog.Entities.Blogs;
using DotNetNuke.Modules.Blog.Entities.Comments;
using DotNetNuke.Modules.Blog.Entities.Posts;
using static DotNetNuke.Modules.Blog.Integration.Integration;
using DotNetNuke.Modules.Blog.Security.Permissions;
using static DotNetNuke.Modules.Blog.Security.Security;
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
// 

using DotNetNuke.Services.Social.Notifications;

namespace DotNetNuke.Modules.Blog.Integration
{

  public class NotificationController
  {

    #region  Integration Methods 
    /// <summary>
    /// This method will send a core notification to blog owners when a blog Post is pending publishing approval.
    /// </summary>
    /// <param name="objBlog"></param>
    /// <param name="objPost"></param>
    /// <param name="portalId"></param>
    /// <param name="summary"></param>
    /// <param name="title"></param>
    /// <remarks></remarks>
    public static void PostPendingApproval(BlogInfo objBlog, PostInfo objPost, int portalId, string summary, string title)
    {
      var notificationType = Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.GetNotificationType(NotificationPublishingTypeName);

      var notificationKey = new NotificationKey(ContentTypeName, objBlog.ModuleID, objPost.BlogID, objPost.ContentItemId, -1);
      var objNotification = new Notification();

      objNotification.NotificationTypeID = notificationType.NotificationTypeId;
      objNotification.Subject = title;
      objNotification.Body = summary;
      objNotification.IncludeDismissAction = false;
      objNotification.SenderUserID = objPost.CreatedByUserID;
      objNotification.Context = notificationKey.ToString();

      var objOwner = UserController.GetUserById(portalId, objBlog.OwnerUserId);
      var colUsers = BlogPermissionsController.GetUsersByBlogPermission(portalId, objBlog.BlogID, (int)BlogPermissionTypes.APPROVE).Values.ToList();
      if (!colUsers.Contains(objOwner))
        colUsers.Add(objOwner);

      Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.SendNotification(objNotification, portalId, null, colUsers);

    }

    /// <summary>
    /// Removes any notifications associated w/ a specific blog Post pending approval.
    /// </summary>
    /// <param name="blogId"></param>
    /// <param name="PostId"></param>
    /// <remarks></remarks>
    public static void RemovePostPendingNotification(int moduleId, int blogId, int PostId)
    {
      var notificationType = Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.GetNotificationType(NotificationPublishingTypeName);
      var notificationKey = new NotificationKey(ContentTypeName, moduleId, blogId, PostId, -1);
      var objNotify = Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.GetNotificationByContext(notificationType.NotificationTypeId, notificationKey.ToString()).SingleOrDefault();
      if (objNotify is not null)
      {
        Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.DeleteAllNotificationRecipients(objNotify.NotificationID);
      }
    }

    /// <summary>
    /// This method will send a core notification to blog owners when a comment is pending approval.
    /// </summary>
    /// <param name="objComment"></param>
    /// <param name="objBlog"></param>
    /// <param name="objPost"></param>
    /// <param name="portalId"></param>
    /// <param name="summary"></param>
    /// <param name="subject"></param>
    /// <remarks></remarks>
    public static void CommentPendingApproval(CommentInfo objComment, BlogInfo objBlog, PostInfo objPost, int portalId, string summary, string subject)
    {
      var notificationType = Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.GetNotificationType(NotificationCommentApprovalTypeName);

      var notificationKey = new NotificationKey(ContentTypeName + NotificationCommentApprovalTypeName, objBlog.ModuleID, objBlog.BlogID, objPost.ContentItemId, objComment.CommentID);
      var objNotification = new Notification();

      int recipientId;
      if (objBlog.PublishAsOwner)
      {
        recipientId = objBlog.OwnerUserId;
      }
      else
      {
        recipientId = objPost.CreatedByUserID;
      }

      objNotification.NotificationTypeID = notificationType.NotificationTypeId;
      objNotification.Subject = subject;
      objNotification.Body = summary;
      objNotification.IncludeDismissAction = true;
      objNotification.SenderUserID = objComment.CreatedByUserID;
      objNotification.Context = notificationKey.ToString();

      var objOwner = UserController.GetUserById(portalId, recipientId);
      var colUsers = BlogPermissionsController.GetUsersByBlogPermission(portalId, objBlog.BlogID, (int)BlogPermissionTypes.APPROVECOMMENT);
      if (!colUsers.ContainsKey(objOwner.Username))
        colUsers.Add(objOwner.Username, objOwner);
      AddNotifications(portalId, colUsers.Values.ToList(), objNotification);

    }

    public static void ReportComment(CommentInfo objComment, BlogInfo objBlog, PostInfo objPost, int portalId, string summary, string subject)
    {
      var notificationType = Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.GetNotificationType(NotificationCommentReportedTypeName);

      var notificationKey = new NotificationKey(ContentTypeName + NotificationCommentReportedTypeName, objBlog.ModuleID, objBlog.BlogID, objPost.ContentItemId, objComment.CommentID);
      var objNotification = new Notification();

      int recipientId;
      if (objBlog.PublishAsOwner)
      {
        recipientId = objBlog.OwnerUserId;
      }
      else
      {
        recipientId = objPost.CreatedByUserID;
      }

      objNotification.NotificationTypeID = notificationType.NotificationTypeId;
      objNotification.Subject = subject;
      objNotification.Body = summary;
      objNotification.IncludeDismissAction = true;
      objNotification.SenderUserID = objComment.CreatedByUserID;
      objNotification.Context = notificationKey.ToString();

      var objOwner = UserController.GetUserById(portalId, recipientId);
      var colUsers = BlogPermissionsController.GetUsersByBlogPermission(portalId, objBlog.BlogID, (int)BlogPermissionTypes.APPROVECOMMENT);
      if (!colUsers.ContainsKey(objOwner.Username))
        colUsers.Add(objOwner.Username, objOwner);
      AddNotifications(portalId, colUsers.Values.ToList(), objNotification);

    }
    /// <summary>
    /// Removes any notifications associated w/ a specific blog comment pending approval.
    /// </summary>
    /// <param name="blogId"></param>
    /// <param name="PostId"></param>
    /// <remarks></remarks>
    public static void RemoveCommentPendingNotification(int moduleId, int blogId, int PostId, int commentId)
    {
      var notificationType = Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.GetNotificationType(NotificationCommentApprovalTypeName);
      var notificationKey = new NotificationKey(ContentTypeName + NotificationCommentApprovalTypeName, moduleId, blogId, PostId, commentId);
      var objNotify = Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.GetNotificationByContext(notificationType.NotificationTypeId, notificationKey.ToString()).SingleOrDefault();
      if (objNotify is not null)
      {
        Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.DeleteAllNotificationRecipients(objNotify.NotificationID);
      }
    }

    /// <summary>
    /// This method will send a core notification to blog owners when a comment is added (they can only dismiss this notification)
    /// </summary>
    /// <param name="objComment"></param>
    /// <param name="objPost"></param>
    /// <param name="objBlog"></param>
    /// <param name="portalId"></param>
    /// <param name="summary"></param>
    /// <param name="subject"></param>
    /// <remarks></remarks>
    public static void CommentAdded(CommentInfo objComment, PostInfo objPost, BlogInfo objBlog, int portalId, string summary, string subject)
    {
      var notificationType = Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.GetNotificationType(NotificationCommentAddedTypeName);

      var notificationKey = new NotificationKey(ContentTypeName + NotificationCommentAddedTypeName, objBlog.ModuleID, objBlog.BlogID, objPost.ContentItemId, objComment.CommentID);
      var objNotification = new Notification();

      int recipientId;
      if (objBlog.PublishAsOwner)
      {
        recipientId = objBlog.OwnerUserId;
      }
      else
      {
        recipientId = objPost.CreatedByUserID;
      }

      objNotification.NotificationTypeID = notificationType.NotificationTypeId;
      objNotification.Subject = subject;
      objNotification.Body = summary;
      objNotification.IncludeDismissAction = true;
      objNotification.SenderUserID = objComment.CreatedByUserID;
      objNotification.Context = notificationKey.ToString();
      objNotification.SendToast = true;

      var objOwner = UserController.GetUserById(portalId, recipientId);
      var colUsers = new List<UserInfo>();

      colUsers.Add(objOwner);

      AddNotifications(portalId, colUsers, objNotification);

    }
    #endregion

    #region  Private Methods 
    private static void AddNotifications(int portalId, List<UserInfo> colUsers, Notification objNotification)
    {
      if (colUsers.Count > Framework.ServiceLocator<DotNetNuke.Services.Social.Messaging.Internal.IInternalMessagingController, DotNetNuke.Services.Social.Messaging.Internal.InternalMessagingController>.Instance.RecipientLimit(portalId))
      {
        foreach (UserInfo u in colUsers)
        {
          var list = new List<UserInfo>();
          list.Add(u);
          Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.SendNotification(objNotification, portalId, null, list);
        }
      }
      else
      {
        Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.SendNotification(objNotification, portalId, null, colUsers);
      }
    }
    #endregion

    #region  Install Methods 
    /// <summary>
    /// This will create a notification type associated w/ the module and also handle the actions that must be associated with it.
    /// </summary>
    /// <remarks>This should only ever run once, during 5.0.0 install (via IUpgradeable)</remarks>
    internal static void AddNotificationTypes()
    {
      var actions = new List<NotificationTypeAction>();
      int deskModuleId = DesktopModuleController.GetDesktopModuleByFriendlyName("Blog").DesktopModuleID;

      var objNotificationType = new NotificationType();
      objNotificationType.Name = NotificationPublishingTypeName;
      objNotificationType.Description = "Blog module post approval.";
      objNotificationType.DesktopModuleId = deskModuleId;

      if (Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.GetNotificationType(objNotificationType.Name) is null)
      {
        var objAction = new NotificationTypeAction();
        objAction.NameResourceKey = "ApprovePost";
        objAction.DescriptionResourceKey = "ApprovePost_Desc";
        objAction.APICall = "DesktopModules/Blog/API/NotificationService/ApprovePost";
        objAction.Order = 1;
        actions.Add(objAction);

        objAction = new NotificationTypeAction();
        objAction.NameResourceKey = "DeletePost";
        objAction.DescriptionResourceKey = "DeletePost_Desc";
        objAction.APICall = "DesktopModules/Blog/API/NotificationService/DeletePost";
        objAction.ConfirmResourceKey = "DeleteItem";
        objAction.Order = 3;
        actions.Add(objAction);

        Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.CreateNotificationType(objNotificationType);
        Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.SetNotificationTypeActions(actions, objNotificationType.NotificationTypeId);
      }

      objNotificationType = new NotificationType();
      objNotificationType.Name = NotificationCommentApprovalTypeName;
      objNotificationType.Description = "Blog module comment approval.";
      objNotificationType.DesktopModuleId = deskModuleId;

      if (Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.GetNotificationType(objNotificationType.Name) is null)
      {
        actions.Clear();

        var objAction = new NotificationTypeAction();
        objAction.NameResourceKey = "ApproveComment";
        objAction.DescriptionResourceKey = "ApproveComment_Desc";
        objAction.APICall = "DesktopModules/Blog/API/NotificationService/ApproveComment";
        objAction.Order = 1;
        actions.Add(objAction);

        objAction = new NotificationTypeAction();
        objAction.NameResourceKey = "DeleteComment";
        objAction.DescriptionResourceKey = "DeleteComment_Desc";
        objAction.APICall = "DesktopModules/Blog/API/NotificationService/DeleteComment";
        objAction.ConfirmResourceKey = "DeleteItem";
        objAction.Order = 3;
        actions.Add(objAction);

        Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.CreateNotificationType(objNotificationType);
        Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.SetNotificationTypeActions(actions, objNotificationType.NotificationTypeId);
      }

      objNotificationType = new NotificationType();
      objNotificationType.Name = NotificationCommentReportedTypeName;
      objNotificationType.Description = "Blog module comment reported.";
      objNotificationType.DesktopModuleId = deskModuleId;

      if (Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.GetNotificationType(objNotificationType.Name) is null)
      {
        actions.Clear();

        var objAction = new NotificationTypeAction();
        objAction.NameResourceKey = "DeleteComment";
        objAction.DescriptionResourceKey = "DeleteComment_Desc";
        objAction.APICall = "DesktopModules/Blog/API/Comments/Delete";
        objAction.ConfirmResourceKey = "DeleteItem";
        objAction.Order = 1;
        actions.Add(objAction);

        Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.CreateNotificationType(objNotificationType);
        Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.SetNotificationTypeActions(actions, objNotificationType.NotificationTypeId);
      }

      objNotificationType = new NotificationType();
      objNotificationType.Name = NotificationCommentAddedTypeName;
      objNotificationType.Description = "Blog module and comments being added.";
      objNotificationType.DesktopModuleId = deskModuleId;

      if (Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.GetNotificationType(objNotificationType.Name) is null)
      {
        Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.CreateNotificationType(objNotificationType);
      }
    }

    #endregion

  }

}