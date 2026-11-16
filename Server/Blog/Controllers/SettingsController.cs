using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Modules.Blog.Common;
using System.Web.Mvc;

namespace DotNetNuke.Modules.Blog.Controllers
{
    [DnnModuleAuthorize(AccessLevel = DotNetNuke.Security.SecurityAccessLevel.Edit)]
    [DnnHandleError]
    public class SettingsController : BlogMvcController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Settings()
        {
            return View(BlogModuleContext.Settings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supportsTokens"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Settings(ModuleSettings settings)
        {
            settings.SaveSettings(ModuleContext.Configuration);
            return RedirectToDefaultRoute();
        }
    }
}