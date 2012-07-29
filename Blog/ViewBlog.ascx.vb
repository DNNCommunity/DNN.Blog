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
Imports DotNetNuke.Web.Client.ClientResourceManagement
Imports DotNetNuke.Modules.Blog.Components.Business
Imports DotNetNuke.Modules.Blog.Components.Controllers
Imports DotNetNuke.Modules.Blog.Components.Common
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization.Localization
Imports DotNetNuke.Modules.Blog.Components.Entities
Imports DotNetNuke.Framework

Partial Public Class ViewBlog
    Inherits BlogModuleBase

#Region "Private Members"

    Private _mSSearchString As String
    Private _mSSearchType As String = "Keyword"
    Private ReadOnly _mOBlogController As New BlogController
    Private _mOBlog As BlogInfo
    Private _mDBlogDate As Date = Date.UtcNow
    Private _mDBlogDateType As String
    Private _mBSearchDisplay As Boolean = False
    Private _mPersonalBlogId As Integer
    Private _mSeoFriendlyUrl As Boolean

    Private ReadOnly Property CurrentPage() As Integer
        Get
            Dim _page As Integer = 1
            If Request.Params("page") IsNot Nothing Then
                _page = Convert.ToInt32(Request.Params("page"))
            End If
            Return _page
        End Get
    End Property

    Private ReadOnly Property Category() As Integer
        Get
            Dim _category As Integer = -1
            If Request.Params("catid") IsNot Nothing Then
                _category = Convert.ToInt32(Request.Params("catid"))
            End If
            Return _category
        End Get
    End Property

    Private ReadOnly Property Tag() As Integer
        Get
            Dim _tag As Integer = -1
            If Request.Params("tagid") IsNot Nothing Then
                _tag = Convert.ToInt32(Request.Params("tagid"))
            End If
            Return _tag
        End Get
    End Property

#End Region

#Region "Event Handlers"

    Protected Overloads Sub Page_Init(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Init
        jQuery.RequestUIRegistration()
        ClientResourceManager.RegisterScript(Page, TemplateSourceDirectory + "/js/jquery.qatooltip.js")
        ClientResourceManager.RegisterScript(Page, "~/Resources/Shared/Scripts/jquery/jquery.hoverIntent.min.js")

        _mOBlog = _mOBlogController.GetBlogFromContext()
        _mPersonalBlogId = BlogSettings.PageBlogs

        If _mPersonalBlogId <> -1 And _mOBlog Is Nothing Then
            Dim objBlog As New BlogController
            _mOBlog = objBlog.GetBlog(_mPersonalBlogId)
        End If
        If Not Request.Params("Search") Is Nothing Then
            _mSSearchString = Request.Params("Search")
            _mBSearchDisplay = True
            If _mOBlog Is Nothing Then
                ModuleConfiguration.ModuleTitle = GetString("msgSearchResults", LocalResourceFile)
            Else
                ModuleConfiguration.ModuleTitle = GetString("msgSearchResultsFor", LocalResourceFile) & " " & _mOBlog.Title
            End If
        End If
        MyActions.Add(GetNextActionID, GetString("msgModuleOptions", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "", EditUrl("", "", "Module_Options"), False, Security.SecurityAccessLevel.Admin, True, False)
    End Sub

    Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            _mSeoFriendlyUrl = BlogSettings.ShowSeoFriendlyUrl
            Dim objEntries As New EntryController
            Dim list As List(Of EntryInfo)

            If Not Request.Params("BlogDate") Is Nothing Then
                _mDBlogDate = CType(Date.Parse(Request.Params("BlogDate")), Date)
                _mDBlogDate = TimeZoneInfo.ConvertTimeToUtc(_mDBlogDate, UiTimeZone)

                If Not Request.Params("DateType") Is Nothing Then
                    _mDBlogDateType = Request.Params("DateType")
                End If
            End If
            If Not Request.Params("SearchType") Is Nothing Then
                _mSSearchType = Request.Params("SearchType")
            End If

            Dim prevPage As String = NavigateURL()
            Dim nextPage As String = NavigateURL()
            Dim pageUrl As String = NavigateURL()

            If Not Page.IsPostBack Then
                Dim HasValue As Boolean = False

                If _mBSearchDisplay Then
                    If _mSSearchType = "Phrase" Then
                        If _mOBlog Is Nothing Then
                            list = New SearchController().SearchByPhraseByPortal(PortalId, _mSSearchString, Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()), Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()))
                        Else
                            list = New SearchController().SearchByPhraseByBlog(_mOBlog.BlogID, _mSSearchString, Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()), Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()))
                        End If
                    Else
                        If _mOBlog Is Nothing Then
                            list = New SearchController().SearchByKeywordByPortal(PortalId, _mSSearchString, Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()), Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()))
                        Else
                            list = New SearchController().SearchByKeywordByBlog(_mOBlog.BlogID, _mSSearchString, Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()), Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()))
                        End If
                    End If
                Else
                    Dim pageTitle As String = BasePage.Title
                    Dim keyWords As String = BasePage.KeyWords
                    Dim pageDescription As String = BasePage.Description
                    Dim pageAuthor As String = BasePage.Author

                    If Category < 1 Then
                        If Tag < 1 Then
                            If _mOBlog Is Nothing Then
                                ' most recent approved blog list (default view), no category/tag specified

                                list = objEntries.GetEntriesByPortal(PortalId, _mDBlogDate, _mDBlogDateType, BlogSettings.RecentEntriesMax, CurrentPage, Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()), Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()))

                                pnlBlogInfo.Visible = False
                                If Not lnkRecentRss Is Nothing Then
                                    lnkRecentRss.NavigateUrl = NavigateURL(ModuleContext.TabId, "", "rssid=0")
                                End If

                                ' No paging or meta updates for this view.
                            Else
                                ' Specific blog view
                                pnlBlogInfo.Visible = True
                                If _mOBlog.ShowFullName Then
                                    hlAuthor.Text = _mOBlog.UserFullName
                                Else
                                    hlAuthor.Text = _mOBlog.UserName
                                End If

                                Dim objAuthor As Entities.Users.UserInfo = Entities.Users.UserController.GetUserById(ModuleContext.PortalId, _mOBlog.UserID)
                                dbiUser.ImageUrl = objAuthor.Profile.PhotoURL
                                hlAuthor.NavigateUrl = UserProfileURL(_mOBlog.UserID)
                                imgAuthorLink.NavigateUrl = UserProfileURL(_mOBlog.UserID)

                                Dim objSecurity As ModuleSecurity = New ModuleSecurity(ModuleContext.ModuleId, ModuleContext.TabId)
                                Dim isOwner As Boolean
                                isOwner = _mOBlog.UserID = ModuleContext.PortalSettings.UserId

                                litBlogDescription.Text = _mOBlog.Description
                                list = objEntries.GetEntriesByBlog(_mOBlog.BlogID, _mDBlogDate, BlogSettings.RecentEntriesMax, CurrentPage, objSecurity.CanAddEntry(isOwner, _mOBlog.AuthorMode), objSecurity.CanAddEntry(isOwner, _mOBlog.AuthorMode))

                                prevPage = Links.ViewEntriesByBlog(ModuleContext, _mOBlog.BlogID, CurrentPage - 1)
                                nextPage = Links.ViewEntriesByBlog(ModuleContext, _mOBlog.BlogID, CurrentPage + 1)
                                pageUrl = Links.ViewEntriesByBlog(ModuleContext, _mOBlog.BlogID, CurrentPage)

                                pageTitle = _mOBlog.Title
                                pageDescription = _mOBlog.Description
                                pageAuthor = _mOBlog.UserFullName
                            End If
                        Else
                            list = objEntries.GetEntriesByTerm(PortalId, _mDBlogDate, Tag, BlogSettings.RecentEntriesMax, CurrentPage, Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()), Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()))
                            pnlBlogInfo.Visible = False
                            If Not lnkRecentRss Is Nothing Then
                                lnkRecentRss.NavigateUrl = Links.RssByTag(ModuleContext, Tag)
                            End If

                            prevPage = Links.ViewEntriesByTag(ModuleContext, Tag, CurrentPage - 1)
                            nextPage = Links.ViewEntriesByTag(ModuleContext, Tag, CurrentPage + 1)
                            pageUrl = Links.ViewEntriesByTag(ModuleContext, Tag, CurrentPage)
                            ' TODO: Page Meta
                        End If
                    Else
                        ' category specific search
                        list = objEntries.GetEntriesByTerm(PortalId, _mDBlogDate, Category, BlogSettings.RecentEntriesMax, CurrentPage, Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()), Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()))
                        pnlBlogInfo.Visible = False
                        If Not lnkRecentRss Is Nothing Then
                            lnkRecentRss.NavigateUrl = Links.RssByCategory(ModuleContext, Category)
                        End If

                        prevPage = Links.ViewEntriesByCategory(ModuleContext, Category, CurrentPage - 1)
                        nextPage = Links.ViewEntriesByCategory(ModuleContext, Category, CurrentPage + 1)
                        pageUrl = Links.ViewEntriesByCategory(ModuleContext, Category, CurrentPage)
                        ' TODO: Page Meta
                    End If

                    lstBlogView.DataSource = list
                    lstBlogView.DataBind()

                    HasValue = (lstBlogView.Items.Count > 0)

                    If HasValue Then
                        Dim TotalRecords As Integer = list(0).TotalRecords
                        Dim totalPages As Double = Convert.ToDouble(CDbl(TotalRecords) / BlogSettings.RecentEntriesMax)

                        If (totalPages > 1) AndAlso (totalPages > CurrentPage) AndAlso (nextPage <> NavigateURL()) Then
                            hlPagerNext.Visible = True
                            hlPagerNext.NavigateUrl = nextPage
                        End If
                    End If

                    If (CurrentPage > 1) AndAlso HasValue AndAlso (prevPage <> NavigateURL()) Then
                        hlPagerPrev.Visible = True
                        hlPagerPrev.NavigateUrl = prevPage
                    End If

                    ' TODO: Page Meta
                    Utility.SetPageMetaAndOpenGraph(BasePage, ModuleContext, pageTitle, pageDescription, keyWords, pageUrl)
                End If ' search display or not

                If Not _mOBlog Is Nothing Then
                    If _mOBlog.Syndicated Then
                        lnkRSS.NavigateUrl = Links.RSSByBlog(ModuleContext, _mOBlog.BlogID)
                        lnkRSS.Visible = True
                    End If
                End If

                ' add rss discovery link
                Dim ph As PlaceHolder = CType(BasePage.FindControl("phDNNHead"), PlaceHolder)
                If Not ph Is Nothing Then
                    If _mOBlog Is Nothing AndAlso pnlBlogRss.Visible Then
                        Dim objLink As New HtmlGenericControl("LINK")
                        objLink.Attributes("title") = "RSS"
                        objLink.Attributes("rel") = "alternate"
                        objLink.Attributes("type") = "application/rss+xml"
                        objLink.Attributes("href") = Links.RSSAggregated(ModuleContext)
                        ph.Controls.Add(objLink)
                    ElseIf Not _mOBlog Is Nothing AndAlso _mOBlog.Syndicated Then
                        Dim objLink As New HtmlGenericControl("LINK")
                        objLink.Attributes("title") = "RSS"
                        objLink.Attributes("rel") = "alternate"
                        objLink.Attributes("type") = "application/rss+xml"
                        objLink.Attributes("href") = Links.RSSByBlog(ModuleContext, _mOBlog.BlogID)
                        ph.Controls.Add(objLink)
                    End If
                End If

                If (HasValue = False) Then
                    Dim message As String = GetString("msgNoResult", LocalResourceFile)

                    If _mBSearchDisplay Then
                        message = GetString("msgNoSearchResult", LocalResourceFile)
                    ElseIf _mDBlogDate <> Date.MinValue Then
                        message = GetString("msgNoPeriodResult", LocalResourceFile)
                    ElseIf Not _mOBlog Is Nothing Then
                        message = GetString("msgNoBlogResult", LocalResourceFile)
                    End If

                    litNoRecords.Text = message
                    pnlNoRecords.Visible = True
                End If
            End If
        Catch exc As Exception
            ProcessModuleLoadException(Me, exc)
        End Try
    End Sub

    Protected Sub lstBlogView_ItemDataBound(ByVal sender As Object, ByVal e As DataListItemEventArgs) Handles lstBlogView.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item Then
            Dim litAuthor As Literal = CType(e.Item.FindControl("litAuthor"), Literal)
            Dim lblDescription As Literal = CType(e.Item.FindControl("litDescription"), Literal)
            Dim lnkComments As HyperLink = CType(e.Item.FindControl("lnkComments"), HyperLink)
            Dim pnNotPublished As Panel = CType(e.Item.FindControl("pnNotPublished"), Panel)
            Dim lblPublishDate As Label = CType(e.Item.FindControl("lblPublishDate"), Label)
            Dim lnkParentBlog As HyperLink = CType(e.Item.FindControl("lnkParentBlog"), HyperLink)
            Dim imgBlogParentSeparator As Image = CType(e.Item.FindControl("imgBlogParentSeparator"), Image)
            Dim lnkChildBlog As HyperLink = CType(e.Item.FindControl("lnkChildBlog"), HyperLink)
            Dim lnkEntry As HyperLink = CType(e.Item.FindControl("lnkEntry"), HyperLink)
            Dim divBlogReadMore As HtmlGenericControl = CType(e.Item.FindControl("divBlogReadMore"), HtmlGenericControl)
            Dim litCategories As Literal = CType(e.Item.FindControl("litCategories"), Literal)
            Dim tagControl As Tags = DirectCast(e.Item.FindControl("dbaTag"), Tags)
            Dim oBlog As BlogInfo

            If _mOBlog Is Nothing Then
                oBlog = _mOBlogController.GetBlog(CType(CType(e.Item.DataItem, EntryInfo).BlogID, Integer))
            Else
                If _mOBlog.BlogID <> CType(CType(e.Item.DataItem, EntryInfo).BlogID, Integer) Then
                    oBlog = _mOBlogController.GetBlog(CType(CType(e.Item.DataItem, EntryInfo).BlogID, Integer))
                Else
                    oBlog = _mOBlog
                End If
            End If

            ' Added this in order to have the Edit Entry Link on all the pages.
            Dim m_oEntry As EntryInfo = CType(e.Item.DataItem, EntryInfo)
            Dim lnkEditEntry As HyperLink
            Dim imgEdit As Image
            lnkEditEntry = CType(e.Item.FindControl("lnkEditEntry"), HyperLink)
            imgEdit = CType(e.Item.FindControl("imgEdit"), Image)

            If Not m_oEntry Is Nothing Then
                Dim objSecurity As ModuleSecurity = New ModuleSecurity(ModuleContext.ModuleId, ModuleContext.TabId)

                Dim isOwner As Boolean
                isOwner = oBlog.UserID = ModuleContext.PortalSettings.UserId

                If objSecurity.CanAddEntry(isOwner, oBlog.AuthorMode) AndAlso Not lnkEditEntry Is Nothing Then
                    lnkEditEntry.Visible = True
                    lnkEditEntry.NavigateUrl = EditUrl("EntryID", CType(e.Item.DataItem, EntryInfo).EntryID.ToString(), "Edit_Entry")
                Else
                    lnkEditEntry.Visible = False
                End If

                Dim hlPermaLink As HyperLink = CType(e.Item.FindControl("hlPermaLink"), HyperLink)
                hlPermaLink.NavigateUrl = m_oEntry.PermaLink
                hlPermaLink.Text = GetString("lnkPermaLink", LocalResourceFile)
                hlPermaLink.Visible = (m_oEntry.PermaLink <> Utility.BlogNavigateURL(TabId, PortalId, m_oEntry.EntryID, m_oEntry.Title, BlogSettings.ShowSeoFriendlyUrl))

                Dim hlMore As HyperLink = CType(e.Item.FindControl("hlMore"), HyperLink)
                hlMore.NavigateUrl = Utility.BlogNavigateURL(TabId, PortalId, m_oEntry.EntryID, m_oEntry.Title, BlogSettings.ShowSeoFriendlyUrl)
                hlMore.Text = GetString("lnkReadMore", LocalResourceFile)
                hlMore.Visible = (m_oEntry.PermaLink <> Utility.BlogNavigateURL(TabId, PortalId, m_oEntry.EntryID, m_oEntry.Title, BlogSettings.ShowSeoFriendlyUrl))

                Dim Categories As String = ""
                Dim i As Integer = 0
                Dim colCategories As List(Of TermInfo) = m_oEntry.EntryTerms(BlogSettings.VocabularyId)

                For Each objTerm As TermInfo In colCategories
                    Categories += "<a href='" + Links.ViewEntriesByCategory(ModuleContext, objTerm.TermId, 1) + "'>" + objTerm.Name + "</a>"
                    i += 1
                    If i <= (colCategories.Count - 1) Then
                        Categories += ", "
                    End If
                Next

                litCategories.Text = Categories

                tagControl.ModContext = ModuleContext
                tagControl.DataSource = m_oEntry.EntryTerms(1)
                tagControl.DataBind()
            End If

            lnkEntry.NavigateUrl = m_oEntry.PermaLink
            lnkComments.NavigateUrl = m_oEntry.PermaLink & "#Comments"

            'DR-04/24/2009-BLG-9712
            lnkEntry.Attributes.Add("rel", "bookmark")

            If CType(e.Item.DataItem, EntryInfo).UserName.Length > 0 Then
                Dim userProfile As String = UserProfileURL(m_oEntry.UserID)

                If oBlog.ShowFullName Then
                    litAuthor.Text = GetString("msgCreateFrom", LocalResourceFile) & " "
                    litAuthor.Text += "<a href='" + userProfile + "'>"
                    litAuthor.Text += CType(e.Item.DataItem, EntryInfo).UserFullName
                    litAuthor.Text += "</a>"
                    litAuthor.Text += " " & GetString("msgCreateOn", LocalResourceFile)
                Else
                    litAuthor.Text = GetString("msgCreateFrom", LocalResourceFile) & " "
                    litAuthor.Text += "<a href='" + userProfile + "'>"
                    litAuthor.Text += CType(e.Item.DataItem, EntryInfo).UserName
                    litAuthor.Text += "</a>"
                    litAuthor.Text += " " & GetString("msgCreateOn", LocalResourceFile)
                End If
                litAuthor.Visible = True
            End If

            pnNotPublished.Visible = Not CType(e.Item.DataItem, EntryInfo).Published

            lblPublishDate.Text = Utility.DateFromUtc(m_oEntry.AddedDate, UiTimeZone).ToString("f")

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
            If BlogSettings.SummaryMaxLength = 0 Then
                If ActualSummary.Length > 0 And Entry.Length > 0 Then
                    'lnkReadMore.Visible = True
                    divBlogReadMore.Visible = True
                Else
                    'lnkReadMore.Visible = False
                    divBlogReadMore.Visible = False
                End If
                lblDescription.Text = GeneratedSummary
            Else
                If GeneratedSummary.Length > BlogSettings.SummaryMaxLength Then
                    'We need to truncate, but only EnforceSummaryTruncation is True
                    If EnforceSummaryTruncation Then
                        lblDescription.Text = Utility.CleanHTML(GeneratedSummary, BlogSettings.SummaryMaxLength)
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

            If ((oBlog.AllowComments Or CType(e.Item.DataItem, EntryInfo).AllowComments)) Then
                ' CP: Removed from above, we should always show the comment link as long as we allow them (in other words, who cares if they are logged in) - CodePlex = 22459
                'And CType(IIf(Me.UserId = -1, oBlog.AllowAnonymous, True), Boolean) 
                lnkComments.Visible = True
                lnkComments.Text = String.Format(GetString("lnkComments", LocalResourceFile), CType(e.Item.DataItem, EntryInfo).CommentCount)
            Else
                lnkComments.Visible = False
            End If

            If oBlog.ParentBlogID = -1 Then
                imgBlogParentSeparator.Visible = False
                lnkChildBlog.Visible = False
                If Not _mOBlog Is Nothing Then
                    If _mOBlog.BlogID = oBlog.BlogID Then
                        lnkParentBlog.Visible = False
                        imgBlogParentSeparator.Visible = False
                        lnkChildBlog.Visible = False
                    End If
                End If
                If lnkParentBlog.Visible Then
                    lnkParentBlog.Text = oBlog.Title
                    lnkParentBlog.NavigateUrl = Links.ViewBlog(ModuleContext, oBlog.BlogID)
                End If
            Else
                If Not _mOBlog Is Nothing Then
                    If _mOBlog.BlogID = oBlog.BlogID Then
                        lnkParentBlog.Visible = False
                        imgBlogParentSeparator.Visible = False
                        lnkChildBlog.Visible = False
                    ElseIf _mOBlog.BlogID = oBlog.ParentBlogID Then
                        lnkParentBlog.Visible = False
                        imgBlogParentSeparator.Visible = False
                    End If
                End If
                If lnkParentBlog.Visible Then
                    Dim oParentBlog As BlogInfo = _mOBlogController.GetBlog(oBlog.ParentBlogID)
                    lnkParentBlog.Text = oParentBlog.Title
                    lnkParentBlog.NavigateUrl = Links.ViewBlog(ModuleContext, oParentBlog.BlogID)
                    imgBlogParentSeparator.Visible = True
                    oParentBlog = Nothing
                End If
                If lnkChildBlog.Visible Then
                    lnkChildBlog.Text = oBlog.Title
                    lnkChildBlog.NavigateUrl = Links.ViewChildBlog(ModuleContext, oBlog.BlogID, oBlog.ParentBlogID)
                    lnkChildBlog.Visible = True
                End If
            End If
            oBlog = Nothing
        End If
    End Sub

    Protected Sub lstBlogView_ItemCommand(ByVal source As Object, ByVal e As DataListCommandEventArgs) Handles lstBlogView.ItemCommand
        Dim entryId As Integer = Int32.Parse(CType(e.CommandArgument, String))
        Dim entryController As New EntryController
        Dim entryInfo As EntryInfo = entryController.GetEntry(entryId, PortalId)

        Select Case e.CommandName
            Case "Entry"
                Response.Redirect(entryInfo.PermaLink)
            Case "Comments"
                Response.Redirect(entryInfo.PermaLink & "#Comments")
            Case "User"
        End Select
    End Sub

#End Region

End Class