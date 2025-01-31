
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
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Data;

namespace DotNetNuke.Modules.Blog.Entities.Posts
{

  public partial class PostsController
  {

    public static PostInfo GetPost(int contentItemId, int moduleId, string locale)
    {

      return CBO.FillObject<PostInfo>(DataProvider.Instance().GetPost(contentItemId, moduleId, locale));

    }

    public static int AddPost(ref PostInfo objPost, int createdByUser)
    {

      objPost.ContentItemId = DataProvider.Instance().AddPost(objPost.AllowComments, objPost.BlogID, objPost.Content, objPost.Copyright, objPost.DisplayCopyright, objPost.Image, objPost.Locale, objPost.Published, objPost.PublishedOnDate, objPost.Summary, objPost.Terms.ToTermIDString(), objPost.Title, objPost.ViewCount, createdByUser);

      // localization
      foreach (string l in objPost.TitleLocalizations.Locales)
        DataProvider.Instance().SetPostLocalization(objPost.ContentItemId, l, objPost.TitleLocalizations[l], objPost.SummaryLocalizations[l], objPost.ContentLocalizations[l], createdByUser);

      return objPost.ContentItemId;

    }

    public static void UpdatePost(PostInfo objPost, int updatedByUser)
    {

      DataProvider.Instance().UpdatePost(objPost.AllowComments, objPost.BlogID, objPost.Content, objPost.ContentItemId, objPost.Copyright, objPost.DisplayCopyright, objPost.Image, objPost.Locale, objPost.Published, objPost.PublishedOnDate, objPost.Summary, objPost.Terms.ToTermIDString(), objPost.Title, objPost.ViewCount, updatedByUser);

      // localization
      foreach (string l in objPost.TitleLocalizations.Locales)
        DataProvider.Instance().SetPostLocalization(objPost.ContentItemId, l, objPost.TitleLocalizations[l], objPost.SummaryLocalizations[l], objPost.ContentLocalizations[l], updatedByUser);

    }

    public static void DeletePost(int contentItemId)
    {

      DataProvider.Instance().DeletePost(contentItemId);

    }

  }
}