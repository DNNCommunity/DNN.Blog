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
using System.Web.UI.WebControls;
using static DotNetNuke.Common.Globals;
using static DotNetNuke.Modules.Blog.Common.Globals;
using DotNetNuke.Modules.Blog.Entities.Blogs;
using DotNetNuke.Modules.Blog.Entities.Terms;

namespace DotNetNuke.Modules.Blog.Entities.Posts
{

  public partial class PostInfo
  {

    public List<TermInfo> PostCategories
    {
      get
      {
        return Terms.Where(t => t.VocabularyId != 1).ToList();
      }
    }

    public List<TermInfo> PostTags
    {
      get
      {
        return Terms.Where(t => t.VocabularyId == 1).ToList();
      }
    }

    public string PermaLink(int strParentTabID)
    {
      var oTabController = new DotNetNuke.Entities.Tabs.TabController();
      var oParentTab = oTabController.GetTab(strParentTabID, DotNetNuke.Entities.Portals.PortalSettings.Current.PortalId, false);
      _permaLink = string.Empty;
      return PermaLink(oParentTab);
    }

    public string PermaLink()
    {
      return PermaLink(DotNetNuke.Entities.Portals.PortalSettings.Current.ActiveTab);
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
        _permaLink = ApplicationURL(tab.TabID);
        if (!string.IsNullOrEmpty(Locale))
        {
          _permaLink += "&language=" + Locale;
        }
        _permaLink += "&Post=" + ContentItemId.ToString();
        if (DotNetNuke.Entities.Host.Host.UseFriendlyUrls)
        {
          _permaLink = FriendlyUrl(tab, _permaLink, GetSafePageName(LocalizedTitle));
        }
        else
        {
          _permaLink = ResolveUrl(_permaLink);
        }
      }
      return _permaLink;
    }

    private List<TermInfo> _terms;
    public new List<TermInfo> Terms
    {
      get
      {
        if (_terms is null)
        {
          _terms = TermsController.GetTermsByPost(ContentItemId, Blog.ModuleID, System.Threading.Thread.CurrentThread.CurrentCulture.Name);
        }
        if (_terms is null)
        {
          _terms = new List<TermInfo>();
        }
        return _terms;
      }
      set
      {
        _terms = value;
      }
    }

    private BlogInfo _blog = null;
    public BlogInfo Blog
    {
      get
      {
        if (_blog is null)
        {
          _blog = BlogsController.GetBlog(BlogID, -1, System.Threading.Thread.CurrentThread.CurrentCulture.Name);
        }
        return _blog;
      }
      set
      {
        _blog = value;
      }
    }
  }
}