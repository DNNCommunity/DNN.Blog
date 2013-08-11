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
Imports System.Data.SqlClient
Imports Microsoft.ApplicationBlocks.Data

Namespace Data

 Partial Public Class SqlDataProvider

  Public Overrides Function AddCommentKarma(commentId As Int32, userId As Int32, karma As Int32) As Integer
   Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "AddCommentKarma", commentId, userId, karma), Integer)
  End Function

  Public Overrides Sub AddPostView(contentItemId As Int32)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "AddPostView", contentItemId)
  End Sub

  Public Overrides Sub ApproveComment(commentId As Int32)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "ApproveComment", commentId)
  End Sub

  Public Overrides Sub DeleteBlogPermissions(blogId As Int32)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "DeleteBlogPermissions", blogId)
  End Sub

  Public Overrides Function GetAuthors(moduleId As Int32, blogID As Int32) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetAuthors", moduleId, blogID), IDataReader)
  End Function

  Public Overrides Function GetBlog(blogId As Int32, userId As Int32, locale As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetBlog", blogId, userId, locale), IDataReader)
  End Function

  Public Overrides Function GetBlogCalendar(moduleId As Int32, blogId As Int32, locale As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetBlogCalendar", moduleId, blogId, locale), IDataReader)
  End Function

  Public Overrides Function GetBlogLocalizations(blogId As Int32) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetBlogLocalizations", blogId), IDataReader)
  End Function

  Public Overrides Function GetBlogPermissionsByBlog(blogId As Int32) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetBlogPermissionsByBlog", blogId), IDataReader)
  End Function

  Public Overrides Function GetBlogsByModule(moduleId As Int32, userId As Int32, locale As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetBlogsByModule", moduleId, userId, locale), IDataReader)
  End Function

  Public Overrides Function GetBlogsByPortal(portalId As Int32, userId As Int32, locale As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetBlogsByPortal", portalId, userId, locale), IDataReader)
  End Function

  Public Overrides Function GetComment(commentID As Int32, userID As Int32) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetComment", commentID, userID), IDataReader)
  End Function

  Public Overrides Function GetCommentsByContentItem(contentItemId As Int32, includeNonApproved As Boolean, userID As Int32) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetCommentsByContentItem", contentItemId, includeNonApproved, userID), IDataReader)
  End Function

  Public Overrides Function GetCommentsByModuleId(moduleId As Int32, userID As Int32, pageIndex As Int32, pageSize As Int32, orderBy As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetCommentsByModuleId", moduleId, userID, pageIndex, pageSize, orderBy), IDataReader)
  End Function

  Public Overrides Function GetPostByLegacyEntryId(entryId As Int32, portalId As Int32, locale As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetPostByLegacyEntryId", entryId, portalId, locale), IDataReader)
  End Function

  Public Overrides Function GetPostByLegacyUrl(url As String, portalId As Int32, locale As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetPostByLegacyUrl", url, portalId, locale), IDataReader)
  End Function

  Public Overrides Function GetPostLocalizations(contentItemId As Int32) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetPostLocalizations", contentItemId), IDataReader)
  End Function

  Public Overrides Function GetPosts(moduleId As Int32, blogID As Int32, displayLocale As String, userId As Int32, userIsAdmin As Boolean, published As Int32, limitToLocale As String, endDate As Date, authorUserId As Int32, onlyActionable As Boolean, pageIndex As Int32, pageSize As Int32, orderBy As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetPosts", moduleId, blogID, displayLocale, userId, userIsAdmin, published, limitToLocale, GetNull(endDate), authorUserId, onlyActionable, pageIndex, pageSize, orderBy), IDataReader)
  End Function

  Public Overrides Function GetPostsByTerm(moduleId As Int32, blogID As Int32, displayLocale As String, userId As Int32, userIsAdmin As Boolean, termID As Int32, published As Int32, limitToLocale As String, endDate As Date, authorUserId As Int32, pageIndex As Int32, pageSize As Int32, orderBy As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetPostsByTerm", moduleId, blogID, displayLocale, userId, userIsAdmin, termID, published, GetNull(limitToLocale), endDate, authorUserId, pageIndex, pageSize, orderBy), IDataReader)
  End Function

  Public Overrides Function GetTerm(termId As Int32, moduleId As Int32, locale As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetTerm", termId, moduleId, locale), IDataReader)
  End Function

  Public Overrides Function GetTermLocalizations(termId As Int32) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetTermLocalizations", termId), IDataReader)
  End Function

  Public Overrides Function GetTermsByModule(moduleId As Int32, locale As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetTermsByModule", moduleId, locale), IDataReader)
  End Function

  Public Overrides Function GetTermsByPost(contentItemId As Int32, moduleId As Int32, locale As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetTermsByPost", contentItemId, moduleId, locale), IDataReader)
  End Function

  Public Overrides Function GetTermsByVocabulary(moduleId As Int32, vocabularyId As Int32, locale As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetTermsByVocabulary", moduleId, vocabularyId, locale), IDataReader)
  End Function

  Public Overrides Function GetUserPermissionsByModule(moduleID As Int32, userId As Int32) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetUserPermissionsByModule", moduleID, userId), IDataReader)
  End Function

  Public Overrides Function GetUsersByBlogPermission(portalId As Int32, blogId As Int32, permissionId As Int32) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetUsersByBlogPermission", portalId, blogId, permissionId), IDataReader)
  End Function

  Public Overrides Function SearchPosts(moduleId As Int32, blogID As Int32, displayLocale As String, userId As Int32, userIsAdmin As Boolean, searchText As String, searchTitle As Boolean, searchContents As Boolean, published As Int32, limitToLocale As String, endDate As Date, authorUserId As Int32, pageIndex As Int32, pageSize As Int32, orderBy As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "SearchPosts", moduleId, blogID, displayLocale, userId, userIsAdmin, searchText, searchTitle, searchContents, published, GetNull(limitToLocale), endDate, authorUserId, pageIndex, pageSize, orderBy), IDataReader)
  End Function

  Public Overrides Function SearchPostsByTerm(moduleId As Int32, blogID As Int32, displayLocale As String, userId As Int32, userIsAdmin As Boolean, termID As Int32, searchText As String, searchTitle As Boolean, searchContents As Boolean, published As Int32, limitToLocale As String, endDate As Date, authorUserId As Int32, pageIndex As Int32, pageSize As Int32, orderBy As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "SearchPostsByTerm", moduleId, blogID, displayLocale, userId, userIsAdmin, termID, searchText, searchTitle, searchContents, published, GetNull(limitToLocale), endDate, authorUserId, pageIndex, pageSize, orderBy), IDataReader)
  End Function

  Public Overrides Sub SetBlogLocalization(blogID As Int32, locale As String, title As String, description As String)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "SetBlogLocalization", blogID, locale, title, description)
  End Sub

  Public Overrides Sub SetPostLocalization(postID As Int32, locale As String, title As String, summary As String, content As String, updatedByUser As Int32)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "SetPostLocalization", postID, locale, title, summary, content, updatedByUser)
  End Sub

  Public Overrides Function SetTerm(termID As Int32, vocabularyID As Int32, parentTermID As Int32, viewOrder As Int32, name As String, description As String, createdByUserID As Int32) As Integer
   Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "SetTerm", termID, vocabularyID, parentTermID, viewOrder, name, description, createdByUserID), Integer)
  End Function

  Public Overrides Sub SetTermLocalization(termID As Int32, locale As String, name As String, description As String)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "SetTermLocalization", termID, locale, name, description)
  End Sub

  Public Overrides Sub UpdateModuleWiring(portalId As Int32, oldModuleId As Int32, newModuleId As Int32)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "UpdateModuleWiring", portalId, oldModuleId, newModuleId)
  End Sub

 End Class

End Namespace

