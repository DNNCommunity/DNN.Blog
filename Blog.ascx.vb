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
 Private _endDate As Date = Date.Now.ToUniversalTime
#End Region

#Region " Event Handlers "
 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

  DotNetNuke.Framework.jQuery.RequestRegistration()
  DotNetNuke.Framework.jQuery.RequestUIRegistration()
  Me.Request.Params.ReadValue("Page", _reqPage)
  Me.Request.Params.ReadValue("EndDate", _endDate)
  DataBind()

 End Sub
#End Region

#Region " Template Data Retrieval "
 Private Sub vtContents_GetData(ByVal DataSource As String, ByVal Parameters As Dictionary(Of String, String), ByRef Replacers As System.Collections.Generic.List(Of GenericTokenReplace), ByRef Arguments As System.Collections.Generic.List(Of String()), callingObject As Object) Handles vtContents.GetData

  Select Case DataSource.ToLower

   Case "blogs"

    Dim blogList As IEnumerable(Of BlogInfo) = BlogsController.GetBlogsByModule(Settings.ModuleId, UserId, Locale).Values.Where(Function(b)
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
       Replacers.Add(New BlogTokenReplace(Me, Settings, b))
      End If
      i += 1
     Next
    Else
     For Each b As BlogInfo In blogList
      Replacers.Add(New BlogTokenReplace(Me, Settings, b))
     Next
    End If

   Case "posts"

    Parameters.ReadValue("pagesize", _pageSize)
    EnsurePostList(_pageSize)
    For Each e As PostInfo In PostList
     Replacers.Add(New BlogTokenReplace(Me, Settings, e))
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
     Replacers.Add(New BlogTokenReplace(Me, Settings))
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
      Replacers.Add(New BlogTokenReplace(Me, Settings, Post, t))
     Next
    ElseIf Post IsNot Nothing Then
     For Each t As TermInfo In Post.Terms
      Replacers.Add(New BlogTokenReplace(Me, Settings, Post, t))
     Next
    Else
     For Each t As TermInfo In TermsController.GetTermsByModule(Settings.ModuleId, Locale)
      Replacers.Add(New BlogTokenReplace(Me, Settings, Nothing, t))
     Next
    End If
    _usePaging = False

   Case "allterms"

    For Each t As TermInfo In TermsController.GetTermsByModule(Settings.ModuleId, Locale)
     Replacers.Add(New BlogTokenReplace(Me, Settings, Nothing, t))
    Next
    _usePaging = False

   Case "keywords", "tags"

    If callingObject IsNot Nothing AndAlso TypeOf callingObject Is PostInfo Then
     For Each t As TermInfo In CType(callingObject, PostInfo).PostTags
      Replacers.Add(New BlogTokenReplace(Me, Settings, Post, t))
     Next
    ElseIf Post IsNot Nothing Then
     For Each t As TermInfo In Post.PostTags
      Replacers.Add(New BlogTokenReplace(Me, Settings, Post, t))
     Next
    Else
     For Each t As TermInfo In TermsController.GetTermsByModule(Settings.ModuleId, Locale).Where(Function(x) x.VocabularyId = 1).ToList
      Replacers.Add(New BlogTokenReplace(Me, Settings, Nothing, t))
     Next
    End If
    _usePaging = False

   Case "allkeywords", "alltags"

    For Each t As TermInfo In TermsController.GetTermsByModule(Settings.ModuleId, Locale).Where(Function(x) x.VocabularyId = 1).ToList
     Replacers.Add(New BlogTokenReplace(Me, Settings, Nothing, t))
    Next
    _usePaging = False

   Case "categories"

    If callingObject IsNot Nothing AndAlso TypeOf callingObject Is PostInfo Then
     For Each t As TermInfo In CType(callingObject, PostInfo).PostCategories
      Replacers.Add(New BlogTokenReplace(Me, Settings, Post, t))
     Next
    ElseIf Post IsNot Nothing Then
     For Each t As TermInfo In Post.PostCategories
      Replacers.Add(New BlogTokenReplace(Me, Settings, Post, t))
     Next
    Else
     For Each t As TermInfo In TermsController.GetTermsByModule(Settings.ModuleId, Locale).Where(Function(x) x.VocabularyId <> 1).ToList
      Replacers.Add(New BlogTokenReplace(Me, Settings, Nothing, t))
     Next
    End If
    _usePaging = False

   Case "allcategories"

    For Each t As TermInfo In TermsController.GetTermsByModule(Settings.ModuleId, Locale).Where(Function(x) x.VocabularyId <> 1).ToList
     Replacers.Add(New BlogTokenReplace(Me, Settings, Nothing, t))
    Next
    _usePaging = False

   Case "comments"

    If callingObject IsNot Nothing AndAlso TypeOf callingObject Is PostInfo Then
     For Each c As CommentInfo In CommentsController.GetCommentsByContentItem(CType(callingObject, PostInfo).ContentItemId, False)
      Replacers.Add(New BlogTokenReplace(Me, Settings, Post, c))
     Next
    ElseIf Post IsNot Nothing Then
     For Each c As CommentInfo In CommentsController.GetCommentsByContentItem(Post.ContentItemId, False)
      Replacers.Add(New BlogTokenReplace(Me, Settings, Post, c))
     Next
    Else
     For Each c As CommentInfo In CommentsController.GetCommentsByModule(ModuleId)
      Replacers.Add(New BlogTokenReplace(Me, Settings, Post, c))
     Next
    End If
    _usePaging = False

  End Select

 End Sub
#End Region

#Region " Post List Stuff "
 Private Property PostList As IEnumerable(Of PostInfo) = Nothing
 Private Sub EnsurePostList(pageSize As Integer)

  If PostList Is Nothing Then
   If pageSize < 1 Then pageSize = 10 ' we will not list "all Posts"
   If Not String.IsNullOrEmpty(SearchString) Then
    Dim publishValue As Integer = 1
    If SearchUnpublished Then publishValue = -1
    If Term Is Nothing Then
     PostList = PostsController.SearchPosts(Settings.ModuleId, BlogId, Locale, SearchString, SearchTitle, SearchContents, publishValue, ShowLocale, _endDate, -1, _reqPage - 1, pageSize, "PUBLISHEDONDATE DESC", _totalRecords, UserId, Security.UserIsAdmin).Values
    Else
     PostList = PostsController.SearchPostsByTerm(Settings.ModuleId, BlogId, Locale, TermId, SearchString, SearchTitle, SearchContents, publishValue, ShowLocale, _endDate, -1, _reqPage - 1, pageSize, "PUBLISHEDONDATE DESC", _totalRecords, UserId, Security.UserIsAdmin).Values
    End If
   ElseIf Term Is Nothing Then
    PostList = PostsController.GetPosts(Settings.ModuleId, BlogId, Locale, -1, ShowLocale, _endDate, AuthorId, _reqPage - 1, pageSize, "PUBLISHEDONDATE DESC", _totalRecords, UserId, Security.UserIsAdmin).Values
   Else
    PostList = PostsController.GetPostsByTerm(Settings.ModuleId, BlogId, Locale, TermId, -1, ShowLocale, _endDate, AuthorId, _reqPage - 1, pageSize, "PUBLISHEDONDATE DESC", _totalRecords, UserId, Security.UserIsAdmin).Values
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
   .DefaultReplacer = New BlogTokenReplace(Me, Settings)
  End With
  vtContents.DataBind()

  ctlComments.Visible = ViewSettings.ShowComments AndAlso Security.CanViewComments
  ctlComments.ClonePropertiesFrom(Me)

  ctlManagement.Visible = ViewSettings.ShowManagementPanel
  ctlManagement.ClonePropertiesFrom(Me)

 End Sub
#End Region

#Region " IActionable "
 Public ReadOnly Property ModuleActions As Actions.ModuleActionCollection Implements IActionable.ModuleActions
  Get
   Dim MyActions As New Actions.ModuleActionCollection
   If IsEditable Or Security.IsBlogger Then
    MyActions.Add(GetNextActionID, Localization.GetString(ModuleActionType.EditContent, LocalResourceFile), ModuleActionType.EditContent, "", "", EditUrl("Manage"), False, DotNetNuke.Security.SecurityAccessLevel.View, True, False)
   End If
   Return MyActions
  End Get
 End Property
#End Region

End Class