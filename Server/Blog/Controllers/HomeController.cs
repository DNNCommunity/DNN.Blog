using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Framework;
using DotNetNuke.Modules.Blog.Core.Common;
using BlogGlobals = DotNetNuke.Modules.Blog.Core.Common.Globals;
using DotNetNuke.Modules.Blog.Core.Entities.Posts;
using DotNetNuke.Modules.Blog.Core.Integration;
using DotNetNuke.Modules.Blog.Models;
using DotNetNuke.Modules.Blog.Services;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.Blog.Controllers
{
    public class HomeController : BlogMvcController
    {
        [HttpGet]
        public ActionResult Index()
        {
            BlogModuleContext.EnsureInitialized();
            BlogModuleContext.AddModuleCss();

            BlogModuleController.CheckupOnImportedFiles(ModuleContext.ModuleId);

            var ctx = BlogModuleContext.BlogContext;
            var settings = BlogModuleContext.Settings;
            var viewSettings = BlogModuleContext.ViewSettings;

            // WLW style detection redirect
            if (!string.IsNullOrEmpty(settings.StyleDetectionUrl) && ctx.WLWRequest)
            {
                var url = settings.StyleDetectionUrl;
                settings.StyleDetectionUrl = "";
                settings.SaveSettings();
                return Redirect(url);
            }

            // Legacy EntryId permanent redirect
            if (ctx.ContentItemId == -1 && ctx.LegacyEntryId > -1)
            {
                var p = PostsController.GetPostByLegacyEntryId(ctx.LegacyEntryId, PortalSettings.PortalId, ctx.Locale);
                if (p != null)
                    return RedirectPermanent(p.PermaLink(PortalSettings));
            }

            ApplyPageChrome(ctx, settings, viewSettings);

            var ds = new BlogTemplateDataSource(
                ModuleContext.Configuration,
                ctx,
                settings,
                viewSettings,
                User.UserID,
                k => Localization.GetString(k, LocalResourceFile));

            // MVC does not attach ViewTemplate to the page, so register template CSS/JS explicitly.
            ds.RegisterClientAssets(BlogModuleContext.Page);

            var showComments = viewSettings.AllowComments
                && ctx.Security.CanViewComments
                && ctx.Post != null;

            var model = new HomeViewModel
            {
                TemplateHtml = ds.Render(),
                ShowComments = showComments,
                ShowManagementPanel = ShouldShowManagementPanel(viewSettings),
                TrackBackBlurb = BuildTrackBackBlurb(ctx)
            };

            if (showComments)
            {
                BlogModuleContext.RegisterAjaxScript();
                BlogModuleContext.AddScript("comments.js");
            }

            if (ctx.ContentItemId > -1 && !IsPostBack())
            {
                int timeout = settings.IncrementViewCount * 1000;
                string script = $"(function($){{$(function(){{setTimeout(function(){{if(window.blogService)blogService.viewPost({ctx.BlogId},{ctx.ContentItemId});}},{timeout});}});}})(jQuery);";
                BlogModuleContext.Page.ClientScript.RegisterClientScriptBlock(GetType(), "PostViewScript", script, true);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewPost(int postId)
        {
            // Temporary MVC stand-in for API view count until Web API is ported
            try
            {
                Core.Data.DataProvider.Instance().AddPostView(postId);
            }
            catch { /* best-effort */ }
            return new HttpStatusCodeResult(204);
        }

        private bool ShouldShowManagementPanel(ViewSettings viewSettings)
        {
            bool show = viewSettings.BlogModuleId == -1 || viewSettings.ShowManagementPanel;
            if (PortalSettings.UserMode == PortalSettings.Mode.View && !viewSettings.ShowManagementPanelViewMode)
                show = false;
            return show;
        }

        private bool IsPostBack()
        {
            return Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase);
        }

        private void ApplyPageChrome(BlogContextInfo ctx, ModuleSettings settings, ViewSettings viewSettings)
        {
            AddWLWManifestLink(viewSettings);

            if (settings.ModifyPageDetails || viewSettings.ModifyPageDetails)
            {
                var dnnPage = BlogModuleContext.Page as PageBase;
                if (dnnPage != null)
                {
                    string pageDescription = null;
                    string pageKeywords = null;
                    if (ctx.Post != null)
                    {
                        dnnPage.Title = ctx.Post.LocalizedTitle;
                        pageDescription = HtmlUtils.Clean(ctx.Post.LocalizedSummary, false);
                        pageKeywords = string.Join(",", ctx.Post.Terms.ToStringArray());
                    }
                    else if (ctx.Blog != null)
                    {
                        dnnPage.Title = ctx.Blog.LocalizedTitle;
                        pageDescription = HtmlUtils.Clean(ctx.Blog.LocalizedDescription, false);
                    }
                    else if (ctx.Author != null)
                    {
                        dnnPage.Title = ctx.Author.DisplayName;
                        pageDescription = HtmlUtils.Clean(ctx.Author.Profile.Biography, false);
                    }
                    else if (ctx.Term != null)
                    {
                        dnnPage.Title = ctx.Term.LocalizedName;
                        pageDescription = HtmlUtils.Clean(ctx.Term.LocalizedDescription, false);
                    }
                    if (!string.IsNullOrEmpty(pageDescription))
                        BlogModuleContext.Page.Header.Controls.Add(new LiteralControl($"<meta name=\"description\" content=\"{BlogGlobals.CleanStringForXmlAttribute(pageDescription)}\" />"));
                    if (!string.IsNullOrEmpty(pageKeywords))
                        BlogModuleContext.Page.Header.Controls.Add(new LiteralControl($"<meta name=\"keywords\" content=\"{BlogGlobals.CleanStringForXmlAttribute(pageKeywords)}\" />"));
                }
            }

            if (ctx.Post != null && ctx.Blog != null)
            {
                AddOpenGraphMetaTags(ctx, settings);
                if (viewSettings.AddCanonicalTag)
                    AddCanonicalTag(ctx);
                if (ctx.Blog.EnablePingBackReceive)
                    AddPingBackLink(ctx);
            }
            else if (viewSettings.AddCanonicalTag)
            {
                AddCanonicalTag(ctx);
            }
        }

        private void AddCanonicalTag(BlogContextInfo ctx)
        {
            string href;
            if (ctx.Post != null)
                href = ctx.Post.PermaLink(PortalSettings);
            else if (ctx.Blog != null)
                href = PortalSettings.ActiveTab.FullUrl;
            else if (ctx.Author != null)
                href = ctx.ModuleUrls.GetUrl(false, false, false, true, false);
            else if (ctx.Term != null)
                href = ctx.Term.PermaLink(PortalSettings);
            else
                href = PortalSettings.ActiveTab.FullUrl;

            BlogModuleContext.Page.Header.Controls.Add(new LiteralControl($"<link rel=\"canonical\" href=\"{href}\" />"));
        }

        private void AddOpenGraphMetaTags(BlogContextInfo ctx, ModuleSettings settings)
        {
            string URL = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Host;
            var h = BlogModuleContext.Page.Header.Controls;
            h.Add(new LiteralControl($"<meta property=\"og:url\" content=\"{ctx.Post.PermaLink(PortalSettings)}\" />"));
            h.Add(new LiteralControl($"<meta property=\"og:title\" content=\"{BlogGlobals.CleanStringForXmlAttribute(ctx.Post.LocalizedTitle)}\" />"));
            string description = BlogGlobals.CleanStringForXmlAttribute(HtmlUtils.Clean(ctx.Post.LocalizedSummary, false));
            if (!string.IsNullOrEmpty(description))
                h.Add(new LiteralControl($"<meta property=\"og:description\" content=\"{description}\" />"));
            if (!string.IsNullOrEmpty(ctx.Post.Image))
            {
                string strPath = string.Format("{0}?TabId={1}&ModuleId={2}&Blog={3}&Post={4}&w=1200&h=630&c=1&key={5}",
                    BlogGlobals.glbImageHandlerPath, ModuleContext.TabId, settings.ModuleId, ctx.BlogId, ctx.ContentItemId, ctx.Post.Image);
                h.Add(new LiteralControl($"<meta property=\"og:image\" content=\"{URL + Url.Content(strPath)}\" />"));
            }
            h.Add(new LiteralControl($"<meta property=\"og:site_name\" content=\"{BlogGlobals.CleanStringForXmlAttribute(PortalSettings.PortalName)}\" />"));
            h.Add(new LiteralControl("<meta property=\"og:type\" content=\"article\" />"));
        }

        private void AddWLWManifestLink(ViewSettings viewSettings)
        {
            if (HttpContext.Items["WLWManifestLinkAdded"] != null) return;
            var link = new HtmlLink();
            link.Attributes.Add("rel", "wlwmanifest");
            link.Attributes.Add("type", "application/wlwmanifest+xml");
            int mid = viewSettings.BlogModuleId == -1 ? ModuleContext.ModuleId : viewSettings.BlogModuleId;
            link.Attributes.Add("href", Url.Content(BlogGlobals.ManifestFilePath(ModuleContext.TabId, mid)));
            BlogModuleContext.Page.Header.Controls.Add(link);
            HttpContext.Items["WLWManifestLinkAdded"] = true;
        }

        private void AddPingBackLink(BlogContextInfo ctx)
        {
            if (HttpContext.Items["PingBackLinkAdded"] != null) return;
            // API not yet ported — placeholder path for when Comments API lands
            string pingbackUrl = Url.Content(BlogGlobals.glbServicesPath + "Comments/Pingback");
            pingbackUrl += $"?tabId={ModuleContext.TabId}&moduleId={ctx.BlogModuleId}&blogId={ctx.BlogId}&postId={ctx.ContentItemId}";
            var link = new HtmlGenericControl("link");
            link.Attributes.Add("rel", "pingback");
            link.Attributes.Add("href", pingbackUrl);
            BlogModuleContext.Page.Header.Controls.Add(link);
            HttpContext.Items["PingBackLinkAdded"] = true;
        }

        private string BuildTrackBackBlurb(BlogContextInfo ctx)
        {
            if (ctx.Post == null || ctx.Blog == null || !ctx.Blog.EnableTrackBackReceive)
                return null;
            if (HttpContext.Items["TrackBackBlurbAdded"] != null)
                return null;

            string trackbackUrl = Url.Content(BlogGlobals.glbServicesPath + "Comments/Trackback");
            trackbackUrl += $"?tabId={ModuleContext.TabId}&moduleId={ctx.BlogModuleId}&blogId={ctx.BlogId}&postId={ctx.ContentItemId}";
            string postUrl = ctx.Post.PermaLink(PortalSettings);
            var sb = new StringBuilder();
            sb.AppendLine("<!--");
            sb.AppendLine(" <rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\"");
            sb.AppendLine("  xmlns:dc=\"http://purl.org/dc/elements/1.1/\"");
            sb.AppendLine("  xmlns:trackback=\"http://madskills.com/public/xml/rss/module/trackback/\">");
            sb.AppendFormat("  <rdf:Description rdf:about=\"{0}\"{1}", postUrl, Environment.NewLine);
            sb.AppendFormat("  dc:identifier=\"{0}\" dc:Title=\"{1}\"{2}", postUrl, ctx.Post.LocalizedTitle, Environment.NewLine);
            sb.AppendFormat("  trackback:ping=\"{0}\" />{1}", trackbackUrl, Environment.NewLine);
            sb.AppendLine(" </rdf:RDF>");
            sb.AppendLine("-->");
            HttpContext.Items["TrackBackBlurbAdded"] = true;
            return sb.ToString();
        }
    }
}
