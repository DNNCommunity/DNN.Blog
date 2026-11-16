using System;
using System.IO;
using System.Web;
using DotNetNuke.Modules.Blog.Core.Common;

namespace DotNetNuke.Modules.Blog
{
    /// <summary>
    /// Serves and optionally resizes blog/post featured images from Blog/Files/{blogId}/{postId}/{key}.*
    /// </summary>
    public class BlogImage : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            int blogId = context.Request.Params.ReadValue("Blog", -1);
            int postId = context.Request.Params.ReadValue("Post", -1);
            string key = context.Request.Params.ReadValue("Key", "");
            int width = context.Request.Params.ReadValue("w", -1);
            int height = context.Request.Params.ReadValue("h", -1);
            bool crop = context.Request.Params.ReadValue("c", false);

            if (string.IsNullOrEmpty(key))
                return;

            try
            {
                string path = "";
                if (postId > -1)
                    path = Globals.GetPostDirectoryMapPath(blogId, postId);
                else if (blogId > -1)
                    path = Globals.GetBlogDirectoryMapPath(blogId);
                else
                    return;

                if (!Directory.Exists(path))
                    return;

                string[] files = Directory.GetFiles(path, string.Format("{0}-{1}-{2}-{3}.*", key, width, height, crop));
                if (files.Length > 0)
                {
                    if (!TrySetContentType(context, Path.GetExtension(files[0])))
                        return;
                    context.Response.WriteFile(files[0]);
                    return;
                }

                files = Directory.GetFiles(path, string.Format("{0}.*", key));
                if (files.Length == 0)
                    return;

                var img = new Image(files[0]);
                try
                {
                    if (!img.IsValidExtension)
                        return;
                    context.Response.ContentType = img.MimeType;
                    string newImg = img.ResizeImage(width, height, crop);
                    context.Response.WriteFile(newImg);
                }
                finally
                {
                    img.Dispose();
                }
            }
            catch (Exception)
            {
                // Ignore invalid/missing images — same as legacy handler.
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }

        private static bool TrySetContentType(HttpContext context, string extension)
        {
            switch ((extension ?? "").ToLowerInvariant())
            {
                case ".jpg":
                case ".jpeg":
                    context.Response.ContentType = "image/jpeg";
                    return true;
                case ".png":
                    context.Response.ContentType = "image/png";
                    return true;
                case ".gif":
                    context.Response.ContentType = "image/gif";
                    return true;
                case ".bmp":
                    context.Response.ContentType = "image/bmp";
                    return true;
                case ".tif":
                case ".tiff":
                    context.Response.ContentType = "image/tiff";
                    return true;
                default:
                    return false;
            }
        }
    }
}
