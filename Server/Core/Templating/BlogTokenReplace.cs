// 
// DNN Connect - http://dnn-connect.org
// Copyright (c) 2015
// by DNN Connect
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
// 

using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Modules.Blog.Core.Common;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using DotNetNuke.Modules.Blog.Core.Entities.Comments;
using DotNetNuke.Modules.Blog.Core.Entities.Posts;
using DotNetNuke.Modules.Blog.Core.Entities.Terms;
using DotNetNuke.Services.Tokens;

namespace DotNetNuke.Modules.Blog.Core.Templating
{
    public class BlogTokenReplace : GenericTokenReplace
    {

        public BlogTokenReplace(int moduleId) : base(Scope.DefaultSettings)
        {
            var actModule = new DotNetNuke.Entities.Modules.ModuleController().GetModule(moduleId);
            ModuleInfo = actModule;
            UseObjectLessExpression = false;

        }

        public BlogTokenReplace(DotNetNuke.Entities.Modules.ModuleInfo actModule, Security.ContextSecurity security, BlogInfo blog, PostInfo post, ModuleSettings settings, ViewSettings viewSettings) : base(Scope.DefaultSettings)
        {
            ModuleInfo = actModule;
            UseObjectLessExpression = false;
            PropertySource["security"] = security;
            PropertySource["settings"] = settings;
            PropertySource["viewsettings"] = viewSettings;
            if (blog != null)
            {
                PropertySource["blog"] = blog;
                PropertySource["owner"] = new LazyLoadingUser(PortalSettings.PortalId, blog.OwnerUserId, blog.Username);
            }
            if (post != null)
            {
                PropertySource["post"] = post;
            }

        }

        public BlogTokenReplace(DotNetNuke.Entities.Modules.ModuleInfo actModule, Security.ContextSecurity security, BlogInfo blog, PostInfo Post, ModuleSettings settings, ViewSettings viewSettings, CommentInfo comment) : base(Scope.DefaultSettings)
        {
            PrimaryObject = comment;
            ModuleInfo = actModule;
            UseObjectLessExpression = false;
            PropertySource["security"] = security;
            PropertySource["settings"] = settings;
            PropertySource["viewsettings"] = viewSettings;
            if (Post != null)
            {
                PropertySource["post"] = Post;
                PropertySource["author"] = new LazyLoadingUser(PortalSettings.PortalId, Post.CreatedByUserID, Post.Username);
                PropertySource["blog"] = Post.Blog;
                PropertySource["owner"] = new LazyLoadingUser(PortalSettings.PortalId, Post.Blog.OwnerUserId, Post.Blog.Username);
            }
            else if (blog != null)
            {
                PropertySource["blog"] = blog;
                PropertySource["owner"] = new LazyLoadingUser(PortalSettings.PortalId, blog.OwnerUserId, blog.Username);
            }
            PropertySource["comment"] = comment;
            PropertySource["commenter"] = new LazyLoadingUser(PortalSettings.PortalId, comment.CreatedByUserID, comment.Username);

        }

        public BlogTokenReplace(ModuleInfo module, BlogContextInfo context, ModuleSettings settings, ViewSettings viewSettings) : base(Scope.DefaultSettings)
        {
            ModuleInfo = module;
            UseObjectLessExpression = false;
            PropertySource["query"] = context;
            PropertySource["security"] = context.Security;
            PropertySource["urls"] = context.ModuleUrls;
            PropertySource["settings"] = settings;
            PropertySource["viewsettings"] = viewSettings;
            if (context.Blog != null)
            {
                PropertySource["blog"] = context.Blog;
                PropertySource["owner"] = new LazyLoadingUser(PortalSettings.PortalId, context.Blog.OwnerUserId, context.Blog.Username);
            }
            if (context.Post != null)
            {
                PropertySource["post"] = context.Post;
                PropertySource["author"] = new LazyLoadingUser(PortalSettings.PortalId, context.Post.CreatedByUserID, context.Post.Username);
            }
            if (context.Term != null)
            {
                PropertySource["selectedterm"] = context.Term;
            }
            if (context.Author != null)
            {
                PropertySource["author"] = context.Author;
            }

        }

        public BlogTokenReplace(ModuleInfo module, BlogContextInfo context, ModuleSettings settings, ViewSettings viewSettings, BlogInfo blog) : base(Scope.DefaultSettings)
        {
            PrimaryObject = blog;
            ModuleInfo = module;
            UseObjectLessExpression = false;
            PropertySource["query"] = context;
            PropertySource["security"] = context.Security;
            PropertySource["urls"] = context.ModuleUrls;
            PropertySource["settings"] = settings;
            PropertySource["viewsettings"] = viewSettings;
            PropertySource["blog"] = blog;
            PropertySource["owner"] = new LazyLoadingUser(PortalSettings.PortalId, blog.OwnerUserId, blog.Username);
            if (context.Post != null)
            {
                PropertySource["post"] = context.Post;
                PropertySource["author"] = new LazyLoadingUser(PortalSettings.PortalId, context.Post.CreatedByUserID, context.Post.Username);
            }
            else if (context.Author != null)
            {
                PropertySource["author"] = context.Author;
            }
            if (context.Term != null)
            {
                PropertySource["selectedterm"] = context.Term;
            }

        }

        public BlogTokenReplace(ModuleInfo module, BlogContextInfo context, ModuleSettings settings, ViewSettings viewSettings, BlogCalendarInfo objBlogCalendar) : base(Scope.DefaultSettings)
        {
            PrimaryObject = objBlogCalendar;
            ModuleInfo = module;
            UseObjectLessExpression = false;
            PropertySource["query"] = context;
            PropertySource["security"] = context.Security;
            PropertySource["urls"] = context.ModuleUrls;
            PropertySource["settings"] = settings;
            PropertySource["viewsettings"] = viewSettings;
            if (context.Blog != null)
            {
                PropertySource["blog"] = context.Blog;
            }
            PropertySource["calendar"] = objBlogCalendar;
            if (context.Post != null)
            {
                PropertySource["post"] = context.Post;
                PropertySource["author"] = new LazyLoadingUser(PortalSettings.PortalId, context.Post.CreatedByUserID, context.Post.Username);
            }
            else if (context.Author != null)
            {
                PropertySource["author"] = context.Author;
            }
            if (context.Term != null)
            {
                PropertySource["selectedterm"] = context.Term;
            }

        }

        public BlogTokenReplace(ModuleInfo module, BlogContextInfo context, ModuleSettings settings, ViewSettings viewSettings, LazyLoadingUser objAuthor) : base(Scope.DefaultSettings)
        {
            PrimaryObject = objAuthor;
            ModuleInfo = module;
            UseObjectLessExpression = false;
            PropertySource["query"] = context;
            PropertySource["security"] = context.Security;
            PropertySource["urls"] = context.ModuleUrls;
            PropertySource["settings"] = settings;
            PropertySource["viewsettings"] = viewSettings;
            if (context.Blog != null)
            {
                PropertySource["blog"] = context.Blog;
            }
            PropertySource["author"] = objAuthor;
            if (context.Post != null)
            {
                PropertySource["post"] = context.Post;
            }
            if (context.Term != null)
            {
                PropertySource["selectedterm"] = context.Term;
            }

        }

        public BlogTokenReplace(ModuleInfo module, BlogContextInfo context, ModuleSettings settings, ViewSettings viewSettings, PostInfo Post) : base(Scope.DefaultSettings)
        {
            PrimaryObject = Post;
            ModuleInfo = module;
            UseObjectLessExpression = false;
            PropertySource["query"] = context;
            PropertySource["security"] = context.Security;
            PropertySource["urls"] = context.ModuleUrls;
            PropertySource["settings"] = settings;
            PropertySource["viewsettings"] = viewSettings;
            PropertySource["post"] = Post;
            PropertySource["author"] = new LazyLoadingUser(PortalSettings.PortalId, Post.CreatedByUserID, Post.Username);
            PropertySource["blog"] = Post.Blog;
            PropertySource["owner"] = new LazyLoadingUser(PortalSettings.PortalId, Post.Blog.OwnerUserId, Post.Blog.Username);
            if (context.Term != null)
            {
                PropertySource["selectedterm"] = context.Term;
            }

        }

        public BlogTokenReplace(ModuleInfo module, BlogContextInfo context, ModuleSettings settings, ViewSettings viewSettings, PostInfo Post, TermInfo term) : base(Scope.DefaultSettings)
        {
            PrimaryObject = term;
            ModuleInfo = module;
            UseObjectLessExpression = false;
            PropertySource["query"] = context;
            PropertySource["security"] = context.Security;
            PropertySource["urls"] = context.ModuleUrls;
            PropertySource["settings"] = settings;
            PropertySource["viewsettings"] = viewSettings;
            if (Post != null)
            {
                PropertySource["post"] = Post;
                PropertySource["author"] = new LazyLoadingUser(PortalSettings.PortalId, Post.CreatedByUserID, Post.Username);
                PropertySource["blog"] = Post.Blog;
                PropertySource["owner"] = new LazyLoadingUser(PortalSettings.PortalId, Post.Blog.OwnerUserId, Post.Blog.Username);
            }
            else if (context.Blog != null)
            {
                PropertySource["blog"] = context.Blog;
                PropertySource["owner"] = new LazyLoadingUser(PortalSettings.PortalId, context.Blog.OwnerUserId, context.Blog.Username);
            }
            PropertySource["term"] = term;
            if (context.Term != null)
            {
                PropertySource["selectedterm"] = context.Term;
            }

        }

        public BlogTokenReplace(ModuleInfo module, BlogContextInfo context, ModuleSettings settings, ViewSettings viewSettings, PostInfo Post, CommentInfo comment) : base(Scope.DefaultSettings)
        {
            PrimaryObject = comment;
            ModuleInfo = module;
            UseObjectLessExpression = false;
            PropertySource["query"] = context;
            PropertySource["security"] = context.Security;
            PropertySource["urls"] = context.ModuleUrls;
            PropertySource["settings"] = settings;
            PropertySource["viewsettings"] = viewSettings;
            if (Post != null)
            {
                PropertySource["post"] = Post;
                PropertySource["author"] = new LazyLoadingUser(PortalSettings.PortalId, Post.CreatedByUserID, Post.Username);
                PropertySource["blog"] = Post.Blog;
                PropertySource["owner"] = new LazyLoadingUser(PortalSettings.PortalId, Post.Blog.OwnerUserId, Post.Blog.Username);
            }
            else if (context.Blog != null)
            {
                PropertySource["blog"] = context.Blog;
                PropertySource["owner"] = new LazyLoadingUser(PortalSettings.PortalId, context.Blog.OwnerUserId, context.Blog.Username);
            }
            PropertySource["comment"] = comment;
            PropertySource["commenter"] = new LazyLoadingUser(PortalSettings.PortalId, comment.CreatedByUserID, comment.Username);
        }
    }
}