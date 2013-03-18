'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2011
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
Option Strict On
Option Explicit On

Imports DotNetNuke.Modules.Blog.Common
Imports System.Globalization
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json
Imports System.Net
Imports System.Net.Http
Imports System.Web.Http
Imports DotNetNuke.Web.Api
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Entities.Entries
Imports DotNetNuke.Modules.Blog.Security
Imports DotNetNuke.Modules.Blog.Integration
Imports DotNetNuke.Entities.Modules

Namespace Services

 Public Class RSSController
  Inherits DnnApiController

#Region " Private Members "
#End Region

#Region " Service Methods "
  <HttpGet()>
  <DnnModuleAuthorize(accesslevel:=DotNetNuke.Security.SecurityAccessLevel.View)>
  <ActionName("Get")>
  Public Function GetRss() As HttpResponseMessage
   Dim BlogId As Integer = -1
   Dim TermId As Integer = -1
   Dim queryString As NameValueCollection = HttpUtility.ParseQueryString(Me.Request.RequestUri.Query)
   queryString.ReadValue("Blog", BlogId)
   queryString.ReadValue("Term", TermId)



   Dim entryList As IEnumerable(Of EntryInfo)
   Dim totalRecords As Integer = -1
   Dim recordsToSend As Integer = 10
   If TermId > -1 Then
    entryList = EntriesController.GetEntriesByTerm(ActiveModule.ModuleID, BlogId, TermId, -1, Date.Now, -1, 0, recordsToSend, "PUBLISHEDONDATE DESC", totalRecords, UserInfo.UserID, False).Values
   Else
    entryList = EntriesController.GetEntries(ActiveModule.ModuleID, BlogId, -1, Date.Now, -1, 0, recordsToSend, "PUBLISHEDONDATE DESC", totalRecords, UserInfo.UserID, False).Values
   End If


  End Function
#End Region

#Region " Private Methods "
#End Region

 End Class

End Namespace