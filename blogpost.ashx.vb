'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2012
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

Imports System
Imports System.Data
Imports System.Text.RegularExpressions
Imports DotNetNuke.Modules.Blog.Components.Controllers
Imports DotNetNuke.Modules.Blog.Components.Blogger
Imports DotNetNuke.Modules.Blog.Components.Common
Imports DotNetNuke.Data
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Entities.Users
Imports System.Collections
Imports CookComputing.XmlRpc
Imports DotNetNuke.Security.Membership
Imports System.IO
Imports System.Web
Imports DotNetNuke.Entities.Content.Taxonomy
Imports System.Linq
Imports DotNetNuke.Modules.Blog.Components.Settings
Imports DotNetNuke.Modules.Blog.Components.WordPress
Imports DotNetNuke.Modules.Blog.Components.MoveableType
Imports DotNetNuke.Modules.Blog.Components.MetaWeblog
Imports DotNetNuke.Modules.Blog.Components.Entities

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
 Private Property BlogSettings As BlogSettings = Nothing
 Private Property ModuleName As String = String.Empty
 Private Property ExpanderScript As String = String.Empty
 Private Property UserInfo As UserInfo = Nothing
 Private Property TabId As Integer = -1
 Private Property ModuleId As Integer = -1
 Private Property PortalId As Integer = -1
 Private Property BlogId As Integer = -1
 Private Property Blog As BlogInfo = Nothing
 Private Property EntryId As Integer = -1
 Private Property Entry As EntryInfo = Nothing
 Private Property UnAuthorized As Boolean = True
 Private Property Security As ModuleSecurity = Nothing

#Region " Method Implementations "
 Public Function getUsersBlogs(appKey As String, username As String, password As String) As BlogInfoStruct() Implements IBlogger.getUsersBlogs
  InitializeMethodCall(username, password, "", "")

  Dim blogs As New List(Of BlogInfoStruct)
  Try
   Dim blogsList As List(Of BlogInfo) = BlogController.GetUsersBlogs(PortalId, UserInfo.Username)
   If Not blogsList Is Nothing Then
    For Each blog As BlogInfo In blogsList
     blogs.Add(New BlogInfoStruct() With {.blogid = blog.BlogID.ToString, .blogName = blog.Title, .url = GetRedirectUrl(TabId)})
    Next
   Else
    Throw New BlogPostException("NoModulesForUser", "There is no blog associated with this user account.  Please enter a user account which has been associated with a blog.")
   End If
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

  Dim post As Post
  Try
   post = ToPost(Entry)
   Dim regexPattern As String = String.Empty
   Dim options As RegexOptions = RegexOptions.Singleline Or RegexOptions.IgnoreCase
   ' Look for and replace relative images with absolute images
   regexPattern = "(src="")/"
   If Not String.IsNullOrEmpty(post.mt_excerpt) Then
    post.mt_excerpt = Regex.Replace(post.mt_excerpt, regexPattern, "$1http://" + Context.Request.Url.Host + "/", options)
   End If
   If Not String.IsNullOrEmpty(post.description) Then
    post.description = Regex.Replace(post.description, regexPattern, "$1http://" + Context.Request.Url.Host + "/", options)
   End If
   If Not String.IsNullOrEmpty(post.mt_text_more) Then
    post.mt_text_more = Regex.Replace(post.mt_text_more, regexPattern, "$1http://" + Context.Request.Url.Host + "/", options)
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
  Return post

 End Function

 Public Function getCategories_WordPress(blogid As String, username As String, password As String) As Components.WordPress.CategoryInfo() Implements IWordPress.getCategories
  InitializeMethodCall(username, password, blogid, "")
  RequireAccessPermission()

  If BlogSettings.VocabularyId > 1 Then
   Dim termController As ITermController = DotNetNuke.Entities.Content.Common.Util.GetTermController()
   Dim colCategories As IQueryable(Of Term) = termController.GetTermsByVocabulary(BlogSettings.VocabularyId)
   Dim res(colCategories.Count - 1) As Components.WordPress.CategoryInfo
   Dim i As Integer = 0
   For Each objTerm As Term In colCategories
    res(i).categoryId = objTerm.TermId
    If objTerm.ParentTermId = Common.Utilities.Null.NullInteger Then
     ' the way vocabs are setup for categories, this shouldn't happen
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

  Dim colCategories As List(Of Term)
  colCategories = Entry.Terms
  Dim res(colCategories.Count - 1) As Category
  Dim i As Integer = 0
  For Each objTerm As TermInfo In colCategories
   If objTerm.VocabularyId > 1 Then
    res(i).categoryId = objTerm.TermId.ToString()
    res(i).categoryName = objTerm.Name
   End If
   i += 1
  Next
  Return res

 End Function

 Public Function setPostCategories(postid As String, username As String, password As String, categories As Category()) As Boolean Implements IMoveableType.setPostCategories
  ' The set is handled in the saving
  'InitializeMethodCall(username, password)

  'Dim EntryController As New EntryController
  'Dim objEntry As EntryInfo = EntryController.GetEntry(Convert.ToInt32(postid), _portalSettings.PortalId)
  'Dim terms As New List(Of Term)

  'For Each t As Category In categories
  '    Dim objTerm As Term = Components.Integration.Terms.GetTermById(Convert.ToInt32(t.categoryId), _blogSettings.VocabularyId)
  '    terms.Add(objTerm)
  'Next

  'objEntry.Terms.Clear()
  'objEntry.Terms.AddRange(terms)

  'EntryController.UpdateEntry(objEntry, _tabId, _portalSettings.PortalId, _blogSettings.VocabularyId)

  Return True
 End Function

 Public Function getRecentPosts(blogid As String, username As String, password As String, numberOfPosts As Integer) As Post() Implements IMetaWeblog.getRecentPosts
  InitializeMethodCall(username, password, blogid, "")
  RequireAccessPermission()

  Dim posts As New List(Of Post)
  Try
   Dim arEntries As List(Of EntryInfo) = EntryController.GetEntriesByBlog(CInt(blogid), DateTime.Now.ToUniversalTime(), BlogSettings.RecentEntriesMax, 1, True, True)
   For Each entry As EntryInfo In arEntries
    posts.Add(ToPost(entry))
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

  Dim styleId As String = String.Empty
  Dim styleDetectionPost As Boolean = False
  Dim moduleId As Integer = -1

  Try
   ' Check to see if this a style detection post.
   styleDetectionPost = (post.title.Length > 116 AndAlso post.title.Substring(80, 36) = "3bfe001a-32de-4114-a6b4-4005b770f6d7")

   ' Check to see if a styleId is passed through the QueryString
   ' however, we'll only do this if we are creating a post for style detection.
   If styleDetectionPost Then
    If TabId > -1 Then
     styleId = TabId.ToString
    End If
   End If

   ' Add the new entry
   MakeImagesRelative(post)
   Dim newEntry As EntryInfo = ToEntry(post)
   newEntry.Published = publish
   newEntry = EntryController.AddEntry(newEntry, TabId)
   ProcessItemImages(newEntry, PortalSettings.HomeDirectoryMapPath & "Blog")

   ' Add keywords and categories
   If Not styleDetectionPost Then
    AddCategoriesAndKeyWords(newEntry, post)
   End If

   ' Publish to journal
   If Not styleDetectionPost And newEntry.Published Then
    PublishToJournal(newEntry)
   End If

   ' If this is a style detection post, then we write to the Blog_MetaWeblogData table to note
   ' that this post is a new post.  We're just using the DAL+ here to manage this feature.
   If styleDetectionPost Then
    Dim blogUrl As String = (New DotNetNuke.Security.PortalSecurity).InputFilter(newEntry.PermaLink, DotNetNuke.Security.PortalSecurity.FilterFlag.NoSQL)
    ' DW - 01/27/2010 - Updated to ensure that at least 1 row exists in this table.
    Dim sSQL As String = "DECLARE @Count INT; SELECT @Count = (SELECT Count(*) FROM {databaseOwner}{objectQualifier}Blog_MetaWeblogData); IF @Count = 0 INSERT INTO {databaseOwner}{objectQualifier}Blog_MetaWeblogData SELECT '" & blogUrl & "' ELSE UPDATE {databaseOwner}{objectQualifier}Blog_MetaWeblogData SET TempInstallUrl = '" & blogUrl & "'"
    DataProvider.Instance.ExecuteSQL(sSQL)
   End If

   Return newEntry.EntryID.ToString

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

 Public Function editPost(postid As String, username As String, password As String, post As Post, publish As Boolean) As Boolean Implements IMetaWeblog.editPost
  InitializeMethodCall(username, password, "", postid)
  RequireEditPermission()

  Dim success As Boolean = False
  Try
   MakeImagesRelative(post)
   Dim newEntry As EntryInfo = ToEntry(post)
   ProcessItemImages(newEntry, PortalSettings.HomeDirectoryMapPath & "Blog")
   AddCategoriesAndKeyWords(newEntry, post)
   If (Not Entry.Published) And newEntry.Published Then
    ' First published
    PublishToJournal(newEntry)
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
   EntryController.DeleteEntry(Entry.EntryID, Entry.ContentItemId, Entry.BlogID, PortalId, BlogSettings.VocabularyId)
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

 Public Function getWPCategories(blogid As String, username As String, password As String) As Components.MetaWeblog.CategoryInfo()
  InitializeMethodCall(username, password, blogid, "")
  RequireAccessPermission()

  Dim categories As New List(Of Components.MetaWeblog.CategoryInfo)
  Try
   If BlogSettings.VocabularyId > 1 Then
    Dim termController As ITermController = DotNetNuke.Entities.Content.Common.Util.GetTermController()
    Dim colCategories As IQueryable(Of Term) = termController.GetTermsByVocabulary(BlogSettings.VocabularyId)
    For Each objTerm As Term In colCategories
     categories.Add(New Components.MetaWeblog.CategoryInfo() With {.categoryId = objTerm.TermId.ToString, .categoryName = objTerm.Name, .description = objTerm.Description, .htmlUrl = "http://google.com", .parentId = objTerm.ParentTermId.ToString, .rssUrl = "http://google.com"})
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

  Dim categories As New List(Of Components.MetaWeblog.MetaWebLogCategoryInfo)
  Try
   If BlogSettings.VocabularyId > 1 Then
    Dim termController As ITermController = DotNetNuke.Entities.Content.Common.Util.GetTermController()
    Dim colCategories As IQueryable(Of Term) = termController.GetTermsByVocabulary(BlogSettings.VocabularyId)
    For Each objTerm As Term In colCategories
     categories.Add(New Components.MetaWeblog.MetaWebLogCategoryInfo() With {.description = objTerm.Description, .htmlUrl = "http://google.com", .rssUrl = "http://google.com"})
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
  Try

   Dim virtualPath As String
   Dim mediaObjectName As String = String.Empty
   Dim fullFilePathAndName As String = String.Empty
   info.url = ""

   Try

    ' Shorten WindowsLiveWriter and create one file name.
    mediaObjectName = mediaobject.name.Replace("WindowsLiveWriter", "WLW")
    mediaObjectName = mediaObjectName.Replace("/", "-").Replace(" ", "_")
    mediaObjectName = HttpContext.Current.Server.UrlEncode(mediaObjectName)

    ' Check permitted file types
    Dim strExtension As String = Path.GetExtension(mediaObjectName).Replace(".", "")
    If String.IsNullOrEmpty(strExtension) OrElse Not DotNetNuke.Entities.Host.Host.AllowedExtensionWhitelist.IsAllowedExtension(strExtension) Then
     Throw New XmlRpcFaultException(0, GetString("SaveError", String.Format("File {0} refused. Uploading this type of file is not allowed.", mediaObjectName)))
    End If

    virtualPath = "Blog/Files/" & blogid.ToString() & "/_temp_images/" & mediaObjectName

    fullFilePathAndName = PortalSettings.HomeDirectoryMapPath + virtualPath.Replace("/", "\")
    IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(fullFilePathAndName))

    Using output As New FileStream(fullFilePathAndName, FileMode.Create)
     Using bw As New BinaryWriter(output)
      bw.Write(mediaobject.bits)
     End Using
    End Using

   Catch exc As IOException
    Throw New XmlRpcFaultException(0, GetString("ImageSaveError", "An error occurred while saving an image related to this blog post."))
   End Try
   Dim finalUrl As String = PortalSettings.HomeDirectory.Replace("\", "/")

   finalUrl = finalUrl & virtualPath
   info.url = finalUrl
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

#Region " Post/Entry Conversion Methods "
 Public Function ToPost(entry As EntryInfo) As Post
  Dim post As New Post

  post.mt_allow_comments = entry.AllowComments.ToInt
  'post.mt_allow_pings
  post.categories = entry.EntryCategories(BlogSettings.VocabularyId).ToStringArray
  post.description = HttpUtility.HtmlDecode(entry.Entry)
  post.dateCreated = Globals.GetLocalAddedTime(entry.AddedDate, PortalSettings.PortalId, UserInfo)
  post.pubDate = entry.AddedDate
  post.date_created_gmt = entry.AddedDate
  'post.mt_text_more =
  post.postid = entry.EntryID.ToString
  post.mt_keywords = String.Join(",", entry.Tags.ToStringArray)
  post.link = entry.PermaLink
  post.permalink = entry.PermaLink
  'post.mt_tb_ping_urls =
  post.title = entry.Title
  'post.wp_slug =
  'post.wp_password =
  'post.wp_page_parent_id =
  'post.wp_page_order =
  'post.wp_author_id =
  post.mt_excerpt = HttpUtility.HtmlDecode(entry.Description)
  'post.mt_tb_ping_urls =
  post.publish = entry.Published

  Return post
 End Function

 Public Function ToEntry(post As Post) As EntryInfo
  Dim entry As New EntryInfo
  If Not String.IsNullOrEmpty(post.postid) Then
   entry.EntryID = post.postid.ToInt
  End If
  entry.BlogID = BlogId
  entry.Title = post.title
  If BlogSettings.AllowSummaryHtml Then
   entry.Description = HttpUtility.HtmlEncode(post.mt_excerpt)
  Else
   entry.Description = Globals.RemoveMarkup(post.mt_excerpt)
  End If
  entry.Entry = HttpUtility.HtmlEncode(post.description)
  If post.dateCreated.Year > 1 Then
   ' WLW manages the TZ offset automatically
   entry.AddedDate = post.dateCreated
  End If
  entry.Published = post.publish
  entry.AllowComments = post.mt_allow_comments.ToBool
  entry.PermaLink = post.permalink
  entry.CreatedUserId = UserInfo.UserID
  entry.TabID = TabId
  entry.ModuleID = ModuleId
  Return entry
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
   If Security.CanEdit Then
    Exit Sub
   End If
  End If
  Throw New XmlRpcFaultException(0, GetString("Blog Access Denied", "Your access to this blog is not permitted. Please check your credentials."))
 End Sub

 Private Sub RequireAddPermission()
  If Security IsNot Nothing Then
   If Security.CanAddEntry Then
    Exit Sub
   End If
  End If
  Throw New XmlRpcFaultException(0, GetString("Blog Access Denied", "Your access to this blog is not permitted. Please check your credentials."))
 End Sub

 Private Sub RequireAccessPermission()
  If Security IsNot Nothing Then
   If Security.CanAddEntry Or Security.CanEdit Then
    Exit Sub
   End If
  End If
  Throw New XmlRpcFaultException(0, GetString("Blog Access Denied", "Your access to this blog is not permitted. Please check your credentials."))
 End Sub
#End Region

#Region " Private Context Methods "
 Private Sub InitializeMethodCall(username As String, password As String, requestedBlogId As String, requestedPostId As String)
  ' NOTE: CP - This method should be updated to support ghost writing, which is a blog level setting and is tied to core permissions (a column in the module permissions grid)
  ' NOTE: PAD - this is impossible as (a) WLW doesn't allow for switching between blogs and (b) the blog is portal-wide data, whereas the permissions are module-wide data
  '             the only way we could implement this is to have a new pattern for the access url which includes module and blog ids.
  Try
   ' Set up the context
   Context.Request.Params.ReadValue("TabId", TabId)
   Context.Request.Params.ReadValue("ModuleId", ModuleId)
   Context.Request.Params.ReadValue("BlogId", BlogId)
   GetPortalSettings()
   BlogSettings = BlogSettings.GetBlogSettings(PortalSettings.PortalId, TabId)
   If Not BlogSettings.AllowWLW Then
    Throw New XmlRpcFaultException(0, GetString("Access Denied", "Access to the blog through this API has been denied. Please contact the Portal Administrator."))
   Else
    UserInfo = ValidateUser(username, password, Me.Context.Request.UserHostAddress)
   End If
   If requestedPostId <> "" Then
    EntryId = CInt(requestedPostId)
    Entry = EntryController.GetEntry(EntryId, PortalId)
    BlogId = Entry.BlogID
   ElseIf requestedBlogId <> "" Then
    BlogId = CInt(requestedBlogId)
   End If
   ' Check for user access to the blog
   If Me.BlogId > -1 Then
    Blog = BlogController.GetBlog(Me.BlogId)
    Security = New ModuleSecurity(ModuleId, TabId, Blog, UserInfo)
   End If
  Catch ex As Exception
   LogException(ex)
   Throw
  End Try
 End Sub

 Private Sub GetPortalSettings()
  Try
   Dim Request As HttpRequest = Me.Context.Request
   Dim oTabController As TabController = New TabController
   Dim oTabInfo As TabInfo = oTabController.GetTab(TabId, PortalId, False)
   PortalId = oTabInfo.PortalID
   Dim pac As New PortalAliasController
   Dim paColl As PortalAliasCollection = pac.GetPortalAliasByPortalID(PortalId)
   Dim pai As PortalAliasInfo = Nothing
   For Each paiKey As String In paColl.Keys
    pai = paColl(paiKey)
    Exit For
   Next
   Dim portalController As New PortalController
   Dim pi As PortalInfo = portalController.GetPortal(PortalId)
   PortalSettings = New PortalSettings(TabId, pai)
  Catch ex As XmlRpcFaultException
   Throw
  Catch generatedExceptionName As Exception
   Throw New XmlRpcFaultException(0, GetString("PortalLoadError", "Please check your URL to make sure you entered the correct URL for your blog.  The blog posting URL is available through the blog settings for your blog.")) ' & "Error:" & generatedExceptionName.ToString() & " PortalId: " & portalID.ToString()
  End Try
 End Sub

 Private Function GetPortalIDFromAlias(portalAlias As String) As Integer
  'Get the PortalAlias based on the Request object
  Dim pc As New PortalController
  Try
   Dim dr As IDataReader = DotNetNuke.Data.DataProvider.Instance().GetPortalByAlias(portalAlias)
   If dr.Read() Then
    PortalId = dr.GetInt32(dr.GetOrdinal("PortalID"))
   End If
   dr.Close()
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

 Private Function GetString(ByVal key As String, ByVal defaultValue As String) As String
  Dim retValue As String = DotNetNuke.Services.Localization.Localization.GetString(key, "/DesktopModules/blog/App_LocalResources/blogpost")
  If retValue Is Nothing Then Return defaultValue
  Return retValue
 End Function

 Private Function GetRedirectUrl(ByVal TabId As Integer) As String
  Dim appPath As String = HttpContext.Current.Request.ApplicationPath
  If appPath = "/" Then
   appPath = String.Empty
  End If
  Dim returnUrl As String = appPath + "/DesktopModules/Blog/blogpostredirect.aspx?tab=" + TabId.ToString
  Return returnUrl
 End Function
#End Region

#Region " Data Handling Methods "
 Private Sub AddCategoriesAndKeyWords(ByRef newEntry As EntryInfo, post As Post)
  Dim terms As New List(Of Term)
  For Each s As String In post.mt_keywords.Replace(";", ",").Split(","c)
   If s.Length > 0 Then
    Dim newTerm As Term = Components.Integration.Terms.CreateAndReturnTerm(s.Trim, 1)
    terms.Add(newTerm)
   End If
  Next
  If BlogSettings.VocabularyId > 1 Then
   For Each s As String In post.categories
    If s.Length > 0 Then
     Dim newTerm As Term = Components.Integration.Terms.CreateAndReturnTerm(s.Trim, BlogSettings.VocabularyId)
     terms.Add(newTerm)
    End If
   Next
  End If
  newEntry.Terms.Clear()
  newEntry.Terms.AddRange(terms)
  EntryController.UpdateEntry(newEntry, newEntry.TabID, PortalId, BlogSettings.VocabularyId)
 End Sub

 Private Sub PublishToJournal(newEntry As EntryInfo)
  Dim cntIntegration As New Components.Integration.Journal()
  Dim journalUserId As Integer
  Select Case Blog.AuthorMode
   Case Constants.AuthorMode.GhostMode
    journalUserId = Blog.UserID
   Case Else
    journalUserId = UserInfo.UserID
  End Select
  cntIntegration.AddBlogEntryToJournal(newEntry, PortalId, newEntry.TabID, journalUserId, newEntry.PermaLink)
 End Sub
#End Region

#Region " Image Handling Methods "
 Private Sub MakeImagesRelative(ByRef post As Post)
  ' Check first to see if we're running on a port other than port 80
  Dim port As String = Context.Request.Url.Port.ToString()
  If port <> "80" Then
   port = ":" & port
  Else
   port = String.Empty
  End If

  Dim urlDomain As String = Context.Request.Url.Host & port
  Dim options As RegexOptions = RegexOptions.IgnoreCase Or RegexOptions.Singleline
  Dim expression As String = "((?:src|href)\s*?=\s*?"")http[s]*://" + urlDomain
  If Not String.IsNullOrEmpty(post.mt_excerpt) Then
   post.mt_excerpt = Regex.Replace(post.mt_excerpt, expression, "$1", options)
  End If
  If Not String.IsNullOrEmpty(post.description) Then
   post.description = Regex.Replace(post.description, expression, "$1", options)
  End If
  If Not String.IsNullOrEmpty(post.mt_text_more) Then
   post.mt_text_more = Regex.Replace(post.mt_text_more, expression, "$1", options)
  End If
 End Sub

 Private Sub ProcessItemImages(ByRef entry As EntryInfo, ByVal rootPath As String)
  Dim imageUrls As New ArrayList

  Try

   ' When the newMediaObject method of the MetaWeblog API is called, we don't 
   ' have enough information to specify the blog entry id for the image path.
   ' So, we place the images in a folder named _temp_images until the 
   ' EditItem method is called (which calls this procedure).  EditItem contains
   ' the item parameter which has an itemId corresponding to the Entry Id of the 
   ' blog post.  We'll replace _temp_images with this EntryId and we'll 
   ' move the images to the right folder.  In order to move the images to the right
   ' folder, we'll use Regex to find the images in the post.

   Dim regexSrc As String = "<img[^>]+?_temp_images/.*?>"
   Dim regexHref As String = "<a[^>]+?(?:png|jpg|jpeg|gif)[^>]+?>[^<]*?<img[^>]+?src=""[^""]+?""[^>]+>[^<]*?</a>"
   ' Note that the inner Regex patterns required a group named 'src'
   Dim regexInnerSrc As String = "src=""(?<src>[^""]+?)"""
   Dim regexInnerHref As String = "href=""(?<src>[^""]+?)"""

   Dim options As RegexOptions = RegexOptions.IgnoreCase Or RegexOptions.Singleline
   Dim input As String = HttpUtility.HtmlDecode(entry.Description + entry.Entry)

   If Not String.IsNullOrEmpty(input) Then
    FindImageMatch(input, regexSrc, regexInnerSrc, options, imageUrls)
    FindImageMatch(input, regexHref, regexInnerHref, options, imageUrls)
   End If

   ' OK, now that we have the matches stored in the imageUrls Arraylist, we'll use this to 
   ' move the images to the right location.  

   For Each url As String In imageUrls
    Try
     Dim moveFromPath As String = HttpContext.Current.Server.MapPath(url)


     Dim moveToPath As String = moveFromPath.Replace("_temp_images", entry.EntryID.ToString())
     Dim moveToFolderPath As String = moveToPath.Substring(0, moveToPath.LastIndexOf("\"))
     ' Make sure the directory exists
     If Not Directory.Exists(moveToFolderPath) Then
      ' No problem, we'll just create it!
      Directory.CreateDirectory(moveToFolderPath)
     End If
     ' File may already have been moved.  We'll check first to see.
     ' Files will haev already been moved in the case where an entry is
     ' reposted from Windows Live Writer.
     If System.IO.File.Exists(moveFromPath) AndAlso IsValidImageLocation(moveFromPath, rootPath) Then
      'Check to see if we need to overwrite an existing image
      If System.IO.File.Exists(moveToPath) Then
       System.IO.File.Delete(moveToPath)
      End If
      System.IO.File.Move(moveFromPath, moveToPath)
     End If
    Catch ex As Exception
     ' We'll log the error and fail silently so we can attempt to save the other 
     ' images.  
     DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
    End Try
   Next

   ' Check for any non-image files left behind
   For Each tempFile As Match In Regex.Matches(HttpUtility.HtmlDecode(entry.Entry), """([^""]*_temp_images/[^""]*)""")
    Try
     Dim moveFromPath As String = HttpContext.Current.Server.MapPath(tempFile.Groups(1).Value)
     Dim strExtension As String = Path.GetExtension(moveFromPath).Replace(".", "")
     If IsValidImageLocation(moveFromPath, rootPath) Then
      If Not String.IsNullOrEmpty(strExtension) AndAlso DotNetNuke.Entities.Host.Host.AllowedExtensionWhitelist.IsAllowedExtension(strExtension) Then
       Dim moveToPath As String = moveFromPath.Replace("_temp_images", entry.EntryID.ToString())
       Dim moveToFolderPath As String = moveToPath.Substring(0, moveToPath.LastIndexOf("\"))
       ' Make sure the directory exists
       If Not Directory.Exists(moveToFolderPath) Then
        ' No problem, we'll just create it!
        Directory.CreateDirectory(moveToFolderPath)
       End If
       ' File may already have been moved.  We'll check first to see.
       ' Files will haev already been moved in the case where an entry is
       ' reposted from Windows Live Writer.
       If System.IO.File.Exists(moveFromPath) Then
        'Check to see if we need to overwrite an existing image
        If System.IO.File.Exists(moveToPath) Then
         System.IO.File.Delete(moveToPath)
        End If
        System.IO.File.Move(moveFromPath, moveToPath)
       End If
      Else ' we have to delete the file
       System.IO.File.Delete(moveFromPath)
      End If
     End If
    Catch ex As Exception
     ' We'll log the error and fail silently so we can attempt to save the other 
     ' images.  
     DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
    End Try
   Next

   ' Clean up old files
   Try
    Dim path As String = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings.HomeDirectoryMapPath & "Blog\Files\" & entry.BlogID.ToString & "\_temp_images"
    ' we need to make sure the directory exists
    If IO.Directory.Exists(path) Then
     Dim colFiles As Array = New IO.DirectoryInfo(path).GetFiles()

     If colFiles IsNot Nothing Then
      For Each f As IO.FileInfo In (colFiles)
       If f.CreationTime < Now.AddHours(-1) Then
        Try
         f.Delete()
        Catch ex1 As Exception
         DotNetNuke.Services.Exceptions.Exceptions.LogException(ex1)
        End Try
       End If
      Next
     End If
    End If
   Catch ex As Exception
    DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
   End Try

   ' Finally, we'll update the URLs
   If Not String.IsNullOrEmpty(entry.Entry) Then
    entry.Entry = entry.Entry.Replace("_temp_images", entry.EntryID.ToString())
   End If
   If Not String.IsNullOrEmpty(entry.Description) Then
    entry.Description = entry.Description.Replace("_temp_images", entry.EntryID.ToString())
   End If
   'Yes!  We made it!!  'The images should be tucked in bed
  Catch ex As Exception
   DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
  End Try
 End Sub

 Private Sub FindImageMatch(ByVal input As String, ByVal sRegex As String, ByVal regexInner As String, ByVal options As RegexOptions, ByVal imageUrls As ArrayList)
  Dim matches As MatchCollection = Regex.Matches(input, sRegex, options)
  For Each match As Match In matches
   ' extract the src attribute from the image
   Dim regexSrc As String = regexInner
   input = match.Value
   Dim src As Match = Regex.Match(input, regexSrc, options)
   If Not src Is Nothing AndAlso src.Groups("src").Captures.Count > 0 Then
    ' We have an image Url
    imageUrls.Add(src.Groups("src").Captures(0).Value)
   End If
  Next
 End Sub

 Private Shared Function IsValidImageLocation(ByVal path As String, ByVal rootpath As String) As Boolean
  Return path.StartsWith(rootpath)
 End Function
#End Region

End Class