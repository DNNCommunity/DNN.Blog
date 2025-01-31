using System;
using System.Data;
using System.Runtime.Serialization;
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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Tokens;
using Microsoft.VisualBasic.CompilerServices;

namespace DotNetNuke.Modules.Blog.Entities.Posts
{

  public class PostAuthor : UserInfo, IHydratable, IPropertyAccess
  {

    #region  Public Properties 
    [DataMember()]
    public int ParentTabID { get; set; } = -1;
    public int NrPosts { get; set; } = 0;
    [DataMember()]
    public int NrViews { get; set; } = 0;
    [DataMember()]
    public DateTime LastPublishDate { get; set; } = DateTime.MinValue;
    [DataMember()]
    public DateTime FirstPublishDate { get; set; } = DateTime.MinValue;
    #endregion

    #region  IHydratable Implementation 
    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Fill hydrates the object from a Datareader
  /// </summary>
  /// <remarks>The Fill method is used by the CBO method to hydrtae the object
  /// rather than using the more expensive Refection  methods.</remarks>
  /// <history>
  /// 	[pdonker]	02/16/2013  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    public void Fill(IDataReader dr)
    {
      PortalID = Convert.ToInt32(Null.SetNull(dr["PortalID"], PortalID));
      IsSuperUser = Convert.ToBoolean(Null.SetNull(dr["IsSuperUser"], IsSuperUser));
      UserID = Convert.ToInt32(Null.SetNull(dr["UserID"], UserID));
      FirstName = Convert.ToString(Null.SetNull(dr["FirstName"], FirstName));
      LastName = Convert.ToString(Null.SetNull(dr["LastName"], LastName));
      DisplayName = Convert.ToString(Null.SetNull(dr["DisplayName"], DisplayName));
      Username = Convert.ToString(Null.SetNull(dr["Username"], Username));
      Email = Convert.ToString(Null.SetNull(dr["Email"], Email));
      NrPosts = Convert.ToInt32(Null.SetNull(dr["NrPosts"], NrPosts));
      NrViews = Convert.ToInt32(Null.SetNull(dr["NrViews"], NrViews));
      LastPublishDate = Conversions.ToDate(Null.SetNull(dr["LastPublishDate"], LastPublishDate));
      FirstPublishDate = Conversions.ToDate(Null.SetNull(dr["FirstPublishDate"], FirstPublishDate));
    }
    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Gets and sets the Key ID
  /// </summary>
  /// <remarks>The KeyID property is part of the IHydratble interface.  It is used
  /// as the key property when creating a Dictionary</remarks>
  /// <history>
  /// 	[pdonker]	02/16/2013  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    public int KeyID
    {
      get
      {
        return default;
      }
      set
      {
      }
    }
    #endregion

    #region  IPropertyAccess Implementation 
    public new string GetProperty(string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, UserInfo AccessingUser, Scope AccessLevel, ref bool PropertyNotFound)
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
        case "nrposts":
          {
            return NrPosts.ToString(OutputFormat, formatProvider);
          }
        case "nrviews":
          {
            return NrViews.ToString(OutputFormat, formatProvider);
          }
        case "lastpublishdate":
          {
            return LastPublishDate.ToString(OutputFormat, formatProvider);
          }
        case "firstpublishdate":
          {
            return FirstPublishDate.ToString(OutputFormat, formatProvider);
          }
        case "parenturl":
          {
            return PermaLink(ParentTabID);
          }

        default:
          {
            return base.GetProperty(strPropertyName, strFormat, formatProvider, AccessingUser, AccessLevel, ref PropertyNotFound);
          }
      }
    }

    public new CacheLevel Cacheability
    {
      get
      {
        return CacheLevel.fullyCacheable;
      }
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
        _permaLink = ApplicationURL(tab.TabID) + "&author=" + UserID.ToString();
        if (DotNetNuke.Entities.Host.Host.UseFriendlyUrls)
        {
          _permaLink = FriendlyUrl(tab, _permaLink, "");
        }
        else
        {
          _permaLink = ResolveUrl(_permaLink);
        }
      }
      return _permaLink;
    }

    #endregion
  }
}