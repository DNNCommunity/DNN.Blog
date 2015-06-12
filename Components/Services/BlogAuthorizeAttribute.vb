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

Imports System.Web
Imports System.Threading

Imports DotNetNuke.Web.Api
Imports DotNetNuke.Common
Imports DotNetNuke.Entities.Users

Imports DotNetNuke.Modules.Blog.Security
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Services.Social.Notifications

Namespace Services

#Region " Security Access Levels "
 Public Enum SecurityAccessLevel As Integer
  Anonymous = 0
  Admin = 1
  ViewModule = 2
  EditModule = 4
  AddPost = 8
  EditPost = 16
  ApprovePost = 32
  AddComment = 64
  ApproveComment = 128
  Blogger = 256
  Owner = 512
 End Enum
#End Region

 Public Class BlogAuthorizeAttribute
  Inherits AuthorizeAttributeBase
  Implements IOverrideDefaultAuthLevel

#Region " Properties "
  Public Property BlogId As Integer = -1
  Public Property NotificationId As Integer = -1
  Public Property AccessLevel As SecurityAccessLevel
  Public Property UserInfo As UserInfo
  Public Property Security As ContextSecurity
#End Region

#Region " Constructors "
  Public Sub New()
   AccessLevel = SecurityAccessLevel.Admin
  End Sub

  Public Sub New(accessLevel As SecurityAccessLevel)
   Me.AccessLevel = accessLevel
  End Sub
#End Region

#Region " Public Methods "
  Public Overrides Function IsAuthorized(context As Web.Api.AuthFilterContext) As Boolean

   If AccessLevel = SecurityAccessLevel.Anonymous Then Return True ' save time by not going through the code below

   HttpContext.Current.Request.Params.ReadValue("blogId", BlogId)
   HttpContext.Current.Request.Params.ReadValue("NotificationId", NotificationId)
   Dim moduleId As Integer = context.ActionContext.Request.FindModuleId
   Dim tabId As Integer = context.ActionContext.Request.FindTabId
   If Not HttpContextSource.Current.Request.IsAuthenticated Then
    UserInfo = New UserInfo
   Else
    Dim portalSettings As DotNetNuke.Entities.Portals.PortalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings()
    UserInfo = UserController.GetCachedUser(portalSettings.PortalId, HttpContextSource.Current.User.Identity.Name)
    If UserInfo Is Nothing Then UserInfo = New UserInfo
   End If
   If NotificationId > -1 Then
    Dim notify As Notification = NotificationsController.Instance.GetNotification(NotificationId)
    Dim nKey As New Integration.NotificationKey(notify.Context)
    BlogId = nKey.BlogId
    moduleId = nKey.ModuleId
   End If
   Dim blog As BlogInfo = BlogsController.GetBlog(BlogId, UserInfo.UserID, Thread.CurrentThread.CurrentCulture.Name)
   If blog Is Nothing Then Return False
   Security = New ContextSecurity(moduleId, tabId, blog, UserInfo)

   If (AccessLevel And SecurityAccessLevel.Admin) = SecurityAccessLevel.Admin AndAlso Security.UserIsAdmin Then
    Return True
   ElseIf (AccessLevel And SecurityAccessLevel.ViewModule) = SecurityAccessLevel.ViewModule AndAlso DotNetNuke.Security.Permissions.ModulePermissionController.CanViewModule(context.ActionContext.Request.FindModuleInfo()) Then
    Return True
   ElseIf (AccessLevel And SecurityAccessLevel.EditModule) = SecurityAccessLevel.EditModule AndAlso DotNetNuke.Security.Permissions.ModulePermissionController.HasModulePermission(context.ActionContext.Request.FindModuleInfo().ModulePermissions, "EDIT") Then
    Return True
   ElseIf (AccessLevel And SecurityAccessLevel.AddPost) = SecurityAccessLevel.AddPost AndAlso Security.CanAddPost Then
    Return True
   ElseIf (AccessLevel And SecurityAccessLevel.EditPost) = SecurityAccessLevel.EditPost AndAlso Security.CanEditPost Then
    Return True
   ElseIf (AccessLevel And SecurityAccessLevel.ApprovePost) = SecurityAccessLevel.ApprovePost AndAlso Security.CanApprovePost Then
    Return True
   ElseIf (AccessLevel And SecurityAccessLevel.AddComment) = SecurityAccessLevel.AddComment AndAlso Security.CanAddComment Then
    Return True
   ElseIf (AccessLevel And SecurityAccessLevel.ApproveComment) = SecurityAccessLevel.ApproveComment AndAlso Security.CanApproveComment Then
    Return True
   ElseIf (AccessLevel And SecurityAccessLevel.Blogger) = SecurityAccessLevel.Blogger AndAlso Security.IsBlogger Then
    Return True
   ElseIf (AccessLevel And SecurityAccessLevel.Owner) = SecurityAccessLevel.Owner AndAlso Security.IsOwner Then
    Return True
   End If

   Return False

  End Function
#End Region

#Region " Private Methods "
#End Region

 End Class
End Namespace
