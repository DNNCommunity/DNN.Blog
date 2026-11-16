using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Modules.Blog.Core.Common;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using DotNetNuke.Modules.Blog.Core.Entities.Terms;
using DotNetNuke.Modules.Blog.Core.Integration;
using DotNetNuke.Modules.Blog.Core.Media;
using DotNetNuke.Modules.Blog.Core.Security;
using DotNetNuke.Services.Localization;
using BlogGlobals = DotNetNuke.Modules.Blog.Core.Common.Globals;

namespace DotNetNuke.Modules.Blog.Core.Entities.Posts
{
  public class PostWriteResult
  {
    public bool Success { get; set; }
    public bool AccessDenied { get; set; }
    public bool NotFound { get; set; }
    public string ValidationMessage { get; set; }
    public PostInfo Post { get; set; }
  }

  public class PostWriteContext
  {
    public int ModuleId { get; set; }
    public int PortalId { get; set; }
    public string PortalDefaultLanguage { get; set; }
    public string AdministratorRoleName { get; set; }
    public string Locale { get; set; }
    public TimeZoneInfo UiTimeZone { get; set; } = TimeZoneInfo.Local;
    public bool IsMultiLingualSite { get; set; }
    public UserInfo User { get; set; }
    public PortalSettings PortalSettings { get; set; }
    public ContextSecurity Security { get; set; }
    public BlogInfo Blog { get; set; }
    public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;
    public string TitleRequiredMessage { get; set; } = "A title is required for the post";
    public string ContentRequiredMessage { get; set; } = "Post is Required";
  }

  public class PostWriteService
  {
    public PostWriteResult Save(PostWriteModel model, PostWriteContext context)
    {
      if (model == null || context?.Blog == null || context.User == null || context.Security == null)
        return new PostWriteResult { AccessDenied = true };

      var blog = context.Blog;
      var security = context.Security;
      bool isNew = model.PostId <= -1;

      PostInfo post;
      if (!isNew)
      {
        post = PostsController.GetPost(model.PostId, context.ModuleId, context.Locale);
        if (post == null)
          return new PostWriteResult { NotFound = true };
        if (!security.CanEditPost && !security.CanEditThisPost(post))
          return new PostWriteResult { AccessDenied = true };
      }
      else
      {
        if (!security.CanAddPost && !blog.CanAdd)
          return new PostWriteResult { AccessDenied = true };
        post = new PostInfo { BlogID = model.BlogId, Blog = blog, Published = true };
      }

      if (string.IsNullOrWhiteSpace(model.Title))
        return new PostWriteResult { ValidationMessage = context.TitleRequiredMessage };

      if (string.IsNullOrWhiteSpace(StripHtmlRough(model.Content)))
        return new PostWriteResult { ValidationMessage = context.ContentRequiredMessage };

      var imageService = new ImageService();
      try { imageService.PurgeTemp(TimeSpan.FromDays(7)); }
      catch (Exception ex)
      {
        DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
      }

      bool firstPublish = !post.Published && model.Published;

      post.Title = model.Title ?? "";
      post.Summary = model.Summary ?? "";
      post.Content = model.Content ?? "";
      post.AllowComments = model.AllowComments;
      post.DisplayCopyright = model.DisplayCopyright;
      post.Copyright = model.Copyright ?? "";

      if (context.IsMultiLingualSite && !blog.FullLocalization)
        post.Locale = model.Locale ?? "";
      else
        post.Locale = "";

      ApplyLocalizations(post, model, blog, context.PortalDefaultLanguage);
      ApplyPublishDate(post, model, isNew, context);

      post.Published = model.Published;
      if (blog.MustApproveGhostPosts && !security.CanApprovePost)
      {
        post.Published = false;
        firstPublish = false;
      }

      post.Terms = BuildTerms(model, context.ModuleId, context.Locale);

      string savedTempFile = null;
      string savedExtension = null;
      if (model.RemoveImage)
      {
        RemoveFeaturedImages(post);
        post.Image = "";
      }
      else if (model.FeaturedImageStream != null
               && !string.IsNullOrEmpty(model.FeaturedImageFileName))
      {
        var upload = SaveFeaturedImage(post, model.FeaturedImageStream, model.FeaturedImageFileName, isNew);
        if (upload != null)
        {
          post.Image = upload.Item1;
          savedTempFile = upload.Item2;
          savedExtension = upload.Item3;
        }
      }
      else if (!string.IsNullOrEmpty(model.ImageKey)
               && (isNew || !string.Equals(model.ImageKey, post.Image, StringComparison.OrdinalIgnoreCase)))
      {
        // Explicit non-empty key from API / form; empty ImageKey without RemoveImage leaves existing image.
        post.Image = model.ImageKey;
      }

      int publishingUserId = context.User.UserID;
      if (blog.PublishAsOwner)
        publishingUserId = blog.OwnerUserId;

      if (isNew)
      {
        post = PostsController.AddPost(post, publishingUserId);
        if (!string.IsNullOrEmpty(savedTempFile) && File.Exists(savedTempFile))
        {
          string saveDir = BlogGlobals.GetPostDirectoryMapPath(post);
          Directory.CreateDirectory(saveDir);
          string dest = saveDir + post.Image + savedExtension;
          File.Move(savedTempFile, dest);
        }
      }
      else
      {
        PostsController.UpdatePost(post, publishingUserId);
      }

      var allHtml = new List<string> { post.Content, post.Summary };
      allHtml.AddRange(post.ContentLocalizations.GetDictionary().Values);
      allHtml.AddRange(post.SummaryLocalizations.GetDictionary().Values);
      bool isAdmin = context.User.IsSuperUser
          || (!string.IsNullOrEmpty(context.AdministratorRoleName)
              && context.User.IsInRole(context.AdministratorRoleName));
      imageService.SyncPostImages(blog.BlogID, post.ContentItemId, context.User.UserID, isAdmin, allHtml.ToArray());

      if (firstPublish)
      {
        PostsController.PublishPost(post, context.User.UserID);
      }
      else if (blog.MustApproveGhostPosts && context.User.UserID != blog.OwnerUserId)
      {
        string title = Localization.GetString("ApprovePostNotifyBody", BlogGlobals.SharedResourceFileName);
        string permalink = context.PortalSettings != null
            ? post.PermaLink(context.PortalSettings)
            : post.Title;
        string summary = "<a target='_blank' href='" + permalink + "'>" + post.Title + "</a>";
        NotificationController.PostPendingApproval(blog, post, context.PortalId, summary, title);
      }

      return new PostWriteResult { Success = true, Post = post };
    }

    public PostWriteResult Approve(int moduleId, int blogId, int postId, string locale, UserInfo user, ContextSecurity security)
    {
      var post = PostsController.GetPost(postId, moduleId, locale);
      if (post == null || post.BlogID != blogId)
        return new PostWriteResult { NotFound = true };

      bool canApprove = post.Blog?.CanApprove == true || (security != null && security.CanApprovePost);
      if (!canApprove)
        return new PostWriteResult { AccessDenied = true };

      PostsController.PublishPost(post, true, user.UserID);
      return new PostWriteResult { Success = true, Post = post };
    }

    public PostWriteResult Delete(int moduleId, int blogId, int postId, string locale, UserInfo user, ContextSecurity security, bool cleanupImages = true)
    {
      var post = PostsController.GetPost(postId, moduleId, locale);
      if (post == null || post.BlogID != blogId)
        return new PostWriteResult { NotFound = true };

      if (post.Blog == null && user != null)
        post.Blog = BlogsController.GetBlog(blogId, user.UserID, locale);

      bool allowed = post.Blog?.CanEdit == true
          || (security != null && security.CanEditThisPost(post));
      if (!allowed)
        return new PostWriteResult { AccessDenied = true };

      if (cleanupImages)
      {
        RemoveFeaturedImages(post);
        new ImageService().DeletePostImages(post.BlogID, post.ContentItemId);
      }

      PostsController.DeletePost(post);
      return new PostWriteResult { Success = true, Post = post };
    }

    private static void ApplyPublishDate(PostInfo post, PostWriteModel model, bool isNew, PostWriteContext context)
    {
      if (model.PublishNow)
      {
        post.PublishedOnDate = DateTime.UtcNow;
        return;
      }

      if (!string.IsNullOrWhiteSpace(model.PublishDate))
      {
        try
        {
          var local = DateTime.Parse(
              model.PublishDate,
              context.Culture ?? CultureInfo.CurrentCulture,
              DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal);
          local = DateTime.SpecifyKind(local, DateTimeKind.Unspecified);
          post.PublishedOnDate = TimeZoneInfo.ConvertTimeToUtc(local, context.UiTimeZone ?? TimeZoneInfo.Local);
        }
        catch
        {
          if (isNew)
            post.PublishedOnDate = DateTime.UtcNow;
        }
        return;
      }

      if (isNew)
        post.PublishedOnDate = DateTime.UtcNow;
    }

    private static void ApplyLocalizations(PostInfo post, PostWriteModel model, BlogInfo blog, string portalDefaultLanguage)
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

    private static List<TermInfo> BuildTerms(PostWriteModel model, int moduleId, string locale)
    {
      var terms = new List<TermInfo>();
      if (!string.IsNullOrWhiteSpace(model.Tags))
      {
        var tagNames = model.Tags.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).ToList();
        terms.AddRange(TermsController.GetTermList(moduleId, tagNames, 1, true, locale));
      }

      var settings = ModuleSettings.GetSettings(moduleId);
      if (settings.VocabularyId > -1 && model.SelectedCategoryIds != null && model.SelectedCategoryIds.Count > 0)
      {
        var vocab = TermsController.GetTermsByVocabulary(moduleId, settings.VocabularyId);
        foreach (int id in model.SelectedCategoryIds.Distinct())
        {
          if (vocab.TryGetValue(id, out var term))
            terms.Add(term);
        }
      }
      return terms;
    }

    private static Tuple<string, string, string> SaveFeaturedImage(PostInfo post, Stream stream, string fileName, bool isNew)
    {
      string extension = Path.GetExtension(fileName).ToLowerInvariant();
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
      using (var fs = File.Create(savedFile))
      {
        stream.CopyTo(fs);
      }
      return Tuple.Create(newFileName, savedFile, extension);
    }

    public static void RemoveFeaturedImages(PostInfo post)
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

    private static string StripHtmlRough(string html)
    {
      if (string.IsNullOrEmpty(html)) return "";
      return Regex.Replace(html, "<[^>]+>", "").Trim();
    }
  }
}
