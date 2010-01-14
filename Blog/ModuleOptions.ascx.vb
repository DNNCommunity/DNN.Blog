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
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Modules.Blog.Business
Imports DotNetNuke.Services.Exceptions.Exceptions
Imports DotNetNuke.Services.Localization.Localization

Partial Class ModuleOptions
 Inherits BlogModuleBase

#Region " Controls "
 Protected WithEvents tblSummaryOptions As System.Web.UI.HtmlControls.HtmlTable
 Protected WithEvents tblEntryOptions As System.Web.UI.HtmlControls.HtmlTable
 Protected WithEvents pnlSummaryOptions As System.Web.UI.WebControls.Panel
 Protected WithEvents DropDownList1 As System.Web.UI.WebControls.DropDownList
#End Region

#Region " Event Handlers "
 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

  Try
   If Not Page.IsPostBack Then

    ' Load settings
    chkForceDescription.Checked = BlogSettings.EntryDescriptionRequired
    txtSummaryLimit.Text = BlogSettings.SummaryMaxLength.ToString
    txtSearchLimit.Text = BlogSettings.SearchSummaryMaxLength.ToString
    txtMaxImageWidth.Text = BlogSettings.MaxImageWidth.ToString
    chkShowGravatars.Checked = BlogSettings.ShowGravatars
    chkShowWebsite.Checked = BlogSettings.ShowWebsite
    txtGravatarImageWidth.Text = BlogSettings.GravatarImageWidth.ToString

    'DW - 06/06/2008
    ' Set the visibility of the Gravatar rows based on the ShowGravatars setting
    trGravatarImageWidth.Visible = chkShowGravatars.Checked
    trGravatarRating.Visible = chkShowGravatars.Checked
    trGravatarDefaultImageUrl.Visible = chkShowGravatars.Checked
    trGravatarDefaultImageCustomURL.Visible = chkShowGravatars.Checked

    Dim ratingValue As String = BlogSettings.GravatarRating
    Dim defaultURL As String = BlogSettings.GravatarDefaultImageUrl
    rblGravatarRating.SelectedIndex = rblGravatarRating.Items.IndexOf(rblGravatarRating.Items.FindByValue(ratingValue))
    rblDefaultImage.SelectedIndex = rblDefaultImage.Items.IndexOf(rblDefaultImage.Items.FindByValue(defaultURL))
    txtGravatarDefaultImageCustomURL.Text = BlogSettings.GravatarCustomUrl

    txtRecentEntriesMax.Text = BlogSettings.RecentEntriesMax.ToString
    txtRecentRssEntriesMax.Text = BlogSettings.RecentRssEntriesMax.ToString
    trSummary.Visible = Not chkForceDescription.Checked
    trSearchSummary.Visible = Not chkForceDescription.Checked
    chkUploadOption.Checked = BlogSettings.EnableUploadOption
    chkShowSummary.Checked = BlogSettings.ShowSummary
    chkShowCommentTitle.Checked = BlogSettings.ShowCommentTitle
    chkShowUniqueTitle.Checked = BlogSettings.ShowUniqueTitle
    chkShowSeoFriendlyUrl.Checked = BlogSettings.ShowSeoFriendlyUrl
    chkEnableBookmarks.Checked = BlogSettings.ShowSocialBookmarks
    chkEnforceSummaryTruncation.Checked = BlogSettings.EnforceSummaryTruncation
    chkAllowCommentAnchors.Checked = BlogSettings.AllowCommentAnchors
    chkAllowCommentImages.Checked = BlogSettings.AllowCommentImages
    chkAllowCommentFormatting.Checked = BlogSettings.AllowCommentFormatting
    chkAllowSummaryHtml.Checked = BlogSettings.AllowSummaryHtml

    chkIncludeBody.Checked = BlogSettings.IncludeBody
    chkIncludeCategoriesInDescription.Checked = BlogSettings.IncludeCategoriesInDescription
    chkIncludeTagsInDescription.Checked = BlogSettings.IncludeTagsInDescription

    txtFeedCacheTime.Text = BlogSettings.FeedCacheTime.ToString
    chkAllowChildBlogs.Checked = BlogSettings.AllowChildBlogs

    ' 6/14/2008
    ' Add icons to radiobutton
    '
    ' have to do localization manually, otherwise, built-in localizer will overwrite the changes made here

    Dim li As ListItem
    For Each li In rblDefaultImage.Items
     If li.Value = "" Then li.Text = "<img src=""" + ModulePath + "images/grayman.png"" alt=""" & GetString("liGrayMan.Text", LocalResourceFile) & """ align=""middle""/> " & GetString("liGrayMan.Text", LocalResourceFile)
     If li.Value = "identicon" Then li.Text = "<img src=""" + ModulePath + "images/identicon.png"" alt=""" & GetString("liIdenticon.Text", LocalResourceFile) & """ align=""middle""/> " & GetString("liIdenticon.Text", LocalResourceFile)
     If li.Value = "wavatar" Then li.Text = "<img src=""" + ModulePath + "images/wavatar.png"" alt=""" & GetString("liWavatar.Text", LocalResourceFile) & """ align=""middle""/> " & GetString("liWavatar.Text", LocalResourceFile)
     If li.Value = "monsterid" Then li.Text = "<img src=""" + ModulePath + "images/monsterid.png"" alt=""" & GetString("liMonsterID.Text", LocalResourceFile) & """ align=""middle""/> " & GetString("liMonsterID.Text", LocalResourceFile)
     If li.Value = "custom" Then li.Text = "<img src=""" + ModulePath + "images/yourimagehere.png"" alt=""" & GetString("liCustom.Text", LocalResourceFile) & """ align=""middle""/> " & GetString("liCustom.Text", LocalResourceFile)
    Next

    Dim objBlog As New BlogController
    cmbPageBlogs.DataSource = objBlog.ListBlogs(PortalId, -1, False)
    cmbPageBlogs.DataBind()
    cmbPageBlogs.Items.Insert(0, New ListItem("<" & GetString("Not_Specified", SharedResourceFile) & ">", "-1"))
    Try
     cmbPageBlogs.Items.FindByValue(CStr(BlogSettings.PageBlogs)).Selected = True
    Catch
    End Try
    chkEnableDNNSearch.Checked = BlogSettings.EnableDNNSearch
    If cmbPageBlogs.Items.Count > 2 Then
     chkEnableDNNSearch.Enabled = (cmbPageBlogs.SelectedIndex = 0)
    End If

    ' calculate child blogs
    Dim totalBlogs As Integer = objBlog.ListBlogsByPortal(PortalId, True).Count
    Dim parentBlogs As Integer = objBlog.ListBlogsRootByPortal(PortalId).Count
    lblChildBlogsStatus.Text = String.Format(GetString("lblChildBlogsStatus", LocalResourceFile), CInt(totalBlogs - parentBlogs))
    DotNetNuke.UI.Utilities.ClientAPI.AddButtonConfirm(cmdMigrateChildblogs, GetString("MigrateConfirm", LocalResourceFile))
    'cmdMigrateChildblogs

    'Load Bookmarks Settings
    'Dim oBookmarks As New DataSet
    'oBookmarks.ReadXml(MapPath(ModulePath & "js/bookmarks.xml"))
    'dlBookmarks.DataSource = oBookmarks
    'dlBookmarks.DataBind()

    ForumBlog.Utils.isForumBlogInstalled(PortalId, TabId, True)
   End If

  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub



 Private Sub cmbPageBlogs_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbPageBlogs.SelectedIndexChanged
  If cmbPageBlogs.Items.Count > 2 Then
   If cmbPageBlogs.SelectedIndex = 0 Then
    chkEnableDNNSearch.Enabled = True
   Else
    chkEnableDNNSearch.Enabled = False
    chkEnableDNNSearch.Checked = False
   End If
  Else
   chkEnableDNNSearch.Enabled = True
  End If

 End Sub

 Private Sub chkShowGravatars_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkShowGravatars.CheckedChanged
  trGravatarImageWidth.Visible = chkShowGravatars.Checked
  trGravatarRating.Visible = chkShowGravatars.Checked
  trGravatarDefaultImageUrl.Visible = chkShowGravatars.Checked
  trGravatarDefaultImageCustomURL.Visible = chkShowGravatars.Checked
 End Sub

#End Region

#Region " Private Methods "
 Private Sub cmdUpdateOptions_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUpdateOptions.Click
  Try
   ' Update Settings
   With BlogSettings
    .EntryDescriptionRequired = chkForceDescription.Checked
    .SummaryMaxLength = CInt(txtSummaryLimit.Text)
    .SearchSummaryMaxLength = CInt(txtSearchLimit.Text)
    .MaxImageWidth = CInt(txtMaxImageWidth.Text)
    .RecentEntriesMax = CInt(txtRecentEntriesMax.Text)
    .RecentRssEntriesMax = CInt(txtRecentRssEntriesMax.Text)
    .EnableUploadOption = chkUploadOption.Checked
    .ShowSummary = chkShowSummary.Checked
    .ShowCommentTitle = chkShowCommentTitle.Checked
    .AllowCommentAnchors = chkAllowCommentAnchors.Checked
    .AllowCommentImages = chkAllowCommentImages.Checked
    .AllowCommentFormatting = chkAllowCommentFormatting.Checked
    .ShowUniqueTitle = chkShowUniqueTitle.Checked
    .ShowSeoFriendlyUrl = chkShowSeoFriendlyUrl.Checked
    .PageBlogs = CInt(cmbPageBlogs.SelectedItem.Value)
    .EnableDNNSearch = chkEnableDNNSearch.Checked
    .ShowGravatars = chkShowGravatars.Checked
    .GravatarImageWidth = CInt(txtGravatarImageWidth.Text)
    .GravatarRating = rblGravatarRating.SelectedValue
    .GravatarDefaultImageUrl = rblDefaultImage.SelectedValue
    .GravatarCustomUrl = txtGravatarDefaultImageCustomURL.Text
    .ShowWebsite = chkShowWebsite.Checked()
    .ShowSocialBookmarks = chkEnableBookmarks.Checked
    .EnforceSummaryTruncation = chkEnforceSummaryTruncation.Checked
    .IncludeBody = chkIncludeBody.Checked
    .IncludeCategoriesInDescription = chkIncludeCategoriesInDescription.Checked
    .IncludeTagsInDescription = chkIncludeTagsInDescription.Checked
    .AllowSummaryHtml = chkAllowSummaryHtml.Checked
    .FeedCacheTime = CInt(txtFeedCacheTime.Text.Trim)
    .AllowChildBlogs = chkAllowChildBlogs.Checked

    .UpdateSettings()
   End With

   'Update Bookmark Settings
   'Dim chkActive As System.Web.UI.WebControls.CheckBox
   'Dim litBookmarkId As System.Web.UI.WebControls.Literal
   'For i As Integer = 0 To dlBookmarks.Items.Count - 1
   '    chkActive = CType(dlBookmarks.Items(i).Controls(1), System.Web.UI.WebControls.CheckBox)
   '    litBookmarkId = CType(dlBookmarks.Items(i).Controls(2), System.Web.UI.WebControls.Literal)
   '    If Not chkActive Is Nothing AndAlso Not litBookmarkId Is Nothing Then
   '        UpdateBookmarksXML(CInt(litBookmarkId.Text), chkActive.Checked)
   '    End If
   'Next

   Response.Redirect(NavigateURL(), True)
  Catch exc As Exception 'Module failed to load
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 'Private Sub UpdateBookmarksXML(ByVal BookmarkId As Integer, ByVal Active As Boolean)
 '    Try
 '        Dim oBookmarks As New DataSet
 '        Dim oDataRow() As DataRow
 '        oBookmarks.ReadXml(MapPath(ModulePath & "js/bookmarks.xml"))

 '        For x As Integer = 0 To oBookmarks.Tables(0).Rows.Count - 1
 '            If CInt(oBookmarks.Tables(0).Rows(x).Item("Id").ToString) = BookmarkId Then
 '                oBookmarks.Tables(0).Rows(x).Item("Active") = Active
 '                Exit For
 '            End If
 '        Next
 '        oBookmarks.AcceptChanges()
 '        oBookmarks.WriteXml(MapPath(ModulePath & "js/bookmarks.xml"))
 '        oBookmarks = Nothing

 '    Catch exc As Exception
 '        ProcessModuleLoadException(Me, exc)
 '    End Try
 'End Sub

 Private Sub cmdCancelOptions_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCancelOptions.Click
  Try
   Response.Redirect(NavigateURL(), True)
  Catch exc As Exception 'Module failed to load
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 Private Sub chkForceDescription_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkForceDescription.CheckedChanged
  Me.trSummary.Visible = Not chkForceDescription.Checked
  Me.trSearchSummary.Visible = Not chkForceDescription.Checked
 End Sub

 Private Sub cmdGenerateLinks_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdGenerateLinks.Click
  Utility.CreateAllEntryLinks(PortalId, , TabId)
 End Sub

 'Private Sub dlBookmarks_ItemDataBound(ByVal sender As System.Object, ByVal e As System.Web.UI.WebControls.DataListItemEventArgs)
 '    Dim imgBookmark As System.Web.UI.WebControls.Image
 '    If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
 '        imgBookmark = CType(e.Item.FindControl("imgBookmark"), System.Web.UI.WebControls.Image)
 '        If Not imgBookmark Is Nothing Then
 '            imgBookmark.ImageUrl = ModulePath & "images/bookmarks/" & CType(e.Item.DataItem, System.Data.DataRowView).Row("Icon").ToString
 '            imgBookmark.AlternateText = CType(e.Item.DataItem, System.Data.DataRowView).Row("Name").ToString
 '        End If
 '    End If
 'End Sub

 Private Sub cmdMigrateChildblogs_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdMigrateChildblogs.Click

  ' get sql
  Dim assembly As Reflection.Assembly = Reflection.Assembly.GetExecutingAssembly()
  Dim sql As String = ""
  Using stream As IO.Stream = assembly.GetManifestResourceStream("DotNetNuke.Modules.Blog.ChildblogsToCategories.sql")
   Using rdr As New IO.StreamReader(stream)
    sql = rdr.ReadToEnd
   End Using
  End Using
  sql = sql.Replace("@portalid", PortalId.ToString)

  ' run the script
  Dim res As String = DotNetNuke.Data.DataProvider.Instance().ExecuteScript(sql, False)

  ' run through all categories to make sure the slug is correctly set
  For Each c As CategoryInfo In CategoryController.ListCategories(PortalId).Values
   CategoryController.UpdateCategory(c.CatId, c.Category, c.ParentId)
  Next

  ' recalculate child blogs
  Dim totalBlogs As Integer = (New BlogController).ListBlogsByPortal(PortalId, True).Count
  Dim parentBlogs As Integer = (New BlogController).ListBlogsRootByPortal(PortalId).Count
  lblChildBlogsStatus.Text = String.Format(GetString("lblChildBlogsStatus", LocalResourceFile), CInt(totalBlogs - parentBlogs))

 End Sub

#End Region

End Class


