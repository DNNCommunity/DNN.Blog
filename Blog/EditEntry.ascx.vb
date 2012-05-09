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
Imports DotNetNuke.Modules.Blog.Components.Business
Imports DotNetNuke.Modules.Blog.Components.Controllers
Imports DotNetNuke.Modules.Blog.Components.Common
Imports DotNetNuke.Web.Client.ClientResourceManagement
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Localization.Localization
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Services.Journal
Imports DotNetNuke.Framework
Imports System.Linq
Imports DotNetNuke.Entities.Content.Taxonomy
Imports System.Globalization
Imports DotNetNuke.Modules.Blog.Components.File
Imports DotNetNuke.Modules.Blog.Components.Entities
Imports Telerik.Web.UI

Partial Class EditEntry
    Inherits BlogModuleBase

#Region "Public Properties"

    Public ReadOnly Property FilePath() As String
        Get
            Return Me.PortalSettings.HomeDirectory & Me.ModuleConfiguration.FriendlyName & "/"
        End Get
    End Property

#End Region

#Region "Private Members"

    Private m_oEntryController As New EntryController
    Private m_oEntry As EntryInfo
    Private m_oBlogController As New BlogController
    Private m_oParentBlog As BlogInfo
    Private m_oBlog As BlogInfo
    Private m_oTags As ArrayList
    Private m_oCats As List(Of CategoryInfo)
    Private m_oEntryCats As List(Of CategoryInfo)
    Private m_oEntryId As Integer = -1

    Private ReadOnly Property VocabularyId() As Integer
        Get
            Return BlogSettings.VocabularyId
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

            Dim cntBlog As New BlogController
            Dim objSecurity As ModuleSecurity = New ModuleSecurity(ModuleContext.ModuleId, ModuleContext.TabId)

            Globals.ReadValue(Me.Request.Params, "EntryID", m_oEntryId)

            If m_oEntryId > -1 Then
                m_oEntry = m_oEntryController.GetEntry(m_oEntryId, PortalId)
                m_oBlog = cntBlog.GetBlog(m_oEntry.BlogID)
            ElseIf SpecificBlogId > 0 Then
                m_oBlog = m_oBlogController.GetBlog(SpecificBlogId)
            Else
                m_oBlog = cntBlog.GetBlogByUserID(Me.PortalId, Me.UserId)
            End If

            If m_oBlog Is Nothing Then
                Response.Redirect(Request.UrlReferrer.ToString, True)
            ElseIf Not m_oEntry Is Nothing Then
                Me.ModuleConfiguration.ModuleTitle = GetString("msgEditBlogEntry", LocalResourceFile)
            Else
                Me.ModuleConfiguration.ModuleTitle = GetString("msgAddBlogEntry", LocalResourceFile)
            End If

            Dim isOwner As Boolean = (m_oBlog.UserID = ModuleContext.PortalSettings.UserId)

            If (objSecurity.CanAddEntry(isOwner, m_oBlog.EnableGhostWriter) = False) Then
                Response.Redirect(NavigateURL("Access Denied"))
            End If

            If m_oBlog.ParentBlogID > -1 Then
                m_oParentBlog = cntBlog.GetBlog(m_oBlog.ParentBlogID)
            Else
                m_oParentBlog = m_oBlog
            End If
        Catch
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            valDescription.Enabled = BlogSettings.EntryDescriptionRequired
            Me.pnlUploads.Visible = BlogSettings.EnableUploadOption

            'Localize the file attachments datagrid
            LocalizeDataGrid(dgLinkedFiles, LocalResourceFile)

            If Not Page.IsPostBack Then
                PopulateCategories()

                Dim colBlogs As ArrayList = m_oBlogController.ListBlogs(Me.PortalId, m_oParentBlog.BlogID, True)

                cboChildBlogs.DataSource = colBlogs
                cboChildBlogs.DataBind()
                cboChildBlogs.Items.Insert(0, New ListItem(m_oParentBlog.Title, m_oParentBlog.BlogID.ToString()))

                If Not cboChildBlogs.Items.FindByValue(m_oBlog.BlogID.ToString()) Is Nothing Then
                    cboChildBlogs.Items.FindByValue(m_oBlog.BlogID.ToString()).Selected = True
                End If

                pnlChildBlogs.Visible = BlogSettings.AllowChildBlogs AndAlso colBlogs.Count > 0

                ' change in 3.1.23 not display the trackback url field if auto trackback is enabled
                Me.lblTrackbackUrl.Visible = Not m_oParentBlog.AutoTrackback
                Me.txtTrackBackUrl.Visible = Not m_oParentBlog.AutoTrackback
                ' end change in 3.1.23

                If BlogSettings.AllowSummaryHtml Then
                    txtDescription.Visible = True
                    txtDescriptionText.Visible = False
                Else
                    txtDescription.Visible = False
                    txtDescriptionText.Visible = True
                End If

                cmdPublish.Text = GetString("SaveAndPublish", LocalResourceFile)
                cmdDraft.Text = GetString("SaveAsDraft", LocalResourceFile)
                'lblPublished.Text = GetString("UnPublished.Status", LocalResourceFile)

                If Not m_oEntry Is Nothing Then
                    Dim userCulture As CultureInfo = New System.Globalization.CultureInfo(ModuleContext.PortalSettings.UserInfo.Profile.PreferredLocale)

                    litTimezone.Text = ModuleContext.PortalSettings.UserInfo.Profile.PreferredTimeZone.DisplayName

                    Dim n As DateTime = Utility.AdjustedDate(m_oEntry.AddedDate, ModuleContext.PortalSettings.UserInfo.Profile.PreferredTimeZone)
                    Dim publishDate As DateTime = n
                    Dim timeOffset As TimeSpan = ModuleContext.PortalSettings.UserInfo.Profile.PreferredTimeZone.BaseUtcOffset

                    publishDate = publishDate.Add(timeOffset)

                    dpEntryDate.Culture = userCulture
                    dpEntryDate.SelectedDate = publishDate
                    tpEntryTime.Culture = userCulture
                    tpEntryTime.SelectedDate = publishDate

                    txtTitle.Text = HttpUtility.HtmlDecode(m_oEntry.Title)
                    If BlogSettings.AllowSummaryHtml Then
                        txtDescription.Text = m_oEntry.Description
                    Else
                        txtDescriptionText.Text = m_oEntry.Description
                    End If
                    teBlogEntry.Text = Server.HtmlDecode(m_oEntry.Entry)
                    'DR-04/16/2009-BLG-9657
                    'lblPublished.Visible = Not m_oEntry.Published
                    chkAllowComments.Checked = m_oEntry.AllowComments
                    chkDisplayCopyright.Checked = m_oEntry.DisplayCopyright
                    If chkDisplayCopyright.Checked Then
                        pnlCopyright.Visible = True
                        txtCopyright.Text = m_oEntry.Copyright
                    End If
                    'DR-05/28/2009-BLG-9556
                    'txtEntryDate.Enabled = (Not m_oEntry.Published)
                    cmdDelete.Visible = True
                    cboChildBlogs.Enabled = True
                    Me.dgLinkedFiles.DataSource = FileController.getFileList(Me.FilePath, m_oEntry)
                    Me.dgLinkedFiles.DataBind()

                    For Each t As Term In m_oEntry.Terms
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
                    If m_oEntry.Published Then
                        cmdDraft.Text = GetString("SaveAndOffline", LocalResourceFile)
                        cmdPublish.Text = GetString("Save", LocalResourceFile)
                        'lblPublished.Text = GetString("Published.Status", LocalResourceFile)
                    End If
                Else
                    ' New Entry
                    litTimezone.Text = ModuleContext.PortalSettings.UserInfo.Profile.PreferredTimeZone.DisplayName

                    'DR-04/16/2009-BLG-9657
                    chkAllowComments.Checked = m_oBlog.AllowComments

                    Dim userCulture As CultureInfo = New System.Globalization.CultureInfo(ModuleContext.PortalSettings.UserInfo.Profile.PreferredLocale)
                    litTimezone.Text = ModuleContext.PortalSettings.UserInfo.Profile.PreferredTimeZone.DisplayName
                    Dim n As Date = Utility.AdjustedDate(DateTime.Now, ModuleContext.PortalSettings.UserInfo.Profile.PreferredTimeZone)
                    dpEntryDate.Culture = userCulture
                    dpEntryDate.SelectedDate = n.Date
                    tpEntryTime.Culture = userCulture
                    tpEntryTime.SelectedDate = Utility.AdjustedDate(DateTime.Now, ModuleContext.PortalSettings.UserInfo.Profile.PreferredTimeZone)
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
        Response.Redirect(NavigateURL(Me.TabId, "", "BlogID=" & m_oBlog.BlogID.ToString()), True)
    End Sub

    Protected Sub cmdDraft_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDraft.Click
        'DR-04/16/2009-BLG-9657
        updateEntry(False)
        Me.Response.Redirect(EditUrl("EntryID", m_oEntry.EntryID.ToString(), "Edit_Entry"), False)
    End Sub

    Protected Sub cmdDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDelete.Click
        Try
            If Not m_oEntry Is Nothing Then
                DeleteAllFiles()
                m_oEntryController.DeleteEntry(m_oEntry.EntryID, m_oEntry.ContentItemId)
                Response.Redirect(Utility.AddTOQueryString(NavigateURL(), "BlogID", m_oEntry.BlogID.ToString()), True)
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
        If m_oEntry Is Nothing Then
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
            maxImageWidth = BlogSettings.MaxImageWidth
            strImage = uploadImage(Me.FilePath, m_oEntry, 0)
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

            If Not m_oEntry Is Nothing Then
                Response.Redirect(EditUrl("EntryID", m_oEntry.EntryID.ToString(), "Edit_Entry"))
            End If

        Else
            Me.dgLinkedFiles.DataSource = FileController.getFileList(Me.FilePath, m_oEntry)
            Me.dgLinkedFiles.DataBind()
        End If
    End Sub

    Protected Sub btnUploadAttachment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUploadAttachment.Click
        Dim bResponse As Boolean = False
        Dim strAttachment As String
        Dim strDescription As String
        If m_oEntry Is Nothing Then
            'DW - 06/06/08 - Doesn't seem to be needed.
            'txtDescription.Text = txtDescription.Text & "UPLOADTEMPLATE"
            teBlogEntry.Text = teBlogEntry.Text & "UPLOADTEMPLATE"
            Me.valDescription.IsValid = True
            Me.valEntry.IsValid = True
            updateEntry(False)
            'DW - 06/06/08 - Doesn't seem to be needed.
            'txtDescription.Text = txtDescription.Text.Replace("UPLOADTEMPLATE", "")
            bResponse = True
        End If
        If Me.attFilename.PostedFile.FileName <> "" Then
            If Me.txtAttachmentDescription.Text = "" Then
                strDescription = System.IO.Path.GetFileName(attFilename.PostedFile.FileName)
            Else
                strDescription = Me.txtAttachmentDescription.Text
            End If
            ' content type will not work with FTB / may be in a later version
            'strExtension = Me.attFilename.PostedFile.ContentType
            'strExtension = Path.GetExtension(Me.attFilename.PostedFile.FileName).Replace(".", "")
            'strExtension = strExtension.ToLower()
            strAttachment = uploadFile(Me.FilePath, m_oEntry, 1)
            If strAttachment <> "" Then
                '    Dim strContentType As String
                '    Select Case strExtension
                '        Case "txt" : strContentType = "text/plain"
                '        Case "htm", "html" : strContentType = "text/html"
                '        Case "rtf" : strContentType = "text/richtext"
                '        Case "jpg", "jpeg" : strContentType = "image/jpeg"
                '        Case "gif" : strContentType = "image/gif"
                '        Case "bmp" : strContentType = "image/bmp"
                '        Case "mpg", "mpeg" : strContentType = "video/mpeg"
                '        Case "avi" : strContentType = "video/avi"
                '        Case "pdf" : strContentType = "application/pdf"
                '        Case "doc", "dot" : strContentType = "application/msword"
                '        Case "csv", "xls", "xlt" : strContentType = "application/x-msexcel"
                '        Case Else : strContentType = "application/octet-stream"
                '    End Select

                strAttachment = FileController.getVirtualFileName(FilePath, strAttachment)
                teBlogEntry.Text = teBlogEntry.Text & "<a href='" & strAttachment & "'>" & strDescription & "</a>"
            End If

        End If
        If bResponse Then
            teBlogEntry.Text = teBlogEntry.Text.Replace("UPLOADTEMPLATE", "")
            updateEntry(False)

            If Not m_oEntry Is Nothing Then
                Response.Redirect(EditUrl("EntryID", m_oEntry.EntryID.ToString(), "Edit_Entry"))
            End If

        Else
            Me.dgLinkedFiles.DataSource = FileController.getFileList(Me.FilePath, m_oEntry)
            Me.dgLinkedFiles.DataBind()
        End If

    End Sub

#End Region

#Region "Private Methods"

    Private Sub updateEntry(ByVal publish As Boolean)
        Try
            If Page.IsValid = True Then
                If m_oEntry Is Nothing Then
                    m_oEntry = New EntryInfo
                    m_oEntry = CType(CBO.InitializeObject(m_oEntry, GetType(EntryInfo)), EntryInfo)
                End If

                Dim firstPublish As Boolean = CBool((Not m_oEntry.Published) And publish)

                With m_oEntry
                    'bind text values to object
                    .BlogID = m_oBlog.BlogID
                    .Title = txtTitle.Text
                    'DR-04/16/2009-BLG-9658
                    Dim descriptionText As String = ""
                    If BlogSettings.AllowSummaryHtml Then
                        descriptionText = Trim(txtDescription.Text)
                    Else
                        descriptionText = (New DotNetNuke.Security.PortalSecurity).InputFilter(Trim(txtDescriptionText.Text), Security.PortalSecurity.FilterFlag.NoMarkup)
                    End If
                    If (descriptionText.Length = 0) OrElse (descriptionText = "&lt;p&gt;&amp;#160;&lt;/p&gt;") Then
                        .Description = Nothing
                    Else
                        .Description = descriptionText
                    End If

                    .Entry = teBlogEntry.Text
                    'DR-04/16/2009-BLG-9657
                    .Published = publish
                    .AllowComments = chkAllowComments.Checked
                    .DisplayCopyright = chkDisplayCopyright.Checked
                    .Copyright = txtCopyright.Text
                    .ModuleID = ModuleContext.ModuleId

                    If Not cboChildBlogs.SelectedItem Is Nothing Then
                        If CType(cboChildBlogs.SelectedItem.Value, Double) > 0 Then
                            .BlogID = CType(cboChildBlogs.SelectedItem.Value, Integer)
                        End If
                    End If

                    .AddedDate = CDate(dpEntryDate.SelectedDate)
                    Dim hour As Integer = tpEntryTime.SelectedDate.Value.Hour
                    Dim minute As Integer = tpEntryTime.SelectedDate.Value.Minute
                    .AddedDate = .AddedDate.AddHours(hour)
                    .AddedDate = .AddedDate.AddMinutes(minute)
                    .AddedDate = TimeZoneInfo.ConvertTimeToUtc(.AddedDate, ModuleContext.PortalSettings.UserInfo.Profile.PreferredTimeZone)

                    If Null.IsNull(m_oEntry.EntryID) Then
                        .EntryID = m_oEntryController.AddEntry(m_oEntry, ModuleContext.TabId).EntryID
                    End If
                    .PermaLink = Utility.GenerateEntryLink(PortalId, .EntryID, Me.TabId, .Title)

                    Dim userEnteredTerms As Array = txtTags.Text.Trim.Split(","c)
                    Dim terms As New List(Of Term)

                    For Each s As String In userEnteredTerms
                        If s.Length > 0 Then
                            'If (ContainsSpecialCharacters) Then
                            '    UI.Skins.Skin.AddModuleMessage(control, msg, ModuleMessage.ModuleMessageType.RedError);
                            'End If
                            Dim newTerm As Term = Components.Integration.Terms.CreateAndReturnTerm(s, 1)
                            terms.Add(newTerm)
                        End If
                    Next

                    If VocabularyId > 0 Then
                        For Each t As RadTreeNode In dtCategories.CheckedNodes
                            Dim objTerm As Term = Components.Integration.Terms.GetTermById(Convert.ToInt32(t.Value), VocabularyId)
                            terms.Add(objTerm)
                        Next
                    End If

                    .Terms.Clear()
                    .Terms.AddRange(terms)

                    m_oEntryController.UpdateEntry(m_oEntry, Me.TabId, PortalId)

                    If txtTrackBackUrl.Text <> "" Then
                        PingBackService.SendTrackBack(txtTrackBackUrl.Text, m_oEntry, m_oBlog.Title)
                    End If
                    If m_oBlog.AutoTrackback Then
                        Utility.AutoTrackback(m_oEntry, m_oBlog.Title)
                    End If

                    ' NOTE: 6.2 Journal Integration
                    If (publish) Then
                        Dim jc As New JournalController
                        Dim objectKey As String = String.Format("{0}:{1}", .BlogID.ToString(), m_oEntry.EntryID.ToString())
                        Dim ji As JournalItem = jc.Journal_GetByKey(PortalId, objectKey)
                        If Not ji Is Nothing Then
                            jc.Journal_DeleteByKey(PortalId, objectKey)
                        End If

                        ji = New JournalItem

                        ji.PortalId = ModuleContext.PortalId
                        ji.ProfileId = ModuleContext.PortalSettings.UserId
                        ji.UserId = ModuleContext.PortalSettings.UserId
                        ji.ContentItemId = m_oEntry.ContentItemId
                        ji.Title = .Title
                        ji.ItemData = New ItemData()
                        ji.ItemData.Url = Utility.AddTOQueryString(NavigateURL(), "EntryId", m_oEntry.EntryID.ToString())
                        ji.Summary = m_oEntry.Description
                        ji.Body = Nothing
                        ji.JournalTypeId = 7
                        ji.ObjectKey = objectKey
                        ji.SecuritySet = "E,"

                        jc.Journal_Save(ji, -1)
                    End If
                End With
            End If
        Catch exc As Exception    'Module failed to load
            ProcessModuleLoadException(Me, exc)
        End Try
    End Sub

    Private Function CreateCopyRight() As String
        Return GetString("msgCopyright", LocalResourceFile) & Date.UtcNow.Year & " " & m_oBlog.UserFullName
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

            If (InStr(1, "," & Entities.Host.Host.FileExtensions.ToUpper, "," & strExtension.ToUpper) <> 0) Or Me.PortalSettings.ActiveTab.ParentId = Me.PortalSettings.SuperTabId Then
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
                Me.valUpload.ErrorMessage = String.Format(GetString("RestrictedFileType"), strFileName, Replace(Me.PortalSettings.HostSettings("FileExtensions").ToString, ",", ", *."))
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
            Dim _filePath As String = FileController.getEntryDir(Me.FilePath, m_oEntry)
            Dim _fullName As String = _filePath & fileName
            System.IO.File.Delete(_fullName)
            Me.dgLinkedFiles.DataSource = FileController.getFileList(Me.FilePath, m_oEntry)
            Me.dgLinkedFiles.DataBind()
        Catch

        End Try
    End Sub

    Private Sub DeleteAllFiles()
        Try
            System.IO.Directory.Delete(FileController.getEntryDir(Me.FilePath, m_oEntry), True)
        Catch

        End Try

    End Sub

#End Region

End Class