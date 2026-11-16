using System.Collections.Generic;
using DotNetNuke.Modules.Blog.Core.Templating;

namespace DotNetNuke.Modules.Blog.Models
{
    public class TemplateSettingsViewModel
    {
        public string TemplateName { get; set; }
        public IList<TemplateSettingRow> Settings { get; set; } = new List<TemplateSettingRow>();
        public bool HasSettings { get; set; }
    }

    public class TemplateSettingRow
    {
        public TemplateSetting Definition { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
        public string Help { get; set; }
    }
}
