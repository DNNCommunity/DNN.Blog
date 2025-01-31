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

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Xml;
using DotNetNuke.Modules.Blog.Common;
using static DotNetNuke.Modules.Blog.Common.Globals;
using DotNetNuke.Modules.Blog.Entities.Blogs;
using DotNetNuke.Modules.Blog.Entities.Posts;
using DotNetNuke.Modules.Blog.Security;
using DotNetNuke.Modules.Blog.Services;
using DotNetNuke.Modules.Blog.Templating;
using DotNetNuke.Web.Api;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace DotNetNuke.Modules.Blog.Entities.Comments
{
  public partial class CommentsController : DnnApiController
  {

    public class CommentDTO
    {
      public int BlogId { get; set; }
      public int CommentId { get; set; }
      public int Karma { get; set; }
    }

    public class FullCommentDTO
    {
      public int BlogId { get; set; }
      public int PostId { get; set; }
      public int ParentId { get; set; }
      public string Comment { get; set; }
      public string Author { get; set; }
      public string Website { get; set; }
      public string Email { get; set; }
    }

    #region  Private Members 
    private BlogInfo Blog { get; set; } = null;
    private PostInfo Post { get; set; } = null;
    private CommentInfo Comment { get; set; } = null;
    private List<CommentInfo> AllComments { get; set; } = new List<CommentInfo>();

    private ModuleSettings _Settings;
    private ModuleSettings Settings
    {
      get
      {
        if (_Settings is null)
        {
          _Settings = ModuleSettings.GetModuleSettings(ActiveModule.ModuleID);
        }
        return _Settings;
      }
      set
      {
        _Settings = value;
      }
    }

    private ViewSettings _viewSettings;
    private ViewSettings ViewSettings
    {
      get
      {
        if (_viewSettings is null)
        {
          _viewSettings = ViewSettings.GetViewSettings(ActiveModule.TabModuleID);
        }
        return _viewSettings;
      }
      set
      {
        _viewSettings = value;
      }
    }

    private ContextSecurity _Security;
    private ContextSecurity Security
    {
      get
      {
        if (_Security is null)
        {
          _Security = new ContextSecurity(ActiveModule.ModuleID, ActiveModule.TabID, Blog, UserInfo);
        }
        return _Security;
      }
      set
      {
        _Security = value;
      }
    }
    #endregion

    #region  Service Methods 
    [HttpPost()]
    [BlogAuthorize(SecurityAccessLevel.ApproveComment)]
    [ValidateAntiForgeryToken()]
    [ActionName("Approve")]
    public HttpResponseMessage ApproveComment(CommentDTO postData)
    {
      SetContext(postData);
      if (Blog is null | Comment is null)
      {
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
      }
      ApproveComment(ActiveModule.ModuleID, Blog.BlogID, Comment);
      return Request.CreateResponse(HttpStatusCode.OK, new { Result = "success" });
    }

    [HttpPost()]
    [BlogAuthorize(SecurityAccessLevel.EditPost)]
    [ValidateAntiForgeryToken()]
    [ActionName("Delete")]
    public HttpResponseMessage DeleteComment(CommentDTO postData)
    {
      SetContext(postData);
      if (Blog is null | Comment is null)
      {
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
      }
      DeleteComment(ActiveModule.ModuleID, Blog.BlogID, Comment);
      return Request.CreateResponse(HttpStatusCode.OK, new { Result = "success" });
    }

    [HttpPost()]
    [BlogAuthorize(SecurityAccessLevel.AddComment)]
    [ValidateAntiForgeryToken()]
    [ActionName("Karma")]
    public HttpResponseMessage ReportComment(CommentDTO postData)
    {
      SetContext(postData);
      if (Blog is null | Comment is null)
      {
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
      }
      if (UserInfo.UserID < 0)
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
      int ret = Data.DataProvider.Instance().AddCommentKarma(postData.CommentId, UserInfo.UserID, postData.Karma);
      if (ret == -1)
        return Request.CreateResponse(HttpStatusCode.OK, new { Result = "exists" });
      return Request.CreateResponse(HttpStatusCode.OK, new { Result = "success" });
    }

    [HttpPost()]
    [BlogAuthorize(SecurityAccessLevel.AddComment)]
    [ValidateAntiForgeryToken()]
    [ActionName("Add")]
    public HttpResponseMessage AddComment(FullCommentDTO postData)
    {
      SetContext(postData);
      Post = PostsController.GetPost(postData.PostId, ActiveModule.ModuleID, "");
      if (Blog is null | Post is null)
      {
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
      }
      var objComment = new CommentInfo();
      objComment.ContentItemId = Post.ContentItemId;
      objComment.CreatedByUserID = UserInfo.UserID;
      objComment.ParentId = postData.ParentId;
      objComment.Comment = HttpUtility.HtmlEncode(SafeStringSimpleHtml(postData.Comment).Replace(Constants.vbCrLf, "<br />"));
      objComment.Approved = Security.CanAutoApproveComment | Security.CanApproveComment | Post.CreatedByUserID == UserInfo.UserID;
      objComment.Author = SafeString(postData.Author);
      objComment.Email = SafeString(postData.Email);
      objComment.Website = SafeString(postData.Website);
      objComment.CommentID = AddComment(Blog, Post, ref objComment);
      if (!objComment.Approved)
      {
        return Request.CreateResponse(HttpStatusCode.OK, new { Result = "successnotapproved" });
      }
      return Request.CreateResponse(HttpStatusCode.OK, new { Result = "success" });
    }

    [HttpGet()]
    [BlogAuthorize(SecurityAccessLevel.ViewModule)]
    [ValidateAntiForgeryToken()]
    [ActionName("List")]
    public HttpResponseMessage ListComments()
    {
      int BlogId = -1;
      int PostId = -1;
      Extensions.ReadValue(ref HttpContext.Current.Request.Params, "blogId", ref BlogId);
      Extensions.ReadValue(ref HttpContext.Current.Request.Params, "postId", ref PostId);
      Blog = BlogsController.GetBlog(BlogId, UserInfo.UserID, System.Threading.Thread.CurrentThread.CurrentCulture.Name);
      Post = PostsController.GetPost(PostId, ActiveModule.ModuleID, "");
      if (Blog is null | Post is null)
      {
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "error" });
      }
      if (!Security.CanViewComments)
        return Request.CreateResponse(HttpStatusCode.OK, new { Result = "" });
      var ViewSettings = Common.ViewSettings.GetViewSettings(ActiveModule.TabModuleID);
      AllComments = GetCommentsByContentItem(Post.ContentItemId, Security.CanApproveComment, UserInfo.UserID);
      var vt = new ViewTemplate();
      var tmgr = new TemplateManager(PortalSettings, ViewSettings.Template);
      vt.TemplatePath = tmgr.TemplatePath;
      vt.TemplateRelPath = tmgr.TemplateRelPath;
      vt.TemplateMapPath = tmgr.TemplateMapPath;
      vt.DefaultReplacer = new BlogTokenReplace(ActiveModule, Security, Blog, Post, Settings, ViewSettings);
      vt.StartTemplate = "CommentsTemplate.html";
      vt.GetData += TemplateGetData;
      vt.DataBind();
      return Request.CreateResponse(HttpStatusCode.OK, new { Result = vt.GetContentsAsString() });
    }

    [HttpPost()]
    [AllowAnonymous()]
    [ActionName("Pingback")]
    public HttpResponseMessage Pingback()
    {

      int BlogId = -1;
      int PostId = -1;
      Extensions.ReadValue(ref HttpContext.Current.Request.Params, "blogId", ref BlogId);
      Extensions.ReadValue(ref HttpContext.Current.Request.Params, "postId", ref PostId);
      Blog = BlogsController.GetBlog(BlogId, UserInfo.UserID, System.Threading.Thread.CurrentThread.CurrentCulture.Name);
      if (!Blog.EnablePingBackReceive)
      {
        return Request.CreateResponse(HttpStatusCode.NotFound, new { Result = "This blog does not accept pingbacks" });
      }
      Post = PostsController.GetPost(PostId, ActiveModule.ModuleID, "");
      if (Post is null)
      {
        return PingBackError(32, "The specified target URI does not exist.");
      }

      var doc = RetrieveXmlDocument(HttpContext.Current);
      var list = doc.SelectNodes("methodCall/params/param/value/string") ?? doc.SelectNodes("methodCall/params/param/value");

      if (list is null)
      {
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "Cannot parse the request" });
      }

      string sourceUrl = SafeString(list[0].InnerText.Trim());
      string targetUrl = SafeString(list[1].InnerText.Trim());

      bool containsHtml = false;
      bool sourceHasLink = false;
      string title = sourceUrl;

      try
      {
        CheckSourcePage(sourceUrl, targetUrl, ref sourceHasLink, ref title);
      }
      catch (Exception ex)
      {
      }

      if (!IsFirstPingBack(Post, sourceUrl))
      {
        return PingBackError(48, "The pingback has already been registered.");
      }

      if (!sourceHasLink)
      {
        return PingBackError(17, "The source URI does not contain a link to the target URI, and so cannot be used as a source.");
      }

      if (containsHtml)
      {
        // spam
        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Result = "Cannot parse the request" });
      }
      else
      {
        var objComment = new CommentInfo() { ContentItemId = Post.ContentItemId, Author = GetDomain(sourceUrl), Website = sourceUrl };
        string comment = string.Format(DotNetNuke.Services.Localization.Localization.GetString("PingbackComment", SharedResourceFileName), objComment.Author, sourceUrl, title);
        objComment.Comment = HttpUtility.HtmlEncode(comment);
        objComment.Approved = Blog.AutoApprovePingBack;
        objComment.CommentID = AddComment(Blog, Post, ref objComment);
        return PingBackSuccess();
      }

    }

    [HttpPost()]
    [AllowAnonymous()]
    [ActionName("Trackback")]
    public HttpResponseMessage Trackback()
    {

      int BlogId = -1;
      int PostId = -1;
      Extensions.ReadValue(ref HttpContext.Current.Request.Params, "blogId", ref BlogId);
      Extensions.ReadValue(ref HttpContext.Current.Request.Params, "postId", ref PostId);
      Blog = BlogsController.GetBlog(BlogId, UserInfo.UserID, System.Threading.Thread.CurrentThread.CurrentCulture.Name);
      if (!Blog.EnableTrackBackReceive)
      {
        return Request.CreateResponse(HttpStatusCode.NotFound, new { Result = "This blog does not accept trackbacks" });
      }
      Post = PostsController.GetPost(PostId, ActiveModule.ModuleID, "");
      if (Post is null)
      {
        return TrackBackResponse("The source page does not link");
      }

      string title = SafeString(HttpContext.Current.Request.Params["title"]);
      string excerpt = SafeString(HttpContext.Current.Request.Params["excerpt"]);
      string blogName = SafeString(HttpContext.Current.Request.Params["blog_name"]);
      string sourceUrl = string.Empty;
      if (HttpContext.Current.Request.Params["url"] is not null)
      {
        sourceUrl = SafeString(HttpContext.Current.Request.Params["url"].Split(',')[0], DotNetNuke.Security.PortalSecurity.FilterFlag.NoSQL & DotNetNuke.Security.PortalSecurity.FilterFlag.NoScripting);
      }

      bool sourceHasLink = false;

      try
      {
        CheckSourcePage(sourceUrl, Post.PermaLink(PortalSettings), ref sourceHasLink, ref title);
      }
      catch (Exception ex)
      {
      }

      if (!IsFirstPingBack(Post, sourceUrl))
      {
        return TrackBackResponse("Trackback already registered");
      }

      if (!sourceHasLink)
      {
        return TrackBackResponse("The source page does not link");
      }

      var objComment = new CommentInfo() { ContentItemId = Post.ContentItemId, Author = blogName, Website = sourceUrl };
      string comment = string.Format(DotNetNuke.Services.Localization.Localization.GetString("TrackbackComment", SharedResourceFileName), blogName, sourceUrl, title);
      objComment.Comment = HttpUtility.HtmlEncode(comment);
      objComment.Approved = Blog.AutoApproveTrackBack;
      objComment.CommentID = AddComment(Blog, Post, ref objComment);
      return TrackBackResponse();

    }
    #endregion

    #region  Private Methods 
    private void SetContext(CommentDTO data)
    {
      Blog = BlogsController.GetBlog(data.BlogId, UserInfo.UserID, System.Threading.Thread.CurrentThread.CurrentCulture.Name);
      Comment = GetComment(data.CommentId, UserInfo.UserID);
    }

    private void SetContext(FullCommentDTO data)
    {
      Blog = BlogsController.GetBlog(data.BlogId, UserInfo.UserID, System.Threading.Thread.CurrentThread.CurrentCulture.Name);
    }

    private void TemplateGetData(string DataSource, Dictionary<string, string> Parameters, ref List<GenericTokenReplace> Replacers, ref List<string[]> Arguments, object callingObject)
    {

      switch (DataSource.ToLower() ?? "")
      {

        case "comments":
          {

            if (callingObject is not null && callingObject is CommentInfo)
            {
              int parent = ((CommentInfo)callingObject).CommentID;
              foreach (CommentInfo c in AllComments.Where(cmt => cmt.ParentId == parent).OrderBy(cmt => cmt.CreatedOnDate))
                Replacers.Add(new BlogTokenReplace(ActiveModule, Security, Blog, Post, Settings, ViewSettings, c));
            }
            else
            {
              foreach (CommentInfo c in AllComments.Where(cmt => cmt.ParentId == -1).OrderBy(cmt => cmt.CreatedOnDate))
                Replacers.Add(new BlogTokenReplace(ActiveModule, Security, Blog, Post, Settings, ViewSettings, c));
            }

            break;
          }

      }

    }

    private static XmlDocument RetrieveXmlDocument(HttpContext context)
    {
      string xml = ParseRequest(context);
      if (!xml.Contains("<methodName>pingback.ping</methodName>"))
      {
        context.Response.StatusCode = 404;
        context.Response.End();
      }
      var doc = new XmlDocument();
      doc.LoadXml(xml);
      return doc;
    }

    private static string ParseRequest(HttpContext context)
    {
      var buffer = new byte[(int)(context.Request.InputStream.Length - 1L) + 1];
      context.Request.InputStream.Read(buffer, 0, buffer.Length);
      return Encoding.Default.GetString(buffer);
    }

    private static bool IsFirstPingBack(PostInfo post, string sourceUrl)
    {
      foreach (CommentInfo c in GetCommentsByContentItem(post.ContentItemId, true, -1))
      {
        if (c.Website.ToString().Equals(sourceUrl, StringComparison.OrdinalIgnoreCase))
          return false;
      }
      return true;
    }

    private static HttpResponseMessage TrackBackResponse()
    {
      return TrackBackResponse("0");
    }
    private static HttpResponseMessage TrackBackResponse(string status)
    {
      string reply = string.Format("<?xml version=\"1.0\" encoding=\"iso-8859-1\"?><response><error>{0}</error></response>", status);
      var res = new HttpResponseMessage(HttpStatusCode.OK);
      res.Content = new StringContent(reply, Encoding.UTF8, "application/xml");
      return res;
    }

    private static HttpResponseMessage PingBackSuccess()
    {
      string Success = "<methodResponse><params><param><value><string>Thanks!</string></value></param></params></methodResponse>";
      var res = new HttpResponseMessage(HttpStatusCode.OK);
      res.Content = new StringContent(Success, Encoding.UTF8, "application/xml");
      return res;
    }

    private static HttpResponseMessage PingBackError(int code, string message)
    {
      var sb = new StringBuilder();
      sb.Append("<?xml version=\"1.0\"?>");
      sb.Append("<methodResponse>");
      sb.Append("<fault>");
      sb.Append("<value>");
      sb.Append("<struct>");
      sb.Append("<member>");
      sb.Append("<name>faultCode</name>");
      sb.AppendFormat("<value><int>{0}</int></value>", code);
      sb.Append("</member>");
      sb.Append("<member>");
      sb.Append("<name>faultString</name>");
      sb.AppendFormat("<value><string>{0}</string></value>", message);
      sb.Append("</member>");
      sb.Append("</struct>");
      sb.Append("</value>");
      sb.Append("</fault>");
      sb.Append("</methodResponse>");
      var res = new HttpResponseMessage(HttpStatusCode.OK);
      res.Content = new StringContent(sb.ToString(), Encoding.UTF8, "application/xml");
      return res;
    }

    private static string GetDomain(string sourceUrl)
    {
      int start = sourceUrl.IndexOf("://") + 3;
      int stop = sourceUrl.IndexOf("/", start);
      return sourceUrl.Substring(start, stop - start).Replace("www.", string.Empty);
    }

    private static void CheckSourcePage(string sourceUrl, string targetUrl, ref bool sourceContainsLink, ref string title)
    {

      var remoteFile = new WebPage(new Uri(sourceUrl));
      string html = remoteFile.GetFileAsString();
      var RegexTitle = new Regex(@"(?<=<title.*>)([\s\S]*)(?=</title>)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
      var titleMatch = RegexTitle.Match(html);
      if (titleMatch.Success)
        title = SafeString(titleMatch.Value.Trim(Conversions.ToChar(Constants.vbCrLf)).Trim());
      html = html.ToUpperInvariant();
      targetUrl = targetUrl.ToUpperInvariant();
      sourceContainsLink = html.Contains("HREF=\"" + targetUrl + "\"") || html.Contains("HREF='" + targetUrl + "'");

    }
    #endregion

  }
}