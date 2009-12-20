Imports System
Imports System.Text
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Users

Namespace MetaWeblog

 ' IPublishable contains the core properties and methods needed for the offline 
 ' publishing of content to DotNetNuke modules
 Public Interface IPublishable

#Region "Properties"
  ''' <summary>
  ''' ProviderKey should be set to the Friendly Name of the module for
  ''' which this provider is being implemented.
  ''' </summary>
  ReadOnly Property ProviderKey() As String
  ''' <summary>
  ''' ManifestFileName is the actual FileName for the manifest file.  The 
  ''' standard name is wlwmanifest.xml, but you can use any filename with an
  ''' xml file extension.
  ''' </summary>
  ReadOnly Property ManifestFilePath() As String
  ''' <summary>
  ''' LocalizationFilePath is the path including the filename to the 
  ''' resource file used by the blog module to retrieve the error messages
  ''' shown to the user (eg. "/DesktopModules/blog/App_LocalResources/blogpost")
  ''' </summary>
  ReadOnly Property LocalizationFilePath() As String
  ''' <summary>
  ''' ImageUploadPath is not used by the blog module.  The image upload path
  ''' used is the default path used by the blog module "Blog/Files/BlogId/EntryId/filename"  
  ''' </summary>
  ReadOnly Property ImageUploadPath() As String
  ''' <summary>
  ''' AttachmentUploadPath is not used by the blog module.  
  ''' </summary>
  ReadOnly Property AttachmentUploadPath() As String
  ''' <summary>
  ''' SettingsUserControlPath is not used by the blog module since all settings related
  ''' to MetaWeblog support are configured through the blog module properties.
  ''' </summary>
  ReadOnly Property SettingsUserControlPath() As String
#End Region

#Region "Methods"

  ''' <summary>
  ''' The purpose of this procedure is to provide a list of all the modules in a 
  ''' given portal to which the currently logged on user has either insert, update
  ''' or delete access.  Note, this may not always correspond to an actual module.
  ''' For example, with the DNN Blog module provider, a list of blogs is returned
  ''' along with URL of where each blog is available within the site.
  ''' </summary>
  ''' <param name="userInfo"></param>
  ''' <param name="portalSettings"></param>
  ''' <returns></returns>
  ''' <param name="providerKey"></param>
  Function GetModulesForUser(ByVal userInfo As UserInfo, ByVal portalSettings As PortalSettings, ByVal providerKey As String) As ModuleInfoStruct()

  ' Note in the blog module, moduleLevelId and ItemId corresponded to BlogId and EntryId
  Function GetRecentItems(ByVal moduleLevelId As String, ByVal userInfo As UserInfo, ByVal portalSettings As PortalSettings, ByVal numberOfItems As Integer, ByVal requestType As RecentItemsRequestType, ByVal providerKey As String) As Item()

  Function GetItem(ByVal itemId As String, ByVal userInfo As UserInfo, ByVal portalSettings As PortalSettings, ByVal itemType As ItemType) As Item

  Function EditItem(ByVal moduleLevelId As String, ByVal userInfo As UserInfo, ByVal portalSettings As PortalSettings, ByVal item As Item) As Boolean



  ' 11/19/2008 Rip Rowan -  XML Comments removed since extraneous
  '''' <param name="publish">Bool - specifies whether the user clicked publish or save as draft.</param>
  '''' <param name="styleDetectionPost">Bool - is true when WLW is creating a new test post to determine the template and styles of the blog.</param>
  '''' <param name="styleId">String - Optional.  Used for style detection when the blog module is installed on more than one tab.  Pass the TabId of the tab to be used for style detection in as the style Id (sid) during account creation.</param>
  '''' <param name="itemType">Enumeration - Set to Page or Post.  Not needed by the current implementation of the blod module since all entries are made as posts.</param>

  ''' <summary>
  ''' NewItem is used for creating new blog entries
  ''' </summary>
  ''' <param name="moduleLevelId">String - the BlogId is tracked through this parameter.</param>
  ''' <param name="userInfo">DotNetNuke UserInfo object.  This UserInfo object of the user posting the blog entry.</param>
  ''' <param name="portalSettings">DotNetNuke PortaSettings object for the portal to which the entry is being posted.</param>
  ''' <param name="item">Custom Struct - The item struct contains a list of fields related to an entry.</param>
  ''' <returns></returns>
  Function NewItem(ByVal moduleLevelId As String, ByVal userInfo As UserInfo, ByVal portalSettings As PortalSettings, ByVal item As Item) As String

  Function DeleteItem(ByVal itemId As String, ByVal userInfo As UserInfo, ByVal portalSettings As PortalSettings, ByVal itemType As ItemType) As Boolean

  Function GetCategories(ByVal moduleLevelId As String, ByVal userInfo As UserInfo, ByVal portalSettings As PortalSettings) As ItemCategoryInfo()

  Function NewCategory(ByVal moduleLevelId As String, ByVal userInfo As UserInfo, ByVal portalSettings As PortalSettings) As Integer

#End Region

 End Interface

 ' ILinkable contains the core methods needed for enabling Trackback and Pingback
 ' and for making use of core services that may be provided in the publishing adapter
 ' used to integrate with your publishing interfaces.  The blog module currently hanldes
 ' its own trackback capability
 Public Interface ILinkable
  Function GetModuleName(ByVal moduleLevelId As String, ByVal userInfo As UserInfo, ByVal portalSettings As PortalSettings) As String

  Function GetPermaLink(ByVal moduleLevelId As String, ByVal itemId As String, ByVal userInfo As UserInfo, ByVal portalSettings As PortalSettings) As String

  Function GetPingbackSettings(ByVal moduleLevelId As String, ByVal userInfo As UserInfo, ByVal portalSettings As PortalSettings) As TrackbackAndPingSettings

 End Interface

 ' IWrappable contains methods needed by publishing adapters which are written to use 
 ' IPublishable and which allow for Header and Footer content to be written into each
 ' post.  The current version of the blog module does not support headers and footers.
 Public Interface IWrappable
  ''' <summary>
  ''' HeaderContent is not used by the blog module.  
  ''' </summary>
  ReadOnly Property HeaderContent() As String
  ''' <summary>
  ''' FooterContent is not used by the blog module.  
  ''' </summary>
  ReadOnly Property FooterContent() As String
 End Interface

End Namespace
