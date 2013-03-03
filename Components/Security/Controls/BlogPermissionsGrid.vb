Imports System
Imports System.Text
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Collections
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Users

Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Modules.Blog.Security.Permissions
Imports DotNetNuke.Modules.Blog.Common.Constants

Namespace Security.Controls

 Public Class BlogPermissionsGrid
  Inherits PermissionsGrid

#Region " Private Members "

  Private _InheritViewPermissionsFromParent As Boolean = False
  Private _BlogID As Integer = -1
  Private _TabId As Integer = -1
  Private _BlogPermissions As BlogPermissionCollection
  Private _ViewColumnIndex As Integer
  Private _BlogType As String
  Private _currentUserId As Integer = -1
  Private _userIsAdmin As Boolean = False

#End Region

#Region " Public Properties "
  Public Property UserIsAdmin() As Boolean
   Get
    Return _userIsAdmin
   End Get
   Set(value As Boolean)
    _userIsAdmin = value
   End Set
  End Property

  Public Property CurrentUserId() As Integer
   Get
    Return _currentUserId
   End Get
   Set(value As Integer)
    _currentUserId = value
   End Set
  End Property

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets and Sets the Id of the Blog
  ''' </summary>
  ''' <history>
  '''     [cnurse]    01/09/2006  Documented
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Property BlogID() As Integer
   Get
    Return _BlogID
   End Get
   Set(Value As Integer)
    _BlogID = Value
    If Not Page.IsPostBack Then
     GetBlogPermissions()
    End If
   End Set
  End Property

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets and Sets the Id of the Tab associated with this Blog
  ''' </summary>
  ''' <history>
  '''     [cnurse]    24/11/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Property TabId() As Integer
   Get
    Return _TabId
   End Get
   Set(Value As Integer)
    _TabId = Value
   End Set
  End Property

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets the BlogPermission Collection
  ''' </summary>
  ''' <history>
  '''     [cnurse]    01/09/2006  Documented
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Property Permissions() As BlogPermissionCollection
   Get
    'First Update Permissions in case they have been changed
    UpdatePermissions()

    'Return the BlogPermissions
    Return _BlogPermissions

   End Get
   Set(value As BlogPermissionCollection)
    _BlogPermissions = value
   End Set
  End Property
#End Region

#Region " Private Methods "

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets the BlogPermissions from the Data Store
  ''' </summary>
  ''' <history>
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Private Sub GetBlogPermissions()

   _BlogPermissions = BlogPermissionsController.GetBlogPermissionsCollection(Me.BlogID)

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Parse the Permission Keys used to persist the Permissions in the ViewState
  ''' </summary>
  ''' <param name="Settings">A string array of settings</param>
  ''' <param name="arrPermisions">An Arraylist to add the Permission object to</param>
  ''' <history>
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Private Sub ParsePermissionKeys(Settings As String(), arrPermisions As ArrayList)

   Dim objBlogPermission As New BlogPermissionInfo
   Dim permission As PermissionInfo = PermissionsController.GetPermission(Convert.ToInt32(Settings(1)))
   objBlogPermission.PermissionId = permission.PermissionId
   objBlogPermission.PermissionKey = permission.PermissionKey
   objBlogPermission.RoleId = Convert.ToInt32(Settings(4))

   objBlogPermission.RoleName = Settings(3)
   objBlogPermission.AllowAccess = CBool(Settings(0))
   objBlogPermission.UserId = Convert.ToInt32(Settings(5))
   objBlogPermission.DisplayName = Settings(6)

   objBlogPermission.BlogId = BlogID
   arrPermisions.Add(objBlogPermission)

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Check if the Role has the permission specified
  ''' </summary>
  ''' <param name="permissionID">The Id of the Permission to check</param>
  ''' <param name="roleid">The role id to check</param>
  ''' <history>
  '''     [cnurse]    01/09/2006  Documented
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Private Function BlogHasRolePermission(permissionID As Integer, roleid As Integer) As BlogPermissionInfo
   Dim i As Integer
   For i = 0 To _BlogPermissions.Count - 1
    Dim objBlogPermission As BlogPermissionInfo = _BlogPermissions(i)
    If permissionID = objBlogPermission.PermissionId And objBlogPermission.RoleId = roleid Then
     Return objBlogPermission
    End If
   Next
   Return Nothing
  End Function

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Check if the Role has the permission specified
  ''' </summary>
  ''' <param name="permissionID">The Id of the Permission to check</param>
  ''' <param name="userid">The user id to check</param>
  ''' <history>
  '''     [cnurse]    01/09/2006  Documented
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Private Function BlogHasUserPermission(permissionID As Integer, userid As Integer) As BlogPermissionInfo
   Dim i As Integer
   For i = 0 To _BlogPermissions.Count - 1
    Dim objBlogPermission As BlogPermissionInfo = _BlogPermissions(i)
    If permissionID = objBlogPermission.PermissionId And objBlogPermission.UserId = userid Then
     Return objBlogPermission
    End If
   Next
   Return Nothing
  End Function

#End Region

#Region " Protected Methods "

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
  Protected Overrides Function GetEnabled(objPerm As PermissionInfo, role As DotNetNuke.Security.Roles.RoleInfo, column As Integer) As Boolean

   If role.RoleID = AdministratorRoleId Then
    Return False
   Else
    Return True
   End If

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
  Protected Overrides Function GetEnabled(objPerm As PermissionInfo, user As DotNetNuke.Entities.Users.UserInfo, column As Integer) As Boolean

   Return True

  End Function

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets the Value of the permission
  ''' </summary>
  ''' <param name="objPerm">The permission being loaded</param>
  ''' <param name="role">The role</param>
  ''' <param name="column">The column of the Grid</param>
  ''' <returns>A Boolean (True or False)</returns>
  ''' <history>
  '''     [cnurse]    01/09/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Protected Overrides Function GetPermission(objPerm As PermissionInfo, role As DotNetNuke.Security.Roles.RoleInfo, column As Integer) As Boolean

   Dim permission As Boolean

   'If role.RoleID = AdministratorRoleId Then
   ' permission = True
   'Else
   Dim objBlogPermission As BlogPermissionInfo = BlogHasRolePermission(objPerm.PermissionId, role.RoleID)
   If Not objBlogPermission Is Nothing Then
    permission = objBlogPermission.AllowAccess
   Else
    permission = False
   End If
   'End If

   Return permission

  End Function

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets the Value of the permission
  ''' </summary>
  ''' <param name="objPerm">The permission being loaded</param>
  ''' <param name="user">The role</param>
  ''' <param name="column">The column of the Grid</param>
  ''' <returns>A Boolean (True or False)</returns>
  ''' <history>
  '''     [cnurse]    01/09/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Protected Overrides Function GetPermission(objPerm As PermissionInfo, user As DotNetNuke.Entities.Users.UserInfo, column As Integer) As Boolean

   Dim permission As Boolean

   Dim objBlogPermission As BlogPermissionInfo = BlogHasUserPermission(objPerm.PermissionId, user.UserID)
   If Not objBlogPermission Is Nothing Then
    permission = objBlogPermission.AllowAccess
   Else
    permission = False
   End If

   Return permission

  End Function

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets the Permissions from the Data Store
  ''' </summary>
  ''' <history>
  '''     [cnurse]    01/09/2006  Documented
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Protected Overrides Function GetPermissions() As PermissionCollection

   _ViewColumnIndex = 0
   Return PermissionsController.GetPermissions

  End Function

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets the users from the Database
  ''' </summary>
  ''' <history>
  '''     [cnurse]    01/12/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Protected Overrides Function GetUsers() As ArrayList

   Dim arrUsers As New ArrayList
   Dim objBlogPermission As BlogPermissionInfo
   Dim objUser As UserInfo
   Dim blnExists As Boolean

   For Each objBlogPermission In _BlogPermissions
    If Not objBlogPermission.UserId = glbUserNothing Then
     blnExists = False
     For Each objUser In arrUsers
      If objBlogPermission.UserId = objUser.UserID Then
       blnExists = True
      End If
     Next
     If Not blnExists Then
      objUser = New UserInfo
      objUser.UserID = objBlogPermission.UserId
      objUser.Username = objBlogPermission.Username
      objUser.DisplayName = objBlogPermission.DisplayName
      arrUsers.Add(objUser)
     End If
    End If
   Next
   Return arrUsers

  End Function

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Load the ViewState
  ''' </summary>
  ''' <param name="savedState">The saved state</param>
  ''' <history>
  '''     [cnurse]    01/12/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Protected Overrides Sub LoadViewState(savedState As Object)

   If Not (savedState Is Nothing) Then
    ' Load State from the array of objects that was saved with SaveViewState.

    Dim myState As Object() = CType(savedState, Object())

    'Load Base Controls ViewState
    If Not (myState(0) Is Nothing) Then
     MyBase.LoadViewState(myState(0))
    End If

    'Load BlogID
    If Not (myState(1) Is Nothing) Then
     BlogID = CInt(myState(1))
    End If

    If Not (myState(2) Is Nothing) Then
     CurrentUserId = CInt(myState(2))
    End If

    'Load BlogPermissions
    If Not (myState(3) Is Nothing) Then
     Dim arrPermissions As New ArrayList
     Dim state As String = CStr(myState(3))
     If state <> "" Then
      'First Break the String into individual Keys
      Dim permissionKeys As String() = Split(state, "##")
      For Each key As String In permissionKeys
       Dim Settings As String() = Split(key, "|")
       ParsePermissionKeys(Settings, arrPermissions)
      Next
     End If
     _BlogPermissions = New BlogPermissionCollection(arrPermissions)
    End If
   End If

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Saves the ViewState
  ''' </summary>
  ''' <history>
  '''     [cnurse]    01/12/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Protected Overrides Function SaveViewState() As Object

   Me.UpdatePermissions()

   Dim allStates(3) As Object

   allStates(0) = MyBase.SaveViewState()
   allStates(1) = BlogID
   allStates(2) = CurrentUserId

   'Persist the BlogPermissions
   Dim sb As New StringBuilder
   Dim addDelimiter As Boolean = False
   For Each objBlogPermission As BlogPermissionInfo In _BlogPermissions
    If addDelimiter Then
     sb.Append("##")
    Else
     addDelimiter = True
    End If
    sb.Append(BuildKey(objBlogPermission.AllowAccess, objBlogPermission.PermissionId, -1, objBlogPermission.RoleId, objBlogPermission.RoleName, objBlogPermission.UserId, objBlogPermission.DisplayName))
   Next
   allStates(3) = sb.ToString()

   Return allStates

  End Function

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Updates a Permission
  ''' </summary>
  ''' <param name="permission">The permission being updated</param>
  ''' <param name="roleName">The name of the role</param>
  ''' <param name="roleId">The id of the role</param>
  ''' <param name="allowAccess">The value of the permission</param>
  ''' <history>
  '''     [cnurse]    01/12/2006  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Protected Overrides Sub UpdatePermission(permission As PermissionInfo, roleId As Integer, roleName As String, allowAccess As Boolean)

   Dim isMatch As Boolean = False
   Dim objPermission As BlogPermissionInfo
   Dim permissionId As Integer = permission.PermissionId

   'Search BlogPermission Collection for the permission to Update
   For Each objPermission In _BlogPermissions
    If objPermission.PermissionId = permissionId And objPermission.RoleId = roleId Then
     'BlogPermission is in collection
     objPermission.AllowAccess = allowAccess
     isMatch = True
     Exit For
    End If
   Next

   'BlogPermission not found so add new
   If Not isMatch And allowAccess Then
    objPermission = New BlogPermissionInfo
    objPermission.PermissionId = permission.PermissionId
    objPermission.PermissionKey = permission.PermissionKey
    objPermission.BlogId = BlogID
    objPermission.RoleId = roleId
    objPermission.RoleName = roleName
    objPermission.AllowAccess = allowAccess
    objPermission.UserId = glbUserNothing
    objPermission.DisplayName = Null.NullString
    _BlogPermissions.Add(objPermission)
   End If

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
  Protected Overrides Sub UpdatePermission(permission As PermissionInfo, displayName As String, userId As Integer, allowAccess As Boolean)

   Dim isMatch As Boolean = False
   Dim objPermission As BlogPermissionInfo
   Dim permissionId As Integer = permission.PermissionId

   'Search BlogPermission Collection for the permission to Update
   For Each objPermission In _BlogPermissions
    If objPermission.PermissionId = permissionId And objPermission.UserId = userId Then
     objPermission.AllowAccess = allowAccess
     isMatch = True
     Exit For
    End If
   Next

   'BlogPermission not found so add new
   If Not isMatch And allowAccess Then
    objPermission = New BlogPermissionInfo
    objPermission.PermissionId = permission.PermissionId
    objPermission.PermissionKey = permission.PermissionKey
    objPermission.BlogId = BlogID
    objPermission.RoleId = glbRoleNothing
    objPermission.RoleName = Null.NullString
    objPermission.AllowAccess = allowAccess
    objPermission.UserId = userId
    objPermission.DisplayName = displayName
    _BlogPermissions.Add(objPermission)
   End If

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
  Protected Overrides Sub AddPermission(permissions As PermissionCollection, user As UserInfo)

   Dim objBlogPermission As BlogPermissionInfo

   'Search TabPermission Collection for the user 
   Dim isMatch As Boolean = False
   For Each objBlogPermission In _BlogPermissions
    If objBlogPermission.UserId = user.UserID Then
     isMatch = True
     Exit For
    End If
   Next

   'user not found so add new
   If Not isMatch Then
    Dim objPermission As PermissionInfo
    For Each objPermission In permissions.Values
     objBlogPermission = New BlogPermissionInfo
     objBlogPermission.PermissionId = objPermission.PermissionId
     objBlogPermission.PermissionKey = objPermission.PermissionKey
     objBlogPermission.BlogId = BlogID
     objBlogPermission.RoleId = glbRoleNothing
     objBlogPermission.RoleName = Null.NullString
     If objPermission.PermissionId = BlogPermissionTypes.ADD Then
      objBlogPermission.AllowAccess = True
     Else
      objBlogPermission.AllowAccess = False
     End If
     objBlogPermission.UserId = user.UserID
     objBlogPermission.DisplayName = user.DisplayName
     _BlogPermissions.Add(objBlogPermission)
    Next
   End If

  End Sub

  Protected Overrides Sub CreateChildControls()
   MyBase.CreateChildControls()
  End Sub
#End Region

#Region " Public Methods "

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Overrides the Base method to Generate the Data Grid
  ''' </summary>
  ''' <history>
  '''     [cnurse]    01/09/2006  Documented
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Overrides Sub GenerateDataGrid()

  End Sub

#End Region

 End Class

End Namespace