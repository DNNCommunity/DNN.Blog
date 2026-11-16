using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Core.Entities.Terms;
using DotNetNuke.Web.Api;
using Microsoft.Extensions.Logging;

namespace DotNetNuke.Modules.Blog.BlogApi
{
  public class CategoriesRouteMapper : IServiceRouteMapper
  {
    public void RegisterRoutes(IMapRoute mapRouteManager)
    {
      mapRouteManager.MapHttpRoute(
          "BlogApi",
          "BlogApiCategoriesTree",
          "modules/{moduleId}/categories",
          new { Controller = "Categories", Action = "Tree" },
          new { moduleId = "-?\\d+", httpMethod = new HttpMethodConstraint(HttpMethod.Get) },
          new[] { "DotNetNuke.Modules.Blog.BlogApi" });
    }
  }

  public class CategoriesController : BlogApiController
  {
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ILogger<CategoriesController> logger)
    {
      _logger = logger;
    }

    public class CategoryNodeDto
    {
      public int CategoryId { get; set; }
      public string Name { get; set; }
      public List<CategoryNodeDto> Children { get; set; } = new List<CategoryNodeDto>();
    }

    [HttpGet]
    [ApiTokenAuthorize("BlogApi", "~/DesktopModules/MVC/Blog/App_LocalResources/SharedResources.resx", Web.Api.Auth.ApiTokens.Models.ApiTokenScope.User)]
    [Route("/api/BlogApi/modules/{moduleId}/categories")]
    public HttpResponseMessage Tree(int moduleId, string language = null)
    {
      _logger.LogDebug("BlogApi categories tree: module {ModuleId}, language {Language}, user {UserId}",
          moduleId, language, UserInfo.UserID);

      var module = BlogApiResourceHelper.GetModuleInPortal(moduleId, PortalSettings.PortalId);
      if (module == null)
        return Request.CreateResponse(HttpStatusCode.NotFound, "Module not found");

      var locale = string.IsNullOrEmpty(language) ? PortalSettings.DefaultLanguage : language;
      if (!BlogApiResourceHelper.UserCanAccessModuleBlogs(moduleId, PortalSettings.PortalId, UserInfo.UserID, locale)
          && !IsModuleEditor(moduleId))
      {
        return AccessViolation("You do not have access to categories for this module");
      }

      var tree = new CategoryTreeService().GetTree(
          moduleId,
          PortalSettings.PortalId,
          PortalSettings.DefaultLanguage,
          locale);

      if (tree.NoVocabulary)
        return Request.CreateResponse(HttpStatusCode.BadRequest, "No vocabulary configured.");

      var items = MapSlim(tree.Items);
      _logger.LogInformation("BlogApi categories returned {RootCount} roots for module {ModuleId}", items.Count, moduleId);
      return Request.CreateResponse(HttpStatusCode.OK, items);
    }

    private bool IsModuleEditor(int moduleId)
    {
      var ctx = BlogApiResourceHelper.Resolve(moduleId, null, PortalSettings, UserInfo);
      return ctx?.Security != null && (ctx.Security.IsEditor || ctx.Security.IsBlogger || ctx.Security.UserIsAdmin);
    }

    private static List<CategoryNodeDto> MapSlim(List<CategoryTreeNode> nodes)
    {
      return (nodes ?? new List<CategoryTreeNode>()).Select(n => new CategoryNodeDto
      {
        CategoryId = n.CategoryId,
        Name = n.Name ?? "",
        Children = MapSlim(n.Children)
      }).ToList();
    }
  }
}
