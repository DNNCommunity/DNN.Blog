'
' Bring2mind - http://www.bring2mind.net
' Copyright (c) 2013
' by Bring2mind
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
Imports System.Data
Imports System.Linq
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Tokens

Imports DotNetNuke.Modules.Blog.Data
Imports DotNetNuke.Entities.Content.Taxonomy

Namespace Entities.Posts

 Partial Public Class PostsController

  Public Shared Function GetPost(contentItemId As Int32, moduleId As Int32, locale As String) As PostInfo

   Return CType(CBO.FillObject(DataProvider.Instance().GetPost(contentItemId, moduleId, locale), GetType(PostInfo)), PostInfo)

  End Function

  Public Shared Function AddPost(ByRef objPost As PostInfo, createdByUser As Integer) As Integer

   objPost.ContentItemId = DataProvider.Instance().AddPost(objPost.AllowComments, objPost.BlogID, objPost.Content, objPost.Copyright, objPost.DisplayCopyright, objPost.Image, objPost.Locale, objPost.Published, objPost.PublishedOnDate, objPost.Summary, objPost.Terms.ToTermIDString, objPost.Title, objPost.ViewCount, createdByUser)

   ' localization
   For Each l As String In objPost.TitleLocalizations.Locales
    DataProvider.Instance().SetPostLocalization(objPost.ContentItemId, l, objPost.TitleLocalizations(l), objPost.SummaryLocalizations(l), objPost.ContentLocalizations(l), createdByUser)
   Next

   Return objPost.ContentItemId

  End Function

  Public Shared Sub UpdatePost(objPost As PostInfo, updatedByUser As Integer)

   DataProvider.Instance().UpdatePost(objPost.AllowComments, objPost.BlogID, objPost.Content, objPost.ContentItemId, objPost.Copyright, objPost.DisplayCopyright, objPost.Image, objPost.Locale, objPost.Published, objPost.PublishedOnDate, objPost.Summary, objPost.Terms.ToTermIDString, objPost.Title, objPost.ViewCount, updatedByUser)

   ' localization
   For Each l As String In objPost.TitleLocalizations.Locales
    DataProvider.Instance().SetPostLocalization(objPost.ContentItemId, l, objPost.TitleLocalizations(l), objPost.SummaryLocalizations(l), objPost.ContentLocalizations(l), updatedByUser)
   Next

  End Sub

  Public Shared Sub DeletePost(contentItemId As Int32)

   DataProvider.Instance().DeletePost(contentItemId)

  End Sub

 End Class
End Namespace