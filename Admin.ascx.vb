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
  AddJavascriptFile("jquery.dynatree.min.js", 60)
  AddCssFile("dynatree.css")
 End Sub

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

  If Not BlogContext.Security.IsEditor Then
   Throw New Exception("You do not have access to this resource. Please check your login status.")
  End If

  If Not Me.IsPostBack Then
   Me.DataBind()
  End If

  DotNetNuke.UI.Utilities.ClientAPI.AddButtonConfirm(cmdEditTagsML, LocalizeString("LeavePage.Confirm"))
  DotNetNuke.UI.Utilities.ClientAPI.AddButtonConfirm(cmdEditCategoriesML, LocalizeString("LeavePage.Confirm"))

 End Sub

 Private Sub cmdEditTagsML_Click(sender As Object, e As System.EventArgs) Handles cmdEditTagsML.Click
  Response.Redirect(EditUrl("TermsEditML"), False)
 End Sub

 Private Sub cmdEditCategoriesML_Click(sender As Object, e As System.EventArgs) Handles cmdEditCategoriesML.Click
  If BlogContext.Settings.VocabularyId > -1 Then
   Response.Redirect(EditUrl("VocabularyId", BlogContext.Settings.VocabularyId.ToString, "TermsEditML"), False)
  End If
 End Sub

 Private Sub cmdCancel_Click(sender As Object, e As System.EventArgs) Handles cmdCancel.Click
  Response.Redirect(DotNetNuke.Common.NavigateURL(TabId), False)
 End Sub

 Private Sub cmdUpdateSettings_Click(sender As Object, e As System.EventArgs) Handles cmdUpdate.Click
  BlogContext.Settings.AllowAttachments = chkAllowAttachments.Checked
  BlogContext.Settings.SummaryModel = CType(ddSummaryModel.SelectedValue.ToInt, SummaryType)
  BlogContext.settings.AllowMultipleCategories = chkAllowMultipleCategories.Checked
  BlogContext.settings.AllowWLW = chkAllowWLW.Checked
  BlogContext.settings.WLWRecentPostsMax = txtWLWRecentPostsMax.Text.ToInt
  BlogContext.settings.AllowAllLocales = chkAllowAllLocales.Checked
  BlogContext.settings.VocabularyId = ddVocabularyId.SelectedValue.ToInt
  BlogContext.settings.RssAllowContentInFeed = chkRssAllowContentInFeed.Checked
  BlogContext.settings.RssDefaultCopyright = txtRssDefaultCopyright.Text
  BlogContext.settings.RssDefaultNrItems = Integer.Parse(txtRssDefaultNrItems.Text)
  BlogContext.settings.RssEmail = txtEmail.Text
  BlogContext.settings.RssImageHeight = Integer.Parse(txtRssImageHeight.Text)
  BlogContext.settings.RssImageWidth = Integer.Parse(txtRssImageWidth.Text)
  BlogContext.settings.RssImageSizeAllowOverride = chkRssImageSizeAllowOverride.Checked
  BlogContext.settings.RssMaxNrItems = Integer.Parse(txtRssMaxNrItems.Text)
  BlogContext.settings.RssTtl = Integer.Parse(txtRssTtl.Text)
  BlogContext.settings.UpdateSettings()
  If treeState.Value <> DotNetNuke.Modules.Blog.Entities.Terms.TermsController.GetCategoryTreeAsJson(BlogContext.Vocabulary) Then
   Dim categoryTree As List(Of Common.DynatreeItem) = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of Common.DynatreeItem))(treeState.Value)
   Dim ReturnedIds As New List(Of Integer)
   Dim i As Integer = 1
   For Each rootNode As Common.DynatreeItem In categoryTree
    AddOrUpdateCategory(-1, i, rootNode, ReturnedIds)
    i += 1
   Next
   Dim deleteCategories As New List(Of Entities.Terms.TermInfo)
   For Each t As Entities.Terms.TermInfo In BlogContext.Vocabulary.Values
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
  termId = Data.DataProvider.Instance.SetTerm(termId, BlogContext.settings.VocabularyId, parentId, viewOrder, category.title, "", UserId)
  returnedIds.Add(termId)
  Dim i As Integer = 1
  For Each subCategory As Common.DynatreeItem In category.children
   AddOrUpdateCategory(termId, i, subCategory, returnedIds)
   i += 1
  Next
 End Sub

 Public Overrides Sub DataBind()
  MyBase.DataBind()

  chkAllowAttachments.Checked = BlogContext.settings.AllowAttachments
  Try
   ddSummaryModel.Items.FindByValue(CInt(BlogContext.Settings.SummaryModel).ToString).Selected = True
  Catch ex As Exception
  End Try
  cmdEditTagsML.Enabled = BlogContext.IsMultiLingualSite
  cmdEditCategoriesML.Enabled = BlogContext.IsMultiLingualSite And BlogContext.Settings.VocabularyId > -1
  chkAllowMultipleCategories.Checked = BlogContext.settings.AllowMultipleCategories
  chkAllowWLW.Checked = BlogContext.settings.AllowWLW
  chkAllowAllLocales.Checked = BlogContext.settings.AllowAllLocales

  chkRssAllowContentInFeed.Checked = BlogContext.settings.RssAllowContentInFeed
  txtRssDefaultCopyright.Text = BlogContext.settings.RssDefaultCopyright
  txtRssDefaultNrItems.Text = BlogContext.settings.RssDefaultNrItems.ToString
  txtEmail.Text = BlogContext.settings.RssEmail
  txtRssImageHeight.Text = BlogContext.settings.RssImageHeight.ToString
  txtRssImageWidth.Text = BlogContext.settings.RssImageWidth.ToString
  chkRssImageSizeAllowOverride.Checked = BlogContext.settings.RssImageSizeAllowOverride
  txtRssMaxNrItems.Text = BlogContext.settings.RssMaxNrItems.ToString
  txtRssTtl.Text = BlogContext.settings.RssTtl.ToString

  txtWLWRecentPostsMax.Text = BlogContext.settings.WLWRecentPostsMax.ToString
  ddVocabularyId.DataSource = Common.Globals.GetPortalVocabularies(PortalId)
  ddVocabularyId.DataBind()
  ddVocabularyId.Items.Insert(0, New ListItem(LocalizeString("NoneSpecified"), "-1"))
  Try
   ddVocabularyId.Items.FindByValue(BlogContext.Settings.VocabularyId.ToString).Selected = True
  Catch ex As Exception
  End Try

  treeState.Value = DotNetNuke.Modules.Blog.Entities.Terms.TermsController.GetCategoryTreeAsJson(BlogContext.Vocabulary)

 End Sub

End Class