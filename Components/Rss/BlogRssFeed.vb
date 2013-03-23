Imports System.Xml
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports DotNetNuke.Modules.Blog.Entities.Entries
Imports DotNetNuke.Modules.Blog.Entities.Terms
Imports DotNetNuke.Modules.Blog.Entities.Blogs

Namespace Rss
 Public Class BlogRssFeed

#Region " Constants "
  Private Const nsBlogPre As String = "blog"
  Private Const nsBlogFull As String = "http://www.dotnetnuke.com/blog/"
  Private Const nsSlashPre As String = "slash"
  Private Const nsSlashFull As String = "http://purl.org/rss/1.0/modules/slash/"
  Private Const nsAtomPre As String = "atom"
  Private Const nsAtomFull As String = "http://www.w3.org/2005/Atom"
  Private Const nsMediaPre As String = "media"
  Private Const nsMediaFull As String = "http://search.yahoo.com/mrss/"
  Private Const nsDublinPre As String = "dc"
  Private Const nsDublinFull As String = "http://purl.org/dc/elements/1.1/"
  Private Const nsContentPre As String = "content"
  Private Const nsContentFull As String = "http://purl.org/rss/1.0/modules/content/"
  Private Const nsOpenSearchPre As String = "os"
  Private Const nsOpenSearchFull As String = "http://opensearch.a9.com/spec/opensearchrss/1.0/"
#End Region

#Region " Properties "
  Public Property Settings As ModuleSettings = Nothing
  Public Property PortalSettings As DotNetNuke.Entities.Portals.PortalSettings = Nothing
  Public Property Entries As IEnumerable(Of EntryInfo) = Nothing
  Public Property CacheFile As String = ""
  Public Property IsCached As Boolean = False
  Public Property ImageHandlerUrl As String = ""
  Public Property TotalRecords As Integer = -1

  ' Requested Properties
  Public Property TermId As Integer = -1
  Public Property BlogId As Integer = -1
  Public Property RecordsToSend As Integer = 20
  Public Property Search As String = ""
  Public Property SearchTitle As Boolean = True
  Public Property SearchContents As Boolean = False
  Public Property ImageWidth As Integer = 144
  Public Property ImageHeight As Integer = 96
  Public Property IncludeContents As Boolean = False

  ' Feed Properties
  Public Property IsSearchFeed As Boolean = False
  Public Property Blog As BlogInfo = Nothing
  Public Property Term As TermInfo = Nothing
  Public Property Title As String = ""
  Public Property Description As String = ""
  Public Property Link As String = ""
  Public Property FeedEmail As String = ""
  Public Property Language As String = ""
  Public Property Copyright As String = ""
#End Region

#Region " Constructors "
  Public Sub New(moduleId As Integer, reqParams As NameValueCollection)

   ' Initialize Settings
   Settings = ModuleSettings.GetModuleSettings(moduleId)
   PortalSettings = DotNetNuke.Entities.Portals.PortalSettings.Current
   RecordsToSend = Settings.RssDefaultNrItems
   ImageWidth = Settings.RssImageWidth
   ImageHeight = Settings.RssImageHeight
   ImageHandlerUrl = ResolveUrl(glbImageHandlerPath)

   ' Read Request Values
   reqParams.ReadValue("blog", BlogId)
   reqParams.ReadValue("blogid", BlogId)
   reqParams.ReadValue("term", TermId)
   reqParams.ReadValue("termid", TermId)
   If Settings.RssMaxNrItems > 0 Then
    reqParams.ReadValue("recs", RecordsToSend)
    If RecordsToSend > Settings.RssMaxNrItems Then RecordsToSend = Settings.RssMaxNrItems
   End If
   If Settings.RssImageSizeAllowOverride Then
    reqParams.ReadValue("w", ImageWidth)
    reqParams.ReadValue("h", ImageHeight)
   End If
   If Settings.RssAllowContentInFeed Then
    reqParams.ReadValue("body", IncludeContents)
   End If
   reqParams.ReadValue("search", Search)
   reqParams.ReadValue("t", SearchTitle)
   reqParams.ReadValue("c", SearchContents)
   reqParams.ReadValue("language", Language)

   ' Start Filling In Feed Properties
   If Search <> "" Then IsSearchFeed = True
   If BlogId > -1 Then Blog = BlogsController.GetBlog(BlogId, -1)
   If TermId > -1 Then Term = TermsController.GetTerm(TermId, moduleId)
   If Blog Is Nothing Then
    Dim m As ModuleInfo = (New ModuleController).GetModule(moduleId)
    If m IsNot Nothing Then
     Title = m.ModuleTitle
    End If
    FeedEmail = Settings.RssEmail
    Copyright = Settings.RssDefaultCopyright
   Else
    Title = Blog.Title
    Description = Blog.Description
    FeedEmail = Blog.Email
    Copyright = Regex.Replace(Blog.Copyright, "(?i)\[year\](?-i)", Now.ToString("yyyy"))
   End If
   If Term IsNot Nothing Then
    Title &= " - " & Term.Name
   End If
   If IsSearchFeed Then
    Title = "DNN Blog Search " & Title
    Description &= String.Format(" - Searching '{0}'", Search)
   End If
   Link = ApplicationURL()
   If Blog IsNot Nothing Then Link &= String.Format("&blog={0}", BlogId)
   If Term IsNot Nothing Then Link &= String.Format("&term={0}", TermId)
   If RecordsToSend <> Settings.RssDefaultNrItems Then Link &= String.Format("&recs={0}", RecordsToSend)
   If ImageWidth <> Settings.RssImageWidth Then Link &= String.Format("&w={0}", ImageWidth)
   If ImageHeight <> Settings.RssImageHeight Then Link &= String.Format("&h={0}", ImageHeight)
   If IncludeContents Then Link &= "&body=true"
   If Language <> "" Then Link &= String.Format("&language={0}", Language)
   If IsSearchFeed Then Link &= String.Format("&search={0}&t={1}&c={2}", HttpUtility.UrlEncode(Search), SearchTitle, SearchContents)
   CacheFile = Link.Substring(Link.IndexOf("?"c) + 1).Replace("&", "+").Replace("=", "-")
   CacheFile = String.Format("{0}\Blog\RssCache\{1}.resources", PortalSettings.HomeDirectoryMapPath, CacheFile)
   Link = FriendlyUrl(PortalSettings.ActiveTab, Link, GetSafePageName(Title))
   If Language = "" Then
    If Blog IsNot Nothing AndAlso Not String.IsNullOrEmpty(Blog.Locale) Then
     Language = Blog.Locale
    Else
     Language = PortalSettings.DefaultLanguage
    End If
   End If

   ' Check Cache
   If IO.File.Exists(CacheFile) Then
    Dim f As New IO.FileInfo(CacheFile)
    If f.LastWriteTime.AddMinutes(Settings.RssTtl) > Now Then IsCached = True
   Else
    Dim pth As String = IO.Path.GetDirectoryName(CacheFile)
    If Not IO.Directory.Exists(pth) Then IO.Directory.CreateDirectory(pth)
   End If

   If Not IsCached Then
    ' Load Posts
    If IsSearchFeed Then
     If Term IsNot Nothing Then
      Entries = EntriesController.SearchEntriesByTerm(moduleId, BlogId, TermId, Search, SearchTitle, SearchContents, 1, Language, Date.Now.ToUniversalTime, -1, 0, RecordsToSend, "PUBLISHEDONDATE DESC", TotalRecords, -1, False).Values
     Else
      Entries = EntriesController.SearchEntries(moduleId, BlogId, Search, SearchTitle, SearchContents, 1, Language, Date.Now.ToUniversalTime, -1, 0, RecordsToSend, "PUBLISHEDONDATE DESC", TotalRecords, -1, False).Values
     End If
    Else
     If Term IsNot Nothing Then
      Entries = EntriesController.GetEntriesByTerm(moduleId, BlogId, TermId, 1, Language, Date.Now.ToUniversalTime, -1, 0, RecordsToSend, "PUBLISHEDONDATE DESC", TotalRecords, -1, False).Values
     Else
      Entries = EntriesController.GetEntries(moduleId, BlogId, 1, Language, Date.Now.ToUniversalTime, -1, 0, RecordsToSend, "PUBLISHEDONDATE DESC", TotalRecords, -1, False).Values
     End If
    End If
    WriteRss(CacheFile)
   End If
  End Sub
#End Region

#Region " Public Methods "
  Public Function WriteRssToString() As String
   Dim sb As New StringBuilder
   WriteRss(sb)
   Return sb.ToString
  End Function

  Public Sub WriteRss(ByRef output As IO.Stream)
   Using xtw As New XmlTextWriter(output, Encoding.UTF8)
    WriteRss(xtw)
    xtw.Flush()
   End Using
  End Sub

  Public Sub WriteRss(fileName As String)
   Using fs As New IO.FileStream(fileName, IO.FileMode.OpenOrCreate, IO.FileAccess.Write)
    Using xtw As New XmlTextWriter(fs, Encoding.UTF8)
     WriteRss(xtw)
     xtw.Flush()
    End Using
   End Using
  End Sub

  Public Sub WriteRss(ByRef output As StringBuilder)
   Using sw As New StringWriterWithEncoding(output, Encoding.UTF8)
    Using xtw As New XmlTextWriter(sw)
     WriteRss(xtw)
     xtw.Flush()
    End Using
    sw.Flush()
   End Using
  End Sub

  Public Sub WriteRss(ByRef output As XmlTextWriter)

   output.Formatting = Formatting.Indented
   output.WriteStartDocument()
   output.WriteStartElement("rss")
   output.WriteAttributeString("version", "2.0")
   output.WriteAttributeString("xmlns", nsBlogPre, Nothing, nsBlogFull)
   'output.WriteAttributeString("xmlns", nsSlashPre, Nothing, nsSlashFull)
   output.WriteAttributeString("xmlns", nsAtomPre, Nothing, nsAtomFull)
   output.WriteAttributeString("xmlns", nsMediaPre, Nothing, nsMediaFull)
   If IsSearchFeed Then output.WriteAttributeString("xmlns", nsOpenSearchPre, Nothing, nsOpenSearchFull)
   If IncludeContents Then output.WriteAttributeString("xmlns", nsContentPre, Nothing, nsContentFull)
   output.WriteStartElement("channel")

   ' Write the channel header block
   output.WriteElementString("title", Title)
   output.WriteElementString("link", Link)
   output.WriteElementString("description", Description)
   ' optional elements
   If Language <> "" Then output.WriteElementString("language", Language)
   If Copyright <> "" Then output.WriteElementString("copyright", Copyright)
   If FeedEmail <> "" Then output.WriteElementString("managingEditor", FeedEmail)
   output.WriteElementString("pubDate", Now.ToString("r"))
   output.WriteElementString("lastBuildDate", Now.ToString("r"))
   If Term IsNot Nothing Then
    output.WriteElementString("category", Term.Name)
   End If
   output.WriteElementString("generator", "DotNetNuke Blog RSS Generator Version " & CType(System.Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString, String))
   output.WriteElementString("ttl", Settings.RssTtl.ToString)
   If Blog IsNot Nothing AndAlso (Blog.IncludeImagesInFeed And Blog.Image <> "") Then
    output.WriteStartElement("image")
    output.WriteElementString("url", ImageHandlerUrl & String.Format("?TabId={0}&ModuleId={1}&Blog={2}&w={4}&h={5}&c=1&key={3}", PortalSettings.ActiveTab.TabID, Settings.ModuleId, BlogId, Blog.Image, ImageWidth, ImageHeight))
    output.WriteElementString("title", Title)
    output.WriteElementString("link", Link)
    output.WriteElementString("width", ImageWidth.ToString) ' default 88 max 144
    output.WriteElementString("height", ImageHeight.ToString) ' default 31 max 400
    output.WriteEndElement() ' image
   End If
   ' extended elements
   output.WriteStartElement(nsAtomPre, "link", nsAtomFull)
   output.WriteAttributeString("href", HttpContext.Current.Request.Url.PathAndQuery)
   output.WriteAttributeString("rel", "self")
   output.WriteAttributeString("type", "application/rss+xml")
   output.WriteEndElement() ' atom:link
   If IsSearchFeed Then
    output.WriteElementString(nsOpenSearchPre, "totalResults", nsOpenSearchFull, TotalRecords.ToString)
    output.WriteElementString(nsOpenSearchPre, "startIndex", nsOpenSearchFull, "0")
    output.WriteElementString(nsOpenSearchPre, "itemsPerPage", nsOpenSearchFull, RecordsToSend.ToString)
   End If

   For Each e As EntryInfo In Entries
    WriteItem(output, e)
   Next

   output.WriteEndElement() ' channel
   output.WriteEndElement() ' rss
   output.Flush()

  End Sub
#End Region

#Region "Private Methods"
  Private Sub WriteItem(ByRef writer As XmlTextWriter, item As EntryInfo)

   writer.WriteStartElement("item")

   ' core data
   writer.WriteElementString("title", item.Title)
   writer.WriteElementString("link", item.PermaLink)
   writer.WriteElementString("description", RemoveHtmlTags(HttpUtility.HtmlDecode(item.Summary)))
   ' optional elements
   If item.Blog.IncludeAuthorInFeed Then writer.WriteElementString("author", String.Format("{0} ({1})", item.Email, item.DisplayName))
   For Each t As TermInfo In TermsController.GetTermsByEntry(item.ContentItemId, Settings.ModuleId)
    writer.WriteElementString("category", t.Name)
   Next
   writer.WriteElementString("guid", String.Format("post={0}", item.ContentItemId))
   writer.WriteElementString("pubDate", item.PublishedOnDate.ToString("r"))
   ' extensions
   If item.Blog.IncludeImagesInFeed And item.Image <> "" Then
    writer.WriteStartElement(nsMediaPre, "thumbnail", nsMediaFull)
    writer.WriteAttributeString("width", ImageWidth.ToString)
    writer.WriteAttributeString("height", ImageHeight.ToString)
    writer.WriteAttributeString("url", ImageHandlerUrl & String.Format("?TabId={0}&ModuleId={1}&Blog={2}&Post={3}&w={5}&h={6}&c=1&key={4}", PortalSettings.ActiveTab.TabID, Settings.ModuleId, item.BlogID, item.ContentItemId, item.Image, ImageWidth, ImageHeight))
    writer.WriteEndElement() ' thumbnail
   End If
   If IncludeContents Then
    writer.WriteStartElement(nsContentPre, "encoded", nsContentFull)
    writer.WriteCData(HttpUtility.HtmlDecode(item.Content))
    writer.WriteEndElement() ' content:encoded
   End If
   ' Blog Extensions
   writer.WriteElementString(nsBlogPre, "publishedon", nsBlogFull, item.PublishedOnDate.ToString("u"))

   writer.WriteEndElement() ' item

  End Sub
#End Region

 End Class
End Namespace
