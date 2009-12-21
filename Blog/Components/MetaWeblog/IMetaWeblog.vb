'
' DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2010
' by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'-------------------------------------------------------------------------

Imports CookComputing.XmlRpc

Namespace MetaWeblog
 ''' <summary>
 ''' Interface for MetaWeblog methods that WLW uses
 ''' </summary>
 ''' <remarks></remarks>
 ''' <history>
 '''		[pdonker]	12/14/2009	created
 ''' </history>
 Public Interface IMetaWeblog

  <XmlRpcMethod("metaWeblog.editPost", Description:="Updates and existing post to a designated blog " + "using the metaWeblog API. Returns true if completed.")> _
  Function editPost(ByVal postid As String, ByVal username As String, ByVal password As String, ByVal post As Post, ByVal publish As Boolean) As Boolean

  <XmlRpcMethod("metaWeblog.getCategories", Description:="Retrieves a list of valid Categories for a post " + "using the metaWeblog API. Returns the metaWeblog Categories " + "struct collection.")> _
  Function getCategories(ByVal blogid As String, ByVal username As String, ByVal password As String) As MetaWebLogCategoryInfo()

  <XmlRpcMethod("metaWeblog.getPost", Description:="Retrieves an existing post using the metaWeblog " + "API. Returns the metaWeblog struct.")> _
  Function getPost(ByVal postid As String, ByVal username As String, ByVal password As String) As Post

  <XmlRpcMethod("metaWeblog.getRecentPosts", Description:="Retrieves a list of the most recent existing post " + "using the metaWeblog API. Returns the metaWeblog struct collection.")> _
  Function getRecentPosts(ByVal blogid As String, ByVal username As String, ByVal password As String, ByVal numberOfPosts As Integer) As Post()

  <XmlRpcMethod("metaWeblog.newPost", Description:="Makes a new post to a designated blog using the " + "metaWeblog API. Returns postid as a string.")> _
  Function newPost(ByVal blogid As String, ByVal username As String, ByVal password As String, ByVal post As Post, ByVal publish As Boolean) As String

  <XmlRpcMethod("metaWeblog.newMediaObject", Description:="Uploads an image, movie, song, or other media " + "using the metaWeblog API. Returns the metaObject struct.")> _
  Function newMediaObject(ByVal blogid As Object, ByVal username As String, ByVal password As String, ByVal mediaobject As mediaObject) As mediaObjectInfo

 End Interface

 <XmlRpcMissingMapping(MappingAction.Ignore)> _
 Public Structure Enclosure
  Public length As Integer
  Public type As String
  Public url As String
 End Structure

 <XmlRpcMissingMapping(MappingAction.Ignore)> _
 Public Structure Source
  Public name As String
  Public url As String
 End Structure

 ''' <summary>
 ''' Central structure to pass a post between WLW and the module
 ''' </summary>
 ''' <remarks></remarks>
 ''' <history>
 '''		[pdonker]	12/20/2009	added 'publish'
 ''' </history>
 <XmlRpcMissingMapping(MappingAction.Ignore)> _
 Public Structure Post

  <XmlRpcMember("pubDate", description:="The date to publish the blog entry.")> _
  Public pubDate As DateTime
  <XmlRpcMember("date_created_gmt", description:="The GMT DateTime value when the blog entry was created.")> _
  Public date_created_gmt As DateTime
  <XmlRpcMissingMapping(MappingAction.[Error])> _
  Public dateCreated As DateTime
  <XmlRpcMissingMapping(MappingAction.[Error])> _
  Public description As String
  <XmlRpcMissingMapping(MappingAction.[Error])> _
  Public title As String
  <XmlRpcMember("categories", description:="Contains Categories for the post.")> _
  Public categories As String()
  <XmlRpcMember("mt_keywords", description:="List of Keywords for the post.")> _
  Public mt_keywords As String
  <XmlRpcMember("mt_allow_comments", description:="Determines if comments will be allowed for this post.  (0 = none, 1 = open, 2=closed)")> _
  Public mt_allow_comments As Integer

  <XmlRpcMember("mt_allow_pings", description:="Determines if comments will be allowed for this post.  (0 = closed, 1 = open) ")> _
  Public mt_allow_pings As Integer
  'Public enclosure As enclosure
  Public link As String
  Public permalink As String
  <XmlRpcMember(description:="Not required when posting. Depending on server may " + "be either string or integer. " + "Use Convert.ToInt32(postid) to treat as integer or " + "Convert.ToString(postid) to treat as string")> _
  Public postid As Object
  <XmlRpcMember("publish", description:="Determines if post will be published ")> _
  Public publish As Boolean


  ''' <summary>
  ''' Used to track an SEO friendly description of the post, usually shorter than
  ''' the Title and formatted for SEO presentation (eg. Using-Trackbacks-1).
  ''' Only utilized if the supportsSlug entity is set to yes in the manifest file.
  ''' </summary>
  Public wp_slug As String
  ''' <summary>
  ''' Only utilized if the supportsPassword entity is set to yes in the manifest file.
  ''' </summary>
  Public wp_password As String
  ''' <summary>
  ''' Only utilized if the supportsPageParent entity is set to yes in the manifest file.
  ''' </summary>
  <XmlRpcMember("wp_page_parent_id", description:="Id for the parent of the page.")> _
  Public wp_page_parent_id As String
  ''' <summary>
  ''' Only utilized if the supportsPageOrder entity is set to yes in the manifest file.
  ''' </summary>
  Public wp_page_order As String
  ''' <summary>
  ''' Only utilized if the supportsAuthor entity is set to yes in the manifest file.
  ''' </summary>
  Public wp_author_id As String
  ''' <summary>
  ''' Only utilized if the supportsExcerpt entity is set to yes in the manifest file.
  ''' </summary>
  <XmlRpcMember("mt_excerpt", description:="Summary of the post.")> _
  Public mt_excerpt As String
  ''' <summary>
  ''' Only utilized if the supportsExtendedEntries entity is set to yes in the manifest file.
  ''' </summary>
  Public mt_text_more As String
  ''' <summary>
  ''' Used to manage trackback URLs.  Will only be populated if 
  ''' supportsTrackbacks is set to yes in the manifest file.
  ''' </summary>
  Public mt_tb_ping_urls As String()
  ''' <summary>
  ''' Used to track the status of the page.  This is a WordPress property.
  ''' </summary>
  Public page_status As String
 End Structure

 Public Structure Page
  <XmlRpcMember("page_id", Description:="Id for the page.")> _
  Public page_id As Object
  <XmlRpcMember("page_title", Description:="Title of the page.")> _
  Public page_title As String
  <XmlRpcMember("page_parent_id", Description:="Id for the parent of the page.")> _
  Public page_parent_id As String
  <XmlRpcMember("DateCreated", Description:="Creation date of the page.")> _
  Public dateCreated As DateTime
 End Structure

 Public Structure CategoryInfo
  Public categoryId As String
  Public parentId As String
  Public description As String
  Public categoryName As String
  Public htmlUrl As String
  Public rssUrl As String
 End Structure

 ''' <summary>
 ''' Metaweblog specific category structure
 ''' </summary>
 ''' <remarks></remarks>
 Public Structure MetaWebLogCategoryInfo
  Public description As String
  Public htmlUrl As String
  Public rssUrl As String
 End Structure

 Public Structure NewCategory
  Public name As String
  Public slug As String
  Public parent_id As Integer
  Public description As String
 End Structure

 Public Structure mediaObject
  Public name As String
  Public type As String
  Public bits As Byte()
 End Structure

 Public Structure mediaObjectInfo
  Public url As String
 End Structure

End Namespace