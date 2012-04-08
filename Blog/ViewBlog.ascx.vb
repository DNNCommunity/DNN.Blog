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
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization.Localization
Imports DotNetNuke.Services.Localization

Partial Public Class ViewBlog
    Inherits BlogModuleBase

#Region " Private Members "
    Private m_sSearchString As String
    Private m_sSearchType As String = "Keyword"
    Private m_oBlogController As New BlogController
    Private m_oBlog As BlogInfo
    Private m_dBlogDate As Date = Date.UtcNow
    Private m_dBlogDateType As String
    Private m_bSearchDisplay As Boolean = False
    Private m_PersonalBlogID As Integer
    Private m_SeoFriendlyUrl As Boolean
#End Region

#Region "Event Handlers"

    Protected Overloads Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        m_oBlog = m_oBlogController.GetBlogFromContext()
        m_PersonalBlogID = BlogSettings.PageBlogs

        If m_PersonalBlogID <> -1 And m_oBlog Is Nothing Then
            Dim objBlog As New BlogController
            m_oBlog = objBlog.GetBlog(m_PersonalBlogID)
            'ModuleConfiguration.ModuleTitle = m_oBlog.Title
        End If
        If Not Request.Params("Search") Is Nothing Then
            m_sSearchString = Request.Params("Search")
            m_bSearchDisplay = True
            If m_oBlog Is Nothing Then
                ModuleConfiguration.ModuleTitle = GetString("msgSearchResults", LocalResourceFile)
            Else
                ModuleConfiguration.ModuleTitle = GetString("msgSearchResultsFor", LocalResourceFile) & " " & m_oBlog.Title
            End If

        Else
            If m_oBlog Is Nothing Then
                'Antonio Chagoury 9/1/2007
                'BLG-6126
                'ModuleConfiguration.ModuleTitle = GetString("msgMostRecentEntries", LocalResourceFile)
            Else
                'BLG-6126
                'ModuleConfiguration.ModuleTitle = m_oBlog.Title
                If Utility.HasBlogPermission(Me.UserId, m_oBlog.UserID, Me.ModuleId) Then
                    MyActions.Add(GetNextActionID, GetString("msgEditBlogSettings", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "", EditUrl("BlogID", m_oBlog.BlogID.ToString(), "Edit_Blog"), False, DotNetNuke.Security.SecurityAccessLevel.Edit, True, False)
                    MyActions.Add(GetNextActionID, GetString("msgAddBlogEntry", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "", EditUrl("BlogID", m_oBlog.BlogID.ToString(), "Edit_Entry"), False, DotNetNuke.Security.SecurityAccessLevel.Edit, True, False)
                    MyActions.Add(GetNextActionID, GetString("msgMassEdit", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "", EditUrl("BlogID", m_oBlog.BlogID.ToString(), "Mass_Edit"), False, DotNetNuke.Security.SecurityAccessLevel.Edit, True, False)
                End If
            End If
        End If
        MyActions.Add(GetNextActionID, GetString("msgModuleOptions", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "", EditUrl("", "", "Module_Options"), False, DotNetNuke.Security.SecurityAccessLevel.Admin, True, False)
        Me.ModuleConfiguration.SupportedFeatures = 0
    End Sub

    Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            m_SeoFriendlyUrl = BlogSettings.ShowSeoFriendlyUrl
            Dim objEntries As New EntryController
            Dim list As ArrayList
            If Not Request.Params("BlogDate") Is Nothing Then
                m_dBlogDate = CType(Date.Parse(Request.Params("BlogDate")), Date)
                'BLG-4154
                'Antonio Chagoury 9/1/2007
                'm_dBlogDate = m_dBlogDate.AddDays(1)
                If Not Request.Params("DateType") Is Nothing Then
                    m_dBlogDateType = Request.Params("DateType")
                End If
            End If
            If Not Request.Params("SearchType") Is Nothing Then
                m_sSearchType = Request.Params("SearchType")
            End If

            If Not Page.IsPostBack Then
                Dim pageTitle As String = ""

                If m_bSearchDisplay Then
                    If m_sSearchType = "Phrase" Then
                        If m_oBlog Is Nothing Then
                            list = New DotNetNuke.Modules.Blog.Business.SearchController().SearchByPhraseByPortal(Me.PortalId, m_sSearchString, DotNetNuke.Security.PortalSecurity.IsInRole(Me.PortalSettings.AdministratorRoleId.ToString()), DotNetNuke.Security.PortalSecurity.IsInRole(Me.PortalSettings.AdministratorRoleId.ToString()))
                        Else
                            list = New DotNetNuke.Modules.Blog.Business.SearchController().SearchByPhraseByBlog(m_oBlog.BlogID, m_sSearchString, DotNetNuke.Security.PortalSecurity.IsInRole(Me.PortalSettings.AdministratorRoleId.ToString()), DotNetNuke.Security.PortalSecurity.IsInRole(Me.PortalSettings.AdministratorRoleId.ToString()))
                        End If
                    Else
                        If m_oBlog Is Nothing Then
                            list = New DotNetNuke.Modules.Blog.Business.SearchController().SearchByKeywordByPortal(Me.PortalId, m_sSearchString, DotNetNuke.Security.PortalSecurity.IsInRole(Me.PortalSettings.AdministratorRoleId.ToString()), DotNetNuke.Security.PortalSecurity.IsInRole(Me.PortalSettings.AdministratorRoleId.ToString()))
                        Else
                            list = New DotNetNuke.Modules.Blog.Business.SearchController().SearchByKeywordByBlog(m_oBlog.BlogID, m_sSearchString, DotNetNuke.Security.PortalSecurity.IsInRole(Me.PortalSettings.AdministratorRoleId.ToString()), DotNetNuke.Security.PortalSecurity.IsInRole(Me.PortalSettings.AdministratorRoleId.ToString()))
                        End If
                    End If
                    lstSearchResults.Visible = True
                    lstBlogView.Visible = False

                    lstSearchResults.DataSource = list
                    lstSearchResults.DataBind()

                    ' if no Entries are shown, show the info Entry
                    InfoEntry.Visible = (lstSearchResults.Items.Count = 0)
                Else ' not a search display
                    If Request.Params("catid") Is Nothing Then
                        If Request.Params("tagid") Is Nothing Then
                            If m_oBlog Is Nothing Then
                                'BLG-4154
                                'Antonio Chagoury 9/1/2007
                                list = objEntries.ListEntriesByPortal(Me.PortalId, m_dBlogDate, m_dBlogDateType, DotNetNuke.Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()), DotNetNuke.Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()), BlogSettings.RecentEntriesMax)
                                pnlBlogInfo.Visible = False
                                If Not lnkRecentRss Is Nothing Then
                                    lnkRecentRss.NavigateUrl = NavigateURL(Me.TabId, "", "rssid=0")
                                End If
                            Else
                                pnlBlogInfo.Visible = True
                                If m_oBlog.ShowFullName Then
                                    lblAuthor.Text = m_oBlog.UserFullName
                                Else
                                    lblAuthor.Text = m_oBlog.UserName
                                End If
                                lblCreated.Text = Utility.FormatDate(m_oBlog.Created, m_oBlog.Culture, m_oBlog.DateFormat, m_oBlog.TimeZone)
                                litBlogDescription.Text = m_oBlog.Description
                                list = objEntries.ListEntriesByBlog(m_oBlog.BlogID, m_dBlogDate, Utility.HasBlogPermission(Me.UserId, m_oBlog.UserID, Me.ModuleId), Utility.HasBlogPermission(Me.UserId, m_oBlog.UserID, Me.ModuleId), BlogSettings.RecentEntriesMax)
                                Me.BasePage.Author = m_oBlog.UserFullName
                                pageTitle = Me.BasePage.Title & " - " & m_oBlog.Title
                            End If
                        Else ' we have a tag id
                            list = objEntries.ListAllEntriesByTag(Me.PortalId, CInt(Request.Params("tagid")), DotNetNuke.Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()), DotNetNuke.Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()))
                            pnlBlogInfo.Visible = False
                            If Not lnkRecentRss Is Nothing Then
                                lnkRecentRss.NavigateUrl = NavigateURL(Me.TabId, "", "rssid=0", "tagid=" + Request.Params("tagid"))
                            End If
                            Dim t As TagInfo = TagController.GetTag(CInt(Request.Params("tagid")))
                            If t IsNot Nothing Then
                                pageTitle = Me.BasePage.Title & " - " & t.Tag
                            End If
                        End If
                    Else ' we have a cat id
                        list = objEntries.ListAllEntriesByCategory(Me.PortalId, CInt(Request.Params("catid")), DotNetNuke.Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()), DotNetNuke.Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()))
                        pnlBlogInfo.Visible = False
                        If Not lnkRecentRss Is Nothing Then
                            lnkRecentRss.NavigateUrl = NavigateURL(Me.TabId, "", "rssid=0", "catid=" + Request.Params("catid"))
                        End If
                        Dim c As CategoryInfo = CategoryController.GetCategory(CInt(Request.Params("catid")))
                        If c IsNot Nothing Then
                            pageTitle = Me.BasePage.Title & " - " & c.Category
                        End If
                    End If ' no cat id present

                    lstBlogView.DataSource = list
                    lstBlogView.DataBind()
                    ' if no Entries are shown, show the info Entry
                    InfoEntry.Visible = (lstBlogView.Items.Count = 0)

                    ' set page title
                    If pageTitle = "" Then
                        pageTitle = Me.BasePage.Title & " - " & ModuleConfiguration.ModuleTitle
                    End If
                    If BlogSettings.ShowUniqueTitle Then
                        Try             ' 4.0.1 Bug
                            If PortalSettings.Version <> "4.0.1" Then
                                Me.BasePage.Title = pageTitle
                            End If
                        Catch exc As Exception
                            ProcessModuleLoadException(Me, exc, True)
                        End Try
                    End If
                End If ' search display or not

                If Not m_oBlog Is Nothing Then
                    If m_oBlog.Syndicated And (m_oBlog.ParentBlogID = -1 Or m_oBlog.SyndicateIndependant) Then
                        lnkRSS.NavigateUrl = NavigateURL(Me.TabId, "", "rssid=" & m_oBlog.BlogID.ToString)
                        lnkRSS.Visible = True
                    End If
                End If

                ' add rss discovery link
                Dim ph As PlaceHolder = CType(BasePage.FindControl("phDNNHead"), PlaceHolder)
                If Not ph Is Nothing Then
                    If m_oBlog Is Nothing AndAlso pnlBlogRss.Visible Then
                        Dim objLink As New HtmlGenericControl("LINK")
                        objLink.Attributes("title") = "RSS"
                        objLink.Attributes("rel") = "alternate"
                        objLink.Attributes("type") = "application/rss+xml"
                        objLink.Attributes("href") = NavigateURL(Me.TabId, "", "rssid=0")
                        ph.Controls.Add(objLink)
                    ElseIf Not m_oBlog Is Nothing AndAlso m_oBlog.Syndicated AndAlso (m_oBlog.ParentBlogID = -1 OrElse m_oBlog.SyndicateIndependant) Then
                        Dim objLink As New HtmlGenericControl("LINK")
                        objLink.Attributes("title") = "RSS"
                        objLink.Attributes("rel") = "alternate"
                        objLink.Attributes("type") = "application/rss+xml"
                        objLink.Attributes("href") = NavigateURL(Me.TabId, "", "rssid=" & m_oBlog.BlogID.ToString)
                        ph.Controls.Add(objLink)
                    End If
                End If

                If (InfoEntry.Visible) Then
                    If m_bSearchDisplay Then
                        InfoEntry.Text = GetString("msgNoSearchResult", LocalResourceFile)
                    ElseIf m_dBlogDate <> Date.MinValue Then
                        InfoEntry.Text = GetString("msgNoPeriodResult", LocalResourceFile)
                    ElseIf Not m_oBlog Is Nothing Then
                        InfoEntry.Text = GetString("msgNoBlogResult", LocalResourceFile)
                    Else
                        InfoEntry.Text = GetString("msgNoResult", LocalResourceFile)
                    End If
                End If
            End If

        Catch exc As Exception
            ProcessModuleLoadException(Me, exc)
        End Try
    End Sub

    Protected Sub lstBlogView_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataListItemEventArgs) Handles lstBlogView.ItemDataBound
        Dim lblUserName As System.Web.UI.WebControls.Label = CType(e.Item.FindControl("lblUserName"), System.Web.UI.WebControls.Label)
        Dim lblDescription As System.Web.UI.WebControls.Literal = CType(e.Item.FindControl("litDescription"), System.Web.UI.WebControls.Literal)
        Dim lnkComments As System.Web.UI.WebControls.HyperLink = CType(e.Item.FindControl("lnkComments"), System.Web.UI.WebControls.HyperLink)
        Dim lblPublished As System.Web.UI.WebControls.Label = CType(e.Item.FindControl("lblPublished"), System.Web.UI.WebControls.Label)
        Dim lblPublishDate As System.Web.UI.WebControls.Label = CType(e.Item.FindControl("lblPublishDate"), System.Web.UI.WebControls.Label)
        Dim lnkParentBlog As System.Web.UI.WebControls.HyperLink = CType(e.Item.FindControl("lnkParentBlog"), System.Web.UI.WebControls.HyperLink)
        Dim imgBlogParentSeparator As System.Web.UI.WebControls.Image = CType(e.Item.FindControl("imgBlogParentSeparator"), System.Web.UI.WebControls.Image)
        Dim lnkChildBlog As System.Web.UI.WebControls.HyperLink = CType(e.Item.FindControl("lnkChildBlog"), System.Web.UI.WebControls.HyperLink)
        Dim lnkEntry As System.Web.UI.WebControls.HyperLink = CType(e.Item.FindControl("lnkEntry"), System.Web.UI.WebControls.HyperLink)
        Dim divBlogReadMore As System.Web.UI.HtmlControls.HtmlGenericControl = CType(e.Item.FindControl("divBlogReadMore"), System.Web.UI.HtmlControls.HtmlGenericControl)

        Dim oBlog As BlogInfo

        If m_oBlog Is Nothing Then
            oBlog = m_oBlogController.GetBlog(CType(CType(e.Item.DataItem, EntryInfo).BlogID, Integer))
        Else
            If m_oBlog.BlogID <> CType(CType(e.Item.DataItem, EntryInfo).BlogID, Integer) Then
                oBlog = m_oBlogController.GetBlog(CType(CType(e.Item.DataItem, EntryInfo).BlogID, Integer))
            Else
                oBlog = m_oBlog
            End If
        End If

        ' Added this in order to have the Edit Entry Link on all the pages.
        Dim m_oEntry As EntryInfo = CType(e.Item.DataItem, EntryInfo)
        Dim lnkEditEntry As HyperLink
        Dim imgEdit As System.Web.UI.WebControls.Image
        lnkEditEntry = CType(e.Item.FindControl("lnkEditEntry"), HyperLink)
        imgEdit = CType(e.Item.FindControl("imgEdit"), System.Web.UI.WebControls.Image)
        If Not m_oEntry Is Nothing Then
            If Utility.HasBlogPermission(Me.UserId, m_oEntry.UserID, Me.ModuleId) AndAlso Not lnkEditEntry Is Nothing Then
                lnkEditEntry.Visible = True
                lnkEditEntry.NavigateUrl = EditUrl("EntryID", CType(e.Item.DataItem, EntryInfo).EntryID.ToString(), "Edit_Entry")
            Else
                lnkEditEntry.Visible = False
            End If

            Dim hlPermaLink As HyperLink = CType(e.Item.FindControl("hlPermaLink"), HyperLink)
            hlPermaLink.NavigateUrl = m_oEntry.PermaLink
            hlPermaLink.Text = Localization.GetString("lnkPermaLink", LocalResourceFile)
            hlPermaLink.Visible = (m_oEntry.PermaLink <> DotNetNuke.Modules.Blog.Business.Utility.BlogNavigateURL(TabId, PortalId, m_oEntry.EntryID, m_oEntry.Title, BlogSettings.ShowSeoFriendlyUrl))

            Dim hlMore As HyperLink = CType(e.Item.FindControl("hlMore"), HyperLink)
            hlMore.NavigateUrl = DotNetNuke.Modules.Blog.Business.Utility.BlogNavigateURL(TabId, PortalId, m_oEntry.EntryID, m_oEntry.Title, BlogSettings.ShowSeoFriendlyUrl)
            hlMore.Text = Localization.GetString("lnkReadMore", LocalResourceFile)
            hlMore.Visible = (m_oEntry.PermaLink <> DotNetNuke.Modules.Blog.Business.Utility.BlogNavigateURL(TabId, PortalId, m_oEntry.EntryID, m_oEntry.Title, BlogSettings.ShowSeoFriendlyUrl))

        End If

        ' 10/28/08 RR Replace all instances of BlogNavigateURL with Permalink
        'lnkEntry.NavigateUrl = Utility.BlogNavigateURL(Me.TabId, CType(e.Item.DataItem, EntryInfo), m_SeoFriendlyUrl)
        lnkEntry.NavigateUrl = m_oEntry.PermaLink
        lnkComments.NavigateUrl = m_oEntry.PermaLink & "#Comments"

        'DR-04/24/2009-BLG-9712
        lnkEntry.Attributes.Add("rel", "bookmark")

        'Antonio Chagoury
        'BLG-5307
        'If Not lnkReadMore Is Nothing Then
        ' ' 10/28/08 RR Replace all instances of BlogNavigateURL with Permalink
        ' 'lnkReadMore.NavigateUrl = Utility.BlogNavigateURL(Me.TabId, CType(e.Item.DataItem, EntryInfo), m_SeoFriendlyUrl)
        ' lnkReadMore.NavigateUrl = m_oEntry.PermaLink
        'End If

        ' Display the proper UserName
        If CType(e.Item.DataItem, EntryInfo).UserName.Length > 0 Then
            If oBlog.ShowFullName Then
                lblUserName.Text = GetString("msgCreateFrom", LocalResourceFile) & " "
                lblUserName.Text += CType(e.Item.DataItem, EntryInfo).UserFullName
                lblUserName.Text += " " & GetString("msgCreateOn", LocalResourceFile)
            Else
                lblUserName.Text = GetString("msgCreateFrom", LocalResourceFile) & " "
                lblUserName.Text += CType(e.Item.DataItem, EntryInfo).UserName
                lblUserName.Text += " " & GetString("msgCreateOn", LocalResourceFile)
            End If
            lblUserName.Visible = True
        End If

        lblPublished.Visible = Not CType(e.Item.DataItem, EntryInfo).Published
        lblPublishDate.Text = Utility.FormatDate(CType(e.Item.DataItem, EntryInfo).AddedDate, oBlog.Culture, oBlog.DateFormat, oBlog.TimeZone)

        Dim SummaryLimit As Integer = 0
        If m_bSearchDisplay Then
            SummaryLimit = BlogSettings.SearchSummaryMaxLength
        Else
            SummaryLimit = BlogSettings.SummaryMaxLength
        End If

        ' Don Worthley - 04/21/2008 - Updated the following logic to account for both the description
        '   and the entry text.  Previously, the code was just looking at the entry text when truncation
        '   was being performed.  The updated code will use the description if one exists and use the 
        '   entry if no description exists.
        ' DW - Updated again to make the truncation of the summary optional based on the EnforceSummaryTruncation
        '       blog setting.
        Dim ActualSummary As String = Server.HtmlDecode(CType(e.Item.DataItem, EntryInfo).Description)
        Dim Entry As String = Server.HtmlDecode(CType(e.Item.DataItem, EntryInfo).Entry)
        Dim GeneratedSummary As String = CType(IIf(ActualSummary Is Nothing, String.Empty, ActualSummary), String)
        Dim EnforceSummaryTruncation As Boolean = BlogSettings.EnforceSummaryTruncation
        If GeneratedSummary = String.Empty Then
            GeneratedSummary = Entry
            ' We'll set the EnforceSummaryTruncation flag to true since generated summaries should
            ' always be truncated.
            EnforceSummaryTruncation = True
        End If
        If SummaryLimit = 0 Then
            If ActualSummary.Length > 0 And Entry.Length > 0 Then
                'lnkReadMore.Visible = True
                divBlogReadMore.Visible = True
            Else
                'lnkReadMore.Visible = False
                divBlogReadMore.Visible = False
            End If
            lblDescription.Text = GeneratedSummary
        Else
            If GeneratedSummary.Length > SummaryLimit Then
                'We need to truncate, but only EnforceSummaryTruncation is True
                If EnforceSummaryTruncation Then
                    lblDescription.Text = Utility.CleanHTML(GeneratedSummary, SummaryLimit)
                Else
                    lblDescription.Text = GeneratedSummary
                End If
                'lnkReadMore.Visible = True
                divBlogReadMore.Visible = True
            Else
                lblDescription.Text = GeneratedSummary
                'Only show the Read More link if there was a Description and an Entry
                If ActualSummary.Length > 0 And Entry.Length > 0 Then
                    'lnkReadMore.Visible = True
                    divBlogReadMore.Visible = True
                Else
                    'lnkReadMore.Visible = False
                    divBlogReadMore.Visible = False
                End If
            End If
        End If

        If ((oBlog.AllowComments Or CType(e.Item.DataItem, EntryInfo).AllowComments) And CType(IIf(Me.UserId = -1, oBlog.AllowAnonymous, True), Boolean)) Then
            'If ((oBlog.AllowComments Or CType(e.Item.DataItem, EntryInfo).AllowComments)) Then
            lnkComments.Visible = True
            lnkComments.Text = String.Format(GetString("lnkComments", LocalResourceFile), CType(e.Item.DataItem, EntryInfo).CommentCount)
        Else
            lnkComments.Visible = False
        End If

        If oBlog.ParentBlogID = -1 Then
            imgBlogParentSeparator.Visible = False
            lnkChildBlog.Visible = False
            If Not m_oBlog Is Nothing Then
                If m_oBlog.BlogID = oBlog.BlogID Then
                    lnkParentBlog.Visible = False
                    imgBlogParentSeparator.Visible = False
                    lnkChildBlog.Visible = False
                End If
            End If
            If lnkParentBlog.Visible Then
                lnkParentBlog.Text = oBlog.Title
                lnkParentBlog.NavigateUrl = NavigateURL(Me.TabId, "", "&BlogID=" & oBlog.BlogID.ToString())
            End If
        Else
            If Not m_oBlog Is Nothing Then
                If m_oBlog.BlogID = oBlog.BlogID Then
                    lnkParentBlog.Visible = False
                    imgBlogParentSeparator.Visible = False
                    lnkChildBlog.Visible = False
                ElseIf m_oBlog.BlogID = oBlog.ParentBlogID Then
                    lnkParentBlog.Visible = False
                    imgBlogParentSeparator.Visible = False
                End If
            End If
            If lnkParentBlog.Visible Then
                Dim oParentBlog As BlogInfo = m_oBlogController.GetBlog(oBlog.ParentBlogID)
                lnkParentBlog.Text = oParentBlog.Title
                lnkParentBlog.NavigateUrl = NavigateURL(Me.TabId, "", "&BlogID=" & oParentBlog.BlogID.ToString())
                imgBlogParentSeparator.Visible = True
                oParentBlog = Nothing
            End If
            If lnkChildBlog.Visible Then
                lnkChildBlog.Text = oBlog.Title
                lnkChildBlog.NavigateUrl = NavigateURL(Me.TabId, "", "&BlogID=" & oBlog.BlogID.ToString() & "&ParentBlogID=" & oBlog.ParentBlogID.ToString())
                lnkChildBlog.Visible = True
            End If
        End If
        oBlog = Nothing
    End Sub

    Protected Sub lstBlogView_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataListCommandEventArgs) Handles lstBlogView.ItemCommand
        ' DW - 04/22/2008 - Added to allow the use of the new BlogNavigateURL method

        Dim EntryID As Integer = Int32.Parse(CType(e.CommandArgument, String))
        Dim EntryController As New EntryController
        Dim EntryInfo As EntryInfo = EntryController.GetEntry(EntryID, PortalId)

        Select Case e.CommandName
            Case "Entry"
                ' DW - 11/12/2008 - Replaced with Permalink
                Response.Redirect(EntryInfo.PermaLink)
            Case "Comments"
                If DotNetNuke.Entities.Host.HostSettings.GetHostSetting("UseFriendlyUrls") = "Y" Then
                    ' DW - 11/12/2008 - Replaced with Permalink
                    Response.Redirect(EntryInfo.PermaLink & "#Comments")
                Else
                    ' DW - 11/12/2008 - Replaced with Permalink
                    Response.Redirect(EntryInfo.PermaLink & "#Comments")
                End If
            Case "User"
        End Select
    End Sub

    Protected Sub lstSearchResults_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataListCommandEventArgs) Handles lstSearchResults.ItemCommand
        Select Case e.CommandName
            Case "Entry"
                Dim EntryID As Integer = Int32.Parse(CType(e.CommandArgument, String))
                ' DW - 04/22/2008 - Added to allow the use of the new BlogNavigateURL method
                Dim EntryController As New EntryController
                Dim EntryInfo As EntryInfo = EntryController.GetEntry(EntryID, PortalId)
                ' DW - 11/12/2008 - Replaced with Permalink
                Response.Redirect(EntryInfo.PermaLink)
            Case "Blog"
                Dim BlogID As Integer = Int32.Parse(CType(e.CommandArgument, String))
                Response.Redirect(NavigateURL(Me.TabId, "", "BlogID=" & BlogID))
            Case "User"

        End Select
    End Sub

    Protected Sub lstSearchResults_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataListItemEventArgs) Handles lstSearchResults.ItemDataBound
        Dim lblEntryDate As System.Web.UI.WebControls.Label = CType(e.Item.FindControl("lblEntryDate"), System.Web.UI.WebControls.Label)
        Dim lblEntryUserName As System.Web.UI.WebControls.Label = CType(e.Item.FindControl("lblEntryUserName"), System.Web.UI.WebControls.Label)
        Dim lnkParentBlog As System.Web.UI.WebControls.HyperLink = CType(e.Item.FindControl("lnkParentBlogSearch"), System.Web.UI.WebControls.HyperLink)
        Dim imgBlogParentSeparator As System.Web.UI.WebControls.Image = CType(e.Item.FindControl("imgBlogParentSeparatorSearch"), System.Web.UI.WebControls.Image)
        Dim lnkChildBlog As System.Web.UI.WebControls.HyperLink = CType(e.Item.FindControl("lnkChildBlogSearch"), System.Web.UI.WebControls.HyperLink)
        Dim lnkEntryTitle As System.Web.UI.WebControls.HyperLink = CType(e.Item.FindControl("lnkEntryTitle"), System.Web.UI.WebControls.HyperLink)
        Dim lblItemSummary As Label = CType(e.Item.FindControl("lblItemSummary"), Label)

        Dim oBlog As BlogInfo

        If m_oBlog Is Nothing Then
            oBlog = m_oBlogController.GetBlog(CType(e.Item.DataItem, SearchResult).BlogID)
        Else
            If m_oBlog.BlogID <> CType(e.Item.DataItem, SearchResult).BlogID Then
                oBlog = m_oBlogController.GetBlog(CType(e.Item.DataItem, SearchResult).BlogID)
            Else
                oBlog = m_oBlog
            End If
        End If

        ' Link The Entry
        Dim oSearchResult As SearchResult = CType(e.Item.DataItem, SearchResult)
        ' DW - 11/12/2008 - Replaced with Permalink
        lnkEntryTitle.NavigateUrl = oSearchResult.PermaLink

        ' Display the proper UserName
        If CType(e.Item.DataItem, SearchResult).UserName.Length > 0 Then
            If oBlog.ShowFullName Then lblEntryUserName.Text = CType(e.Item.DataItem, SearchResult).UserFullName
            lblEntryUserName.Visible = True
        End If

        lblEntryDate.Text = Utility.FormatDate(CType(e.Item.DataItem, SearchResult).AddedDate, oBlog.Culture, oBlog.DateFormat, oBlog.TimeZone)

        'Setup blog path
        If oBlog.ParentBlogID = -1 Then
            If Not IsNothing(imgBlogParentSeparator) Then
                imgBlogParentSeparator.Visible = False
            End If
            If Not IsNothing(lnkChildBlog) Then
                lnkChildBlog.Visible = False
            End If
            If Not IsNothing(lnkParentBlog) Then
                lnkParentBlog.Text = oBlog.Title
            End If
            If Not IsNothing(lnkParentBlog) Then
                lnkParentBlog.NavigateUrl = NavigateURL(Me.TabId, "", "&BlogID=" & oBlog.BlogID)
            End If
        Else
            Dim oParentBlog As BlogInfo = m_oBlogController.GetBlog(oBlog.ParentBlogID)
            lnkParentBlog.Text = oParentBlog.Title
            lnkParentBlog.NavigateUrl = NavigateURL(Me.TabId, "", "&BlogID=" & oParentBlog.BlogID.ToString())
            lnkChildBlog.Text = oBlog.Title
            lnkChildBlog.NavigateUrl = NavigateURL(Me.TabId, "", "&BlogID=" & oBlog.BlogID.ToString() & "&ParentBlogID=" & oParentBlog.BlogID.ToString())
            imgBlogParentSeparator.Visible = True
            lnkChildBlog.Visible = True
            oParentBlog = Nothing
        End If
        oBlog = Nothing


        Dim SummaryLimit As Integer = 0
        SummaryLimit = BlogSettings.SearchSummaryMaxLength
        Dim Summary As String = HttpUtility.HtmlDecode(oSearchResult.Summary)

        If SummaryLimit = 0 OrElse Summary.Length <= SummaryLimit Then
            lblItemSummary.Text = Summary
        Else
            lblItemSummary.Text = Utility.CleanHTML(Summary, SummaryLimit)
        End If

    End Sub

#End Region

End Class