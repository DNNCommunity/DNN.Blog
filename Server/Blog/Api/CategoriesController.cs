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
using DotNetNuke.Web.Api;
using Microsoft.Extensions.Logging;
using CoreTerms = DotNetNuke.Modules.Blog.Core.Entities.Terms.TermsController;

namespace DotNetNuke.Modules.Blog.Api
{
  public class CategoriesController : BlogApiController
  {
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ILogger<CategoriesController> logger) : base()
    {
      _logger = logger;
    }

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
      var moduleId = BlogModuleContext.Settings.ModuleId;
      _logger.LogDebug("Tree requested by user {UserId} for module {ModuleId}", UserInfo.UserID, moduleId);

      var tree = new CategoryTreeService().GetTree(
          moduleId,
          PortalSettings.PortalId,
          PortalSettings.DefaultLanguage);

      if (tree.NoVocabulary)
      {
        _logger.LogWarning("Tree failed: no vocabulary configured for module {ModuleId}", moduleId);
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "No vocabulary configured." });
      }

      var roots = MapNodes(tree.Items);
      _logger.LogInformation("Tree returned {RootCount} roots, {TermCount} terms, vocabulary {VocabularyId}",
          roots.Count, tree.TermCount, tree.VocabularyId);
      return Request.CreateResponse(HttpStatusCode.OK, new CategoryTreeDto
      {
        Items = roots,
        DefaultLocale = tree.DefaultLocale,
        Locales = tree.OtherLocales.Select(l => new CategoryLocaleDto { Code = l.Code, Name = l.Name }).ToList(),
        IsMultilingual = tree.IsMultilingual
      });
    }

    private static List<CategoryNodeDto> MapNodes(List<CategoryTreeNode> nodes)
    {
      return nodes.Select(n => new CategoryNodeDto
      {
        TermId = n.CategoryId,
        Name = n.Name ?? "",
        ParentTermId = n.ParentTermId,
        ViewOrder = n.ViewOrder,
        Localizations = n.Localizations ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
        Children = MapNodes(n.Children ?? new List<CategoryTreeNode>())
      }).ToList();
    }

    [HttpPost]
    [BlogAuthorize(SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public HttpResponseMessage Add(CategoryAddDto data)
    {
      _logger.LogDebug("Add categories requested by user {UserId}, parentTermId {ParentTermId}", UserInfo.UserID, data?.ParentTermId);

      if (data == null || string.IsNullOrWhiteSpace(data.Lines))
      {
        _logger.LogWarning("Add categories failed: empty payload");
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
      }
      if (!TryGetVocabularyId(out int vocabularyId))
      {
        _logger.LogWarning("Add categories failed: no vocabulary configured for module {ModuleId}", BlogModuleContext.Settings.ModuleId);
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "No vocabulary configured." });
      }

      var parentId = data.ParentTermId < 1 ? -1 : data.ParentTermId;
      if (parentId > 0 && !TermExists(vocabularyId, parentId))
      {
        _logger.LogWarning("Add categories failed: parent {ParentTermId} not found in vocabulary {VocabularyId}", parentId, vocabularyId);
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "Parent not found." });
      }

      var parsed = ParseLines(data.Lines);
      if (parsed.Count == 0)
      {
        _logger.LogWarning("Add categories failed: no parseable lines");
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "No categories to add." });
      }

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
        _logger.LogDebug("Created category {TermId} name {Name} depth {Depth} parent {ParentId}", newId, item.Name, depth, parentForDepth);
        depthParents[depth] = newId;
      }

      ClearCache(vocabularyId);
      _logger.LogInformation("Added {Count} categories under parent {ParentTermId} in vocabulary {VocabularyId} by user {UserId}",
          parsed.Count, parentId, vocabularyId, UserInfo.UserID);
      return Tree();
    }

    [HttpPost]
    [BlogAuthorize(SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public HttpResponseMessage Update(CategoryUpdateDto data)
    {
      _logger.LogDebug("Update category requested: termId {TermId}, user {UserId}", data?.TermId, UserInfo.UserID);

      if (data == null || data.TermId < 1)
      {
        _logger.LogWarning("Update category failed: invalid payload");
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
      }
      if (!TryGetVocabularyId(out int vocabularyId))
      {
        _logger.LogWarning("Update category failed: no vocabulary configured");
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
      }

      var terms = CoreTerms.GetTermsByVocabulary(BlogModuleContext.Settings.ModuleId, vocabularyId);
      if (!terms.TryGetValue(data.TermId, out TermInfo term))
      {
        _logger.LogWarning("Update category failed: term {TermId} not found", data.TermId);
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "Term not found." });
      }

      string name = string.IsNullOrWhiteSpace(data.Name) ? term.Name : data.Name.Trim();
      if (string.IsNullOrEmpty(name))
      {
        _logger.LogWarning("Update category failed: empty name for term {TermId}", data.TermId);
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "Name is required." });
      }

      int parentId = data.ParentTermId.HasValue
          ? (data.ParentTermId.Value < 1 ? -1 : data.ParentTermId.Value)
          : (term.ParentTermId.HasValue && term.ParentTermId.Value > 0 ? term.ParentTermId.Value : -1);

      if (parentId == data.TermId)
      {
        _logger.LogWarning("Update category failed: term {TermId} cannot parent itself", data.TermId);
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "Cannot parent a term to itself." });
      }
      if (parentId > 0 && IsDescendantOf(terms, parentId, data.TermId))
      {
        _logger.LogWarning("Update category failed: cannot move term {TermId} under descendant {ParentId}", data.TermId, parentId);
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "Cannot move under a descendant." });
      }

      int viewOrder = data.ViewOrder ?? GetViewOrder(terms, data.TermId, parentId);

      DataProvider.Instance().SetTerm(
          data.TermId, vocabularyId, parentId, viewOrder, name, term.Description ?? "", UserInfo.UserID);

      ClearCache(vocabularyId);
      _logger.LogInformation("Updated category {TermId} to name {Name}, parent {ParentId}, viewOrder {ViewOrder}",
          data.TermId, name, parentId, viewOrder);
      return Tree();
    }

    [HttpPost]
    [BlogAuthorize(SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public HttpResponseMessage Delete(CategoryDeleteDto data)
    {
      _logger.LogDebug("Delete category requested: termId {TermId}, user {UserId}", data?.TermId, UserInfo.UserID);

      if (data == null || data.TermId < 1)
      {
        _logger.LogWarning("Delete category failed: invalid payload");
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
      }
      if (!TryGetVocabularyId(out int vocabularyId))
      {
        _logger.LogWarning("Delete category failed: no vocabulary configured");
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
      }

      var terms = CoreTerms.GetTermsByVocabulary(BlogModuleContext.Settings.ModuleId, vocabularyId);
      if (!terms.ContainsKey(data.TermId))
      {
        _logger.LogWarning("Delete category failed: term {TermId} not found", data.TermId);
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "Term not found." });
      }

      var toDelete = new List<TermInfo>();
      CollectSubtree(terms, data.TermId, toDelete);
      // Delete deepest first
      foreach (var t in toDelete.OrderByDescending(x => x.TermId))
      {
        _logger.LogDebug("Deleting category term {TermId} ({Name})", t.TermId, t.Name);
        Util.GetTermController().DeleteTerm(t);
      }

      ClearCache(vocabularyId);
      _logger.LogInformation("Deleted category subtree rooted at {TermId} ({Count} terms) by user {UserId}",
          data.TermId, toDelete.Count, UserInfo.UserID);
      return Tree();
    }

    [HttpPost]
    [BlogAuthorize(SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public HttpResponseMessage Localize(CategoryLocalizeDto data)
    {
      _logger.LogDebug("Localize category requested: termId {TermId}, user {UserId}", data?.TermId, UserInfo.UserID);

      if (data == null || data.TermId < 1)
      {
        _logger.LogWarning("Localize category failed: invalid payload");
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
      }
      if (!TryGetVocabularyId(out int vocabularyId))
      {
        _logger.LogWarning("Localize category failed: no vocabulary configured");
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
      }

      var terms = CoreTerms.GetTermsByVocabulary(BlogModuleContext.Settings.ModuleId, vocabularyId);
      if (!terms.TryGetValue(data.TermId, out TermInfo term))
      {
        _logger.LogWarning("Localize category failed: term {TermId} not found", data.TermId);
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error", Message = "Term not found." });
      }

      if (!string.IsNullOrWhiteSpace(data.Name) && data.Name.Trim() != term.Name)
      {
        int parentId = term.ParentTermId.HasValue && term.ParentTermId.Value > 0 ? term.ParentTermId.Value : -1;
        DataProvider.Instance().SetTerm(
            data.TermId, vocabularyId, parentId, GetViewOrder(terms, data.TermId, parentId),
            data.Name.Trim(), term.Description ?? "", UserInfo.UserID);
        _logger.LogDebug("Updated default name for term {TermId} to {Name}", data.TermId, data.Name.Trim());
      }

      var localizationCount = 0;
      if (data.Localizations != null)
      {
        foreach (var kv in data.Localizations)
        {
          if (string.IsNullOrWhiteSpace(kv.Key))
            continue;
          DataProvider.Instance().SetTermLocalization(data.TermId, kv.Key, kv.Value ?? "", "");
          localizationCount++;
          _logger.LogDebug("Set localization {Locale} for term {TermId}", kv.Key, data.TermId);
        }
      }

      ClearCache(vocabularyId);
      _logger.LogInformation("Localized category {TermId} with {Count} locale entries by user {UserId}",
          data.TermId, localizationCount, UserInfo.UserID);
      return Tree();
    }

    private bool TryGetVocabularyId(out int vocabularyId)
    {
      vocabularyId = BlogModuleContext.Settings.VocabularyId;
      return vocabularyId > -1;
    }

    private void ClearCache(int vocabularyId)
    {
      _logger.LogDebug("Clearing term cache for vocabulary {VocabularyId}, module {ModuleId}", vocabularyId, BlogModuleContext.Settings.ModuleId);
      CoreTerms.GetTermsByVocabulary(BlogModuleContext.Settings.ModuleId, vocabularyId, "", true);
      CoreTerms.GetTermsByVocabulary(BlogModuleContext.Settings.ModuleId, vocabularyId);
    }

    private bool TermExists(int vocabularyId, int termId)
    {
      var terms = CoreTerms.GetTermsByVocabulary(BlogModuleContext.Settings.ModuleId, vocabularyId);
      return terms.ContainsKey(termId);
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
