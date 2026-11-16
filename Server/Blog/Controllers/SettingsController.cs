using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Core.Common;
using DotNetNuke.Modules.Blog.Models;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Web.Mvc;
using CoreTermsController = DotNetNuke.Modules.Blog.Core.Entities.Terms.TermsController;

namespace DotNetNuke.Modules.Blog.Controllers
{
  [DnnModuleAuthorize(AccessLevel = Security.SecurityAccessLevel.Edit)]
  [DnnHandleError]
  public class SettingsController : BlogMvcController
  {
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(ILogger<SettingsController> logger) : base()
    {
      _logger = logger;
    }

    [HttpGet]
    public ActionResult Settings(int? blogModuleId)
    {
      _logger.LogDebug("Settings GET: blogModuleId {BlogModuleId}, tabModuleId {TabModuleId}, user {UserId}",
          blogModuleId, ModuleContext.TabModuleId, User.UserID);
      BlogModuleContext.AddModuleCss();
      var model = BuildViewModel(blogModuleId);
      _logger.LogInformation("Settings page rendered for tabModule {TabModuleId}, blogModuleId {BlogModuleId}",
          ModuleContext.TabModuleId, model.BlogModuleId);
      return View(model);
    }

    [HttpPost]
    [ValidateInput(false)]
    [Web.Api.ValidateAntiForgeryToken]
    public ActionResult Settings(SettingsViewModel model)
    {
      _logger.LogDebug("Settings POST: tabModuleId {TabModuleId}, blogModuleId {BlogModuleId}, user {UserId}",
          ModuleContext.TabModuleId, model?.BlogModuleId, User.UserID);

      var vs = ViewSettings.GetViewSettings(ModuleContext.TabModuleId);
      vs.BlogModuleId = model.BlogModuleId;
      vs.BlogId = model.BlogId;
      vs.Categories = CategoryTreeHelper.JoinCategoryIds(model.SelectedCategoryIds);
      vs.AuthorId = model.AuthorId;
      vs.Template = model.Template ?? vs.Template;
      vs.ShowAllLocales = model.ShowAllLocales;
      vs.ModifyPageDetails = model.ModifyPageDetails;
      vs.AddCanonicalTag = model.AddCanonicalTag;
      vs.ShowManagementPanel = model.ShowManagementPanel;
      vs.ShowManagementPanelViewMode = model.ShowManagementPanelViewMode;
      vs.HideUnpublishedBlogsViewMode = model.HideUnpublishedBlogsViewMode;
      vs.HideUnpublishedBlogsEditMode = model.HideUnpublishedBlogsEditMode;
      vs.AllowComments = model.AllowComments;
      vs.UpdateSettings();

      _logger.LogInformation("View settings saved for tabModule {TabModuleId}: blogModuleId {BlogModuleId}, blogId {BlogId}, template {Template}",
          ModuleContext.TabModuleId, vs.BlogModuleId, vs.BlogId, vs.Template);
      return RedirectToDefaultRoute();
    }

    private SettingsViewModel BuildViewModel(int? blogModuleIdOverride)
    {
      var vs = ViewSettings.GetViewSettings(ModuleContext.TabModuleId);
      int blogModuleId = blogModuleIdOverride ?? SettingsListHelper.ResolveBlogModuleId(vs, ModuleContext.ModuleId);
      var selectedIds = CategoryTreeHelper.ParseCategoryIds(vs.Categories);
      var categoryTree = BuildCategoryTreeForModule(blogModuleId, selectedIds);

      return new SettingsViewModel
      {
        Settings = vs,
        BlogModules = SettingsListHelper.GetBlogModuleItems(PortalSettings.PortalId, ModuleContext.ModuleId),
        Blogs = SettingsListHelper.GetBlogItems(blogModuleId, User.UserID, Locale),
        Authors = SettingsListHelper.GetAuthorItems(blogModuleId),
        Templates = SettingsListHelper.GetTemplateItems(blogModuleId, PortalSettings, HttpContext),
        BlogModuleId = blogModuleIdOverride ?? vs.BlogModuleId,
        BlogId = vs.BlogId,
        Categories = vs.Categories,
        SelectedCategoryIds = selectedIds,
        CategoryTree = categoryTree,
        ShowCategories = categoryTree != null,
        AuthorId = vs.AuthorId,
        Template = vs.Template,
        ShowAllLocales = vs.ShowAllLocales,
        ModifyPageDetails = vs.ModifyPageDetails,
        AddCanonicalTag = vs.AddCanonicalTag,
        ShowManagementPanel = vs.ShowManagementPanel,
        ShowManagementPanelViewMode = vs.ShowManagementPanelViewMode,
        HideUnpublishedBlogsViewMode = vs.HideUnpublishedBlogsViewMode,
        HideUnpublishedBlogsEditMode = vs.HideUnpublishedBlogsEditMode,
        AllowComments = vs.AllowComments
      };
    }

    private List<PostCategoryNode> BuildCategoryTreeForModule(int blogModuleId, List<int> selectedIds)
    {
      int moduleId = blogModuleId > -1 ? blogModuleId : ModuleContext.ModuleId;
      var moduleSettings = ModuleSettings.GetSettings(moduleId);
      if (moduleSettings == null || moduleSettings.VocabularyId <= 0)
      {
        _logger.LogDebug("No category vocabulary for module {ModuleId}", moduleId);
        return null;
      }

      var vocab = CoreTermsController.GetTermsByVocabulary(moduleId, moduleSettings.VocabularyId, Locale);
      if (vocab == null || vocab.Count == 0)
        return new List<PostCategoryNode>();

      return CategoryTreeHelper.BuildCategoryTree(vocab.Values, selectedIds);
    }

    private string Locale => System.Threading.Thread.CurrentThread.CurrentCulture.Name;
  }
}
