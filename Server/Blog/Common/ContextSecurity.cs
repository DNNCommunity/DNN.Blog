using System.Web.Caching;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;

namespace DotNetNuke.Modules.Blog.Common
{
    public class ContextSecurity
    {
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
        public bool IsAdmin { get; set; }
        private UserInfo user { get; set; }
        public int UserId
        {
            get
            {
                return user.UserID;
            }
        }

        public static ContextSecurity GetSecurity(ModuleInfo objModule)
        {
            return GetSecurity(objModule, UserController.Instance.GetCurrentUserInfo());
        }
        public static ContextSecurity GetSecurity(ModuleInfo objModule, UserInfo user)
        {
            return CBO.GetCachedObject<ContextSecurity>(new CacheItemArgs(string.Format("security-{0}-{1}", user.UserID, objModule.ModuleID), 10, CacheItemPriority.BelowNormal, objModule, user), GetSecurityCallBack);
        }
        private static object GetSecurityCallBack(CacheItemArgs args)
        {
            return new ContextSecurity((ModuleInfo)args.Params[0], (UserInfo)args.Params[1]);
        }

        #region ctor
        public ContextSecurity(ModuleInfo objModule, UserInfo user)
        {
            this.user = user;
            if (user.IsSuperUser)
            {
                CanView = CanEdit = IsAdmin = true;
            }
            else
            {
                IsAdmin = PortalSecurity.IsInRole(PortalSettings.Current.AdministratorRoleName);
                if (IsAdmin)
                {
                    CanView = CanEdit = true;
                }
                else
                {
                    CanView = ModulePermissionController.CanViewModule(objModule);
                    CanEdit = ModulePermissionController.HasModulePermission(objModule.ModulePermissions, "EDIT");
                }
            }
        }
        #endregion
    }
}