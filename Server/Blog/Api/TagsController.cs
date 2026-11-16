using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Entities.Content.Common;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Core.Data;
using DotNetNuke.Modules.Blog.Core.Entities.Terms;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Api;
using Microsoft.Extensions.Logging;
using CoreTerms = DotNetNuke.Modules.Blog.Core.Entities.Terms.TermsController;

namespace DotNetNuke.Modules.Blog.Api
{
    public class TagsController : BlogApiController
    {
        private readonly ILogger<TagsController> _logger;
        private const int TagsVocabularyId = 1;

        public TagsController(ILogger<TagsController> logger) : base()
        {
            _logger = logger;
        }

        public class TagLocaleDto
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

        public class TagItemDto
        {
            public int TermId { get; set; }
            public string Name { get; set; }
            public Dictionary<string, string> Localizations { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public class TagListDto
        {
            public List<TagItemDto> Items { get; set; } = new List<TagItemDto>();
            public int Page { get; set; }
            public int PageSize { get; set; }
            public int Total { get; set; }
            public string DefaultLocale { get; set; }
            public List<TagLocaleDto> Locales { get; set; } = new List<TagLocaleDto>();
        }

        public class TagAddDto
        {
            public string Name { get; set; }
        }

        public class TagUpdateCellDto
        {
            public int TermId { get; set; }
            public string Locale { get; set; }
            public string Value { get; set; }
        }

        public class TagDeleteDto
        {
            public int TermId { get; set; }
        }

        private const string DisallowedTagCharacters = "%?*&;:'\\";

        /// <summary>Autocomplete for post edit (bloggers). Returns matching localized tag names.</summary>
        [HttpGet]
        [BlogAuthorize(SecurityAccessLevel.Edit)]
        public HttpResponseMessage Search(string term = null)
        {
            string searchString = (term ?? "").Trim();
            _logger.LogDebug("Tag search requested: term length {Length}, user {UserId}", searchString.Length, UserInfo.UserID);

            if (searchString.Length < 1)
            {
                _logger.LogDebug("Tag search skipped: empty term");
                return Request.CreateResponse(HttpStatusCode.OK, new string[0]);
            }

            var locale = BlogModuleContext.BlogContext?.Locale
                ?? System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            var terms = CoreTerms.GetTermsByVocabulary(
                BlogModuleContext.Settings.ModuleId, TagsVocabularyId, locale);

            char[] disallowed = DisallowedTagCharacters.ToCharArray();
            var matches = terms.Values
                .Select(t => t.LocalizedName ?? t.Name ?? "")
                .Where(name =>
                    name.Length > 0 &&
                    name.IndexOfAny(disallowed) < 0 &&
                    name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                .Distinct(StringComparer.CurrentCultureIgnoreCase)
                .OrderBy(n => n, StringComparer.CurrentCultureIgnoreCase)
                .Take(25)
                .ToList();

            _logger.LogInformation("Tag search for '{Search}' returned {Count} matches (locale {Locale})", searchString, matches.Count, locale);
            return Request.CreateResponse(HttpStatusCode.OK, matches);
        }

        [HttpGet]
        [BlogAuthorize(SecurityAccessLevel.Admin)]
        public HttpResponseMessage List(int page = 0, int pageSize = 25, string search = null)
        {
            _logger.LogDebug("Tag list requested: page {Page}, pageSize {PageSize}, search {HasSearch}", page, pageSize, !string.IsNullOrWhiteSpace(search));

            if (page < 0) page = 0;
            if (pageSize < 1) pageSize = 25;
            if (pageSize > 100) pageSize = 100;

            var localeCodes = GetOtherLocaleCodes();
            var terms = GetAllTags();
            IEnumerable<TermInfo> filtered = terms.Values.OrderBy(t => t.Name, StringComparer.CurrentCultureIgnoreCase);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var q = search.Trim();
                filtered = filtered.Where(t =>
                    (t.Name != null && t.Name.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    localeCodes.Any(code =>
                    {
                        var loc = t.NameLocalizations[code];
                        return !string.IsNullOrEmpty(loc) && loc.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0;
                    }));
            }

            var list = filtered.ToList();
            int total = list.Count;
            var pageItems = list.Skip(page * pageSize).Take(pageSize).Select(t => ToItem(t, localeCodes)).ToList();

            _logger.LogInformation("Tag list returned {ItemCount}/{Total} tags (page {Page})", pageItems.Count, total, page);
            return Request.CreateResponse(HttpStatusCode.OK, new TagListDto
            {
                Items = pageItems,
                Page = page,
                PageSize = pageSize,
                Total = total,
                DefaultLocale = PortalSettings.DefaultLanguage,
                Locales = GetOtherLocales()
            });
        }

        [HttpPost]
        [BlogAuthorize(SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage Add(TagAddDto data)
        {
            _logger.LogDebug("Add tag requested by user {UserId}", UserInfo.UserID);

            if (data == null)
            {
                _logger.LogWarning("Add tag failed: null payload");
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "Invalid request." });
            }

            string name = (data.Name ?? "").Trim();
            if (string.IsNullOrEmpty(name))
            {
                _logger.LogWarning("Add tag failed: empty name");
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "Tag name is required." });
            }

            var terms = GetAllTags();
            if (IsDuplicateDefaultName(terms, name, -1))
            {
                _logger.LogWarning("Add tag failed: duplicate name '{Name}'", name);
                return Request.CreateResponse(HttpStatusCode.Conflict, new { Result = "duplicate", Message = "A tag with this name already exists." });
            }

            int termId = Util.GetTermController().AddTerm(new Term(TagsVocabularyId) { Name = name });
            ClearCache();

            var created = CoreTerms.GetTermsByVocabulary(BlogModuleContext.Settings.ModuleId, TagsVocabularyId);
            TermInfo term;
            if (!created.TryGetValue(termId, out term))
                term = new TermInfo(name) { TermId = termId };

            _logger.LogInformation("Added tag {TermId} '{Name}' by user {UserId}", termId, name, UserInfo.UserID);
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Result = "success",
                Item = ToItem(term, GetOtherLocaleCodes())
            });
        }

        [HttpPost]
        [BlogAuthorize(SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateCell(TagUpdateCellDto data)
        {
            _logger.LogDebug("UpdateCell tag requested: termId {TermId}, locale '{Locale}', user {UserId}",
                data?.TermId, data?.Locale, UserInfo.UserID);

            if (data == null || data.TermId < 1)
            {
                _logger.LogWarning("UpdateCell failed: invalid payload");
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "Invalid request." });
            }

            var terms = GetAllTags();
            if (!terms.TryGetValue(data.TermId, out TermInfo term))
            {
                _logger.LogWarning("UpdateCell failed: tag {TermId} not found", data.TermId);
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "Tag not found." });
            }

            string locale = (data.Locale ?? "").Trim();
            string value = data.Value ?? "";

            if (string.IsNullOrEmpty(locale))
            {
                string name = value.Trim();
                if (string.IsNullOrEmpty(name))
                {
                    _logger.LogWarning("UpdateCell failed: empty default name for term {TermId}", data.TermId);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "Tag name is required." });
                }

                if (IsDuplicateDefaultName(terms, name, data.TermId))
                {
                    _logger.LogWarning("UpdateCell failed: duplicate default name '{Name}' for term {TermId}", name, data.TermId);
                    return Request.CreateResponse(HttpStatusCode.Conflict, new { Result = "duplicate", Message = "A tag with this name already exists." });
                }

                if (!string.Equals(term.Name, name, StringComparison.Ordinal))
                {
                    term.Name = name;
                    Util.GetTermController().UpdateTerm(term);
                    ClearCache();
                    _logger.LogInformation("Updated tag {TermId} default name to '{Name}'", data.TermId, name);
                }
                else
                {
                    _logger.LogDebug("UpdateCell skipped: default name unchanged for term {TermId}", data.TermId);
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { Result = "success", TermId = data.TermId, Locale = "", Value = name });
            }

            string locValue = value.Trim();
            if (!string.IsNullOrEmpty(locValue) && IsDuplicateLocalization(terms, locale, locValue, data.TermId))
            {
                _logger.LogWarning("UpdateCell failed: duplicate localization '{Locale}'='{Value}' for term {TermId}", locale, locValue, data.TermId);
                return Request.CreateResponse(HttpStatusCode.Conflict, new { Result = "duplicate", Message = "Another tag already uses this translation." });
            }

            DataProvider.Instance().SetTermLocalization(data.TermId, locale, locValue, "");
            ClearCache();

            _logger.LogInformation("Updated tag {TermId} localization '{Locale}' to '{Value}'", data.TermId, locale, locValue);
            return Request.CreateResponse(HttpStatusCode.OK, new { Result = "success", TermId = data.TermId, Locale = locale, Value = locValue });
        }

        [HttpPost]
        [BlogAuthorize(SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage Delete(TagDeleteDto data)
        {
            _logger.LogDebug("Delete tag requested: termId {TermId}, user {UserId}", data?.TermId, UserInfo.UserID);

            if (data == null || data.TermId < 1)
            {
                _logger.LogWarning("Delete tag failed: invalid payload");
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "Invalid request." });
            }

            var terms = GetAllTags();
            if (!terms.TryGetValue(data.TermId, out TermInfo term))
            {
                _logger.LogWarning("Delete tag failed: tag {TermId} not found", data.TermId);
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "Tag not found." });
            }

            Util.GetTermController().DeleteTerm(term);
            ClearCache();
            _logger.LogInformation("Deleted tag {TermId} '{Name}' by user {UserId}", data.TermId, term.Name, UserInfo.UserID);
            return Request.CreateResponse(HttpStatusCode.OK, new { Result = "success" });
        }

        private Dictionary<int, TermInfo> GetAllTags()
        {
            return CoreTerms.GetTermsByVocabulary(BlogModuleContext.Settings.ModuleId, TagsVocabularyId);
        }

        private void ClearCache()
        {
            _logger.LogDebug("Clearing tags vocabulary cache for module {ModuleId}", BlogModuleContext.Settings.ModuleId);
            CoreTerms.GetTermsByVocabulary(BlogModuleContext.Settings.ModuleId, TagsVocabularyId, "", true);
            CoreTerms.GetTermsByVocabulary(BlogModuleContext.Settings.ModuleId, TagsVocabularyId);
        }

        private List<string> GetOtherLocaleCodes()
        {
            return GetOtherLocales().Select(l => l.Code).ToList();
        }

        private List<TagLocaleDto> GetOtherLocales()
        {
            return LocaleController.Instance.GetLocales(PortalSettings.PortalId).Values
                .Where(l => l.Code != PortalSettings.DefaultLanguage)
                .OrderBy(l => l.Code)
                .Select(l => new TagLocaleDto { Code = l.Code, Name = l.EnglishName ?? l.Code })
                .ToList();
        }

        private static TagItemDto ToItem(TermInfo t, List<string> localeCodes)
        {
            var item = new TagItemDto
            {
                TermId = t.TermId,
                Name = t.Name ?? ""
            };
            foreach (var code in localeCodes)
                item.Localizations[code] = t.NameLocalizations[code] ?? "";
            return item;
        }

        private static bool IsDuplicateDefaultName(Dictionary<int, TermInfo> terms, string name, int excludeTermId)
        {
            return terms.Values.Any(t =>
                t.TermId != excludeTermId &&
                string.Equals((t.Name ?? "").Trim(), name, StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsDuplicateLocalization(Dictionary<int, TermInfo> terms, string locale, string value, int excludeTermId)
        {
            return terms.Values.Any(t =>
                t.TermId != excludeTermId &&
                string.Equals((t.NameLocalizations[locale] ?? "").Trim(), value, StringComparison.OrdinalIgnoreCase));
        }
    }
}
