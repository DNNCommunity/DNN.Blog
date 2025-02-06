﻿'
' DNN Connect - http://dnn-connect.org
' Copyright (c) 2015
' by DNN Connect
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

Imports System.Linq
Imports DotNetNuke.Common
Imports DotNetNuke.Framework
Imports DotNetNuke.Modules.Blog.Core.Common
Imports DotNetNuke.Modules.Blog.Core.Entities.Blogs
Imports DotNetNuke.Services.Exceptions

Public Class BlogEdit
  Inherits BlogModuleBase

  Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init

    JavaScriptLibraries.JavaScript.RequestRegistration(JavaScriptLibraries.CommonJs.DnnPlugins)
    If Not BlogContext.Security.IsEditor Then
      If Not BlogContext.Security.IsBlogger Then
        Response.Redirect(NavigateURL("Access Denied"), True)
      End If
      If BlogContext.Blog IsNot Nothing AndAlso BlogContext.Blog.OwnerUserId <> UserId Then
        Response.Redirect(NavigateURL("Access Denied"), True)
      End If
    End If
    txtTitle.DefaultLanguage = PortalSettings.DefaultLanguage
    txtDescription.DefaultLanguage = PortalSettings.DefaultLanguage

  End Sub

  Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

    If Not BlogContext.Security.IsBlogger Then
      Throw New Exception("You do not have access to this resource. Please check your login status.")
    End If

    If Not Page.IsPostBack Then
      If BlogContext.Blog Is Nothing Then BlogContext.Blog = New BlogInfo ' initialize fields

      txtTitle.DefaultText = BlogContext.Blog.Title
      txtTitle.LocalizedTexts = BlogContext.Blog.TitleLocalizations
      txtTitle.InitialBind()
      txtDescription.DefaultText = BlogContext.Blog.Description
      txtDescription.LocalizedTexts = BlogContext.Blog.DescriptionLocalizations
      txtDescription.InitialBind()

      If BlogContext.IsMultiLingualSite Then
        ddLocale.DataSource = DotNetNuke.Services.Localization.LocaleController.Instance.GetLocales(PortalId).Values.OrderBy(Function(t) t.NativeName)
        ddLocale.DataValueField = "Code"
        ddLocale.DataBind()
        Try
          ddLocale.Items.FindByValue(BlogContext.Blog.Locale).Selected = True
        Catch ex As Exception
        End Try
        chkFullLocalization.Checked = BlogContext.Blog.FullLocalization
      Else
        rowLocale.Visible = False
        rowFullLocalization.Visible = False
      End If
      chkPublic.Checked = BlogContext.Blog.Published
      chkSyndicate.Checked = BlogContext.Blog.Syndicated
      If BlogContext.Blog.SyndicationEmail Is Nothing Then
        txtSyndicationEmail.Text = ModuleContext.PortalSettings.UserInfo.Email
      Else
        txtSyndicationEmail.Text = BlogContext.Blog.SyndicationEmail
      End If
      chkIncludeImagesInFeed.Checked = BlogContext.Blog.IncludeImagesInFeed
      chkIncludeAuthorInFeed.Checked = BlogContext.Blog.IncludeAuthorInFeed
      chkEnablePingBackReceive.Checked = BlogContext.Blog.EnablePingBackReceive
      chkEnablePingBackSend.Checked = BlogContext.Blog.EnablePingBackSend
      chkAutoApprovePingBack.Checked = BlogContext.Blog.AutoApprovePingBack
      chkEnableTrackBackReceive.Checked = BlogContext.Blog.EnableTrackBackReceive
      chkEnableTrackBackSend.Checked = BlogContext.Blog.EnableTrackBackSend
      chkAutoApproveTrackBack.Checked = BlogContext.Blog.AutoApproveTrackBack
      txtCopyright.Text = BlogContext.Blog.Copyright
      cmdDelete.Visible = CBool(BlogContext.BlogId <> -1)
      If Not String.IsNullOrEmpty(BlogContext.Blog.Image) Then
        imgBlogImage.ImageUrl = ResolveUrl(Core.Common.Globals.glbImageHandlerPath) & String.Format("?TabId={0}&ModuleId={1}&Blog={2}&w=100&h=100&c=1&key={3}", TabId, Settings.ModuleId, BlogContext.BlogId, BlogContext.Blog.Image)
        imgBlogImage.Visible = True
        cmdImageRemove.Visible = True
      Else
        imgBlogImage.Visible = False
        cmdImageRemove.Visible = False
      End If
      ' ghost writing
      chkMustApproveGhostPosts.Checked = BlogContext.Blog.MustApproveGhostPosts
      chkPublishAsOwner.Checked = BlogContext.Blog.PublishAsOwner
      ctlPermissions.Permissions = BlogContext.Blog.Permissions
      ctlPermissions.TabId = TabId
      ctlPermissions.CurrentUserId = UserId
      ctlPermissions.UserIsAdmin = BlogContext.Security.UserIsAdmin

      hlCancel.NavigateUrl = EditUrl("Manage")
    End If

  End Sub

  Protected Sub cmdDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDelete.Click
    Try
      If Not BlogContext.Blog Is Nothing Then
        BlogsController.DeleteBlog(BlogContext.Blog.BlogID)
      End If
      Response.Redirect(EditUrl("Manage"), False)
    Catch exc As Exception
      ProcessModuleLoadException(Me, exc)
    End Try
  End Sub

  Private Sub cmdUpdate_Click(sender As Object, e As EventArgs) Handles cmdUpdate.Click
    Try
      If Page.IsValid = True Then
        If BlogContext.Blog Is Nothing Then
          BlogContext.Blog = New BlogInfo
          With BlogContext.Blog
            .ModuleID = Settings.ModuleId
            .OwnerUserId = UserId
          End With
        End If
        With BlogContext.Blog
          .Title = txtTitle.DefaultText
          .TitleLocalizations = txtTitle.GetLocalizedTexts
          .Description = txtDescription.DefaultText
          .DescriptionLocalizations = txtDescription.GetLocalizedTexts
          If BlogContext.IsMultiLingualSite Then
            .Locale = ddLocale.SelectedValue
            .FullLocalization = chkFullLocalization.Checked
          Else
            .Locale = PortalSettings.DefaultLanguage
          End If
          .Published = chkPublic.Checked
          .Syndicated = chkSyndicate.Checked
          .SyndicationEmail = txtSyndicationEmail.Text
          .IncludeImagesInFeed = chkIncludeImagesInFeed.Checked
          .IncludeAuthorInFeed = chkIncludeAuthorInFeed.Checked
          .EnablePingBackReceive = chkEnablePingBackReceive.Checked
          .EnablePingBackSend = chkEnablePingBackSend.Checked
          .AutoApprovePingBack = chkAutoApprovePingBack.Checked
          .EnableTrackBackReceive = chkEnableTrackBackReceive.Checked
          .EnableTrackBackSend = chkEnableTrackBackSend.Checked
          .AutoApproveTrackBack = chkAutoApproveTrackBack.Checked
          .Copyright = txtCopyright.Text.Trim
          .MustApproveGhostPosts = chkMustApproveGhostPosts.Checked
          .PublishAsOwner = chkPublishAsOwner.Checked
          .Permissions = ctlPermissions.Permissions
          If BlogContext.BlogId = -1 Then
            .BlogID = BlogsController.AddBlog(BlogContext.Blog, UserId)
            BlogContext.BlogId = .BlogID
          Else
            BlogsController.UpdateBlog(BlogContext.Blog, UserId)
          End If
          Modules.Blog.Core.Security.Permissions.BlogPermissionsController.UpdateBlogPermissions(BlogContext.Blog)
        End With

        If fileImage.HasFile Then
          Dim extension As String = IO.Path.GetExtension(fileImage.FileName).ToLower
          If Core.Common.Globals.glbPermittedFileExtensions.IndexOf(extension & ",") > -1 Then
            Dim saveDir As String = Core.Common.Globals.GetBlogDirectoryMapPath(BlogContext.BlogId)
            If Not IO.Directory.Exists(saveDir) Then IO.Directory.CreateDirectory(saveDir)
            If BlogContext.Blog.Image <> "" Then
              ' remove old images
              Dim imagesToDelete As New List(Of String)
              Dim d As New IO.DirectoryInfo(saveDir)
              For Each f As IO.FileInfo In d.GetFiles
                If f.Name.StartsWith(BlogContext.Blog.Image) Then
                  imagesToDelete.Add(f.FullName)
                End If
              Next
              For Each f As String In imagesToDelete
                Try
                  IO.File.Delete(f)
                Catch ex As Exception
                End Try
              Next
            End If
            Dim newFileName As String = Guid.NewGuid.ToString("D")
            fileImage.SaveAs(saveDir & newFileName & IO.Path.GetExtension(fileImage.FileName).ToLower)
            BlogContext.Blog.Image = newFileName
            BlogsController.UpdateBlog(BlogContext.Blog, UserId)
          End If
        End If

      End If
      Response.Redirect(EditUrl("Manage"), False)
    Catch exc As Exception
      ProcessModuleLoadException(Me, exc)
    End Try
  End Sub

  Private Sub cmdImageRemove_Click(sender As Object, e As EventArgs) Handles cmdImageRemove.Click

    If BlogContext.Blog IsNot Nothing Then
      If BlogContext.Blog.Image <> "" Then
        ' remove old images
        Dim saveDir As String = Core.Common.Globals.GetBlogDirectoryMapPath(BlogContext.BlogId)
        Dim imagesToDelete As New List(Of String)
        Dim d As New IO.DirectoryInfo(saveDir)
        For Each f As IO.FileInfo In d.GetFiles
          If f.Name.StartsWith(BlogContext.Blog.Image) Then
            imagesToDelete.Add(f.FullName)
          End If
        Next
        For Each f As String In imagesToDelete
          Try
            IO.File.Delete(f)
          Catch ex As Exception
          End Try
        Next
      End If
      BlogContext.Blog.Image = ""
      BlogsController.UpdateBlog(BlogContext.Blog, UserId)
    End If
    imgBlogImage.Visible = False
    cmdImageRemove.Visible = False

  End Sub


End Class