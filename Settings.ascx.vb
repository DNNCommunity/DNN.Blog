Public Class Settings
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

  If Not Me.IsPostBack Then

   ddVocabularyId.DataSource = Common.Globals.GetPortalVocabularies(PortalId)
   ddVocabularyId.DataBind()
   ddVocabularyId.Items.Insert(0, New ListItem(LocalizeString("NoneSpecified"), "-1"))

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

  End If

 End Sub
#End Region

#Region " Base Method Implementations "
 Public Overrides Sub LoadSettings()

  chkAllowAttachments.Checked = Settings.AllowAttachments
  chkAllowHtmlSummary.Checked = Settings.AllowHtmlSummary
  chkAllowMultipleCategories.Checked = Settings.AllowMultipleCategories
  chkAllowWLW.Checked = Settings.AllowWLW
  txtEmail.Text = Settings.Email
  txtWLWRecentEntriesMax.Text = Settings.WLWRecentEntriesMax.ToString
  Try
   ddVocabularyId.Items.FindByValue(Settings.VocabularyId.ToString).Selected = True
  Catch ex As Exception
  End Try
  Try
   ddTemplate.Items.FindByValue(ViewSettings.Template).Selected = True
  Catch ex As Exception
  End Try

 End Sub

 Public Overrides Sub UpdateSettings()

  Settings.AllowAttachments = chkAllowAttachments.Checked
  Settings.AllowHtmlSummary = chkAllowHtmlSummary.Checked
  Settings.AllowMultipleCategories = chkAllowMultipleCategories.Checked
  Settings.AllowWLW = chkAllowWLW.Checked
  Settings.Email = txtEmail.Text
  Settings.WLWRecentEntriesMax = txtWLWRecentEntriesMax.Text.ToInt
  Settings.VocabularyId = ddVocabularyId.SelectedValue.ToInt
  Settings.UpdateSettings()

  ViewSettings.Template = ddTemplate.SelectedValue
  ViewSettings.UpdateSettings()

 End Sub
#End Region

End Class