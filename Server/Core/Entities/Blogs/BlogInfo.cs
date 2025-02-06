
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

using DotNetNuke.Modules.Blog.Core.Common;
using System.Runtime.Serialization;

namespace DotNetNuke.Modules.Blog.Core.Entities.Blogs
{

  public partial class BlogInfo
  {

    private Security.Permissions.BlogPermissionCollection _permissions;
    public Security.Permissions.BlogPermissionCollection Permissions
    {
      get
      {
        if (_permissions is null)
        {
          _permissions = Security.Permissions.BlogPermissionsController.GetBlogPermissionsCollection(BlogID);
        }
        return _permissions;
      }
      set
      {
        _permissions = value;
      }
    }

    [DataMember()]
    public bool CanEdit { get; set; } = false;
    [DataMember()]
    public bool CanAdd { get; set; } = false;
    [DataMember()]
    public bool CanApprove { get; set; } = false;
    [DataMember()]
    public bool IsOwner { get; set; } = false;

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
        _permaLink = DotNetNuke.Common.Globals.ApplicationURL(tab.TabID) + "&Blog=" + BlogID.ToString();
        if (DotNetNuke.Entities.Host.Host.UseFriendlyUrls)
        {
          _permaLink = DotNetNuke.Common.Globals.FriendlyUrl(tab, _permaLink, Globals.GetSafePageName(LocalizedTitle));
        }
        else
        {
          _permaLink = DotNetNuke.Common.Globals.ResolveUrl(_permaLink);
        }
      }
      return _permaLink;
    }

  }

}