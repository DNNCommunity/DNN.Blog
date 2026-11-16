using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Modules.Blog.Core.Common;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using DotNetNuke.Modules.Blog.Core.Entities.Posts;
namespace DotNetNuke.Modules.Blog.Common
{
    public static class SettingsListHelper
    {
        public static List<SelectListItem> GetBlogModuleItems(int portalId, int currentModuleId)
        {
            var tabController = new TabController();
            var list = new List<SelectListItem>
            {
                new SelectListItem { Text = "No parent", Value = "-1" }
            };

            foreach (ModuleInfo blogModule in DotNetNuke.Entities.Modules.ModuleController.Instance.GetModulesByDefinition(portalId, "DNNBlog.Blog"))
            {
                var blogPage = tabController.GetTab(blogModule.TabID, portalId, false);
                if (blogPage != null)
                    blogModule.ModuleTitle = string.Concat(blogPage.TabName, " - ", blogModule.ModuleTitle);

                var targetViewSetting = ViewSettings.GetViewSettings(blogModule.TabModuleID);
                if (targetViewSetting.BlogModuleId == -1 && blogModule.ModuleID != currentModuleId)
                {
                    list.Add(new SelectListItem
                    {
                        Text = blogModule.ModuleTitle,
                        Value = blogModule.ModuleID.ToString()
                    });
                }
            }

            return list;
        }

        public static List<SelectListItem> GetBlogItems(int blogModuleId, int userId, string locale)
        {
            var list = new List<SelectListItem>
            {
                new SelectListItem { Text = "All", Value = "-1" }
            };

            if (blogModuleId <= 0)
                return list;

            var blogs = BlogsController.GetBlogsByModule(blogModuleId, userId, locale).Values
                .Where(b => b.Published)
                .OrderBy(b => b.Title);

            foreach (var b in blogs)
            {
                list.Add(new SelectListItem
                {
                    Text = b.LocalizedTitle,
                    Value = b.BlogID.ToString()
                });
            }

            return list;
        }

        public static List<SelectListItem> GetAuthorItems(int blogModuleId)
        {
            var list = new List<SelectListItem>
            {
                new SelectListItem { Text = "All", Value = "-1" }
            };

            if (blogModuleId <= 0)
                return list;

            foreach (var a in PostsController.GetAuthors(blogModuleId, -1).OrderBy(t => t.DisplayName))
            {
                list.Add(new SelectListItem
                {
                    Text = a.DisplayName,
                    Value = a.UserID.ToString()
                });
            }

            return list;
        }

        public static List<SelectListItem> GetTemplateItems(int blogModuleId, PortalSettings portalSettings, HttpContextBase httpContext)
        {
            var list = new List<SelectListItem>
            {
                new SelectListItem { Text = "Default [System]", Value = "[G]_default" }
            };

            string templatesMapPath = httpContext.Server.MapPath(DotNetNuke.Common.Globals.ResolveUrl(DotNetNuke.Modules.Blog.Core.Common.Globals.glbTemplatesPath));
            if (Directory.Exists(templatesMapPath))
            {
                foreach (var d in new DirectoryInfo(templatesMapPath).GetDirectories())
                {
                    if (d.Name != "_default")
                        list.Add(new SelectListItem { Text = d.Name + " [System]", Value = "[G]" + d.Name });
                }
            }

            int effectiveModuleId = blogModuleId > -1 ? blogModuleId : portalSettings.ActiveTab.Modules.Cast<ModuleInfo>().FirstOrDefault(m => m.DesktopModuleID > 0)?.ModuleID ?? -1;
            if (effectiveModuleId > -1)
            {
                var moduleSettings = ModuleSettings.GetSettings(effectiveModuleId);
                if (Directory.Exists(moduleSettings.PortalTemplatesMapPath))
                {
                    foreach (var d in new DirectoryInfo(moduleSettings.PortalTemplatesMapPath).GetDirectories())
                        list.Add(new SelectListItem { Text = d.Name + " [Local]", Value = "[P]" + d.Name });
                }
            }

            return list;
        }

        public static int ResolveBlogModuleId(ViewSettings viewSettings, int moduleId)
        {
            return viewSettings.BlogModuleId > -1 ? viewSettings.BlogModuleId : moduleId;
        }
    }
}
