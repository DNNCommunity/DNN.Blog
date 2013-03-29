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
Imports DotNetNuke.Modules.Blog.Entities.Entries
Imports DotNetNuke.Services.Localization
Imports Telerik.Web.UI
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports DotNetNuke.Modules.Blog.Integration
Imports DotNetNuke.Modules.Blog.Entities.Terms

Public Class EntryEdit
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

#Region " Controls "

 Protected WithEvents txtDescription As DotNetNuke.UI.UserControls.TextEditor
 Protected WithEvents teBlogEntry As DotNetNuke.UI.UserControls.TextEditor

#End Region

#Region " Event Handlers "
 Protected Overloads Sub Page_Init(sender As System.Object, e As System.EventArgs) Handles MyBase.Init

  Try

   ctlTags.ModuleConfiguration = Me.ModuleConfiguration
   ctlCategories.ModuleConfiguration = Me.ModuleConfiguration
   ctlCategories.VocabularyId = Settings.VocabularyId

   If Not Me.IsPostBack Then
    Dim blogList As IEnumerable(Of BlogInfo) = Nothing
    If Security.IsEditor Then
     blogList = BlogsController.GetBlogsByModule(Settings.ModuleId, UserId).Values
    Else
     blogList = BlogsController.GetBlogsByModule(Settings.ModuleId, UserId).Values.Where(Function(b)
                                                                                          Return b.OwnerUserId = UserId Or (b.CanAdd And Security.CanAddEntry) Or (b.CanEdit And Security.CanEditEntry And ContentItemId > -1)
                                                                                         End Function)
    End If
    ddBlog.DataSource = blogList
    ddBlog.DataBind()
    If BlogId > -1 Then
     ddBlog.Items.FindByValue(BlogId.ToString).Selected = True
    Else
     Dim userBlog As BlogInfo = blogList.First(Function(b)
                                                Return b.OwnerUserId = UserId
                                               End Function)
     If userBlog IsNot Nothing Then
      ddBlog.Items.FindByValue(userBlog.BlogID.ToString).Selected = True
      Blog = userBlog
     Else
      Blog = BlogsController.GetBlog(ddBlog.Items(0).Value.ToInt, UserId)
     End If
     Security = New Modules.Blog.Security.ContextSecurity(Settings.ModuleId, TabId, Blog, UserInfo)
    End If
    If ContentItemId > -1 Then
     ddBlog.Enabled = False ' we're not going to allow someone to change the blog to which the post belongs - at least, not yet
    End If
   End If

   If Not Security.CanAddEntry Then
    Response.Redirect(NavigateURL("Access Denied"))
   End If

  Catch
  End Try

 End Sub

 Protected Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

  Try

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

    ' Summary
    Select Case Settings.SummaryModel
     Case SummaryType.HtmlIndependent
      txtDescription.Visible = True
      txtDescriptionText.Visible = False
     Case SummaryType.HtmlPrecedesPost
      txtDescription.Visible = True
      txtDescriptionText.Visible = False
      lblSummaryPrecedingWarning.Visible = True
     Case Else ' plain text
      txtDescription.Visible = False
      txtDescriptionText.Visible = True
    End Select

    ' Buttons
    If BlogId > -1 Then
     hlCancel.NavigateUrl = NavigateURL(TabId, "", "Blog=" & BlogId.ToString)
    ElseIf ContentItemId > -1 Then
     hlCancel.NavigateUrl = NavigateURL(TabId, "", "Post=" & ContentItemId.ToString)
    Else
     hlCancel.NavigateUrl = ModuleContext.NavigateUrl(ModuleContext.TabId, "", False, "")
    End If
    cmdDelete.Visible = CBool(Entry IsNot Nothing)

    If Not Entry Is Nothing Then

     Dim entryBody As New PostBodyAndSummary(Entry, Settings.SummaryModel)

     ' Content
     txtTitle.Text = HttpUtility.HtmlDecode(Entry.Title)
     teBlogEntry.Text = entryBody.Body

     ' Publishing
     chkPublished.Checked = Entry.Published
     chkAllowComments.Checked = Entry.AllowComments
     chkDisplayCopyright.Checked = Entry.DisplayCopyright
     If chkDisplayCopyright.Checked Then
      pnlCopyright.Visible = True
      txtCopyright.Text = Entry.Copyright
     End If
     ' Date
     litTimezone.Text = UiTimeZone.DisplayName
     Dim publishDate As DateTime = UtcToLocalTime(Entry.PublishedOnDate, UiTimeZone)
     dpEntryDate.Culture = Threading.Thread.CurrentThread.CurrentUICulture
     dpEntryDate.SelectedDate = publishDate
     tpEntryTime.Culture = Threading.Thread.CurrentThread.CurrentUICulture
     tpEntryTime.SelectedDate = publishDate

     ' Summary, Image, Categories, Tags
     Try
      ddLocale.Items.FindByValue(Entry.Locale).Selected = True
     Catch ex As Exception
     End Try
     Select Case Settings.SummaryModel
      Case SummaryType.PlainTextIndependent
       txtDescriptionText.Text = entryBody.Summary
      Case Else ' plain text
       txtDescription.Text = entryBody.Summary
     End Select
     If Not String.IsNullOrEmpty(Entry.Image) Then
      imgEntryImage.ImageUrl = ResolveUrl(glbImageHandlerPath) & String.Format("?TabId={0}&ModuleId={1}&Blog={2}&Post={3}&w=100&h=100&c=1&key={4}", TabId, Settings.ModuleId, BlogId, ContentItemId, Entry.Image)
      imgEntryImage.Visible = True
      cmdImageRemove.Visible = True
     Else
      imgEntryImage.Visible = False
      cmdImageRemove.Visible = False
     End If
     ctlTags.Terms = Entry.EntryTags
     ctlCategories.SelectedCategories = Entry.EntryCategories

    Else

     Try
      ddLocale.Items.FindByValue(Blog.Locale).Selected = True
     Catch ex As Exception
     End Try
     ' Publishing
     chkAllowComments.Checked = Blog.AllowComments
     ' Date
     litTimezone.Text = UiTimeZone.DisplayName
     Dim publishDate As Date = UtcToLocalTime(DateTime.Now.ToUniversalTime, UiTimeZone)
     dpEntryDate.Culture = Threading.Thread.CurrentThread.CurrentUICulture
     dpEntryDate.SelectedDate = publishDate
     tpEntryTime.Culture = Threading.Thread.CurrentThread.CurrentUICulture
     tpEntryTime.SelectedDate = publishDate

    End If

    ' Security
    If Blog.MustApproveGhostPosts AndAlso Not Security.CanApproveEntry Then
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

    If Entry Is Nothing Then Entry = New EntryInfo
    If BlogId = -1 OrElse Blog.BlogID <> ddBlog.SelectedValue.ToInt Then
     BlogId = ddBlog.SelectedValue.ToInt
     Blog = BlogsController.GetBlog(BlogId, UserId)
     Security = New Modules.Blog.Security.ContextSecurity(Settings.ModuleId, TabId, Blog, UserInfo)
    End If
    If ContentItemId = -1 And Not Security.CanAddEntry Then Throw New Exception("You can't add posts to this blog")
    If ContentItemId <> -1 And Not Security.CanEditEntry Then Throw New Exception("You're not allowed to edit this post")

    Dim firstPublish As Boolean = CBool((Not Entry.Published) And chkPublished.Checked)

    ' Contents and summary
    Entry.BlogID = BlogId
    Entry.Title = txtTitle.Text
    Dim entryBody As New PostBodyAndSummary(teBlogEntry.Text)
    If Settings.SummaryModel = SummaryType.PlainTextIndependent Then
     entryBody.Summary = RemoveHtmlTags(Trim(txtDescriptionText.Text))
    Else
     entryBody.Summary = Trim(txtDescription.Text)
    End If
    entryBody.WriteToEntry(Entry, Settings.SummaryModel, False)

    ' Publishing
    Entry.Published = chkPublished.Checked
    Entry.AllowComments = chkAllowComments.Checked
    Entry.DisplayCopyright = chkDisplayCopyright.Checked
    Entry.Copyright = txtCopyright.Text
    ' Date
    Entry.PublishedOnDate = CDate(dpEntryDate.SelectedDate)
    Dim hour As Integer = tpEntryTime.SelectedDate.Value.Hour
    Dim minute As Integer = tpEntryTime.SelectedDate.Value.Minute
    Entry.PublishedOnDate = Entry.PublishedOnDate.AddHours(hour)
    Entry.PublishedOnDate = Entry.PublishedOnDate.AddMinutes(minute)
    Entry.PublishedOnDate = TimeZoneInfo.ConvertTimeToUtc(Entry.PublishedOnDate, UiTimeZone)

    ' Categories, Tags
    Dim terms As New List(Of TermInfo)
    ctlTags.CreateMissingTerms()
    terms.AddRange(ctlTags.Terms)
    terms.AddRange(ctlCategories.SelectedCategories)
    Entry.Terms.Clear()
    Entry.Terms.AddRange(terms)

    Entry.Locale = ddLocale.SelectedValue

    ' Image
    Dim saveDir As String = GetPostDirectoryMapPath(Entry)
    Dim savedFile As String = ""
    If fileImage.HasFile Then
     Dim extension As String = IO.Path.GetExtension(fileImage.FileName).ToLower
     If glbPermittedFileExtensions.IndexOf(extension & ",") > -1 Then
      If Not IO.Directory.Exists(saveDir) Then IO.Directory.CreateDirectory(saveDir)
      If Entry.Image <> "" Then
       ' remove old images
       Dim imagesToDelete As New List(Of String)
       Dim d As New IO.DirectoryInfo(saveDir)
       For Each f As IO.FileInfo In d.GetFiles
        If f.Name.StartsWith(Entry.Image) Then
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
      Entry.Image = newFileName
     End If
    End If

    ' Final Security Check
    If Blog.MustApproveGhostPosts AndAlso Not Security.CanApproveEntry Then
     Entry.Published = False
     firstPublish = False
    End If

    ' Add if new, otherwise update
    If ContentItemId = -1 Then
     Entry.ContentItemId = EntriesController.AddEntry(Entry, UserId)
     ContentItemId = Entry.ContentItemId
     If savedFile <> "" Then ' move file if it was saved
      saveDir = GetPostDirectoryMapPath(Entry)
      IO.Directory.CreateDirectory(saveDir)
      Dim dest As String = saveDir & Entry.Image & IO.Path.GetExtension(fileImage.FileName).ToLower
      IO.File.Move(savedFile, dest)
     End If
    Else
     EntriesController.UpdateEntry(Entry, UserId)
    End If

    If (Entry.Published) Then

     Dim journalUrl As String = Entry.PermaLink(PortalSettings)
     Dim journalUserId As Integer = UserId
     If Blog.PublishAsOwner Then journalUserId = Blog.OwnerUserId
     JournalController.AddBlogEntryToJournal(Entry, ModuleContext.PortalId, ModuleContext.TabId, journalUserId, journalUrl)
     NotificationController.RemoveEntryPendingNotification(Settings.ModuleId, Blog.BlogID, Entry.ContentItemId)

    ElseIf Blog.MustApproveGhostPosts And UserId <> Blog.OwnerUserId Then

     Dim title As String = Localization.GetString("ApprovePostNotifyBody", SharedResourceFileName)
     Dim summary As String = "<a target='_blank' href='" + Entry.PermaLink(PortalSettings) + "'>" + Entry.Title + "</a>"
     NotificationController.EntryPendingApproval(Blog, Entry, ModuleContext.PortalId, summary, title)

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
   EntriesController.DeleteEntry(Entry.ContentItemId, Entry.BlogID, ModuleContext.PortalId, Settings.VocabularyId)
   Response.Redirect(NavigateURL(TabId, "", "Blog=" & BlogId.ToString), False)
  Catch exc As Exception    'Module failed to load
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 Protected Sub valEntry_ServerValidate(source As Object, args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles valEntry.ServerValidate
  args.IsValid = teBlogEntry.Text.Length > 0
 End Sub

 Protected Sub chkDisplayCopyright_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkDisplayCopyright.CheckedChanged
  pnlCopyright.Visible = chkDisplayCopyright.Checked
  If pnlCopyright.Visible Then
   txtCopyright.Text = CreateCopyRight()
  End If
 End Sub

 Private Sub ddBlog_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddBlog.SelectedIndexChanged
  Blog = BlogsController.GetBlog(ddBlog.SelectedItem.Value.ToInt, UserId)
  Security = New Modules.Blog.Security.ContextSecurity(Settings.ModuleId, TabId, Blog, UserInfo)
  If Blog.MustApproveGhostPosts AndAlso Not Security.CanApproveEntry Then
   chkPublished.Checked = False
   chkPublished.Enabled = False
  End If
  If ContentItemId = -1 Then
   chkAllowComments.Checked = Blog.AllowComments
  End If
 End Sub

 Private Sub cmdImageRemove_Click(sender As Object, e As System.EventArgs) Handles cmdImageRemove.Click

  If Entry IsNot Nothing Then
   If Entry.Image <> "" Then
    ' remove old images
    Dim saveDir As String = PortalSettings.HomeDirectoryMapPath & String.Format("\Blog\Files\{0}\{1}\", BlogId, ContentItemId)
    Dim imagesToDelete As New List(Of String)
    Dim d As New IO.DirectoryInfo(saveDir)
    For Each f As IO.FileInfo In d.GetFiles
     If f.Name.StartsWith(Entry.Image) Then
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
   Entry.Image = ""
   EntriesController.UpdateEntry(Entry, UserId)
  End If
  imgEntryImage.Visible = False
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
   System.IO.Directory.Delete(FileController.getEntryDir(Me.FilePath, Entry), True)
  Catch

  End Try
 End Sub

#End Region

End Class