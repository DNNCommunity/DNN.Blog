using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Modules.Blog.Core.Common;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.Blog.Core.Entities.Terms
{
  public class CategoryTreeNode
  {
    public int CategoryId { get; set; }
    public string Name { get; set; }
    public int ParentTermId { get; set; }
    public int ViewOrder { get; set; }
    public Dictionary<string, string> Localizations { get; set; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public List<CategoryTreeNode> Children { get; set; } = new List<CategoryTreeNode>();
  }

  public class CategoryLocaleInfo
  {
    public string Code { get; set; }
    public string Name { get; set; }
  }

  public class CategoryTreeResult
  {
    public bool NoVocabulary { get; set; }
    public List<CategoryTreeNode> Items { get; set; } = new List<CategoryTreeNode>();
    public string DefaultLocale { get; set; }
    public List<CategoryLocaleInfo> OtherLocales { get; set; } = new List<CategoryLocaleInfo>();
    public bool IsMultilingual { get; set; }
    public int VocabularyId { get; set; }
    public int TermCount { get; set; }
  }

  public class CategoryTreeService
  {
    public CategoryTreeResult GetTree(int moduleId, int portalId, string defaultLocale, string language = null)
    {
      var settings = ModuleSettings.GetSettings(moduleId);
      if (settings.VocabularyId < 0)
      {
        return new CategoryTreeResult { NoVocabulary = true };
      }

      var terms = TermsController.GetTermsByVocabulary(moduleId, settings.VocabularyId);
      var portalLocales = LocaleController.Instance.GetLocales(portalId);
      if (string.IsNullOrEmpty(defaultLocale))
        defaultLocale = portalLocales.Values.Select(l => l.Code).FirstOrDefault() ?? "";

      var otherLocales = portalLocales.Values
          .Where(l => !string.Equals(l.Code, defaultLocale, StringComparison.OrdinalIgnoreCase))
          .OrderBy(l => l.Code)
          .Select(l => new CategoryLocaleInfo { Code = l.Code, Name = l.EnglishName ?? l.Code })
          .ToList();

      var localeCodes = otherLocales.Select(l => l.Code).ToList();
      var roots = BuildTree(terms, -1, localeCodes, language, defaultLocale);

      return new CategoryTreeResult
      {
        Items = roots,
        DefaultLocale = defaultLocale,
        OtherLocales = otherLocales,
        IsMultilingual = portalLocales.Count > 1,
        VocabularyId = settings.VocabularyId,
        TermCount = terms.Count
      };
    }

    private static List<CategoryTreeNode> BuildTree(
        Dictionary<int, TermInfo> terms,
        int parentId,
        List<string> localeCodes,
        string language,
        string defaultLocale)
    {
      IEnumerable<TermInfo> children;
      if (parentId < 1)
        children = terms.Values.Where(t => !t.ParentTermId.HasValue || t.ParentTermId.Value < 1);
      else
        children = terms.Values.Where(t => t.ParentTermId.HasValue && t.ParentTermId.Value == parentId);

      var ordered = children.OrderBy(t => t.Weight).ThenBy(t => t.Name).ToList();
      var result = new List<CategoryTreeNode>();
      int order = 0;
      foreach (var t in ordered)
      {
        order++;
        var node = new CategoryTreeNode
        {
          CategoryId = t.TermId,
          Name = ResolveName(t, language, defaultLocale),
          ParentTermId = t.ParentTermId.HasValue && t.ParentTermId.Value > 0 ? t.ParentTermId.Value : -1,
          ViewOrder = t.Weight > 0 ? t.Weight : order,
          Children = BuildTree(terms, t.TermId, localeCodes, language, defaultLocale)
        };
        foreach (var code in localeCodes)
          node.Localizations[code] = t.NameLocalizations[code] ?? "";
        result.Add(node);
      }
      return result;
    }

    private static string ResolveName(TermInfo term, string language, string defaultLocale)
    {
      if (!string.IsNullOrEmpty(language)
          && !string.Equals(language, defaultLocale, StringComparison.OrdinalIgnoreCase))
      {
        var localized = term.NameLocalizations[language];
        if (!string.IsNullOrEmpty(localized))
          return localized;
      }
      return term.Name ?? "";
    }
  }
}
