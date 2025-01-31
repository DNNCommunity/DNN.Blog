using System;
using System.Collections.Specialized;
using System.Web;
using static DotNetNuke.Modules.Blog.Common.Globals;
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

using DotNetNuke.Modules.Blog.Security;
using DotNetNuke.Services.Tokens;

namespace DotNetNuke.Modules.Blog.Common
{

  public class BlogContextInfo : IPropertyAccess
  {

    #region  Private Members 
    private NameValueCollection RequestParams { get; set; }
    #endregion

    #region  Public Methods 
    public BlogContextInfo(HttpContext context, BlogModuleBase blogModule)
    {

      BlogModuleId = blogModule.ModuleId;

      // Initialize values from View Settings
      if (blogModule.ViewSettings.BlogModuleId != -1)
      {
        BlogModuleId = blogModule.ViewSettings.BlogModuleId;
        ParentModule = new DotNetNuke.Entities.Modules.ModuleController().GetModule(BlogModuleId);
      }
      BlogId = blogModule.ViewSettings.BlogId;
      Categories = blogModule.ViewSettings.Categories;
      AuthorId = blogModule.ViewSettings.AuthorId;

      Locale = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
      if (context.Request.UrlReferrer is not null)
        Referrer = context.Request.UrlReferrer.PathAndQuery;
      RequestParams = context.Request.Params;

      int argVariable = BlogId;
      Extensions.ReadValue(ref context.Request.Params, "Blog", ref argVariable);
      BlogId = argVariable;
      int argVariable1 = ContentItemId;
      Extensions.ReadValue(ref context.Request.Params, "Post", ref argVariable1);
      ContentItemId = argVariable1;
      int argVariable2 = TermId;
      Extensions.ReadValue(ref context.Request.Params, "Term", ref argVariable2);
      TermId = argVariable2;
      string argVariable3 = Categories;
      Extensions.ReadValue(ref context.Request.Params, "Categories", ref argVariable3);
      Categories = argVariable3;
      int argVariable4 = AuthorId;
      Extensions.ReadValue(ref context.Request.Params, "User", ref argVariable4);
      AuthorId = argVariable4;
      int argVariable5 = AuthorId;
      Extensions.ReadValue(ref context.Request.Params, "uid", ref argVariable5);
      AuthorId = argVariable5;
      int argVariable6 = AuthorId;
      Extensions.ReadValue(ref context.Request.Params, "UserId", ref argVariable6);
      AuthorId = argVariable6;
      int argVariable7 = AuthorId;
      Extensions.ReadValue(ref context.Request.Params, "Author", ref argVariable7);
      AuthorId = argVariable7;
      var argVariable8 = EndDate;
      Extensions.ReadValue(ref context.Request.Params, "end", ref argVariable8);
      EndDate = argVariable8;
      string argVariable9 = SearchString;
      Extensions.ReadValue(ref context.Request.Params, "search", ref argVariable9);
      SearchString = argVariable9;
      bool argVariable10 = SearchTitle;
      Extensions.ReadValue(ref context.Request.Params, "t", ref argVariable10);
      SearchTitle = argVariable10;
      bool argVariable11 = SearchContents;
      Extensions.ReadValue(ref context.Request.Params, "c", ref argVariable11);
      SearchContents = argVariable11;
      bool argVariable12 = SearchUnpublished;
      Extensions.ReadValue(ref context.Request.Params, "u", ref argVariable12);
      SearchUnpublished = argVariable12;
      int argVariable13 = LegacyEntryId;
      Extensions.ReadValue(ref context.Request.Params, "EntryId", ref argVariable13);
      LegacyEntryId = argVariable13;
      if (ContentItemId > -1)
        Post = Entities.Posts.PostsController.GetPost(ContentItemId, BlogModuleId, Locale);
      if (BlogId > -1 & Post is not null && Post.BlogID != BlogId)
        Post = null; // double check in case someone is hacking to retrieve an Post from another blog
      if (BlogId == -1 & Post is not null)
        BlogId = Post.BlogID;
      if (BlogId > -1)
        Blog = Entities.Blogs.BlogsController.GetBlog(BlogId, blogModule.UserInfo.UserID, Locale);
      if (BlogId > -1)
        BlogMapPath = GetBlogDirectoryMapPath(BlogId);
      if (!string.IsNullOrEmpty(BlogMapPath) && !System.IO.Directory.Exists(BlogMapPath))
        System.IO.Directory.CreateDirectory(BlogMapPath);
      if (ContentItemId > -1)
        PostMapPath = GetPostDirectoryMapPath(BlogId, ContentItemId);
      if (!string.IsNullOrEmpty(PostMapPath) && !System.IO.Directory.Exists(PostMapPath))
        System.IO.Directory.CreateDirectory(PostMapPath);
      if (TermId > -1)
        Term = Entities.Terms.TermsController.GetTerm(TermId, BlogModuleId, Locale);
      if (AuthorId > -1)
        Author = DotNetNuke.Entities.Users.UserController.GetUserById(blogModule.PortalId, AuthorId);
      if (context.Request.UserAgent is not null)
      {
        WLWRequest = context.Request.UserAgent.IndexOf("Windows Live Writer") > -1;
      }
      Security = new ContextSecurity(BlogModuleId, blogModule.TabId, Blog, blogModule.UserInfo);
      if (EndDate < DateTime.Now.AddDays(-1))
      {
        EndDate = EndDate.Date.AddDays(1d).AddMinutes(-1);
        EndDateOrNow = EndDate;
      }
      else if (Security.CanAddPost)
      {
        EndDate = default;
      }
      else
      {
        EndDate = DateTime.Now.ToUniversalTime(); // security measure to stop people prying into future posts
        EndDateOrNow = EndDate;
      }

      // security
      bool isStylePostRequest = false;
      if (Post is not null && !(Post.Published | Security.CanEditThisPost(Post)) && !Security.IsEditor)
      {
        if (Post.Title.Contains("3bfe001a-32de-4114-a6b4-4005b770f6d7") & WLWRequest)
        {
          isStylePostRequest = true;
        }
        else
        {
          Post = null;
          ContentItemId = -1;
        }
      }
      if (Blog is not null && !Blog.Published && !Security.IsOwner && !Security.UserIsAdmin && !isStylePostRequest)
      {
        Blog = null;
        BlogId = -1;
      }

      // set urls for use in module
      if (ParentModule is null)
      {
        ModuleUrls = new ModuleUrls(blogModule.TabId, BlogId, ContentItemId, TermId, AuthorId);
      }
      else
      {
        ModuleUrls = new ModuleUrls(blogModule.TabId, ParentModule.TabID, BlogId, ContentItemId, TermId, AuthorId);
      }
      IsMultiLingualSite = ComponentModel.ComponentBase<DotNetNuke.Services.Localization.ILocaleController, DotNetNuke.Services.Localization.LocaleController>.Instance.GetLocales(blogModule.PortalId).Count > 1;
      if (!blogModule.ViewSettings.ShowAllLocales)
      {
        ShowLocale = Locale;
      }
      if (Referrer.Contains("/ctl/") | Referrer.Contains("&ctl="))
      {
        Referrer = DotNetNuke.Common.Globals.NavigateURL(blogModule.TabId); // just catch 99% of bad referrals to edit pages
      }

      UiTimeZone = blogModule.ModuleContext.PortalSettings.TimeZone;
      if (blogModule.UserInfo.Profile.PreferredTimeZone is not null)
      {
        UiTimeZone = blogModule.UserInfo.Profile.PreferredTimeZone;
      }

    }

    public static BlogContextInfo GetBlogContext(ref HttpContext context, BlogModuleBase blogModule)
    {
      BlogContextInfo res;
      if (context.Items["BlogContext" + blogModule.TabModuleId.ToString()] is null)
      {
        res = new BlogContextInfo(context, blogModule);
        context.Items["BlogContext" + blogModule.TabModuleId.ToString()] = res;
      }
      else
      {
        res = (BlogContextInfo)context.Items["BlogContext" + blogModule.TabModuleId.ToString()];
      }
      return res;
    }
    #endregion

    #region  Public Properties 
    public int BlogModuleId { get; set; } = -1;
    public DotNetNuke.Entities.Modules.ModuleInfo ParentModule { get; set; } = null;
    public int BlogId { get; set; } = -1;
    public int ContentItemId { get; set; } = -1;
    public int TermId { get; set; } = -1;
    public string Categories { get; set; } = "";
    public int AuthorId { get; set; } = -1;
    public DateTime EndDate { get; set; } = DateTime.Now.ToUniversalTime();
    public DateTime EndDateOrNow { get; set; } = DateTime.Now;
    public Entities.Blogs.BlogInfo Blog { get; set; } = null;
    public Entities.Posts.PostInfo Post { get; set; } = null;
    public Entities.Terms.TermInfo Term { get; set; } = null;
    public DotNetNuke.Entities.Users.UserInfo Author { get; set; } = null;
    public string BlogMapPath { get; set; } = "";
    public string PostMapPath { get; set; } = "";
    public bool OutputAdditionalFiles { get; set; }
    public ModuleUrls ModuleUrls { get; set; } = null;
    public string SearchString { get; set; } = "";
    public bool SearchTitle { get; set; } = true;
    public bool SearchContents { get; set; } = false;
    public bool SearchUnpublished { get; set; } = false;
    public bool IsMultiLingualSite { get; set; } = false;
    public string ShowLocale { get; set; } = null;
    public string Locale { get; set; } = "";
    public string Referrer { get; set; } = "";
    public bool WLWRequest { get; set; } = false;
    public TimeZoneInfo UiTimeZone { get; set; }
    public ContextSecurity Security { get; set; }
    public int LegacyEntryId { get; set; } = -1;
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
        case "blogmoduleid":
          {
            return BlogModuleId.ToString(OutputFormat, formatProvider);
          }
        case "blogid":
          {
            return BlogId.ToString(OutputFormat, formatProvider);
          }
        case "Postid":
        case "contentitemid":
        case "postid":
        case "post":
          {
            return ContentItemId.ToString(OutputFormat, formatProvider);
          }
        case "termid":
        case "term":
          {
            return TermId.ToString(OutputFormat, formatProvider);
          }
        case "categories":
          {
            return Categories;
          }
        case "authorid":
        case "author":
          {
            return AuthorId.ToString(OutputFormat, formatProvider);
          }
        case "enddate":
          {
            return EndDate.ToString(OutputFormat, formatProvider);
          }
        case "enddateornow":
          {
            return EndDateOrNow.ToString(OutputFormat, formatProvider);
          }
        case "blogselected":
          {
            return (BlogId > -1).ToString();
          }
        case "postselected":
          {
            return (ContentItemId > -1).ToString();
          }
        case "termselected":
          {
            return (TermId > -1).ToString();
          }
        case "authorselected":
          {
            return (AuthorId > -1).ToString();
          }
        case "ismultilingualsite":
          {
            return IsMultiLingualSite.ToString();
          }
        case "showlocale":
          {
            return ShowLocale;
          }
        case "locale":
          {
            switch (strFormat.ToLower() ?? "")
            {
              case "3":
                {
                  return System.Threading.Thread.CurrentThread.CurrentCulture.ThreeLetterISOLanguageName;
                }
              case "ietf":
                {
                  return System.Threading.Thread.CurrentThread.CurrentCulture.IetfLanguageTag;
                }
              case "displayname":
              case "display":
                {
                  return System.Threading.Thread.CurrentThread.CurrentCulture.DisplayName;
                }
              case "englishname":
              case "english":
                {
                  return System.Threading.Thread.CurrentThread.CurrentCulture.EnglishName;
                }
              case "nativename":
              case "native":
                {
                  return System.Threading.Thread.CurrentThread.CurrentCulture.NativeName;
                }
              case "generic":
              case "2":
                {
                  return System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                }

              default:
                {
                  return Locale;
                }
            }

            break;
          }
        case "searchstring":
          {
            return SearchString;
          }
        case "issearch":
          {
            return (!string.IsNullOrEmpty(SearchString)).ToString();
          }
        case "referrer":
          {
            return Referrer;
          }

        default:
          {
            if (RequestParams[strPropertyName] is not null)
            {
              return RequestParams[strPropertyName];
            }
            else
            {
              PropertyNotFound = true;
            }

            break;
          }
      }
      return DotNetNuke.Common.Utilities.Null.NullString;
    }

    public CacheLevel Cacheability
    {
      get
      {
        return CacheLevel.fullyCacheable;
      }
    }
    #endregion

  }

}