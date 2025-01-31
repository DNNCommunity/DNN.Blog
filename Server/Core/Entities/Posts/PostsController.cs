using System;
using System.Collections.Generic;
using DotNetNuke.Common.Utilities;
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

using DotNetNuke.Modules.Blog.Data;
using DotNetNuke.Modules.Blog.Entities.Blogs;
using DotNetNuke.Modules.Blog.Integration;

namespace DotNetNuke.Modules.Blog.Entities.Posts
{

  public partial class PostsController
  {

    public static void PublishPost(PostInfo Post, bool publish, int publishedByUser)
    {

      if (Post.Published == publish)
        return;
      Post.Published = publish;
      UpdatePost(Post, publishedByUser);
      PublishPost(Post, publishedByUser);

    }

    public static void PublishPost(PostInfo Post, int publishedByUser)
    {

      var blog = BlogsController.GetBlog(Post.BlogID, publishedByUser, System.Threading.Thread.CurrentThread.CurrentCulture.Name);
      string journalUrl = Post.PermaLink(DotNetNuke.Entities.Portals.PortalSettings.Current);
      int journalUserId = publishedByUser;
      if (blog.PublishAsOwner)
        journalUserId = blog.OwnerUserId;
      JournalController.AddBlogPostToJournal(Post, DotNetNuke.Entities.Portals.PortalSettings.Current.PortalId, DotNetNuke.Entities.Portals.PortalSettings.Current.ActiveTab.TabID, journalUserId, journalUrl);
      NotificationController.RemovePostPendingNotification(blog.ModuleID, blog.BlogID, Post.ContentItemId);

      var trackAndPingbacks = new Services.TrackAndPingBackController(Post);
      var trd = new System.Threading.Thread(trackAndPingbacks.SendTrackAndPingBacks);
      trd.IsBackground = true;
      trd.Start();

    }

    public static void DeletePost(PostInfo Post)
    {
      DataProvider.Instance().DeletePost(Post.ContentItemId);
      var blog = BlogsController.GetBlog(Post.BlogID, -1, System.Threading.Thread.CurrentThread.CurrentCulture.Name);
      NotificationController.RemovePostPendingNotification(blog.ModuleID, blog.BlogID, Post.ContentItemId);
    }

    public static void DeletePost(int contentItemId, int blogId, int portalId, int vocabularyId)
    {
      DataProvider.Instance().DeletePost(contentItemId);
    }

    public static Dictionary<int, PostInfo> GetPosts(int moduleId, int blogID, string displayLocale, int published, string limitToLocale, DateTime endDate, int authorUserId, bool onlyActionable, int pageIndex, int pageSize, string orderBy, ref int totalRecords, int userId, bool userIsAdmin)
    {

      if (pageIndex < 0)
      {
        pageIndex = 0;
        pageSize = int.MaxValue;
      }

      var res = new Dictionary<int, PostInfo>();
      using (var ir = DataProvider.Instance().GetPosts(moduleId, blogID, displayLocale, userId, userIsAdmin, published, limitToLocale, endDate, authorUserId, onlyActionable, pageIndex, pageSize, orderBy))
      {
        res = CBO.FillDictionary<int, PostInfo>("ContentItemID", ir, false);
        ir.NextResult();
        var argdr = ir;
        totalRecords = DotNetNuke.Common.Globals.GetTotalRecords(ref argdr);
      }
      return GetPostsWithBlog(res, blogID, moduleId, userId, displayLocale);

    }

    public static Dictionary<int, PostInfo> GetPostsByTerm(int moduleId, int blogID, string displayLocale, int termId, int published, string limitToLocale, DateTime endDate, int authorUserId, int pageIndex, int pageSize, string orderBy, ref int totalRecords, int userId, bool userIsAdmin)
    {

      if (pageIndex < 0)
      {
        pageIndex = 0;
        pageSize = int.MaxValue;
      }

      var res = new Dictionary<int, PostInfo>();
      using (var ir = DataProvider.Instance().GetPostsByTerm(moduleId, blogID, displayLocale, userId, userIsAdmin, termId, published, limitToLocale, endDate, authorUserId, pageIndex, pageSize, orderBy))
      {
        res = CBO.FillDictionary<int, PostInfo>("ContentItemID", ir, false);
        ir.NextResult();
        var argdr = ir;
        totalRecords = DotNetNuke.Common.Globals.GetTotalRecords(ref argdr);
      }
      return GetPostsWithBlog(res, blogID, moduleId, userId, displayLocale);

    }

    public static Dictionary<int, PostInfo> GetPostsByCategory(int moduleId, int blogID, string displayLocale, string categories, int published, string limitToLocale, DateTime endDate, int authorUserId, int pageIndex, int pageSize, string orderBy, ref int totalRecords, int userId, bool userIsAdmin)
    {

      if (pageIndex < 0)
      {
        pageIndex = 0;
        pageSize = int.MaxValue;
      }

      var res = new Dictionary<int, PostInfo>();
      using (var ir = DataProvider.Instance().GetPostsByCategory(moduleId, blogID, displayLocale, userId, userIsAdmin, categories, published, limitToLocale, endDate, authorUserId, pageIndex, pageSize, orderBy))
      {
        res = CBO.FillDictionary<int, PostInfo>("ContentItemID", ir, false);
        ir.NextResult();
        var argdr = ir;
        totalRecords = DotNetNuke.Common.Globals.GetTotalRecords(ref argdr);
      }
      return GetPostsWithBlog(res, blogID, moduleId, userId, displayLocale);

    }

    public static Dictionary<int, PostInfo> GetPostsByBlog(int moduleId, int blogID, string displayLocale, int userId, int pageIndex, int pageSize, string orderBy, ref int totalRecords)
    {

      if (pageIndex < 0)
      {
        pageIndex = 0;
        pageSize = int.MaxValue;
      }

      var res = new Dictionary<int, PostInfo>();
      using (var ir = DataProvider.Instance().GetPostsByBlog(blogID, displayLocale, pageIndex, pageSize, orderBy))
      {
        res = CBO.FillDictionary<int, PostInfo>("ContentItemID", ir, false);
        ir.NextResult();
        var argdr = ir;
        totalRecords = DotNetNuke.Common.Globals.GetTotalRecords(ref argdr);
      }
      return GetPostsWithBlog(res, blogID, moduleId, userId, displayLocale);

    }

    public static Dictionary<int, PostInfo> SearchPosts(int moduleId, int blogID, string displayLocale, string searchText, bool searchTitle, bool searchContents, int published, string limitToLocale, DateTime endDate, int authorUserId, int pageIndex, int pageSize, string orderBy, ref int totalRecords, int userId, bool userIsAdmin)
    {

      if (pageIndex < 0)
      {
        pageIndex = 0;
        pageSize = int.MaxValue;
      }

      var res = new Dictionary<int, PostInfo>();
      using (var ir = DataProvider.Instance().SearchPosts(moduleId, blogID, displayLocale, userId, userIsAdmin, searchText, searchTitle, searchContents, published, limitToLocale, endDate, authorUserId, pageIndex, pageSize, orderBy))
      {
        res = CBO.FillDictionary<int, PostInfo>("ContentItemID", ir, false);
        ir.NextResult();
        var argdr = ir;
        totalRecords = DotNetNuke.Common.Globals.GetTotalRecords(ref argdr);
      }
      return GetPostsWithBlog(res, blogID, moduleId, userId, displayLocale);

    }

    public static Dictionary<int, PostInfo> SearchPostsByTerm(int moduleId, int blogID, string displayLocale, int termId, string searchText, bool searchTitle, bool searchContents, int published, string limitToLocale, DateTime endDate, int authorUserId, int pageIndex, int pageSize, string orderBy, ref int totalRecords, int userId, bool userIsAdmin)
    {

      if (pageIndex < 0)
      {
        pageIndex = 0;
        pageSize = int.MaxValue;
      }

      var res = new Dictionary<int, PostInfo>();
      using (var ir = DataProvider.Instance().SearchPostsByTerm(moduleId, blogID, displayLocale, userId, userIsAdmin, termId, searchText, searchTitle, searchContents, published, limitToLocale, endDate, authorUserId, pageIndex, pageSize, orderBy))
      {
        res = CBO.FillDictionary<int, PostInfo>("ContentItemID", ir, false);
        ir.NextResult();
        var argdr = ir;
        totalRecords = DotNetNuke.Common.Globals.GetTotalRecords(ref argdr);
      }
      return GetPostsWithBlog(res, blogID, moduleId, userId, displayLocale);

    }

    public static Dictionary<int, PostInfo> SearchPostsByCategory(int moduleId, int blogID, string displayLocale, string categories, string searchText, bool searchTitle, bool searchContents, int published, string limitToLocale, DateTime endDate, int authorUserId, int pageIndex, int pageSize, string orderBy, ref int totalRecords, int userId, bool userIsAdmin)
    {

      if (pageIndex < 0)
      {
        pageIndex = 0;
        pageSize = int.MaxValue;
      }

      var res = new Dictionary<int, PostInfo>();
      using (var ir = DataProvider.Instance().SearchPostsByCategory(moduleId, blogID, displayLocale, userId, userIsAdmin, categories, searchText, searchTitle, searchContents, published, limitToLocale, endDate, authorUserId, pageIndex, pageSize, orderBy))
      {
        res = CBO.FillDictionary<int, PostInfo>("ContentItemID", ir, false);
        ir.NextResult();
        var argdr = ir;
        totalRecords = DotNetNuke.Common.Globals.GetTotalRecords(ref argdr);
      }
      return GetPostsWithBlog(res, blogID, moduleId, userId, displayLocale);

    }

    public static List<PostAuthor> GetAuthors(int moduleId, int blogId)
    {
      return CBO.FillCollection<PostAuthor>(DataProvider.Instance().GetAuthors(moduleId, blogId));
    }

    public static PostInfo GetPostByLegacyEntryId(int entryId, int portalId, string locale)
    {
      return CBO.FillObject<PostInfo>(DataProvider.Instance().GetPostByLegacyEntryId(entryId, portalId, locale));
    }

    public static PostInfo GetPostByLegacyUrl(string url, int portalId, string locale)
    {
      return CBO.FillObject<PostInfo>(DataProvider.Instance().GetPostByLegacyUrl(url, portalId, locale));
    }

    public static List<PostInfo> GetChangedPosts(int moduleId, DateTime lastChange)
    {
      return CBO.FillCollection<PostInfo>(DotNetNuke.Data.DataProvider.Instance().ExecuteReader("Blog_GetChangedPosts", moduleId, lastChange));
    }

    #region  Private Methods 
    private static Dictionary<int, PostInfo> GetPostsWithBlog(Dictionary<int, PostInfo> selection, int blogId, int moduleId, int userId, string displayLocale)
    {

      var res = new Dictionary<int, PostInfo>();
      if (blogId == -1)
      {
        var blogs = BlogsController.GetBlogsByModule(moduleId, userId, displayLocale);
        foreach (PostInfo e in selection.Values)
        {
          if (blogs.ContainsKey(e.BlogID))
          {
            e.Blog = blogs[e.BlogID];
            res.Add(e.ContentItemId, e);
          }
        }
      }
      else
      {
        var blog = BlogsController.GetBlog(blogId, userId, displayLocale);
        foreach (PostInfo e in selection.Values)
        {
          e.Blog = blog;
          res.Add(e.ContentItemId, e);
        }
      }
      return res;

    }
    #endregion

  }

}