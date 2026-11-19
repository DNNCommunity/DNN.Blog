using System.Collections.Generic;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using DotNetNuke.Modules.Blog.Core.Entities.Posts;

namespace DotNetNuke.Modules.Blog.Models
{
    public class ManageViewModel
    {
        public const string SectionPosts = "posts";
        public const string SectionComments = "comments";
        public const string SectionBlogs = "blogs";
        public const string SectionApiTokens = "apitokens";

        public IList<BlogInfo> Blogs { get; set; } = new List<BlogInfo>();
        public IList<PostInfo> Posts { get; set; } = new List<PostInfo>();

        public bool ShowBlogsSection { get; set; }
        public bool ShowPostsSection { get; set; }
        public bool ShowCommentsSection { get; set; }
        public bool ShowApiTokensSection { get; set; }
        public bool CanAddBlog { get; set; }
        /// <summary>Active manage section. Query param is "section" (not "tab" — reserved by DNN).</summary>
        public string ActiveSection { get; set; } = SectionPosts;
        public int PageIndex { get; set; }
        public int TotalPosts { get; set; }
        public int PageSize { get; set; } = 25;
    }
}
