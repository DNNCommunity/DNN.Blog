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

Imports DotNetNuke.Modules.Blog.Data
Imports DotNetNuke.Entities.Content
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Modules.Blog.Integration
Imports DotNetNuke.Modules.Blog.Entities
Imports DotNetNuke.Modules.Blog.Entities.Blogs

Namespace Entities.Entries

 Partial Public Class EntriesController

  Public Shared Sub PublishEntry(entry As EntryInfo, publish As Boolean, publishedByUser As Integer)
   If entry.Published = publish Then Exit Sub
   entry.Published = publish
   UpdateEntry(entry, publishedByUser)
   If publish Then
    Dim blog As BlogInfo = BlogsController.GetBlog(entry.BlogID, publishedByUser)
    Dim journalUrl As String = entry.PermaLink(DotNetNuke.Entities.Portals.PortalSettings.Current)
    Dim journalUserId As Integer = publishedByUser
    If Blog.PublishAsOwner Then journalUserId = Blog.OwnerUserId
    JournalController.AddBlogEntryToJournal(entry, DotNetNuke.Entities.Portals.PortalSettings.Current.PortalId, DotNetNuke.Entities.Portals.PortalSettings.Current.ActiveTab.TabID, journalUserId, journalUrl)
    NotificationController.RemoveEntryPendingNotification(blog.ModuleID, blog.BlogID, entry.ContentItemId)
   End If
  End Sub

  Public Shared Sub DeleteEntry(entry As EntryInfo)
   DataProvider.Instance().DeleteEntry(entry.ContentItemId)
   Dim blog As BlogInfo = BlogsController.GetBlog(entry.BlogID, -1)
   NotificationController.RemoveEntryPendingNotification(blog.ModuleID, blog.BlogID, entry.ContentItemId)
  End Sub

  Public Shared Sub DeleteEntry(contentItemId As Integer, blogId As Integer, portalId As Integer, vocabularyId As Integer)
   DataProvider.Instance().DeleteEntry(contentItemId)
   'CompleteEntryDelete(contentItemId, blogId, entryId, portalId, vocabularyId)
  End Sub

  Public Shared Function GetEntries(moduleId As Int32, blogID As Int32, published As Integer, endDate As Date, authorUserId As Int32, pageIndex As Int32, pageSize As Int32, orderBy As String, ByRef totalRecords As Integer, userId As Integer) As Dictionary(Of Integer, EntryInfo)

   If pageIndex < 0 Then
    pageIndex = 0
    pageSize = Integer.MaxValue
   End If

   Dim res As New Dictionary(Of Integer, EntryInfo)
   Using ir As IDataReader = DataProvider.Instance().GetEntries(moduleId, blogID, published, endDate, authorUserId, pageIndex, pageSize, orderBy)
    res = DotNetNuke.Common.Utilities.CBO.FillDictionary(Of Integer, EntryInfo)("ContentItemID", ir, False)
    If blogID = -1 Then
     Dim blogs As Dictionary(Of Integer, BlogInfo) = BlogsController.GetBlogsByModule(moduleId, userId)
     For Each e As EntryInfo In res.Values
      e.Blog = blogs(e.BlogID)
     Next
    Else
     Dim blog As BlogInfo = BlogsController.GetBlog(blogID, userId)
     For Each e As EntryInfo In res.Values
      e.Blog = blog
     Next
    End If
    ir.NextResult()
    totalRecords = DotNetNuke.Common.Globals.GetTotalRecords(ir)
   End Using
   Return res

  End Function


#Region "Private Methods"
  ''' <summary>
  ''' Handles any content item/taxonomy updates, then deals w/ cache clearing (if applicable)
  ''' </summary>
  ''' <param name="objEntry"></param>
  ''' <param name="tabId"></param>
  ''' <remarks></remarks>
  Private Shared Sub CompleteEntryUpdate(objEntry As EntryInfo, tabId As Integer, portalId As Integer, vocabularyId As Integer)
   ' Clear Cache
   Dim tagsKey As String = Common.Constants.TermsKey + "1"
   DataCache.RemoveCache(tagsKey)
   Dim contentItemKey As String = Common.Constants.ContentItemsKey + objEntry.ContentItemId.ToString() + "-" + "1"
   DataCache.RemoveCache(contentItemKey)
   Dim contentItemCatKey As String = Common.Constants.ContentItemsKey + objEntry.ContentItemId.ToString() + "-" + vocabularyId.ToString()
   DataCache.RemoveCache(contentItemCatKey)
   Dim categoriesKey As String = Common.Constants.TermsKey + vocabularyId.ToString()
   DataCache.RemoveCache(categoriesKey)
   Dim portalKey As String = Common.Constants.PortalBlogsCacheKey & CStr(portalId)
   DataCache.RemoveCache(portalKey)
  End Sub

  Private Shared Sub CompleteEntryDelete(contentItemId As Integer, blogId As Integer, entryId As Integer, portalId As Integer, vocabularyId As Integer)

   JournalController.RemoveBlogEntryFromJournal(blogId, entryId, portalId)

   ' Clear Cache
   Dim tagsKey As String = Common.Constants.TermsKey + "1"
   DataCache.RemoveCache(tagsKey)
   Dim contentItemKey As String = Common.Constants.ContentItemsKey + contentItemId.ToString() + "-" + "1"
   DataCache.RemoveCache(contentItemKey)
   Dim contentItemCatKey As String = Common.Constants.ContentItemsKey + contentItemId.ToString() + "-" + vocabularyId.ToString()
   DataCache.RemoveCache(contentItemCatKey)
   Dim categoriesKey As String = Common.Constants.TermsKey + vocabularyId.ToString()
   DataCache.RemoveCache(categoriesKey)
   Dim portalKey As String = Common.Constants.PortalBlogsCacheKey & CStr(portalId)
   DataCache.RemoveCache(portalKey)
  End Sub
#End Region

 End Class

End Namespace