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

Namespace Components.Common

    Public Class ModuleSecurity

        Private ReadOnly HasEdit As Boolean
        Private ReadOnly HasBlogger As Boolean
        Private ReadOnly HasGhost As Boolean

        Public Sub New(ByVal moduleId As Integer, ByVal tabId As Integer)
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

        Public Function CanCreateBlog() As Boolean
            Return HasEdit Or HasBlogger
        End Function

        Public Function CanEditBlog(ByVal IsOwner As Boolean) As Boolean
            Return HasEdit Or (HasBlogger AndAlso IsOwner)
        End Function

        Public Function CanAddEntry(ByVal IsOwner As Boolean, ByVal BlogPermitsGhost As Boolean) As Boolean
            Return HasEdit Or (HasGhost AndAlso BlogPermitsGhost) Or (HasBlogger AndAlso IsOwner)
        End Function

        <Obsolete("This method is deprecated, use specific functions instead (CanCreateBlog, CanEditBlog, CanAddEntry, etc.)")> _
        Public Shared Function HasBlogPermission(ByVal UserID As Integer, ByVal BlogUserID As Integer, ByVal ModuleID As Integer) As Boolean
            Dim PortalSettings As DotNetNuke.Entities.Portals.PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), DotNetNuke.Entities.Portals.PortalSettings)
            ' 11/19/2008 Rip Rowan
            ' Added following 2 lines & replaced deprecated HasEditPermissions function
            Dim mController As New DotNetNuke.Entities.Modules.ModuleController
            Dim ModuleConfiguration As DotNetNuke.Entities.Modules.ModuleInfo = mController.GetModule(ModuleID, DotNetNuke.Common.Utilities.Null.NullInteger)

            If ((UserID = BlogUserID And DotNetNuke.Security.PortalSecurity.HasNecessaryPermission(SecurityAccessLevel.Edit, PortalSettings, ModuleConfiguration)) Or DotNetNuke.Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName)) Then
                Return True
            Else
                Return False
            End If
        End Function

    End Class
End Namespace