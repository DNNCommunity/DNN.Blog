using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using DotNetNuke.Modules.Blog.Core.Media;
using DotNetNuke.Web.Api;

namespace DotNetNuke.Modules.Blog.Api
{
    public class FileController : DotNetNuke.Modules.Blog.Common.BlogApiController
    {
        private static readonly MediaFileService Service = new MediaFileService();

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage Get(Guid id, string v = null)
        {
            var file = Service.Get(id);
            if (file == null || file.PortalId != PortalSettings.PortalId)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            if (file.IsTemp)
            {
                var u = UserInfo;
                var allowed = u != null && u.UserID > 0 &&
                    (u.UserID == file.CreatedByUserId || u.IsSuperUser || u.IsInRole(PortalSettings.AdministratorRoleName));
                if (!allowed) return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var path = Service.GetPath(file);
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return Request.CreateResponse(HttpStatusCode.NotFound);

            var lastModified = File.GetLastWriteTimeUtc(path);
            lastModified = new DateTime(lastModified.Ticks - (lastModified.Ticks % TimeSpan.TicksPerSecond), DateTimeKind.Utc);
            var etag = "\"" + (file.Hash ?? "") + lastModified.Ticks + "\"";

            var cache = new CacheControlHeaderValue { Public = !file.IsTemp };
            if (file.IsTemp) { cache.Private = true; cache.NoCache = true; }
            else if (!string.IsNullOrEmpty(v)) cache.MaxAge = TimeSpan.FromDays(365);
            else cache.MaxAge = TimeSpan.FromHours(1);

            var inm = Request.Headers.IfNoneMatch;
            bool notModified = false;
            foreach (var t in inm) if (t.Tag == etag) notModified = true;

            HttpResponseMessage resp;
            if (notModified)
            {
                resp = Request.CreateResponse(HttpStatusCode.NotModified);
            }
            else
            {
                resp = Request.CreateResponse(HttpStatusCode.OK);
                resp.Content = new StreamContent(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read));
                resp.Content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
                resp.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline")
                {
                    FileName = file.OriginalFileName
                };
                resp.Content.Headers.LastModified = lastModified;
            }
            resp.Headers.CacheControl = cache;
            resp.Headers.ETag = new EntityTagHeaderValue(etag);
            resp.Headers.Add("X-Content-Type-Options", "nosniff");
            return resp;
        }
    }
}
