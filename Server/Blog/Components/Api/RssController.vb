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

Imports System.Net
Imports System.Net.Http
Imports System.Web.Http
Imports DotNetNuke.Modules.Blog.Core.Common
Imports DotNetNuke.Modules.Blog.Core.Rss
Imports DotNetNuke.Web.Api

Namespace Api
  Public Class RssController
    Inherits DnnApiController

    <HttpGet()>
    <DnnModuleAuthorize(AccessLevel:=DotNetNuke.Security.SecurityAccessLevel.View)>
    <ActionName("Get")>
    Public Function GetRss() As HttpResponseMessage
      Dim res As New HttpResponseMessage(HttpStatusCode.OK)
      Dim queryString As NameValueCollection = HttpUtility.ParseQueryString(Request.RequestUri.Query)
      Dim feed As New BlogRssFeed(ActiveModule.ModuleID, queryString)
      res.Content = New StringContent(Globals.ReadFile(feed.CacheFile), Encoding.UTF8, "application/xml")
      Return res
    End Function

  End Class
End Namespace
