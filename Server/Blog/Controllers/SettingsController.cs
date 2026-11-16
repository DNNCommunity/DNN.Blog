using System.Linq;
using System.Web.Mvc;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Core.Common;
using DotNetNuke.Modules.Blog.Models;
using DnnHandleErrorAttribute = DotNetNuke.Web.Mvc.Framework.ActionFilters.DnnHandleErrorAttribute;

namespace DotNetNuke.Modules.Blog.Controllers
{
    [DotNetNuke.Web.Mvc.Framework.ActionFilters.DnnModuleAuthorize(AccessLevel = DotNetNuke.Security.SecurityAccessLevel.Edit)]
    [DnnHandleErrorAttribute]
    public class SettingsController : BlogMvcController
    {
        [HttpGet]
        public ActionResult Settings(int? blogModuleId)
        {
            return View(BuildViewModel(blogModuleId));
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Settings(SettingsViewModel model)
        {
            var vs = ViewSettings.GetViewSettings(ModuleContext.TabModuleId);
            vs.BlogModuleId = model.BlogModuleId;
            vs.BlogId = model.BlogId;
            vs.Categories = model.Categories ?? "";
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
            return RedirectToDefaultRoute();
        }

        private SettingsViewModel BuildViewModel(int? blogModuleIdOverride)
        {
            var vs = ViewSettings.GetViewSettings(ModuleContext.TabModuleId);
            int blogModuleId = blogModuleIdOverride ?? SettingsListHelper.ResolveBlogModuleId(vs, ModuleContext.ModuleId);

            return new SettingsViewModel
            {
                Settings = vs,
                BlogModules = SettingsListHelper.GetBlogModuleItems(PortalSettings.PortalId, ModuleContext.ModuleId),
                Blogs = SettingsListHelper.GetBlogItems(blogModuleId, User.UserID, Locale),
                Authors = SettingsListHelper.GetAuthorItems(blogModuleId),
                Templates = SettingsListHelper.GetTemplateItems(blogModuleId, PortalSettings, HttpContext),
                BlogModuleId = vs.BlogModuleId,
                BlogId = vs.BlogId,
                Categories = vs.Categories,
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

        private string Locale => System.Threading.Thread.CurrentThread.CurrentCulture.Name;
    }
}
