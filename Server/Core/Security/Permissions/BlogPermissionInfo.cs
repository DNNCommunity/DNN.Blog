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

using DotNetNuke.Common.Utilities;
using DotNetNuke.Modules.Blog.Common;

using static DotNetNuke.Modules.Blog.Security.Security;

namespace DotNetNuke.Modules.Blog.Security.Permissions
{
  public partial class BlogPermissionInfo
  {

    #region  Constructors 
    public BlogPermissionInfo()
    {
      BlogId = Null.NullInteger;
      RoleId = glbRoleNothing;
      AllowAccess = false;
      _RoleName = Null.NullString;
      UserId = glbUserNothing;
      Username = Null.NullString;
      DisplayName = Null.NullString;
    } // New

    public BlogPermissionInfo(PermissionInfo pi)
    {
      BlogId = Null.NullInteger;
      RoleId = glbRoleNothing;
      AllowAccess = false;
      _RoleName = Null.NullString;
      UserId = glbUserNothing;
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
      int argVariable = PermissionId;
      Extensions.ReadValue(ref ht, "PermissionId", ref argVariable);
      PermissionId = argVariable;
      bool argVariable1 = AllowAccess;
      Extensions.ReadValue(ref ht, "AllowAccess", ref argVariable1);
      AllowAccess = argVariable1;
      string argVariable2 = PermissionKey;
      Extensions.ReadValue(ref ht, "PermissionKey", ref argVariable2);
      PermissionKey = argVariable2;
      int argVariable3 = RoleId;
      Extensions.ReadValue(ref ht, "RoleID", ref argVariable3);
      RoleId = argVariable3;
      int argVariable4 = UserId;
      Extensions.ReadValue(ref ht, "UserID", ref argVariable4);
      UserId = argVariable4;
      string argVariable5 = RoleName;
      Extensions.ReadValue(ref ht, "RoleName", ref argVariable5);
      RoleName = argVariable5;
      string argVariable6 = Username;
      Extensions.ReadValue(ref ht, "UserName", ref argVariable6);
      Username = argVariable6;

    }
    #endregion

  }
}