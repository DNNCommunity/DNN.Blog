'
' DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2010
' by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
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
'-------------------------------------------------------------------------

Imports DotNetNuke.Modules.Blog.Business
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization

Partial Class RecentCommentsSettings
 Inherits Entities.Modules.ModuleSettingsBase

#Region "Base Method Implementations"

 Public Overrides Sub LoadSettings()
  Try
   If (Page.IsPostBack = False) Then

    If CType(TabModuleSettings("RecentCommentsTemplate"), String) <> "" Then
     txtTemplate.Text = CType(TabModuleSettings("RecentCommentsTemplate"), String)
    Else
     txtTemplate.Text = Localization.GetString("DefaultRecentCommentsTemplate", BlogModuleBase.BLOG_TEMPLATES_RESOURCE)
    End If

    If TabModuleSettings("RecentEntriesMax") IsNot Nothing Then
     txtMaxCount.Text = CType(TabModuleSettings("RecentEntriesMax"), String)
    Else
     txtMaxCount.Text = "10"
    End If

   End If
  Catch exc As Exception           'Module failed to load
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 Public Overrides Sub UpdateSettings()
  Try

   Dim objModules As New Entities.Modules.ModuleController
   With objModules
    .UpdateTabModuleSetting(TabModuleId, "RecentCommentsTemplate", txtTemplate.Text)
    If Utility.IsInteger(txtMaxCount.Text) Then
     .UpdateTabModuleSetting(TabModuleId, "RecentEntriesMax", txtMaxCount.Text)
    End If
   End With

  Catch exc As Exception           'Module failed to load
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

#End Region

End Class


