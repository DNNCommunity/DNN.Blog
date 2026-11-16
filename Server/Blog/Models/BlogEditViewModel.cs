namespace DotNetNuke.Modules.Blog.Models
{
    public class BlogEditViewModel
    {
        public int BlogId { get; set; } = -1;
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public byte ContentFormat { get; set; } = 0;
        public bool Published { get; set; } = true;
        public bool Syndicated { get; set; }
        public string Copyright { get; set; } = "";
        public bool EnablePingBackReceive { get; set; } = true;
        public bool EnablePingBackSend { get; set; } = true;
        public bool AutoApprovePingBack { get; set; }
        public bool EnableTrackBackReceive { get; set; } = true;
        public bool EnableTrackBackSend { get; set; } = true;
        public bool AutoApproveTrackBack { get; set; }
        public bool IsNew { get; set; } = true;
    }
}
