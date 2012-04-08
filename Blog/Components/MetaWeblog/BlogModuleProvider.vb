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
Imports System.Collections
Imports System.Reflection
Imports System.Web
Imports DotNetNuke.Data
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Modules.Blog.Business

Namespace MetaWeblog
 Public Class BlogModuleProvider
  Implements IPublishable, ILinkable

#Region " Private Members "
  Private _portalId As Integer = -1
#End Region

#Region " Constructors "
  Public Sub New(ByVal PortalId As Integer)
   _portalId = PortalId
  End Sub
#End Region

#Region " Public Properties "

  ''' <summary>
  ''' ProviderKey should be set to the Friendly Name of the module for
  ''' which this provider is being implemented.
  ''' </summary>
  Public ReadOnly Property ProviderKey() As String Implements IPublishable.ProviderKey
   Get
    Return BlogPostServices.GetFriendlyNameFromModuleDefinition("View_Blog")
   End Get
  End Property

  Public ReadOnly Property RootBlogPath() As String
   Get
                Dim ps As PortalSettings = Common.GetPortalSettings
    Return ps.HomeDirectoryMapPath + ProviderKey
   End Get
  End Property
  ''' <summary>
  ''' ManifestFileName is the actual FileName for the manifest file.  The 
  ''' standard name is wlwmanifest.xml, but you can use any filename with an
  ''' xml file extension.
  ''' </summary>
  Public ReadOnly Property ManifestFilePath() As String Implements IPublishable.ManifestFilePath
   Get
    Return "/DesktopModules/Blog/WLWManifest.aspx?PortalID=" & _portalId.ToString
   End Get
  End Property

  ''' <summary>
  ''' LocalizationFilePath is the path to the localization file used for messages returned to 
  ''' Windows Live Writer.  This path should not end in a forward slash, but should begin with a
  ''' forward slass.  An example path might be /DesktopModules/modulename/App_LocalResources/filename
  ''' where filename would not include the .ascx.resx extension.
  ''' </summary>
  Public ReadOnly Property LocalizationFilePath() As String Implements IPublishable.LocalizationFilePath
   Get
    Return "/DesktopModules/blog/App_LocalResources/blogpost"
   End Get
  End Property

  ''' <summary>
  ''' ImageUploadPath is a property used to determine where 
  ''' to store the media files (images).  It is not currently used by the blog  
  ''' module since images are saved to the default location for saving images
  ''' (portalroot)/blog/Files/BlogId/EntryId/...
  ''' </summary>
  Public ReadOnly Property ImageUploadPath() As String Implements IPublishable.ImageUploadPath
   Get
    ' This Is not used by the blog module since the images are currently saved to the 
    ' default path used by the blog module for saving images.
    Return BlogPostServices.IMPLEMENTED_BY_MODULE
   End Get
  End Property

  ''' <summary>
  ''' AttachmentUploadPath is a property used to determine where 
  ''' to store attached media files which are not included in the post as images.  
  ''' </summary>
  Public ReadOnly Property AttachmentUploadPath() As String Implements IPublishable.AttachmentUploadPath
   Get
    ' This Is not used by the blog module since the images are currently saved to the 
    ' default path used by the blog module for saving images.
    Return BlogPostServices.IMPLEMENTED_BY_MODULE
   End Get
  End Property

  ''' <summary>
  ''' SettingsUserControlPath is for future use.  It will return the path to the
  ''' the settings user control for this provider.  
  ''' </summary>
  Public ReadOnly Property SettingsUserControlPath() As String Implements IPublishable.SettingsUserControlPath
   Get
    Throw New NotImplementedException
   End Get
  End Property

#End Region

#Region " IPublishable Members "
  Public Function GetModulesForUser(ByVal userInfo As UserInfo, ByVal portalSettings As PortalSettings, ByVal blogSettings As Settings.BlogSettings, ByVal providerKey As String) As ModuleInfoStruct() Implements IPublishable.GetModulesForUser

   Dim infoArrayList As ArrayList = New ArrayList

   Dim blogController As New BlogController


   Dim blogTabId As Integer = -1
   If Not HttpContext.Current.Request("tabid") Is Nothing AndAlso HttpContext.Current.Request("tabid").ToString() <> String.Empty Then
    blogTabId = Convert.ToInt32(HttpContext.Current.Request("tabid"))
   Else
    blogTabId = Utility.GetTabIDByPortalID(portalSettings.PortalId.ToString())
   End If

   ' Retrieve the blogs if there are any
   Dim blogsList As ArrayList = blogController.GetBlogsByUserName(userInfo.PortalID, userInfo.Username)

   If Not blogsList Is Nothing Then
    For Each blog As BlogInfo In blogsList
     Dim blogInfo As New ModuleInfoStruct
     blogInfo.BlogID = blog.BlogID.ToString()
     blogInfo.ModuleName = blog.Title
     blogInfo.Url = BlogPostServices.GetRedirectUrl(providerKey, blogTabId)
     infoArrayList.Add(blogInfo)
    Next
   Else
    Throw New BlogPostException("NoModulesForUser", "There is no blog associated with this user account.  Please enter a user account which has been associated with a blog.")
   End If

   Return CType(infoArrayList.ToArray(GetType(ModuleInfoStruct)), ModuleInfoStruct())

  End Function

#Region " Item Related Procedures "

  Public Function GetItem(ByVal itemId As String, ByVal userInfo As UserInfo, ByVal portalSettings As PortalSettings, ByVal blogSettings As Settings.BlogSettings, ByVal itemType As ItemType) As Item Implements IPublishable.GetItem
   Dim entryController As New EntryController
   Dim item As New Item

   ' Need to use reflection to get the right procedure since 
   ' the blog module authors changed the signature in
   ' release 03.04.00
   Dim BlogType As Type = entryController.[GetType]()
   Dim miGetEntry As MethodInfo = BlogType.GetMethod("GetEntry")
   Dim piParameters As ParameterInfo() = miGetEntry.GetParameters()
   Dim entryInfo As EntryInfo
   If piParameters.Length = 1 Then
    Dim methodParams As Object() = New Object(0) {}
    methodParams.SetValue(Convert.ToInt32(itemId), 0)
    entryInfo = DirectCast(miGetEntry.Invoke(entryController, methodParams), EntryInfo)
   Else
    Dim methodParams As Object() = New Object(1) {}
    methodParams.SetValue(Convert.ToInt32(itemId), 0)
    methodParams.SetValue(portalSettings.PortalId, 1)
    entryInfo = DirectCast(miGetEntry.Invoke(entryController, methodParams), EntryInfo)
   End If
   If entryInfo Is Nothing Then
    Throw New BlogPostException("NoPostAvailable", "There was en error retrieving the blog entry.  Try closing and restarting the software you're using to edit your blog post.")
   End If
   'Check to make sure user is authorized to view this content

   BlogPostServices.AuthorizeUser(entryInfo.BlogID.ToString(), GetModulesForUser(userInfo, portalSettings, blogSettings, ProviderKey))

   item.Link = entryInfo.PermaLink
   item.Content = HttpUtility.HtmlDecode(entryInfo.Entry)
   item.Summary = HttpUtility.HtmlDecode(entryInfo.Description)
   item.DateCreated = entryInfo.AddedDate.AddMinutes(GetTimeZoneOffset(entryInfo.BlogID))
   item.StartDate = entryInfo.AddedDate
   item.ItemId = entryInfo.EntryID.ToString()
   item.Title = entryInfo.Title
   item.Permalink = entryInfo.PermaLink
   item.AllowComments = DirectCast(IIf((entryInfo.AllowComments), 1, 0), Integer)
   item.Publish = entryInfo.Published

   Dim TagController As New TagController
   item.Keywords = TagController.GetTagsByEntry(entryInfo.EntryID)

   item.Categories = CategoryController.StringListCatsByEntry(entryInfo.EntryID)

   If itemType = itemType.Post And Not blogSettings.ExcerptEnabled Then
    FindAndPlaceSummary(item)
   End If

   Return item
  End Function

  Public Function GetRecentItems(ByVal blogId As String, ByVal userInfo As UserInfo, ByVal portalSettings As PortalSettings, ByVal blogSettings As Settings.BlogSettings, ByVal numberOfItems As Integer, ByVal requestType As RecentItemsRequestType, ByVal providerKey As String) As Item() Implements IPublishable.GetRecentItems

   'Authorize User
   BlogPostServices.AuthorizeUser(blogId, GetModulesForUser(userInfo, portalSettings, blogSettings, providerKey))

   Dim itemArray As Item() = Nothing
   Dim objBlogController As New BlogController
   Dim intBlogID As Integer = objBlogController.GetBlogByUserID(portalSettings.PortalId, userInfo.UserID).BlogID
   Dim arEntries As ArrayList = New EntryController().ListEntriesByBlog(intBlogID, DateTime.Now.ToUniversalTime(), True, True, 20)

   ' Find which is the least, numberOfPosts or arEntries.Count
   Dim loopCutOff As Integer = DirectCast(IIf((numberOfItems >= arEntries.Count), arEntries.Count, numberOfItems), Integer)

   itemArray = New Item(loopCutOff - 1) {}

   Dim i As Integer = 0
   For Each entry As EntryInfo In arEntries
    Dim item As New Item
    item.Link = entry.PermaLink
    item.Content = HttpUtility.HtmlDecode(entry.Entry)
    item.Summary = HttpUtility.HtmlDecode(entry.Description)
    item.DateCreated = entry.AddedDate.AddMinutes(GetTimeZoneOffset(Convert.ToInt32(blogId)))
    item.ItemId = entry.EntryID.ToString()
    item.Title = entry.Title
    item.Publish = entry.Published
    item.Permalink = entry.PermaLink
    itemArray(i) = item
    i = i + 1
    If i >= loopCutOff Then
     Exit For
    End If
   Next

   Return itemArray
  End Function

  Public Function NewItem(ByVal blogId As String, ByVal userInfo As UserInfo, ByVal portalSettings As PortalSettings, ByVal blogSettings As Settings.BlogSettings, ByVal item As Item) As String Implements IPublishable.NewItem

   BlogPostServices.AuthorizeUser(blogId, GetModulesForUser(userInfo, portalSettings, blogSettings, ProviderKey))

   ExtractSummaryFromExtendedContent(item)

   Dim entryId As Integer = 0
   Dim blogController As New BlogController
   Dim objEntryController As New EntryController
   Dim objEntry As New EntryInfo
   Dim blogTabID As Integer = 0
   Dim tempBlogID As Integer = 0
   Dim PortalID As Integer = portalSettings.PortalId
   objEntry.Title = item.Title

   ' HtmlEncode the entry.
   objEntry.Entry = HttpUtility.HtmlEncode(item.Content)

   ' Retrieve the TabID where this blog can be viewed
   If Not HttpContext.Current.Request("tabid") Is Nothing AndAlso HttpContext.Current.Request("tabid").ToString() <> String.Empty Then
    blogTabID = Convert.ToInt32(HttpContext.Current.Request("tabid"))
   Else
    blogTabID = Utility.GetTabIDByPortalID(portalSettings.PortalId.ToString())
   End If

   If tempBlogID = 0 Then
    tempBlogID = Convert.ToInt32(blogId)
   End If

   ' Make sure the AddedDate is valid
   If item.DateCreated.Year > 1 Then
    ' WLW manages the TZ offset automatically
    objEntry.AddedDate = item.DateCreated
   Else
    objEntry.AddedDate = DateTime.Now.ToUniversalTime()
   End If

   objEntry.AllowComments = DirectCast(IIf((item.AllowComments = -1 OrElse item.AllowComments = 1), True, False), Boolean)

   objEntry.Published = item.Publish
   If blogSettings.AllowSummaryHtml Then
    objEntry.Description = item.Summary
   Else
    objEntry.Description = Globals.RemoveMarkup(item.Summary)
   End If
   objEntry.DisplayCopyright = False
   objEntry.Copyright = ""
   objEntry.BlogID = tempBlogID
   entryId = objEntryController.AddEntry(objEntry)
   objEntry.EntryID = entryId

   'True in the last parameter just specifies that we want the SEO Friendly URL to be saved
   ' in the permalink field which is used by WLW to redirect to post after entry is made.
   objEntry.PermaLink = Utility.BlogNavigateURL(blogTabID, PortalID, objEntry, True)

   BlogPostServices.ProcessItemImages(objEntry, RootBlogPath)

   objEntryController.UpdateEntry(objEntry)

   TagController.UpdateTagsByEntry(entryId, item.Keywords)

   CategoryController.UpdateCategoriesByEntry(entryId, item.Categories)

   ' If this is a style detection post, then we write to the Blog_MetaWeblogData table to note
   ' that this post is a new post.  We're just using the DAL+ here to manage this feature.
   If (item.StyleDetectionPost) Then
    Dim blogUrl As String = (New DotNetNuke.Security.PortalSecurity).InputFilter(objEntry.PermaLink, Security.PortalSecurity.FilterFlag.NoSQL)
    ' DW - 01/27/2010 - Updated to ensure that at least 1 row exists in this table.
    Dim sSQL As String = "DECLARE @Count INT; SELECT @Count = (SELECT Count(*) FROM {databaseOwner}{objectQualifier}Blog_MetaWeblogData); IF @Count = 0 INSERT INTO {databaseOwner}{objectQualifier}Blog_MetaWeblogData SELECT '" & blogUrl & "' ELSE UPDATE dbo.Blog_MetaWeblogData SET TempInstallUrl = '" & blogUrl & "'"
    DataProvider.Instance.ExecuteSQL(sSQL)
   End If

   Return entryId.ToString()
  End Function

  Public Function EditItem(ByVal userInfo As UserInfo, ByVal portalSettings As PortalSettings, ByVal blogSettings As Settings.BlogSettings, ByVal item As Item) As Boolean Implements IPublishable.EditItem

   Dim objEntryController As New EntryController
   ' Need to use reflection to get the right procedure since 
   ' the signature changed in release 03.04.00
   Dim BlogType As Type = objEntryController.[GetType]()
   Dim miGetEntry As MethodInfo = BlogType.GetMethod("GetEntry")
   Dim piParameters As ParameterInfo() = miGetEntry.GetParameters()
   Dim objEntry As EntryInfo
   If piParameters.Length = 1 Then
    Dim methodParams As Object() = New Object(0) {}
    methodParams.SetValue(Convert.ToInt32(item.ItemId), 0)
    objEntry = DirectCast(miGetEntry.Invoke(objEntryController, methodParams), EntryInfo)
   Else
    Dim methodParams As Object() = New Object(1) {}
    methodParams.SetValue(Convert.ToInt32(item.ItemId), 0)
    methodParams.SetValue(portalSettings.PortalId, 1)
    objEntry = DirectCast(miGetEntry.Invoke(objEntryController, methodParams), EntryInfo)
   End If

   ' Check user's authorization to edit post
   BlogPostServices.AuthorizeUser(objEntry.BlogID.ToString, GetModulesForUser(userInfo, portalSettings, blogSettings, ProviderKey))

   ExtractSummaryFromExtendedContent(item)

   objEntry.Title = item.Title
   objEntry.Entry = item.Content
   If blogSettings.AllowSummaryHtml Then
    objEntry.Description = item.Summary
   Else
    objEntry.Description = Globals.RemoveMarkup(item.Summary)
   End If
   objEntry.AllowComments = DirectCast(IIf((item.AllowComments = -1 OrElse item.AllowComments = 1), True, False), Boolean)
   If item.DateCreated.Year > 1 Then
    ' WLW manages the TZ offset automatically
    objEntry.AddedDate = item.DateCreated
   End If

   ' HtmlEncode the entry
   objEntry.Entry = HttpUtility.HtmlEncode(objEntry.Entry)

   ' Is it a first publish?
   Dim firstPublish As Boolean = CBool((Not objEntry.Published) And item.Publish)

   If item.DateCreated.Year > 1 Then
    ' WLW handles TZ offset automatically.
    ' objEntry.AddedDate = item.DateCreated;
    ' It appears that the Blog module ignores changes to the publish date, 
    ' so we'll update it ourselves
    Dim SQL As String = "UPDATE {databaseOwner}[{objectQualifier}Blog_Entries] SET AddedDate = '" + item.DateCreated.ToString() + "' WHERE EntryID = " & objEntry.EntryID.ToString()

    DotNetNuke.Data.DataProvider.Instance().ExecuteSQL(SQL)
    ' Nothing happens since we want to keep the time previously entered.
   Else
   End If
   objEntry.Published = item.Publish

   BlogPostServices.ProcessItemImages(objEntry, RootBlogPath)

   objEntryController.UpdateEntry(objEntry)

   TagController.UpdateTagsByEntry(objEntry.EntryID, item.Keywords)
   CategoryController.UpdateCategoriesByEntry(objEntry.EntryID, item.Categories)

   Return True
  End Function

  Public Function DeleteItem(ByVal itemId As String, ByVal userInfo As UserInfo, ByVal portalSettings As PortalSettings, ByVal blogSettings As Settings.BlogSettings, ByVal itemType As ItemType) As Boolean Implements IPublishable.DeleteItem

   'Authorize User
   Dim objEntry As EntryInfo = GetEntry(itemId, portalSettings.PortalId)
   Dim blogId As String = GetBlogIdFromEntry(objEntry)

   BlogPostServices.AuthorizeUser(blogId, GetModulesForUser(userInfo, portalSettings, blogSettings, ProviderKey))

   'Create new BlogController to delete the blog entry.
   Dim objEntryController As New EntryController
   objEntryController.DeleteEntry(Convert.ToInt32(itemId))
   Return True
  End Function

#End Region

#Region " Category Related Procedures "

  Public Function GetCategories(ByVal moduleLevelId As String, ByVal userInfo As UserInfo, ByVal portalSettings As PortalSettings, ByVal blogSettings As Settings.BlogSettings) As ItemCategoryInfo() Implements IPublishable.GetCategories

   Dim BlogController As New BlogController

   Dim CatList As IDictionary(Of Integer, Business.CategoryInfo) = CategoryController.ListCategories(portalSettings.PortalId)

   Dim categories As ItemCategoryInfo() = Nothing
   categories = New ItemCategoryInfo(CatList.Count - 1) {}

   Dim category As New ItemCategoryInfo

   Dim i As Integer = 0
   For Each c As Business.CategoryInfo In CatList.Values
    category.CategoryId = c.CatId
    category.CategoryName = c.FullCat
    category.Description = c.FullCat
    category.HtmlUrl = "http://google.com"
    category.RssUrl = "http://google.com"
    ' category.ParentId = CatList(i).ParentId
    categories(i) = category
    i += 1
   Next

   Return categories

  End Function

  Public Function NewCategory(ByVal moduleLevelId As String, ByVal userInfo As UserInfo, ByVal portalSettings As PortalSettings, ByVal blogSettings As Settings.BlogSettings) As Integer Implements IPublishable.NewCategory
   Throw New BlogPostException("FeatureNotImplemented", "This feature is currently not implemented.")
  End Function

#End Region

#Region " Optional Procedures (See Comments For Details) "

  ''' <summary>
  ''' ModuleName is used when sending Trackbacks and Pings.  If these are not used by your 
  ''' module, then you don't need to implement this procedure.
  ''' </summary>
  ''' <param name="moduleLevelId"></param>
  ''' <param name="userInfo"></param>
  ''' <param name="portalSettings"></param>
  ''' <returns></returns>
  Public Function GetModuleName(ByVal moduleLevelId As String, ByVal userInfo As UserInfo, ByVal portalSettings As PortalSettings) As String Implements ILinkable.GetModuleName

   Dim blogId As Integer

   blogId = Convert.ToInt32(moduleLevelId)

   Dim blogController As New BlogController
   Dim blogInfo As BlogInfo = blogController.GetBlog(blogId)

   Return blogInfo.Title

  End Function

  ''' <summary>
  ''' ModuleName is used when sending Trackbacks and Pings.  If these are not used by your 
  ''' module, then you don't need to implement this procedure.
  ''' </summary>
  ''' <param name="id"></param>
  ''' <param name="userInfo"></param>
  ''' <param name="portalSettings"></param>
  ''' <returns></returns>
  Public Function GetPermaLink(ByVal id As String, ByVal itemId As String, ByVal userInfo As UserInfo, ByVal portalSettings As PortalSettings) As String Implements ILinkable.GetPermaLink

   Dim entryId As Integer

   entryId = Convert.ToInt32(itemId)

   Dim entryController As New EntryController

   ' Need to use reflection to get the right procedure since 
   ' the blog module authors changed the signature in
   ' release 03.04.00
   Dim BlogType As Type = entryController.[GetType]()
   Dim miGetEntry As MethodInfo = BlogType.GetMethod("GetEntry")
   Dim piParameters As ParameterInfo() = miGetEntry.GetParameters()
   Dim entryInfo As EntryInfo
   If piParameters.Length = 1 Then
    Dim methodParams As Object() = New Object(0) {}
    methodParams.SetValue(Convert.ToInt32(itemId), 0)
    entryInfo = DirectCast(miGetEntry.Invoke(entryController, methodParams), EntryInfo)
   Else
    Dim methodParams As Object() = New Object(1) {}
    methodParams.SetValue(Convert.ToInt32(itemId), 0)
    methodParams.SetValue(portalSettings.PortalId, 1)
    entryInfo = DirectCast(miGetEntry.Invoke(entryController, methodParams), EntryInfo)
   End If

   Return entryInfo.PermaLink

  End Function


  Public Function GetPingbackSettings(ByVal moduleLevelId As String, ByVal userInfo As UserInfo, ByVal portalSettings As PortalSettings) As TrackbackAndPingSettings Implements ILinkable.GetPingbackSettings
   Dim trackbackSettings As New TrackbackAndPingSettings

   trackbackSettings.allowAutoDiscovery = True
   trackbackSettings.AllowForPage = False 'Not implemented with blog
   trackbackSettings.AllowForPost = True
   trackbackSettings.allowOverrideByPingDropDown = True

   Return trackbackSettings
  End Function

#End Region

#Region " Private Procedures Specific to Blog Module "
  Private Function GetBlogIdFromEntry(ByVal entryInfo As EntryInfo) As String
   Dim blogId As String
   ' Make sure we have the parent blog
   Dim bc As New BlogController()
   Dim blogInfo As BlogInfo = bc.GetBlog(entryInfo.BlogID)
   If blogInfo.ParentBlogID <> -1 Then
    blogId = blogInfo.ParentBlogID.ToString()
   Else
    blogId = blogInfo.BlogID.ToString()
   End If
   Return blogId
  End Function

  Private Function GetEntry(ByVal itemId As String, ByVal portalId As Integer) As EntryInfo
   Dim objEntryController As New EntryController()
   ' Need to use reflection to get the right procedure since 
   ' the blog module authors changed the signature in
   ' release 03.04.00
   Dim BlogType As Type = objEntryController.[GetType]()
   Dim miGetEntry As MethodInfo = BlogType.GetMethod("GetEntry")
   Dim piParameters As ParameterInfo() = miGetEntry.GetParameters()
   Dim objEntry As EntryInfo
   If piParameters.Length = 1 Then
    Dim methodParams As Object() = New Object(0) {}
    methodParams.SetValue(Convert.ToInt32(itemId), 0)
    objEntry = DirectCast(miGetEntry.Invoke(objEntryController, methodParams), EntryInfo)
   Else
    Dim methodParams As Object() = New Object(1) {}
    methodParams.SetValue(Convert.ToInt32(itemId), 0)
    methodParams.SetValue(portalId, 1)
    objEntry = DirectCast(miGetEntry.Invoke(objEntryController, methodParams), EntryInfo)
   End If
   Return objEntry
  End Function

  Private Function GetTimeZoneOffset(ByVal blogId As Integer) As Integer
   Dim blogController As New BlogController
   Dim blog As BlogInfo = blogController.GetBlog(blogId)
   Return blog.TimeZone
  End Function

  Private Sub ExtractSummaryFromExtendedContent(ByRef item As Item)
   If Not BlogPostServices.IsNullOrEmpty(item.ExtendedContent) Then
    ' Extract the summary from the excerpt
    item.Summary = item.Content
    item.Content = item.ExtendedContent
   End If
  End Sub

  Private Sub FindAndPlaceSummary(ByRef item As Item)

   If BlogPostServices.ContainsHtml(item.Summary) Then
    ' We have HTML, so put the HTML in the content above a <!--more--> tag
    item.Content = item.Summary + "<!--more-->" + item.Content
   End If

  End Sub
#End Region

#End Region

    End Class

End Namespace