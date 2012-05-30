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
Imports DotNetNuke.Services.Exceptions.Exceptions
Imports DotNetNuke.Services.Localization.Localization
Imports DotNetNuke.Framework
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Modules.Blog.Components.Integration
Imports DotNetNuke.Services.Upgrade
Imports DotNetNuke.Modules.Blog.Components

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
                txtMaxImageWidth.Text = BlogSettings.MaxImageWidth.ToString
                dntxtbxRecentEntriesMax.Value = BlogSettings.RecentEntriesMax
                txtRecentRssEntriesMax.Text = BlogSettings.RecentRssEntriesMax.ToString
                chkUploadOption.Checked = BlogSettings.EnableUploadOption
                chkShowSummary.Checked = BlogSettings.ShowSummary
                chkShowSeoFriendlyUrl.Checked = BlogSettings.ShowSeoFriendlyUrl
                chkEnforceSummaryTruncation.Checked = BlogSettings.EnforceSummaryTruncation
                chkAllowSummaryHtml.Checked = BlogSettings.AllowSummaryHtml
                chkAllowWLW.Checked = BlogSettings.AllowWLW
                chkIncludeBody.Checked = BlogSettings.IncludeBody
                chkIncludeCategoriesInDescription.Checked = BlogSettings.IncludeCategoriesInDescription
                chkIncludeTagsInDescription.Checked = BlogSettings.IncludeTagsInDescription
                chkAllowChildBlogs.Checked = BlogSettings.AllowChildBlogs
                chkAllowMultipleCategories.Checked = BlogSettings.AllowMultipleCategories
                chkUseWLWExcerpt.Checked = BlogSettings.UseWLWExcerpt
                ddlCatVocabRoot.SelectedValue = BlogSettings.VocabularyId.ToString()
                ddlSocialSharingMode.SelectedValue = BlogSettings.SocialSharingMode
                txtAddThisId.Text = BlogSettings.AddThisId
                txtFacebookAppId.Text = BlogSettings.FacebookAppId
                chkEnablePlusOne.Checked = BlogSettings.EnablePlusOne
                chkEnableTwitter.Checked = BlogSettings.EnableTwitter
                chkEnableLinkedIN.Checked = BlogSettings.EnableLinkedIn

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

                If cblPortalFiles.Items.Count < 1 Then
                    pnlPortalFiles.Visible = False
                End If

                'ForumBlog.Utils.isForumBlogInstalled(PortalId, TabId, True)

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
                .MaxImageWidth = CInt(txtMaxImageWidth.Text)
                .RecentEntriesMax = CInt(dntxtbxRecentEntriesMax.Value)
                .RecentRssEntriesMax = CInt(txtRecentRssEntriesMax.Text)
                .EnableUploadOption = chkUploadOption.Checked
                .ShowSummary = chkShowSummary.Checked
                .ShowSeoFriendlyUrl = chkShowSeoFriendlyUrl.Checked
                .PageBlogs = CInt(cmbPageBlogs.SelectedItem.Value)
                .EnforceSummaryTruncation = chkEnforceSummaryTruncation.Checked
                .IncludeBody = chkIncludeBody.Checked
                .IncludeCategoriesInDescription = chkIncludeCategoriesInDescription.Checked
                .IncludeTagsInDescription = chkIncludeTagsInDescription.Checked
                .AllowSummaryHtml = chkAllowSummaryHtml.Checked
                .AllowChildBlogs = chkAllowChildBlogs.Checked
                .AllowWLW = chkAllowWLW.Checked
                .AllowMultipleCategories = chkAllowMultipleCategories.Checked
                .UseWLWExcerpt = chkUseWLWExcerpt.Checked
                .VocabularyId = Convert.ToInt32(ddlCatVocabRoot.SelectedValue)
                .SocialSharingMode = ddlSocialSharingMode.SelectedItem.Value
                .AddThisId = txtAddThisId.Text
                .FacebookAppId = txtFacebookAppId.Text
                .EnablePlusOne = chkEnablePlusOne.Checked
                .EnableTwitter = chkEnableTwitter.Checked
                .EnableLinkedIn = chkEnableLinkedIN.Checked

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

        ' recalculate child blogs
        Dim totalBlogs As Integer = (New BlogController).GetPortalBlogs(PortalId, True).Count
        Dim parentBlogs As Integer = (New BlogController).GetPortalParentBlogs(PortalId).Count
        lblChildBlogsStatus.Text = String.Format(GetString("lblChildBlogsStatus", LocalResourceFile), CInt(totalBlogs - parentBlogs))
    End Sub

    Protected Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click
        Dim _CustomUpgrade As New Components.Upgrade.ModuleUpgrade
        '_CustomUpgrade.MigrateTaxonomyFolksonomy()
        _CustomUpgrade.MigrateContentItems(ModuleContext.PortalId)
    End Sub

#End Region

#Region "Private Methods"

    Private Sub PopulateDropDowns()
        Dim objBlog As New BlogController
        cmbPageBlogs.DataSource = objBlog.GetParentsChildBlogs(PortalId, -1, False)
        cmbPageBlogs.DataBind()
        cmbPageBlogs.Items.Insert(0, New ListItem("<" & GetString("Not_Specified", SharedResourceFile) & ">", "-1"))
        Try
            cmbPageBlogs.Items.FindByValue(CStr(BlogSettings.PageBlogs)).Selected = True
        Catch
        End Try

        ' calculate child blogs
        Dim totalBlogs As Integer = objBlog.GetPortalBlogs(PortalId, True).Count
        Dim parentBlogs As Integer = objBlog.GetPortalParentBlogs(PortalId).Count
        Dim childBlogs As Integer = CInt(totalBlogs - parentBlogs)

        If childBlogs < 1 Then
            ' prevent those without child blogs from adding them (we want to remove in the future)
            pnlAllowChildBlogs.Visible = False
            chkAllowChildBlogs.Checked = False
            pnlMigrateChildBlogs.Visible = False
        Else
            lblChildBlogsStatus.Text = String.Format(GetString("lblChildBlogsStatus", LocalResourceFile), childBlogs)
        End If

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