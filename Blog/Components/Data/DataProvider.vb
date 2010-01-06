'
' DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2010
'' by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
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

Namespace Data

 Public MustInherit Class DataProvider

#Region " Shared/Static Methods "

  ' singleton reference to the instantiated object 
  Private Shared objProvider As DataProvider = Nothing

  ' constructor
  Shared Sub New()
   CreateProvider()
  End Sub

  ' dynamically create provider
  Private Shared Sub CreateProvider()
   objProvider = CType(Framework.Reflection.CreateObject("data", "DotNetNuke.Modules.Blog.Data", ""), DataProvider)
  End Sub

  ' return the provider
  Public Shared Shadows Function Instance() As DataProvider
   Return objProvider
  End Function

#End Region

#Region " Abstract Methods "

#Region "Blog_Blogs Methods"
  Public MustOverride Function GetBlogByUserName(ByVal PortalID As Integer, ByVal UserName As String) As IDataReader
  Public MustOverride Function GetBlogsByUserName(ByVal PortalID As Integer, ByVal UserName As String) As IDataReader
  Public MustOverride Function GetBlogByUserID(ByVal PortalID As Integer, ByVal UserID As Integer) As IDataReader
  Public MustOverride Function GetBlog(ByVal blogID As Integer) As IDataReader
  Public MustOverride Function ListBlogs(ByVal PortalID As Integer, ByVal ParentBlogID As Integer, ByVal ShowNonPublic As Boolean) As IDataReader
  Public MustOverride Function ListBlogsByPortal(ByVal PortalID As Integer, ByVal ShowNonPublic As Boolean) As IDataReader
  Public MustOverride Function ListBlogsRootByPortal(ByVal PortalID As Integer) As IDataReader
  Public MustOverride Function AddBlog(ByVal PortalID As Integer, ByVal ParentBlogID As Integer, ByVal userID As Integer, ByVal title As String, ByVal description As String, ByVal [Public] As Boolean, ByVal allowComments As Boolean, ByVal allowAnonymous As Boolean, ByVal ShowFullName As Boolean, ByVal Culture As String, ByVal DateFormat As String, ByVal TimeZone As Integer, ByVal syndicated As Boolean, ByVal SyndicateIndependant As Boolean, ByVal SyndicationURL As String, ByVal SyndicationEmail As String, ByVal EmailNotification As Boolean, ByVal AllowTrackbacks As Boolean, ByVal AutoTrackback As Boolean, ByVal MustApproveComments As Boolean, ByVal MustApproveAnonymous As Boolean, ByVal MustApproveTrackbacks As Boolean, ByVal UseCaptcha As Boolean, ByVal EnableTwitterIntegration As Boolean, ByVal TwitterUsername As String, ByVal TwitterPassword As String, ByVal TweetTemplate As String) As Integer
  Public MustOverride Sub UpdateBlog(ByVal PortalID As Integer, ByVal blogID As Integer, ByVal ParentBlogID As Integer, ByVal userID As Integer, ByVal title As String, ByVal description As String, ByVal [Public] As Boolean, ByVal allowComments As Boolean, ByVal allowAnonymous As Boolean, ByVal ShowFullName As Boolean, ByVal Culture As String, ByVal DateFormat As String, ByVal TimeZone As Integer, ByVal syndicated As Boolean, ByVal SyndicateIndependant As Boolean, ByVal SyndicationURL As String, ByVal SyndicationEmail As String, ByVal EmailNotification As Boolean, ByVal AllowTrackbacks As Boolean, ByVal AutoTrackback As Boolean, ByVal MustApproveComments As Boolean, ByVal MustApproveAnonymous As Boolean, ByVal MustApproveTrackbacks As Boolean, ByVal UseCaptcha As Boolean, ByVal EnableTwitterIntegration As Boolean, ByVal TwitterUsername As String, ByVal TwitterPassword As String, ByVal TweetTemplate As String)
  Public MustOverride Sub DeleteBlog(ByVal blogID As Integer)
#End Region

#Region "Blog_Entries Methods"
  Public MustOverride Function GetEntry(ByVal EntryID As Integer, ByVal PortalId As Integer) As IDataReader
  Public MustOverride Function ListEntries(ByVal PortalID As Integer, ByVal BlogID As Integer, ByVal BlogDate As Date, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False) As IDataReader
  Public MustOverride Function AddEntry(ByVal blogID As Integer, ByVal title As String, ByVal description As String, ByVal Entry As String, ByVal Published As Boolean, ByVal AllowComments As Boolean, ByVal AddedDate As DateTime, ByVal DisplayCopyright As Boolean, ByVal Copyright As String, ByVal PermaLink As String) As Integer
  Public MustOverride Sub UpdateEntry(ByVal EntryID As Integer, ByVal blogID As Integer, ByVal title As String, ByVal description As String, ByVal Entry As String, ByVal Published As Boolean, ByVal AllowComments As Boolean, ByVal AddedDate As DateTime, ByVal DisplayCopyright As Boolean, ByVal Copyright As String, ByVal PermaLink As String)
  Public MustOverride Sub DeleteEntry(ByVal EntryID As Integer)
  Public MustOverride Function ListEntriesByBlog(ByVal BlogID As Integer, ByVal BlogDate As Date, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False, Optional ByVal RecentEntriesMax As Integer = 10) As IDataReader
  Public MustOverride Function ListAllEntriesByBlog(ByVal BlogID As Integer) As IDataReader
  Public MustOverride Function ListEntriesByPortal(ByVal PortalID As Integer, ByVal BlogDate As Date, ByVal BlogDateType As String, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False, Optional ByVal RecentEntriesMax As Integer = 10) As IDataReader
  Public MustOverride Function ListAllEntriesByPortal(ByVal PortalID As Integer, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False) As IDataReader
  Public MustOverride Function ListAllEntriesByCategory(ByVal PortalID As Integer, ByVal CatID As Integer, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False) As IDataReader
  Public MustOverride Function ListAllEntriesByTag(ByVal PortalID As Integer, ByVal TagID As Integer, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False) As IDataReader
#End Region

#Region "Blog_Comments Methods"
  Public MustOverride Function GetComment(ByVal commentID As Integer) As IDataReader
  Public MustOverride Function ListComments(ByVal EntryID As Integer, ByVal Approved As Boolean) As IDataReader
        Public MustOverride Function ListCommentsByBlog(ByVal BlogID As Integer, ByVal Approved As Boolean, ByVal MaximumComments As Integer) As IDataReader
        Public MustOverride Function ListCommentsByPortal(ByVal PortalID As Integer, ByVal Approved As Boolean, ByVal MaximumComments As Integer) As IDataReader
  Public MustOverride Function AddComment(ByVal EntryID As Integer, ByVal UserID As Integer, ByVal Title As String, ByVal comment As String, ByVal Author As String, ByVal Approved As Boolean, ByVal Website As String, ByVal Email As String) As Integer
  Public MustOverride Sub UpdateComment(ByVal commentID As Integer, ByVal EntryID As Integer, ByVal userID As Integer, ByVal Title As String, ByVal comment As String, ByVal Author As String, ByVal Approved As Boolean, ByVal Website As String, ByVal Email As String)
  Public MustOverride Sub DeleteComment(ByVal commentID As Integer)
  Public MustOverride Sub DeleteAllUnapproved(ByVal EntryID As Integer)
#End Region

#Region "Blog_Tags / Blog_Cats Methods"
  Public MustOverride Function ListTagsByEntry(ByVal EntryID As Integer) As IDataReader
  Public MustOverride Sub AddEntryTag(ByVal EntryID As Integer, ByVal Tag As String, ByVal Slug As String)
  Public MustOverride Sub DeleteEntryTag(ByVal EntryID As Integer, ByVal Tag As String)
  Public MustOverride Function ListTagsAlpha(ByVal PortalID As Integer) As IDataReader
  Public MustOverride Function ListTagsCnt(ByVal PortalID As Integer) As IDataReader
  Public MustOverride Function ListCategories(ByVal PortalID As Integer) As IDataReader
  Public MustOverride Function GetCategory(ByVal CatID As Integer) As IDataReader
  Public MustOverride Sub AddCategory(ByVal Category As String, ByVal ParentID As Integer, ByVal PortalID As Integer)
  Public MustOverride Sub DeleteCategory(ByVal CatID As Integer)
  Public MustOverride Sub UpdateCategory(ByVal catiD As Integer, ByVal Category As String, ByVal ParentID As Integer)
  Public MustOverride Function ListEntryCategories(ByVal EntryID As Integer) As IDataReader
  Public MustOverride Sub DeleteEntryCategories(ByVal EntryID As Integer, ByVal CatID As Integer)
  Public MustOverride Sub AddEntryCategories(ByVal EntryID As Integer, ByVal CatID As Integer)
#End Region


#Region "Archive Methods"
  Public MustOverride Function GetBlogMonths(ByVal PortalID As Integer, ByVal BlogID As Integer) As IDataReader
  Public MustOverride Function GetBlogDaysForMonth(ByVal PortalID As Integer, ByVal BlogID As Integer, ByVal BlogDate As Date) As IDataReader
#End Region

#Region "Search Methods"
  Public MustOverride Function SearchByKeyWordByPortal(ByVal PortalID As Integer, ByVal SearchString As String, ByVal ShowNonPublic As Boolean, ByVal ShowNonPublished As Boolean) As IDataReader
  Public MustOverride Function SearchByKeyWordByBlog(ByVal BlogID As Integer, ByVal SearchString As String, ByVal ShowNonPublic As Boolean, ByVal ShowNonPublished As Boolean) As IDataReader
  Public MustOverride Function SearchByPhraseByPortal(ByVal PortalID As Integer, ByVal SearchString As String, ByVal ShowNonPublic As Boolean, ByVal ShowNonPublished As Boolean) As IDataReader
  Public MustOverride Function SearchByPhraseByBlog(ByVal BlogID As Integer, ByVal SearchString As String, ByVal ShowNonPublic As Boolean, ByVal ShowNonPublished As Boolean) As IDataReader
#End Region

#Region "Settings Methods"
  Public MustOverride Function GetBlogModuleSettings(ByVal PortalID As Integer, ByVal TabID As Integer) As IDataReader
  Public MustOverride Sub UpdateBlogModuleSetting(ByVal PortalID As Integer, ByVal TabID As Integer, ByVal Key As String, ByVal Value As String)
  Public MustOverride Function GetBlogViewEntryModuleID(ByVal tabID As Integer) As Integer
#End Region

#Region "NewBlog upgrade methods"

#Region "Blog methods"
  Public MustOverride Function Upgrade_ListBlogs(ByVal PortalID As Integer, ByVal ParentBlogID As Integer, ByVal ShowNonPublic As Boolean) As IDataReader
  Public MustOverride Sub Upgrade_DeleteBlog(ByVal blogID As Integer)
#End Region

#Region "Blog entries upgrade methods"
  Public MustOverride Function Upgrade_ListEntriesByBlog(ByVal BlogID As Integer, ByVal BlogDate As Date, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False) As IDataReader
  Public MustOverride Sub Upgrade_DeleteEntry(ByVal EntryID As Integer)
#End Region

#Region "Blog comment upgrade methods"
  Public MustOverride Function Upgrade_ListComments(ByVal EntryID As Integer) As IDataReader
  Public MustOverride Sub Upgrade_DeleteComment(ByVal commentID As Integer)
#End Region

#Region "General upgrade methods"
  Public MustOverride Sub Upgrade_UpdateModuleDefId(ByVal ModuleDefID As Integer, ByVal ModuleId As Integer)
  Public MustOverride Function Upgrade_GetBlogModuleSettings() As IDataReader
#End Region

#End Region

#Region "ForumBlog upgrade methods"

#Region "Forum_Groups Methods"
  Public MustOverride Function Upgrade_ListForum_Groups(ByVal PortalID As Integer) As IDataReader
  Public MustOverride Function Upgrade_ListForum_GroupsByGroup(ByVal GroupID As Integer) As IDataReader
#End Region

#Region "Forum_Forums Methods"
  Public MustOverride Function Upgrade_ListForum_Forums(ByVal GroupID As Integer) As IDataReader
#End Region

#Region "Forum_Threads Methods"
  Public MustOverride Function Upgrade_ListForum_Threads(ByVal ForumID As Integer) As IDataReader
#End Region

#Region "Forum_Posts Methods"
  Public MustOverride Function Upgrade_ListForum_Posts(ByVal ThreadID As Integer) As IDataReader
#End Region

#Region "Forum_ThreadRating Methods"
  Public MustOverride Function Upgrade_ListForum_ThreadRating(ByVal ThreadID As Integer) As IDataReader
#End Region

#End Region

#End Region

 End Class

End Namespace
