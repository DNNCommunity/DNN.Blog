using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DotNetNuke.Entities.Content.Common;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Core.Data;
using CoreTermsController = DotNetNuke.Modules.Blog.Core.Entities.Terms.TermsController;
using DotNetNuke.Modules.Blog.Core.Entities.Terms;
using DotNetNuke.Modules.Blog.Models;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using Microsoft.Extensions.Logging;

namespace DotNetNuke.Modules.Blog.Controllers
{
  [DnnHandleError]
  public class TermsController : BlogMvcController
  {
    private readonly ILogger<TermsController> _logger;

    public TermsController(ILogger<TermsController> logger) : base()
    {
      _logger = logger;
    }

    [HttpGet]
    public ActionResult EditML(int? VocabularyId)
    {
      _logger.LogDebug("Terms EditML GET: vocabularyId {VocabularyId}, user {UserId}", VocabularyId, User.UserID);
      BlogModuleContext.RequirePermissionLevel(BlogModuleContext.Security.IsEditor);

      int vocabularyId = VocabularyId ?? 1;
      var settings = BlogModuleContext.Settings;
      if (vocabularyId != settings.VocabularyId && vocabularyId != 1)
      {
        _logger.LogDebug("Vocabulary {Requested} not allowed; falling back to tags vocabulary 1", vocabularyId);
        vocabularyId = 1;
      }

      var terms = CoreTermsController.GetTermsByVocabulary(ModuleContext.ModuleId, vocabularyId);
      var locales = LocaleController.Instance.GetLocales(PortalSettings.PortalId).Values
          .Where(l => l.Code != PortalSettings.DefaultLanguage)
          .ToList();

      var rows = new List<TermRowViewModel>();
      foreach (var t in terms.Values.OrderBy(x => x.Name))
      {
        var row = new TermRowViewModel { TermId = t.TermId, DefaultName = t.Name };
        foreach (var loc in locales)
          row.Localizations[loc.Code] = t.LocalizedName ?? "";
        rows.Add(row);
      }

      _logger.LogInformation("Terms EditML rendered: vocabulary {VocabularyId}, termCount {TermCount}, localeCount {LocaleCount}",
          vocabularyId, rows.Count, locales.Count);
      return View(new TermsEditViewModel { VocabularyId = vocabularyId, Terms = rows });
    }

    [HttpPost]
    [Web.Api.ValidateAntiForgeryToken]
    public ActionResult EditML(int VocabularyId, FormCollection form)
    {
      _logger.LogDebug("Terms EditML POST: vocabularyId {VocabularyId}, user {UserId}", VocabularyId, User.UserID);
      BlogModuleContext.RequirePermissionLevel(BlogModuleContext.Security.IsEditor);

      if (VocabularyId != BlogModuleContext.Settings.VocabularyId && VocabularyId != 1)
      {
        _logger.LogDebug("Vocabulary {Requested} not allowed; falling back to tags vocabulary 1", VocabularyId);
        VocabularyId = 1;
      }

      var current = CoreTermsController.GetTermsByVocabulary(ModuleContext.ModuleId, VocabularyId);
      int nameUpdates = 0;
      int localizationUpdates = 0;
      foreach (string key in form.AllKeys)
      {
        if (!key.StartsWith("term_"))
          continue;
        var parts = key.Split('_');
        if (parts.Length < 3 || !int.TryParse(parts[1], out int termId))
          continue;

        string field = parts[2];
        string value = form[key];
        if (!current.TryGetValue(termId, out TermInfo term))
          continue;

        if (field == "name" && term.Name != value)
        {
          term.Name = value;
          Util.GetTermController().UpdateTerm(term);
          nameUpdates++;
          _logger.LogDebug("Updated term {TermId} default name", termId);
        }
        else if (field.StartsWith("loc_"))
        {
          string locale = field.Substring(4);
          DataProvider.Instance().SetTermLocalization(termId, locale, value, "");
          localizationUpdates++;
          _logger.LogDebug("Updated term {TermId} localization {Locale}", termId, locale);
        }
      }

      _logger.LogInformation("Terms EditML saved: vocabulary {VocabularyId}, nameUpdates {NameUpdates}, localizationUpdates {LocUpdates}",
          VocabularyId, nameUpdates, localizationUpdates);
      return Redirect(Url.Action("Index", "Admin"));
    }
  }
}
