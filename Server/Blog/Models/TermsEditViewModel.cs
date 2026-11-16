using System.Collections.Generic;
using DotNetNuke.Modules.Blog.Core.Entities.Terms;

namespace DotNetNuke.Modules.Blog.Models
{
    public class TermsEditViewModel
    {
        public int VocabularyId { get; set; } = 1;
        public IList<TermRowViewModel> Terms { get; set; } = new List<TermRowViewModel>();
    }

    public class TermRowViewModel
    {
        public int TermId { get; set; }
        public string DefaultName { get; set; }
        public Dictionary<string, string> Localizations { get; set; } = new Dictionary<string, string>();
    }
}
