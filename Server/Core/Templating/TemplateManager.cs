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
using System;
using System.Web;

namespace DotNetNuke.Modules.Blog.Core.Templating
{
  [Serializable()]
  public class TemplateManager
  {

    public TemplateManager(PortalSettings portalsettings, string template)
    {
      if (template.StartsWith("[G]"))
      {
        TemplatePath = DotNetNuke.Common.Globals.ResolveUrl(Globals.glbTemplatesPath) + template.Substring(4) + "/";
        TemplateRelPath = Globals.glbTemplatesPath + template.Substring(4) + "/";
        TemplateMapPath = HttpContext.Current.Server.MapPath(DotNetNuke.Common.Globals.ResolveUrl(Globals.glbTemplatesPath)) + template.Substring(4) + @"\";
      }
      else if (template.StartsWith("[S]"))
      {
        TemplatePath = portalsettings.ActiveTab.SkinPath + "Templates/Blog/" + template.Substring(4) + "/";
        TemplateRelPath = "~" + TemplatePath.Substring(DotNetNuke.Common.Globals.ApplicationPath.Length);
        TemplateMapPath = HttpContext.Current.Server.MapPath(TemplatePath);
      }
      else
      {
        TemplatePath = portalsettings.HomeDirectory.TrimEnd('/') + "/Blog/Templates/" + template.Substring(4) + "/";
        var pi = new PortalController().GetPortal(portalsettings.PortalId);
        TemplateRelPath = "~/" + pi.HomeDirectory.TrimEnd('/') + "/Blog/Templates/" + template.Substring(4) + "/";
        TemplateMapPath = portalsettings.HomeDirectoryMapPath.TrimEnd('\\') + @"\Blog\Templates\" + template.Substring(4) + @"\";
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

  }
}