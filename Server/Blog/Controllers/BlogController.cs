using System.IO;
using System.Web.Mvc;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using DotNetNuke.Modules.Blog.Models;
using DnnHandleErrorAttribute = DotNetNuke.Web.Mvc.Framework.ActionFilters.DnnHandleErrorAttribute;

namespace DotNetNuke.Modules.Blog.Controllers
{
    [DnnHandleErrorAttribute]
    public class BlogController : BlogMvcController
    {
        [HttpGet]
        public ActionResult Export(int Blog)
        {
            var security = BlogModuleContext.Security;
            BlogModuleContext.RequirePermissionLevel(security.IsBlogger || security.IsEditor);

            var blog = BlogsController.GetBlog(Blog, User.UserID, BlogModuleContext.BlogContext.Locale);
            if (blog == null || (!security.IsEditor && blog.OwnerUserId != User.UserID))
                BlogModuleContext.ThrowAccessViolation();

            string zipPath = BlogExportHelper.ExportToZip(
                blog, BlogModuleContext.Settings, User.UserID, BlogModuleContext.BlogContext.Locale);
            if (!System.IO.File.Exists(zipPath))
                return RedirectToAction("Index", "Manage");

            return File(zipPath, "application/zip", Path.GetFileName(zipPath));
        }

        [HttpGet]
        public ActionResult Edit(int? Blog)
        {
            var security = BlogModuleContext.Security;
            BlogModuleContext.RequirePermissionLevel(security.IsBlogger || security.IsEditor);
            BlogModuleContext.EnsureInitialized();
            BlogModuleContext.AddModuleCss();

            int blogId = Blog ?? -1;
            BlogInfo blog = null;
            if (blogId > -1)
                blog = BlogsController.GetBlog(blogId, User.UserID, BlogModuleContext.BlogContext.Locale);

            if (blog != null && !security.IsEditor && blog.OwnerUserId != User.UserID)
                BlogModuleContext.ThrowAccessViolation();

            var model = blog == null ? new BlogEditViewModel { IsNew = true } : MapFromBlog(blog);
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BlogEditViewModel model)
        {
            var security = BlogModuleContext.Security;
            BlogModuleContext.RequirePermissionLevel(security.IsBlogger || security.IsEditor);

            BlogInfo blog;
            if (model.BlogId > -1)
            {
                blog = BlogsController.GetBlog(model.BlogId, User.UserID, BlogModuleContext.BlogContext.Locale);
                if (blog == null)
                    blog = new BlogInfo();
            }
            else
            {
                blog = new BlogInfo
                {
                    ModuleID = BlogModuleContext.Settings.ModuleId,
                    OwnerUserId = User.UserID
                };
            }

            ApplyModel(blog, model);
            string locale = PortalSettings.DefaultLanguage;
            blog.TitleLocalizations[locale] = blog.Title;
            blog.DescriptionLocalizations[locale] = blog.Description;

            if (blog.BlogID > -1)
                BlogsController.UpdateBlog(blog, User.UserID);
            else
                BlogsController.AddBlog(ref blog, User.UserID);

            return RedirectToAction("Index", "Manage");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int Blog)
        {
            var security = BlogModuleContext.Security;
            BlogModuleContext.RequirePermissionLevel(security.IsBlogger || security.IsEditor);

            var blog = BlogsController.GetBlog(Blog, User.UserID, BlogModuleContext.BlogContext.Locale);
            if (blog != null && (security.IsEditor || blog.OwnerUserId == User.UserID))
                BlogsController.DeleteBlog(blog.BlogID);

            return RedirectToAction("Index", "Manage");
        }

        private static BlogEditViewModel MapFromBlog(BlogInfo blog)
        {
            return new BlogEditViewModel
            {
                BlogId = blog.BlogID,
                Title = blog.Title,
                Description = blog.Description,
                ContentFormat = blog.ContentFormat,
                Published = blog.Published,
                Syndicated = blog.Syndicated,
                Copyright = blog.Copyright,
                EnablePingBackReceive = blog.EnablePingBackReceive,
                EnablePingBackSend = blog.EnablePingBackSend,
                AutoApprovePingBack = blog.AutoApprovePingBack,
                EnableTrackBackReceive = blog.EnableTrackBackReceive,
                EnableTrackBackSend = blog.EnableTrackBackSend,
                AutoApproveTrackBack = blog.AutoApproveTrackBack,
                IsNew = false
            };
        }

        private static void ApplyModel(BlogInfo blog, BlogEditViewModel model)
        {
            blog.Title = model.Title ?? "";
            blog.Description = model.Description ?? "";
            if (blog.BlogID <= -1)
                blog.ContentFormat = model.ContentFormat;
            blog.Published = model.Published;
            blog.Syndicated = model.Syndicated;
            blog.Copyright = model.Copyright ?? "";
            blog.EnablePingBackReceive = model.EnablePingBackReceive;
            blog.EnablePingBackSend = model.EnablePingBackSend;
            blog.AutoApprovePingBack = model.AutoApprovePingBack;
            blog.EnableTrackBackReceive = model.EnableTrackBackReceive;
            blog.EnableTrackBackSend = model.EnableTrackBackSend;
            blog.AutoApproveTrackBack = model.AutoApproveTrackBack;
        }
    }
}
