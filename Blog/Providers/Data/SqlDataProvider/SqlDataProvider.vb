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
Imports System.Data
Imports Microsoft.ApplicationBlocks.Data
Imports DotNetNuke.Common.Utilities

Namespace Data

    Public Class SqlDataProvider

        Inherits DataProvider

#Region "Private Members"

        Private Const ProviderType As String = "data"
        Private Const ModuleQualifier As String = "Blog_"
        Private _providerConfiguration As Framework.Providers.ProviderConfiguration = Framework.Providers.ProviderConfiguration.GetProviderConfiguration(ProviderType)
        Private _connectionString As String
        Private _providerPath As String
        Private _objectQualifier As String
        Private _databaseOwner As String

#End Region

#Region "Constructors"
        Public Sub New()

            Dim objProvider As Framework.Providers.Provider = CType(_providerConfiguration.Providers(_providerConfiguration.DefaultProvider), Framework.Providers.Provider)

            'DR - 01/15/2009
            'BLG-9133 Updated to remove reference to appsettings connection string.
            _connectionString = Config.GetConnectionString()

            If _connectionString = "" Then
                ' Use connection string specified in provider
                _connectionString = objProvider.Attributes("connectionString")
            End If

            _providerPath = objProvider.Attributes("providerPath")

            _objectQualifier = objProvider.Attributes("objectQualifier")
            If _objectQualifier <> "" And _objectQualifier.EndsWith("_") = False Then
                _objectQualifier += "_"
            End If

            _databaseOwner = objProvider.Attributes("databaseOwner")
            If _databaseOwner <> "" And _databaseOwner.EndsWith(".") = False Then
                _databaseOwner += "."
            End If

        End Sub
#End Region

#Region "Properties"
        Public ReadOnly Property ConnectionString() As String
            Get
                Return _connectionString
            End Get
        End Property

        Public ReadOnly Property ProviderPath() As String
            Get
                Return _providerPath
            End Get
        End Property

        Public ReadOnly Property ObjectQualifier() As String
            Get
                Return _objectQualifier
            End Get
        End Property

        Public ReadOnly Property DatabaseOwner() As String
            Get
                Return _databaseOwner
            End Get
        End Property
#End Region

#Region "Private Methods"

        Private Function GetNull(ByVal Field As Object) As Object
            Return Null.GetNull(Field, DBNull.Value)
        End Function

        Private Function GetFullyQualifiedName(name As String) As String
            Return DatabaseOwner + ObjectQualifier + ModuleQualifier + name
        End Function

#End Region

#Region "Blog Methods"

#Region "Blog_Blogs Methods"

        Public Overrides Function GetBlog(ByVal blogID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_GetBlog", blogID), IDataReader)
        End Function

        Public Overrides Function GetBlogByUserName(ByVal PortalID As Integer, ByVal UserName As String) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_GetBlogByUserName", PortalID, UserName), IDataReader)
        End Function

        Public Overrides Function GetBlogsByUserName(ByVal PortalID As Integer, ByVal UserName As String) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_GetBlogsByUserName", PortalID, UserName), IDataReader)
        End Function

        Public Overrides Function GetBlogByUserID(ByVal PortalID As Integer, ByVal UserID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_GetBlogByUserID", PortalID, UserID), IDataReader)
        End Function

        Public Overrides Function ListBlogs(ByVal PortalID As Integer, ByVal ParentBlogID As Integer, ByVal ShowNonPublic As Boolean) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_ListBlogs", PortalID, ParentBlogID, ShowNonPublic), IDataReader)
        End Function

        Public Overrides Function ListBlogsByPortal(ByVal PortalID As Integer, ByVal ShowNonPublic As Boolean) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_ListBlogsByPortal", PortalID, ShowNonPublic), IDataReader)
        End Function

        Public Overrides Function ListBlogsRootByPortal(ByVal PortalID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_ListBlogsRootByPortal", PortalID), IDataReader)
        End Function

        Public Overrides Function AddBlog(ByVal PortalID As Integer, ByVal ParentBlogID As Integer, ByVal userID As Integer, ByVal title As String, ByVal description As String, ByVal [Public] As Boolean, ByVal allowComments As Boolean, ByVal allowAnonymous As Boolean, ByVal ShowFullName As Boolean, ByVal syndicated As Boolean, ByVal SyndicateIndependant As Boolean, ByVal SyndicationURL As String, ByVal SyndicationEmail As String, ByVal EmailNotification As Boolean, ByVal AllowTrackbacks As Boolean, ByVal AutoTrackback As Boolean, ByVal MustApproveComments As Boolean, ByVal MustApproveAnonymous As Boolean, ByVal MustApproveTrackbacks As Boolean, ByVal UseCaptcha As Boolean, ByVal EnableGhostWriter As Boolean) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_Blog_Add", PortalID, ParentBlogID, userID, title, Null.GetNull(description, DBNull.Value), [Public], allowComments, allowAnonymous, ShowFullName, syndicated, SyndicateIndependant, SyndicationURL, SyndicationEmail, EmailNotification, AllowTrackbacks, AutoTrackback, MustApproveComments, MustApproveAnonymous, MustApproveTrackbacks, UseCaptcha, EnableGhostWriter), Integer)
        End Function

        Public Overrides Sub UpdateBlog(ByVal PortalID As Integer, ByVal blogID As Integer, ByVal ParentBlogID As Integer, ByVal userID As Integer, ByVal title As String, ByVal description As String, ByVal [Public] As Boolean, ByVal allowComments As Boolean, ByVal allowAnonymous As Boolean, ByVal ShowFullName As Boolean, ByVal syndicated As Boolean, ByVal SyndicateIndependant As Boolean, ByVal SyndicationURL As String, ByVal SyndicationEmail As String, ByVal EmailNotification As Boolean, ByVal AllowTrackbacks As Boolean, ByVal AutoTrackback As Boolean, ByVal MustApproveComments As Boolean, ByVal MustApproveAnonymous As Boolean, ByVal MustApproveTrackbacks As Boolean, ByVal UseCaptcha As Boolean, ByVal EnableGhostWriter As Boolean)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_Blog_Update", PortalID, blogID, ParentBlogID, userID, title, Null.GetNull(description, DBNull.Value), [Public], allowComments, allowAnonymous, ShowFullName, syndicated, SyndicateIndependant, SyndicationURL, SyndicationEmail, EmailNotification, AllowTrackbacks, AutoTrackback, MustApproveComments, MustApproveAnonymous, MustApproveTrackbacks, UseCaptcha, EnableGhostWriter)
        End Sub

        Public Overrides Sub DeleteBlog(ByVal blogID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_DeleteBlog", blogID)
        End Sub

#End Region

#Region "Blog_Entries Methods"
        ' Entries
        Public Overrides Function GetEntry(ByVal EntryID As Integer, ByVal PortalId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_Entry_Get", EntryID, PortalId), IDataReader)
        End Function

        Public Overrides Function ListEntries(ByVal PortalID As Integer, ByVal BlogID As Integer, ByVal BlogDate As Date, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_ListEntries", PortalID, BlogID, Null.GetNull(BlogDate, DBNull.Value), ShowNonPublic, ShowNonPublished), IDataReader)
        End Function

        Public Overrides Function ListEntriesByBlog(ByVal BlogID As Integer, ByVal BlogDate As Date, ByVal PageSize As Integer, ByVal CurrentPage As Integer, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_Entry_GetByBlog", BlogID, Null.GetNull(BlogDate, DBNull.Value), PageSize, CurrentPage, ShowNonPublic, ShowNonPublished), IDataReader)
        End Function

        Public Overrides Function ListAllEntriesByBlog(ByVal BlogID As Integer) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_ListAllEntriesByBlog", BlogID), IDataReader)
        End Function

        Public Overrides Function ListEntriesByPortal(ByVal PortalID As Integer, ByVal BlogDate As Date, ByVal BlogDateType As String, ByVal PageSize As Integer, ByVal CurrentPage As Integer, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False) As System.Data.IDataReader

            Dim sproc As String = ""
            Select Case BlogDateType
                Case Nothing
                    sproc = "Blog_Entry_GetByPortal"
                Case "day"
                    sproc = "Blog_Entry_GetByDay"
                Case "month"
                    sproc = "Blog_Entry_GetByMonth"
            End Select

            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & sproc, PortalID, Null.GetNull(BlogDate, DBNull.Value), PageSize, CurrentPage, ShowNonPublic, ShowNonPublished), IDataReader)
        End Function


        Public Overrides Function ListAllEntriesByPortal(ByVal PortalID As Integer, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_ListAllEntriesByPortal", PortalID, ShowNonPublic, ShowNonPublished), IDataReader)
        End Function

        Public Overrides Function GetAllEntriesByTerm(ByVal portalId As Integer, ByVal BlogDate As Date, ByVal termId As Integer, ByVal pageSize As Integer, ByVal currentPage As Integer, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_Entry_GetByTerm", portalId, BlogDate, termId, pageSize, currentPage, ShowNonPublic, ShowNonPublished), IDataReader)
        End Function

        Public Overrides Function AddEntry(ByVal blogID As Integer, ByVal title As String, ByVal description As String, ByVal Entry As String, ByVal Published As Boolean, ByVal AllowComments As Boolean, ByVal AddedDate As DateTime, ByVal DisplayCopyright As Boolean, ByVal Copyright As String, ByVal PermaLink As String) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_AddEntry", blogID, title, Null.GetNull(description, DBNull.Value), Null.GetNull(Entry, DBNull.Value), Published, AllowComments, AddedDate, DisplayCopyright, Null.GetNull(Copyright, DBNull.Value), Null.GetNull(PermaLink, DBNull.Value)), Integer)
        End Function

        Public Overrides Sub UpdateEntry(ByVal BlogID As Integer, ByVal EntryID As Integer, ByVal Title As String, ByVal Description As String, ByVal Entry As String, ByVal Published As Boolean, ByVal AllowComments As Boolean, ByVal AddedDate As DateTime, ByVal DisplayCopyright As Boolean, ByVal Copyright As String, ByVal PermaLink As String, ByVal contentItemId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_UpdateEntry", BlogID, EntryID, Title, Null.GetNull(Description, DBNull.Value), Entry, Published, AllowComments, AddedDate, DisplayCopyright, Null.GetNull(Copyright, DBNull.Value), Null.GetNull(PermaLink, DBNull.Value), Null.GetNull(contentItemId, DBNull.Value))
        End Sub

        Public Overrides Sub DeleteEntry(ByVal EntryID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_DeleteEntry", EntryID)
        End Sub

#End Region

#Region "Blog_Comments Methods"
        Public Overrides Function GetComment(ByVal commentID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_GetComment", commentID), IDataReader)
        End Function

        Public Overrides Function ListComments(ByVal EntryID As Integer, ByVal ShowNonApproved As Boolean) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_ListComments", EntryID, ShowNonApproved), IDataReader)
        End Function

        Public Overrides Function ListCommentsByBlog(ByVal BlogID As Integer, ByVal ShowNonApproved As Boolean, ByVal MaximumComments As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_ListCommentsByBlog", BlogID, ShowNonApproved, MaximumComments), IDataReader)
        End Function

        Public Overrides Function ListCommentsByPortal(ByVal PortalID As Integer, ByVal ShowNonApproved As Boolean, ByVal MaximumComments As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_ListCommentsByPortal", PortalID, ShowNonApproved, MaximumComments), IDataReader)
        End Function

        Public Overrides Function AddComment(ByVal EntryID As Integer, ByVal userID As Integer, ByVal Title As String, ByVal comment As String, ByVal Author As String, ByVal Approved As Boolean, ByVal Website As String, ByVal Email As String, ByVal AddedDate As DateTime) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_AddComment", EntryID, Null.GetNull(userID, DBNull.Value), Title, comment, Null.GetNull(Author, DBNull.Value), Approved, Null.GetNull(Website, DBNull.Value), Null.GetNull(Email, DBNull.Value), Null.GetNull(AddedDate, DBNull.Value)), Integer)
        End Function

        Public Overrides Sub UpdateComment(ByVal commentID As Integer, ByVal EntryID As Integer, ByVal userID As Integer, ByVal Title As String, ByVal comment As String, ByVal Author As String, ByVal Approved As Boolean, ByVal Website As String, ByVal Email As String, ByVal AddedDate As DateTime)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_UpdateComment", commentID, EntryID, Null.GetNull(userID, DBNull.Value), Title, comment, Null.GetNull(Author, DBNull.Value), Approved, Null.GetNull(Website, DBNull.Value), Null.GetNull(Email, DBNull.Value), Null.GetNull(AddedDate, DBNull.Value))
        End Sub

        Public Overrides Sub DeleteComment(ByVal commentID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_DeleteComment", commentID)
        End Sub

        Public Overrides Sub DeleteAllUnapproved(ByVal EntryID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_DelUnAppCommByEntry", EntryID)
        End Sub

#End Region

#Region "Blog_Tags / Blog_Cats Methods"

        Public Overrides Function GetTag(ByVal TagID As Integer) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_GetTag", TagID), IDataReader)
        End Function

        Public Overrides Function ListTagsByEntry(ByVal EntryID As Integer) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_ListTagsByEntry", EntryID), IDataReader)
        End Function

        Public Overrides Sub AddEntryTag(ByVal EntryID As Integer, ByVal Tag As String, ByVal Slug As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_AddEntryTag", EntryID, Tag, Slug)
        End Sub

        Public Overrides Sub DeleteEntryTag(ByVal EntryID As Integer, ByVal Tag As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_DeleteEntryTag", EntryID, Tag)
        End Sub

        Public Overrides Function ListTagsAlpha(ByVal PortalID As Integer) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_ListTagsAlpha", PortalID), IDataReader)
        End Function

        Public Overrides Function ListTagsCnt(ByVal PortalID As Integer) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_ListTagsCnt", PortalID), IDataReader)
        End Function

        Public Overrides Function ListCategories(ByVal PortalID As Integer) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_ListCategories", PortalID), IDataReader)
        End Function

        Public Overrides Function GetCategory(ByVal CatID As Integer) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_GetCategory", CatID), IDataReader)
        End Function

        Public Overrides Function AddCategory(ByVal Category As String, ByVal ParentId As Int32, ByVal PortalId As Int32, ByVal Slug As String) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_AddCategory", Category, GetNull(ParentId), GetNull(PortalId), GetNull(Slug)), Integer)
        End Function

        Public Overrides Sub UpdateCategory(ByVal CatId As Int32, ByVal Category As String, ByVal ParentId As Int32, ByVal Slug As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_UpdateCategory", CatId, Category, ParentId, GetNull(Slug))
        End Sub

        Public Overrides Sub DeleteCategory(ByVal CatID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_DeleteCategory", CatID)
        End Sub

        Public Overrides Function ListEntryCategories(ByVal EntryID As Integer) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_ListEntryCategories", EntryID), IDataReader)
        End Function

        Public Overrides Sub DeleteEntryCategories(ByVal EntryID As Integer, ByVal CatID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_DeleteEntryCategories", EntryID, CatID)
        End Sub

        Public Overrides Sub AddEntryCategories(ByVal EntryID As Integer, ByVal CatID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_AddEntryCategories", EntryID, CatID)
        End Sub
#End Region

#Region "Archive Methods"
        Public Overrides Function GetBlogMonths(ByVal PortalID As Integer, ByVal BlogID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_GetBlogMonths", PortalID, BlogID), IDataReader)
        End Function

        Public Overrides Function GetBlogDaysForMonth(ByVal PortalID As Integer, ByVal BlogID As Integer, ByVal BlogDate As Date) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_GetBlogDaysForMonth", PortalID, BlogID, BlogDate), IDataReader)
        End Function
#End Region

#Region "Settings Methods"
        Public Overrides Function GetBlogModuleSettings(ByVal PortalID As Integer, ByVal TabID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_GetSettings", PortalID, TabID), IDataReader)
        End Function

        Public Overrides Sub UpdateBlogModuleSetting(ByVal PortalID As Integer, ByVal TabID As Integer, ByVal Key As String, ByVal Value As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_UpdateSetting", PortalID, TabID, Key, Value)
        End Sub

        Public Overrides Function GetBlogViewEntryModuleID(ByVal TabID As Integer) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_GetBlogViewEntryModuleID", TabID), Integer)
        End Function
#End Region

#Region "Search Methods"
        Public Overrides Function SearchByKeyWordByPortal(ByVal PortalID As Integer, ByVal SearchString As String, ByVal ShowNonPublic As Boolean, ByVal ShowNonPublished As Boolean) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_SearchByKeywordByPortal", PortalID, SearchString, ShowNonPublic, ShowNonPublished), IDataReader)
        End Function

        Public Overrides Function SearchByKeyWordByBlog(ByVal BlogID As Integer, ByVal SearchString As String, ByVal ShowNonPublic As Boolean, ByVal ShowNonPublished As Boolean) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_SearchByKeywordByBlog", BlogID, SearchString, ShowNonPublic, ShowNonPublished), IDataReader)
        End Function

        Public Overrides Function SearchByPhraseByPortal(ByVal PortalID As Integer, ByVal SearchString As String, ByVal ShowNonPublic As Boolean, ByVal ShowNonPublished As Boolean) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_SearchByPhraseByPortal", PortalID, SearchString, ShowNonPublic, ShowNonPublished), IDataReader)
        End Function

        Public Overrides Function SearchByPhraseByBlog(ByVal BlogID As Integer, ByVal SearchString As String, ByVal ShowNonPublic As Boolean, ByVal ShowNonPublished As Boolean) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_SearchByPhraseByBlog", BlogID, SearchString, ShowNonPublic, ShowNonPublished), IDataReader)
        End Function
#End Region

#End Region

#Region "NewBlog upgrade methods"

#Region "Blog methods"
        Public Overrides Function Upgrade_ListBlogs(ByVal PortalID As Integer, ByVal ParentBlogID As Integer, ByVal ShowNonPublic As Boolean) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_Upgrade_ListBlogs", PortalID, ParentBlogID, ShowNonPublic), IDataReader)
        End Function

        Public Overrides Sub Upgrade_DeleteBlog(ByVal blogID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_Upgrade_DeleteBlog", blogID)
        End Sub
#End Region

#Region "Blog entries upgrade methods"
        Public Overrides Function Upgrade_ListEntriesByBlog(ByVal BlogID As Integer, ByVal BlogDate As Date, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_Upgrade_ListEntriesByBlog", BlogID, Null.GetNull(BlogDate, DBNull.Value), ShowNonPublic, ShowNonPublished), IDataReader)
        End Function

        Public Overrides Sub Upgrade_DeleteEntry(ByVal EntryID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_Upgrade_DeleteEntry", EntryID)
        End Sub
#End Region

#Region "Blog comment upgrade methods"
        Public Overrides Function Upgrade_ListComments(ByVal EntryID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_Upgrade_ListComments", EntryID), IDataReader)
        End Function

        Public Overrides Sub Upgrade_DeleteComment(ByVal commentID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_Upgrade_DeleteComment", commentID)
        End Sub
#End Region

#Region "General upgrade methods"
        Public Overrides Sub Upgrade_UpdateModuleDefId(ByVal ModuleDefID As Integer, ByVal ModuleId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_Upgrade_UpdateModuleDefId", ModuleDefID, ModuleId)
        End Sub

        Public Overrides Function Upgrade_GetBlogModuleSettings() As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_Upgrade_GetSettings"), IDataReader)
        End Function
#End Region

#End Region

#Region "ForumBlog upgrade methods"

#Region "Forum_Groups Methods"

        Public Overrides Function Upgrade_ListForum_Groups(ByVal portalID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("Upgrade_ForumGroupsList"), GetNull(portalID)), IDataReader)
        End Function

        Public Overrides Function Upgrade_ListForum_GroupsByGroup(ByVal groupID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("Upgrade_ForumGroupsByGroup"), GetNull(groupID)), IDataReader)
        End Function

#End Region

#Region "Forum_Forums Methods"
        Public Overrides Function Upgrade_ListForum_Forums(ByVal GroupId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_Upgrade_ForumForumsList", GetNull(GroupId)), IDataReader)
        End Function
#End Region

#Region "Forum_Threads Methods"
        Public Overrides Function Upgrade_ListForum_Threads(ByVal ForumID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_Upgrade_ForumThreadsList", GetNull(ForumID)), IDataReader)
        End Function
#End Region

#Region "Forum_Posts Methods"
        Public Overrides Function Upgrade_ListForum_Posts(ByVal ThreadID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_Upgrade_ForumPostsList", ThreadID), IDataReader)
        End Function
#End Region

#Region "Forum_ThreadRating Methods"
        Public Overrides Function Upgrade_ListForum_ThreadRating(ByVal ThreadID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "Blog_Upgrade_ForumThreadRatingList", ThreadID), IDataReader)
        End Function
#End Region

#End Region

#Region "Terms"

        Public Overrides Function GetTermsByContentType(portalId As Integer, contentTypeId As Integer, vocabularyId As Integer) As IDataReader
            Return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("Term_GetByContentType"), portalId, contentTypeId, vocabularyId)
        End Function

        Public Overrides Function GetTermsByContentItem(contentItemId As Integer, vocabularyId As Integer) As IDataReader
            Return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("Term_GetByContentItem"), contentItemId, vocabularyId)
        End Function

#End Region

    End Class

End Namespace
