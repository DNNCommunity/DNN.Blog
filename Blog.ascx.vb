Imports System.Linq
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Modules.Blog.Templating
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Entities.Posts
Imports DotNetNuke.Modules.Blog.Entities.Terms
Imports DotNetNuke.Modules.Blog.Entities.Comments

Public Class Blog
 Inherits BlogModuleBase
 Implements IActionable

#Region " Private Members "
 Private _urlParameters As New List(Of String)
 Private _pageSize As Integer = -1
 Private _totalRecords As Integer = 0
 Private _reqPage As Integer = 1
 Private _usePaging As Boolean = False
#End Region

#Region " Event Handlers "
 Private Sub Page_Init1(sender As Object, e As System.EventArgs) Handles Me.Init
  ctlComments.ModuleConfiguration = Me.ModuleConfiguration
  ctlComments.BlogContext = Me.BlogContext
  ctlManagement.ModuleConfiguration = Me.ModuleConfiguration
  ctlManagement.BlogContext = Me.BlogContext
 End Sub

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

  If Context.Items("BlogPageInitialized") Is Nothing Then

   ' wlw style detection post redirect?
   If Not String.IsNullOrEmpty(Settings.StyleDetectionUrl) And BlogContext.WLWRequest Then
    ' we have a style detection post in storage and it's being requested
    Dim url As String = Settings.StyleDetectionUrl
    Settings.StyleDetectionUrl = ""
    Settings.UpdateSettings()
    Response.Redirect(url, False)
   End If

   AddBlogService()
   If Not Me.IsPostBack And BlogContext.ContentItemId > -1 Then
    Dim scriptBlock As String = "(function ($, Sys) {$(document).ready(function () {blogService.viewPost(" & BlogContext.BlogId.ToString & ", " & BlogContext.ContentItemId.ToString & ")});} (jQuery, window.Sys));"
    Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "PostViewScript", scriptBlock, True)
   End If

   AddWLWManifestLink()

   If Settings.ModifyPageDetails Then
    If BlogContext.Post IsNot Nothing Then
     Page.Title = BlogContext.Post.LocalizedTitle
     Page.Description = BlogContext.Post.LocalizedSummary
     Page.KeyWords = String.Join(",", BlogContext.Post.Terms.ToStringArray)
    ElseIf BlogContext.Blog IsNot Nothing Then
     Page.Title = BlogContext.Blog.LocalizedTitle
     Page.Description = BlogContext.Blog.LocalizedDescription
    ElseIf BlogContext.Author IsNot Nothing Then
     Page.Title = BlogContext.Author.DisplayName
     Page.Description = BlogContext.Author.Profile.Biography
    ElseIf BlogContext.Term IsNot Nothing Then
     Page.Title = BlogContext.Term.LocalizedName
     Page.Description = BlogContext.Term.LocalizedDescription
    End If
   End If

   Context.Items("BlogPageInitialized") = True
  End If

  Me.Request.Params.ReadValue("Page", _reqPage)
  DataBind()

 End Sub
#End Region

#Region " Template Data Retrieval "
 Private Sub vtContents_GetData(ByVal DataSource As String, ByVal Parameters As Dictionary(Of String, String), ByRef Replacers As System.Collections.Generic.List(Of GenericTokenReplace), ByRef Arguments As System.Collections.Generic.List(Of String()), callingObject As Object) Handles vtContents.GetData

  Select Case DataSource.ToLower

   Case "blogs"

    Dim blogList As IEnumerable(Of BlogInfo) = BlogsController.GetBlogsByModule(Settings.ModuleId, UserId, BlogContext.Locale).Values.Where(Function(b)
                                                                                                                                             Return b.Published = True
                                                                                                                                            End Function).OrderBy(Function(b) b.Title)
    Parameters.ReadValue("pagesize", _pageSize)
    If _pageSize > 0 Then
     _usePaging = True
     Dim startRec As Integer = ((_reqPage - 1) * _pageSize) + 1
     Dim endRec As Integer = _reqPage * _pageSize
     Dim i As Integer = 1
     For Each b As BlogInfo In blogList
      If i >= startRec And i <= endRec Then
       Replacers.Add(New BlogTokenReplace(Me, b))
      End If
      i += 1
     Next
    Else
     For Each b As BlogInfo In blogList
      Replacers.Add(New BlogTokenReplace(Me, b))
     Next
    End If

   Case "posts"

    Parameters.ReadValue("pagesize", _pageSize)
    EnsurePostList(_pageSize)
    For Each e As PostInfo In PostList
     Replacers.Add(New BlogTokenReplace(Me, e))
    Next

   Case "postspager"

    Parameters.ReadValue("pagesize", _pageSize)
    EnsurePostList(_pageSize)
    Dim pagerType As String = "allpages"
    Parameters.ReadValue("pagertype", pagerType)
    Dim remdr As Integer = 0
    Dim nrPages As Integer = Math.DivRem(_totalRecords, _pageSize, remdr)
    If remdr > 0 Then
     nrPages += 1
    End If
    If nrPages < 2 Then
    Else
     Replacers.Add(New BlogTokenReplace(Me))
     Select Case pagerType.ToLower
      Case "allpages"
       For i As Integer = 1 To nrPages
        Dim s As String() = {"page=" & i.ToString, "pageiscurrent=" & CBool(i = _reqPage).ToString, "pagename=" & i.ToString, "pagetype=number"}
        Arguments.Add(s)
       Next
      Case "somepages"
       If _reqPage > 3 Then
        Dim s As String() = {"page=1", "pageiscurrent=False", "pagename=1", "pagetype=firstpage"}
        Arguments.Add(s)
       End If
       For i As Integer = Math.Max(_reqPage - 2, 1) To Math.Min(_reqPage + 2, nrPages)
        Dim s As String() = {"page=" & i.ToString, "pageiscurrent=" & CBool(i = _reqPage).ToString, "pagename=" & i.ToString, "pagetype=number"}
        Arguments.Add(s)
       Next
       If _reqPage < nrPages - 3 Then
        Dim s As String() = {"page=" & nrPages.ToString, "pageiscurrent=False", "pagename=" & nrPages.ToString, "pagetype=lastpage"}
        Arguments.Add(s)
       End If
      Case "newerolder"
       If _reqPage > 1 Then
        Dim s As String() = {"page=" & (_reqPage - 1).ToString, "pageiscurrent=False", "pagename=" & LocalizeString("Newer"), "pagetype=previous"}
        Arguments.Add(s)
       End If
       If _reqPage < nrPages Then
        Dim s As String() = {"page=" & (_reqPage + 1).ToString, "pageiscurrent=False", "pagename=" & LocalizeString("Older"), "pagetype=next"}
        Arguments.Add(s)
       End If
     End Select
    End If

   Case "terms"

    If callingObject IsNot Nothing AndAlso TypeOf callingObject Is PostInfo Then
     For Each t As TermInfo In CType(callingObject, PostInfo).Terms
      Replacers.Add(New BlogTokenReplace(Me, BlogContext.Post, t))
     Next
    ElseIf BlogContext.Post IsNot Nothing Then
     For Each t As TermInfo In BlogContext.Post.Terms
      Replacers.Add(New BlogTokenReplace(Me, BlogContext.Post, t))
     Next
    Else
     For Each t As TermInfo In TermsController.GetTermsByModule(Settings.ModuleId, BlogContext.Locale)
      Replacers.Add(New BlogTokenReplace(Me, Nothing, t))
     Next
    End If
    _usePaging = False

   Case "allterms"

    For Each t As TermInfo In TermsController.GetTermsByModule(Settings.ModuleId, BlogContext.Locale)
     Replacers.Add(New BlogTokenReplace(Me, Nothing, t))
    Next
    _usePaging = False

   Case "keywords", "tags"

    If callingObject IsNot Nothing AndAlso TypeOf callingObject Is PostInfo Then
     For Each t As TermInfo In CType(callingObject, PostInfo).PostTags
      Replacers.Add(New BlogTokenReplace(Me, BlogContext.Post, t))
     Next
    ElseIf BlogContext.Post IsNot Nothing Then
     For Each t As TermInfo In BlogContext.Post.PostTags
      Replacers.Add(New BlogTokenReplace(Me, BlogContext.Post, t))
     Next
    Else
     For Each t As TermInfo In TermsController.GetTermsByModule(Settings.ModuleId, BlogContext.Locale).Where(Function(x) x.VocabularyId = 1).ToList
      Replacers.Add(New BlogTokenReplace(Me, Nothing, t))
     Next
    End If
    _usePaging = False

   Case "allkeywords", "alltags"

    For Each t As TermInfo In TermsController.GetTermsByModule(Settings.ModuleId, BlogContext.Locale).Where(Function(x) x.VocabularyId = 1).ToList
     Replacers.Add(New BlogTokenReplace(Me, Nothing, t))
    Next
    _usePaging = False

   Case "categories"

    If callingObject IsNot Nothing AndAlso TypeOf callingObject Is PostInfo Then
     For Each t As TermInfo In CType(callingObject, PostInfo).PostCategories
      Replacers.Add(New BlogTokenReplace(Me, BlogContext.Post, t))
     Next
    ElseIf BlogContext.Post IsNot Nothing Then
     For Each t As TermInfo In BlogContext.Post.PostCategories
      Replacers.Add(New BlogTokenReplace(Me, BlogContext.Post, t))
     Next
    Else
     For Each t As TermInfo In TermsController.GetTermsByModule(Settings.ModuleId, BlogContext.Locale).Where(Function(x) x.VocabularyId <> 1).ToList
      Replacers.Add(New BlogTokenReplace(Me, Nothing, t))
     Next
    End If
    _usePaging = False

   Case "allcategories"

    For Each t As TermInfo In TermsController.GetTermsByModule(Settings.ModuleId, BlogContext.Locale).Where(Function(x) x.VocabularyId <> 1).ToList
     Replacers.Add(New BlogTokenReplace(Me, Nothing, t))
    Next
    _usePaging = False

   Case "comments"

    If callingObject IsNot Nothing AndAlso TypeOf callingObject Is PostInfo Then
     For Each c As CommentInfo In CommentsController.GetCommentsByContentItem(CType(callingObject, PostInfo).ContentItemId, False)
      Replacers.Add(New BlogTokenReplace(Me, BlogContext.Post, c))
     Next
    ElseIf BlogContext.Post IsNot Nothing Then
     For Each c As CommentInfo In CommentsController.GetCommentsByContentItem(BlogContext.Post.ContentItemId, False)
      Replacers.Add(New BlogTokenReplace(Me, BlogContext.Post, c))
     Next
    Else
     For Each c As CommentInfo In CommentsController.GetCommentsByModule(ModuleId)
      Replacers.Add(New BlogTokenReplace(Me, BlogContext.Post, c))
     Next
    End If
    _usePaging = False

   Case "calendar", "blogcalendar"

    For Each bci As BlogCalendarInfo In BlogsController.GetBlogCalendar(Settings.ModuleId, BlogContext.BlogId, BlogContext.ShowLocale)
     Replacers.Add(New BlogTokenReplace(Me, bci))
    Next

   Case "authors"

    Dim sort As String = ""
    Parameters.ReadValue("sort", sort)
    Select Case sort.ToLower
     Case "username"
      For Each u As PostAuthor In PostsController.GetAuthors(Settings.ModuleId, BlogContext.BlogId).OrderBy(Function(t) t.Username)
       Replacers.Add(New BlogTokenReplace(Me, New LazyLoadingUser(u)))
      Next
     Case "email"
      For Each u As PostAuthor In PostsController.GetAuthors(Settings.ModuleId, BlogContext.BlogId).OrderBy(Function(t) t.Email)
       Replacers.Add(New BlogTokenReplace(Me, New LazyLoadingUser(u)))
      Next
     Case "firstname"
      For Each u As PostAuthor In PostsController.GetAuthors(Settings.ModuleId, BlogContext.BlogId).OrderBy(Function(t) t.FirstName)
       Replacers.Add(New BlogTokenReplace(Me, New LazyLoadingUser(u)))
      Next
     Case "displayname"
      For Each u As PostAuthor In PostsController.GetAuthors(Settings.ModuleId, BlogContext.BlogId).OrderBy(Function(t) t.DisplayName)
       Replacers.Add(New BlogTokenReplace(Me, New LazyLoadingUser(u)))
      Next
     Case Else ' last name
      For Each u As PostAuthor In PostsController.GetAuthors(Settings.ModuleId, BlogContext.BlogId)
       Replacers.Add(New BlogTokenReplace(Me, New LazyLoadingUser(u)))
      Next
    End Select

  End Select

 End Sub
#End Region

#Region " Post List Stuff "
 Private Property PostList As IEnumerable(Of PostInfo) = Nothing
 Private Sub EnsurePostList(pageSize As Integer)

  If PostList Is Nothing Then
   If pageSize < 1 Then pageSize = 10 ' we will not list "all Posts"
   If Not String.IsNullOrEmpty(BlogContext.SearchString) Then
    Dim publishValue As Integer = 1
    If BlogContext.SearchUnpublished Then publishValue = -1
    If BlogContext.Term Is Nothing Then
     PostList = PostsController.SearchPosts(Settings.ModuleId, BlogContext.BlogId, BlogContext.Locale, BlogContext.SearchString, BlogContext.SearchTitle, BlogContext.SearchContents, publishValue, BlogContext.ShowLocale, BlogContext.EndDate, -1, _reqPage - 1, pageSize, "PUBLISHEDONDATE DESC", _totalRecords, UserId, BlogContext.Security.UserIsAdmin).Values
    Else
     PostList = PostsController.SearchPostsByTerm(Settings.ModuleId, BlogContext.BlogId, BlogContext.Locale, BlogContext.TermId, BlogContext.SearchString, BlogContext.SearchTitle, BlogContext.SearchContents, publishValue, BlogContext.ShowLocale, BlogContext.EndDate, -1, _reqPage - 1, pageSize, "PUBLISHEDONDATE DESC", _totalRecords, UserId, BlogContext.Security.UserIsAdmin).Values
    End If
   ElseIf BlogContext.Term Is Nothing Then
    PostList = PostsController.GetPosts(Settings.ModuleId, BlogContext.BlogId, BlogContext.Locale, -1, BlogContext.ShowLocale, BlogContext.EndDate, BlogContext.AuthorId, _reqPage - 1, pageSize, "PUBLISHEDONDATE DESC", _totalRecords, UserId, BlogContext.Security.UserIsAdmin).Values
   Else
    PostList = PostsController.GetPostsByTerm(Settings.ModuleId, BlogContext.BlogId, BlogContext.Locale, BlogContext.TermId, -1, BlogContext.ShowLocale, BlogContext.EndDate, BlogContext.AuthorId, _reqPage - 1, pageSize, "PUBLISHEDONDATE DESC", _totalRecords, UserId, BlogContext.Security.UserIsAdmin).Values
   End If
   _usePaging = True
  End If

 End Sub
#End Region

#Region " Overrides "
 Public Overrides Sub DataBind()

  Dim tmgr As New TemplateManager(PortalSettings, ViewSettings.Template)
  With vtContents
   .TemplatePath = tmgr.TemplatePath
   .TemplateMapPath = tmgr.TemplateMapPath
   .DefaultReplacer = New BlogTokenReplace(Me)
  End With
  vtContents.DataBind()

  ctlComments.Visible = ViewSettings.ShowComments AndAlso BlogContext.Security.CanViewComments
  ctlManagement.Visible = ViewSettings.ShowManagementPanel

 End Sub
#End Region

#Region " IActionable "
 Public ReadOnly Property ModuleActions As Actions.ModuleActionCollection Implements IActionable.ModuleActions
  Get
   Dim MyActions As New Actions.ModuleActionCollection
   If IsEditable Or BlogContext.Security.IsBlogger Then
    MyActions.Add(GetNextActionID, Localization.GetString(ModuleActionType.EditContent, LocalResourceFile), ModuleActionType.EditContent, "", "", EditUrl("Manage"), False, DotNetNuke.Security.SecurityAccessLevel.View, True, False)
   End If
   If IsEditable Then
    MyActions.Add(GetNextActionID, LocalizeString("TemplateSettings"), ModuleActionType.EditContent, "", "", EditUrl("TemplateSettings"), False, DotNetNuke.Security.SecurityAccessLevel.Edit, True, False)
   End If
   Return MyActions
  End Get
 End Property
#End Region

End Class