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

Imports DotNetNuke.Modules.Blog.Data
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Modules.Blog.Integration
Imports DotNetNuke.Modules.Blog.Entities.Blogs

Namespace Entities.Posts

 Partial Public Class PostsController

  Public Shared Sub PublishPost(Post As PostInfo, publish As Boolean, publishedByUser As Integer)

   If Post.Published = publish Then Exit Sub
   Post.Published = publish
   UpdatePost(Post, publishedByUser)
   PublishPost(Post, publishedByUser)

  End Sub

  Public Shared Sub PublishPost(Post As PostInfo, publishedByUser As Integer)

   Dim blog As BlogInfo = BlogsController.GetBlog(Post.BlogID, publishedByUser, Threading.Thread.CurrentThread.CurrentCulture.Name)
   Dim journalUrl As String = Post.PermaLink(DotNetNuke.Entities.Portals.PortalSettings.Current)
   Dim journalUserId As Integer = publishedByUser
   If blog.PublishAsOwner Then journalUserId = blog.OwnerUserId
   JournalController.AddBlogPostToJournal(Post, DotNetNuke.Entities.Portals.PortalSettings.Current.PortalId, DotNetNuke.Entities.Portals.PortalSettings.Current.ActiveTab.TabID, journalUserId, journalUrl)
   NotificationController.RemovePostPendingNotification(blog.ModuleID, blog.BlogID, Post.ContentItemId)

   Dim trackAndPingbacks As New Services.TrackAndPingBackController(Post)
   Dim trd As New Threading.Thread(AddressOf trackAndPingbacks.SendTrackAndPingBacks)
   trd.IsBackground = True
   trd.Start()

  End Sub

  Public Shared Sub DeletePost(Post As PostInfo)
   DataProvider.Instance().DeletePost(Post.ContentItemId)
   Dim blog As BlogInfo = BlogsController.GetBlog(Post.BlogID, -1, Threading.Thread.CurrentThread.CurrentCulture.Name)
   NotificationController.RemovePostPendingNotification(blog.ModuleID, blog.BlogID, Post.ContentItemId)
  End Sub

  Public Shared Sub DeletePost(contentItemId As Integer, blogId As Integer, portalId As Integer, vocabularyId As Integer)
   DataProvider.Instance().DeletePost(contentItemId)
  End Sub

  Public Shared Function GetPosts(moduleId As Int32, blogID As Int32, displayLocale As String, published As Integer, limitToLocale As String, endDate As Date, authorUserId As Int32, onlyActionable As Boolean, pageIndex As Int32, pageSize As Int32, orderBy As String, ByRef totalRecords As Integer, userId As Integer, userIsAdmin As Boolean) As Dictionary(Of Integer, PostInfo)

   If pageIndex < 0 Then
    pageIndex = 0
    pageSize = Integer.MaxValue
   End If

   Dim res As New Dictionary(Of Integer, PostInfo)
   Using ir As IDataReader = DataProvider.Instance().GetPosts(moduleId, blogID, displayLocale, userId, userIsAdmin, published, limitToLocale, endDate, authorUserId, onlyActionable, pageIndex, pageSize, orderBy)
    res = DotNetNuke.Common.Utilities.CBO.FillDictionary(Of Integer, PostInfo)("ContentItemID", ir, False)
    ir.NextResult()
    totalRecords = DotNetNuke.Common.Globals.GetTotalRecords(ir)
   End Using
   Return GetPostsWithBlog(res, blogID, moduleId, userId, displayLocale)

  End Function

  Public Shared Function GetPostsByTerm(moduleId As Int32, blogID As Int32, displayLocale As String, termId As Integer, published As Integer, limitToLocale As String, endDate As Date, authorUserId As Int32, pageIndex As Int32, pageSize As Int32, orderBy As String, ByRef totalRecords As Integer, userId As Integer, userIsAdmin As Boolean) As Dictionary(Of Integer, PostInfo)

   If pageIndex < 0 Then
    pageIndex = 0
    pageSize = Integer.MaxValue
   End If

   Dim res As New Dictionary(Of Integer, PostInfo)
   Using ir As IDataReader = DataProvider.Instance().GetPostsByTerm(moduleId, blogID, displayLocale, userId, userIsAdmin, termId, published, limitToLocale, endDate, authorUserId, pageIndex, pageSize, orderBy)
    res = DotNetNuke.Common.Utilities.CBO.FillDictionary(Of Integer, PostInfo)("ContentItemID", ir, False)
    ir.NextResult()
    totalRecords = DotNetNuke.Common.Globals.GetTotalRecords(ir)
   End Using
   Return GetPostsWithBlog(res, blogID, moduleId, userId, displayLocale)

  End Function

  Public Shared Function GetPostsByCategory(moduleId As Int32, blogID As Int32, displayLocale As String, categories As String, published As Integer, limitToLocale As String, endDate As Date, authorUserId As Int32, pageIndex As Int32, pageSize As Int32, orderBy As String, ByRef totalRecords As Integer, userId As Integer, userIsAdmin As Boolean) As Dictionary(Of Integer, PostInfo)

   If pageIndex < 0 Then
    pageIndex = 0
    pageSize = Integer.MaxValue
   End If

   Dim res As New Dictionary(Of Integer, PostInfo)
   Using ir As IDataReader = DataProvider.Instance().GetPostsByCategory(moduleId, blogID, displayLocale, userId, userIsAdmin, categories, published, limitToLocale, endDate, authorUserId, pageIndex, pageSize, orderBy)
    res = DotNetNuke.Common.Utilities.CBO.FillDictionary(Of Integer, PostInfo)("ContentItemID", ir, False)
    ir.NextResult()
    totalRecords = DotNetNuke.Common.Globals.GetTotalRecords(ir)
   End Using
   Return GetPostsWithBlog(res, blogID, moduleId, userId, displayLocale)

  End Function

  Public Shared Function GetPostsByBlog(moduleId As Int32, blogID As Int32, displayLocale As String, userId As Int32, pageIndex As Int32, pageSize As Int32, orderBy As String, ByRef totalRecords As Integer) As Dictionary(Of Integer, PostInfo)

   If pageIndex < 0 Then
    pageIndex = 0
    pageSize = Integer.MaxValue
   End If

   Dim res As New Dictionary(Of Integer, PostInfo)
   Using ir As IDataReader = DataProvider.Instance().GetPostsByBlog(blogID, displayLocale, pageIndex, pageSize, orderBy)
    res = DotNetNuke.Common.Utilities.CBO.FillDictionary(Of Integer, PostInfo)("ContentItemID", ir, False)
    ir.NextResult()
    totalRecords = DotNetNuke.Common.Globals.GetTotalRecords(ir)
   End Using
   Return GetPostsWithBlog(res, blogID, moduleId, userId, displayLocale)

  End Function

  Public Shared Function SearchPosts(moduleId As Int32, blogID As Int32, displayLocale As String, searchText As String, searchTitle As Boolean, searchContents As Boolean, published As Integer, limitToLocale As String, endDate As Date, authorUserId As Int32, pageIndex As Int32, pageSize As Int32, orderBy As String, ByRef totalRecords As Integer, userId As Integer, userIsAdmin As Boolean) As Dictionary(Of Integer, PostInfo)

   If pageIndex < 0 Then
    pageIndex = 0
    pageSize = Integer.MaxValue
   End If

   Dim res As New Dictionary(Of Integer, PostInfo)
   Using ir As IDataReader = DataProvider.Instance().SearchPosts(moduleId, blogID, displayLocale, userId, userIsAdmin, searchText, searchTitle, searchContents, published, limitToLocale, endDate, authorUserId, pageIndex, pageSize, orderBy)
    res = DotNetNuke.Common.Utilities.CBO.FillDictionary(Of Integer, PostInfo)("ContentItemID", ir, False)
    ir.NextResult()
    totalRecords = DotNetNuke.Common.Globals.GetTotalRecords(ir)
   End Using
   Return GetPostsWithBlog(res, blogID, moduleId, userId, displayLocale)

  End Function

  Public Shared Function SearchPostsByTerm(moduleId As Int32, blogID As Int32, displayLocale As String, termId As Integer, searchText As String, searchTitle As Boolean, searchContents As Boolean, published As Integer, limitToLocale As String, endDate As Date, authorUserId As Int32, pageIndex As Int32, pageSize As Int32, orderBy As String, ByRef totalRecords As Integer, userId As Integer, userIsAdmin As Boolean) As Dictionary(Of Integer, PostInfo)

   If pageIndex < 0 Then
    pageIndex = 0
    pageSize = Integer.MaxValue
   End If

   Dim res As New Dictionary(Of Integer, PostInfo)
   Using ir As IDataReader = DataProvider.Instance().SearchPostsByTerm(moduleId, blogID, displayLocale, userId, userIsAdmin, termId, searchText, searchTitle, searchContents, published, limitToLocale, endDate, authorUserId, pageIndex, pageSize, orderBy)
    res = DotNetNuke.Common.Utilities.CBO.FillDictionary(Of Integer, PostInfo)("ContentItemID", ir, False)
    ir.NextResult()
    totalRecords = DotNetNuke.Common.Globals.GetTotalRecords(ir)
   End Using
   Return GetPostsWithBlog(res, blogID, moduleId, userId, displayLocale)

  End Function

  Public Shared Function SearchPostsByCategory(moduleId As Int32, blogID As Int32, displayLocale As String, categories As String, searchText As String, searchTitle As Boolean, searchContents As Boolean, published As Integer, limitToLocale As String, endDate As Date, authorUserId As Int32, pageIndex As Int32, pageSize As Int32, orderBy As String, ByRef totalRecords As Integer, userId As Integer, userIsAdmin As Boolean) As Dictionary(Of Integer, PostInfo)

   If pageIndex < 0 Then
    pageIndex = 0
    pageSize = Integer.MaxValue
   End If

   Dim res As New Dictionary(Of Integer, PostInfo)
   Using ir As IDataReader = DataProvider.Instance().SearchPostsByCategory(moduleId, blogID, displayLocale, userId, userIsAdmin, categories, searchText, searchTitle, searchContents, published, limitToLocale, endDate, authorUserId, pageIndex, pageSize, orderBy)
    res = DotNetNuke.Common.Utilities.CBO.FillDictionary(Of Integer, PostInfo)("ContentItemID", ir, False)
    ir.NextResult()
    totalRecords = DotNetNuke.Common.Globals.GetTotalRecords(ir)
   End Using
   Return GetPostsWithBlog(res, blogID, moduleId, userId, displayLocale)

  End Function

  Public Shared Function GetAuthors(moduleId As Integer, blogId As Integer) As List(Of PostAuthor)
   Return DotNetNuke.Common.Utilities.CBO.FillCollection(Of PostAuthor)(DataProvider.Instance.GetAuthors(moduleId, blogId))
  End Function

  Public Shared Function GetPostByLegacyEntryId(entryId As Int32, portalId As Int32, locale As String) As PostInfo
   Return CType(CBO.FillObject(DataProvider.Instance().GetPostByLegacyEntryId(entryId, portalId, locale), GetType(PostInfo)), PostInfo)
  End Function

  Public Shared Function GetPostByLegacyUrl(url As String, portalId As Int32, locale As String) As PostInfo
   Return CType(CBO.FillObject(DataProvider.Instance().GetPostByLegacyUrl(url, portalId, locale), GetType(PostInfo)), PostInfo)
  End Function

  Public Shared Function GetChangedPosts(moduleId As Integer, lastChange As DateTime) As List(Of PostInfo)
   Return DotNetNuke.Common.Utilities.CBO.FillCollection(Of PostInfo)(DotNetNuke.Data.DataProvider.Instance().ExecuteReader("Blog_GetChangedPosts", moduleId, lastChange))
  End Function

#Region " Private Methods "
  Private Shared Function GetPostsWithBlog(selection As Dictionary(Of Integer, PostInfo), blogId As Integer, moduleId As Integer, userId As Integer, displayLocale As String) As Dictionary(Of Integer, PostInfo)

   Dim res As New Dictionary(Of Integer, PostInfo)
   If blogId = -1 Then
    Dim blogs As Dictionary(Of Integer, BlogInfo) = BlogsController.GetBlogsByModule(moduleId, userId, displayLocale)
    For Each e As PostInfo In selection.Values
     If blogs.ContainsKey(e.BlogID) Then
      e.Blog = blogs(e.BlogID)
      res.Add(e.ContentItemId, e)
     End If
    Next
   Else
    Dim blog As BlogInfo = BlogsController.GetBlog(blogId, userId, displayLocale)
    For Each e As PostInfo In selection.Values
     e.Blog = blog
     res.Add(e.ContentItemId, e)
    Next
   End If
   Return res

  End Function
#End Region

 End Class

End Namespace