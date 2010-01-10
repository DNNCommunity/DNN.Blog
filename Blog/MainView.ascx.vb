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
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Services.Exceptions.Exceptions
Imports DotNetNuke.Services.Localization.Localization

Partial Public Class MainView
 Inherits BlogModuleBase
 Implements Entities.Modules.IActionable

#Region " IActionable "
 Public ReadOnly Property ModuleActions() As DotNetNuke.Entities.Modules.Actions.ModuleActionCollection Implements DotNetNuke.Entities.Modules.IActionable.ModuleActions
  Get
   Return MyActions
  End Get
 End Property
#End Region

#Region "Private Methods"
 Private Function resolveParams(ByVal params As System.Collections.Specialized.NameValueCollection) As String
  Dim sRet As String = BlogModuleBase.CONTROL_VIEW_VIEWBLOG
  RssView = RssViews.None
  If Not Request.Params("EntryId") Is Nothing And Request.Params("BlogDate") Is Nothing Then
   sRet = BlogModuleBase.CONTROL_VIEW_VIEWENTRY
  ElseIf Not Request.Params("RssId") Is Nothing And Request.Params("BlogDate") Is Nothing Then
   If Int32.Parse(Request.Params("RssId")) = 0 Then
    RssView = RssViews.RecentEntries
   Else
    RssView = RssViews.BlogEntries
   End If
   sRet = BlogModuleBase.CONTROL_VIEW_BLOGFEED
  ElseIf Not Request.Params("rssentryid") Is Nothing Then
   sRet = BlogModuleBase.CONTROL_VIEW_BLOGFEED
   RssView = RssViews.SingleEntry
  ElseIf Not Request.Params("rssdate") Is Nothing Then
   sRet = BlogModuleBase.CONTROL_VIEW_BLOGFEED
   RssView = RssViews.ArchivEntries
  End If
  Return sRet
 End Function
#End Region

#Region "Event Handlers"
 Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
  Dim moduleControl As String
  moduleControl = resolveParams(Request.Params)
  Select Case moduleControl

   Case BlogModuleBase.CONTROL_VIEW_BLOGFEED
    Dim BlogFeedCtl As BlogFeed = CType(Me.LoadControl(Me.ModulePath & "BlogFeed.ascx"), BlogFeed)
    BlogFeedCtl.SetModuleConfiguration(ModuleConfiguration)
    BlogFeedCtl.ID = System.IO.Path.GetFileNameWithoutExtension(Me.ModulePath & "BlogFeed.ascx")
    Controls.Add(BlogFeedCtl)
    ModuleConfiguration = BlogFeedCtl.ModuleConfiguration

   Case BlogModuleBase.CONTROL_VIEW_VIEWBLOG
    Dim ViewBlogCtl As ViewBlog = CType(Me.LoadControl(Me.ModulePath & "ViewBlog.ascx"), ViewBlog)
    ViewBlogCtl.SetModuleConfiguration(ModuleConfiguration)
    ViewBlogCtl.ID = System.IO.Path.GetFileNameWithoutExtension(Me.ModulePath & "ViewBlog.ascx")
    Controls.Add(ViewBlogCtl)
    ModuleConfiguration = ViewBlogCtl.ModuleConfiguration
    'DR-01/30/2009-BLG-8538
    For Each action As ModuleAction In ViewBlogCtl.MyActions
     action.ID = GetNextActionID()
     Me.MyActions.Add(action)
    Next

   Case BlogModuleBase.CONTROL_VIEW_VIEWENTRY
    Dim ViewEntryCtl As ViewEntry = CType(Me.LoadControl(Me.ModulePath & "ViewEntry.ascx"), ViewEntry)
    ViewEntryCtl.SetModuleConfiguration(ModuleConfiguration)
    ViewEntryCtl.ID = System.IO.Path.GetFileNameWithoutExtension(Me.ModulePath & "ViewEntry.ascx")
    Controls.Add(ViewEntryCtl)
    ModuleConfiguration = ViewEntryCtl.ModuleConfiguration
    'DR-01/30/2009-BLG-8538
    For Each action As ModuleAction In ViewEntryCtl.MyActions
     action.ID = GetNextActionID()
     Me.MyActions.Add(action)
    Next

   Case Else
    Dim ViewBlogCtl As ViewBlog = CType(Me.LoadControl(Me.ModulePath & "ViewBlog.ascx"), ViewBlog)
    ViewBlogCtl.SetModuleConfiguration(ModuleConfiguration)
    ViewBlogCtl.ID = System.IO.Path.GetFileNameWithoutExtension(Me.ModulePath & "ViewBlog.ascx")
    Controls.Add(ViewBlogCtl)
    ModuleConfiguration = ViewBlogCtl.ModuleConfiguration
    'DR-01/30/2009-BLG-8538
    For Each action As ModuleAction In ViewBlogCtl.MyActions
     action.ID = GetNextActionID()
     Me.MyActions.Add(action)
    Next

  End Select

  If Not Me.IsPostBack Then
   If DotNetNuke.Security.Permissions.TabPermissionController.HasTabPermission("EDIT") Then
    pnlAddModuleDefs.Visible = True
    ddModuleDef.DataSource = (New DotNetNuke.Entities.Modules.Definitions.ModuleDefinitionController).GetModuleDefinitions(Me.ModuleConfiguration.DesktopModuleID)
    ddModuleDef.DataBind()
    ddModuleDef.Items.Insert(0, New ListItem(GetString("Select", LocalResourceFile), "-1"))
    ddModuleDef.Items.Remove(ddModuleDef.Items.FindByText("View_Blog"))
    Dim intItem As Integer
    For intItem = 0 To PortalSettings.ActiveTab.Panes.Count - 1
     ddPane.Items.Add(Convert.ToString(PortalSettings.ActiveTab.Panes(intItem)))
    Next intItem
    If Not ddPane.Items.FindByValue(DotNetNuke.Common.Globals.glbDefaultPane) Is Nothing Then
     ddPane.Items.FindByValue(DotNetNuke.Common.Globals.glbDefaultPane).Selected = True
    End If
    ddPosition.Items.Clear()
    ddPosition.Items.Add(New ListItem(GetString("Top", "admin/controlpanel/App_LocalResources/iconbar"), "0"))
    ddPosition.Items.Add(New ListItem(GetString("Bottom", "admin/controlpanel/App_LocalResources/iconbar"), "-1"))
    txtTitle.Text = GetString("Title", LocalResourceFile)
   End If
  End If

 End Sub

 Private Sub cmdAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAdd.Click
  If CInt(ddModuleDef.SelectedValue) = -1 Then Exit Sub
  Globals.AddModDef(PortalSettings, CInt(ddModuleDef.SelectedValue), TabId, ddPane.SelectedValue, CInt(ddPosition.SelectedValue), txtTitle.Text.Trim)
  Me.Response.Redirect(DotNetNuke.Common.NavigateURL(), False)
 End Sub
#End Region

End Class