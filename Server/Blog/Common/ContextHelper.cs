using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Modules.Blog.Core.Common;
using DotNetNuke.Modules.Blog.Core.Templating;
using DotNetNuke.UI.Utilities;
using DotNetNuke.Web.Api;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using CoreModuleSettings = DotNetNuke.Modules.Blog.Core.Common.ModuleSettings;
using CoreViewSettings = DotNetNuke.Modules.Blog.Core.Common.ViewSettings;
using CoreSecurity = DotNetNuke.Modules.Blog.Core.Security.ContextSecurity;
using BlogGlobals = DotNetNuke.Modules.Blog.Core.Common.Globals;

namespace DotNetNuke.Modules.Blog.Common
{
    public class ContextHelper : IBlogModuleHost
    {
        public ModuleInfo ModuleContext { get; set; }
        public System.Web.UI.Page Page { get; set; }
        public DnnController Controller { get; private set; }

        public virtual string ModulePath
        {
            get { return BlogGlobals.ModuleFolderPath; }
        }

        public ContextHelper(ViewContext viewContext)
        {
            Requires.NotNull("viewContext", viewContext);

            var controller = viewContext.Controller as IDnnController;
            if (controller == null)
            {
                throw new InvalidOperationException("The ContextHelper can only be used in Views that inherit from DnnWebViewPage");
            }

            ModuleContext = controller.ModuleContext.Configuration;
            Page = controller.DnnPage;
            Controller = viewContext.Controller as DnnController;
        }

        public ContextHelper(DnnController context)
        {
            Requires.NotNull("context", context);
            ModuleContext = context.ModuleContext.Configuration;
            Page = context.DnnPage;
            Controller = context;
        }

        public ContextHelper(DnnApiController context)
        {
            Requires.NotNull("context", context);
            ModuleContext = context.ActiveModule;
        }

        #region IBlogModuleHost
        public int ModuleId => ModuleContext.ModuleID;
        public int TabModuleId => ModuleContext.TabModuleID;
        public int TabId => ModuleContext.TabID;
        public int PortalId => ModuleContext.PortalID;
        public UserInfo UserInfo => UserController.Instance.GetCurrentUserInfo();
        public ModuleInfo ModuleConfiguration => ModuleContext;
        public TimeZoneInfo TimeZone =>
            DotNetNuke.Entities.Portals.PortalSettings.Current?.TimeZone ?? TimeZoneInfo.Local;
        #endregion

        private CoreViewSettings _viewSettings;
        public CoreViewSettings ViewSettings
        {
            get { return _viewSettings ?? (_viewSettings = CoreViewSettings.GetViewSettings(TabModuleId)); }
        }

        private CoreModuleSettings _moduleSettings;
        public CoreModuleSettings Settings
        {
            get
            {
                if (_moduleSettings == null)
                {
                    if (ViewSettings.BlogModuleId == -1)
                        _moduleSettings = CoreModuleSettings.GetSettings(ModuleConfiguration);
                    else
                        _moduleSettings = CoreModuleSettings.GetSettings(ViewSettings.BlogModuleId);
                }
                return _moduleSettings;
            }
        }

        private BlogContextInfo _blogContext;
        public BlogContextInfo BlogContext
        {
            get
            {
                if (_blogContext == null)
                {
                    var httpContext = HttpContext.Current;
                    _blogContext = BlogContextInfo.GetBlogContext(ref httpContext, this);
                }
                return _blogContext;
            }
        }

        public CoreSecurity Security => BlogContext.Security;

        public void RequirePermissionLevel(bool level)
        {
            if (!level)
            {
                ThrowAccessViolation();
            }
        }

        public void EnsureInitialized()
        {
            if (Page == null || HttpContext.Current.Items["BlogModuleBaseInitialized"] != null)
                return;

            JavaScript.RequestRegistration(CommonJs.jQuery);
            JavaScript.RequestRegistration(CommonJs.jQueryUI);
            var script = new StringBuilder();
            script.AppendLine("<script type=\"text/javascript\">");
            script.AppendLine("//<![CDATA[");
            script.AppendLine(string.Format("var appPath='{0}'", DotNetNuke.Common.Globals.ApplicationPath));
            script.AppendLine("//]]>");
            script.AppendLine("</script>");
            ClientAPI.RegisterClientScriptBlock(Page, "blogAppPath", script.ToString());
            AddBlogService();
            HttpContext.Current.Items["BlogModuleBaseInitialized"] = true;
        }

        public void AddBlogService()
        {
            if (Page == null || HttpContext.Current.Items["BlogServiceAdded"] != null)
                return;

            JavaScript.RequestRegistration(CommonJs.DnnPlugins);
            ServicesFramework.Instance.RequestAjaxScriptSupport();
            if (UserInfo.UserID > -1)
            {
                ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            }
            AddScript("dotnetnuke.blog.js");

            string scriptPath = HttpContext.Current.Server.MapPath(ModulePath + "/js/dotnetnuke.blog.pagescript.js");
            if (System.IO.File.Exists(scriptPath))
            {
                string scriptBlock = BlogGlobals.ReadFile(scriptPath);
                var tr = new BlogTokenReplace(BlogContext.BlogModuleId);
                tr.AddResources(BlogGlobals.SharedResourceFileName);
                scriptBlock = tr.ReplaceTokens(scriptBlock);
                scriptBlock = "<script type=\"text/javascript\">" + Environment.NewLine + "//<![CDATA[" + Environment.NewLine + scriptBlock + Environment.NewLine + "//]]>" + Environment.NewLine + "</script>";
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "BlogServiceScript", scriptBlock);
            }

            HttpContext.Current.Items["BlogServiceAdded"] = true;
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
        public void AddModuleCss()
        {
            ClientResourceManager.RegisterStyleSheet(Page, ModulePath + "/module.css", 9);
        }
        #endregion

        #region Js Files
        public void AddJqueryUi()
        {
            JavaScript.RequestRegistration(CommonJs.jQueryUI);
        }
        public void AddJquery()
        {
            JavaScript.RequestRegistration(CommonJs.jQuery);
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
            // Prefer legacy blog service script over React scaffold
            AddScript("dotnetnuke.blog.js");
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
            ClientAPI.RegisterClientVariable(Page, "sf_siteRoot", path, true);
            ClientAPI.RegisterClientVariable(Page, "sf_tabId", DotNetNuke.Entities.Portals.PortalSettings.Current.ActiveTab.TabID.ToString(System.Globalization.CultureInfo.InvariantCulture), true);

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
