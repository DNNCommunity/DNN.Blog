﻿using System.Data;
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

namespace DotNetNuke.Modules.Blog.Core.Data
{

  public partial class SqlDataProvider
  {

    #region  BlogPermission Methods 
    public override IDataReader GetBlogPermissionsByBlog(int blogID, int StartRowIndex, int MaximumRows, string OrderBy)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetBlogPermissionsByBlog", blogID, StartRowIndex, MaximumRows, OrderBy.ToUpper());
    }

    #endregion

    #region  Blog Methods 

    public override IDataReader GetBlogsByCreatedByUser(int userID, int StartRowIndex, int MaximumRows, string OrderBy)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetBlogsByCreatedByUser", userID, StartRowIndex, MaximumRows, OrderBy.ToUpper());
    }

    #endregion

    #region  Comment Methods 
    #endregion

    #region  Post Methods 
    public override IDataReader GetPostsByBlog(int blogID, string displayLocale, int pageIndex, int pageSize, string orderBy)
    {
      return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetPostsByBlog", blogID, displayLocale, pageIndex, pageSize, orderBy);
    }
    #endregion

  }

}