using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Core.Entities.Posts;
using DotNetNuke.Web.Api;
using CorePosts = DotNetNuke.Modules.Blog.Core.Entities.Posts.PostsController;

namespace DotNetNuke.Modules.Blog.Api
{
    public class PostsController : BlogApiController
    {
        private static readonly HashSet<string> AllowedOrderColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "PUBLISHEDONDATE",
            "TITLE",
            "PUBLISHED"
        };

        public class PostActionDto
        {
            public int BlogId { get; set; }
            public int PostId { get; set; }
        }

        public class ManagePostDto
        {
            public int BlogId { get; set; }
            public int PostId { get; set; }
            public string Title { get; set; }
            public DateTime PublishedOnDate { get; set; }
            public bool Published { get; set; }
            public string BlogTitle { get; set; }
            public bool CanEdit { get; set; }
            public bool CanApprove { get; set; }
        }

        public class ManagePostListDto
        {
            public IList<ManagePostDto> Items { get; set; } = new List<ManagePostDto>();
            public int Page { get; set; }
            public int PageSize { get; set; }
            public int Total { get; set; }
        }

        [HttpGet]
        [BlogAuthorize(SecurityAccessLevel.Edit)]
        public HttpResponseMessage List(int page = 0, int pageSize = 25, string orderBy = null, string search = null, int blogId = -1)
        {
            if (page < 0) page = 0;
            if (pageSize < 1) pageSize = 25;
            if (pageSize > 100) pageSize = 100;
            if (blogId < 1) blogId = -1;

            var sanitizedOrderBy = SanitizeOrderBy(orderBy);
            var security = BlogModuleContext.Security;
            var settings = BlogModuleContext.Settings;
            var locale = BlogModuleContext.BlogContext.Locale;
            int total = 0;

            IEnumerable<PostInfo> posts;
            if (string.IsNullOrWhiteSpace(search))
            {
                posts = CorePosts.GetPosts(
                    settings.ModuleId, blogId, locale, -1, "", default(DateTime), -1, true,
                    page, pageSize, sanitizedOrderBy, ref total, UserInfo.UserID, security.UserIsAdmin).Values;
            }
            else
            {
                posts = CorePosts.SearchPosts(
                    settings.ModuleId, blogId, locale, search.Trim(), true, false, -1, "", default(DateTime), -1,
                    page, pageSize, sanitizedOrderBy, ref total, UserInfo.UserID, security.UserIsAdmin).Values;
            }

            var items = posts.Select(p =>
            {
                var canEdit = p.Blog != null && (p.Blog.CanEdit || security.CanEditThisPost(p));
                var canApprove = p.Blog != null && p.Blog.CanApprove && !p.Published;
                return new ManagePostDto
                {
                    BlogId = p.BlogID,
                    PostId = p.ContentItemId,
                    Title = p.Title ?? string.Empty,
                    PublishedOnDate = p.PublishedOnDate,
                    Published = p.Published,
                    BlogTitle = p.Blog?.Title ?? string.Empty,
                    CanEdit = canEdit,
                    CanApprove = canApprove
                };
            }).ToList();

            return Request.CreateResponse(HttpStatusCode.OK, new ManagePostListDto
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                Total = total
            });
        }

        [HttpPost]
        [BlogAuthorize(SecurityAccessLevel.Edit)]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage Approve(PostActionDto postData)
        {
            if (postData == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });

            var post = CorePosts.GetPost(postData.PostId, BlogModuleContext.Settings.ModuleId, BlogModuleContext.BlogContext.Locale);
            if (post == null || post.BlogID != postData.BlogId)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });

            if (post.Blog == null || !post.Blog.CanApprove)
                return AccessViolation("You are not allowed to approve this post");

            CorePosts.PublishPost(post, true, UserInfo.UserID);
            return Request.CreateResponse(HttpStatusCode.OK, new { Result = "success" });
        }

        [HttpPost]
        [BlogAuthorize(SecurityAccessLevel.Edit)]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage Delete(PostActionDto postData)
        {
            if (postData == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });

            var security = BlogModuleContext.Security;
            var post = CorePosts.GetPost(postData.PostId, BlogModuleContext.Settings.ModuleId, BlogModuleContext.BlogContext.Locale);
            if (post == null || post.BlogID != postData.BlogId)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });

            if (!(post.Blog?.CanEdit == true || security.CanEditThisPost(post)))
                return AccessViolation("You are not allowed to delete this post");

            CorePosts.DeletePost(post);
            return Request.CreateResponse(HttpStatusCode.OK, new { Result = "success" });
        }

        private static string SanitizeOrderBy(string orderBy)
        {
            const string fallback = "PUBLISHEDONDATE DESC";
            if (string.IsNullOrWhiteSpace(orderBy))
                return fallback;

            var parts = orderBy.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 1 || !AllowedOrderColumns.Contains(parts[0]))
                return fallback;

            var column = parts[0].ToUpperInvariant();
            var direction = "DESC";
            if (parts.Length > 1)
            {
                var dir = parts[1].ToUpperInvariant();
                if (dir == "ASC" || dir == "DESC")
                    direction = dir;
            }

            return column + " " + direction;
        }
    }
}
