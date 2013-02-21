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
Imports DotNetNuke.Modules.Blog.Common
Imports DotNetNuke.Modules.Blog.Settings

Partial Public Class ViewTagsSettings
 Inherits DotNetNuke.Entities.Modules.ModuleSettingsBase

#Region "Private Members"

 Private _settings As TagViewSettings

#End Region

#Region "Event Handlers"

 Protected Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
  _settings = TagViewSettings.GetTagViewSettings(TabModuleId)
 End Sub

#End Region

#Region "Base Methods"

 Public Overrides Sub LoadSettings()
  Try
   If (Page.IsPostBack = False) Then
    BindTelerikSkins()

    ddlDisplayMode.SelectedValue = _settings.TagDisplayMode
    ddlTelerikSkin.SelectedValue = _settings.CloudSkin
   End If
  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 Public Overrides Sub UpdateSettings()
  Try
   _settings.TagDisplayMode = ddlDisplayMode.SelectedValue
   _settings.CloudSkin = ddlTelerikSkin.SelectedValue

   _settings.UpdateSettings()

  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

#End Region

#Region "Private Methods"

 Private Sub BindTelerikSkins()
  ddlTelerikSkin.Items.Insert(0, New ListItem("Default", "Default"))
  ddlTelerikSkin.Items.Insert(1, New ListItem("Black", "Black"))
  ddlTelerikSkin.Items.Insert(2, New ListItem("Forest", "Forest"))
  ddlTelerikSkin.Items.Insert(3, New ListItem("Hay", "Hay"))
  ddlTelerikSkin.Items.Insert(4, New ListItem("Metro", "Metro"))
  ddlTelerikSkin.Items.Insert(5, New ListItem("Office2007", "Office2007"))
  ddlTelerikSkin.Items.Insert(6, New ListItem("Office2010Black", "Office2010Black"))
  ddlTelerikSkin.Items.Insert(7, New ListItem("Office2010Blue", "Office2010Blue"))
  ddlTelerikSkin.Items.Insert(8, New ListItem("Office2010Silver", "Office2010Silver"))
  ddlTelerikSkin.Items.Insert(9, New ListItem("Outlook", "Outlook"))
  ddlTelerikSkin.Items.Insert(10, New ListItem("Simple", "Simple"))
  ddlTelerikSkin.Items.Insert(11, New ListItem("Sitefinity", "Sitefinity"))
  ddlTelerikSkin.Items.Insert(12, New ListItem("Sunset", "Sunset"))
  ddlTelerikSkin.Items.Insert(13, New ListItem("Telerik", "Telerik"))
  ddlTelerikSkin.Items.Insert(14, New ListItem("Transparent", "Transparent"))
  ddlTelerikSkin.Items.Insert(15, New ListItem("Vista", "Vista"))
  ddlTelerikSkin.Items.Insert(16, New ListItem("Web20", "Web20"))
  ddlTelerikSkin.Items.Insert(17, New ListItem("WebBlue", "WebBlue"))
  ddlTelerikSkin.Items.Insert(18, New ListItem("Windows7", "Windows7"))
 End Sub

#End Region

End Class