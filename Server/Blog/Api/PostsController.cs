using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Core.Entities.Posts;
using DotNetNuke.Web.Api;
using Microsoft.Extensions.Logging;
using CorePosts = DotNetNuke.Modules.Blog.Core.Entities.Posts.PostsController;

namespace DotNetNuke.Modules.Blog.Api
{
    public class PostsController : BlogApiController
    {
        private readonly ILogger<PostsController> _logger;

        public PostsController(ILogger<PostsController> logger) : base()
        {
            _logger = logger;
        }

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
            _logger.LogDebug("Post list requested: page {Page}, pageSize {PageSize}, blogId {BlogId}, orderBy {OrderBy}, hasSearch {HasSearch}, user {UserId}",
                page, pageSize, blogId, orderBy, !string.IsNullOrWhiteSpace(search), UserInfo.UserID);

            if (page < 0) page = 0;
            if (pageSize < 1) pageSize = 25;
            if (pageSize > 100) pageSize = 100;
            if (blogId < 1) blogId = -1;

            var sanitizedOrderBy = SanitizeOrderBy(orderBy);
            if (!string.Equals(orderBy?.Trim(), sanitizedOrderBy, StringComparison.OrdinalIgnoreCase))
                _logger.LogDebug("Sanitized orderBy from '{Original}' to '{Sanitized}'", orderBy, sanitizedOrderBy);

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
                _logger.LogDebug("Searching posts with query length {Length}", search.Trim().Length);
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

            _logger.LogInformation("Post list returned {ItemCount}/{Total} posts for module {ModuleId} (page {Page})",
                items.Count, total, settings.ModuleId, page);
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
            _logger.LogDebug("Approve post requested: blogId {BlogId}, postId {PostId}, user {UserId}",
                postData?.BlogId, postData?.PostId, UserInfo.UserID);

            if (postData == null)
            {
                _logger.LogWarning("Approve post failed: null payload");
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
            }

            var result = new PostWriteService().Approve(
                BlogModuleContext.Settings.ModuleId,
                postData.BlogId,
                postData.PostId,
                BlogModuleContext.BlogContext.Locale,
                UserInfo,
                BlogModuleContext.Security);

            if (result.NotFound)
            {
                _logger.LogWarning("Approve post failed: post {PostId} not found or blog mismatch {BlogId}", postData.PostId, postData.BlogId);
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
            }
            if (result.AccessDenied)
            {
                _logger.LogWarning("Approve post denied: user {UserId} cannot approve post {PostId}", UserInfo.UserID, postData.PostId);
                return AccessViolation("You are not allowed to approve this post");
            }

            _logger.LogInformation("Approved/published post {PostId} on blog {BlogId} by user {UserId}", postData.PostId, postData.BlogId, UserInfo.UserID);
            return Request.CreateResponse(HttpStatusCode.OK, new { Result = "success" });
        }

        [HttpPost]
        [BlogAuthorize(SecurityAccessLevel.Edit)]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage Delete(PostActionDto postData)
        {
            _logger.LogDebug("Delete post requested: blogId {BlogId}, postId {PostId}, user {UserId}",
                postData?.BlogId, postData?.PostId, UserInfo.UserID);

            if (postData == null)
            {
                _logger.LogWarning("Delete post failed: null payload");
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
            }

            var result = new PostWriteService().Delete(
                BlogModuleContext.Settings.ModuleId,
                postData.BlogId,
                postData.PostId,
                BlogModuleContext.BlogContext.Locale,
                UserInfo,
                BlogModuleContext.Security,
                cleanupImages: true);

            if (result.NotFound)
            {
                _logger.LogWarning("Delete post failed: post {PostId} not found or blog mismatch {BlogId}", postData.PostId, postData.BlogId);
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
            }
            if (result.AccessDenied)
            {
                _logger.LogWarning("Delete post denied: user {UserId} cannot delete post {PostId}", UserInfo.UserID, postData.PostId);
                return AccessViolation("You are not allowed to delete this post");
            }

            _logger.LogInformation("Deleted post {PostId} from blog {BlogId} by user {UserId}", postData.PostId, postData.BlogId, UserInfo.UserID);
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
