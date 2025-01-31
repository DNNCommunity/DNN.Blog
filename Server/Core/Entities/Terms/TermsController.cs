using System;
using System.Collections.Generic;
using System.Data;
// 
// DNN Connect - http://dnn-connect.org
// Copyright (c) 2015
// by DNN Connect
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
// 

using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content.Taxonomy;
using Microsoft.VisualBasic;

namespace DotNetNuke.Modules.Blog.Entities.Terms
{
  public partial class TermsController
  {

    public static TermInfo GetTerm(int termId, int moduleId, string locale)
    {
      return CBO.FillObject<TermInfo>(Data.DataProvider.Instance().GetTerm(termId, moduleId, locale));
    }

    public static List<TermInfo> GetTermsByPost(int contentItemId, int moduleId, string locale)
    {
      return CBO.FillCollection<TermInfo>(Data.DataProvider.Instance().GetTermsByPost(contentItemId, moduleId, locale));
    }

    public static List<TermInfo> GetTermsByModule(int moduleId, string locale)
    {
      return CBO.FillCollection<TermInfo>(Data.DataProvider.Instance().GetTermsByModule(moduleId, locale));
    }

    public static Dictionary<string, TermInfo> GetTermsByVocabulary(int moduleId, int vocabularyId, string locale)
    {
      return GetTermsByVocabulary(moduleId, vocabularyId, locale, false);
    }

    public static Dictionary<string, TermInfo> GetTermsByVocabulary(int moduleId, int vocabularyId, string locale, bool clearCache)
    {
      string CacheKey = string.Format("BlogVocab-{0}-{1}-{2}", moduleId, vocabularyId, locale);
      Dictionary<string, TermInfo> res = (Dictionary<string, TermInfo>)DataCache.GetCache(CacheKey);
      if (res is null | clearCache)
      {
        res = new Dictionary<string, TermInfo>(StringComparer.CurrentCultureIgnoreCase);
        CBO.FillDictionary("Name", Data.DataProvider.Instance().GetTermsByVocabulary(moduleId, vocabularyId, locale), res);
        DataCache.SetCache(CacheKey, res);
      }
      return res;
    }

    public static Dictionary<int, TermInfo> GetTermsByVocabulary(int moduleId, int vocabularyId)
    {
      var res = new Dictionary<int, TermInfo>();
      CBO.FillDictionary("TermID", Data.DataProvider.Instance().GetTermsByVocabulary(moduleId, vocabularyId, ""), res);
      return res;
    }

    public static List<TermInfo> GetTermList(int moduleId, string termList, int vocabularyId, bool autoCreate, string locale)
    {
      if (termList is null)
        termList = "";
      string[] termNames = termList.Replace(";", ",").Trim(',').Split(',');
      return GetTermList(moduleId, termNames.ToList(), vocabularyId, autoCreate, locale);
    }

    public static List<TermInfo> GetTermList(int moduleId, List<string> termList, int vocabularyId, bool autoCreate, string locale)
    {
      var vocab = GetTermsByVocabulary(moduleId, vocabularyId, locale, true);
      var res = new List<TermInfo>();
      foreach (string termName in termList)
      {
        string name = termName.Trim();
        TermInfo existantTerm = null;
        if (vocab.ContainsKey(name))
          existantTerm = vocab[name];
        if (existantTerm is not null)
        {
          res.Add(existantTerm);
        }
        else if (autoCreate & !string.IsNullOrEmpty(name))
        {
          int termId = DotNetNuke.Entities.Content.Common.Util.GetTermController().AddTerm(new Term(vocabularyId) { Name = name });
          res.Add(new TermInfo(name) { Description = "", TermId = termId, TotalPosts = 0, Weight = 0 });
        }
      }
      return res;
    }

    #region  (De)serialization 
    public static List<TermInfo> FromXml(XmlNode xml)
    {
      var res = new List<TermInfo>();
      if (xml is null)
        return res;
      foreach (XmlNode xTerm in xml.SelectNodes("Term"))
      {
        var t = new TermInfo();
        t.FromXml(xTerm);
        res.Add(t);
      }
      return res;
    }

    public static void WriteVocabulary(XmlTextWriter writer, string name, List<TermInfo> vocabulary)
    {
      writer.WriteStartElement(name);
      foreach (TermInfo t in vocabulary.Where(x => x.ParentTermId is null))
        t.WriteXml(writer, vocabulary);
      writer.WriteEndElement(); // Vocabulary
    }

    public static void AddTags(int moduleId, List<TermInfo> tagList)
    {
      var allTags = GetTermsByVocabulary(moduleId, 1, "");
      foreach (TermInfo t in tagList)
      {
        if (!allTags.ContainsKey(t.Name))
        {
          var newTerm = new Term(1) { Name = t.Name.Trim(), Description = t.Description };
          newTerm.TermId = DotNetNuke.Entities.Content.Common.Util.GetTermController().AddTerm(newTerm);
          foreach (string l in t.NameLocalizations.Locales)
            Data.DataProvider.Instance().SetTermLocalization(newTerm.TermId, l, t.NameLocalizations[l], t.DescriptionLocalizations[l]);
        }
      }
    }

    public static void AddVocabulary(int vocabularyId, List<TermInfo> vocabulary)
    {
      foreach (TermInfo t in vocabulary.Where(x => x.ParentTermId is null))
        AddTerm(vocabularyId, vocabulary, 0, t);
    }

    private static void AddTerm(int vocabularyId, List<TermInfo> vocabulary, int parentId, TermInfo term)
    {
      var newTerm = new Term(vocabularyId) { Name = term.Name.Trim(), Description = term.Description, ParentTermId = parentId };
      newTerm.TermId = DotNetNuke.Entities.Content.Common.Util.GetTermController().AddTerm(newTerm);
      foreach (string l in term.NameLocalizations.Locales)
        Data.DataProvider.Instance().SetTermLocalization(newTerm.TermId, l, term.NameLocalizations[l], term.DescriptionLocalizations[l]);
      foreach (TermInfo t in vocabulary.Where(x => (bool)(term.TermId is var arg3 && x.ParentTermId is { } arg4 ? arg4 == arg3 : (bool?)null)))
        AddTerm(vocabularyId, vocabulary, newTerm.TermId, t);
    }
    #endregion

    #region  UI Functions 
    public static string GetCategoryTreeAsJson(Dictionary<string, TermInfo> vocabulary)
    {
      return GetCategoryTreeAsJson(vocabulary, new List<int>());
    }

    public static string GetCategoryTreeAsJson(Dictionary<string, TermInfo> vocabulary, List<int> selectedIds)
    {
      var childTreeBuilder = new StringBuilder();
      GetCategoryTree(childTreeBuilder, vocabulary, -1, selectedIds);
      string res = childTreeBuilder.ToString();
      if (res.Length > 0)
      {
        res = res.Substring(14); // cut off the first children declaration
      }
      return res;
    }

    private static void GetCategoryTree(StringBuilder @out, Dictionary<string, TermInfo> vocabulary, int parentId, List<int> selectedIds)
    {
      IEnumerable<TermInfo> selection;
      if (parentId == -1)
      {
        selection = vocabulary.Values.Where(t => t.ParentTermId is null);
      }
      else
      {
        selection = vocabulary.Values.Where(t => t.ParentTermId is not null && (bool)(t.ParentTermId is { } arg6 ? arg6 == parentId : (bool?)null));
      }

      if (selection.Count() > 0)
      {
        out.Append(", \"children\": [");
        bool first = true;
        foreach (TermInfo cat in selection)
        {
          if (!first)
            out.Append(",");
          out.Append("{");
          out.Append(string.Format("\"title\": \"{0}\",", cat.LocalizedName));
          out.Append(string.Format("\"key\": \"{0}\",", cat.TermId));
          out.Append("\"icon\": false,");
          out.Append("\"expand\": true,");
          out.Append("\"isFolder\": true,");
          out.Append(string.Format("\"select\": {0}", Interaction.IIf(selectedIds.Contains(cat.TermId), "true", "false")));
          GetCategoryTree(out, vocabulary, cat.TermId, selectedIds);
          out.Append("}");
          first = false;
        }
        out.Append("]");
      }
    }
    #endregion

  }
}