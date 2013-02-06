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

Namespace Common

 Public Class BlogModuleBase
  Inherits PortalModuleBase

#Region "Public Constants"

  Public Const RSS_RECENT_ENTRIES As Integer = 0
  Public Const RSS_BLOG_ENTRIES As Integer = 1
  Public Const RSS_SINGLE_ENTRY As Integer = 2
  Public Const RSS_ARCHIV_VIEW As Integer = 3
  Public Const CONTROL_VIEW_VIEWBLOG As String = "ViewBlog.ascx"
  Public Const CONTROL_VIEW_VIEWENTRY As String = "ViewEntry.ascx"
  Public Const CONTROL_VIEW_BLOGFEED As String = "BlogFeed.ascx"
  Public Const ONLINE_HELP_URL As String = ""
  Public Const BLOG_TEMPLATES_RESOURCE As String = "/DesktopModules/Blog/App_LocalResources/BlogTemplates.ascx.resx"

#End Region

#Region "Public Members"

  Public MyActions As New Actions.ModuleActionCollection
  Public Shared RssView As RssViews

#End Region

#Region "Public Methods"

  Public Sub SetModuleConfiguration(ByVal config As ModuleInfo)
   ModuleConfiguration = config
  End Sub

#End Region

#Region "Public Properties"
  Public Property BlogId As Integer = -1
  Public Property EntryId As Integer = -1
  Public Property Blog As Entities.BlogInfo = Nothing
  Public Property Entry As Entities.EntryInfo = Nothing
  Public Property Security As ModuleSecurity = Nothing
  Public Shadows Property Settings As Settings.BlogSettings = Nothing
  Public Property OutputAdditionalFiles As Boolean

  Public ReadOnly Property BasePage() As CDefault
   Get
    Try
     Return CType(Me.Page, CDefault)
    Catch
     Return Nothing
    End Try
   End Get
  End Property

  Public ReadOnly Property UiCulture As Globalization.CultureInfo
   Get
    Return Threading.Thread.CurrentThread.CurrentCulture
   End Get
  End Property

  Public ReadOnly Property UiTimeZone As TimeZoneInfo
   Get
    If _uiTimezone Is Nothing Then
     _uiTimezone = ModuleContext.PortalSettings.TimeZone
     If UserInfo.Profile.PreferredTimeZone IsNot Nothing Then
      _uiTimezone = UserInfo.Profile.PreferredTimeZone
     End If
    End If
    Return _uiTimezone
   End Get
  End Property
#End Region

#Region "Private Members"

  Private _uiTimezone As TimeZoneInfo = Nothing

#End Region

#Region "Event Handlers"

  Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init

   jQuery.RequestUIRegistration()
   Dim script As New StringBuilder
   script.AppendLine("<script type=""text/javascript"">")
   script.AppendLine("//<![CDATA[")
   script.AppendLine(String.Format("var appPath='{0}'", DotNetNuke.Common.ApplicationPath))
   script.AppendLine("//]]>")
   script.AppendLine("</script>")
   UI.Utilities.ClientAPI.RegisterClientScriptBlock(Page, "blogAppPath", script.ToString)

   Settings = Modules.Blog.Settings.BlogSettings.GetBlogSettings(PortalId, TabId)

   Request.Params.ReadValue("BlogId", BlogId)
   Request.Params.ReadValue("EntryId", EntryId)
   If EntryId > -1 Then Entry = Controllers.EntryController.GetEntry(EntryId, PortalId)
   If Settings.PageBlogs > -1 Then BlogId = Settings.PageBlogs ' page blog trumps requested blog
   If BlogId > -1 AndAlso Entry.BlogID <> BlogId Then Entry = Nothing ' double check in case someone is hacking to retrieve an entry from another blog
   If BlogId = -1 And Entry IsNot Nothing Then BlogId = Entry.BlogID
   If BlogId > -1 Then Blog = Controllers.BlogController.GetBlog(BlogId)
   Security = New ModuleSecurity(ModuleId, TabId, Blog, UserInfo)

  End Sub

  Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreRender
   If OutputAdditionalFiles Then
    For Each f As String In Settings.IncludeFiles.Split(";"c)
     If Not String.IsNullOrEmpty(f) Then
      If f.ToLower.EndsWith(".js") Then
       Dim path As String = f.Replace("[P]", PortalSettings.HomeDirectory & "Blog/Include/").Replace("[H]", DotNetNuke.Common.ApplicationPath & "/DesktopModules/Blog/include/")
       ClientResourceManager.RegisterScript(Page, path)
      ElseIf f.ToLower.EndsWith(".css") Then
       Dim path As String = f.Replace("[P]", PortalSettings.HomeDirectory & "Blog/Include/").Replace("[H]", DotNetNuke.Common.ApplicationPath & "/DesktopModules/Blog/include/")
       ClientResourceManager.RegisterStyleSheet(Page, path, Web.Client.FileOrder.Css.ModuleCss)
      End If
     End If
    Next
   End If
  End Sub

#End Region

 End Class

End Namespace