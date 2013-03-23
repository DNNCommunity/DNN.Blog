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

Namespace Common

 Public Class BlogModuleBase
  Inherits BlogContextBase
  Implements IPropertyAccess

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

   Request.Params.ReadValue("Blog", BlogId)
   Request.Params.ReadValue("Post", ContentItemId)
   Request.Params.ReadValue("Term", TermId)
   Request.Params.ReadValue("search", SearchString)
   Request.Params.ReadValue("t", SearchTitle)
   Request.Params.ReadValue("c", SearchContents)
   Request.Params.ReadValue("u", SearchUnpublished)
   If ContentItemId > -1 Then Entry = Entities.Entries.EntriesController.GetEntry(ContentItemId, Settings.ModuleId)
   If BlogId > -1 And Entry IsNot Nothing AndAlso Entry.BlogID <> BlogId Then Entry = Nothing ' double check in case someone is hacking to retrieve an entry from another blog
   If BlogId = -1 And Entry IsNot Nothing Then BlogId = Entry.BlogID
   If BlogId > -1 Then Blog = Entities.Blogs.BlogsController.GetBlog(BlogId, UserInfo.UserID)
   If BlogId > -1 Then BlogMapPath = GetBlogDirectoryMapPath(BlogId)
   If BlogMapPath <> "" AndAlso Not IO.Directory.Exists(BlogMapPath) Then IO.Directory.CreateDirectory(BlogMapPath)
   If ContentItemId > -1 Then EntryMapPath = GetPostDirectoryMapPath(BlogId, ContentItemId)
   If EntryMapPath <> "" AndAlso Not IO.Directory.Exists(EntryMapPath) Then IO.Directory.CreateDirectory(EntryMapPath)
   If TermId > -1 Then Term = Entities.Terms.TermsController.GetTerm(TermId, Settings.ModuleId)
   ' set urls for use in module
   Dim params As New List(Of String)
   If BlogId > -1 Then params.Add("Blog=" & BlogId.ToString)
   If ContentItemId > -1 Then params.Add("Post=" & ContentItemId.ToString)
   If TermId > -1 Then params.Add("Term=" & TermId.ToString)
   ModuleUrls = New ModuleUrls(TabId, BlogId, ContentItemId, TermId)
   If Not Settings.ShowAllLocales Then
    ShowLocale = Threading.Thread.CurrentThread.CurrentCulture.Name
   End If

  End Sub
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
    Case "blogid"
     Return (Me.BlogID.ToString(OutputFormat, formatProvider))
    Case "entryid", "contentitemid", "postid", "post"
     Return (Me.ContentItemId.ToString(OutputFormat, formatProvider))
    Case "blogselected"
     Return CBool(BlogId > -1).ToString(formatProvider)
    Case "postselected"
     Return CBool(ContentItemId > -1).ToString(formatProvider)
    Case Else
     If Me.Request.Params(strPropertyName) IsNot Nothing Then
      Return Me.Request.Params(strPropertyName)
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

#Region " Public Methods "
  Public Sub AddBlogService()

   DotNetNuke.Framework.jQuery.RequestDnnPluginsRegistration()
   DotNetNuke.Framework.ServicesFramework.Instance.RequestAjaxScriptSupport()
   DotNetNuke.Framework.ServicesFramework.Instance.RequestAjaxAntiForgerySupport()
   Web.Client.ClientResourceManagement.ClientResourceManager.RegisterScript(Page, ResolveUrl("~/DesktopModules/Blog/js/dotnetnuke.blog.js"))

   ' Load initialization snippet
   Dim scriptBlock As String = Common.Globals.ReadFile(DotNetNuke.Common.ApplicationMapPath & "\DesktopModules\Blog\js\dotnetnuke.blog.pagescript.js")
   Dim tr As New Templating.GenericTokenReplace(Scope.DefaultSettings, ModuleId)
   tr.AddResources(DotNetNuke.Common.ApplicationMapPath & "\DesktopModules\Blog\App_LocalResources\SharedResources.resx")
   scriptBlock = tr.ReplaceTokens(scriptBlock)
   scriptBlock = "<script type=""text/javascript"">" & vbCrLf & "//<![CDATA[" & vbCrLf & scriptBlock & vbCrLf & "//]]>" & vbCrLf & "</script>"
   Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "BlogServiceScript", scriptBlock)

  End Sub
#End Region

 End Class

End Namespace