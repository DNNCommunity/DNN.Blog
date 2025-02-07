using DotNetNuke.Common.Utilities;
using DotNetNuke.Modules.Blog.Core.Common;
using System;
using System.Collections;
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

using System.Runtime.Serialization;
using System.Xml;

namespace DotNetNuke.Modules.Blog.Core.Security.Permissions
{
    public partial class BlogPermissionInfo
    {

        #region  Constructors 
        public BlogPermissionInfo()
        {
            BlogId = Null.NullInteger;
            RoleId = Security.glbRoleNothing;
            AllowAccess = false;
            _RoleName = Null.NullString;
            UserId = Security.glbUserNothing;
            Username = Null.NullString;
            DisplayName = Null.NullString;
        } // New

        public BlogPermissionInfo(PermissionInfo pi)
        {
            BlogId = Null.NullInteger;
            RoleId = Security.glbRoleNothing;
            AllowAccess = false;
            _RoleName = Null.NullString;
            UserId = Security.glbUserNothing;
            Username = Null.NullString;
            DisplayName = Null.NullString;
            PermissionId = pi.PermissionId;
            PermissionKey = pi.PermissionKey;
        } // New
        #endregion


        #region  Public Properties 
        private string _RoleName;
        [DataMember()]
        public string RoleName
        {
            get
            {
                if (string.IsNullOrEmpty(_RoleName))
                {
                    if (RoleId == -1)
                    {
                        _RoleName = DotNetNuke.Common.Globals.glbRoleAllUsersName;
                    }
                }
                return _RoleName;
            }
            set
            {
                _RoleName = value;
            }
        }

        public string PermissionKey { get; set; }
        #endregion

        #region  Public Methods 
        public override bool Equals(object obj)
        {
            if (obj is null | !ReferenceEquals(GetType(), obj.GetType()))
            {
                return false;
            }
            BlogPermissionInfo perm = (BlogPermissionInfo)obj;
            return AllowAccess == perm.AllowAccess & Expires > DateTime.Now & BlogId == perm.BlogId & RoleId == perm.RoleId & UserId == perm.UserId & PermissionId == perm.PermissionId;
        }

        public BlogPermissionInfo Clone()
        {
            var res = new BlogPermissionInfo();
            res.AllowAccess = AllowAccess;
            res.DisplayName = DisplayName;
            res.BlogId = BlogId;
            res.Expires = Expires;
            res.PermissionId = PermissionId;
            res.PermissionKey = PermissionKey;
            res.RoleId = RoleId;
            res.RoleName = RoleName;
            res.UserId = UserId;
            res.Username = Username;
            return res;
        }

        public void ReadXml(XmlNode xN)
        {
            var ht = new Hashtable();
            foreach (XmlNode n in xN.ChildNodes)
                ht.Add(n.Name, n.InnerText);
            PermissionId = ht.ReadValue("PermissionId", PermissionId);
            AllowAccess = ht.ReadValue("AllowAccess", AllowAccess);
            PermissionKey = ht.ReadValue("PermissionKey", PermissionKey);
            RoleId= ht.ReadValue("RoleID", RoleId);
            UserId = ht.ReadValue("UserID", UserId);
            RoleName = ht.ReadValue("RoleName", RoleName);
            Username = ht.ReadValue("UserName", Username);
        }
        #endregion

    }
}