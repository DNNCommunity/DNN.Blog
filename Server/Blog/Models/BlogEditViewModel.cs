using System.Collections.Generic;

namespace DotNetNuke.Modules.Blog.Models
{
    public class BlogEditViewModel
    {
        public int BlogId { get; set; } = -1;
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public byte DefaultContentFormat { get; set; } = 0;
        public string Locale { get; set; } = "";
        public bool FullLocalization { get; set; }
        public bool ShowLocaleFields { get; set; }
        public List<PostLocaleOption> LocaleOptions { get; set; } = new List<PostLocaleOption>();
        public string ImageKey { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public bool RemoveImage { get; set; }
        public bool Published { get; set; } = true;
        public bool Syndicated { get; set; }
        public string SyndicationEmail { get; set; } = "";
        public string Copyright { get; set; } = "";
        public bool IncludeImagesInFeed { get; set; } = true;
        public bool IncludeAuthorInFeed { get; set; }
        public bool EnablePingBackReceive { get; set; } = true;
        public bool EnablePingBackSend { get; set; } = true;
        public bool AutoApprovePingBack { get; set; }
        public bool EnableTrackBackReceive { get; set; } = true;
        public bool EnableTrackBackSend { get; set; } = true;
        public bool AutoApproveTrackBack { get; set; }
        public bool MustApproveGhostPosts { get; set; }
        public bool PublishAsOwner { get; set; }
        public bool IsNew { get; set; } = true;

        public List<BlogPermissionColumnModel> PermissionColumns { get; set; } = new List<BlogPermissionColumnModel>();
        public List<BlogPermissionRoleRowModel> RolePermissions { get; set; } = new List<BlogPermissionRoleRowModel>();
        public List<BlogPermissionUserRowModel> UserPermissions { get; set; } = new List<BlogPermissionUserRowModel>();
        public string AddPermissionUsername { get; set; } = "";
        public string PermissionAddError { get; set; } = "";
    }
}
