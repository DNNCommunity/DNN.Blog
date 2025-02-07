using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Modules.Blog.Core.Common;
using DotNetNuke.Services.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

using static DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Blog.Core.Entities.Terms
{
  [Serializable()]
  public class TermInfo : Term, IHydratable, IPropertyAccess
  {

    #region  Constructors 
    public TermInfo()
    {
    }
    public TermInfo(string name)
    {
      Description = "";
      Name = name;
      ParentTermId = 0;
      TermId = -1;
    }
    #endregion

    #region  Public Properties 
    public int TotalPosts { get; set; }
    #endregion

    #region  ML Properties 
    public int ParentTabID { get; set; } = -1;
    public string AltLocale { get; set; } = "";
    public string AltName { get; set; } = "";
    public string AltDescription { get; set; } = "";

    public string LocalizedName
    {
      get
      {
        return string.IsNullOrEmpty(AltName) ? Name : AltName;
      }
    }

    public string LocalizedDescription
    {
      get
      {
        return string.IsNullOrEmpty(AltDescription) ? Description : AltDescription;
      }
    }

    /// <summary>
    /// ML text type to handle the name of the term
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public LocalizedText NameLocalizations
    {
      get
      {
        if (_nameLocalizations is null)
        {
          if (TermId == -1)
          {
            _nameLocalizations = new LocalizedText();
          }
          else
          {
            _nameLocalizations = new LocalizedText(Data.DataProvider.Instance().GetTermLocalizations(TermId), "Name");
          }
        }
        return _nameLocalizations;
      }
      set
      {
        _nameLocalizations = value;
      }
    }
    private LocalizedText _nameLocalizations;

    /// <summary>
    /// ML text type to handle the description of the term
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public LocalizedText DescriptionLocalizations
    {
      get
      {
        if (_descriptionLocalizations is null)
        {
          if (TermId == -1)
          {
            _descriptionLocalizations = new LocalizedText();
          }
          else
          {
            _descriptionLocalizations = new LocalizedText(Data.DataProvider.Instance().GetTermLocalizations(TermId), "Description");
          }
        }
        return _descriptionLocalizations;
      }
      set
      {
        _descriptionLocalizations = value;
      }
    }
    private LocalizedText _descriptionLocalizations;
    #endregion

    #region  IHydratable Implementation 
    public override void Fill(IDataReader dr)
    {
      base.Fill(dr);
      base.FillInternal(dr);

      TotalPosts = Null.SetNullInteger(dr["TotalPosts"]);
      AltLocale = Convert.ToString(Null.SetNull(dr["AltLocale"], AltLocale));
      AltName = Convert.ToString(Null.SetNull(dr["AltName"], AltName));
      AltDescription = Convert.ToString(Null.SetNull(dr["AltDescription"], AltDescription));
    }
    #endregion

    #region  IPropertyAccess Implementation 
    public string GetProperty(string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, DotNetNuke.Entities.Users.UserInfo AccessingUser, Scope AccessLevel, ref bool PropertyNotFound)
    {
      string OutputFormat = string.Empty;
      var portalSettings = Framework.ServiceLocator<DotNetNuke.Entities.Portals.IPortalController, DotNetNuke.Entities.Portals.PortalController>.Instance.GetCurrentPortalSettings();

      var ParentModule = new ModuleInfo();
      // PropertySource(strObjectName.ToLower).GetProperty(strPropertyName, strFormat, formatProvider, AccessingUser, CurrentAccessLevel, PropertyNotFound)

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
        case "description":
          {
            return PropertyAccess.FormatString(Description, strFormat);
          }
        case "isheirarchical":
          {
            return IsHeirarchical.ToString();
          }
        case "isheirarchicalyesno":
          {
            return PropertyAccess.Boolean2LocalizedYesNo(IsHeirarchical, formatProvider);
          }
        case "left":
          {
            return Left.ToString(OutputFormat, formatProvider);
          }
        case "title":
        case "name":
          {
            return PropertyAccess.FormatString(Name, strFormat);
          }
        case "parenttermid":
          {
            return ParentTermId.ToStringOrZero();
          }
        case "right":
          {
            return Right.ToString(OutputFormat, formatProvider);
          }
        case "termid":
          {
            return TermId.ToString(OutputFormat, formatProvider);
          }
        case "vocabularyid":
          {
            return VocabularyId.ToString(OutputFormat, formatProvider);
          }
        case "weight":
          {
            return Weight.ToString(OutputFormat, formatProvider);
          }
        case "createdbyuserid":
          {
            return CreatedByUserID.ToString(OutputFormat, formatProvider);
          }
        case "createdondate":
          {
            return CreatedOnDate.ToString(OutputFormat, formatProvider);
          }
        case "lastmodifiedbyuserid":
          {
            return LastModifiedByUserID.ToString(OutputFormat, formatProvider);
          }
        case "lastmodifiedondate":
          {
            return LastModifiedOnDate.ToString(OutputFormat, formatProvider);
          }
        case "totalposts":
          {
            return TotalPosts.ToString(OutputFormat, formatProvider);
          }
        case "altlocale":
          {
            return PropertyAccess.FormatString(AltLocale, strFormat);
          }
        case "altname":
          {
            return PropertyAccess.FormatString(AltName, strFormat);
          }
        case "altdescription":
          {
            return PropertyAccess.FormatString(AltDescription, strFormat);
          }
        case "localizedname":
          {
            return PropertyAccess.FormatString(LocalizedName, strFormat);
          }
        case "localizeddescription":
          {
            return PropertyAccess.FormatString(LocalizedDescription, strFormat);
          }
        case "link":
        case "permalink":
          {
            return PermaLink(DotNetNuke.Entities.Portals.PortalSettings.Current);
          }
        case "parenturl":
          {
            return PermaLink(ParentTabID);
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

    #region  (De)serialization 
    internal List<TermInfo> ImportedChildTerms { get; set; }
    public void FromXml(XmlNode xml)
    {
      if (xml is null)
        return;
      Name = xml.ReadValue("Name", Name);
      NameLocalizations = xml.ReadValue("NameLocalizations", NameLocalizations);
      Description = xml.ReadValue("Description", Description);
      DescriptionLocalizations = xml.ReadValue("DescriptionLocalizations", DescriptionLocalizations);
      foreach (XmlNode xTerm in xml.SelectNodes("Term"))
      {
        var t = new TermInfo();
        t.FromXml(xTerm);
        ImportedChildTerms.Add(t);
      }
    }

    public void WriteXml(XmlWriter writer, List<TermInfo> vocabulary)
    {
      writer.WriteStartElement("Term");
      writer.WriteElementString("Name", Name);
      writer.WriteElementString("NameLocalizations", NameLocalizations.ToString());
      writer.WriteElementString("Description", Description);
      writer.WriteElementString("DescriptionLocalizations", DescriptionLocalizations.ToString());
      foreach (TermInfo t in vocabulary.Where(x => x.ParentTermId.HasValue && x.ParentTermId.Value == TermId))
      {
        t.WriteXml(writer, vocabulary);
      }
      writer.WriteEndElement(); // Term
    }
    #endregion

    #region  Public Methods 
    public string PermaLink(DotNetNuke.Entities.Portals.PortalSettings portalSettings)
    {
      return PermaLink(portalSettings.ActiveTab);
    }
    public string PermaLink(int strParentTabID)
    {
      var oTabController = new DotNetNuke.Entities.Tabs.TabController();
      var oParentTab = oTabController.GetTab(strParentTabID, DotNetNuke.Entities.Portals.PortalSettings.Current.PortalId, false);
      _permaLink = string.Empty;
      return PermaLink(oParentTab);
    }

    private string _permaLink = "";
    public string PermaLink(DotNetNuke.Entities.Tabs.TabInfo tab)
    {
      if (string.IsNullOrEmpty(_permaLink))
      {
        _permaLink = ApplicationURL(tab.TabID) + "&Term=" + TermId.ToString();
        if (DotNetNuke.Entities.Host.Host.UseFriendlyUrls)
        {
          _permaLink = FriendlyUrl(tab, _permaLink, Globals.GetSafePageName(LocalizedName));
        }
        else
        {
          _permaLink = ResolveUrl(_permaLink);
        }
      }
      return _permaLink;
    }

    public TermInfo FlatClone()
    {
      var res = new TermInfo();
      res.Name = Name;
      res.NameLocalizations = NameLocalizations;
      res.Name = Description;
      res.DescriptionLocalizations = DescriptionLocalizations;
      res.Weight = Weight;
      return res;
    }
    #endregion

  }
}