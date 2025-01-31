using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using static DotNetNuke.Modules.Blog.Common.Globals;
using DotNetNuke.Modules.Blog.Security.Permissions;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.WebControls;
using Microsoft.VisualBasic;

namespace DotNetNuke.Modules.Blog.Security.Controls
{

  public abstract class PermissionsGrid : Control, INamingContainer
  {

    private Panel pnlPermissions;
    private Label lblGroups;
    private DropDownList _cboRoleGroups;

    private DropDownList cboRoleGroups
    {
      [MethodImpl(MethodImplOptions.Synchronized)]
      get
      {
        return _cboRoleGroups;
      }

      [MethodImpl(MethodImplOptions.Synchronized)]
      set
      {
        if (_cboRoleGroups != null)
        {
          _cboRoleGroups.SelectedIndexChanged -= RoleGroupsSelectedIndexChanged;
        }

        _cboRoleGroups = value;
        if (_cboRoleGroups != null)
        {
          _cboRoleGroups.SelectedIndexChanged += RoleGroupsSelectedIndexChanged;
        }
      }
    }
    private DataGrid dgRolePermissions;
    private Label lblUser;
    private TextBox txtUser;
    private LinkButton _cmdUser;

    private LinkButton cmdUser
    {
      [MethodImpl(MethodImplOptions.Synchronized)]
      get
      {
        return _cmdUser;
      }

      [MethodImpl(MethodImplOptions.Synchronized)]
      set
      {
        if (_cmdUser != null)
        {
          _cmdUser.Click -= AddUser;
        }

        _cmdUser = value;
        if (_cmdUser != null)
        {
          _cmdUser.Click += AddUser;
        }
      }
    }
    private DataGrid dgUserPermissions;

    #region  Private Members 

    private DataTable _dtRolePermissions = new DataTable();
    private DataTable _dtUserPermissions = new DataTable();
    private List<RoleInfo> _roles;
    private ArrayList _users;
    private PermissionCollection _permissions;
    private string _resourceFile;

    #endregion

    #region  Public Properties 

    #region  DataGrid Properties 

    public TableItemStyle AlternatingItemStyle
    {
      get
      {
        return dgRolePermissions.AlternatingItemStyle;
      }
    }

    public bool AutoGenerateColumns
    {
      get
      {
        return dgRolePermissions.AutoGenerateColumns;
      }
      set
      {
        dgRolePermissions.AutoGenerateColumns = value;
        dgUserPermissions.AutoGenerateColumns = value;
      }
    }

    public int CellSpacing
    {
      get
      {
        return dgRolePermissions.CellSpacing;
      }
      set
      {
        dgRolePermissions.CellSpacing = value;
        dgUserPermissions.CellSpacing = value;
      }
    }

    public DataGridColumnCollection Columns
    {
      get
      {
        return dgRolePermissions.Columns;
      }
    }

    public TableItemStyle FooterStyle
    {
      get
      {
        return dgRolePermissions.FooterStyle;
      }
    }

    public GridLines GridLines
    {
      get
      {
        return dgRolePermissions.GridLines;
      }
      set
      {
        dgRolePermissions.GridLines = value;
        dgUserPermissions.GridLines = value;
      }
    }

    public TableItemStyle HeaderStyle
    {
      get
      {
        return dgRolePermissions.HeaderStyle;
      }
    }

    public TableItemStyle ItemStyle
    {
      get
      {
        return dgRolePermissions.ItemStyle;
      }
    }

    public DataGridItemCollection Items
    {
      get
      {
        return dgRolePermissions.Items;
      }
    }

    public TableItemStyle SelectedItemStyle
    {
      get
      {
        return dgRolePermissions.SelectedItemStyle;
      }
    }

    public bool IncludeAdministratorRole { get; set; } = true;
    #endregion

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Gets the Id of the Administrator Role
  /// </summary>
  /// <history>
  ///     [cnurse]    01/16/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    public int AdministratorRoleId
    {
      get
      {
        return Framework.ServiceLocator<IPortalController, PortalController>.Instance.GetCurrentPortalSettings().AdministratorRoleId;
      }
    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Gets the Id of the Registered Users Role
  /// </summary>
  /// <history>
  ///     [cnurse]    01/16/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    public int RegisteredUsersRoleId
    {
      get
      {
        return Framework.ServiceLocator<IPortalController, PortalController>.Instance.GetCurrentPortalSettings().RegisteredRoleId;
      }
    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Gets and Sets whether a Dynamic Column has been added
  /// </summary>
  /// <history>
  ///     [cnurse]    01/09/2006  Documented
  /// </history>
  /// -----------------------------------------------------------------------------
    public bool DynamicColumnAdded
    {
      get
      {
        if (ViewState["ColumnAdded"] is null)
        {
          return false;
        }
        else
        {
          return true;
        }
      }
      set
      {
        ViewState["ColumnAdded"] = value;
      }
    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Gets the underlying Permissions Data Table
  /// </summary>
  /// <history>
  ///     [cnurse]    01/09/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    public DataTable dtRolePermissions
    {
      get
      {
        return _dtRolePermissions;
      }
    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Gets the underlying Permissions Data Table
  /// </summary>
  /// <history>
  ///     [cnurse]    01/09/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    public DataTable dtUserPermissions
    {
      get
      {
        return _dtUserPermissions;
      }
    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Gets the Id of the Portal
  /// </summary>
  /// <history>
  ///     [cnurse]    01/16/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    public int PortalId
    {
      get
      {
        // Obtain PortalSettings from Current Context
        var _portalSettings = Framework.ServiceLocator<IPortalController, PortalController>.Instance.GetCurrentPortalSettings();
        int intPortalID;

        if (_portalSettings.ActiveTab.ParentId == _portalSettings.SuperTabId) // if we are in host filemanager then we need to pass a null portal id
        {
          intPortalID = Null.NullInteger;
        }
        else
        {
          intPortalID = _portalSettings.PortalId;
        }

        return intPortalID;
      }
    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Gets and Sets the collection of Roles to display
  /// </summary>
  /// <history>
  ///     [cnurse]    01/09/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    public List<RoleInfo> Roles
    {
      get
      {
        return _roles;
      }
      set
      {
        _roles = value;
      }
    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Gets and Sets the ResourceFile to localize permissions
  /// </summary>
  /// <history>
  ///     [vmasanas]    02/24/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    public string ResourceFile
    {
      get
      {
        return _resourceFile;
      }
      set
      {
        _resourceFile = value;
      }
    }
    #endregion

    #region  Abstract Methods 

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Generate the Data Grid
  /// </summary>
  /// <history>
  ///     [cnurse]    01/09/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    public abstract void GenerateDataGrid();

    #endregion

    #region  Private Methods 

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Bind the data to the controls
  /// </summary>
  /// <history>
  ///     [cnurse]    01/09/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    private void BindData()
    {

      EnsureChildControls();

      BindRolesGrid();
      BindUsersGrid();

    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Bind the Roles data to the Grid
  /// </summary>
  /// <history>
  ///     [cnurse]    01/09/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    private void BindRolesGrid()
    {

      dtRolePermissions.Columns.Clear();
      dtRolePermissions.Rows.Clear();

      DataColumn col;

      // Add Roles Column
      col = new DataColumn("RoleId");
      dtRolePermissions.Columns.Add(col);

      // Add Roles Column
      col = new DataColumn("RoleName");
      dtRolePermissions.Columns.Add(col);

      foreach (PermissionInfo pi in _permissions.Values)
      {
        // Add Enabled Column
        col = new DataColumn(pi.PermissionKey + "_Enabled");
        dtRolePermissions.Columns.Add(col);
        // Add Permission Column
        col = new DataColumn(pi.PermissionKey);
        dtRolePermissions.Columns.Add(col);
      }
      int i;

      GetRoles();

      UpdateRolePermissions();
      DataRow row;
      var loopTo = Roles.Count - 1;
      for (i = 0; i <= loopTo; i++)
      {
        var role = Roles[i];
        row = dtRolePermissions.NewRow();
        row["RoleId"] = role.RoleID;
        row["RoleName"] = Localization.LocalizeRole(role.RoleName);

        int j = 0;
        foreach (PermissionInfo pi in _permissions.Values)
        {
          row[pi.PermissionKey + "_Enabled"] = GetEnabled(pi, role, j + 1);
          row[pi.PermissionKey] = GetPermission(pi, role, j + 1);
          j += 1;
        }
        dtRolePermissions.Rows.Add(row);
      }

      dgRolePermissions.DataSource = dtRolePermissions;
      dgRolePermissions.DataBind();

    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Bind the Roles data to the Grid
  /// </summary>
  /// <history>
  ///     [cnurse]    01/09/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    private void BindUsersGrid()
    {

      dtUserPermissions.Columns.Clear();
      dtUserPermissions.Rows.Clear();

      DataColumn col;

      // Add Roles Column
      col = new DataColumn("UserId");
      dtUserPermissions.Columns.Add(col);

      // Add Roles Column
      col = new DataColumn("DisplayName");
      dtUserPermissions.Columns.Add(col);

      int i;
      foreach (PermissionInfo pi in _permissions.Values)
      {
        // Add Enabled Column
        col = new DataColumn(pi.PermissionKey + "_Enabled");
        dtUserPermissions.Columns.Add(col);
        // Add Permission Column
        col = new DataColumn(pi.PermissionKey);
        dtUserPermissions.Columns.Add(col);
      }

      if (dgUserPermissions is not null)
      {

        _users = GetUsers();

        if (_users.Count != 0)
        {
          dgUserPermissions.Visible = true;

          UpdateUserPermissions();

          DataRow row;
          var loopTo = _users.Count - 1;
          for (i = 0; i <= loopTo; i++)
          {
            UserInfo user = (UserInfo)_users[i];
            row = dtUserPermissions.NewRow();
            row["UserId"] = user.UserID;
            row["DisplayName"] = user.DisplayName;

            int j = 0;
            foreach (PermissionInfo pi in _permissions.Values)
            {
              row[pi.PermissionKey + "_Enabled"] = GetEnabled(pi, user, j + 1);
              row[pi.PermissionKey] = GetPermission(pi, user, j + 1);
              j += 1;
            }
            dtUserPermissions.Rows.Add(row);
          }

          dgUserPermissions.DataSource = dtUserPermissions;
          dgUserPermissions.DataBind();
        }
        else
        {
          dgUserPermissions.Visible = false;
        }
      }

    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Gets the roles from the Database and loads them into the Roles property
  /// </summary>
  /// <history>
  ///     [cnurse]    01/09/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    private void GetRoles()
    {
      var objRoleController = new RoleController();
      int RoleGroupId = -2;
      if (cboRoleGroups is not null && cboRoleGroups.SelectedValue is not null)
      {
        RoleGroupId = int.Parse(cboRoleGroups.SelectedValue);
      }

      if (RoleGroupId > -2)
      {
        _roles = GetRolesByGroup(Framework.ServiceLocator<IPortalController, PortalController>.Instance.GetCurrentPortalSettings().PortalId, RoleGroupId);
      }
      else
      {
        _roles = GetRolesByPortal(Framework.ServiceLocator<IPortalController, PortalController>.Instance.GetCurrentPortalSettings().PortalId);
      }
      if (!IncludeAdministratorRole)
      {
        var newList = new List<RoleInfo>();
        foreach (RoleInfo r in _roles)
        {
          if (r.RoleID != AdministratorRoleId)
          {
            newList.Add(r);
          }
        }
        _roles = newList;
      }

      if (RoleGroupId < 0)
      {
        var r = new RoleInfo();
        r.RoleID = -1; // all users
        r.RoleName = DotNetNuke.Common.Globals.glbRoleAllUsersName;
        _roles.Add(r);
      }
      _roles.Reverse();
      _roles = _roles.OrderBy(r => r.RoleName.ToLower()).ToList();
    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Sets up the columns for the Grid
  /// </summary>
  /// <history>
  ///     [cnurse]    01/09/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    private void SetUpRolesGrid()
    {

      dgRolePermissions.Columns.Clear();

      var textCol = new BoundColumn();
      textCol.HeaderText = "&nbsp;";
      textCol.DataField = "RoleName";
      textCol.ItemStyle.Width = Unit.Parse("100px");
      dgRolePermissions.Columns.Add(textCol);

      var idCol = new BoundColumn();
      idCol.HeaderText = "";
      idCol.DataField = "roleid";
      idCol.Visible = false;
      dgRolePermissions.Columns.Add(idCol);

      TemplateColumn checkCol;

      // Dim objPermission As PermissionInfo
      foreach (PermissionInfo objPermission in _permissions.Values)
      {
        checkCol = new TemplateColumn();
        var columnTemplate = new CheckBoxColumnTemplate();
        columnTemplate.DataField = objPermission.PermissionKey;
        columnTemplate.EnabledField = objPermission.PermissionKey + "_Enabled";
        checkCol.ItemTemplate = columnTemplate;
        string locName = "";
        locName = Localization.GetString(objPermission.PermissionKey + ".Permission", SharedResourceFileName);
        if (locName is null)
        {
          locName = objPermission.PermissionKey;
        }
        checkCol.HeaderText = Interaction.IIf(!string.IsNullOrEmpty(locName), locName, objPermission.PermissionKey).ToString();
        checkCol.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        checkCol.HeaderStyle.Wrap = true;
        dgRolePermissions.Columns.Add(checkCol);
      }

    }

    private void SetUpUsersGrid()
    {

      if (dgUserPermissions is not null)
      {
        dgUserPermissions.Columns.Clear();

        var textCol = new BoundColumn();
        textCol.HeaderText = "&nbsp;";
        textCol.DataField = "DisplayName";
        textCol.ItemStyle.Width = Unit.Parse("100px");
        dgUserPermissions.Columns.Add(textCol);

        var idCol = new BoundColumn();
        idCol.HeaderText = "";
        idCol.DataField = "userid";
        idCol.Visible = false;
        dgUserPermissions.Columns.Add(idCol);

        TemplateColumn checkCol;

        foreach (PermissionInfo objPermission in _permissions.Values)
        {
          checkCol = new TemplateColumn();
          var columnTemplate = new CheckBoxColumnTemplate();
          columnTemplate.DataField = objPermission.PermissionKey;
          columnTemplate.EnabledField = objPermission.PermissionKey + "_Enabled";
          checkCol.ItemTemplate = columnTemplate;
          string locName = "";
          locName = Localization.GetString(objPermission.PermissionKey + ".Permission", SharedResourceFileName);
          if (locName is null)
          {
            locName = objPermission.PermissionKey;
          }
          checkCol.HeaderText = Interaction.IIf(!string.IsNullOrEmpty(locName), locName, objPermission.PermissionKey).ToString();
          checkCol.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
          checkCol.HeaderStyle.Wrap = true;
          dgUserPermissions.Columns.Add(checkCol);
        }
      }

    }


    #endregion

    #region  Protected Methods 

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Builds the key used to store the "permission" information in the ViewState
  /// </summary>
  /// <param name="checked">Is the checkbox checked</param>
  /// <param name="permissionId">The Id of the permission</param>
  /// <param name="objectPermissionId">The Id of the object permission</param>
  /// <param name="roleId">The role id</param>
  /// <param name="roleName">The role name</param>
  /// <history>
  /// </history>
  /// -----------------------------------------------------------------------------
    protected string BuildKey(bool @checked, int permissionId, int objectPermissionId, int roleId, string roleName)
    {
      return BuildKey(@checked, permissionId, objectPermissionId, roleId, roleName, Null.NullInteger, Null.NullString);
    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Builds the key used to store the "permission" information in the ViewState
  /// </summary>
  /// <param name="checked">Is the checkbox checked</param>
  /// <param name="permissionId">The Id of the permission</param>
  /// <param name="objectPermissionId">The Id of the object permission</param>
  /// <param name="roleId">The role id</param>
  /// <param name="roleName">The role name</param>
  /// <param name="userID">The user id</param>
  /// <param name="displayName">The user display name</param>
  /// <history>
  ///     [cnurse]    01/09/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    protected string BuildKey(bool @checked, int permissionId, int objectPermissionId, int roleId, string roleName, int userID, string displayName)
    {

      string key;

      if (@checked)
      {
        key = "True";
      }
      else
      {
        key = "False";
      }

      key += "|" + Convert.ToString(permissionId);

      // Add objectPermissionId
      key += "|";
      if (objectPermissionId > -1)
      {
        key += Convert.ToString(objectPermissionId);
      }

      key += "|" + roleName;
      key += "|" + roleId.ToString();
      key += "|" + userID.ToString();
      key += "|" + displayName.ToString();

      return key;

    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Creates the Child Controls
  /// </summary>
  /// <history>
  ///     [cnurse]    02/23/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    protected override void CreateChildControls()
    {

      // get data
      _permissions = GetPermissions();

      pnlPermissions = new Panel();
      pnlPermissions.CssClass = "DataGrid_Container";

      // Optionally Add Role Group Filter
      var _portalSettings = Framework.ServiceLocator<IPortalController, PortalController>.Instance.GetCurrentPortalSettings();
      var arrGroups = RoleController.GetRoleGroups(_portalSettings.PortalId);
      if (arrGroups.Count > 0)
      {
        lblGroups = new Label();
        lblGroups.Text = Localization.GetString("RoleGroupFilter");
        lblGroups.CssClass = "SubHead";
        pnlPermissions.Controls.Add(lblGroups);

        pnlPermissions.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));

        cboRoleGroups = new DropDownList();
        cboRoleGroups.AutoPostBack = true;

        cboRoleGroups.Items.Add(new ListItem(Localization.GetString("AllRoles"), "-2"));
        var liItem = new ListItem(Localization.GetString("GlobalRoles"), "-1");
        liItem.Selected = true;
        cboRoleGroups.Items.Add(liItem);
        foreach (RoleGroupInfo roleGroup in arrGroups)
          cboRoleGroups.Items.Add(new ListItem(roleGroup.RoleGroupName, roleGroup.RoleGroupID.ToString()));
        pnlPermissions.Controls.Add(cboRoleGroups);

        pnlPermissions.Controls.Add(new LiteralControl("<br/><br/>"));
      }

      dgRolePermissions = new DataGrid();
      dgRolePermissions.AutoGenerateColumns = false;
      dgRolePermissions.CellSpacing = 0;
      dgRolePermissions.GridLines = GridLines.None;
      dgRolePermissions.CssClass = "PermissionsGrid";
      dgRolePermissions.FooterStyle.CssClass = "DataGrid_Footer";
      dgRolePermissions.HeaderStyle.CssClass = "DataGrid_Header";
      dgRolePermissions.ItemStyle.CssClass = "DataGrid_Item";
      dgRolePermissions.AlternatingItemStyle.CssClass = "DataGrid_AlternatingItem";
      SetUpRolesGrid();
      pnlPermissions.Controls.Add(dgRolePermissions);

      _users = GetUsers();

      if (_users is not null)
      {
        dgUserPermissions = new DataGrid();
        dgUserPermissions.AutoGenerateColumns = false;
        dgUserPermissions.CellSpacing = 0;
        dgUserPermissions.GridLines = GridLines.None;
        dgUserPermissions.FooterStyle.CssClass = "DataGrid_Footer";
        dgUserPermissions.HeaderStyle.CssClass = "DataGrid_Header";
        dgUserPermissions.ItemStyle.CssClass = "DataGrid_Item";
        dgUserPermissions.AlternatingItemStyle.CssClass = "DataGrid_AlternatingItem";
        SetUpUsersGrid();
        pnlPermissions.Controls.Add(dgUserPermissions);

        pnlPermissions.Controls.Add(new LiteralControl("<br/>"));

        lblUser = new Label();
        lblUser.Text = Localization.GetString("User");
        lblUser.CssClass = "SubHead";
        pnlPermissions.Controls.Add(lblUser);

        pnlPermissions.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));

        txtUser = new TextBox();
        txtUser.CssClass = "NormalTextBox";
        pnlPermissions.Controls.Add(txtUser);

        pnlPermissions.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));

        cmdUser = new LinkButton();
        cmdUser.Text = Localization.GetString("Add");
        cmdUser.CssClass = "dnnSecondaryAction";
        pnlPermissions.Controls.Add(cmdUser);
      }

      Controls.Add(pnlPermissions);

    }

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
    protected virtual bool GetEnabled(PermissionInfo objPerm, RoleInfo role, int column)
    {
      return false;
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
    protected virtual bool GetEnabled(PermissionInfo objPerm, UserInfo user, int column)
    {
      return false;
    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Gets the Value of the permission
  /// </summary>
  /// <param name="objPerm">The permission being loaded</param>
  /// <param name="role">The role</param>
  /// <param name="column">The column of the Grid</param>
  /// <history>
  ///     [cnurse]    01/13/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    protected virtual bool GetPermission(PermissionInfo objPerm, RoleInfo role, int column)
    {
      return false;
    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Gets the Value of the permission
  /// </summary>
  /// <param name="objPerm">The permission being loaded</param>
  /// <param name="user">The user</param>
  /// <param name="column">The column of the Grid</param>
  /// <history>
  ///     [cnurse]    01/13/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    protected virtual bool GetPermission(PermissionInfo objPerm, UserInfo user, int column)
    {
      return false;
    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Gets the permissions from the Database
  /// </summary>
  /// <history>
  ///     [cnurse]    01/12/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    protected virtual PermissionCollection GetPermissions()
    {

      return null;

    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Gets the users from the Database
  /// </summary>
  /// <history>
  ///     [cnurse]    01/12/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    protected virtual ArrayList GetUsers()
    {

      return null;

    }

    protected override void OnLoad(EventArgs e)
    {

    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Overrides the base OnPreRender method to Bind the Grid to the Permissions
  /// </summary>
  /// <history>
  ///     [cnurse]    01/09/2006  Documented
  /// </history>
  /// -----------------------------------------------------------------------------
    protected override void OnPreRender(EventArgs e)
    {
      BindData();
    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Updates a Permission
  /// </summary>
  /// <param name="permission">The permission being updated</param>
  /// <param name="roleName">The name of the role</param>
  /// <param name="allowAccess">The value of the permission</param>
  /// <history>
  ///     [cnurse]    01/12/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    protected virtual void UpdatePermission(PermissionInfo permission, int roleId, string roleName, bool allowAccess)
    {

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
    protected virtual void UpdatePermission(PermissionInfo permission, string displayName, int userId, bool allowAccess)
    {

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
    protected virtual void AddPermission(PermissionCollection permissions, UserInfo user)
    {

    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Updates the permissions
  /// </summary>
  /// <history>
  ///     [cnurse]    01/09/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    protected void UpdatePermissions()
    {

      EnsureChildControls();

      UpdateRolePermissions();
      UpdateUserPermissions();

    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Updates the permissions
  /// </summary>
  /// <history>
  ///     [cnurse]    01/09/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    protected void UpdateRolePermissions()
    {

      if (dgRolePermissions is not null)
      {
        foreach (DataGridItem dgi in dgRolePermissions.Items)
        {
          int i;
          var loopTo = dgi.Cells.Count - 1;
          for (i = 2; i <= loopTo; i++)
          {
            // all except first two cells which is role names and role ids
            if (dgi.Cells[i].Controls.Count > 0)
            {
              CheckBox cb = (CheckBox)dgi.Cells[i].Controls[0];
              UpdatePermission(_permissions[i - 2], int.Parse(dgi.Cells[1].Text), dgi.Cells[0].Text, cb.Checked);
            }
          }
        }
      }

    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Updates the permissions
  /// </summary>
  /// <history>
  ///     [cnurse]    01/09/2006  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    protected void UpdateUserPermissions()
    {

      if (dgUserPermissions is not null)
      {
        foreach (DataGridItem dgi in dgUserPermissions.Items)
        {
          int i;
          var loopTo = dgi.Cells.Count - 1;
          for (i = 2; i <= loopTo; i++)
          {
            // all except first two cells which is displayname and userid
            if (dgi.Cells[i].Controls.Count > 0)
            {
              CheckBox cb = (CheckBox)dgi.Cells[i].Controls[0];
              UpdatePermission(_permissions[i - 2], dgi.Cells[0].Text, int.Parse(dgi.Cells[1].Text), cb.Checked);
            }
          }
        }
      }

    }

    #endregion

    #region  Event Handlers 

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// RoleGroupsSelectedIndexChanged runs when the Role Group is changed
  /// </summary>
  /// <history>
  ///     [cnurse]    01/06/2006  Documented
  /// </history>
  /// -----------------------------------------------------------------------------
    private void RoleGroupsSelectedIndexChanged(object sender, EventArgs e)
    {

      UpdatePermissions();

    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// AddUser runs when the Add user linkbutton is clicked
  /// </summary>
  /// <history>
  /// </history>
  /// -----------------------------------------------------------------------------
    private void AddUser(object sender, EventArgs e)
    {

      UpdatePermissions();

      if (!string.IsNullOrEmpty(txtUser.Text))
      {
        // verify username
        var objUser = UserController.GetUserByName(PortalId, txtUser.Text);
        if (objUser is not null)
        {
          AddPermission(_permissions, objUser);
          BindData();
        }
        else
        {
          // user does not exist
          lblUser = new Label();
          lblUser.Text = "<br>" + Localization.GetString("InvalidUserName");
          lblUser.CssClass = "NormalRed";
          pnlPermissions.Controls.Add(lblUser);
        }
      }

    }

    #endregion

  }


}