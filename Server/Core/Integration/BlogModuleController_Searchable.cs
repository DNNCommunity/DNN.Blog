using System;
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

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Modules.Blog.Core.Common;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using DotNetNuke.Modules.Blog.Core.Entities.Posts;
using DotNetNuke.Services.Search.Entities;

namespace DotNetNuke.Modules.Blog.Core.Integration
{
  public partial class BlogModuleController : ModuleSearchBase
  {

    #region  Search Implementation 
    public override IList<SearchDocument> GetModifiedSearchDocuments(ModuleInfo moduleInfo, DateTime beginDate)
    {

      var res = new List<SearchDocument>();
      var settings = new ViewSettings(moduleInfo.TabModuleID, true);
      if (settings.BlogModuleId != -1 & settings.BlogModuleId != moduleInfo.ModuleID)
        return res; // bail out if it's a slave module

      // Blogs
      foreach (BlogInfo b in BlogsController.GetBlogsByModule(moduleInfo.ModuleID, "").Values)
      {
        if (b.LastModifiedOnDate.ToUniversalTime() >= beginDate)
        {
          res.Add(new SearchDocument()
          {
            AuthorUserId = b.OwnerUserId,
            Body = b.Title + " - " + b.Description,
            Description = b.Description,
            ModifiedTimeUtc = b.LastModifiedOnDate.ToUniversalTime(),
            PortalId = moduleInfo.PortalID,
            QueryString = "Blog=" + b.BlogID.ToString(),
            Title = b.Title,
            UniqueKey = "Blog" + b.BlogID.ToString()
          });
        }
      }

      // Posts
      var addedPrimaryPosts = new List<int>();
      foreach (PostInfo p in PostsController.GetChangedPosts(moduleInfo.ModuleID, beginDate))
      {
        if (!addedPrimaryPosts.Contains(p.ContentItemId))
        {
          res.Add(new SearchDocument()
          {
            AuthorUserId = p.CreatedByUserID,
            Body = p.Summary + " " + HtmlUtils.Clean(p.Content, false),
            Description = p.Summary,
            ModifiedTimeUtc = p.LastModifiedOnDate.ToUniversalTime(),
            PortalId = moduleInfo.PortalID,
            QueryString = "Post=" + p.ContentItemId.ToString(),
            Title = p.Title,
            UniqueKey = "BlogPost" + p.ContentItemId.ToString()
          });
          addedPrimaryPosts.Add(p.ContentItemId);
        }
        if (!string.IsNullOrEmpty(p.AltLocale))
        {
          res.Add(new SearchDocument()
          {
            AuthorUserId = p.CreatedByUserID,
            Body = p.AltSummary + " " + HtmlUtils.Clean(p.AltContent, false),
            CultureCode = p.AltLocale,
            Description = p.AltSummary,
            ModifiedTimeUtc = p.LastModifiedOnDate.ToUniversalTime(),
            PortalId = moduleInfo.PortalID,
            QueryString = "Post=" + p.ContentItemId.ToString(),
            Title = p.AltTitle,
            UniqueKey = "BlogPost" + p.ContentItemId.ToString()
          });
        }
      }
      return res;

    }
    #endregion

  }
}