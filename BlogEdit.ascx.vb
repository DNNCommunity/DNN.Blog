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
  If Not Security.IsEditor Then
   If Not Security.IsBlogger Then
    Response.Redirect(NavigateURL("Access Denied"), True)
   End If
   If Blog IsNot Nothing AndAlso Blog.OwnerUserId <> UserId Then
    Response.Redirect(NavigateURL("Access Denied"), True)
   End If
  End If
  txtTitle.DefaultLanguage = PortalSettings.DefaultLanguage
  txtDescription.DefaultLanguage = PortalSettings.DefaultLanguage

 End Sub

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

  If Not Page.IsPostBack Then
   If Blog Is Nothing Then Blog = New BlogInfo ' initialize fields

   txtTitle.DefaultText = Blog.Title
   txtTitle.LocalizedTexts = Blog.TitleLocalizations
   txtTitle.InitialBind()
   txtDescription.DefaultText = Blog.Description
   txtDescription.LocalizedTexts = Blog.DescriptionLocalizations
   txtDescription.InitialBind()

   If IsMultiLingualSite Then
    If Settings.AllowAllLocales Then
     ddLocale.DataSource = System.Globalization.CultureInfo.GetCultures(Globalization.CultureTypes.SpecificCultures)
     ddLocale.DataValueField = "Name"
    Else
     ddLocale.DataSource = DotNetNuke.Services.Localization.LocaleController.Instance.GetLocales(PortalId).Values
     ddLocale.DataValueField = "Code"
    End If
    ddLocale.DataBind()
    Try
     ddLocale.Items.FindByValue(Blog.Locale).Selected = True
    Catch ex As Exception
    End Try
    chkFullLocalization.Checked = Blog.FullLocalization
   Else
    rowLocale.Visible = False
    rowFullLocalization.Visible = False
   End If
   chkPublic.Checked = Blog.Published
   chkSyndicate.Checked = Blog.Syndicated
   If Blog.SyndicationEmail Is Nothing Then
    txtSyndicationEmail.Text = ModuleContext.PortalSettings.UserInfo.Email
   Else
    txtSyndicationEmail.Text = Blog.SyndicationEmail
   End If
   chkIncludeImagesInFeed.Checked = Blog.IncludeImagesInFeed
   chkIncludeAuthorInFeed.Checked = Blog.IncludeAuthorInFeed
   txtCopyright.Text = Blog.Copyright
   cmdDelete.Visible = CBool(BlogId <> -1)
   If Not String.IsNullOrEmpty(Blog.Image) Then
    imgBlogImage.ImageUrl = ResolveUrl(glbImageHandlerPath) & String.Format("?TabId={0}&ModuleId={1}&Blog={2}&w=100&h=100&c=1&key={3}", TabId, Settings.ModuleId, BlogId, Blog.Image)
    imgBlogImage.Visible = True
    cmdImageRemove.Visible = True
   Else
    imgBlogImage.Visible = False
    cmdImageRemove.Visible = False
   End If
   ' ghost writing
   chkMustApproveGhostPosts.Checked = Blog.MustApproveGhostPosts
   chkPublishAsOwner.Checked = Blog.PublishAsOwner
   ctlPermissions.Permissions = Blog.Permissions
   ctlPermissions.TabId = TabId
   ctlPermissions.CurrentUserId = UserId
   ctlPermissions.UserIsAdmin = Security.UserIsAdmin

   hlCancel.NavigateUrl = EditUrl("Manage")
  End If

 End Sub

 Protected Sub cmdDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDelete.Click
  Try
   If Not Blog Is Nothing Then
    BlogsController.DeleteBlog(Blog.BlogID)
   End If
   Response.Redirect(EditUrl("Manage"), False)
  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 Private Sub cmdUpdate_Click(sender As Object, e As System.EventArgs) Handles cmdUpdate.Click
  Try
   If Page.IsValid = True Then
    If Blog Is Nothing Then
     Blog = New BlogInfo
     With Blog
      .ModuleID = Settings.ModuleId
      .OwnerUserId = Me.UserId
     End With
    End If
    With Blog
     .Title = txtTitle.DefaultText
     .TitleLocalizations = txtTitle.GetLocalizedTexts
     .Description = txtDescription.DefaultText
     .DescriptionLocalizations = txtDescription.GetLocalizedTexts
     If IsMultiLingualSite Then
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
     .Copyright = txtCopyright.Text.Trim
     .MustApproveGhostPosts = chkMustApproveGhostPosts.Checked
     .PublishAsOwner = chkPublishAsOwner.Checked
     .Permissions = ctlPermissions.Permissions
     If BlogId = -1 Then
      .BlogID = BlogsController.AddBlog(Blog, UserId)
     Else
      BlogsController.UpdateBlog(Blog, UserId)
     End If
     Modules.Blog.Security.Permissions.BlogPermissionsController.UpdateBlogPermissions(Blog)
    End With

    If fileImage.HasFile Then
     Dim extension As String = IO.Path.GetExtension(fileImage.FileName).ToLower
     If glbPermittedFileExtensions.IndexOf(extension & ",") > -1 Then
      Dim saveDir As String = GetBlogDirectoryMapPath(BlogId)
      If Not IO.Directory.Exists(saveDir) Then IO.Directory.CreateDirectory(saveDir)
      If Blog.Image <> "" Then
       ' remove old images
       Dim imagesToDelete As New List(Of String)
       Dim d As New IO.DirectoryInfo(saveDir)
       For Each f As IO.FileInfo In d.GetFiles
        If f.Name.StartsWith(Blog.Image) Then
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
      Blog.Image = newFileName
      BlogsController.UpdateBlog(Blog, UserId)
     End If
    End If

   End If
   Response.Redirect(EditUrl("Manage"), False)
  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 Private Sub cmdImageRemove_Click(sender As Object, e As System.EventArgs) Handles cmdImageRemove.Click

  If Blog IsNot Nothing Then
   If Blog.Image <> "" Then
    ' remove old images
    Dim saveDir As String = GetBlogDirectoryMapPath(BlogId)
    Dim imagesToDelete As New List(Of String)
    Dim d As New IO.DirectoryInfo(saveDir)
    For Each f As IO.FileInfo In d.GetFiles
     If f.Name.StartsWith(Blog.Image) Then
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
   Blog.Image = ""
   BlogsController.UpdateBlog(Blog, UserId)
  End If
  imgBlogImage.Visible = False
  cmdImageRemove.Visible = False

 End Sub


End Class