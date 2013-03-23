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
Imports DotNetNuke.Modules.Blog.Services.WLW.Blogger
Imports DotNetNuke.Modules.Blog.Common
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
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports DotNetNuke.Modules.Blog.Services.WLW
Imports DotNetNuke.Modules.Blog.Services.WLW.WordPress
Imports DotNetNuke.Modules.Blog.Services.WLW.MoveableType
Imports DotNetNuke.Modules.Blog.Services.WLW.MetaWeblog
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Entities.Entries
Imports DotNetNuke.Modules.Blog.Common.Extensions
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
 Private Property Blog As BlogInfo = Nothing
 Private Property EntryId As Integer = -1
 Private Property Entry As EntryInfo = Nothing
 Private Property UnAuthorized As Boolean = True
 Private Property Security As ContextSecurity = Nothing
 Private Property UserTimeZone As TimeZoneInfo = Nothing

#Region " Method Implementations "
 Public Function getUsersBlogs(appKey As String, username As String, password As String) As BlogInfoStruct() Implements IBlogger.getUsersBlogs
  InitializeMethodCall(username, password, "", "")

  Dim blogs As New List(Of BlogInfoStruct)
  Try
   For Each blog As BlogInfo In BlogsController.GetBlogsByModule(ModuleId, UserInfo.UserID).Values.Where(Function(b)
                                                                                                          Return b.CreatedByUserID = UserInfo.UserID Or b.CanAdd Or b.CanEdit
                                                                                                         End Function).ToList
    blogs.Add(New BlogInfoStruct() With {.blogid = blog.BlogID.ToString, .blogName = blog.Title, .url = GetRedirectUrl(TabId, blog.ModuleID)})
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
    If objTerm.ParentTermId = DotNetNuke.Common.Utilities.Null.NullInteger Then
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

  Dim colCategories As List(Of TermInfo)
  colCategories = Entry.Terms
  Dim res(colCategories.Count - 1) As Category
  Dim i As Integer = 0
  For Each objTerm As Term In colCategories
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

  'Dim EntriesController As New EntriesController
  'Dim objEntry As EntryInfo = EntriesController.GetEntry(Convert.ToInt32(postid), _portalSettings.PortalId)
  'Dim terms As New List(Of Term)

  'For Each t As Category In categories
  '    Dim objTerm As Term = Integration.Terms.GetTermById(Convert.ToInt32(t.categoryId), _blogSettings.VocabularyId)
  '    terms.Add(objTerm)
  'Next

  'objEntry.Terms.Clear()
  'objEntry.Terms.AddRange(terms)

  'EntriesController.UpdateEntry(objEntry, _tabId, _portalSettings.PortalId, _blogSettings.VocabularyId)

  Return True
 End Function

 Public Function getRecentPosts(blogid As String, username As String, password As String, numberOfPosts As Integer) As Post() Implements IMetaWeblog.getRecentPosts
  InitializeMethodCall(username, password, blogid, "")
  RequireAccessPermission()

  Dim posts As New List(Of Post)
  Try
   Dim arEntries As List(Of EntryInfo) = EntriesController.GetEntriesByBlog(CInt(blogid), 1, Settings.WLWRecentEntriesMax, "PublishedOnDate DESC")
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
   styleDetectionPost = post.title.Contains("3bfe001a-32de-4114-a6b4-4005b770f6d7")

   ' Check to see if a styleId is passed through the QueryString
   ' however, we'll only do this if we are creating a post for style detection.
   If styleDetectionPost Then
    If TabId > -1 Then
     styleId = TabId.ToString
    End If
   End If

   ' Add the new entry
   Dim newEntry As EntryInfo = ToEntry(post)
   If Blog.MustApproveGhostPosts And Not Security.CanApproveEntry Then
    newEntry.Published = False
   Else
    newEntry.Published = publish
   End If
   EntriesController.AddEntry(newEntry, UserInfo.UserID)

   HandleAttachments(newEntry)
   EntriesController.UpdateEntry(newEntry, UserInfo.UserID)

   ' Add keywords and categories
   If Not styleDetectionPost Then
    AddCategoriesAndKeyWords(newEntry, post)
   End If

   ' Publish to journal
   If Not styleDetectionPost And newEntry.Published Then
    PublishToJournal(newEntry)
   End If

   ' If this is a style detection post, then we write to the Blog_MetaWeblogData table to note
   ' that this post is a new post.
   If styleDetectionPost Then
    Dim blogUrl As String = (New DotNetNuke.Security.PortalSecurity).InputFilter(newEntry.PermaLink(PortalSettings), DotNetNuke.Security.PortalSecurity.FilterFlag.NoSQL)
    Settings.StyleDetectionUrl = blogUrl
    Settings.UpdateSettings()
   End If

   Return newEntry.ContentItemId.ToString

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
   post.postid = postid
   Dim newEntry As EntryInfo = ToEntry(post)
   AddCategoriesAndKeyWords(newEntry, post)
   If Blog.MustApproveGhostPosts And Not Security.CanApproveEntry Then
    newEntry.Published = False
   Else
    newEntry.Published = publish
   End If

   HandleAttachments(newEntry)
   EntriesController.UpdateEntry(newEntry, UserInfo.UserID)

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
   EntriesController.DeleteEntry(Entry.ContentItemId, Entry.BlogID, PortalId, Settings.VocabularyId)
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
     categories.Add(New Services.WLW.MetaWeblog.CategoryInfo() With {.categoryId = objTerm.TermId.ToString, .categoryName = objTerm.Name, .description = objTerm.Description, .htmlUrl = "http://google.com", .parentId = objTerm.ParentTermId.ToString, .rssUrl = "http://google.com"})
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
     categories.Add(New Services.WLW.MetaWeblog.MetaWebLogCategoryInfo() With {.description = objTerm.Description, .htmlUrl = "http://google.com", .rssUrl = "http://google.com"})
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

   Dim strExtension As String = IO.Path.GetExtension(mediaobject.name)
   If Not Settings.AllowAttachments Or String.IsNullOrEmpty(strExtension) OrElse Not DotNetNuke.Entities.Host.Host.AllowedExtensionWhitelist.IsAllowedExtension(strExtension) Then
    Throw New XmlRpcFaultException(0, GetString("SaveError", String.Format("File {0} refused. Uploading this type of file is not allowed.", mediaobject.name)))
   End If
   Dim newMediaObjectName As String = Guid.NewGuid.ToString("D") & strExtension
   Dim fullFilePathAndName As String = GetTempPostDirectoryMapPath(Me.BlogId) & newMediaObjectName
   IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(fullFilePathAndName))

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

#Region " Post/Entry Conversion Methods "
 Public Function ToPost(entry As EntryInfo) As Post

  Dim post As New Post
  Dim entryBody As New PostBodyAndSummary(entry, Settings.SummaryModel)
  entryBody.WriteToPost(post, Settings.SummaryModel)

  post.mt_allow_comments = entry.AllowComments.ToInt
  post.categories = entry.EntryCategories.ToStringArray
  post.dateCreated = GetLocalAddedTime(entry.CreatedOnDate, PortalSettings.PortalId, UserInfo)
  post.pubDate = entry.PublishedOnDate
  post.date_created_gmt = entry.PublishedOnDate
  post.postid = entry.ContentItemId.ToString
  post.mt_keywords = String.Join(",", entry.EntryTags.ToStringArray)
  post.link = entry.PermaLink(PortalSettings)
  post.permalink = entry.PermaLink(PortalSettings)
  post.title = entry.Title
  post.publish = entry.Published

  Return post

 End Function

 Public Function ToEntry(post As Post) As EntryInfo

  Dim entry As New EntryInfo
  Dim postBody As New PostBodyAndSummary(post, Settings.SummaryModel)
  postBody.WriteToEntry(entry, Settings.SummaryModel, True)

  If Not String.IsNullOrEmpty(post.postid) Then
   entry.ContentItemId = post.postid.ToInt
  End If
  entry.BlogID = BlogId
  entry.Title = post.title
  If post.dateCreated.Year > 1 Then
   entry.PublishedOnDate = TimeZoneInfo.ConvertTimeToUtc(post.dateCreated, UserTimeZone)
  End If
  entry.Published = post.publish
  entry.AllowComments = post.mt_allow_comments.ToBool
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
   If Security.CanEditEntry Then
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
   If Security.CanAddEntry Or Security.CanEditEntry Then
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
    UserInfo = ValidateUser(username, password, Me.Context.Request.UserHostAddress)
   End If
   If requestedPostId <> "" Then
    EntryId = CInt(requestedPostId)
    Entry = EntriesController.GetEntry(EntryId, ModuleId)
    BlogId = Entry.BlogID
   ElseIf requestedBlogId <> "" Then
    BlogId = CInt(requestedBlogId)
   End If
   ' Check for user access to the blog
   If Me.BlogId > -1 Then
    Blog = BlogsController.GetBlog(Me.BlogId, UserInfo.UserID)
    Security = New ContextSecurity(ModuleId, TabId, Blog, UserInfo)
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
   Dim pAlias As PortalAliasInfo = PortalAliasController.GetPortalAliasInfo(portalAlias)
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

 Private Function GetRedirectUrl(tabId As Integer, moduleId As Integer) As String
  Dim appPath As String = HttpContext.Current.Request.ApplicationPath
  If appPath = "/" Then
   appPath = String.Empty
  End If
  Dim returnUrl As String = appPath + String.Format("/DesktopModules/Blog/blogpostredirect.aspx?tab={0}&ModuleId={1}", tabId, moduleId)
  Return returnUrl
 End Function
#End Region

#Region " Data Handling Methods "
 Private Sub AddCategoriesAndKeyWords(ByRef newEntry As EntryInfo, post As Post)
  Dim terms As New List(Of TermInfo)
  terms.AddRange(TermsController.GetTermList(ModuleId, post.mt_keywords, 1, True))
  If Settings.VocabularyId > 1 Then
   terms.AddRange(TermsController.GetTermList(ModuleId, post.categories.ToList, Settings.VocabularyId, False))
  End If
  newEntry.Terms.Clear()
  newEntry.Terms.AddRange(terms)
  EntriesController.UpdateEntry(newEntry, UserInfo.UserID)
 End Sub

 Private Sub PublishToJournal(newEntry As EntryInfo)
  Dim journalUserId As Integer
  If newEntry.CreatedByUserID <> UserInfo.UserID AndAlso Not Blog.PublishAsOwner Then
   journalUserId = UserInfo.UserID
  Else
   journalUserId = Blog.OwnerUserId
  End If
  Integration.JournalController.AddBlogEntryToJournal(newEntry, PortalId, newEntry.TabID, journalUserId, newEntry.PermaLink(PortalSettings))
 End Sub
#End Region

#Region " Attachment Handling Methods "
 Private Sub HandleAttachments(ByRef newEntry As EntryInfo)

  ' Handle attachments
  Dim contents As String = newEntry.Content
  If Not String.IsNullOrEmpty(newEntry.Summary) Then contents &= newEntry.Summary
  Dim d As New IO.DirectoryInfo(GetTempPostDirectoryMapPath(Me.BlogId))
  Dim targetDir As String = GetPostDirectoryMapPath(newEntry)
  If Not IO.Directory.Exists(targetDir) Then
   IO.Directory.CreateDirectory(targetDir)
  Else
   For Each f As String In IO.Directory.GetFiles(targetDir) ' remove deprecated files
    If Not contents.Contains(IO.Path.GetFileName(f)) Then
     Try
      IO.File.Delete(f)
     Catch ex As Exception
      ' we're not too bothered if this doesn't succeed
     End Try
    End If
   Next
  End If
  If d.Exists Then
   For Each f As IO.FileInfo In d.GetFiles
    If contents.Contains(f.Name) Then
     f.MoveTo(targetDir & f.Name)
    End If
   Next
  End If
  newEntry.Content = newEntry.Content.Replace(String.Format("Blog/Files/{0}/_temp_images/", Me.BlogId), String.Format("Blog/Files/{0}/{1}/", Me.BlogId, newEntry.ContentItemId))
  If Not String.IsNullOrEmpty(newEntry.Summary) Then newEntry.Summary = newEntry.Summary.Replace(String.Format("Blog/Files/{0}/_temp_images/", Me.BlogId), String.Format("Blog/Files/{0}/{1}/", Me.BlogId, newEntry.ContentItemId))

 End Sub
#End Region

End Class