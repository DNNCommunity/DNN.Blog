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

Imports System.Xml
Imports DotNetNuke.Modules.Blog.Providers.Data
Imports DotNetNuke.Modules.Blog.Components.Business
Imports DotNetNuke.Modules.Blog.Components.Controllers
Imports DotNetNuke.Modules.Blog.Components.Common
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Common
Imports DotNetNuke.Modules.Blog.Components.Settings
Imports DotNetNuke.Modules.Blog.Components.Entities

Namespace Components.Rss

 ''' <summary>
 ''' This class will produce a RSS 2.0 compliant feed for the Blog module
 ''' </summary>
 ''' <remarks>
 ''' This replaces the old mechanism that passed through an elaborate construction of objects. This
 ''' method will allow us to pass additional info with the feed and remain RSS compliant. You
 ''' can obtain the full post contents if the admin allows this and you include 'body=true' in the
 ''' querystring.
 ''' </remarks>
 ''' <history>
 '''		[pdonker]	12/14/2009	created
 ''' </history>
 Public Class BlogRssFeed

#Region " Private Members "

  Private Const nsBlogPre As String = "blog"
  Private Const nsBlogFull As String = "http://www.dotnetnuke.com/blog/"
  Private Const nsSlashPre As String = "slash"
  Private Const nsSlashFull As String = "http://purl.org/rss/1.0/modules/slash/"
  Private Const nsTrackbackPre As String = "trackback"
  Private Const nsTrackbackFull As String = "http://madskills.com/public/xml/rss/module/trackback/"
  Private Const DateTimeFormatString As String = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'"
  Private Const ModulePath As String = "DesktopModules/Blog"

  ' Environment vars
  Private _locale As String = Threading.Thread.CurrentThread.CurrentCulture.Name
  Private _portalSettings As DotNetNuke.Entities.Portals.PortalSettings
  Private _useFriendlyUrls As Boolean = False
  Private _blogSettings As BlogSettings = Nothing
  Private _userId As Integer = -1
  Private _moduleId As Integer = -1
  Private _tabId As Integer = -1
  Private _rssView As RssViews
  Private _security As ModuleSecurity

  ' Request Params
  Private _requestUrl As System.Uri
  Private _rssId As Integer = -1
  Private _blog As BlogInfo = Nothing
  Private _rssEntryId As Integer = -1
  Private _rssDate As String = ""
  Private _rssDateAsDate As Date = Now

  ' Other
  Private _includeBody As Boolean = False
  Private _includeCategoriesInDescription As Boolean = False
  Private _includeTagsInDescription As Boolean = False

#End Region

#Region "Constructors"

  Public Sub New(ByVal moduleConfiguration As DotNetNuke.Entities.Modules.ModuleInfo, ByVal request As HttpRequest, ByVal rssView As RssViews)
   ' Set variables
   _blogSettings = BlogSettings.GetBlogSettings(moduleConfiguration.PortalID, moduleConfiguration.TabID)
   _moduleId = moduleConfiguration.ModuleID
   _tabId = moduleConfiguration.TabID
   _rssView = rssView
   _includeBody = _blogSettings.IncludeBody
   If _includeBody Then
    _includeBody = False
    request.Params.ReadValue("body", _includeBody)
   End If
   _includeCategoriesInDescription = _blogSettings.IncludeCategoriesInDescription
   _includeTagsInDescription = _blogSettings.IncludeTagsInDescription

   ' Read request
   _requestUrl = request.Url
   request.Params.ReadValue("rssid", _rssId)
   _blog = BlogController.GetBlog(_rssId)
   request.Params.ReadValue("rssentryid", _rssEntryId)
   request.Params.ReadValue("rssdate", _rssDate)
   If _rssDate <> "" Then
    _rssDateAsDate = CType(_rssDate, Date)
    _rssDateAsDate = _rssDateAsDate.AddDays(1)
   End If

   ' Read environment variables
   _portalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings
   _useFriendlyUrls = DotNetNuke.Entities.Host.Host.UseFriendlyUrls
   _userId = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo.UserID
   _security = New ModuleSecurity(_moduleId, _tabId, _blog, DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo)
  End Sub

#End Region

#Region "Public Methods"

  Public Function WriteRssToString() As String
   Dim sb As New StringBuilder
   WriteRss(sb)
   Return sb.ToString
  End Function

  Public Sub WriteRss(ByRef output As IO.Stream)
   Dim xtw As New XmlTextWriter(output, Encoding.UTF8)
   WriteRss(xtw)
   xtw.Flush()
  End Sub

  Public Sub WriteRss(ByVal fileName As String)
   Dim xtw As New XmlTextWriter(fileName, Encoding.UTF8)
   WriteRss(xtw)
   xtw.Flush()
   xtw.Close()
  End Sub

  Public Sub WriteRss(ByRef output As StringBuilder)
   Dim sw As New StringWriterWithEncoding(output, Encoding.UTF8)
   Dim xtw As New XmlTextWriter(sw)
   WriteRss(xtw)
   xtw.Flush()
   xtw.Close()
   sw.Flush()
   sw.Close()
  End Sub

  Public Sub WriteRss(ByRef output As XmlTextWriter)
   output.Formatting = Formatting.Indented
   output.WriteStartDocument()
   output.WriteStartElement("rss")
   output.WriteAttributeString("version", "2.0")
   output.WriteStartElement("channel")
   output.WriteAttributeString("xmlns", nsBlogPre, Nothing, nsBlogFull)
   output.WriteAttributeString("xmlns", nsSlashPre, Nothing, nsSlashFull)
   'output.WriteAttributeString("xmlns", nsTrackbackPre, Nothing, nsTrackbackFull)

   ' CP - Updates to allow aggreated feed (via ID 0) and to avoid unhandled exception if not found. 
   If _blog Is Nothing Then
    _blog = New BlogInfo
    _blog.SyndicationEmail = _portalSettings.Email
   End If
   ' End Updates

   ' set variables for channel header
   Dim managingEditor As String = _blog.SyndicationEmail
   Dim title As String = _blog.Title
   Dim description As String = _blog.Description
   Dim link As Uri = _requestUrl
   Dim Language As String = _portalSettings.DefaultLanguage

   Select Case _rssView
    Case RssViews.ArchivEntries
     managingEditor = _portalSettings.Email
     title = Localization.GetString("lblArchive.Text", ModulePath & Localization.LocalResourceDirectory & "/Archive")
     If _portalSettings.ActiveTab.Description <> "" Then
      description = _portalSettings.ActiveTab.Description
     Else
      description = _portalSettings.Description
     End If
     If _rssId = -1 Then
      'Dim userCulture As CultureInfo = New System.Globalization.CultureInfo(ModuleContext.PortalSettings.CultureCode)
      'Dim n As DateTime = Utility.AdjustedDate(CType(e.Item.DataItem, ArchiveMonths).AddedDate, ModuleContext.PortalSettings.TimeZone)
      'Dim publishDate As DateTime = n
      'Dim timeOffset As TimeSpan = UITimezone.BaseUtcOffset

      'publishDate = publishDate.Add(timeOffset)

      'Language = _blog.Culture
      If _useFriendlyUrls Then
       link = New Uri(Utility.checkUriFormat(NavigateURL(_tabId, "", "BlogId=" & _blog.BlogID.ToString()) & "?BlogDate=" & _rssDate))
      Else
       link = New Uri(Utility.checkUriFormat(NavigateURL(_tabId, "", "BlogId=" & _blog.BlogID.ToString()) & "&BlogDate=" & _rssDate))
      End If
     Else
      Language = _portalSettings.DefaultLanguage
      If _useFriendlyUrls Then
       link = New Uri(Utility.checkUriFormat(NavigateURL(_tabId) & "?BlogDate=" & _rssDate))
      Else
       link = New Uri(Utility.checkUriFormat(NavigateURL(_tabId) & "&BlogDate=" & _rssDate))
      End If
     End If
    Case RssViews.SingleEntry, RssViews.BlogEntries

     link = New Uri(Utility.checkUriFormat(NavigateURL(_tabId, "", "BlogId=" & _blog.BlogID)))
     'Language = _blog.Culture
    Case RssViews.RecentEntries
     managingEditor = _portalSettings.Email
     title = Localization.GetString("msgMostRecentEntries.Text", ModulePath & Localization.LocalResourceDirectory & "/ViewBlog")
     If _portalSettings.ActiveTab.Description <> "" Then
      description = _portalSettings.ActiveTab.Description
     Else
      description = _portalSettings.Description
     End If
     link = New Uri(Utility.checkUriFormat(NavigateURL(_tabId)))
   End Select

   ' Write the channel header block
   output.WriteElementString("title", title)
   output.WriteElementString("description", description)
   output.WriteElementString("link", link.ToString)
   'output.WriteElementString("language", Language)
   output.WriteElementString("webMaster", managingEditor)
   output.WriteElementString("pubDate", Now.ToString(DateTimeFormatString))
   output.WriteElementString("lastBuildDate", Now.ToString(DateTimeFormatString))
   output.WriteElementString("docs", "http://backend.userland.com/rss")
   output.WriteElementString("generator", "Blog RSS Generator Version " & CType(System.Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString, String))
   'output.WriteElementString("copyright", _portal.FooterText)

   ' Get the blog entries
   Dim dr As IDataReader = Nothing
   Select Case _rssView
    Case RssViews.None ' could not be, but ...
    Case RssViews.RecentEntries
     dr = DataProvider.Instance().GetEntriesByPortal(_portalSettings.PortalId, Date.UtcNow, Nothing, _blogSettings.RecentRssEntriesMax, 1, _security.UserIsAdmin, _security.UserIsAdmin)
    Case RssViews.BlogEntries
     If Not _blog Is Nothing Then
      Dim isOwner As Boolean
      isOwner = _blog.UserID = _userId
      dr = DataProvider.Instance().GetEntriesByBlog(_rssId, Date.UtcNow, _blogSettings.RecentRssEntriesMax, 0, _security.CanAddEntry, _security.CanAddEntry)
     End If
    Case RssViews.ArchivEntries
     Dim m_dBlogDate As Date
     If _blog IsNot Nothing Then
      Dim isOwner As Boolean
      isOwner = _blog.UserID = _userId
      dr = DataProvider.Instance().GetEntriesByBlog(_rssId, m_dBlogDate.ToUniversalTime, _blogSettings.RecentRssEntriesMax, 1, _security.CanAddEntry, _security.CanAddEntry)
     Else
      dr = DataProvider.Instance().GetEntriesByPortal(_portalSettings.PortalId, m_dBlogDate.ToUniversalTime, Nothing, _blogSettings.RecentRssEntriesMax, 1, _security.UserIsAdmin, _security.UserIsAdmin)
     End If
    Case RssViews.SingleEntry
     dr = DataProvider.Instance().GetEntry(_rssEntryId, _portalSettings.PortalId)
   End Select

   ' Now that we should shave a reader let's fill the feed with the contents
   If dr Is Nothing Then
    ' throw an error or not?
   Else
    Do While dr.Read
     WriteItem(output, dr)
    Loop
    dr.Close()
    dr.Dispose()
   End If

   output.WriteEndElement()
   output.WriteEndElement()
   output.Flush()
   output.Close()

  End Sub

  Public Function CacheKey() As String
   Return String.Format("ID{0}TAB{1}MID{2}ENT{3}DAT{4}BODY{5}LOC{6}USER{7}", _rssId, _tabId, _moduleId, _rssEntryId, _rssDate.Replace(" ", "_"), _includeBody, _locale, _userId)
  End Function

#End Region

#Region "Private Methods"

  Private Sub WriteItem(ByRef writer As XmlTextWriter, ByVal ir As IDataReader)
   Dim EntryId As Integer = CInt(ir.Item("EntryId"))
   Dim PermaLink As String = HttpUtility.HtmlDecode(CStr(ir.Item("PermaLink")))

   ' Write core RSS elements
   writer.WriteStartElement("item")
   WriteElement(writer, "title", ir, "Title", True)
   writer.WriteElementString("link", PermaLink)
   Dim Description As String = ""
   If ir.Item("Description") Is DBNull.Value Then
    Description = Utility.RewriteRefs(HttpUtility.HtmlDecode(CStr(ir.Item("Entry")))) & "<br /><a href=" + PermaLink + ">" & Localization.GetString("More", Components.Common.Globals.glbSharedResourceFile) & "</a>"
   Else
    Description = Utility.RewriteRefs(HttpUtility.HtmlDecode(CStr(ir.Item("Description"))))
   End If

   Dim colTags As List(Of TermInfo) = TermController.GetTermsByContentItem(EntryId, 1)
   If _includeTagsInDescription Then
    For Each c As TermInfo In colTags
     Description &= "<div class=""tags"">" & Localization.GetString("Tags", Components.Common.Globals.glbSharedResourceFile) & ": <a href=" & NavigateURL(_tabId, False, _portalSettings, "", "tagid=" + c.TermId.ToString) & ">" & c.Name & "</a></div>"
    Next
   End If

   Dim colCategories As List(Of TermInfo) = TermController.GetTermsByContentItem(EntryId, _blogSettings.VocabularyId)
   If _includeCategoriesInDescription AndAlso _blogSettings.VocabularyId > 0 Then
    For Each c As TermInfo In colCategories
     Description &= "<div class=""category"">" & Localization.GetString("Category", Components.Common.Globals.glbSharedResourceFile) & ": <a href=" & NavigateURL(_tabId, False, _portalSettings, "", "catid=" + c.TermId.ToString) & ">" & c.Name & "</a></div>"
    Next
   End If

   writer.WriteElementString("description", Description)
   WriteElement(writer, "author", ir, "SyndicationEmail", False)

   For Each c As TermInfo In colCategories
    writer.WriteStartElement("category")
    writer.WriteAttributeString("domain", NavigateURL(_tabId, "", "CatID=" & CType(c.TermId, String)))
    writer.WriteString(c.Name)
    writer.WriteEndElement()
   Next
   If CBool(ir.Item("AllowComments")) Then
    If _useFriendlyUrls Then
     writer.WriteElementString("comments", Utility.checkUriFormat(PermaLink & "#Comments"))
    Else
     writer.WriteElementString("comments", Utility.checkUriFormat(PermaLink & "&#Comments"))
    End If
    WriteElement(writer, "comments", ir, "CommentCount", False, nsSlashPre)
   End If
   writer.WriteStartElement("guid")
   writer.WriteAttributeString("isPermaLink", "true")
   writer.WriteString(PermaLink)
   writer.WriteEndElement()
   writer.WriteElementString("pubDate", XmlConvert.ToString(CDate(ir.Item("AddedDate")), DateTimeFormatString))
   'writer.WriteElementString(nsTrackbackPre, "ping", Nothing, HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) & ModulePath & "Trackback.aspx?id=" & EntryId.ToString)

   For Each c As TermInfo In colTags
    writer.WriteStartElement(nsBlogPre, "tag", nsBlogFull)
    writer.WriteAttributeString(nsBlogPre, "url", Nothing, NavigateURL(_tabId, False, _portalSettings, "", "tagid=" + c.TermId.ToString))
    writer.WriteString(c.Name)
    writer.WriteEndElement()
   Next

   If _includeBody Then
    writer.WriteElementString(nsBlogPre, "body", Nothing, Utility.RewriteRefs(HttpUtility.HtmlDecode(CStr(ir.Item("Entry")))))
   End If
   writer.WriteEndElement()
  End Sub

  Public Sub WriteElement(ByRef output As XmlTextWriter, ByVal elementName As String, ByVal ir As IDataReader, ByVal columnName As String, ByVal required As Boolean)
   WriteElement(output, elementName, ir, columnName, required, "")
  End Sub

  Public Sub WriteElement(ByRef output As XmlTextWriter, ByVal elementName As String, ByVal ir As IDataReader, ByVal columnName As String, ByVal required As Boolean, ByVal nsPrefix As String)
   Try
    WriteElement(output, elementName, CStr(ir.Item(columnName)), required, nsPrefix)
   Catch ex As Exception
    If required Then
     Throw New ArgumentException(columnName + " can not be null.")
    End If
   End Try
  End Sub

  Public Sub WriteElement(ByRef output As XmlTextWriter, ByVal elementName As String, ByVal value As String, ByVal required As Boolean, ByVal nsPrefix As String)
   If nsPrefix = "" Then
    output.WriteElementString(elementName, value)
   Else
    output.WriteElementString(nsPrefix, elementName, Nothing, value)
   End If
  End Sub

#End Region

 End Class

End Namespace