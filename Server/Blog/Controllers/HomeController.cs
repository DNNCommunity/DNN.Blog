using System.Web.Mvc;
using DotNetNuke.Modules.Blog.Common;

namespace DotNetNuke.Modules.Blog.Controllers
{
    public class HomeController : BlogMvcController
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View(BlogModuleContext.Settings.View);
        }
    }
}
