using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
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

using System.Xml;
using static DotNetNuke.Common.Globals;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Modules.Blog.Common;
using static DotNetNuke.Modules.Blog.Common.Globals;
using DotNetNuke.Modules.Blog.Entities.Blogs;
using DotNetNuke.Modules.Blog.Entities.Posts;
using DotNetNuke.Modules.Blog.Entities.Terms;

namespace DotNetNuke.Modules.Blog.Rss
{
  public class BlogRssFeed
  {

    #region  Constants 
    private const string nsBlogPre = "blog";
    private const string nsBlogFull = "http://dnn-connect.org/blog/";
    private const string nsSlashPre = "slash";
    private const string nsSlashFull = "http://purl.org/rss/1.0/modules/slash/";
    private const string nsAtomPre = "atom";
    private const string nsAtomFull = "http://www.w3.org/2005/Atom";
    private const string nsMediaPre = "media";
    private const string nsMediaFull = "http://search.yahoo.com/mrss/";
    private const string nsDublinPre = "dc";
    private const string nsDublinFull = "http://purl.org/dc/elements/1.1/";
    private const string nsContentPre = "content";
    private const string nsContentFull = "http://purl.org/rss/1.0/modules/content/";
    private const string nsOpenSearchPre = "os";
    private const string nsOpenSearchFull = "http://opensearch.a9.com/spec/opensearchrss/1.0/";
    #endregion

    #region  Properties 
    public ModuleSettings Settings { get; set; } = null;
    public DotNetNuke.Entities.Portals.PortalSettings PortalSettings { get; set; } = null;
    public IEnumerable<PostInfo> Posts { get; set; } = null;
    public string CacheFile { get; set; } = "";
    public bool IsCached { get; set; } = false;
    public string ImageHandlerUrl { get; set; } = "";
    public int TotalRecords { get; set; } = -1;

    // Requested Properties
    public int TermId { get; set; } = -1;
    public int BlogId { get; set; } = -1;
    public int RecordsToSend { get; set; } = 20;
    public string Search { get; set; } = "";
    public bool SearchTitle { get; set; } = true;
    public bool SearchContents { get; set; } = false;
    public int ImageWidth { get; set; } = 144;
    public int ImageHeight { get; set; } = 96;
    public bool IncludeContents { get; set; } = false;

    // Feed Properties
    public bool IsSearchFeed { get; set; } = false;
    public BlogInfo Blog { get; set; } = null;
    public TermInfo Term { get; set; } = null;
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Link { get; set; } = "";
    public string FeedEmail { get; set; } = "";
    public string Language { get; set; } = "";
    public string Locale { get; set; } = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
    public string Copyright { get; set; } = "";
    public string URL { get; set; } = "";
    #endregion

    #region  Constructors 
    public BlogRssFeed(int moduleId, NameValueCollection reqParams)
    {

      // Initialize Settings
      Settings = ModuleSettings.GetModuleSettings(moduleId);
      PortalSettings = DotNetNuke.Entities.Portals.PortalSettings.Current;
      RecordsToSend = Settings.RssDefaultNrItems;
      ImageWidth = Settings.RssImageWidth;
      ImageHeight = Settings.RssImageHeight;
      string port = string.Empty;
      if (HttpContext.Current.Request.Url.Port != 80)
      {
        port = ":" + HttpContext.Current.Request.Url.Port.ToString();
      }
      ImageHandlerUrl = string.Format("{0}://{1}{2}{3}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Host, port, VirtualPathUtility.ToAbsolute(glbImageHandlerPath));

      // Read Request Values
      int argVariable = BlogId;
      Extensions.ReadValue(ref reqParams, "blog", ref argVariable);
      BlogId = argVariable;
      int argVariable1 = BlogId;
      Extensions.ReadValue(ref reqParams, "blogid", ref argVariable1);
      BlogId = argVariable1;
      int argVariable2 = TermId;
      Extensions.ReadValue(ref reqParams, "term", ref argVariable2);
      TermId = argVariable2;
      int argVariable3 = TermId;
      Extensions.ReadValue(ref reqParams, "termid", ref argVariable3);
      TermId = argVariable3;
      if (Settings.RssMaxNrItems > 0)
      {
        int argVariable4 = RecordsToSend;
        Extensions.ReadValue(ref reqParams, "recs", ref argVariable4);
        RecordsToSend = argVariable4;
        if (RecordsToSend > Settings.RssMaxNrItems)
          RecordsToSend = Settings.RssMaxNrItems;
      }
      if (Settings.RssImageSizeAllowOverride)
      {
        int argVariable5 = ImageWidth;
        Extensions.ReadValue(ref reqParams, "w", ref argVariable5);
        ImageWidth = argVariable5;
        int argVariable6 = ImageHeight;
        Extensions.ReadValue(ref reqParams, "h", ref argVariable6);
        ImageHeight = argVariable6;
      }
      if (Settings.RssAllowContentInFeed)
      {
        bool argVariable7 = IncludeContents;
        Extensions.ReadValue(ref reqParams, "body", ref argVariable7);
        IncludeContents = argVariable7;
      }
      string argVariable8 = Search;
      Extensions.ReadValue(ref reqParams, "search", ref argVariable8);
      Search = argVariable8;
      bool argVariable9 = SearchTitle;
      Extensions.ReadValue(ref reqParams, "t", ref argVariable9);
      SearchTitle = argVariable9;
      bool argVariable10 = SearchContents;
      Extensions.ReadValue(ref reqParams, "c", ref argVariable10);
      SearchContents = argVariable10;
      string argVariable11 = Language;
      Extensions.ReadValue(ref reqParams, "language", ref argVariable11);
      Language = argVariable11;
      if (!string.IsNullOrEmpty(Language))
        Locale = Language;

      // Start Filling In Feed Properties
      if (!string.IsNullOrEmpty(Search))
        IsSearchFeed = true;
      if (BlogId > -1)
        Blog = BlogsController.GetBlog(BlogId, -1, System.Threading.Thread.CurrentThread.CurrentCulture.Name);
      if (TermId > -1)
        Term = TermsController.GetTerm(TermId, moduleId, Locale);
      if (Blog is null)
      {
        var m = new ModuleController().GetModule(moduleId);
        if (m is not null)
        {
          Title = m.ModuleTitle;
        }
        FeedEmail = Settings.RssEmail;
        Copyright = Settings.RssDefaultCopyright;
      }
      else
      {
        Title = Blog.Title;
        Description = Blog.Description;
        FeedEmail = Blog.Email;
        Copyright = Regex.Replace(Blog.Copyright, @"(?i)\[year\](?-i)", DateTime.Now.ToString("yyyy"));
      }
      if (Term is not null)
      {
        Title += " - " + Term.LocalizedName;
      }
      if (IsSearchFeed)
      {
        Title = "DNN Blog Search " + Title;
        Description += string.Format(" - Searching '{0}'", Search);
      }
      Link = ApplicationURL();
      if (Blog is not null)
        Link += string.Format("&blog={0}", BlogId);
      if (Term is not null)
        Link += string.Format("&term={0}", TermId);
      if (RecordsToSend != Settings.RssDefaultNrItems)
        Link += string.Format("&recs={0}", RecordsToSend);
      if (ImageWidth != Settings.RssImageWidth)
        Link += string.Format("&w={0}", ImageWidth);
      if (ImageHeight != Settings.RssImageHeight)
        Link += string.Format("&h={0}", ImageHeight);
      if (IncludeContents)
        Link += "&body=true";
      if (!string.IsNullOrEmpty(Language))
        Link += string.Format("&language={0}", Language);
      if (!string.IsNullOrEmpty(Locale))
        Link += string.Format("&locale={0}", Locale);
      if (IsSearchFeed)
        Link += string.Format("&search={0}&t={1}&c={2}", HttpUtility.UrlEncode(Search), SearchTitle, SearchContents);
      CacheFile = Link.Substring(Link.IndexOf('?') + 1).Replace("&", "+").Replace("=", "-");
      CacheFile = string.Format(@"{0}\Blog\RssCache\{1}.resources", PortalSettings.HomeDirectoryMapPath.TrimEnd('\\'), CacheFile);
      URL = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority;
      if (DotNetNuke.Entities.Host.Host.UseFriendlyUrls)
      {
        Link = FriendlyUrl(PortalSettings.ActiveTab, Link, GetSafePageName(Title));
      }
      else
      {
        Link = URL + ResolveUrl(Link);
      }

      // Check Cache
      if (System.IO.File.Exists(CacheFile))
      {
        var f = new System.IO.FileInfo(CacheFile);
        if (f.LastWriteTime.AddMinutes(Settings.RssTtl) > DateTime.Now)
          IsCached = true;
      }
      else
      {
        string pth = System.IO.Path.GetDirectoryName(CacheFile);
        if (!System.IO.Directory.Exists(pth))
          System.IO.Directory.CreateDirectory(pth);
      }

      if (!IsCached)
      {
        // Load Posts
        if (IsSearchFeed)
        {
          if (Term is not null)
          {
            Dictionary<int, PostInfo> localSearchPostsByTerm() { int argtotalRecords = TotalRecords; var ret = PostsController.SearchPostsByTerm(moduleId, BlogId, Locale, TermId, Search, SearchTitle, SearchContents, 1, Language, DateTime.Now.ToUniversalTime(), -1, 0, RecordsToSend, "PUBLISHEDONDATE DESC", ref argtotalRecords, -1, false); TotalRecords = argtotalRecords; return ret; }

            Posts = localSearchPostsByTerm().Values;
          }
          else
          {
            Dictionary<int, PostInfo> localSearchPosts() { int argtotalRecords1 = TotalRecords; var ret = PostsController.SearchPosts(moduleId, BlogId, Locale, Search, SearchTitle, SearchContents, 1, Language, DateTime.Now.ToUniversalTime(), -1, 0, RecordsToSend, "PUBLISHEDONDATE DESC", ref argtotalRecords1, -1, false); TotalRecords = argtotalRecords1; return ret; }

            Posts = localSearchPosts().Values;
          }
        }
        else if (Term is not null)
        {
          Dictionary<int, PostInfo> localGetPostsByTerm() { int argtotalRecords2 = TotalRecords; var ret = PostsController.GetPostsByTerm(moduleId, BlogId, Locale, TermId, 1, Language, DateTime.Now.ToUniversalTime(), -1, 0, RecordsToSend, "PUBLISHEDONDATE DESC", ref argtotalRecords2, -1, false); TotalRecords = argtotalRecords2; return ret; }

          Posts = localGetPostsByTerm().Values;
        }
        else
        {
          Dictionary<int, PostInfo> localGetPosts() { int argtotalRecords3 = TotalRecords; var ret = PostsController.GetPosts(moduleId, BlogId, Locale, 1, Language, DateTime.Now.ToUniversalTime(), -1, false, 0, RecordsToSend, "PUBLISHEDONDATE DESC", ref argtotalRecords3, -1, false); TotalRecords = argtotalRecords3; return ret; }

          Posts = localGetPosts().Values;
        }
        WriteRss(CacheFile);
      }
    }
    #endregion

    #region  Public Methods 
    public string WriteRssToString()
    {
      var sb = new StringBuilder();
      WriteRss(ref sb);
      return sb.ToString();
    }

    public void WriteRss(ref System.IO.Stream output)
    {
      using (var xtw = new XmlTextWriter(output, Encoding.UTF8))
      {
        var argoutput = xtw;
        WriteRss(ref argoutput);
        xtw.Flush();
      }
    }

    public void WriteRss(string fileName)
    {
      using (var fs = new System.IO.FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
      {
        using (var xtw = new XmlTextWriter(fs, Encoding.UTF8))
        {
          var argoutput = xtw;
          WriteRss(ref argoutput);
          xtw.Flush();
        }
      }
    }

    public void WriteRss(ref StringBuilder output)
    {
      using (var sw = new StringWriterWithEncoding(output, Encoding.UTF8))
      {
        using (var xtw = new XmlTextWriter(sw))
        {
          var argoutput = xtw;
          WriteRss(ref argoutput);
          xtw.Flush();
        }
        sw.Flush();
      }
    }

    public void WriteRss(ref XmlTextWriter output)
    {

      output.Formatting = Formatting.Indented;
      output.WriteStartDocument();
      output.WriteStartElement("rss");
      output.WriteAttributeString("version", "2.0");
      output.WriteAttributeString("xmlns", nsBlogPre, null, nsBlogFull);
      // output.WriteAttributeString("xmlns", nsSlashPre, Nothing, nsSlashFull)
      output.WriteAttributeString("xmlns", nsAtomPre, null, nsAtomFull);
      output.WriteAttributeString("xmlns", nsMediaPre, null, nsMediaFull);
      if (IsSearchFeed)
        output.WriteAttributeString("xmlns", nsOpenSearchPre, null, nsOpenSearchFull);
      if (IncludeContents)
        output.WriteAttributeString("xmlns", nsContentPre, null, nsContentFull);
      output.WriteStartElement("channel");

      // Write the channel header block
      output.WriteElementString("title", Title);
      output.WriteElementString("link", Link);
      output.WriteElementString("description", Description);
      // optional elements
      if (!string.IsNullOrEmpty(Language))
        output.WriteElementString("language", Language);
      if (!string.IsNullOrEmpty(Copyright))
        output.WriteElementString("copyright", Copyright);
      if (!string.IsNullOrEmpty(FeedEmail))
        output.WriteElementString("managingEditor", FeedEmail);
      output.WriteElementString("pubDate", DateTime.Now.ToString("r"));
      output.WriteElementString("lastBuildDate", DateTime.Now.ToString("r"));
      if (Term is not null)
      {
        output.WriteElementString("category", Term.LocalizedName);
      }
      output.WriteElementString("generator", "DotNetNuke Blog RSS Generator Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
      output.WriteElementString("ttl", Settings.RssTtl.ToString());
      if (Blog is not null && Blog.IncludeImagesInFeed & !string.IsNullOrEmpty(Blog.Image))
      {
        output.WriteStartElement("image");
        output.WriteElementString("url", ImageHandlerUrl + string.Format("?TabId={0}&ModuleId={1}&Blog={2}&w={4}&h={5}&c=1&key={3}", PortalSettings.ActiveTab.TabID, Settings.ModuleId, BlogId, Blog.Image, ImageWidth, ImageHeight));
        output.WriteElementString("title", Title);
        output.WriteElementString("link", Link);
        output.WriteElementString("width", ImageWidth.ToString()); // default 88 max 144
        output.WriteElementString("height", ImageHeight.ToString()); // default 31 max 400
        output.WriteEndElement(); // image
      }
      // extended elements
      output.WriteStartElement(nsAtomPre, "link", nsAtomFull);
      output.WriteAttributeString("href", HttpContext.Current.Request.Url.AbsoluteUri);
      output.WriteAttributeString("rel", "self");
      output.WriteAttributeString("type", "application/rss+xml");
      output.WriteEndElement(); // atom:link
      if (IsSearchFeed)
      {
        output.WriteElementString(nsOpenSearchPre, "totalResults", nsOpenSearchFull, TotalRecords.ToString());
        output.WriteElementString(nsOpenSearchPre, "startIndex", nsOpenSearchFull, "0");
        output.WriteElementString(nsOpenSearchPre, "itemsPerPage", nsOpenSearchFull, RecordsToSend.ToString());
      }

      foreach (PostInfo e in Posts)
        WriteItem(ref output, e);

      output.WriteEndElement(); // channel
      output.WriteEndElement(); // rss
      output.Flush();

    }
    #endregion

    #region  Private Methods 
    private void WriteItem(ref XmlTextWriter writer, PostInfo item)
    {

      writer.WriteStartElement("item");

      // core data
      writer.WriteElementString("title", item.LocalizedTitle);

      if (DotNetNuke.Entities.Host.Host.UseFriendlyUrls)
      {
        writer.WriteElementString("link", item.PermaLink());
      }
      else
      {
        writer.WriteElementString("link", URL + item.PermaLink());
      }

      writer.WriteElementString("description", HttpUtility.HtmlDecode(item.LocalizedSummary));
      // optional elements
      if (item.Blog.IncludeAuthorInFeed)
      {
        writer.WriteElementString("author", string.Format("{0} ({1})", item.Email, item.DisplayName));
        writer.WriteElementString(nsBlogPre, "author", nsBlogFull, item.DisplayName);
      }
      foreach (TermInfo t in TermsController.GetTermsByPost(item.ContentItemId, Settings.ModuleId, Locale))
        writer.WriteElementString("category", t.LocalizedName);

      // guid needs to have the isPermaLink=false attribute for some rss readers
      writer.WriteStartElement("guid");
      writer.WriteAttributeString("isPermaLink", "true");

      if (DotNetNuke.Entities.Host.Host.UseFriendlyUrls)
      {
        writer.WriteRaw(string.Format("{0}", item.PermaLink()));
      }
      else
      {
        writer.WriteRaw(string.Format("{0}", URL + HttpUtility.HtmlEncode(item.PermaLink())));
      }

      writer.WriteEndElement();

      writer.WriteElementString("pubDate", item.PublishedOnDate.ToString("r"));
      // extensions
      if (item.Blog.IncludeImagesInFeed & !string.IsNullOrEmpty(item.Image))
      {
        writer.WriteStartElement(nsMediaPre, "thumbnail", nsMediaFull);
        writer.WriteAttributeString("width", ImageWidth.ToString());
        writer.WriteAttributeString("height", ImageHeight.ToString());
        writer.WriteAttributeString("url", ImageHandlerUrl + string.Format("?TabId={0}&ModuleId={1}&Blog={2}&Post={3}&w={5}&h={6}&c=1&key={4}", PortalSettings.ActiveTab.TabID, Settings.ModuleId, item.BlogID, item.ContentItemId, item.Image, ImageWidth, ImageHeight));
        writer.WriteEndElement(); // thumbnail
      }
      if (IncludeContents)
      {
        writer.WriteStartElement(nsContentPre, "encoded", nsContentFull);
        writer.WriteCData(HttpUtility.HtmlDecode(item.Content));
        writer.WriteEndElement(); // content:encoded
      }
      // Blog Extensions
      writer.WriteElementString(nsBlogPre, "publishedon", nsBlogFull, item.PublishedOnDate.ToString("u"));

      writer.WriteEndElement(); // item

    }
    #endregion

  }
}