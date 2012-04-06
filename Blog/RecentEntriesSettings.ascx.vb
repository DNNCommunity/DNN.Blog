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

Imports DotNetNuke.Services.Exceptions

Partial Class RecentEntriesSettings
 Inherits Entities.Modules.ModuleSettingsBase

 Private _settings As Settings.RecentEntriesSettings

 Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
  _settings = DotNetNuke.Modules.Blog.Settings.RecentEntriesSettings.GetRecentEntriesSettings(TabModuleId)
 End Sub

#Region "Base Method Implementations"

 Public Overrides Sub LoadSettings()
  Try
   If (Page.IsPostBack = False) Then

    txtTemplate.Text = _settings.RecentEntriesTemplate
    txtMaxCount.Text = _settings.RecentEntriesMax.ToString

   End If
  Catch exc As Exception           'Module failed to load
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 Public Overrides Sub UpdateSettings()
  Try

   _settings.RecentEntriesTemplate = txtTemplate.Text
   _settings.RecentEntriesMax = CInt(txtMaxCount.Text)
   _settings.UpdateSettings()

  Catch exc As Exception           'Module failed to load
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

#End Region

End Class