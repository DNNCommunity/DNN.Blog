using System.Collections.Generic;
using System.Web.Mvc;
using DotNetNuke.Modules.Blog.Core.Common;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.Blog.Models
{
    public class AdminViewModel
    {
        public ModuleSettings Settings { get; set; }

        public IEnumerable<SelectListItem> Vocabularies { get; set; }
        public IEnumerable<SelectListItem> FacebookProfileProperties { get; set; }

        public bool ShowMultilingualLinks { get; set; }
        public bool HasVocabulary { get; set; }
        public string DefaultLocale { get; set; }
        public IList<Locale> Locales { get; set; } = new List<Locale>();
    }
}
