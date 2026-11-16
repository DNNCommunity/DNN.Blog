using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using DotNetNuke.Modules.Blog.Core.BlogML;
using DotNetNuke.Modules.Blog.Core.BlogML.Xml;
using DotNetNuke.Modules.Blog.Core.Common;
using ContentTypes = DotNetNuke.Modules.Blog.Core.BlogML.ContentTypes;
using BlogPostTypes = DotNetNuke.Modules.Blog.Core.BlogML.BlogPostTypes;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using DotNetNuke.Modules.Blog.Core.Entities.Comments;
using DotNetNuke.Modules.Blog.Core.Entities.Posts;
using DotNetNuke.Modules.Blog.Core.Entities.Terms;
using BlogGlobals = DotNetNuke.Modules.Blog.Core.Common.Globals;

namespace DotNetNuke.Modules.Blog.Common
{
    /// <summary>Builds a BlogML zip export for a blog (ported from legacy Web API Export).</summary>
    internal static class BlogExportHelper
    {
        /// <returns>Physical path of the generated .zip file.</returns>
        public static string ExportToZip(BlogInfo blog, ModuleSettings settings, int userId, string locale)
        {
            var saveDir = new DirectoryInfo(BlogGlobals.GetBlogDirectoryMapPath(blog.BlogID));
            if (!saveDir.Exists)
                saveDir.Create();
            BlogGlobals.RemoveOldTimeStampedFiles(saveDir);

            var blogMl = new BlogMLBlog
            {
                Title = blog.Title,
                SubTitle = blog.Description,
                DateCreated = blog.CreatedOnDate
            };

            foreach (var author in PostsController.GetAuthors(settings.ModuleId, blog.BlogID))
            {
                blogMl.Authors.Add(new BlogMLAuthor
                {
                    Title = author.DisplayName,
                    ID = author.UserID.ToString(),
                    Email = author.Email
                });
            }

            AddCategories(blogMl, settings, locale);
            AddPosts(blogMl, blog, settings, userId, locale);

            string fileBase = DateTime.Now.ToString("yyyy-MM-dd") + "-" + Guid.NewGuid().ToString("D");
            string zipPath = Path.Combine(saveDir.FullName, fileBase + ".zip");

            using (var zipStream = File.Create(zipPath))
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
            {
                var entry = archive.CreateEntry(fileBase + ".xml");
                using (var entryStream = entry.Open())
                using (var writer = XmlWriter.Create(entryStream))
                {
                    BlogMLSerializer.Serialize(writer, blogMl);
                    writer.Flush();
                }
            }

            return zipPath;
        }

        private static void AddCategories(BlogMLBlog target, ModuleSettings settings, string locale)
        {
            if (settings.VocabularyId <= -1)
                return;

            foreach (var c in TermsController.GetTermsByVocabulary(settings.ModuleId, settings.VocabularyId, locale).Values)
            {
                target.Categories.Add(new BlogMLCategory
                {
                    Approved = true,
                    DateCreated = c.CreatedOnDate,
                    Description = "",
                    Title = c.Name,
                    ID = c.TermId.ToString(),
                    ParentRef = c.ParentTermId.ToStringOrZero()
                });
            }
        }

        private static void AddPosts(BlogMLBlog target, BlogInfo blog, ModuleSettings settings, int userId, string locale)
        {
            int totalRecs = 0;
            int handledRecs = 0;
            int page = 0;
            do
            {
                foreach (var post in PostsController.GetPostsByBlog(
                    settings.ModuleId, blog.BlogID, locale, -1, page, 10, "PUBLISHEDONDATE", ref totalRecs).Values)
                {
                    handledRecs++;
                    target.Posts.Add(ConvertPost(post, settings, userId));
                }
                page++;
            } while (totalRecs > handledRecs);
        }

        private static BlogMLPost ConvertPost(PostInfo post, ModuleSettings settings, int userId)
        {
            var imageService = new DotNetNuke.Modules.Blog.Core.Media.ImageService();
            var exported = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);

            // Export placeholders to filename src + attachment bytes before BlogML HtmlEncode.
            string contentHtml = imageService.ExportEmbeddedImages(
                HttpUtility.HtmlDecode(post.Content ?? ""), exported);

            string excerptHtml = null;
            bool hasExcerpt = !string.IsNullOrEmpty(post.Summary);
            if (hasExcerpt)
            {
                excerptHtml = settings.SummaryModel == SummaryType.PlainTextIndependent
                    ? post.Summary
                    : imageService.ExportEmbeddedImages(HttpUtility.HtmlDecode(post.Summary), exported);
            }

            var postMl = new BlogMLPost
            {
                Approved = post.Published,
                Title = post.Title,
                Content = BlogMLContent.Create(contentHtml, ContentTypes.Html),
                PostType = BlogPostTypes.Normal,
                DateCreated = post.PublishedOnDate,
                ID = post.ContentItemId.ToString(),
                PostUrl = post.PermaLink(DotNetNuke.Entities.Portals.PortalSettings.Current),
                Image = post.Image,
                AllowComments = post.AllowComments,
                DisplayCopyright = post.DisplayCopyright,
                Copyright = post.Copyright,
                Locale = post.Locale,
                IsPublished = post.Published,
                HasExcerpt = hasExcerpt
            };

            if (post.PostCategories != null)
            {
                foreach (var t in post.PostCategories)
                    postMl.Categories.Add(new BlogMLCategoryReference { Ref = t.TermId.ToString() });
            }

            postMl.Authors.Add(post.CreatedByUserID.ToString());

            if (hasExcerpt)
            {
                postMl.Excerpt = settings.SummaryModel == SummaryType.PlainTextIndependent
                    ? BlogMLContent.Create(excerptHtml, ContentTypes.Text)
                    : BlogMLContent.Create(excerptHtml, ContentTypes.Html);
            }

            foreach (var kv in exported)
                postMl.Attachments.Add(new BlogMLAttachment { Embedded = true, Path = kv.Key, Data = kv.Value });

            string postDir = BlogGlobals.GetPostDirectoryMapPath(post.BlogID, post.ContentItemId);
            if (Directory.Exists(postDir))
            {
                foreach (string f in Directory.GetFiles(postDir))
                {
                    string fileName = Path.GetFileName(f);
                    if (exported.ContainsKey(fileName)) continue;
                    string regexPattern = "&quot;([^\\s]*)\\/" + Regex.Escape(fileName) + "&quot;";
                    var options = RegexOptions.Singleline | RegexOptions.IgnoreCase;
                    if (postMl.HasExcerpt)
                        postMl.Excerpt.Text = Regex.Replace(postMl.Excerpt.Text, regexPattern, "&quot;" + fileName + "&quot;", options);
                    postMl.Content.Text = Regex.Replace(postMl.Content.Text, regexPattern, "&quot;" + fileName + "&quot;", options);

                    var att = new BlogMLAttachment { Embedded = true, Path = fileName };
                    using (var fs = new FileStream(f, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        if (fs.Length > 0)
                        {
                            var fileData = new byte[fs.Length];
                            fs.Read(fileData, 0, fileData.Length);
                            att.Data = fileData;
                            att.Embedded = true;
                        }
                    }
                    postMl.Attachments.Add(att);
                }
            }

            foreach (var comment in CommentsController.GetCommentsByContentItem(post.ContentItemId, false, userId))
            {
                postMl.Comments.Add(new BlogMLComment
                {
                    Approved = comment.Approved,
                    Content = new BlogMLContent { Text = comment.Comment },
                    DateCreated = comment.CreatedOnDate,
                    Title = "",
                    UserEMail = comment.Email,
                    UserName = comment.Username,
                    UserUrl = comment.Website
                });
            }

            return postMl;
        }
    }
}
