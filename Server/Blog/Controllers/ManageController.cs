using System.Linq;
using System.Web.Mvc;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using DotNetNuke.Modules.Blog.Models;
using DnnHandleErrorAttribute = DotNetNuke.Web.Mvc.Framework.ActionFilters.DnnHandleErrorAttribute;

namespace DotNetNuke.Modules.Blog.Controllers
{
    [DnnHandleErrorAttribute]
    public class ManageController : BlogMvcController
    {
        [HttpGet]
        public ActionResult Index()
        {
            var security = BlogModuleContext.Security;
            BlogModuleContext.RequirePermissionLevel(security.CanDoSomethingWithPosts);
            BlogModuleContext.EnsureInitialized();
            BlogModuleContext.RegisterAjaxScript();
            BlogModuleContext.AddModuleCss();
            BlogModuleContext.AddScript("blog.js");

            var ctx = BlogModuleContext.BlogContext;
            var settings = BlogModuleContext.Settings;

            System.Collections.Generic.IEnumerable<BlogInfo> blogs = BlogsController.GetBlogsByModule(settings.ModuleId, User.UserID, ctx.Locale).Values;
            if (!security.IsEditor)
                blogs = blogs.Where(b => b.OwnerUserId == User.UserID);

            var model = new ManageViewModel
            {
                Blogs = blogs.ToList(),
                ShowBlogsSection = security.IsBlogger,
                ShowPostsSection = security.CanAddPost || security.CanEditPost,
                CanAddBlog = security.IsBlogger
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApprovePost(int blogId, int postId)
        {
            var security = BlogModuleContext.Security;
            BlogModuleContext.RequirePermissionLevel(security.CanDoSomethingWithPosts);

            var post = DotNetNuke.Modules.Blog.Core.Entities.Posts.PostsController.GetPost(postId, BlogModuleContext.Settings.ModuleId, BlogModuleContext.BlogContext.Locale);
            if (post != null)
                DotNetNuke.Modules.Blog.Core.Entities.Posts.PostsController.PublishPost(post, true, User.UserID);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(int blogId, int postId)
        {
            var security = BlogModuleContext.Security;
            BlogModuleContext.RequirePermissionLevel(security.CanDoSomethingWithPosts);

            var post = DotNetNuke.Modules.Blog.Core.Entities.Posts.PostsController.GetPost(postId, BlogModuleContext.Settings.ModuleId, BlogModuleContext.BlogContext.Locale);
            if (post != null && (post.Blog?.CanEdit == true || security.CanEditThisPost(post)))
                DotNetNuke.Modules.Blog.Core.Entities.Posts.PostsController.DeletePost(post);

            return RedirectToAction("Index");
        }
    }
}
