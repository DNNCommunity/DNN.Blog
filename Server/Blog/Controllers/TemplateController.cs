using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Core.Templating;
using DotNetNuke.Modules.Blog.Models;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Web.Mvc;

namespace DotNetNuke.Modules.Blog.Controllers
{
  [DnnModuleAuthorize(AccessLevel = Security.SecurityAccessLevel.Edit)]
  [DnnHandleError]
  public class TemplateController : BlogMvcController
  {
    private readonly ILogger<TemplateController> _logger;

    public TemplateController(ILogger<TemplateController> logger) : base()
    {
      _logger = logger;
    }

    [HttpGet]
    public ActionResult Settings()
    {
      _logger.LogDebug("Template Settings GET: tabModuleId {TabModuleId}, user {UserId}", ModuleContext.TabModuleId, User.UserID);
      var model = BuildViewModel();
      _logger.LogInformation("Template settings page rendered: template {Template}, settingCount {Count}",
          model.TemplateName, model.Settings?.Count ?? 0);
      return View(model);
    }

    [HttpPost]
    [ValidateInput(false)]
    [Web.Api.ValidateAntiForgeryToken]
    public ActionResult Settings(FormCollection form)
    {
      _logger.LogDebug("Template Settings POST: tabModuleId {TabModuleId}, user {UserId}", ModuleContext.TabModuleId, User.UserID);
      var viewSettings = BlogModuleContext.ViewSettings;
      var tmgr = new TemplateManager(PortalSettings, viewSettings.Template);

      int updated = 0;
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
        updated++;
        _logger.LogDebug("Template setting {Key} updated", ts.Key);
      }

      viewSettings.SaveTemplateSettings();
      _logger.LogInformation("Template settings saved for template {Template}: {Count} values updated",
          viewSettings.Template, updated);
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
