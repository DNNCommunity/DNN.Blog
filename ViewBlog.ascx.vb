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
Imports DotNetNuke.Modules.Blog.Business
Imports DotNetNuke.Modules.Blog.Controllers
Imports DotNetNuke.Modules.Blog.Common
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization.Localization
Imports DotNetNuke.Modules.Blog.Entities
Imports DotNetNuke.Framework
Imports DotNetNuke.Entities.Users

Partial Public Class ViewBlog
 Inherits BlogModuleBase

#Region " Private Members "

 Private _searchString As String = ""
 Private _searchType As String = "Keyword"
 Private _blogDate As Date = Date.UtcNow
 Private _blogDateType As String = ""
 Private _searchDisplay As Boolean = False
 'Private _personalBlogId As Integer = -1
 Private _seoFriendlyUrl As Boolean = True

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

#Region " Event Handlers "

 Protected Overloads Sub Page_Init(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Init
  jQuery.RequestUIRegistration()
  ClientResourceManager.RegisterScript(Page, TemplateSourceDirectory + "/js/jquery.qatooltip.js")
  ClientResourceManager.RegisterScript(Page, "~/Resources/Shared/Scripts/jquery/jquery.hoverIntent.min.js")

  ' Read optional querystring parameters
  Request.Params.ReadValue("Search", _searchString)
  Request.Params.ReadValue("BlogDate", _blogDate)
  Request.Params.ReadValue("DateType", _blogDateType)
  Request.Params.ReadValue("SearchType", _searchType)

  ' Initialize properties
  _blogDate = TimeZoneInfo.ConvertTimeToUtc(_blogDate, UiTimeZone)

  If _searchString <> "" Then
   _searchDisplay = True
   If Blog Is Nothing Then
    ModuleConfiguration.ModuleTitle = GetString("msgSearchResults", LocalResourceFile)
   Else
    ModuleConfiguration.ModuleTitle = GetString("msgSearchResultsFor", LocalResourceFile) & " " & Blog.Title
   End If
  End If
  MyActions.Add(GetNextActionID, GetString("msgModuleOptions", LocalResourceFile), DotNetNuke.Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "", EditUrl("", "", "Module_Options"), False, DotNetNuke.Security.SecurityAccessLevel.Admin, True, False)
 End Sub

 Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  Try
   _seoFriendlyUrl = Settings.ShowSeoFriendlyUrl
   Dim list As List(Of EntryInfo)

   Dim prevPage As String = NavigateURL()
   Dim nextPage As String = NavigateURL()
   Dim pageUrl As String = NavigateURL()

   If Not Page.IsPostBack Then
    Dim HasValue As Boolean = False

    If _searchDisplay Then
     If _searchType = "Phrase" Then
      If Blog Is Nothing Then
       list = SearchController.SearchByPhraseByPortal(PortalId, _searchString, Security.UserIsAdmin, Security.UserIsAdmin)
      Else
       list = SearchController.SearchByPhraseByBlog(Blog.BlogID, _searchString, Security.UserIsAdmin, Security.UserIsAdmin)
      End If
     Else
      If Blog Is Nothing Then
       list = SearchController.SearchByKeywordByPortal(PortalId, _searchString, Security.UserIsAdmin, Security.UserIsAdmin)
      Else
       list = SearchController.SearchByKeywordByBlog(Blog.BlogID, _searchString, Security.UserIsAdmin, Security.UserIsAdmin)
      End If
     End If
    Else
     Dim pageTitle As String = BasePage.Title
     Dim keyWords As String = BasePage.KeyWords
     Dim pageDescription As String = BasePage.Description
     Dim pageAuthor As String = BasePage.Author

     If Category < 1 Then
      If Tag < 1 Then
       If Blog Is Nothing Then
        ' most recent approved blog list (default view), no category/tag specified

        list = EntryController.GetEntriesByPortal(PortalId, _blogDate, _blogDateType, Settings.RecentEntriesMax, CurrentPage, Security.UserIsAdmin, Security.UserIsAdmin)

        If Not lnkRecentRss Is Nothing Then
         lnkRecentRss.NavigateUrl = NavigateURL(ModuleContext.TabId, "", "rssid=0")
        End If

        ' No paging or meta updates for this view.
       Else
        ' Specific blog view
        If Blog.ShowFullName Then
         hlAuthor.Text = Blog.UserFullName
        Else
         hlAuthor.Text = Blog.UserName
        End If

        If Blog.AuthorMode <> Constants.AuthorMode.BloggerMode Then
         Dim objAuthor As DotNetNuke.Entities.Users.UserInfo = DotNetNuke.Entities.Users.UserController.GetUserById(ModuleContext.PortalId, Blog.UserID)
         dbiUser.ImageUrl = objAuthor.Profile.PhotoURL
         hlAuthor.NavigateUrl = UserProfileURL(Blog.UserID)
         imgAuthorLink.NavigateUrl = UserProfileURL(Blog.UserID)

         pnlBlogInfo.Visible = True
        End If

        Dim isOwner As Boolean
        isOwner = Blog.UserID = ModuleContext.PortalSettings.UserId

        litBlogDescription.Text = Blog.Description
        list = EntryController.GetEntriesByBlog(Blog.BlogID, _blogDate, Settings.RecentEntriesMax, CurrentPage, Security.CanAddEntry, Security.CanAddEntry)

        prevPage = Links.ViewEntriesByBlog(ModuleContext, Blog.BlogID, CurrentPage - 1)
        nextPage = Links.ViewEntriesByBlog(ModuleContext, Blog.BlogID, CurrentPage + 1)
        pageUrl = Links.ViewEntriesByBlog(ModuleContext, Blog.BlogID, CurrentPage)

        pageTitle = Blog.Title
        pageDescription = Blog.Description
        pageAuthor = Blog.UserFullName
       End If
      Else
       list = EntryController.GetEntriesByTerm(PortalId, _blogDate, Tag, Settings.RecentEntriesMax, CurrentPage, Security.UserIsAdmin, Security.UserIsAdmin)

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
      list = EntryController.GetEntriesByTerm(PortalId, _blogDate, Category, Settings.RecentEntriesMax, CurrentPage, Security.UserIsAdmin, Security.UserIsAdmin)

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
      Dim totalPages As Double = Convert.ToDouble(CDbl(TotalRecords) / Settings.RecentEntriesMax)

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

    If Not Blog Is Nothing Then
     If Blog.Syndicated Then
      lnkRSS.NavigateUrl = Links.RSSByBlog(ModuleContext, Blog.BlogID)
      lnkRSS.Visible = True
     End If
    End If

    ' add rss discovery link
    Dim ph As PlaceHolder = CType(BasePage.FindControl("phDNNHead"), PlaceHolder)
    If Not ph Is Nothing Then
     If Blog Is Nothing AndAlso pnlBlogRss.Visible Then
      Dim objLink As New HtmlGenericControl("LINK")
      objLink.Attributes("title") = "RSS"
      objLink.Attributes("rel") = "alternate"
      objLink.Attributes("type") = "application/rss+xml"
      objLink.Attributes("href") = Links.RSSAggregated(ModuleContext)
      ph.Controls.Add(objLink)
     ElseIf Not Blog Is Nothing AndAlso Blog.Syndicated Then
      Dim objLink As New HtmlGenericControl("LINK")
      objLink.Attributes("title") = "RSS"
      objLink.Attributes("rel") = "alternate"
      objLink.Attributes("type") = "application/rss+xml"
      objLink.Attributes("href") = Links.RSSByBlog(ModuleContext, Blog.BlogID)
      ph.Controls.Add(objLink)
     End If
    End If

    If (HasValue = False) Then
     Dim message As String = GetString("msgNoResult", LocalResourceFile)

     If _searchDisplay Then
      message = GetString("msgNoSearchResult", LocalResourceFile)
     ElseIf _blogDate <> Date.MinValue Then
      message = GetString("msgNoPeriodResult", LocalResourceFile)
     ElseIf Not Blog Is Nothing Then
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
   Dim lnkEntry As HyperLink = CType(e.Item.FindControl("lnkEntry"), HyperLink)
   Dim divBlogReadMore As HtmlGenericControl = CType(e.Item.FindControl("divBlogReadMore"), HtmlGenericControl)
   Dim litCategories As Literal = CType(e.Item.FindControl("litCategories"), Literal)
   Dim tagControl As Tags = DirectCast(e.Item.FindControl("dbaTag"), Tags)
   Dim objBlog As BlogInfo
   Dim objEntry As EntryInfo = CType(e.Item.DataItem, EntryInfo)

   If Blog Is Nothing Then
    objBlog = BlogController.GetBlog(objEntry.BlogID)
   Else
    If Blog.BlogID <> objEntry.BlogID Then
     objBlog = BlogController.GetBlog(objEntry.BlogID)
    Else
     objBlog = Blog
    End If
   End If

   Dim lnkEditEntry As HyperLink
   Dim imgEdit As Image
   lnkEditEntry = CType(e.Item.FindControl("lnkEditEntry"), HyperLink)
   imgEdit = CType(e.Item.FindControl("imgEdit"), Image)

   If Not objEntry Is Nothing Then
    Dim isOwner As Boolean = objBlog.UserID = ModuleContext.PortalSettings.UserId

    If Security.CanAddEntry AndAlso Not lnkEditEntry Is Nothing Then
     lnkEditEntry.Visible = True
     lnkEditEntry.NavigateUrl = EditUrl("EntryID", objEntry.EntryID.ToString(), "Edit_Entry")
    Else
     lnkEditEntry.Visible = False
    End If

    Dim hlPermaLink As HyperLink = CType(e.Item.FindControl("hlPermaLink"), HyperLink)
    hlPermaLink.NavigateUrl = objEntry.PermaLink
    hlPermaLink.Text = GetString("lnkPermaLink", LocalResourceFile)
    hlPermaLink.Visible = (objEntry.PermaLink <> Utility.BlogNavigateURL(TabId, PortalId, objEntry.EntryID, objEntry.Title, Settings.ShowSeoFriendlyUrl))

    Dim hlMore As HyperLink = CType(e.Item.FindControl("hlMore"), HyperLink)
    hlMore.NavigateUrl = Utility.BlogNavigateURL(TabId, PortalId, objEntry.EntryID, objEntry.Title, Settings.ShowSeoFriendlyUrl)
    hlMore.Text = GetString("lnkReadMore", LocalResourceFile)
    hlMore.Visible = (objEntry.PermaLink <> Utility.BlogNavigateURL(TabId, PortalId, objEntry.EntryID, objEntry.Title, Settings.ShowSeoFriendlyUrl))

    If objEntry.ContentItemId > 0 Then
     Dim Categories As String = ""
     Dim i As Integer = 0
     Dim colCategories As List(Of TermInfo) = objEntry.EntryCategories(Settings.VocabularyId)

     For Each objTerm As TermInfo In colCategories
      Categories += "<a href='" + Links.ViewEntriesByCategory(ModuleContext, objTerm.TermId, 1) + "'>" + objTerm.Name + "</a>"
      i += 1
      If i <= (colCategories.Count - 1) Then
       Categories += ", "
      End If
     Next

     litCategories.Text = Categories

     tagControl.ModContext = ModuleContext
     tagControl.DataSource = objEntry.EntryTags()
     tagControl.DataBind()
    End If
   End If

   lnkEntry.NavigateUrl = objEntry.PermaLink
   lnkComments.NavigateUrl = objEntry.PermaLink & "#Comments"

   'DR-04/24/2009-BLG-9712
   lnkEntry.Attributes.Add("rel", "bookmark")

   Dim profileUrl As String = UserProfileURL(objEntry.UserID)
   Dim username As String = "Anonymous"
   Dim displayName As String = "Anonymous"

   If objBlog.AuthorMode = Constants.AuthorMode.BloggerMode Then
    ' This means the credit always goes to the user who posted the entry (other modes credit the blog owner
    Dim objUser As UserInfo = UserController.GetUserById(ModuleContext.PortalId, objEntry.CreatedUserId)
    If objUser IsNot Nothing Then
     username = objUser.Username
     displayName = objUser.DisplayName
     profileUrl = UserProfileURL(objEntry.CreatedUserId)
     litAuthor.Visible = True
    End If
   Else
    Dim objUser As UserInfo = UserController.GetUserById(ModuleContext.PortalId, objBlog.UserID)
    If objUser IsNot Nothing Then
     username = objUser.Username
     displayName = objUser.DisplayName
     profileUrl = UserProfileURL(objEntry.CreatedUserId)
     litAuthor.Visible = True
    End If
   End If

   If objBlog.ShowFullName Then
    litAuthor.Text = GetString("msgCreateFrom", LocalResourceFile) & " "
    litAuthor.Text += "<a href='" + profileUrl + "'>"
    litAuthor.Text += username
    litAuthor.Text += "</a>"
    litAuthor.Text += " " & GetString("msgCreateOn", LocalResourceFile)
   Else
    litAuthor.Text = GetString("msgCreateFrom", LocalResourceFile) & " "
    litAuthor.Text += "<a href='" + profileUrl + "'>"
    litAuthor.Text += displayName
    litAuthor.Text += "</a>"
    litAuthor.Text += " " & GetString("msgCreateOn", LocalResourceFile)
   End If

   pnNotPublished.Visible = Not objEntry.Published
   lblPublishDate.Text = Utility.DateFromUtc(objEntry.AddedDate, UiTimeZone).ToString("f")

   ' DW - Updated again to make the truncation of the summary optional based on the EnforceSummaryTruncation
   '       blog setting.
   Dim ActualSummary As String = Server.HtmlDecode(objEntry.Description)
   Dim Entry As String = Server.HtmlDecode(objEntry.Entry)
   Dim GeneratedSummary As String = CType(IIf(ActualSummary Is Nothing, String.Empty, ActualSummary), String)
   Dim EnforceSummaryTruncation As Boolean = Settings.EnforceSummaryTruncation
   If GeneratedSummary = String.Empty Then
    GeneratedSummary = Entry
    ' We'll set the EnforceSummaryTruncation flag to true since generated summaries should
    ' always be truncated.
    EnforceSummaryTruncation = True
   End If
   If Settings.SummaryMaxLength = 0 Then
    If ActualSummary.Length > 0 And Entry.Length > 0 Then
     'lnkReadMore.Visible = True
     divBlogReadMore.Visible = True
    Else
     'lnkReadMore.Visible = False
     divBlogReadMore.Visible = False
    End If
    lblDescription.Text = GeneratedSummary
   Else
    If GeneratedSummary.Length > Settings.SummaryMaxLength Then
     'We need to truncate, but only EnforceSummaryTruncation is True
     If EnforceSummaryTruncation Then
      lblDescription.Text = Utility.CleanHTML(GeneratedSummary, Settings.SummaryMaxLength)
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

   If (objBlog.AllowComments Or objEntry.AllowComments) Then
    ' CP: Removed from above, we should always show the comment link as long as we allow them (in other words, who cares if they are logged in) - CodePlex = 22459
    'And CType(IIf(Me.UserId = -1, oBlog.AllowAnonymous, True), Boolean) 
    lnkComments.Visible = True
    lnkComments.Text = String.Format(GetString("lnkComments", LocalResourceFile), objEntry.CommentCount)
   Else
    lnkComments.Visible = False
   End If

   objBlog = Nothing
  End If
 End Sub

 Protected Sub lstBlogView_ItemCommand(ByVal source As Object, ByVal e As DataListCommandEventArgs) Handles lstBlogView.ItemCommand
  Dim entryId As Integer = Int32.Parse(CType(e.CommandArgument, String))
  Dim entryInfo As EntryInfo = EntryController.GetEntry(entryId, PortalId)

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