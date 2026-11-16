using System.Linq;
using System.Web.Mvc;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using DotNetNuke.Modules.Blog.Models;
using DotNetNuke.Web.Api.Auth.ApiTokens;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using Microsoft.Extensions.Logging;

namespace DotNetNuke.Modules.Blog.Controllers
{
  [DnnHandleError]
  public class ManageController : BlogMvcController
  {
    private readonly ILogger<ManageController> _logger;
    private readonly ApiTokenSettingsRepository _apiTokenSettingsRepository;

    public ManageController(ILogger<ManageController> logger, ApiTokenSettingsRepository apiTokenSettingsRepository) : base()
    {
      _logger = logger;
      _apiTokenSettingsRepository = apiTokenSettingsRepository;
    }

    [HttpGet]
    public ActionResult Index(string section = null)
    {
      _logger.LogDebug("Manage Index requested: section {Section}, user {UserId}", section, User.UserID);
      var security = BlogModuleContext.Security;
      BlogModuleContext.RequirePermissionLevel(security.CanDoSomethingWithPosts);
      BlogModuleContext.EnsureInitialized();
      BlogModuleContext.RegisterAjaxScript();
      BlogModuleContext.AddModuleCss();

      var ctx = BlogModuleContext.BlogContext;
      var settings = BlogModuleContext.Settings;

      System.Collections.Generic.IEnumerable<BlogInfo> blogs = BlogsController.GetBlogsByModule(settings.ModuleId, User.UserID, ctx.Locale).Values;
      if (!security.IsEditor)
        blogs = blogs.Where(b => b.OwnerUserId == User.UserID);

      bool showBlogs = security.IsBlogger;
      bool showPosts = security.CanAddPost || security.CanEditPost;
      var tokenSettings = ApiTokenSettings.GetSettings(_apiTokenSettingsRepository, PortalSettings.PortalId);
      bool showApiTokens = tokenSettings.ApiTokensEnabled && tokenSettings.AllowApiTokens;
      string activeSection = NormalizeSection(section, showPosts, showBlogs, showApiTokens);

      if (showPosts && activeSection == ManageViewModel.SectionPosts)
        BlogModuleContext.AddScript("blog.js");
      if (showApiTokens && activeSection == ManageViewModel.SectionApiTokens)
        BlogModuleContext.AddScript("apitokens.js");

      var blogList = blogs.ToList();
      var model = new ManageViewModel
      {
        Blogs = blogList,
        ShowBlogsSection = showBlogs,
        ShowPostsSection = showPosts,
        ShowApiTokensSection = showApiTokens,
        CanAddBlog = security.IsBlogger,
        ActiveSection = activeSection
      };

      _logger.LogInformation("Manage page rendered: section {Section}, blogCount {BlogCount}, user {UserId}",
          activeSection, blogList.Count, User.UserID);
      return View(model);
    }

    [HttpPost]
    [Web.Api.ValidateAntiForgeryToken]
    public ActionResult ApprovePost(int blogId, int postId)
    {
      _logger.LogDebug("ApprovePost: blogId {BlogId}, postId {PostId}, user {UserId}", blogId, postId, User.UserID);
      var security = BlogModuleContext.Security;
      BlogModuleContext.RequirePermissionLevel(security.CanDoSomethingWithPosts);

      var post = DotNetNuke.Modules.Blog.Core.Entities.Posts.PostsController.GetPost(postId, BlogModuleContext.Settings.ModuleId, BlogModuleContext.BlogContext.Locale);
      if (post != null)
      {
        DotNetNuke.Modules.Blog.Core.Entities.Posts.PostsController.PublishPost(post, true, User.UserID);
        _logger.LogInformation("Approved post {PostId} on blog {BlogId} by user {UserId}", postId, blogId, User.UserID);
      }
      else
      {
        _logger.LogWarning("ApprovePost skipped: post {PostId} not found", postId);
      }

      return RedirectToManagePosts();
    }

    [HttpPost]
    [Web.Api.ValidateAntiForgeryToken]
    public ActionResult DeletePost(int blogId, int postId)
    {
      _logger.LogDebug("DeletePost: blogId {BlogId}, postId {PostId}, user {UserId}", blogId, postId, User.UserID);
      var security = BlogModuleContext.Security;
      BlogModuleContext.RequirePermissionLevel(security.CanDoSomethingWithPosts);

      var post = DotNetNuke.Modules.Blog.Core.Entities.Posts.PostsController.GetPost(postId, BlogModuleContext.Settings.ModuleId, BlogModuleContext.BlogContext.Locale);
      if (post != null && (post.Blog?.CanEdit == true || security.CanEditThisPost(post)))
      {
        DotNetNuke.Modules.Blog.Core.Entities.Posts.PostsController.DeletePost(post);
        _logger.LogInformation("Deleted post {PostId} from blog {BlogId} by user {UserId}", postId, blogId, User.UserID);
      }
      else
      {
        _logger.LogWarning("DeletePost skipped: post {PostId} missing or user {UserId} lacks permission", postId, User.UserID);
      }

      return RedirectToManagePosts();
    }

    private ActionResult RedirectToManagePosts()
    {
      var baseUrl = Url.Action("Index", "Manage") ?? "";
      if (baseUrl.Length == 0)
        return RedirectToAction("Index");
      var sep = baseUrl.IndexOf('?') >= 0 ? "&" : "?";
      return Redirect(baseUrl + sep + "section=" + ManageViewModel.SectionPosts);
    }

    private static string NormalizeSection(string section, bool showPosts, bool showBlogs, bool showApiTokens)
    {
      if (string.Equals(section, ManageViewModel.SectionApiTokens, System.StringComparison.OrdinalIgnoreCase) && showApiTokens)
        return ManageViewModel.SectionApiTokens;
      if (string.Equals(section, ManageViewModel.SectionBlogs, System.StringComparison.OrdinalIgnoreCase) && showBlogs)
        return ManageViewModel.SectionBlogs;
      if (showPosts)
        return ManageViewModel.SectionPosts;
      if (showBlogs)
        return ManageViewModel.SectionBlogs;
      if (showApiTokens)
        return ManageViewModel.SectionApiTokens;
      return ManageViewModel.SectionPosts;
    }
  }
}
