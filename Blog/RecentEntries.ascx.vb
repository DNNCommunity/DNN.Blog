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

Imports System
Imports DotNetNuke.Modules.Blog.Business
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports System.Reflection
Imports DotNetNuke.Security

Partial Public Class RecentEntries
 Inherits BlogModuleBase

 Private _settings As Settings.RecentEntriesSettings

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
  _settings = DotNetNuke.Modules.Blog.Settings.RecentEntriesSettings.GetRecentEntriesSettings(TabModuleId)
  LoadRecentEntries()
 End Sub

 Private Sub LoadRecentEntries()
  Dim RecentEntries As ArrayList = Nothing
  Dim oController As EntryController = Nothing
  Dim strTemplate As String
  Dim strBuilder As StringBuilder = Nothing
  Dim strRecentEntries As String = Nothing
  Try
   oController = New EntryController

   If Request.QueryString("BlogID") IsNot Nothing Then
    RecentEntries = oController.ListEntriesByBlog(CInt(Request.QueryString("BlogID")), Nothing, PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()), PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()), _settings.RecentEntriesMax)
   Else
    RecentEntries = oController.ListEntriesByPortal(PortalId, Nothing, Nothing, PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()), PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()), _settings.RecentEntriesMax)
   End If

   If RecentEntries IsNot Nothing AndAlso RecentEntries.Count > 0 Then
    strBuilder = New StringBuilder
    For Each Entry As EntryInfo In RecentEntries

     strTemplate = _settings.RecentEntriesTemplate
     strTemplate = ProcessTemplate(Entry, strTemplate)
     strBuilder.Append(strTemplate)

    Next

   Else
    UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("MsgNoRecentEntries", LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
   End If

   ' assign the content
   If strBuilder IsNot Nothing Then
    Dim lblContent As New Label
    lblContent.Text = strBuilder.ToString
    Me.RecentEntries.Controls.Add(lblContent)
   End If

  Catch ex As Exception
   ProcessModuleLoadException(Me, ex)
  Finally
   If RecentEntries IsNot Nothing Then RecentEntries = Nothing
   If oController IsNot Nothing Then oController = Nothing
   If strBuilder IsNot Nothing Then strBuilder = Nothing
  End Try
 End Sub

 Private Function ProcessTemplate(ByVal objEntry As EntryInfo, ByVal strTemplate As String) _
As String
  Dim TemplateManager As TemplateManager = Nothing
  Try
   TemplateManager = New TemplateManager(objEntry)
   Return TemplateManager.ProcessTemplate(strTemplate)
  Catch ex As Exception
   ProcessModuleLoadException(Me, ex)
   Return Nothing
  Finally
   If TemplateManager IsNot Nothing Then TemplateManager = Nothing
  End Try
 End Function

End Class
