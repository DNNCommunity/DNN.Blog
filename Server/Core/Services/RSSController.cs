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

using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using static DotNetNuke.Modules.Blog.Common.Globals;
using DotNetNuke.Modules.Blog.Rss;
using DotNetNuke.Web.Api;

namespace DotNetNuke.Modules.Blog.Services
{

  public class RSSController : DnnApiController
  {

    #region  Private Members 
    #endregion

    #region  Service Methods 
    [HttpGet()]
    [DnnModuleAuthorize(AccessLevel = DotNetNuke.Security.SecurityAccessLevel.View)]
    [ActionName("Get")]
    public HttpResponseMessage GetRss()
    {
      var res = new HttpResponseMessage(HttpStatusCode.OK);
      var queryString = HttpUtility.ParseQueryString(Request.RequestUri.Query);
      var feed = new BlogRssFeed(ActiveModule.ModuleID, queryString);
      res.Content = new StringContent(ReadFile(feed.CacheFile), Encoding.UTF8, "application/xml");
      return res;
    }
    #endregion

    #region  Private Methods 
    #endregion

  }

}