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
Imports DotNetNuke.Modules.Blog.Providers.Data
Imports DotNetNuke.Entities.Content
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Modules.Blog.Components.Integration
Imports DotNetNuke.Modules.Blog.Components.Entities

Namespace Components.Controllers


 Public Class EntryController

  Public Function GetEntry(ByVal EntryID As Integer, ByVal PortalId As Integer) As EntryInfo
   Return CType(CBO.FillObject(DataProvider.Instance().GetEntry(EntryID, PortalId), GetType(EntryInfo)), EntryInfo)
  End Function

  Public Function GetEntries(ByVal PortalID As Integer, ByVal BlogID As Integer, ByVal BlogDate As Date, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False) As List(Of EntryInfo)
   Return CBO.FillCollection(Of EntryInfo)(DataProvider.Instance().GetEntries(PortalID, BlogID, BlogDate, ShowNonPublic, ShowNonPublished))
  End Function

  Public Function GetEntriesByBlog(ByVal BlogID As Integer, ByVal BlogDate As Date, ByVal PageSize As Integer, ByVal CurrentPage As Integer, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False) As List(Of EntryInfo)
   Return CBO.FillCollection(Of EntryInfo)(DataProvider.Instance().GetEntriesByBlog(BlogID, BlogDate, PageSize, CurrentPage, ShowNonPublic, ShowNonPublished))
  End Function

  Public Function GetAllEntriesByBlog(ByVal BlogID As Integer) As List(Of EntryInfo)
   Return CBO.FillCollection(Of EntryInfo)(DataProvider.Instance().GetAllEntriesByBlog(BlogID))
  End Function

  Public Function GetEntriesByPortal(ByVal PortalID As Integer, ByVal BlogDate As Date, ByVal BlogDateType As String, ByVal PageSize As Integer, ByVal CurrentPage As Integer, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False) As List(Of EntryInfo)
   Return CBO.FillCollection(Of EntryInfo)(DataProvider.Instance().GetEntriesByPortal(PortalID, BlogDate, BlogDateType, PageSize, CurrentPage, ShowNonPublic, ShowNonPublished))
  End Function

  Public Function GetAllEntriesByPortal(ByVal PortalID As Integer, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False) As List(Of EntryInfo)
   Return CBO.FillCollection(Of EntryInfo)(DataProvider.Instance().GetAllEntriesByPortal(PortalID, ShowNonPublic, ShowNonPublished))
  End Function

  Public Function GetEntriesByTerm(ByVal portalId As Integer, ByVal BlogDate As Date, ByVal termId As Integer, ByVal pageSize As Integer, ByVal currentPage As Integer, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False) As List(Of EntryInfo)
   Return CBO.FillCollection(Of EntryInfo)(DataProvider.Instance().GetEntriesByTerm(portalId, BlogDate, termId, pageSize, currentPage, ShowNonPublic, ShowNonPublished))
  End Function

  Public Function AddEntry(ByVal objEntry As EntryInfo, ByVal tabId As Integer) As EntryInfo
   objEntry.EntryID = CType(DataProvider.Instance().AddEntry(objEntry.BlogID, objEntry.Title, objEntry.Description, objEntry.Entry, objEntry.Published, objEntry.AllowComments, objEntry.AddedDate, objEntry.DisplayCopyright, objEntry.Copyright, objEntry.PermaLink, objEntry.CreatedUserId), Integer)

   objEntry.ContentItemId = CompleteEntryCreation(objEntry, tabId)

   Return objEntry
  End Function

  Public Sub UpdateEntry(ByVal objEntry As EntryInfo, ByVal tabId As Integer, ByVal portalId As Integer)
   DataProvider.Instance().UpdateEntry(objEntry.BlogID, objEntry.EntryID, objEntry.Title, objEntry.Description, objEntry.Entry, objEntry.Published, objEntry.AllowComments, objEntry.AddedDate, objEntry.DisplayCopyright, objEntry.Copyright, objEntry.PermaLink, objEntry.ContentItemId)

   CompleteEntryUpdate(objEntry, tabId, portalId)
  End Sub

  Public Sub UpdateEntryViewCount(ByVal EntryID As Integer)
   DataProvider.Instance().UpdateEntryViewCount(EntryID)
  End Sub

  Public Sub DeleteEntry(ByVal entryId As Integer, ByVal contentItemId As Integer, ByVal blogId As Integer, ByVal portalId As Integer)
   DataProvider.Instance().DeleteEntry(entryId)

   CompleteEntryDelete(contentItemId, blogId, entryId, portalId)
  End Sub

#Region "Private Methods"

  ''' <summary>
  ''' This completes the things necessary for creating a content item in the data store.
  ''' </summary>
  ''' <param name="objEntry"></param>
  ''' <param name="tabId"></param>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Private Shared Function CompleteEntryCreation(ByVal objEntry As EntryInfo, ByVal tabId As Integer) As Integer
   Dim cntTaxonomy As New Content()
   Dim objContentItem As ContentItem = cntTaxonomy.CreateContentItem(objEntry, tabId)

   Return objContentItem.ContentItemId
  End Function

  ''' <summary>
  ''' Handles any content item/taxonomy updates, then deals w/ cache clearing (if applicable)
  ''' </summary>
  ''' <param name="objEntry"></param>
  ''' <param name="tabId"></param>
  ''' <remarks></remarks>
  Private Shared Sub CompleteEntryUpdate(ByVal objEntry As EntryInfo, ByVal tabId As Integer, ByVal portalId As Integer)
   Dim cntTaxonomy As New Content()
   cntTaxonomy.UpdateContentItem(objEntry, tabId, portalId)
  End Sub

  Private Shared Sub CompleteEntryDelete(ByVal contentItemId As Integer, ByVal blogId As Integer, ByVal entryId As Integer, ByVal portalId As Integer)
   Content.DeleteContentItem(contentItemId)
   'TODO: Remove from Journal?
   Dim cntJournal As New Integration.Journal()
   cntJournal.RemoveBlogEntryFromJournal(blogId, entryId, portalId)
  End Sub

#Region "5.0 Taxonomy Migration"

  Public Function RetrieveTaxonomyRelatedPosts() As List(Of EntryInfo)
   Return CBO.FillCollection(Of EntryInfo)(DataProvider.Instance().RetrieveTaxonomyRelatedPosts())
  End Function

#End Region

#End Region

 End Class
End Namespace