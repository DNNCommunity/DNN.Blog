using System;
using System.Data;
using DotNetNuke.Framework.Providers;
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

using Microsoft.ApplicationBlocks.Data;
using Microsoft.VisualBasic.CompilerServices;

namespace DotNetNuke.Modules.Blog.Data
{

  public partial class SqlDataProvider : DataProvider
  {

    #region  Private Members 

    private const string ProviderType = "data";
    private const string ModuleQualifier = "Blog_";

    private ProviderConfiguration _providerConfiguration = ProviderConfiguration.GetProviderConfiguration(ProviderType);
    private string _connectionString;
    private string _providerPath;
    private string _objectQualifier;
    private string _databaseOwner;

    #endregion

    #region  Constructors 

    public SqlDataProvider()
    {

      // Read the configuration specific information for this provider
      Provider objProvider = (Provider)_providerConfiguration.Providers[_providerConfiguration.DefaultProvider];

      // Get Connection string from web.config
      _connectionString = DotNetNuke.Common.Utilities.Config.GetConnectionString();

      if (string.IsNullOrEmpty(_connectionString))
      {
        // Use connection string specified in provider
        _connectionString = objProvider.Attributes["connectionString"];
      }

      _providerPath = objProvider.Attributes["providerPath"];

      _objectQualifier = objProvider.Attributes["objectQualifier"];
      if (!string.IsNullOrEmpty(_objectQualifier) & _objectQualifier.EndsWith("_") == false)
      {
        _objectQualifier += "_";
      }

      _databaseOwner = objProvider.Attributes["databaseOwner"];
      if (!string.IsNullOrEmpty(_databaseOwner) & _databaseOwner.EndsWith(".") == false)
      {
        _databaseOwner += ".";
      }

    }

    #endregion

    #region  Properties 

    public string ConnectionString
    {
      get
      {
        return _connectionString;
      }
    }

    public string ProviderPath
    {
      get
      {
        return _providerPath;
      }
    }

    public string ObjectQualifier
    {
      get
      {
        return _objectQualifier;
      }
    }

    public string DatabaseOwner
    {
      get
      {
        return _databaseOwner;
      }
    }

    #endregion

    #region  General Methods 
    public override object GetNull(object Field)
    {
      return DotNetNuke.Common.Utilities.Null.GetNull(Field, DBNull.Value);
    }
    #endregion


    #region  BlogPermission Methods 

    public override IDataReader GetBlogPermission(int blogId, int permissionId, int roleId, int userId)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetBlogPermission", blogId, permissionId, roleId, userId);
    }

    public override void AddBlogPermission(bool allowAccess, int blogId, DateTime expires, int permissionId, int roleId, int userId)
    {
      SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "AddBlogPermission", allowAccess, blogId, GetNull(expires), permissionId, roleId, userId);
    }

    public override void UpdateBlogPermission(bool allowAccess, int blogId, DateTime expires, int permissionId, int roleId, int userId)
    {
      SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "UpdateBlogPermission", allowAccess, blogId, GetNull(expires), permissionId, roleId, userId);
    }

    public override void DeleteBlogPermission(int blogId, int permissionId, int roleId, int userId)
    {
      SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "DeleteBlogPermission", blogId, permissionId, roleId, userId);
    }

    #endregion

    #region  Blog Methods 

    public override int AddBlog(bool autoApprovePingBack, int moduleID, bool autoApproveTrackBack, string copyright, string description, bool enablePingBackReceive, bool enablePingBackSend, bool enableTrackBackReceive, bool enableTrackBackSend, bool fullLocalization, string image, bool includeAuthorInFeed, bool includeImagesInFeed, string locale, bool mustApproveGhostPosts, int ownerUserId, bool publishAsOwner, bool published, bool syndicated, string syndicationEmail, string title, int createdByUser)
    {
      return Conversions.ToInteger(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "AddBlog", autoApprovePingBack, moduleID, autoApproveTrackBack, GetNull(copyright), GetNull(description), enablePingBackReceive, enablePingBackSend, enableTrackBackReceive, enableTrackBackSend, fullLocalization, GetNull(image), includeAuthorInFeed, includeImagesInFeed, locale, mustApproveGhostPosts, ownerUserId, publishAsOwner, published, syndicated, GetNull(syndicationEmail), title, createdByUser));
    }

    public override void UpdateBlog(bool autoApprovePingBack, int moduleID, bool autoApproveTrackBack, int blogID, string copyright, string description, bool enablePingBackReceive, bool enablePingBackSend, bool enableTrackBackReceive, bool enableTrackBackSend, bool fullLocalization, string image, bool includeAuthorInFeed, bool includeImagesInFeed, string locale, bool mustApproveGhostPosts, int ownerUserId, bool publishAsOwner, bool published, bool syndicated, string syndicationEmail, string title, int updatedByUser)
    {
      SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "UpdateBlog", autoApprovePingBack, moduleID, autoApproveTrackBack, blogID, GetNull(copyright), GetNull(description), enablePingBackReceive, enablePingBackSend, enableTrackBackReceive, enableTrackBackSend, fullLocalization, GetNull(image), includeAuthorInFeed, includeImagesInFeed, locale, mustApproveGhostPosts, ownerUserId, publishAsOwner, published, syndicated, GetNull(syndicationEmail), title, updatedByUser);
    }

    public override void DeleteBlog(int blogID)
    {
      SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "DeleteBlog", blogID);
    }

    #endregion

    #region  Comment Methods 

    public override int AddComment(bool approved, string author, string comment, int contentItemId, string email, int parentId, string website, int createdByUser)
    {
      return Conversions.ToInteger(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "AddComment", approved, GetNull(author), comment, contentItemId, GetNull(email), GetNull(parentId), GetNull(website), createdByUser));
    }

    public override void UpdateComment(bool approved, string author, string comment, int commentID, int contentItemId, string email, int parentId, string website, int updatedByUser)
    {
      SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "UpdateComment", approved, GetNull(author), comment, commentID, contentItemId, GetNull(email), GetNull(parentId), GetNull(website), updatedByUser);
    }

    public override void DeleteComment(int commentID)
    {
      SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "DeleteComment", commentID);
    }

    #endregion

    #region  LegacyUrl Methods 
    public override void AddLegacyUrl(int contentItemId, int entryId, string url)
    {
      SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "AddLegacyUrl", contentItemId, GetNull(entryId), url);
    }
    #endregion

    #region  Post Methods 

    public override IDataReader GetPost(int contentItemId, int moduleId, string locale)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetPost", contentItemId, moduleId, locale);
    }

    public override int AddPost(bool allowComments, int blogID, string content, string copyright, bool displayCopyright, string image, string locale, bool published, DateTime publishedOnDate, string summary, string termIds, string title, int viewCount, int createdByUser)
    {
      return Conversions.ToInteger(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "AddPost", allowComments, blogID, content, copyright, displayCopyright, image, GetNull(locale), published, publishedOnDate, summary, termIds, title, viewCount, createdByUser));
    }

    public override void UpdatePost(bool allowComments, int blogID, string content, int contentItemId, string copyright, bool displayCopyright, string image, string locale, bool published, DateTime publishedOnDate, string summary, string termIds, string title, int viewCount, int updatedByUser)
    {
      SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "UpdatePost", allowComments, blogID, content, contentItemId, copyright, displayCopyright, image, GetNull(locale), published, publishedOnDate, summary, termIds, title, viewCount, updatedByUser);
    }

    public override void DeletePost(int contentItemId)
    {
      SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "DeletePost", contentItemId);
    }

    #endregion


  }

}