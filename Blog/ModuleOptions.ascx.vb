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
Imports DotNetNuke.Modules.Blog.Components.Business
Imports DotNetNuke.Modules.Blog.Components.Controllers
Imports DotNetNuke.Modules.Blog.Components.Common
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Modules.Blog.Business
Imports DotNetNuke.Services.Exceptions.Exceptions
Imports DotNetNuke.Services.Localization.Localization
Imports DotNetNuke.Framework
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Modules.Blog.Components.Integration
Imports DotNetNuke.Modules.Blog.Components.Entities

Partial Public Class ModuleOptions
    Inherits BlogModuleBase

#Region "Event Handlers"

    Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            jQuery.RequestDnnPluginsRegistration()

            If Not Page.IsPostBack Then
                PopulateDropDowns()

                chkForceDescription.Checked = BlogSettings.EntryDescriptionRequired
                txtSummaryLimit.Text = BlogSettings.SummaryMaxLength.ToString
                txtSearchLimit.Text = BlogSettings.SearchSummaryMaxLength.ToString
                txtMaxImageWidth.Text = BlogSettings.MaxImageWidth.ToString
                chkShowGravatars.Checked = BlogSettings.ShowGravatars
                chkShowWebsite.Checked = BlogSettings.ShowWebsite
                txtGravatarImageWidth.Text = BlogSettings.GravatarImageWidth.ToString

                Dim ratingValue As String = BlogSettings.GravatarRating
                Dim defaultURL As String = BlogSettings.GravatarDefaultImageUrl
                rblGravatarRating.SelectedIndex = rblGravatarRating.Items.IndexOf(rblGravatarRating.Items.FindByValue(ratingValue))
                rblDefaultImage.SelectedIndex = rblDefaultImage.Items.IndexOf(rblDefaultImage.Items.FindByValue(defaultURL))
                txtGravatarDefaultImageCustomURL.Text = BlogSettings.GravatarCustomUrl
                txtRecentEntriesMax.Text = BlogSettings.RecentEntriesMax.ToString
                txtRecentRssEntriesMax.Text = BlogSettings.RecentRssEntriesMax.ToString
                chkUploadOption.Checked = BlogSettings.EnableUploadOption
                chkShowSummary.Checked = BlogSettings.ShowSummary
                chkShowCommentTitle.Checked = BlogSettings.ShowCommentTitle
                chkShowSeoFriendlyUrl.Checked = BlogSettings.ShowSeoFriendlyUrl
                'chkEnableBookmarks.Checked = BlogSettings.ShowSocialBookmarks
                chkEnforceSummaryTruncation.Checked = BlogSettings.EnforceSummaryTruncation
                chkAllowCommentAnchors.Checked = BlogSettings.AllowCommentAnchors
                chkAllowCommentImages.Checked = BlogSettings.AllowCommentImages
                chkAllowCommentFormatting.Checked = BlogSettings.AllowCommentFormatting
                chkAllowSummaryHtml.Checked = BlogSettings.AllowSummaryHtml
                chkAllowWLW.Checked = BlogSettings.AllowWLW

                chkIncludeBody.Checked = BlogSettings.IncludeBody
                chkIncludeCategoriesInDescription.Checked = BlogSettings.IncludeCategoriesInDescription
                chkIncludeTagsInDescription.Checked = BlogSettings.IncludeTagsInDescription

                txtFeedCacheTime.Text = BlogSettings.FeedCacheTime.ToString
                chkAllowChildBlogs.Checked = BlogSettings.AllowChildBlogs
                chkEnableArchiveDropDown.Checked = BlogSettings.EnableArchiveDropDown
                chkAllowMultipleCategories.Checked = BlogSettings.AllowMultipleCategories
                chkUseWLWExcerpt.Checked = BlogSettings.UseWLWExcerpt
                ddlCatVocabRoot.SelectedValue = BlogSettings.VocabularyId.ToString()

                ' Additional files to load
                Dim fileList As String = ";" & BlogSettings.IncludeFiles
                AddFolderToList(cblHostFiles, Server.MapPath("~/DesktopModules/Blog/include"), "")
                AddFolderToList(cblPortalFiles, PortalSettings.HomeDirectoryMapPath & "Blog\include\", "")
                For Each itm As ListItem In cblHostFiles.Items
                    If fileList.IndexOf(";[H]" & itm.Value & ";") > -1 Then
                        itm.Selected = True
                    End If
                Next
                For Each itm As ListItem In cblPortalFiles.Items
                    If fileList.IndexOf(";[P]" & itm.Value & ";") > -1 Then
                        itm.Selected = True
                    End If
                Next

                'cmdMigrateChildblogs

                'Load Bookmarks Settings
                'Dim oBookmarks As New DataSet
                'oBookmarks.ReadXml(MapPath(ModulePath & "js/bookmarks.xml"))
                'dlBookmarks.DataSource = oBookmarks
                'dlBookmarks.DataBind()

                ForumBlog.Utils.isForumBlogInstalled(PortalId, TabId, True)

                hlCancelOptions.NavigateUrl = ModuleContext.NavigateUrl(ModuleContext.TabId, "", False, "")
            End If
        Catch exc As Exception
            ProcessModuleLoadException(Me, exc)
        End Try
    End Sub

    Protected Sub cmdUpdateOptions_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUpdateOptions.Click
        Try
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
                '.ShowUniqueTitle = chkShowUniqueTitle.Checked
                .ShowSeoFriendlyUrl = chkShowSeoFriendlyUrl.Checked
                .PageBlogs = CInt(cmbPageBlogs.SelectedItem.Value)
                '.EnableDNNSearch = chkEnableDNNSearch.Checked
                .ShowGravatars = chkShowGravatars.Checked
                .GravatarImageWidth = CInt(txtGravatarImageWidth.Text)
                .GravatarRating = rblGravatarRating.SelectedValue
                .GravatarDefaultImageUrl = rblDefaultImage.SelectedValue
                .GravatarCustomUrl = txtGravatarDefaultImageCustomURL.Text
                .ShowWebsite = chkShowWebsite.Checked()
                '.ShowSocialBookmarks = chkEnableBookmarks.Checked
                .EnforceSummaryTruncation = chkEnforceSummaryTruncation.Checked
                .IncludeBody = chkIncludeBody.Checked
                .IncludeCategoriesInDescription = chkIncludeCategoriesInDescription.Checked
                .IncludeTagsInDescription = chkIncludeTagsInDescription.Checked
                .AllowSummaryHtml = chkAllowSummaryHtml.Checked
                .FeedCacheTime = CInt(txtFeedCacheTime.Text.Trim)
                .AllowChildBlogs = chkAllowChildBlogs.Checked
                .EnableArchiveDropDown = chkEnableArchiveDropDown.Checked
                .AllowWLW = chkAllowWLW.Checked
                .AllowMultipleCategories = chkAllowMultipleCategories.Checked
                .UseWLWExcerpt = chkUseWLWExcerpt.Checked
                .VocabularyId = Convert.ToInt32(ddlCatVocabRoot.SelectedValue)

                ' additional files
                Dim fileList As String = ""
                For Each itm As ListItem In cblHostFiles.Items
                    If itm.Selected Then
                        fileList &= "[H]" & itm.Value & ";"
                    End If
                Next
                For Each itm As ListItem In cblPortalFiles.Items
                    If itm.Selected Then
                        fileList &= "[P]" & itm.Value & ";"
                    End If
                Next
                .IncludeFiles = fileList

                .UpdateSettings()
            End With

            Response.Redirect(NavigateURL(), True)
        Catch exc As Exception 'Module failed to load
            ProcessModuleLoadException(Me, exc)
        End Try
    End Sub

    Protected Sub cmdGenerateLinks_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdGenerateLinks.Click
        Utility.CreateAllEntryLinks(PortalId, , TabId)
    End Sub

    Protected Sub cmdMigrateChildblogs_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdMigrateChildblogs.Click
        ' get sql
        Dim assembly As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly()
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

#Region "Private Methods"

    Private Sub PopulateDropDowns()
        ' 6/14/2008
        ' Add icons to radiobutton
        ' have to do localization manually, otherwise, built-in localizer will overwrite the changes made here
        Dim li As ListItem
        For Each li In rblDefaultImage.Items
            If li.Value = "" Then li.Text = "<img src=""" + ControlPath + "images/grayman.png"" alt=""" & GetString("liGrayMan.Text", LocalResourceFile) & """ align=""middle""/> " & GetString("liGrayMan.Text", LocalResourceFile)
            If li.Value = "identicon" Then li.Text = "<img src=""" + ControlPath + "images/identicon.png"" alt=""" & GetString("liIdenticon.Text", LocalResourceFile) & """ align=""middle""/> " & GetString("liIdenticon.Text", LocalResourceFile)
            If li.Value = "wavatar" Then li.Text = "<img src=""" + ControlPath + "images/wavatar.png"" alt=""" & GetString("liWavatar.Text", LocalResourceFile) & """ align=""middle""/> " & GetString("liWavatar.Text", LocalResourceFile)
            If li.Value = "monsterid" Then li.Text = "<img src=""" + ControlPath + "images/monsterid.png"" alt=""" & GetString("liMonsterID.Text", LocalResourceFile) & """ align=""middle""/> " & GetString("liMonsterID.Text", LocalResourceFile)
            If li.Value = "custom" Then li.Text = "<img src=""" + ControlPath + "images/yourimagehere.png"" alt=""" & GetString("liCustom.Text", LocalResourceFile) & """ align=""middle""/> " & GetString("liCustom.Text", LocalResourceFile)
        Next

        Dim objBlog As New BlogController
        cmbPageBlogs.DataSource = objBlog.ListBlogs(PortalId, -1, False)
        cmbPageBlogs.DataBind()
        cmbPageBlogs.Items.Insert(0, New ListItem("<" & GetString("Not_Specified", SharedResourceFile) & ">", "-1"))
        Try
            cmbPageBlogs.Items.FindByValue(CStr(BlogSettings.PageBlogs)).Selected = True
        Catch
        End Try

        ' calculate child blogs
        Dim totalBlogs As Integer = objBlog.ListBlogsByPortal(PortalId, True).Count
        Dim parentBlogs As Integer = objBlog.ListBlogsRootByPortal(PortalId).Count
        lblChildBlogsStatus.Text = String.Format(GetString("lblChildBlogsStatus", LocalResourceFile), CInt(totalBlogs - parentBlogs))
        DotNetNuke.UI.Utilities.ClientAPI.AddButtonConfirm(cmdMigrateChildblogs, GetString("MigrateConfirm", LocalResourceFile))

        ddlCatVocabRoot.DataSource = Terms.GetPortalVocabularies(ModuleContext.PortalId)
        ddlCatVocabRoot.DataBind()

        Dim catli As New ListItem
        catli.Text = Localization.GetString("NoneSpecified", LocalResourceFile)
        catli.Value = "0"
        ddlCatVocabRoot.Items.Insert(0, catli)
    End Sub

    Private Sub AddFolderToList(ByRef cbList As CheckBoxList, ByVal fullPath As String, ByVal relativePath As String)
        If Not IO.Directory.Exists(fullPath) Then Exit Sub
        Dim baseDir As New IO.DirectoryInfo(fullPath)
        For Each d As IO.DirectoryInfo In baseDir.GetDirectories()
            AddFolderToList(cbList, d.FullName, relativePath & d.Name & "/")
        Next
        For Each f As IO.FileInfo In baseDir.GetFiles()
            cbList.Items.Add(New ListItem(relativePath & f.Name, relativePath & f.Name))
        Next
    End Sub

#End Region

End Class