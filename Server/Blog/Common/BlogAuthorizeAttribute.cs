using System;
using System.Collections.Generic;
using System.Reflection;
using DotNetNuke.Common;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Modules.Blog.Core.Common;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using DotNetNuke.Modules.Blog.Core.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Web.Api;

namespace DotNetNuke.Modules.Blog.Common
{
    public enum SecurityAccessLevel
    {
        Anonymous = 0,
        Authenticated = 1,
        View = 2,
        Edit = 3,
        Admin = 4,
        Host = 5,
        AddComment = 6,
        ApproveComment = 7
    }

    public class BlogAuthorizeAttribute : AuthorizeAttributeBase, IOverrideDefaultAuthLevel
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(BlogAuthorizeAttribute));
        public SecurityAccessLevel SecurityLevel { get; set; }
        public UserInfo User { get; set; }

        public BlogAuthorizeAttribute()
        {
            SecurityLevel = SecurityAccessLevel.Admin;
        }

        public BlogAuthorizeAttribute(SecurityAccessLevel accessLevel)
        {
            SecurityLevel = accessLevel;
        }

        public override bool IsAuthorized(AuthFilterContext context)
        {
            Logger.Trace("IsAuthorized");
            if (SecurityLevel == SecurityAccessLevel.Anonymous)
            {
                return true;
            }

            User = HttpContextSource.Current.Request.IsAuthenticated
                ? UserController.Instance.GetCurrentUserInfo()
                : new UserInfo();

            var module = context.ActionContext.Request.FindModuleInfo();
            if (module == null)
                return false;

            if (SecurityLevel == SecurityAccessLevel.AddComment || SecurityLevel == SecurityAccessLevel.ApproveComment)
            {
                var blogId = ResolveBlogId(context);
                if (blogId > 0)
                {
                    var blog = BlogsController.GetBlog(blogId, User.UserID, System.Threading.Thread.CurrentThread.CurrentCulture.Name);
                    var security = new ContextSecurity(module.ModuleID, module.TabID, blog, User);
                    if (SecurityLevel == SecurityAccessLevel.AddComment)
                        return security.CanAddComment;
                    return security.CanApproveComment;
                }

                // Body-bound BlogId may not be available yet during authorization; actions re-check.
                return ModulePermissionController.CanViewModule(module);
            }

            var moduleSecurity = new ContextSecurity(module.ModuleID, module.TabID, null, User);
            switch (SecurityLevel)
            {
                case SecurityAccessLevel.Authenticated:
                    return User.UserID != -1;
                case SecurityAccessLevel.Host:
                    return User.IsSuperUser;
                case SecurityAccessLevel.Admin:
                    return moduleSecurity.UserIsAdmin || moduleSecurity.IsEditor;
                case SecurityAccessLevel.Edit:
                    return moduleSecurity.IsEditor || moduleSecurity.IsBlogger || moduleSecurity.CanAddPost || moduleSecurity.CanEditPost;
                case SecurityAccessLevel.View:
                    return true;
            }
            return false;
        }

        private static int ResolveBlogId(AuthFilterContext context)
        {
            int blogId = -1;
            blogId = HttpContextSource.Current.Request.Params.ReadValue("blogId", blogId);
            blogId = HttpContextSource.Current.Request.Params.ReadValue("BlogId", blogId);
            if (blogId > 0)
                return blogId;

            foreach (var arg in context.ActionContext.ActionArguments.Values)
            {
                if (arg == null)
                    continue;
                var id = ReadBlogIdFromObject(arg);
                if (id > 0)
                    return id;
            }
            return -1;
        }

        private static int ReadBlogIdFromObject(object arg)
        {
            if (arg is IDictionary<string, object> dict)
            {
                foreach (var key in new[] { "blogId", "BlogId" })
                {
                    if (dict.TryGetValue(key, out var raw) && raw != null && int.TryParse(raw.ToString(), out var id) && id > 0)
                        return id;
                }
                return -1;
            }

            var type = arg.GetType();
            var prop = type.GetProperty("BlogId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
                       ?? type.GetProperty("blogId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (prop == null)
                return -1;
            var value = prop.GetValue(arg, null);
            if (value == null)
                return -1;
            return Convert.ToInt32(value);
        }
    }
}
