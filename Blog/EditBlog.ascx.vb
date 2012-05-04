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
Imports DotNetNuke.Modules.Blog.Business
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Framework
Imports DotNetNuke.Common.Utilities

Partial Public Class EditBlog
    Inherits BlogModuleBase

#Region " Private Members "
    Private m_oBlogController As New BlogController
    Private m_oParentBlog As BlogInfo
    Private m_oBlog As BlogInfo
    Private m_oBlogSettings As Hashtable
#End Region

#Region " Controls "
#End Region

#Region "Event Handlers"

    Protected Overloads Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        jQuery.RequestUIRegistration()

        If Not (Request.Params("BlogID") Is Nothing) Then
            If Int32.Parse(Request.Params("BlogID")) > 0 Then
                m_oBlog = m_oBlogController.GetBlog(Int32.Parse(Request.Params("BlogID")))
                If Not Blog.Business.Security.HasBlogPermission(Me.UserId, m_oBlog.UserID, Me.ModuleId) Then
                    Response.Redirect(NavigateURL())
                ElseIf m_oBlog.ParentBlogID > -1 Then
                    m_oParentBlog = m_oBlogController.GetBlog(m_oBlog.ParentBlogID)
                End If
            End If
        End If

        If m_oBlog Is Nothing And Not (Request.Params("ParentBlogID") Is Nothing) And BlogSettings.AllowChildBlogs Then
            If Int32.Parse(Request.Params("ParentBlogID")) > 0 Then
                m_oParentBlog = m_oBlogController.GetBlog(Int32.Parse(Request.Params("ParentBlogID")))
            End If
        End If

        If Not m_oParentBlog Is Nothing Then
            If Not Blog.Business.Security.HasBlogPermission(Me.UserId, m_oParentBlog.UserID, Me.ModuleId) Then
                Response.Redirect(NavigateURL())
            Else
                If m_oBlog Is Nothing Then
                    Me.ModuleConfiguration.ModuleTitle = Localization.GetString("msgCreateNewChildBlog", LocalResourceFile)
                Else
                    Me.ModuleConfiguration.ModuleTitle = Localization.GetString("msgEditChildBlog", LocalResourceFile)
                End If
            End If
        Else
            If m_oBlog Is Nothing Then
                Me.ModuleConfiguration.ModuleTitle = Localization.GetString("msgCreateBlog", LocalResourceFile)
                Me.cmdGenerateLinks.Enabled = False
            Else
                Me.ModuleConfiguration.ModuleTitle = Localization.GetString("msgEditBlog", LocalResourceFile)
                Me.cmdGenerateLinks.Enabled = True
            End If
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            If Not Page.IsPostBack Then
                Localization.LoadCultureDropDownList(cboCulture, CultureDropDownTypes.NativeName, CType(Page, PageBase).PageCulture.Name)

                lblChildBlogsOff.Visible = (Not BlogSettings.AllowChildBlogs)

                If Not BlogSettings.AllowWLW Then
                    lblMetaWeblogUrl.Visible = False
                    lblMetaWeblogNotAvailable.Visible = True
                Else
                    lblMetaWeblogUrl.Visible = True
                    lblMetaWeblogNotAvailable.Visible = False
                End If

                If Not m_oBlog Is Nothing Then
                    'Load data
                    txtTitle.Text = m_oBlog.Title
                    txtDescription.Text = m_oBlog.Description
                    chkPublic.Checked = m_oBlog.Public

                    Try
                        ddTimeZone.Items.FindByValue(m_oBlog.TimeZone.Id).Selected = True
                    Catch ex As Exception
                    End Try

                    If m_oBlog.AllowComments Then
                        If m_oBlog.MustApproveComments Then
                            rdoUsersComments.Items.FindByValue("RequireApproval").Selected = True
                        Else
                            rdoUsersComments.Items.FindByValue("Allow").Selected = True
                        End If
                    Else
                        rdoUsersComments.Items.FindByValue("Disallow").Selected = True
                    End If
                    If m_oBlog.AllowAnonymous Then
                        If m_oBlog.MustApproveAnonymous Then
                            rdoAnonymousComments.Items.FindByValue("RequireApproval").Selected = True
                        Else
                            rdoAnonymousComments.Items.FindByValue("Allow").Selected = True
                        End If
                    Else
                        rdoAnonymousComments.Items.FindByValue("Disallow").Selected = True
                    End If
                    If m_oBlog.AllowTrackbacks Then
                        If m_oBlog.MustApproveTrackbacks Then
                            rdoTrackbacks.Items.FindByValue("RequireApproval").Selected = True
                        Else
                            rdoTrackbacks.Items.FindByValue("Allow").Selected = True
                        End If
                    Else
                        rdoTrackbacks.Items.FindByValue("Disallow").Selected = True
                    End If
                    chkEmailNotification.Checked = m_oBlog.EmailNotification
                    chkAutoTrackbacks.Checked = m_oBlog.AutoTrackback
                    chkCaptcha.Checked = m_oBlog.UseCaptcha
                    chkEnableGhostWriting.Checked = m_oBlog.EnableGhostWriter

                    chkSyndicate.Checked = m_oBlog.Syndicated
                    If m_oBlog.ParentBlogID = -1 Then
                        chkSyndicateIndependant.Visible = False
                    Else
                        chkSyndicateIndependant.Checked = m_oBlog.SyndicateIndependant
                    End If
                    If m_oBlog.SyndicationEmail Is Nothing Then
                        txtSyndicationEmail.Text = ModuleContext.PortalSettings.UserInfo.Email
                    Else
                        txtSyndicationEmail.Text = m_oBlog.SyndicationEmail
                    End If
                    cmdDelete.Visible = True
                    cmdAddChildBlog.Enabled = BlogSettings.AllowChildBlogs
                    If Not rdoUserName.SelectedItem Is Nothing Then rdoUserName.SelectedItem.Selected = False
                    rdoUserName.Items.FindByValue(m_oBlog.ShowFullName.ToString()).Selected = True
                    lstChildBlogs.Attributes.Add("onclick", "if (this.selectedIndex > -1) { " & cmdEditChildBlog.ClientID & ".disabled = false; " & cmdDeleteChildBlog.ClientID & ".disabled = false; }")

                    If m_oParentBlog Is Nothing Then
                        lstChildBlogs.DataTextField = "Title"
                        lstChildBlogs.DataValueField = "BlogID"
                        lstChildBlogs.DataSource = m_oBlogController.ListBlogs(Me.PortalId, m_oBlog.BlogID, True)
                        lstChildBlogs.DataBind()
                    Else
                        dnnSitePanelChildBlogs.Visible = False
                        fsChildBlogs.Visible = False
                    End If

     BindDateOptions(m_oBlog.Culture, m_oBlog.DateFormat)
                    lblMetaWeblogUrl.Text = "http://" & Request.Url.Host & ControlPath & "blogpost.ashx?tabid=" & TabId

                ElseIf Not m_oParentBlog Is Nothing Then
                    If Not rdoUserName.SelectedItem Is Nothing Then rdoUserName.SelectedItem.Selected = False
                    rdoUserName.Items.FindByValue(m_oParentBlog.ShowFullName.ToString()).Selected = True
     BindDateOptions(m_oParentBlog.Culture, m_oParentBlog.DateFormat)

                    dnnSitePanelChildBlogs.Visible = False
                    fsChildBlogs.Visible = False
                Else
     BindDateOptions(System.Threading.Thread.CurrentThread.CurrentCulture.Name, "g")
                End If

                If Not Request.UrlReferrer Is Nothing Then
                    ViewState("URLReferrer") = Request.UrlReferrer.ToString
                End If

                If m_oParentBlog Is Nothing Then
                    hlCancel.NavigateUrl = ModuleContext.NavigateUrl(ModuleContext.TabId, "", False, "")
                Else
                    hlCancel.NavigateUrl = Request.UrlReferrer.ToString
                End If
            End If
        Catch exc As Exception
            ProcessModuleLoadException(Me, exc)
        End Try
    End Sub

    Protected Sub cmdDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDelete.Click
        Try
            If Not m_oBlog Is Nothing Then
                m_oBlogController.DeleteBlog(m_oBlog.BlogID)
            End If
            Response.Redirect(NavigateURL(), True)
        Catch exc As Exception    'Module failed to load
            ProcessModuleLoadException(Me, exc)
        End Try
    End Sub

    Protected Sub cboCulture_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboCulture.SelectedIndexChanged
        BindDateFormats()
    End Sub

    Protected Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
        If UpdateBlog() Then
            If Not m_oParentBlog Is Nothing Then
                Response.Redirect(EditUrl("BlogID", m_oParentBlog.BlogID.ToString(), "Edit_Blog"), True)
            Else
                Response.Redirect(Utility.AddTOQueryString(NavigateURL(), "BlogID", m_oBlog.BlogID.ToString()), True)
            End If
        End If
    End Sub

    Protected Sub btnAddChildBlog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAddChildBlog.Click
        If UpdateBlog() Then
            Response.Redirect(EditUrl("ParentBlogID", m_oBlog.BlogID.ToString(), "Edit_Blog"))
        End If
    End Sub

    Protected Sub btnEditChildBlog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdEditChildBlog.Click
        If Not lstChildBlogs.SelectedItem Is Nothing Then
            If UpdateBlog() Then
                Response.Redirect(EditUrl("BlogID", lstChildBlogs.SelectedValue, "Edit_Blog"))
            End If
        End If
    End Sub

    Protected Sub btnDeleteChildBlog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDeleteChildBlog.Click
        If Not lstChildBlogs.SelectedItem Is Nothing Then
            m_oBlogController.DeleteBlog(CType(lstChildBlogs.SelectedValue, Integer))
            lstChildBlogs.Items.Remove(lstChildBlogs.SelectedItem)
        End If
    End Sub

    Protected Sub cmdGenerateLinks_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdGenerateLinks.Click
        Utility.CreateAllEntryLinks(PortalId, m_oBlog.BlogID)
    End Sub

#End Region

#Region "Private Methods"

    Private Sub BindDateOptions(ByVal Culture As String, ByVal DateFormat As String)
        If Not cboCulture.Items.FindByValue(Culture) Is Nothing Then
            cboCulture.SelectedValue = Culture
        End If
        'If Not cboTimeZone.Items.FindByValue(TimeZone.ToString()) Is Nothing Then
        ' cboTimeZone.SelectedValue = TimeZone.ToString
        'End If
        BindDateFormats()
        If Not cboDateFormat.Items.FindByValue(DateFormat) Is Nothing Then
            cboDateFormat.SelectedValue = DateFormat
        End If
    End Sub

    Private Function UpdateBlog() As Boolean
        Try
            If Page.IsValid = True Then
                If m_oBlog Is Nothing Then
                    m_oBlog = New BlogInfo
                    m_oBlog = CType(CBO.InitializeObject(m_oBlog, GetType(BlogInfo)), BlogInfo)
                    With m_oBlog
                        .UserID = Me.UserId
                        .PortalID = Me.PortalId
                        If Not m_oParentBlog Is Nothing Then
                            .ParentBlogID = m_oParentBlog.BlogID
                        End If
                    End With
                End If
                With m_oBlog
                    'bind text values to object
                    .Title = txtTitle.Text
                    .Description = txtDescription.Text
                    .Public = chkPublic.Checked
                    .ShowFullName = CType(rdoUserName.SelectedItem.Value, Boolean)
                    .Culture = cboCulture.SelectedItem.Value
                    .DateFormat = cboDateFormat.SelectedItem.Value
                    .TimeZone = TimeZoneInfo.FindSystemTimeZoneById(ddTimeZone.SelectedValue)
                    .Syndicated = chkSyndicate.Checked
                    .SyndicateIndependant = chkSyndicateIndependant.Checked
                    .SyndicationEmail = txtSyndicationEmail.Text
                    .EmailNotification = chkEmailNotification.Checked
                    .AutoTrackback = chkAutoTrackbacks.Checked
                    .UseCaptcha = chkCaptcha.Checked
                    .EnableGhostWriter = chkEnableGhostWriting.Checked

                    Select Case rdoUsersComments.SelectedItem.Value
                        Case "Allow"
                            .AllowComments = True
                            .MustApproveComments = False
                        Case "RequireApproval"
                            .AllowComments = True
                            .MustApproveComments = True
                        Case "Disallow"
                            .AllowComments = False
                            .MustApproveComments = True
                        Case Else
                            .AllowComments = True
                            .MustApproveComments = True
                    End Select
                    Select Case rdoAnonymousComments.SelectedItem.Value
                        Case "Allow"
                            .AllowAnonymous = True
                            .MustApproveAnonymous = False
                        Case "RequireApproval"
                            .AllowAnonymous = True
                            .MustApproveAnonymous = True
                        Case "Disallow"
                            .AllowAnonymous = False
                            .MustApproveAnonymous = True
                        Case Else
                            .AllowAnonymous = True
                            .MustApproveAnonymous = True
                    End Select
                    Select Case rdoTrackbacks.SelectedItem.Value
                        Case "Allow"
                            .AllowTrackbacks = True
                            .MustApproveTrackbacks = False
                        Case "RequireApproval"
                            .AllowTrackbacks = True
                            .MustApproveTrackbacks = True
                        Case "Disallow"
                            .AllowTrackbacks = False
                            .MustApproveTrackbacks = True
                        Case Else
                            .AllowTrackbacks = True
                            .MustApproveTrackbacks = True
                    End Select
                    'DR-19/04/2009-BLG-9760
                    If Null.IsNull(m_oBlog.BlogID) Then
                        .BlogID = m_oBlogController.AddBlog(m_oBlog)
                        If .Syndicated Then m_oBlog = m_oBlogController.GetBlog(.BlogID)
                    Else
                        m_oBlogController.UpdateBlog(m_oBlog)
                    End If
                End With
                Return True
            End If
        Catch exc As Exception    'Module failed to load
            ProcessModuleLoadException(Me, exc)
        End Try
        Return False
    End Function

    Private Sub BindDateFormats()
        If Not cboCulture.SelectedItem Is Nothing Then
            Dim tzi As TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(ddTimeZone.SelectedValue)
            Dim dt As Date = TimeZoneInfo.ConvertTimeFromUtc(Date.UtcNow, tzi)
            Dim dtf As System.Globalization.DateTimeFormatInfo = New System.Globalization.CultureInfo(cboCulture.SelectedItem.Value, False).DateTimeFormat
            Dim i As Integer = cboDateFormat.SelectedIndex
            With cboDateFormat
                .Items.Clear()
                .Items.Add(New ListItem(dt.ToString("d", dtf), "d"))
                .Items.Add(New ListItem(dt.ToString("g", dtf), "g"))
                .Items.Add(New ListItem(dt.ToString("G", dtf), "G"))
                .Items.Add(New ListItem(dt.ToString("D", dtf), "D"))
                .Items.Add(New ListItem(dt.ToString("f", dtf), "f"))
                .Items.Add(New ListItem(dt.ToString("F", dtf), "F"))
                .Items.Add(New ListItem(dt.ToString("U", dtf), "U"))
                .Items.Add(New ListItem(dt.ToString("r", dtf), "r"))
                .Items.Add(New ListItem(dt.ToString("s", dtf), "s"))
                .Items.Add(New ListItem(dt.ToString("u", dtf), "u"))
                .SelectedIndex = i
            End With
        End If
    End Sub

#End Region

End Class