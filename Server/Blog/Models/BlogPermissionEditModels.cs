using System.Collections.Generic;

namespace DotNetNuke.Modules.Blog.Models
{
    public class BlogPermissionColumnModel
    {
        public int PermissionId { get; set; }
        public string PermissionKey { get; set; } = "";
        public string Header { get; set; } = "";
    }

    public class BlogPermissionCellModel
    {
        public int PermissionId { get; set; }
        public string PermissionKey { get; set; } = "";
        public bool AllowAccess { get; set; }
        public bool Enabled { get; set; } = true;
    }

    public class BlogPermissionRoleRowModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = "";
        public List<BlogPermissionCellModel> Cells { get; set; } = new List<BlogPermissionCellModel>();
    }

    public class BlogPermissionUserRowModel
    {
        public int UserId { get; set; }
        public string DisplayName { get; set; } = "";
        public string Username { get; set; } = "";
        public List<BlogPermissionCellModel> Cells { get; set; } = new List<BlogPermissionCellModel>();
    }
}
