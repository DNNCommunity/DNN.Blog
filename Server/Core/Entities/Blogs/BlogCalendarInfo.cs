using System;
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

using System.Runtime.Serialization;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Tokens;
using Microsoft.VisualBasic;

namespace DotNetNuke.Modules.Blog.Core.Entities.Blogs
{
  public class BlogCalendarInfo : IHydratable, IPropertyAccess
  {

    #region  ML Properties 
    public int ParentTabID { get; set; } = -1;
    #endregion

    #region  Public Properties 
    [DataMember()]
    public int PostYear { get; set; } = -1;
    [DataMember()]
    public int PostMonth { get; set; } = -1;
    [DataMember()]
    public int PostCount { get; set; } = -1;
    [DataMember()]
    public int ViewCount { get; set; } = -1;

    private DateTime _FirstDay = DateTime.MinValue;
    public DateTime FirstDay
    {
      get
      {
        if (_FirstDay == DateTime.MinValue)
        {
          _FirstDay = new DateTime(PostYear, PostMonth, 1);
        }
        return _FirstDay;
      }
    }

    private DateTime _FirstDayNextMonth = DateTime.MinValue;
    public DateTime FirstDayNextMonth
    {
      get
      {
        if (_FirstDayNextMonth == DateTime.MinValue)
        {
          _FirstDayNextMonth = new DateTime(PostYear, PostMonth, 1).AddMonths(1);
        }
        return _FirstDayNextMonth;
      }
    }

    private DateTime _LastDay = DateTime.MinValue;
    public DateTime LastDay
    {
      get
      {
        if (_LastDay == DateTime.MinValue)
        {
          _LastDay = new DateTime(PostYear, PostMonth, 1).AddMonths(1).AddDays(-1);
        }
        return _LastDay;
      }
    }
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
      PostYear = Convert.ToInt32(Null.SetNull(dr["PostYear"], PostYear));
      PostMonth = Convert.ToInt32(Null.SetNull(dr["PostMonth"], PostMonth));
      PostCount = Convert.ToInt32(Null.SetNull(dr["PostCount"], PostCount));
      ViewCount = Convert.ToInt32(Null.SetNull(dr["ViewCount"], ViewCount));
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
        case "postyear":
          {
            return PostYear.ToString(OutputFormat, formatProvider);
          }
        case "postmonth":
          {
            return PostMonth.ToString(OutputFormat, formatProvider);
          }
        case "postcount":
          {
            return PostCount.ToString(OutputFormat, formatProvider);
          }
        case "viewcount":
          {
            return ViewCount.ToString(OutputFormat, formatProvider);
          }
        case "firstday":
          {
            return FirstDay.ToString(OutputFormat, formatProvider);
          }
        case "lastday":
          {
            return LastDay.ToString(OutputFormat, formatProvider);
          }
        case "firstdaynextmonth":
          {
            return FirstDayNextMonth.ToString(OutputFormat, formatProvider);
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

    public string PermaLink(int strParentTabID)
    {
      var oTabController = new DotNetNuke.Entities.Tabs.TabController();
      var oParentTab = oTabController.GetTab(strParentTabID, DotNetNuke.Entities.Portals.PortalSettings.Current.PortalId, false);
      _permaLink = string.Empty;
      return PermaLink(oParentTab);
    }

    public string PermaLink(DotNetNuke.Entities.Portals.PortalSettings portalSettings)
    {
      return PermaLink(portalSettings.ActiveTab);
    }

    private string _permaLink = "";
    public string PermaLink(DotNetNuke.Entities.Tabs.TabInfo tab)
    {
      if (string.IsNullOrEmpty(_permaLink))
      {
        _permaLink = DotNetNuke.Common.Globals.ApplicationURL(tab.TabID) + "&end=" + FirstDayNextMonth.ToString();
        _permaLink = DotNetNuke.Common.Globals.FriendlyUrl(tab, _permaLink);
      }
      return _permaLink;
    }

  }
}