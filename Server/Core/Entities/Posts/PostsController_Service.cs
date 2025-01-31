
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

using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Modules.Blog.Entities.Blogs;
using DotNetNuke.Modules.Blog.Services;
using DotNetNuke.Web.Api;

namespace DotNetNuke.Modules.Blog.Entities.Posts
{
  public partial class PostsController : DnnApiController
  {

    public class PostDTO
    {
      public int BlogId { get; set; }
      public int PostId { get; set; }
    }

    #region  Private Members 
    private BlogInfo Blog { get; set; } = null;
    private PostInfo Post { get; set; } = null;
    #endregion

    #region  Service Methods 
    [HttpPost()]
    [BlogAuthorize(SecurityAccessLevel.ApprovePost)]
    [ValidateAntiForgeryToken()]
    [ActionName("Approve")]
    public HttpResponseMessage ApprovePost(PostDTO postData)
    {
      SetContext(postData);
      if (Blog is null | Post is null)
      {
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
      }
      PublishPost(Post, true, UserInfo.UserID);
      return Request.CreateResponse(HttpStatusCode.OK, new { Result = "success" });
    }

    [HttpPost()]
    [BlogAuthorize(SecurityAccessLevel.EditPost)]
    [ValidateAntiForgeryToken()]
    [ActionName("Delete")]
    public HttpResponseMessage DeletePost(PostDTO postData)
    {
      SetContext(postData);
      if (Blog is null | Post is null)
      {
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
      }
      DeletePost(Post);
      return Request.CreateResponse(HttpStatusCode.OK, new { Result = "success" });
    }

    [HttpPost()]
    [AllowAnonymous()]
    [ValidateAntiForgeryToken()]
    [ActionName("View")]
    public HttpResponseMessage ViewPost(PostDTO postData)
    {
      SetContext(postData);
      if (Blog is null | Post is null)
      {
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
      }
      Data.DataProvider.Instance().AddPostView(postData.PostId);
      return Request.CreateResponse(HttpStatusCode.OK, new { Result = "success" });
    }
    #endregion

    #region  Private Methods 
    private void SetContext(PostDTO data)
    {
      Blog = BlogsController.GetBlog(data.BlogId, UserInfo.UserID, System.Threading.Thread.CurrentThread.CurrentCulture.Name);
      Post = GetPost(data.PostId, ActiveModule.ModuleID, System.Threading.Thread.CurrentThread.CurrentCulture.Name);
    }
    #endregion

  }
}