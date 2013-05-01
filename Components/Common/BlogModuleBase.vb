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
Imports DotNetNuke.Modules.Blog.Entities.Terms

Namespace Common

 Public Class BlogModuleBase
  Inherits PortalModuleBase

#Region " Private Members "
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

  Private _settings As ModuleSettings
  Public Shadows Property Settings() As ModuleSettings
   Get
    If _settings Is Nothing Then
     If ViewSettings.BlogModuleId = -1 Then
      _settings = ModuleSettings.GetModuleSettings(ModuleConfiguration.ModuleID)
     Else
      _settings = ModuleSettings.GetModuleSettings(ViewSettings.BlogModuleId)
     End If
    End If
    Return _settings
   End Get
   Set(ByVal value As ModuleSettings)
    _settings = value
   End Set
  End Property

  Private _categories As Dictionary(Of String, TermInfo)
  Public Property Categories() As Dictionary(Of String, TermInfo)
   Get
    If _categories Is Nothing Then
     _categories = TermsController.GetTermsByVocabulary(ModuleId, Settings.VocabularyId, BlogContext.Locale)
    End If
    Return _categories
   End Get
   Set(ByVal value As Dictionary(Of String, TermInfo))
    _categories = value
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

  Public Shadows ReadOnly Property Page As DotNetNuke.Framework.CDefault
   Get
    Return CType(MyBase.Page, DotNetNuke.Framework.CDefault)
   End Get
  End Property
#End Region

#Region " Event Handlers "
  Private Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

   jQuery.RequestRegistration()
   jQuery.RequestUIRegistration()
   Dim script As New StringBuilder
   script.AppendLine("<script type=""text/javascript"">")
   script.AppendLine("//<![CDATA[")
   script.AppendLine(String.Format("var appPath='{0}'", DotNetNuke.Common.ApplicationPath))
   script.AppendLine("//]]>")
   script.AppendLine("</script>")
   UI.Utilities.ClientAPI.RegisterClientScriptBlock(Page, "blogAppPath", script.ToString)

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
    Dim tr As New Templating.GenericTokenReplace(Scope.DefaultSettings, BlogContext.BlogModuleId)
    tr.AddResources(DotNetNuke.Common.ApplicationMapPath & "\DesktopModules\Blog\App_LocalResources\SharedResources.resx")
    scriptBlock = tr.ReplaceTokens(scriptBlock)
    scriptBlock = "<script type=""text/javascript"">" & vbCrLf & "//<![CDATA[" & vbCrLf & scriptBlock & vbCrLf & "//]]>" & vbCrLf & "</script>"
    Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "BlogServiceScript", scriptBlock)

    Context.Items("BlogServiceAdded") = True
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