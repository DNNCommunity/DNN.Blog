using System;
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

namespace DotNetNuke.Modules.Blog.Core.Services.WLW.MetaWeblog
{

  /// <summary>
 /// Interface for MetaWeblog methods that WLW uses
 /// </summary>
 /// <remarks></remarks>
 /// <history>
 /// 		[pdonker]	12/14/2009	created
 /// </history>
  public interface IMetaWeblog
  {

    [XmlRpcMethod("metaWeblog.editPost", Description = "Updates and existing post to a designated blog using the metaWeblog API. Returns true if completed.")]
    bool editPost(string postid, string username, string password, Post post, bool publish);

    [XmlRpcMethod("metaWeblog.getCategories", Description = "Retrieves a list of valid Categories for a post using the metaWeblog API. Returns the metaWeblog Categories struct collection.")]
    MetaWebLogCategoryInfo[] getCategories(string blogid, string username, string password);

    [XmlRpcMethod("metaWeblog.getPost", Description = "Retrieves an existing post using the metaWeblog API. Returns the metaWeblog struct.")]
    Post getPost(string postid, string username, string password);

    [XmlRpcMethod("metaWeblog.getRecentPosts", Description = "Retrieves a list of the most recent existing post using the metaWeblog API. Returns the metaWeblog struct collection.")]
    Post[] getRecentPosts(string blogid, string username, string password, int numberOfPosts);

    [XmlRpcMethod("metaWeblog.newPost", Description = "Makes a new post to a designated blog using the metaWeblog API. Returns postid as a string.")]
    string newPost(string blogid, string username, string password, Post post, bool publish);

    [XmlRpcMethod("metaWeblog.newMediaObject", Description = "Uploads an image, movie, song, or other media using the metaWeblog API. Returns the metaObject struct.")]
    mediaObjectInfo newMediaObject(string blogid, string username, string password, mediaObject mediaobject);

  }

  [XmlRpcMissingMapping(MappingAction.Ignore)]
  public struct Enclosure
  {
    public int length;
    public string @type;
    public string url;
  }

  [XmlRpcMissingMapping(MappingAction.Ignore)]
  public struct Source
  {
    public string name;
    public string url;
  }

  /// <summary>
 /// Central structure to pass a post between WLW and the module
 /// </summary>
 /// <remarks></remarks>
 /// <history>
 /// 		[pdonker]	12/20/2009	added 'publish'
 /// </history>
  [XmlRpcMissingMapping(MappingAction.Ignore)]
  public struct Post
  {

    /// <summary>
  /// The date to publish the blog Post.
  /// </summary>
  /// <remarks></remarks>
    [XmlRpcMember("pubDate", Description = "The date to publish the blog Post.")]
    public DateTime pubDate;

    /// <summary>
  /// The GMT DateTime value when the blog Post was created.
  /// </summary>
  /// <remarks></remarks>
    [XmlRpcMember("date_created_gmt", Description = "The GMT DateTime value when the blog Post was created.")]
    public DateTime date_created_gmt;

    /// <summary>
  /// The DateTime value when the blog Post was created.
  /// </summary>
  /// <remarks></remarks>
    [XmlRpcMissingMapping(MappingAction.Error)]
    public DateTime dateCreated;

    /// <summary>
  /// Post Content
  /// </summary>
  /// <remarks></remarks>
    [XmlRpcMissingMapping(MappingAction.Error)]
    public string description;

    /// <summary>
  /// Post Title
  /// </summary>
  /// <remarks></remarks>
    [XmlRpcMissingMapping(MappingAction.Error)]
    public string title;

    /// <summary>
  /// List of categories for the post
  /// </summary>
  /// <remarks></remarks>
    [XmlRpcMember("categories", Description = "Contains Categories for the post.")]
    public string[] categories;

    /// <summary>
  /// 
  /// </summary>
  /// <remarks></remarks>
    [XmlRpcMember("mt_keywords", Description = "List of Keywords for the post.")]
    public string mt_keywords;

    /// <summary>
  /// 
  /// </summary>
  /// <remarks></remarks>
    [XmlRpcMember("mt_allow_comments", Description = "Determines if comments will be allowed for this post.  (0 = none, 1 = open, 2=closed)")]
    public int mt_allow_comments;

    /// <summary>
  /// 
  /// </summary>
  /// <remarks></remarks>
    [XmlRpcMember("mt_allow_pings", Description = "Determines if comments will be allowed for this post.  (0 = closed, 1 = open) ")]
    public int mt_allow_pings;

    /// <summary>
  /// 
  /// </summary>
  /// <remarks></remarks>
    public string link;

    /// <summary>
  /// 
  /// </summary>
  /// <remarks></remarks>
    public string permalink;

    /// <summary>
  /// 
  /// </summary>
  /// <remarks></remarks>
    [XmlRpcMember(Description = "Not required when posting. Depending on server may be either string or integer. Use Convert.ToInt32(postid) to treat as integer or Convert.ToString(postid) to treat as string")]
    public string postid;

    /// <summary>
  /// 
  /// </summary>
  /// <remarks></remarks>
    [XmlRpcMember("publish", Description = "Determines if post will be published ")]
    public bool publish;


    /// <summary>
  /// Used to track an SEO friendly description of the post, usually shorter than
  /// the Title and formatted for SEO presentation (eg. Using-Trackbacks-1).
  /// Only utilized if the supportsSlug entity is set to yes in the manifest file.
  /// </summary>
  /// <remarks></remarks>
    public string wp_slug;

    /// <summary>
  /// Only utilized if the supportsPassword entity is set to yes in the manifest file.
  /// </summary>
  /// <remarks></remarks>
    public string wp_password;

    /// <summary>
  /// Only utilized if the supportsPageParent entity is set to yes in the manifest file.
  /// </summary>
  /// <remarks></remarks>
    [XmlRpcMember("wp_page_parent_id", Description = "Id for the parent of the page.")]
    public string wp_page_parent_id;

    /// <summary>
  /// Only utilized if the supportsPageOrder entity is set to yes in the manifest file.
  /// </summary>
  /// <remarks></remarks>
    public string wp_page_order;

    /// <summary>
  /// Only utilized if the supportsAuthor entity is set to yes in the manifest file.
  /// </summary>
  /// <remarks></remarks>
    public string wp_author_id;

    /// <summary>
  /// Only utilized if the supportsExcerpt entity is set to yes in the manifest file. In plain HTML.
  /// </summary>
  /// <remarks></remarks>
    [XmlRpcMember("mt_excerpt", Description = "Summary of the post.")]
    public string mt_excerpt;

    /// <summary>
  /// Only utilized if the supportsExtendedPosts entity is set to yes in the manifest file.
  /// </summary>
  /// <remarks></remarks>
    public string mt_text_more;

    /// <summary>
  /// Used to manage trackback URLs.  Will only be populated if 
  /// supportsTrackbacks is set to yes in the manifest file.
  /// </summary>
  /// <remarks></remarks>

    public string[] mt_tb_ping_urls;

    /// <summary>
  /// Used to track the status of the page.  This is a WordPress property.
  /// </summary>
  /// <remarks></remarks>
    public string page_status;

  }

  public struct Page
  {
    [XmlRpcMember("page_id", Description = "Id for the page.")]
    public object page_id;
    [XmlRpcMember("page_title", Description = "Title of the page.")]
    public string page_title;
    [XmlRpcMember("page_parent_id", Description = "Id for the parent of the page.")]
    public string page_parent_id;
    [XmlRpcMember("DateCreated", Description = "Creation date of the page.")]
    public DateTime dateCreated;
  }

  public struct CategoryInfo
  {
    public string categoryId;
    public string parentId;
    public string description;
    public string categoryName;
    public string htmlUrl;
    public string rssUrl;
  }

  /// <summary>
 /// Metaweblog specific category structure
 /// </summary>
 /// <remarks></remarks>
  public struct MetaWebLogCategoryInfo
  {
    public string description;
    public string htmlUrl;
    public string rssUrl;
  }

  public struct NewCategory
  {
    public string name;
    public string slug;
    public int parent_id;
    public string description;
  }

  public struct mediaObject
  {
    public string name;
    public string @type;
    public byte[] bits;
  }

  public struct mediaObjectInfo
  {
    public string url;
  }

}