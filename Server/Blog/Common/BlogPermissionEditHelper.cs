using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using DotNetNuke.Modules.Blog.Core.Security;
using DotNetNuke.Modules.Blog.Core.Security.Permissions;
using DotNetNuke.Modules.Blog.Models;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Localization;
using BlogGlobals = DotNetNuke.Modules.Blog.Core.Common.Globals;
using BlogSecurity = DotNetNuke.Modules.Blog.Core.Security.Security;

namespace DotNetNuke.Modules.Blog.Common
{
    public static class BlogPermissionEditHelper
    {
        private static readonly string[] PermissionKeys =
        {
            "ADD", "EDIT", "APPROVE", "VIEWCOMMENT", "ADDCOMMENT", "APPROVECOMMENT", "AUTOAPPROVECOMMENT"
        };

        public static void PopulatePermissions(BlogEditViewModel model, BlogInfo blog, PortalSettings portalSettings)
        {
            var existing = blog?.Permissions ?? new BlogPermissionCollection();
            PopulatePermissions(model, existing, portalSettings);
        }

        public static void PopulatePermissions(BlogEditViewModel model, BlogPermissionCollection existing, PortalSettings portalSettings)
        {
            var permissions = new PermissionCollection();
            model.PermissionColumns = PermissionKeys.Select(key =>
            {
                var info = permissions[key];
                var header = Localization.GetString(key + ".Permission", BlogGlobals.SharedResourceFileName);
                return new BlogPermissionColumnModel
                {
                    PermissionId = info.PermissionId,
                    PermissionKey = key,
                    Header = string.IsNullOrWhiteSpace(header) ? key : header
                };
            }).ToList();

            model.RolePermissions = BuildRoleRows(existing, portalSettings, model.PermissionColumns);
            model.UserPermissions = BuildUserRows(existing, model.PermissionColumns);
        }

        public static bool TryAddUser(BlogEditViewModel model, PortalSettings portalSettings, out string error)
        {
            error = null;
            var username = (model.AddPermissionUsername ?? "").Trim();
            if (string.IsNullOrEmpty(username))
            {
                error = "Enter a username.";
                return false;
            }

            var user = UserController.GetUserByName(portalSettings.PortalId, username);
            if (user == null)
            {
                error = "User not found.";
                return false;
            }

            if (model.UserPermissions == null)
                model.UserPermissions = new List<BlogPermissionUserRowModel>();

            if (model.UserPermissions.Any(u => u.UserId == user.UserID))
            {
                error = "User is already in the permissions list.";
                return false;
            }

            EnsureColumns(model);
            NormalizeEnabledFlags(model);
            model.UserPermissions.Add(new BlogPermissionUserRowModel
            {
                UserId = user.UserID,
                DisplayName = user.DisplayName,
                Username = user.Username,
                Cells = model.PermissionColumns.Select(col => new BlogPermissionCellModel
                {
                    PermissionId = col.PermissionId,
                    PermissionKey = col.PermissionKey,
                    AllowAccess = col.PermissionKey == "ADD",
                    Enabled = true
                }).ToList()
            });

            model.AddPermissionUsername = "";
            return true;
        }

        public static void NormalizeEnabledFlags(BlogEditViewModel model)
        {
            if (model?.RolePermissions == null) return;
            foreach (var row in model.RolePermissions)
            {
                if (row?.Cells == null) continue;
                foreach (var cell in row.Cells)
                {
                    if (cell == null) continue;
                    cell.Enabled = IsRoleCellEnabled(row.RoleId, cell.PermissionId);
                    if (!cell.Enabled)
                        cell.AllowAccess = false;
                }
            }
        }

        public static BlogPermissionCollection ToPermissionCollection(BlogEditViewModel model, int blogId)
        {
            var result = new BlogPermissionCollection();
            EnsureColumns(model);

            if (model.RolePermissions != null)
            {
                foreach (var row in model.RolePermissions)
                {
                    if (row?.Cells == null) continue;
                    foreach (var cell in row.Cells)
                    {
                        if (cell == null || !cell.AllowAccess) continue;
                        result.Add(new BlogPermissionInfo
                        {
                            BlogId = blogId,
                            PermissionId = cell.PermissionId,
                            PermissionKey = cell.PermissionKey,
                            RoleId = row.RoleId,
                            RoleName = row.RoleName,
                            UserId = BlogSecurity.glbUserNothing,
                            DisplayName = Null.NullString,
                            AllowAccess = true
                        });
                    }
                }
            }

            if (model.UserPermissions != null)
            {
                foreach (var row in model.UserPermissions)
                {
                    if (row?.Cells == null) continue;
                    foreach (var cell in row.Cells)
                    {
                        if (cell == null || !cell.AllowAccess) continue;
                        result.Add(new BlogPermissionInfo
                        {
                            BlogId = blogId,
                            PermissionId = cell.PermissionId,
                            PermissionKey = cell.PermissionKey,
                            RoleId = BlogSecurity.glbRoleNothing,
                            RoleName = Null.NullString,
                            UserId = row.UserId,
                            DisplayName = row.DisplayName,
                            AllowAccess = true
                        });
                    }
                }
            }

            return result;
        }

        private static void EnsureColumns(BlogEditViewModel model)
        {
            if (model.PermissionColumns != null && model.PermissionColumns.Count > 0)
                return;

            var permissions = new PermissionCollection();
            model.PermissionColumns = PermissionKeys.Select(key =>
            {
                var info = permissions[key];
                return new BlogPermissionColumnModel
                {
                    PermissionId = info.PermissionId,
                    PermissionKey = key,
                    Header = key
                };
            }).ToList();
        }

        private static List<BlogPermissionRoleRowModel> BuildRoleRows(
            BlogPermissionCollection existing,
            PortalSettings portalSettings,
            List<BlogPermissionColumnModel> columns)
        {
            var roles = BlogGlobals.GetRolesByPortal(portalSettings.PortalId)
                .Where(r => r.RoleID != portalSettings.AdministratorRoleId)
                .ToList();

            roles.Add(new RoleInfo
            {
                RoleID = -1,
                RoleName = DotNetNuke.Common.Globals.glbRoleAllUsersName
            });

            roles = roles.OrderBy(r => (r.RoleName ?? "").ToLowerInvariant()).ToList();

            return roles.Select(role => new BlogPermissionRoleRowModel
            {
                RoleId = role.RoleID,
                RoleName = role.RoleName,
                Cells = columns.Select(col => new BlogPermissionCellModel
                {
                    PermissionId = col.PermissionId,
                    PermissionKey = col.PermissionKey,
                    AllowAccess = HasRolePermission(existing, col.PermissionId, role.RoleID),
                    Enabled = IsRoleCellEnabled(role.RoleID, col.PermissionId)
                }).ToList()
            }).ToList();
        }

        private static List<BlogPermissionUserRowModel> BuildUserRows(
            BlogPermissionCollection existing,
            List<BlogPermissionColumnModel> columns)
        {
            var userIds = existing
                .Where(p => p.UserId != BlogSecurity.glbUserNothing && p.UserId > 0)
                .Select(p => p.UserId)
                .Distinct()
                .ToList();

            return userIds.Select(userId =>
            {
                var sample = existing.First(p => p.UserId == userId);
                return new BlogPermissionUserRowModel
                {
                    UserId = userId,
                    DisplayName = sample.DisplayName ?? "",
                    Username = sample.Username ?? "",
                    Cells = columns.Select(col => new BlogPermissionCellModel
                    {
                        PermissionId = col.PermissionId,
                        PermissionKey = col.PermissionKey,
                        AllowAccess = HasUserPermission(existing, col.PermissionId, userId),
                        Enabled = true
                    }).ToList()
                };
            }).OrderBy(u => (u.DisplayName ?? "").ToLowerInvariant()).ToList();
        }

        private static bool IsRoleCellEnabled(int roleId, int permissionId)
        {
            if (roleId != -1)
                return true;

            return permissionId != (int)BlogPermissionTypes.ADD
                && permissionId != (int)BlogPermissionTypes.EDIT
                && permissionId != (int)BlogPermissionTypes.APPROVE;
        }

        private static bool HasRolePermission(BlogPermissionCollection existing, int permissionId, int roleId)
        {
            return existing.Any(p =>
                p.PermissionId == permissionId
                && p.RoleId == roleId
                && p.UserId == BlogSecurity.glbUserNothing
                && p.AllowAccess);
        }

        private static bool HasUserPermission(BlogPermissionCollection existing, int permissionId, int userId)
        {
            return existing.Any(p =>
                p.PermissionId == permissionId
                && p.UserId == userId
                && p.AllowAccess);
        }
    }
}
