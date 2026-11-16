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
using DnnHandleErrorAttribute = DotNetNuke.Web.Mvc.Framework.ActionFilters.DnnHandleErrorAttribute;

namespace DotNetNuke.Modules.Blog.Controllers
{
    [DnnHandleErrorAttribute]
    public class TermsController : BlogMvcController
    {
        [HttpGet]
        public ActionResult EditML(int? VocabularyId)
        {
            BlogModuleContext.RequirePermissionLevel(BlogModuleContext.Security.IsEditor);

            int vocabularyId = VocabularyId ?? 1;
            var settings = BlogModuleContext.Settings;
            if (vocabularyId != settings.VocabularyId && vocabularyId != 1)
                vocabularyId = 1;

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

            return View(new TermsEditViewModel { VocabularyId = vocabularyId, Terms = rows });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditML(int VocabularyId, FormCollection form)
        {
            BlogModuleContext.RequirePermissionLevel(BlogModuleContext.Security.IsEditor);

            if (VocabularyId != BlogModuleContext.Settings.VocabularyId && VocabularyId != 1)
                VocabularyId = 1;

            var current = CoreTermsController.GetTermsByVocabulary(ModuleContext.ModuleId, VocabularyId);
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
                }
                else if (field.StartsWith("loc_"))
                {
                    string locale = field.Substring(4);
                    DataProvider.Instance().SetTermLocalization(termId, locale, value, "");
                }
            }

            return Redirect(Url.Action("Index", "Admin"));
        }
    }
}
