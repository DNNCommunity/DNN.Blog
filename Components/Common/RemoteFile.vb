Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Net
Imports System.Security
Imports System.IO

Namespace Common
 Friend NotInheritable Class RemoteFile

#Region " Properties "
  Private ReadOnly url As Uri
  Public ReadOnly Property Uri() As Uri
   Get
    Return Me.url
   End Get
  End Property
#End Region

#Region " Constructors "
  Friend Sub New(filePath As Uri)
   If filePath Is Nothing Then
    Throw New ArgumentNullException("filePath")
   End If
   Me.url = filePath
  End Sub
#End Region

#Region " Public Methods "
  Public Function GetWebResponse() As WebResponse
   Dim response As WebResponse = Me.GetWebRequest().GetResponse()
   Dim contentLength As Long = response.ContentLength
   If contentLength = -1 Then
    Dim headerContentLength As String = response.Headers("Content-Length")
    If Not [String].IsNullOrEmpty(headerContentLength) Then
     contentLength = Long.Parse(headerContentLength)
    End If
   End If
   If contentLength <= -1 Then
    response.Close()
    Return Nothing
   End If
   Return response
  End Function

  Private _webRequest As WebRequest

  Public Function GetFileAsString() As String
   Using response As WebResponse = Me.GetWebResponse()
    If response Is Nothing Then
     Return String.Empty
    End If
    Using reader As New StreamReader(response.GetResponseStream())
     Return reader.ReadToEnd()
    End Using
   End Using
  End Function
#End Region

#Region " Private Methods "
  Private Function GetWebRequest() As WebRequest

   If Me._webRequest Is Nothing Then
    Dim request As HttpWebRequest = DirectCast(WebRequest.Create(Me.Uri), HttpWebRequest)
    request.Headers("Accept-Encoding") = "gzip"
    request.Headers("Accept-Language") = "en-us"
    request.Credentials = CredentialCache.DefaultNetworkCredentials
    request.AutomaticDecompression = DecompressionMethods.GZip
    request.Timeout = 1000 * 30 ' 30 secs timeout
    Me._webRequest = request
   End If
   Return Me._webRequest

  End Function
#End Region

 End Class
End Namespace
