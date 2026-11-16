using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Web.Http;
using DotNetNuke.Modules.Blog.Core.Media;
using DotNetNuke.Web.Api;

namespace DotNetNuke.Modules.Blog.Api
{
    public class ImageController : DotNetNuke.Modules.Blog.Common.BlogApiController
    {
        private static readonly ImageService Service = new ImageService();

        /// <summary>Serves an original or resized variant. Ids are unguessable guids; temp images are limited to their uploader and admins.</summary>
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage Get(Guid id, int w = 0, int h = 0, bool c = false, string v = null)
        {
            var img = Service.Get(id);
            if (img == null || img.PortalId != PortalSettings.PortalId)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            if (img.IsTemp)
            {
                var u = UserInfo;
                var allowed = u != null && u.UserID > 0 &&
                    (u.UserID == img.CreatedByUserId || u.IsSuperUser || u.IsInRole(PortalSettings.AdministratorRoleName));
                if (!allowed) return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            string path;
            try
            {
                path = Service.GetVariantPath(img, w, h, c);
            }
            catch (Exception ex) when (ex is IOException || ex is ExternalException || ex is OutOfMemoryException)
            {
                Logger.Error("Image variant failed for " + id, ex);
                path = null;
            }
            if (path == null || !File.Exists(path))
                return Request.CreateResponse(HttpStatusCode.NotFound);

            var lastModified = File.GetLastWriteTimeUtc(path);
            lastModified = new DateTime(lastModified.Ticks - (lastModified.Ticks % TimeSpan.TicksPerSecond), DateTimeKind.Utc);
            var etag = "\"" + (img.Hash ?? "") + "-" + Path.GetFileNameWithoutExtension(path).Substring(0, Math.Min(4, Path.GetFileNameWithoutExtension(path).Length)) + lastModified.Ticks + "\"";

            var cache = new CacheControlHeaderValue { Public = !img.IsTemp };
            if (img.IsTemp) { cache.Private = true; cache.NoCache = true; }
            else if (!string.IsNullOrEmpty(v)) cache.MaxAge = TimeSpan.FromDays(365);
            else cache.MaxAge = TimeSpan.FromHours(1);

            var inm = Request.Headers.IfNoneMatch;
            var ims = Request.Headers.IfModifiedSince;
            bool notModified = false;
            foreach (var t in inm) if (t.Tag == etag) notModified = true;
            if (!notModified && inm.Count == 0 && ims.HasValue && ims.Value.UtcDateTime >= lastModified) notModified = true;

            HttpResponseMessage resp;
            if (notModified)
            {
                resp = Request.CreateResponse(HttpStatusCode.NotModified);
            }
            else
            {
                resp = Request.CreateResponse(HttpStatusCode.OK);
                resp.Content = new StreamContent(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read));
                resp.Content.Headers.ContentType = new MediaTypeHeaderValue(img.ContentType);
                resp.Content.Headers.LastModified = lastModified;
            }
            resp.Headers.CacheControl = cache;
            resp.Headers.ETag = new EntityTagHeaderValue(etag);
            resp.Headers.Add("X-Content-Type-Options", "nosniff");
            return resp;
        }
    }
}
