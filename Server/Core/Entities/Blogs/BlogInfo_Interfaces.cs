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

using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Modules.Blog.Core.Common;
using DotNetNuke.Services.Tokens;

namespace DotNetNuke.Modules.Blog.Core.Entities.Blogs
{

    [Serializable()]
    [XmlRoot("Blog")]
    [DataContract()]
    public partial class BlogInfo : IHydratable, IPropertyAccess, IXmlSerializable
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
        /// 	[pdonker]	02/16/2013  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void Fill(IDataReader dr)
        {
            BlogID = Convert.ToInt32(Null.SetNull(dr["BlogID"], BlogID));
            ModuleID = Convert.ToInt32(Null.SetNull(dr["ModuleID"], ModuleID));
            Title = Convert.ToString(Null.SetNull(dr["Title"], Title));
            Description = Convert.ToString(Null.SetNull(dr["Description"], Description));
            Image = Convert.ToString(Null.SetNull(dr["Image"], Image));
            Locale = Convert.ToString(Null.SetNull(dr["Locale"], Locale));
            FullLocalization = Convert.ToBoolean(Null.SetNull(dr["FullLocalization"], FullLocalization));
            Published = Convert.ToBoolean(Null.SetNull(dr["Published"], Published));
            IncludeImagesInFeed = Convert.ToBoolean(Null.SetNull(dr["IncludeImagesInFeed"], IncludeImagesInFeed));
            IncludeAuthorInFeed = Convert.ToBoolean(Null.SetNull(dr["IncludeAuthorInFeed"], IncludeAuthorInFeed));
            Syndicated = Convert.ToBoolean(Null.SetNull(dr["Syndicated"], Syndicated));
            SyndicationEmail = Convert.ToString(Null.SetNull(dr["SyndicationEmail"], SyndicationEmail));
            Copyright = Convert.ToString(Null.SetNull(dr["Copyright"], Copyright));
            MustApproveGhostPosts = Convert.ToBoolean(Null.SetNull(dr["MustApproveGhostPosts"], MustApproveGhostPosts));
            PublishAsOwner = Convert.ToBoolean(Null.SetNull(dr["PublishAsOwner"], PublishAsOwner));
            EnablePingBackSend = Convert.ToBoolean(Null.SetNull(dr["EnablePingBackSend"], EnablePingBackSend));
            EnablePingBackReceive = Convert.ToBoolean(Null.SetNull(dr["EnablePingBackReceive"], EnablePingBackReceive));
            AutoApprovePingBack = Convert.ToBoolean(Null.SetNull(dr["AutoApprovePingBack"], AutoApprovePingBack));
            EnableTrackBackSend = Convert.ToBoolean(Null.SetNull(dr["EnableTrackBackSend"], EnableTrackBackSend));
            EnableTrackBackReceive = Convert.ToBoolean(Null.SetNull(dr["EnableTrackBackReceive"], EnableTrackBackReceive));
            AutoApproveTrackBack = Convert.ToBoolean(Null.SetNull(dr["AutoApproveTrackBack"], AutoApproveTrackBack));
            OwnerUserId = Convert.ToInt32(Null.SetNull(dr["OwnerUserId"], OwnerUserId));
            CreatedByUserID = Convert.ToInt32(Null.SetNull(dr["CreatedByUserID"], CreatedByUserID));
            CreatedOnDate = Convert.ToDateTime(Null.SetNull(dr["CreatedOnDate"], CreatedOnDate));
            LastModifiedByUserID = Convert.ToInt32(Null.SetNull(dr["LastModifiedByUserID"], LastModifiedByUserID));
            LastModifiedOnDate = Convert.ToDateTime(Null.SetNull(dr["LastModifiedOnDate"], LastModifiedOnDate));
            DisplayName = Convert.ToString(Null.SetNull(dr["DisplayName"], DisplayName));
            Email = Convert.ToString(Null.SetNull(dr["Email"], Email));
            Username = Convert.ToString(Null.SetNull(dr["Username"], Username));
            NrPosts = Convert.ToInt32(Null.SetNull(dr["NrPosts"], NrPosts));
            LastPublishDate = Convert.ToDateTime(Null.SetNull(dr["LastPublishDate"], LastPublishDate));
            NrViews = Convert.ToInt32(Null.SetNull(dr["NrViews"], NrViews));
            FirstPublishDate = Convert.ToDateTime(Null.SetNull(dr["FirstPublishDate"], FirstPublishDate));
            AltLocale = Convert.ToString(Null.SetNull(dr["AltLocale"], AltLocale));
            AltTitle = Convert.ToString(Null.SetNull(dr["AltTitle"], AltTitle));
            AltDescription = Convert.ToString(Null.SetNull(dr["AltDescription"], AltDescription));
            CanEdit = Convert.ToBoolean(Null.SetNull(dr["CanEdit"], CanEdit));
            CanAdd = Convert.ToBoolean(Null.SetNull(dr["CanAdd"], CanAdd));
            CanApprove = Convert.ToBoolean(Null.SetNull(dr["CanApprove"], CanApprove));

        }
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Key ID
        /// </summary>
        /// <remarks>The KeyID property is part of the IHydratble interface.  It is used
        /// as the key property when creating a Dictionary</remarks>
        /// <history>
        /// 	[pdonker]	02/16/2013  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public int KeyID
        {
            get
            {
                return BlogID;
            }
            set
            {
                BlogID = value;
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
                case "moduleid":
                    {
                        return ModuleID.ToString(OutputFormat, formatProvider);
                    }
                case "title":
                    {
                        return PropertyAccess.FormatString(Title, strFormat);
                    }
                case "description":
                    {
                        return PropertyAccess.FormatString(Description, strFormat);
                    }
                case "image":
                    {
                        return PropertyAccess.FormatString(Image, strFormat);
                    }
                case "hasimage":
                    {
                        return (!string.IsNullOrEmpty(Image)).ToString(formatProvider);
                    }
                case "locale":
                    {
                        return PropertyAccess.FormatString(Locale, strFormat);
                    }
                case "fulllocalization":
                    {
                        return FullLocalization.ToString();
                    }
                case "fulllocalizationyesno":
                    {
                        return PropertyAccess.Boolean2LocalizedYesNo(FullLocalization, formatProvider);
                    }
                case "published":
                    {
                        return Published.ToString();
                    }
                case "publishedyesno":
                    {
                        return PropertyAccess.Boolean2LocalizedYesNo(Published, formatProvider);
                    }
                case "includeimagesinfeed":
                    {
                        return IncludeImagesInFeed.ToString();
                    }
                case "includeimagesinfeedyesno":
                    {
                        return PropertyAccess.Boolean2LocalizedYesNo(IncludeImagesInFeed, formatProvider);
                    }
                case "includeauthorinfeed":
                    {
                        return IncludeAuthorInFeed.ToString();
                    }
                case "includeauthorinfeedyesno":
                    {
                        return PropertyAccess.Boolean2LocalizedYesNo(IncludeAuthorInFeed, formatProvider);
                    }
                case "syndicated":
                    {
                        return Syndicated.ToString();
                    }
                case "syndicatedyesno":
                    {
                        return PropertyAccess.Boolean2LocalizedYesNo(Syndicated, formatProvider);
                    }
                case "syndicationemail":
                    {
                        return PropertyAccess.FormatString(SyndicationEmail, strFormat);
                    }
                case "copyright":
                    {
                        return PropertyAccess.FormatString(Copyright, strFormat);
                    }
                case "mustapproveghostposts":
                    {
                        return MustApproveGhostPosts.ToString();
                    }
                case "mustapproveghostpostsyesno":
                    {
                        return PropertyAccess.Boolean2LocalizedYesNo(MustApproveGhostPosts, formatProvider);
                    }
                case "publishasowner":
                    {
                        return PublishAsOwner.ToString();
                    }
                case "publishasowneryesno":
                    {
                        return PropertyAccess.Boolean2LocalizedYesNo(PublishAsOwner, formatProvider);
                    }
                case "enablepingbacksend":
                    {
                        return EnablePingBackSend.ToString();
                    }
                case "enablepingbacksendyesno":
                    {
                        return PropertyAccess.Boolean2LocalizedYesNo(EnablePingBackSend, formatProvider);
                    }
                case "enablepingbackreceive":
                    {
                        return EnablePingBackReceive.ToString();
                    }
                case "enablepingbackreceiveyesno":
                    {
                        return PropertyAccess.Boolean2LocalizedYesNo(EnablePingBackReceive, formatProvider);
                    }
                case "autoapprovepingback":
                    {
                        return AutoApprovePingBack.ToString();
                    }
                case "autoapprovepingbackyesno":
                    {
                        return PropertyAccess.Boolean2LocalizedYesNo(AutoApprovePingBack, formatProvider);
                    }
                case "enabletrackbacksend":
                    {
                        return EnableTrackBackSend.ToString();
                    }
                case "enabletrackbacksendyesno":
                    {
                        return PropertyAccess.Boolean2LocalizedYesNo(EnableTrackBackSend, formatProvider);
                    }
                case "enabletrackbackreceive":
                    {
                        return EnableTrackBackReceive.ToString();
                    }
                case "enabletrackbackreceiveyesno":
                    {
                        return PropertyAccess.Boolean2LocalizedYesNo(EnableTrackBackReceive, formatProvider);
                    }
                case "autoapprovetrackback":
                    {
                        return AutoApproveTrackBack.ToString();
                    }
                case "autoapprovetrackbackyesno":
                    {
                        return PropertyAccess.Boolean2LocalizedYesNo(AutoApproveTrackBack, formatProvider);
                    }
                case "owneruserid":
                    {
                        return OwnerUserId.ToString(OutputFormat, formatProvider);
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
                case "displayname":
                    {
                        return PropertyAccess.FormatString(DisplayName, strFormat);
                    }
                case "email":
                    {
                        return PropertyAccess.FormatString(Email, strFormat);
                    }
                case "username":
                    {
                        return PropertyAccess.FormatString(Username, strFormat);
                    }
                case "nrposts":
                    {
                        return NrPosts.ToString(OutputFormat, formatProvider);
                    }
                case "lastpublishdate":
                    {
                        return LastPublishDate.ToString(OutputFormat, formatProvider);
                    }
                case "nrviews":
                    {
                        return NrViews.ToString(OutputFormat, formatProvider);
                    }
                case "firstpublishdate":
                    {
                        return FirstPublishDate.ToString(OutputFormat, formatProvider);
                    }
                case "altlocale":
                    {
                        return PropertyAccess.FormatString(AltLocale, strFormat);
                    }
                case "alttitle":
                    {
                        return PropertyAccess.FormatString(AltTitle, strFormat);
                    }
                case "altdescription":
                    {
                        return PropertyAccess.FormatString(AltDescription, strFormat);
                    }
                case "localizedtitle":
                    {
                        return PropertyAccess.FormatString(LocalizedTitle, strFormat);
                    }
                case "localizeddescription":
                    {
                        return PropertyAccess.FormatString(LocalizedDescription, strFormat);
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
        /// 	[pdonker]	02/16/2013  Created
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
        /// 	[pdonker]	02/16/2013  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void ReadXml(XmlReader reader)
        {
            // not implemented
        }

        internal int ImportedBlogId { get; set; } = -1;
        internal List<Posts.PostInfo> ImportedPosts { get; set; }
        internal List<string> ImportedFiles { get; set; }

        public void FromXml(XmlNode xml)
        {
            if (xml is null)
                return;

            ImportedBlogId = xml.ReadValue("BlogId", ImportedBlogId);
            Title = xml.ReadValue("Title", Title);
            TitleLocalizations = xml.ReadValue("TitleLocalizations", TitleLocalizations);
            Description = xml.ReadValue("Description", Description);
            DescriptionLocalizations = xml.ReadValue("DescriptionLocalizations", DescriptionLocalizations);
            Image = xml.ReadValue("Image", Image);
            Locale = xml.ReadValue("Locale", Locale);
            FullLocalization = xml.ReadValue("FullLocalization", FullLocalization);
            Published = xml.ReadValue("Published", Published);
            IncludeImagesInFeed = xml.ReadValue("IncludeImagesInFeed", IncludeImagesInFeed);
            IncludeAuthorInFeed = xml.ReadValue("IncludeAuthorInFeed", IncludeAuthorInFeed);
            Syndicated = xml.ReadValue("Syndicated", Syndicated);
            SyndicationEmail = xml.ReadValue("SyndicationEmail", SyndicationEmail);
            Copyright = xml.ReadValue("Copyright", Copyright);
            MustApproveGhostPosts = xml.ReadValue("MustApproveGhostPosts", MustApproveGhostPosts);
            PublishAsOwner = xml.ReadValue("PublishAsOwner", PublishAsOwner);
            EnablePingBackSend = xml.ReadValue("EnablePingBackSend", EnablePingBackSend);
            EnablePingBackReceive = xml.ReadValue("EnablePingBackReceive", EnablePingBackReceive);
            AutoApprovePingBack = xml.ReadValue("AutoApprovePingBack", AutoApprovePingBack);
            EnableTrackBackSend = xml.ReadValue("EnableTrackBackSend", EnableTrackBackSend);
            EnableTrackBackReceive = xml.ReadValue("EnableTrackBackReceive", EnableTrackBackReceive);
            AutoApproveTrackBack = xml.ReadValue("AutoApproveTrackBack", AutoApproveTrackBack);
            Username = xml.ReadValue("Username", Username);
            Email = xml.ReadValue("Email", Email);

            ImportedPosts = new List<Posts.PostInfo>();
            foreach (XmlNode xPost in xml.SelectNodes("Posts/Post"))
            {
                var post = new Posts.PostInfo();
                post.FromXml(xPost);
                ImportedPosts.Add(post);
            }

            ImportedFiles = new List<string>();
            foreach (XmlNode xFile in xml.SelectNodes("Files/File"))
                ImportedFiles.Add(xFile.InnerText);

        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// WriteXml converts the object to Xml (serializes it) and writes it using the XmlWriter passed
        /// </summary>
        /// <remarks></remarks>
        /// <param name="writer">The XmlWriter that contains the xml for the object</param>
        /// <history>
        /// 	[pdonker]	02/16/2013  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Blog");
            writer.WriteElementString("BlogId", BlogID.ToString());
            writer.WriteElementString("Title", Title);
            writer.WriteElementString("TitleLocalizations", TitleLocalizations.ToString());
            writer.WriteElementString("Description", Description);
            writer.WriteElementString("DescriptionLocalizations", DescriptionLocalizations.ToString());
            writer.WriteElementString("Image", Image);
            writer.WriteElementString("Locale", Locale);
            writer.WriteElementString("FullLocalization", FullLocalization.ToString());
            writer.WriteElementString("Published", Published.ToString());
            writer.WriteElementString("IncludeImagesInFeed", IncludeImagesInFeed.ToString());
            writer.WriteElementString("IncludeAuthorInFeed", IncludeAuthorInFeed.ToString());
            writer.WriteElementString("Syndicated", Syndicated.ToString());
            writer.WriteElementString("SyndicationEmail", SyndicationEmail);
            writer.WriteElementString("Copyright", Copyright);
            writer.WriteElementString("MustApproveGhostPosts", MustApproveGhostPosts.ToString());
            writer.WriteElementString("PublishAsOwner", PublishAsOwner.ToString());
            writer.WriteElementString("EnablePingBackSend", EnablePingBackSend.ToString());
            writer.WriteElementString("EnablePingBackReceive", EnablePingBackReceive.ToString());
            writer.WriteElementString("AutoApprovePingBack", AutoApprovePingBack.ToString());
            writer.WriteElementString("EnableTrackBackSend", EnableTrackBackSend.ToString());
            writer.WriteElementString("EnableTrackBackReceive", EnableTrackBackReceive.ToString());
            writer.WriteElementString("AutoApproveTrackBack", AutoApproveTrackBack.ToString());
            writer.WriteElementString("Username", Username);
            writer.WriteElementString("Email", Email);
            writer.WriteStartElement("Posts");
            int page = 0;
            int totalRecords = 1;
            while (page * 10 < totalRecords)
            {
                foreach (Posts.PostInfo p in Posts.PostsController.GetPostsByBlog(ModuleID, BlogID, "", -1, page, 20, "PUBLISHEDONDATE DESC", ref totalRecords).Values)
                    p.WriteXml(writer);
                page += 1;
            }
            writer.WriteEndElement(); // Posts
            writer.WriteStartElement("Files");
            // pack files
            string postDir = Globals.GetBlogDirectoryMapPath(BlogID);
            if (System.IO.Directory.Exists(postDir))
            {
                foreach (string f in System.IO.Directory.GetFiles(postDir))
                {
                    string fileName = System.IO.Path.GetFileName(f);
                    writer.WriteElementString("File", fileName);
                }
            }
            writer.WriteEndElement(); // Files
            writer.WriteEndElement(); // Blog
        }
        #endregion

    }
}