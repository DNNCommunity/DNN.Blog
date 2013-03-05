Imports System.Web
Imports System.Threading

Imports DotNetNuke.Web.Api
Imports DotNetNuke.Common
Imports DotNetNuke.Security
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Users

Imports DotNetNuke.Modules.Blog.Security
Imports DotNetNuke.Modules.Blog.Entities.Blogs

Namespace Services

#Region " Security Access Levels "
 Public Enum SecurityAccessLevel As Integer
  Anonymous = 0
  Admin = 1
  ViewModule = 2
  EditModule = 3
  AddPost = 4
  EditPost = 5
  ApprovePost = 6
  AddComment = 7
  ApproveComment = 8
 End Enum
#End Region

 Public Class BlogAuthorizeAttribute
  Inherits AuthorizeAttributeBase
  Implements IOverrideDefaultAuthLevel

#Region " Properties "
  Public Property BlogId As Integer = -1
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
   Dim moduleId As Integer = context.ActionContext.Request.FindModuleId
   Dim tabId As Integer = context.ActionContext.Request.FindTabId
   If Not HttpContextSource.Current.Request.IsAuthenticated Then
    UserInfo = New UserInfo
   Else
    Dim portalSettings As DotNetNuke.Entities.Portals.PortalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings()
    UserInfo = UserController.GetCachedUser(portalSettings.PortalId, HttpContextSource.Current.User.Identity.Name)
    If UserInfo Is Nothing Then UserInfo = New UserInfo
   End If
   Dim blog As BlogInfo = BlogsController.GetBlog(BlogId, UserInfo.UserID)
   If blog Is Nothing Then Return False
   Security = New ContextSecurity(moduleId, tabId, blog, UserInfo)

   Select Case AccessLevel
    Case SecurityAccessLevel.Admin
     Return Security.UserIsAdmin
    Case SecurityAccessLevel.ViewModule
     Try
      Return DotNetNuke.Security.Permissions.ModulePermissionController.CanViewModule(context.ActionContext.Request.FindModuleInfo())
     Catch ex As Exception
      Return False
     End Try
    Case SecurityAccessLevel.EditModule
     Try
      Return DotNetNuke.Security.Permissions.ModulePermissionController.HasModulePermission(context.ActionContext.Request.FindModuleInfo().ModulePermissions, "EDIT")
     Catch ex As Exception
      Return False
     End Try
    Case SecurityAccessLevel.AddPost
     Return Security.CanAddEntry
    Case SecurityAccessLevel.EditPost
     Return Security.CanEditEntry
    Case SecurityAccessLevel.ApprovePost
     Return Security.CanApproveEntry
    Case SecurityAccessLevel.AddComment
     Return Security.CanAddComment
    Case SecurityAccessLevel.ApproveComment
     Return Security.CanApproveComment
   End Select

   Return False

  End Function
#End Region

#Region " Private Methods "
#End Region

 End Class
End Namespace
