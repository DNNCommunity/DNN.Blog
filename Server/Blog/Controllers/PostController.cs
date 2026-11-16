using System;
using DotNetNuke.Modules.Blog.Core.Media;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Core.Common;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using DotNetNuke.Modules.Blog.Core.Entities.Posts;
using DotNetNuke.Modules.Blog.Core.Entities.Terms;
using DotNetNuke.Modules.Blog.Core.Integration;
using DotNetNuke.Modules.Blog.Models;
using DotNetNuke.Services.Localization;
using CoreTermsController = DotNetNuke.Modules.Blog.Core.Entities.Terms.TermsController;
using DnnHandleErrorAttribute = DotNetNuke.Web.Mvc.Framework.ActionFilters.DnnHandleErrorAttribute;
using BlogGlobals = DotNetNuke.Modules.Blog.Core.Common.Globals;

namespace DotNetNuke.Modules.Blog.Controllers
{
    [DnnHandleError]
    public class PostController : BlogMvcController
    {
        [HttpGet]
        public ActionResult Edit(int? Blog, int? Post)
        {
            BlogModuleContext.EnsureInitialized();
            BlogModuleContext.AddModuleCss();
            BlogModuleContext.RegisterAjaxScript();
            BlogModuleContext.AddScript("postedit.js");

            var security = BlogModuleContext.Security;
            var ctx = BlogModuleContext.BlogContext;
            int blogId = Blog ?? ctx.BlogId;
            int postId = Post ?? ctx.ContentItemId;

            if (blogId <= 0)
                return RedirectToFirstBlogOrManage();

            var blog = BlogsController.GetBlog(blogId, User.UserID, ctx.Locale);
            if (blog == null)
                BlogModuleContext.ThrowAccessViolation();

            PostInfo post = null;
            if (postId > -1)
                post = PostsController.GetPost(postId, BlogModuleContext.Settings.ModuleId, ctx.Locale);

            if (post != null && !security.CanEditPost && !security.CanEditThisPost(post))
                BlogModuleContext.ThrowAccessViolation();
            if (post == null && !security.CanAddPost && !blog.CanAdd)
                BlogModuleContext.ThrowAccessViolation();

            var model = post == null
                ? NewPostModel(blog, ctx)
                : MapFromPost(blog, post, ctx);

            ApplyPublishSecurity(model, blog, security);
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PostEditViewModel model, HttpPostedFileBase ImageFile)
        {
            BlogModuleContext.EnsureInitialized();
            BlogModuleContext.AddModuleCss();

            var security = BlogModuleContext.Security;
            var ctx = BlogModuleContext.BlogContext;
            var settings = BlogModuleContext.Settings;

            var blog = BlogsController.GetBlog(model.BlogId, User.UserID, ctx.Locale);
            if (blog == null)
                BlogModuleContext.ThrowAccessViolation();

            MergePostedLocalizations(model);
            var imageService = new ImageService();
            try { imageService.PurgeTemp(TimeSpan.FromDays(7)); } catch (Exception ex) { DotNetNuke.Services.Exceptions.Exceptions.LogException(ex); }
            ProcessSubmittedImages(model, blog, imageService);

            if (string.IsNullOrWhiteSpace(model.Title))
            {
                model.ValidationMessage = LocalizeOr("Title.Required", "A title is required for the post");
                return Redisplay(model, blog, ctx);
            }
            if (string.IsNullOrWhiteSpace(StripHtmlRough(model.Content)))
            {
                model.ValidationMessage = LocalizeOr("valPost.ErrorMessage", "Post is Required");
                return Redisplay(model, blog, ctx);
            }

            PostInfo post;
            bool isNew = model.PostId <= -1;
            if (!isNew)
            {
                post = PostsController.GetPost(model.PostId, settings.ModuleId, ctx.Locale);
                if (post == null)
                    post = new PostInfo { BlogID = model.BlogId, Blog = blog };
                else if (!security.CanEditPost && !security.CanEditThisPost(post))
                    BlogModuleContext.ThrowAccessViolation();
            }
            else
            {
                if (!security.CanAddPost && !blog.CanAdd)
                    BlogModuleContext.ThrowAccessViolation();
                post = new PostInfo { BlogID = model.BlogId, Blog = blog, Published = true };
            }

            bool firstPublish = !post.Published && model.Published;

            post.Title = model.Title ?? "";
            post.Summary = model.Summary ?? "";
            post.Content = model.Content ?? "";
            post.AllowComments = model.AllowComments;
            post.DisplayCopyright = model.DisplayCopyright;
            post.Copyright = model.Copyright ?? "";

            if (ctx.IsMultiLingualSite && !blog.FullLocalization)
                post.Locale = model.Locale ?? "";
            else
                post.Locale = "";

            ApplyLocalizations(post, model, blog, PortalSettings.DefaultLanguage);

            if (model.PublishNow)
                post.PublishedOnDate = DateTime.UtcNow;
            else if (!string.IsNullOrWhiteSpace(model.PublishDate))
            {
                try
                {
                    var local = DateTime.Parse(model.PublishDate, CultureInfo.CurrentCulture);
                    post.PublishedOnDate = TimeZoneInfo.ConvertTimeToUtc(local, ctx.UiTimeZone);
                }
                catch
                {
                    post.PublishedOnDate = DateTime.UtcNow;
                }
            }

            post.Published = model.Published;
            if (blog.MustApproveGhostPosts && !security.CanApprovePost)
            {
                post.Published = false;
                firstPublish = false;
            }

            post.Terms = BuildTerms(model, settings.ModuleId, ctx.Locale);

            string savedTempFile = null;
            string savedExtension = null;
            if (model.RemoveImage)
            {
                RemovePostImages(post);
                post.Image = "";
            }
            else if (ImageFile != null && ImageFile.ContentLength > 0)
            {
                var upload = SaveUploadedImage(post, ImageFile, isNew);
                if (upload != null)
                {
                    post.Image = upload.Item1;
                    savedTempFile = upload.Item2;
                    savedExtension = upload.Item3;
                }
            }

            int publishingUserId = User.UserID;
            if (blog.PublishAsOwner)
                publishingUserId = blog.OwnerUserId;

            if (isNew)
            {
                post = PostsController.AddPost(post, publishingUserId);
                if (!string.IsNullOrEmpty(savedTempFile) && System.IO.File.Exists(savedTempFile))
                {
                    string saveDir = BlogGlobals.GetPostDirectoryMapPath(post);
                    Directory.CreateDirectory(saveDir);
                    string dest = saveDir + post.Image + savedExtension;
                    System.IO.File.Move(savedTempFile, dest);
                }
            }
            else
            {
                PostsController.UpdatePost(post, publishingUserId);
            }

            var allHtml = new List<string> { post.Content, post.Summary };
            allHtml.AddRange(post.ContentLocalizations.GetDictionary().Values);
            allHtml.AddRange(post.SummaryLocalizations.GetDictionary().Values);
            imageService.SyncPostImages(blog.BlogID, post.ContentItemId, User.UserID,
                User.IsSuperUser || User.IsInRole(PortalSettings.AdministratorRoleName), allHtml.ToArray());

            if (firstPublish)
            {
                PostsController.PublishPost(post, User.UserID);
            }
            else if (blog.MustApproveGhostPosts && User.UserID != blog.OwnerUserId)
            {
                string title = Localization.GetString("ApprovePostNotifyBody", BlogGlobals.SharedResourceFileName);
                string summary = "<a target='_blank' href='" + post.PermaLink(PortalSettings) + "'>" + post.Title + "</a>";
                NotificationController.PostPendingApproval(blog, post, PortalSettings.PortalId, summary, title);
            }

            return Redirect(DotNetNuke.Common.Globals.NavigateURL(ModuleContext.TabId, "", "Post=" + post.ContentItemId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int Blog, int Post, string returnUrl = null)
        {
            var security = BlogModuleContext.Security;
            var post = PostsController.GetPost(Post, BlogModuleContext.Settings.ModuleId, BlogModuleContext.BlogContext.Locale);
            if (post != null && (security.CanEditThisPost(post) || post.Blog?.CanEdit == true))
            {
                RemovePostImages(post);
                new ImageService().DeletePostImages(post.BlogID, post.ContentItemId);
                PostsController.DeletePost(post);
            }

            var safeUrl = SafeReturnUrl(returnUrl);
            return safeUrl != null ? (ActionResult)Redirect(safeUrl) : RedirectToAction("Index", "Manage");
        }

        private string SafeReturnUrl(string returnUrl)
        {
            return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : null;
        }

        private ActionResult Redisplay(PostEditViewModel model, BlogInfo blog, BlogContextInfo ctx)
        {
            BlogModuleContext.RegisterAjaxScript();
            BlogModuleContext.AddScript("postedit.js");
            ModelToEditor(model);
            EnrichViewModel(model, blog, ctx);
            ApplyPublishSecurity(model, blog, BlogModuleContext.Security);
            return View(model);
        }

        private ActionResult RedirectToFirstBlogOrManage()
        {
            var ctx = BlogModuleContext.BlogContext;
            int blogId = BlogNavigationHelper.ResolveBlogIdForNewPost(
                BlogModuleContext.Settings.ModuleId,
                User.UserID,
                ctx.Locale,
                ctx.BlogId,
                BlogModuleContext.Security,
                ctx.ContentItemId);
            if (blogId > 0)
                return RedirectToAction("Edit", new { Blog = blogId, returnUrl = SafeReturnUrl(Request["returnUrl"]) });
            return RedirectToAction("Index", "Manage");
        }

        private PostEditViewModel NewPostModel(BlogInfo blog, BlogContextInfo ctx)
        {
            var model = new PostEditViewModel
            {
                BlogId = blog.BlogID,
                BlogTitle = blog.LocalizedTitle,
                Published = true,
                PublishNow = true,
                PublishDate = DateTime.Now.ToString("g"),
                AllowComments = true,
                IsNew = true
            };
            EnrichViewModel(model, blog, ctx);
            return model;
        }

        private PostEditViewModel MapFromPost(BlogInfo blog, PostInfo post, BlogContextInfo ctx)
        {
            var tags = post.PostTags?.Select(t => t.Name) ?? Enumerable.Empty<string>();
            var model = new PostEditViewModel
            {
                BlogId = blog.BlogID,
                PostId = post.ContentItemId,
                BlogTitle = blog.LocalizedTitle,
                Title = HttpUtility.HtmlDecode(post.Title ?? ""),
                Summary = post.Summary ?? "",
                Content = post.Content ?? "",
                Published = post.Published,
                PublishNow = false,
                PublishDate = BlogGlobals.UtcToLocalTime(post.PublishedOnDate, ctx.UiTimeZone).ToString("g"),
                AllowComments = post.AllowComments,
                DisplayCopyright = post.DisplayCopyright,
                Copyright = post.Copyright ?? "",
                Locale = post.Locale ?? "",
                ImageKey = post.Image ?? "",
                Tags = string.Join(", ", tags),
                SelectedCategoryIds = post.PostCategories?.Select(c => c.TermId).ToList() ?? new List<int>(),
                IsNew = false
            };

            if (!string.IsNullOrEmpty(post.Image))
            {
                model.ImageUrl = VirtualPathUtility.ToAbsolute(BlogGlobals.glbImageHandlerPath)
                    + string.Format("?TabId={0}&ModuleId={1}&Blog={2}&Post={3}&w=100&h=100&c=1&key={4}",
                        ModuleContext.TabId, BlogModuleContext.Settings.ModuleId, blog.BlogID, post.ContentItemId, post.Image);
            }

            ModelToEditor(model);
            EnrichViewModel(model, blog, ctx);

            foreach (var loc in model.Locales)
            {
                if (loc.Code == model.DefaultLocale)
                    continue;
                model.Titles[loc.Code] = post.TitleLocalizations[loc.Code] ?? "";
                model.Summaries[loc.Code] = post.SummaryLocalizations[loc.Code] ?? "";
                model.Contents[loc.Code] = ImagePlaceholders.ToEditor(post.ContentLocalizations[loc.Code] ?? "", EditorImageUrl);
                model.Summaries[loc.Code] = ImagePlaceholders.ToEditor(model.Summaries[loc.Code], EditorImageUrl);
            }

            return model;
        }

        private static string EditorImageUrl(Guid id)
        {
            return ImageUrls.Build(id);
        }

        private void ModelToEditor(PostEditViewModel model)
        {
            model.Content = ImagePlaceholders.ToEditor(model.Content, EditorImageUrl);
            model.Summary = ImagePlaceholders.ToEditor(model.Summary, EditorImageUrl);
            if (model.Contents != null)
                foreach (var k in model.Contents.Keys.ToList())
                    model.Contents[k] = ImagePlaceholders.ToEditor(model.Contents[k], EditorImageUrl);
            if (model.Summaries != null)
                foreach (var k in model.Summaries.Keys.ToList())
                    model.Summaries[k] = ImagePlaceholders.ToEditor(model.Summaries[k], EditorImageUrl);
        }

        // Browser-local images arrive as files named after their pending id; temp uploads arrive as endpoint URLs.
        private void ProcessSubmittedImages(PostEditViewModel model, BlogInfo blog, ImageService svc)
        {
            var files = new List<PendingImageFile>();
            for (int i = 0; i < Request.Files.Count; i++)
            {
                if (Request.Files.AllKeys[i] != "PendingImages") continue;
                var f = Request.Files[i];
                if (f == null || f.ContentLength == 0) continue;
                files.Add(new PendingImageFile { FileName = f.FileName, Open = () => f.InputStream });
            }
            var pending = ImageService.IndexPending(files);
            Func<string, string> conv = h => svc.ProcessSubmittedHtml(h, PortalSettings.PortalId, blog.BlogID, null, User.UserID, pending);

            model.Content = conv(model.Content);
            model.Summary = conv(model.Summary);
            if (model.Contents != null)
                foreach (var k in model.Contents.Keys.ToList()) model.Contents[k] = conv(model.Contents[k]);
            if (model.Summaries != null)
                foreach (var k in model.Summaries.Keys.ToList()) model.Summaries[k] = conv(model.Summaries[k]);
        }

        private void EnrichViewModel(PostEditViewModel model, BlogInfo blog, BlogContextInfo ctx)
        {
            var settings = BlogModuleContext.Settings;
            model.BlogTitle = blog.LocalizedTitle;
            model.TimezoneDisplay = ctx.UiTimeZone.DisplayName;
            model.SummaryIsHtml = settings.SummaryModel == SummaryType.HtmlIndependent
                || settings.SummaryModel == SummaryType.HtmlPrecedesPost;
            model.ShowSummaryPrecedingWarning = settings.SummaryModel == SummaryType.HtmlPrecedesPost;
            model.UseFullLocalization = blog.FullLocalization && ctx.IsMultiLingualSite;
            model.ShowLocale = ctx.IsMultiLingualSite && !blog.FullLocalization;
            model.DefaultLocale = string.IsNullOrEmpty(blog.Locale) ? PortalSettings.DefaultLanguage : blog.Locale;
            model.ReturnUrl = SafeReturnUrl(string.IsNullOrEmpty(model.ReturnUrl) ? Request["returnUrl"] : model.ReturnUrl) ?? "";
            model.CancelUrl = model.ReturnUrl.Length > 0 ? model.ReturnUrl : Url.Action("Index", "Manage");

            var portalLocales = LocaleController.Instance.GetLocales(PortalSettings.PortalId).Values
                .OrderBy(l => l.NativeName)
                .ToList();

            model.Locales = portalLocales.Select(ToLocaleOption).ToList();
            model.OtherLocales = model.Locales.Where(l => !string.Equals(l.Code, model.DefaultLocale, StringComparison.OrdinalIgnoreCase)).ToList();

            model.LocaleDropdownOptions = new List<PostLocaleOption>
            {
                new PostLocaleOption { Code = "", Name = LocalizeOr("DefaultLocale", "< Default Blog Language >") }
            };
            model.LocaleDropdownOptions.AddRange(model.Locales);

            model.ShowCategories = settings.VocabularyId > 0;
            if (model.ShowCategories)
            {
                var vocab = CoreTermsController.GetTermsByVocabulary(settings.ModuleId, settings.VocabularyId, ctx.Locale);
                // Keep hierarchy for React category picker (dynatree-style)
                model.Categories = BuildCategoryTree(
                    vocab.Values.ToList(), model.SelectedCategoryIds ?? new List<int>());
            }

            if (model.Titles == null) model.Titles = new Dictionary<string, string>();
            if (model.Summaries == null) model.Summaries = new Dictionary<string, string>();
            if (model.Contents == null) model.Contents = new Dictionary<string, string>();
            foreach (var loc in model.OtherLocales)
            {
                if (!model.Titles.ContainsKey(loc.Code)) model.Titles[loc.Code] = "";
                if (!model.Summaries.ContainsKey(loc.Code)) model.Summaries[loc.Code] = "";
                if (!model.Contents.ContainsKey(loc.Code)) model.Contents[loc.Code] = "";
            }
        }

        private static void ApplyPublishSecurity(PostEditViewModel model, BlogInfo blog, Core.Security.ContextSecurity security)
        {
            if (blog.MustApproveGhostPosts && !security.CanApprovePost)
            {
                model.Published = false;
                model.PublishCanChange = false;
            }
        }

        private static PostLocaleOption ToLocaleOption(Locale locale)
        {
            string flagPath = DotNetNuke.Common.Globals.ResolveUrl("~/images/flags/" + locale.Code + ".gif");
            return new PostLocaleOption
            {
                Code = locale.Code,
                Name = locale.NativeName,
                FlagUrl = flagPath
            };
        }

        private void MergePostedLocalizations(PostEditViewModel model)
        {
            if (model.Titles == null) model.Titles = new Dictionary<string, string>();
            if (model.Summaries == null) model.Summaries = new Dictionary<string, string>();
            if (model.Contents == null) model.Contents = new Dictionary<string, string>();

            // Form may post Contents[code] via Request when model binder misses nested keys
            foreach (string key in Request.Form.AllKeys ?? Array.Empty<string>())
            {
                if (key == null) continue;
                MergeDictFromForm(model.Titles, key, "Titles[");
                MergeDictFromForm(model.Summaries, key, "Summaries[");
                MergeDictFromForm(model.Contents, key, "Contents[");
            }
        }

        private void MergeDictFromForm(Dictionary<string, string> dict, string formKey, string prefix)
        {
            if (!formKey.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) || !formKey.EndsWith("]", StringComparison.Ordinal))
                return;
            string code = formKey.Substring(prefix.Length, formKey.Length - prefix.Length - 1);
            dict[code] = Request.Form[formKey] ?? "";
        }

        private static void ApplyLocalizations(PostInfo post, PostEditViewModel model, BlogInfo blog, string portalDefaultLanguage)
        {
            string defaultLocale = string.IsNullOrEmpty(blog.Locale) ? portalDefaultLanguage : blog.Locale;
            post.TitleLocalizations = new LocalizedText();
            post.SummaryLocalizations = new LocalizedText();
            post.ContentLocalizations = new LocalizedText();

            post.TitleLocalizations[defaultLocale] = post.Title;
            post.SummaryLocalizations[defaultLocale] = post.Summary;
            post.ContentLocalizations[defaultLocale] = post.Content;

            if (!blog.FullLocalization)
                return;

            if (model.Titles != null)
            {
                foreach (var kv in model.Titles)
                    if (!string.IsNullOrEmpty(kv.Key)) post.TitleLocalizations[kv.Key] = kv.Value ?? "";
            }
            if (model.Summaries != null)
            {
                foreach (var kv in model.Summaries)
                    if (!string.IsNullOrEmpty(kv.Key)) post.SummaryLocalizations[kv.Key] = kv.Value ?? "";
            }
            if (model.Contents != null)
            {
                foreach (var kv in model.Contents)
                    if (!string.IsNullOrEmpty(kv.Key)) post.ContentLocalizations[kv.Key] = kv.Value ?? "";
            }
        }

        private static List<TermInfo> BuildTerms(PostEditViewModel model, int moduleId, string locale)
        {
            var terms = new List<TermInfo>();
            if (!string.IsNullOrWhiteSpace(model.Tags))
            {
                var tagNames = model.Tags.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).ToList();
                terms.AddRange(CoreTermsController.GetTermList(moduleId, tagNames, 1, true, locale));
            }

            var settings = ModuleSettings.GetSettings(moduleId);
            if (settings.VocabularyId > -1 && model.SelectedCategoryIds != null && model.SelectedCategoryIds.Count > 0)
            {
                var vocab = CoreTermsController.GetTermsByVocabulary(moduleId, settings.VocabularyId);
                foreach (int id in model.SelectedCategoryIds.Distinct())
                {
                    if (vocab.TryGetValue(id, out var term))
                        terms.Add(term);
                }
            }
            return terms;
        }

        private static List<PostCategoryNode> BuildCategoryTree(List<TermInfo> terms, List<int> selectedIds)
        {
            var roots = terms.Where(t => t.ParentTermId == null || t.ParentTermId == 0).OrderBy(t => t.Name).ToList();
            var result = new List<PostCategoryNode>();
            foreach (var root in roots)
                result.Add(BuildCategoryNode(root, terms, selectedIds, 0));
            // Orphans with missing parents
            var included = new HashSet<int>(FlattenIds(result));
            foreach (var t in terms.Where(t => !included.Contains(t.TermId)).OrderBy(t => t.Name))
                result.Add(BuildCategoryNode(t, terms, selectedIds, 0));
            return result;
        }

        private static PostCategoryNode BuildCategoryNode(TermInfo term, List<TermInfo> all, List<int> selectedIds, int depth)
        {
            var node = new PostCategoryNode
            {
                TermId = term.TermId,
                Name = term.Name,
                Depth = depth,
                Selected = selectedIds.Contains(term.TermId)
            };
            foreach (var child in all.Where(t => t.ParentTermId.HasValue && t.ParentTermId.Value == term.TermId).OrderBy(t => t.Name))
                node.Children.Add(BuildCategoryNode(child, all, selectedIds, depth + 1));
            return node;
        }

        private static IEnumerable<int> FlattenIds(IEnumerable<PostCategoryNode> nodes)
        {
            foreach (var n in nodes)
            {
                yield return n.TermId;
                foreach (var c in FlattenIds(n.Children))
                    yield return c;
            }
        }

        private static List<PostCategoryNode> FlattenCategoryTree(List<PostCategoryNode> roots)
        {
            var flat = new List<PostCategoryNode>();
            void Walk(IEnumerable<PostCategoryNode> nodes)
            {
                foreach (var n in nodes)
                {
                    flat.Add(new PostCategoryNode
                    {
                        TermId = n.TermId,
                        Name = n.Name,
                        Depth = n.Depth,
                        Selected = n.Selected
                    });
                    if (n.Children != null && n.Children.Count > 0)
                        Walk(n.Children);
                }
            }
            Walk(roots);
            return flat;
        }

        private Tuple<string, string, string> SaveUploadedImage(PostInfo post, HttpPostedFileBase file, bool isNew)
        {
            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (BlogGlobals.glbPermittedFileExtensions.IndexOf(extension + ",", StringComparison.OrdinalIgnoreCase) < 0)
                return null;

            string saveDir = isNew || post.ContentItemId <= 0
                ? BlogGlobals.GetTempPostDirectoryMapPath(post.BlogID)
                : BlogGlobals.GetPostDirectoryMapPath(post);
            Directory.CreateDirectory(saveDir);

            if (!string.IsNullOrEmpty(post.Image) && !isNew && post.ContentItemId > 0)
                DeleteImagesWithKey(BlogGlobals.GetPostDirectoryMapPath(post), post.Image);

            string newFileName = Guid.NewGuid().ToString("D");
            string savedFile = saveDir + newFileName + extension;
            file.SaveAs(savedFile);
            return Tuple.Create(newFileName, savedFile, extension);
        }

        private static void RemovePostImages(PostInfo post)
        {
            if (post == null || string.IsNullOrEmpty(post.Image) || post.ContentItemId <= 0)
                return;
            DeleteImagesWithKey(BlogGlobals.GetPostDirectoryMapPath(post), post.Image);
        }

        private static void DeleteImagesWithKey(string saveDir, string imageKey)
        {
            if (!Directory.Exists(saveDir) || string.IsNullOrEmpty(imageKey))
                return;
            var d = new DirectoryInfo(saveDir);
            foreach (var f in d.GetFiles().Where(f => f.Name.StartsWith(imageKey, StringComparison.OrdinalIgnoreCase)))
            {
                try { f.Delete(); } catch { /* ignore locked files */ }
            }
        }

        private string LocalizeOr(string key, string fallback)
        {
            string resFile = "~/DesktopModules/MVC/Blog/App_LocalResources/Post.resx";
            var s = Localization.GetString(key, resFile);
            if (string.IsNullOrWhiteSpace(s))
                s = Localization.GetString(key + ".Text", resFile);
            return string.IsNullOrWhiteSpace(s) ? fallback : s;
        }

        private static string StripHtmlRough(string html)
        {
            if (string.IsNullOrEmpty(html)) return "";
            return System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", "").Trim();
        }
    }
}
