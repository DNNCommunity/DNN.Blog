using System.Web.Mvc;
using System.Web.Mvc.Html;
using DotNetNuke.Web.Mvc.Helpers;
using DotNetNuke.Web.Mvc.Framework;

namespace DotNetNuke.Modules.Blog.Common
{
    public abstract class BlogWebPage : DnnWebViewPage
    {

        public ContextHelper BlogModuleContext { get; set; }

        public HtmlHelper<object> MvcHtml { get; set; }

        public override void InitHelpers()
        {
            Ajax = new AjaxHelper<object>(ViewContext, this);
            Html = new DnnHtmlHelper<object>(ViewContext, this);
            MvcHtml = new HtmlHelper<object>(ViewContext, this);
            Url = new DnnUrlHelper(ViewContext);
            Dnn = new DnnHelper<object>(ViewContext, this);
            BlogModuleContext = new ContextHelper(ViewContext);
        }

        public MvcForm BeginForm(string actionName, string controllerName, FormMethod method, object htmlAttributes = null)
        {
            return FormExtensions.BeginForm(MvcHtml, actionName, controllerName, method, htmlAttributes);
        }

        public MvcForm BeginForm(string actionName, string controllerName, FormMethod method)
        {
            return FormExtensions.BeginForm(MvcHtml, actionName, controllerName, method, null);
        }

        public string SerializedResources()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(DotNetNuke.Services.Localization.LocalizationProvider.Instance.GetCompiledResourceFile(Dnn.PortalSettings, "/DesktopModules/MVC/Blog/App_LocalResources/ClientResources.resx",
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