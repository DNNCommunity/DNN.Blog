// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
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
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Modules.Blog.Entities.Blogs;
using DotNetNuke.Modules.Blog.Entities.Comments;
using DotNetNuke.Modules.Blog.Entities.Posts;
using DotNetNuke.Modules.Blog.Services;
using DotNetNuke.Services.Social.Notifications;
using DotNetNuke.Web.Api;

namespace DotNetNuke.Modules.Blog.Integration.Services
{

  public class NotificationServiceController : DnnApiController
  {

    public class NotificationDTO
    {
      public int NotificationId { get; set; }
    }

    #region  Private Members 

    private int BlogModuleId { get; set; } = -1;
    private int BlogId { get; set; } = -1;
    private BlogInfo Blog { get; set; } = null;
    private int ContentItemId { get; set; } = -1;
    private PostInfo Post { get; set; } = null;
    private int CommentId { get; set; } = -1;
    private CommentInfo Comment { get; set; } = null;

    #endregion

    #region  Service Methods 
    [HttpPost()]
    [BlogAuthorize(SecurityAccessLevel.ApprovePost)]
    [ValidateAntiForgeryToken()]
    public HttpResponseMessage ApprovePost(NotificationDTO postData)
    {
      var notify = Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.GetNotification(postData.NotificationId);
      ParsePublishKey(notify.Context);
      if (Blog is null | Post is null)
      {
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
      }
      Post.Published = true;
      PostsController.UpdatePost(Post, UserInfo.UserID);
      Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.DeleteNotification(postData.NotificationId);
      return Request.CreateResponse(HttpStatusCode.OK, new { Result = "success" });
    }

    [HttpPost()]
    [BlogAuthorize(SecurityAccessLevel.EditPost)]
    [ValidateAntiForgeryToken()]
    public HttpResponseMessage DeletePost(NotificationDTO postData)
    {
      var notify = Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.GetNotification(postData.NotificationId);
      ParsePublishKey(notify.Context);
      if (Blog is null | Post is null)
      {
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
      }
      PostsController.DeletePost(ContentItemId);
      Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.DeleteNotification(postData.NotificationId);
      return Request.CreateResponse(HttpStatusCode.OK, new { Result = "success" });
    }

    [HttpPost()]
    [BlogAuthorize(SecurityAccessLevel.ApproveComment)]
    [ValidateAntiForgeryToken()]
    public HttpResponseMessage ApproveComment(NotificationDTO postData)
    {
      var notify = Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.GetNotification(postData.NotificationId);
      ParseCommentKey(notify.Context);
      if (Blog is null | Post is null | Comment is null)
      {
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
      }
      CommentsController.ApproveComment(BlogModuleId, BlogId, Comment);
      Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.DeleteNotification(postData.NotificationId);
      return Request.CreateResponse(HttpStatusCode.OK, new { Result = "success" });
    }

    [HttpPost()]
    [BlogAuthorize(SecurityAccessLevel.ApproveComment)]
    [ValidateAntiForgeryToken()]
    public HttpResponseMessage DeleteComment(NotificationDTO postData)
    {
      var notify = Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.GetNotification(postData.NotificationId);
      ParseCommentKey(notify.Context);
      if (Blog is null | Post is null | Comment is null)
      {
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
      }
      CommentsController.DeleteComment(BlogModuleId, BlogId, Comment);
      Framework.ServiceLocator<INotificationsController, NotificationsController>.Instance.DeleteNotification(postData.NotificationId);
      return Request.CreateResponse(HttpStatusCode.OK, new { Result = "success" });
    }
    #endregion

    #region  Private Methods 
    private void ParsePublishKey(string key)
    {
      var nKey = new NotificationKey(key);
      BlogModuleId = nKey.ModuleId;
      BlogId = nKey.BlogId;
      ContentItemId = nKey.ContentItemId;
      Blog = BlogsController.GetBlog(BlogId, UserInfo.UserID, System.Threading.Thread.CurrentThread.CurrentCulture.Name);
      Post = PostsController.GetPost(ContentItemId, BlogModuleId, System.Threading.Thread.CurrentThread.CurrentCulture.Name);
    }

    private void ParseCommentKey(string key)
    {
      var nKey = new NotificationKey(key);
      BlogModuleId = nKey.ModuleId;
      BlogId = nKey.BlogId;
      ContentItemId = nKey.ContentItemId;
      CommentId = nKey.CommentId;
      Blog = BlogsController.GetBlog(BlogId, UserInfo.UserID, System.Threading.Thread.CurrentThread.CurrentCulture.Name);
      Post = PostsController.GetPost(ContentItemId, BlogModuleId, System.Threading.Thread.CurrentThread.CurrentCulture.Name);
      Comment = CommentsController.GetComment(CommentId, UserInfo.UserID);
    }
    #endregion

  }

}