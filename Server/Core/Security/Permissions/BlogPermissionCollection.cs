using System;
using System.Collections;
using System.Collections.Generic;
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

using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using DotNetNuke.Entities.Users;

namespace DotNetNuke.Modules.Blog.Security.Permissions
{
  [Serializable()]
  public class BlogPermissionCollection : List<BlogPermissionInfo>, IXmlSerializable
  {

    public BlogPermissionCollection(ArrayList BlogPermissions) : base()
    {
      int i;
      var loopTo = BlogPermissions.Count - 1;
      for (i = 0; i <= loopTo; i++)
      {
        BlogPermissionInfo objBlogPermission = (BlogPermissionInfo)BlogPermissions[i];
        Add(objBlogPermission);
      }
    }

    public BlogPermissionCollection() : base()
    {
    }

    public new BlogPermissionInfo this[int index]
    {
      get
      {
        return base[index];
      }
      set
      {
        base[index] = value;
      }
    }

    public new string ToString(string PermissionKey)
    {
      string res = "";
      foreach (BlogPermissionInfo epi in this)
      {
        if ((epi.PermissionKey ?? "") == (PermissionKey ?? ""))
        {
          if (epi.RoleId == -1)
          {
            res += ";" + DotNetNuke.Common.Globals.glbRoleAllUsersName;
          }
          else if (epi.RoleId > -1)
          {
            res += ";" + epi.RoleName;
          }
          else if (epi.UserId > -1)
          {
            res += ";" + epi.UserId.ToString();
          }
        }
      }
      return res + ";";
    }

    public bool CurrentUserHasPermission(string PermissionKey)
    {
      var u = Framework.ServiceLocator<IUserController, UserController>.Instance.GetCurrentUserInfo();
      if (u is not null)
      {
        foreach (BlogPermissionInfo epi in this)
        {
          if (epi.PermissionKey is null)
          {
            var objP = PermissionsController.GetPermission(epi.PermissionId);
            epi.PermissionKey = objP.PermissionKey;
          }
          if (epi.AllowAccess && (epi.PermissionKey ?? "") == (PermissionKey ?? ""))
          {
            if (epi.RoleId == -1)
              return true;
            if (epi.UserId == u.UserID)
              return true;
            foreach (string role in u.Roles)
            {
              if ((epi.RoleName ?? "") == (role ?? ""))
              {
                return true;
                break;
              }
            }
          }
        }
      }
      return false;
    }

    public bool ContainsPermissionKey(string permissionKey)
    {
      foreach (BlogPermissionInfo epi in this)
      {
        if (epi.PermissionKey is null)
        {
          var objP = PermissionsController.GetPermission(epi.PermissionId);
          epi.PermissionKey = objP.PermissionKey;
        }
        if (epi.AllowAccess && (epi.PermissionKey ?? "") == (permissionKey ?? ""))
        {
          return true;
          break;
        }
      }
      return false;
    }

    public bool ContainsPermission(string permissionKey, int RoleId, int UserId)
    {
      foreach (BlogPermissionInfo epi in this)
      {
        if (epi.PermissionKey is null)
        {
          var objP = PermissionsController.GetPermission(epi.PermissionId);
          epi.PermissionKey = objP.PermissionKey;
        }
        if ((epi.PermissionKey ?? "") == (permissionKey ?? "") & epi.RoleId == RoleId & epi.UserId == UserId)
        {
          return true;
          break;
        }
      }
      return false;
    }

    public void AddPermission(int PortalId, int BlogId, string PermissionKey, int RoleId, int UserId)
    {
      var objP = PermissionsController.GetPermissions()[PermissionKey];
      var pi = new BlogPermissionInfo(objP);
      pi.BlogId = BlogId;
      pi.RoleId = RoleId;
      pi.UserId = UserId;
      Add(pi);
    }

    #region  IXmlSerializable Implementation 
    /// -----------------------------------------------------------------------------
  /// <summary>
  /// GetSchema returns the XmlSchema for this class
  /// </summary>
  /// <remarks>GetSchema is implemented as a stub method as it is not required</remarks>
  /// <history>
  /// 	[pdonker]	05/21/2008  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    public XmlSchema GetSchema()
    {
      return null;
    }

    private string readElement(XmlReader reader, string ElementName)
    {
      if (!(reader.NodeType == XmlNodeType.Element) || (reader.Name ?? "") != (ElementName ?? ""))
      {
        reader.ReadToFollowing(ElementName);
      }
      if (reader.NodeType == XmlNodeType.Element)
      {
        return reader.ReadElementContentAsString();
      }
      else
      {
        return "";
      }
    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// ReadXml fills the object (de-serializes it) from the XmlReader passed
  /// </summary>
  /// <remarks></remarks>
  /// <param name="reader">The XmlReader that contains the xml for the object</param>
  /// <history>
  /// 	[pdonker]	05/21/2008  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    public void ReadXml(XmlReader reader)
    {
      try
      {
        reader.ReadStartElement("Permissions");
        do
        {
          reader.ReadStartElement("Permission");
          var epi = new BlogPermissionInfo();
          epi.ReadXml(reader);
          Add(epi);
        }
        while (reader.ReadToNextSibling("Permission"));
      }
      catch (Exception ex)
      {
        // log exception as DNN import routine does not do that
        DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
        // re-raise exception to make sure import routine displays a visible error to the user
        throw new Exception("An error occured during import of an Category", ex);
      }

    }

    public void ReadXml(XmlNode xN)
    {

      foreach (XmlNode xPermission in xN.ChildNodes)
      {
        var epi = new BlogPermissionInfo();
        epi.ReadXml(xPermission);
        if (!string.IsNullOrEmpty(epi.RoleName)) // we don't import user permissions
        {
          Add(epi);
        }
      }

    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// WriteXml converts the object to Xml (serializes it) and writes it using the XmlWriter passed
  /// </summary>
  /// <remarks></remarks>
  /// <param name="writer">The XmlWriter that contains the xml for the object</param>
  /// <history>
  /// 	[pdonker]	05/21/2008  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    public void WriteXml(XmlWriter writer)
    {
      writer.WriteStartElement("Permissions");
      foreach (BlogPermissionInfo epi in this)
      {
        if (epi.PermissionKey is null)
        {
          var pi = PermissionsController.GetPermission(epi.PermissionId);
          epi.PermissionKey = pi.PermissionKey;
        }
        epi.WriteXml(writer);
      }
      writer.WriteEndElement();
    }
    #endregion

  }
}