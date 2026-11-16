using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Core.Rss;
using Microsoft.Extensions.Logging;
using BlogGlobals = DotNetNuke.Modules.Blog.Core.Common.Globals;

namespace DotNetNuke.Modules.Blog.Api
{
    public class RssController : BlogApiController
    {
        private readonly ILogger<RssController> _logger;

        public RssController(ILogger<RssController> logger) : base()
        {
            _logger = logger;
        }

        [HttpGet]
        [BlogAuthorize(SecurityAccessLevel.View)]
        [ActionName("Get")]
        public HttpResponseMessage GetRss()
        {
            _logger.LogDebug("RSS feed requested for module {ModuleId}, query {Query}",
                ActiveModule.ModuleID, Request.RequestUri.Query);

            try
            {
                var queryString = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var feed = new BlogRssFeed(ActiveModule.ModuleID, queryString);
                var content = BlogGlobals.ReadFile(feed.CacheFile);
                _logger.LogInformation("RSS feed served for module {ModuleId} from cache file {CacheFile}, length {Length}",
                    ActiveModule.ModuleID, feed.CacheFile, content?.Length ?? 0);
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(content, Encoding.UTF8, "application/xml")
                };
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RSS feed failed for module {ModuleId}", ActiveModule.ModuleID);
                throw;
            }
        }
    }
}
