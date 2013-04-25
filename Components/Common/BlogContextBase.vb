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
Imports DotNetNuke.Modules.Blog.Entities.Terms

Namespace Common

 Public Class BlogContextBase
  Inherits PortalModuleBase
  Implements IPropertyAccess

#Region " Public Methods "
  Public Sub ClonePropertiesFrom(contextBase As BlogContextBase)
   With contextBase
    Me.ModuleConfiguration = contextBase.ModuleConfiguration
    Me.Settings = contextBase.Settings
    Me.ViewSettings = contextBase.ViewSettings
    Me.Security = contextBase.Security
    Me.Vocabulary = contextBase.Vocabulary

    Me.BlogId = contextBase.BlogId
    Me.ContentItemId = contextBase.ContentItemId
    Me.TermId = contextBase.TermId
    Me.AuthorId = contextBase.AuthorId
    Me.Blog = contextBase.Blog
    Me.Post = contextBase.Post
    Me.Term = contextBase.Term
    Me.Author = contextBase.Author
    Me.BlogMapPath = contextBase.BlogMapPath
    Me.PostMapPath = contextBase.PostMapPath
    Me.OutputAdditionalFiles = contextBase.OutputAdditionalFiles
    Me.ModuleUrls = contextBase.ModuleUrls
    Me.SearchString = contextBase.SearchString
    Me.SearchTitle = contextBase.SearchTitle
    Me.SearchContents = contextBase.SearchContents
    Me.SearchUnpublished = contextBase.SearchUnpublished
    Me.IsMultiLingualSite = contextBase.IsMultiLingualSite
    Me.ShowLocale = contextBase.ShowLocale
    Me.Locale = contextBase.Locale
    Me.Referrer = contextBase.Referrer
   End With
  End Sub
#End Region

#Region " Public Properties "
  Public Property BlogId As Integer = -1
  Public Property ContentItemId As Integer = -1
  Public Property TermId As Integer = -1
  Public Property AuthorId As Integer = -1
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
    If _settings Is Nothing Then
     If ViewSettings.BlogModuleId = -1 Then
      _settings = ModuleSettings.GetModuleSettings(ModuleId)
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

  Private _Vocabulary As Dictionary(Of String, TermInfo)
  Public Property Vocabulary() As Dictionary(Of String, TermInfo)
   Get
    If _Vocabulary Is Nothing Then
     _Vocabulary = TermsController.GetTermsByVocabulary(ModuleConfiguration.ModuleID, Settings.VocabularyId, Locale)
     If _Vocabulary Is Nothing Then
      _Vocabulary = New Dictionary(Of String, TermInfo)
     End If
    End If
    Return _Vocabulary
   End Get
   Set(ByVal value As Dictionary(Of String, TermInfo))
    _Vocabulary = value
   End Set
  End Property
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
    Case "Postid", "contentitemid", "postid", "post"
     Return (Me.ContentItemId.ToString(OutputFormat, formatProvider))
    Case "termid", "term"
     Return (Me.TermId.ToString(OutputFormat, formatProvider))
    Case "authorid", "author"
     Return (Me.AuthorId.ToString(OutputFormat, formatProvider))
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

 End Class

End Namespace