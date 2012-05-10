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

Imports System

Namespace Providers.Data

    Public MustInherit Class DataProvider

#Region "Shared/Static Methods"

        ' singleton reference to the instantiated object 
        Private Shared objProvider As DataProvider = Nothing

        ' constructor
        Shared Sub New()
            CreateProvider()
        End Sub

        ' dynamically create provider
        Private Shared Sub CreateProvider()
            objProvider = CType(Framework.Reflection.CreateObject("data", "DotNetNuke.Modules.Blog.Providers.Data", ""), DataProvider)
        End Sub

        ' return the provider
        Public Shared Shadows Function Instance() As DataProvider
            Return objProvider
        End Function

#End Region

#Region "Abstract Methods"

#Region "Blog_Blogs Methods"

        Public MustOverride Function GetBlog(ByVal blogID As Integer) As IDataReader
        Public MustOverride Function GetPortalBlogs(ByVal portalId As Integer) As IDataReader
        Public MustOverride Function AddBlog(ByVal PortalID As Integer, ByVal ParentBlogID As Integer, ByVal userID As Integer, ByVal title As String, ByVal description As String, ByVal [Public] As Boolean, ByVal allowComments As Boolean, ByVal allowAnonymous As Boolean, ByVal ShowFullName As Boolean, ByVal syndicated As Boolean, ByVal SyndicateIndependant As Boolean, ByVal SyndicationURL As String, ByVal SyndicationEmail As String, ByVal EmailNotification As Boolean, ByVal AllowTrackbacks As Boolean, ByVal AutoTrackback As Boolean, ByVal MustApproveComments As Boolean, ByVal MustApproveAnonymous As Boolean, ByVal MustApproveTrackbacks As Boolean, ByVal UseCaptcha As Boolean, ByVal EnableGhostWriter As Integer) As Integer
        Public MustOverride Sub UpdateBlog(ByVal PortalID As Integer, ByVal blogID As Integer, ByVal ParentBlogID As Integer, ByVal userID As Integer, ByVal title As String, ByVal description As String, ByVal [Public] As Boolean, ByVal allowComments As Boolean, ByVal allowAnonymous As Boolean, ByVal ShowFullName As Boolean, ByVal syndicated As Boolean, ByVal SyndicateIndependant As Boolean, ByVal SyndicationURL As String, ByVal SyndicationEmail As String, ByVal EmailNotification As Boolean, ByVal AllowTrackbacks As Boolean, ByVal AutoTrackback As Boolean, ByVal MustApproveComments As Boolean, ByVal MustApproveAnonymous As Boolean, ByVal MustApproveTrackbacks As Boolean, ByVal UseCaptcha As Boolean, ByVal EnableGhostWriter As Integer)
        Public MustOverride Sub DeleteBlog(ByVal blogID As Integer, ByVal portalId As Integer)

#End Region

#Region "Blog_Entries Methods"

        Public MustOverride Function GetEntry(ByVal EntryID As Integer, ByVal PortalId As Integer) As IDataReader
        Public MustOverride Function ListEntries(ByVal PortalID As Integer, ByVal BlogID As Integer, ByVal BlogDate As Date, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False) As IDataReader
        Public MustOverride Function AddEntry(ByVal blogID As Integer, ByVal title As String, ByVal description As String, ByVal Entry As String, ByVal Published As Boolean, ByVal AllowComments As Boolean, ByVal AddedDate As DateTime, ByVal DisplayCopyright As Boolean, ByVal Copyright As String, ByVal PermaLink As String) As Integer
        Public MustOverride Sub UpdateEntry(ByVal EntryID As Integer, ByVal blogID As Integer, ByVal title As String, ByVal description As String, ByVal Entry As String, ByVal Published As Boolean, ByVal AllowComments As Boolean, ByVal AddedDate As DateTime, ByVal DisplayCopyright As Boolean, ByVal Copyright As String, ByVal PermaLink As String, ByVal contentItemId As Integer)
        Public MustOverride Sub DeleteEntry(ByVal EntryID As Integer)
        Public MustOverride Function ListEntriesByBlog(ByVal BlogID As Integer, ByVal BlogDate As Date, ByVal PageSize As Integer, ByVal CurrentPage As Integer, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False) As IDataReader
        Public MustOverride Function ListAllEntriesByBlog(ByVal BlogID As Integer) As IDataReader
        Public MustOverride Function ListEntriesByPortal(ByVal PortalID As Integer, ByVal BlogDate As Date, ByVal BlogDateType As String, ByVal PageSize As Integer, ByVal CurrentPage As Integer, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False) As IDataReader
        Public MustOverride Function ListAllEntriesByPortal(ByVal PortalID As Integer, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False) As IDataReader
        Public MustOverride Function GetAllEntriesByTerm(ByVal portalId As Integer, ByVal BlogDate As Date, ByVal termId As Integer, ByVal pageSize As Integer, ByVal currentPage As Integer, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False) As IDataReader

#End Region

#Region "Blog_Comments Methods"

        Public MustOverride Function GetComment(ByVal commentID As Integer) As IDataReader
        Public MustOverride Function ListComments(ByVal EntryID As Integer, ByVal ShowNonApproved As Boolean) As IDataReader
        Public MustOverride Function ListCommentsByBlog(ByVal BlogID As Integer, ByVal ShowNonApproved As Boolean, ByVal MaximumComments As Integer) As IDataReader
        Public MustOverride Function ListCommentsByPortal(ByVal PortalID As Integer, ByVal ShowNonApproved As Boolean, ByVal MaximumComments As Integer) As IDataReader
        Public MustOverride Function AddComment(ByVal EntryID As Integer, ByVal UserID As Integer, ByVal Title As String, ByVal comment As String, ByVal Author As String, ByVal Approved As Boolean, ByVal Website As String, ByVal Email As String, ByVal AddedDate As DateTime) As Integer
        Public MustOverride Sub UpdateComment(ByVal commentID As Integer, ByVal EntryID As Integer, ByVal userID As Integer, ByVal Title As String, ByVal comment As String, ByVal Author As String, ByVal Approved As Boolean, ByVal Website As String, ByVal Email As String, ByVal AddedDate As DateTime)
        Public MustOverride Sub DeleteComment(ByVal commentID As Integer)
        Public MustOverride Sub DeleteAllUnapproved(ByVal EntryID As Integer)

#End Region

#Region "Blog_Tags / Blog_Cats Methods"

        Public MustOverride Function GetEntryTagsForUpgrade(ByVal EntryID As Integer) As IDataReader
        Public MustOverride Function GetEntryCategoriesForUpgrade(ByVal EntryID As Integer) As IDataReader
        Public MustOverride Function GetAllTagsForUpgrade() As IDataReader
        Public MustOverride Function GetAllCategoriesForUpgrade() As IDataReader
        Public MustOverride Function RetrieveTaxonomyRelatedPosts() As IDataReader

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

#Region "Terms"

        Public MustOverride Function GetTermsByContentType(ByVal portalId As Integer, ByVal contentTypeId As Integer, ByVal vocabularyId As Integer) As IDataReader

        Public MustOverride Function GetTermsByContentItem(ByVal contentItemId As Integer, ByVal vocabularyId As Integer) As IDataReader

#End Region

#End Region

    End Class
End Namespace