using System.Linq;
using System.Web.Mvc;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Core.Templating;
using DotNetNuke.Modules.Blog.Models;
using DotNetNuke.Services.Localization;
using DnnHandleErrorAttribute = DotNetNuke.Web.Mvc.Framework.ActionFilters.DnnHandleErrorAttribute;

namespace DotNetNuke.Modules.Blog.Controllers
{
    [DotNetNuke.Web.Mvc.Framework.ActionFilters.DnnModuleAuthorize(AccessLevel = DotNetNuke.Security.SecurityAccessLevel.Edit)]
    [DnnHandleErrorAttribute]
    public class TemplateController : BlogMvcController
    {
        [HttpGet]
        public ActionResult Settings()
        {
            return View(BuildViewModel());
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Settings(FormCollection form)
        {
            var viewSettings = BlogModuleContext.ViewSettings;
            var tmgr = new TemplateManager(PortalSettings, viewSettings.Template);

            foreach (var ts in tmgr.TemplateSettings.Settings)
            {
                string key = "ts_" + ts.Key;
                if (!form.AllKeys.Contains(key))
                    continue;

                string value = form[key];
                if (ts.ValueType.ToLower() == "truefalse")
                    value = (value == "true" || value == "on").ToString().ToLower();
                else if (ts.ValueType.ToLower() == "10")
                    value = (value == "true" || value == "on" || value == "1") ? "1" : "0";

                viewSettings.SetTemplateSetting(ts.Key, value ?? "");
            }

            viewSettings.SaveTemplateSettings();
            return RedirectToDefaultRoute();
        }

        private TemplateSettingsViewModel BuildViewModel()
        {
            var viewSettings = BlogModuleContext.ViewSettings;
            var tmgr = new TemplateManager(PortalSettings, viewSettings.Template);
            var rows = tmgr.TemplateSettings.Settings.Select(ts =>
            {
                string value = ts.DefaultValue;
                if (viewSettings.TemplateSettings.ContainsKey(ts.Key))
                    value = viewSettings.TemplateSettings[ts.Key];

                return new TemplateSettingRow
                {
                    Definition = ts,
                    Value = value,
                    Label = Localization.GetString(ts.Key + ".Text", tmgr.SharedResourcesFile),
                    Help = Localization.GetString(ts.Key + ".Help", tmgr.SharedResourcesFile)
                };
            }).ToList();

            return new TemplateSettingsViewModel
            {
                TemplateName = viewSettings.Template,
                Settings = rows,
                HasSettings = rows.Count > 0
            };
        }
    }
}
