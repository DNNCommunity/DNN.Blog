using System.Web.Mvc;
using DotNetNuke.Web.Mvc.Helpers;
using DotNetNuke.Web.Mvc.Framework;

namespace DotNetNuke.Modules.Blog.Common
{
    public abstract class BlogWebPage<TModel> : DnnWebViewPage<TModel>
    {

        public ContextHelper BlogModuleContext { get; set; }

        public override void InitHelpers()
        {
            Ajax = new AjaxHelper<TModel>(ViewContext, this);
            Html = new DnnHtmlHelper<TModel>(ViewContext, this);
            Url = new DnnUrlHelper(ViewContext);
            Dnn = new DnnHelper<TModel>(ViewContext, this);
            BlogModuleContext = new ContextHelper(ViewContext);
        }

        public string SerializedResources()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(DotNetNuke.Services.Localization.LocalizationProvider.Instance.GetCompiledResourceFile(Dnn.PortalSettings, "/DesktopModules/MVC/DotNetNuke/Blog/App_LocalResources/ClientResources.resx",
                    System.Threading.Thread.CurrentThread.CurrentCulture.Name));
        }

        public void RequirePermissionLevel(bool level)
        {
            BlogModuleContext.RequirePermissionLevel(level);
        }

        public string Locale
        {
            get
            {
                return System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            }
        }

    }
}