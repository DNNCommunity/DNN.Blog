Imports DotNetNuke.Modules.Blog.Common.Globals

Namespace Controls
 Public Class ViewSettings
  Inherits DotNetNuke.Entities.Modules.ModuleSettingsBase

#Region " Properties "
  Private _settings As Common.ModuleSettings
  Public Shadows Property Settings() As Common.ModuleSettings
   Get
    If _settings Is Nothing Then _settings = Common.ModuleSettings.GetModuleSettings(ModuleId)
    Return _settings
   End Get
   Set(ByVal value As Common.ModuleSettings)
    _settings = value
   End Set
  End Property

  Private _viewSettings As Common.ViewSettings
  Public Property ViewSettings() As Common.ViewSettings
   Get
    If _viewSettings Is Nothing Then _viewSettings = Common.ViewSettings.GetViewSettings(TabModuleId)
    Return _viewSettings
   End Get
   Set(ByVal value As Common.ViewSettings)
    _viewSettings = value
   End Set
  End Property
#End Region

#Region " Page Events "
  Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

  End Sub
#End Region

#Region " Base Method Implementations "
  Public Overrides Sub LoadSettings()

   If Not Me.IsPostBack Then

    ddTemplate.Items.Clear()
    ddTemplate.Items.Add(New ListItem("Default [System]", "[G]_default"))
    For Each d As IO.DirectoryInfo In (New IO.DirectoryInfo(HttpContext.Current.Server.MapPath(DotNetNuke.Common.ResolveUrl(glbTemplatesPath)))).GetDirectories
     If d.Name <> "_default" Then
      ddTemplate.Items.Add(New ListItem(d.Name & " [System]", "[G]" & d.Name))
     End If
    Next
    For Each d As IO.DirectoryInfo In (New IO.DirectoryInfo(Settings.PortalTemplatesMapPath)).GetDirectories
     ddTemplate.Items.Add(New ListItem(d.Name & " [Local]", "[P]" & d.Name))
    Next
    ddBlogModuleId.Items.Clear()
    ddBlogModuleId.DataSource = (New DotNetNuke.Entities.Modules.ModuleController).GetModulesByDefinition(PortalId, "Blog")
    ddBlogModuleId.DataBind()
    Try
     ddBlogModuleId.Items.Remove(ddBlogModuleId.Items.FindByValue(ModuleId.ToString))
    Catch ex As Exception
    End Try
    ddBlogModuleId.Items.Insert(0, New ListItem(LocalizeString("NoParent"), "-1"))

   End If
   Try
    ddBlogModuleId.Items.FindByValue(ViewSettings.BlogModuleId.ToString).Selected = True
   Catch ex As Exception
   End Try
   chkShowManagementPanel.Checked = ViewSettings.ShowManagementPanel
   chkShowComments.Checked = ViewSettings.ShowComments
   chkShowAllLocales.Checked = ViewSettings.ShowAllLocales
   Try
    ddTemplate.Items.FindByValue(ViewSettings.Template).Selected = True
   Catch ex As Exception
   End Try

  End Sub

  Public Overrides Sub UpdateSettings()

   ViewSettings.BlogModuleId = CInt(ddBlogModuleId.SelectedValue)
   ViewSettings.ShowManagementPanel = chkShowManagementPanel.Checked
   ViewSettings.ShowComments = chkShowComments.Checked
   ViewSettings.ShowAllLocales = chkShowAllLocales.Checked
   ViewSettings.Template = ddTemplate.SelectedValue
   ViewSettings.UpdateSettings()

  End Sub
#End Region

 End Class
End Namespace