Imports System.Xml
Imports DotNetNuke.Common.Utilities

Namespace Common
 Public Class ViewSettings

#Region " Private Members "
  Private _allSettings As Hashtable = Nothing
  Private _tabModuleId As Integer = -1
#End Region

#Region " Properties "
  Public Property Template As String = "[G]_default"
  Public Property ShowManagementPanel As Boolean = True
  Public Property BlogModuleId As Integer = -1
#End Region

#Region " Constructors "
  Public Sub New(tabModuleId As Integer)

   _tabModuleId = tabModuleId
   _allSettings = (New DotNetNuke.Entities.Modules.ModuleController).GetTabModuleSettings(tabModuleId)
   _allSettings.ReadValue("Template", Template)
   _allSettings.ReadValue("ShowManagementPanel", ShowManagementPanel)
   _allSettings.ReadValue("BlogModuleId", BlogModuleId)

  End Sub

  Public Shared Function GetViewSettings(tabModuleId As Integer) As ViewSettings
   Dim CacheKey As String = "TabModuleSettings" & tabModuleId.ToString
   Dim settings As ViewSettings = CType(DotNetNuke.Common.Utilities.DataCache.GetCache(CacheKey), ViewSettings)
   If settings Is Nothing Then
    settings = New ViewSettings(tabModuleId)
    DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey, settings)
   End If
   Return settings
  End Function
#End Region

#Region " Public Members "
  Public Overridable Sub UpdateSettings()
   UpdateSettings(_tabModuleId)
  End Sub

  Public Overridable Sub UpdateSettings(tabModuleId As Integer)

   Dim objModules As New DotNetNuke.Entities.Modules.ModuleController
   objModules.UpdateTabModuleSetting(tabModuleId, "Template", Template)
   objModules.UpdateTabModuleSetting(tabModuleId, "ShowManagementPanel", ShowManagementPanel.ToString)
   objModules.UpdateTabModuleSetting(tabModuleId, "BlogModuleId", BlogModuleId.ToString)

   Dim CacheKey As String = "TabModuleSettings" & tabModuleId.ToString
   DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey, Me)

  End Sub
#End Region

 End Class
End Namespace
