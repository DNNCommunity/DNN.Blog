using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Xml;
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

using DotNetNuke.Entities.Modules;
using DotNetNuke.Modules.Blog.Common;
using static DotNetNuke.Modules.Blog.Common.Globals;
using DotNetNuke.Modules.Blog.Entities.Blogs;
using DotNetNuke.Modules.Blog.Entities.Posts;
using DotNetNuke.Modules.Blog.Entities.Terms;
using Microsoft.VisualBasic;

namespace DotNetNuke.Modules.Blog.Integration
{
  public partial class BlogModuleController : IPortable
  {

    private const string LogFilePattern = "{0}BlogImport_{1}.resources";

    #region  Post Import Logic 
    public static void CheckupOnImportedFiles(int moduleId)
    {
      string CacheKey = "Blog_CheckupOnImportedFiles" + moduleId.ToString();
      if (DotNetNuke.Common.Utilities.DataCache.GetCache(CacheKey) is null)
      {
        string logFile = string.Format(LogFilePattern, DotNetNuke.Common.Globals.HostMapPath, moduleId);
        if (System.IO.File.Exists(logFile))
        {
          using (var sr = new System.IO.StreamReader(logFile))
          {
            string line;
            int currentBlog = -1;
            do
            {
              line = sr.ReadLine();
              if (!string.IsNullOrEmpty(line))
              {
                string t = line.Substring(0, 1);
                int oldId = int.Parse(line.Substring(1, line.IndexOf("-") - 1));
                int newId = int.Parse(line.Substring(line.IndexOf("-") + 1));
                if (t == "M")
                {
                  Data.DataProvider.Instance().UpdateModuleWiring(DotNetNuke.Entities.Portals.PortalSettings.Current.PortalId, oldId, newId);
                }
                else if (t == "B")
                {
                  var d = new System.IO.DirectoryInfo(string.Format(@"{0}Blog\Files\{1}\", DotNetNuke.Entities.Portals.PortalSettings.Current.HomeDirectoryMapPath, oldId));
                  if (d.Exists)
                  {
                    d.MoveTo(string.Format(@"{0}Blog\Files\{1}\", DotNetNuke.Entities.Portals.PortalSettings.Current.HomeDirectoryMapPath, newId));
                  }
                  currentBlog = newId;
                }
                else if (t == "P")
                {
                  var d = new System.IO.DirectoryInfo(string.Format(@"{0}Blog\Files\{1}\{2}\", DotNetNuke.Entities.Portals.PortalSettings.Current.HomeDirectoryMapPath, currentBlog, oldId));
                  if (d.Exists)
                  {
                    d.MoveTo(string.Format(@"{0}Blog\Files\{1}\{2}\", DotNetNuke.Entities.Portals.PortalSettings.Current.HomeDirectoryMapPath, currentBlog, newId));
                  }
                  if (d.GetFiles("*.*").Count() == 0)
                  {
                  }
                  else
                  {
                    var post = PostsController.GetPost(newId, moduleId, "");
                    if (post is not null)
                    {
                      string postPath = GetPostDirectoryPath(post);
                      foreach (System.IO.FileInfo f in d.GetFiles("*.*"))
                      {
                        string filename = f.Name;
                        string reg = @"\&quot;([^\&]*)" + filename + @"\&quot;";
                        string repl = "&quot;" + postPath + filename + "&quot;";
                        post.Content = Regex.Replace(post.Content, reg, repl);
                        foreach (string l in post.ContentLocalizations.Locales)
                          post.ContentLocalizations[l] = Regex.Replace(post.ContentLocalizations[l], reg, repl);
                        if (!string.IsNullOrEmpty(post.Summary))
                          post.Summary = Regex.Replace(post.Summary, reg, repl);
                        foreach (string l in post.SummaryLocalizations.Locales)
                          post.SummaryLocalizations[l] = Regex.Replace(post.SummaryLocalizations[l], reg, repl);
                      }
                      PostsController.UpdatePost(post, -1);
                    }
                  }
                }
              }
            }
            while (!(line is null));
            sr.ReadLine();
          }
          System.IO.File.Delete(logFile);
        }
        DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey, true);
      }
    }
    #endregion

    #region  IPortable Methods 
    public string ExportModule(int ModuleID)
    {

      var strXml = new StringBuilder();
      using (var sw = new System.IO.StringWriter(strXml))
      {
        using (var xml = new XmlTextWriter(sw))
        {
          xml.WriteStartElement("dnnblog");
          xml.WriteElementString("ModuleId", ModuleID.ToString());
          var tabMods = new ModuleController().GetAllTabsModulesByModuleID(ModuleID);
          if (tabMods.Count > 0)
          {
            var vs = ViewSettings.GetViewSettings(((ModuleInfo)tabMods[0]).TabModuleID);
            vs.Serialize(xml);
            if (vs.BlogModuleId == -1)
            {
              var ms = ModuleSettings.GetModuleSettings(ModuleID);
              ms.Serialize(xml);
              if (ms.VocabularyId > -1)
              {
                TermsController.WriteVocabulary(xml, "Categories", TermsController.GetTermsByVocabulary(ModuleID, ms.VocabularyId).Values.ToList());
              }
              TermsController.WriteVocabulary(xml, "Tags", TermsController.GetTermsByModule(ModuleID, "").Where(t => t.VocabularyId == 1).ToList());
            }
          }
          foreach (BlogInfo b in BlogsController.GetBlogsByModule(ModuleID, "").Values)
            b.WriteXml(xml);
          xml.WriteEndElement(); // dnnblog
        }
      }
      return strXml.ToString();

    }

    public void ImportModule(int ModuleID, string Content, string Version, int UserID)
    {
      try
      {

        var xContent = DotNetNuke.Common.Globals.GetContent(Content, "dnnblog");
        var importReport = new StringBuilder();
        int oldModuleId = -1;
        Extensions.ReadValue(ref xContent, "ModuleId", ref oldModuleId);
        importReport.AppendFormat("M{0}-{1}" + Constants.vbCrLf, oldModuleId, ModuleID);

        var tabMods = new ModuleController().GetAllTabsModulesByModuleID(ModuleID);
        if (tabMods.Count > 0)
        {
          var vs = ViewSettings.GetViewSettings(((ModuleInfo)tabMods[0]).TabModuleID);
          vs.FromXml(xContent.SelectSingleNode("ViewSettings"));
          vs.UpdateSettings();
          if (vs.BlogModuleId == -1)
          {
            var settings = ModuleSettings.GetModuleSettings(ModuleID);
            settings.FromXml(xContent.SelectSingleNode("Settings"));
            var vocabulary = TermsController.FromXml(xContent.SelectSingleNode("Categories"));
            var categories = new Dictionary<string, TermInfo>();
            if (vocabulary.Count > 0)
            {
              settings.VocabularyId = Integration.CreateNewVocabulary(((ModuleInfo)tabMods[0]).PortalID).VocabularyId;
              TermsController.AddVocabulary(settings.VocabularyId, vocabulary);
              categories = TermsController.GetTermsByVocabulary(ModuleID, settings.VocabularyId, "", true);
            }
            settings.UpdateSettings();
            vocabulary = TermsController.FromXml(xContent.SelectSingleNode("Tags"));
            if (vocabulary.Count > 0)
            {
              TermsController.AddTags(ModuleID, vocabulary);
            }
            var tags = TermsController.GetTermsByVocabulary(ModuleID, 1, "");
            foreach (XmlNode xBlog in xContent.SelectNodes("Blog"))
            {
              var blog = new BlogInfo();
              blog.FromXml(xBlog);
              blog.ModuleID = ModuleID;
              blog.OwnerUserId = UserID;
              blog.BlogID = BlogsController.AddBlog(ref blog, UserID);
              importReport.AppendFormat("B{0}-{1}" + Constants.vbCrLf, blog.ImportedBlogId, blog.BlogID);
              foreach (PostInfo p in blog.ImportedPosts)
              {
                p.BlogID = blog.BlogID;
                foreach (string tagName in p.ImportedTags)
                {
                  if (tags.ContainsKey(tagName))
                  {
                    p.Terms.Add(tags[tagName]);
                  }
                }
                foreach (string catName in p.ImportedCategories)
                {
                  if (categories.ContainsKey(catName))
                  {
                    p.Terms.Add(categories[catName]);
                  }
                }
                PostsController.AddPost(ref p, UserID);
                importReport.AppendFormat("P{0}-{1}" + Constants.vbCrLf, p.ImportedPostId, p.ContentItemId);
              }
            }
          }
        }

        string importLogFile = string.Format(LogFilePattern, DotNetNuke.Common.Globals.HostMapPath, ModuleID);
        WriteToFile(importLogFile, importReport.ToString());
      }

      catch (Exception ex)
      {
        DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
      }
    }
    #endregion

    #region  Private Methods 
    #endregion

  }
}