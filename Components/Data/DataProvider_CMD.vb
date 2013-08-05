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
Imports DotNetNuke

Namespace Data

 Partial Public MustInherit Class DataProvider

  Public MustOverride Sub AddPostView(contentItemId As Int32)
  Public MustOverride Sub ApproveComment(commentId As Int32)
  Public MustOverride Function AddCommentKarma(commentId As Int32, userId As Int32, karma As Int32) As Integer
  Public MustOverride Sub DeleteBlogPermissions(blogId As Int32)
  Public MustOverride Function GetAuthors(moduleId As Int32, blogID As Int32) As IDataReader
  Public MustOverride Function GetBlog(blogId As Int32, userId As Int32, locale As String) As IDataReader
  Public MustOverride Function GetBlogCalendar(moduleId As Int32, blogId As Int32, locale As String) As IDataReader
  Public MustOverride Function GetBlogLocalizations(blogId As Int32) As IDataReader
  Public MustOverride Function GetBlogPermissionsByBlog(blogId As Int32) As IDataReader
  Public MustOverride Function GetBlogsByModule(moduleId As Int32, userId As Int32, locale As String) As IDataReader
  Public MustOverride Function GetBlogsByPortal(portalId As Int32, userId As Int32, locale As String) As IDataReader
  Public MustOverride Function GetComment(commentID As Int32, userID As Int32) As IDataReader
  Public MustOverride Function GetCommentsByContentItem(contentItemId As Int32, includeNonApproved As Boolean, userID As Int32) As IDataReader
  Public MustOverride Function GetCommentsByModuleId(moduleId As Int32, userID As Int32) As IDataReader
  Public MustOverride Function GetPostByLegacyEntryId(entryId As Int32, portalId As Int32, locale As String) As IDataReader
  Public MustOverride Function GetPostByLegacyUrl(url As String, portalId As Int32, locale As String) As IDataReader
  Public MustOverride Function GetPostLocalizations(contentItemId As Int32) As IDataReader
  Public MustOverride Function GetPosts(moduleId As Int32, blogID As Int32, displayLocale As String, userId As Int32, userIsAdmin As Boolean, published As Int32, limitToLocale As String, endDate As Date, authorUserId As Int32, onlyActionable As Boolean, pageIndex As Int32, pageSize As Int32, orderBy As String) As IDataReader
  Public MustOverride Function GetPostsByTerm(moduleId As Int32, blogID As Int32, displayLocale As String, userId As Int32, userIsAdmin As Boolean, termID As Int32, published As Int32, limitToLocale As String, endDate As Date, authorUserId As Int32, pageIndex As Int32, pageSize As Int32, orderBy As String) As IDataReader
  Public MustOverride Function GetTerm(termId As Int32, moduleId As Int32, locale As String) As IDataReader
  Public MustOverride Function GetTermLocalizations(termId As Int32) As IDataReader
  Public MustOverride Function GetTermsByModule(moduleId As Int32, locale As String) As IDataReader
  Public MustOverride Function GetTermsByPost(contentItemId As Int32, moduleId As Int32, locale As String) As IDataReader
  Public MustOverride Function GetTermsByVocabulary(moduleId As Int32, vocabularyId As Int32, locale As String) As IDataReader
  Public MustOverride Function GetUserPermissionsByModule(moduleID As Int32, userId As Int32) As IDataReader
  Public MustOverride Function GetUsersByBlogPermission(portalId As Int32, blogId As Int32, permissionId As Int32) As IDataReader
  Public MustOverride Function SearchPosts(moduleId As Int32, blogID As Int32, displayLocale As String, userId As Int32, userIsAdmin As Boolean, searchText As String, searchTitle As Boolean, searchContents As Boolean, published As Int32, limitToLocale As String, endDate As Date, authorUserId As Int32, pageIndex As Int32, pageSize As Int32, orderBy As String) As IDataReader
  Public MustOverride Function SearchPostsByTerm(moduleId As Int32, blogID As Int32, displayLocale As String, userId As Int32, userIsAdmin As Boolean, termID As Int32, searchText As String, searchTitle As Boolean, searchContents As Boolean, published As Int32, limitToLocale As String, endDate As Date, authorUserId As Int32, pageIndex As Int32, pageSize As Int32, orderBy As String) As IDataReader
  Public MustOverride Sub SetBlogLocalization(blogID As Int32, locale As String, title As String, description As String)
  Public MustOverride Sub SetPostLocalization(postID As Int32, locale As String, title As String, summary As String, content As String, updatedByUser As Int32)
  Public MustOverride Function SetTerm(termID As Int32, vocabularyID As Int32, parentTermID As Int32, viewOrder As Int32, name As String, description As String, createdByUserID As Int32) As Integer
  Public MustOverride Sub SetTermLocalization(termID As Int32, locale As String, name As String, description As String)
  Public MustOverride Sub UpdateModuleWiring(portalId As Int32, oldModuleId As Int32, newModuleId As Int32)

 End Class

End Namespace