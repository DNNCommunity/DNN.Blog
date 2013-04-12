Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports System.Linq
Imports DotNetNuke.Modules.Blog.Entities.Posts
Imports DotNetNuke.Web.UI.WebControls
Imports DotNetNuke.Web.Client.ClientResourceManagement
Imports DotNetNuke.Modules.Blog.Common.Globals

Public Class Manage
 Inherits BlogModuleBase

 Private _totalPosts As Integer = -1

 Private Sub Page_Init1(sender As Object, e As System.EventArgs) Handles Me.Init
  AddBlogService()
  ClientResourceManager.RegisterScript(Page, ResolveUrl("~/DesktopModules/Blog/js/jquery.dynatree.min.js"))
  ClientResourceManager.RegisterStyleSheet(Page, ResolveUrl("~/DesktopModules/Blog/css/dynatree.css"), Web.Client.FileOrder.Css.ModuleCss)
 End Sub

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

  If Not Me.IsPostBack Then
   Me.DataBind()
  End If

 End Sub

 Private Sub cmdAdd_Click(sender As Object, e As System.EventArgs) Handles cmdAdd.Click
  Response.Redirect(EditUrl("BlogEdit"), False)
 End Sub

 Private Sub cmdEditTagsML_Click(sender As Object, e As System.EventArgs) Handles cmdEditTagsML.Click
  Response.Redirect(EditUrl("TermsEditML"), False)
 End Sub

 Private Sub cmdCancel_Click(sender As Object, e As System.EventArgs) Handles cmdCancel.Click
  Response.Redirect(DotNetNuke.Common.NavigateURL(TabId), False)
 End Sub

 Private Sub cmdUpdateSettings_Click(sender As Object, e As System.EventArgs) Handles cmdUpdate.Click
  Settings.AllowAttachments = chkAllowAttachments.Checked
  Settings.SummaryModel = CType(ddSummaryModel.SelectedValue.ToInt, SummaryType)
  Settings.LocalizationModel = CType(ddSummaryModel.SelectedValue.ToInt, LocalizationType)
  Settings.AllowMultipleCategories = chkAllowMultipleCategories.Checked
  Settings.AllowWLW = chkAllowWLW.Checked
  Settings.WLWRecentPostsMax = txtWLWRecentPostsMax.Text.ToInt
  Settings.AllowAllLocales = chkAllowAllLocales.Checked
  Settings.ShowAllLocales = chkShowAllLocales.Checked
  Settings.VocabularyId = ddVocabularyId.SelectedValue.ToInt
  Settings.BloggersCanEditCategories = chkBloggersCanEditCategories.Checked
  Settings.RssAllowContentInFeed = chkRssAllowContentInFeed.Checked
  Settings.RssDefaultCopyright = txtRssDefaultCopyright.Text
  Settings.RssDefaultNrItems = Integer.Parse(txtRssDefaultNrItems.Text)
  Settings.RssEmail = txtEmail.Text
  Settings.RssImageHeight = Integer.Parse(txtRssImageHeight.Text)
  Settings.RssImageWidth = Integer.Parse(txtRssImageWidth.Text)
  Settings.RssImageSizeAllowOverride = chkRssImageSizeAllowOverride.Checked
  Settings.RssMaxNrItems = Integer.Parse(txtRssMaxNrItems.Text)
  Settings.RssTtl = Integer.Parse(txtRssTtl.Text)
  Settings.UpdateSettings()
  If treeState.Value <> DotNetNuke.Modules.Blog.Entities.Terms.TermsController.GetCategoryTreeAsJson(Vocabulary) Then
   Dim categoryTree As List(Of Common.DynatreeItem) = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of Common.DynatreeItem))(treeState.Value)
   For Each rootNode As Common.DynatreeItem In categoryTree

   Next
  End If
  Response.Redirect(DotNetNuke.Common.NavigateURL(TabId), False)
 End Sub

 Public Overrides Sub DataBind()
  MyBase.DataBind()

  SettingsHeader.Visible = Security.IsEditor
  SettingsTab.Visible = Security.IsEditor
  If Security.IsEditor Then
   dlBlogs.DataSource = BlogsController.GetBlogsByModule(Settings.ModuleId, UserId, Locale).Values
   chkAllowAttachments.Checked = Settings.AllowAttachments
   Try
    ddSummaryModel.Items.FindByValue(CInt(Settings.SummaryModel).ToString).Selected = True
   Catch ex As Exception
   End Try
   Try
    ddLocalizationModel.Items.FindByValue(CInt(Settings.LocalizationModel).ToString).Selected = True
   Catch ex As Exception
   End Try
   ddLocalizationModel.Enabled = CBool((New DotNetNuke.Services.Localization.LocaleController).GetLocales(PortalId).Count > 1)
   chkAllowMultipleCategories.Checked = Settings.AllowMultipleCategories
   chkAllowWLW.Checked = Settings.AllowWLW
   chkAllowAllLocales.Checked = Settings.AllowAllLocales
   chkShowAllLocales.Checked = Settings.ShowAllLocales

   chkRssAllowContentInFeed.Checked = Settings.RssAllowContentInFeed
   txtRssDefaultCopyright.Text = Settings.RssDefaultCopyright
   txtRssDefaultNrItems.Text = Settings.RssDefaultNrItems.ToString
   txtEmail.Text = Settings.RssEmail
   txtRssImageHeight.Text = Settings.RssImageHeight.ToString
   txtRssImageWidth.Text = Settings.RssImageWidth.ToString
   chkRssImageSizeAllowOverride.Checked = Settings.RssImageSizeAllowOverride
   txtRssMaxNrItems.Text = Settings.RssMaxNrItems.ToString
   txtRssTtl.Text = Settings.RssTtl.ToString

   txtWLWRecentPostsMax.Text = Settings.WLWRecentPostsMax.ToString
   ddVocabularyId.DataSource = Common.Globals.GetPortalVocabularies(PortalId)
   ddVocabularyId.DataBind()
   ddVocabularyId.Items.Insert(0, New ListItem(LocalizeString("NoneSpecified"), "-1"))
   Try
    ddVocabularyId.Items.FindByValue(Settings.VocabularyId.ToString).Selected = True
   Catch ex As Exception
   End Try
   chkBloggersCanEditCategories.Checked = Settings.BloggersCanEditCategories
  Else
   dlBlogs.DataSource = BlogsController.GetBlogsByModule(Settings.ModuleId, UserId, Locale).Values.Where(Function(b)
                                                                                                          Return b.OwnerUserId = UserId
                                                                                                         End Function)
  End If
  dlBlogs.DataBind()
  If dlBlogs.Items.Count = 0 Then dlBlogs.Visible = False

  treeState.Value = DotNetNuke.Modules.Blog.Entities.Terms.TermsController.GetCategoryTreeAsJson(Vocabulary)

 End Sub

 Public Sub GetPosts()

  grdPosts.DataSource = PostsController.GetPosts(Settings.ModuleId, -1, Locale, -1, "", Now, -1, grdPosts.CurrentPageIndex, grdPosts.PageSize, "PUBLISHEDONDATE DESC", _totalPosts, UserId, Security.UserIsAdmin).Values
  grdPosts.VirtualItemCount = _totalPosts

 End Sub

 Public Sub RebindPosts()
  GetPosts()
  grdPosts.Rebind()
 End Sub

End Class