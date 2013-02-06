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
Imports System.IO
Imports DotNetNuke.Modules.Blog.Business
Imports DotNetNuke.Modules.Blog.Controllers
Imports DotNetNuke.Modules.Blog.Common
Imports DotNetNuke.Web.Client.ClientResourceManagement
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Localization.Localization
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Framework
Imports System.Linq
Imports DotNetNuke.Entities.Content.Taxonomy
Imports DotNetNuke.Modules.Blog.File
Imports DotNetNuke.Modules.Blog.Entities
Imports DotNetNuke.Services.Localization
Imports Telerik.Web.UI

Partial Class EditEntry
 Inherits BlogModuleBase

#Region "Public Properties"

 Public ReadOnly Property FilePath() As String
  Get
   Return Me.PortalSettings.HomeDirectory & Me.ModuleConfiguration.DesktopModule.FriendlyName & "/"
  End Get
 End Property

#End Region

#Region "Private Members"

 Private m_oTags As ArrayList

 Private ReadOnly Property VocabularyId() As Integer
  Get
   Return Settings.VocabularyId
  End Get
 End Property

#End Region

#Region "Controls"

 Protected WithEvents txtDescription As DotNetNuke.UI.UserControls.TextEditor
 Protected WithEvents teBlogEntry As DotNetNuke.UI.UserControls.TextEditor

#End Region

#Region "Event Handlers"

 Protected Overloads Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
  Try
   jQuery.RequestDnnPluginsRegistration()
   ClientResourceManager.RegisterScript(Page, TemplateSourceDirectory + "/js/jquery.tagify.js")
   ClientResourceManager.RegisterScript(Page, TemplateSourceDirectory + "/js/jquery.qaplaceholder.js")

   If Blog Is Nothing Then
    Response.Redirect(Request.UrlReferrer.ToString, True)
   ElseIf Not Entry Is Nothing Then
    Me.ModuleConfiguration.ModuleTitle = GetString("msgEditBlogEntry", LocalResourceFile)
   Else
    Me.ModuleConfiguration.ModuleTitle = GetString("msgAddBlogEntry", LocalResourceFile)
   End If

   If Not Security.CanAddEntry Then
    Response.Redirect(NavigateURL("Access Denied"))
   End If

  Catch
  End Try
 End Sub

 Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  Try
   valDescription.Enabled = Settings.EntryDescriptionRequired
   Me.pnlUploads.Visible = Settings.EnableUploadOption

   'Localize the file attachments datagrid
   LocalizeDataGrid(dgLinkedFiles, LocalResourceFile)

   If Not Page.IsPostBack Then
    PopulateCategories()

    If Settings.AllowSummaryHtml Then
     txtDescription.Visible = True
     txtDescriptionText.Visible = False
    Else
     txtDescription.Visible = False
     txtDescriptionText.Visible = True
    End If

    cmdPublish.Text = GetString("SaveAndPublish", LocalResourceFile)
    cmdDraft.Text = GetString("SaveAsDraft", LocalResourceFile)

    pnlComments.Visible = ((Settings.CommentMode = Constants.CommentMode.Default) AndAlso Blog.AllowComments)

    If Not Entry Is Nothing Then
     litTimezone.Text = UiTimeZone.DisplayName

     Dim n As DateTime = Utility.AdjustedDate(Entry.AddedDate, UiTimeZone)
     Dim publishDate As DateTime = n
     Dim timeOffset As TimeSpan = UiTimeZone.BaseUtcOffset

     publishDate = publishDate.Add(timeOffset)

     dpEntryDate.Culture = Threading.Thread.CurrentThread.CurrentUICulture
     dpEntryDate.SelectedDate = publishDate
     tpEntryTime.Culture = Threading.Thread.CurrentThread.CurrentUICulture
     tpEntryTime.SelectedDate = publishDate

     txtTitle.Text = HttpUtility.HtmlDecode(Entry.Title)
     If Settings.AllowSummaryHtml Then
      txtDescription.Text = Entry.Description
     Else
      txtDescriptionText.Text = Entry.Description
     End If
     teBlogEntry.Text = Server.HtmlDecode(Entry.Entry)
     'DR-04/16/2009-BLG-9657
     'lblPublished.Visible = Not m_oEntry.Published
     chkAllowComments.Checked = Entry.AllowComments
     chkDisplayCopyright.Checked = Entry.DisplayCopyright
     If chkDisplayCopyright.Checked Then
      pnlCopyright.Visible = True
      txtCopyright.Text = Entry.Copyright
     End If
     'DR-05/28/2009-BLG-9556
     'txtEntryDate.Enabled = (Not m_oEntry.Published)
     cmdDelete.Visible = True
     Me.dgLinkedFiles.DataSource = FileController.getFileList(Me.FilePath, Entry)
     Me.dgLinkedFiles.DataBind()

     For Each t As Term In Entry.Terms
      If t.VocabularyId = 1 Then
       txtTags.Text = txtTags.Text + t.Name + ","
      Else
       Dim objNode As RadTreeNode = dtCategories.FindNodeByValue(t.TermId.ToString())
       If objNode IsNot Nothing Then
        objNode.Checked = True
        objNode.ExpandParentNodes()
       End If
      End If
     Next

     ' set UI based on published status
     If Entry.Published Then
      cmdDraft.Text = GetString("SaveAndOffline", LocalResourceFile)
      cmdPublish.Text = GetString("Save", LocalResourceFile)
      'lblPublished.Text = GetString("Published.Status", LocalResourceFile)
     End If
    Else
     ' New Entry
     litTimezone.Text = UiTimeZone.DisplayName

     'DR-04/16/2009-BLG-9657
     chkAllowComments.Checked = Blog.AllowComments

     litTimezone.Text = UiTimeZone.DisplayName
     Dim n As Date = Utility.AdjustedDate(DateTime.Now, UiTimeZone)
     dpEntryDate.Culture = Threading.Thread.CurrentThread.CurrentUICulture
     dpEntryDate.SelectedDate = n.Date
     tpEntryTime.Culture = Threading.Thread.CurrentThread.CurrentUICulture
     tpEntryTime.SelectedDate = Utility.AdjustedDate(DateTime.Now, UiTimeZone)
    End If

    If Not Request.UrlReferrer Is Nothing Then
     ViewState("URLReferrer") = Request.UrlReferrer.ToString
    End If

    Dim cancelUrl As String = ModuleContext.NavigateUrl(ModuleContext.TabId, "", False, "")
    If Not Request.QueryString("BlogId") Is Nothing Then
     cancelUrl = Utility.AddTOQueryString(NavigateURL(), "BlogID", Request.QueryString("BlogId").ToString())
    ElseIf Not Request.QueryString("EntryId") Is Nothing Then
     cancelUrl = Utility.AddTOQueryString(NavigateURL(), "EntryId", Request.QueryString("EntryId").ToString())
    End If

    hlCancel.NavigateUrl = cancelUrl
   End If
  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 Protected Sub cmdPublish_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdPublish.Click
  updateEntry(True)
 End Sub

 Protected Sub cmdDraft_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDraft.Click
  updateEntry(False)
 End Sub

 Protected Sub cmdDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDelete.Click
  Try
   If Not Entry Is Nothing Then
    DeleteAllFiles()
    EntryController.DeleteEntry(Entry.EntryID, Entry.ContentItemId, Entry.BlogID, ModuleContext.PortalId, VocabularyId)
    Response.Redirect(Utility.AddTOQueryString(NavigateURL(), "BlogID", Entry.BlogID.ToString()), True)
   Else
    Response.Redirect(NavigateURL(), True)
   End If
  Catch exc As Exception    'Module failed to load
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 Protected Sub valEntry_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles valEntry.ServerValidate
  args.IsValid = teBlogEntry.Text.Length > 0
 End Sub

 Protected Sub chkDisplayCopyright_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkDisplayCopyright.CheckedChanged
  pnlCopyright.Visible = chkDisplayCopyright.Checked
  If pnlCopyright.Visible Then
   txtCopyright.Text = CreateCopyRight()
  End If
 End Sub

 Protected Sub dgLinkedFiles_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgLinkedFiles.ItemDataBound
  Dim lnkDeleteFile As ImageButton
  If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.EditItem Or e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.SelectedItem Then
   lnkDeleteFile = CType(e.Item.FindControl("lnkDeleteFile"), System.Web.UI.WebControls.ImageButton)
   If Not lnkDeleteFile Is Nothing Then
    If dgLinkedFiles.EditItemIndex = -1 Then
     lnkDeleteFile.Attributes.Add("onclick", "return confirm('" & String.Format(GetString("msgEnsureDeleteFile", Me.LocalResourceFile), lnkDeleteFile.CommandName) & "');")
     lnkDeleteFile.ImageUrl = Me.ControlPath & "Images/delete_file.gif"
    End If
   End If
  End If
 End Sub

 Protected Sub lnkDeleteFile_Command(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs)
  DeleteFile(e.CommandName)
 End Sub

 Protected Sub btnUploadPicture_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUploadPicture.Click
  Dim bResponse As Boolean = False
  Dim strImage As String
  If Entry Is Nothing Then
   'DW - 06/06/08 - Doesn't seem to be needed.
   'txtDescription.Text = txtDescription.Text & "UPLOADTEMPLATE"
   teBlogEntry.Text = teBlogEntry.Text & "UPLOADTEMPLATE"
   Me.valDescription.IsValid = True
   Me.valEntry.IsValid = True
   updateEntry(False)
   'DW - 06/06/08 - Doesn't seem to be needed.
   'txtDescription.Text = txtDescription.Text.Replace("UPLOADTEMPLATE", String.Empty)
   bResponse = True
  End If
  If Me.picFilename.PostedFile.FileName <> "" Then
   Dim maxImageWidth As Integer
   maxImageWidth = Settings.MaxImageWidth
   strImage = uploadImage(Me.FilePath, Entry, 0)
   If strImage <> "" Then
    Dim pHeight As Integer
    Dim pWidth As Integer
    Dim fullImage As System.Drawing.Image
    fullImage = System.Drawing.Image.FromFile(strImage)
    If fullImage.Width > maxImageWidth Then
     pWidth = maxImageWidth
     pHeight = CType((fullImage.Height / (fullImage.Width / maxImageWidth)), Integer)
    Else
     pWidth = fullImage.Width
     pHeight = fullImage.Height
    End If
    strImage = FileController.getVirtualFileName(FilePath, strImage)
    teBlogEntry.Text = teBlogEntry.Text & "<img src='" & strImage & " 'Alt='" & Me.txtAltText.Text & "' Width='" & pWidth.ToString & "' Height='" & pHeight.ToString & "' >"
   End If
  End If
  If bResponse Then
   teBlogEntry.Text = teBlogEntry.Text.Replace("UPLOADTEMPLATE", "")
   updateEntry(False)

   If Not Entry Is Nothing Then
    Response.Redirect(EditUrl("EntryID", Entry.EntryID.ToString(), "Edit_Entry"))
   End If

  Else
   Me.dgLinkedFiles.DataSource = FileController.getFileList(Me.FilePath, Entry)
   Me.dgLinkedFiles.DataBind()
  End If
 End Sub

 Protected Sub btnUploadAttachment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUploadAttachment.Click
  Dim bResponse As Boolean = False
  Dim strAttachment As String
  Dim strDescription As String
  If Entry Is Nothing Then
   teBlogEntry.Text = teBlogEntry.Text & "UPLOADTEMPLATE"
   Me.valDescription.IsValid = True
   Me.valEntry.IsValid = True
   updateEntry(False)
   bResponse = True
  End If
  If Me.attFilename.PostedFile.FileName <> "" Then
   If Me.txtAttachmentDescription.Text = "" Then
    strDescription = System.IO.Path.GetFileName(attFilename.PostedFile.FileName)
   Else
    strDescription = Me.txtAttachmentDescription.Text
   End If
   ' content type will not work with FTB / may be in a later version
   strAttachment = uploadFile(Me.FilePath, Entry, 1)
   If strAttachment <> "" Then
    strAttachment = FileController.getVirtualFileName(FilePath, strAttachment)
    teBlogEntry.Text = teBlogEntry.Text & "<a href='" & strAttachment & "'>" & strDescription & "</a>"
   End If
  End If
  If bResponse Then
   teBlogEntry.Text = teBlogEntry.Text.Replace("UPLOADTEMPLATE", "")
   updateEntry(False)
   If Not Entry Is Nothing Then
    Response.Redirect(EditUrl("EntryID", Entry.EntryID.ToString(), "Edit_Entry"))
   End If
  Else
   Me.dgLinkedFiles.DataSource = FileController.getFileList(Me.FilePath, Entry)
   Me.dgLinkedFiles.DataBind()
  End If
 End Sub
#End Region

#Region "Private Methods"

 ''' <summary>
 ''' This method will update a blog entry and fire off any necessary notifications and add journal entries depending on publishing status.
 ''' </summary>
 ''' <param name="publish"></param>
 ''' <remarks></remarks>
 Private Sub updateEntry(ByVal publish As Boolean)
  Try
   If Page.IsValid = True Then
    If Entry Is Nothing Then
     Entry = New EntryInfo
     Entry = CType(CBO.InitializeObject(Entry, GetType(EntryInfo)), EntryInfo)
     Entry.CreatedUserId = ModuleContext.PortalSettings.UserId
    End If

    Dim firstPublish As Boolean = CBool((Not Entry.Published) And publish)

    With Entry
     .BlogID = Blog.BlogID
     .Title = txtTitle.Text

     Dim descriptionText As String = ""
     If Settings.AllowSummaryHtml Then
      descriptionText = Trim(txtDescription.Text)
     Else
      descriptionText = (New DotNetNuke.Security.PortalSecurity).InputFilter(Trim(txtDescriptionText.Text), DotNetNuke.Security.PortalSecurity.FilterFlag.NoMarkup)
     End If

     If (descriptionText.Length = 0) OrElse (descriptionText = "&lt;p&gt;&amp;#160;&lt;/p&gt;") Then
      .Description = Nothing
     Else
      .Description = descriptionText
     End If

     .Entry = teBlogEntry.Text
     .Published = publish
     .AllowComments = chkAllowComments.Checked
     .DisplayCopyright = chkDisplayCopyright.Checked
     .Copyright = txtCopyright.Text
     .ModuleID = ModuleContext.ModuleId

     .AddedDate = CDate(dpEntryDate.SelectedDate)
     Dim hour As Integer = tpEntryTime.SelectedDate.Value.Hour
     Dim minute As Integer = tpEntryTime.SelectedDate.Value.Minute
     .AddedDate = .AddedDate.AddHours(hour)
     .AddedDate = .AddedDate.AddMinutes(minute)
     .AddedDate = TimeZoneInfo.ConvertTimeToUtc(.AddedDate, UiTimeZone)

     If Null.IsNull(Entry.EntryID) Then
      .EntryID = EntryController.AddEntry(Entry, ModuleContext.TabId).EntryID
     End If
     .PermaLink = Utility.GenerateEntryLink(PortalId, .EntryID, Me.TabId, .Title)

     Dim userEnteredTerms As Array = txtTags.Text.Trim.Split(","c)
     Dim terms As New List(Of Term)

     For Each s As String In userEnteredTerms
      If s.Length > 0 Then
       Dim newTerm As Term = Integration.Terms.CreateAndReturnTerm(s, 1)
       terms.Add(newTerm)
      End If
     Next

     If VocabularyId > 0 Then
      For Each t As Telerik.Web.UI.RadTreeNode In dtCategories.CheckedNodes
       Dim objTerm As Term = Integration.Terms.GetTermById(Convert.ToInt32(t.Value), VocabularyId)
       terms.Add(objTerm)
      Next
     End If

     .Terms.Clear()
     .Terms.AddRange(terms)

     EntryController.UpdateEntry(Entry, Me.TabId, PortalId, VocabularyId)

     If (publish) Then
      Dim cntIntegration As New Integration.Journal()
      Dim journalUserId As Integer
      Dim journalUrl As String = Utility.AddTOQueryString(NavigateURL(), "EntryId", Entry.EntryID.ToString())

      Select Case Blog.AuthorMode
       Case Constants.AuthorMode.GhostMode
        journalUserId = Blog.UserID
       Case Else
        journalUserId = ModuleContext.PortalSettings.UserId
      End Select

      cntIntegration.AddBlogEntryToJournal(Entry, ModuleContext.PortalId, ModuleContext.TabId, journalUserId, journalUrl)

      Dim cntNotifications As New Integration.Notifications
      cntNotifications.RemoveEntryPendingNotification(Blog.BlogID, Entry.EntryID)
     Else
      If (Blog.UserID <> ModuleContext.PortalSettings.UserId) AndAlso (Blog.AuthorMode = Constants.AuthorMode.GhostMode) Then
       Dim cntNotifications As New Integration.Notifications
       Dim title As String = Localization.GetString("ApprovePostNotifyBody", Constants.SharedResourceFileName)
       Dim summary As String = "<a target='_blank' href='" + Entry.PermaLink + "'>" + Entry.Title + "</a>"

       cntNotifications.EntryPendingApproval(Blog, Entry, ModuleContext.PortalId, summary, title)
      End If
     End If

     Response.Redirect(Entry.PermaLink, False)
    End With
   End If
  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 Private Function CreateCopyRight() As String
  Return GetString("msgCopyright", LocalResourceFile) & Date.UtcNow.Year & " " & Blog.UserFullName
 End Function

 Private Sub PopulateCategories()
  If VocabularyId > 0 Then
   Dim termController As ITermController = DotNetNuke.Entities.Content.Common.Util.GetTermController()
   Dim colCategories As IQueryable(Of Term) = termController.GetTermsByVocabulary(VocabularyId)

   dtCategories.DataSource = colCategories
   dtCategories.DataBind()
  Else
   pnlCategories.Visible = False
  End If
 End Sub

#End Region

#Region "Upload Feature Methods"

 Private Function uploadImage(ByVal pFilePath As String, ByVal pEntry As EntryInfo, ByVal FileType As Integer) As String
  uploadImage = UploadFiles(Me.PortalId, FileController.getEntryDir(pFilePath, pEntry), Me.picFilename.PostedFile, FileType)
 End Function

 Private Function uploadFile(ByVal pFilePath As String, ByVal pEntry As EntryInfo, ByVal FileType As Integer) As String
  uploadFile = UploadFiles(Me.PortalId, FileController.getEntryDir(pFilePath, pEntry), Me.attFilename.PostedFile, FileType)
 End Function

 Private Function UploadFiles(ByVal PortalId As Integer, ByVal EntryPath As String, ByVal objFile As HttpPostedFile, ByVal FileType As Integer) As String
  Dim objPortalController As New PortalController
  Dim strMessage As String = ""
  Dim strFileName As String = ""
  Dim strExtension As String = ""

  If objFile.FileName <> "" Then
   strFileName = EntryPath & Path.GetFileName(objFile.FileName)
   strExtension = Replace(Path.GetExtension(strFileName), ".", "")
  End If

  ' FileType 0 == Picture
  If FileType = 0 Then
   If strExtension <> "" Then
    If strExtension.ToLower() <> "jpg" And strExtension.ToLower() <> "gif" And strExtension.ToLower() <> "png" Then
     Me.valUpload.ErrorMessage = GetString("valUpload.ErrorMessage", LocalResourceFile)
     Me.valUpload.ErrorMessage = Me.valUpload.ErrorMessage.Replace("[FILENAME]", objFile.FileName)
     Me.valUpload.IsValid = False
     'The File [FILENAME] Is A Restricted File Type for Images. Valid File Types Include JPG, GIF and PNG<br>
     Return ""
    End If
   End If
  End If

  If ((((objPortalController.GetPortalSpaceUsedBytes(PortalId) + objFile.ContentLength) / 1000000) <= Me.PortalSettings.HostSpace) Or Me.PortalSettings.HostSpace = 0) Or (Me.PortalSettings.ActiveTab.ParentId = Me.PortalSettings.SuperTabId) Then

   If DotNetNuke.Entities.Host.Host.AllowedExtensionWhitelist.IsAllowedExtension(strExtension) Or Me.PortalSettings.ActiveTab.ParentId = Me.PortalSettings.SuperTabId Then
    Try
     If strFileName <> "" Then
      If System.IO.File.Exists(strFileName) Then
       System.IO.File.SetAttributes(strFileName, FileAttributes.Normal)
       System.IO.File.Delete(strFileName)
      End If
      ' DW - 04/16/2008 - Check to make sure the directory exists.
      FileController.createFileDirectory(strFileName)
      objFile.SaveAs(strFileName)
      strMessage = strFileName
      Me.valUpload.IsValid = True
     End If

    Catch ex As Exception
     ProcessModuleLoadException(String.Format(GetString("SaveFileError"), strFileName), Me, ex, True)
    End Try
   Else
    Me.valUpload.ErrorMessage = String.Format(GetString("RestrictedFileType"), strFileName, DotNetNuke.Entities.Host.Host.AllowedExtensionWhitelist.ToDisplayString)
    Me.valUpload.IsValid = False
   End If

  Else
   Me.valUpload.ErrorMessage = String.Format(GetString("DiskSpaceExceeded"), strFileName)
   Me.valUpload.IsValid = False
  End If
  Return strMessage
 End Function

 Private Sub DeleteFile(ByVal fileName As String)
  Try
   Dim _filePath As String = FileController.getEntryDir(Me.FilePath, Entry)
   Dim _fullName As String = _filePath & fileName
   System.IO.File.Delete(_fullName)
   Me.dgLinkedFiles.DataSource = FileController.getFileList(Me.FilePath, Entry)
   Me.dgLinkedFiles.DataBind()
  Catch

  End Try
 End Sub

 Private Sub DeleteAllFiles()
  Try
   System.IO.Directory.Delete(FileController.getEntryDir(Me.FilePath, Entry), True)
  Catch

  End Try
 End Sub

#End Region

End Class