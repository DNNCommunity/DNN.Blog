using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Xml;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Core.Common;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using DotNetNuke.Modules.Blog.Core.Entities.Comments;
using DotNetNuke.Modules.Blog.Core.Entities.Posts;
using DotNetNuke.Modules.Blog.Core.Security;
using DotNetNuke.Web.Api;
using Microsoft.Extensions.Logging;
using BlogGlobals = DotNetNuke.Modules.Blog.Core.Common.Globals;
using CoreComments = DotNetNuke.Modules.Blog.Core.Entities.Comments.CommentsController;
using CoreBlogs = DotNetNuke.Modules.Blog.Core.Entities.Blogs.BlogsController;
using CorePosts = DotNetNuke.Modules.Blog.Core.Entities.Posts.PostsController;

namespace DotNetNuke.Modules.Blog.Api
{
    public class CommentsController : BlogApiController
    {
        private readonly ILogger<CommentsController> _logger;

        public CommentsController(ILogger<CommentsController> logger) : base()
        {
            _logger = logger;
        }

        public class CommentActionDto
        {
            public int BlogId { get; set; }
            public int CommentId { get; set; }
            public int Karma { get; set; }
        }

        public class CommentAddDto
        {
            public int BlogId { get; set; }
            public int PostId { get; set; }
            public int ParentId { get; set; } = -1;
            public string Comment { get; set; }
            public string Author { get; set; }
            public string Website { get; set; }
            public string Email { get; set; }
        }

        public class CommentDto
        {
            public int CommentId { get; set; }
            public int ParentId { get; set; }
            public string DisplayName { get; set; }
            public int AuthorUserId { get; set; }
            public string ProfilePicUrl { get; set; }
            public string ProfileUrl { get; set; }
            public string CommentHtml { get; set; }
            public DateTime CreatedOnDateUtc { get; set; }
            public bool Approved { get; set; }
            public int Likes { get; set; }
            public int Dislikes { get; set; }
            public int Reports { get; set; }
            public bool Liked { get; set; }
            public bool Disliked { get; set; }
            public bool Reported { get; set; }
            public List<CommentDto> Replies { get; set; } = new List<CommentDto>();
        }

        public class CommentListDto
        {
            public string Result { get; set; }
            public int Total { get; set; }
            public List<CommentDto> Comments { get; set; } = new List<CommentDto>();
        }

        public class CommentAddResultDto
        {
            public string Result { get; set; }
            public CommentDto Comment { get; set; }
        }

        [HttpGet]
        [BlogAuthorize(SecurityAccessLevel.View)]
        public HttpResponseMessage List(int blogId, int postId)
        {
            _logger.LogDebug("Comment list requested: blogId {BlogId}, postId {PostId}, user {UserId}", blogId, postId, UserInfo.UserID);

            var blog = CoreBlogs.GetBlog(blogId, UserInfo.UserID, BlogModuleContext.BlogContext.Locale);
            var post = CorePosts.GetPost(postId, ActiveModule.ModuleID, "");
            if (blog == null || post == null)
            {
                _logger.LogWarning("Comment list failed: blog {BlogId} or post {PostId} not found", blogId, postId);
                return Request.CreateResponse(HttpStatusCode.BadRequest, new CommentListDto { Result = "error" });
            }

            var security = CreateSecurity(blog);
            if (!security.CanViewComments)
            {
                _logger.LogInformation("Comment list empty: user {UserId} cannot view comments on blog {BlogId}", UserInfo.UserID, blogId);
                return Request.CreateResponse(HttpStatusCode.OK, new CommentListDto { Result = "success", Total = 0 });
            }

            var all = CoreComments.GetCommentsByContentItem(post.ContentItemId, security.CanApproveComment, UserInfo.UserID);
            var tree = BuildTree(all);
            var total = CountApprovedOrVisible(all, security.CanApproveComment);
            _logger.LogInformation("Comment list returned {Total} comments for post {PostId}", total, postId);
            return Request.CreateResponse(HttpStatusCode.OK, new CommentListDto
            {
                Result = "success",
                Total = total,
                Comments = tree
            });
        }

        [HttpPost]
        [BlogAuthorize(SecurityAccessLevel.AddComment)]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage Add(CommentAddDto postData)
        {
            _logger.LogDebug("Add comment requested: blogId {BlogId}, postId {PostId}, parentId {ParentId}, user {UserId}",
                postData?.BlogId, postData?.PostId, postData?.ParentId, UserInfo.UserID);

            if (postData == null)
            {
                _logger.LogWarning("Add comment failed: null payload");
                return Request.CreateResponse(HttpStatusCode.BadRequest, new CommentAddResultDto { Result = "error" });
            }

            var blog = CoreBlogs.GetBlog(postData.BlogId, UserInfo.UserID, BlogModuleContext.BlogContext.Locale);
            var post = CorePosts.GetPost(postData.PostId, ActiveModule.ModuleID, "");
            if (blog == null || post == null)
            {
                _logger.LogWarning("Add comment failed: blog {BlogId} or post {PostId} not found", postData.BlogId, postData.PostId);
                return Request.CreateResponse(HttpStatusCode.BadRequest, new CommentAddResultDto { Result = "error" });
            }

            var security = CreateSecurity(blog);
            if (!security.CanAddComment || !post.AllowComments)
            {
                _logger.LogWarning("Add comment denied: user {UserId}, canAdd={CanAdd}, allowComments={AllowComments}",
                    UserInfo.UserID, security.CanAddComment, post.AllowComments);
                return AccessViolation("You are not allowed to add comments");
            }

            var parentId = postData.ParentId > 0 ? postData.ParentId : -1;
            var author = UserInfo.UserID > -1
                ? (UserInfo.DisplayName ?? "")
                : BlogGlobals.SafeString(postData.Author ?? "");
            var email = UserInfo.UserID > -1
                ? (UserInfo.Email ?? "")
                : BlogGlobals.SafeString(postData.Email ?? "");
            var website = UserInfo.UserID > -1
                ? (UserInfo.Profile?.Website ?? "")
                : BlogGlobals.SafeString(postData.Website ?? "");

            var objComment = new CommentInfo
            {
                ContentItemId = post.ContentItemId,
                CreatedByUserID = UserInfo.UserID,
                ParentId = parentId,
                Comment = HttpUtility.HtmlEncode(
                    BlogGlobals.SafeStringSimpleHtml(postData.Comment ?? "").Replace("\r\n", "<br />").Replace("\n", "<br />")),
                Approved = security.CanAutoApproveComment || security.CanApproveComment || post.CreatedByUserID == UserInfo.UserID,
                Author = author,
                Email = email,
                Website = website,
                DisplayName = author
            };

            objComment.CommentID = CoreComments.AddComment(blog, post, ref objComment);

            // Reload so CreatedOnDate and karma flags come from the DB (Add leaves MinValue).
            var saved = CoreComments.GetComment(objComment.CommentID, UserInfo.UserID) ?? objComment;
            if (saved.CreatedOnDate == DateTime.MinValue)
                saved.CreatedOnDate = DateTime.Now;

            if (!objComment.Approved)
            {
                _logger.LogInformation("Added comment {CommentId} on post {PostId} awaiting approval", objComment.CommentID, postData.PostId);
                return Request.CreateResponse(HttpStatusCode.OK, new CommentAddResultDto
                {
                    Result = "successnotapproved",
                    Comment = MapComment(saved)
                });
            }

            _logger.LogInformation("Added approved comment {CommentId} on post {PostId}", objComment.CommentID, postData.PostId);
            return Request.CreateResponse(HttpStatusCode.OK, new CommentAddResultDto
            {
                Result = "success",
                Comment = MapComment(saved)
            });
        }

        [HttpPost]
        [BlogAuthorize(SecurityAccessLevel.AddComment)]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage Karma(CommentActionDto postData)
        {
            _logger.LogDebug("Karma requested: commentId {CommentId}, karma {Karma}, user {UserId}",
                postData?.CommentId, postData?.Karma, UserInfo.UserID);

            if (postData == null)
            {
                _logger.LogWarning("Karma failed: null payload");
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
            }
            if (UserInfo.UserID < 0)
            {
                _logger.LogWarning("Karma failed: anonymous user");
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
            }

            var blog = CoreBlogs.GetBlog(postData.BlogId, UserInfo.UserID, BlogModuleContext.BlogContext.Locale);
            var comment = CoreComments.GetComment(postData.CommentId, UserInfo.UserID);
            if (blog == null || comment == null)
            {
                _logger.LogWarning("Karma failed: blog {BlogId} or comment {CommentId} not found", postData.BlogId, postData.CommentId);
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
            }

            var security = CreateSecurity(blog);
            if (!security.CanAddComment)
            {
                _logger.LogWarning("Karma denied: user {UserId} cannot karma on blog {BlogId}", UserInfo.UserID, postData.BlogId);
                return AccessViolation("You are not allowed to karma comments");
            }

            var ret = CoreComments.CommentKarma(ActiveModule, comment, UserInfo, postData.Karma);
            if (ret < 0)
            {
                _logger.LogWarning("Karma failed for comment {CommentId}: result {Result}", postData.CommentId, ret);
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
            }

            var refreshed = CoreComments.GetComment(postData.CommentId, UserInfo.UserID);
            var result = ret == 1 ? "added" : "removed";
            _logger.LogInformation("Karma {Result} for comment {CommentId} by user {UserId}", result, postData.CommentId, UserInfo.UserID);
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Result = result,
                Comment = refreshed != null ? MapComment(refreshed) : null
            });
        }

        [HttpPost]
        [BlogAuthorize(SecurityAccessLevel.ApproveComment)]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage Approve(CommentActionDto postData)
        {
            _logger.LogDebug("Approve comment requested: commentId {CommentId}, blogId {BlogId}, user {UserId}",
                postData?.CommentId, postData?.BlogId, UserInfo.UserID);

            if (postData == null)
            {
                _logger.LogWarning("Approve comment failed: null payload");
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
            }

            var blog = CoreBlogs.GetBlog(postData.BlogId, UserInfo.UserID, BlogModuleContext.BlogContext.Locale);
            var comment = CoreComments.GetComment(postData.CommentId, UserInfo.UserID);
            if (blog == null || comment == null)
            {
                _logger.LogWarning("Approve comment failed: blog {BlogId} or comment {CommentId} not found", postData.BlogId, postData.CommentId);
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
            }

            var security = CreateSecurity(blog);
            if (!security.CanApproveComment)
            {
                _logger.LogWarning("Approve comment denied: user {UserId}", UserInfo.UserID);
                return AccessViolation("You are not allowed to approve comments");
            }

            CoreComments.ApproveComment(ActiveModule.ModuleID, blog.BlogID, comment);
            _logger.LogInformation("Approved comment {CommentId} on blog {BlogId} by user {UserId}", postData.CommentId, blog.BlogID, UserInfo.UserID);
            return Request.CreateResponse(HttpStatusCode.OK, new { Result = "success" });
        }

        [HttpPost]
        [BlogAuthorize(SecurityAccessLevel.ApproveComment)]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage Delete(CommentActionDto postData)
        {
            _logger.LogDebug("Delete comment requested: commentId {CommentId}, blogId {BlogId}, user {UserId}",
                postData?.CommentId, postData?.BlogId, UserInfo.UserID);

            if (postData == null)
            {
                _logger.LogWarning("Delete comment failed: null payload");
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
            }

            var blog = CoreBlogs.GetBlog(postData.BlogId, UserInfo.UserID, BlogModuleContext.BlogContext.Locale);
            var comment = CoreComments.GetComment(postData.CommentId, UserInfo.UserID);
            if (blog == null || comment == null)
            {
                _logger.LogWarning("Delete comment failed: blog {BlogId} or comment {CommentId} not found", postData.BlogId, postData.CommentId);
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
            }

            var security = CreateSecurity(blog);
            if (!security.CanApproveComment)
            {
                _logger.LogWarning("Delete comment denied: user {UserId}", UserInfo.UserID);
                return AccessViolation("You are not allowed to delete comments");
            }

            CoreComments.DeleteComment(ActiveModule.ModuleID, blog.BlogID, comment);
            _logger.LogInformation("Deleted comment {CommentId} from blog {BlogId} by user {UserId}", postData.CommentId, blog.BlogID, UserInfo.UserID);
            return Request.CreateResponse(HttpStatusCode.OK, new { Result = "success" });
        }

        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage Pingback()
        {
            int blogId = -1;
            int postId = -1;
            blogId = HttpContext.Current.Request.Params.ReadValue("blogId", blogId);
            postId = HttpContext.Current.Request.Params.ReadValue("postId", postId);
            _logger.LogDebug("Pingback received: blogId {BlogId}, postId {PostId}", blogId, postId);

            var blog = CoreBlogs.GetBlog(blogId, UserInfo.UserID, System.Threading.Thread.CurrentThread.CurrentCulture.Name);
            if (blog == null || !blog.EnablePingBackReceive)
            {
                _logger.LogWarning("Pingback rejected: blog {BlogId} missing or pingbacks disabled", blogId);
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Result = "This blog does not accept pingbacks" });
            }

            var post = CorePosts.GetPost(postId, ActiveModule.ModuleID, "");
            if (post == null)
            {
                _logger.LogWarning("Pingback rejected: post {PostId} not found", postId);
                return PingBackError(32, "The specified target URI does not exist.");
            }

            XmlDocument doc;
            try
            {
                doc = RetrieveXmlDocument(HttpContext.Current);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Pingback parse failed for blog {BlogId}, post {PostId}", blogId, postId);
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "Cannot parse the request" });
            }

            var list = doc.SelectNodes("methodCall/params/param/value/string")
                        ?? doc.SelectNodes("methodCall/params/param/value");
            if (list == null || list.Count < 2)
            {
                _logger.LogWarning("Pingback parse failed: insufficient params for post {PostId}", postId);
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "Cannot parse the request" });
            }

            var sourceUrl = BlogGlobals.SafeString(list[0].InnerText.Trim());
            var targetUrl = BlogGlobals.SafeString(list[1].InnerText.Trim());
            _logger.LogDebug("Pingback source {SourceUrl} -> target {TargetUrl}", sourceUrl, targetUrl);

            var sourceHasLink = false;
            var title = sourceUrl;
            try
            {
                CheckSourcePage(sourceUrl, targetUrl, ref sourceHasLink, ref title);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Pingback source page check failed for {SourceUrl}", sourceUrl);
            }

            if (!IsFirstPingBack(post, sourceUrl))
            {
                _logger.LogInformation("Pingback duplicate ignored for source {SourceUrl} on post {PostId}", sourceUrl, postId);
                return PingBackError(48, "The pingback has already been registered.");
            }
            if (!sourceHasLink)
            {
                _logger.LogWarning("Pingback rejected: source {SourceUrl} does not link to target", sourceUrl);
                return PingBackError(17, "The source URI does not contain a link to the target URI, and so cannot be used as a source.");
            }

            var objComment = new CommentInfo
            {
                ContentItemId = post.ContentItemId,
                Author = GetDomain(sourceUrl),
                Website = sourceUrl
            };
            var comment = string.Format(
                DotNetNuke.Services.Localization.Localization.GetString("PingbackComment", BlogGlobals.SharedResourceFileName),
                objComment.Author, sourceUrl, title);
            objComment.Comment = HttpUtility.HtmlEncode(comment);
            objComment.Approved = blog.AutoApprovePingBack;
            CoreComments.AddComment(blog, post, ref objComment);
            _logger.LogInformation("Pingback comment {CommentId} added for post {PostId} from {SourceUrl} (approved={Approved})",
                objComment.CommentID, postId, sourceUrl, objComment.Approved);
            return PingBackSuccess();
        }

        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage Trackback()
        {
            int blogId = -1;
            int postId = -1;
            blogId = HttpContext.Current.Request.Params.ReadValue("blogId", blogId);
            postId = HttpContext.Current.Request.Params.ReadValue("postId", postId);
            _logger.LogDebug("Trackback received: blogId {BlogId}, postId {PostId}", blogId, postId);

            var blog = CoreBlogs.GetBlog(blogId, UserInfo.UserID, System.Threading.Thread.CurrentThread.CurrentCulture.Name);
            if (blog == null || !blog.EnableTrackBackReceive)
            {
                _logger.LogWarning("Trackback rejected: blog {BlogId} missing or trackbacks disabled", blogId);
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Result = "This blog does not accept trackbacks" });
            }

            var post = CorePosts.GetPost(postId, ActiveModule.ModuleID, "");
            if (post == null)
            {
                _logger.LogWarning("Trackback rejected: post {PostId} not found", postId);
                return TrackBackResponse("The source page does not link");
            }

            var title = BlogGlobals.SafeString(HttpContext.Current.Request.Params["title"]);
            var blogName = BlogGlobals.SafeString(HttpContext.Current.Request.Params["blog_name"]);
            var sourceUrl = string.Empty;
            if (HttpContext.Current.Request.Params["url"] != null)
            {
                sourceUrl = BlogGlobals.SafeString(
                    HttpContext.Current.Request.Params["url"].Split(',')[0],
                    DotNetNuke.Security.PortalSecurity.FilterFlag.NoSQL & DotNetNuke.Security.PortalSecurity.FilterFlag.NoScripting);
            }

            _logger.LogDebug("Trackback source {SourceUrl}, blogName {BlogName}", sourceUrl, blogName);

            var sourceHasLink = false;
            try
            {
                CheckSourcePage(sourceUrl, post.PermaLink(PortalSettings), ref sourceHasLink, ref title);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Trackback source page check failed for {SourceUrl}", sourceUrl);
            }

            if (!IsFirstPingBack(post, sourceUrl))
            {
                _logger.LogInformation("Trackback duplicate ignored for source {SourceUrl} on post {PostId}", sourceUrl, postId);
                return TrackBackResponse("Trackback already registered");
            }
            if (!sourceHasLink)
            {
                _logger.LogWarning("Trackback rejected: source {SourceUrl} does not link to post {PostId}", sourceUrl, postId);
                return TrackBackResponse("The source page does not link");
            }

            var objComment = new CommentInfo
            {
                ContentItemId = post.ContentItemId,
                Author = blogName,
                Website = sourceUrl
            };
            var comment = string.Format(
                DotNetNuke.Services.Localization.Localization.GetString("TrackbackComment", BlogGlobals.SharedResourceFileName),
                blogName, sourceUrl, title);
            objComment.Comment = HttpUtility.HtmlEncode(comment);
            objComment.Approved = blog.AutoApproveTrackBack;
            CoreComments.AddComment(blog, post, ref objComment);
            _logger.LogInformation("Trackback comment {CommentId} added for post {PostId} from {SourceUrl} (approved={Approved})",
                objComment.CommentID, postId, sourceUrl, objComment.Approved);
            return TrackBackResponse();
        }

        private ContextSecurity CreateSecurity(BlogInfo blog)
        {
            return new ContextSecurity(ActiveModule.ModuleID, ActiveModule.TabID, blog, UserInfo);
        }

        private static int CountApprovedOrVisible(IEnumerable<CommentInfo> all, bool canApprove)
        {
            return all.Count(c => canApprove || c.Approved);
        }

        private static List<CommentDto> BuildTree(List<CommentInfo> all)
        {
            var lookup = all.ToLookup(c => c.ParentId > 0 ? c.ParentId : -1);
            List<CommentDto> MapChildren(int parentId)
            {
                return lookup[parentId]
                    .OrderBy(c => c.CreatedOnDate)
                    .Select(c =>
                    {
                        var dto = MapComment(c);
                        dto.Replies = MapChildren(c.CommentID);
                        return dto;
                    })
                    .ToList();
            }
            return MapChildren(-1);
        }

        private static DateTime ToUtcOrNow(DateTime value)
        {
            if (value == DateTime.MinValue || value.Year < 1900)
                return DateTime.UtcNow;
            return value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
        }

        private static CommentDto MapComment(CommentInfo c)
        {
            var userId = c.CreatedByUserID;
            var displayName = !string.IsNullOrWhiteSpace(c.DisplayName)
                ? c.DisplayName
                : (!string.IsNullOrWhiteSpace(c.Author) ? c.Author : "Anonymous");

            return new CommentDto
            {
                CommentId = c.CommentID,
                ParentId = c.ParentId > 0 ? c.ParentId : -1,
                DisplayName = displayName,
                AuthorUserId = userId,
                ProfilePicUrl = userId > 0
                    ? DotNetNuke.Common.Globals.ResolveUrl(string.Format("~/DnnImageHandler.ashx?mode=profilepic&userId={0}&w={1}&h={1}", userId, 32))
                    : "",
                ProfileUrl = userId > 0 ? DotNetNuke.Common.Globals.UserProfileURL(userId) : "",
                CommentHtml = c.Comment ?? "",
                CreatedOnDateUtc = ToUtcOrNow(c.CreatedOnDate),
                Approved = c.Approved,
                Likes = c.Likes,
                Dislikes = c.Dislikes,
                Reports = c.Reports,
                Liked = c.Liked > 0,
                Disliked = c.Disliked > 0,
                Reported = c.Reported > 0
            };
        }

        private static bool IsFirstPingBack(PostInfo post, string sourceUrl)
        {
            foreach (var c in CoreComments.GetCommentsByContentItem(post.ContentItemId, true, -1))
            {
                if (!string.IsNullOrEmpty(c.Website) &&
                    c.Website.Equals(sourceUrl, StringComparison.OrdinalIgnoreCase))
                    return false;
            }
            return true;
        }

        private static XmlDocument RetrieveXmlDocument(HttpContext context)
        {
            var xml = ParseRequest(context);
            if (!xml.Contains("<methodName>pingback.ping</methodName>"))
                throw new InvalidOperationException("Not a pingback request");
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc;
        }

        private static string ParseRequest(HttpContext context)
        {
            var buffer = new byte[context.Request.InputStream.Length];
            context.Request.InputStream.Read(buffer, 0, buffer.Length);
            return Encoding.Default.GetString(buffer);
        }

        private static HttpResponseMessage TrackBackResponse(string status = "0")
        {
            var reply = string.Format("<?xml version=\"1.0\" encoding=\"iso-8859-1\"?><response><error>{0}</error></response>", status);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(reply, Encoding.UTF8, "application/xml")
            };
        }

        private static HttpResponseMessage PingBackSuccess()
        {
            const string success = "<methodResponse><params><param><value><string>Thanks!</string></value></param></params></methodResponse>";
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(success, Encoding.UTF8, "application/xml")
            };
        }

        private static HttpResponseMessage PingBackError(int code, string message)
        {
            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\"?>");
            sb.Append("<methodResponse><fault><value><struct>");
            sb.Append("<member><name>faultCode</name>");
            sb.AppendFormat("<value><int>{0}</int></value></member>", code);
            sb.Append("<member><name>faultString</name>");
            sb.AppendFormat("<value><string>{0}</string></value></member>", message);
            sb.Append("</struct></value></fault></methodResponse>");
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(sb.ToString(), Encoding.UTF8, "application/xml")
            };
        }

        private static string GetDomain(string sourceUrl)
        {
            var start = sourceUrl.IndexOf("://", StringComparison.Ordinal) + 3;
            var stop = sourceUrl.IndexOf("/", start, StringComparison.Ordinal);
            if (start < 3 || stop < 0)
                return sourceUrl;
            return sourceUrl.Substring(start, stop - start).Replace("www.", string.Empty);
        }

        private static void CheckSourcePage(string sourceUrl, string targetUrl, ref bool sourceContainsLink, ref string title)
        {
            var remoteFile = new WebPage(new Uri(sourceUrl));
            var html = remoteFile.GetFileAsString();
            var regexTitle = new Regex("(?<=<title.*>)([\\s\\S]*)(?=</title>)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var titleMatch = regexTitle.Match(html);
            if (titleMatch.Success)
                title = BlogGlobals.SafeString(titleMatch.Value.Trim('\r', '\n').Trim());
            html = html.ToUpperInvariant();
            targetUrl = targetUrl.ToUpperInvariant();
            sourceContainsLink = html.Contains("HREF=\"" + targetUrl + "\"") || html.Contains("HREF='" + targetUrl + "'");
        }
    }
}
