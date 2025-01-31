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

using System.Web;
using DotNetNuke.Web.Api;

namespace DotNetNuke.Modules.Blog.Services
{

  public class BlogRouteMapper : IServiceRouteMapper
  {

    public enum ServiceControllers
    {
      Blogs,
      Comments,
      Posts,
      Terms
    }

    public const string ServicePath = "~/DesktopModules/Blog/API/";

    public void RegisterRoutes(IMapRoute mapRouteManager)
    {
      mapRouteManager.MapHttpRoute("Blog", "Blogs", "Blogs/{action}", new { Controller = "Blogs" }, new string[] { "DotNetNuke.Modules.Blog.Entities.Blogs" });
      mapRouteManager.MapHttpRoute("Blog", "Comments", "Comments/{action}", new { Controller = "Comments" }, new string[] { "DotNetNuke.Modules.Blog.Entities.Comments" });
      mapRouteManager.MapHttpRoute("Blog", "Posts", "Posts/{action}", new { Controller = "Posts" }, new string[] { "DotNetNuke.Modules.Blog.Entities.Posts" });
      mapRouteManager.MapHttpRoute("Blog", "Terms", "Terms/{action}", new { Controller = "Terms" }, new string[] { "DotNetNuke.Modules.Blog.Entities.Terms" });
      mapRouteManager.MapHttpRoute("Blog", "Other", "{controller}/{action}", new string[] { "DotNetNuke.Modules.Blog.Services", "DotNetNuke.Modules.Blog.Integration.Services" });
    }

    public static string GetRoute(ServiceControllers controller, string @method)
    {
      switch (controller)
      {
        case ServiceControllers.Blogs:
          {
            return GetRoute("Blogs", @method);
          }
        case ServiceControllers.Comments:
          {
            return GetRoute("Comments", @method);
          }
        case ServiceControllers.Posts:
          {
            return GetRoute("Posts", @method);
          }

        default:
          {
            return GetRoute("Terms", @method);
          }
      }
    }

    public static string GetRoute(string controller, string @method)
    {
      return HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + DotNetNuke.Common.Globals.ResolveUrl(ServicePath + controller + "/" + @method);
    }

  }

}