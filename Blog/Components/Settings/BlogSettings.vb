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
Imports System.Xml
Imports DotNetNuke.Modules.Blog.Components.Common
Imports DotNetNuke.Common.Utilities

Namespace Components.Settings


    ''' <summary>
    ''' This class abstracts all settings for the module and makes sure they're (a) defaulted and (b) hard typed 
    ''' throughout the application.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''		[pdonker]	12/14/2009	created
    ''' </history>
    Public Class BlogSettings

#Region "Private Members"

        Private _allSettings As Hashtable
        Private _PageBlogs As Integer = -1
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
        Private _ShowCommentTitle As Boolean = False
        Private _AllowCommentAnchors As Boolean = True
        Private _AllowCommentImages As Boolean = False
        Private _AllowCommentFormatting As Boolean = True
        Private _ForumBlogInstalled As String = "None"
        Private _ShowGravatars As Boolean = False
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
        Private _allowSummaryHtml As Boolean = True
        Private _excerptEnabled As Boolean = False
        Private _feedCacheTime As Integer = 10
        Private _AllowChildBlogs As Boolean = False
        Private _allowWLW As Boolean = True
        Private _EnableArchiveDropDown As Boolean = True
        Private _AllowMultipleCategories As Boolean = True
        Private _useWLWExcerpt As Boolean = False
        Private _includeFiles As String = ""

        Private _portalId As Integer = -1
        Private _tabId As Integer = -1

        Private Const version As String = "03.05.00"
        Private _vocabularyId As Integer = 1
        Private _enableGhostWriter As Boolean = False

#End Region

#Region "Constructors"

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

            Globals.ReadValue(_allSettings, "PageBlogs", PageBlogs)
            Globals.ReadValue(_allSettings, "EntryDescriptionRequired", EntryDescriptionRequired)
            Globals.ReadValue(_allSettings, "SummaryMaxLength", SummaryMaxLength)
            Globals.ReadValue(_allSettings, "SearchSummaryMaxLength", SearchSummaryMaxLength)
            Globals.ReadValue(_allSettings, "MaxImageWidth", MaxImageWidth)
            Globals.ReadValue(_allSettings, "RecentEntriesMax", RecentEntriesMax)
            Globals.ReadValue(_allSettings, "RecentRssEntriesMax", RecentRssEntriesMax)
            Globals.ReadValue(_allSettings, "SearchBlogContent", SearchBlogContent)
            Globals.ReadValue(_allSettings, "SearchBlogComment", SearchBlogComment)
            Globals.ReadValue(_allSettings, "EnableUploadOption", EnableUploadOption)
            Globals.ReadValue(_allSettings, "ShowSummary", ShowSummary)
            Globals.ReadValue(_allSettings, "ShowCommentTitle", ShowCommentTitle)
            Globals.ReadValue(_allSettings, "AllowCommentAnchors", AllowCommentAnchors)
            Globals.ReadValue(_allSettings, "AllowCommentImages", AllowCommentImages)
            Globals.ReadValue(_allSettings, "AllowCommentFormatting", AllowCommentFormatting)
            Globals.ReadValue(_allSettings, "ForumBlogInstalled", ForumBlogInstalled)
            Globals.ReadValue(_allSettings, "ShowGravatars", ShowGravatars)
            Globals.ReadValue(_allSettings, "GravatarImageWidth", GravatarImageWidth)
            Globals.ReadValue(_allSettings, "GravatarRating", GravatarRating)
            Globals.ReadValue(_allSettings, "ShowWebsite", ShowWebsite)
            Globals.ReadValue(_allSettings, "ShowSeoFriendlyUrl", ShowSeoFriendlyUrl)
            Globals.ReadValue(_allSettings, "EnforceSummaryTruncation", EnforceSummaryTruncation)
            Globals.ReadValue(_allSettings, "DataVersion", DataVersion)
            Globals.ReadValue(_allSettings, "IncludeBody", IncludeBody)
            Globals.ReadValue(_allSettings, "IncludeCategoriesInDescription", IncludeCategoriesInDescription)
            Globals.ReadValue(_allSettings, "IncludeTagsInDescription", IncludeTagsInDescription)
            Globals.ReadValue(_allSettings, "GravatarDefaultImageUrl", GravatarDefaultImageUrl)
            Globals.ReadValue(_allSettings, "GravatarCustomUrl", GravatarCustomUrl)
            Globals.ReadValue(_allSettings, "AllowSummaryHtml", AllowSummaryHtml)
            Globals.ReadValue(_allSettings, "FeedCacheTime", FeedCacheTime)
            Globals.ReadValue(_allSettings, "AllowChildBlogs", AllowChildBlogs)
            Globals.ReadValue(_allSettings, "AllowWLW", AllowWLW)
            Globals.ReadValue(_allSettings, "EnableArchiveDropDown", EnableArchiveDropDown)
            Globals.ReadValue(_allSettings, "IncludeFiles", IncludeFiles)

            ' WLW implementation parameters
            Globals.ReadValue(_allSettings, "AllowMultipleCategories", AllowMultipleCategories)
            Globals.ReadValue(_allSettings, "UseWLWExcerpt", UseWLWExcerpt)

            Globals.ReadValue(_allSettings, "VocabularyId", VocabularyId)

            If DataVersion < version Then
                DataVersion = version
                Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "DataVersion", version)
                Business.Utility.UpgradeApplication(PortalID, version)
            End If

            Try
                Dim wlwSettings As New System.Xml.XmlDocument
                wlwSettings.Load(DotNetNuke.Common.ApplicationMapPath & "\DesktopModules\Blog\wlwblog.xml")
                Dim nsm As New System.Xml.XmlNamespaceManager(wlwSettings.NameTable)
                nsm.AddNamespace("wlw", "http://schemas.microsoft.com/wlw/manifest/weblog")
                Dim n As System.Xml.XmlNode = wlwSettings.DocumentElement.SelectSingleNode("wlw:options/wlw:supportsExcerpt", nsm)
                If n IsNot Nothing Then
                    _excerptEnabled = CBool(n.InnerText.ToLower = "yes")
                End If
            Catch
            End Try
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

#Region "Public Members"

        Public Overridable Sub UpdateSettings()
            Dim objModules As New DotNetNuke.Entities.Modules.ModuleController
            Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "PageBlogs", Me.PageBlogs.ToString)
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
            Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "AllowSummaryHtml", Me.AllowSummaryHtml.ToString)
            Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "FeedCacheTime", Me.FeedCacheTime.ToString)
            Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "AllowChildBlogs", Me.AllowChildBlogs.ToString)
            Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "AllowWLW", Me.AllowWLW.ToString)
            Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "EnableArchiveDropDown", Me.EnableArchiveDropDown.ToString)
            Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "IncludeFiles", Me.IncludeFiles)

            ' WLW implementation parameters
            Business.Utility.UpdateBlogModuleSetting(_portalId, -1, "AllowMultipleCategories", Me.AllowMultipleCategories.ToString)
            Business.Utility.UpdateBlogModuleSetting(_portalId, -1, "UseWLWExcerpt", Me.UseWLWExcerpt.ToString)


            ' We save this at 'global' and tab level so its available to WLW as well as normal module usage.
            Business.Utility.UpdateBlogModuleSetting(_portalId, -1, "VocabularyId", Me.VocabularyId.ToString)
            Business.Utility.UpdateBlogModuleSetting(_portalId, _tabId, "VocabularyId", Me.VocabularyId.ToString)

            ' Social Sharing


            ' Clear cache
            DataCache.ClearCache(Constants.ModuleCacheKeyPrefix + Constants.VocabTermsCacheKey + Constants.VocabSuffixCacheKey + VocabularyId.ToString())
            DataCache.ClearCache(Constants.ModuleCacheKeyPrefix + Constants.VocabTermsCacheKey + Constants.VocabSuffixCacheKey + "1") ' Tags

            Dim CacheKey As String = "BlogSettings" & _portalId.ToString & "-" & _tabId.ToString
            DotNetNuke.Common.Utilities.DataCache.RemoveCache(CacheKey)
        End Sub

        Public Sub WriteWLWManifest(ByRef output As XmlTextWriter)
            output.Formatting = Formatting.Indented
            output.WriteStartDocument()
            output.WriteStartElement("manifest")
            output.WriteAttributeString("xmlns", "http://schemas.microsoft.com/wlw/manifest/weblog")
            output.WriteStartElement("options")

            output.WriteElementString("clientType", "Metaweblog")
            output.WriteElementString("supportsMultipleCategories", Globals.CYesNo(AllowMultipleCategories))
            output.WriteElementString("supportsCategories", "Yes")
            output.WriteElementString("supportsCustomDate", "Yes")
            output.WriteElementString("supportsKeywords", "Yes")
            output.WriteElementString("supportsTrackbacks", "Yes")
            output.WriteElementString("supportsEmbeds", "No")
            output.WriteElementString("supportsAuthor", "No")
            output.WriteElementString("supportsExcerpt", Globals.CYesNo(UseWLWExcerpt))
            output.WriteElementString("supportsPassword", "No")
            output.WriteElementString("supportsPages", "No")
            output.WriteElementString("supportsPageParent", "No")
            output.WriteElementString("supportsPageOrder", "No")
            output.WriteElementString("supportsExtendedEntries", "Yes")
            output.WriteElementString("supportsCommentPolicy", "Yes")
            output.WriteElementString("supportsPingPolicy", "Yes")
            output.WriteElementString("supportsPostAsDraft", "Yes")
            output.WriteElementString("supportsFileUpload", "Yes")
            output.WriteElementString("supportsSlug", "No")
            output.WriteElementString("supportsHierarchicalCategories", "Yes")
            output.WriteElementString("supportsCategoriesInline", "Yes")
            output.WriteElementString("supportsNewCategories", "No")
            output.WriteElementString("supportsNewCategoriesInline", "No")

            output.WriteEndElement() ' manifest
            output.WriteEndElement() ' options
            output.Flush()
        End Sub

#End Region

#Region "Properties"

        Public Property PageBlogs() As Integer
            Get
                Return _PageBlogs
            End Get
            Set(ByVal Value As Integer)
                _PageBlogs = Value
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

        Public Property AllowSummaryHtml() As Boolean
            Get
                Return _allowSummaryHtml
            End Get
            Set(ByVal value As Boolean)
                _allowSummaryHtml = value
            End Set
        End Property

        Public ReadOnly Property ExcerptEnabled() As Boolean
            Get
                Return _excerptEnabled
            End Get
        End Property

        Public Property FeedCacheTime() As Integer
            Get
                Return _feedCacheTime
            End Get
            Set(ByVal value As Integer)
                _feedCacheTime = value
            End Set
        End Property

        Public Property AllowChildBlogs() As Boolean
            Get
                Return _AllowChildBlogs
            End Get
            Set(ByVal value As Boolean)
                _AllowChildBlogs = value
            End Set
        End Property

        Public Property AllowWLW() As Boolean
            Get
                Return _allowWLW
            End Get
            Set(ByVal value As Boolean)
                _allowWLW = value
            End Set
        End Property

        ''' <summary>
        ''' Used to determine if the archive list should display as a data list of links (default) or as a drop down list.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>CP: Added</remarks>
        Public Property EnableArchiveDropDown() As Boolean
            Get
                Return _EnableArchiveDropDown
            End Get
            Set(ByVal value As Boolean)
                _EnableArchiveDropDown = value
            End Set
        End Property

        Public Property AllowMultipleCategories() As Boolean
            Get
                Return _AllowMultipleCategories
            End Get
            Set(ByVal value As Boolean)
                _AllowMultipleCategories = value
            End Set
        End Property

        Public Property UseWLWExcerpt() As Boolean
            Get
                Return _useWLWExcerpt
            End Get
            Set(ByVal value As Boolean)
                _useWLWExcerpt = value
            End Set
        End Property

        Public Property IncludeFiles() As String
            Get
                Return _includeFiles
            End Get
            Set(ByVal value As String)
                _includeFiles = value
            End Set
        End Property

        ''' <summary>
        ''' This is the base vocabulary (restricted to current portal level) to be used for all categories selection options.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property VocabularyId() As Integer
            Get
                Return _vocabularyId
            End Get
            Set(value As Integer)
                _vocabularyId = value
            End Set
        End Property

#End Region

    End Class
End Namespace