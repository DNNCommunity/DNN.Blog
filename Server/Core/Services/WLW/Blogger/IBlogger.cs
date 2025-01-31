
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2015
// by DotNetNuke Corporation
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

using CookComputing.XmlRpc;

namespace DotNetNuke.Modules.Blog.Services.WLW.Blogger
{

  /// <summary>
 /// Interface for Blogger Methods that WLW uses
 /// </summary>
 /// <remarks></remarks>
 /// <history>
 /// 		[pdonker]	12/14/2009	created
 /// </history>
  public interface IBlogger
  {

    [XmlRpcMethod("blogger.deletePost", Description = "Deletes a post.")]
    bool deletePost(string appKey, string postid, string username, string password, [XmlRpcParameter(Description = "Where applicable, this specifies whether the blog " + "should be republished after the post has been deleted.")] bool publish);

    [XmlRpcMethod("blogger.getUsersBlogs", Description = "Returns information on all the blogs a given user " + "is a member.")]
    BlogInfoStruct[] getUsersBlogs(string appKey, string username, string password);

  }

  public struct BlogInfoStruct
  {
    public string url;
    public string blogName;
    public string blogid;
  }
}