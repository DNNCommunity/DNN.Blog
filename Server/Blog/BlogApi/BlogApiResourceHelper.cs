using System;
using System.Linq;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Modules.Blog.Core.Common;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using DotNetNuke.Modules.Blog.Core.Security;
using DotNetNuke.Services.Localization;
using CoreBlogs = DotNetNuke.Modules.Blog.Core.Entities.Blogs.BlogsController;

namespace DotNetNuke.Modules.Blog.BlogApi
{
  public class BlogApiResolvedContext
  {
    public ModuleInfo Module { get; set; }
    public ModuleSettings Settings { get; set; }
    public BlogInfo Blog { get; set; }
    public ContextSecurity Security { get; set; }
    public string Locale { get; set; }
    public bool IsMultiLingualSite { get; set; }
    public TimeZoneInfo UiTimeZone { get; set; }
  }

  public static class BlogApiResourceHelper
  {
    public static ModuleInfo GetModuleInPortal(int moduleId, int portalId)
    {
      if (moduleId < 1)
        return null;

      var module = ModuleController.Instance.GetModule(moduleId, -1, true);
      if (module == null || module.PortalID != portalId || module.IsDeleted)
        return null;
      return module;
    }

    public static BlogApiResolvedContext Resolve(
        int moduleId,
        int? blogId,
        PortalSettings portalSettings,
        UserInfo user,
        string locale = null)
    {
      var module = GetModuleInPortal(moduleId, portalSettings.PortalId);
      if (module == null)
        return null;

      if (string.IsNullOrEmpty(locale))
        locale = portalSettings.DefaultLanguage;

      var settings = ModuleSettings.GetSettings(module);
      BlogInfo blog = null;
      if (blogId.HasValue && blogId.Value > 0)
      {
        blog = CoreBlogs.GetBlog(blogId.Value, user.UserID, locale);
        if (blog == null || blog.ModuleID != moduleId)
          return null;
      }

      var security = new ContextSecurity(module.ModuleID, module.TabID, blog, user);
      var locales = LocaleController.Instance.GetLocales(portalSettings.PortalId);
      var uiTimeZone = portalSettings.TimeZone ?? TimeZoneInfo.Local;
      if (user?.Profile?.PreferredTimeZone != null)
        uiTimeZone = user.Profile.PreferredTimeZone;

      return new BlogApiResolvedContext
      {
        Module = module,
        Settings = settings,
        Blog = blog,
        Security = security,
        Locale = locale,
        IsMultiLingualSite = locales != null && locales.Count > 1,
        UiTimeZone = uiTimeZone
      };
    }

    public static bool UserCanAccessModuleBlogs(int moduleId, int portalId, int userId, string locale)
    {
      var blogs = CoreBlogs.GetBlogsByModule(moduleId, userId, locale).Values;
      return blogs.Any(b => b.CanAdd || b.CanEdit || b.CanApprove || b.IsOwner || b.OwnerUserId == userId);
    }

    public static bool UserCanEditBlog(BlogApiResolvedContext ctx)
    {
      if (ctx?.Security == null || ctx.Blog == null)
        return false;
      return ctx.Security.CanAddPost
          || ctx.Security.CanEditPost
          || ctx.Blog.CanAdd
          || ctx.Blog.CanEdit
          || ctx.Security.IsEditor
          || ctx.Security.IsBlogger;
    }
  }
}
