using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Entities.Content.Common;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Core.Data;
using DotNetNuke.Modules.Blog.Core.Entities.Terms;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Api;
using CoreTerms = DotNetNuke.Modules.Blog.Core.Entities.Terms.TermsController;

namespace DotNetNuke.Modules.Blog.Api
{
    public class CategoriesController : BlogApiController
    {
        public class CategoryLocaleDto
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

        public class CategoryNodeDto
        {
            public int TermId { get; set; }
            public string Name { get; set; }
            public int ParentTermId { get; set; }
            public int ViewOrder { get; set; }
            public Dictionary<string, string> Localizations { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            public List<CategoryNodeDto> Children { get; set; } = new List<CategoryNodeDto>();
        }

        public class CategoryTreeDto
        {
            public List<CategoryNodeDto> Items { get; set; } = new List<CategoryNodeDto>();
            public string DefaultLocale { get; set; }
            public List<CategoryLocaleDto> Locales { get; set; } = new List<CategoryLocaleDto>();
            public bool IsMultilingual { get; set; }
        }

        public class CategoryAddDto
        {
            public string Lines { get; set; }
            public int ParentTermId { get; set; } = -1;
        }

        public class CategoryUpdateDto
        {
            public int TermId { get; set; }
            public string Name { get; set; }
            public int? ParentTermId { get; set; }
            public int? ViewOrder { get; set; }
        }

        public class CategoryDeleteDto
        {
            public int TermId { get; set; }
        }

        public class CategoryLocalizeDto
        {
            public int TermId { get; set; }
            public string Name { get; set; }
            public Dictionary<string, string> Localizations { get; set; }
        }

        [HttpGet]
        [BlogAuthorize(SecurityAccessLevel.Admin)]
        public HttpResponseMessage Tree()
        {
            if (!TryGetVocabularyId(out int vocabularyId))
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "No vocabulary configured." });

            var terms = CoreTerms.GetTermsByVocabulary(BlogModuleContext.Settings.ModuleId, vocabularyId);
            var portalLocales = LocaleController.Instance.GetLocales(PortalSettings.PortalId);
            var defaultLocale = PortalSettings.DefaultLanguage;
            var otherLocales = portalLocales.Values
                .Where(l => l.Code != defaultLocale)
                .OrderBy(l => l.Code)
                .Select(l => new CategoryLocaleDto { Code = l.Code, Name = l.EnglishName ?? l.Code })
                .ToList();

            var roots = BuildTree(terms, -1, otherLocales.Select(l => l.Code).ToList());
            return Request.CreateResponse(HttpStatusCode.OK, new CategoryTreeDto
            {
                Items = roots,
                DefaultLocale = defaultLocale,
                Locales = otherLocales,
                IsMultilingual = portalLocales.Count > 1
            });
        }

        [HttpPost]
        [BlogAuthorize(SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage Add(CategoryAddDto data)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.Lines))
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
            if (!TryGetVocabularyId(out int vocabularyId))
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "No vocabulary configured." });

            var parentId = data.ParentTermId < 1 ? -1 : data.ParentTermId;
            if (parentId > 0 && !TermExists(vocabularyId, parentId))
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "Parent not found." });

            var parsed = ParseLines(data.Lines);
            if (parsed.Count == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "No categories to add." });

            var terms = CoreTerms.GetTermsByVocabulary(BlogModuleContext.Settings.ModuleId, vocabularyId);
            var siblingCount = CountChildren(terms, parentId);
            var depthParents = new Dictionary<int, int> { [-1] = parentId };
            int viewOrderBase = siblingCount;

            foreach (var item in parsed)
            {
                int depth = item.Depth;
                int parentForDepth = depth == 0
                    ? parentId
                    : (depthParents.ContainsKey(depth - 1) ? depthParents[depth - 1] : parentId);

                int order;
                if (depth == 0)
                {
                    viewOrderBase++;
                    order = viewOrderBase;
                }
                else
                {
                    order = CountChildren(CoreTerms.GetTermsByVocabulary(BlogModuleContext.Settings.ModuleId, vocabularyId), parentForDepth) + 1;
                }

                int newId = DataProvider.Instance().SetTerm(
                    -1, vocabularyId, parentForDepth, order, item.Name, "", UserInfo.UserID);
                depthParents[depth] = newId;
            }

            ClearCache(vocabularyId);
            return Tree();
        }

        [HttpPost]
        [BlogAuthorize(SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage Update(CategoryUpdateDto data)
        {
            if (data == null || data.TermId < 1)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
            if (!TryGetVocabularyId(out int vocabularyId))
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });

            var terms = CoreTerms.GetTermsByVocabulary(BlogModuleContext.Settings.ModuleId, vocabularyId);
            if (!terms.TryGetValue(data.TermId, out TermInfo term))
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "Term not found." });

            string name = string.IsNullOrWhiteSpace(data.Name) ? term.Name : data.Name.Trim();
            if (string.IsNullOrEmpty(name))
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "Name is required." });

            int parentId = data.ParentTermId.HasValue
                ? (data.ParentTermId.Value < 1 ? -1 : data.ParentTermId.Value)
                : (term.ParentTermId.HasValue && term.ParentTermId.Value > 0 ? term.ParentTermId.Value : -1);

            if (parentId == data.TermId)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "Cannot parent a term to itself." });
            if (parentId > 0 && IsDescendantOf(terms, parentId, data.TermId))
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "Cannot move under a descendant." });

            int viewOrder = data.ViewOrder ?? GetViewOrder(terms, data.TermId, parentId);

            DataProvider.Instance().SetTerm(
                data.TermId, vocabularyId, parentId, viewOrder, name, term.Description ?? "", UserInfo.UserID);

            ClearCache(vocabularyId);
            return Tree();
        }

        [HttpPost]
        [BlogAuthorize(SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage Delete(CategoryDeleteDto data)
        {
            if (data == null || data.TermId < 1)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
            if (!TryGetVocabularyId(out int vocabularyId))
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });

            var terms = CoreTerms.GetTermsByVocabulary(BlogModuleContext.Settings.ModuleId, vocabularyId);
            if (!terms.ContainsKey(data.TermId))
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "Term not found." });

            var toDelete = new List<TermInfo>();
            CollectSubtree(terms, data.TermId, toDelete);
            // Delete deepest first
            foreach (var t in toDelete.OrderByDescending(x => x.TermId))
                Util.GetTermController().DeleteTerm(t);

            ClearCache(vocabularyId);
            return Tree();
        }

        [HttpPost]
        [BlogAuthorize(SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage Localize(CategoryLocalizeDto data)
        {
            if (data == null || data.TermId < 1)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
            if (!TryGetVocabularyId(out int vocabularyId))
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });

            var terms = CoreTerms.GetTermsByVocabulary(BlogModuleContext.Settings.ModuleId, vocabularyId);
            if (!terms.TryGetValue(data.TermId, out TermInfo term))
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "Term not found." });

            if (!string.IsNullOrWhiteSpace(data.Name) && data.Name.Trim() != term.Name)
            {
                int parentId = term.ParentTermId.HasValue && term.ParentTermId.Value > 0 ? term.ParentTermId.Value : -1;
                DataProvider.Instance().SetTerm(
                    data.TermId, vocabularyId, parentId, GetViewOrder(terms, data.TermId, parentId),
                    data.Name.Trim(), term.Description ?? "", UserInfo.UserID);
            }

            if (data.Localizations != null)
            {
                foreach (var kv in data.Localizations)
                {
                    if (string.IsNullOrWhiteSpace(kv.Key))
                        continue;
                    DataProvider.Instance().SetTermLocalization(data.TermId, kv.Key, kv.Value ?? "", "");
                }
            }

            ClearCache(vocabularyId);
            return Tree();
        }

        private bool TryGetVocabularyId(out int vocabularyId)
        {
            vocabularyId = BlogModuleContext.Settings.VocabularyId;
            return vocabularyId > -1;
        }

        private void ClearCache(int vocabularyId)
        {
            CoreTerms.GetTermsByVocabulary(BlogModuleContext.Settings.ModuleId, vocabularyId, "", true);
            CoreTerms.GetTermsByVocabulary(BlogModuleContext.Settings.ModuleId, vocabularyId);
        }

        private bool TermExists(int vocabularyId, int termId)
        {
            var terms = CoreTerms.GetTermsByVocabulary(BlogModuleContext.Settings.ModuleId, vocabularyId);
            return terms.ContainsKey(termId);
        }

        private static List<CategoryNodeDto> BuildTree(Dictionary<int, TermInfo> terms, int parentId, List<string> localeCodes)
        {
            IEnumerable<TermInfo> children;
            if (parentId < 1)
                children = terms.Values.Where(t => !t.ParentTermId.HasValue || t.ParentTermId.Value < 1);
            else
                children = terms.Values.Where(t => t.ParentTermId.HasValue && t.ParentTermId.Value == parentId);

            var ordered = children.OrderBy(t => t.Weight).ThenBy(t => t.Name).ToList();
            var result = new List<CategoryNodeDto>();
            int order = 0;
            foreach (var t in ordered)
            {
                order++;
                var node = new CategoryNodeDto
                {
                    TermId = t.TermId,
                    Name = t.Name ?? "",
                    ParentTermId = t.ParentTermId.HasValue && t.ParentTermId.Value > 0 ? t.ParentTermId.Value : -1,
                    ViewOrder = t.Weight > 0 ? t.Weight : order,
                    Children = BuildTree(terms, t.TermId, localeCodes)
                };
                foreach (var code in localeCodes)
                    node.Localizations[code] = t.NameLocalizations[code] ?? "";
                result.Add(node);
            }
            return result;
        }

        private static int CountChildren(Dictionary<int, TermInfo> terms, int parentId)
        {
            if (parentId < 1)
                return terms.Values.Count(t => !t.ParentTermId.HasValue || t.ParentTermId.Value < 1);
            return terms.Values.Count(t => t.ParentTermId.HasValue && t.ParentTermId.Value == parentId);
        }

        private static int GetViewOrder(Dictionary<int, TermInfo> terms, int termId, int parentId)
        {
            if (terms.TryGetValue(termId, out TermInfo existing) && existing.Weight > 0)
                return existing.Weight;

            var siblings = parentId < 1
                ? terms.Values.Where(t => !t.ParentTermId.HasValue || t.ParentTermId.Value < 1).OrderBy(t => t.Weight).ThenBy(t => t.Name).ToList()
                : terms.Values.Where(t => t.ParentTermId.HasValue && t.ParentTermId.Value == parentId).OrderBy(t => t.Weight).ThenBy(t => t.Name).ToList();
            int idx = siblings.FindIndex(t => t.TermId == termId);
            return idx >= 0 ? idx + 1 : siblings.Count + 1;
        }

        private static bool IsDescendantOf(Dictionary<int, TermInfo> terms, int candidateParentId, int ancestorId)
        {
            int current = candidateParentId;
            var guard = 0;
            while (current > 0 && guard++ < 1000)
            {
                if (current == ancestorId)
                    return true;
                if (!terms.TryGetValue(current, out TermInfo t))
                    return false;
                current = t.ParentTermId.HasValue && t.ParentTermId.Value > 0 ? t.ParentTermId.Value : -1;
            }
            return false;
        }

        private static void CollectSubtree(Dictionary<int, TermInfo> terms, int termId, List<TermInfo> bucket)
        {
            if (!terms.TryGetValue(termId, out TermInfo term))
                return;
            foreach (var child in terms.Values.Where(t => t.ParentTermId.HasValue && t.ParentTermId.Value == termId))
                CollectSubtree(terms, child.TermId, bucket);
            bucket.Add(term);
        }

        private class ParsedLine
        {
            public int Depth { get; set; }
            public string Name { get; set; }
        }

        private static List<ParsedLine> ParseLines(string lines)
        {
            var result = new List<ParsedLine>();
            foreach (var raw in lines.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n'))
            {
                if (string.IsNullOrWhiteSpace(raw))
                    continue;
                int depth = 0;
                int i = 0;
                while (i < raw.Length && raw[i] == '-')
                {
                    depth++;
                    i++;
                }
                while (i < raw.Length && char.IsWhiteSpace(raw[i]))
                    i++;
                string name = raw.Substring(i).Trim();
                if (string.IsNullOrEmpty(name))
                    continue;
                result.Add(new ParsedLine { Depth = depth, Name = name });
            }
            return result;
        }
    }
}
