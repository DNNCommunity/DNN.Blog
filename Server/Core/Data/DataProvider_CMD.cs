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


namespace DotNetNuke.Modules.Blog.Data
{

  public abstract partial class DataProvider
  {

    public abstract void AddPostView(int contentItemId);
    public abstract void ApproveComment(int commentId);
    public abstract int AddCommentKarma(int commentId, int userId, int karma);
    public abstract void DeleteBlogPermissions(int blogId);
    public abstract IDataReader GetAuthors(int moduleId, int blogID);
    public abstract IDataReader GetBlog(int blogId, int userId, string locale);
    public abstract IDataReader GetBlogCalendar(int moduleId, int blogId, string locale);
    public abstract IDataReader GetBlogLocalizations(int blogId);
    public abstract IDataReader GetBlogPermissionsByBlog(int blogId);
    public abstract IDataReader GetBlogsByModule(int moduleId, int userId, string locale);
    public abstract IDataReader GetBlogsByPortal(int portalId, int userId, string locale);
    public abstract IDataReader GetComment(int commentID, int userID);
    public abstract IDataReader GetCommentsByContentItem(int contentItemId, bool includeNonApproved, int userID);
    public abstract IDataReader GetCommentsByModuleId(int moduleId, int userID, int pageIndex, int pageSize, string orderBy);
    public abstract IDataReader GetPostByLegacyEntryId(int entryId, int portalId, string locale);
    public abstract IDataReader GetPostByLegacyUrl(string url, int portalId, string locale);
    public abstract IDataReader GetPostLocalizations(int contentItemId);
    public abstract IDataReader GetPosts(int moduleId, int blogID, string displayLocale, int userId, bool userIsAdmin, int published, string limitToLocale, DateTime endDate, int authorUserId, bool onlyActionable, int pageIndex, int pageSize, string orderBy);
    public abstract IDataReader GetPostsByTerm(int moduleId, int blogID, string displayLocale, int userId, bool userIsAdmin, int termID, int published, string limitToLocale, DateTime endDate, int authorUserId, int pageIndex, int pageSize, string orderBy);
    public abstract IDataReader GetPostsByCategory(int moduleId, int blogID, string displayLocale, int userId, bool userIsAdmin, string categories, int published, string limitToLocale, DateTime endDate, int authorUserId, int pageIndex, int pageSize, string orderBy);
    public abstract IDataReader GetTerm(int termId, int moduleId, string locale);
    public abstract IDataReader GetTermLocalizations(int termId);
    public abstract IDataReader GetTermsByModule(int moduleId, string locale);
    public abstract IDataReader GetTermsByPost(int contentItemId, int moduleId, string locale);
    public abstract IDataReader GetTermsByVocabulary(int moduleId, int vocabularyId, string locale);
    public abstract IDataReader GetUserPermissionsByModule(int moduleID, int userId);
    public abstract IDataReader GetUsersByBlogPermission(int portalId, int blogId, int permissionId);
    public abstract IDataReader SearchPosts(int moduleId, int blogID, string displayLocale, int userId, bool userIsAdmin, string searchText, bool searchTitle, bool searchContents, int published, string limitToLocale, DateTime endDate, int authorUserId, int pageIndex, int pageSize, string orderBy);
    public abstract IDataReader SearchPostsByTerm(int moduleId, int blogID, string displayLocale, int userId, bool userIsAdmin, int termID, string searchText, bool searchTitle, bool searchContents, int published, string limitToLocale, DateTime endDate, int authorUserId, int pageIndex, int pageSize, string orderBy);
    public abstract IDataReader SearchPostsByCategory(int moduleId, int blogID, string displayLocale, int userId, bool userIsAdmin, string categories, string searchText, bool searchTitle, bool searchContents, int published, string limitToLocale, DateTime endDate, int authorUserId, int pageIndex, int pageSize, string orderBy);
    public abstract void SetBlogLocalization(int blogID, string locale, string title, string description);
    public abstract void SetPostLocalization(int postID, string locale, string title, string summary, string content, int updatedByUser);
    public abstract int SetTerm(int termID, int vocabularyID, int parentTermID, int viewOrder, string name, string description, int createdByUserID);
    public abstract void SetTermLocalization(int termID, string locale, string name, string description);
    public abstract void UpdateModuleWiring(int portalId, int oldModuleId, int newModuleId);

  }

}