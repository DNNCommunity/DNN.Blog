using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using DotNetNuke.Modules.Blog.Core.Security.Permissions;
using DotNetNuke.Modules.Blog.Models;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using Microsoft.Extensions.Logging;
using BlogGlobals = DotNetNuke.Modules.Blog.Core.Common.Globals;

namespace DotNetNuke.Modules.Blog.Controllers
{
  [DnnHandleError]
  public class BlogController : BlogMvcController
  {
    private readonly ILogger<BlogController> _logger;

    public BlogController(ILogger<BlogController> logger) : base()
    {
      _logger = logger;
    }

    [HttpGet]
    public ActionResult Export(int Blog)
    {
      _logger.LogDebug("Blog Export requested: blogId {BlogId}, user {UserId}", Blog, User.UserID);
      var security = BlogModuleContext.Security;
      BlogModuleContext.RequirePermissionLevel(security.IsBlogger || security.IsEditor);

      var blog = BlogsController.GetBlog(Blog, User.UserID, BlogModuleContext.BlogContext.Locale);
      if (blog == null || (!security.IsEditor && blog.OwnerUserId != User.UserID))
      {
        _logger.LogWarning("Blog Export denied: blog {BlogId}, user {UserId}", Blog, User.UserID);
        BlogModuleContext.ThrowAccessViolation();
      }

      string zipPath = BlogExportHelper.ExportToZip(
          blog, BlogModuleContext.Settings, User.UserID, BlogModuleContext.BlogContext.Locale);
      if (!System.IO.File.Exists(zipPath))
      {
        _logger.LogWarning("Blog Export produced no zip for blog {BlogId}: {Path}", Blog, zipPath);
        return RedirectToManageBlogs();
      }

      _logger.LogInformation("Blog Export ready for blog {BlogId}: {FileName}", Blog, Path.GetFileName(zipPath));
      return File(zipPath, "application/zip", Path.GetFileName(zipPath));
    }

    [HttpGet]
    public ActionResult Edit(int? Blog)
    {
      _logger.LogDebug("Blog Edit GET: blogId {BlogId}, user {UserId}", Blog, User.UserID);
      var security = BlogModuleContext.Security;
      BlogModuleContext.RequirePermissionLevel(security.IsBlogger || security.IsEditor);
      BlogModuleContext.EnsureInitialized();
      BlogModuleContext.AddModuleCss();

      int blogId = Blog ?? -1;
      BlogInfo blog = null;
      if (blogId > -1)
        blog = BlogsController.GetBlog(blogId, User.UserID, BlogModuleContext.BlogContext.Locale);

      if (blog != null && !security.IsEditor && blog.OwnerUserId != User.UserID)
      {
        _logger.LogWarning("Blog Edit GET denied: blog {BlogId}, user {UserId}", blogId, User.UserID);
        BlogModuleContext.ThrowAccessViolation();
      }

      var model = blog == null ? CreateNewModel() : MapFromBlog(blog);
      EnrichLocaleAndImage(model, blog);
      BlogPermissionEditHelper.PopulatePermissions(model, blog, PortalSettings);
      _logger.LogInformation("Blog Edit page rendered: blogId {BlogId}, isNew {IsNew}", model.BlogId, model.IsNew);
      return View(model);
    }

    [HttpPost]
    [ValidateInput(false)]
    [Web.Api.ValidateAntiForgeryToken]
    public ActionResult Edit(BlogEditViewModel model, string permissionAction, HttpPostedFileBase ImageFile)
    {
      _logger.LogDebug("Blog Edit POST: blogId {BlogId}, permissionAction {PermissionAction}, hasImage {HasImage}, user {UserId}",
          model?.BlogId, permissionAction, ImageFile != null && ImageFile.ContentLength > 0, User.UserID);
      var security = BlogModuleContext.Security;
      BlogModuleContext.RequirePermissionLevel(security.IsBlogger || security.IsEditor);
      BlogModuleContext.EnsureInitialized();
      BlogModuleContext.AddModuleCss();

      if (string.Equals(permissionAction, "addUser", StringComparison.OrdinalIgnoreCase))
      {
        if (!BlogPermissionEditHelper.TryAddUser(model, PortalSettings, out string addError))
        {
          _logger.LogWarning("Blog permission addUser failed for blog {BlogId}: {Error}", model.BlogId, addError);
          model.PermissionAddError = addError;
        }
        else
        {
          _logger.LogInformation("Blog permission user added for blog {BlogId}", model.BlogId);
        }
        RefreshPermissionColumnHeaders(model);
        EnrichLocaleAndImage(model, model.BlogId > -1
            ? BlogsController.GetBlog(model.BlogId, User.UserID, BlogModuleContext.BlogContext.Locale)
            : null);
        return View(model);
      }

      BlogInfo blog;
      bool isNew = model.BlogId <= -1;
      if (model.BlogId > -1)
      {
        blog = BlogsController.GetBlog(model.BlogId, User.UserID, BlogModuleContext.BlogContext.Locale);
        if (blog == null)
          blog = new BlogInfo();
        else if (!security.IsEditor && blog.OwnerUserId != User.UserID)
        {
          _logger.LogWarning("Blog Edit POST denied: blog {BlogId}, user {UserId}", model.BlogId, User.UserID);
          BlogModuleContext.ThrowAccessViolation();
        }
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

      ApplyImageChanges(blog, model, ImageFile);

      BlogPermissionEditHelper.NormalizeEnabledFlags(model);
      blog.Permissions = BlogPermissionEditHelper.ToPermissionCollection(model, blog.BlogID);
      BlogPermissionsController.UpdateBlogPermissions(blog);

      _logger.LogInformation("{Action} blog {BlogId} '{Title}' by user {UserId}",
          isNew ? "Created" : "Updated", blog.BlogID, blog.Title, User.UserID);
      return RedirectToManageBlogs();
    }

    [HttpPost]
    [Web.Api.ValidateAntiForgeryToken]
    public ActionResult Delete(int Blog)
    {
      _logger.LogDebug("Blog Delete: blogId {BlogId}, user {UserId}", Blog, User.UserID);
      var security = BlogModuleContext.Security;
      BlogModuleContext.RequirePermissionLevel(security.IsBlogger || security.IsEditor);

      var blog = BlogsController.GetBlog(Blog, User.UserID, BlogModuleContext.BlogContext.Locale);
      if (blog != null && (security.IsEditor || blog.OwnerUserId == User.UserID))
      {
        BlogsController.DeleteBlog(blog.BlogID);
        _logger.LogInformation("Deleted blog {BlogId} by user {UserId}", blog.BlogID, User.UserID);
      }
      else
      {
        _logger.LogWarning("Blog Delete skipped: blog {BlogId} missing or user {UserId} lacks permission", Blog, User.UserID);
      }

      return RedirectToManageBlogs();
    }

    /// <summary>
    /// Append section as a query string. Route-value redirects can lose it under DNN friendly URLs.
    /// </summary>
    private ActionResult RedirectToManageBlogs()
    {
      var baseUrl = Url.Action("Index", "Manage") ?? "";
      if (baseUrl.Length == 0)
        return RedirectToAction("Index", "Manage");
      var sep = baseUrl.IndexOf('?') >= 0 ? "&" : "?";
      return Redirect(baseUrl + sep + "section=" + ManageViewModel.SectionBlogs);
    }

    private BlogEditViewModel CreateNewModel()
    {
      return new BlogEditViewModel
      {
        IsNew = true,
        Locale = PortalSettings.DefaultLanguage,
        SyndicationEmail = User?.Email ?? "",
        IncludeImagesInFeed = true,
        EnablePingBackReceive = true,
        EnablePingBackSend = true,
        EnableTrackBackReceive = true,
        EnableTrackBackSend = true
      };
    }

    private BlogEditViewModel MapFromBlog(BlogInfo blog)
    {
      return new BlogEditViewModel
      {
        BlogId = blog.BlogID,
        Title = blog.Title,
        Description = blog.Description,
        ContentFormat = blog.ContentFormat,
        Locale = string.IsNullOrEmpty(blog.Locale) ? PortalSettings.DefaultLanguage : blog.Locale,
        FullLocalization = blog.FullLocalization,
        ImageKey = blog.Image ?? "",
        Published = blog.Published,
        Syndicated = blog.Syndicated,
        SyndicationEmail = string.IsNullOrEmpty(blog.SyndicationEmail) ? (User?.Email ?? "") : blog.SyndicationEmail,
        Copyright = blog.Copyright,
        IncludeImagesInFeed = blog.IncludeImagesInFeed,
        IncludeAuthorInFeed = blog.IncludeAuthorInFeed,
        EnablePingBackReceive = blog.EnablePingBackReceive,
        EnablePingBackSend = blog.EnablePingBackSend,
        AutoApprovePingBack = blog.AutoApprovePingBack,
        EnableTrackBackReceive = blog.EnableTrackBackReceive,
        EnableTrackBackSend = blog.EnableTrackBackSend,
        AutoApproveTrackBack = blog.AutoApproveTrackBack,
        MustApproveGhostPosts = blog.MustApproveGhostPosts,
        PublishAsOwner = blog.PublishAsOwner,
        IsNew = false
      };
    }

    private void EnrichLocaleAndImage(BlogEditViewModel model, BlogInfo blog)
    {
      var ctx = BlogModuleContext.BlogContext;
      model.ShowLocaleFields = ctx.IsMultiLingualSite;
      if (model.ShowLocaleFields)
      {
        model.LocaleOptions = LocaleController.Instance.GetLocales(PortalSettings.PortalId).Values
            .OrderBy(l => l.NativeName)
            .Select(l => new PostLocaleOption { Code = l.Code, Name = l.NativeName })
            .ToList();
      }

      string imageKey = blog != null ? (blog.Image ?? "") : (model.ImageKey ?? "");
      if (model.RemoveImage)
        imageKey = "";
      model.ImageKey = imageKey;
      if (!string.IsNullOrEmpty(imageKey) && model.BlogId > -1)
      {
        model.ImageUrl = VirtualPathUtility.ToAbsolute(BlogGlobals.glbImageHandlerPath)
            + string.Format("?TabId={0}&ModuleId={1}&Blog={2}&w=100&h=100&c=1&key={3}",
                ModuleContext.TabId, BlogModuleContext.Settings.ModuleId, model.BlogId, imageKey);
      }
      else
      {
        model.ImageUrl = "";
      }
    }

    private void ApplyModel(BlogInfo blog, BlogEditViewModel model)
    {
      blog.Title = model.Title ?? "";
      blog.Description = model.Description ?? "";
      if (blog.BlogID <= -1)
        blog.ContentFormat = model.ContentFormat;

      if (BlogModuleContext.BlogContext.IsMultiLingualSite)
      {
        blog.Locale = string.IsNullOrEmpty(model.Locale) ? PortalSettings.DefaultLanguage : model.Locale;
        blog.FullLocalization = model.FullLocalization;
      }
      else
      {
        blog.Locale = PortalSettings.DefaultLanguage;
      }

      blog.Published = model.Published;
      blog.Syndicated = model.Syndicated;
      blog.SyndicationEmail = model.SyndicationEmail ?? "";
      blog.Copyright = model.Copyright ?? "";
      blog.IncludeImagesInFeed = model.IncludeImagesInFeed;
      blog.IncludeAuthorInFeed = model.IncludeAuthorInFeed;
      blog.EnablePingBackReceive = model.EnablePingBackReceive;
      blog.EnablePingBackSend = model.EnablePingBackSend;
      blog.AutoApprovePingBack = model.AutoApprovePingBack;
      blog.EnableTrackBackReceive = model.EnableTrackBackReceive;
      blog.EnableTrackBackSend = model.EnableTrackBackSend;
      blog.AutoApproveTrackBack = model.AutoApproveTrackBack;
      blog.MustApproveGhostPosts = model.MustApproveGhostPosts;
      blog.PublishAsOwner = model.PublishAsOwner;

      if (!string.IsNullOrEmpty(model.ImageKey) && !model.RemoveImage)
        blog.Image = model.ImageKey;
    }

    private void ApplyImageChanges(BlogInfo blog, BlogEditViewModel model, HttpPostedFileBase imageFile)
    {
      if (model.RemoveImage)
      {
        _logger.LogDebug("Removing image for blog {BlogId}", blog.BlogID);
        DeleteBlogImages(blog);
        blog.Image = "";
        BlogsController.UpdateBlog(blog, User.UserID);
        return;
      }

      if (imageFile == null || imageFile.ContentLength <= 0)
        return;

      string extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
      if (BlogGlobals.glbPermittedFileExtensions.IndexOf(extension + ",", StringComparison.OrdinalIgnoreCase) < 0)
      {
        _logger.LogWarning("Blog image upload rejected: extension {Extension} for blog {BlogId}", extension, blog.BlogID);
        return;
      }

      string saveDir = BlogGlobals.GetBlogDirectoryMapPath(blog.BlogID);
      Directory.CreateDirectory(saveDir);

      if (!string.IsNullOrEmpty(blog.Image))
        DeleteImagesWithKey(saveDir, blog.Image);

      string newFileName = Guid.NewGuid().ToString("D");
      imageFile.SaveAs(saveDir + newFileName + extension);
      blog.Image = newFileName;
      BlogsController.UpdateBlog(blog, User.UserID);
      _logger.LogInformation("Blog image saved for blog {BlogId}: {ImageKey}{Extension}", blog.BlogID, newFileName, extension);
    }

    private static void DeleteBlogImages(BlogInfo blog)
    {
      if (blog == null || string.IsNullOrEmpty(blog.Image) || blog.BlogID <= 0)
        return;
      DeleteImagesWithKey(BlogGlobals.GetBlogDirectoryMapPath(blog.BlogID), blog.Image);
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

    private static void RefreshPermissionColumnHeaders(BlogEditViewModel model)
    {
      if (model.PermissionColumns == null) return;
      foreach (var col in model.PermissionColumns)
      {
        if (string.IsNullOrWhiteSpace(col.PermissionKey)) continue;
        var header = Localization.GetString(
            col.PermissionKey + ".Permission",
            BlogGlobals.SharedResourceFileName);
        if (!string.IsNullOrWhiteSpace(header))
          col.Header = header;
      }
    }
  }
}
