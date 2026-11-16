namespace DotNetNuke.Modules.Blog.Models
{
    public class ImportViewModel
    {
        public int BlogId { get; set; } = -1;
        public string BlogTitle { get; set; } = "";
        public string Message { get; set; } = "";
        public bool MessageIsError { get; set; }
        public bool ImportStep { get; set; }
        public bool ImportComplete { get; set; }
        public string OriginalTitle { get; set; } = "";
        public int PostCount { get; set; }
        public int CategoryCount { get; set; }
        public int AttachmentCount { get; set; }
    }
}
