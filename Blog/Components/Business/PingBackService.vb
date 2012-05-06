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
Imports System
Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports DotNetNuke.Modules.Blog.Business
Imports DotNetNuke.Modules.Blog.Components.Entities

Namespace Components.Business


#Region "Class PingBackService"

    Public Class PingBackService

#Region "Public methods"

        Public Shared Sub SendTrackBack(ByVal url As String, ByVal entry As EntryInfo, ByVal blogname As String)
            Try
                If Not IsNothing(url) Then
                    If url.Length > 0 Then
                        Dim desc As String
                        Dim entr As String
                        Dim TrackbackMsg As String = "url=" & HttpUtility.UrlEncode(entry.PermaLink)
                        TrackbackMsg += "&title=" & HttpUtility.UrlEncode(entry.Title)
                        Dim excerpt As String
                        If Not IsNothing(entry.Description) And entry.Description <> "" Then
                            desc = Utility.removeAllHtmltags(entry.Description)
                            If desc.Length > 200 Then
                                excerpt = desc.Substring(0, 200)
                            Else
                                excerpt = desc
                            End If
                        Else
                            entr = HttpUtility.HtmlDecode(entry.Entry)
                            entr = Utility.removeAllHtmltags(entr)
                            If entr.Length > 200 Then
                                excerpt = HttpUtility.HtmlDecode(entr.Substring(0, 200))
                            Else
                                excerpt = HttpUtility.HtmlDecode(entr)
                            End If
                        End If
                        If Not IsNothing(excerpt) Then
                            TrackbackMsg += "&excerpt=" & HttpUtility.UrlEncode(Utility.removeHtmlTags(excerpt & " ..."))
                        End If
                        TrackbackMsg += "&blog_name=" & blogname
                        Dim request As WebRequest = WebRequest.Create(New Uri(url))
                        request.Method = "POST"
                        request.ContentType = "application/x-www-form-urlencoded"

                        Dim requestWriter As StreamWriter = New StreamWriter(request.GetRequestStream)
                        Try
                            requestWriter.Write(TrackbackMsg)
                        Finally
                            requestWriter.Close()
                        End Try

                        request.GetResponse()

                    End If
                End If
            Catch ex As Exception

            End Try
        End Sub
        Public Function CheckForURL(ByVal sURI As String, _
                                    ByVal tURI As String, ByVal pageTitle As String) As String
            Try
                Dim page As String = GetPageHTML(sURI)
                If (page.Trim = "") Or ((page.IndexOf(HttpUtility.HtmlEncode(tURI)) < 0) And (page.IndexOf(HttpUtility.HtmlEncode(ToSfuUrl(tURI))) < 0)) Then
                    Return "0"
                Else
                    Dim pat As String = "<head.*?>.*<title.*?>(.*)</title.*?>.*</head.*?>"
                    Dim reg As Regex = New Regex(pat, RegexOptions.Singleline)   ' 11/19/2008 Rip Rowan - replaced RegexOptions.Ignorecase.Singleline
                    Dim m As Match = reg.Match(page)
                    If m.Success Then
                        pageTitle = m.Result("$1")
                    Else
                        Return pageTitle
                    End If
                End If
            Catch ex As Exception
                Return "0"
            End Try
            Return pageTitle
        End Function

        Public Function GetPageHTML(ByVal inURL As String) As String
            Dim req As WebRequest = WebRequest.Create(inURL)
            ' 11/19/2008 Rip Rowan - replaced Null with Nothing
            ' Dim Null As Object
            Dim wreq As HttpWebRequest = CType(req, HttpWebRequest)
            If Not (wreq Is Nothing) Then
                wreq.UserAgent = "Blog for DNN"
                wreq.Referer = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)
                wreq.Timeout = 60000
            End If
            Dim response As HttpWebResponse = CType(wreq.GetResponse, HttpWebResponse)
            Dim s As Stream = response.GetResponseStream
            Dim enc As String = response.ContentEncoding.Trim
            If enc = "" Then enc = "us-ascii"
            Dim encode As Encoding = System.Text.Encoding.GetEncoding(enc)
            Dim sr As StreamReader = New StreamReader(s, encode)
            Return sr.ReadToEnd
        End Function

        ' if SFU URL is used, check the sfu part of the URL
        Private Function ToSfuUrl(ByVal path As String) As String
            Dim sesPath As String = path
            Dim queryStringMatch As Match = Regex.Match(path, "(.[^\\?]*)\\?(.*)", RegexOptions.IgnoreCase)
            If Not (queryStringMatch Is Match.Empty) Then
                Try
                    sesPath = ""
                    Dim queryString As String = queryStringMatch.Groups(2).Value.Replace("&amp;", "&")
                    If queryString.StartsWith("?") Then
                        queryString = queryString.Remove(0, 1)
                    End If
                    Dim nameValuePairs As String() = queryString.Split("&"c)

                    Dim idx As Integer = 0
                    While idx < nameValuePairs.Length
                        Dim pair As String() = nameValuePairs(idx).Split("="c)
                        sesPath += CType(IIf((pair(0).Length > 0), "/" + pair(0), EmptySesParamValueIdentifier), String)
                        If (pair.Length = 1) AndAlso (pair(0).StartsWith("#")) Then
                        Else
                            sesPath += CType(IIf((pair(1).Length > 0), "/" + pair(1), "/" + EmptySesParamValueIdentifier), String)
                        End If
                        idx = idx + 1
                    End While
                    sesPath += "/Default.aspx"
                Catch ex As Exception
                    System.Diagnostics.Debug.WriteLine(ex.Message)
                End Try
            End If
            Return sesPath
        End Function
        Private ReadOnly Property EmptySesParamValueIdentifier() As String
            Get
                Return System.Web.HttpUtility.UrlEncode(" ").ToString
            End Get
        End Property

#End Region

    End Class

#End Region
End Namespace