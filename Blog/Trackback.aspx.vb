'
' DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2005
' by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
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
'-------------------------------------------------------------------------

Imports System
Imports System.Xml
Imports DotNetNuke.Modules.Blog.Business
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Entities.Users



#Region "Class Trackback"
Partial Class Trackback
 Inherits DotNetNuke.Framework.PageBase
 'Inherits System.Web.UI.Page

#Region "Private member"
 Private m_oBlogController As New BlogController
 Private m_oEntryController As New EntryController
 Private m_oCommentController As New CommentController
#End Region

#Region " Vom Web Form Designer generierter Code "

 'Dieser Aufruf ist für den Web Form-Designer erforderlich.
 <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

 End Sub

 'HINWEIS: Die folgende Platzhalterdeklaration ist für den Web Form-Designer erforderlich.
 'Nicht löschen oder verschieben.
 Private designerPlaceholderDeclaration As System.Object

 Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
  'CODEGEN: Dieser Methodenaufruf ist für den Web Form-Designer erforderlich
  'Verwenden Sie nicht den Code-Editor zur Bearbeitung.
  InitializeComponent()
 End Sub

#End Region

#Region "Event Handlers"
 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

  Try
   Response.Clear()
   Response.ContentType = "text/xml"
   If Request.Params("id") <> "" Then
    Dim entryId As Integer = CType(Request.Params("id"), Integer)
    Dim title As String = ""
    Dim excerpt As String = ""
    Dim url As String = ""
    Dim blog_name As String = ""
    Dim BlogId As Integer

    If Not Request.Params("title") Is Nothing Then
     title = Request.Params("title").ToString
    End If

    If Not Request.Params("blogid") Is Nothing Then
     BlogId = CInt(Request.Params("blogid"))
    End If

    If Not Request.Params("excerpt") Is Nothing Then
     excerpt = Request.Params("excerpt").ToString
    End If
    If Not Request.Params("url") Is Nothing Then
     Dim aurl As String()
     aurl = Split(Request.Params("url"), ",")
     url = aurl(0)
    End If
    If Not Request.Params("blog_name") Is Nothing Then
     blog_name = Request.Params("blog_name").ToString
    End If

    Dim m_oEntry As EntryInfo
    Dim m_oBlog As BlogInfo

    m_oBlog = m_oBlogController.GetBlog(BlogId)
    m_oEntry = m_oEntryController.GetEntry(entryId, m_oBlog.PortalID)

    If Request.HttpMethod = "POST" Then
     If url.Length > 0 Then
      Dim objBlogs As New PingBackService
      Dim tmpTitle As String = objBlogs.CheckForURL(url, m_oEntry.PermaLink, title)
      If tmpTitle <> "0" Then
       Dim oComment As New CommentInfo
       oComment.EntryID = entryId
       oComment.Title = title
       oComment.UserID = -2
       oComment.Approved = Not m_oBlog.MustApproveTrackbacks

       oComment.Comment = Utility.removeAllHtmltags(excerpt) & CType(IIf(excerpt.Length > 0, "<br>", ""), String) & "<a href=""" & url & """ target=""_blank""># " & blog_name & "</a>"
       If m_oBlog.AllowTrackbacks Then
        m_oCommentController.AddComment(oComment)
        If m_oBlog.EmailNotification Then
         sendMail(m_oBlog, m_oEntry, oComment)
        End If
       End If
       TrackbackResponse(0, "")
      Else
       TrackbackResponse(1, "Sorry couldn't find a " & _
          "relevant link for " & m_oEntry.PermaLink & " in " & url)
      End If
     End If
    Else
     Dim description As String = ""
     Dim desc As String
     If Not IsNothing(m_oEntry.Description) And m_oEntry.Description <> "" Then
      desc = Utility.removeAllHtmltags(m_oEntry.Description)
      If desc.Length > 200 Then
       description = HttpUtility.HtmlDecode(desc.Substring(0, 200))
      Else
       description = HttpUtility.HtmlDecode(desc)
      End If
     Else
      Dim m_Entry As String = Utility.removeAllHtmltags(HttpUtility.HtmlDecode(m_oEntry.Entry))
      If m_Entry.Length > 200 Then
       description = m_Entry.Substring(0, 200)
      Else
       description = m_Entry
      End If
     End If

     Dim w As XmlTextWriter = New XmlTextWriter(Response.Output)
     w.Formatting = Formatting.Indented
     w.WriteStartDocument()
     w.WriteStartElement("response")
     w.WriteElementString("error", "0")
     w.WriteStartElement("rss")
     w.WriteAttributeString("version", "0.91")
     w.WriteStartElement("channel")

     'Antonio Chagoury
     'BLG(-4471): Fixed CDATA enconding in 
     'title and description for trackbacks

     w.WriteStartElement("title")
     w.WriteCData(m_oEntry.Title)
     w.WriteEndElement()

     w.WriteElementString("link", m_oEntry.PermaLink)

     w.WriteStartElement("description")
     w.WriteCData(Utility.RewriteRefs(Utility.removeHtmlTags(description & " ...")))
     w.WriteEndElement()

     w.WriteElementString("language", m_oBlog.Culture)
     w.WriteEndElement()
     w.WriteEndElement()
     w.WriteEndElement()
     w.WriteEndDocument()
     Response.End()
    End If
   Else

    TrackbackResponse(1, String.Format(Localization.GetString("msgTrackbackInvalid", LocalResourceFile), ID.ToLower()))
   End If
  Catch ex As Exception
   'no error handling because of Resopinse.End will always create an error
  End Try
 End Sub
#End Region

#Region "Private Methods"
 Private Sub TrackbackResponse(ByVal errNum As Integer, ByVal errText As String)

  Try
   Dim d As New XmlDocument
   Dim root As XmlElement = d.CreateElement("response")
   d.AppendChild(root)
   Dim er As XmlElement = d.CreateElement("error")
   root.AppendChild(er)
   er.AppendChild(d.CreateTextNode(errNum.ToString))
   If errText <> "" Then
    Dim msg As XmlElement = d.CreateElement("message")
    root.AppendChild(msg)
    msg.AppendChild(d.CreateTextNode(errText))
   End If
   Response.Clear()
   Response.ClearContent()
   Response.ContentType = "application/xml"
   d.Save(Response.OutputStream)
   Response.End()
  Catch ex As Exception
   'no error handling because of Resopinse.End will always create an error
  End Try
 End Sub

 Private Sub sendMail(ByVal oBlog As BlogInfo, ByVal oEntry As EntryInfo, ByVal Comment As CommentInfo)
  Dim strSubject As String = Localization.GetString("msgMailSubject", LocalResourceFile)
  Dim strBody As String = Localization.GetString("msgMailBody", LocalResourceFile)
  ' replace [BLOG] with Blog.Title
  strSubject = strSubject.Replace("[BLOG]", oBlog.Title)
  strBody = strBody.Replace("[BLOG]", oBlog.Title)
  ' replace [TITLE] with Comment.Title
  strSubject = strSubject.Replace("[TITLE]", Comment.Title)
  strBody = strBody.Replace("[TITLE]", Comment.Title)
  ' replace [COMMENT] with Comment.Comment
  strSubject = strSubject.Replace("[COMMENT]", Comment.Comment)
  strBody = strBody.Replace("[COMMENT]", Comment.Comment)
  ' replace [DATE] with Comment.Date
  strSubject = strSubject.Replace("[DATE]", Now.ToShortDateString)
  strBody = strBody.Replace("[DATE]", Now.ToShortDateString)
  ' replace [USER] with Comment.UserName
  strSubject = strSubject.Replace("[USER]", Comment.UserName)
  strBody = strBody.Replace("[USER]", Comment.UserName)
  ' replace [URL] with NavigateUrl()
  strSubject = strSubject.Replace("[URL]", oEntry.PermaLink)
  strBody = strBody.Replace("[URL]", "<a href='" & oEntry.PermaLink & "'>" & oBlog.Title & "</a>")

  Dim m_PortalSettings As DotNetNuke.Entities.Portals.PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), DotNetNuke.Entities.Portals.PortalSettings)
  Dim uc As New UserController
  Dim ui As UserInfo = uc.GetUser(m_PortalSettings.PortalId, oBlog.UserID)

  DotNetNuke.Services.Mail.Mail.SendMail(PortalSettings.Email, ui.Email, "", strSubject, strBody, "", "html", "", "", "", "")
 End Sub

#End Region

End Class
#End Region

#Region "Class TrackBackReferrer"
Public Class TrackBackReferrer
 Public EntryTitle As String = ""
 Public Excerpt As String = ""
 Public URL As String = ""
 Public BlogName As String = ""
 Public LocalEntryID As Integer
 Public PageBody As String
 Public LocalEntry As EntryInfo
End Class
#End Region

