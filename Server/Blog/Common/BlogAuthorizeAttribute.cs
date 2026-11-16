using DotNetNuke.Common;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Modules.Blog.Core.Security;
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
        Host = 5
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
            User = HttpContextSource.Current.Request.IsAuthenticated ? UserController.Instance.GetCurrentUserInfo() : new UserInfo();
            var module = context.ActionContext.Request.FindModuleInfo();
            var security = new ContextSecurity(module.ModuleID, module.TabID, null, User);
            switch (SecurityLevel)
            {
                case SecurityAccessLevel.Authenticated:
                    return User.UserID != -1;
                case SecurityAccessLevel.Host:
                    return User.IsSuperUser;
                case SecurityAccessLevel.Admin:
                    return security.UserIsAdmin || security.IsEditor;
                case SecurityAccessLevel.Edit:
                    return security.IsEditor || security.IsBlogger || security.CanAddPost || security.CanEditPost;
                case SecurityAccessLevel.View:
                    return true;
            }
            return false;
        }
    }
}
