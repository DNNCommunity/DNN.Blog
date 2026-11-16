using System.Web.Mvc;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using DotNetNuke.Modules.Blog.Core.Entities.Comments;
using DotNetNuke.Modules.Blog.Core.Entities.Posts;
using DnnHandleErrorAttribute = DotNetNuke.Web.Mvc.Framework.ActionFilters.DnnHandleErrorAttribute;

namespace DotNetNuke.Modules.Blog.Controllers
{
    [DnnHandleErrorAttribute]
    public class CommentController : BlogMvcController
    {
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Add(int postId, int blogId, string comment)
        {
            var security = BlogModuleContext.Security;
            BlogModuleContext.RequirePermissionLevel(security.CanAddComment);

            var blog = BlogsController.GetBlog(blogId, User.UserID, BlogModuleContext.BlogContext.Locale);
            var post = PostsController.GetPost(postId, BlogModuleContext.Settings.ModuleId, BlogModuleContext.BlogContext.Locale);
            if (blog == null || post == null)
                BlogModuleContext.ThrowAccessViolation();

            var objComment = new CommentInfo
            {
                ContentItemId = postId,
                Comment = comment ?? "",
                Author = User.DisplayName,
                Email = User.Email,
                Website = User.Profile?.Website ?? "",
                Approved = security.CanAutoApproveComment || security.CanApproveComment || post.CreatedByUserID == User.UserID
            };

            CommentsController.AddComment(blog, post, ref objComment);

            return Redirect(DotNetNuke.Common.Globals.NavigateURL(
                ModuleContext.TabId,
                "",
                "Blog=" + blogId,
                "Post=" + postId));
        }
    }
}
