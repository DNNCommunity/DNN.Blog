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


namespace DotNetNuke.Modules.Blog.Data
{

  public abstract partial class DataProvider
  {

    #region  Shared/Static Methods 

    // singleton reference to the instantiated object 
    private static DataProvider objProvider = null;

    // constructor
    static DataProvider()
    {
      CreateProvider();
    }

    // dynamically create provider
    private static void CreateProvider()
    {
      objProvider = (DataProvider)Framework.Reflection.CreateObject("data", "DotNetNuke.Modules.Blog.Data", "");
    }

    // return the provider
    public static new DataProvider Instance()
    {
      return objProvider;
    }

    #endregion

    #region  General Methods 
    public abstract object GetNull(object Field);
    #endregion

    #region  BlogPermission Methods 
    public abstract IDataReader GetBlogPermission(int blogId, int permissionId, int roleId, int userId);
    public abstract void AddBlogPermission(bool allowAccess, int blogId, DateTime expires, int permissionId, int roleId, int userId);
    public abstract void UpdateBlogPermission(bool allowAccess, int blogId, DateTime expires, int permissionId, int roleId, int userId);
    public abstract void DeleteBlogPermission(int blogId, int permissionId, int roleId, int userId);
    #endregion

    #region  Blog Methods 
    public abstract int AddBlog(bool autoApprovePingBack, int moduleID, bool autoApproveTrackBack, string copyright, string description, bool enablePingBackReceive, bool enablePingBackSend, bool enableTrackBackReceive, bool enableTrackBackSend, bool fullLocalization, string image, bool includeAuthorInFeed, bool includeImagesInFeed, string locale, bool mustApproveGhostPosts, int ownerUserId, bool publishAsOwner, bool published, bool syndicated, string syndicationEmail, string title, int createdByUser);
    public abstract void UpdateBlog(bool autoApprovePingBack, int moduleID, bool autoApproveTrackBack, int blogID, string copyright, string description, bool enablePingBackReceive, bool enablePingBackSend, bool enableTrackBackReceive, bool enableTrackBackSend, bool fullLocalization, string image, bool includeAuthorInFeed, bool includeImagesInFeed, string locale, bool mustApproveGhostPosts, int ownerUserId, bool publishAsOwner, bool published, bool syndicated, string syndicationEmail, string title, int updatedByUser);
    public abstract void DeleteBlog(int blogID);
    #endregion

    #region  Comment Methods 
    public abstract int AddComment(bool approved, string author, string comment, int contentItemId, string email, int parentId, string website, int createdByUser);
    public abstract void UpdateComment(bool approved, string author, string comment, int commentID, int contentItemId, string email, int parentId, string website, int updatedByUser);
    public abstract void DeleteComment(int commentID);
    #endregion

    #region  LegacyUrl Methods 
    public abstract void AddLegacyUrl(int contentItemId, int entryId, string url);
    #endregion

    #region  Post Methods 
    public abstract IDataReader GetPost(int contentItemId, int moduleId, string locale);
    public abstract int AddPost(bool allowComments, int blogID, string content, string copyright, bool displayCopyright, string image, string locale, bool published, DateTime publishedOnDate, string summary, string termIds, string title, int viewCount, int createdByUser);
    public abstract void UpdatePost(bool allowComments, int blogID, string content, int contentItemId, string copyright, bool displayCopyright, string image, string locale, bool published, DateTime publishedOnDate, string summary, string termIds, string title, int viewCount, int updatedByUser);
    public abstract void DeletePost(int contentItemId);
    #endregion

  }

}