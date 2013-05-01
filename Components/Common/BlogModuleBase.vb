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
  Inherits PortalModuleBase

#Region " Private Members "
  Private fulllocs As List(Of String) = {"pt-br", "zh-cn", "zh-tw"}.ToList
  Private twoletterlocs As List(Of String) = {"ar", "bg", "bs", "ca", "cy", "cz", "da", "de", "el", "en", "es", "fa", "fi", "fr", "he", "hr", "hu", "hy", "id", "it", "ja", "ko", "mk", "nl", "no", "pl", "pt", "ro", "ru", "sk", "sv", "th", "tr", "uk", "uz"}.ToList
#End Region

#Region " Properties "
  Private _blogContext As BlogContextInfo
  Public Property BlogContext() As BlogContextInfo
   Get
    If _blogContext Is Nothing Then
     _blogContext = BlogContextInfo.GetBlogContext(Context, Me)
    End If
    Return _blogContext
   End Get
   Set(ByVal value As BlogContextInfo)
    _blogContext = value
   End Set
  End Property

  Private _viewSettings As ViewSettings
  Public Property ViewSettings() As ViewSettings
   Get
    If _viewSettings Is Nothing Then _viewSettings = ViewSettings.GetViewSettings(TabModuleId)
    Return _viewSettings
   End Get
   Set(ByVal value As ViewSettings)
    _viewSettings = value
   End Set
  End Property
#End Region

#Region " Event Handlers "
  Private Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

   jQuery.RequestUIRegistration()
   Dim script As New StringBuilder
   script.AppendLine("<script type=""text/javascript"">")
   script.AppendLine("//<![CDATA[")
   script.AppendLine(String.Format("var appPath='{0}'", DotNetNuke.Common.ApplicationPath))
   script.AppendLine("//]]>")
   script.AppendLine("</script>")
   UI.Utilities.ClientAPI.RegisterClientScriptBlock(Page, "blogAppPath", script.ToString)

   ' wlw style detection post redirect?
   If Not String.IsNullOrEmpty(BlogContext.Settings.StyleDetectionUrl) And BlogContext.WLWRequest Then
    ' we have a style detection post in storage and it's being requested
    Dim url As String = BlogContext.Settings.StyleDetectionUrl
    BlogContext.Settings.StyleDetectionUrl = ""
    BlogContext.Settings.UpdateSettings()
    Response.Redirect(url, False)
   End If

   If ViewSettings.BlogModuleId = -1 Then
    AddBlogService()
    AddJavascriptFile("jquery.timeago.js", 60)
    If fulllocs.Contains(BlogContext.Locale.ToLower) Then
     AddJavascriptFile("time-ago-locales/jquery.timeago." & BlogContext.Locale.ToLower & ".js", 60)
    ElseIf twoletterlocs.Contains(Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower) Then
     AddJavascriptFile("time-ago-locales/jquery.timeago." & Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower & ".js", 61)
    End If
    If Not Me.IsPostBack And BlogContext.ContentItemId > -1 Then
     Dim scriptBlock As String = "(function ($, Sys) {$(document).ready(function () {blogService.viewPost(" & BlogContext.BlogId.ToString & ", " & BlogContext.ContentItemId.ToString & ")});} (jQuery, window.Sys));"
     Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "PostViewScript", scriptBlock, True)
    End If
   End If

  End Sub
#End Region

#Region " Public Methods "
  Public Sub AddBlogService()

   If Context.Items("BlogServiceAdded") Is Nothing Then

    DotNetNuke.Framework.jQuery.RequestDnnPluginsRegistration()
    DotNetNuke.Framework.ServicesFramework.Instance.RequestAjaxScriptSupport()
    DotNetNuke.Framework.ServicesFramework.Instance.RequestAjaxAntiForgerySupport()
    AddJavascriptFile("dotnetnuke.blog.js", 70)

    ' Load initialization snippet
    Dim scriptBlock As String = Common.Globals.ReadFile(DotNetNuke.Common.ApplicationMapPath & "\DesktopModules\Blog\js\dotnetnuke.blog.pagescript.js")
    Dim tr As New Templating.GenericTokenReplace(Scope.DefaultSettings, ModuleId)
    tr.AddResources(DotNetNuke.Common.ApplicationMapPath & "\DesktopModules\Blog\App_LocalResources\SharedResources.resx")
    scriptBlock = tr.ReplaceTokens(scriptBlock)
    scriptBlock = "<script type=""text/javascript"">" & vbCrLf & "//<![CDATA[" & vbCrLf & scriptBlock & vbCrLf & "//]]>" & vbCrLf & "</script>"
    Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "BlogServiceScript", scriptBlock)

    Context.Items("BlogServiceAdded") = True
   End If

  End Sub

  Public Sub AddWLWManifestLink()
   If Context.Items("WLWManifestLinkAdded") Is Nothing Then
    Dim link As New HtmlGenericControl("link")
    link.Attributes.Add("rel", "wlwmanifest")
    link.Attributes.Add("type", "application/wlwmanifest+xml")
    link.Attributes.Add("href", ResolveUrl(ManifestFilePath(TabId, ModuleId)))
    Me.Page.Header.Controls.Add(link)
    Context.Items("WLWManifestLinkAdded") = True
   End If
  End Sub

  Public Sub AddPingBackLink()
   If Context.Items("PingBackLinkAdded") Is Nothing Then
    Dim link As New HtmlGenericControl("link")
    link.Attributes.Add("rel", "pingback")
    link.Attributes.Add("href", "") ' url to service
    Me.Page.Header.Controls.Add(link)
    Context.Items("PingBackLinkAdded") = True
   End If
  End Sub

  Public Sub AddTrackBackBlurb()
   If Context.Items("TrackBackBlurbAdded") Is Nothing Then
    Dim sb As New StringBuilder
    sb.AppendLine("<!--")
    sb.AppendLine(" <rdf:RDF xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#""")
    sb.AppendLine("  xmlns:dc=""http://purl.org/dc/elements/1.1/""")
    sb.AppendLine("  xmlns:trackback=""http://madskills.com/public/xml/rss/module/trackback/"">")
    sb.AppendFormat("  <rdf:Description rdf:about=""{0}""" & vbCrLf, "") ' about
    sb.AppendFormat("  dc:identifier=""{0}"" dc:Title=""{1}""" & vbCrLf, "") ' trackback url
    sb.AppendFormat("  trackback:ping=""{0}"" />" & vbCrLf, "") ' trackback url
    sb.AppendLine(" </rdf:RDF>")
    sb.AppendLine("-->")
    Context.Items("TrackBackBlurbAdded") = True
   End If
  End Sub

  Public Sub AddJavascriptFile(jsFilename As String, priority As Integer)
   ClientResourceManager.RegisterScript(Page, ResolveUrl("~/DesktopModules/Blog/js/" & jsFilename), priority)
  End Sub

  Public Sub AddCssFile(cssFilename As String)
   ClientResourceManager.RegisterStyleSheet(Page, ResolveUrl("~/DesktopModules/Blog/css/" & cssFilename), Web.Client.FileOrder.Css.ModuleCss)
  End Sub
#End Region

 End Class

End Namespace