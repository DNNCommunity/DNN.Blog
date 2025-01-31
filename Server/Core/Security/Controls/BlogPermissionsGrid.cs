using System;
using System.Collections;
using System.Text;
using System.Web.UI;
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
using DotNetNuke.Modules.Blog.Security.Permissions;
using static DotNetNuke.Modules.Blog.Security.Security;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace DotNetNuke.Modules.Blog.Security.Controls
{

  public class BlogPermissionsGrid : PermissionsGrid
  {

    #region  Private Members 

    private bool _InheritViewPermissionsFromParent = false;
    private int _BlogID = -1;
    private int _TabId = -1;
    private BlogPermissionCollection _BlogPermissions;
    private int _ViewColumnIndex;
    private string _BlogType;
    private int _currentUserId = -1;
    private bool _userIsAdmin = false;

    #endregion

    #region  Public Properties 
    public bool UserIsAdmin
    {
      get
      {
        return _userIsAdmin;
      }
      set
      {
        _userIsAdmin = value;
      }
    }

    public int CurrentUserId
    {
      get
      {
        return _currentUserId;
      }
      set
      {
        _currentUserId = value;
      }
    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Gets and Sets the Id of the Blog
  /// </summary>
  /// <history>
  ///     [cnurse]    01/09/2006  Documented
  /// </history>
  /// -----------------------------------------------------------------------------
    public int BlogID
    {
      get
      {
        return _BlogID;
      }
      set
      {
        _BlogID = value;
        if (!Page.IsPostBack)
        {
          GetBlogPermissions();
        }
      }
    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Gets and Sets the Id of the Tab associated with this Blog
  /// </summary>
  /// <history>
  ///     [cnurse]    24/11/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    public int TabId
    {
      get
      {
        return _TabId;
      }
      set
      {
        _TabId = value;
      }
    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Gets the BlogPermission Collection
  /// </summary>
  /// <history>
  ///     [cnurse]    01/09/2006  Documented
  /// </history>
  /// -----------------------------------------------------------------------------
    public BlogPermissionCollection Permissions
    {
      get
      {
        // First Update Permissions in case they have been changed
        UpdatePermissions();

        // Return the BlogPermissions
        return _BlogPermissions;

      }
      set
      {
        _BlogPermissions = value;
      }
    }
    #endregion

    #region  Private Methods 

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Gets the BlogPermissions from the Data Store
  /// </summary>
  /// <history>
  /// </history>
  /// -----------------------------------------------------------------------------
    private void GetBlogPermissions()
    {

      _BlogPermissions = BlogPermissionsController.GetBlogPermissionsCollection(BlogID);

    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Parse the Permission Keys used to persist the Permissions in the ViewState
  /// </summary>
  /// <param name="Settings">A string array of settings</param>
  /// <param name="arrPermisions">An Arraylist to add the Permission object to</param>
  /// <history>
  /// </history>
  /// -----------------------------------------------------------------------------
    private void ParsePermissionKeys(string[] Settings, ArrayList arrPermisions)
    {

      var objBlogPermission = new BlogPermissionInfo();
      var permission = PermissionsController.GetPermission(Convert.ToInt32(Settings[1]));
      objBlogPermission.PermissionId = permission.PermissionId;
      objBlogPermission.PermissionKey = permission.PermissionKey;
      objBlogPermission.RoleId = Convert.ToInt32(Settings[4]);

      objBlogPermission.RoleName = Settings[3];
      objBlogPermission.AllowAccess = Conversions.ToBoolean(Settings[0]);
      objBlogPermission.UserId = Convert.ToInt32(Settings[5]);
      objBlogPermission.DisplayName = Settings[6];

      objBlogPermission.BlogId = BlogID;
      arrPermisions.Add(objBlogPermission);

    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Check if the Role has the permission specified
  /// </summary>
  /// <param name="permissionID">The Id of the Permission to check</param>
  /// <param name="roleid">The role id to check</param>
  /// <history>
  ///     [cnurse]    01/09/2006  Documented
  /// </history>
  /// -----------------------------------------------------------------------------
    private BlogPermissionInfo BlogHasRolePermission(int permissionID, int roleid)
    {
      int i;
      var loopTo = _BlogPermissions.Count - 1;
      for (i = 0; i <= loopTo; i++)
      {
        var objBlogPermission = _BlogPermissions[i];
        if (permissionID == objBlogPermission.PermissionId & objBlogPermission.RoleId == roleid)
        {
          return objBlogPermission;
        }
      }
      return null;
    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Check if the Role has the permission specified
  /// </summary>
  /// <param name="permissionID">The Id of the Permission to check</param>
  /// <param name="userid">The user id to check</param>
  /// <history>
  ///     [cnurse]    01/09/2006  Documented
  /// </history>
  /// -----------------------------------------------------------------------------
    private BlogPermissionInfo BlogHasUserPermission(int permissionID, int userid)
    {
      int i;
      var loopTo = _BlogPermissions.Count - 1;
      for (i = 0; i <= loopTo; i++)
      {
        var objBlogPermission = _BlogPermissions[i];
        if (permissionID == objBlogPermission.PermissionId & objBlogPermission.UserId == userid)
        {
          return objBlogPermission;
        }
      }
      return null;
    }

    #endregion

    #region  Protected Methods 

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Gets the Enabled status of the permission
  /// </summary>
  /// <param name="objPerm">The permission being loaded</param>
  /// <param name="role">The role</param>
  /// <param name="column">The column of the Grid</param>
  /// <history>
  ///     [cnurse]    01/13/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    protected override bool GetEnabled(PermissionInfo objPerm, DotNetNuke.Security.Roles.RoleInfo role, int column)
    {

      if (role.RoleID == AdministratorRoleId)
      {
        return false;
      }
      else if (role.RoleID == -1) // all users
      {
        if (objPerm.PermissionId == (int)BlogPermissionTypes.ADD | objPerm.PermissionId == (int)BlogPermissionTypes.APPROVE | objPerm.PermissionId == (int)BlogPermissionTypes.EDIT)
        {
          return false;
        }
        else
        {
          return true;
        }
      }
      else
      {
        return true;
      }

    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Gets the Enabled status of the permission
  /// </summary>
  /// <param name="objPerm">The permission being loaded</param>
  /// <param name="user">The user</param>
  /// <param name="column">The column of the Grid</param>
  /// <history>
  ///     [cnurse]    01/13/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    protected override bool GetEnabled(PermissionInfo objPerm, UserInfo user, int column)
    {

      return true;

    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Gets the Value of the permission
  /// </summary>
  /// <param name="objPerm">The permission being loaded</param>
  /// <param name="role">The role</param>
  /// <param name="column">The column of the Grid</param>
  /// <returns>A Boolean (True or False)</returns>
  /// <history>
  ///     [cnurse]    01/09/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    protected override bool GetPermission(PermissionInfo objPerm, DotNetNuke.Security.Roles.RoleInfo role, int column)
    {

      bool permission;

      var objBlogPermission = BlogHasRolePermission(objPerm.PermissionId, role.RoleID);
      if (objBlogPermission is not null)
      {
        permission = objBlogPermission.AllowAccess;
      }
      else
      {
        permission = false;
      }

      return permission;

    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Gets the Value of the permission
  /// </summary>
  /// <param name="objPerm">The permission being loaded</param>
  /// <param name="user">The role</param>
  /// <param name="column">The column of the Grid</param>
  /// <returns>A Boolean (True or False)</returns>
  /// <history>
  ///     [cnurse]    01/09/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    protected override bool GetPermission(PermissionInfo objPerm, UserInfo user, int column)
    {

      bool permission;

      var objBlogPermission = BlogHasUserPermission(objPerm.PermissionId, user.UserID);
      if (objBlogPermission is not null)
      {
        permission = objBlogPermission.AllowAccess;
      }
      else
      {
        permission = false;
      }

      return permission;

    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Gets the Permissions from the Data Store
  /// </summary>
  /// <history>
  ///     [cnurse]    01/09/2006  Documented
  /// </history>
  /// -----------------------------------------------------------------------------
    protected override PermissionCollection GetPermissions()
    {

      _ViewColumnIndex = 0;
      return PermissionsController.GetPermissions();

    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Gets the users from the Database
  /// </summary>
  /// <history>
  ///     [cnurse]    01/12/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    protected override ArrayList GetUsers()
    {

      var arrUsers = new ArrayList();
      UserInfo objUser;
      bool blnExists;

      foreach (var objBlogPermission in _BlogPermissions)
      {
        if (!(objBlogPermission.UserId == glbUserNothing))
        {
          blnExists = false;
          foreach (UserInfo currentObjUser in arrUsers)
          {
            objUser = currentObjUser;
            if (objBlogPermission.UserId == objUser.UserID)
            {
              blnExists = true;
            }
          }
          if (!blnExists)
          {
            objUser = new UserInfo();
            objUser.UserID = objBlogPermission.UserId;
            objUser.Username = objBlogPermission.Username;
            objUser.DisplayName = objBlogPermission.DisplayName;
            arrUsers.Add(objUser);
          }
        }
      }
      return arrUsers;

    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Load the ViewState
  /// </summary>
  /// <param name="savedState">The saved state</param>
  /// <history>
  ///     [cnurse]    01/12/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    protected override void LoadViewState(object savedState)
    {

      if (savedState is not null)
      {
        // Load State from the array of objects that was saved with SaveViewState.

        object[] myState = (object[])savedState;

        // Load Base Controls ViewState
        if (myState[0] is not null)
        {
          base.LoadViewState(myState[0]);
        }

        // Load BlogID
        if (myState[1] is not null)
        {
          BlogID = Conversions.ToInteger(myState[1]);
        }

        if (myState[2] is not null)
        {
          CurrentUserId = Conversions.ToInteger(myState[2]);
        }

        // Load BlogPermissions
        if (myState[3] is not null)
        {
          var arrPermissions = new ArrayList();
          string state = Conversions.ToString(myState[3]);
          if (!string.IsNullOrEmpty(state))
          {
            // First Break the String into individual Keys
            string[] permissionKeys = Strings.Split(state, "##");
            foreach (string key in permissionKeys)
            {
              string[] Settings = Strings.Split(key, "|");
              ParsePermissionKeys(Settings, arrPermissions);
            }
          }
          _BlogPermissions = new BlogPermissionCollection(arrPermissions);
        }
      }

    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Saves the ViewState
  /// </summary>
  /// <history>
  ///     [cnurse]    01/12/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    protected override object SaveViewState()
    {

      UpdatePermissions();

      var allStates = new object[4];

      allStates[0] = base.SaveViewState();
      allStates[1] = BlogID;
      allStates[2] = CurrentUserId;

      // Persist the BlogPermissions
      var sb = new StringBuilder();
      bool addDelimiter = false;
      foreach (BlogPermissionInfo objBlogPermission in _BlogPermissions)
      {
        if (addDelimiter)
        {
          sb.Append("##");
        }
        else
        {
          addDelimiter = true;
        }
        sb.Append(BuildKey(objBlogPermission.AllowAccess, objBlogPermission.PermissionId, -1, objBlogPermission.RoleId, objBlogPermission.RoleName, objBlogPermission.UserId, objBlogPermission.DisplayName));
      }
      allStates[3] = sb.ToString();

      return allStates;

    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Updates a Permission
  /// </summary>
  /// <param name="permission">The permission being updated</param>
  /// <param name="roleName">The name of the role</param>
  /// <param name="roleId">The id of the role</param>
  /// <param name="allowAccess">The value of the permission</param>
  /// <history>
  ///     [cnurse]    01/12/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    protected override void UpdatePermission(PermissionInfo permission, int roleId, string roleName, bool allowAccess)
    {

      bool isMatch = false;
      BlogPermissionInfo objPermission;
      int permissionId = permission.PermissionId;

      // Search BlogPermission Collection for the permission to Update
      foreach (var currentObjPermission in _BlogPermissions)
      {
        objPermission = currentObjPermission;
        if (objPermission.PermissionId == permissionId & objPermission.RoleId == roleId)
        {
          // BlogPermission is in collection
          objPermission.AllowAccess = allowAccess;
          isMatch = true;
          break;
        }
      }

      // BlogPermission not found so add new
      if (!isMatch & allowAccess)
      {
        objPermission = new BlogPermissionInfo();
        objPermission.PermissionId = permission.PermissionId;
        objPermission.PermissionKey = permission.PermissionKey;
        objPermission.BlogId = BlogID;
        objPermission.RoleId = roleId;
        objPermission.RoleName = roleName;
        objPermission.AllowAccess = allowAccess;
        objPermission.UserId = glbUserNothing;
        objPermission.DisplayName = Null.NullString;
        _BlogPermissions.Add(objPermission);
      }

    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Updates a Permission
  /// </summary>
  /// <param name="permission">The permission being updated</param>
  /// <param name="displayName">The user's displayname</param>
  /// <param name="userId">The user's id</param>
  /// <param name="allowAccess">The value of the permission</param>
  /// <history>
  ///     [cnurse]    01/12/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    protected override void UpdatePermission(PermissionInfo permission, string displayName, int userId, bool allowAccess)
    {

      bool isMatch = false;
      BlogPermissionInfo objPermission;
      int permissionId = permission.PermissionId;

      // Search BlogPermission Collection for the permission to Update
      foreach (var currentObjPermission in _BlogPermissions)
      {
        objPermission = currentObjPermission;
        if (objPermission.PermissionId == permissionId & objPermission.UserId == userId)
        {
          objPermission.AllowAccess = allowAccess;
          isMatch = true;
          break;
        }
      }

      // BlogPermission not found so add new
      if (!isMatch & allowAccess)
      {
        objPermission = new BlogPermissionInfo();
        objPermission.PermissionId = permission.PermissionId;
        objPermission.PermissionKey = permission.PermissionKey;
        objPermission.BlogId = BlogID;
        objPermission.RoleId = glbRoleNothing;
        objPermission.RoleName = Null.NullString;
        objPermission.AllowAccess = allowAccess;
        objPermission.UserId = userId;
        objPermission.DisplayName = displayName;
        _BlogPermissions.Add(objPermission);
      }

    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Updates a Permission
  /// </summary>
  /// <param name="permissions">The permissions collection</param>
  /// <param name="user">The user to add</param>
  /// <history>
  ///     [cnurse]    01/12/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    protected override void AddPermission(PermissionCollection permissions, UserInfo user)
    {

      BlogPermissionInfo objBlogPermission;

      // Search TabPermission Collection for the user 
      bool isMatch = false;
      foreach (var currentObjBlogPermission in _BlogPermissions)
      {
        objBlogPermission = currentObjBlogPermission;
        if (objBlogPermission.UserId == user.UserID)
        {
          isMatch = true;
          break;
        }
      }

      // user not found so add new
      if (!isMatch)
      {
        foreach (var objPermission in permissions.Values)
        {
          objBlogPermission = new BlogPermissionInfo();
          objBlogPermission.PermissionId = objPermission.PermissionId;
          objBlogPermission.PermissionKey = objPermission.PermissionKey;
          objBlogPermission.BlogId = BlogID;
          objBlogPermission.RoleId = glbRoleNothing;
          objBlogPermission.RoleName = Null.NullString;
          if (objPermission.PermissionId == (int)BlogPermissionTypes.ADD)
          {
            objBlogPermission.AllowAccess = true;
          }
          else
          {
            objBlogPermission.AllowAccess = false;
          }
          objBlogPermission.UserId = user.UserID;
          objBlogPermission.DisplayName = user.DisplayName;
          _BlogPermissions.Add(objBlogPermission);
        }
      }

    }

    protected override void CreateChildControls()
    {
      base.CreateChildControls();
    }
    #endregion

    #region  Public Methods 

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Overrides the Base method to Generate the Data Grid
  /// </summary>
  /// <history>
  ///     [cnurse]    01/09/2006  Documented
  /// </history>
  /// -----------------------------------------------------------------------------
    public override void GenerateDataGrid()
    {

    }

    #endregion

  }

}