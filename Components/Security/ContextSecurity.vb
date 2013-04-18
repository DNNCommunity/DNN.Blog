'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2012
' by DotNetNuke Corporation
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
Imports DotNetNuke.Security
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Modules.Blog.Entities
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Security.Security
Imports DotNetNuke.Services.Tokens

Namespace Security

 Public Class ContextSecurity
  Implements IPropertyAccess

#Region " Private Members "
  Private _canAdd As Boolean = False
  Private _canEdit As Boolean = False
  Private _canApprove As Boolean = False
  Private _canAddComment As Boolean = False
  Private _canApproveComment As Boolean = False
  Private _canViewComment As Boolean = False
  Private _userIsAdmin As Boolean = False
  Private _isBlogger As Boolean = False
  Private _isEditor As Boolean = False
#End Region

#Region " Constructor "
  Public Sub New(moduleId As Integer, tabId As Integer, blog As BlogInfo, user As UserInfo)
   If blog IsNot Nothing Then
    IsOwner = CBool(blog.CreatedByUserID = user.UserID)
    _canAdd = blog.CanAdd
    _canEdit = blog.CanEdit
    _canApprove = blog.CanApprove
    _canViewComment = blog.Permissions.CurrentUserHasPermission("VIEWCOMMENT")
    _canApproveComment = blog.Permissions.CurrentUserHasPermission("APPROVECOMMENT")
    _canAddComment = blog.Permissions.CurrentUserHasPermission("ADDCOMMENT")
   Else
    Using ir As IDataReader = Data.DataProvider.Instance().GetUserPermissionsByModule(moduleId, user.UserID)
     Do While ir.Read
      Dim permissionId As Integer = CInt(ir.Item("PermissionId"))
      Dim hasPermission As Integer = CInt(ir.Item("HasPermission"))
      If hasPermission > 0 Then
       Select Case permissionId
        Case BlogPermissionTypes.ADD
         _canAdd = True
        Case BlogPermissionTypes.EDIT
         _canEdit = True
        Case BlogPermissionTypes.APPROVE
         _canApprove = True
       End Select
      End If
     Loop
    End Using
   End If
   _userIsAdmin = DotNetNuke.Security.PortalSecurity.IsInRole(DotNetNuke.Entities.Portals.PortalSettings.Current.AdministratorRoleName)
   Dim mc As New DotNetNuke.Entities.Modules.ModuleController
   Dim objMod As New DotNetNuke.Entities.Modules.ModuleInfo
   objMod = mc.GetModule(moduleId, tabId, False)
   If objMod IsNot Nothing Then
    _isBlogger = ModulePermissionController.HasModulePermission(objMod.ModulePermissions, BloggerPermission)
    _isEditor = ModulePermissionController.HasModulePermission(objMod.ModulePermissions, "EDIT")
   End If
  End Sub
#End Region

#Region " Public Properties "
  Public Property IsOwner As Boolean = False

  Public ReadOnly Property CanEditPost() As Boolean
   Get
    Return _canEdit Or IsOwner Or UserIsAdmin
   End Get
  End Property

  Public ReadOnly Property CanAddPost() As Boolean
   Get
    Return _canAdd Or IsOwner Or UserIsAdmin
   End Get
  End Property

  Public ReadOnly Property CanAddComment() As Boolean
   Get
    Return _canAddComment Or IsOwner Or UserIsAdmin
   End Get
  End Property

  Public ReadOnly Property CanViewComments() As Boolean
   Get
    Return _canViewComment Or IsOwner Or UserIsAdmin
   End Get
  End Property

  Public ReadOnly Property CanApprovePost() As Boolean
   Get
    Return _canApprove Or IsOwner Or UserIsAdmin
   End Get
  End Property

  Public ReadOnly Property CanApproveComment() As Boolean
   Get
    Return _canApproveComment Or IsOwner Or UserIsAdmin
   End Get
  End Property

  Public ReadOnly Property CanDoSomethingWithPosts As Boolean
   Get
    Return _canEdit Or _canAdd Or _canApprove Or UserIsAdmin
   End Get
  End Property

  Public ReadOnly Property UserIsAdmin As Boolean
   Get
    Return _userIsAdmin
   End Get
  End Property

  Public ReadOnly Property IsBlogger As Boolean
   Get
    Return _isBlogger
   End Get
  End Property

  Public ReadOnly Property IsEditor As Boolean
   Get
    Return _isEditor
   End Get
  End Property
#End Region

#Region " IPropertyAccess Implementation "
  Public Function GetProperty(strPropertyName As String, strFormat As String, formatProvider As System.Globalization.CultureInfo, AccessingUser As DotNetNuke.Entities.Users.UserInfo, AccessLevel As DotNetNuke.Services.Tokens.Scope, ByRef PropertyNotFound As Boolean) As String Implements DotNetNuke.Services.Tokens.IPropertyAccess.GetProperty
   Dim OutputFormat As String = String.Empty
   Dim portalSettings As DotNetNuke.Entities.Portals.PortalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings()
   If strFormat = String.Empty Then
    OutputFormat = "D"
   Else
    OutputFormat = strFormat
   End If
   Select Case strPropertyName.ToLower
    Case "isowner"
     Return Me.IsOwner.ToString
    Case "isowneryesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.IsOwner, formatProvider)
    Case "caneditpost"
     Return Me.CanEditPost.ToString
    Case "caneditpostyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.CanEditPost, formatProvider)
    Case "canaddpost"
     Return Me.CanAddPost.ToString
    Case "canaddpostyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.CanAddPost, formatProvider)
    Case "canaddcomment"
     Return Me.CanAddComment.ToString
    Case "canaddcommentyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.CanAddComment, formatProvider)
    Case "canviewcomments"
     Return Me.CanViewComments.ToString
    Case "canviewcommentsyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.CanViewComments, formatProvider)
    Case "canapprovepost"
     Return Me.CanApprovePost.ToString
    Case "canapprovepostyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.CanApprovePost, formatProvider)
    Case "canapprovecomment"
     Return Me.CanApproveComment.ToString
    Case "canapprovecommentyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.CanApproveComment, formatProvider)
    Case "candosomethingwithposts"
     Return Me.CanDoSomethingWithPosts.ToString
    Case "candosomethingwithpostsyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.CanDoSomethingWithPosts, formatProvider)
    Case "userisadmin"
     Return Me.UserIsAdmin.ToString
    Case "userisadminyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.UserIsAdmin, formatProvider)
    Case "isblogger"
     Return Me.IsBlogger.ToString
    Case "isbloggeryesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.IsBlogger, formatProvider)
    Case "iseditor"
     Return Me.IsEditor.ToString
    Case "iseditoryesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.IsEditor, formatProvider)
    Case Else
     PropertyNotFound = True
   End Select
   Return DotNetNuke.Common.Utilities.Null.NullString
  End Function

  Public ReadOnly Property Cacheability() As DotNetNuke.Services.Tokens.CacheLevel Implements DotNetNuke.Services.Tokens.IPropertyAccess.Cacheability
   Get
    Return CacheLevel.fullyCacheable
   End Get
  End Property
#End Region

 End Class
End Namespace