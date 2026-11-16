using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Core.Rss;
using BlogGlobals = DotNetNuke.Modules.Blog.Core.Common.Globals;

namespace DotNetNuke.Modules.Blog.Api
{
    public class RssController : BlogApiController
    {
        [HttpGet]
        [BlogAuthorize(SecurityAccessLevel.View)]
        [ActionName("Get")]
        public HttpResponseMessage GetRss()
        {
            var queryString = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var feed = new BlogRssFeed(ActiveModule.ModuleID, queryString);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(BlogGlobals.ReadFile(feed.CacheFile), Encoding.UTF8, "application/xml")
            };
            return response;
        }
    }
}
