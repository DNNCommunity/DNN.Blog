Imports DotNetNuke.Services.Localization.Localization

Namespace Controls
 Public Class ManagementPanel
  Inherits BlogContextBase

  Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

   LocalResourceFile = "~/DesktopModules/Blog/Controls/App_LocalResources/ManagementPanel.ascx.resx"
   If Not Me.IsPostBack Then
    cmdManageBlogs.Visible = Security.IsBlogger Or Security.CanApproveEntry
    cmdBlog.Visible = Security.CanAddEntry
    cmdEditPost.Visible = (Entry IsNot Nothing) And Security.CanEditEntry
    cmdCopyModule.Visible = Security.IsEditor
    wlwlink.Title = LocalizeString("WLW")
    wlwlink.Visible = Security.CanAddEntry
    If Entry IsNot Nothing Then
     cmdBlog.Text = LocalizeString("cmdEdit")
    End If
    If Security.IsEditor Then
     ddTemplate.Items.Clear()
     ddTemplate.Items.Add(New ListItem("Default [System]", "[G]_default"))
     For Each d As IO.DirectoryInfo In (New IO.DirectoryInfo(HttpContext.Current.Server.MapPath(DotNetNuke.Common.ResolveUrl(Common.Constants.glbTemplatesPath)))).GetDirectories
      If d.Name <> "_default" Then
       ddTemplate.Items.Add(New ListItem(d.Name & " [System]", "[G]" & d.Name))
      End If
     Next
     For Each d As IO.DirectoryInfo In (New IO.DirectoryInfo(Settings.PortalTemplatesMapPath)).GetDirectories
      ddTemplate.Items.Add(New ListItem(d.Name & " [Local]", "[P]" & d.Name))
     Next
     For intItem As Integer = 0 To PortalSettings.ActiveTab.Panes.Count - 1
      ddPane.Items.Add(Convert.ToString(PortalSettings.ActiveTab.Panes(intItem)))
     Next intItem
     If Not ddPane.Items.FindByValue(DotNetNuke.Common.Globals.glbDefaultPane) Is Nothing Then
      ddPane.Items.FindByValue(DotNetNuke.Common.Globals.glbDefaultPane).Selected = True
     End If
     ddPosition.Items.Clear()
     ddPosition.Items.Add(New ListItem(GetString("Top", "admin/controlpanel/App_LocalResources/iconbar"), "0"))
     ddPosition.Items.Add(New ListItem(GetString("Bottom", "admin/controlpanel/App_LocalResources/iconbar"), "-1"))
    End If
   End If

  End Sub

  Private Sub cmdManageBlogs_Click(sender As Object, e As System.EventArgs) Handles cmdManageBlogs.Click
   Response.Redirect(EditUrl("Manage"), False)
  End Sub

  Private Sub cmdBlog_Click(sender As Object, e As System.EventArgs) Handles cmdBlog.Click
   If BlogId <> -1 Then
    Response.Redirect(EditUrl("Blog", BlogId.ToString, "EntryEdit"), False)
   Else
    Response.Redirect(EditUrl("EntryEdit"), False)
   End If
  End Sub

  Private Sub cmdEditPost_Click(sender As Object, e As System.EventArgs) Handles cmdEditPost.Click
   Response.Redirect(EditUrl("Post", ContentItemId.ToString, "EntryEdit"), False)
  End Sub

  'Private Sub cmdAdd_Click(sender As Object, e As System.EventArgs) Handles cmdAdd.Click
  ' Dim newSettings As New Common.ViewSettings(TabModuleId)
  ' newSettings.BlogModuleId = Settings.ModuleId
  ' newSettings.Template = ddTemplate.SelectedValue
  ' newSettings.ShowManagementPanel = chkShowManagementPanel.Checked
  ' Common.Globals.CopyModule(PortalId, TabId, Me.ModuleConfiguration.ModuleDefID, ddPane.SelectedValue, CInt(ddPosition.SelectedValue), txtTitle.Text.Trim, newSettings)
  ' Me.Response.Redirect(DotNetNuke.Common.NavigateURL(), False)
  'End Sub
 End Class
End Namespace