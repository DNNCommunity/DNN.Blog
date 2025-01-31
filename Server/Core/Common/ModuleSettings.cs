﻿using System;
using System.Collections;
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
using DotNetNuke.Common.Utilities;
using static DotNetNuke.Modules.Blog.Common.Globals;
using DotNetNuke.Services.Tokens;

namespace DotNetNuke.Modules.Blog.Common
{
  [Serializable()]
  public class ModuleSettings : IPropertyAccess
  {

    #region  Private Members 
    private Hashtable _allSettings = null;
    private int _moduleId = -1;
    private int _importedModuleId = -1;
    #endregion

    #region  Properties 
    public string Version { get; set; } = "0.0.0";
    public bool AllowWLW { get; set; } = false;
    public bool AllowMultipleCategories { get; set; } = true;
    public int VocabularyId { get; set; } = -1;
    public bool AllowAttachments { get; set; } = true;
    public SummaryType SummaryModel { get; set; } = SummaryType.HtmlIndependent;
    public string StyleDetectionUrl { get; set; } = "";
    public int WLWRecentPostsMax { get; set; } = 10;
    public bool AutoGenerateMissingSummary { get; set; } = true;
    public int AutoGeneratedSummaryLength { get; set; } = 1000;
    public string FacebookAppId { get; set; } = "";
    public int FacebookProfileIdProperty { get; set; } = -1;

    public bool ModifyPageDetails { get; set; } = false;

    public string RssEmail { get; set; } = "";
    public int RssDefaultNrItems { get; set; } = 20;
    public int RssMaxNrItems { get; set; } = 50;
    public int RssTtl { get; set; } = 30;
    public int RssImageWidth { get; set; } = 144;
    public int RssImageHeight { get; set; } = 96;
    public bool RssImageSizeAllowOverride { get; set; } = true;
    public bool RssAllowContentInFeed { get; set; } = true;
    public string RssDefaultCopyright { get; set; } = "";

    public string PortalTemplatesPath { get; set; } = "";
    public int IncrementViewCount { get; set; } = 60; // seconds
    private string PortalModulePath { get; set; } = "";
    private string PortalModuleMapPath { get; set; } = "";
    private string _portalTemplatesMapPath = "";
    public bool UseFriendlyURLs { get; set; } = true;

    public string PortalTemplatesMapPath
    {
      get
      {
        return _portalTemplatesMapPath;
      }
    }
    public int ModuleId
    {
      get
      {
        return _moduleId;
      }
    }
    #endregion

    #region  Constructors 
    public ModuleSettings(int moduleId)
    {

      _moduleId = moduleId;
      Version = GetType().Assembly.GetName().Version.ToString();
      _allSettings = new DotNetNuke.Entities.Modules.ModuleController().GetModule(moduleId).ModuleSettings;
      bool argVariable = AllowWLW;
      Extensions.ReadValue(ref _allSettings, "AllowWLW", ref argVariable);
      AllowWLW = argVariable;
      bool argVariable1 = AllowMultipleCategories;
      Extensions.ReadValue(ref _allSettings, "AllowMultipleCategories", ref argVariable1);
      AllowMultipleCategories = argVariable1;
      int argVariable2 = VocabularyId;
      Extensions.ReadValue(ref _allSettings, "VocabularyId", ref argVariable2);
      VocabularyId = argVariable2;
      bool argVariable3 = AllowAttachments;
      Extensions.ReadValue(ref _allSettings, "AllowAttachments", ref argVariable3);
      AllowAttachments = argVariable3;
      var argVariable4 = SummaryModel;
      Extensions.ReadValue(ref _allSettings, "SummaryModel", ref argVariable4);
      SummaryModel = argVariable4;
      string argVariable5 = StyleDetectionUrl;
      Extensions.ReadValue(ref _allSettings, "StyleDetectionUrl", ref argVariable5);
      StyleDetectionUrl = argVariable5;
      int argVariable6 = WLWRecentPostsMax;
      Extensions.ReadValue(ref _allSettings, "WLWRecentPostsMax", ref argVariable6);
      WLWRecentPostsMax = argVariable6;
      bool argVariable7 = ModifyPageDetails;
      Extensions.ReadValue(ref _allSettings, "ModifyPageDetails", ref argVariable7);
      ModifyPageDetails = argVariable7;
      bool argVariable8 = AutoGenerateMissingSummary;
      Extensions.ReadValue(ref _allSettings, "AutoGenerateMissingSummary", ref argVariable8);
      AutoGenerateMissingSummary = argVariable8;
      int argVariable9 = AutoGeneratedSummaryLength;
      Extensions.ReadValue(ref _allSettings, "AutoGeneratedSummaryLength", ref argVariable9);
      AutoGeneratedSummaryLength = argVariable9;
      string argVariable10 = FacebookAppId;
      Extensions.ReadValue(ref _allSettings, "FacebookAppId", ref argVariable10);
      FacebookAppId = argVariable10;
      int argVariable11 = FacebookProfileIdProperty;
      Extensions.ReadValue(ref _allSettings, "FacebookProfileIdProperty", ref argVariable11);
      FacebookProfileIdProperty = argVariable11;

      string argVariable12 = RssEmail;
      Extensions.ReadValue(ref _allSettings, "RssEmail", ref argVariable12);
      RssEmail = argVariable12;
      int argVariable13 = RssDefaultNrItems;
      Extensions.ReadValue(ref _allSettings, "RssDefaultNrItems", ref argVariable13);
      RssDefaultNrItems = argVariable13;
      int argVariable14 = RssMaxNrItems;
      Extensions.ReadValue(ref _allSettings, "RssMaxNrItems", ref argVariable14);
      RssMaxNrItems = argVariable14;
      int argVariable15 = RssTtl;
      Extensions.ReadValue(ref _allSettings, "RssTtl", ref argVariable15);
      RssTtl = argVariable15;
      int argVariable16 = RssImageWidth;
      Extensions.ReadValue(ref _allSettings, "RssImageWidth", ref argVariable16);
      RssImageWidth = argVariable16;
      int argVariable17 = RssImageHeight;
      Extensions.ReadValue(ref _allSettings, "RssImageHeight", ref argVariable17);
      RssImageHeight = argVariable17;
      bool argVariable18 = RssImageSizeAllowOverride;
      Extensions.ReadValue(ref _allSettings, "RssImageSizeAllowOverride", ref argVariable18);
      RssImageSizeAllowOverride = argVariable18;
      bool argVariable19 = RssAllowContentInFeed;
      Extensions.ReadValue(ref _allSettings, "RssAllowContentInFeed", ref argVariable19);
      RssAllowContentInFeed = argVariable19;
      string argVariable20 = RssDefaultCopyright;
      Extensions.ReadValue(ref _allSettings, "RssDefaultCopyright", ref argVariable20);
      RssDefaultCopyright = argVariable20;
      int argVariable21 = IncrementViewCount;
      Extensions.ReadValue(ref _allSettings, "IncrementViewCount", ref argVariable21);
      IncrementViewCount = argVariable21;

      PortalModulePath = DotNetNuke.Entities.Portals.PortalSettings.Current.HomeDirectory;
      if (!PortalModulePath.EndsWith("/"))
      {
        PortalModulePath += "/";
      }
      PortalModulePath += string.Format("Blog/", moduleId);

      PortalModuleMapPath = DotNetNuke.Entities.Portals.PortalSettings.Current.HomeDirectoryMapPath;
      if (!PortalModuleMapPath.EndsWith(@"\"))
      {
        PortalModuleMapPath += @"\";
      }
      PortalModuleMapPath += string.Format(@"Blog\", moduleId);

      _portalTemplatesMapPath = string.Format(@"{0}Templates\", PortalModuleMapPath);
      if (!System.IO.Directory.Exists(_portalTemplatesMapPath))
      {
        System.IO.Directory.CreateDirectory(_portalTemplatesMapPath);
      }
      PortalTemplatesPath = string.Format("{0}Templates/", PortalModulePath);

    }

    public static ModuleSettings GetModuleSettings(int moduleId)
    {
      string CacheKey = "Blog_ModuleSettings" + moduleId.ToString();
      ModuleSettings settings = (ModuleSettings)DataCache.GetCache(CacheKey);
      if (settings is null)
      {
        settings = new ModuleSettings(moduleId);
        DataCache.SetCache(CacheKey, settings);
      }
      return settings;
    }
    #endregion

    #region  Public Members 
    public virtual void UpdateSettings()
    {

      var objModules = new DotNetNuke.Entities.Modules.ModuleController();
      objModules.UpdateModuleSetting(_moduleId, "AllowWLW", AllowWLW.ToString());
      objModules.UpdateModuleSetting(_moduleId, "AllowMultipleCategories", AllowMultipleCategories.ToString());
      objModules.UpdateModuleSetting(_moduleId, "VocabularyId", VocabularyId.ToString());
      objModules.UpdateModuleSetting(_moduleId, "AllowAttachments", AllowAttachments.ToString());
      objModules.UpdateModuleSetting(_moduleId, "SummaryModel", ((int)SummaryModel).ToString());
      objModules.UpdateModuleSetting(_moduleId, "StyleDetectionUrl", StyleDetectionUrl);
      objModules.UpdateModuleSetting(_moduleId, "WLWRecentPostsMax", WLWRecentPostsMax.ToString());
      objModules.UpdateModuleSetting(_moduleId, "ModifyPageDetails", ModifyPageDetails.ToString());
      objModules.UpdateModuleSetting(_moduleId, "AutoGenerateMissingSummary", AutoGenerateMissingSummary.ToString());
      objModules.UpdateModuleSetting(_moduleId, "AutoGeneratedSummaryLength", AutoGeneratedSummaryLength.ToString());
      objModules.UpdateModuleSetting(_moduleId, "FacebookAppId", FacebookAppId);
      objModules.UpdateModuleSetting(_moduleId, "FacebookProfileIdProperty", FacebookProfileIdProperty.ToString());

      objModules.UpdateModuleSetting(_moduleId, "RssEmail", RssEmail);
      objModules.UpdateModuleSetting(_moduleId, "RssDefaultNrItems", RssDefaultNrItems.ToString());
      objModules.UpdateModuleSetting(_moduleId, "RssMaxNrItems", RssMaxNrItems.ToString());
      objModules.UpdateModuleSetting(_moduleId, "RssTtl", RssTtl.ToString());
      objModules.UpdateModuleSetting(_moduleId, "RssImageWidth", RssImageWidth.ToString());
      objModules.UpdateModuleSetting(_moduleId, "RssImageHeight", RssImageHeight.ToString());
      objModules.UpdateModuleSetting(_moduleId, "RssImageSizeAllowOverride", RssImageSizeAllowOverride.ToString());
      objModules.UpdateModuleSetting(_moduleId, "RssAllowContentInFeed", RssAllowContentInFeed.ToString());
      objModules.UpdateModuleSetting(_moduleId, "RssDefaultCopyright", RssDefaultCopyright);
      objModules.UpdateModuleSetting(_moduleId, "IncrementViewCount", IncrementViewCount.ToString());
      if (_importedModuleId > -1)
        objModules.UpdateModuleSetting(_moduleId, "ImportedModuleID", _importedModuleId.ToString());

      string CacheKey = "Blog_ModuleSettings" + _moduleId.ToString();
      DataCache.SetCache(CacheKey, this);
    }
    #endregion

    #region  IPropertyAccess Implementation 
    public string GetProperty(string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, DotNetNuke.Entities.Users.UserInfo AccessingUser, Scope AccessLevel, ref bool PropertyNotFound)
    {
      string OutputFormat = string.Empty;
      var portalSettings = Framework.ServiceLocator<DotNetNuke.Entities.Portals.IPortalController, DotNetNuke.Entities.Portals.PortalController>.Instance.GetCurrentPortalSettings();
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
        case "email":
          {
            return PropertyAccess.FormatString(RssEmail, strFormat);
          }
        case "allowmultiplecategories":
          {
            return AllowMultipleCategories.ToString(formatProvider);
          }
        case "allowattachments":
          {
            return AllowAttachments.ToString(formatProvider);
          }
        case "summarymodel":
          {
            return ((int)SummaryModel).ToString();
          }

        case "portaltemplatespath":
          {
            return PropertyAccess.FormatString(PortalTemplatesPath, strFormat);
          }
        case "portalmodulepath":
          {
            return PropertyAccess.FormatString(PortalModulePath, strFormat);
          }
        case "apppath":
          {
            return DotNetNuke.Common.Globals.ApplicationPath;
          }
        case "imagehandlerpath":
          {
            return DotNetNuke.Common.Globals.ResolveUrl(glbImageHandlerPath);
          }
        case "facebookappid":
          {
            return FacebookAppId;
          }

        default:
          {
            PropertyNotFound = true;
            break;
          }
      }

      return Null.NullString;
    }

    public CacheLevel Cacheability
    {
      get
      {
        return CacheLevel.fullyCacheable;
      }
    }
    #endregion

    #region  Serialization 
    public void Serialize(XmlWriter writer)
    {
      writer.WriteStartElement("Settings");
      writer.WriteElementString("ModuleID", ModuleId.ToString());
      writer.WriteElementString("AllowWLW", AllowWLW.ToString());
      writer.WriteElementString("AllowMultipleCategories", AllowMultipleCategories.ToString());
      writer.WriteElementString("VocabularyId", VocabularyId.ToString());
      writer.WriteElementString("AllowAttachments", AllowAttachments.ToString());
      writer.WriteElementString("SummaryModel", SummaryModel.ToString());
      writer.WriteElementString("StyleDetectionUrl", StyleDetectionUrl);
      writer.WriteElementString("WLWRecentPostsMax", WLWRecentPostsMax.ToString());
      writer.WriteElementString("ModifyPageDetails", ModifyPageDetails.ToString());
      writer.WriteElementString("AutoGenerateMissingSummary", AutoGenerateMissingSummary.ToString());
      writer.WriteElementString("AutoGeneratedSummaryLength", AutoGeneratedSummaryLength.ToString());
      writer.WriteElementString("FacebookAppId", FacebookAppId);
      writer.WriteElementString("FacebookProfileIdProperty", FacebookProfileIdProperty.ToString());

      writer.WriteElementString("RssEmail", RssEmail);
      writer.WriteElementString("RssDefaultNrItems", RssDefaultNrItems.ToString());
      writer.WriteElementString("RssMaxNrItems", RssMaxNrItems.ToString());
      writer.WriteElementString("RssTtl", RssTtl.ToString());
      writer.WriteElementString("RssImageWidth", RssImageWidth.ToString());
      writer.WriteElementString("RssImageHeight", RssImageHeight.ToString());
      writer.WriteElementString("RssImageSizeAllowOverride", RssImageSizeAllowOverride.ToString());
      writer.WriteElementString("RssAllowContentInFeed", RssAllowContentInFeed.ToString());
      writer.WriteElementString("RssDefaultCopyright", RssDefaultCopyright);
      writer.WriteElementString("IncrementViewCount", IncrementViewCount.ToString());
      writer.WriteEndElement(); // settings
    }

    public void FromXml(XmlNode xml)
    {
      if (xml is null)
        return;
      Extensions.ReadValue(ref xml, "ModuleID", ref _importedModuleId);
      bool argVariable = AllowWLW;
      Extensions.ReadValue(ref xml, "AllowWLW", ref argVariable);
      AllowWLW = argVariable;
      bool argVariable1 = AllowMultipleCategories;
      Extensions.ReadValue(ref xml, "AllowMultipleCategories", ref argVariable1);
      AllowMultipleCategories = argVariable1;
      int argVariable2 = VocabularyId;
      Extensions.ReadValue(ref xml, "VocabularyId", ref argVariable2);
      VocabularyId = argVariable2;
      bool argVariable3 = AllowAttachments;
      Extensions.ReadValue(ref xml, "AllowAttachments", ref argVariable3);
      AllowAttachments = argVariable3;
      var argVariable4 = SummaryModel;
      Extensions.ReadValue(ref xml, "SummaryModel", ref argVariable4);
      SummaryModel = argVariable4;
      string argVariable5 = StyleDetectionUrl;
      Extensions.ReadValue(ref xml, "StyleDetectionUrl", ref argVariable5);
      StyleDetectionUrl = argVariable5;
      int argVariable6 = WLWRecentPostsMax;
      Extensions.ReadValue(ref xml, "WLWRecentPostsMax", ref argVariable6);
      WLWRecentPostsMax = argVariable6;
      bool argVariable7 = ModifyPageDetails;
      Extensions.ReadValue(ref xml, "ModifyPageDetails", ref argVariable7);
      ModifyPageDetails = argVariable7;
      bool argVariable8 = AutoGenerateMissingSummary;
      Extensions.ReadValue(ref xml, "AutoGenerateMissingSummary", ref argVariable8);
      AutoGenerateMissingSummary = argVariable8;
      int argVariable9 = AutoGeneratedSummaryLength;
      Extensions.ReadValue(ref xml, "AutoGeneratedSummaryLength", ref argVariable9);
      AutoGeneratedSummaryLength = argVariable9;
      string argVariable10 = FacebookAppId;
      Extensions.ReadValue(ref xml, "FacebookAppId", ref argVariable10);
      FacebookAppId = argVariable10;
      int argVariable11 = FacebookProfileIdProperty;
      Extensions.ReadValue(ref xml, "FacebookProfileIdProperty", ref argVariable11);
      FacebookProfileIdProperty = argVariable11;

      string argVariable12 = RssEmail;
      Extensions.ReadValue(ref xml, "RssEmail", ref argVariable12);
      RssEmail = argVariable12;
      int argVariable13 = RssDefaultNrItems;
      Extensions.ReadValue(ref xml, "RssDefaultNrItems", ref argVariable13);
      RssDefaultNrItems = argVariable13;
      int argVariable14 = RssMaxNrItems;
      Extensions.ReadValue(ref xml, "RssMaxNrItems", ref argVariable14);
      RssMaxNrItems = argVariable14;
      int argVariable15 = RssTtl;
      Extensions.ReadValue(ref xml, "RssTtl", ref argVariable15);
      RssTtl = argVariable15;
      int argVariable16 = RssImageWidth;
      Extensions.ReadValue(ref xml, "RssImageWidth", ref argVariable16);
      RssImageWidth = argVariable16;
      int argVariable17 = RssImageHeight;
      Extensions.ReadValue(ref xml, "RssImageHeight", ref argVariable17);
      RssImageHeight = argVariable17;
      bool argVariable18 = RssImageSizeAllowOverride;
      Extensions.ReadValue(ref xml, "RssImageSizeAllowOverride", ref argVariable18);
      RssImageSizeAllowOverride = argVariable18;
      bool argVariable19 = RssAllowContentInFeed;
      Extensions.ReadValue(ref xml, "RssAllowContentInFeed", ref argVariable19);
      RssAllowContentInFeed = argVariable19;
      string argVariable20 = RssDefaultCopyright;
      Extensions.ReadValue(ref xml, "RssDefaultCopyright", ref argVariable20);
      RssDefaultCopyright = argVariable20;
      int argVariable21 = IncrementViewCount;
      Extensions.ReadValue(ref xml, "IncrementViewCount", ref argVariable21);
      IncrementViewCount = argVariable21;
    }
    #endregion

  }
}