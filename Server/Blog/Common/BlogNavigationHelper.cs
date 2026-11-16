using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using DotNetNuke.Modules.Blog.Core.Security;

namespace DotNetNuke.Modules.Blog.Common
{
    /// <summary>Resolves blogs for New Post navigation and the blog chooser dialog.</summary>
    public static class BlogNavigationHelper
    {
        public static List<BlogInfo> GetBlogsForNewPost(int moduleId, int userId, string locale, ContextSecurity security, int contentItemId)
        {
            var all = BlogsController.GetBlogsByModule(moduleId, userId, locale).Values;
            if (security.IsEditor)
                return all.OrderBy(b => b.LocalizedTitle).ToList();

            return all
                .Where(b => b.OwnerUserId == userId || b.CanAdd || (b.CanEdit && contentItemId > -1))
                .OrderBy(b => b.LocalizedTitle)
                .ToList();
        }

        public static int ResolveBlogIdForNewPost(int moduleId, int userId, string locale, int currentBlogId)
        {
            if (currentBlogId > 0)
                return currentBlogId;

            var blogs = BlogsController.GetBlogsByModule(moduleId, userId, locale).Values.ToList();
            if (blogs.Count == 0)
                return -1;

            var owned = blogs.FirstOrDefault(b => b.OwnerUserId == userId);
            if (owned != null)
                return owned.BlogID;

            var canAdd = blogs.FirstOrDefault(b => b.CanAdd);
            if (canAdd != null)
                return canAdd.BlogID;

            return blogs[0].BlogID;
        }

        public static int ResolveBlogIdForNewPost(
            int moduleId,
            int userId,
            string locale,
            int currentBlogId,
            ContextSecurity security,
            int contentItemId)
        {
            if (currentBlogId > 0)
                return currentBlogId;

            var blogs = GetBlogsForNewPost(moduleId, userId, locale, security, contentItemId);
            return blogs.Count > 0 ? blogs[0].BlogID : -1;
        }
    }
}
