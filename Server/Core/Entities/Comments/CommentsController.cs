using System.Collections.Generic;
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


using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Modules.Blog.Core.Common;
using DotNetNuke.Modules.Blog.Core.Data;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using DotNetNuke.Modules.Blog.Core.Entities.Posts;
using DotNetNuke.Modules.Blog.Core.Integration;
using static DotNetNuke.Services.Localization.Localization;

namespace DotNetNuke.Modules.Blog.Core.Entities.Comments
{

  public partial class CommentsController
  {

    public static CommentInfo GetComment(int commentID, int userId)
    {

      return CBO.FillObject<CommentInfo>(DataProvider.Instance().GetComment(commentID, userId));

    }

    public static Dictionary<int, CommentInfo> GetCommentsByModule(int moduleId, int userID, int pageIndex, int pageSize, string orderBy, ref int totalRecords)
    {

      if (pageIndex < 0)
      {
        pageIndex = 0;
        pageSize = int.MaxValue;
      }

      var res = new Dictionary<int, CommentInfo>();
      using (var ir = DataProvider.Instance().GetCommentsByModuleId(moduleId, userID, pageIndex, pageSize, orderBy))
      {
        res = CBO.FillDictionary<int, CommentInfo>("CommentId", ir, false);
        ir.NextResult();
        var argdr = ir;
        totalRecords = DotNetNuke.Common.Globals.GetTotalRecords(ref argdr);
      }
      return res;

    }

    public static int AddComment(BlogInfo blog, PostInfo Post, ref CommentInfo comment)
    {

      AddComment(ref comment, PortalSettings.Current.UserId);
      if (comment.Approved)
      {
        JournalController.AddOrUpdateCommentInJournal(blog, Post, comment, PortalSettings.Current.PortalId, PortalSettings.Current.ActiveTab.TabID, PortalSettings.Current.UserId, Post.PermaLink(PortalSettings.Current));
      }
      else
      {
        string title = GetString("CommentPendingNotify.Subject", Globals.SharedResourceFileName);
        string summary = string.Format(GetString("CommentPendingNotify.Body", Globals.SharedResourceFileName), Post.PermaLink(PortalSettings.Current), comment.CommentID, Post.Title, comment.Comment);
        NotificationController.CommentPendingApproval(comment, blog, Post, PortalSettings.Current.PortalId, summary, title);
      }
      return comment.CommentID;

    }

    public static void UpdateComment(BlogInfo blog, PostInfo Post, CommentInfo comment)
    {

      UpdateComment(comment, PortalSettings.Current.UserId);
      NotificationController.RemoveCommentPendingNotification(blog.ModuleID, blog.BlogID, comment.ContentItemId, comment.CommentID);
      if (comment.Approved)
      {
        JournalController.AddOrUpdateCommentInJournal(blog, Post, comment, PortalSettings.Current.PortalId, PortalSettings.Current.ActiveTab.TabID, PortalSettings.Current.UserId, Post.PermaLink(PortalSettings.Current));
      }
      else
      {
        string title = GetString("CommentPendingNotify.Subject", Globals.SharedResourceFileName);
        string summary = string.Format(GetString("CommentPendingNotify.Body", Globals.SharedResourceFileName), Post.PermaLink(PortalSettings.Current), comment.CommentID, Post.Title, comment.Comment);
        NotificationController.CommentPendingApproval(comment, blog, Post, PortalSettings.Current.PortalId, summary, title);
      }

    }

    public static void ApproveComment(int moduleId, int blogId, CommentInfo comment)
    {

      DataProvider.Instance().ApproveComment(comment.CommentID);
      NotificationController.RemoveCommentPendingNotification(moduleId, blogId, comment.ContentItemId, comment.CommentID);

    }

    public static void DeleteComment(int moduleId, int blogId, CommentInfo comment)
    {

      DataProvider.Instance().DeleteComment(comment.CommentID);
      NotificationController.RemoveCommentPendingNotification(moduleId, blogId, comment.ContentItemId, comment.CommentID);

    }

    public static int CommentKarma(ModuleInfo callingModule, CommentInfo comment, DotNetNuke.Entities.Users.UserInfo user, int karma)
    {

      if (user.UserID < 0)
        return -1;
      int ret = DataProvider.Instance().AddCommentKarma(comment.CommentID, user.UserID, karma);
      if (ret > -1)
      {
        if (karma == 2) // reporting comment as inappropriate
        {
          string title = string.Format(GetString("CommentReportedNotify", Globals.SharedResourceFileName), user.DisplayName);
          var post = PostsController.GetPost(comment.ContentItemId, callingModule.ModuleID, "");
          string summary = string.Format(GetString("CommentReportedNotify.Body", Globals.SharedResourceFileName), user.DisplayName, comment.DisplayName, comment.Comment, post.PermaLink(PortalSettings.Current), post.Title);
          NotificationController.ReportComment(comment, post.Blog, post, callingModule.PortalID, summary, title);
        }
        return ret;
      }
      return 0;

    }

  }
}