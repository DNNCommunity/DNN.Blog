'
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

Imports System.Globalization
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Localization.Localization
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Common.Globals
Imports System.Linq
Imports DotNetNuke.Framework.JavaScriptLibraries
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Entities.Posts
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports DotNetNuke.Modules.Blog.Integration
Imports DotNetNuke.Modules.Blog.Entities.Terms
Imports DotNetNuke.Security
Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.Web.Client
Imports DotNetNuke.Web.Client.ClientResourceManagement
Imports DotNetNuke.Web.Client.Providers

Public Class PostEdit
  Inherits BlogModuleBase

  Public ReadOnly Property FilePath As String
    Get
      Return PortalSettings.HomeDirectory & ModuleConfiguration.DesktopModule.FriendlyName & "/"
    End Get
  End Property

  Private m_oTags As ArrayList

  Public ReadOnly Property GeneralShortTimePattern As String
    Get
      Dim f As DateTimeFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat
      Return $"{f.ShortDatePattern} {f.ShortTimePattern}"
    End Get
  End Property

  Public ReadOnly Property FlatPickrFormat As String
    Get
      Dim p As String = GeneralShortTimePattern
      Dim res As String = p.Replace("dd", "d").Replace("MM", "m").Replace("yyyy", "Y").Replace("HH", "H").Replace("mm", "i").Replace("M", "n").Replace("d", "j").Replace("tt", "K")
      Return res
    End Get
  End Property

  Protected Overloads Sub Page_Init(sender As Object, e As EventArgs) Handles MyBase.Init

    Try

      ctlTags.ModuleConfiguration = ModuleConfiguration
      ctlCategories.ModuleConfiguration = ModuleConfiguration
      ctlCategories.VocabularyId = Settings.VocabularyId

      If BlogContext.Blog Is Nothing Then
        Dim blogList As IEnumerable(Of BlogInfo) = Nothing
        blogList = BlogsController.GetBlogsByModule(Settings.ModuleId, UserId, BlogContext.Locale).Values.Where(Function(b)
                                                                                                                  Return b.OwnerUserId = UserId
                                                                                                                End Function)
        If blogList.Count > 0 Then
          Response.Redirect(EditUrl("Blog", blogList(0).BlogID.ToString, "PostEdit"), False)
        Else
          blogList = BlogsController.GetBlogsByModule(Settings.ModuleId, UserId, BlogContext.Locale).Values.Where(Function(b)
                                                                                                                    Return (b.CanAdd And BlogContext.Security.CanAddPost) Or (b.CanEdit And BlogContext.Security.CanEditPost And BlogContext.ContentItemId > -1)
                                                                                                                  End Function)
          If blogList.Count > 0 Then
            Response.Redirect(EditUrl("Blog", blogList(0).BlogID.ToString, "PostEdit"), False)
          Else
            Throw New Exception("Could not find a blog for you to post to")
          End If
        End If
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

  Protected Sub Page_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    Try

      If Not BlogContext.Security.CanEditPost And Not (BlogContext.Security.CanAddPost And BlogContext.Post Is Nothing) And Not BlogContext.Security.CanEditThisPost(BlogContext.Post) Then
        Throw New Exception("You do not have access to this resource. Please check your login status.")
      End If

      If Not Page.IsPostBack Then

        ClientResourceManager.RegisterScript(Me.Page, "~/DesktopModules/Blog/Js/flatpickr/flatpickr.min.js", FileOrder.Js.DefaultPriority + 1, DnnPageHeaderProvider.DefaultName)
        Dim genericLocale As String = Threading.Thread.CurrentThread.CurrentCulture.Name.Substring(0, 2)
        If genericLocale <> "en" Then
          ClientResourceManager.RegisterScript(Me.Page, $"~/DesktopModules/Blog/Js/flatpickr/l10n/{genericLocale}.js", FileOrder.Js.DefaultPriority + 1, DnnPageHeaderProvider.DefaultName)
        End If
        ClientResourceManager.RegisterStyleSheet(Me.Page, "~/DesktopModules/Blog/Js/flatpickr/flatpickr.min.css", FileOrder.Css.DefaultPriority + 1, DnnPageHeaderProvider.DefaultName)

        ' Categories
        If Settings.VocabularyId < 1 Then
          pnlCategories.Visible = False
        End If
        ' Clear the tags and categories cache
        Entities.Terms.TermsController.GetTermsByVocabulary(ModuleId, Settings.VocabularyId, Threading.Thread.CurrentThread.CurrentCulture.Name, True)
        Entities.Terms.TermsController.GetTermsByVocabulary(ModuleId, 1, Threading.Thread.CurrentThread.CurrentCulture.Name, True)

        If BlogContext.IsMultiLingualSite And Not BlogContext.Blog.FullLocalization Then
          ddLocale.DataSource = DotNetNuke.Services.Localization.LocaleController.Instance.GetLocales(PortalId).Values.OrderBy(Function(t) t.NativeName)
          ddLocale.DataValueField = "Code"
          ddLocale.DataBind()
          ddLocale.Items.Insert(0, New ListItem(LocalizeString("DefaultLocale"), ""))
          rowLocale.Visible = True
          Try
            ddLocale.Items.FindByValue(BlogContext.Post.Locale).Selected = True
          Catch ex As Exception
          End Try
        Else
          rowLocale.Visible = False
        End If

        ' Buttons
        If BlogContext.BlogId > -1 Then
          hlCancel.NavigateUrl = NavigateURL(TabId, "", "Blog=" & BlogContext.BlogId.ToString)
        ElseIf BlogContext.ContentItemId > -1 Then
          hlCancel.NavigateUrl = NavigateURL(TabId, "", "Post=" & BlogContext.ContentItemId.ToString)
        Else
          hlCancel.NavigateUrl = ModuleContext.NavigateUrl(ModuleContext.TabId, "", False, "")
        End If
        cmdDelete.Visible = CBool(BlogContext.Post IsNot Nothing)

        If BlogContext.Post Is Nothing Then
          BlogContext.Post = New PostInfo
          BlogContext.Post.Blog = BlogContext.Blog
          BlogContext.Post.BlogID = BlogContext.Blog.BlogID
          BlogContext.Post.Published = True
          chkPublishNow.Checked = True
        End If

        If Not BlogContext.Post Is Nothing Then

          Dim PostBody As New PostBodyAndSummary(BlogContext.Post, Settings.SummaryModel, True, Settings.AutoGenerateMissingSummary, Settings.AutoGeneratedSummaryLength)

          ' Content
          txtTitle.DefaultText = HttpUtility.HtmlDecode(BlogContext.Post.Title)
          txtTitle.LocalizedTexts = BlogContext.Post.TitleLocalizations
          txtTitle.InitialBind()
          txtDescription.DefaultText = PostBody.Summary
          txtDescription.LocalizedTexts = PostBody.SummaryLocalizations
          txtDescription.InitialBind()
          teBlogPost.DefaultText = HttpUtility.HtmlEncode(PostBody.Body)
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
          dpPostDate.Text = publishDate.ToString(GeneralShortTimePattern)

          ' Summary, Image, Categories, Tags
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

  Private Sub cmdSave_Click(sender As Object, e As EventArgs) Handles cmdSave.Click

    Try

      If Page.IsValid = True Then

        If BlogContext.Post Is Nothing Then
          BlogContext.Post = New PostInfo
          BlogContext.Post.Blog = BlogContext.Blog
        End If
        If BlogContext.ContentItemId = -1 And Not BlogContext.Security.CanAddPost Then Throw New Exception("You can't add posts to this blog")
        If BlogContext.ContentItemId <> -1 And Not BlogContext.Security.CanEditThisPost(BlogContext.Post) Then Throw New Exception("You're not allowed to edit this post")

        Dim firstPublish As Boolean = CBool((Not BlogContext.Post.Published) And chkPublished.Checked)

        ' Contents and summary
        BlogContext.Post.BlogID = BlogContext.BlogId
        BlogContext.Post.Title = txtTitle.DefaultText
        BlogContext.Post.TitleLocalizations = txtTitle.GetLocalizedTexts
        Dim PostBody As New PostBodyAndSummary(teBlogPost, txtDescription, Settings.SummaryModel, True, Settings.AutoGenerateMissingSummary, Settings.AutoGeneratedSummaryLength)
        PostBody.WriteToPost(BlogContext.Post, Settings.SummaryModel, False, True)

        ' Publishing
        BlogContext.Post.Published = chkPublished.Checked
        BlogContext.Post.AllowComments = chkAllowComments.Checked
        BlogContext.Post.DisplayCopyright = chkDisplayCopyright.Checked
        BlogContext.Post.Copyright = txtCopyright.Text
        ' Date
        If chkPublishNow.Checked Then
          BlogContext.Post.PublishedOnDate = Now.ToUniversalTime
        Else
          BlogContext.Post.PublishedOnDate = CDate(PortalSecurity.Instance.InputFilter(dpPostDate.Text.Trim(), PortalSecurity.FilterFlag.NoMarkup))
          BlogContext.Post.PublishedOnDate = TimeZoneInfo.ConvertTimeToUtc(BlogContext.Post.PublishedOnDate, BlogContext.UiTimeZone)
        End If

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
        Dim publishingUserId As Integer = UserId
        If BlogContext.Blog.PublishAsOwner Then publishingUserId = BlogContext.Blog.OwnerUserId
        If BlogContext.ContentItemId = -1 Then
          BlogContext.Post.ContentItemId = PostsController.AddPost(BlogContext.Post, publishingUserId)
          BlogContext.ContentItemId = BlogContext.Post.ContentItemId
          If savedFile <> "" Then ' move file if it was saved
            saveDir = GetPostDirectoryMapPath(BlogContext.Post)
            IO.Directory.CreateDirectory(saveDir)
            Dim dest As String = saveDir & BlogContext.Post.Image & IO.Path.GetExtension(fileImage.FileName).ToLower
            IO.File.Move(savedFile, dest)
          End If
        Else
          PostsController.UpdatePost(BlogContext.Post, publishingUserId)
        End If

        If firstPublish Then

          PostsController.PublishPost(BlogContext.Post, UserInfo.UserID)

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

  Protected Sub chkDisplayCopyright_CheckedChanged(sender As Object, e As EventArgs) Handles chkDisplayCopyright.CheckedChanged
    pnlCopyright.Visible = chkDisplayCopyright.Checked
    If pnlCopyright.Visible Then
      txtCopyright.Text = CreateCopyRight()
    End If
  End Sub

  Private Sub cmdImageRemove_Click(sender As Object, e As EventArgs) Handles cmdImageRemove.Click

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

  Private Function CreateCopyRight() As String
    Return GetString("msgCopyright", LocalResourceFile) & Date.UtcNow.Year & " " & BlogContext.Blog.DisplayName
  End Function

  Private Sub DeleteAllFiles()
    Try
      System.IO.Directory.Delete(FileController.getPostDir(FilePath, BlogContext.Post), True)
    Catch

    End Try
  End Sub

End Class