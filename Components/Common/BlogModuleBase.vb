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

Imports DotNetNuke.Web.Client.ClientResourceManagement
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Framework
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports DotNetNuke.Modules.Blog.Security
Imports DotNetNuke.Services.Tokens
Imports System.Linq

Namespace Common

 Public Class BlogModuleBase
  Inherits BlogContextBase

#Region " Private Members "
  Private fulllocs As List(Of String) = {"pt-br", "zh-cn", "zh-tw"}.ToList
  Private twoletterlocs As List(Of String) = {"ar", "bg", "bs", "ca", "cy", "cz", "da", "de", "el", "en", "es", "fa", "fi", "fr", "he", "hr", "hu", "hy", "id", "it", "ja", "ko", "mk", "nl", "no", "pl", "pt", "ro", "ru", "sk", "sv", "th", "tr", "uk", "uz"}.ToList
#End Region

#Region " Event Handlers "
  Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init

   jQuery.RequestUIRegistration()
   Dim script As New StringBuilder
   script.AppendLine("<script type=""text/javascript"">")
   script.AppendLine("//<![CDATA[")
   script.AppendLine(String.Format("var appPath='{0}'", DotNetNuke.Common.ApplicationPath))
   script.AppendLine("//]]>")
   script.AppendLine("</script>")
   UI.Utilities.ClientAPI.RegisterClientScriptBlock(Page, "blogAppPath", script.ToString)

   If ViewSettings.BlogModuleId = -1 Then
    Settings = ModuleSettings.GetModuleSettings(ModuleConfiguration.ModuleID)
   Else
    Settings = ModuleSettings.GetModuleSettings(ViewSettings.BlogModuleId)
   End If
   Locale = Threading.Thread.CurrentThread.CurrentCulture.Name

   Request.Params.ReadValue("Blog", BlogId)
   Request.Params.ReadValue("Post", ContentItemId)
   Request.Params.ReadValue("Term", TermId)
   Request.Params.ReadValue("Author", AuthorId)
   Request.Params.ReadValue("search", SearchString)
   Request.Params.ReadValue("t", SearchTitle)
   Request.Params.ReadValue("c", SearchContents)
   Request.Params.ReadValue("u", SearchUnpublished)
   ModuleUrls = New ModuleUrls(TabId, BlogId, ContentItemId, TermId, AuthorId)
   If ContentItemId > -1 Then Post = Entities.Posts.PostsController.GetPost(ContentItemId, Settings.ModuleId, Locale)
   If BlogId > -1 And Post IsNot Nothing AndAlso Post.BlogID <> BlogId Then Post = Nothing ' double check in case someone is hacking to retrieve an Post from another blog
   If BlogId = -1 And Post IsNot Nothing Then BlogId = Post.BlogID
   If BlogId > -1 Then Blog = Entities.Blogs.BlogsController.GetBlog(BlogId, UserInfo.UserID, Locale)
   If BlogId > -1 Then BlogMapPath = GetBlogDirectoryMapPath(BlogId)
   If BlogMapPath <> "" AndAlso Not IO.Directory.Exists(BlogMapPath) Then IO.Directory.CreateDirectory(BlogMapPath)
   If ContentItemId > -1 Then PostMapPath = GetPostDirectoryMapPath(BlogId, ContentItemId)
   If PostMapPath <> "" AndAlso Not IO.Directory.Exists(PostMapPath) Then IO.Directory.CreateDirectory(PostMapPath)
   If TermId > -1 Then Term = Entities.Terms.TermsController.GetTerm(TermId, Settings.ModuleId, Locale)
   If AuthorId > -1 Then Author = DotNetNuke.Entities.Users.UserController.GetUserById(PortalId, AuthorId)
   ' set urls for use in module
   Dim params As New List(Of String)
   If BlogId > -1 Then params.Add("Blog=" & BlogId.ToString)
   If ContentItemId > -1 Then params.Add("Post=" & ContentItemId.ToString)
   If TermId > -1 Then params.Add("Term=" & TermId.ToString)
   IsMultiLingualSite = CBool((New DotNetNuke.Services.Localization.LocaleController).GetLocales(PortalId).Count > 1)
   If Not ViewSettings.ShowAllLocales Then
    ShowLocale = Locale
   End If

   If ViewSettings.BlogModuleId = -1 Then
    AddBlogService()
    AddJavascriptFile("jquery.timeago.js")
    If fulllocs.Contains(Locale.ToLower) Then
     AddJavascriptFile("time-ago-locales/jquery.timeago." & Locale.ToLower & ".js")
    ElseIf twoletterlocs.Contains(Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower) Then
     AddJavascriptFile("time-ago-locales/jquery.timeago." & Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower & ".js")
    End If
   End If

  End Sub
#End Region

#Region " Public Methods "
  Public Sub AddBlogService()

   DotNetNuke.Framework.jQuery.RequestDnnPluginsRegistration()
   DotNetNuke.Framework.ServicesFramework.Instance.RequestAjaxScriptSupport()
   DotNetNuke.Framework.ServicesFramework.Instance.RequestAjaxAntiForgerySupport()
   AddJavascriptFile("dotnetnuke.blog.js")

   ' Load initialization snippet
   Dim scriptBlock As String = Common.Globals.ReadFile(DotNetNuke.Common.ApplicationMapPath & "\DesktopModules\Blog\js\dotnetnuke.blog.pagescript.js")
   Dim tr As New Templating.GenericTokenReplace(Scope.DefaultSettings, ModuleId)
   tr.AddResources(DotNetNuke.Common.ApplicationMapPath & "\DesktopModules\Blog\App_LocalResources\SharedResources.resx")
   scriptBlock = tr.ReplaceTokens(scriptBlock)
   scriptBlock = "<script type=""text/javascript"">" & vbCrLf & "//<![CDATA[" & vbCrLf & scriptBlock & vbCrLf & "//]]>" & vbCrLf & "</script>"
   Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "BlogServiceScript", scriptBlock)

  End Sub

  Public Sub AddJavascriptFile(jsFilename As String)
   ClientResourceManager.RegisterScript(Page, ResolveUrl("~/DesktopModules/Blog/js/" & jsFilename))
  End Sub

  Public Sub AddCssFile(cssFilename As String)
   ClientResourceManager.RegisterStyleSheet(Page, ResolveUrl("~/DesktopModules/Blog/css/" & cssFilename), Web.Client.FileOrder.Css.ModuleCss)
  End Sub
#End Region

 End Class

End Namespace