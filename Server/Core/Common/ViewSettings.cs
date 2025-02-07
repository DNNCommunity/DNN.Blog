using DotNetNuke.Services.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
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

using System.Xml;

namespace DotNetNuke.Modules.Blog.Core.Common
{
  [Serializable()]
  public class ViewSettings : IPropertyAccess
  {

    #region  Private Members 
    private Hashtable _allSettings = null;
    private int _tabModuleId = -1;
    #endregion

    #region  Properties 
    public string Template { get; set; } = "[G]_default";
    public int BlogModuleId { get; set; } = -1;
    public bool ShowAllLocales { get; set; } = true;
    public bool ModifyPageDetails { get; set; } = false;
    public bool AddCanonicalTag { get; set; } = false;
    public bool ShowManagementPanel { get; set; } = false;
    public bool ShowManagementPanelViewMode { get; set; } = true;
    public bool HideUnpublishedBlogsViewMode { get; set; } = false;
    public bool HideUnpublishedBlogsEditMode { get; set; } = false;
    public bool AllowComments { get; set; } = true;
    public int BlogId { get; set; } = -1;
    public string Categories { get; set; } = "";
    public int AuthorId { get; set; } = -1;
    public Dictionary<string, string> TemplateSettings { get; set; } = new Dictionary<string, string>();
    private Templating.TemplateManager TemplateManager { get; set; }
    internal bool CanCache { get; set; } = true;
    #endregion

    #region  ReadOnly Properties 
    private List<int> _categoryList;
    public List<int> CategoryList
    {
      get
      {
        if (_categoryList is null)
        {
          _categoryList = new List<int>();
          foreach (string c in Categories.Split(','))
          {
            if (int.TryParse(c, out int categoryId))
            {
              _categoryList.Add(categoryId);
            }
          }
        }
        return _categoryList;
      }
    }
    #endregion

    #region  Constructors 
    public ViewSettings(int tabModuleId) : this(tabModuleId, false)
    {
    }

    public ViewSettings(int tabModuleId, bool justLoadSettings)
    {

      _tabModuleId = tabModuleId;
      _allSettings = new DotNetNuke.Entities.Modules.ModuleController().GetTabModule(tabModuleId).TabModuleSettings;

      this.Template = _allSettings.ReadValue("Template", Template);
      this.BlogModuleId = _allSettings.ReadValue("BlogModuleId", BlogModuleId);
      this.ShowAllLocales = _allSettings.ReadValue("ShowAllLocales", ShowAllLocales);
      this.ModifyPageDetails = _allSettings.ReadValue("ModifyPageDetails", ModifyPageDetails);
      this.AddCanonicalTag = _allSettings.ReadValue("AddCanonicalTag", AddCanonicalTag);
      this.ShowManagementPanel = _allSettings.ReadValue("ShowManagementPanel", ShowManagementPanel);
      this.ShowManagementPanelViewMode = _allSettings.ReadValue("ShowManagementPanelViewMode", ShowManagementPanelViewMode);
      this.HideUnpublishedBlogsViewMode = _allSettings.ReadValue("HideUnpublishedBlogsViewMode", HideUnpublishedBlogsViewMode);
      this.HideUnpublishedBlogsEditMode = _allSettings.ReadValue("HideUnpublishedBlogsEditMode", HideUnpublishedBlogsEditMode);
      this.AllowComments = _allSettings.ReadValue("AllowComments", AllowComments);
      this.BlogId = _allSettings.ReadValue("BlogId", BlogId);
      this.Categories = _allSettings.ReadValue("Categories", Categories);
      this.AuthorId = _allSettings.ReadValue("AuthorId", AuthorId);

      if (BlogModuleId > -1 & tabModuleId > -1) // security check
      {
        var parentModule = new DotNetNuke.Entities.Modules.ModuleController().GetModule(BlogModuleId);
        var thisTabModule = new DotNetNuke.Entities.Modules.ModuleController().GetTabModule(tabModuleId);
        if (parentModule is null || parentModule.PortalID != thisTabModule.PortalID)
        {
          BlogModuleId = -1;
          CanCache = false;
        }
      }
      if (justLoadSettings)
        return;

      // Template Settings - first load defaults
      SetTemplate(Template);

    }

    public static ViewSettings GetViewSettings(int tabModuleId)
    {
      string CacheKey = "Blog_TabModuleSettings" + tabModuleId.ToString();
      ViewSettings settings = (ViewSettings)DotNetNuke.Common.Utilities.DataCache.GetCache(CacheKey);
      if (settings is null)
      {
        settings = new ViewSettings(tabModuleId);
        if (settings.CanCache)
          DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey, settings);
      }
      return settings;
    }
    #endregion

    #region  Public Members 
    public virtual void UpdateSettings()
    {
      UpdateSettings(_tabModuleId);
    }

    public virtual void UpdateSettings(int tabModuleId)
    {

      var objModules = new DotNetNuke.Entities.Modules.ModuleController();
      objModules.UpdateTabModuleSetting(tabModuleId, "Template", Template);
      objModules.UpdateTabModuleSetting(tabModuleId, "BlogModuleId", BlogModuleId.ToString());
      objModules.UpdateTabModuleSetting(tabModuleId, "ShowAllLocales", ShowAllLocales.ToString());
      objModules.UpdateTabModuleSetting(tabModuleId, "ModifyPageDetails", ModifyPageDetails.ToString());
      objModules.UpdateTabModuleSetting(tabModuleId, "AddCanonicalTag", AddCanonicalTag.ToString());
      objModules.UpdateTabModuleSetting(tabModuleId, "ShowManagementPanel", ShowManagementPanel.ToString());
      objModules.UpdateTabModuleSetting(tabModuleId, "ShowManagementPanelViewMode", ShowManagementPanelViewMode.ToString());
      objModules.UpdateTabModuleSetting(tabModuleId, "HideUnpublishedBlogsViewMode", HideUnpublishedBlogsViewMode.ToString());
      objModules.UpdateTabModuleSetting(tabModuleId, "HideUnpublishedBlogsEditMode", HideUnpublishedBlogsEditMode.ToString());
      objModules.UpdateTabModuleSetting(tabModuleId, "AllowComments", AllowComments.ToString());
      objModules.UpdateTabModuleSetting(tabModuleId, "BlogId", BlogId.ToString());
      objModules.UpdateTabModuleSetting(tabModuleId, "Categories", Categories.ToString());
      objModules.UpdateTabModuleSetting(tabModuleId, "AuthorId", AuthorId.ToString());
      _categoryList = null;

      string CacheKey = "Blog_TabModuleSettings" + tabModuleId.ToString();
      DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey, this);

    }

    public void SaveTemplateSettings()
    {
      var objModules = new DotNetNuke.Entities.Modules.ModuleController();
      foreach (string key in TemplateSettings.Keys)
        objModules.UpdateTabModuleSetting(_tabModuleId, "t_" + key, TemplateSettings[key]);
      string CacheKey = "Blog_TabModuleSettings" + _tabModuleId.ToString();
      DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey, this);
    }

    public void SetTemplateSetting(string key, string value)
    {
      if (!TemplateSettings.ContainsKey(key))
      {
        TemplateSettings.Add(key, value);
      }
      else
      {
        TemplateSettings[key] = value;
      }
    }
    #endregion

    #region  Private Methods 
    private void SetTemplate(string template)
    {
      TemplateManager = new Templating.TemplateManager(DotNetNuke.Entities.Portals.PortalSettings.Current, template);
      TemplateSettings.Clear();
      foreach (Templating.TemplateSetting st in TemplateManager.TemplateSettings.Settings)
        TemplateSettings.Add(st.Key, st.DefaultValue);
      foreach (string key in _allSettings.Keys)
      {
        if (key.StartsWith("t_"))
        {
          SetTemplateSetting(key.Substring(2), Convert.ToString(_allSettings[key]));
        }
      }
    }
    #endregion

    #region  IPropertyAccess 
    public CacheLevel Cacheability
    {
      get
      {
        return CacheLevel.fullyCacheable;
      }
    }

    public string GetProperty(string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, DotNetNuke.Entities.Users.UserInfo AccessingUser, Scope AccessLevel, ref bool PropertyNotFound)
    {
      string OutputFormat = string.Empty;
      if (string.IsNullOrEmpty(strFormat))
      {
        OutputFormat = "D";
      }
      else
      {
        OutputFormat = strFormat;
      }
      switch (strPropertyName.ToLower() ?? "")
      {
        case "tabmoduleid":
          {
            return _tabModuleId.ToString(OutputFormat, formatProvider);
          }
        case "templatepath":
          {
            return PropertyAccess.FormatString(TemplateManager.TemplatePath, strFormat);
          }
        case "templatemappath":
          {
            return PropertyAccess.FormatString(TemplateManager.TemplateMapPath, strFormat);
          }

        default:
          {
            if (TemplateSettings.ContainsKey(strPropertyName))
            {
              return PropertyAccess.FormatString(TemplateSettings[strPropertyName], strFormat);
            }
            if (strPropertyName.StartsWith("t_"))
              strPropertyName = strPropertyName.Substring(3);
            if (TemplateSettings.ContainsKey(strPropertyName))
            {
              return PropertyAccess.FormatString(TemplateSettings[strPropertyName], strFormat);
            }
            switch (strPropertyName.ToLower() ?? "")
            {
              case "termid":
              case "categories": // termid is for legacy purposes
                {
                  return Categories;
                }
              case "authorid":
                {
                  return AuthorId.ToString(OutputFormat, formatProvider);
                }
              case "blogid":
                {
                  return BlogId.ToString(OutputFormat, formatProvider);
                }
              case "blogmoduleid":
                {
                  return BlogModuleId.ToString(OutputFormat, formatProvider);
                }

              default:
                {
                  return "";
                }
            }

            break;
          }
      }
    }
    #endregion

    #region  Serialization 
    public void Serialize(XmlWriter writer)
    {
      writer.WriteStartElement("ViewSettings");
      writer.WriteElementString("Template", Template);
      writer.WriteElementString("BlogModuleId", BlogModuleId.ToString());
      writer.WriteElementString("ShowAllLocales", ShowAllLocales.ToString());
      writer.WriteElementString("ModifyPageDetails", ModifyPageDetails.ToString());
      writer.WriteElementString("AddCanonicalTag", AddCanonicalTag.ToString());
      writer.WriteElementString("ShowManagementPanel", ShowManagementPanel.ToString());
      writer.WriteElementString("ShowManagementPanelViewMode", ShowManagementPanelViewMode.ToString());
      writer.WriteElementString("HideUnpublishedBlogsViewMode", HideUnpublishedBlogsViewMode.ToString());
      writer.WriteElementString("HideUnpublishedBlogsEditMode", HideUnpublishedBlogsEditMode.ToString());
      writer.WriteElementString("AllowComments", AllowComments.ToString());
      writer.WriteElementString("BlogId", BlogId.ToString());
      writer.WriteElementString("AuthorId", AuthorId.ToString());
      writer.WriteEndElement(); // viewsettings
    }

    public void FromXml(XmlNode xml)
    {
      if (xml is null)
        return;

      this.Template = xml.ReadValue("Template", Template);
      this.BlogModuleId = xml.ReadValue("BlogModuleId", BlogModuleId);
      this.ShowAllLocales = xml.ReadValue("ShowAllLocales", ShowAllLocales);
      this.ModifyPageDetails = xml.ReadValue("ModifyPageDetails", ModifyPageDetails);
      this.AddCanonicalTag = xml.ReadValue("AddCanonicalTag", AddCanonicalTag);
      this.ShowManagementPanel = xml.ReadValue("ShowManagementPanel", ShowManagementPanel);
      this.ShowManagementPanelViewMode = xml.ReadValue("ShowManagementPanelViewMode", ShowManagementPanelViewMode);
      this.HideUnpublishedBlogsViewMode = xml.ReadValue("HideUnpublishedBlogsViewMode", HideUnpublishedBlogsViewMode);
      this.HideUnpublishedBlogsEditMode = xml.ReadValue("HideUnpublishedBlogsEditMode", HideUnpublishedBlogsEditMode);
      this.AllowComments = xml.ReadValue("AllowComments", AllowComments);
      this.BlogId = xml.ReadValue("BlogId", BlogId);
      this.AuthorId = xml.ReadValue("AuthorId", AuthorId);

      _categoryList = null;
    }
    #endregion

  }
}