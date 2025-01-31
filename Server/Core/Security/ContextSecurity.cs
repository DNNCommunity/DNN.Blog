using DotNetNuke.Entities.Users;
using static DotNetNuke.Modules.Blog.Common.Globals;
using DotNetNuke.Modules.Blog.Entities.Blogs;
using static DotNetNuke.Modules.Blog.Security.Security;
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

using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Tokens;
using Microsoft.VisualBasic.CompilerServices;

namespace DotNetNuke.Modules.Blog.Security
{

  public class ContextSecurity : IPropertyAccess
  {

    #region  Private Members 
    private bool _canAdd = false;
    private bool _canEdit = false;
    private bool _canApprove = false;
    private bool _canAddComment = false;
    private bool _canApproveComment = false;
    private bool _canAutoApproveComment = false;
    private bool _canViewComment = false;
    private bool _userIsAdmin = false;
    private bool _isBlogger = false;
    private bool _isEditor = false;
    private int _userId = -1;
    #endregion

    #region  Constructor 
    public ContextSecurity(int moduleId, int tabId, BlogInfo blog, UserInfo user)
    {
      _userId = user.UserID;
      if (blog is not null)
      {
        IsOwner = blog.CreatedByUserID == user.UserID;
        _canAdd = blog.CanAdd;
        _canEdit = blog.CanEdit;
        _canApprove = blog.CanApprove;
        _canViewComment = blog.Permissions.CurrentUserHasPermission("VIEWCOMMENT");
        _canApproveComment = blog.Permissions.CurrentUserHasPermission("APPROVECOMMENT");
        _canAutoApproveComment = blog.Permissions.CurrentUserHasPermission("AUTOAPPROVECOMMENT");
        _canAddComment = blog.Permissions.CurrentUserHasPermission("ADDCOMMENT");
      }
      else
      {
        using (var ir = Data.DataProvider.Instance().GetUserPermissionsByModule(moduleId, user.UserID))
        {
          while (ir.Read())
          {
            int permissionId = Conversions.ToInteger(ir["PermissionId"]);
            int hasPermission = Conversions.ToInteger(ir["HasPermission"]);
            if (hasPermission > 0)
            {
              switch (permissionId)
              {
                case (int)BlogPermissionTypes.ADD:
                  {
                    _canAdd = true;
                    break;
                  }
                case (int)BlogPermissionTypes.EDIT:
                  {
                    _canEdit = true;
                    break;
                  }
                case (int)BlogPermissionTypes.APPROVE:
                  {
                    _canApprove = true;
                    break;
                  }
              }
            }
          }
        }
      }
      LoggedIn = user.UserID > -1;
      _userIsAdmin = DotNetNuke.Security.PortalSecurity.IsInRole(DotNetNuke.Entities.Portals.PortalSettings.Current.AdministratorRoleName);
      var mc = new DotNetNuke.Entities.Modules.ModuleController();
      var objMod = new DotNetNuke.Entities.Modules.ModuleInfo();
      objMod = mc.GetModule(moduleId, tabId, false);
      if (objMod is not null)
      {
        _isBlogger = ModulePermissionController.HasModulePermission(objMod.ModulePermissions, BloggerPermission);
        _isEditor = ModulePermissionController.HasModulePermission(objMod.ModulePermissions, "EDIT");
      }
    }
    #endregion

    #region  Public Properties 
    public bool IsOwner { get; set; } = false;
    public bool LoggedIn { get; set; } = false;

    public bool CanEditPost
    {
      get
      {
        return _canEdit | IsOwner | UserIsAdmin;
      }
    }
    public bool CanEditThisPost(Entities.Posts.PostInfo post)
    {
      if (CanEditPost)
        return true;
      if (post is null)
        return false;
      if (post.Blog.MustApproveGhostPosts && !CanApprovePost)
      {
        if (post.CreatedByUserID == _userId & !post.Published)
          return true;
      }
      else if (post.CreatedByUserID == _userId)
        return true;
      return false;
    }

    public bool CanAddPost
    {
      get
      {
        return _canAdd | IsOwner | UserIsAdmin;
      }
    }

    public bool CanAddComment
    {
      get
      {
        return _canAddComment | IsOwner | UserIsAdmin;
      }
    }

    public bool CanViewComments
    {
      get
      {
        return _canViewComment | IsOwner | UserIsAdmin;
      }
    }

    public bool CanApprovePost
    {
      get
      {
        return _canApprove | IsOwner | UserIsAdmin;
      }
    }

    public bool CanApproveComment
    {
      get
      {
        return _canApproveComment | IsOwner | UserIsAdmin;
      }
    }

    public bool CanAutoApproveComment
    {
      get
      {
        return _canAutoApproveComment | IsOwner | UserIsAdmin;
      }
    }

    public bool CanDoSomethingWithPosts
    {
      get
      {
        return _canEdit | _canAdd | _canApprove | _isBlogger | UserIsAdmin;
      }
    }

    public bool UserIsAdmin
    {
      get
      {
        return _userIsAdmin;
      }
    }

    public bool IsBlogger
    {
      get
      {
        return _isBlogger;
      }
    }

    public bool IsEditor
    {
      get
      {
        return _isEditor;
      }
    }
    #endregion

    #region  IPropertyAccess Implementation 
    public string GetProperty(string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, UserInfo AccessingUser, Scope AccessLevel, ref bool PropertyNotFound)
    {
      string OutputFormat = string.Empty;
      var portalSettings = Framework.ServiceLocator<DotNetNuke.Entities.Portals.IPortalController, DotNetNuke.Entities.Portals.PortalController>.Instance.GetCurrentPortalSettings();
      if (string.IsNullOrEmpty(strFormat))
      {
        OutputFormat = "D";
      }
      else
      {
        OutputFormat = strFormat;
      }
      switch (strPropertyName.ToLower() ?? "")
      {
        case "isowner":
          {
            return IsOwner.ToString();
          }
        case "isowneryesno":
          {
            return PropertyAccess.Boolean2LocalizedYesNo(IsOwner, formatProvider);
          }
        case "caneditpost":
          {
            return CanEditPost.ToString();
          }
        case "caneditpostyesno":
          {
            return PropertyAccess.Boolean2LocalizedYesNo(CanEditPost, formatProvider);
          }
        case "canaddpost":
          {
            return CanAddPost.ToString();
          }
        case "canaddpostyesno":
          {
            return PropertyAccess.Boolean2LocalizedYesNo(CanAddPost, formatProvider);
          }
        case "canaddcomment":
          {
            return CanAddComment.ToString();
          }
        case "canaddcommentyesno":
          {
            return PropertyAccess.Boolean2LocalizedYesNo(CanAddComment, formatProvider);
          }
        case "canviewcomments":
          {
            return CanViewComments.ToString();
          }
        case "canviewcommentsyesno":
          {
            return PropertyAccess.Boolean2LocalizedYesNo(CanViewComments, formatProvider);
          }
        case "canapprovepost":
          {
            return CanApprovePost.ToString();
          }
        case "canapprovepostyesno":
          {
            return PropertyAccess.Boolean2LocalizedYesNo(CanApprovePost, formatProvider);
          }
        case "canapprovecomment":
          {
            return CanApproveComment.ToString();
          }
        case "canapprovecommentyesno":
          {
            return PropertyAccess.Boolean2LocalizedYesNo(CanApproveComment, formatProvider);
          }
        case "canautoapprovecomment":
          {
            return CanAutoApproveComment.ToString();
          }
        case "canautoapprovecommentyesno":
          {
            return PropertyAccess.Boolean2LocalizedYesNo(CanAutoApproveComment, formatProvider);
          }
        case "candosomethingwithposts":
          {
            return CanDoSomethingWithPosts.ToString();
          }
        case "candosomethingwithpostsyesno":
          {
            return PropertyAccess.Boolean2LocalizedYesNo(CanDoSomethingWithPosts, formatProvider);
          }
        case "userisadmin":
          {
            return UserIsAdmin.ToString();
          }
        case "userisadminyesno":
          {
            return PropertyAccess.Boolean2LocalizedYesNo(UserIsAdmin, formatProvider);
          }
        case "isblogger":
          {
            return IsBlogger.ToString();
          }
        case "isbloggeryesno":
          {
            return PropertyAccess.Boolean2LocalizedYesNo(IsBlogger, formatProvider);
          }
        case "iseditor":
          {
            return IsEditor.ToString();
          }
        case "iseditoryesno":
          {
            return PropertyAccess.Boolean2LocalizedYesNo(IsEditor, formatProvider);
          }
        case "loggedin":
          {
            return LoggedIn.ToString();
          }
        case "loggedinyesno":
          {
            return PropertyAccess.Boolean2LocalizedYesNo(LoggedIn, formatProvider);
          }

        default:
          {
            PropertyNotFound = true;
            break;
          }
      }
      return DotNetNuke.Common.Utilities.Null.NullString;
    }

    public CacheLevel Cacheability
    {
      get
      {
        return CacheLevel.fullyCacheable;
      }
    }
    #endregion

  }
}