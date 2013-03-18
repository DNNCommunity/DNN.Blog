Imports System.Xml
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports DotNetNuke.Modules.Blog.Entities.Entries
Imports DotNetNuke.Modules.Blog.Entities.Terms

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
#End Region

#Region " Properties "
  Public Property Entries As IEnumerable(Of EntryInfo) = Nothing
  Public Property Title As String = ""
  Public Property Description As String = ""
  Public Property Link As String = ""
  Public Property FeedEmail As String = ""
  Public Property Language As String = ""
  Public Property Copyright As String = ""
  Public Property Term As TermInfo = Nothing
#End Region

#Region " Constructors "
  Public Sub New(moduleId As Integer, blogId As Integer, termId As Integer, recordsToSend As Integer)

   Dim totalRecords As Integer = -1
   If termId > -1 Then
    Entries = EntriesController.GetEntriesByTerm(moduleId, blogId, termId, 1, Date.Now, -1, 0, recordsToSend, "PUBLISHEDONDATE DESC", totalRecords, -1, False).Values
   Else
    Entries = EntriesController.GetEntries(moduleId, blogId, 1, Date.Now, -1, 0, recordsToSend, "PUBLISHEDONDATE DESC", totalRecords, -1, False).Values
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
   Dim xtw As New XmlTextWriter(output, Encoding.UTF8)
   WriteRss(xtw)
   xtw.Flush()
  End Sub

  Public Sub WriteRss(fileName As String)
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
   output.WriteAttributeString("xmlns", nsBlogPre, Nothing, nsBlogFull)
   output.WriteAttributeString("xmlns", nsSlashPre, Nothing, nsSlashFull)
   output.WriteAttributeString("xmlns", nsAtomPre, Nothing, nsAtomFull)
   output.WriteAttributeString("xmlns", nsMediaPre, Nothing, nsMediaFull)
   output.WriteStartElement("channel")

   ' Write the channel header block
   output.WriteElementString("title", Title)
   output.WriteElementString("link", Link)
   output.WriteElementString("description", Description)
   ' optional elements
   output.WriteElementString("language", Language)
   output.WriteElementString("copyright", Copyright)
   output.WriteElementString("managingEditor", FeedEmail)
   output.WriteElementString("webMaster", FeedEmail)
   output.WriteElementString("pubDate", Now.ToString("r"))
   output.WriteElementString("lastBuildDate", Now.ToString("r"))
   If Term IsNot Nothing Then
    output.WriteElementString("category", Term.Name)
   End If
   output.WriteElementString("generator", "DotNetNuke Blog RSS Generator Version " & CType(System.Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString, String))
   output.WriteElementString("ttl", "")
   output.WriteStartElement("image")
   output.WriteElementString("url", "")
   output.WriteElementString("title", "")
   output.WriteElementString("link", "")
   output.WriteElementString("width", "88") ' default 88 max 144
   output.WriteElementString("height", "31") ' default 31 max 400
   output.WriteEndElement() ' image
   ' extended elements
   output.WriteStartElement(nsAtomPre, "link", nsAtomFull)
   output.WriteAttributeString("href", HttpContext.Current.Request.Url.PathAndQuery)
   output.WriteAttributeString("rel", "self")
   output.WriteAttributeString("type", "application/rss+xml")
   output.WriteEndElement() ' atom:link

   For Each e As EntryInfo In Entries
    WriteItem(output, e)
   Next

   output.WriteEndElement() ' channel
   output.WriteEndElement() ' rss
   output.Flush()
   output.Close()

  End Sub
#End Region

#Region "Private Methods"
  Private Sub WriteItem(ByRef writer As XmlTextWriter, item As EntryInfo)

   writer.WriteStartElement("item")

   ' core data
   writer.WriteElementString("title", item.Title)
   writer.WriteElementString("link", item.PermaLink)
   writer.WriteElementString("description", RemoveHtmlTags(item.Summary))
   ' optional elements
   writer.WriteElementString("author", "")
   ' category
   ' enclosure
   'writer.WriteElementString("guid", item.Title)
   writer.WriteElementString("pubDate", item.PublishedOnDate.ToString("r"))
   ' extensions
   writer.WriteStartElement(nsMediaPre, "thumbnail", nsMediaFull)
   writer.WriteAttributeString("width", "self")
   writer.WriteAttributeString("height", "self")
   writer.WriteAttributeString("url", "self")
   writer.WriteEndElement() ' thumbnail

   writer.WriteEndElement() ' item

  End Sub
#End Region

 End Class
End Namespace
