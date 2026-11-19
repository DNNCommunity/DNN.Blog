using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Core.Common;
using DotNetNuke.Modules.Blog.Core.Entities.Posts;
using DotNetNuke.Web.Api;
using Microsoft.Extensions.Logging;
using CorePosts = DotNetNuke.Modules.Blog.Core.Entities.Posts.PostsController;
using BlogGlobals = DotNetNuke.Modules.Blog.Core.Common.Globals;

namespace DotNetNuke.Modules.Blog.BlogApi
{
  public class PostsRouteMapper : IServiceRouteMapper
  {
    public void RegisterRoutes(IMapRoute mapRouteManager)
    {
      var ns = new[] { "DotNetNuke.Modules.Blog.BlogApi" };

      mapRouteManager.MapHttpRoute("BlogApi", "BlogApiPostsCreate",
          "modules/{moduleId}/blogs/{blogId}/posts",
          new { Controller = "Posts", Action = "Create" },
          new { moduleId = "-?\\d+", blogId = "-?\\d+", httpMethod = new HttpMethodConstraint(HttpMethod.Post) },
          ns);

      mapRouteManager.MapHttpRoute("BlogApi", "BlogApiPostsGet",
          "modules/{moduleId}/blogs/{blogId}/posts/{postId}",
          new { Controller = "Posts", Action = "Get" },
          new { moduleId = "-?\\d+", blogId = "-?\\d+", postId = "-?\\d+", httpMethod = new HttpMethodConstraint(HttpMethod.Get) },
          ns);

      // PUT is often blocked by IIS WebDAV; POST .../update is the portable alternative.
      mapRouteManager.MapHttpRoute("BlogApi", "BlogApiPostsUpdatePut",
          "modules/{moduleId}/blogs/{blogId}/posts/{postId}",
          new { Controller = "Posts", Action = "Update" },
          new { moduleId = "-?\\d+", blogId = "-?\\d+", postId = "-?\\d+", httpMethod = new HttpMethodConstraint(HttpMethod.Put) },
          ns);

      mapRouteManager.MapHttpRoute("BlogApi", "BlogApiPostsUpdatePost",
          "modules/{moduleId}/blogs/{blogId}/posts/{postId}/update",
          new { Controller = "Posts", Action = "Update" },
          new { moduleId = "-?\\d+", blogId = "-?\\d+", postId = "-?\\d+", httpMethod = new HttpMethodConstraint(HttpMethod.Post) },
          ns);

      mapRouteManager.MapHttpRoute("BlogApi", "BlogApiPostsApprove",
          "modules/{moduleId}/blogs/{blogId}/posts/{postId}/approve",
          new { Controller = "Posts", Action = "Approve" },
          new { moduleId = "-?\\d+", blogId = "-?\\d+", postId = "-?\\d+", httpMethod = new HttpMethodConstraint(HttpMethod.Post) },
          ns);

      // DELETE is often blocked by IIS WebDAV; POST .../delete is the portable alternative.
      mapRouteManager.MapHttpRoute("BlogApi", "BlogApiPostsDeleteVerb",
          "modules/{moduleId}/blogs/{blogId}/posts/{postId}",
          new { Controller = "Posts", Action = "Delete" },
          new { moduleId = "-?\\d+", blogId = "-?\\d+", postId = "-?\\d+", httpMethod = new HttpMethodConstraint(HttpMethod.Delete) },
          ns);

      mapRouteManager.MapHttpRoute("BlogApi", "BlogApiPostsDeletePost",
          "modules/{moduleId}/blogs/{blogId}/posts/{postId}/delete",
          new { Controller = "Posts", Action = "Delete" },
          new { moduleId = "-?\\d+", blogId = "-?\\d+", postId = "-?\\d+", httpMethod = new HttpMethodConstraint(HttpMethod.Post) },
          ns);
    }
  }

  public class PostsController : BlogApiController
  {
    private readonly ILogger<PostsController> _logger;
    private readonly PostWriteService _writeService = new PostWriteService();

    public PostsController(ILogger<PostsController> logger)
    {
      _logger = logger;
    }

    public class PostWriteDto
    {
      public string Title { get; set; } = "";
      public string Summary { get; set; } = "";
      public string Content { get; set; } = "";
      public Dictionary<string, string> Titles { get; set; } = new Dictionary<string, string>();
      public Dictionary<string, string> Summaries { get; set; } = new Dictionary<string, string>();
      public Dictionary<string, string> Contents { get; set; } = new Dictionary<string, string>();
      public bool Published { get; set; } = true;
      public bool PublishNow { get; set; }
      public string PublishDate { get; set; } = "";
      public bool AllowComments { get; set; } = true;
      public bool DisplayCopyright { get; set; }
      public string Copyright { get; set; } = "";
      public string Locale { get; set; } = "";
      public string Tags { get; set; } = "";
      public List<int> SelectedCategoryIds { get; set; } = new List<int>();
      public string ImageKey { get; set; } = "";
      public bool RemoveImage { get; set; }
      /// <summary>Optional on create (0 = HTML, 1 = Markdown). Ignored on update.</summary>
      public byte? ContentFormat { get; set; }
    }

    public class PostResponseDto
    {
      public int BlogId { get; set; }
      public int PostId { get; set; }
      public int ModuleId { get; set; }
      public string Title { get; set; }
      public string Summary { get; set; }
      public string Content { get; set; }
      public Dictionary<string, string> Titles { get; set; }
      public Dictionary<string, string> Summaries { get; set; }
      public Dictionary<string, string> Contents { get; set; }
      public bool Published { get; set; }
      public DateTime PublishedOnDate { get; set; }
      public bool AllowComments { get; set; }
      public bool DisplayCopyright { get; set; }
      public string Copyright { get; set; }
      public string Locale { get; set; }
      public string Tags { get; set; }
      public List<int> SelectedCategoryIds { get; set; }
      public string ImageKey { get; set; }
      public byte ContentFormat { get; set; }
    }

    [HttpPost]
    [ApiTokenAuthorize("BlogApi", "~/DesktopModules/MVC/Blog/App_LocalResources/SharedResources.resx", Web.Api.Auth.ApiTokens.Models.ApiTokenScope.User)]
    [Route("/api/BlogApi/modules/{moduleId}/blogs/{blogId}/posts")]
    public HttpResponseMessage Create(int moduleId, int blogId, PostWriteDto data)
    {
      _logger.LogDebug("BlogApi create post: module {ModuleId}, blog {BlogId}, user {UserId}", moduleId, blogId, UserInfo.UserID);

      var ctx = BlogApiResourceHelper.Resolve(moduleId, blogId, PortalSettings, UserInfo);
      if (ctx == null)
        return Request.CreateResponse(HttpStatusCode.NotFound, "Module or blog not found");

      var model = ToWriteModel(data, blogId, -1);
      var result = _writeService.Save(model, ToWriteContext(ctx));
      return ToSaveResponse(result, moduleId, created: true);
    }

    [HttpGet]
    [ApiTokenAuthorize("BlogApi", "~/DesktopModules/MVC/Blog/App_LocalResources/SharedResources.resx", Web.Api.Auth.ApiTokens.Models.ApiTokenScope.User)]
    [Route("/api/BlogApi/modules/{moduleId}/blogs/{blogId}/posts/{postId}")]
    public HttpResponseMessage Get(int moduleId, int blogId, int postId)
    {
      _logger.LogDebug("BlogApi get post: module {ModuleId}, blog {BlogId}, post {PostId}, user {UserId}",
          moduleId, blogId, postId, UserInfo.UserID);

      var ctx = BlogApiResourceHelper.Resolve(moduleId, blogId, PortalSettings, UserInfo);
      if (ctx == null)
        return Request.CreateResponse(HttpStatusCode.NotFound, "Module or blog not found");

      var post = CorePosts.GetPost(postId, moduleId, ctx.Locale);
      if (post == null || post.BlogID != blogId)
        return Request.CreateResponse(HttpStatusCode.NotFound, "Post not found");

      if (!ctx.Security.CanEditPost && !ctx.Security.CanEditThisPost(post) && !ctx.Security.CanAddPost)
        return AccessViolation("You do not have permission to view this post for editing");

      return Request.CreateResponse(HttpStatusCode.OK, ToResponseDto(post, moduleId, ctx));
    }

    [HttpPut]
    [HttpPost]
    [ApiTokenAuthorize("BlogApi", "~/DesktopModules/MVC/Blog/App_LocalResources/SharedResources.resx", Web.Api.Auth.ApiTokens.Models.ApiTokenScope.User)]
    [Route("/api/BlogApi/modules/{moduleId}/blogs/{blogId}/posts/{postId}")]
    [Route("/api/BlogApi/modules/{moduleId}/blogs/{blogId}/posts/{postId}/update")]
    public HttpResponseMessage Update(int moduleId, int blogId, int postId, PostWriteDto data)
    {
      _logger.LogDebug("BlogApi update post: module {ModuleId}, blog {BlogId}, post {PostId}, user {UserId}",
          moduleId, blogId, postId, UserInfo.UserID);

      var ctx = BlogApiResourceHelper.Resolve(moduleId, blogId, PortalSettings, UserInfo);
      if (ctx == null)
        return Request.CreateResponse(HttpStatusCode.NotFound, "Module or blog not found");

      var model = ToWriteModel(data, blogId, postId);
      var result = _writeService.Save(model, ToWriteContext(ctx));
      return ToSaveResponse(result, moduleId, created: false);
    }

    [HttpPost]
    [ApiTokenAuthorize("BlogApi", "~/DesktopModules/MVC/Blog/App_LocalResources/SharedResources.resx", Web.Api.Auth.ApiTokens.Models.ApiTokenScope.User)]
    [Route("/api/BlogApi/modules/{moduleId}/blogs/{blogId}/posts/{postId}/approve")]
    public HttpResponseMessage Approve(int moduleId, int blogId, int postId)
    {
      _logger.LogDebug("BlogApi approve post: module {ModuleId}, blog {BlogId}, post {PostId}, user {UserId}",
          moduleId, blogId, postId, UserInfo.UserID);

      var ctx = BlogApiResourceHelper.Resolve(moduleId, blogId, PortalSettings, UserInfo);
      if (ctx == null)
        return Request.CreateResponse(HttpStatusCode.NotFound, "Module or blog not found");

      var result = _writeService.Approve(moduleId, blogId, postId, ctx.Locale, UserInfo, ctx.Security);
      if (result.NotFound)
        return Request.CreateResponse(HttpStatusCode.NotFound, "Post not found");
      if (result.AccessDenied)
        return AccessViolation("You are not allowed to approve this post");

      _logger.LogInformation("BlogApi approved post {PostId} on blog {BlogId}", postId, blogId);
      return Request.CreateResponse(HttpStatusCode.OK, ToResponseDto(result.Post, moduleId, ctx));
    }

    [HttpDelete]
    [HttpPost]
    [ApiTokenAuthorize("BlogApi", "~/DesktopModules/MVC/Blog/App_LocalResources/SharedResources.resx", Web.Api.Auth.ApiTokens.Models.ApiTokenScope.User)]
    [Route("/api/BlogApi/modules/{moduleId}/blogs/{blogId}/posts/{postId}")]
    [Route("/api/BlogApi/modules/{moduleId}/blogs/{blogId}/posts/{postId}/delete")]
    public HttpResponseMessage Delete(int moduleId, int blogId, int postId)
    {
      _logger.LogDebug("BlogApi delete post: module {ModuleId}, blog {BlogId}, post {PostId}, user {UserId}",
          moduleId, blogId, postId, UserInfo.UserID);

      var ctx = BlogApiResourceHelper.Resolve(moduleId, blogId, PortalSettings, UserInfo);
      if (ctx == null)
        return Request.CreateResponse(HttpStatusCode.NotFound, "Module or blog not found");

      var result = _writeService.Delete(moduleId, blogId, postId, ctx.Locale, UserInfo, ctx.Security, cleanupImages: true);
      if (result.NotFound)
        return Request.CreateResponse(HttpStatusCode.NotFound, "Post not found");
      if (result.AccessDenied)
        return AccessViolation("You are not allowed to delete this post");

      _logger.LogInformation("BlogApi deleted post {PostId} from blog {BlogId}", postId, blogId);
      return Request.CreateResponse(HttpStatusCode.OK, new { Result = "success" });
    }

    private PostWriteContext ToWriteContext(BlogApiResolvedContext ctx)
    {
      return new PostWriteContext
      {
        ModuleId = ctx.Module.ModuleID,
        PortalId = PortalSettings.PortalId,
        PortalDefaultLanguage = PortalSettings.DefaultLanguage,
        AdministratorRoleName = PortalSettings.AdministratorRoleName,
        Locale = ctx.Locale,
        UiTimeZone = ctx.UiTimeZone,
        IsMultiLingualSite = ctx.IsMultiLingualSite,
        User = UserInfo,
        PortalSettings = PortalSettings,
        Security = ctx.Security,
        Blog = ctx.Blog
      };
    }

    private static PostWriteModel ToWriteModel(PostWriteDto data, int blogId, int postId)
    {
      data = data ?? new PostWriteDto();
      return new PostWriteModel
      {
        BlogId = blogId,
        PostId = postId,
        Title = data.Title,
        Summary = data.Summary,
        Content = data.Content,
        Titles = data.Titles ?? new Dictionary<string, string>(),
        Summaries = data.Summaries ?? new Dictionary<string, string>(),
        Contents = data.Contents ?? new Dictionary<string, string>(),
        Published = data.Published,
        PublishNow = data.PublishNow,
        PublishDate = data.PublishDate,
        AllowComments = data.AllowComments,
        DisplayCopyright = data.DisplayCopyright,
        Copyright = data.Copyright,
        Locale = data.Locale,
        Tags = data.Tags,
        SelectedCategoryIds = data.SelectedCategoryIds ?? new List<int>(),
        ImageKey = data.ImageKey,
        RemoveImage = data.RemoveImage,
        ContentFormat = data.ContentFormat
      };
    }

    private HttpResponseMessage ToSaveResponse(PostWriteResult result, int moduleId, bool created)
    {
      if (result.AccessDenied)
        return AccessViolation("You do not have permission to save this post");
      if (result.NotFound)
        return Request.CreateResponse(HttpStatusCode.NotFound, "Post not found");
      if (!result.Success)
        return Request.CreateResponse(HttpStatusCode.BadRequest, result.ValidationMessage);

      var ctx = BlogApiResourceHelper.Resolve(moduleId, result.Post.BlogID, PortalSettings, UserInfo);
      return Request.CreateResponse(
          created ? HttpStatusCode.Created : HttpStatusCode.OK,
          ToResponseDto(result.Post, moduleId, ctx));
    }

    private PostResponseDto ToResponseDto(PostInfo post, int moduleId, BlogApiResolvedContext ctx)
    {
      var tags = post.PostTags?.Select(t => t.Name) ?? Enumerable.Empty<string>();
      var titles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
      var summaries = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
      var contents = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      // Legacy posts may store HtmlEncoded fields; decode for editors/API consumers.
      if (post.TitleLocalizations != null)
      {
        foreach (var kv in post.TitleLocalizations.GetDictionary())
          titles[kv.Key] = HttpUtility.HtmlDecode(kv.Value ?? "");
      }
      if (post.SummaryLocalizations != null)
      {
        foreach (var kv in post.SummaryLocalizations.GetDictionary())
          summaries[kv.Key] = HttpUtility.HtmlDecode(kv.Value ?? "");
      }
      if (post.ContentLocalizations != null)
      {
        foreach (var kv in post.ContentLocalizations.GetDictionary())
          contents[kv.Key] = HttpUtility.HtmlDecode(kv.Value ?? "");
      }

      return new PostResponseDto
      {
        BlogId = post.BlogID,
        PostId = post.ContentItemId,
        ModuleId = moduleId,
        Title = HttpUtility.HtmlDecode(post.Title ?? ""),
        Summary = HttpUtility.HtmlDecode(post.Summary ?? ""),
        Content = HttpUtility.HtmlDecode(post.Content ?? ""),
        Titles = titles,
        Summaries = summaries,
        Contents = contents,
        Published = post.Published,
        PublishedOnDate = ctx != null
            ? BlogGlobals.UtcToLocalTime(post.PublishedOnDate, ctx.UiTimeZone)
            : post.PublishedOnDate,
        AllowComments = post.AllowComments,
        DisplayCopyright = post.DisplayCopyright,
        Copyright = post.Copyright ?? "",
        Locale = post.Locale ?? "",
        Tags = string.Join(", ", tags),
        SelectedCategoryIds = post.PostCategories?.Select(c => c.TermId).ToList() ?? new List<int>(),
        ImageKey = post.Image ?? "",
        ContentFormat = post.ContentFormat
      };
    }
  }
}
