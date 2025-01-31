using System;
using System.IO.Compression;
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
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Xml;
using DotNetNuke.Modules.Blog.BlogML.Xml;
using DotNetNuke.Modules.Blog.Common;
using static DotNetNuke.Modules.Blog.Common.Globals;
using DotNetNuke.Modules.Blog.Entities.Comments;
using DotNetNuke.Modules.Blog.Entities.Posts;
using DotNetNuke.Modules.Blog.Entities.Terms;
using DotNetNuke.Modules.Blog.Services;
using DotNetNuke.Web.Api;

namespace DotNetNuke.Modules.Blog.Entities.Blogs
{
  public partial class BlogsController : DnnApiController
  {

    public class BlogDTO
    {
      public int BlogId { get; set; }
    }

    #region  Private Members 
    private BlogInfo Blog { get; set; } = null;
    private ModuleSettings Settings { get; set; } = null;
    #endregion

    #region  Service Methods 
    [HttpPost()]
    [BlogAuthorize(SecurityAccessLevel.Owner | SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken()]
    [ActionName("Export")]
    public HttpResponseMessage ExportBlog(BlogDTO postData)
    {
      SetContext(postData);

      var saveDir = new System.IO.DirectoryInfo(GetBlogDirectoryMapPath(Blog.BlogID));
      if (!saveDir.Exists)
        saveDir.Create();
      RemoveOldTimeStampedFiles(saveDir);

      var newBlogML = new BlogMLBlog();
      newBlogML.Title = Blog.Title;
      newBlogML.SubTitle = Blog.Description;
      newBlogML.Authors.Add(new BlogMLAuthor() { Title = Blog.DisplayName });
      newBlogML.DateCreated = Blog.CreatedOnDate;
      AddCategories(ref newBlogML);
      AddPosts(ref newBlogML);
      string blogMLFile = DateTime.Now.ToString("yyyy-MM-dd") + "-" + Guid.NewGuid().ToString("D");

      using (var objZipOutputStream = new ZipArchive(System.IO.File.Create(GetBlogDirectoryMapPath(Blog.BlogID) + blogMLFile + ".zip"), ZipArchiveMode.Create))
      {
        var objZipEntry = objZipOutputStream.CreateEntry(blogMLFile + ".xml");
        using (var stream = XmlWriter.Create(objZipEntry.Open()))
        {
          BlogMLSerializer.Serialize(stream, newBlogML);
          stream.Flush();
        }
      }

      return Request.CreateResponse(HttpStatusCode.OK, new { Result = GetBlogDirectoryPath(Blog.BlogID) + blogMLFile + ".zip" });
    }
    #endregion

    #region  Private Methods 
    private void SetContext(BlogDTO data)
    {
      Blog = GetBlog(data.BlogId, UserInfo.UserID, System.Threading.Thread.CurrentThread.CurrentCulture.Name);
      Settings = ModuleSettings.GetModuleSettings(ActiveModule.ModuleID);
    }

    private void AddCategories(ref BlogMLBlog TargetBlogML)
    {
      if (Settings.VocabularyId > -1)
      {
        foreach (TermInfo c in TermsController.GetTermsByVocabulary(ActiveModule.ModuleID, Settings.VocabularyId, System.Threading.Thread.CurrentThread.CurrentCulture.Name).Values)
        {
          var categoryML = new BlogMLCategory();
          categoryML.Approved = true;
          categoryML.DateCreated = c.CreatedOnDate;
          categoryML.Description = "";
          categoryML.Title = c.Name;
          categoryML.ID = c.TermId.ToString();
          categoryML.ParentRef = c.ParentTermId.ToStringOrZero();
          TargetBlogML.Categories.Add(categoryML);
        }
      }
    }

    private void AddPosts(ref BlogMLBlog TargetBlogML)
    {
      int totalRecs = 0;
      int handledRecs = 0;
      int page = 0;
      do
      {
        foreach (PostInfo post in PostsController.GetPostsByBlog(ActiveModule.ModuleID, Blog.BlogID, System.Threading.Thread.CurrentThread.CurrentCulture.Name, -1, page, 10, "PUBLISHEDONDATE", ref totalRecs).Values)
        {
          handledRecs += 1;
          TargetBlogML.Posts.Add(ConvertPost(post));
        }
        page += 1;
      }
      while (totalRecs > handledRecs);
    }

    private BlogMLPost ConvertPost(PostInfo post)
    {

      var newPostML = new BlogMLPost();
      newPostML.Approved = post.Published;
      newPostML.Title = post.Title;
      newPostML.Content = BlogMLContent.Create(HttpUtility.HtmlDecode(post.Content), BlogML.ContentTypes.Html);
      foreach (TermInfo t in post.PostCategories)
        newPostML.Categories.Add(new BlogMLCategoryReference() { Ref = t.TermId.ToString() });
      newPostML.Authors.Add(post.DisplayName);
      newPostML.PostType = BlogML.BlogPostTypes.Normal;
      newPostML.DateCreated = post.PublishedOnDate;
      if (!string.IsNullOrEmpty(post.Summary))
      {
        if (Settings.SummaryModel == SummaryType.PlainTextIndependent)
        {
          newPostML.Excerpt = BlogMLContent.Create(post.Summary, BlogML.ContentTypes.Text);
        }
        else
        {
          newPostML.Excerpt = BlogMLContent.Create(HttpUtility.HtmlDecode(post.Summary), BlogML.ContentTypes.Html);
        }
        newPostML.HasExcerpt = true;
      }
      else
      {
        newPostML.HasExcerpt = false;
      }
      newPostML.ID = post.ContentItemId.ToString();
      newPostML.PostUrl = post.PermaLink();
      newPostML.Image = post.Image;
      newPostML.AllowComments = post.AllowComments;
      newPostML.DisplayCopyright = post.DisplayCopyright;
      newPostML.Copyright = post.Copyright;
      newPostML.Locale = post.Locale;

      // pack files
      string postDir = GetPostDirectoryMapPath(post.BlogID, post.ContentItemId);
      if (System.IO.Directory.Exists(postDir))
      {
        foreach (string f in System.IO.Directory.GetFiles(postDir))
        {
          string fileName = System.IO.Path.GetFileName(f);
          string regexPattern = @"&quot;([^\s]*)\/" + fileName + "&quot;";
          var options = RegexOptions.Singleline | RegexOptions.IgnoreCase;
          if (newPostML.HasExcerpt)
          {
            newPostML.Excerpt.Text = Regex.Replace(newPostML.Excerpt.Text, regexPattern, "&quot;" + fileName + "&quot;", options);
          }
          newPostML.Content.Text = Regex.Replace(newPostML.Content.Text, regexPattern, "&quot;" + fileName + "&quot;", options);
          var att = new BlogMLAttachment() { Embedded = true, Path = fileName };
          using (var fs = new System.IO.FileStream(f, System.IO.FileMode.Open))
          {
            var fileData = new byte[(int)(fs.Length - 1L) + 1];
            if (fs.Length > 0L)
            {
              fs.Read(fileData, 0, (int)fs.Length);
              att.Data = fileData;
              att.Embedded = true;
            }
            else
            {
              // Empty File
            }
          }
          newPostML.Attachments.Add(att);
        }
      }

      foreach (CommentInfo comment in CommentsController.GetCommentsByContentItem(post.ContentItemId, false, UserInfo.UserID))
      {
        var newComment = new BlogMLComment();
        newComment.Approved = comment.Approved;
        newComment.Content = new BlogMLContent();
        newComment.Content.Text = comment.Comment;
        newComment.DateCreated = comment.CreatedOnDate;
        newComment.Title = "";
        newComment.UserEMail = comment.Email;
        newComment.UserName = comment.Username;
        newComment.UserUrl = comment.Website;
        newPostML.Comments.Add(newComment);
      }

      return newPostML;

    }
    #endregion

  }
}