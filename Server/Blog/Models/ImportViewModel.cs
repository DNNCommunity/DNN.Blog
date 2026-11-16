namespace DotNetNuke.Modules.Blog.Models
{
    public class ImportViewModel
    {
        public int BlogId { get; set; } = -1;
        public string BlogTitle { get; set; } = "";
        public string Message { get; set; } = "";
        public string Analysis { get; set; } = "";
        public bool CanImportCategories { get; set; }
        public bool ImportCategories { get; set; }
        public bool ImportStep { get; set; }
    }
}
