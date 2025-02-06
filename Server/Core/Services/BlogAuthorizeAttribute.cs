
// 
// DNN Connect - http://dnn-connect.org
// Copyright (c) 2015
// by DNN Connect
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

using System.Threading;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Entities.Users;
using DotNetNuke.Modules.Blog.Core.Common;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using DotNetNuke.Modules.Blog.Core.Integration;
using DotNetNuke.Modules.Blog.Core.Security;
using DotNetNuke.Services.Social.Notifications;

using DotNetNuke.Web.Api;

namespace DotNetNuke.Modules.Blog.Core.Services
{

  #region  Security Access Levels 
  public enum SecurityAccessLevel : int
  {
    Anonymous = 0,
    Admin = 1,
    ViewModule = 2,
    EditModule = 4,
    AddPost = 8,
    EditPost = 16,
    ApprovePost = 32,
    AddComment = 64,
    ApproveComment = 128,
    Blogger = 256,
    Owner = 512
  }
  #endregion

  public class BlogAuthorizeAttribute : AuthorizeAttributeBase, IOverrideDefaultAuthLevel
  {

    #region  Properties 
    public int BlogId { get; set; } = -1;
    public int NotificationId { get; set; } = -1;
    public SecurityAccessLevel AccessLevel { get; set; }
    public UserInfo UserInfo { get; set; }
    public ContextSecurity Security { get; set; }
    #endregion

    #region  Constructors 
    public BlogAuthorizeAttribute()
    {
      AccessLevel = SecurityAccessLevel.Admin;
    }

    public BlogAuthorizeAttribute(SecurityAccessLevel accessLevel)
    {
      AccessLevel = accessLevel;
    }
    #endregion

    #region  Public Methods 
    public override bool IsAuthorized(AuthFilterContext context)
    {

      if (AccessLevel == SecurityAccessLevel.Anonymous)
        return true; // save time by not going through the code below

      BlogId = HttpContext.Current.Request.Params.ReadValue("BlogId", BlogId);
      NotificationId = HttpContext.Current.Request.Params.ReadValue("NotificationId", NotificationId);

      int moduleId = context.ActionContext.Request.FindModuleId();
      int tabId = context.ActionContext.Request.FindTabId();
      if (!HttpContextSource.Current.Request.IsAuthenticated)
      {
        UserInfo = new UserInfo();
      }
      else
      {
        var portalSettings = Framework.ServiceLocator<DotNetNuke.Entities.Portals.IPortalController, DotNetNuke.Entities.Portals.PortalController>.Instance.GetCurrentPortalSettings();
        UserInfo = UserController.GetCachedUser(portalSettings.PortalId, HttpContextSource.Current.User.Identity.Name);
        if (UserInfo is null)
          UserInfo = new UserInfo();
      }
      if (NotificationId > -1)
      {
        var notify = Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.GetNotification(NotificationId);
        var nKey = new NotificationKey(notify.Context);
        BlogId = nKey.BlogId;
        moduleId = nKey.ModuleId;
      }
      var blog = BlogsController.GetBlog(BlogId, UserInfo.UserID, Thread.CurrentThread.CurrentCulture.Name);
      if (blog is null)
        return false;
      Security = new ContextSecurity(moduleId, tabId, blog, UserInfo);

      if ((AccessLevel & SecurityAccessLevel.Admin) == SecurityAccessLevel.Admin && Security.UserIsAdmin)
      {
        return true;
      }
      else if ((AccessLevel & SecurityAccessLevel.ViewModule) == SecurityAccessLevel.ViewModule && DotNetNuke.Security.Permissions.ModulePermissionController.CanViewModule(context.ActionContext.Request.FindModuleInfo()))
      {
        return true;
      }
      else if ((AccessLevel & SecurityAccessLevel.EditModule) == SecurityAccessLevel.EditModule && DotNetNuke.Security.Permissions.ModulePermissionController.HasModulePermission(context.ActionContext.Request.FindModuleInfo().ModulePermissions, "EDIT"))
      {
        return true;
      }
      else if ((AccessLevel & SecurityAccessLevel.AddPost) == SecurityAccessLevel.AddPost && Security.CanAddPost)
      {
        return true;
      }
      else if ((AccessLevel & SecurityAccessLevel.EditPost) == SecurityAccessLevel.EditPost && Security.CanEditPost)
      {
        return true;
      }
      else if ((AccessLevel & SecurityAccessLevel.ApprovePost) == SecurityAccessLevel.ApprovePost && Security.CanApprovePost)
      {
        return true;
      }
      else if ((AccessLevel & SecurityAccessLevel.AddComment) == SecurityAccessLevel.AddComment && Security.CanAddComment)
      {
        return true;
      }
      else if ((AccessLevel & SecurityAccessLevel.ApproveComment) == SecurityAccessLevel.ApproveComment && Security.CanApproveComment)
      {
        return true;
      }
      else if ((AccessLevel & SecurityAccessLevel.Blogger) == SecurityAccessLevel.Blogger && Security.IsBlogger)
      {
        return true;
      }
      else if ((AccessLevel & SecurityAccessLevel.Owner) == SecurityAccessLevel.Owner && Security.IsOwner)
      {
        return true;
      }

      return false;

    }
    #endregion

    #region  Private Methods 
    #endregion

  }
}