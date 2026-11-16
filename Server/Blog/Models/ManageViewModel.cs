using System.Collections.Generic;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using DotNetNuke.Modules.Blog.Core.Entities.Posts;

namespace DotNetNuke.Modules.Blog.Models
{
    public class ManageViewModel
    {
        public IList<BlogInfo> Blogs { get; set; } = new List<BlogInfo>();
        public IList<PostInfo> Posts { get; set; } = new List<PostInfo>();

        public bool ShowBlogsSection { get; set; }
        public bool ShowPostsSection { get; set; }
        public bool CanAddBlog { get; set; }
        public int PageIndex { get; set; }
        public int TotalPosts { get; set; }
        public int PageSize { get; set; } = 25;
    }
}
