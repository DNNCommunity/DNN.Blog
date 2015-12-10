'
' DNN Connect - http://dnn-connect.org
' Copyright (c) 2015
' by DNN Connect
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

Imports System.Linq
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Modules.Blog.Common.Globals
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
        Integration.BlogModuleController.CheckupOnImportedFiles(ModuleId)
        ctlComments.ModuleConfiguration = Me.ModuleConfiguration
        ctlComments.BlogContext = Me.BlogContext
        ctlManagement.ModuleConfiguration = Me.ModuleConfiguration
        ctlManagement.BlogContext = Me.BlogContext
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        ViewSettings.TemplateSettings.ReadValue("pagesize", _pageSize)
        Me.Request.Params.ReadValue("Page", _reqPage)

        If Context.Items("BlogPageInitialized") Is Nothing Then

            ' wlw style detection post redirect?
            If Not String.IsNullOrEmpty(Settings.StyleDetectionUrl) And BlogContext.WLWRequest Then
                ' we have a style detection post in storage and it's being requested
                Dim url As String = Settings.StyleDetectionUrl
                Settings.StyleDetectionUrl = ""
                Settings.UpdateSettings()
                Response.Redirect(url, False)
            End If

            If BlogContext.ContentItemId = -1 AndAlso BlogContext.LegacyEntryId > -1 Then
                ' we have a legacy url
                Dim p As PostInfo = PostsController.GetPostByLegacyEntryId(BlogContext.LegacyEntryId, PortalId, BlogContext.Locale)
                If p IsNot Nothing Then
                    Response.RedirectPermanent(p.PermaLink(PortalSettings), False)
                End If
            End If

            If Not Me.IsPostBack And BlogContext.ContentItemId > -1 Then
                Dim viewCountTimeout As Integer = Settings.IncrementViewCount * 1000 'in milliseconds
                Dim scriptBlock As String = "(function ($, Sys) {$(document).ready(function () {setTimeout(function(){blogService.viewPost(" & BlogContext.BlogId.ToString & ", " & BlogContext.ContentItemId.ToString & ")}," & viewCountTimeout.ToString & ")});} (jQuery, window.Sys));"
                Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "PostViewScript", scriptBlock, True)
            End If

            AddWLWManifestLink()

            If Settings.ModifyPageDetails OrElse ViewSettings.ModifyPageDetails Then
                ' force modify on all modules orlse modify on selected modules only?
                If BlogContext.Post IsNot Nothing Then
                    Page.Title = BlogContext.Post.LocalizedTitle
                    Page.Description = DotNetNuke.Common.Utilities.HtmlUtils.Clean(BlogContext.Post.LocalizedSummary, False)
                    Page.KeyWords = String.Join(",", BlogContext.Post.Terms.ToStringArray)
                    'AddOpenGraphMetaTags()
                ElseIf BlogContext.Blog IsNot Nothing Then
                    Page.Title = BlogContext.Blog.LocalizedTitle
                    Page.Description = DotNetNuke.Common.Utilities.HtmlUtils.Clean(BlogContext.Blog.LocalizedDescription, False)
                ElseIf BlogContext.Author IsNot Nothing Then
                    Page.Title = BlogContext.Author.DisplayName
                    Page.Description = DotNetNuke.Common.Utilities.HtmlUtils.Clean(BlogContext.Author.Profile.Biography, False)
                ElseIf BlogContext.Term IsNot Nothing Then
                    Page.Title = BlogContext.Term.LocalizedName
                    Page.Description = DotNetNuke.Common.Utilities.HtmlUtils.Clean(BlogContext.Term.LocalizedDescription, False)
                End If

                If _reqPage > 1 Then
                    Page.Title = String.Format(Localization.GetString("PageTitle.Format", LocalResourceFile), Page.Title, _reqPage)
                End If

            End If

            If BlogContext.Post IsNot Nothing AndAlso BlogContext.Blog IsNot Nothing Then
                AddOpenGraphMetaTags()
                If BlogContext.Blog.EnablePingBackReceive Then
                    AddPingBackLink()
                End If
                If BlogContext.Blog.EnableTrackBackReceive Then
                    AddTrackBackBlurb()
                End If
            End If

            Context.Items("BlogPageInitialized") = True
        End If

        DataBind()

    End Sub
#End Region

#Region " Open Graph Meta Tags "
    Private Sub AddOpenGraphMetaTags()
        Dim URL As String = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host
        Page.Header.Controls.Add(New LiteralControl(String.Format("<meta property=""og:title"" content=""{0}"" />", CleanStringForXmlAttribute(BlogContext.Post.LocalizedTitle))))
        Page.Header.Controls.Add(New LiteralControl(String.Format("<meta property=""og:site_name"" content=""{0}"" />", CleanStringForXmlAttribute(PortalSettings.PortalName))))
        Page.Header.Controls.Add(New LiteralControl(String.Format("<meta property=""og:type"" content=""{0}"" />", "article")))
        If Not String.IsNullOrEmpty(BlogContext.Post.Locale) Then
            Page.Header.Controls.Add(New LiteralControl(String.Format("<meta property=""og:locale"" content=""{0}"" />", BlogContext.Post.Locale.Replace("-", "_"))))
        ElseIf Not String.IsNullOrEmpty(BlogContext.Blog.Locale) Then
            Page.Header.Controls.Add(New LiteralControl(String.Format("<meta property=""og:locale"" content=""{0}"" />", BlogContext.Blog.Locale.Replace("-", "_"))))
        Else
            Page.Header.Controls.Add(New LiteralControl(String.Format("<meta property=""og:locale"" content=""{0}"" />", PortalSettings.DefaultLanguage.Replace("-", "_"))))
        End If
        Page.Header.Controls.Add(New LiteralControl(String.Format("<meta property=""og:updated_time"" content=""{0}"" />", BlogContext.Post.LastModifiedOnDate.ToString("u"))))
        Page.Header.Controls.Add(New LiteralControl(String.Format("<meta property=""og:url"" content=""{0}"" />", BlogContext.Post.PermaLink)))
        Page.Header.Controls.Add(New LiteralControl(String.Format("<meta property=""og:description"" content=""{0}"" />", CleanStringForXmlAttribute(DotNetNuke.Common.Utilities.HtmlUtils.Clean(BlogContext.Post.LocalizedSummary, False)))))
        If Not String.IsNullOrEmpty(BlogContext.Post.Image) Then
            Dim strPath As String = String.Format("{0}?TabId={1}&ModuleId={2}&Blog={3}&Post={4}&w=1200&h=630&c=1&key={5}", glbImageHandlerPath, TabId.ToString, Settings.ModuleId.ToString, BlogContext.BlogId.ToString, BlogContext.ContentItemId.ToString, BlogContext.Post.Image)
            Page.Header.Controls.Add(New LiteralControl(String.Format("<meta property=""og:image"" content=""{0}"" />", URL + ResolveUrl(strPath))))
        End If
        If Not String.IsNullOrEmpty(Settings.FacebookAppId) Then
            Page.Header.Controls.Add(New LiteralControl(String.Format("<meta property=""fb:app_id"" content=""{0}"" />", Settings.FacebookAppId)))
        End If
        If Settings.FacebookProfileIdProperty <> -1 Then
            Dim pp As DotNetNuke.Entities.Profile.ProfilePropertyDefinition = BlogContext.Author.Profile.ProfileProperties.GetById(Settings.FacebookProfileIdProperty)
            If pp IsNot Nothing AndAlso Not String.IsNullOrEmpty(pp.PropertyValue) Then
                Page.Header.Controls.Add(New LiteralControl(String.Format("<meta property=""fb:profile_id"" content=""{0}"" />", pp.PropertyValue)))
            End If
        End If
    End Sub
#End Region

#Region " Public Methods "
    Private Sub AddWLWManifestLink()
        If Context.Items("WLWManifestLinkAdded") Is Nothing Then
            Dim link As New HtmlLink()
            link.Attributes.Add("rel", "wlwmanifest")
            link.Attributes.Add("type", "application/wlwmanifest+xml")
            If ViewSettings.BlogModuleId = -1 Then
                link.Attributes.Add("href", ResolveUrl(ManifestFilePath(TabId, ModuleId)))
            Else
                link.Attributes.Add("href", ResolveUrl(ManifestFilePath(TabId, ViewSettings.BlogModuleId)))
            End If
            Me.Page.Header.Controls.Add(link)
            Context.Items("WLWManifestLinkAdded") = True
        End If
    End Sub

    Private Sub AddPingBackLink()
        If Context.Items("PingBackLinkAdded") Is Nothing Then
            Dim pingbackUrl As String = Services.BlogRouteMapper.GetRoute(Services.BlogRouteMapper.ServiceControllers.Comments, "Pingback")
            pingbackUrl &= String.Format("?tabId={0}&moduleId={1}&blogId={2}&postId={3}", TabId, BlogContext.BlogModuleId, BlogContext.BlogId, BlogContext.ContentItemId)
            Dim link As New HtmlGenericControl("link")
            link.Attributes.Add("rel", "pingback")
            link.Attributes.Add("href", pingbackUrl)
            Me.Page.Header.Controls.Add(link)
            Context.Items("PingBackLinkAdded") = True
        End If
    End Sub

    Private Sub AddTrackBackBlurb()
        If Context.Items("TrackBackBlurbAdded") Is Nothing Then
            Dim trackbackUrl As String = Services.BlogRouteMapper.GetRoute(Services.BlogRouteMapper.ServiceControllers.Comments, "Trackback")
            trackbackUrl &= String.Format("?tabId={0}&moduleId={1}&blogId={2}&postId={3}", TabId, BlogContext.BlogModuleId, BlogContext.BlogId, BlogContext.ContentItemId)
            Dim postUrl As String = BlogContext.Post.PermaLink(PortalSettings)
            Dim sb As New StringBuilder
            sb.AppendLine("<!--")
            sb.AppendLine(" <rdf:RDF xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#""")
            sb.AppendLine("  xmlns:dc=""http://purl.org/dc/elements/1.1/""")
            sb.AppendLine("  xmlns:trackback=""http://madskills.com/public/xml/rss/module/trackback/"">")
            sb.AppendFormat("  <rdf:Description rdf:about=""{0}""" & vbCrLf, postUrl)
            sb.AppendFormat("  dc:identifier=""{0}"" dc:Title=""{1}""" & vbCrLf, postUrl, BlogContext.Post.LocalizedTitle)
            sb.AppendFormat("  trackback:ping=""{0}"" />" & vbCrLf, trackbackUrl) ' trackback url
            sb.AppendLine(" </rdf:RDF>")
            sb.AppendLine("-->")
            litTrackback.Text = sb.ToString
            Context.Items("TrackBackBlurbAdded") = True
        End If
    End Sub
#End Region

#Region " Template Data Retrieval "
    Private Sub vtContents_GetData(ByVal DataSource As String, ByVal Parameters As Dictionary(Of String, String), ByRef Replacers As System.Collections.Generic.List(Of GenericTokenReplace), ByRef Arguments As System.Collections.Generic.List(Of String()), callingObject As Object) Handles vtContents.GetData

        Select Case DataSource.ToLower

            Case "blogs"

                Dim blogList As IEnumerable(Of BlogInfo) = BlogsController.GetBlogsByModule(BlogContext.BlogModuleId, UserId, BlogContext.Locale).Values.Where(Function(b) b.Published = True).OrderBy(Function(b) b.Title)
                Parameters.ReadValue("pagesize", _pageSize)
                If _pageSize > 0 Then
                    _usePaging = True
                    Dim startRec As Integer = ((_reqPage - 1) * _pageSize) + 1
                    Dim endRec As Integer = _reqPage * _pageSize
                    Dim i As Integer = 1
                    For Each b As BlogInfo In blogList
                        If i >= startRec And i <= endRec Then
                            If BlogContext.ParentModule IsNot Nothing Then
                                b.ParentTabID = BlogContext.ParentModule.TabID
                            End If
                            Replacers.Add(New BlogTokenReplace(Me, b))
                        End If
                        i += 1
                    Next
                Else
                    For Each b As BlogInfo In blogList
                        If BlogContext.ParentModule IsNot Nothing Then
                            b.ParentTabID = BlogContext.ParentModule.TabID
                        End If
                        Replacers.Add(New BlogTokenReplace(Me, b))
                    Next
                End If

            Case "posts"

                Parameters.ReadValue("pagesize", _pageSize)
                EnsurePostList(_pageSize)
                For Each e As PostInfo In PostList
                    If BlogContext.ParentModule IsNot Nothing Then
                        e.ParentTabID = BlogContext.ParentModule.TabID
                    End If
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
                    For Each t As TermInfo In TermsController.GetTermsByModule(BlogContext.BlogModuleId, BlogContext.Locale)
                        Replacers.Add(New BlogTokenReplace(Me, Nothing, t))
                    Next
                End If
                _usePaging = False

            Case "allterms"

                For Each t As TermInfo In TermsController.GetTermsByModule(BlogContext.BlogModuleId, BlogContext.Locale)
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
                    For Each t As TermInfo In TermsController.GetTermsByModule(BlogContext.BlogModuleId, BlogContext.Locale).Where(Function(x) x.VocabularyId = 1).ToList
                        If BlogContext.ParentModule IsNot Nothing Then
                            t.ParentTabID = BlogContext.ParentModule.TabID
                        End If
                        Replacers.Add(New BlogTokenReplace(Me, Nothing, t))
                    Next
                End If
                _usePaging = False

            Case "allkeywords", "alltags"

                For Each t As TermInfo In TermsController.GetTermsByModule(BlogContext.BlogModuleId, BlogContext.Locale).Where(Function(x) x.VocabularyId = 1).ToList
                    If BlogContext.ParentModule IsNot Nothing Then
                        t.ParentTabID = BlogContext.ParentModule.TabID
                    End If
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
                ElseIf ViewSettings.Categories = "" Then
                    For Each t As TermInfo In TermsController.GetTermsByModule(BlogContext.BlogModuleId, BlogContext.Locale).Where(Function(x) x.VocabularyId <> 1).ToList
                        Replacers.Add(New BlogTokenReplace(Me, Nothing, t))
                    Next
                Else
                    For Each t As TermInfo In TermsController.GetTermsByModule(BlogContext.BlogModuleId, BlogContext.Locale).Where(Function(x) ViewSettings.CategoryList.Contains(x.VocabularyId)).ToList
                        Replacers.Add(New BlogTokenReplace(Me, Nothing, t))
                    Next
                End If
                _usePaging = False

            Case "allcategories"

                For Each t As TermInfo In TermsController.GetTermsByVocabulary(BlogContext.BlogModuleId, Settings.VocabularyId, BlogContext.Locale).Values
                    If BlogContext.ParentModule IsNot Nothing Then
                        t.ParentTabID = BlogContext.ParentModule.TabID
                    End If
                    Replacers.Add(New BlogTokenReplace(Me, Nothing, t))
                Next
                _usePaging = False

            Case "selectcategories"

                For Each t As TermInfo In TermsController.GetTermsByModule(BlogContext.BlogModuleId, BlogContext.Locale).Where(Function(x) ViewSettings.CategoryList.Contains(x.TermId)).ToList
                    If BlogContext.ParentModule IsNot Nothing Then
                        t.ParentTabID = BlogContext.ParentModule.TabID
                    End If
                    Replacers.Add(New BlogTokenReplace(Me, Nothing, t))
                Next
                _usePaging = False

            Case "comments"

                If callingObject IsNot Nothing AndAlso TypeOf callingObject Is PostInfo Then
                    For Each c As CommentInfo In CommentsController.GetCommentsByContentItem(CType(callingObject, PostInfo).ContentItemId, False, UserId)
                        Replacers.Add(New BlogTokenReplace(Me, BlogContext.Post, c))
                    Next
                    _usePaging = False
                ElseIf BlogContext.Post IsNot Nothing Then
                    For Each c As CommentInfo In CommentsController.GetCommentsByContentItem(BlogContext.Post.ContentItemId, False, UserId)
                        Replacers.Add(New BlogTokenReplace(Me, BlogContext.Post, c))
                    Next
                    _usePaging = False
                Else
                    Parameters.ReadValue("pagesize", _pageSize)
                    If _pageSize < 1 Then _pageSize = 10 ' we will not list "all Posts"
                    For Each c As CommentInfo In CommentsController.GetCommentsByModule(BlogContext.BlogModuleId, UserId, _reqPage - 1, _pageSize, "CREATEDONDATE DESC", _totalRecords).Values
                        Replacers.Add(New BlogTokenReplace(Me, BlogContext.Post, c))
                    Next
                End If

            Case "allcomments"

                Parameters.ReadValue("pagesize", _pageSize)
                Dim loadPosts As Boolean = False
                Parameters.ReadValue("loadposts", loadPosts)
                If _pageSize < 1 Then _pageSize = 10 ' we will not list "all Posts"
                For Each c As CommentInfo In CommentsController.GetCommentsByModule(BlogContext.BlogModuleId, UserId, _reqPage - 1, _pageSize, "CREATEDONDATE DESC", _totalRecords).Values
                    If loadPosts Then
                        Replacers.Add(New BlogTokenReplace(Me, PostsController.GetPost(c.ContentItemId, BlogContext.BlogModuleId, BlogContext.Locale), c))
                    Else
                        Replacers.Add(New BlogTokenReplace(Me, Nothing, c))
                    End If
                Next

            Case "calendar", "blogcalendar"

                For Each bci As BlogCalendarInfo In BlogsController.GetBlogCalendar(BlogContext.BlogModuleId, BlogContext.BlogId, BlogContext.ShowLocale)
                    If BlogContext.ParentModule IsNot Nothing Then
                        bci.ParentTabID = BlogContext.ParentModule.TabID
                    End If
                    Replacers.Add(New BlogTokenReplace(Me, bci))
                Next

            Case "authors", "allauthors"

                Dim blogToShow As Integer = BlogContext.BlogId
                If DataSource.ToLower = "allauthors" Then blogToShow = -1
                Dim sort As String = ""
                Parameters.ReadValue("sort", sort)
                Select Case sort.ToLower
                    Case "username"
                        For Each u As PostAuthor In PostsController.GetAuthors(BlogContext.BlogModuleId, blogToShow).OrderBy(Function(t) t.Username)
                            Replacers.Add(New BlogTokenReplace(Me, New LazyLoadingUser(u)))
                        Next
                    Case "email"
                        For Each u As PostAuthor In PostsController.GetAuthors(BlogContext.BlogModuleId, blogToShow).OrderBy(Function(t) t.Email)
                            Replacers.Add(New BlogTokenReplace(Me, New LazyLoadingUser(u)))
                        Next
                    Case "firstname"
                        For Each u As PostAuthor In PostsController.GetAuthors(BlogContext.BlogModuleId, blogToShow).OrderBy(Function(t) t.FirstName)
                            Replacers.Add(New BlogTokenReplace(Me, New LazyLoadingUser(u)))
                        Next
                    Case "displayname"
                        For Each u As PostAuthor In PostsController.GetAuthors(BlogContext.BlogModuleId, blogToShow).OrderBy(Function(t) t.DisplayName)
                            Replacers.Add(New BlogTokenReplace(Me, New LazyLoadingUser(u)))
                        Next
                    Case Else ' last name
                        For Each u As PostAuthor In PostsController.GetAuthors(BlogContext.BlogModuleId, blogToShow)
                            If BlogContext.ParentModule IsNot Nothing Then
                                u.ParentTabID = BlogContext.ParentModule.TabID
                            End If
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
            Dim publishValue As Integer = 1
            If Not String.IsNullOrEmpty(BlogContext.SearchString) Then
                If BlogContext.SearchUnpublished Then publishValue = -1
                If String.IsNullOrEmpty(BlogContext.Categories) Then
                    If BlogContext.Term Is Nothing Then
                        PostList = PostsController.SearchPosts(Settings.ModuleId, BlogContext.BlogId, BlogContext.Locale, BlogContext.SearchString, BlogContext.SearchTitle, BlogContext.SearchContents, publishValue, BlogContext.ShowLocale, BlogContext.EndDate, -1, _reqPage - 1, pageSize, "PUBLISHEDONDATE DESC", _totalRecords, UserId, BlogContext.Security.UserIsAdmin).Values
                    Else
                        PostList = PostsController.SearchPostsByTerm(Settings.ModuleId, BlogContext.BlogId, BlogContext.Locale, BlogContext.TermId, BlogContext.SearchString, BlogContext.SearchTitle, BlogContext.SearchContents, publishValue, BlogContext.ShowLocale, BlogContext.EndDate, -1, _reqPage - 1, pageSize, "PUBLISHEDONDATE DESC", _totalRecords, UserId, BlogContext.Security.UserIsAdmin).Values
                    End If
                Else
                    PostList = PostsController.SearchPostsByCategory(Settings.ModuleId, BlogContext.BlogId, BlogContext.Locale, BlogContext.Categories, BlogContext.SearchString, BlogContext.SearchTitle, BlogContext.SearchContents, publishValue, BlogContext.ShowLocale, BlogContext.EndDate, -1, _reqPage - 1, pageSize, "PUBLISHEDONDATE DESC", _totalRecords, UserId, BlogContext.Security.UserIsAdmin).Values
                End If
            ElseIf String.IsNullOrEmpty(BlogContext.Categories) Then
                publishValue = -1
                If ViewSettings.HideUnpublishedBlogsViewMode AndAlso PortalSettings.UserMode = PortalSettings.Mode.View Then publishValue = 1
                If ViewSettings.HideUnpublishedBlogsEditMode AndAlso PortalSettings.UserMode = PortalSettings.Mode.Edit Then publishValue = 1
                If BlogContext.Term Is Nothing Then
                    PostList = PostsController.GetPosts(Settings.ModuleId, BlogContext.BlogId, BlogContext.Locale, publishValue, BlogContext.ShowLocale, BlogContext.EndDate, BlogContext.AuthorId, False, _reqPage - 1, pageSize, "PUBLISHEDONDATE DESC", _totalRecords, UserId, BlogContext.Security.UserIsAdmin).Values
                Else
                    PostList = PostsController.GetPostsByTerm(Settings.ModuleId, BlogContext.BlogId, BlogContext.Locale, BlogContext.TermId, publishValue, BlogContext.ShowLocale, BlogContext.EndDate, BlogContext.AuthorId, _reqPage - 1, pageSize, "PUBLISHEDONDATE DESC", _totalRecords, UserId, BlogContext.Security.UserIsAdmin).Values
                End If
            Else
                publishValue = -1
                If ViewSettings.HideUnpublishedBlogsViewMode AndAlso PortalSettings.UserMode = PortalSettings.Mode.View Then publishValue = 1
                If ViewSettings.HideUnpublishedBlogsEditMode AndAlso PortalSettings.UserMode = PortalSettings.Mode.Edit Then publishValue = 1
                PostList = PostsController.GetPostsByCategory(Settings.ModuleId, BlogContext.BlogId, BlogContext.Locale, BlogContext.Categories, publishValue, BlogContext.ShowLocale, BlogContext.EndDate, BlogContext.AuthorId, _reqPage - 1, pageSize, "PUBLISHEDONDATE DESC", _totalRecords, UserId, BlogContext.Security.UserIsAdmin).Values
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
            .TemplateRelPath = tmgr.TemplateRelPath
            .TemplateMapPath = tmgr.TemplateMapPath
            .DefaultReplacer = New BlogTokenReplace(Me)
        End With
        vtContents.DataBind()

        ctlComments.Visible = CBool(ViewSettings.BlogModuleId = -1) AndAlso BlogContext.Security.CanViewComments
        ctlManagement.Visible = CBool(ViewSettings.BlogModuleId = -1) OrElse ViewSettings.ShowManagementPanel

        If PortalSettings.UserMode = PortalSettings.Mode.View AndAlso ViewSettings.ShowManagementPanelViewMode = False Then
            ctlManagement.Visible = False
        End If

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
