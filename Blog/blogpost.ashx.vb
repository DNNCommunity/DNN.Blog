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
''' </history>
Public Class BlogPost
    Inherits XmlRpcService
    Implements IMetaWeblog
    Implements IWordPress
    Implements IBlogger
    Implements IMoveableType

    Private _portalSettings As PortalSettings = Nothing
    Private _blogSettings As BlogSettings = Nothing
    Private _moduleName As String = String.Empty
    Private _expanderScript As String = String.Empty
    Private _userInfo As UserInfo = Nothing
    Private _provider As IPublishable = Nothing
    Private _tabId As Integer = -1

    Public Function getUsersBlogs(ByVal appKey As String, ByVal username As String, ByVal password As String) As BlogInfoStruct() Implements IBlogger.getUsersBlogs
        InitializeMethodCall(username, password)

        Dim infoArray As BlogInfoStruct()
        Try

            Dim misArray As ModuleInfoStruct() = _provider.GetModulesForUser(_userInfo, _portalSettings, _blogSettings, "Blog")

            ' Translate this to a BlogInfoStruct
            infoArray = New BlogInfoStruct(misArray.Length - 1) {}
            Dim i As Integer = 0
            While i < misArray.Length
                infoArray(i).blogid = misArray(i).BlogID
                infoArray(i).blogName = misArray(i).ModuleName
                infoArray(i).url = misArray(i).Url
                i = i + 1
            End While
        Catch mex As BlogPostException
            Throw New XmlRpcFaultException(0, GetString(mex.ResourceKey, mex.Message))
        Catch generatedExceptionName As XmlRpcFaultException
            Throw
            'Catch generatedExceptionName As Exception
            '    Throw New XmlRpcFaultException(0, GetString("GetUsersModulesError", "There was an error retrieving the list of the user's items."))
        End Try

        Return infoArray

    End Function

    Public Function getPost(ByVal postid As String, ByVal username As String, ByVal password As String) As Post Implements IMetaWeblog.getPost
        InitializeMethodCall(username, password)

        Dim post As Post
        Try
            post = getPostFromItem(_provider.GetItem(postid.ToString(), _userInfo, _portalSettings, _blogSettings, ItemType.Post))

            Dim regexPattern As String = String.Empty
            Dim options As RegexOptions = RegexOptions.Singleline Or RegexOptions.IgnoreCase

            ' Look for and replace relative images with absolute images
            regexPattern = "(src="")/"
            If Not BlogPostServices.IsNullOrEmpty(post.mt_excerpt) Then
                post.mt_excerpt = Regex.Replace(post.mt_excerpt, regexPattern, "$1http://" + Context.Request.Url.Host + "/", options)
            End If
            If Not BlogPostServices.IsNullOrEmpty(post.description) Then
                post.description = Regex.Replace(post.description, regexPattern, "$1http://" + Context.Request.Url.Host + "/", options)
            End If
            If Not BlogPostServices.IsNullOrEmpty(post.mt_text_more) Then
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

    Public Function getCategories_WordPress(ByVal blogid As String, ByVal username As String, ByVal password As String) As Components.WordPress.CategoryInfo() Implements IWordPress.getCategories
        InitializeMethodCall(username, password)

        Dim termController As ITermController = DotNetNuke.Entities.Content.Common.Util.GetTermController()
        Dim colCategories As IQueryable(Of Term) = termController.GetTermsByVocabulary(_blogSettings.VocabularyId)
        Dim res(colCategories.Count - 1) As Components.WordPress.CategoryInfo
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
    End Function

    Public Function getPostCategories(ByVal postid As String, ByVal username As String, ByVal password As String) As Category() Implements IMoveableType.getPostCategories
        InitializeMethodCall(username, password)

        Dim cntEntry As New EntryController
        Dim objEntry As EntryInfo = cntEntry.GetEntry(Convert.ToInt32(postid), _portalSettings.PortalId)

        Dim colCategories As List(Of Term)
        colCategories = objEntry.Terms

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

    Public Function setPostCategories(ByVal postid As String, ByVal username As String, ByVal password As String, ByVal categories As Category()) As Boolean Implements IMoveableType.setPostCategories
        InitializeMethodCall(username, password)

        Dim cntEntry As New EntryController
        Dim objEntry As EntryInfo = cntEntry.GetEntry(Convert.ToInt32(postid), _portalSettings.PortalId)
        Dim terms As New List(Of Term)

        For Each t As Category In categories
            Dim objTerm As Term = Components.Integration.Terms.GetTermById(Convert.ToInt32(t.categoryId), _blogSettings.VocabularyId)
            terms.Add(objTerm)
        Next

        objEntry.Terms.Clear()
        objEntry.Terms.AddRange(terms)

        cntEntry.UpdateEntry(objEntry, _tabId, _portalSettings.PortalId)

        Return True
    End Function

    Public Function getRecentPosts(ByVal blogid As String, ByVal username As String, ByVal password As String, ByVal numberOfPosts As Integer) As Post() Implements IMetaWeblog.getRecentPosts

        InitializeMethodCall(username, password)

        Dim posts As Post()
        Try
            posts = getPostsFromItems(_provider.GetRecentItems(blogid.ToString(), _userInfo, _portalSettings, _blogSettings, numberOfPosts, RecentItemsRequestType.RecentPosts, _provider.ProviderKey))
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

        Return posts

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
    Public Function newPost(ByVal blogId As String, ByVal username As String, ByVal password As String, ByVal post As Post, ByVal publish As Boolean) As String Implements IMetaWeblog.newPost
        InitializeMethodCall(username, password)

        Dim pageId As String = String.Empty
        Dim styleId As String = String.Empty
        Dim styleDetectionPost As Boolean = False
        Dim moduleId As Integer = -1

        Try
            ' Check to see if this a style detection post.
            styleDetectionPost = (post.title.Length > 116 AndAlso post.title.Substring(80, 36) = "3bfe001a-32de-4114-a6b4-4005b770f6d7")

            ' Check to see if a styleId is passed through the QueryString
            ' however, we'll only do this if we are creating a post for style detection.
            If styleDetectionPost Then
                If _tabId > -1 Then
                    styleId = _tabId.ToString
                End If
            Else
                MakeImagesRelative(post)
            End If

            Dim item As Item = getItemFromPost(post)

            item.Publish = publish
            item.StyleDetectionPost = styleDetectionPost
            item.StyleId = styleId
            item.ItemType = ItemType.Post
            item.AuthorId = _userInfo.UserID.ToString()

            pageId = _provider.NewItem(blogId.ToString(), _userInfo, _portalSettings, _blogSettings, item)

            Dim pingableProvider As ILinkable = CType(_provider, ILinkable)
            Dim taps As TrackbackAndPingSettings = pingableProvider.GetPingbackSettings(blogId, _userInfo, _portalSettings)
            Dim autoDiscovery As Boolean = CType(IIf((taps.allowOverrideByPingDropDown), (post.mt_allow_pings = 1), taps.allowAutoDiscovery), Boolean)
            If taps.AllowForPost Then
                HandleTrackbacksOrPings(blogId, pageId, post, publish, _provider, autoDiscovery)
            End If

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

        Return pageId
    End Function

    Public Function editPost(ByVal postid As String, ByVal username As String, ByVal password As String, ByVal post As Post, ByVal publish As Boolean) As Boolean Implements IMetaWeblog.editPost
        InitializeMethodCall(username, password)

        Dim success As Boolean = False
        Try


            MakeImagesRelative(post)

            Dim item As Item = getItemFromPost(post)

            item.ItemId = postid.ToString()
            item.Publish = publish
            item.ItemType = ItemType.Post

            success = CType(_provider.EditItem(_userInfo, _portalSettings, _blogSettings, item), Boolean)

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

    Public Function deletePost(ByVal appKey As String, ByVal postid As String, ByVal username As String, ByVal password As String, <XmlRpcParameter(Description:="Where applicable, this specifies whether the blog should be republished after the post has been deleted.")> ByVal publish As Boolean) As Boolean Implements IBlogger.deletePost

        InitializeMethodCall(username, password)

        Dim success As Boolean = False
        Try
            success = CType(_provider.DeleteItem(postid.ToString(), _userInfo, _portalSettings, _blogSettings, ItemType.Post), Boolean)
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

    Public Function getWPCategories(ByVal blog_id As String, ByVal username As String, ByVal password As String) As Components.MetaWeblog.CategoryInfo()
        InitializeMethodCall(username, password)

        Dim categories As Components.MetaWeblog.CategoryInfo()
        Try
            categories = getCategoryInfosFromItemCategoryInfos(_provider.GetCategories(blog_id.ToString(), _userInfo, _portalSettings, _blogSettings))
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
        'Check to make sure we're not returning Nothing, which throws WLW for a loop.
        If categories Is Nothing Then
            categories = New Components.MetaWeblog.CategoryInfo(-1) {}
        End If

        Return categories
    End Function

    Public Function getCategories(ByVal blogid As String, ByVal username As String, ByVal password As String) As MetaWebLogCategoryInfo() Implements IMetaWeblog.getCategories
        Return getMetaWebLogCategoryInfosFromCateogryInfos(getWPCategories(blogid, username, password))
    End Function

    Public Function newMediaObject(ByVal blogid As Object, ByVal username As String, ByVal password As String, ByVal mediaobject As mediaObject) As mediaObjectInfo Implements IMetaWeblog.newMediaObject
        InitializeMethodCall(username, password)
        BlogPostServices.AuthorizeUser(blogid.ToString(), _provider.GetModulesForUser(_userInfo, _portalSettings, _blogSettings, _provider.ProviderKey))

        Dim info As mediaObjectInfo

        Try
            Dim virtualPath As String
            Dim mediaObjectName As String = String.Empty
            Dim fullFilePathAndName As String = String.Empty
            info.url = ""

            Dim strWhiteList As String = "," & DotNetNuke.Entities.Host.Host.FileExtensions & ","
            Try

                ' Shorten WindowsLiveWriter and create one file name.
                mediaObjectName = mediaobject.name.Replace("WindowsLiveWriter", "WLW")
                mediaObjectName = mediaObjectName.Replace("/", "-").Replace(" ", "_")
                mediaObjectName = HttpContext.Current.Server.UrlEncode(mediaObjectName)

                ' Check permitted file types
                Dim strExtension As String = Path.GetExtension(mediaObjectName).Replace(".", "")
                If String.IsNullOrEmpty(strExtension) OrElse strWhiteList.IndexOf("," & strExtension.ToLower & ",") = -1 Then
                    Throw New XmlRpcFaultException(0, GetString("SaveError", String.Format("File {0} refused. Uploading this type of file is not allowed.", mediaObjectName)))
                End If

                virtualPath = _provider.ProviderKey & "/Files/" & blogid.ToString() & "/_temp_images/" & mediaObjectName

                fullFilePathAndName = _portalSettings.HomeDirectoryMapPath + virtualPath.Replace("/", "\")

                '_provider.BeforeMediaSaved(fullFilePathAndName);

                Dim output As New FileStream(CreateFoldersForFilePath(fullFilePathAndName), FileMode.Create)
                Dim bw As BinaryWriter = New BinaryWriter(output)
                bw.Write(mediaobject.bits)

                output.Close()
            Catch exc As IOException
                Throw New XmlRpcFaultException(0, GetString("ImageSaveError", "An error occurred while saving an image related to this blog post."))
            End Try
            Dim finalUrl As String = _portalSettings.HomeDirectory.Replace("\", "/")

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

#Region "Private Procedures"

    Private Function ValidateUser(ByVal username As String, ByVal password As String, ByVal ipAddress As String) As UserInfo
        Dim userInfo As UserInfo = Nothing
        Try

            Dim loginStatus As UserLoginStatus = UserLoginStatus.LOGIN_FAILURE
            userInfo = UserController.ValidateUser(_portalSettings.PortalId, username, password, "", _portalSettings.PortalName, ipAddress, _
             loginStatus)

            If userInfo.Roles Is Nothing Then
                ' This fix was added to address an issue with DNN 04.05.05
                userInfo.Roles = GetRolesByUser(userInfo.UserID, _portalSettings.PortalId)
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

    Private Function GetRolesByUser(ByVal userId As Integer, ByVal portalId As Integer) As String()
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

    Private Sub GetPortalSettings()
        Dim portalID As Integer = -1
        Try
            Dim Request As HttpRequest = Me.Context.Request

            Dim oTabController As TabController = New TabController
            Dim oTabInfo As TabInfo = oTabController.GetTab(_tabId, portalID, False)

            portalID = oTabInfo.PortalID

            Dim pac As New PortalAliasController
            Dim paColl As PortalAliasCollection = pac.GetPortalAliasByPortalID(portalID)

            Dim pai As PortalAliasInfo = Nothing

            For Each paiKey As String In paColl.Keys
                pai = paColl(paiKey)
                Exit For
            Next

            Dim portalController As New PortalController
            Dim pi As PortalInfo = portalController.GetPortal(portalID)

            _portalSettings = New PortalSettings(_tabId, pai)

        Catch ex As XmlRpcFaultException
            Throw
        Catch generatedExceptionName As Exception
            Throw New XmlRpcFaultException(0, GetString("PortalLoadError", "Please check your URL to make sure you entered the correct URL for your blog.  The blog posting URL is available through the blog settings for your blog.")) ' & "Error:" & generatedExceptionName.ToString() & " PortalId: " & portalID.ToString()
        End Try
    End Sub

    Private Function GetPortalIDFromAlias(ByVal portalAlias As String) As Integer
        'Get the PortalAlias based on the Request object
        Dim pc As New PortalController
        Dim portalID As Integer = -1
        Try
            Dim dr As IDataReader = DotNetNuke.Data.DataProvider.Instance().GetPortalByAlias(portalAlias)
            If dr.Read() Then
                portalID = dr.GetInt32(dr.GetOrdinal("PortalID"))
            End If
            dr.Close()
        Catch generatedExceptionName As Exception
            ' Just ignore the errors if any and pass up 0 for the portalID.  0 for the portalID
            ' will throw an error in the calling procedure.
        End Try
        Return portalID
    End Function

    Private Function CreateFoldersForFilePath(ByVal folderPath As String) As String
        Dim path As String = folderPath.Substring(0, folderPath.LastIndexOf("\"))
        If Not Directory.Exists(path) Then
            Directory.CreateDirectory(path)
        End If
        Return folderPath
    End Function

    Private Sub MakeImagesRelative(ByRef post As Post)
        ' Make the images relative

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
        If Not BlogPostServices.IsNullOrEmpty(post.mt_excerpt) Then
            post.mt_excerpt = Regex.Replace(post.mt_excerpt, expression, "$1", options)
        End If
        If Not BlogPostServices.IsNullOrEmpty(post.description) Then
            post.description = Regex.Replace(post.description, expression, "$1", options)
        End If
        If Not BlogPostServices.IsNullOrEmpty(post.mt_text_more) Then
            post.mt_text_more = Regex.Replace(post.mt_text_more, expression, "$1", options)
        End If
    End Sub

    Private Sub getProvider()
        _provider = New BlogModuleProvider(_portalSettings.PortalId)
    End Sub

    Private Function getItemFromPost(ByVal content As Post) As Item
        Dim item As New Item

        item.AllowComments = CType(IIf(content.mt_allow_comments = 0, -1, content.mt_allow_comments), Integer)
        item.AllowTrackbacksOrPings = content.mt_allow_pings
        item.Categories = content.categories
        item.Content = content.description
        item.DateCreated = content.dateCreated
        item.StartDate = content.pubDate
        item.ExtendedContent = content.mt_text_more
        item.ItemId = content.postid
        item.Keywords = content.mt_keywords
        item.Link = content.link
        item.Permalink = content.permalink
        item.PingUrls = content.mt_tb_ping_urls
        item.Title = content.title
        item.SeoFriendlyTitle = content.wp_slug
        item.ItemPassword = content.wp_password
        item.ParentId = content.wp_page_parent_id
        item.PageOrder = content.wp_page_order
        item.AuthorId = content.wp_author_id
        item.Summary = content.mt_excerpt
        item.PingUrls = content.mt_tb_ping_urls
        item.Publish = content.publish

        Return item
    End Function

    Private Function getPostFromItem(ByVal item As Item) As Post
        Dim post As New Post

        post.mt_allow_comments = item.AllowComments
        post.mt_allow_pings = item.AllowTrackbacksOrPings
        post.categories = item.Categories
        post.description = item.Content
        post.dateCreated = item.DateCreated
        post.pubDate = item.StartDate
        post.date_created_gmt = item.DateCreated
        post.mt_text_more = item.ExtendedContent
        post.postid = item.ItemId
        post.mt_keywords = item.Keywords
        post.link = item.Link
        post.permalink = item.Permalink
        post.mt_tb_ping_urls = item.PingUrls
        post.title = item.Title
        post.wp_slug = item.SeoFriendlyTitle
        post.wp_password = item.ItemPassword
        post.wp_page_parent_id = item.ParentId
        post.wp_page_order = item.PageOrder
        post.wp_author_id = item.AuthorId
        post.mt_excerpt = item.Summary
        post.mt_tb_ping_urls = item.PingUrls
        post.publish = item.Publish

        Return post
    End Function

    Private Function getPageFromItem(ByVal item As Item) As Page
        Dim page As New Page

        page.page_id = item.ItemId
        page.page_title = item.Title
        If BlogPostServices.IsNullOrEmpty(item.ParentId) OrElse item.ParentId = "0" Then
            page.page_parent_id = "0"
        Else
            page.page_parent_id = item.ParentId.ToString()
        End If
        page.dateCreated = item.DateCreated

        Return page
    End Function

    Private Function getPostsFromItems(ByVal items As Item()) As Post()
        Dim posts As Post() = New Post(items.Length - 1) {}
        Dim i As Integer = 0
        While i < items.Length
            posts(i) = getPostFromItem(items(i))
            i = i + 1
        End While
        Return posts
    End Function

    Private Function getPagesFromItems(ByVal items As Item()) As Page()
        Dim pages As Page() = New Page(items.Length - 1) {}
        Dim i As Integer = 0
        While i < items.Length
            pages(i) = getPageFromItem(items(i))
            i = i + 1
        End While
        Return pages
    End Function

    Private Function getCategoryInfoFromItemCategoryInfo(ByVal ici As ItemCategoryInfo) As Components.MetaWeblog.CategoryInfo
        Dim ci As New Components.MetaWeblog.CategoryInfo
        ci.categoryId = ici.CategoryId.ToString()
        ci.categoryName = ici.CategoryName
        ci.description = ici.Description
        ci.htmlUrl = ici.HtmlUrl
        ci.parentId = ici.ParentId.ToString()
        ci.rssUrl = ici.RssUrl
        Return ci
    End Function

    Private Function getCategoryInfosFromItemCategoryInfos(ByVal ici As ItemCategoryInfo()) As Components.MetaWeblog.CategoryInfo()
        Dim ci As Components.MetaWeblog.CategoryInfo() = Nothing
        If Not ici Is Nothing Then
            ci = New Components.MetaWeblog.CategoryInfo(ici.Length - 1) {}
            Dim i As Integer = 0
            While i < ici.Length
                ci(i) = getCategoryInfoFromItemCategoryInfo(ici(i))
                i = i + 1
            End While
        End If
        Return ci
    End Function

    Private Function getMetaWebLogCategoryInfosFromItemCategoryInfo(ByVal ci As Components.MetaWeblog.CategoryInfo) As MetaWebLogCategoryInfo
        Dim mwci As New MetaWebLogCategoryInfo
        mwci.description = ci.description
        mwci.htmlUrl = ci.htmlUrl
        mwci.rssUrl = ci.rssUrl
        Return mwci
    End Function

    Private Function getMetaWebLogCategoryInfosFromCateogryInfos(ByVal ci As Components.MetaWeblog.CategoryInfo()) As MetaWebLogCategoryInfo()
        Dim mwci As MetaWebLogCategoryInfo() = New MetaWebLogCategoryInfo(ci.Length - 1) {}
        Dim i As Integer = 0
        While i < ci.Length
            mwci(i) = getMetaWebLogCategoryInfosFromItemCategoryInfo(ci(i))
            i = i + 1
        End While
        Return mwci
    End Function

    Private Sub InitializeMethodCall(ByVal username As String, ByVal password As String)
        ' NOTE: CP - This method should be updated to support ghost writing, which is a blog level setting and is tied to core permissions (a column in the module permissions grid)
        Try
            Globals.ReadValue(Context.Request.Params, "tabid", _tabId)
            GetPortalSettings()
            getProvider()
            _blogSettings = BlogSettings.GetBlogSettings(_portalSettings.PortalId, _tabId)
            If Not _blogSettings.AllowWLW Then
                Throw New XmlRpcFaultException(0, GetString("Access Denied", "Access to the module through this API has been denied. Please contact the Portal Administrator."))
            Else
                _userInfo = ValidateUser(username, password, Me.Context.Request.UserHostAddress)
            End If
        Catch ex As Exception
            LogException(ex)
            Throw
        End Try
    End Sub

    Private Function GetSummary(ByRef content As Post) As String
        Dim summary As String = String.Empty

        If Not BlogPostServices.IsNullOrEmpty(content.mt_excerpt) Then
            summary = content.mt_excerpt
        Else
            summary = content.description
        End If

        Return summary
    End Function

    Private Function GetPostText(ByRef content As Post) As String
        Dim postContent As String = String.Empty
        If BlogPostServices.IsNullOrEmpty(content.mt_excerpt) AndAlso Not BlogPostServices.IsNullOrEmpty(content.mt_text_more) AndAlso Not BlogPostServices.IsNullOrEmpty(content.description) Then
            postContent = content.mt_text_more
        Else
            postContent = content.description
        End If
        Return postContent
    End Function

    Private Sub HandleTrackbacksOrPings(ByVal moduleLevelId As String, ByVal itemId As String, ByRef content As Post, ByVal publish As Boolean, ByVal provider As IPublishable, ByVal autoDiscovery As Boolean)
        ' Send Pings or Trackbacks if publishing this page and there are trackbacks
        If publish Then
            ' Retrieve the Blog Name
            Dim pingableProvider As ILinkable = CType(_provider, ILinkable)
            Dim blogName As String = pingableProvider.GetModuleName(moduleLevelId, _userInfo, _portalSettings)
            Dim permaLink As String = pingableProvider.GetPermaLink(moduleLevelId, itemId, _userInfo, _portalSettings)

            ' Will add when upgraded to 2.0 version of the Framework
            'TrackingService.TrackbackOrPing(content.mt_tb_ping_urls, content.title, permaLink, blogName, GetSummary(content), GetPostText(content), _
            'autoDiscovery)
        End If
    End Sub

    Private Function GetString(ByVal localizationKey As String, ByVal defaultValue As String) As String
        Return BlogPostServices.GetString(localizationKey, defaultValue)
    End Function

    Private Sub LogException(ByVal ex As Exception)
        Try
            DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
        Catch
        End Try
    End Sub

#End Region

End Class