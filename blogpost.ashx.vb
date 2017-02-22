'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2015
' by DotNetNuke Corporation
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
'

Imports DotNetNuke.Modules.Blog.Services.WLW.Blogger
Imports DotNetNuke.Data
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Entities.Users
Imports CookComputing.XmlRpc
Imports DotNetNuke.Security.Membership
Imports System.IO
Imports DotNetNuke.Entities.Content.Taxonomy
Imports System.Linq
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports DotNetNuke.Modules.Blog.Services.WLW
Imports DotNetNuke.Modules.Blog.Services.WLW.WordPress
Imports DotNetNuke.Modules.Blog.Services.WLW.MoveableType
Imports DotNetNuke.Modules.Blog.Services.WLW.MetaWeblog
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Entities.Posts
Imports DotNetNuke.Modules.Blog.Security
Imports DotNetNuke.Modules.Blog.Entities.Terms

''' <summary>
''' Implements the MetaBlog API.
''' </summary>
''' <history>
'''		[pdonker]	12/21/2009	added passing in of blog settings to all methods that go to the provider. Added _TabId variable.
'''  [pdonker] 02/03/2013 complete overhaul with removal of old duplicate code layer. New security pattern to make this more solid and allow for ghost writing.
''' </history>
Public Class BlogPost
 Inherits XmlRpcService
 Implements IMetaWeblog
 Implements IWordPress
 Implements IBlogger
 Implements IMoveableType

 Private Property PortalSettings As PortalSettings = Nothing
 Private Property Settings As ModuleSettings = Nothing
 Private Property ModuleName As String = String.Empty
 Private Property ExpanderScript As String = String.Empty
 Private Property UserInfo As UserInfo = Nothing
 Private Property TabId As Integer = -1
 Private Property ModuleId As Integer = -1
 Private Property PortalId As Integer = -1
 Private Property BlogId As Integer = -1
 Private Property RequestedBlog As BlogInfo = Nothing
 Private Property PostId As Integer = -1
 Private Property RequestedPost As PostInfo = Nothing
 Private Property UnAuthorized As Boolean = True
 Private Property Security As ContextSecurity = Nothing
 Private Property UserTimeZone As TimeZoneInfo = Nothing
 Private Property Locale As String = Threading.Thread.CurrentThread.CurrentCulture.Name

#Region " Method Implementations "
 Public Function getUsersBlogs(appKey As String, username As String, password As String) As BlogInfoStruct() Implements IBlogger.getUsersBlogs
  InitializeMethodCall(username, password, "", "")

  Dim blogs As New List(Of BlogInfoStruct)
  Try
   For Each blog As BlogInfo In BlogsController.GetBlogsByModule(ModuleId, UserInfo.UserID, Locale).Values.Where(Function(b)
                                                                                                                  Return b.CreatedByUserID = UserInfo.UserID Or b.CanAdd Or b.CanEdit
                                                                                                                 End Function).ToList
    blogs.Add(New BlogInfoStruct() With {.blogid = blog.BlogID.ToString, .blogName = blog.Title, .url = DotNetNuke.Common.Globals.NavigateURL(TabId)})
   Next
  Catch mex As BlogPostException
   Throw New XmlRpcFaultException(0, GetString(mex.ResourceKey, mex.Message))
  Catch generatedExceptionName As XmlRpcFaultException
   Throw
  End Try
  Return blogs.ToArray

 End Function

 Public Function getPost(postid As String, username As String, password As String) As Post Implements IMetaWeblog.getPost
  InitializeMethodCall(username, password, "", postid)
  RequireEditPermission()

  Dim result As Post
  Try
   result = ToMwlPost(RequestedPost)
   Dim regexPattern As String = String.Empty
   Dim options As RegexOptions = RegexOptions.Singleline Or RegexOptions.IgnoreCase
   ' Look for and replace relative images with absolute images
   regexPattern = "(src="")/"
   If Not String.IsNullOrEmpty(result.mt_excerpt) Then
    result.mt_excerpt = Regex.Replace(result.mt_excerpt, regexPattern, "$1http://" + Context.Request.Url.Host + "/", options)
   End If
   If Not String.IsNullOrEmpty(result.description) Then
    result.description = Regex.Replace(result.description, regexPattern, "$1http://" + Context.Request.Url.Host + "/", options)
   End If
   If Not String.IsNullOrEmpty(result.mt_text_more) Then
    result.mt_text_more = Regex.Replace(result.mt_text_more, regexPattern, "$1http://" + Context.Request.Url.Host + "/", options)
   End If
  Catch ex As BlogPostException
   LogException(ex)
   Throw New XmlRpcFaultException(0, GetString(ex.ResourceKey, ex.Message))
  Catch ex As XmlRpcFaultException
   LogException(ex)
   Throw
  Catch ex As Exception
   LogException(ex)
   Throw New XmlRpcFaultException(0, GetString("GetPostError", "There was an error retrieving this item."))
  End Try
  Return result

 End Function

 Public Function getCategories_WordPress(blogid As String, username As String, password As String) As Services.WLW.WordPress.CategoryInfo() Implements IWordPress.getCategories
  InitializeMethodCall(username, password, blogid, "")
  RequireAccessPermission()

  If Settings.VocabularyId > 1 Then
   Dim termController As ITermController = DotNetNuke.Entities.Content.Common.Util.GetTermController()
   Dim colCategories As IQueryable(Of Term) = termController.GetTermsByVocabulary(Settings.VocabularyId)
   Dim res(colCategories.Count - 1) As Services.WLW.WordPress.CategoryInfo
   Dim i As Integer = 0
   For Each objTerm As Term In colCategories
    res(i).categoryId = objTerm.TermId
    If objTerm.ParentTermId Is Nothing Then
     res(i).parentId = 0
    Else
     res(i).parentId = Convert.ToInt32(objTerm.ParentTermId)
    End If
    res(i).categoryName = objTerm.Name
    res(i).description = objTerm.Description
    res(i).htmlUrl = "http://google.com"
    res(i).rssUrl = "http://google.com"
    i += 1
   Next
   Return res
  Else
   Return Nothing
  End If

 End Function

 ''' <summary>
 ''' 
 ''' </summary>
 ''' <param name="postid"></param>
 ''' <param name="username"></param>
 ''' <param name="password"></param>
 ''' <returns></returns>
 ''' <remarks>We do not handle tags in here, handled in provider. Likewise, provider 'retrieval' doesn't handle categories.</remarks>
 Public Function getPostCategories(postid As String, username As String, password As String) As Category() Implements IMoveableType.getPostCategories
  InitializeMethodCall(username, password, "", postid)
  RequireEditPermission()

  Dim colCategories As List(Of TermInfo)
  colCategories = RequestedPost.Terms
  Dim res As New List(Of Category)
  Dim i As Integer = 0
  For Each objTerm As Term In colCategories
   If objTerm.VocabularyId > 1 Then
    res.Add(New Category With {.categoryId = objTerm.TermId.ToString(), .categoryName = objTerm.Name})
   End If
   i += 1
  Next
  Return res.ToArray

 End Function

 Public Function setPostCategories(postid As String, username As String, password As String, categories As Category()) As Boolean Implements IMoveableType.setPostCategories
  ' The set is handled in the saving
  Return True
 End Function

 Public Function getRecentPosts(blogid As String, username As String, password As String, numberOfPosts As Integer) As Post() Implements IMetaWeblog.getRecentPosts
  InitializeMethodCall(username, password, blogid, "")
  RequireAccessPermission()

  Dim posts As New List(Of Post)
  Try
   Dim totalRecs As Integer
   Dim arPosts As Dictionary(Of Integer, PostInfo) = PostsController.GetPostsByBlog(ModuleId, CInt(blogid), Locale, UserInfo.UserID, 0, Settings.WLWRecentPostsMax, "PublishedOnDate DESC", totalRecs)
   For Each Post As PostInfo In arPosts.Values
    posts.Add(ToMwlPost(Post))
   Next
  Catch ex As BlogPostException
   LogException(ex)
   Throw New XmlRpcFaultException(0, GetString(ex.ResourceKey, ex.Message))
  Catch ex As XmlRpcFaultException
   LogException(ex)
   Throw
  Catch ex As Exception
   LogException(ex)
   Throw New XmlRpcFaultException(0, GetString("RecentPostsError", "There was an error retrieving the list of items."))
  End Try
  Return posts.ToArray

 End Function

 ''' <summary>
 ''' Creates a new post.  The publish boolean is used to determine whether the item 
 ''' should be published or not.
 ''' </summary>
 ''' <param name="blogid">The blogid.</param>
 ''' <param name="username">The username.</param>
 ''' <param name="password">The password.</param>
 ''' <param name="post">The post.</param>
 ''' <param name="publish">if set to <c>true</c> [publish].</param>
 ''' <returns></returns>
 Public Function newPost(blogId As String, username As String, password As String, post As Post, publish As Boolean) As String Implements IMetaWeblog.newPost
  InitializeMethodCall(username, password, blogId, "")
  RequireAddPermission()

  Dim styleDetectionPost As Boolean = False
  Dim moduleId As Integer = -1

  Try

   ' Check to see if this a style detection post.
   styleDetectionPost = post.title.Contains("3bfe001a-32de-4114-a6b4-4005b770f6d7")

   ' Add the new Post
   Dim newBlogPost As PostInfo = ToBlogPost(post)
   If RequestedBlog.MustApproveGhostPosts And Not Security.CanApprovePost Then
    newBlogPost.Published = False
   Else
    newBlogPost.Published = publish
   End If
   If styleDetectionPost Then
    newBlogPost.Published = True
   End If
   PostsController.AddPost(newBlogPost, UserInfo.UserID)

   HandleAttachments(newBlogPost)
   CheckForMainImage(newBlogPost)
   PostsController.UpdatePost(newBlogPost, UserInfo.UserID)

   ' Add keywords and categories
   If Not styleDetectionPost Then
    AddCategoriesAndKeyWords(newBlogPost, post)
   End If

   ' Publish to journal
   If Not styleDetectionPost And newBlogPost.Published Then
    PublishToJournal(newBlogPost)
   End If

   ' If this is a style detection post, then we write to the Blog_MetaWeblogData table to note
   ' that this post is a new post.
   If styleDetectionPost Then
    Dim blogUrl As String = newBlogPost.PermaLink(PortalSettings)
    Settings.StyleDetectionUrl = blogUrl
    Settings.UpdateSettings()
   End If

   Return newBlogPost.ContentItemId.ToString

  Catch ex As BlogPostException
   LogException(ex)
   Throw New XmlRpcFaultException(0, GetString(ex.ResourceKey, ex.Message))
  Catch ex As XmlRpcFaultException
   LogException(ex)
   Throw
  Catch ex As Exception
   LogException(ex)
   Throw New XmlRpcFaultException(0, GetString("CreateError", "There was an error adding this new item."))
  End Try

 End Function

 Public Function editPost(postid As String, username As String, password As String, mwlPost As Post, publish As Boolean) As Boolean Implements IMetaWeblog.editPost
  InitializeMethodCall(username, password, "", postid)

  Dim success As Boolean = False
  Try

   mwlPost.postid = postid
   Dim newPost As PostInfo = ToBlogPost(mwlPost)
   RequireEditPermission(newPost) ' security moved here to cater for editing of non-approved post
   AddCategoriesAndKeyWords(newPost, mwlPost)
   If RequestedBlog.MustApproveGhostPosts And Not Security.CanApprovePost Then
    newPost.Published = False
   Else
    newPost.Published = publish
   End If

   HandleAttachments(newPost)
   CheckForMainImage(newPost)
   PostsController.UpdatePost(newPost, UserInfo.UserID)

   If (Not RequestedPost.Published) And newPost.Published Then
    ' First published
    PublishToJournal(newPost)
   End If
   success = True
  Catch ex As BlogPostException
   LogException(ex)
   Throw New XmlRpcFaultException(0, GetString(ex.ResourceKey, ex.Message))
  Catch ex As XmlRpcFaultException
   LogException(ex)
   Throw
  Catch ex As Exception
   LogException(ex)
   Throw New XmlRpcFaultException(0, GetString("EditError", "There was an error editing this item."))
  End Try
  Return success

 End Function

 Public Function deletePost(appKey As String, postid As String, username As String, password As String, <XmlRpcParameter(Description:="Where applicable, this specifies whether the blog should be republished after the post has been deleted.")> publish As Boolean) As Boolean Implements IBlogger.deletePost
  InitializeMethodCall(username, password, "", postid)
  RequireEditPermission()

  Dim success As Boolean = False
  Try
   PostsController.DeletePost(RequestedPost.ContentItemId, RequestedPost.BlogID, PortalId, Settings.VocabularyId)
  Catch ex As BlogPostException
   LogException(ex)
   Throw New XmlRpcFaultException(0, GetString(ex.ResourceKey, ex.Message))
  Catch ex As XmlRpcFaultException
   LogException(ex)
   Throw
  Catch ex As Exception
   LogException(ex)
   Throw New XmlRpcFaultException(0, GetString("DeletePostError", "There was an error deleting this item."))
  End Try
  Return success

 End Function

 Public Function getWPCategories(blogid As String, username As String, password As String) As Services.WLW.MetaWeblog.CategoryInfo()
  InitializeMethodCall(username, password, blogid, "")
  RequireAccessPermission()

  Dim categories As New List(Of Services.WLW.MetaWeblog.CategoryInfo)
  Try
   If Settings.VocabularyId > 1 Then
    Dim termController As ITermController = DotNetNuke.Entities.Content.Common.Util.GetTermController()
    Dim colCategories As IQueryable(Of Term) = termController.GetTermsByVocabulary(Settings.VocabularyId)
    For Each objTerm As Term In colCategories
     categories.Add(New Services.WLW.MetaWeblog.CategoryInfo() With {.categoryId = objTerm.TermId.ToString, .categoryName = objTerm.Name, .description = objTerm.Description, .htmlUrl = "http://google.com", .parentId = objTerm.ParentTermId.ToStringOrZero, .rssUrl = "http://google.com"})
    Next
   End If
  Catch ex As BlogPostException
   LogException(ex)
   Throw New XmlRpcFaultException(0, GetString(ex.ResourceKey, ex.Message))
  Catch ex As XmlRpcFaultException
   LogException(ex)
   Throw
  Catch ex As Exception
   LogException(ex)
   Throw New XmlRpcFaultException(0, GetString("GetCategoriesError", "There was an error retrieving the list of items."))
  End Try
  Return categories.ToArray

 End Function

 Public Function getCategories(blogid As String, username As String, password As String) As MetaWebLogCategoryInfo() Implements IMetaWeblog.getCategories
  InitializeMethodCall(username, password, blogid, "")
  RequireAccessPermission()

  Dim categories As New List(Of Services.WLW.MetaWeblog.MetaWebLogCategoryInfo)
  Try
   If Settings.VocabularyId > 1 Then
    Dim termController As ITermController = DotNetNuke.Entities.Content.Common.Util.GetTermController()
    Dim colCategories As IQueryable(Of Term) = termController.GetTermsByVocabulary(Settings.VocabularyId)
    For Each objTerm As Term In colCategories
     categories.Add(New Services.WLW.MetaWeblog.MetaWebLogCategoryInfo() With {.description = objTerm.Name, .htmlUrl = "http://google.com", .rssUrl = "http://google.com"})
    Next
   End If
  Catch ex As BlogPostException
   LogException(ex)
   Throw New XmlRpcFaultException(0, GetString(ex.ResourceKey, ex.Message))
  Catch ex As XmlRpcFaultException
   LogException(ex)
   Throw
  Catch ex As Exception
   LogException(ex)
   Throw New XmlRpcFaultException(0, GetString("GetCategoriesError", "There was an error retrieving the list of items."))
  End Try
  Return categories.ToArray

 End Function

 Public Function newMediaObject(blogid As String, username As String, password As String, mediaobject As mediaObject) As mediaObjectInfo Implements IMetaWeblog.newMediaObject
  InitializeMethodCall(username, password, blogid, "")
  RequireAddPermission()

  Dim info As mediaObjectInfo
  info.url = ""

  Try

   Dim strExtension As String = Path.GetExtension(mediaobject.name)
   If Not Settings.AllowAttachments Or String.IsNullOrEmpty(strExtension) OrElse Not DotNetNuke.Entities.Host.Host.AllowedExtensionWhitelist.IsAllowedExtension(strExtension) Then
    Throw New XmlRpcFaultException(0, GetString("SaveError", String.Format("File {0} refused. Uploading this type of file is not allowed.", mediaobject.name)))
   End If
   Dim newMediaObjectName As String = Guid.NewGuid.ToString("D") & strExtension
   Dim fullFilePathAndName As String = GetTempPostDirectoryMapPath(Me.BlogId) & newMediaObjectName
   Directory.CreateDirectory(Path.GetDirectoryName(fullFilePathAndName))

   Using output As New FileStream(fullFilePathAndName, FileMode.Create)
    Using bw As New BinaryWriter(output)
     bw.Write(mediaobject.bits)
    End Using
   End Using

   info.url = GetTempPostDirectoryPath(Me.BlogId) & newMediaObjectName

  Catch exc As IOException
   Throw New XmlRpcFaultException(0, GetString("ImageSaveError", "An error occurred while saving an image related to this blog post."))
  Catch ex As BlogPostException
   LogException(ex)
   Throw New XmlRpcFaultException(0, GetString(ex.ResourceKey, ex.Message))
  Catch ex As XmlRpcFaultException
   LogException(ex)
   Throw
  Catch ex As Exception
   LogException(ex)
   Throw New XmlRpcFaultException(0, GetString("ImageSaveError", "An error occurred while saving an image related to this blog post."))
  End Try
  Return info

 End Function
#End Region

#Region " Metaweblog/Blog Module Post Conversion Methods "
 Public Function ToMwlPost(post As PostInfo) As Post

  Dim newPost As New Post
  Dim PostBody As New PostBodyAndSummary(post, Settings.SummaryModel, False, Settings.AutoGenerateMissingSummary, Settings.AutoGeneratedSummaryLength)
  PostBody.WriteToPost(newPost, Settings.SummaryModel)

  newPost.mt_allow_comments = post.AllowComments.ToInt
  newPost.categories = post.PostCategories.ToStringArray
  newPost.dateCreated = GetLocalAddedTime(post.CreatedOnDate, PortalSettings.PortalId, UserInfo)
  newPost.pubDate = post.PublishedOnDate
  newPost.date_created_gmt = post.PublishedOnDate
  newPost.postid = post.ContentItemId.ToString
  newPost.mt_keywords = String.Join(",", post.PostTags.ToStringArray)
  newPost.link = post.PermaLink(PortalSettings)
  newPost.permalink = post.PermaLink(PortalSettings)
  newPost.title = post.Title
  newPost.publish = post.Published

  Return newPost

 End Function

 Public Function ToBlogPost(post As Post) As PostInfo

  Dim newPost As New PostInfo
  If Not String.IsNullOrEmpty(post.postid) Then
   newPost = PostsController.GetPost(post.postid.ToInt, ModuleId, "")
  End If

  Dim postBody As New PostBodyAndSummary(post, Settings.SummaryModel, Settings.AutoGenerateMissingSummary, Settings.AutoGeneratedSummaryLength)
  postBody.WriteToPost(newPost, Settings.SummaryModel, True, False)

  newPost.BlogID = BlogId
  newPost.Title = post.title
  If post.dateCreated.Year > 1 Then
   newPost.PublishedOnDate = TimeZoneInfo.ConvertTimeToUtc(post.dateCreated, UserTimeZone)
  End If
  newPost.Published = post.publish
  newPost.AllowComments = post.mt_allow_comments.ToBool
  newPost.TabID = TabId
  newPost.ModuleID = ModuleId
  Return newPost

 End Function
#End Region

#Region " Private Security Methods "
 Private Function ValidateUser(username As String, password As String, ipAddress As String) As UserInfo
  Dim userInfo As UserInfo = Nothing
  Try
   Dim loginStatus As UserLoginStatus = UserLoginStatus.LOGIN_FAILURE
   userInfo = UserController.ValidateUser(PortalSettings.PortalId, username, password, "", PortalSettings.PortalName, ipAddress, loginStatus)
   If userInfo.Roles Is Nothing Then
    ' This fix was added to address an issue with DNN 04.05.05
    userInfo.Roles = GetRolesByUser(userInfo.UserID, PortalSettings.PortalId)
   End If
   If loginStatus = UserLoginStatus.LOGIN_FAILURE Then
    ' testing code that can be added to the end of the previous string.
    ' PortalId:" + _portalSettings.PortalId + " PortalName:" + _portalSettings.PortalName + " portalAlias:" + portalAlias
    Throw New XmlRpcFaultException(0, GetString("UserInvalid", "Either the username or the password is invalid. "))
   End If
  Catch generatedExceptionName As XmlRpcFaultException
   Throw
  Catch ex As Exception
   Throw New XmlRpcFaultException(0, GetString("AuthenticationError", "An error occurred while trying to authenticate the user. "))
  End Try
  Return userInfo
 End Function

 Private Function GetRolesByUser(userId As Integer, portalId As Integer) As String()
  Dim userRoles As String()
  Dim roles As New ArrayList
  Dim SQL As String = "GetRolesByUser"
  Dim methodParams As Object() = {userId, portalId}
  Dim dr As IDataReader = DataProvider.Instance().ExecuteReader(SQL, methodParams)
  While dr.Read()
   roles.Add(dr("RoleName").ToString())
  End While
  userRoles = CType(roles.ToArray(GetType(String)), String())
  Return userRoles
 End Function

 Private Sub RequireEditPermission()
  If Security IsNot Nothing Then
   If Security.CanEditPost Then
    Exit Sub
   End If
  End If
  Throw New XmlRpcFaultException(0, GetString("Blog Access Denied", "Your access to this blog is not permitted. Please check your credentials."))
 End Sub

 Private Sub RequireEditPermission(existingPost As PostInfo)
  If Security IsNot Nothing Then
   If Security.CanEditPost Then
    Exit Sub
   End If
   If Security.CanEditThisPost(existingPost) Then
    Exit Sub
   End If
  End If
  Throw New XmlRpcFaultException(0, GetString("Blog Access Denied", "Your access to this blog is not permitted. Please check your credentials."))
 End Sub

 Private Sub RequireAddPermission()
  If Security IsNot Nothing Then
   If Security.CanAddPost Then
    Exit Sub
   End If
  End If
  Throw New XmlRpcFaultException(0, GetString("Blog Access Denied", "Your access to this blog is not permitted. Please check your credentials."))
 End Sub

 Private Sub RequireAccessPermission()
  If Security IsNot Nothing Then
   If Security.CanAddPost Or Security.CanEditPost Then
    Exit Sub
   End If
  End If
  Throw New XmlRpcFaultException(0, GetString("Blog Access Denied", "Your access to this blog is not permitted. Please check your credentials."))
 End Sub
#End Region

#Region " Private Context Methods "
 Private Sub InitializeMethodCall(username As String, password As String, requestedBlogId As String, requestedPostId As String)
  Try
   ' Set up the context
   Context.Request.Params.ReadValue("TabId", TabId)
   Context.Request.Params.ReadValue("ModuleId", ModuleId)
   Context.Request.Params.ReadValue("Blog", BlogId)
   GetPortalSettings()
   Settings = ModuleSettings.GetModuleSettings(ModuleId)
   If Not Settings.AllowWLW Then
    Throw New XmlRpcFaultException(0, GetString("Access Denied", "Access to the blog through this API has been denied. Please contact the Portal Administrator."))
   Else
    UserInfo = ValidateUser(username, password, Context.Request.UserHostAddress)
   End If
   If requestedPostId <> "" Then
    PostId = CInt(requestedPostId)
    RequestedPost = PostsController.GetPost(PostId, ModuleId, Locale)
    BlogId = RequestedPost.BlogID
   ElseIf requestedBlogId <> "" Then
    BlogId = CInt(requestedBlogId)
   End If
   ' Check for user access to the blog
   If BlogId > -1 Then
    RequestedBlog = BlogsController.GetBlog(BlogId, UserInfo.UserID, Locale)
    Security = New ContextSecurity(ModuleId, TabId, RequestedBlog, UserInfo)
   End If
   UserTimeZone = PortalSettings.TimeZone
   If UserInfo.Profile.PreferredTimeZone IsNot Nothing Then
    UserTimeZone = UserInfo.Profile.PreferredTimeZone
   End If
  Catch ex As Exception
   LogException(ex)
   Throw
  End Try
 End Sub

 Private Sub GetPortalSettings()
  Try
   Dim oTabController As TabController = New TabController
   Dim oTabInfo As TabInfo = oTabController.GetTab(TabId, PortalId, False)
   PortalId = oTabInfo.PortalID
   PortalSettings = New PortalSettings(PortalId)
  Catch ex As XmlRpcFaultException
   Throw
  Catch generatedExceptionName As Exception
   Throw New XmlRpcFaultException(0, GetString("PortalLoadError", "Please check your URL to make sure you entered the correct URL for your blog.  The blog posting URL is available through the blog settings for your blog."))
  End Try
 End Sub

 Private Function GetPortalIDFromAlias(portalAlias As String) As Integer
  'Get the PortalAlias based on the Request object
  Dim pc As New PortalController
  Try
   Dim pAlias As PortalAliasInfo = PortalAliasController.Instance.GetPortalAlias(portalAlias)
   If pAlias IsNot Nothing Then
    PortalId = pAlias.PortalID
   End If
  Catch generatedExceptionName As Exception
   ' Just ignore the errors if any and pass up 0 for the portalID.  0 for the portalID
   ' will throw an error in the calling procedure.
  End Try
  Return PortalId
 End Function
#End Region

#Region " Various Private Methods "
 Private Sub LogException(ex As Exception)
  Try
   DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
  Catch
  End Try
 End Sub

 Private Function GetString(key As String, defaultValue As String) As String
  Dim retValue As String = DotNetNuke.Services.Localization.Localization.GetString(key, "/DesktopModules/blog/App_LocalResources/blogpost")
  If retValue Is Nothing Then Return defaultValue
  Return retValue
 End Function
#End Region

#Region " Data Handling Methods "
 Private Sub AddCategoriesAndKeyWords(ByRef newPost As PostInfo, post As Post)
  Dim terms As New List(Of TermInfo)
  terms.AddRange(TermsController.GetTermList(ModuleId, post.mt_keywords, 1, True, Locale))
  If Settings.VocabularyId > 1 Then
   terms.AddRange(TermsController.GetTermList(ModuleId, post.categories.ToList, Settings.VocabularyId, False, Locale))
  End If
  newPost.Terms.Clear()
  newPost.Terms.AddRange(terms)
  PostsController.UpdatePost(newPost, UserInfo.UserID)
 End Sub

 Private Sub PublishToJournal(newPost As PostInfo)
  Dim journalUserId As Integer
  If newPost.CreatedByUserID <> UserInfo.UserID AndAlso Not RequestedBlog.PublishAsOwner Then
   journalUserId = UserInfo.UserID
  Else
   journalUserId = RequestedBlog.OwnerUserId
  End If
  Integration.JournalController.AddBlogPostToJournal(newPost, PortalId, newPost.TabID, journalUserId, newPost.PermaLink(PortalSettings))
 End Sub
#End Region

#Region " Attachment Handling Methods "
 Private Sub HandleAttachments(ByRef newPost As PostInfo)

  ' Handle attachments
  Dim contents As String = newPost.Content
  If Not String.IsNullOrEmpty(newPost.Summary) Then contents &= newPost.Summary
  Dim d As New DirectoryInfo(GetTempPostDirectoryMapPath(BlogId))
  Dim targetDir As String = GetPostDirectoryMapPath(newPost)
  If Not Directory.Exists(targetDir) Then
   Directory.CreateDirectory(targetDir)
  Else
   For Each f As String In Directory.GetFiles(targetDir) ' remove deprecated files
    If Not contents.Contains(Path.GetFileName(f)) Then
     Try
      File.Delete(f)
     Catch ex As Exception
      ' we're not too bothered if this doesn't succeed
     End Try
    End If
   Next
  End If
  If d.Exists Then
   For Each f As FileInfo In d.GetFiles
    If contents.Contains(f.Name) Then
     f.MoveTo(targetDir & f.Name)
    End If
   Next
  End If
  newPost.Content = newPost.Content.Replace(String.Format("Blog/Files/{0}/_temp_images/", BlogId), String.Format("Blog/Files/{0}/{1}/", BlogId, newPost.ContentItemId))
  If Not String.IsNullOrEmpty(newPost.Summary) Then newPost.Summary = newPost.Summary.Replace(String.Format("Blog/Files/{0}/_temp_images/", BlogId), String.Format("Blog/Files/{0}/{1}/", BlogId, newPost.ContentItemId))

 End Sub

 Private Sub CheckForMainImage(ByRef newPost As PostInfo)
  Dim contents As String = newPost.Content
  ' Find first image in contents
  Dim m As Match = Regex.Match(contents, "&lt;img .*?&gt;")
  If m.Success Then
   Dim preceding As String = contents.Substring(0, m.Index)
   preceding = Regex.Replace(preceding, "&lt;.*?&gt;", "") ' remove all HTML tags
   preceding = Regex.Replace(preceding, "\s", "") ' remove all whitespace
   preceding = Regex.Replace(preceding, "[\r\n]", "") ' remove newlines
   If preceding = "" Then
    ' Begin extraction process
    Dim srcM As Match = Regex.Match(m.Value, "(?i)src=&quot;(.*?)\.\w+&quot;(?-i)")
    If srcM.Success Then ' successfully parsed filename
     newPost.Image = Path.GetFileName(srcM.Groups(1).Value)
     ' Now remove image from contents
     preceding = contents.Substring(0, m.Index)
     Dim remaining As String = contents.Substring(m.Index + m.Length)
     For Each tagM As Match In Regex.Matches(preceding, "&lt;(\w+).*?&gt;", RegexOptions.RightToLeft)
      Dim closingTag As String = "&lt;/" & tagM.Groups(1).Value & "&gt;"
      If remaining.StartsWith(closingTag) Then
       preceding = preceding.Substring(0, tagM.Index) & preceding.Substring(tagM.Index + tagM.Length)
       remaining = remaining.Substring(closingTag.Length)
      End If
     Next
     newPost.Content = Trim(preceding & remaining)
    End If
   End If
  End If
 End Sub
#End Region

End Class