using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Modules.Blog.Core.Entities.Terms;
using DotNetNuke.Modules.Blog.Core.Templating;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetNuke.Modules.Blog.Core.Common
{

  public class BlogModuleBase : PortalModuleBase
  {
    private BlogContextInfo _blogContext;
    public BlogContextInfo BlogContext
    {
      get
      {
        if (_blogContext is null)
        {
          var argcontext = Context;
          _blogContext = BlogContextInfo.GetBlogContext(ref argcontext, this);
        }
        return _blogContext;
      }
      set
      {
        _blogContext = value;
      }
    }

    private ModuleSettings _settings;
    public new ModuleSettings Settings
    {
      get
      {
        if (_settings is null)
        {
          if (ViewSettings.BlogModuleId == -1)
          {
            _settings = ModuleSettings.GetModuleSettings(ModuleConfiguration.ModuleID);
          }
          else
          {
            _settings = ModuleSettings.GetModuleSettings(ViewSettings.BlogModuleId);
          }
        }
        return _settings;
      }
      set
      {
        _settings = value;
      }
    }

    private Dictionary<string, TermInfo> _categories;
    public Dictionary<string, TermInfo> Categories
    {
      get
      {
        if (_categories is null)
        {
          _categories = TermsController.GetTermsByVocabulary(ModuleId, Settings.VocabularyId, BlogContext.Locale);
        }
        return _categories;
      }
      set
      {
        _categories = value;
      }
    }

    private ViewSettings _viewSettings;
    public ViewSettings ViewSettings
    {
      get
      {
        if (_viewSettings is null)
          _viewSettings = ViewSettings.GetViewSettings(TabModuleId);
        return _viewSettings;
      }
      set
      {
        _viewSettings = value;
      }
    }

    public new CDefault Page
    {
      get
      {
        return (CDefault)base.Page;
      }
    }

    private string _BlogModuleMapPath = "";
    public string BlogModuleMapPath
    {
      get
      {
        if (string.IsNullOrEmpty(_BlogModuleMapPath))
        {
          _BlogModuleMapPath = Server.MapPath("~/DesktopModules/Blog") + @"\";
        }
        return _BlogModuleMapPath;
      }
    }

    public BlogModuleBase()
    {
      Load += Page_Load;
    }

    private void Page_Load(object sender, EventArgs e)
    {

      if (Context.Items["BlogModuleBaseInitialized"] is null)
      {

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

        Context.Items["BlogModuleBaseInitialized"] = true;
      }

    }

    public void AddBlogService()
    {

      if (Context.Items["BlogServiceAdded"] is null)
      {

        JavaScript.RequestRegistration(CommonJs.DnnPlugins);
        ServiceLocator<IServicesFramework, ServicesFramework>.Instance.RequestAjaxScriptSupport();
        ServiceLocator<IServicesFramework, ServicesFramework>.Instance.RequestAjaxAntiForgerySupport();
        AddJavascriptFile("dotnetnuke.blog.js", 70);

        // Load initialization snippet
        string scriptBlock = Globals.ReadFile(DotNetNuke.Common.Globals.ApplicationMapPath + @"\DesktopModules\Blog\js\dotnetnuke.blog.pagescript.js");
        var tr = new BlogTokenReplace(BlogContext.BlogModuleId);
        tr.AddResources("~/DesktopModules/Blog/App_LocalResources/SharedResources.resx");
        scriptBlock = tr.ReplaceTokens(scriptBlock);
        scriptBlock = "<script type=\"text/javascript\">" + Environment.NewLine + "//<![CDATA[" + Environment.NewLine + scriptBlock + Environment.NewLine + "//]]>" + Environment.NewLine + "</script>";
        Page.ClientScript.RegisterClientScriptBlock(GetType(), "BlogServiceScript", scriptBlock);

        Context.Items["BlogServiceAdded"] = true;
      }

    }

    public void AddJavascriptFile(string jsFilename, int priority)
    {
      Page.AddJavascriptFile(Settings.Version, jsFilename, priority);
    }

    public void AddJavascriptFile(string jsFilename, string name, string version, int priority)
    {
      Page.AddJavascriptFile(Settings.Version, jsFilename, name, version, priority);
    }

    public void AddCssFile(string cssFilename)
    {
      Page.AddCssFile(Settings.Version, cssFilename);
    }

    public void AddCssFile(string cssFilename, string name, string version)
    {
      Page.AddCssFile(Settings.Version, cssFilename, name, version);
    }

    public string LocalizeJSString(string resourceKey)
    {
      return ClientAPI.GetSafeJSString(LocalizeString(resourceKey));
    }

    public string LocalizeJSString(string resourceKey, string resourceFile)
    {
      return ClientAPI.GetSafeJSString(Localization.GetString(resourceKey, resourceFile));
    }

  }

}