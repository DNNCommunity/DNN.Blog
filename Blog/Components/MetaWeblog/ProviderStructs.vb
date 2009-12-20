Imports System
Imports System.Text

Namespace MetaWeblog
 Public Structure Item
  Public DateCreated As DateTime
  Public StartDate As DateTime
  Public ModuleTitle As String
  Public Content As String
  ''' <summary>
  ''' Maps to the supportsExtendedEntries manifest file entry.
  ''' </summary>
  Public ExtendedContent As String
  Public Title As String
  Public Categories As String()
  Public Keywords As String
  Public AllowComments As Integer
  Public AllowTrackbacksOrPings As Integer
  'public Enclosure enclosure;     // Not currently used
  Public Link As String
  Public Permalink As String
  Public ItemId As Object
  ''' <summary>
  ''' Not currently used by the blog module
  ''' </summary>
  Public SeoFriendlyTitle As String
  ''' <summary>
  ''' Not currently used by the blog module
  ''' </summary>
  Public ItemPassword As String
  ''' <summary>
  ''' Not currently used by the blog module
  ''' </summary>
  Public ParentId As String
  ''' <summary>
  ''' Not currently used by the blog module
  ''' </summary>
  Public PageOrder As String
  ''' <summary>
  ''' Not currently used by the blog module
  ''' </summary>
  Public AuthorId As String
  ''' <summary>
  ''' Used to track the blog entry summary.
  ''' </summary>
  Public Summary As String
  ''' <summary>
  ''' Used to manage trackback URLs.  Will only be populated if 
  ''' supportsTrackbacks is set to yes in the manifest file.
  ''' </summary>
  Public PingUrls As String()
  Public Publish As Boolean
  Public ItemType As ItemType
  Public StyleDetectionPost As Boolean
  Public StyleId As String
 End Structure

 ''' <summary>
 ''' The concept of the module here is fluid and may not represent the actual
 ''' module for the implementation of your provider.  For example, with the Blog
 ''' module, the moduleLevelId is actually set to the blogId.  The purpose of this
 ''' struct is to track units where items are aggregated.  If your module doesn't 
 ''' have an object like this, then just return information related to modules 
 ''' to which the currently logged on user would have access to either insert,
 ''' update or delete items.
 ''' </summary>
 Public Structure ModuleInfoStruct
  Public ModuleId As String
  Public Url As String
  Public ModuleName As String
 End Structure
 Public Structure ItemCategoryInfo
  Public CategoryId As Integer
  Public ParentId As Integer
  Public Description As String
  Public CategoryName As String
  Public HtmlUrl As String
  Public RssUrl As String
 End Structure
 Public Structure ItemCategory
  Public CategoryId As String
  Public CategoryName As String
 End Structure
 Public Structure NewItemCategory
  Public Name As String
  Public Slug As String
  Public ParentId As Integer
  Public Description As String
 End Structure
 Public Structure ItemMediaObject
  Public Name As String
  Public Type As String
  Public Bits As Byte()
 End Structure
 Public Structure DnnModuleInfoStruct
  Public TabModuleId As Integer
  Public ModuleId As Integer
  Public TabId As Integer
  Public TabPath As String
  Public ModuleTitle As String
  Public CreatedDate As DateTime
 End Structure
 Public Structure TrackbackAndPingSettings
  Public AllowForPost As Boolean
  Public AllowForPage As Boolean
  Public allowAutoDiscovery As Boolean
  Public allowOverrideByPingDropDown As Boolean
 End Structure
 Public Structure ImageFileInfo
  Public Extension As String
  Public Size As Long
  Public Width As Integer
  Public Height As Integer
  Public ContentType As String
 End Structure
 Public Structure ProviderStruct
  Public ProviderKey As String
  Public ProviderAssemblyName As String
  Public ProviderTypeName As String
  Public ManifestFileName As String
  Public ManifestPath As String
  Public DesktopModuleId As Integer
 End Structure
End Namespace
