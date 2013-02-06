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

Imports DotNetNuke.Entities.Modules

Namespace Components.Common

    Public Enum RssViews
        None = 0
        ArchivEntries = 1
        BlogEntries = 2
        RecentEntries = 3
        SingleEntry = 4
    End Enum

    Public Class Globals
        Public Const glbSharedResourceFile As String = "DesktopModules/Blog/App_LocalResources/SharedResources"

        Public Shared Function RemoveMarkup(ByVal input As String) As String
            If String.IsNullOrEmpty(input) Then Return ""
            Return Regex.Replace(input, "<[^>]+>", "")
        End Function

#Region " Other "
        Public Shared Function CYesNo(ByVal value As Boolean) As String
            If value Then
                Return "Yes"
            Else
                Return "No"
            End If
        End Function

        Public Shared Sub AddModDef(ByVal PortalSettings As DotNetNuke.Entities.Portals.PortalSettings, ByVal ModuleDefID As Integer, ByVal TabID As Integer, ByVal paneName As String, ByVal position As Integer, ByVal title As String)

            Dim objModuleDefinition As Definitions.ModuleDefinitionInfo = Definitions.ModuleDefinitionController.GetModuleDefinitionByID(ModuleDefID)
            Dim objTabPermissions As Security.Permissions.TabPermissionCollection = Security.Permissions.TabPermissionController.GetTabPermissions(TabID, PortalSettings.PortalId)
            Dim objModule As New ModuleInfo
            objModule.Initialize(PortalSettings.PortalId)
            objModule.PortalID = PortalSettings.PortalId
            objModule.TabID = PortalSettings.ActiveTab.TabID
            objModule.ModuleOrder = position
            If title = "" Then
                objModule.ModuleTitle = objModuleDefinition.FriendlyName
            Else
                objModule.ModuleTitle = title
            End If
            objModule.PaneName = paneName
            objModule.ModuleDefID = objModuleDefinition.ModuleDefID
            objModule.InheritViewPermissions = True

            ' get the default module view permissions
            Dim arrSystemModuleViewPermissions As ArrayList = (New DotNetNuke.Security.Permissions.PermissionController).GetPermissionByCodeAndKey("SYSTEM_MODULE_DEFINITION", "VIEW")

            objModule.AllTabs = False
            objModule.Alignment = ""
            Dim objModules As New ModuleController
            objModules.AddModule(objModule)

        End Sub

  Public Shared Function GetLocalAddedTime(ByVal AddedDate As DateTime, ByVal PortalId As Integer, ByVal user As DotNetNuke.Entities.Users.UserInfo) As DateTime
   Return TimeZoneInfo.ConvertTimeToUtc(AddedDate, user.Profile.PreferredTimeZone)
  End Function

  Public Shared Function ManifestFilePath(portalId As Integer) As String
   Return "/DesktopModules/Blog/WLWManifest.aspx?PortalID=" & portalId.ToString
  End Function
#End Region

    End Class

End Namespace