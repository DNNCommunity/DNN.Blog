'
' DNN Connect - http://dnn-connect.org
' Copyright (c) 2015
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

Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Net
Imports System.Security
Imports System.IO

Namespace Common
 Public Class WebPage

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
   Try
    Using response As WebResponse = Me.GetWebResponse()
     If response Is Nothing Then
      Return String.Empty
     End If
     Using reader As New StreamReader(response.GetResponseStream())
      Return reader.ReadToEnd()
     End Using
    End Using
   Catch ex As Exception
    DotNetNuke.Services.Exceptions.LogException(New Exception(String.Format("Track/Pingback Verification Request To '{0}' Failed", Me.Uri.PathAndQuery), ex))
    Return ""
   End Try
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
