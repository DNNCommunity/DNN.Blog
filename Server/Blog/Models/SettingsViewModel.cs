using System.Collections.Generic;
using System.Web.Mvc;
using DotNetNuke.Modules.Blog.Core.Common;

namespace DotNetNuke.Modules.Blog.Models
{
    public class SettingsViewModel
    {
        public ViewSettings Settings { get; set; }

        public IEnumerable<SelectListItem> BlogModules { get; set; }
        public IEnumerable<SelectListItem> Blogs { get; set; }
        public IEnumerable<SelectListItem> Authors { get; set; }
        public IEnumerable<SelectListItem> Templates { get; set; }

        public int BlogModuleId { get; set; } = -1;
        public int BlogId { get; set; } = -1;
        public string Categories { get; set; } = "";
        public int AuthorId { get; set; } = -1;
        public string Template { get; set; } = "[G]_default";

        public bool ShowAllLocales { get; set; } = true;
        public bool ModifyPageDetails { get; set; }
        public bool AddCanonicalTag { get; set; }
        public bool ShowManagementPanel { get; set; }
        public bool ShowManagementPanelViewMode { get; set; } = true;
        public bool HideUnpublishedBlogsViewMode { get; set; }
        public bool HideUnpublishedBlogsEditMode { get; set; }
        public bool AllowComments { get; set; } = true;
    }
}
