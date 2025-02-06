using System;
using System.Data;
// 
// DNN Connect - http://dnn-connect.org
// Copyright (c) 2015
// by DNN Connect
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
// 

using Microsoft.ApplicationBlocks.Data;

namespace DotNetNuke.Modules.Blog.Core.Data
{

  public partial class SqlDataProvider
  {

    public override int AddCommentKarma(int commentId, int userId, int karma)
    {
      return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "AddCommentKarma", commentId, userId, karma));
    }

    public override void AddPostView(int contentItemId)
    {
      SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "AddPostView", contentItemId);
    }

    public override void ApproveComment(int commentId)
    {
      SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "ApproveComment", commentId);
    }

    public override void DeleteBlogPermissions(int blogId)
    {
      SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "DeleteBlogPermissions", blogId);
    }

    public override IDataReader GetAuthors(int moduleId, int blogID)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetAuthors", moduleId, blogID);
    }

    public override IDataReader GetBlog(int blogId, int userId, string locale)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetBlog", blogId, userId, locale);
    }

    public override IDataReader GetBlogCalendar(int moduleId, int blogId, string locale)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetBlogCalendar", moduleId, blogId, GetNull(locale));
    }

    public override IDataReader GetBlogLocalizations(int blogId)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetBlogLocalizations", blogId);
    }

    public override IDataReader GetBlogPermissionsByBlog(int blogId)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetBlogPermissionsByBlog", blogId);
    }

    public override IDataReader GetBlogsByModule(int moduleId, int userId, string locale)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetBlogsByModule", moduleId, userId, locale);
    }

    public override IDataReader GetBlogsByPortal(int portalId, int userId, string locale)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetBlogsByPortal", portalId, userId, locale);
    }

    public override IDataReader GetComment(int commentID, int userID)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetComment", commentID, userID);
    }

    public override IDataReader GetCommentsByContentItem(int contentItemId, bool includeNonApproved, int userID)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetCommentsByContentItem", contentItemId, includeNonApproved, userID);
    }

    public override IDataReader GetCommentsByModuleId(int moduleId, int userID, int pageIndex, int pageSize, string orderBy)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetCommentsByModuleId", moduleId, userID, pageIndex, pageSize, orderBy);
    }

    public override IDataReader GetPostByLegacyEntryId(int entryId, int portalId, string locale)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetPostByLegacyEntryId", entryId, portalId, locale);
    }

    public override IDataReader GetPostByLegacyUrl(string url, int portalId, string locale)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetPostByLegacyUrl", url, portalId, locale);
    }

    public override IDataReader GetPostLocalizations(int contentItemId)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetPostLocalizations", contentItemId);
    }

    public override IDataReader GetPosts(int moduleId, int blogID, string displayLocale, int userId, bool userIsAdmin, int published, string limitToLocale, DateTime endDate, int authorUserId, bool onlyActionable, int pageIndex, int pageSize, string orderBy)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetPosts", moduleId, blogID, displayLocale, userId, userIsAdmin, published, GetNull(limitToLocale), GetNull(endDate), authorUserId, onlyActionable, pageIndex, pageSize, orderBy);
    }

    public override IDataReader GetPostsByTerm(int moduleId, int blogID, string displayLocale, int userId, bool userIsAdmin, int termID, int published, string limitToLocale, DateTime endDate, int authorUserId, int pageIndex, int pageSize, string orderBy)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetPostsByTerm", moduleId, blogID, displayLocale, userId, userIsAdmin, termID, published, GetNull(limitToLocale), GetNull(endDate), authorUserId, pageIndex, pageSize, orderBy);
    }

    public override IDataReader GetPostsByCategory(int moduleId, int blogID, string displayLocale, int userId, bool userIsAdmin, string categories, int published, string limitToLocale, DateTime endDate, int authorUserId, int pageIndex, int pageSize, string orderBy)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetPostsByCategory", moduleId, blogID, displayLocale, userId, userIsAdmin, categories, published, GetNull(limitToLocale), GetNull(endDate), authorUserId, pageIndex, pageSize, orderBy);
    }

    public override IDataReader GetTerm(int termId, int moduleId, string locale)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetTerm", termId, moduleId, locale);
    }

    public override IDataReader GetTermLocalizations(int termId)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetTermLocalizations", termId);
    }

    public override IDataReader GetTermsByModule(int moduleId, string locale)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetTermsByModule", moduleId, locale);
    }

    public override IDataReader GetTermsByPost(int contentItemId, int moduleId, string locale)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetTermsByPost", contentItemId, moduleId, locale);
    }

    public override IDataReader GetTermsByVocabulary(int moduleId, int vocabularyId, string locale)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetTermsByVocabulary", moduleId, vocabularyId, locale);
    }

    public override IDataReader GetUserPermissionsByModule(int moduleID, int userId)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetUserPermissionsByModule", moduleID, userId);
    }

    public override IDataReader GetUsersByBlogPermission(int portalId, int blogId, int permissionId)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetUsersByBlogPermission", portalId, blogId, permissionId);
    }

    public override IDataReader SearchPosts(int moduleId, int blogID, string displayLocale, int userId, bool userIsAdmin, string searchText, bool searchTitle, bool searchContents, int published, string limitToLocale, DateTime endDate, int authorUserId, int pageIndex, int pageSize, string orderBy)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "SearchPosts", moduleId, blogID, displayLocale, userId, userIsAdmin, searchText, searchTitle, searchContents, published, GetNull(limitToLocale), GetNull(endDate), authorUserId, pageIndex, pageSize, orderBy);
    }

    public override IDataReader SearchPostsByTerm(int moduleId, int blogID, string displayLocale, int userId, bool userIsAdmin, int termID, string searchText, bool searchTitle, bool searchContents, int published, string limitToLocale, DateTime endDate, int authorUserId, int pageIndex, int pageSize, string orderBy)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "SearchPostsByTerm", moduleId, blogID, displayLocale, userId, userIsAdmin, termID, searchText, searchTitle, searchContents, published, GetNull(limitToLocale), GetNull(endDate), authorUserId, pageIndex, pageSize, orderBy);
    }

    public override IDataReader SearchPostsByCategory(int moduleId, int blogID, string displayLocale, int userId, bool userIsAdmin, string categories, string searchText, bool searchTitle, bool searchContents, int published, string limitToLocale, DateTime endDate, int authorUserId, int pageIndex, int pageSize, string orderBy)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "SearchPostsByCategory", moduleId, blogID, displayLocale, userId, userIsAdmin, categories, searchText, searchTitle, searchContents, published, GetNull(limitToLocale), GetNull(endDate), authorUserId, pageIndex, pageSize, orderBy);
    }


    public override void SetBlogLocalization(int blogID, string locale, string title, string description)
    {
      SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "SetBlogLocalization", blogID, locale, title, description);
    }

    public override void SetPostLocalization(int postID, string locale, string title, string summary, string content, int updatedByUser)
    {
      SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "SetPostLocalization", postID, locale, title, summary, content, updatedByUser);
    }

    public override int SetTerm(int termID, int vocabularyID, int parentTermID, int viewOrder, string name, string description, int createdByUserID)
    {
      return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "SetTerm", termID, vocabularyID, parentTermID, viewOrder, name, description, createdByUserID));
    }

    public override void SetTermLocalization(int termID, string locale, string name, string description)
    {
      SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "SetTermLocalization", termID, locale, name, description);
    }

    public override void UpdateModuleWiring(int portalId, int oldModuleId, int newModuleId)
    {
      SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "UpdateModuleWiring", portalId, oldModuleId, newModuleId);
    }

  }

}