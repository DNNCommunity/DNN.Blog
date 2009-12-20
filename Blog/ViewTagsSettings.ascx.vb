
Imports DotNetNuke.Modules.Blog.Business
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization



Partial Public Class ViewTagsSettings
 Inherits Entities.Modules.ModuleSettingsBase

 Public Overrides Sub LoadSettings()
  Try
   If (Page.IsPostBack = False) Then

    If CType(TabModuleSettings("TagDisplayMode"), String) <> "" Then
     rblTagDisplayMode.SelectedValue = CType(TabModuleSettings("TagDisplayMode"), String)
    End If

   End If
  Catch exc As Exception           'Module failed to load
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 Public Overrides Sub UpdateSettings()
  Try
   If Not rblTagDisplayMode.SelectedValue Is Nothing Then
    Dim objModules As New Entities.Modules.ModuleController
    With objModules
     .UpdateTabModuleSetting(TabModuleId, "TagDisplayMode", rblTagDisplayMode.SelectedValue)
    End With
   End If
  Catch exc As Exception           'Module failed to load
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

End Class


