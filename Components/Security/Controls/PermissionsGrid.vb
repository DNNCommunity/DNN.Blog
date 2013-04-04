Imports System
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Collections
Imports System.Data
Imports System.Linq
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Modules.Blog.Security.Permissions
Imports DotNetNuke.Modules.Blog.Common.Globals

Namespace Security.Controls

 Public MustInherit Class PermissionsGrid
  Inherits Control
  Implements INamingContainer

  Private pnlPermissions As Panel
  Private lblGroups As Label
  Private WithEvents cboRoleGroups As DropDownList
  Private dgRolePermissions As DataGrid
  Private lblUser As Label
  Private txtUser As TextBox
  Private WithEvents cmdUser As LinkButton
  Private dgUserPermissions As DataGrid

#Region " Private Members "

  Private _dtRolePermissions As New DataTable
  Private _dtUserPermissions As New DataTable
  Private _roles As ArrayList
  Private _users As ArrayList
  Private _permissions As PermissionCollection
  Private _resourceFile As String

#End Region

#Region " Public Properties "

#Region " DataGrid Properties "

  Public ReadOnly Property AlternatingItemStyle() As TableItemStyle
   Get
    Return dgRolePermissions.AlternatingItemStyle
   End Get
  End Property

  Public Property AutoGenerateColumns() As Boolean
   Get
    Return dgRolePermissions.AutoGenerateColumns
   End Get
   Set(Value As Boolean)
    dgRolePermissions.AutoGenerateColumns = Value
    dgUserPermissions.AutoGenerateColumns = Value
   End Set
  End Property

  Public Property CellSpacing() As Integer
   Get
    Return dgRolePermissions.CellSpacing
   End Get
   Set(Value As Integer)
    dgRolePermissions.CellSpacing = Value
    dgUserPermissions.CellSpacing = Value
   End Set
  End Property

  Public ReadOnly Property Columns() As DataGridColumnCollection
   Get
    Return dgRolePermissions.Columns()
   End Get
  End Property

  Public ReadOnly Property FooterStyle() As TableItemStyle
   Get
    Return dgRolePermissions.FooterStyle
   End Get
  End Property

  Public Property GridLines() As GridLines
   Get
    Return dgRolePermissions.GridLines
   End Get
   Set(Value As GridLines)
    dgRolePermissions.GridLines = Value
    dgUserPermissions.GridLines = Value
   End Set
  End Property

  Public ReadOnly Property HeaderStyle() As TableItemStyle
   Get
    Return dgRolePermissions.HeaderStyle
   End Get
  End Property

  Public ReadOnly Property ItemStyle() As TableItemStyle
   Get
    Return dgRolePermissions.ItemStyle
   End Get
  End Property

  Public ReadOnly Property Items() As DataGridItemCollection
   Get
    Return dgRolePermissions.Items()
   End Get
  End Property

  Public ReadOnly Property SelectedItemStyle() As TableItemStyle
   Get
    Return dgRolePermissions.SelectedItemStyle
   End Get
  End Property

  Public Property IncludeAdministratorRole As Boolean = True
#End Region

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets the Id of the Administrator Role
  ''' </summary>
  ''' <history>
  '''     [cnurse]    01/16/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public ReadOnly Property AdministratorRoleId() As Integer
   Get
    Return PortalController.GetCurrentPortalSettings.AdministratorRoleId
   End Get
  End Property

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets the Id of the Registered Users Role
  ''' </summary>
  ''' <history>
  '''     [cnurse]    01/16/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public ReadOnly Property RegisteredUsersRoleId() As Integer
   Get
    Return PortalController.GetCurrentPortalSettings.RegisteredRoleId
   End Get
  End Property

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets and Sets whether a Dynamic Column has been added
  ''' </summary>
  ''' <history>
  '''     [cnurse]    01/09/2006  Documented
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Property DynamicColumnAdded() As Boolean
   Get
    If ViewState("ColumnAdded") Is Nothing Then
     Return False
    Else
     Return True
    End If
   End Get
   Set(Value As Boolean)
    ViewState("ColumnAdded") = Value
   End Set
  End Property

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets the underlying Permissions Data Table
  ''' </summary>
  ''' <history>
  '''     [cnurse]    01/09/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public ReadOnly Property dtRolePermissions() As DataTable
   Get
    Return _dtRolePermissions
   End Get
  End Property

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets the underlying Permissions Data Table
  ''' </summary>
  ''' <history>
  '''     [cnurse]    01/09/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public ReadOnly Property dtUserPermissions() As DataTable
   Get
    Return _dtUserPermissions
   End Get
  End Property

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets the Id of the Portal
  ''' </summary>
  ''' <history>
  '''     [cnurse]    01/16/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public ReadOnly Property PortalId() As Integer
   Get
    ' Obtain PortalSettings from Current Context
    Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
    Dim intPortalID As Integer

    If _portalSettings.ActiveTab.ParentId = _portalSettings.SuperTabId Then 'if we are in host filemanager then we need to pass a null portal id
     intPortalID = Null.NullInteger
    Else
     intPortalID = _portalSettings.PortalId
    End If

    Return intPortalID
   End Get
  End Property

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets and Sets the collection of Roles to display
  ''' </summary>
  ''' <history>
  '''     [cnurse]    01/09/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Property Roles() As ArrayList
   Get
    Return _roles
   End Get
   Set(Value As ArrayList)
    _roles = Value
   End Set
  End Property

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets and Sets the ResourceFile to localize permissions
  ''' </summary>
  ''' <history>
  '''     [vmasanas]    02/24/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Property ResourceFile() As String
   Get
    Return _resourceFile
   End Get
   Set(Value As String)
    _resourceFile = Value
   End Set
  End Property
#End Region

#Region " Abstract Methods "

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Generate the Data Grid
  ''' </summary>
  ''' <history>
  '''     [cnurse]    01/09/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public MustOverride Sub GenerateDataGrid()

#End Region

#Region " Private Methods "

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Bind the data to the controls
  ''' </summary>
  ''' <history>
  '''     [cnurse]    01/09/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Private Sub BindData()

   Me.EnsureChildControls()

   BindRolesGrid()
   BindUsersGrid()

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Bind the Roles data to the Grid
  ''' </summary>
  ''' <history>
  '''     [cnurse]    01/09/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Private Sub BindRolesGrid()

   dtRolePermissions.Columns.Clear()
   dtRolePermissions.Rows.Clear()

   Dim col As DataColumn

   'Add Roles Column
   col = New DataColumn("RoleId")
   dtRolePermissions.Columns.Add(col)

   'Add Roles Column
   col = New DataColumn("RoleName")
   dtRolePermissions.Columns.Add(col)

   For Each pi As PermissionInfo In _permissions.Values
    'Add Enabled Column
    col = New DataColumn(pi.PermissionKey & "_Enabled")
    dtRolePermissions.Columns.Add(col)
    'Add Permission Column
    col = New DataColumn(pi.PermissionKey)
    dtRolePermissions.Columns.Add(col)
   Next
   Dim i As Integer

   GetRoles()

   UpdateRolePermissions()
   Dim row As DataRow
   For i = 0 To Roles.Count - 1
    Dim role As RoleInfo = DirectCast(Roles(i), RoleInfo)
    row = dtRolePermissions.NewRow
    row("RoleId") = role.RoleID
    row("RoleName") = Localization.LocalizeRole(role.RoleName)

    Dim j As Integer = 0
    For Each pi As PermissionInfo In _permissions.Values
     row(pi.PermissionKey & "_Enabled") = GetEnabled(pi, role, j + 1)
     row(pi.PermissionKey) = GetPermission(pi, role, j + 1)
     j += 1
    Next
    dtRolePermissions.Rows.Add(row)
   Next

   dgRolePermissions.DataSource = dtRolePermissions
   dgRolePermissions.DataBind()

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Bind the Roles data to the Grid
  ''' </summary>
  ''' <history>
  '''     [cnurse]    01/09/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Private Sub BindUsersGrid()

   dtUserPermissions.Columns.Clear()
   dtUserPermissions.Rows.Clear()

   Dim col As DataColumn

   'Add Roles Column
   col = New DataColumn("UserId")
   dtUserPermissions.Columns.Add(col)

   'Add Roles Column
   col = New DataColumn("DisplayName")
   dtUserPermissions.Columns.Add(col)

   Dim i As Integer
   For Each pi As PermissionInfo In _permissions.Values
    'Add Enabled Column
    col = New DataColumn(pi.PermissionKey & "_Enabled")
    dtUserPermissions.Columns.Add(col)
    'Add Permission Column
    col = New DataColumn(pi.PermissionKey)
    dtUserPermissions.Columns.Add(col)
   Next

   If Not dgUserPermissions Is Nothing Then

    _users = GetUsers()

    If _users.Count <> 0 Then
     dgUserPermissions.Visible = True

     UpdateUserPermissions()

     Dim row As DataRow
     For i = 0 To _users.Count - 1
      Dim user As UserInfo = DirectCast(_users(i), UserInfo)
      row = dtUserPermissions.NewRow
      row("UserId") = user.UserID
      row("DisplayName") = user.DisplayName

      Dim j As Integer = 0
      For Each pi As PermissionInfo In _permissions.Values
       row(pi.PermissionKey & "_Enabled") = GetEnabled(pi, user, j + 1)
       row(pi.PermissionKey) = GetPermission(pi, user, j + 1)
       j += 1
      Next
      dtUserPermissions.Rows.Add(row)
     Next

     dgUserPermissions.DataSource = dtUserPermissions
     dgUserPermissions.DataBind()
    Else
     dgUserPermissions.Visible = False
    End If
   End If

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets the roles from the Database and loads them into the Roles property
  ''' </summary>
  ''' <history>
  '''     [cnurse]    01/09/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Private Sub GetRoles()
   Dim objRoleController As New RoleController
   Dim RoleGroupId As Integer = -2
   If (Not cboRoleGroups Is Nothing) AndAlso (Not cboRoleGroups.SelectedValue Is Nothing) Then
    RoleGroupId = Integer.Parse(cboRoleGroups.SelectedValue)
   End If

   If RoleGroupId > -2 Then
    _roles = DotNetNuke.Security.Roles.RoleProvider.Instance.GetRolesByGroup(PortalController.GetCurrentPortalSettings.PortalId, RoleGroupId)
   Else
    _roles = DotNetNuke.Security.Roles.RoleProvider.Instance.GetRoles(PortalController.GetCurrentPortalSettings.PortalId)
   End If
   If Not IncludeAdministratorRole Then
    Dim newList As New ArrayList
    For Each r As RoleInfo In _roles
     If r.RoleID <> AdministratorRoleId Then
      newList.Add(r)
     End If
    Next
    _roles = newList
   End If

   If RoleGroupId < 0 Then
    Dim r As New RoleInfo
    r.RoleID = Integer.Parse(glbRoleUnauthUser)
    r.RoleName = glbRoleUnauthUserName
    _roles.Add(r)
    r = New RoleInfo
    r.RoleID = Integer.Parse(glbRoleAllUsers)
    r.RoleName = glbRoleAllUsersName
    _roles.Add(r)
   End If
   _roles.Reverse()
   _roles.Sort(New RoleComparer)
  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Sets up the columns for the Grid
  ''' </summary>
  ''' <history>
  '''     [cnurse]    01/09/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Private Sub SetUpRolesGrid()

   dgRolePermissions.Columns.Clear()

   Dim textCol As New BoundColumn
   textCol.HeaderText = "&nbsp;"
   textCol.DataField = "RoleName"
   textCol.ItemStyle.Width = Unit.Parse("100px")
   dgRolePermissions.Columns.Add(textCol)

   Dim idCol As New BoundColumn
   idCol.HeaderText = ""
   idCol.DataField = "roleid"
   idCol.Visible = False
   dgRolePermissions.Columns.Add(idCol)

   Dim checkCol As TemplateColumn

   'Dim objPermission As PermissionInfo
   For Each objPermission As PermissionInfo In _permissions.Values
    checkCol = New TemplateColumn
    Dim columnTemplate As New CheckBoxColumnTemplate
    columnTemplate.DataField = objPermission.PermissionKey
    columnTemplate.EnabledField = objPermission.PermissionKey & "_Enabled"
    checkCol.ItemTemplate = columnTemplate
    Dim locName As String = ""
    locName = Localization.GetString(objPermission.PermissionKey + ".Permission", SharedResourceFileName)
    If locName Is Nothing Then
     locName = objPermission.PermissionKey
    End If
    checkCol.HeaderText = IIf(locName <> "", locName, objPermission.PermissionKey).ToString
    checkCol.ItemStyle.HorizontalAlign = HorizontalAlign.Center
    checkCol.HeaderStyle.Wrap = True
    dgRolePermissions.Columns.Add(checkCol)
   Next

  End Sub

  Private Sub SetUpUsersGrid()

   If Not dgUserPermissions Is Nothing Then
    dgUserPermissions.Columns.Clear()

    Dim textCol As New BoundColumn
    textCol.HeaderText = "&nbsp;"
    textCol.DataField = "DisplayName"
    textCol.ItemStyle.Width = Unit.Parse("100px")
    dgUserPermissions.Columns.Add(textCol)

    Dim idCol As New BoundColumn
    idCol.HeaderText = ""
    idCol.DataField = "userid"
    idCol.Visible = False
    dgUserPermissions.Columns.Add(idCol)

    Dim checkCol As TemplateColumn

    For Each objPermission As PermissionInfo In _permissions.Values
     checkCol = New TemplateColumn
     Dim columnTemplate As New CheckBoxColumnTemplate
     columnTemplate.DataField = objPermission.PermissionKey
     columnTemplate.EnabledField = objPermission.PermissionKey & "_Enabled"
     checkCol.ItemTemplate = columnTemplate
     Dim locName As String = ""
     locName = Localization.GetString(objPermission.PermissionKey + ".Permission", SharedResourceFileName)
     If locName Is Nothing Then
      locName = objPermission.PermissionKey
     End If
     checkCol.HeaderText = IIf(locName <> "", locName, objPermission.PermissionKey).ToString
     checkCol.ItemStyle.HorizontalAlign = HorizontalAlign.Center
     checkCol.HeaderStyle.Wrap = True
     dgUserPermissions.Columns.Add(checkCol)
    Next
   End If

  End Sub


#End Region

#Region " Protected Methods "

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Builds the key used to store the "permission" information in the ViewState
  ''' </summary>
  ''' <param name="checked">Is the checkbox checked</param>
  ''' <param name="permissionId">The Id of the permission</param>
  ''' <param name="objectPermissionId">The Id of the object permission</param>
  ''' <param name="roleId">The role id</param>
  ''' <param name="roleName">The role name</param>
  ''' <history>
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Protected Function BuildKey(checked As Boolean, permissionId As Integer, objectPermissionId As Integer, roleId As Integer, roleName As String) As String
   Return BuildKey(checked, permissionId, objectPermissionId, roleId, roleName, Null.NullInteger, Null.NullString)
  End Function

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Builds the key used to store the "permission" information in the ViewState
  ''' </summary>
  ''' <param name="checked">Is the checkbox checked</param>
  ''' <param name="permissionId">The Id of the permission</param>
  ''' <param name="objectPermissionId">The Id of the object permission</param>
  ''' <param name="roleId">The role id</param>
  ''' <param name="roleName">The role name</param>
  ''' <param name="userID">The user id</param>
  ''' <param name="displayName">The user display name</param>
  ''' <history>
  '''     [cnurse]    01/09/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Protected Function BuildKey(checked As Boolean, permissionId As Integer, objectPermissionId As Integer, roleId As Integer, roleName As String, userID As Integer, displayName As String) As String

   Dim key As String

   If checked Then
    key = "True"
   Else
    key = "False"
   End If

   key += "|" + Convert.ToString(permissionId)

   'Add objectPermissionId
   key += "|"
   If objectPermissionId > -1 Then
    key += Convert.ToString(objectPermissionId)
   End If

   key += "|" + roleName
   key += "|" + roleId.ToString
   key += "|" + userID.ToString
   key += "|" + displayName.ToString

   Return key

  End Function

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Creates the Child Controls
  ''' </summary>
  ''' <history>
  '''     [cnurse]    02/23/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Protected Overrides Sub CreateChildControls()

   ' get data
   _permissions = GetPermissions()

   pnlPermissions = New Panel
   pnlPermissions.CssClass = "DataGrid_Container"

   'Optionally Add Role Group Filter
   Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
   Dim arrGroups As ArrayList = RoleController.GetRoleGroups(_portalSettings.PortalId)
   If arrGroups.Count > 0 Then
    lblGroups = New Label
    lblGroups.Text = Localization.GetString("RoleGroupFilter")
    lblGroups.CssClass = "SubHead"
    pnlPermissions.Controls.Add(lblGroups)

    pnlPermissions.Controls.Add(New LiteralControl("&nbsp;&nbsp;"))

    cboRoleGroups = New DropDownList
    cboRoleGroups.AutoPostBack = True

    cboRoleGroups.Items.Add(New ListItem(Localization.GetString("AllRoles"), "-2"))
    Dim liItem As ListItem = New ListItem(Localization.GetString("GlobalRoles"), "-1")
    liItem.Selected = True
    cboRoleGroups.Items.Add(liItem)
    For Each roleGroup As RoleGroupInfo In arrGroups
     cboRoleGroups.Items.Add(New ListItem(roleGroup.RoleGroupName, roleGroup.RoleGroupID.ToString))
    Next
    pnlPermissions.Controls.Add(cboRoleGroups)

    pnlPermissions.Controls.Add(New LiteralControl("<br/><br/>"))
   End If

   dgRolePermissions = New DataGrid
   dgRolePermissions.AutoGenerateColumns = False
   dgRolePermissions.CellSpacing = 0
   dgRolePermissions.GridLines = GridLines.None
   dgRolePermissions.CssClass = "PermissionsGrid"
   dgRolePermissions.FooterStyle.CssClass = "DataGrid_Footer"
   dgRolePermissions.HeaderStyle.CssClass = "DataGrid_Header"
   dgRolePermissions.ItemStyle.CssClass = "DataGrid_Item"
   dgRolePermissions.AlternatingItemStyle.CssClass = "DataGrid_AlternatingItem"
   SetUpRolesGrid()
   pnlPermissions.Controls.Add(dgRolePermissions)

   _users = GetUsers()

   If Not _users Is Nothing Then
    dgUserPermissions = New DataGrid
    dgUserPermissions.AutoGenerateColumns = False
    dgUserPermissions.CellSpacing = 0
    dgUserPermissions.GridLines = GridLines.None
    dgUserPermissions.FooterStyle.CssClass = "DataGrid_Footer"
    dgUserPermissions.HeaderStyle.CssClass = "DataGrid_Header"
    dgUserPermissions.ItemStyle.CssClass = "DataGrid_Item"
    dgUserPermissions.AlternatingItemStyle.CssClass = "DataGrid_AlternatingItem"
    SetUpUsersGrid()
    pnlPermissions.Controls.Add(dgUserPermissions)

    pnlPermissions.Controls.Add(New LiteralControl("<br/>"))

    lblUser = New Label
    lblUser.Text = Localization.GetString("User")
    lblUser.CssClass = "SubHead"
    pnlPermissions.Controls.Add(lblUser)

    pnlPermissions.Controls.Add(New LiteralControl("&nbsp;&nbsp;"))

    txtUser = New TextBox
    txtUser.CssClass = "NormalTextBox"
    pnlPermissions.Controls.Add(txtUser)

    pnlPermissions.Controls.Add(New LiteralControl("&nbsp;&nbsp;"))

    cmdUser = New LinkButton
    cmdUser.Text = Localization.GetString("Add")
    cmdUser.CssClass = "dnnSecondaryAction"
    pnlPermissions.Controls.Add(cmdUser)
   End If

   Me.Controls.Add(pnlPermissions)

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets the Enabled status of the permission
  ''' </summary>
  ''' <param name="objPerm">The permission being loaded</param>
  ''' <param name="role">The role</param>
  ''' <param name="column">The column of the Grid</param>
  ''' <history>
  '''     [cnurse]    01/13/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Protected Overridable Function GetEnabled(objPerm As PermissionInfo, role As RoleInfo, column As Integer) As Boolean
   Return False
  End Function

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets the Enabled status of the permission
  ''' </summary>
  ''' <param name="objPerm">The permission being loaded</param>
  ''' <param name="user">The user</param>
  ''' <param name="column">The column of the Grid</param>
  ''' <history>
  '''     [cnurse]    01/13/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Protected Overridable Function GetEnabled(objPerm As PermissionInfo, user As UserInfo, column As Integer) As Boolean
   Return False
  End Function

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets the Value of the permission
  ''' </summary>
  ''' <param name="objPerm">The permission being loaded</param>
  ''' <param name="role">The role</param>
  ''' <param name="column">The column of the Grid</param>
  ''' <history>
  '''     [cnurse]    01/13/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Protected Overridable Function GetPermission(objPerm As PermissionInfo, role As RoleInfo, column As Integer) As Boolean
   Return False
  End Function

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets the Value of the permission
  ''' </summary>
  ''' <param name="objPerm">The permission being loaded</param>
  ''' <param name="user">The user</param>
  ''' <param name="column">The column of the Grid</param>
  ''' <history>
  '''     [cnurse]    01/13/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Protected Overridable Function GetPermission(objPerm As PermissionInfo, user As UserInfo, column As Integer) As Boolean
   Return False
  End Function

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets the permissions from the Database
  ''' </summary>
  ''' <history>
  '''     [cnurse]    01/12/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Protected Overridable Function GetPermissions() As PermissionCollection

   Return Nothing

  End Function

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets the users from the Database
  ''' </summary>
  ''' <history>
  '''     [cnurse]    01/12/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Protected Overridable Function GetUsers() As ArrayList

   Return Nothing

  End Function

  Protected Overrides Sub OnLoad(e As System.EventArgs)

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Overrides the base OnPreRender method to Bind the Grid to the Permissions
  ''' </summary>
  ''' <history>
  '''     [cnurse]    01/09/2006  Documented
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Protected Overrides Sub OnPreRender(e As System.EventArgs)
   BindData()
  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Updates a Permission
  ''' </summary>
  ''' <param name="permission">The permission being updated</param>
  ''' <param name="roleName">The name of the role</param>
  ''' <param name="allowAccess">The value of the permission</param>
  ''' <history>
  '''     [cnurse]    01/12/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Protected Overridable Sub UpdatePermission(permission As PermissionInfo, roleId As Integer, roleName As String, allowAccess As Boolean)

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Updates a Permission
  ''' </summary>
  ''' <param name="permission">The permission being updated</param>
  ''' <param name="displayName">The user's displayname</param>
  ''' <param name="userId">The user's id</param>
  ''' <param name="allowAccess">The value of the permission</param>
  ''' <history>
  '''     [cnurse]    01/12/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Protected Overridable Sub UpdatePermission(permission As PermissionInfo, displayName As String, userId As Integer, allowAccess As Boolean)

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Updates a Permission
  ''' </summary>
  ''' <param name="permissions">The permissions collection</param>
  ''' <param name="user">The user to add</param>
  ''' <history>
  '''     [cnurse]    01/12/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Protected Overridable Sub AddPermission(permissions As PermissionCollection, user As UserInfo)

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Updates the permissions
  ''' </summary>
  ''' <history>
  '''     [cnurse]    01/09/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Protected Sub UpdatePermissions()

   Me.EnsureChildControls()

   UpdateRolePermissions()
   UpdateUserPermissions()

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Updates the permissions
  ''' </summary>
  ''' <history>
  '''     [cnurse]    01/09/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Protected Sub UpdateRolePermissions()

   If Not dgRolePermissions Is Nothing Then
    Dim dgi As DataGridItem
    For Each dgi In dgRolePermissions.Items
     Dim i As Integer
     For i = 2 To dgi.Cells.Count - 1
      'all except first two cells which is role names and role ids
      If dgi.Cells(i).Controls.Count > 0 Then
       Dim cb As CheckBox = CType(dgi.Cells(i).Controls(0), CheckBox)
       UpdatePermission(CType(_permissions.Item(i - 2), PermissionInfo), Integer.Parse(dgi.Cells(1).Text), dgi.Cells(0).Text, cb.Checked)
      End If
     Next
    Next
   End If

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Updates the permissions
  ''' </summary>
  ''' <history>
  '''     [cnurse]    01/09/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Protected Sub UpdateUserPermissions()

   If Not dgUserPermissions Is Nothing Then
    Dim dgi As DataGridItem
    For Each dgi In dgUserPermissions.Items
     Dim i As Integer
     For i = 2 To dgi.Cells.Count - 1
      'all except first two cells which is displayname and userid
      If dgi.Cells(i).Controls.Count > 0 Then
       Dim cb As CheckBox = CType(dgi.Cells(i).Controls(0), CheckBox)
       UpdatePermission(CType(_permissions(i - 2), PermissionInfo), dgi.Cells(0).Text, Integer.Parse(dgi.Cells(1).Text), cb.Checked)
      End If
     Next
    Next
   End If

  End Sub

#End Region

#Region " Event Handlers "

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' RoleGroupsSelectedIndexChanged runs when the Role Group is changed
  ''' </summary>
  ''' <history>
  '''     [cnurse]    01/06/2006  Documented
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Private Sub RoleGroupsSelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cboRoleGroups.SelectedIndexChanged

   UpdatePermissions()

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' AddUser runs when the Add user linkbutton is clicked
  ''' </summary>
  ''' <history>
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Private Sub AddUser(sender As Object, e As System.EventArgs) Handles cmdUser.Click

   UpdatePermissions()

   If txtUser.Text <> "" Then
    ' verify username
    Dim objUser As UserInfo = UserController.GetUserByName(PortalId, txtUser.Text)
    If Not objUser Is Nothing Then
     AddPermission(_permissions, objUser)
     BindData()
    Else
     ' user does not exist
     lblUser = New Label
     lblUser.Text = "<br>" & Localization.GetString("InvalidUserName")
     lblUser.CssClass = "NormalRed"
     pnlPermissions.Controls.Add(lblUser)
    End If
   End If

  End Sub

#End Region

 End Class


End Namespace