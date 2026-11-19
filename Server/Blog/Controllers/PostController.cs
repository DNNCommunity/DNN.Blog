using System;
using DotNetNuke.Modules.Blog.Core.Media;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Core.Common;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using DotNetNuke.Modules.Blog.Core.Entities.Posts;
using DotNetNuke.Modules.Blog.Core.Entities.Terms;
using DotNetNuke.Modules.Blog.Models;
using DotNetNuke.Services.Localization;
using CoreTermsController = DotNetNuke.Modules.Blog.Core.Entities.Terms.TermsController;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using Microsoft.Extensions.Logging;
using BlogGlobals = DotNetNuke.Modules.Blog.Core.Common.Globals;

namespace DotNetNuke.Modules.Blog.Controllers
{
  [DnnHandleError]
  public class PostController : BlogMvcController
  {
    private readonly ILogger<PostController> _logger;

    public PostController(ILogger<PostController> logger) : base()
    {
      _logger = logger;
    }

    [HttpGet]
    public ActionResult Edit(int? Blog, int? Post, string returnUrl = null)
    {
      _logger.LogDebug("Post Edit GET: blog {Blog}, post {Post}, user {UserId}", Blog, Post, User.UserID);
      BlogModuleContext.EnsureInitialized();
      BlogModuleContext.AddModuleCss();
      BlogModuleContext.RegisterAjaxScript();
      BlogModuleContext.AddScript("postedit.js");

      var security = BlogModuleContext.Security;
      var ctx = BlogModuleContext.BlogContext;
      int blogId = Blog ?? ctx.BlogId;
      int postId = Post ?? ctx.ContentItemId;

      if (blogId <= 0)
      {
        _logger.LogInformation("Post Edit GET: no blog selected, redirecting to resolve blog");
        return RedirectToFirstBlogOrManage();
      }

      var blog = BlogsController.GetBlog(blogId, User.UserID, ctx.Locale);
      if (blog == null)
      {
        _logger.LogWarning("Post Edit GET denied: blog {BlogId} not found", blogId);
        BlogModuleContext.ThrowAccessViolation();
      }

      PostInfo post = null;
      if (postId > -1)
        post = PostsController.GetPost(postId, BlogModuleContext.Settings.ModuleId, ctx.Locale);

      if (post != null && !security.CanEditPost && !security.CanEditThisPost(post))
      {
        _logger.LogWarning("Post Edit GET denied: user {UserId} cannot edit post {PostId}", User.UserID, postId);
        BlogModuleContext.ThrowAccessViolation();
      }
      if (post == null && !security.CanAddPost && !blog.CanAdd)
      {
        _logger.LogWarning("Post Edit GET denied: user {UserId} cannot add post to blog {BlogId}", User.UserID, blogId);
        BlogModuleContext.ThrowAccessViolation();
      }

      var model = post == null
          ? NewPostModel(blog, ctx)
          : MapFromPost(blog, post, ctx);

      // Prefer the explicit action parameter (DNN can strip or mangle query keys).
      if (!string.IsNullOrWhiteSpace(returnUrl))
        model.ReturnUrl = returnUrl;
      ApplyReturnUrls(model);

      ApplyPublishSecurity(model, blog, security);
      _logger.LogInformation("Post Edit page rendered: blogId {BlogId}, postId {PostId}, isNew {IsNew}",
          model.BlogId, model.PostId, model.IsNew);
      return View(model);
    }

    [HttpPost]
    [ValidateInput(false)]
    [Web.Api.ValidateAntiForgeryToken]
    public ActionResult Edit(PostEditViewModel model, HttpPostedFileBase ImageFile)
    {
      _logger.LogDebug("Post Edit POST: blogId {BlogId}, postId {PostId}, hasImage {HasImage}, user {UserId}",
          model?.BlogId, model?.PostId, ImageFile != null && ImageFile.ContentLength > 0, User.UserID);
      BlogModuleContext.EnsureInitialized();
      BlogModuleContext.AddModuleCss();

      var security = BlogModuleContext.Security;
      var ctx = BlogModuleContext.BlogContext;
      var settings = BlogModuleContext.Settings;

      var blog = BlogsController.GetBlog(model.BlogId, User.UserID, ctx.Locale);
      if (blog == null)
      {
        _logger.LogWarning("Post Edit POST denied: blog {BlogId} not found", model.BlogId);
        BlogModuleContext.ThrowAccessViolation();
      }

      MergePostedLocalizations(model);
      var imageService = new ImageService();
      ProcessSubmittedImages(model, blog, imageService);

      var writeModel = ToWriteModel(model, ImageFile);
      var writeContext = new PostWriteContext
      {
        ModuleId = settings.ModuleId,
        PortalId = PortalSettings.PortalId,
        PortalDefaultLanguage = PortalSettings.DefaultLanguage,
        AdministratorRoleName = PortalSettings.AdministratorRoleName,
        Locale = ctx.Locale,
        UiTimeZone = ctx.UiTimeZone,
        IsMultiLingualSite = ctx.IsMultiLingualSite,
        User = User,
        PortalSettings = PortalSettings,
        Security = security,
        Blog = blog,
        Culture = CultureInfo.CurrentCulture,
        TitleRequiredMessage = LocalizeOr("Title.Required", "A title is required for the post"),
        ContentRequiredMessage = LocalizeOr("valPost.ErrorMessage", "Post is Required")
      };

      var result = new PostWriteService().Save(writeModel, writeContext);
      if (result.AccessDenied)
      {
        _logger.LogWarning("Post Edit POST denied: user {UserId} blog {BlogId} post {PostId}",
            User.UserID, model.BlogId, model.PostId);
        BlogModuleContext.ThrowAccessViolation();
      }
      if (result.NotFound)
      {
        _logger.LogWarning("Post Edit POST: post {PostId} not found", model.PostId);
        BlogModuleContext.ThrowAccessViolation();
      }
      if (!result.Success)
      {
        _logger.LogWarning("Post Edit validation failed for blog {BlogId}: {Message}", model.BlogId, result.ValidationMessage);
        model.ValidationMessage = result.ValidationMessage;
        return Redisplay(model, blog, ctx);
      }

      var post = result.Post;
      _logger.LogInformation("Saved post {PostId} on blog {BlogId} by user {UserId}", post.ContentItemId, blog.BlogID, User.UserID);

      var safeReturn = SafeReturnUrl(model.ReturnUrl)
          ?? SafeReturnUrl(Request["ReturnUrl"])
          ?? SafeReturnUrl(Request["returnUrl"]);
      if (safeReturn != null)
        return Redirect(safeReturn);

      return Redirect(DotNetNuke.Common.Globals.NavigateURL(ModuleContext.TabId, "", "Post=" + post.ContentItemId));
    }

    [HttpPost]
    [Web.Api.ValidateAntiForgeryToken]
    public ActionResult Delete(int Blog, int Post, string returnUrl = null)
    {
      _logger.LogDebug("Post Delete: blog {BlogId}, post {PostId}, user {UserId}", Blog, Post, User.UserID);
      var result = new PostWriteService().Delete(
          BlogModuleContext.Settings.ModuleId,
          Blog,
          Post,
          BlogModuleContext.BlogContext.Locale,
          User,
          BlogModuleContext.Security,
          cleanupImages: true);

      if (result.Success)
        _logger.LogInformation("Deleted post {PostId} from blog {BlogId} by user {UserId}", Post, Blog, User.UserID);
      else
        _logger.LogWarning("Post Delete skipped: post {PostId} missing or user {UserId} lacks permission", Post, User.UserID);

      var safeUrl = SafeReturnUrl(returnUrl);
      return safeUrl != null
          ? (ActionResult)Redirect(safeUrl)
          : RedirectToAction("Index", "Manage", new { section = ManageViewModel.SectionPosts });
    }

    private static PostWriteModel ToWriteModel(PostEditViewModel model, HttpPostedFileBase imageFile)
    {
      var write = new PostWriteModel
      {
        BlogId = model.BlogId,
        PostId = model.PostId,
        Title = model.Title,
        Summary = model.Summary,
        Content = model.Content,
        Titles = model.Titles,
        Summaries = model.Summaries,
        Contents = model.Contents,
        Published = model.Published,
        PublishNow = model.PublishNow,
        PublishDate = model.PublishDate,
        AllowComments = model.AllowComments,
        DisplayCopyright = model.DisplayCopyright,
        Copyright = model.Copyright,
        Locale = model.Locale,
        Tags = model.Tags,
        SelectedCategoryIds = model.SelectedCategoryIds,
        ImageKey = model.ImageKey,
        RemoveImage = model.RemoveImage,
        ContentFormat = model.ContentFormat
      };
      if (imageFile != null && imageFile.ContentLength > 0)
      {
        write.FeaturedImageStream = imageFile.InputStream;
        write.FeaturedImageFileName = imageFile.FileName;
      }
      return write;
    }

    private string ModuleHomeUrl()
    {
      return DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID);
    }

    private string SafeReturnUrl(string returnUrl)
    {
      if (string.IsNullOrWhiteSpace(returnUrl))
        return null;

      returnUrl = returnUrl.Trim();

      // Prefer an app-relative path so cancel/redirect stay local.
      if (Uri.TryCreate(returnUrl, UriKind.Absolute, out var absolute))
      {
        var host = Request?.Url?.Host;
        if (!string.IsNullOrEmpty(host)
            && string.Equals(absolute.Host, host, StringComparison.OrdinalIgnoreCase))
        {
          returnUrl = absolute.PathAndQuery;
        }
        else
        {
          return null;
        }
      }

      // DNN Url.Action often yields app-rooted paths (/blogdev/...); IsLocalUrl accepts those.
      if (Url.IsLocalUrl(returnUrl))
        return returnUrl;

      // Last resort: same-site path that simply wasn't recognized as "local".
      if (returnUrl.StartsWith("/") && !returnUrl.StartsWith("//"))
        return returnUrl;

      return null;
    }

    private void ApplyReturnUrls(PostEditViewModel model)
    {
      model.ReturnUrl = SafeReturnUrl(string.IsNullOrEmpty(model.ReturnUrl) ? Request["returnUrl"] : model.ReturnUrl) ?? "";
      if (model.ReturnUrl.Length > 0)
        model.CancelUrl = model.ReturnUrl;
      else if (model.IsNew)
        model.CancelUrl = ModuleHomeUrl();
      else
        model.CancelUrl = Url.Action("Index", "Manage", new { section = ManageViewModel.SectionPosts });
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
      return RedirectToAction("Index", "Manage", new { section = ManageViewModel.SectionPosts });
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
        IsNew = true,
        ContentFormat = blog.DefaultContentFormat == (byte)BlogContentFormat.Markdown
            ? (byte)BlogContentFormat.Markdown
            : (byte)BlogContentFormat.Html
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
        IsNew = false,
        ContentFormat = post.ContentFormat == (byte)BlogContentFormat.Markdown
            ? (byte)BlogContentFormat.Markdown
            : (byte)BlogContentFormat.Html
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
      if (model.ContentFormat != (byte)BlogContentFormat.Html
          && model.ContentFormat != (byte)BlogContentFormat.Markdown)
      {
        model.ContentFormat = blog.DefaultContentFormat == (byte)BlogContentFormat.Markdown
            ? (byte)BlogContentFormat.Markdown
            : (byte)BlogContentFormat.Html;
      }
      model.EditorMode = model.ContentFormat == (byte)BlogContentFormat.Markdown
          ? "markdown"
          : "html";
      model.UseFullLocalization = blog.FullLocalization && ctx.IsMultiLingualSite;
      model.ShowLocale = ctx.IsMultiLingualSite && !blog.FullLocalization;
      model.DefaultLocale = string.IsNullOrEmpty(blog.Locale) ? PortalSettings.DefaultLanguage : blog.Locale;
      ApplyReturnUrls(model);

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

    private static List<PostCategoryNode> BuildCategoryTree(List<TermInfo> terms, List<int> selectedIds)
    {
      return CategoryTreeHelper.BuildCategoryTree(terms, selectedIds);
    }

    private string LocalizeOr(string key, string fallback)
    {
      string resFile = "~/DesktopModules/MVC/Blog/App_LocalResources/Post.resx";
      var s = Localization.GetString(key, resFile);
      if (string.IsNullOrWhiteSpace(s))
        s = Localization.GetString(key + ".Text", resFile);
      return string.IsNullOrWhiteSpace(s) ? fallback : s;
    }
  }
}
