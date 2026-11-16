using DotNetNuke.Web.Api;

namespace DotNetNuke.Modules.Blog.Common
{
    public class RouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute("DotNetNuke/Blog", "BlogMap1", "{controller}/{action}", null, null, new[] { "DotNetNuke.Modules.Blog.Api" });
            mapRouteManager.MapHttpRoute("DotNetNuke/Blog", "BlogMap2", "{controller}/{action}/{id}", null, new { id = "-?\\d+" }, new[] { "DotNetNuke.Modules.Blog.Api" });
        }
    }
}