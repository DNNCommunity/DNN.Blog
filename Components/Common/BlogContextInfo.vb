'
' Bring2mind - http://www.bring2mind.net
' Copyright (c) 2013
' by Bring2mind
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

Imports DotNetNuke.Web.Client.ClientResourceManagement
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Framework
Imports DotNetNuke.Modules.Blog.Security
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports DotNetNuke.Services.Tokens
Imports DotNetNuke.Modules.Blog.Entities.Terms

Namespace Common

 Public Class BlogContextInfo
  Implements IPropertyAccess

#Region " Private Members "
  Private Property RequestParams As NameValueCollection
#End Region

#Region " Public Methods "
  Public Sub New(context As HttpContext, blogModule As BlogModuleBase)

   BlogModuleId = blogModule.ModuleId

   ' Initialize values from View Settings
   If blogModule.ViewSettings.BlogModuleId <> -1 Then
    BlogModuleId = blogModule.ViewSettings.BlogModuleId
   End If
   BlogId = blogModule.ViewSettings.BlogId
   TermId = blogModule.ViewSettings.TermId
   AuthorId = blogModule.ViewSettings.AuthorId

   Locale = Threading.Thread.CurrentThread.CurrentCulture.Name
   If context.Request.UrlReferrer IsNot Nothing Then Referrer = context.Request.UrlReferrer.PathAndQuery
   RequestParams = context.Request.Params

   context.Request.Params.ReadValue("Blog", BlogId)
   context.Request.Params.ReadValue("Post", ContentItemId)
   context.Request.Params.ReadValue("Term", TermId)
   context.Request.Params.ReadValue("Author", AuthorId)
   context.Request.Params.ReadValue("end", EndDate)
   context.Request.Params.ReadValue("search", SearchString)
   context.Request.Params.ReadValue("t", SearchTitle)
   context.Request.Params.ReadValue("c", SearchContents)
   context.Request.Params.ReadValue("u", SearchUnpublished)
   context.Request.Params.ReadValue("EntryId", LegacyEntryId)
   If ContentItemId > -1 Then Post = Entities.Posts.PostsController.GetPost(ContentItemId, BlogModuleId, Locale)
   If BlogId > -1 And Post IsNot Nothing AndAlso Post.BlogID <> BlogId Then Post = Nothing ' double check in case someone is hacking to retrieve an Post from another blog
   If BlogId = -1 And Post IsNot Nothing Then BlogId = Post.BlogID
   If BlogId > -1 Then Blog = Entities.Blogs.BlogsController.GetBlog(BlogId, blogModule.UserInfo.UserID, Locale)
   If BlogId > -1 Then BlogMapPath = GetBlogDirectoryMapPath(BlogId)
   If BlogMapPath <> "" AndAlso Not IO.Directory.Exists(BlogMapPath) Then IO.Directory.CreateDirectory(BlogMapPath)
   If ContentItemId > -1 Then PostMapPath = GetPostDirectoryMapPath(BlogId, ContentItemId)
   If PostMapPath <> "" AndAlso Not IO.Directory.Exists(PostMapPath) Then IO.Directory.CreateDirectory(PostMapPath)
   If TermId > -1 Then Term = Entities.Terms.TermsController.GetTerm(TermId, BlogModuleId, Locale)
   If AuthorId > -1 Then Author = DotNetNuke.Entities.Users.UserController.GetUserById(blogModule.PortalId, AuthorId)
   If context.Request.UserAgent IsNot Nothing Then
    WLWRequest = CBool(context.Request.UserAgent.IndexOf("Windows Live Writer") > -1)
   End If
   Security = New ContextSecurity(BlogModuleId, blogModule.TabId, Blog, blogModule.UserInfo)
   If EndDate < Now.AddDays(-1) Then
    EndDate = EndDate.Date.AddDays(1).AddMinutes(-1)
   End If

   ' security
   Dim isStylePostRequest As Boolean = False
   If Post IsNot Nothing AndAlso Not (Post.Published Or Security.CanEditThisPost(Post)) AndAlso Not Security.IsEditor Then
    If Post.Title.Contains("3bfe001a-32de-4114-a6b4-4005b770f6d7") And WLWRequest Then
     isStylePostRequest = True
    Else
     Post = Nothing
     ContentItemId = -1
    End If
   End If
   If Blog IsNot Nothing AndAlso Not Blog.Published AndAlso Not Security.IsOwner AndAlso Not Security.UserIsAdmin AndAlso Not isStylePostRequest Then
    Blog = Nothing
    BlogId = -1
   End If

   ' set urls for use in module
   ModuleUrls = New ModuleUrls(blogModule.TabId, BlogId, ContentItemId, TermId, AuthorId)
   IsMultiLingualSite = CBool((New DotNetNuke.Services.Localization.LocaleController).GetLocales(blogModule.PortalId).Count > 1)
   If Not blogModule.ViewSettings.ShowAllLocales Then
    ShowLocale = Locale
   End If
   If Referrer.Contains("/ctl/") Or Referrer.Contains("&ctl=") Then
    Referrer = DotNetNuke.Common.NavigateURL(blogModule.TabId) ' just catch 99% of bad referrals to edit pages
   End If

   UiTimeZone = blogModule.ModuleContext.PortalSettings.TimeZone
   If blogModule.UserInfo.Profile.PreferredTimeZone IsNot Nothing Then
    UiTimeZone = blogModule.UserInfo.Profile.PreferredTimeZone
   End If

  End Sub

  Public Shared Function GetBlogContext(ByRef context As HttpContext, blogModule As BlogModuleBase) As BlogContextInfo
   Dim res As BlogContextInfo
   If context.Items("BlogContext") Is Nothing Then
    res = New BlogContextInfo(context, blogModule)
    context.Items("BlogContext") = res
   Else
    res = CType(context.Items("BlogContext"), BlogContextInfo)
   End If
   Return res
  End Function
#End Region

#Region " Public Properties "
  Public Property BlogModuleId As Integer = -1
  Public Property BlogId As Integer = -1
  Public Property ContentItemId As Integer = -1
  Public Property TermId As Integer = -1
  Public Property AuthorId As Integer = -1
  Public Property EndDate As Date = DateTime.Now
  Public Property Blog As Entities.Blogs.BlogInfo = Nothing
  Public Property Post As Entities.Posts.PostInfo = Nothing
  Public Property Term As Entities.Terms.TermInfo = Nothing
  Public Property Author As DotNetNuke.Entities.Users.UserInfo = Nothing
  Public Property BlogMapPath As String = ""
  Public Property PostMapPath As String = ""
  Public Property OutputAdditionalFiles As Boolean
  Public Property ModuleUrls As ModuleUrls = Nothing
  Public Property SearchString As String = ""
  Public Property SearchTitle As Boolean = True
  Public Property SearchContents As Boolean = False
  Public Property SearchUnpublished As Boolean = False
  Public Property IsMultiLingualSite As Boolean = False
  Public Property ShowLocale As String = ""
  Public Property Locale As String = ""
  Public Property Referrer As String = ""
  Public Property WLWRequest As Boolean = False
  Public Property UiTimeZone As TimeZoneInfo
  Public Property Security As ContextSecurity
  Public Property LegacyEntryId As Integer = -1
#End Region

#Region " IPropertyAccess Implementation "
  Public Function GetProperty(strPropertyName As String, strFormat As String, formatProvider As System.Globalization.CultureInfo, AccessingUser As DotNetNuke.Entities.Users.UserInfo, AccessLevel As DotNetNuke.Services.Tokens.Scope, ByRef PropertyNotFound As Boolean) As String Implements DotNetNuke.Services.Tokens.IPropertyAccess.GetProperty
   Dim OutputFormat As String = String.Empty
   Dim portalSettings As DotNetNuke.Entities.Portals.PortalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings()
   If strFormat = String.Empty Then
    OutputFormat = "D"
   Else
    OutputFormat = strFormat
   End If
   Select Case strPropertyName.ToLower
    Case "blogmoduleid"
     Return (Me.BlogModuleId.ToString(OutputFormat, formatProvider))
    Case "blogid"
     Return (Me.BlogId.ToString(OutputFormat, formatProvider))
    Case "Postid", "contentitemid", "postid", "post"
     Return (Me.ContentItemId.ToString(OutputFormat, formatProvider))
    Case "termid", "term"
     Return (Me.TermId.ToString(OutputFormat, formatProvider))
    Case "authorid", "author"
     Return (Me.AuthorId.ToString(OutputFormat, formatProvider))
    Case "enddate"
     Return (Me.EndDate.ToString(OutputFormat, formatProvider))
    Case "blogselected"
     Return CBool(BlogId > -1).ToString()
    Case "postselected"
     Return CBool(ContentItemId > -1).ToString()
    Case "termselected"
     Return CBool(TermId > -1).ToString()
    Case "authorselected"
     Return CBool(AuthorId > -1).ToString()
    Case "ismultilingualsite"
     Return IsMultiLingualSite.ToString()
    Case "showlocale"
     Return ShowLocale
    Case "locale"
     Select Case strFormat.ToLower
      Case "3"
       Return Threading.Thread.CurrentThread.CurrentCulture.ThreeLetterISOLanguageName
      Case "ietf"
       Return Threading.Thread.CurrentThread.CurrentCulture.IetfLanguageTag
      Case "displayname", "display"
       Return Threading.Thread.CurrentThread.CurrentCulture.DisplayName
      Case "englishname", "english"
       Return Threading.Thread.CurrentThread.CurrentCulture.EnglishName
      Case "nativename", "native"
       Return Threading.Thread.CurrentThread.CurrentCulture.NativeName
      Case "generic", "2"
       Return Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName
      Case Else
       Return Locale
     End Select
    Case "searchstring"
     Return SearchString
    Case "issearch"
     Return CBool(SearchString <> "").ToString()
    Case "referrer"
     Return Referrer
    Case Else
     If RequestParams(strPropertyName) IsNot Nothing Then
      Return RequestParams(strPropertyName)
     Else
      PropertyNotFound = True
     End If
   End Select
   Return DotNetNuke.Common.Utilities.Null.NullString
  End Function

  Public ReadOnly Property Cacheability() As DotNetNuke.Services.Tokens.CacheLevel Implements DotNetNuke.Services.Tokens.IPropertyAccess.Cacheability
   Get
    Return CacheLevel.fullyCacheable
   End Get
  End Property
#End Region

 End Class

End Namespace