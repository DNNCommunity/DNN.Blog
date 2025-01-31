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
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Services.Tokens;
using Microsoft.VisualBasic.CompilerServices;

namespace DotNetNuke.Modules.Blog.Entities.Comments
{

  [Serializable()]
  [XmlRoot("Comment")]
  [DataContract()]
  public partial class CommentInfo : IHydratable, IPropertyAccess, IXmlSerializable
  {

    #region  IHydratable Implementation 
    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Fill hydrates the object from a Datareader
  /// </summary>
  /// <remarks>The Fill method is used by the CBO method to hydrtae the object
  /// rather than using the more expensive Refection  methods.</remarks>
  /// <history>
  /// 	[pdonker]	02/24/2013  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    public void Fill(IDataReader dr)
    {
      CommentID = Convert.ToInt32(Null.SetNull(dr["CommentID"], CommentID));
      ContentItemId = Convert.ToInt32(Null.SetNull(dr["ContentItemId"], ContentItemId));
      ParentId = Convert.ToInt32(Null.SetNull(dr["ParentId"], ParentId));
      Comment = Convert.ToString(Null.SetNull(dr["Comment"], Comment));
      Approved = Convert.ToBoolean(Null.SetNull(dr["Approved"], Approved));
      Author = Convert.ToString(Null.SetNull(dr["Author"], Author));
      Website = Convert.ToString(Null.SetNull(dr["Website"], Website));
      Email = Convert.ToString(Null.SetNull(dr["Email"], Email));
      CreatedByUserID = Convert.ToInt32(Null.SetNull(dr["CreatedByUserID"], CreatedByUserID));
      CreatedOnDate = Conversions.ToDate(Null.SetNull(dr["CreatedOnDate"], CreatedOnDate));
      LastModifiedByUserID = Convert.ToInt32(Null.SetNull(dr["LastModifiedByUserID"], LastModifiedByUserID));
      LastModifiedOnDate = Conversions.ToDate(Null.SetNull(dr["LastModifiedOnDate"], LastModifiedOnDate));
      Username = Convert.ToString(Null.SetNull(dr["Username"], Username));
      DisplayName = Convert.ToString(Null.SetNull(dr["DisplayName"], DisplayName));
      Likes = Convert.ToInt32(Null.SetNull(dr["Likes"], Likes));
      Dislikes = Convert.ToInt32(Null.SetNull(dr["Dislikes"], Dislikes));
      Reports = Convert.ToInt32(Null.SetNull(dr["Reports"], Reports));
      Liked = Convert.ToInt32(Null.SetNull(dr["Liked"], Likes));
      Disliked = Convert.ToInt32(Null.SetNull(dr["Disliked"], Dislikes));
      Reported = Convert.ToInt32(Null.SetNull(dr["Reported"], Reports));
    }
    /// -----------------------------------------------------------------------------
  /// <summary>
  /// Gets and sets the Key ID
  /// </summary>
  /// <remarks>The KeyID property is part of the IHydratble interface.  It is used
  /// as the key property when creating a Dictionary</remarks>
  /// <history>
  /// 	[pdonker]	02/24/2013  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    public int KeyID
    {
      get
      {
        return CommentID;
      }
      set
      {
        CommentID = value;
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
        case "commentid":
          {
            return CommentID.ToString(OutputFormat, formatProvider);
          }
        case "contentitemid":
          {
            return ContentItemId.ToString(OutputFormat, formatProvider);
          }
        case "parentid":
          {
            return ParentId.ToString(OutputFormat, formatProvider);
          }
        case "comment":
          {
            return Comment.OutputHtml(strFormat);
          }
        case "approved":
          {
            return Approved.ToString();
          }
        case "approvedyesno":
          {
            return PropertyAccess.Boolean2LocalizedYesNo(Approved, formatProvider);
          }
        case "author":
          {
            return PropertyAccess.FormatString(Author, strFormat);
          }
        case "website":
          {
            return PropertyAccess.FormatString(Website, strFormat);
          }
        case "email":
          {
            return PropertyAccess.FormatString(Email, strFormat);
          }
        case "createdbyuserid":
          {
            return CreatedByUserID.ToString(OutputFormat, formatProvider);
          }
        case "createdondate":
          {
            return CreatedOnDate.ToString(OutputFormat, formatProvider);
          }
        case "createdondateutc":
          {
            return CreatedOnDate.ToUniversalTime().ToString(OutputFormat, formatProvider);
          }
        case "lastmodifiedbyuserid":
          {
            return LastModifiedByUserID.ToString(OutputFormat, formatProvider);
          }
        case "lastmodifiedondate":
          {
            return LastModifiedOnDate.ToString(OutputFormat, formatProvider);
          }
        case "lastmodifiedondateutc":
          {
            return LastModifiedOnDate.ToUniversalTime().ToString(OutputFormat, formatProvider);
          }
        case "username":
          {
            return PropertyAccess.FormatString(Username, strFormat);
          }
        case "displayname":
          {
            return PropertyAccess.FormatString(DisplayName, strFormat);
          }
        case "likes":
          {
            return Likes.ToString(OutputFormat, formatProvider);
          }
        case "dislikes":
          {
            return Dislikes.ToString(OutputFormat, formatProvider);
          }
        case "reports":
          {
            return Reports.ToString(OutputFormat, formatProvider);
          }
        case "liked":
          {
            return Liked.ToBool().ToString();
          }
        case "likedyesno":
          {
            return PropertyAccess.Boolean2LocalizedYesNo(Liked.ToBool(), formatProvider);
          }
        case "disliked":
          {
            return Disliked.ToBool().ToString();
          }
        case "dislikedyesno":
          {
            return PropertyAccess.Boolean2LocalizedYesNo(Disliked.ToBool(), formatProvider);
          }
        case "reported":
          {
            return Reported.ToBool().ToString();
          }
        case "reportedyesno":
          {
            return PropertyAccess.Boolean2LocalizedYesNo(Reported.ToBool(), formatProvider);
          }
        case "karmaed":
          {
            return (Reported + Liked + Disliked).ToBool().ToString();
          }
        case "karmaedyesno":
          {
            return PropertyAccess.Boolean2LocalizedYesNo((Reported + Liked + Disliked).ToBool(), formatProvider);
          }
        case "posturl":
          {
            string _Link = DotNetNuke.Common.Globals.ApplicationURL(DotNetNuke.Entities.Portals.PortalSettings.Current.ActiveTab.TabID) + "&Post=" + ContentItemId.ToString();
            return DotNetNuke.Common.Globals.FriendlyUrl(DotNetNuke.Entities.Portals.PortalSettings.Current.ActiveTab, _Link);
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
  /// 	[pdonker]	02/24/2013  Created
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
  /// 	[pdonker]	02/24/2013  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    public void ReadXml(XmlReader reader)
    {
      try
      {

        bool localTryParse() { int argresult = CommentID; var ret = int.TryParse(readElement(reader, "CommentID"), out argresult); CommentID = argresult; return ret; }

        if (!localTryParse())
        {
          CommentID = Null.NullInteger;
        }
        bool localTryParse1() { int argresult1 = ContentItemId; var ret = int.TryParse(readElement(reader, "ContentItemId"), out argresult1); ContentItemId = argresult1; return ret; }

        if (!localTryParse1())
        {
          ContentItemId = Null.NullInteger;
        }
        bool localTryParse2() { int argresult2 = ParentId; var ret = int.TryParse(readElement(reader, "ParentId"), out argresult2); ParentId = argresult2; return ret; }

        if (!localTryParse2())
        {
          ParentId = Null.NullInteger;
        }
        Comment = readElement(reader, "Comment");
        bool argresult = Approved;
        bool.TryParse(readElement(reader, "Approved"), out argresult);
        Approved = argresult;
        Author = readElement(reader, "Author");
        Website = readElement(reader, "Website");
        Email = readElement(reader, "Email");
        bool localTryParse3() { int argresult3 = CreatedByUserID; var ret = int.TryParse(readElement(reader, "CreatedByUserID"), out argresult3); CreatedByUserID = argresult3; return ret; }

        if (!localTryParse3())
        {
          CreatedByUserID = Null.NullInteger;
        }
        var argresult1 = CreatedOnDate;
        DateTime.TryParse(readElement(reader, "CreatedOnDate"), out argresult1);
        CreatedOnDate = argresult1;
        bool localTryParse4() { int argresult4 = LastModifiedByUserID; var ret = int.TryParse(readElement(reader, "LastModifiedByUserID"), out argresult4); LastModifiedByUserID = argresult4; return ret; }

        if (!localTryParse4())
        {
          LastModifiedByUserID = Null.NullInteger;
        }
        var argresult2 = LastModifiedOnDate;
        DateTime.TryParse(readElement(reader, "LastModifiedOnDate"), out argresult2);
        LastModifiedOnDate = argresult2;
        Username = readElement(reader, "Username");
        DisplayName = readElement(reader, "DisplayName");
        bool localTryParse5() { int argresult5 = Likes; var ret = int.TryParse(readElement(reader, "Likes"), out argresult5); Likes = argresult5; return ret; }

        if (!localTryParse5())
        {
          Likes = Null.NullInteger;
        }
        bool localTryParse6() { int argresult6 = Dislikes; var ret = int.TryParse(readElement(reader, "Dislikes"), out argresult6); Dislikes = argresult6; return ret; }

        if (!localTryParse6())
        {
          Dislikes = Null.NullInteger;
        }
        bool localTryParse7() { int argresult7 = Reports; var ret = int.TryParse(readElement(reader, "Reports"), out argresult7); Reports = argresult7; return ret; }

        if (!localTryParse7())
        {
          Reports = Null.NullInteger;
        }
      }
      catch (Exception ex)
      {
        // log exception as DNN import routine does not do that
        DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
        // re-raise exception to make sure import routine displays a visible error to the user
        throw new Exception("An error occured during import of an Comment", ex);
      }

    }

    /// -----------------------------------------------------------------------------
  /// <summary>
  /// WriteXml converts the object to Xml (serializes it) and writes it using the XmlWriter passed
  /// </summary>
  /// <remarks></remarks>
  /// <param name="writer">The XmlWriter that contains the xml for the object</param>
  /// <history>
  /// 	[pdonker]	02/24/2013  Created
  /// </history>
  /// -----------------------------------------------------------------------------
    public void WriteXml(XmlWriter writer)
    {
      writer.WriteStartElement("Comment");
      writer.WriteElementString("CommentID", CommentID.ToString());
      writer.WriteElementString("ContentItemId", ContentItemId.ToString());
      writer.WriteElementString("ParentId", ParentId.ToString());
      writer.WriteElementString("Comment", Comment);
      writer.WriteElementString("Approved", Approved.ToString());
      writer.WriteElementString("Author", Author);
      writer.WriteElementString("Website", Website);
      writer.WriteElementString("Email", Email);
      writer.WriteElementString("CreatedByUserID", CreatedByUserID.ToString());
      writer.WriteElementString("CreatedOnDate", CreatedOnDate.ToString());
      writer.WriteElementString("LastModifiedByUserID", LastModifiedByUserID.ToString());
      writer.WriteElementString("LastModifiedOnDate", LastModifiedOnDate.ToString());
      writer.WriteElementString("Username", Username);
      writer.WriteElementString("DisplayName", DisplayName);
      writer.WriteElementString("Likes", Likes.ToString());
      writer.WriteElementString("Dislikes", Dislikes.ToString());
      writer.WriteElementString("Reports", Reports.ToString());
      writer.WriteEndElement();
    }
    #endregion

    #region  ToXml Methods 
    public string ToXml()
    {
      return ToXml("Comment");
    }

    public string ToXml(string elementName)
    {
      var xml = new StringBuilder();
      xml.Append("<");
      xml.Append(elementName);
      AddAttribute(ref xml, "CommentID", CommentID.ToString());
      AddAttribute(ref xml, "ContentItemId", ContentItemId.ToString());
      AddAttribute(ref xml, "ParentId", ParentId.ToString());
      AddAttribute(ref xml, "Comment", Comment);
      AddAttribute(ref xml, "Approved", Approved.ToString());
      AddAttribute(ref xml, "Author", Author);
      AddAttribute(ref xml, "Website", Website);
      AddAttribute(ref xml, "Email", Email);
      AddAttribute(ref xml, "CreatedByUserID", CreatedByUserID.ToString());
      AddAttribute(ref xml, "CreatedOnDate", CreatedOnDate.ToUniversalTime().ToString("u"));
      AddAttribute(ref xml, "LastModifiedByUserID", LastModifiedByUserID.ToString());
      AddAttribute(ref xml, "LastModifiedOnDate", LastModifiedOnDate.ToUniversalTime().ToString("u"));
      AddAttribute(ref xml, "Username", Username);
      AddAttribute(ref xml, "DisplayName", DisplayName);
      AddAttribute(ref xml, "Likes", Likes.ToString());
      AddAttribute(ref xml, "Dislikes", Dislikes.ToString());
      AddAttribute(ref xml, "Reports", Reports.ToString());
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
      var ser = new DataContractJsonSerializer(typeof(CommentInfo));
      ser.WriteObject(s, this);
    }
    #endregion

  }
}