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

#Region "Private Members"

    Private m_oParentBlog As BlogInfo
    Private objBlog As BlogInfo
    Private m_oBlogSettings As Hashtable

#End Region

#Region " Controls "
#End Region

#Region "Event Handlers"

    Protected Overloads Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        jQuery.RequestDnnPluginsRegistration()

        Dim cntBlog As New BlogController
        Dim objSecurity As ModuleSecurity = New ModuleSecurity(ModuleContext.ModuleId, ModuleContext.TabId)

        If SpecificBlogId > 0 Then
            objBlog = cntBlog.GetBlog(SpecificBlogId)

            Dim isOwner As Boolean = (objBlog.UserID = ModuleContext.PortalSettings.UserId)

            If (objSecurity.CanEditBlog(isOwner) = False) Then
                Response.Redirect(NavigateURL("Access Denied"))
            ElseIf objBlog.ParentBlogID > -1 Then
                m_oParentBlog = cntBlog.GetBlog(objBlog.ParentBlogID)
            End If
        End If

        If objBlog Is Nothing And Not (Request.Params("ParentBlogID") Is Nothing) And BlogSettings.AllowChildBlogs Then
            If Int32.Parse(Request.Params("ParentBlogID")) > 0 Then
                m_oParentBlog = cntBlog.GetBlog(Int32.Parse(Request.Params("ParentBlogID")))
            End If
        End If

        If Not m_oParentBlog Is Nothing Then
            Dim isOwner As Boolean = (m_oParentBlog.UserID = ModuleContext.PortalSettings.UserId)

            If objSecurity.CanEditBlog(isOwner) Then
                If objBlog Is Nothing Then
                    Me.ModuleConfiguration.ModuleTitle = Localization.GetString("msgCreateNewChildBlog", LocalResourceFile)
                Else
                    Me.ModuleConfiguration.ModuleTitle = Localization.GetString("msgEditChildBlog", LocalResourceFile)
                End If
            Else
                Response.Redirect(NavigateURL("Access Denied"))
            End If
        Else
            If objBlog Is Nothing Then
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
                'Localization.LoadCultureDropDownList(cboCulture, CultureDropDownTypes.NativeName, CType(Page, PageBase).PageCulture.Name)

                fsChildBlogs.Visible = BlogSettings.AllowChildBlogs
                dnnSitePanelChildBlogs.Visible = BlogSettings.AllowChildBlogs

                If Not BlogSettings.AllowWLW Then
                    lblMetaWeblogUrl.Visible = False
                    lblMetaWeblogNotAvailable.Visible = True
                Else
                    lblMetaWeblogUrl.Visible = True
                    lblMetaWeblogNotAvailable.Visible = False
                End If

                If Not objBlog Is Nothing Then
                    'Load data
                    txtTitle.Text = objBlog.Title
                    txtDescription.Text = objBlog.Description
                    chkPublic.Checked = objBlog.Public

                    'Try
                    '    ddTimeZone.Items.FindByValue(objBlog.TimeZone.Id).Selected = True
                    'Catch ex As Exception
                    'End Try

                    If objBlog.AllowComments Then
                        If objBlog.MustApproveComments Then
                            rdoUsersComments.Items.FindByValue("RequireApproval").Selected = True
                        Else
                            rdoUsersComments.Items.FindByValue("Allow").Selected = True
                        End If
                    Else
                        rdoUsersComments.Items.FindByValue("Disallow").Selected = True
                    End If
                    If objBlog.AllowAnonymous Then
                        If objBlog.MustApproveAnonymous Then
                            rdoAnonymousComments.Items.FindByValue("RequireApproval").Selected = True
                        Else
                            rdoAnonymousComments.Items.FindByValue("Allow").Selected = True
                        End If
                    Else
                        rdoAnonymousComments.Items.FindByValue("Disallow").Selected = True
                    End If
                    If objBlog.AllowTrackbacks Then
                        If objBlog.MustApproveTrackbacks Then
                            rdoTrackbacks.Items.FindByValue("RequireApproval").Selected = True
                        Else
                            rdoTrackbacks.Items.FindByValue("Allow").Selected = True
                        End If
                    Else
                        rdoTrackbacks.Items.FindByValue("Disallow").Selected = True
                    End If
                    chkEmailNotification.Checked = objBlog.EmailNotification
                    chkAutoTrackbacks.Checked = objBlog.AutoTrackback
                    chkCaptcha.Checked = objBlog.UseCaptcha
                    chkEnableGhostWriting.Checked = objBlog.EnableGhostWriter

                    chkSyndicate.Checked = objBlog.Syndicated
                    If objBlog.ParentBlogID = -1 Then
                        chkSyndicateIndependant.Visible = False
                    Else
                        chkSyndicateIndependant.Checked = objBlog.SyndicateIndependant
                    End If
                    If objBlog.SyndicationEmail Is Nothing Then
                        txtSyndicationEmail.Text = ModuleContext.PortalSettings.UserInfo.Email
                    Else
                        txtSyndicationEmail.Text = objBlog.SyndicationEmail
                    End If
                    cmdDelete.Visible = True
                    cmdAddChildBlog.Enabled = BlogSettings.AllowChildBlogs
                    If Not rdoUserName.SelectedItem Is Nothing Then rdoUserName.SelectedItem.Selected = False
                    rdoUserName.Items.FindByValue(objBlog.ShowFullName.ToString()).Selected = True
                    lstChildBlogs.Attributes.Add("onclick", "if (this.selectedIndex > -1) { " & cmdEditChildBlog.ClientID & ".disabled = false; " & cmdDeleteChildBlog.ClientID & ".disabled = false; }")

                    If m_oParentBlog Is Nothing Then
                        Dim cntBlog As New BlogController

                        lstChildBlogs.DataTextField = "Title"
                        lstChildBlogs.DataValueField = "BlogID"
                        lstChildBlogs.DataSource = cntBlog.ListBlogs(Me.PortalId, objBlog.BlogID, True)
                        lstChildBlogs.DataBind()
                    Else
                        dnnSitePanelChildBlogs.Visible = False
                    End If

                    'BindDateOptions(objBlog.Culture, objBlog.DateFormat)
                    lblMetaWeblogUrl.Text = "http://" & Request.Url.Host & ControlPath & "blogpost.ashx?tabid=" & TabId

                ElseIf Not m_oParentBlog Is Nothing Then
                    If Not rdoUserName.SelectedItem Is Nothing Then rdoUserName.SelectedItem.Selected = False
                    rdoUserName.Items.FindByValue(m_oParentBlog.ShowFullName.ToString()).Selected = True
                    'BindDateOptions(m_oParentBlog.Culture, m_oParentBlog.DateFormat)

                    dnnSitePanelChildBlogs.Visible = False
                    'Else
                    '    BindDateOptions(System.Threading.Thread.CurrentThread.CurrentCulture.Name, "g")
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
            If Not objBlog Is Nothing Then
                Dim cntBlog As New BlogController
                cntBlog.DeleteBlog(objBlog.BlogID)
            End If
            Response.Redirect(NavigateURL(), True)
        Catch exc As Exception    'Module failed to load
            ProcessModuleLoadException(Me, exc)
        End Try
    End Sub

    'Protected Sub cboCulture_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboCulture.SelectedIndexChanged
    '    BindDateFormats()
    'End Sub

    Protected Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
        If UpdateBlog() Then
            If Not m_oParentBlog Is Nothing Then
                Response.Redirect(EditUrl("BlogID", m_oParentBlog.BlogID.ToString(), "Edit_Blog"), True)
            Else
                Response.Redirect(Utility.AddTOQueryString(NavigateURL(), "BlogID", objBlog.BlogID.ToString()), True)
            End If
        End If
    End Sub

    Protected Sub btnAddChildBlog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAddChildBlog.Click
        If UpdateBlog() Then
            Response.Redirect(EditUrl("ParentBlogID", objBlog.BlogID.ToString(), "Edit_Blog"))
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
            Dim cntBlog As New BlogController

            cntBlog.DeleteBlog(CType(lstChildBlogs.SelectedValue, Integer))
            lstChildBlogs.Items.Remove(lstChildBlogs.SelectedItem)
        End If
    End Sub

    Protected Sub cmdGenerateLinks_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdGenerateLinks.Click
        Utility.CreateAllEntryLinks(PortalId, objBlog.BlogID)
    End Sub

#End Region

#Region "Private Methods"

    'Private Sub BindDateOptions(ByVal Culture As String, ByVal DateFormat As String)
    '    'If Not cboCulture.Items.FindByValue(Culture) Is Nothing Then
    '    '    cboCulture.SelectedValue = Culture
    '    'End If
    '    'If Not cboTimeZone.Items.FindByValue(TimeZone.ToString()) Is Nothing Then
    '    ' cboTimeZone.SelectedValue = TimeZone.ToString
    '    'End If
    '    'BindDateFormats()
    '    'If Not cboDateFormat.Items.FindByValue(DateFormat) Is Nothing Then
    '    '    cboDateFormat.SelectedValue = DateFormat
    '    'End If
    'End Sub

    Private Function UpdateBlog() As Boolean
        Try
            If Page.IsValid = True Then
                If objBlog Is Nothing Then
                    objBlog = New BlogInfo
                    objBlog = CType(CBO.InitializeObject(objBlog, GetType(BlogInfo)), BlogInfo)
                    With objBlog
                        .UserID = Me.UserId
                        .PortalID = Me.PortalId
                        If Not m_oParentBlog Is Nothing Then
                            .ParentBlogID = m_oParentBlog.BlogID
                        End If
                    End With
                End If
                With objBlog
                    'bind text values to object
                    .Title = txtTitle.Text
                    .Description = txtDescription.Text
                    .Public = chkPublic.Checked
                    .ShowFullName = CType(rdoUserName.SelectedItem.Value, Boolean)
                    '.Culture = cboCulture.SelectedItem.Value
                    '.DateFormat = cboDateFormat.SelectedItem.Value
                    '.TimeZone = TimeZoneInfo.FindSystemTimeZoneById(ddTimeZone.SelectedValue)
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
                    Dim cntBlog As New BlogController

                    If Null.IsNull(objBlog.BlogID) Then
                        .BlogID = cntBlog.AddBlog(objBlog)
                        If .Syndicated Then objBlog = cntBlog.GetBlog(.BlogID)
                    Else
                        cntBlog.UpdateBlog(objBlog)
                    End If
                End With
                Return True
            End If
        Catch exc As Exception    'Module failed to load
            ProcessModuleLoadException(Me, exc)
        End Try
        Return False
    End Function

    'Private Sub BindDateFormats()
    '    If Not cboCulture.SelectedItem Is Nothing Then
    '        Dim tzi As TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(ddTimeZone.SelectedValue)
    '        Dim dt As Date = TimeZoneInfo.ConvertTimeFromUtc(Date.UtcNow, tzi)
    '        Dim dtf As System.Globalization.DateTimeFormatInfo = New System.Globalization.CultureInfo(cboCulture.SelectedItem.Value, False).DateTimeFormat
    '        Dim i As Integer = cboDateFormat.SelectedIndex
    '        With cboDateFormat
    '            .Items.Clear()
    '            .Items.Add(New ListItem(dt.ToString("d", dtf), "d"))
    '            .Items.Add(New ListItem(dt.ToString("g", dtf), "g"))
    '            .Items.Add(New ListItem(dt.ToString("G", dtf), "G"))
    '            .Items.Add(New ListItem(dt.ToString("D", dtf), "D"))
    '            .Items.Add(New ListItem(dt.ToString("f", dtf), "f"))
    '            .Items.Add(New ListItem(dt.ToString("F", dtf), "F"))
    '            .Items.Add(New ListItem(dt.ToString("U", dtf), "U"))
    '            .Items.Add(New ListItem(dt.ToString("r", dtf), "r"))
    '            .Items.Add(New ListItem(dt.ToString("s", dtf), "s"))
    '            .Items.Add(New ListItem(dt.ToString("u", dtf), "u"))
    '            .SelectedIndex = i
    '        End With
    '    End If
    'End Sub

#End Region

End Class