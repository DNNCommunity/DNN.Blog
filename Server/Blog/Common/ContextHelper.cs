using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using DotNetNuke.UI.Utilities;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using DotNetNuke.Web.Api;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using DotNetNuke.Framework.JavaScriptLibraries;

namespace DotNetNuke.Modules.Blog.Common
{
    public class ContextHelper
    {
        public ModuleInfo ModuleContext { get; set; }
        public System.Web.UI.Page Page { get; set; }
        public virtual string ModulePath
        {
            get
            {
                return "~/DesktopModules/MVC/DotNetNuke/Blog";
            }
        }

        public ContextHelper(ViewContext viewContext)
        {
            Requires.NotNull("viewContext", viewContext);

            var controller = viewContext.Controller as IDnnController;

            if (controller == null)
            {
                throw new InvalidOperationException("The DnnUrlHelper class can only be used in Views that inherit from DnnWebViewPage");
            }

            ModuleContext = controller.ModuleContext.Configuration;
            Page = controller.DnnPage;
        }

        public ContextHelper(DnnController context)
        {
            Requires.NotNull("context", context);
            ModuleContext = context.ModuleContext.Configuration;
            Page = context.DnnPage;
        }

        public ContextHelper(DnnApiController context)
        {
            Requires.NotNull("context", context);
            ModuleContext = context.ActiveModule;
        }

        private ContextSecurity _security;
        public ContextSecurity Security
        {
            get { return _security ?? (_security = ContextSecurity.GetSecurity(ModuleContext)); }
        }

        public void RequirePermissionLevel(bool level)
        {
            if (!level)
            {
                ThrowAccessViolation();
            }
        }

        private ModuleSettings _settings;
        public ModuleSettings Settings
        {
            get { return _settings ?? (_settings = ModuleSettings.GetSettings(ModuleContext)); }
        }

        #region Css Files
        public void AddCss(string cssFile, string name, string version)
        {
            ClientResourceManager.RegisterStyleSheet(Page, string.Format(ModulePath + "/css/{0}", cssFile), 9, "", name, version);
        }
        public void AddCss(string cssFile)
        {
            ClientResourceManager.RegisterStyleSheet(Page, string.Format(ModulePath + "/css/{0}", cssFile), 9);
        }

        #endregion

        #region Js Files
        public void AddJqueryUi()
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(DotNetNuke.Framework.JavaScriptLibraries.CommonJs.jQueryUI);
        }
        public void AddJquery()
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(DotNetNuke.Framework.JavaScriptLibraries.CommonJs.jQuery);
        }
        public void AddDnnSF()
        {
            RegisterAjaxScript();
        }
        public void AddScript(string scriptName, string name, string version)
        {
            ClientResourceManager.RegisterScript(Page, string.Format(ModulePath + "/js/{0}", scriptName), 70, "", name, version);
        }
        public void AddScript(string scriptName)
        {
            ClientResourceManager.RegisterScript(Page, string.Format(ModulePath + "/js/{0}", scriptName));
        }
        public void AddModuleScript(string name)
        {
            AddScript(name + ".js");
        }
        public void AddModuleScript()
        {
            AddModuleScript("blog");
        }

        public void ThrowAccessViolation()
        {
            throw new Exception("You do not have adequate permissions to view this resource. Please check your login status.");
        }
        #endregion

        #region fixes for changes to DNN
        public void RegisterAjaxScript()
        {
            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            RegisterAjaxAntiForgery();
            var path = ServicesFramework.GetServiceFrameworkRoot();
            if (String.IsNullOrEmpty(path))
            {
                return;
            }

            JavaScript.RegisterClientReference(Page, ClientAPI.ClientNamespaceReferences.dnn);
            ClientAPI.RegisterClientVariable(Page, "sf_siteRoot", path, /*overwrite*/ true);
            ClientAPI.RegisterClientVariable(Page, "sf_tabId", DotNetNuke.Entities.Portals.PortalSettings.Current.ActiveTab.TabID.ToString(System.Globalization.CultureInfo.InvariantCulture), /*overwrite*/ true);

            string scriptPath;
            if (HttpContextSource.Current.IsDebuggingEnabled)
            {
                scriptPath = "~/js/Debug/dnn.servicesframework.js";
            }
            else
            {
                scriptPath = "~/js/dnn.servicesframework.js";
            }

            ClientResourceManager.RegisterScript(Page, scriptPath);
        }
        public void RegisterAjaxAntiForgery()
        {
            var ctl = Page.FindControl("ClientResourcesFormBottom");
            if (ctl != null)
            {
                var cc = ctl.Controls
                    .Cast<Control>()
                    .Where(l => l is LiteralControl)
                    .Select(l => (LiteralControl)l)
                    .Where(l => l.Text.IndexOf("__RequestVerificationToken") > 0)
                    .FirstOrDefault();
                if (cc == null)
                {
                    ctl.Controls.Add(new LiteralControl(System.Web.Helpers.AntiForgery.GetHtml().ToHtmlString()));
                }
            }
        }
        #endregion

    }
}
