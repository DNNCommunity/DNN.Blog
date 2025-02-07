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

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using System;
using System.Collections.Generic;
using System.Data;

namespace DotNetNuke.Modules.Blog.Core.Security.Permissions
{
  public partial class BlogPermissionsController
  {

    #region  Public Methods 
    public static bool HasBlogPermission(BlogPermissionCollection objBlogPermissions, int PermissionId)
    {
      if (BlogUser.GetCurrentUser().IsAdministrator)
        return true;
      var m = objBlogPermissions;
      int i;
      var loopTo = m.Count - 1;
      for (i = 0; i <= loopTo; i++)
      {
        BlogPermissionInfo mp;
        mp = m[i];
        if (mp.PermissionId == PermissionId && DotNetNuke.Security.PortalSecurity.IsInRoles(mp.RoleName) | (Framework.ServiceLocator<IUserController, UserController>.Instance.GetCurrentUserInfo().Username ?? "") == (mp.Username ?? "") & mp.UserId != Security.glbUserNothing && mp.Expires > DateTime.Now | mp.Expires == DateTime.MinValue)
        {
          return true;
        }
      }
      return false;
    }

    public static bool HasBlogPermission(BlogPermissionCollection objBlogPermissions, string PermissionKey)
    {
      if (BlogUser.GetCurrentUser().IsAdministrator)
        return true;
      var m = objBlogPermissions;
      int i;
      var loopTo = m.Count - 1;
      for (i = 0; i <= loopTo; i++)
      {
        BlogPermissionInfo mp;
        mp = m[i];
        if ((mp.PermissionKey ?? "") == (PermissionKey ?? "") && DotNetNuke.Security.PortalSecurity.IsInRoles(mp.RoleName) | (Framework.ServiceLocator<IUserController, UserController>.Instance.GetCurrentUserInfo().Username ?? "") == (mp.Username ?? "") & mp.UserId != Security.glbUserNothing && mp.Expires > DateTime.Now | mp.Expires == DateTime.MinValue)
        {
          return true;
        }
      }
      return false;
    }

    public static bool HasBlogPermission(BlogPermissionCollection objBlogPermissions, string PermissionKey, UserInfo User)
    {
      if (BlogUser.GetUser(User).IsAdministrator)
        return true;
      var m = objBlogPermissions;
      int i;
      var loopTo = m.Count - 1;
      for (i = 0; i <= loopTo; i++)
      {
        BlogPermissionInfo mp;
        mp = m[i];
        if ((mp.PermissionKey ?? "") == (PermissionKey ?? "") && User.IsInRole(mp.RoleName) | (User.Username ?? "") == (mp.Username ?? "") & mp.UserId != Security.glbUserNothing && mp.Expires > DateTime.Now | mp.Expires == DateTime.MinValue)
        {
          return true;
        }
      }
      return false;
    }

    public static BlogPermissionCollection GetBlogPermissionsCollection(int BlogId)
    {

      var epc = new BlogPermissionCollection();
      if (BlogId > 0)
      {
        using (var ir = Data.DataProvider.Instance().GetBlogPermissionsByBlog(BlogId))
        {
          while (ir.Read())
            epc.Add(FillBlogPermissionInfo(ir, false, false));
        }
      }
      return epc;

    }

    public static void DeleteBlogPermissionsByBlogId(int BlogId)
    {

      Data.DataProvider.Instance().DeleteBlogPermissions(BlogId);

    }

    public static void UpdateBlogPermissions(BlogInfo Blog)
    {
      DeleteBlogPermissionsByBlogId(Blog.BlogID);
      foreach (BlogPermissionInfo objBlogPermission in Blog.Permissions)
      {
        objBlogPermission.BlogId = Blog.BlogID;
        if (objBlogPermission.AllowAccess)
        {
          try
          {
            AddBlogPermission(objBlogPermission);
          }
          catch
          {
          }
        }
      }
    }

    public static Dictionary<string, UserInfo> GetUsersByBlogPermission(int portalId, int blogId, int permissionId)
    {
      return CBO.FillDictionary<string, UserInfo>("username", Data.DataProvider.Instance().GetUsersByBlogPermission(portalId, blogId, permissionId));
    }
    public static Dictionary<string, UserInfo> GetUsersByBlogPermission(int portalId, int blogId, string permission)
    {
      return CBO.FillDictionary<string, UserInfo>("username", Data.DataProvider.Instance().GetUsersByBlogPermission(portalId, blogId, GetPermissionId(permission)));
    }

    public static int GetPermissionId(string permission)
    {
      switch (permission.ToUpper() ?? "")
      {
        case "ADD":
          {
            return (int)BlogPermissionTypes.ADD;
          }
        case "ADDCOMMENT":
          {
            return (int)BlogPermissionTypes.ADDCOMMENT;
          }
        case "APPROVE":
          {
            return (int)BlogPermissionTypes.APPROVE;
          }
        case "APPROVECOMMENT":
          {
            return (int)BlogPermissionTypes.APPROVECOMMENT;
          }
        case "AUTOAPPROVECOMMENT":
          {
            return (int)BlogPermissionTypes.AUTOAPPROVECOMMENT;
          }
        case "EDIT":
          {
            return (int)BlogPermissionTypes.EDIT;
          }

        default:
          {
            return -1;
          }
      }
    }
    #endregion

    #region  Fill Methods 
    private static BlogPermissionInfo FillBlogPermissionInfo(IDataReader dr)
    {
      return FillBlogPermissionInfo(dr, true, true);
    }

    private static BlogPermissionInfo FillBlogPermissionInfo(IDataReader dr, bool CheckForOpenDataReader, bool CloseReader)
    {

      var objBlogPermission = new BlogPermissionInfo();
      bool canContinue = true;
      if (CheckForOpenDataReader)
      {
        canContinue = false;
        if (dr.Read())
        {
          canContinue = true;
        }
      }
      if (canContinue)
      {
        {
          ref var withBlock = ref objBlogPermission;
          withBlock.AllowAccess = Convert.ToBoolean(Null.SetNull(dr["AllowAccess"], objBlogPermission.AllowAccess));
          withBlock.DisplayName = Convert.ToString(Null.SetNull(dr["DisplayName"], objBlogPermission.DisplayName));
          withBlock.BlogId = Convert.ToInt32(Null.SetNull(dr["BlogId"], objBlogPermission.BlogId));
          withBlock.Expires = Convert.ToDateTime(Null.SetNull(dr["Expires"], objBlogPermission.Expires));
          withBlock.PermissionId = Convert.ToInt32(Null.SetNull(dr["PermissionId"], objBlogPermission.PermissionId));
          switch (withBlock.PermissionId)
          {
            case (int)BlogPermissionTypes.ADD:
              {
                withBlock.PermissionKey = "ADD";
                break;
              }
            case (int)BlogPermissionTypes.EDIT:
              {
                withBlock.PermissionKey = "EDIT";
                break;
              }
            case (int)BlogPermissionTypes.APPROVE:
              {
                withBlock.PermissionKey = "APPROVE";
                break;
              }
            case (int)BlogPermissionTypes.ADDCOMMENT:
              {
                withBlock.PermissionKey = "ADDCOMMENT";
                break;
              }
            case (int)BlogPermissionTypes.APPROVECOMMENT:
              {
                withBlock.PermissionKey = "APPROVECOMMENT";
                break;
              }
            case (int)BlogPermissionTypes.AUTOAPPROVECOMMENT:
              {
                withBlock.PermissionKey = "AUTOAPPROVECOMMENT";
                break;
              }
          }
          withBlock.RoleId = Convert.ToInt32(Null.SetNull(dr["RoleID"], objBlogPermission.RoleId));
          if (withBlock.RoleId > -1)
          {
            string rn = Convert.ToString(Null.SetNull(dr["RoleName"], ""));
            withBlock.RoleName = rn;
          }
          withBlock.UserId = Convert.ToInt32(Null.SetNull(dr["UserID"], objBlogPermission.UserId));
          withBlock.Username = Convert.ToString(Null.SetNull(dr["UserName"], objBlogPermission.Username));
        }
      }
      else
      {
        objBlogPermission = null;
      }

      if (CloseReader)
      {
        dr.Close();
      }

      return objBlogPermission;

    }

    #endregion

  }



}