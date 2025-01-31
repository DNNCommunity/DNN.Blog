using System;
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

using DotNetNuke.Entities.Users;

namespace DotNetNuke.Modules.Blog.Security
{

  /// <summary>
 /// This class makes the UserInfo more robust and adds the much missed IsAdministrator to this 
 /// object so we don't have to keep on looking this up.
 /// </summary>
 /// <remarks></remarks>
  [Serializable()]
  public class BlogUser : UserInfo
  {

    #region  Private Members 
    #endregion

    #region  Constructors 
    public BlogUser()
    {
      LoadUser(null);
    }

    public BlogUser(ref UserInfo user)
    {
      LoadUser(user);
    }

    private void LoadUser(UserInfo user)
    {
      if (user is null)
      {
        user = Framework.ServiceLocator<IUserController, UserController>.Instance.GetCurrentUserInfo();
      }

      AffiliateID = user.AffiliateID;
      DisplayName = Common.Globals.GetAString(user.DisplayName);
      Email = Common.Globals.GetAString(user.Email);
      FirstName = Common.Globals.GetAString(user.FirstName);
      IsSuperUser = user.IsSuperUser;
      LastName = Common.Globals.GetAString(user.LastName);
      Membership = user.Membership;
      PortalID = user.PortalID;
      Profile = user.Profile;
      Roles = user.Roles;
      UserID = user.UserID;
      UserID = user.UserID;
      Username = Common.Globals.GetAString(user.Username);

      if (IsSuperUser)
      {
        IsAdministrator = true;
      }
      else if (UserID > -1)
      {
        var objPortals = new DotNetNuke.Entities.Portals.PortalController();
        var objPortal = objPortals.GetPortal(PortalID);
        if (IsInRole(objPortal.AdministratorRoleName))
        {
          IsAdministrator = true;
        }
      }

      if (UserID == -1)
      {
        try
        {
          PortalID = Framework.ServiceLocator<DotNetNuke.Entities.Portals.IPortalController, DotNetNuke.Entities.Portals.PortalController>.Instance.GetCurrentPortalSettings().PortalId;
        }
        catch (Exception ex)
        {
          return;
        }
      }

    }
    #endregion

    #region  Public Properties 
    public bool IsAdministrator { get; set; } = false;
    #endregion

    #region  Public Shared Methods 
    public static BlogUser GetCurrentUser()
    {
      var dnnUser = Framework.ServiceLocator<IUserController, UserController>.Instance.GetCurrentUserInfo();
      string cacheKey = string.Format("BlogUser{0}-{1}", dnnUser.PortalID, dnnUser.UserID);
      BlogUser du = null;
      try
      {
        du = (BlogUser)DotNetNuke.Common.Utilities.DataCache.GetCache(cacheKey);
      }
      catch
      {
      }
      if (du is null)
      {
        du = new BlogUser(ref dnnUser);
        DotNetNuke.Common.Utilities.DataCache.SetCache(cacheKey, du);
      }
      return du;
    }

    public static BlogUser GetUser(int portalId, int userId)
    {
      string cacheKey = string.Format("BlogUser{0}-{1}", portalId, userId);
      BlogUser du = null;
      try
      {
        du = (BlogUser)DotNetNuke.Common.Utilities.DataCache.GetCache(cacheKey);
      }
      catch
      {
      }
      if (du is null)
      {
        var dnnUser = UserController.GetUserById(portalId, userId);
        if (dnnUser is null)
        {
          dnnUser = new UserInfo();
          dnnUser.PortalID = portalId;
        }
        du = new BlogUser(ref dnnUser);
        DotNetNuke.Common.Utilities.DataCache.SetCache(cacheKey, du);
      }
      return du;
    }

    public static BlogUser GetUser(UserInfo user)
    {
      string cacheKey = string.Format("BlogUser{0}-{1}", user.PortalID, user.UserID);
      BlogUser du = null;
      try
      {
        du = (BlogUser)DotNetNuke.Common.Utilities.DataCache.GetCache(cacheKey);
      }
      catch
      {
      }
      if (du is null)
      {
        du = new BlogUser(ref user);
        DotNetNuke.Common.Utilities.DataCache.SetCache(cacheKey, du);
      }
      return du;
    }
    #endregion

  }
}