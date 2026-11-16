using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Core.BlogML.Xml;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using DotNetNuke.Modules.Blog.Core.Entities.Posts;
using DotNetNuke.Modules.Blog.Core.Media;
using DotNetNuke.Modules.Blog.Models;
using BlogGlobals = DotNetNuke.Modules.Blog.Core.Common.Globals;
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
                BlogTitle = blog?.LocalizedTitle ?? ""
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(int Blog, HttpPostedFileBase uploadFile, bool confirmImport = false)
        {
            BlogModuleContext.RequirePermissionLevel(BlogModuleContext.Security.IsBlogger);

            var blog = BlogsController.GetBlog(Blog, User.UserID, BlogModuleContext.BlogContext.Locale);
            if (blog == null)
                BlogModuleContext.ThrowAccessViolation();

            var model = new ImportViewModel
            {
                BlogId = Blog,
                BlogTitle = blog.LocalizedTitle
            };

            // Use the posted Blog id — BlogContext may still be -1 on Manage→Import routes.
            string importPath = Path.Combine(BlogGlobals.GetBlogDirectoryMapPath(Blog), "import.resources");

            if (uploadFile != null && uploadFile.ContentLength > 0)
            {
                string uploadError;
                if (!SaveUpload(uploadFile, importPath, out uploadError))
                {
                    model.Message = uploadError;
                    model.MessageIsError = true;
                    return View(model);
                }
            }
            else if (!confirmImport || !System.IO.File.Exists(importPath))
            {
                model.Message = "Please select a BlogML (.xml) or BlogML export (.zip) file.";
                model.MessageIsError = true;
                return View(model);
            }

            BlogMLBlog blogMl;
            try
            {
                using (var reader = new StreamReader(importPath))
                using (var xml = new System.Xml.XmlTextReader(reader))
                {
                    blogMl = BlogMLSerializer.Deserialize(xml);
                }
            }
            catch (Exception)
            {
                model.Message = "The uploaded file could not be read as a BlogML document.";
                model.MessageIsError = true;
                return View(model);
            }

            if (blogMl == null)
            {
                model.Message = "The uploaded file is not a valid BlogML document.";
                model.MessageIsError = true;
                return View(model);
            }

            if (!confirmImport)
            {
                model.OriginalTitle = blogMl.Title ?? "";
                model.PostCount = blogMl.Posts?.Count ?? 0;
                model.CategoryCount = blogMl.Categories?.Count ?? 0;
                model.AttachmentCount = CountAttachments(blogMl);
                model.ImportStep = true;
                model.Message = "Review the analysis, then confirm import to create posts.";
                return View(model);
            }

            int imported = 0;
            var imageService = new ImageService();
            if (blogMl.Posts != null)
            {
                foreach (var p in blogMl.Posts)
                {
                    if (p == null || string.IsNullOrEmpty(p.Title) || p.Content == null)
                        continue;

                    var content = p.Content.UncodedText ?? "";
                    var summary = p.Excerpt?.UncodedText ?? "";
                    if (string.IsNullOrEmpty(content))
                        continue;

                    var files = BuildAttachmentMap(p);
                    content = imageService.ImportEmbeddedImages(content, files, PortalSettings.PortalId, blog.BlogID, User.UserID);
                    summary = imageService.ImportEmbeddedImages(summary, files, PortalSettings.PortalId, blog.BlogID, User.UserID);

                    var newPost = new PostInfo
                    {
                        BlogID = blog.BlogID,
                        ViewCount = 0,
                        Title = p.Title,
                        Content = content,
                        Summary = summary,
                        Published = p.Approved,
                        PublishedOnDate = p.DateCreated,
                        Image = p.Image ?? "",
                        AllowComments = p.AllowComments,
                        DisplayCopyright = p.DisplayCopyright,
                        Copyright = p.Copyright ?? "",
                        Locale = p.Locale ?? ""
                    };

                    newPost = PostsController.AddPost(newPost, User.UserID);
                    imageService.SyncPostImages(blog.BlogID, newPost.ContentItemId, User.UserID, true, newPost.Content, newPost.Summary);
                    ImportMainImage(newPost, p.Image, files);
                    imported++;
                }
            }

            try { System.IO.File.Delete(importPath); } catch { }

            model.ImportComplete = true;
            model.Message = $"Import complete. Added {imported} post(s).";
            return View(model);
        }

        private static int CountAttachments(BlogMLBlog blogMl)
        {
            if (blogMl?.Posts == null) return 0;
            int count = 0;
            foreach (var p in blogMl.Posts)
            {
                if (p?.Attachments == null) continue;
                count += p.Attachments.Count;
            }
            return count;
        }

        private static Dictionary<string, byte[]> BuildAttachmentMap(BlogMLPost post)
        {
            var files = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
            if (post?.Attachments == null) return files;
            foreach (BlogMLAttachment att in post.Attachments)
            {
                if (att == null || att.Data == null || string.IsNullOrEmpty(att.Path)) continue;
                files[att.Path] = att.Data;
            }
            return files;
        }

        /// <summary>
        /// Writes the featured image attachment as {Image}{ext} under the post folder.
        /// Matches basename exactly so resized variants ({guid}-100-100-True.jpg) are skipped.
        /// </summary>
        private static void ImportMainImage(PostInfo post, string imageKey, IDictionary<string, byte[]> files)
        {
            if (post == null || string.IsNullOrEmpty(imageKey) || files == null || files.Count == 0)
                return;

            foreach (var kv in files)
            {
                var fileName = Path.GetFileName(kv.Key);
                if (string.IsNullOrEmpty(fileName) || kv.Value == null || kv.Value.Length == 0)
                    continue;
                if (!string.Equals(Path.GetFileNameWithoutExtension(fileName), imageKey, StringComparison.OrdinalIgnoreCase))
                    continue;

                var ext = Path.GetExtension(fileName);
                if (string.IsNullOrEmpty(ext))
                    continue;

                string postDir = BlogGlobals.GetPostDirectoryMapPath(post);
                Directory.CreateDirectory(postDir);
                System.IO.File.WriteAllBytes(postDir + imageKey + ext, kv.Value);
                return;
            }
        }

        private static bool SaveUpload(HttpPostedFileBase file, string importPath, out string error)
        {
            error = null;
            Directory.CreateDirectory(Path.GetDirectoryName(importPath));

            string name = (file.FileName ?? "").ToLowerInvariant();
            if (name.EndsWith(".zip"))
            {
                string tempZip = importPath + ".zip";
                try
                {
                    file.SaveAs(tempZip);
                    using (var archive = ZipFile.OpenRead(tempZip))
                    {
                        var entry = archive.Entries.FirstOrDefault(e =>
                            !string.IsNullOrEmpty(e.Name)
                            && e.Name.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)
                            && e.Length > 0);
                        if (entry == null)
                        {
                            error = "The ZIP archive does not contain a BlogML .xml file.";
                            return false;
                        }
                        if (System.IO.File.Exists(importPath))
                            System.IO.File.Delete(importPath);
                        entry.ExtractToFile(importPath, true);
                    }
                    return true;
                }
                catch (InvalidDataException)
                {
                    error = "The uploaded ZIP file is invalid or corrupt.";
                    return false;
                }
                catch (Exception)
                {
                    error = "Could not extract a BlogML .xml file from the uploaded ZIP.";
                    return false;
                }
                finally
                {
                    try { if (System.IO.File.Exists(tempZip)) System.IO.File.Delete(tempZip); } catch { }
                }
            }

            if (!name.EndsWith(".xml") && !string.IsNullOrEmpty(Path.GetExtension(name)))
            {
                error = "Please upload a BlogML .xml file or a .zip export containing one.";
                return false;
            }

            file.SaveAs(importPath);
            return true;
        }
    }
}
