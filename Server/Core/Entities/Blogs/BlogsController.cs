using System.Collections.Generic;
using DotNetNuke.Common.Utilities;
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

using DotNetNuke.Modules.Blog.Data;

namespace DotNetNuke.Modules.Blog.Entities.Blogs
{

  public partial class BlogsController
  {

    public static BlogInfo GetBlog(int blogID, int userId, string locale)
    {

      return CBO.FillObject<BlogInfo>(DataProvider.Instance().GetBlog(blogID, userId, locale));

    }

    public static Dictionary<int, BlogInfo> GetBlogsByModule(int moduleID, string locale)
    {

      return GetBlogsByModule(moduleID, -1, locale);

    }

    public static Dictionary<int, BlogInfo> GetBlogsByModule(int moduleID, int userId, string locale)
    {

      var res = CBO.FillDictionary<int, BlogInfo>("BlogID", DataProvider.Instance().GetBlogsByModule(moduleID, userId, locale));
      if (userId > -1)
      {
        foreach (BlogInfo b in res.Values)
        {
          if (b.OwnerUserId == userId)
          {
            b.CanAdd = true;
            b.CanEdit = true;
            b.CanApprove = true;
            b.IsOwner = true;
          }
        }
      }
      return res;

    }

    public static Dictionary<int, BlogInfo> GetBlogsByPortal(int portalId, int userId, string locale)
    {

      var res = CBO.FillDictionary<int, BlogInfo>("BlogID", DataProvider.Instance().GetBlogsByPortal(portalId, userId, locale));
      if (userId > -1)
      {
        foreach (BlogInfo b in res.Values)
        {
          if (b.OwnerUserId == userId)
          {
            b.CanAdd = true;
            b.CanEdit = true;
            b.CanApprove = true;
            b.IsOwner = true;
          }
        }
      }
      return res;

    }

    public static List<BlogCalendarInfo> GetBlogCalendar(int moduleId, int blogId, string locale)
    {
      return CBO.FillCollection<BlogCalendarInfo>(DataProvider.Instance().GetBlogCalendar(moduleId, blogId, locale));
    }

  }

}