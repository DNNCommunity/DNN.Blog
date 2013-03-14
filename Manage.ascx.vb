Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports System.Linq
Imports DotNetNuke.Modules.Blog.Entities.Entries
Imports DotNetNuke.Web.UI.WebControls
Imports DotNetNuke.Modules.Blog.Common.Globals

Public Class Manage
 Inherits BlogModuleBase

 Private _totalEntries As Integer = -1

 Private Sub Page_Init1(sender As Object, e As System.EventArgs) Handles Me.Init
  AddBlogService()
 End Sub

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

  If Not Me.IsPostBack Then
   Me.DataBind()
  End If

 End Sub

 Private Sub cmdAdd_Click(sender As Object, e As System.EventArgs) Handles cmdAdd.Click
  Response.Redirect(EditUrl("BlogEdit"), False)
 End Sub

 Private Sub cmdCancel_Click(sender As Object, e As System.EventArgs) Handles cmdCancel.Click
  Response.Redirect(DotNetNuke.Common.NavigateURL(TabId), False)
 End Sub
 Private Sub cmdCancelSettings_Click(sender As Object, e As System.EventArgs) Handles cmdCancelSettings.Click
  Response.Redirect(DotNetNuke.Common.NavigateURL(TabId), False)
 End Sub

 Private Sub cmdUpdateSettings_Click(sender As Object, e As System.EventArgs) Handles cmdUpdateSettings.Click
  Settings.AllowAttachments = chkAllowAttachments.Checked
  Settings.SummaryModel = CType(ddSummaryModel.SelectedValue.ToInt, SummaryType)
  Settings.AllowMultipleCategories = chkAllowMultipleCategories.Checked
  Settings.AllowWLW = chkAllowWLW.Checked
  Settings.Email = txtEmail.Text
  Settings.WLWRecentEntriesMax = txtWLWRecentEntriesMax.Text.ToInt
  Settings.AllowAllLocales = chkAllowAllLocales.Checked
  Settings.VocabularyId = ddVocabularyId.SelectedValue.ToInt
  Settings.UpdateSettings()
  Response.Redirect(DotNetNuke.Common.NavigateURL(TabId), False)
 End Sub

 Public Overrides Sub DataBind()
  MyBase.DataBind()

  SettingsHeader.Visible = Security.IsEditor
  SettingsTab.Visible = Security.IsEditor
  If Security.IsEditor Then
   dlBlogs.DataSource = BlogsController.GetBlogsByModule(Settings.ModuleId, UserId).Values
   chkAllowAttachments.Checked = Settings.AllowAttachments
   Try
    ddSummaryModel.Items.FindByValue(CInt(Settings.SummaryModel).ToString).Selected = True
   Catch ex As Exception
   End Try
   chkAllowMultipleCategories.Checked = Settings.AllowMultipleCategories
   chkAllowWLW.Checked = Settings.AllowWLW
   chkAllowAllLocales.Checked = Settings.AllowAllLocales
   txtEmail.Text = Settings.Email
   txtWLWRecentEntriesMax.Text = Settings.WLWRecentEntriesMax.ToString
   ddVocabularyId.DataSource = Common.Globals.GetPortalVocabularies(PortalId)
   ddVocabularyId.DataBind()
   ddVocabularyId.Items.Insert(0, New ListItem(LocalizeString("NoneSpecified"), "-1"))
   Try
    ddVocabularyId.Items.FindByValue(Settings.VocabularyId.ToString).Selected = True
   Catch ex As Exception
   End Try
  Else
   dlBlogs.DataSource = BlogsController.GetBlogsByModule(Settings.ModuleId, UserId).Values.Where(Function(b)
                                                                                                  Return b.OwnerUserId = UserId
                                                                                                 End Function)
  End If
  dlBlogs.DataBind()
  If dlBlogs.Items.Count = 0 Then dlBlogs.Visible = False

 End Sub

 Public Sub GetEntries()

  grdEntries.DataSource = EntriesController.GetEntries(Settings.ModuleId, -1, -1, Now, -1, grdEntries.CurrentPageIndex, grdEntries.PageSize, "PUBLISHEDONDATE DESC", _totalEntries, UserId, Security.UserIsAdmin).Values
  grdEntries.VirtualItemCount = _totalEntries

 End Sub

 Public Sub RebindEntries()
  GetEntries()
  grdEntries.Rebind()
 End Sub

End Class