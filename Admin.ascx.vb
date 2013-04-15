Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports System.Linq
Imports DotNetNuke.Modules.Blog.Entities.Posts
Imports DotNetNuke.Web.UI.WebControls
Imports DotNetNuke.Web.Client.ClientResourceManagement
Imports DotNetNuke.Modules.Blog.Common.Globals

Public Class Admin
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
   Dim ReturnedIds As New List(Of Integer)
   Dim i As Integer = 1
   For Each rootNode As Common.DynatreeItem In categoryTree
    AddOrUpdateCategory(-1, i, rootNode, ReturnedIds)
    i += 1
   Next
   Dim deleteCategories As New List(Of Entities.Terms.TermInfo)
   For Each t As Entities.Terms.TermInfo In Vocabulary.Values
    If Not ReturnedIds.Contains(t.TermId) Then deleteCategories.Add(t)
   Next
   For Each categoryToDelete As Entities.Terms.TermInfo In deleteCategories
    DotNetNuke.Entities.Content.Common.Util.GetTermController().DeleteTerm(categoryToDelete)
   Next
  End If
  Response.Redirect(DotNetNuke.Common.NavigateURL(TabId), False)
 End Sub

 Private Sub AddOrUpdateCategory(parentId As Integer, viewOrder As Integer, category As Common.DynatreeItem, ByRef returnedIds As List(Of Integer))
  If String.IsNullOrEmpty(category.title) Then Exit Sub
  Dim termId As Integer = -1
  If IsNumeric(category.key) Then termId = Integer.Parse(category.key)
  termId = Data.DataProvider.Instance.SetTerm(termId, Settings.VocabularyId, parentId, viewOrder, category.title, "", UserId)
  returnedIds.Add(termId)
  Dim i As Integer = 1
  For Each subCategory As Common.DynatreeItem In category.children
   AddOrUpdateCategory(termId, i, subCategory, returnedIds)
   i += 1
  Next
 End Sub

 Public Overrides Sub DataBind()
  MyBase.DataBind()

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

  treeState.Value = DotNetNuke.Modules.Blog.Entities.Terms.TermsController.GetCategoryTreeAsJson(Vocabulary)

 End Sub

End Class