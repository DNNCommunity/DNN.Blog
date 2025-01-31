using System.Collections.Generic;
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

using DotNetNuke.Services.Tokens;

namespace DotNetNuke.Modules.Blog.Common
{
  public class ModuleUrls : IPropertyAccess
  {

    #region  Properties 
    public int TabId { get; set; } = -1;
    public int ParentTabId { get; set; } = -1;
    public int BlogId { get; set; } = -1;
    public int ContentItemId { get; set; } = -1;
    public int TermId { get; set; } = -1;
    public int AuthorId { get; set; } = -1;
    private Dictionary<string, string> Cache { get; set; } = new Dictionary<string, string>();
    #endregion

    #region  Constructors 
    public ModuleUrls(int tabId, int blogId, int contentItemId, int termId, int authorId)
    {
      TabId = tabId;
      BlogId = blogId;
      ContentItemId = contentItemId;
      TermId = termId;
      AuthorId = authorId;
    }

    public ModuleUrls(int tabId, int parenttabId, int blogId, int contentItemId, int termId, int authorId)
    {
      TabId = tabId;
      ParentTabId = parenttabId;
      BlogId = blogId;
      ContentItemId = contentItemId;
      TermId = termId;
      AuthorId = authorId;
    }
    #endregion

    #region  Public Methods 
    public string GetUrl(bool includeBlog, bool includePost, bool includeTerm, bool includeAuthor, bool includeEnding)
    {
      string urlType = "";
      if (includeBlog)
        urlType = "blog";
      if (includePost)
        urlType += "post";
      if (includeTerm)
        urlType += "term";
      if (includeAuthor)
        urlType += "author";
      string cacheKey = string.Format("{0}:{1}", urlType, includeEnding.ToString().ToLower());
      if (Cache.ContainsKey(cacheKey))
        return Cache[cacheKey];
      BuildUrl(urlType);
      return Cache[cacheKey];
    }
    #endregion

    #region  Private Methods 
    private void BuildUrl(string urlType)
    {
      var @params = new List<string>();
      switch (urlType ?? "")
      {
        case "blog":
          {
            if (BlogId > -1)
              @params.Add("Blog=" + BlogId.ToString());
            break;
          }
        case "post":
          {
            if (ContentItemId > -1)
              @params.Add("Post=" + ContentItemId.ToString());
            break;
          }
        case "term":
          {
            if (TermId > -1)
              @params.Add("Term=" + TermId.ToString());
            break;
          }
        case "author":
          {
            if (AuthorId > -1)
              @params.Add("Author=" + AuthorId.ToString());
            break;
          }
        case "blogpost":
          {
            if (BlogId > -1)
              @params.Add("Blog=" + BlogId.ToString());
            if (ContentItemId > -1)
              @params.Add("Post=" + ContentItemId.ToString());
            break;
          }
        case "blogterm":
          {
            if (BlogId > -1)
              @params.Add("Blog=" + BlogId.ToString());
            if (TermId > -1)
              @params.Add("Term=" + TermId.ToString());
            break;
          }
        case "blogauthor":
          {
            if (BlogId > -1)
              @params.Add("Blog=" + BlogId.ToString());
            if (AuthorId > -1)
              @params.Add("Author=" + AuthorId.ToString());
            break;
          }
        case "postterm":
          {
            if (ContentItemId > -1)
              @params.Add("Post=" + ContentItemId.ToString());
            if (TermId > -1)
              @params.Add("Term=" + TermId.ToString());
            break;
          }
        case "postauthor":
          {
            if (ContentItemId > -1)
              @params.Add("Post=" + ContentItemId.ToString());
            if (AuthorId > -1)
              @params.Add("Author=" + AuthorId.ToString());
            break;
          }
        case "termauthor":
          {
            if (TermId > -1)
              @params.Add("Term=" + TermId.ToString());
            if (AuthorId > -1)
              @params.Add("Author=" + AuthorId.ToString());
            break;
          }
        case "blogpostterm":
          {
            if (BlogId > -1)
              @params.Add("Blog=" + BlogId.ToString());
            if (ContentItemId > -1)
              @params.Add("Post=" + ContentItemId.ToString());
            if (TermId > -1)
              @params.Add("Term=" + TermId.ToString());
            break;
          }
        case "blogpostauthor":
          {
            if (BlogId > -1)
              @params.Add("Blog=" + BlogId.ToString());
            if (ContentItemId > -1)
              @params.Add("Post=" + ContentItemId.ToString());
            if (AuthorId > -1)
              @params.Add("Author=" + AuthorId.ToString());
            break;
          }
        case "blogtermauthor":
          {
            if (BlogId > -1)
              @params.Add("Blog=" + BlogId.ToString());
            if (TermId > -1)
              @params.Add("Term=" + TermId.ToString());
            if (AuthorId > -1)
              @params.Add("Author=" + AuthorId.ToString());
            break;
          }
        case "posttermauthor":
          {
            if (ContentItemId > -1)
              @params.Add("Post=" + ContentItemId.ToString());
            if (TermId > -1)
              @params.Add("Term=" + TermId.ToString());
            if (AuthorId > -1)
              @params.Add("Author=" + AuthorId.ToString());
            break;
          }
        case "blogposttermauthor":
        case "all":
          {
            if (BlogId > -1)
              @params.Add("Blog=" + BlogId.ToString());
            if (ContentItemId > -1)
              @params.Add("Post=" + ContentItemId.ToString());
            if (TermId > -1)
              @params.Add("Term=" + TermId.ToString());
            if (AuthorId > -1)
              @params.Add("Author=" + AuthorId.ToString());
            break;
          }
        case "parenturl":
          {
            TabId = ParentTabId;
            break;
          }
      }
      string BaseUrl = DotNetNuke.Common.Globals.NavigateURL(TabId, "", @params.ToArray());
      string BaseUrlPlusEnding;
      if (BaseUrl.Contains("?"))
      {
        BaseUrlPlusEnding = BaseUrl + "&";
      }
      else
      {
        BaseUrlPlusEnding = BaseUrl + "?";
      }
      Cache.Add(string.Format("{0}:false", urlType), BaseUrl);
      Cache.Add(string.Format("{0}:true", urlType), BaseUrlPlusEnding);
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
      strFormat = strFormat.ToLower();
      if (strFormat != "true" && strFormat != "false")
        strFormat = "false";
      strPropertyName = strPropertyName.ToLower();
      string cacheKey = string.Format("{0}:{1}", strPropertyName, strFormat);
      if (Cache.ContainsKey(cacheKey))
        return Cache[cacheKey];
      BuildUrl(strPropertyName);
      return Cache[cacheKey];
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