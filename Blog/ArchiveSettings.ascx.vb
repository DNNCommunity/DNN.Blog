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
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Modules.Blog.Components.Settings

Partial Public Class ArchiveSettings
    Inherits Entities.Modules.ModuleSettingsBase

#Region "Private Members"

    Private _settings As ArchiveViewSettings

#End Region

#Region "Base Methods"

    Public Overrides Sub LoadSettings()
        Try
            If (Page.IsPostBack = False) Then
                'chkEnableArchiveDropDown.Checked = _settings.EnableArchiveDropDown
                'rblLoadCss.SelectedValue = _settings.TagDisplayMode

            End If
        Catch exc As Exception
            ProcessModuleLoadException(Me, exc)
        End Try
    End Sub

    Public Overrides Sub UpdateSettings()
        Try
            Dim objModule As ModuleController = New ModuleController

            'objModule.UpdateTabModuleSetting(ModuleContext.TabModuleId, "SettingName", chkEnableArchiveDropDown.Checked.ToString())
            'objModule.UpdateTabModuleSetting(ModuleContext.TabModuleId, "SettingName", rblLoadCss.SelectedValue)

        Catch exc As Exception
            ProcessModuleLoadException(Me, exc)
        End Try
    End Sub

#End Region

End Class