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

Imports System
Imports DotNetNuke.Modules.Blog.Business
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Services.Localization.Localization
Imports DotNetNuke.Services.Localization

Partial Public Class MainView
    Inherits BlogModuleBase
    Implements Entities.Modules.IActionable

#Region "IActionable"

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

    Private Function GetModuleDefinitions() As ArrayList
        Dim mdc As New ModuleDefinitionController
        Dim definitions As New ArrayList

        Dim colDefinitions As Dictionary(Of String, ModuleDefinitionInfo) = ModuleDefinitionController.GetModuleDefinitionsByDesktopModuleID(ModuleConfiguration.DesktopModuleID)

        For Each pair As KeyValuePair(Of String, ModuleDefinitionInfo) In colDefinitions
            If pair.Key <> "View_Blog" Then
                Dim mdi As New ModuleDefinitionInfo()

                mdi.FriendlyName = Localization.GetString(pair.Value.FriendlyName, Me.LocalResourceFile)
                mdi.ModuleDefID = pair.Value.ModuleDefID
                definitions.Add(mdi)
            End If
        Next
        Return definitions
    End Function

#End Region

#Region "Event Handlers"

    Protected Overloads Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        Dim moduleControl As String
        moduleControl = resolveParams(Request.Params)
        Select Case moduleControl

            Case BlogModuleBase.CONTROL_VIEW_BLOGFEED
                Dim BlogFeedCtl As BlogFeed = CType(Me.LoadControl(Me.ControlPath & "BlogFeed.ascx"), BlogFeed)
                BlogFeedCtl.SetModuleConfiguration(ModuleConfiguration)
                BlogFeedCtl.ID = System.IO.Path.GetFileNameWithoutExtension(Me.ControlPath & "BlogFeed.ascx")
                Controls.Add(BlogFeedCtl)
                ModuleConfiguration = BlogFeedCtl.ModuleConfiguration

            Case BlogModuleBase.CONTROL_VIEW_VIEWBLOG
                Dim ViewBlogCtl As ViewBlog = CType(Me.LoadControl(Me.ControlPath & "ViewBlog.ascx"), ViewBlog)
                ViewBlogCtl.SetModuleConfiguration(ModuleConfiguration)
                ViewBlogCtl.ID = System.IO.Path.GetFileNameWithoutExtension(Me.ControlPath & "ViewBlog.ascx")
                Controls.Add(ViewBlogCtl)
                ModuleConfiguration = ViewBlogCtl.ModuleConfiguration
                'DR-01/30/2009-BLG-8538
                For Each action As ModuleAction In ViewBlogCtl.MyActions
                    action.ID = GetNextActionID()
                    Me.MyActions.Add(action)
                Next

            Case BlogModuleBase.CONTROL_VIEW_VIEWENTRY
                Dim ViewEntryCtl As ViewEntry = CType(Me.LoadControl(Me.ControlPath & "ViewEntry.ascx"), ViewEntry)
                ViewEntryCtl.SetModuleConfiguration(ModuleConfiguration)
                ViewEntryCtl.ID = System.IO.Path.GetFileNameWithoutExtension(Me.ControlPath & "ViewEntry.ascx")
                Controls.Add(ViewEntryCtl)
                ModuleConfiguration = ViewEntryCtl.ModuleConfiguration
                'DR-01/30/2009-BLG-8538
                For Each action As ModuleAction In ViewEntryCtl.MyActions
                    action.ID = GetNextActionID()
                    Me.MyActions.Add(action)
                Next

            Case Else
                Dim ViewBlogCtl As ViewBlog = CType(Me.LoadControl(Me.ControlPath & "ViewBlog.ascx"), ViewBlog)
                ViewBlogCtl.SetModuleConfiguration(ModuleConfiguration)
                ViewBlogCtl.ID = System.IO.Path.GetFileNameWithoutExtension(Me.ControlPath & "ViewBlog.ascx")
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
                ddModuleDef.DataSource = GetModuleDefinitions()
                ddModuleDef.DataBind()
                ddModuleDef.Items.Insert(0, New ListItem(GetString("Select", LocalResourceFile), "-1"))
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

            liAddPart.Visible = ModuleContext.IsEditable
            pnlAddModuleDefs.Visible = ModuleContext.IsEditable

            Dim objSecurity As New Blog.Business.Security
            Dim hasGhostWriter As Boolean = objSecurity.HasGhostWriterPerms(ModuleContext.PortalSettings.UserId, ModuleContext.ModuleId, ModuleContext.TabId)

            If ModuleContext.IsEditable Or hasGhostWriter Then
                Dim cntBlog As New BlogController
                Dim objBlog As BlogInfo

                objBlog = cntBlog.GetBlogByUserID(ModuleContext.PortalId, ModuleContext.PortalSettings.UserId)

                If objBlog Is Nothing Then
                    ' need to see if the user is a ghost writer
                    If hasGhostWriter Then
                        ' check the url
                        If Request.QueryString("BlogID") IsNot Nothing Then
                            Dim blogId As Integer = Convert.ToInt32(Request.QueryString("BlogID"))
                            objBlog = cntBlog.GetBlog(blogId)

                            liCreateBlog.Visible = ModuleContext.IsEditable
                            hlCreateBlog.NavigateUrl = EditUrl("BlogID", "-1", "Edit_Blog")
                            liView.Visible = ModuleContext.IsEditable
                            hlView.NavigateUrl = ModuleContext.NavigateUrl(ModuleContext.TabId, "", False, "BlogID=" & objBlog.BlogID)
                            liEditBlog.Visible = ModuleContext.IsEditable
                            hlEditBlog.NavigateUrl = EditUrl("BlogID", objBlog.BlogID.ToString(), "Edit_Blog")

                            If objBlog IsNot Nothing Then
                                If objBlog.EnableGhostWriter Then
                                    liAddEntry.Visible = True
                                    hlAddEntry.NavigateUrl = EditUrl("", "", "Edit_Entry")
                                    pnlAddModuleDefs.Visible = True
                                End If
                            End If
                        Else
                            If Request.QueryString("EntryId") IsNot Nothing Then
                                Dim cntEntry As New EntryController
                                Dim entryId As Integer = Convert.ToInt32(Request.QueryString("EntryId"))
                                Dim objEntry As EntryInfo = cntEntry.GetEntry(entryId, PortalId)

                                If objEntry IsNot Nothing Then
                                    objBlog = cntBlog.GetBlog(objEntry.BlogID)
                                    If objBlog IsNot Nothing Then
                                        If objBlog.EnableGhostWriter Then
                                            liAddEntry.Visible = True
                                            hlAddEntry.NavigateUrl = EditUrl("", "", "Edit_Entry")
                                            pnlAddModuleDefs.Visible = True
                                        End If
                                    End If
                                End If
                            Else
                                liCreateBlog.Visible = ModuleContext.IsEditable
                                hlCreateBlog.NavigateUrl = EditUrl("BlogID", "-1", "Edit_Blog")
                            End If
                        End If
                    End If
                Else
                    liView.Visible = True
                    hlView.NavigateUrl = ModuleContext.NavigateUrl(ModuleContext.TabId, "", False, "BlogID=" & objBlog.BlogID)

                    liEditBlog.Visible = True
                    hlEditBlog.NavigateUrl = EditUrl("BlogID", objBlog.BlogID.ToString(), "Edit_Blog")

                    liAddEntry.Visible = True
                    hlAddEntry.NavigateUrl = EditUrl("", "", "Edit_Entry")

                    ViewState("BlogID") = objBlog.BlogID
                End If
            Else
                If BlogSettings.PageBlogs = -1 Then
                    liCreateBlog.Visible = ModuleContext.IsEditable
                    hlCreateBlog.NavigateUrl = EditUrl("BlogID", "-1", "Edit_Blog")
                End If
            End If
        End If
    End Sub

    Protected Sub cmdAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If (Convert.ToInt32(ddModuleDef.SelectedValue) = -1) Then Exit Sub
        Globals.AddModDef(PortalSettings, CInt(ddModuleDef.SelectedValue), TabId, ddPane.SelectedValue, CInt(ddPosition.SelectedValue), txtTitle.Text.Trim)
        Me.Response.Redirect(DotNetNuke.Common.NavigateURL(), False)
    End Sub

#End Region

End Class