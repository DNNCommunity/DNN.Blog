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
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Modules.Blog.Components.Entities
Imports DotNetNuke.Framework

Partial Public Class ViewBlog
    Inherits BlogModuleBase

#Region "Private Members"

    Private m_sSearchString As String
    Private m_sSearchType As String = "Keyword"
    Private m_oBlogController As New BlogController
    Private m_oBlog As BlogInfo
    Private m_dBlogDate As Date = Date.UtcNow
    Private m_dBlogDateType As String
    Private m_bSearchDisplay As Boolean = False
    Private m_PersonalBlogID As Integer
    Private m_SeoFriendlyUrl As Boolean

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

    Protected Overloads Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        jQuery.RequestUIRegistration()
        ClientResourceManager.RegisterScript(Page, TemplateSourceDirectory + "/js/jquery.qatooltip.js")
        ClientResourceManager.RegisterScript(Page, "~/Resources/Shared/Scripts/jquery/jquery.hoverIntent.min.js")

        m_oBlog = m_oBlogController.GetBlogFromContext()
        m_PersonalBlogID = BlogSettings.PageBlogs

        If m_PersonalBlogID <> -1 And m_oBlog Is Nothing Then
            Dim objBlog As New BlogController
            m_oBlog = objBlog.GetBlog(m_PersonalBlogID)
        End If
        If Not Request.Params("Search") Is Nothing Then
            m_sSearchString = Request.Params("Search")
            m_bSearchDisplay = True
            If m_oBlog Is Nothing Then
                ModuleConfiguration.ModuleTitle = GetString("msgSearchResults", LocalResourceFile)
            Else
                ModuleConfiguration.ModuleTitle = GetString("msgSearchResultsFor", LocalResourceFile) & " " & m_oBlog.Title
            End If
        End If
        MyActions.Add(GetNextActionID, GetString("msgModuleOptions", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "", EditUrl("", "", "Module_Options"), False, DotNetNuke.Security.SecurityAccessLevel.Admin, True, False)
    End Sub

    Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            m_SeoFriendlyUrl = BlogSettings.ShowSeoFriendlyUrl
            Dim objEntries As New EntryController
            Dim list As List(Of EntryInfo)

            If Not Request.Params("BlogDate") Is Nothing Then
                m_dBlogDate = CType(Date.Parse(Request.Params("BlogDate")), Date)

                If Not Request.Params("DateType") Is Nothing Then
                    m_dBlogDateType = Request.Params("DateType")
                End If
            End If
            If Not Request.Params("SearchType") Is Nothing Then
                m_sSearchType = Request.Params("SearchType")
            End If

            Dim prevPage As String = NavigateURL()
            Dim nextPage As String = NavigateURL()
            Dim pageUrl As String = NavigateURL()

            If Not Page.IsPostBack Then
                Dim HasValue As Boolean = False

                If m_bSearchDisplay Then
                    If m_sSearchType = "Phrase" Then
                        If m_oBlog Is Nothing Then
                            list = New SearchController().SearchByPhraseByPortal(Me.PortalId, m_sSearchString, DotNetNuke.Security.PortalSecurity.IsInRole(Me.PortalSettings.AdministratorRoleId.ToString()), DotNetNuke.Security.PortalSecurity.IsInRole(Me.PortalSettings.AdministratorRoleId.ToString()))
                        Else
                            list = New SearchController().SearchByPhraseByBlog(m_oBlog.BlogID, m_sSearchString, DotNetNuke.Security.PortalSecurity.IsInRole(Me.PortalSettings.AdministratorRoleId.ToString()), DotNetNuke.Security.PortalSecurity.IsInRole(Me.PortalSettings.AdministratorRoleId.ToString()))
                        End If
                    Else
                        If m_oBlog Is Nothing Then
                            list = New SearchController().SearchByKeywordByPortal(Me.PortalId, m_sSearchString, DotNetNuke.Security.PortalSecurity.IsInRole(Me.PortalSettings.AdministratorRoleId.ToString()), DotNetNuke.Security.PortalSecurity.IsInRole(Me.PortalSettings.AdministratorRoleId.ToString()))
                        Else
                            list = New SearchController().SearchByKeywordByBlog(m_oBlog.BlogID, m_sSearchString, DotNetNuke.Security.PortalSecurity.IsInRole(Me.PortalSettings.AdministratorRoleId.ToString()), DotNetNuke.Security.PortalSecurity.IsInRole(Me.PortalSettings.AdministratorRoleId.ToString()))
                        End If
                    End If
                Else
                    Dim pageTitle As String = Me.BasePage.Title
                    Dim keyWords As String = Me.BasePage.KeyWords
                    Dim pageDescription As String = Me.BasePage.Description
                    Dim pageAuthor As String = Me.BasePage.Author

                    If Category < 1 Then
                        If Tag < 1 Then
                            If m_oBlog Is Nothing Then
                                ' most recent approved blog list (default view), no category/tag specified

                                list = objEntries.GetEntriesByPortal(Me.PortalId, m_dBlogDate, m_dBlogDateType, BlogSettings.RecentEntriesMax, CurrentPage, DotNetNuke.Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()), DotNetNuke.Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()))

                                pnlBlogInfo.Visible = False
                                If Not lnkRecentRss Is Nothing Then
                                    lnkRecentRss.NavigateUrl = NavigateURL(Me.TabId, "", "rssid=0")
                                End If

                                ' No paging or meta updates for this view.
                            Else
                                ' Specific blog view
                                pnlBlogInfo.Visible = True
                                If m_oBlog.ShowFullName Then
                                    hlAuthor.Text = m_oBlog.UserFullName
                                Else
                                    hlAuthor.Text = m_oBlog.UserName
                                End If

                                Dim objAuthor As Entities.Users.UserInfo = Entities.Users.UserController.GetUserById(ModuleContext.PortalId, m_oBlog.UserID)
                                dbiUser.ImageUrl = objAuthor.Profile.PhotoURL
                                hlAuthor.NavigateUrl = UserProfileURL(m_oBlog.UserID)
                                imgAuthorLink.NavigateUrl = UserProfileURL(m_oBlog.UserID)

                                Dim objSecurity As ModuleSecurity = New ModuleSecurity(ModuleContext.ModuleId, ModuleContext.TabId)
                                Dim isOwner As Boolean
                                isOwner = m_oBlog.UserID = ModuleContext.PortalSettings.UserId

                                litBlogDescription.Text = m_oBlog.Description
                                list = objEntries.GetEntriesByBlog(m_oBlog.BlogID, m_dBlogDate, BlogSettings.RecentEntriesMax, CurrentPage, objSecurity.CanAddEntry(isOwner, m_oBlog.AuthorMode), objSecurity.CanAddEntry(isOwner, m_oBlog.AuthorMode))

                                prevPage = Links.ViewEntriesByBlog(ModuleContext, m_oBlog.BlogID, CurrentPage - 1)
                                nextPage = Links.ViewEntriesByBlog(ModuleContext, m_oBlog.BlogID, CurrentPage + 1)
                                pageUrl = Links.ViewEntriesByBlog(ModuleContext, m_oBlog.BlogID, CurrentPage)

                                pageTitle = m_oBlog.Title
                                pageDescription = m_oBlog.Description
                                pageAuthor = m_oBlog.UserFullName
                            End If
                        Else
                            list = objEntries.GetEntriesByTerm(PortalId, m_dBlogDate, Tag, BlogSettings.RecentEntriesMax, CurrentPage, Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()), Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()))
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
                        list = objEntries.GetEntriesByTerm(Me.PortalId, m_dBlogDate, Category, BlogSettings.RecentEntriesMax, CurrentPage, DotNetNuke.Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()), DotNetNuke.Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()))
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

                If Not m_oBlog Is Nothing Then
                    If m_oBlog.Syndicated Then
                        lnkRSS.NavigateUrl = Links.RSSByBlog(ModuleContext, m_oBlog.BlogID)
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
                        objLink.Attributes("href") = Links.RSSAggregated(ModuleContext)
                        ph.Controls.Add(objLink)
                    ElseIf Not m_oBlog Is Nothing AndAlso m_oBlog.Syndicated Then
                        Dim objLink As New HtmlGenericControl("LINK")
                        objLink.Attributes("title") = "RSS"
                        objLink.Attributes("rel") = "alternate"
                        objLink.Attributes("type") = "application/rss+xml"
                        objLink.Attributes("href") = Links.RSSByBlog(ModuleContext, m_oBlog.BlogID)
                        ph.Controls.Add(objLink)
                    End If
                End If

                If (HasValue = False) Then
                    Dim message As String = GetString("msgNoResult", LocalResourceFile)

                    If m_bSearchDisplay Then
                        message = GetString("msgNoSearchResult", LocalResourceFile)
                    ElseIf m_dBlogDate <> Date.MinValue Then
                        message = GetString("msgNoPeriodResult", LocalResourceFile)
                    ElseIf Not m_oBlog Is Nothing Then
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

    Protected Sub lstBlogView_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataListItemEventArgs) Handles lstBlogView.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item Then
            Dim litAuthor As System.Web.UI.WebControls.Literal = CType(e.Item.FindControl("litAuthor"), System.Web.UI.WebControls.Literal)
            Dim lblDescription As System.Web.UI.WebControls.Literal = CType(e.Item.FindControl("litDescription"), System.Web.UI.WebControls.Literal)
            Dim lnkComments As System.Web.UI.WebControls.HyperLink = CType(e.Item.FindControl("lnkComments"), System.Web.UI.WebControls.HyperLink)
            Dim pnNotPublished As System.Web.UI.WebControls.Panel = CType(e.Item.FindControl("pnNotPublished"), System.Web.UI.WebControls.Panel)
            Dim lblPublishDate As System.Web.UI.WebControls.Label = CType(e.Item.FindControl("lblPublishDate"), System.Web.UI.WebControls.Label)
            Dim lnkParentBlog As System.Web.UI.WebControls.HyperLink = CType(e.Item.FindControl("lnkParentBlog"), System.Web.UI.WebControls.HyperLink)
            Dim imgBlogParentSeparator As System.Web.UI.WebControls.Image = CType(e.Item.FindControl("imgBlogParentSeparator"), System.Web.UI.WebControls.Image)
            Dim lnkChildBlog As System.Web.UI.WebControls.HyperLink = CType(e.Item.FindControl("lnkChildBlog"), System.Web.UI.WebControls.HyperLink)
            Dim lnkEntry As System.Web.UI.WebControls.HyperLink = CType(e.Item.FindControl("lnkEntry"), System.Web.UI.WebControls.HyperLink)
            Dim divBlogReadMore As System.Web.UI.HtmlControls.HtmlGenericControl = CType(e.Item.FindControl("divBlogReadMore"), System.Web.UI.HtmlControls.HtmlGenericControl)
            Dim litCategories As System.Web.UI.WebControls.Literal = CType(e.Item.FindControl("litCategories"), System.Web.UI.WebControls.Literal)
            Dim tagControl As Tags = DirectCast(e.Item.FindControl("dbaTag"), Tags)
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
                hlPermaLink.Text = Localization.GetString("lnkPermaLink", LocalResourceFile)
                hlPermaLink.Visible = (m_oEntry.PermaLink <> Utility.BlogNavigateURL(TabId, PortalId, m_oEntry.EntryID, m_oEntry.Title, BlogSettings.ShowSeoFriendlyUrl))

                Dim hlMore As HyperLink = CType(e.Item.FindControl("hlMore"), HyperLink)
                hlMore.NavigateUrl = Utility.BlogNavigateURL(TabId, PortalId, m_oEntry.EntryID, m_oEntry.Title, BlogSettings.ShowSeoFriendlyUrl)
                hlMore.Text = Localization.GetString("lnkReadMore", LocalResourceFile)
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
                Dim userProfile As String = DotNetNuke.Common.Globals.UserProfileURL(m_oEntry.UserID)

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
                If Not m_oBlog Is Nothing Then
                    If m_oBlog.BlogID = oBlog.BlogID Then
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

    Protected Sub lstBlogView_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataListCommandEventArgs) Handles lstBlogView.ItemCommand
        ' DW - 04/22/2008 - Added to allow the use of the new BlogNavigateURL method

        Dim EntryID As Integer = Int32.Parse(CType(e.CommandArgument, String))
        Dim EntryController As New EntryController
        Dim EntryInfo As EntryInfo = EntryController.GetEntry(EntryID, PortalId)

        Select Case e.CommandName
            Case "Entry"
                Response.Redirect(EntryInfo.PermaLink)
            Case "Comments"
                Response.Redirect(EntryInfo.PermaLink & "#Comments")
            Case "User"
        End Select
    End Sub

#End Region

End Class