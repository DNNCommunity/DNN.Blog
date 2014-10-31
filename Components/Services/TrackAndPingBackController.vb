'
' DNN Connect - http://dnn-connect.org
' Copyright (c) 2014
' by DNN Connect
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

Imports System.Linq
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports DotNetNuke.Modules.Blog.Entities.Posts
Imports System.Net

Namespace Services
 Public Class TrackAndPingBackController

#Region " Private Members "
  Private Shared ReadOnly TrackbackLinkRegex As New Regex("trackback:ping=""([^""]+)""", RegexOptions.IgnoreCase Or RegexOptions.Compiled)
  Private Shared ReadOnly UrlsRegex As New Regex("<a.*?href=[""'](?<url>.*?)[""'].*?>(?<name>.*?)</a>", RegexOptions.IgnoreCase Or RegexOptions.Compiled)
  Private Property Post As PostInfo = Nothing
  Private Property PortalSettings As DotNetNuke.Entities.Portals.PortalSettings = Nothing
#End Region

#Region " TrackbackMessage "
  Public Structure TrackbackMessage

#Region " Properties "
   Public Property BlogName As String
   Public Property Excerpt As String
   Public Property PostUrl As Uri
   Public Property Title As String
   Public Property UrlToNotifyTrackback As Uri
#End Region

#Region " Constructors "
   Public Sub New(post As PostInfo, urlToNotifyTrackback As Uri, portalSettings As DotNetNuke.Entities.Portals.PortalSettings)
    Me.Title = post.Title
    Me.PostUrl = New Uri(post.PermaLink(portalSettings))
    Me.Excerpt = post.Summary
    Me.BlogName = post.Blog.Title
    Me.UrlToNotifyTrackback = urlToNotifyTrackback
   End Sub
#End Region

#Region " Public Methods "
   Public Overrides Function ToString() As String
    Return String.Format(Globalization.CultureInfo.InvariantCulture, "title={0}&url={1}&excerpt={2}&blog_name={3}", Me.Title, Me.PostUrl, Me.Excerpt, Me.BlogName)
   End Function
#End Region

  End Structure
#End Region

#Region " Constructors "
  Public Sub New(post As PostInfo)
   Me.Post = post
   Me.PortalSettings = DotNetNuke.Entities.Portals.PortalSettings.Current
  End Sub
#End Region

#Region " Public Methods "
  Public Sub SendTrackAndPingBacks()

   If Not Post.Blog.EnableTrackBackSend And Not Post.Blog.EnablePingBackSend Then Exit Sub

   For Each url As Uri In GetUrlsFromContent(HttpUtility.HtmlDecode(Post.Content))

    Dim trackbackSent As Boolean = False
    If Post.Blog.EnableTrackBackSend Then
     Dim remoteFile As New WebPage(url)
     Dim pageContent As String = remoteFile.GetFileAsString
     Dim trackbackUrl As Uri = GetTrackBackUrlFromPage(pageContent)
     If trackbackUrl IsNot Nothing Then
      Dim message As New TrackbackMessage(Post, trackbackUrl, PortalSettings)
      trackbackSent = SendTrackback(message)
     End If
    End If
    If Not trackbackSent AndAlso Post.Blog.EnablePingBackSend Then
     SendPingback(New Uri(Post.PermaLink(PortalSettings)), url)
    End If

   Next

  End Sub
#End Region

#Region " Trackback "
  Public Function SendTrackback(message As TrackbackMessage) As Boolean

   Dim request As HttpWebRequest = DirectCast(WebRequest.Create(message.UrlToNotifyTrackback), HttpWebRequest)

   request.Credentials = CredentialCache.DefaultNetworkCredentials
   request.Method = "POST"
   request.ContentLength = message.ToString().Length
   request.ContentType = "application/x-www-form-urlencoded"
   request.KeepAlive = False
   request.Timeout = 30000

   Using writer As New IO.StreamWriter(request.GetRequestStream)
    writer.Write(message.ToString)
   End Using

   Dim result As Boolean = False
   Dim response As HttpWebResponse
   Try
    response = DirectCast(request.GetResponse(), HttpWebResponse)
    Dim answer As String
    Using sr As New IO.StreamReader(response.GetResponseStream)
     answer = sr.ReadToEnd
    End Using
    result = CBool(response.StatusCode = HttpStatusCode.OK AndAlso answer.Contains("<error>0</error>"))
   Catch ex As Exception
   End Try

   Return result

  End Function
#End Region

#Region " Pingback "
  Public Sub SendPingback(sourceUrl As Uri, targetUrl As Uri)

   Try

    Dim pingUrl As String = Nothing
    Dim request As HttpWebRequest = DirectCast(WebRequest.Create(targetUrl), HttpWebRequest)
    request.Credentials = CredentialCache.DefaultNetworkCredentials
    Using response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
     For Each k As String In response.Headers.AllKeys
      If k.Equals("x-pingback", StringComparison.OrdinalIgnoreCase) OrElse k.Equals("pingback", StringComparison.OrdinalIgnoreCase) Then
       pingUrl = response.Headers(k)
      End If
     Next
     If String.IsNullOrEmpty(pingUrl) Then
      Using reader As New IO.StreamReader(response.GetResponseStream())
       Dim content As String = reader.ReadToEnd()
       Dim m As Match = Regex.Match(content, "<link[^>]*rel=""pingback""[^>]*>|<link[^>]*rel='pingback'[^>]*>")
       If m.Success Then
        Dim link As String = m.Value
        Dim m2 As Match = Regex.Match(link, "href=""(?<link>[^""]*)""|href='(?<link>[^']*)'")
        If m2.Success Then
         pingUrl = m2.Groups("link").Value.Replace("&amp;", "&")
        End If
       End If
      End Using
     End If
    End Using

    Dim url As Uri = Nothing
    If Not String.IsNullOrEmpty(pingUrl) AndAlso Uri.TryCreate(pingUrl, UriKind.Absolute, url) Then
     request = DirectCast(WebRequest.Create(url), HttpWebRequest)
     request.Method = "POST"
     request.Timeout = 30000
     request.ContentType = "text/xml"
     request.ProtocolVersion = HttpVersion.Version11
     request.Headers("Accept-Language") = "en-us"
     AddXmlToRequest(sourceUrl, targetUrl, request)
     Using response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
     End Using
    End If

   Catch ex As Exception
    ex = New Exception()
   End Try

  End Sub
#End Region

#Region " Private Methods "
  Private Shared Function GetTrackBackUrlFromPage(input As String) As Uri
   Dim url As String = TrackbackLinkRegex.Match(input).Groups(1).ToString().Trim()
   Dim uri As Uri = Nothing
   Return If(uri.TryCreate(url, UriKind.Absolute, uri), uri, Nothing)
  End Function

  Private Shared Function GetUrlsFromContent(content As String) As IEnumerable(Of Uri)
   Dim urlsList As New List(Of Uri)
   For Each m As Match In UrlsRegex.Matches(content)
    Dim url As String = m.Groups("url").ToString().Trim()
    Dim uri As Uri = Nothing
    If uri.TryCreate(url, UriKind.Absolute, uri) Then
     urlsList.Add(uri)
    End If
   Next
   Return urlsList
  End Function

  Private Shared Sub AddXmlToRequest(sourceUrl As Uri, targetUrl As Uri, webreqPing As HttpWebRequest)
   Using stream As IO.Stream = webreqPing.GetRequestStream
    Using writer As New System.Xml.XmlTextWriter(stream, Encoding.ASCII)
     writer.WriteStartDocument(True)
     writer.WriteStartElement("methodCall")
     writer.WriteElementString("methodName", "pingback.ping")
     writer.WriteStartElement("params")

     writer.WriteStartElement("param")
     writer.WriteStartElement("value")
     writer.WriteElementString("string", sourceUrl.ToString())
     writer.WriteEndElement()
     writer.WriteEndElement()

     writer.WriteStartElement("param")
     writer.WriteStartElement("value")
     writer.WriteElementString("string", targetUrl.ToString())
     writer.WriteEndElement()
     writer.WriteEndElement()

     writer.WriteEndElement()
     writer.WriteEndElement()
    End Using
   End Using
  End Sub
#End Region

 End Class
End Namespace