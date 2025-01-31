
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

using DotNetNuke.Entities.Portals;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Modules.Blog.Entities.Blogs;
using DotNetNuke.Modules.Blog.Entities.Comments;
using DotNetNuke.Modules.Blog.Entities.Posts;
using DotNetNuke.Modules.Blog.Entities.Terms;
using DotNetNuke.Services.Tokens;

namespace DotNetNuke.Modules.Blog.Templating
{
  public class BlogTokenReplace : GenericTokenReplace
  {

    public BlogTokenReplace(int moduleId)
    {
      base.ctor(Scope.DefaultSettings);

      var actModule = new DotNetNuke.Entities.Modules.ModuleController().GetModule(moduleId);
      ModuleInfo = actModule;
      UseObjectLessExpression = false;

    }

    public BlogTokenReplace(DotNetNuke.Entities.Modules.ModuleInfo actModule, Security.ContextSecurity security, BlogInfo blog, PostInfo post, ModuleSettings settings, ViewSettings viewSettings)
    {
      base.ctor(Scope.DefaultSettings);

      ModuleInfo = actModule;
      UseObjectLessExpression = false;
      PropertySource["security"] = security;
      PropertySource["settings"] = settings;
      PropertySource["viewsettings"] = viewSettings;
      if (blog is not null)
      {
        PropertySource["blog"] = blog;
        PropertySource["owner"] = new LazyLoadingUser(PortalSettings.PortalId, blog.OwnerUserId, blog.Username);
      }
      if (post is not null)
      {
        PropertySource["post"] = post;
      }

    }

    public BlogTokenReplace(DotNetNuke.Entities.Modules.ModuleInfo actModule, Security.ContextSecurity security, BlogInfo blog, PostInfo Post, ModuleSettings settings, ViewSettings viewSettings, CommentInfo comment)
    {
      base.ctor(Scope.DefaultSettings);

      PrimaryObject = comment;
      ModuleInfo = actModule;
      UseObjectLessExpression = false;
      PropertySource["security"] = security;
      PropertySource["settings"] = settings;
      PropertySource["viewsettings"] = viewSettings;
      if (Post is not null)
      {
        PropertySource["post"] = Post;
        PropertySource["author"] = new LazyLoadingUser(PortalSettings.PortalId, Post.CreatedByUserID, Post.Username);
        PropertySource["blog"] = Post.Blog;
        PropertySource["owner"] = new LazyLoadingUser(PortalSettings.PortalId, Post.Blog.OwnerUserId, Post.Blog.Username);
      }
      else if (blog is not null)
      {
        PropertySource["blog"] = blog;
        PropertySource["owner"] = new LazyLoadingUser(PortalSettings.PortalId, blog.OwnerUserId, blog.Username);
      }
      PropertySource["comment"] = comment;
      PropertySource["commenter"] = new LazyLoadingUser(PortalSettings.PortalId, comment.CreatedByUserID, comment.Username);

    }

    public BlogTokenReplace(BlogModuleBase blogModule)
    {
      base.ctor(Scope.DefaultSettings);

      ModuleInfo = blogModule.ModuleConfiguration;
      UseObjectLessExpression = false;
      PropertySource["query"] = blogModule.BlogContext;
      PropertySource["security"] = blogModule.BlogContext.Security;
      PropertySource["urls"] = blogModule.BlogContext.ModuleUrls;
      PropertySource["settings"] = blogModule.Settings;
      PropertySource["viewsettings"] = blogModule.ViewSettings;
      if (blogModule.BlogContext.Blog is not null)
      {
        PropertySource["blog"] = blogModule.BlogContext.Blog;
        PropertySource["owner"] = new LazyLoadingUser(PortalSettings.PortalId, blogModule.BlogContext.Blog.OwnerUserId, blogModule.BlogContext.Blog.Username);
      }
      if (blogModule.BlogContext.Post is not null)
      {
        PropertySource["post"] = blogModule.BlogContext.Post;
        PropertySource["author"] = new LazyLoadingUser(PortalSettings.PortalId, blogModule.BlogContext.Post.CreatedByUserID, blogModule.BlogContext.Post.Username);
      }
      if (blogModule.BlogContext.Term is not null)
      {
        PropertySource["selectedterm"] = blogModule.BlogContext.Term;
      }
      if (blogModule.BlogContext.Author is not null)
      {
        PropertySource["author"] = blogModule.BlogContext.Author;
      }

    }

    public BlogTokenReplace(BlogModuleBase blogModule, BlogInfo blog)
    {
      base.ctor(Scope.DefaultSettings);

      PrimaryObject = blog;
      ModuleInfo = blogModule.ModuleConfiguration;
      UseObjectLessExpression = false;
      PropertySource["query"] = blogModule.BlogContext;
      PropertySource["security"] = blogModule.BlogContext.Security;
      PropertySource["urls"] = blogModule.BlogContext.ModuleUrls;
      PropertySource["settings"] = blogModule.Settings;
      PropertySource["viewsettings"] = blogModule.ViewSettings;
      PropertySource["blog"] = blog;
      PropertySource["owner"] = new LazyLoadingUser(PortalSettings.PortalId, blog.OwnerUserId, blog.Username);
      if (blogModule.BlogContext.Post is not null)
      {
        PropertySource["post"] = blogModule.BlogContext.Post;
        PropertySource["author"] = new LazyLoadingUser(PortalSettings.PortalId, blogModule.BlogContext.Post.CreatedByUserID, blogModule.BlogContext.Post.Username);
      }
      else if (blogModule.BlogContext.Author is not null)
      {
        PropertySource["author"] = blogModule.BlogContext.Author;
      }
      if (blogModule.BlogContext.Term is not null)
      {
        PropertySource["selectedterm"] = blogModule.BlogContext.Term;
      }

    }

    public BlogTokenReplace(BlogModuleBase blogModule, BlogCalendarInfo objBlogCalendar)
    {
      base.ctor(Scope.DefaultSettings);

      PrimaryObject = objBlogCalendar;
      ModuleInfo = blogModule.ModuleConfiguration;
      UseObjectLessExpression = false;
      PropertySource["query"] = blogModule.BlogContext;
      PropertySource["security"] = blogModule.BlogContext.Security;
      PropertySource["urls"] = blogModule.BlogContext.ModuleUrls;
      PropertySource["settings"] = blogModule.Settings;
      PropertySource["viewsettings"] = blogModule.ViewSettings;
      if (blogModule.BlogContext.Blog is not null)
      {
        PropertySource["blog"] = blogModule.BlogContext.Blog;
      }
      PropertySource["calendar"] = objBlogCalendar;
      if (blogModule.BlogContext.Post is not null)
      {
        PropertySource["post"] = blogModule.BlogContext.Post;
        PropertySource["author"] = new LazyLoadingUser(PortalSettings.PortalId, blogModule.BlogContext.Post.CreatedByUserID, blogModule.BlogContext.Post.Username);
      }
      else if (blogModule.BlogContext.Author is not null)
      {
        PropertySource["author"] = blogModule.BlogContext.Author;
      }
      if (blogModule.BlogContext.Term is not null)
      {
        PropertySource["selectedterm"] = blogModule.BlogContext.Term;
      }

    }

    public BlogTokenReplace(BlogModuleBase blogModule, LazyLoadingUser objAuthor)
    {
      base.ctor(Scope.DefaultSettings);

      PrimaryObject = objAuthor;
      ModuleInfo = blogModule.ModuleConfiguration;
      UseObjectLessExpression = false;
      PropertySource["query"] = blogModule.BlogContext;
      PropertySource["security"] = blogModule.BlogContext.Security;
      PropertySource["urls"] = blogModule.BlogContext.ModuleUrls;
      PropertySource["settings"] = blogModule.Settings;
      PropertySource["viewsettings"] = blogModule.ViewSettings;
      if (blogModule.BlogContext.Blog is not null)
      {
        PropertySource["blog"] = blogModule.BlogContext.Blog;
      }
      PropertySource["author"] = objAuthor;
      if (blogModule.BlogContext.Post is not null)
      {
        PropertySource["post"] = blogModule.BlogContext.Post;
      }
      if (blogModule.BlogContext.Term is not null)
      {
        PropertySource["selectedterm"] = blogModule.BlogContext.Term;
      }

    }

    public BlogTokenReplace(BlogModuleBase blogModule, PostInfo Post)
    {
      base.ctor(Scope.DefaultSettings);

      PrimaryObject = Post;
      ModuleInfo = blogModule.ModuleConfiguration;
      UseObjectLessExpression = false;
      PropertySource["query"] = blogModule.BlogContext;
      PropertySource["security"] = blogModule.BlogContext.Security;
      PropertySource["urls"] = blogModule.BlogContext.ModuleUrls;
      PropertySource["settings"] = blogModule.Settings;
      PropertySource["viewsettings"] = blogModule.ViewSettings;
      PropertySource["post"] = Post;
      PropertySource["author"] = new LazyLoadingUser(PortalSettings.PortalId, Post.CreatedByUserID, Post.Username);
      PropertySource["blog"] = Post.Blog;
      PropertySource["owner"] = new LazyLoadingUser(PortalSettings.PortalId, Post.Blog.OwnerUserId, Post.Blog.Username);
      if (blogModule.BlogContext.Term is not null)
      {
        PropertySource["selectedterm"] = blogModule.BlogContext.Term;
      }

    }

    public BlogTokenReplace(BlogModuleBase blogModule, PostInfo Post, TermInfo term)
    {
      base.ctor(Scope.DefaultSettings);

      PrimaryObject = term;
      ModuleInfo = blogModule.ModuleConfiguration;
      UseObjectLessExpression = false;
      PropertySource["query"] = blogModule.BlogContext;
      PropertySource["security"] = blogModule.BlogContext.Security;
      PropertySource["urls"] = blogModule.BlogContext.ModuleUrls;
      PropertySource["settings"] = blogModule.Settings;
      PropertySource["viewsettings"] = blogModule.ViewSettings;
      if (Post is not null)
      {
        PropertySource["post"] = Post;
        PropertySource["author"] = new LazyLoadingUser(PortalSettings.PortalId, Post.CreatedByUserID, Post.Username);
        PropertySource["blog"] = Post.Blog;
        PropertySource["owner"] = new LazyLoadingUser(PortalSettings.PortalId, Post.Blog.OwnerUserId, Post.Blog.Username);
      }
      else if (blogModule.BlogContext.Blog is not null)
      {
        PropertySource["blog"] = blogModule.BlogContext.Blog;
        PropertySource["owner"] = new LazyLoadingUser(PortalSettings.PortalId, blogModule.BlogContext.Blog.OwnerUserId, blogModule.BlogContext.Blog.Username);
      }
      PropertySource["term"] = term;
      if (blogModule.BlogContext.Term is not null)
      {
        PropertySource["selectedterm"] = blogModule.BlogContext.Term;
      }

    }

    public BlogTokenReplace(BlogModuleBase blogModule, PostInfo Post, CommentInfo comment)
    {
      base.ctor(Scope.DefaultSettings);

      PrimaryObject = comment;
      ModuleInfo = blogModule.ModuleConfiguration;
      UseObjectLessExpression = false;
      PropertySource["query"] = blogModule.BlogContext;
      PropertySource["security"] = blogModule.BlogContext.Security;
      PropertySource["urls"] = blogModule.BlogContext.ModuleUrls;
      PropertySource["settings"] = blogModule.Settings;
      PropertySource["viewsettings"] = blogModule.ViewSettings;
      if (Post is not null)
      {
        PropertySource["post"] = Post;
        PropertySource["author"] = new LazyLoadingUser(PortalSettings.PortalId, Post.CreatedByUserID, Post.Username);
        PropertySource["blog"] = Post.Blog;
        PropertySource["owner"] = new LazyLoadingUser(PortalSettings.PortalId, Post.Blog.OwnerUserId, Post.Blog.Username);
      }
      else if (blogModule.BlogContext.Blog is not null)
      {
        PropertySource["blog"] = blogModule.BlogContext.Blog;
        PropertySource["owner"] = new LazyLoadingUser(PortalSettings.PortalId, blogModule.BlogContext.Blog.OwnerUserId, blogModule.BlogContext.Blog.Username);
      }
      PropertySource["comment"] = comment;
      PropertySource["commenter"] = new LazyLoadingUser(PortalSettings.PortalId, comment.CreatedByUserID, comment.Username);

    }

  }
}