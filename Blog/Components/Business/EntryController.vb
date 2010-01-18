'
' DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2010
' by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
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
'-------------------------------------------------------------------------

Imports System
Imports DotNetNuke.Modules.Blog.Data
Imports DotNetNuke.Common.Utilities

Namespace Business

 Public Class EntryController

  Public Function GetEntry(ByVal EntryID As Integer, ByVal PortalId As Integer) As EntryInfo
   Return CType(CBO.FillObject(DataProvider.Instance().GetEntry(EntryID, PortalId), GetType(EntryInfo)), EntryInfo)
  End Function

  Public Function ListEntries(ByVal PortalID As Integer, ByVal BlogID As Integer, ByVal BlogDate As Date, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False) As ArrayList
   Return CBO.FillCollection(DataProvider.Instance().ListEntries(PortalID, BlogID, BlogDate, ShowNonPublic, ShowNonPublished), GetType(EntryInfo))
  End Function

  Public Function ListEntriesByBlog(ByVal BlogID As Integer, ByVal BlogDate As Date, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False, Optional ByVal RecentEntriesMax As Integer = 10) As ArrayList
   Return CBO.FillCollection(DataProvider.Instance().ListEntriesByBlog(BlogID, BlogDate, ShowNonPublic, ShowNonPublished, RecentEntriesMax), GetType(EntryInfo))
  End Function

  Public Function ListAllEntriesByBlog(ByVal BlogID As Integer) As ArrayList
   Return CBO.FillCollection(DataProvider.Instance().ListAllEntriesByBlog(BlogID), GetType(EntryInfo))
  End Function

  Public Function ListEntriesByPortal(ByVal PortalID As Integer, ByVal BlogDate As Date, ByVal BlogDateType As String, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False, Optional ByVal RecentEntriesMax As Integer = 10) As ArrayList
   Return CBO.FillCollection(DataProvider.Instance().ListEntriesByPortal(PortalID, BlogDate, BlogDateType, ShowNonPublic, ShowNonPublished, RecentEntriesMax), GetType(EntryInfo))
  End Function

  Public Function ListAllEntriesByPortal(ByVal PortalID As Integer, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False) As ArrayList
   Return CBO.FillCollection(DataProvider.Instance().ListAllEntriesByPortal(PortalID, ShowNonPublic, ShowNonPublished), GetType(EntryInfo))
  End Function

  Public Function ListAllEntriesByCategory(ByVal PortalID As Integer, ByVal CatID As Integer, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False) As ArrayList
   Return CBO.FillCollection(DataProvider.Instance().ListAllEntriesByCategory(PortalID, CatID, ShowNonPublic, ShowNonPublished), GetType(EntryInfo))
  End Function

  Public Function ListAllEntriesByTag(ByVal PortalID As Integer, ByVal TagID As Integer, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False) As ArrayList
   Return CBO.FillCollection(DataProvider.Instance().ListAllEntriesByTag(PortalID, TagID, ShowNonPublic, ShowNonPublished), GetType(EntryInfo))
  End Function
  Public Function AddEntry(ByVal objEntry As EntryInfo) As Integer
   Return CType(DataProvider.Instance().AddEntry(objEntry.BlogID, objEntry.Title, objEntry.Description, objEntry.Entry, objEntry.Published, objEntry.AllowComments, objEntry.AddedDate, objEntry.DisplayCopyright, objEntry.Copyright, objEntry.PermaLink), Integer)
  End Function

  Public Sub UpdateEntry(ByVal objEntry As EntryInfo, Optional ByVal objBlog As BlogInfo = Nothing)
   DataProvider.Instance().UpdateEntry(objEntry.BlogID, objEntry.EntryID, objEntry.Title, objEntry.Description, objEntry.Entry, objEntry.Published, objEntry.AllowComments, objEntry.AddedDate, objEntry.DisplayCopyright, objEntry.Copyright, objEntry.PermaLink)
   'DR-19/04/2009-BLG-9760
   If objBlog IsNot Nothing AndAlso objBlog.EnableTwitterIntegration Then
    Utility.Tweet(objBlog, objEntry)
   End If
  End Sub

  Public Sub DeleteEntry(ByVal EntryID As Integer)
   DataProvider.Instance().DeleteEntry(EntryID)
  End Sub

 End Class

End Namespace
