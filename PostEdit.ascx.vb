Imports System
Imports System.IO
Imports DotNetNuke.Modules.Blog.Common
Imports DotNetNuke.Web.Client.ClientResourceManagement
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Localization.Localization
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Framework
Imports System.Linq
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Entities.Posts
Imports DotNetNuke.Services.Localization
Imports Telerik.Web.UI
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports DotNetNuke.Modules.Blog.Integration
Imports DotNetNuke.Modules.Blog.Entities.Terms

Public Class PostEdit
 Inherits BlogModuleBase

#Region " Public Properties "

 Public ReadOnly Property FilePath() As String
  Get
   Return Me.PortalSettings.HomeDirectory & Me.ModuleConfiguration.DesktopModule.FriendlyName & "/"
  End Get
 End Property

#End Region

#Region " Private Members "

 Private m_oTags As ArrayList

#End Region

#Region " Event Handlers "
 Protected Overloads Sub Page_Init(sender As System.Object, e As System.EventArgs) Handles MyBase.Init

  Try

   ctlTags.ModuleConfiguration = Me.ModuleConfiguration
   ctlCategories.ModuleConfiguration = Me.ModuleConfiguration
   ctlCategories.VocabularyId = Settings.VocabularyId

   If BlogContext.Blog Is Nothing Then
    Dim blogList As IEnumerable(Of BlogInfo) = Nothing
    blogList = BlogsController.GetBlogsByModule(Settings.ModuleId, UserId, BlogContext.Locale).Values.Where(Function(b)
                                                                                                             Return b.OwnerUserId = UserId
                                                                                                            End Function)
    If blogList.Count > 0 Then
     BlogContext.Blog = blogList(0)
     BlogContext.Security = New Modules.Blog.Security.ContextSecurity(Settings.ModuleId, TabId, BlogContext.Blog, UserInfo)
    Else
     blogList = BlogsController.GetBlogsByModule(Settings.ModuleId, UserId, BlogContext.Locale).Values.Where(Function(b)
                                                                                                              Return (b.CanAdd And BlogContext.Security.CanAddPost) Or (b.CanEdit And BlogContext.Security.CanEditPost And BlogContext.ContentItemId > -1)
                                                                                                             End Function)
     If blogList.Count > 0 Then
      BlogContext.Blog = blogList(0)
      BlogContext.Security = New Modules.Blog.Security.ContextSecurity(Settings.ModuleId, TabId, BlogContext.Blog, UserInfo)
     Else
      Throw New Exception("Could not find a blog for you to post to")
     End If
    End If
   End If

   If Not BlogContext.security.CanAddPost Then
    Response.Redirect(NavigateURL("Access Denied"))
   End If

   txtTitle.DefaultLanguage = BlogContext.Blog.Locale
   txtDescription.DefaultLanguage = BlogContext.Blog.Locale
   teBlogPost.DefaultLanguage = BlogContext.Blog.Locale
   txtTitle.ShowTranslations = BlogContext.Blog.FullLocalization
   txtDescription.ShowTranslations = BlogContext.Blog.FullLocalization
   teBlogPost.ShowTranslations = BlogContext.Blog.FullLocalization

   ' Summary
   Select Case Settings.SummaryModel
    Case SummaryType.HtmlIndependent
     txtDescription.ShowRichTextBox = True
    Case SummaryType.HtmlPrecedesPost
     txtDescription.ShowRichTextBox = True
     lblSummaryPrecedingWarning.Visible = True
    Case Else ' plain text
     txtDescription.ShowRichTextBox = False
   End Select

  Catch
  End Try

 End Sub

 Protected Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

  Try

   If Not BlogContext.security.CanEditPost And Not BlogContext.security.CanAddPost Then
    Throw New Exception("You do not have access to this resource. Please check your login status.")
   End If

   If Not Page.IsPostBack Then

    ' Categories
    If Settings.VocabularyId < 1 Then
     pnlCategories.Visible = False
    End If

    If Settings.AllowAllLocales Then
     ddLocale.DataSource = System.Globalization.CultureInfo.GetCultures(Globalization.CultureTypes.SpecificCultures)
     ddLocale.DataValueField = "Name"
    Else
     ddLocale.DataSource = DotNetNuke.Services.Localization.LocaleController.Instance.GetLocales(PortalId).Values
     ddLocale.DataValueField = "Code"
    End If
    ddLocale.DataBind()
    ddLocale.Items.Insert(0, New ListItem(LocalizeString("DefaultLocale"), ""))
    rowLocale.Visible = CBool(BlogContext.IsMultiLingualSite And Not BlogContext.Blog.FullLocalization)

    ' Buttons
    If BlogContext.BlogId > -1 Then
     hlCancel.NavigateUrl = NavigateURL(TabId, "", "Blog=" & BlogContext.BlogId.ToString)
    ElseIf BlogContext.ContentItemId > -1 Then
     hlCancel.NavigateUrl = NavigateURL(TabId, "", "Post=" & BlogContext.ContentItemId.ToString)
    Else
     hlCancel.NavigateUrl = ModuleContext.NavigateUrl(ModuleContext.TabId, "", False, "")
    End If
    cmdDelete.Visible = CBool(BlogContext.Post IsNot Nothing)

    If Not BlogContext.Post Is Nothing Then

     Dim PostBody As New PostBodyAndSummary(BlogContext.Post, Settings.SummaryModel, True)

     ' Content
     txtTitle.DefaultText = HttpUtility.HtmlDecode(BlogContext.Post.Title)
     txtTitle.LocalizedTexts = BlogContext.Post.TitleLocalizations
     txtTitle.InitialBind()
     txtDescription.DefaultText = PostBody.Summary
     txtDescription.LocalizedTexts = PostBody.SummaryLocalizations
     txtDescription.InitialBind()
     teBlogPost.DefaultText = PostBody.Body
     teBlogPost.LocalizedTexts = PostBody.BodyLocalizations
     teBlogPost.InitialBind()

     ' Publishing
     chkPublished.Checked = BlogContext.Post.Published
     chkAllowComments.Checked = BlogContext.Post.AllowComments
     chkDisplayCopyright.Checked = BlogContext.Post.DisplayCopyright
     If chkDisplayCopyright.Checked Then
      pnlCopyright.Visible = True
      txtCopyright.Text = BlogContext.Post.Copyright
     End If
     ' Date
     litTimezone.Text = BlogContext.UiTimeZone.DisplayName
     Dim publishDate As DateTime = UtcToLocalTime(BlogContext.Post.PublishedOnDate, BlogContext.UiTimeZone)
     dpPostDate.Culture = Threading.Thread.CurrentThread.CurrentUICulture
     dpPostDate.SelectedDate = publishDate
     tpPostTime.Culture = Threading.Thread.CurrentThread.CurrentUICulture
     tpPostTime.SelectedDate = publishDate

     ' Summary, Image, Categories, Tags
     Try
      ddLocale.Items.FindByValue(BlogContext.Post.Locale).Selected = True
     Catch ex As Exception
     End Try
     If Not String.IsNullOrEmpty(BlogContext.Post.Image) Then
      imgPostImage.ImageUrl = ResolveUrl(glbImageHandlerPath) & String.Format("?TabId={0}&ModuleId={1}&Blog={2}&Post={3}&w=100&h=100&c=1&key={4}", TabId, Settings.ModuleId, BlogContext.BlogId, BlogContext.ContentItemId, BlogContext.Post.Image)
      imgPostImage.Visible = True
      cmdImageRemove.Visible = True
     Else
      imgPostImage.Visible = False
      cmdImageRemove.Visible = False
     End If
     ctlTags.Terms = BlogContext.Post.PostTags
     ctlCategories.SelectedCategories = BlogContext.Post.PostCategories

    Else

     ddLocale.Items.FindByValue("").Selected = True
     ' Publishing
     chkAllowComments.Checked = BlogContext.Blog.Permissions.ContainsPermissionKey("ADDCOMMENT")
     ' Date
     litTimezone.Text = BlogContext.UiTimeZone.DisplayName
     Dim publishDate As Date = UtcToLocalTime(DateTime.Now.ToUniversalTime, BlogContext.UiTimeZone)
     dpPostDate.Culture = Threading.Thread.CurrentThread.CurrentUICulture
     dpPostDate.SelectedDate = publishDate
     tpPostTime.Culture = Threading.Thread.CurrentThread.CurrentUICulture
     tpPostTime.SelectedDate = publishDate

    End If

    ' Security
    If BlogContext.Blog.MustApproveGhostPosts AndAlso Not BlogContext.Security.CanApprovePost Then
     chkPublished.Checked = False
     chkPublished.Enabled = False
    End If

   End If

  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try

 End Sub

 Private Sub cmdSave_Click(sender As Object, e As System.EventArgs) Handles cmdSave.Click

  Try

   If Page.IsValid = True Then

    If BlogContext.Post Is Nothing Then
     BlogContext.Post = New PostInfo
     BlogContext.Post.Blog = BlogContext.Blog
    End If
    If BlogContext.ContentItemId = -1 And Not BlogContext.Security.CanAddPost Then Throw New Exception("You can't add posts to this blog")
    If BlogContext.ContentItemId <> -1 And Not BlogContext.Security.CanEditPost Then Throw New Exception("You're not allowed to edit this post")

    Dim firstPublish As Boolean = CBool((Not BlogContext.Post.Published) And chkPublished.Checked)

    ' Contents and summary
    BlogContext.Post.BlogID = BlogContext.BlogId
    BlogContext.Post.Title = txtTitle.DefaultText
    BlogContext.Post.TitleLocalizations = txtTitle.GetLocalizedTexts
    Dim PostBody As New PostBodyAndSummary(teBlogPost, txtDescription, Settings.SummaryModel, True)
    PostBody.WriteToPost(BlogContext.Post, Settings.SummaryModel, False, True)

    ' Publishing
    BlogContext.Post.Published = chkPublished.Checked
    BlogContext.Post.AllowComments = chkAllowComments.Checked
    BlogContext.Post.DisplayCopyright = chkDisplayCopyright.Checked
    BlogContext.Post.Copyright = txtCopyright.Text
    ' Date
    BlogContext.Post.PublishedOnDate = CDate(dpPostDate.SelectedDate)
    Dim hour As Integer = tpPostTime.SelectedDate.Value.Hour
    Dim minute As Integer = tpPostTime.SelectedDate.Value.Minute
    BlogContext.Post.PublishedOnDate = BlogContext.Post.PublishedOnDate.AddHours(hour)
    BlogContext.Post.PublishedOnDate = BlogContext.Post.PublishedOnDate.AddMinutes(minute)
    BlogContext.Post.PublishedOnDate = TimeZoneInfo.ConvertTimeToUtc(BlogContext.Post.PublishedOnDate, BlogContext.UiTimeZone)

    ' Categories, Tags
    Dim terms As New List(Of TermInfo)
    ctlTags.CreateMissingTerms()
    terms.AddRange(ctlTags.Terms)
    terms.AddRange(ctlCategories.SelectedCategories)
    BlogContext.Post.Terms.Clear()
    BlogContext.Post.Terms.AddRange(terms)

    If BlogContext.IsMultiLingualSite And Not BlogContext.Blog.FullLocalization Then
     BlogContext.Post.Locale = ddLocale.SelectedValue
    Else
     BlogContext.Post.Locale = ""
    End If

    ' Image
    Dim saveDir As String = GetPostDirectoryMapPath(BlogContext.Post)
    Dim savedFile As String = ""
    If fileImage.HasFile Then
     Dim extension As String = IO.Path.GetExtension(fileImage.FileName).ToLower
     If glbPermittedFileExtensions.IndexOf(extension & ",") > -1 Then
      If Not IO.Directory.Exists(saveDir) Then IO.Directory.CreateDirectory(saveDir)
      If BlogContext.Post.Image <> "" Then
       ' remove old images
       Dim imagesToDelete As New List(Of String)
       Dim d As New IO.DirectoryInfo(saveDir)
       For Each f As IO.FileInfo In d.GetFiles
        If f.Name.StartsWith(BlogContext.Post.Image) Then
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
      savedFile = saveDir & newFileName & IO.Path.GetExtension(fileImage.FileName).ToLower
      fileImage.SaveAs(savedFile)
      BlogContext.Post.Image = newFileName
     End If
    End If

    ' Final Security Check
    If BlogContext.Blog.MustApproveGhostPosts AndAlso Not BlogContext.Security.CanApprovePost Then
     BlogContext.Post.Published = False
     firstPublish = False
    End If

    ' Add if new, otherwise update
    If BlogContext.ContentItemId = -1 Then
     BlogContext.Post.ContentItemId = PostsController.AddPost(BlogContext.Post, UserId)
     BlogContext.ContentItemId = BlogContext.Post.ContentItemId
     If savedFile <> "" Then ' move file if it was saved
      saveDir = GetPostDirectoryMapPath(BlogContext.Post)
      IO.Directory.CreateDirectory(saveDir)
      Dim dest As String = saveDir & BlogContext.Post.Image & IO.Path.GetExtension(fileImage.FileName).ToLower
      IO.File.Move(savedFile, dest)
     End If
    Else
     PostsController.UpdatePost(BlogContext.Post, UserId)
    End If

    If firstPublish Then

     PostsController.PublishPost(BlogContext.Post, True, UserInfo.UserID)

    ElseIf BlogContext.Blog.MustApproveGhostPosts And UserId <> BlogContext.Blog.OwnerUserId Then

     Dim title As String = Localization.GetString("ApprovePostNotifyBody", SharedResourceFileName)
     Dim summary As String = "<a target='_blank' href='" + BlogContext.Post.PermaLink(PortalSettings) + "'>" + BlogContext.Post.Title + "</a>"
     NotificationController.PostPendingApproval(BlogContext.Blog, BlogContext.Post, ModuleContext.PortalId, summary, title)

    End If

    Response.Redirect(NavigateURL(TabId, "", "Post=" & BlogContext.ContentItemId.ToString), False)

   End If
  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try

 End Sub

 Protected Sub cmdDelete_Click(sender As Object, e As EventArgs) Handles cmdDelete.Click
  Try
   DeleteAllFiles()
   PostsController.DeletePost(BlogContext.Post.ContentItemId, BlogContext.Post.BlogID, ModuleContext.PortalId, Settings.VocabularyId)
   Response.Redirect(NavigateURL(TabId, "", "Blog=" & BlogContext.BlogId.ToString), False)
  Catch exc As Exception    'Module failed to load
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 Protected Sub valPost_ServerValidate(source As Object, args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles valPost.ServerValidate
  args.IsValid = teBlogPost.DefaultText.Length > 0
 End Sub

 Protected Sub chkDisplayCopyright_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkDisplayCopyright.CheckedChanged
  pnlCopyright.Visible = chkDisplayCopyright.Checked
  If pnlCopyright.Visible Then
   txtCopyright.Text = CreateCopyRight()
  End If
 End Sub

 Private Sub cmdImageRemove_Click(sender As Object, e As System.EventArgs) Handles cmdImageRemove.Click

  If BlogContext.Post IsNot Nothing Then
   If BlogContext.Post.Image <> "" Then
    ' remove old images
    Dim saveDir As String = PortalSettings.HomeDirectoryMapPath & String.Format("\Blog\Files\{0}\{1}\", BlogContext.BlogId, BlogContext.ContentItemId)
    Dim imagesToDelete As New List(Of String)
    Dim d As New IO.DirectoryInfo(saveDir)
    For Each f As IO.FileInfo In d.GetFiles
     If f.Name.StartsWith(BlogContext.Post.Image) Then
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
   BlogContext.Post.Image = ""
   PostsController.UpdatePost(BlogContext.Post, UserId)
  End If
  imgPostImage.Visible = False
  cmdImageRemove.Visible = False

 End Sub
#End Region

#Region " Private Methods "
 Private Function CreateCopyRight() As String
  Return GetString("msgCopyright", LocalResourceFile) & Date.UtcNow.Year & " " & BlogContext.Blog.DisplayName
 End Function
#End Region

#Region " Upload Feature Methods "
 Private Sub DeleteAllFiles()
  Try
   System.IO.Directory.Delete(FileController.getPostDir(Me.FilePath, BlogContext.Post), True)
  Catch

  End Try
 End Sub

#End Region

End Class