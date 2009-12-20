Public Class BlogSettings

#Region " Private Members "
 Private _allSettings As Hashtable
 Private _PageBlogs As Integer = -1
 Private _EnableDNNSearch As Boolean = False
 Private _EntryDescriptionRequired As Boolean = False
 Private _SummaryMaxLength As Integer = 1024
 Private _SearchSummaryMaxLength As Integer = 255
 Private _MaxImageWidth As Integer = 400
 Private _RecentEntriesMax As Integer = 10
 Private _RecentRssEntriesMax As Integer = 10
 Private _SearchBlogContent As Boolean = False
 Private _SearchBlogComment As Boolean = False
 Private _EnableUploadOption As Boolean = False
 Private _ShowSummary As Boolean = False
 Private _ShowUniqueTitle As Boolean = True
 Private _ShowCommentTitle As Boolean = True
 Private _AllowCommentAnchors As Boolean = True
 Private _AllowCommentImages As Boolean = False
 Private _AllowCommentFormatting As Boolean = True
 Private _ForumBlogInstalled As String = "None"
 Private _ShowGravatars As Boolean = True
 Private _GravatarImageWidth As Integer = 48
 Private _GravatarRating As String = "G"
 Private _ShowWebsite As Boolean = True
 Private _ShowSeoFriendlyUrl As Boolean = True
 Private _EnforceSummaryTruncation As Boolean = False
 Private _DataVersion As String = "00.00.00"
 Private _IncludeBody As Boolean = False
 Private _IncludeCategoriesInDescription As Boolean = True
 Private _IncludeTagsInDescription As Boolean = True
 Private _GravatarDefaultImageUrl As String = ""
 Private _GravatarCustomUrl As String = ""
 Private _ShowSocialBookmarks As Boolean = True

 Private _portalId As Integer = -1
 Private _tabId As Integer = -1

 Private Const version As String = "03.05.00"
#End Region

#Region " Constructors "
 Public Sub New(ByVal PortalID As Integer, ByVal TabID As Integer)

  _portalId = PortalID
  _tabId = TabID
  Dim mc As New DotNetNuke.Entities.Modules.ModuleController
  _allSettings = New Hashtable
  Dim dr As IDataReader = DotNetNuke.Modules.Blog.Data.DataProvider.Instance().GetBlogModuleSettings(PortalID, TabID)
  While dr.Read()
   _allSettings(dr.GetString(0)) = dr.GetValue(1)
  End While
  dr.Close()

  ReadValue(_allSettings, "PageBlogs", PageBlogs)
  ReadValue(_allSettings, "EnableDNNSearch", EnableDNNSearch)
  ReadValue(_allSettings, "EntryDescriptionRequired", EntryDescriptionRequired)
  ReadValue(_allSettings, "SummaryMaxLength", SummaryMaxLength)
  ReadValue(_allSettings, "SearchSummaryMaxLength", SearchSummaryMaxLength)
  ReadValue(_allSettings, "MaxImageWidth", MaxImageWidth)
  ReadValue(_allSettings, "RecentEntriesMax", RecentEntriesMax)
  ReadValue(_allSettings, "RecentRssEntriesMax", RecentRssEntriesMax)
  ReadValue(_allSettings, "SearchBlogContent", SearchBlogContent)
  ReadValue(_allSettings, "SearchBlogComment", SearchBlogComment)
  ReadValue(_allSettings, "EnableUploadOption", EnableUploadOption)
  ReadValue(_allSettings, "ShowSummary", ShowSummary)
  ReadValue(_allSettings, "ShowUniqueTitle", ShowUniqueTitle)
  ReadValue(_allSettings, "ShowCommentTitle", ShowCommentTitle)
  ReadValue(_allSettings, "AllowCommentAnchors", AllowCommentAnchors)
  ReadValue(_allSettings, "AllowCommentImages", AllowCommentImages)
  ReadValue(_allSettings, "AllowCommentFormatting", AllowCommentFormatting)
  ReadValue(_allSettings, "ForumBlogInstalled", ForumBlogInstalled)
  ReadValue(_allSettings, "ShowGravatars", ShowGravatars)
  ReadValue(_allSettings, "GravatarImageWidth", GravatarImageWidth)
  ReadValue(_allSettings, "GravatarRating", GravatarRating)
  ReadValue(_allSettings, "ShowWebsite", ShowWebsite)
  ReadValue(_allSettings, "ShowSeoFriendlyUrl", ShowSeoFriendlyUrl)
  ReadValue(_allSettings, "EnforceSummaryTruncation", EnforceSummaryTruncation)
  ReadValue(_allSettings, "DataVersion", DataVersion)
  ReadValue(_allSettings, "IncludeBody", IncludeBody)
  ReadValue(_allSettings, "IncludeCategoriesInDescription", IncludeCategoriesInDescription)
  ReadValue(_allSettings, "IncludeTagsInDescription", IncludeTagsInDescription)
  ReadValue(_allSettings, "GravatarDefaultImageUrl", GravatarDefaultImageUrl)
  ReadValue(_allSettings, "GravatarCustomUrl", GravatarCustomUrl)
  ReadValue(_allSettings, "ShowSocialBookmarks", ShowSocialBookmarks)

  If DataVersion < version Then
   DataVersion = version
   Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "DataVersion", version)
   Business.Utility.UpgradeApplication(PortalID, version)
  End If

 End Sub

 Public Shared Function GetBlogSettings(ByVal PortalId As Integer, ByVal TabId As Integer) As BlogSettings
  Dim CacheKey As String = "BlogSettings" & PortalId.ToString & "-" & TabId.ToString
  Dim bs As BlogSettings = CType(DotNetNuke.Common.Utilities.DataCache.GetCache(CacheKey), BlogSettings)
  If bs Is Nothing Then
   bs = New BlogSettings(PortalId, TabId)
   DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey, bs)
  End If
  Return bs
 End Function
#End Region

#Region " Public Members "
 Public Overridable Sub UpdateSettings()
  Dim objModules As New DotNetNuke.Entities.Modules.ModuleController
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "PageBlogs", Me.PageBlogs.ToString)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "EnableDNNSearch", Me.EnableDNNSearch.ToString)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "EntryDescriptionRequired", Me.EntryDescriptionRequired.ToString)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "SummaryMaxLength", Me.SummaryMaxLength.ToString)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "SearchSummaryMaxLength", Me.SearchSummaryMaxLength.ToString)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "MaxImageWidth", Me.MaxImageWidth.ToString)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "RecentEntriesMax", Me.RecentEntriesMax.ToString)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "RecentRssEntriesMax", Me.RecentRssEntriesMax.ToString)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "SearchBlogContent", Me.SearchBlogContent.ToString)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "SearchBlogComment", Me.SearchBlogComment.ToString)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "EnableUploadOption", Me.EnableUploadOption.ToString)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "ShowSummary", Me.ShowSummary.ToString)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "ShowUniqueTitle", Me.ShowUniqueTitle.ToString)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "ShowCommentTitle", Me.ShowCommentTitle.ToString)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "AllowCommentAnchors", Me.AllowCommentAnchors.ToString)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "AllowCommentImages", Me.AllowCommentImages.ToString)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "AllowCommentFormatting", Me.AllowCommentFormatting.ToString)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "ForumBlogInstalled", Me.ForumBlogInstalled)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "ShowGravatars", Me.ShowGravatars.ToString)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "GravatarImageWidth", Me.GravatarImageWidth.ToString)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "GravatarRating", Me.GravatarRating)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "ShowWebsite", Me.ShowWebsite.ToString)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "ShowSeoFriendlyUrl", Me.ShowSeoFriendlyUrl.ToString)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "EnforceSummaryTruncation", Me.EnforceSummaryTruncation.ToString)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "DataVersion", Me.DataVersion)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "IncludeBody", Me.IncludeBody.ToString)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "IncludeCategoriesInDescription", Me.IncludeCategoriesInDescription.ToString)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "IncludeTagsInDescription", Me.IncludeTagsInDescription.ToString)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "GravatarDefaultImageUrl", Me.GravatarDefaultImageUrl)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "GravatarCustomUrl", Me.GravatarCustomUrl)
  Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "ShowSocialBookmarks", Me.ShowSocialBookmarks.ToString)

 End Sub

#End Region

#Region " Properties "

 Public Property PageBlogs() As Integer
  Get
   Return _PageBlogs
  End Get
  Set(ByVal Value As Integer)
   _PageBlogs = Value
  End Set
 End Property

 Public Property EnableDNNSearch() As Boolean
  Get
   Return _EnableDNNSearch
  End Get
  Set(ByVal Value As Boolean)
   _EnableDNNSearch = Value
  End Set
 End Property

 Public Property EntryDescriptionRequired() As Boolean
  Get
   Return _EntryDescriptionRequired
  End Get
  Set(ByVal Value As Boolean)
   _EntryDescriptionRequired = Value
  End Set
 End Property

 Public Property SummaryMaxLength() As Integer
  Get
   Return _SummaryMaxLength
  End Get
  Set(ByVal Value As Integer)
   _SummaryMaxLength = Value
  End Set
 End Property

 Public Property SearchSummaryMaxLength() As Integer
  Get
   Return _SearchSummaryMaxLength
  End Get
  Set(ByVal Value As Integer)
   _SearchSummaryMaxLength = Value
  End Set
 End Property

 Public Property MaxImageWidth() As Integer
  Get
   Return _MaxImageWidth
  End Get
  Set(ByVal Value As Integer)
   _MaxImageWidth = Value
  End Set
 End Property

 Public Property RecentEntriesMax() As Integer
  Get
   Return _RecentEntriesMax
  End Get
  Set(ByVal Value As Integer)
   _RecentEntriesMax = Value
  End Set
 End Property

 Public Property RecentRssEntriesMax() As Integer
  Get
   Return _RecentRssEntriesMax
  End Get
  Set(ByVal Value As Integer)
   _RecentRssEntriesMax = Value
  End Set
 End Property

 Public Property SearchBlogContent() As Boolean
  Get
   Return _SearchBlogContent
  End Get
  Set(ByVal Value As Boolean)
   _SearchBlogContent = Value
  End Set
 End Property

 Public Property SearchBlogComment() As Boolean
  Get
   Return _SearchBlogComment
  End Get
  Set(ByVal Value As Boolean)
   _SearchBlogComment = Value
  End Set
 End Property

 Public Property EnableUploadOption() As Boolean
  Get
   Return _EnableUploadOption
  End Get
  Set(ByVal Value As Boolean)
   _EnableUploadOption = Value
  End Set
 End Property

 Public Property ShowSummary() As Boolean
  Get
   Return _ShowSummary
  End Get
  Set(ByVal Value As Boolean)
   _ShowSummary = Value
  End Set
 End Property

 Public Property ShowUniqueTitle() As Boolean
  Get
   Return _ShowUniqueTitle
  End Get
  Set(ByVal Value As Boolean)
   _ShowUniqueTitle = Value
  End Set
 End Property

 Public Property ShowCommentTitle() As Boolean
  Get
   Return _ShowCommentTitle
  End Get
  Set(ByVal Value As Boolean)
   _ShowCommentTitle = Value
  End Set
 End Property

 Public Property AllowCommentAnchors() As Boolean
  Get
   Return _AllowCommentAnchors
  End Get
  Set(ByVal Value As Boolean)
   _AllowCommentAnchors = Value
  End Set
 End Property

 Public Property AllowCommentImages() As Boolean
  Get
   Return _AllowCommentImages
  End Get
  Set(ByVal Value As Boolean)
   _AllowCommentImages = Value
  End Set
 End Property

 Public Property AllowCommentFormatting() As Boolean
  Get
   Return _AllowCommentFormatting
  End Get
  Set(ByVal Value As Boolean)
   _AllowCommentFormatting = Value
  End Set
 End Property

 Public Property ForumBlogInstalled() As String
  Get
   Return _ForumBlogInstalled
  End Get
  Set(ByVal Value As String)
   _ForumBlogInstalled = Value
  End Set
 End Property

 Public Property ShowGravatars() As Boolean
  Get
   Return _ShowGravatars
  End Get
  Set(ByVal Value As Boolean)
   _ShowGravatars = Value
  End Set
 End Property

 Public Property GravatarImageWidth() As Integer
  Get
   Return _GravatarImageWidth
  End Get
  Set(ByVal Value As Integer)
   _GravatarImageWidth = Value
  End Set
 End Property

 Public Property GravatarRating() As String
  Get
   Return _GravatarRating
  End Get
  Set(ByVal Value As String)
   _GravatarRating = Value
  End Set
 End Property

 Public Property ShowWebsite() As Boolean
  Get
   Return _ShowWebsite
  End Get
  Set(ByVal Value As Boolean)
   _ShowWebsite = Value
  End Set
 End Property

 Public Property ShowSeoFriendlyUrl() As Boolean
  Get
   Return _ShowSeoFriendlyUrl
  End Get
  Set(ByVal Value As Boolean)
   _ShowSeoFriendlyUrl = Value
  End Set
 End Property

 Public Property EnforceSummaryTruncation() As Boolean
  Get
   Return _EnforceSummaryTruncation
  End Get
  Set(ByVal Value As Boolean)
   _EnforceSummaryTruncation = Value
  End Set
 End Property

 Public Property DataVersion() As String
  Get
   Return _DataVersion
  End Get
  Set(ByVal Value As String)
   _DataVersion = Value
  End Set
 End Property

 Public Property IncludeBody() As Boolean
  Get
   Return _IncludeBody
  End Get
  Set(ByVal Value As Boolean)
   _IncludeBody = Value
  End Set
 End Property

 Public Property IncludeCategoriesInDescription() As Boolean
  Get
   Return _IncludeCategoriesInDescription
  End Get
  Set(ByVal Value As Boolean)
   _IncludeCategoriesInDescription = Value
  End Set
 End Property

 Public Property IncludeTagsInDescription() As Boolean
  Get
   Return _IncludeTagsInDescription
  End Get
  Set(ByVal Value As Boolean)
   _IncludeTagsInDescription = Value
  End Set
 End Property

 Public Property GravatarDefaultImageUrl() As String
  Get
   Return _GravatarDefaultImageUrl
  End Get
  Set(ByVal Value As String)
   _GravatarDefaultImageUrl = Value
  End Set
 End Property

 Public Property GravatarCustomUrl() As String
  Get
   Return _GravatarCustomUrl
  End Get
  Set(ByVal Value As String)
   _GravatarCustomUrl = Value
  End Set
 End Property

 Public Property ShowSocialBookmarks() As Boolean
  Get
   Return _ShowSocialBookmarks
  End Get
  Set(ByVal Value As Boolean)
   _ShowSocialBookmarks = Value
  End Set
 End Property

#End Region

#Region " Support Methods "
 Public Shared Sub ReadValue(ByRef ValueTable As Hashtable, ByVal ValueName As String, ByRef Variable As Integer)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Integer)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As Hashtable, ByVal ValueName As String, ByRef Variable As Long)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Long)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As Hashtable, ByVal ValueName As String, ByRef Variable As String)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), String)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As Hashtable, ByVal ValueName As String, ByRef Variable As Boolean)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Boolean)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As Hashtable, ByVal ValueName As String, ByRef Variable As Date)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Date)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As Hashtable, ByVal ValueName As String, ByRef Variable As TimeSpan)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = TimeSpan.Parse(CType(ValueTable.Item(ValueName), String))
   Catch ex As Exception
   End Try
  End If
 End Sub
#End Region

End Class

