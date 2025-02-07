using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Modules.Blog.Core.Common;
using DotNetNuke.Services.Tokens;
using System;
using System.Collections.Generic;
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
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DotNetNuke.Modules.Blog.Core.Entities.Posts
{

    [Serializable()]
    [XmlRoot("Post")]
    [DataContract()]
    public partial class PostInfo : IHydratable, IPropertyAccess, IXmlSerializable
    {
        public int ParentTabID { get; set; } = -1;

        #region  IHydratable Implementation 
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Fill hydrates the object from a Datareader
        /// </summary>
        /// <remarks>The Fill method is used by the CBO method to hydrtae the object
        /// rather than using the more expensive Refection  methods.</remarks>
        /// <history>
        /// 	[pdonker]	02/19/2013  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override void Fill(IDataReader dr)
        {
            FillInternal(dr);
            BlogID = Convert.ToInt32(Null.SetNull(dr["BlogID"], BlogID));
            Title = Convert.ToString(Null.SetNull(dr["Title"], Title));
            Summary = Convert.ToString(Null.SetNull(dr["Summary"], Summary));
            Image = Convert.ToString(Null.SetNull(dr["Image"], Image));
            Published = Convert.ToBoolean(Null.SetNull(dr["Published"], Published));
            PublishedOnDate = Convert.ToDateTime(Null.SetNull(dr["PublishedOnDate"], PublishedOnDate));
            AllowComments = Convert.ToBoolean(Null.SetNull(dr["AllowComments"], AllowComments));
            DisplayCopyright = Convert.ToBoolean(Null.SetNull(dr["DisplayCopyright"], DisplayCopyright));
            Copyright = Convert.ToString(Null.SetNull(dr["Copyright"], Copyright));
            Locale = Convert.ToString(Null.SetNull(dr["Locale"], Locale));
            ViewCount = Convert.ToInt32(Null.SetNull(dr["ViewCount"], ViewCount));
            Username = Convert.ToString(Null.SetNull(dr["Username"], Username));
            Email = Convert.ToString(Null.SetNull(dr["Email"], Email));
            DisplayName = Convert.ToString(Null.SetNull(dr["DisplayName"], DisplayName));
            AltLocale = Convert.ToString(Null.SetNull(dr["AltLocale"], AltLocale));
            AltTitle = Convert.ToString(Null.SetNull(dr["AltTitle"], AltTitle));
            AltSummary = Convert.ToString(Null.SetNull(dr["AltSummary"], AltSummary));
            AltContent = Convert.ToString(Null.SetNull(dr["AltContent"], AltContent));
            NrComments = Convert.ToInt32(Null.SetNull(dr["NrComments"], NrComments));
            PublishedOnDate = DateTime.SpecifyKind(PublishedOnDate, DateTimeKind.Utc);
        }
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Key ID
        /// </summary>
        /// <remarks>The KeyID property is part of the IHydratble interface.  It is used
        /// as the key property when creating a Dictionary</remarks>
        /// <history>
        /// 	[pdonker]	02/19/2013  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override int KeyID
        {
            get
            {
                return ContentItemId;
            }
            set
            {
                ContentItemId = value;
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
                case "blogid":
                    {
                        return BlogID.ToString(OutputFormat, formatProvider);
                    }
                case "title":
                    {
                        return PropertyAccess.FormatString(Title, strFormat);
                    }
                case "summary":
                    {
                        return Summary.OutputHtml(strFormat);
                    }
                case "image":
                    {
                        return PropertyAccess.FormatString(Image, strFormat);
                    }
                case "isvisible":
                    {
                        return Published.ToString();
                    }
                case "published":
                    {
                        return (Published && PublishedOnDate < DateTime.UtcNow).ToString();
                    }
                case "publishedyesno":
                    {
                        return PropertyAccess.Boolean2LocalizedYesNo(Published, formatProvider);
                    }
                case "publishedondate":
                    {
                        var userTimeZone = portalSettings.TimeZone;
                        if (AccessingUser.Profile.PreferredTimeZone != null)
                        {
                            userTimeZone = AccessingUser.Profile.PreferredTimeZone;
                        }
                        return Globals.UtcToLocalTime(PublishedOnDate, userTimeZone).ToString(OutputFormat, formatProvider);
                    }
                case "publishedondateutc":
                    {
                        return PublishedOnDate.ToString(OutputFormat, formatProvider);
                    }
                case "allowcomments":
                    {
                        return AllowComments.ToString();
                    }
                case "allowcommentsyesno":
                    {
                        return PropertyAccess.Boolean2LocalizedYesNo(AllowComments, formatProvider);
                    }
                case "displaycopyright":
                    {
                        return DisplayCopyright.ToString();
                    }
                case "displaycopyrightyesno":
                    {
                        return PropertyAccess.Boolean2LocalizedYesNo(DisplayCopyright, formatProvider);
                    }
                case "copyright":
                    {
                        return PropertyAccess.FormatString(Copyright, strFormat);
                    }
                case "locale":
                    {
                        return PropertyAccess.FormatString(Locale, strFormat);
                    }
                case "nrcomments":
                    {
                        return NrComments.ToString(OutputFormat, formatProvider);
                    }
                case "viewcount":
                    {
                        return ViewCount.ToString(OutputFormat, formatProvider);
                    }
                case "username":
                    {
                        return PropertyAccess.FormatString(Username, strFormat);
                    }
                case "email":
                    {
                        return PropertyAccess.FormatString(Email, strFormat);
                    }
                case "displayname":
                    {
                        return PropertyAccess.FormatString(DisplayName, strFormat);
                    }
                case "altlocale":
                    {
                        return PropertyAccess.FormatString(AltLocale, strFormat);
                    }
                case "alttitle":
                    {
                        return PropertyAccess.FormatString(AltTitle, strFormat);
                    }
                case "altsummary":
                    {
                        return PropertyAccess.FormatString(AltSummary, strFormat);
                    }
                case "altcontent":
                    {
                        return PropertyAccess.FormatString(AltContent, strFormat);
                    }
                case "localizedtitle":
                    {
                        return PropertyAccess.FormatString(LocalizedTitle, strFormat);
                    }
                case "localizedsummary":
                    {
                        return LocalizedSummary.OutputHtml(strFormat);
                    }
                case "localizedcontent":
                    {
                        return LocalizedContent.OutputHtml(strFormat);
                    }
                case "contentitemid":
                    {
                        return ContentItemId.ToString(OutputFormat, formatProvider);
                    }
                case "createdbyuserid":
                    {
                        return CreatedByUserID.ToString(OutputFormat, formatProvider);
                    }
                case "createdondate":
                    {
                        return CreatedOnDate.ToString(OutputFormat, formatProvider);
                    }
                case "lastmodifiedbyuserid":
                    {
                        return LastModifiedByUserID.ToString(OutputFormat, formatProvider);
                    }
                case "lastmodifiedondate":
                    {
                        return LastModifiedOnDate.ToString(OutputFormat, formatProvider);
                    }
                case "content":
                case "contents":
                    {
                        return Content.OutputHtml(strFormat);
                    }
                case "hasimage":
                    {
                        return (!string.IsNullOrEmpty(Image)).ToString(formatProvider);
                    }
                case "link":
                case "permalink":
                    {
                        return PermaLink(DotNetNuke.Entities.Portals.PortalSettings.Current);
                    }
                case "parenturl":
                    {
                        return PermaLink(ParentTabID);
                    }
                case "currentmode":
                    {
                        return DotNetNuke.Entities.Portals.PortalSettings.Current.UserMode.ToString();
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
        /// 	[pdonker]	05/03/2013  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ReadXml fills the object (de-serializes it) from the XmlReader passed
        /// </summary>
        /// <remarks></remarks>
        /// <param name="reader">The XmlReader that contains the xml for the object</param>
        /// <history>
        /// 	[pdonker]	05/03/2013  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void ReadXml(XmlReader reader)
        {
            // not implemented
        }

        internal int ImportedPostId { get; set; } = -1;
        internal List<string> ImportedFiles { get; set; }
        internal List<string> ImportedTags { get; set; }
        internal List<string> ImportedCategories { get; set; }

        public void FromXml(XmlNode xml)
        {
            if (xml is null)
                return;

            ImportedPostId = xml.ReadValue("PostId", ImportedPostId);
            Title = xml.ReadValue("Title", Title);
            TitleLocalizations = xml.ReadValue("TitleLocalizations", TitleLocalizations);
            Summary = xml.ReadValue("Summary", Summary);
            SummaryLocalizations = xml.ReadValue("SummaryLocalizations", SummaryLocalizations);
            Content = xml.ReadValue("Content", Content);
            ContentLocalizations = xml.ReadValue("ContentLocalizations", ContentLocalizations);
            Image = xml.ReadValue("Image", Image);
            Published = xml.ReadValue("Published", Published);
            PublishedOnDate = xml.ReadValue("PublishedOnDate", PublishedOnDate);
            AllowComments = xml.ReadValue("AllowComments", AllowComments);
            DisplayCopyright = xml.ReadValue("DisplayCopyright", DisplayCopyright);
            Copyright = xml.ReadValue("Copyright", Copyright);
            Locale = xml.ReadValue("Locale", Locale);
            Username = xml.ReadValue("Username", Username);
            Email = xml.ReadValue("Email", Email);

            ImportedFiles = new List<string>();
            foreach (XmlNode xFile in xml.SelectNodes("Files/File"))
                ImportedFiles.Add(xFile.InnerText);
            ImportedTags = new List<string>();
            foreach (XmlNode xTag in xml.SelectNodes("Tag"))
                ImportedTags.Add(xTag.InnerText);
            ImportedCategories = new List<string>();
            foreach (XmlNode xCategory in xml.SelectNodes("Category"))
                ImportedCategories.Add(xCategory.InnerText);

        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// WriteXml converts the object to Xml (serializes it) and writes it using the XmlWriter passed
        /// </summary>
        /// <remarks></remarks>
        /// <param name="writer">The XmlWriter that contains the xml for the object</param>
        /// <history>
        /// 	[pdonker]	05/03/2013  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Post");
            writer.WriteElementString("PostId", ContentItemId.ToString());
            writer.WriteElementString("Title", Title);
            writer.WriteElementString("TitleLocalizations", TitleLocalizations.ToString());
            writer.WriteElementString("Summary", Summary);
            writer.WriteElementString("SummaryLocalizations", SummaryLocalizations.ToString());
            writer.WriteElementString("Content", Content);
            writer.WriteElementString("ContentLocalizations", ContentLocalizations.ToString());
            writer.WriteElementString("Image", Image);
            writer.WriteElementString("Published", Published.ToString());
            writer.WriteElementString("PublishedOnDate", PublishedOnDate.ToString());
            writer.WriteElementString("AllowComments", AllowComments.ToString());
            writer.WriteElementString("DisplayCopyright", DisplayCopyright.ToString());
            writer.WriteElementString("Copyright", Copyright);
            writer.WriteElementString("Locale", Locale);
            writer.WriteElementString("Username", Username);
            writer.WriteElementString("Email", Email);
            writer.WriteStartElement("Files");
            // pack files
            string postDir = Globals.GetPostDirectoryMapPath(BlogID, ContentItemId);

            if (Directory.Exists(postDir))
            {
                foreach (string fileName in from f in Directory.GetFiles(postDir)
                                            select Path.GetFileName(f))
                    writer.WriteElementString("File", fileName);
            }
            writer.WriteEndElement(); // Files
            foreach (Terms.TermInfo t in PostTags)
                writer.WriteElementString("Tag", t.Name);
            foreach (Terms.TermInfo t in PostCategories)
                writer.WriteElementString("Category", t.Name);
            writer.WriteEndElement(); // Post
        }
        #endregion

        #region  ToXml Methods 
        public string ToXml()
        {
            return ToXml("Post");
        }

        public string ToXml(string elementName)
        {
            var xml = new StringBuilder();
            xml.Append("<");
            xml.Append(elementName);
            AddAttribute(ref xml, "BlogID", BlogID.ToString());
            AddAttribute(ref xml, "Title", Title);
            AddAttribute(ref xml, "Summary", Summary);
            AddAttribute(ref xml, "Image", Image);
            AddAttribute(ref xml, "Published", Published.ToString());
            AddAttribute(ref xml, "PublishedOnDate", PublishedOnDate.ToUniversalTime().ToString("u"));
            AddAttribute(ref xml, "AllowComments", AllowComments.ToString());
            AddAttribute(ref xml, "DisplayCopyright", DisplayCopyright.ToString());
            AddAttribute(ref xml, "Copyright", Copyright);
            AddAttribute(ref xml, "Locale", Locale);
            AddAttribute(ref xml, "ViewCount", ViewCount.ToString());
            AddAttribute(ref xml, "Username", Username);
            AddAttribute(ref xml, "Email", Email);
            AddAttribute(ref xml, "DisplayName", DisplayName);
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
            var ser = new DataContractJsonSerializer(typeof(PostInfo));
            ser.WriteObject(s, this);
        }
        #endregion

    }
}