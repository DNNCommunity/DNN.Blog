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

   If Blog Is Nothing Then
    Dim blogList As IEnumerable(Of BlogInfo) = Nothing
    blogList = BlogsController.GetBlogsByModule(Settings.ModuleId, UserId, Locale).Values.Where(Function(b)
                                                                                                 Return b.OwnerUserId = UserId
                                                                                                End Function)
    If blogList.Count > 0 Then
     Blog = blogList(0)
     Security = New Modules.Blog.Security.ContextSecurity(Settings.ModuleId, TabId, Blog, UserInfo)
    Else
     blogList = BlogsController.GetBlogsByModule(Settings.ModuleId, UserId, Locale).Values.Where(Function(b)
                                                                                                  Return (b.CanAdd And Security.CanAddPost) Or (b.CanEdit And Security.CanEditPost And ContentItemId > -1)
                                                                                                 End Function)
     If blogList.Count > 0 Then
      Blog = blogList(0)
      Security = New Modules.Blog.Security.ContextSecurity(Settings.ModuleId, TabId, Blog, UserInfo)
     Else
      Throw New Exception("Could not find a blog for you to post to")
     End If
    End If
   End If

   If Not Security.CanAddPost Then
    Response.Redirect(NavigateURL("Access Denied"))
   End If

   txtTitle.DefaultLanguage = Blog.Locale
   txtDescription.DefaultLanguage = Blog.Locale
   teBlogPost.DefaultLanguage = Blog.Locale
   txtTitle.ShowTranslations = Blog.FullLocalization
   txtDescription.ShowTranslations = Blog.FullLocalization
   teBlogPost.ShowTranslations = Blog.FullLocalization

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

   If Not Security.CanEditPost And Not Security.CanAddPost Then
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
    rowLocale.Visible = CBool(IsMultiLingualSite And Not Blog.FullLocalization)

    ' Buttons
    If BlogId > -1 Then
     hlCancel.NavigateUrl = NavigateURL(TabId, "", "Blog=" & BlogId.ToString)
    ElseIf ContentItemId > -1 Then
     hlCancel.NavigateUrl = NavigateURL(TabId, "", "Post=" & ContentItemId.ToString)
    Else
     hlCancel.NavigateUrl = ModuleContext.NavigateUrl(ModuleContext.TabId, "", False, "")
    End If
    cmdDelete.Visible = CBool(Post IsNot Nothing)

    If Not Post Is Nothing Then

     Dim PostBody As New PostBodyAndSummary(Post, Settings.SummaryModel, True)

     ' Content
     txtTitle.DefaultText = HttpUtility.HtmlDecode(Post.Title)
     txtTitle.LocalizedTexts = Post.TitleLocalizations
     txtTitle.InitialBind()
     txtDescription.DefaultText = PostBody.Summary
     txtDescription.LocalizedTexts = PostBody.SummaryLocalizations
     txtDescription.InitialBind()
     teBlogPost.DefaultText = PostBody.Body
     teBlogPost.LocalizedTexts = PostBody.BodyLocalizations
     teBlogPost.InitialBind()

     ' Publishing
     chkPublished.Checked = Post.Published
     chkAllowComments.Checked = Post.AllowComments
     chkDisplayCopyright.Checked = Post.DisplayCopyright
     If chkDisplayCopyright.Checked Then
      pnlCopyright.Visible = True
      txtCopyright.Text = Post.Copyright
     End If
     ' Date
     litTimezone.Text = UiTimeZone.DisplayName
     Dim publishDate As DateTime = UtcToLocalTime(Post.PublishedOnDate, UiTimeZone)
     dpPostDate.Culture = Threading.Thread.CurrentThread.CurrentUICulture
     dpPostDate.SelectedDate = publishDate
     tpPostTime.Culture = Threading.Thread.CurrentThread.CurrentUICulture
     tpPostTime.SelectedDate = publishDate

     ' Summary, Image, Categories, Tags
     Try
      ddLocale.Items.FindByValue(Post.Locale).Selected = True
     Catch ex As Exception
     End Try
     If Not String.IsNullOrEmpty(Post.Image) Then
      imgPostImage.ImageUrl = ResolveUrl(glbImageHandlerPath) & String.Format("?TabId={0}&ModuleId={1}&Blog={2}&Post={3}&w=100&h=100&c=1&key={4}", TabId, Settings.ModuleId, BlogId, ContentItemId, Post.Image)
      imgPostImage.Visible = True
      cmdImageRemove.Visible = True
     Else
      imgPostImage.Visible = False
      cmdImageRemove.Visible = False
     End If
     ctlTags.Terms = Post.PostTags
     ctlCategories.SelectedCategories = Post.PostCategories

    Else

     ddLocale.Items.FindByValue("").Selected = True
     ' Publishing
     chkAllowComments.Checked = Blog.Permissions.ContainsPermissionKey("ADDCOMMENT")
     ' Date
     litTimezone.Text = UiTimeZone.DisplayName
     Dim publishDate As Date = UtcToLocalTime(DateTime.Now.ToUniversalTime, UiTimeZone)
     dpPostDate.Culture = Threading.Thread.CurrentThread.CurrentUICulture
     dpPostDate.SelectedDate = publishDate
     tpPostTime.Culture = Threading.Thread.CurrentThread.CurrentUICulture
     tpPostTime.SelectedDate = publishDate

    End If

    ' Security
    If Blog.MustApproveGhostPosts AndAlso Not Security.CanApprovePost Then
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

    If Post Is Nothing Then Post = New PostInfo
    If ContentItemId = -1 And Not Security.CanAddPost Then Throw New Exception("You can't add posts to this blog")
    If ContentItemId <> -1 And Not Security.CanEditPost Then Throw New Exception("You're not allowed to edit this post")

    Dim firstPublish As Boolean = CBool((Not Post.Published) And chkPublished.Checked)

    ' Contents and summary
    Post.BlogID = BlogId
    Post.Title = txtTitle.DefaultText
    Post.TitleLocalizations = txtTitle.GetLocalizedTexts
    Dim PostBody As New PostBodyAndSummary(teBlogPost, txtDescription, Settings.SummaryModel, True)
    PostBody.WriteToPost(Post, Settings.SummaryModel, False, True)

    ' Publishing
    Post.Published = chkPublished.Checked
    Post.AllowComments = chkAllowComments.Checked
    Post.DisplayCopyright = chkDisplayCopyright.Checked
    Post.Copyright = txtCopyright.Text
    ' Date
    Post.PublishedOnDate = CDate(dpPostDate.SelectedDate)
    Dim hour As Integer = tpPostTime.SelectedDate.Value.Hour
    Dim minute As Integer = tpPostTime.SelectedDate.Value.Minute
    Post.PublishedOnDate = Post.PublishedOnDate.AddHours(hour)
    Post.PublishedOnDate = Post.PublishedOnDate.AddMinutes(minute)
    Post.PublishedOnDate = TimeZoneInfo.ConvertTimeToUtc(Post.PublishedOnDate, UiTimeZone)

    ' Categories, Tags
    Dim terms As New List(Of TermInfo)
    ctlTags.CreateMissingTerms()
    terms.AddRange(ctlTags.Terms)
    terms.AddRange(ctlCategories.SelectedCategories)
    Post.Terms.Clear()
    Post.Terms.AddRange(terms)

    If IsMultiLingualSite And Not Blog.FullLocalization Then
     Post.Locale = ddLocale.SelectedValue
    Else
     Post.Locale = ""
    End If

    ' Image
    Dim saveDir As String = GetPostDirectoryMapPath(Post)
    Dim savedFile As String = ""
    If fileImage.HasFile Then
     Dim extension As String = IO.Path.GetExtension(fileImage.FileName).ToLower
     If glbPermittedFileExtensions.IndexOf(extension & ",") > -1 Then
      If Not IO.Directory.Exists(saveDir) Then IO.Directory.CreateDirectory(saveDir)
      If Post.Image <> "" Then
       ' remove old images
       Dim imagesToDelete As New List(Of String)
       Dim d As New IO.DirectoryInfo(saveDir)
       For Each f As IO.FileInfo In d.GetFiles
        If f.Name.StartsWith(Post.Image) Then
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
      Post.Image = newFileName
     End If
    End If

    ' Final Security Check
    If Blog.MustApproveGhostPosts AndAlso Not Security.CanApprovePost Then
     Post.Published = False
     firstPublish = False
    End If

    ' Add if new, otherwise update
    If ContentItemId = -1 Then
     Post.ContentItemId = PostsController.AddPost(Post, UserId)
     ContentItemId = Post.ContentItemId
     If savedFile <> "" Then ' move file if it was saved
      saveDir = GetPostDirectoryMapPath(Post)
      IO.Directory.CreateDirectory(saveDir)
      Dim dest As String = saveDir & Post.Image & IO.Path.GetExtension(fileImage.FileName).ToLower
      IO.File.Move(savedFile, dest)
     End If
    Else
     PostsController.UpdatePost(Post, UserId)
    End If

    If (Post.Published) Then

     Dim journalUrl As String = Post.PermaLink(PortalSettings)
     Dim journalUserId As Integer = UserId
     If Blog.PublishAsOwner Then journalUserId = Blog.OwnerUserId
     JournalController.AddBlogPostToJournal(Post, ModuleContext.PortalId, ModuleContext.TabId, journalUserId, journalUrl)
     NotificationController.RemovePostPendingNotification(Settings.ModuleId, Blog.BlogID, Post.ContentItemId)

    ElseIf Blog.MustApproveGhostPosts And UserId <> Blog.OwnerUserId Then

     Dim title As String = Localization.GetString("ApprovePostNotifyBody", SharedResourceFileName)
     Dim summary As String = "<a target='_blank' href='" + Post.PermaLink(PortalSettings) + "'>" + Post.Title + "</a>"
     NotificationController.PostPendingApproval(Blog, Post, ModuleContext.PortalId, summary, title)

    End If

    Response.Redirect(NavigateURL(TabId, "", "Post=" & ContentItemId.ToString), False)

   End If
  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try

 End Sub

 Protected Sub cmdDelete_Click(sender As Object, e As EventArgs) Handles cmdDelete.Click
  Try
   DeleteAllFiles()
   PostsController.DeletePost(Post.ContentItemId, Post.BlogID, ModuleContext.PortalId, Settings.VocabularyId)
   Response.Redirect(NavigateURL(TabId, "", "Blog=" & BlogId.ToString), False)
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

  If Post IsNot Nothing Then
   If Post.Image <> "" Then
    ' remove old images
    Dim saveDir As String = PortalSettings.HomeDirectoryMapPath & String.Format("\Blog\Files\{0}\{1}\", BlogId, ContentItemId)
    Dim imagesToDelete As New List(Of String)
    Dim d As New IO.DirectoryInfo(saveDir)
    For Each f As IO.FileInfo In d.GetFiles
     If f.Name.StartsWith(Post.Image) Then
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
   Post.Image = ""
   PostsController.UpdatePost(Post, UserId)
  End If
  imgPostImage.Visible = False
  cmdImageRemove.Visible = False

 End Sub
#End Region

#Region " Private Methods "
 Private Function CreateCopyRight() As String
  Return GetString("msgCopyright", LocalResourceFile) & Date.UtcNow.Year & " " & Blog.DisplayName
 End Function
#End Region

#Region " Upload Feature Methods "
 Private Sub DeleteAllFiles()
  Try
   System.IO.Directory.Delete(FileController.getPostDir(Me.FilePath, Post), True)
  Catch

  End Try
 End Sub

#End Region

End Class