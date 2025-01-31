using System;
using System.Data;
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

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Tokens;
using Microsoft.VisualBasic.CompilerServices;

namespace DotNetNuke.Modules.Blog.Security.Permissions
{

  [Serializable()]
  [XmlRoot("BlogPermission")]
  [DataContract()]
  public partial class BlogPermissionInfo : IHydratable, IPropertyAccess, IXmlSerializable
  {

    #region  IHydratable Implementation 
    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Fill hydrates the object from a Datareader
  /// </summary>
  /// <remarks>The Fill method is used by the CBO method to hydrtae the object
  /// rather than using the more expensive Refection  methods.</remarks>
  /// <history>
  /// 	[pdonker]	02/08/2013  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    public void Fill(IDataReader dr)
    {

      AllowAccess = Convert.ToBoolean(Null.SetNull(dr["AllowAccess"], AllowAccess));
      BlogId = Convert.ToInt32(Null.SetNull(dr["BlogId"], BlogId));
      Expires = Conversions.ToDate(Null.SetNull(dr["Expires"], Expires));
      PermissionId = Convert.ToInt32(Null.SetNull(dr["PermissionId"], PermissionId));
      RoleId = Convert.ToInt32(Null.SetNull(dr["RoleId"], RoleId));
      UserId = Convert.ToInt32(Null.SetNull(dr["UserId"], UserId));
      Username = Convert.ToString(Null.SetNull(dr["Username"], Username));
      DisplayName = Convert.ToString(Null.SetNull(dr["DisplayName"], DisplayName));
      RoleName = Convert.ToString(Null.SetNull(dr["RoleName"], RoleName));

    }
    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Gets and sets the Key ID
  /// </summary>
  /// <remarks>The KeyID property is part of the IHydratble interface.  It is used
  /// as the key property when creating a Dictionary</remarks>
  /// <history>
  /// 	[pdonker]	02/08/2013  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    public int KeyID
    {
      get
      {
        return default;
      }
      set
      {
      }
    }
    #endregion

    #region  IPropertyAccess Implementation 
    public string GetProperty(string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, DotNetNuke.Entities.Users.UserInfo AccessingUser, Scope AccessLevel, ref bool PropertyNotFound)
    {
      string OutputFormat = string.Empty;
      var portalSettings = Framework.ServiceLocator<DotNetNuke.Entities.Portals.IPortalController, DotNetNuke.Entities.Portals.PortalController>.Instance.GetCurrentPortalSettings();
      if (string.IsNullOrEmpty(strFormat))
      {
        OutputFormat = "D";
      }
      else
      {
        OutputFormat = strFormat;
      }
      switch (strPropertyName.ToLower() ?? "")
      {
        case "allowaccess":
          {
            return PropertyAccess.Boolean2LocalizedYesNo(AllowAccess, formatProvider);
          }
        case "blogid":
          {
            return BlogId.ToString(OutputFormat, formatProvider);
          }
        case "expires":
          {
            return Expires.ToString(OutputFormat, formatProvider);
          }
        case "permissionid":
          {
            return PermissionId.ToString(OutputFormat, formatProvider);
          }
        case "roleid":
          {
            return RoleId.ToString(OutputFormat, formatProvider);
          }
        case "userid":
          {
            return UserId.ToString(OutputFormat, formatProvider);
          }
        case "username":
          {
            return PropertyAccess.FormatString(Username, strFormat);
          }
        case "displayname":
          {
            return PropertyAccess.FormatString(DisplayName, strFormat);
          }
        case "rolename":
          {
            return PropertyAccess.FormatString(RoleName, strFormat);
          }

        default:
          {
            PropertyNotFound = true;
            break;
          }
      }

      return Null.NullString;
    }

    public CacheLevel Cacheability
    {
      get
      {
        return CacheLevel.fullyCacheable;
      }
    }
    #endregion

    #region  IXmlSerializable Implementation 
    /// -----------------------------------------------------------------------------
  /// <summary>
  /// GetSchema returns the XmlSchema for this class
  /// </summary>
  /// <remarks>GetSchema is implemented as a stub method as it is not required</remarks>
  /// <history>
  /// 	[pdonker]	02/08/2013  Created
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
  /// 	[pdonker]	02/08/2013  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    public void ReadXml(XmlReader reader)
    {
      try
      {

        bool argresult = AllowAccess;
        bool.TryParse(readElement(reader, "AllowAccess"), out argresult);
        AllowAccess = argresult;
        bool localTryParse() { int argresult = BlogId; var ret = int.TryParse(readElement(reader, "BlogId"), out argresult); BlogId = argresult; return ret; }

        if (!localTryParse())
        {
          BlogId = Null.NullInteger;
        }
        var argresult1 = Expires;
        DateTime.TryParse(readElement(reader, "Expires"), out argresult1);
        Expires = argresult1;
        bool localTryParse1() { int argresult1 = PermissionId; var ret = int.TryParse(readElement(reader, "PermissionId"), out argresult1); PermissionId = argresult1; return ret; }

        if (!localTryParse1())
        {
          PermissionId = Null.NullInteger;
        }
        bool localTryParse2() { int argresult2 = RoleId; var ret = int.TryParse(readElement(reader, "RoleId"), out argresult2); RoleId = argresult2; return ret; }

        if (!localTryParse2())
        {
          RoleId = Null.NullInteger;
        }
        bool localTryParse3() { int argresult3 = UserId; var ret = int.TryParse(readElement(reader, "UserId"), out argresult3); UserId = argresult3; return ret; }

        if (!localTryParse3())
        {
          UserId = Null.NullInteger;
        }
        Username = readElement(reader, "Username");
        DisplayName = readElement(reader, "DisplayName");
        RoleName = readElement(reader, "RoleName");
      }
      catch (Exception ex)
      {
        // log exception as DNN import routine does not do that
        DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
        // re-raise exception to make sure import routine displays a visible error to the user
        throw new Exception("An error occured during import of an BlogPermission", ex);
      }

    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// WriteXml converts the object to Xml (serializes it) and writes it using the XmlWriter passed
  /// </summary>
  /// <remarks></remarks>
  /// <param name="writer">The XmlWriter that contains the xml for the object</param>
  /// <history>
  /// 	[pdonker]	02/08/2013  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    public void WriteXml(XmlWriter writer)
    {
      writer.WriteStartElement("BlogPermission");
      writer.WriteElementString("AllowAccess", AllowAccess.ToString());
      writer.WriteElementString("BlogId", BlogId.ToString());
      writer.WriteElementString("Expires", Expires.ToString());
      writer.WriteElementString("PermissionId", PermissionId.ToString());
      writer.WriteElementString("RoleId", RoleId.ToString());
      writer.WriteElementString("UserId", UserId.ToString());
      writer.WriteElementString("Username", Username);
      writer.WriteElementString("DisplayName", DisplayName);
      writer.WriteElementString("RoleName", RoleName);
      writer.WriteEndElement();
    }
    #endregion

    #region  ToXml Methods 
    public string ToXml()
    {
      return ToXml("BlogPermission");
    }

    public string ToXml(string elementName)
    {
      var xml = new StringBuilder();
      xml.Append("<");
      xml.Append(elementName);
      AddAttribute(ref xml, "AllowAccess", AllowAccess.ToString());
      AddAttribute(ref xml, "BlogId", BlogId.ToString());
      AddAttribute(ref xml, "Expires", Expires.ToString());
      AddAttribute(ref xml, "PermissionId", PermissionId.ToString());
      AddAttribute(ref xml, "RoleId", RoleId.ToString());
      AddAttribute(ref xml, "UserId", UserId.ToString());
      AddAttribute(ref xml, "Username", Username);
      AddAttribute(ref xml, "DisplayName", DisplayName);
      AddAttribute(ref xml, "RoleName", RoleName);
      AddAttribute(ref xml, "AllowAccess", AllowAccess.ToString());
      AddAttribute(ref xml, "BlogId", BlogId.ToString());
      AddAttribute(ref xml, "Expires", Expires.ToUniversalTime().ToString("u"));
      AddAttribute(ref xml, "PermissionId", PermissionId.ToString());
      AddAttribute(ref xml, "RoleId", RoleId.ToString());
      AddAttribute(ref xml, "UserId", UserId.ToString());
      AddAttribute(ref xml, "Username", Username);
      AddAttribute(ref xml, "DisplayName", DisplayName);
      AddAttribute(ref xml, "RoleName", RoleName);
      xml.Append(" />");
      return xml.ToString();
    }

    private void AddAttribute(ref StringBuilder xml, string attributeName, string attributeValue)
    {
      xml.Append(" " + attributeName);
      xml.Append("=\"" + attributeValue + "\"");
    }
    #endregion

    #region  JSON Serialization 
    public void WriteJSON(ref Stream s)
    {
      var ser = new DataContractJsonSerializer(typeof(BlogPermissionInfo));
      ser.WriteObject(s, this);
    }
    #endregion

  }
}