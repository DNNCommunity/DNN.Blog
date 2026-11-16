using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Core.BlogML.Xml;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using DotNetNuke.Modules.Blog.Core.Entities.Posts;
using DotNetNuke.Modules.Blog.Models;
using DnnHandleErrorAttribute = DotNetNuke.Web.Mvc.Framework.ActionFilters.DnnHandleErrorAttribute;

namespace DotNetNuke.Modules.Blog.Controllers
{
    [DnnHandleErrorAttribute]
    public class ImportController : BlogMvcController
    {
        [HttpGet]
        public ActionResult Index(int? Blog)
        {
            BlogModuleContext.RequirePermissionLevel(BlogModuleContext.Security.IsBlogger);
            int blogId = Blog ?? BlogModuleContext.BlogContext.BlogId;
            var blog = blogId > -1
                ? BlogsController.GetBlog(blogId, User.UserID, BlogModuleContext.BlogContext.Locale)
                : null;

            return View(new ImportViewModel
            {
                BlogId = blogId,
                BlogTitle = blog?.LocalizedTitle ?? "",
                CanImportCategories = BlogModuleContext.Settings.VocabularyId > -1 && BlogModuleContext.Security.IsEditor
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(int Blog, HttpPostedFileBase uploadFile, bool importCategories = false, bool confirmImport = false)
        {
            BlogModuleContext.RequirePermissionLevel(BlogModuleContext.Security.IsBlogger);

            var blog = BlogsController.GetBlog(Blog, User.UserID, BlogModuleContext.BlogContext.Locale);
            if (blog == null)
                BlogModuleContext.ThrowAccessViolation();

            var model = new ImportViewModel
            {
                BlogId = Blog,
                BlogTitle = blog.LocalizedTitle,
                CanImportCategories = BlogModuleContext.Settings.VocabularyId > -1 && BlogModuleContext.Security.IsEditor,
                ImportCategories = importCategories
            };

            // Use the posted Blog id — BlogContext may still be -1 on Manage→Import routes.
            string importPath = Path.Combine(
                DotNetNuke.Modules.Blog.Core.Common.Globals.GetBlogDirectoryMapPath(Blog),
                "import.resources");

            if (uploadFile != null && uploadFile.ContentLength > 0)
            {
                SaveUpload(uploadFile, importPath);
            }
            else if (!confirmImport || !System.IO.File.Exists(importPath))
            {
                model.Message = "Please select a BlogML (.xml) or BlogML export (.zip) file.";
                return View(model);
            }

            BlogMLBlog blogMl;
            using (var reader = new StreamReader(importPath))
            using (var xml = new System.Xml.XmlTextReader(reader))
            {
                blogMl = BlogMLSerializer.Deserialize(xml);
            }

            if (blogMl == null)
            {
                model.Message = "The uploaded file is not a valid BlogML document.";
                return View(model);
            }

            if (!confirmImport)
            {
                var analysis = new StringBuilder();
                analysis.AppendLine("File is a valid BlogML file.");
                analysis.AppendFormat("Original Title: {0}\r\n", blogMl.Title);
                analysis.AppendFormat("Total Posts: {0}\r\n", blogMl.Posts?.Count ?? 0);
                analysis.AppendFormat("Total Categories: {0}\r\n", blogMl.Categories?.Count ?? 0);
                model.Analysis = analysis.ToString();
                model.ImportStep = true;
                model.Message = "Review the analysis, then confirm import to create posts.";
                return View(model);
            }

            int imported = 0;
            if (blogMl.Posts != null)
            {
                foreach (var p in blogMl.Posts)
                {
                    if (p == null || string.IsNullOrEmpty(p.Title) || p.Content == null)
                        continue;
                    var newPost = new PostInfo
                    {
                        BlogID = blog.BlogID,
                        ViewCount = 0,
                        Title = p.Title,
                        Content = p.Content.Text ?? "",
                        Summary = p.Excerpt?.Text ?? "",
                        Published = p.Approved,
                        PublishedOnDate = p.DateCreated
                    };
                    if (!string.IsNullOrEmpty(newPost.Title) && !string.IsNullOrEmpty(newPost.Content))
                    {
                        var files = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
                        if (p.Attachments != null)
                            foreach (BlogMLAttachment att in p.Attachments)
                                if (att != null && att.Data != null && !string.IsNullOrEmpty(att.Path)) files[att.Path] = att.Data;
                        var imageService = new DotNetNuke.Modules.Blog.Core.Media.ImageService();
                        newPost.Content = imageService.ImportEmbeddedImages(newPost.Content, files, PortalSettings.PortalId, blog.BlogID, User.UserID);
                        newPost.Summary = imageService.ImportEmbeddedImages(newPost.Summary, files, PortalSettings.PortalId, blog.BlogID, User.UserID);
                        newPost = PostsController.AddPost(newPost, User.UserID);
                        imageService.SyncPostImages(blog.BlogID, newPost.ContentItemId, User.UserID, true, newPost.Content, newPost.Summary);
                        imported++;
                    }
                }
            }

            try { System.IO.File.Delete(importPath); } catch { }

            model.Message = $"Import complete. Added {imported} post(s). Category import from BlogML is not yet implemented in MVC.";
            return View(model);
        }

        private static void SaveUpload(HttpPostedFileBase file, string importPath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(importPath));
            if (file.FileName.ToLower().EndsWith(".zip"))
            {
                throw new System.InvalidOperationException("ZIP BlogML upload requires System.IO.Compression; upload the extracted .xml file for now.");
            }
            file.SaveAs(importPath);
        }
    }
}
