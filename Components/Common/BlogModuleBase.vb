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
Imports DotNetNuke.Modules.Blog.Security
Imports DotNetNuke.Services.Tokens

Namespace Common

 Public Class BlogModuleBase
  Inherits PortalModuleBase
  Implements IPropertyAccess

#Region " Public Members "
#End Region

#Region " Public Methods "
#End Region

#Region " Public Properties "
  Public Property BlogId As Integer = -1
  Public Property ContentItemId As Integer = -1
  Public Property Blog As Entities.Blogs.BlogInfo = Nothing
  Public Property Entry As Entities.Entries.EntryInfo = Nothing
  Public Property BlogMapPath As String = ""
  Public Property EntryMapPath As String = ""
  Public Property OutputAdditionalFiles As Boolean

  Private _uiTimezone As TimeZoneInfo = Nothing
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

  Private _settings As ModuleSettings
  Public Shadows Property Settings() As ModuleSettings
   Get
    If _settings Is Nothing Then _settings = ModuleSettings.GetModuleSettings(ModuleId)
    Return _settings
   End Get
   Set(ByVal value As ModuleSettings)
    _settings = value
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

  Private _security As ContextSecurity
  Public Property Security() As ContextSecurity
   Get
    If _security Is Nothing Then _security = New ContextSecurity(ModuleId, TabId, Blog, UserInfo)
    Return _security
   End Get
   Set(ByVal value As ContextSecurity)
    _security = value
   End Set
  End Property
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

   Settings = ModuleSettings.GetModuleSettings(ModuleId)

   Request.Params.ReadValue("Blog", BlogId)
   Request.Params.ReadValue("Post", ContentItemId)
   If ContentItemId > -1 Then Entry = Entities.Entries.EntriesController.GetEntry(ContentItemId, ModuleId)
   If BlogId > -1 And Entry IsNot Nothing AndAlso Entry.BlogID <> BlogId Then Entry = Nothing ' double check in case someone is hacking to retrieve an entry from another blog
   If BlogId = -1 And Entry IsNot Nothing Then BlogId = Entry.BlogID
   If BlogId > -1 Then Blog = Entities.Blogs.BlogsController.GetBlog(BlogId, UserInfo.UserID)
   If BlogId > -1 Then BlogMapPath = PortalSettings.HomeDirectoryMapPath & String.Format("\Blog\Files\{0}\", BlogId)
   If BlogMapPath <> "" AndAlso Not IO.Directory.Exists(BlogMapPath) Then IO.Directory.CreateDirectory(BlogMapPath)
   If ContentItemId > -1 Then EntryMapPath = PortalSettings.HomeDirectoryMapPath & String.Format("\Blog\Files\{0}\{1}\", BlogId, ContentItemId)
   If EntryMapPath <> "" AndAlso Not IO.Directory.Exists(EntryMapPath) Then IO.Directory.CreateDirectory(EntryMapPath)

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
     PropertyNotFound = True
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