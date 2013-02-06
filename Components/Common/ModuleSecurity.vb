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
Imports DotNetNuke.Modules.Blog.Components.Entities
Imports DotNetNuke.Entities.Users

Namespace Components.Common

    Public Class ModuleSecurity
  Private Property HasEdit As Boolean = False
  Private Property HasBlogger As Boolean = False
  Private Property HasGhost As Boolean = False
  Private Property IsOwner As Boolean = False
  Private Property BlogAuthorMode As Integer
  Private _userIsAdmin As Boolean = False

  Public Sub New(moduleId As Integer, tabId As Integer, blog As BlogInfo, user As UserInfo)
   If blog IsNot Nothing Then
    IsOwner = CBool(blog.UserID = user.UserID)
    BlogAuthorMode = blog.AuthorMode
   End If
   _userIsAdmin = DotNetNuke.Security.PortalSecurity.IsInRole(DotNetNuke.Entities.Portals.PortalSettings.Current.AdministratorRoleName)
            Dim mc As New DotNetNuke.Entities.Modules.ModuleController
            Dim objMod As New DotNetNuke.Entities.Modules.ModuleInfo
            objMod = mc.GetModule(moduleId, tabId, False)
            If objMod Is Nothing Then
                Return
            End If
            HasEdit = ModulePermissionController.CanEditModuleContent(objMod)
            HasBlogger = ModulePermissionController.HasModulePermission(objMod.ModulePermissions, Constants.BloggerPermission)
            HasGhost = ModulePermissionController.HasModulePermission(objMod.ModulePermissions, Constants.GhostWriterPermission)
        End Sub

  Public ReadOnly Property CanCreate() As Boolean
   Get
            Return HasEdit Or HasBlogger
   End Get
  End Property

  Public ReadOnly Property CanEdit() As Boolean
   Get
            Return HasEdit Or (HasBlogger AndAlso IsOwner)
   End Get
  End Property

  Public ReadOnly Property CanAddEntry() As Boolean
   Get
            Return HasEdit Or (HasGhost AndAlso BlogAuthorMode = Constants.AuthorMode.GhostMode) Or (HasBlogger AndAlso IsOwner) Or (HasBlogger AndAlso BlogAuthorMode = Constants.AuthorMode.BloggerMode)
   End Get
  End Property

  Public ReadOnly Property CanApproveComment() As Boolean
   Get
            If HasEdit Or HasBlogger Or HasGhost Then
                Return True
            Else
                Return False
            End If
   End Get
  End Property

  Public ReadOnly Property CanAddApprovedComment() As Boolean
   Get
    Return HasEdit Or HasBlogger Or HasGhost
   End Get
  End Property

  Public ReadOnly Property UserIsAdmin As Boolean
   Get
    Return _userIsAdmin
   End Get
  End Property
    End Class
End Namespace