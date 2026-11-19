using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Core.Common;
using DotNetNuke.Web.Api;
using Microsoft.Extensions.Logging;
using CoreBlogs = DotNetNuke.Modules.Blog.Core.Entities.Blogs.BlogsController;

namespace DotNetNuke.Modules.Blog.BlogApi
{
  public class BlogsRouteMapper : IServiceRouteMapper
  {
    public void RegisterRoutes(IMapRoute mapRouteManager)
    {
      mapRouteManager.MapHttpRoute(
          "BlogApi",
          "BlogApiBlogsList",
          "blogs",
          new { Controller = "Blogs", Action = "List" },
          new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) },
          new[] { "DotNetNuke.Modules.Blog.BlogApi" });
    }
  }

  public class BlogsController : BlogApiController
  {
    private readonly ILogger<BlogsController> _logger;

    public BlogsController(ILogger<BlogsController> logger)
    {
      _logger = logger;
    }

    public class BlogPermissionsDto
    {
      public bool CanAdd { get; set; }
      public bool CanEdit { get; set; }
      public bool CanApprove { get; set; }
      public bool IsOwner { get; set; }
    }

    public class AccessibleBlogDto
    {
      public int BlogId { get; set; }
      public int ModuleId { get; set; }
      public string Title { get; set; }
      public BlogPermissionsDto Permissions { get; set; }
      public byte DefaultContentFormat { get; set; }
      public byte SummaryFormat { get; set; }
      public string Language { get; set; }
      public bool FullyLocalizable { get; set; }
    }

    [HttpGet]
    [ApiTokenAuthorize("BlogApi", "~/DesktopModules/MVC/Blog/App_LocalResources/SharedResources.resx", Web.Api.Auth.ApiTokens.Models.ApiTokenScope.User)]
    [Route("/api/BlogApi/blogs")]
    public HttpResponseMessage List()
    {
      var portalId = PortalSettings.PortalId;
      var locale = PortalSettings.DefaultLanguage;
      _logger.LogDebug("BlogApi blogs list requested by user {UserId} portal {PortalId}", UserInfo.UserID, portalId);

      var blogs = CoreBlogs.GetBlogsByPortal(portalId, UserInfo.UserID, locale).Values
          .Where(b => b.CanAdd || b.CanEdit || b.CanApprove || b.IsOwner)
          .OrderBy(b => b.LocalizedTitle)
          .ToList();

      var settingsByModule = new Dictionary<int, ModuleSettings>();
      var result = new List<AccessibleBlogDto>();
      foreach (var blog in blogs)
      {
        if (!settingsByModule.TryGetValue(blog.ModuleID, out var settings))
        {
          try
          {
            settings = ModuleSettings.GetSettings(blog.ModuleID);
            settingsByModule[blog.ModuleID] = settings;
          }
          catch
          {
            settings = null;
          }
        }

        result.Add(new AccessibleBlogDto
        {
          BlogId = blog.BlogID,
          ModuleId = blog.ModuleID,
          Title = blog.LocalizedTitle ?? blog.Title ?? "",
          Permissions = new BlogPermissionsDto
          {
            CanAdd = blog.CanAdd,
            CanEdit = blog.CanEdit,
            CanApprove = blog.CanApprove,
            IsOwner = blog.IsOwner
          },
          DefaultContentFormat = blog.DefaultContentFormat,
          SummaryFormat = settings != null ? (byte)settings.SummaryModel : (byte)0,
          Language = blog.Locale ?? "",
          FullyLocalizable = blog.FullLocalization
        });
      }

      _logger.LogInformation("BlogApi blogs list returned {Count} blogs for user {UserId}", result.Count, UserInfo.UserID);
      return Request.CreateResponse(HttpStatusCode.OK, result);
    }
  }
}
