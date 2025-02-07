using System.Collections.Generic;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using DotNetNuke.Modules.Blog.Core.Entities.Posts;
using DotNetNuke.Security.Permissions;
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

using DotNetNuke.Services.Sitemap;

namespace DotNetNuke.Modules.Blog.Core.Integration
{
  public class BlogSiteMapProvider : SitemapProvider
  {

    public override List<SitemapUrl> GetUrls(int portalId, DotNetNuke.Entities.Portals.PortalSettings ps, string version)
    {

      var SitemapUrls = new List<SitemapUrl>();
      var moduleTabs = new Dictionary<int, List<DotNetNuke.Entities.Tabs.TabInfo>>();
      foreach (BlogInfo blog in BlogsController.GetBlogsByPortal(portalId, -1, "").Values)
      {
        if (!moduleTabs.ContainsKey(blog.ModuleID))
        {
          var tabs = new List<DotNetNuke.Entities.Tabs.TabInfo>();
          foreach (DotNetNuke.Entities.Tabs.TabInfo t in new DotNetNuke.Entities.Tabs.TabController().GetTabsByModuleID(blog.ModuleID).Values)
          {
            foreach (TabPermissionInfo tp in t.TabPermissions)
            {
              if ((tp.RoleName ?? "") == DotNetNuke.Common.Globals.glbRoleAllUsersName)
              {
                tabs.Add(t);
                break;
              }
            }
          }
          moduleTabs.Add(blog.ModuleID, tabs);
        }
        int totalRecs = 0;
        foreach (PostInfo p in PostsController.GetPostsByBlog(blog.ModuleID, blog.BlogID, System.Threading.Thread.CurrentThread.CurrentCulture.Name, -1, -1, 0, null, ref totalRecs).Values)
        {
          if (p.Published)
          {
            foreach (DotNetNuke.Entities.Tabs.TabInfo t in moduleTabs[blog.ModuleID])
            {
              var smu = new SitemapUrl() { ChangeFrequency = SitemapChangeFrequency.Daily, LastModified = p.LastModifiedOnDate, Priority = 0.5f, Url = p.PermaLink(t) };
              SitemapUrls.Add(smu);
            }
          }
        }
      }
      return SitemapUrls;

    }

  }
}