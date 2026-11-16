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

using DotNetNuke.Entities.Portals;
using DotNetNuke.Modules.Blog.Core.Common;
using DotNetNuke.Web.Client.ClientResourceManagement;
using System;
using System.Threading;
using System.Web;
using System.Web.UI;

namespace DotNetNuke.Modules.Blog.Core.Templating
{
  [Serializable()]
  public class TemplateManager
  {

    public TemplateManager(PortalSettings portalsettings, string template)
    {
      if (template.StartsWith("[G]"))
      {
        TemplatePath = DotNetNuke.Common.Globals.ResolveUrl(Globals.glbTemplatesPath) + template.Substring(3) + "/";
        TemplateRelPath = Globals.glbTemplatesPath + template.Substring(3) + "/";
        TemplateMapPath = HttpContext.Current.Server.MapPath(DotNetNuke.Common.Globals.ResolveUrl(Globals.glbTemplatesPath)) + template.Substring(3) + @"\";
      }
      else if (template.StartsWith("[S]"))
      {
        TemplatePath = portalsettings.ActiveTab.SkinPath + "Templates/Blog/" + template.Substring(3) + "/";
        TemplateRelPath = "~" + TemplatePath.Substring(DotNetNuke.Common.Globals.ApplicationPath.Length);
        TemplateMapPath = HttpContext.Current.Server.MapPath(TemplatePath);
      }
      else
      {
        TemplatePath = portalsettings.HomeDirectory.TrimEnd('/') + "/Blog/Templates/" + template.Substring(3) + "/";
        var pi = new PortalController().GetPortal(portalsettings.PortalId);
        TemplateRelPath = "~/" + pi.HomeDirectory.TrimEnd('/') + "/Blog/Templates/" + template.Substring(3) + "/";
        TemplateMapPath = portalsettings.HomeDirectoryMapPath.TrimEnd('\\') + @"\Blog\Templates\" + template.Substring(3) + @"\";
      }
    }

    #region  Properties 
    public string TemplatePath { get; set; }
    public string TemplateRelPath { get; set; }
    public string TemplateMapPath { get; set; }

    private TemplateSettings _templateSettings;
    public TemplateSettings TemplateSettings
    {
      get
      {
        if (_templateSettings is null)
        {
          _templateSettings = new TemplateSettings(TemplateMapPath);
        }
        return _templateSettings;
      }
      set
      {
        _templateSettings = value;
      }
    }

    public string SharedResourcesFile
    {
      get
      {
        return TemplateRelPath + "App_LocalResources/SharedResources";
      }
    }

    private string _description = null;
    public string Description
    {
      get
      {
        if (_description is null)
        {
          _description = "";
          if (System.IO.File.Exists(TemplateMapPath + "description.txt"))
          {
            _description = Globals.ReadFile(TemplateMapPath + "description.txt");
          }
        }
        return _description;
      }
    }
    #endregion

    /// <summary>
    /// Registers this template's CSS/JS with the page. Used by WebForms ViewTemplate PreRender
    /// and by MVC hosts that render templates without attaching the control to the page.
    /// </summary>
    public void RegisterClientAssets(Page page, GenericTokenReplace defaultReplacer = null)
    {
      RegisterClientAssets(page, TemplatePath, TemplateMapPath, defaultReplacer);
    }

    public static void RegisterClientAssets(Page page, string templatePath, string templateMapPath, GenericTokenReplace defaultReplacer = null)
    {
      if (page == null || string.IsNullOrEmpty(templatePath) || string.IsNullOrEmpty(templateMapPath))
      {
        return;
      }

      if (System.IO.File.Exists(templateMapPath + "template.css"))
      {
        ClientResourceManager.RegisterStyleSheet(page, templatePath + "template.css");
      }
      if (System.IO.Directory.Exists(templateMapPath + "css"))
      {
        foreach (System.IO.FileInfo f in new System.IO.DirectoryInfo(templateMapPath + "css").GetFiles("*.css"))
        {
          ClientResourceManager.RegisterStyleSheet(page, templatePath + "css/" + f.Name);
        }
      }

      if (System.IO.File.Exists(templateMapPath + "template.js"))
      {
        ClientResourceManager.RegisterScript(page, templatePath + "template.js");
      }
      if (System.IO.Directory.Exists(templateMapPath + "js"))
      {
        foreach (System.IO.FileInfo f in new System.IO.DirectoryInfo(templateMapPath + "js").GetFiles("*.js"))
        {
          ClientResourceManager.RegisterScript(page, templatePath + "js/" + f.Name);
        }
      }

      string locale = Thread.CurrentThread.CurrentCulture.Name.ToLower();
      if (System.IO.Directory.Exists(templateMapPath + @"js\" + locale))
      {
        foreach (System.IO.FileInfo f in new System.IO.DirectoryInfo(templateMapPath + @"js\" + locale).GetFiles("*.js"))
        {
          ClientResourceManager.RegisterScript(page, templatePath + "js/" + locale + "/" + f.Name);
        }
      }
      else
      {
        locale = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower();
        if (System.IO.Directory.Exists(templateMapPath + @"js\" + locale))
        {
          foreach (System.IO.FileInfo f in new System.IO.DirectoryInfo(templateMapPath + @"js\" + locale).GetFiles("*.js"))
          {
            ClientResourceManager.RegisterScript(page, templatePath + "js/" + locale + "/" + f.Name);
          }
        }
      }

      if (defaultReplacer != null && System.IO.Directory.Exists(templateMapPath + "jsblocks"))
      {
        foreach (System.IO.FileInfo f in new System.IO.DirectoryInfo(templateMapPath + "jsblocks").GetFiles("*.js"))
        {
          var t = new Template(templateMapPath + @"jsblocks\", templatePath, f.Name, defaultReplacer, null);
          string s = t.ReplaceContents();
          if (!s.StartsWith("<"))
          {
            s = string.Format("<script type=\"text/javascript\">{0}//<![CDATA[{0}{1}//]]>{0}</script>", Environment.NewLine, s);
          }
          page.ClientScript.RegisterClientScriptBlock(typeof(string), f.Name, s);
        }
      }
    }

  }
}