Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports DotNetNuke.Framework

Public Class BlogEdit
 Inherits BlogModuleBase

 Private Sub Page_Init1(sender As Object, e As System.EventArgs) Handles Me.Init

  jQuery.RequestDnnPluginsRegistration()
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

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

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
    If Settings.AllowAllLocales Then
     ddLocale.DataSource = System.Globalization.CultureInfo.GetCultures(Globalization.CultureTypes.SpecificCultures)
     ddLocale.DataValueField = "Name"
    Else
     ddLocale.DataSource = DotNetNuke.Services.Localization.LocaleController.Instance.GetLocales(PortalId).Values
     ddLocale.DataValueField = "Code"
    End If
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
    imgBlogImage.ImageUrl = ResolveUrl(glbImageHandlerPath) & String.Format("?TabId={0}&ModuleId={1}&Blog={2}&w=100&h=100&c=1&key={3}", TabId, Settings.ModuleId, BlogContext.BlogId, BlogContext.Blog.Image)
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

 Private Sub cmdUpdate_Click(sender As Object, e As System.EventArgs) Handles cmdUpdate.Click
  Try
   If Page.IsValid = True Then
    If BlogContext.Blog Is Nothing Then
     BlogContext.Blog = New BlogInfo
     With BlogContext.Blog
      .ModuleID = Settings.ModuleId
      .OwnerUserId = Me.UserId
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
     Modules.Blog.Security.Permissions.BlogPermissionsController.UpdateBlogPermissions(BlogContext.Blog)
    End With

    If fileImage.HasFile Then
     Dim extension As String = IO.Path.GetExtension(fileImage.FileName).ToLower
     If glbPermittedFileExtensions.IndexOf(extension & ",") > -1 Then
      Dim saveDir As String = GetBlogDirectoryMapPath(BlogContext.BlogId)
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

 Private Sub cmdImageRemove_Click(sender As Object, e As System.EventArgs) Handles cmdImageRemove.Click

  If BlogContext.Blog IsNot Nothing Then
   If BlogContext.Blog.Image <> "" Then
    ' remove old images
    Dim saveDir As String = GetBlogDirectoryMapPath(BlogContext.BlogId)
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