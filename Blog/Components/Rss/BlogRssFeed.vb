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
Imports DotNetNuke.Modules.Blog.Business
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

#Region " Constructors "
        Public Sub New(ByVal ModuleConfiguration As DotNetNuke.Entities.Modules.ModuleInfo, ByVal Request As HttpRequest, ByVal RssView As RssViews)

            ' Set variables
            _blogSettings = BlogSettings.GetBlogSettings(ModuleConfiguration.PortalID, ModuleConfiguration.TabID)
            _moduleId = ModuleConfiguration.ModuleID
            _tabId = ModuleConfiguration.TabID
            _rssView = RssView
            _includeBody = _blogSettings.IncludeBody
            If _includeBody Then
                _includeBody = False
                Components.Common.Globals.ReadValue(Request.Params, "body", _includeBody)
            End If
            _includeCategoriesInDescription = _blogSettings.IncludeCategoriesInDescription
            _includeTagsInDescription = _blogSettings.IncludeTagsInDescription

            ' Read request
            _requestUrl = Request.Url
            Components.Common.Globals.ReadValue(Request.Params, "rssid", _rssId)
            _blog = (New BlogController).GetBlog(_rssId)
            Components.Common.Globals.ReadValue(Request.Params, "rssentryid", _rssEntryId)
            Components.Common.Globals.ReadValue(Request.Params, "rssdate", _rssDate)
            If _rssDate <> "" Then
                _rssDateAsDate = CType(_rssDate, Date)
                _rssDateAsDate = _rssDateAsDate.AddDays(1)
            End If

            ' Read environment variables
            _portalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings
            _useFriendlyUrls = DotNetNuke.Entities.Host.Host.UseFriendlyUrls
            _userId = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo.UserID

        End Sub
#End Region

#Region " Public Methods "
        Public Function WriteRssToString() As String
            Dim sb As New StringBuilder
            WriteRss(sb)
            Return sb.ToString
        End Function

        Public Sub WriteRss(ByRef output As IO.Stream)
            Dim xtw As New XmlTextWriter(output, System.Text.Encoding.UTF8)
            WriteRss(xtw)
            xtw.Flush()
        End Sub

        Public Sub WriteRss(ByVal fileName As String)
            Dim xtw As New XmlTextWriter(fileName, System.Text.Encoding.UTF8)
            WriteRss(xtw)
            xtw.Flush()
            xtw.Close()
        End Sub

        Public Sub WriteRss(ByRef output As StringBuilder)
            Dim sw As New StringWriterWithEncoding(output, System.Text.Encoding.UTF8)
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
            output.WriteAttributeString("xmlns", nsTrackbackPre, Nothing, nsTrackbackFull)

            ' CP - Updates to allow aggreated feed (via ID 0) and to avoid unhandled exception if not found. 
            If _blog Is Nothing Then
                _blog = New BlogInfo
                _blog.SyndicationEmail = _portalSettings.Email
            End If
            ' End Updates

            ' set variables for channel header
            Dim ManagingEditor As String = _blog.SyndicationEmail
            Dim Title As String = _blog.Title
            Dim Description As String = _blog.Description
            Dim Link As System.Uri = _requestUrl
            Dim Language As String = _portalSettings.DefaultLanguage

            Select Case _rssView

                Case RssViews.ArchivEntries

                    ManagingEditor = _portalSettings.Email
                    Title = Localization.GetString("lblArchive.Text", ModulePath & Localization.LocalResourceDirectory & "/Archive")
                    If _portalSettings.ActiveTab.Description <> "" Then
                        Description = _portalSettings.ActiveTab.Description
                    Else
                        Description = _portalSettings.Description
                    End If
                    If _rssId = -1 Then
                        'Dim userCulture As CultureInfo = New System.Globalization.CultureInfo(ModuleContext.PortalSettings.CultureCode)
                        'Dim n As DateTime = Utility.AdjustedDate(CType(e.Item.DataItem, ArchiveMonths).AddedDate, ModuleContext.PortalSettings.TimeZone)
                        'Dim publishDate As DateTime = n
                        'Dim timeOffset As TimeSpan = ModuleContext.PortalSettings.UserInfo.Profile.PreferredTimeZone.BaseUtcOffset

                        'publishDate = publishDate.Add(timeOffset)

                        'Language = _blog.Culture
                        If _useFriendlyUrls Then
                            Link = New Uri(Utility.checkUriFormat(NavigateURL(_tabId, "", "BlogId=" & _blog.BlogID.ToString()) & "?BlogDate=" & _rssDate))
                        Else
                            Link = New Uri(Utility.checkUriFormat(NavigateURL(_tabId, "", "BlogId=" & _blog.BlogID.ToString()) & "&BlogDate=" & _rssDate))
                        End If
                    Else
                        Language = _portalSettings.DefaultLanguage
                        If _useFriendlyUrls Then
                            Link = New Uri(Utility.checkUriFormat(NavigateURL(_tabId) & "?BlogDate=" & _rssDate))
                        Else
                            Link = New Uri(Utility.checkUriFormat(NavigateURL(_tabId) & "&BlogDate=" & _rssDate))
                        End If
                    End If

                Case RssViews.RecentEntries

                    ManagingEditor = _portalSettings.Email
                    Title = Localization.GetString("msgMostRecentEntries.Text", ModulePath & Localization.LocalResourceDirectory & "/ViewBlog")
                    If _portalSettings.ActiveTab.Description <> "" Then
                        Description = _portalSettings.ActiveTab.Description
                    Else
                        Description = _portalSettings.Description
                    End If
                    Link = New Uri(Utility.checkUriFormat(NavigateURL(_tabId)))

                Case RssViews.SingleEntry, RssViews.BlogEntries

                    Link = New Uri(Utility.checkUriFormat(NavigateURL(_tabId, "", "BlogId=" & _blog.BlogID)))
                    'Language = _blog.Culture

            End Select

            ' Write the channel header block
            output.WriteElementString("title", Title)
            output.WriteElementString("description", Description)
            output.WriteElementString("link", Link.ToString)
            'output.WriteElementString("language", Language)
            output.WriteElementString("webMaster", ManagingEditor)
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
                    dr = DataProvider.Instance().ListEntriesByPortal(_portalSettings.PortalId, Date.UtcNow, Nothing, _blogSettings.RecentRssEntriesMax, 1, DotNetNuke.Security.PortalSecurity.IsInRole(_portalSettings.AdministratorRoleName), DotNetNuke.Security.PortalSecurity.IsInRole(_portalSettings.AdministratorRoleName))
                Case RssViews.BlogEntries
                    If Not _blog Is Nothing Then
                        dr = DataProvider.Instance().ListEntriesByBlog(_rssId, Date.UtcNow, _blogSettings.RecentRssEntriesMax, 1, ModuleSecurity.HasBlogPermission(_userId, _blog.UserID, _moduleId), ModuleSecurity.HasBlogPermission(_userId, _blog.UserID, _moduleId))
                    End If
                Case RssViews.ArchivEntries
                    Dim m_dBlogDate As Date
                    If _blog IsNot Nothing Then
                        dr = DataProvider.Instance().ListEntriesByBlog(_rssId, m_dBlogDate.ToUniversalTime, _blogSettings.RecentRssEntriesMax, 1, ModuleSecurity.HasBlogPermission(_userId, _blog.UserID, _moduleId), ModuleSecurity.HasBlogPermission(_userId, _blog.UserID, _moduleId))
                    Else
                        dr = DataProvider.Instance().ListEntriesByPortal(_portalSettings.PortalId, m_dBlogDate.ToUniversalTime, Nothing, _blogSettings.RecentRssEntriesMax, 1, DotNetNuke.Security.PortalSecurity.IsInRole(_portalSettings.AdministratorRoleName), DotNetNuke.Security.PortalSecurity.IsInRole(_portalSettings.AdministratorRoleName))
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

#Region "Private Methods "
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
            If _includeTagsInDescription Then
                Dim TagString As String = TagController.GetTagsByEntry(EntryId)
                If Not TagString = "" Then
                    Description &= "<div class=""tags"">" & Localization.GetString("Tags", Components.Common.Globals.glbSharedResourceFile) & ": " & TagString & "</div>"
                End If
            End If
            If _includeCategoriesInDescription Then
                For Each c As CategoryInfo In CategoryController.ListCatsByEntry(EntryId)
                    Description &= "<div class=""category"">" & Localization.GetString("Category", Components.Common.Globals.glbSharedResourceFile) & ": <a href=" & NavigateURL(_tabId, "", "CatID=" & c.CatId.ToString) & ">" & c.Category & "</a></div>"
                Next
            End If
            writer.WriteElementString("description", Description)
            WriteElement(writer, "author", ir, "SyndicationEmail", False)
            ' categories
            For Each c As CategoryInfo In CategoryController.ListCatsByEntry(EntryId)
                writer.WriteStartElement("category")
                writer.WriteAttributeString("domain", NavigateURL(_tabId, "", "CatID=" & CType(c.CatId, String)))
                writer.WriteString(c.Category)
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
            writer.WriteElementString(nsTrackbackPre, "ping", Nothing, HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) & ModulePath & "Trackback.aspx?id=" & EntryId.ToString)

            ' Write Blog specific data
            ' Write tags
            For Each t As TagInfo In TagController.ListTagsByEntry(EntryId)
                writer.WriteStartElement(nsBlogPre, "tag", nsBlogFull)
                writer.WriteAttributeString(nsBlogPre, "url", Nothing, NavigateURL(_tabId, "", "TagID=" & t.TagId.ToString))
                writer.WriteString(t.Tag)
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