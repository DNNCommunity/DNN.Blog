using System.Collections.Generic;
using System.Web.Mvc;

namespace DotNetNuke.Modules.Blog.Models
{
    public class PostEditViewModel
    {
        public int BlogId { get; set; } = -1;
        public int PostId { get; set; } = -1;
        public string BlogTitle { get; set; } = "";
        public bool IsNew { get; set; } = true;

        public string Title { get; set; } = "";
        [AllowHtml]
        public string Summary { get; set; } = "";
        [AllowHtml]
        public string Content { get; set; } = "";
        public Dictionary<string, string> Titles { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Summaries { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Contents { get; set; } = new Dictionary<string, string>();

        public bool Published { get; set; } = true;
        public bool PublishCanChange { get; set; } = true;
        public bool PublishNow { get; set; } = true;
        public string PublishDate { get; set; } = "";
        public string TimezoneDisplay { get; set; } = "";

        public bool AllowComments { get; set; } = true;
        public bool DisplayCopyright { get; set; }
        public string Copyright { get; set; } = "";

        public string Locale { get; set; } = "";
        public bool ShowLocale { get; set; }
        public bool UseFullLocalization { get; set; }
        public string DefaultLocale { get; set; } = "";
        public List<PostLocaleOption> Locales { get; set; } = new List<PostLocaleOption>();
        public List<PostLocaleOption> OtherLocales { get; set; } = new List<PostLocaleOption>();

        public string Tags { get; set; } = "";
        public string ImageKey { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public bool RemoveImage { get; set; }

        public bool ShowCategories { get; set; }
        public List<int> SelectedCategoryIds { get; set; } = new List<int>();
        public List<PostCategoryNode> Categories { get; set; } = new List<PostCategoryNode>();

        public bool SummaryIsHtml { get; set; }
        public bool ShowSummaryPrecedingWarning { get; set; }

        /// <summary>html or markdown — from the blog's ContentFormat.</summary>
        public string EditorMode { get; set; } = "html";

        public List<PostLocaleOption> LocaleDropdownOptions { get; set; } = new List<PostLocaleOption>();
        public string ValidationMessage { get; set; } = "";
        public string CancelUrl { get; set; } = "";
        public string ReturnUrl { get; set; } = "";
    }

    public class PostLocaleOption
    {
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string FlagUrl { get; set; } = "";
    }

    public class PostCategoryNode
    {
        public int TermId { get; set; }
        public string Name { get; set; } = "";
        public int Depth { get; set; }
        public bool Selected { get; set; }
        public List<PostCategoryNode> Children { get; set; } = new List<PostCategoryNode>();
    }
}
