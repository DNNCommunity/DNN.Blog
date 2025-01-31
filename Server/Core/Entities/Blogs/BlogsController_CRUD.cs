
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

    public static int AddBlog(ref BlogInfo objBlog, int createdByUser)
    {

      objBlog.BlogID = DataProvider.Instance().AddBlog(objBlog.AutoApprovePingBack, objBlog.ModuleID, objBlog.AutoApproveTrackBack, objBlog.Copyright, objBlog.Description, objBlog.EnablePingBackReceive, objBlog.EnablePingBackSend, objBlog.EnableTrackBackReceive, objBlog.EnableTrackBackSend, objBlog.FullLocalization, objBlog.Image, objBlog.IncludeAuthorInFeed, objBlog.IncludeImagesInFeed, objBlog.Locale, objBlog.MustApproveGhostPosts, objBlog.OwnerUserId, objBlog.PublishAsOwner, objBlog.Published, objBlog.Syndicated, objBlog.SyndicationEmail, objBlog.Title, createdByUser);

      // localization
      foreach (string l in objBlog.TitleLocalizations.Locales)
        DataProvider.Instance().SetBlogLocalization(objBlog.BlogID, l, objBlog.TitleLocalizations[l], objBlog.DescriptionLocalizations[l]);

      return objBlog.BlogID;

    }

    public static void UpdateBlog(BlogInfo objBlog, int updatedByUser)
    {

      DataProvider.Instance().UpdateBlog(objBlog.AutoApprovePingBack, objBlog.ModuleID, objBlog.AutoApproveTrackBack, objBlog.BlogID, objBlog.Copyright, objBlog.Description, objBlog.EnablePingBackReceive, objBlog.EnablePingBackSend, objBlog.EnableTrackBackReceive, objBlog.EnableTrackBackSend, objBlog.FullLocalization, objBlog.Image, objBlog.IncludeAuthorInFeed, objBlog.IncludeImagesInFeed, objBlog.Locale, objBlog.MustApproveGhostPosts, objBlog.OwnerUserId, objBlog.PublishAsOwner, objBlog.Published, objBlog.Syndicated, objBlog.SyndicationEmail, objBlog.Title, updatedByUser);

      // localization
      foreach (string l in objBlog.TitleLocalizations.Locales)
        DataProvider.Instance().SetBlogLocalization(objBlog.BlogID, l, objBlog.TitleLocalizations[l], objBlog.DescriptionLocalizations[l]);

    }

    public static void DeleteBlog(int blogID)
    {

      DataProvider.Instance().DeleteBlog(blogID);

    }

  }
}