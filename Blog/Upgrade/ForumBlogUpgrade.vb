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

Imports System
Imports System.Data
Imports DotNetNuke.Modules.Blog.Providers.Data
Imports DotNetNuke.Framework
Imports DotNetNuke.Common.Utilities

Namespace Business

#Region "Forum_Groups Upgrade"

#Region "Forum_GroupsInfo"
 Public Class Forum_GroupsInfo

#Region "Private Members"
  Dim _groupID As Integer
  Dim _name As String
  Dim _portalID As Integer
  Dim _moduleID As Integer
  Dim _sortOrder As Integer
  Dim _createdDate As DateTime
  Dim _createdByUser As Integer
  Dim _updatedByUser As Integer
  Dim _updatedDate As DateTime
  Dim _groupType As Integer
  Dim _server As String
  Dim _port As Integer
  Dim _logonRequired As Boolean
  Dim _userName As String
  Dim _password As String
#End Region

#Region "Constructors"
  Public Sub New()
  End Sub

  Public Sub New(ByVal groupID As Integer, ByVal name As String, ByVal portalID As Integer, ByVal moduleID As Integer, ByVal sortOrder As Integer, ByVal createdDate As DateTime, ByVal createdByUser As Integer, ByVal updatedByUser As Integer, ByVal updatedDate As DateTime, ByVal groupType As Integer, ByVal server As String, ByVal port As Integer, ByVal logonRequired As Boolean, ByVal userName As String, ByVal password As String)
   Me.GroupID = groupID
   Me.Name = name
   Me.PortalID = portalID
   Me.ModuleID = moduleID
   Me.SortOrder = sortOrder
   Me.CreatedDate = createdDate
   Me.CreatedByUser = createdByUser
   Me.UpdatedByUser = updatedByUser
   Me.UpdatedDate = updatedDate
   Me.GroupType = groupType
   Me.Server = server
   Me.Port = port
   Me.LogonRequired = logonRequired
   Me.UserName = userName
   Me.Password = password
  End Sub
#End Region

#Region "Public Properties"
  Public Property GroupID() As Integer
   Get
    Return _groupID
   End Get
   Set(ByVal Value As Integer)
    _groupID = Value
   End Set
  End Property

  Public Property Name() As String
   Get
    Return _name
   End Get
   Set(ByVal Value As String)
    _name = Value
   End Set
  End Property

  Public Property PortalID() As Integer
   Get
    Return _portalID
   End Get
   Set(ByVal Value As Integer)
    _portalID = Value
   End Set
  End Property

  Public Property ModuleID() As Integer
   Get
    Return _moduleID
   End Get
   Set(ByVal Value As Integer)
    _moduleID = Value
   End Set
  End Property

  Public Property SortOrder() As Integer
   Get
    Return _sortOrder
   End Get
   Set(ByVal Value As Integer)
    _sortOrder = Value
   End Set
  End Property

  Public Property CreatedDate() As DateTime
   Get
    Return _createdDate
   End Get
   Set(ByVal Value As DateTime)
    _createdDate = Value
   End Set
  End Property

  Public Property CreatedByUser() As Integer
   Get
    Return _createdByUser
   End Get
   Set(ByVal Value As Integer)
    _createdByUser = Value
   End Set
  End Property

  Public Property UpdatedByUser() As Integer
   Get
    Return _updatedByUser
   End Get
   Set(ByVal Value As Integer)
    _updatedByUser = Value
   End Set
  End Property

  Public Property UpdatedDate() As DateTime
   Get
    Return _updatedDate
   End Get
   Set(ByVal Value As DateTime)
    _updatedDate = Value
   End Set
  End Property

  Public Property GroupType() As Integer
   Get
    Return _groupType
   End Get
   Set(ByVal Value As Integer)
    _groupType = Value
   End Set
  End Property

  Public Property Server() As String
   Get
    Return _server
   End Get
   Set(ByVal Value As String)
    _server = Value
   End Set
  End Property

  Public Property Port() As Integer
   Get
    Return _port
   End Get
   Set(ByVal Value As Integer)
    _port = Value
   End Set
  End Property

  Public Property LogonRequired() As Boolean
   Get
    Return _logonRequired
   End Get
   Set(ByVal Value As Boolean)
    _logonRequired = Value
   End Set
  End Property

  Public Property UserName() As String
   Get
    Return _userName
   End Get
   Set(ByVal Value As String)
    _userName = Value
   End Set
  End Property

  Public Property Password() As String
   Get
    Return _password
   End Get
   Set(ByVal Value As String)
    _password = Value
   End Set
  End Property
#End Region

 End Class
#End Region

#Region "class Forum_GroupsController"
 Public Class Forum_GroupsController

#Region "Public Methods"

  Public Function List(ByVal PortalId As Integer) As ArrayList

   Return CBO.FillCollection(DataProvider.Instance().Upgrade_ListForum_Groups(PortalId), GetType(Forum_GroupsInfo))

  End Function

  Public Function GetByGroupID(ByVal groupID As Integer) As Forum_GroupsInfo

   Return CType(CBO.FillObject(DataProvider.Instance().Upgrade_ListForum_GroupsByGroup(groupID), GetType(Forum_GroupsInfo)), Forum_GroupsInfo)

  End Function

#End Region

 End Class

#End Region

#End Region

#Region "Forum_Forums Upgrade"  ' Blogs

#Region "Forums_ForumsInfo"
 Public Class Forum_ForumsInfo

#Region "Private Members"
  Dim _forumID As Integer
  Dim _groupID As Integer
  Dim _isActive As Boolean
  Dim _parentID As Integer
  Dim _name As String
  Dim _description As String
  Dim _createdDate As DateTime
  Dim _createdByUser As Integer
  Dim _updatedByUser As Integer
  Dim _updatedDate As DateTime
  Dim _isModerated As Boolean
  Dim _daysToView As Integer
  Dim _sortOrder As Integer
  Dim _totalPosts As Integer
  Dim _totalThreads As Integer
  Dim _enablePostStatistics As Boolean
  Dim _enableAutoDelete As Boolean
  Dim _autoDeleteThreshold As Integer
  Dim _mostRecentPostID As Integer
  Dim _mostRecentThreadID As Integer
  Dim _mostRecentPostAuthorID As Integer
  Dim _mostRecentPostDate As DateTime
  Dim _postsToModerate As Integer
  Dim _forumType As Integer
  Dim _isIntegrated As Boolean
  Dim _integratedModuleID As Integer
  Dim _integratedObjects As String
  Dim _isPrivate As Boolean
  Dim _authorizedRoles As String
  Dim _authorizedEditRoles As String
#End Region

#Region "Constructors"
  Public Sub New()
  End Sub

  Public Sub New(ByVal forumID As Integer, ByVal groupID As Integer, ByVal isActive As Boolean, ByVal parentID As Integer, ByVal name As String, ByVal description As String, ByVal createdDate As DateTime, ByVal createdByUser As Integer, ByVal updatedByUser As Integer, ByVal updatedDate As DateTime, ByVal isModerated As Boolean, ByVal daysToView As Integer, ByVal sortOrder As Integer, ByVal totalPosts As Integer, ByVal totalThreads As Integer, ByVal enablePostStatistics As Boolean, ByVal enableAutoDelete As Boolean, ByVal autoDeleteThreshold As Integer, ByVal mostRecentPostID As Integer, ByVal mostRecentThreadID As Integer, ByVal mostRecentPostAuthorID As Integer, ByVal mostRecentPostDate As DateTime, ByVal postsToModerate As Integer, ByVal forumType As Integer, ByVal isIntegrated As Boolean, ByVal integratedModuleID As Integer, ByVal integratedObjects As String, ByVal isPrivate As Boolean, ByVal authorizedRoles As String, ByVal authorizedEditRoles As String)
   Me.ForumID = forumID
   Me.GroupID = groupID
   Me.IsActive = isActive
   Me.ParentID = parentID
   Me.Name = name
   Me.Description = description
   Me.CreatedDate = createdDate
   Me.CreatedByUser = createdByUser
   Me.UpdatedByUser = updatedByUser
   Me.UpdatedDate = updatedDate
   Me.IsModerated = isModerated
   Me.DaysToView = daysToView
   Me.SortOrder = sortOrder
   Me.TotalPosts = totalPosts
   Me.TotalThreads = totalThreads
   Me.EnablePostStatistics = enablePostStatistics
   Me.EnableAutoDelete = enableAutoDelete
   Me.AutoDeleteThreshold = autoDeleteThreshold
   Me.MostRecentPostID = mostRecentPostID
   Me.MostRecentThreadID = mostRecentThreadID
   Me.MostRecentPostAuthorID = mostRecentPostAuthorID
   Me.MostRecentPostDate = mostRecentPostDate
   Me.PostsToModerate = postsToModerate
   Me.ForumType = forumType
   Me.IsIntegrated = isIntegrated
   Me.IntegratedModuleID = integratedModuleID
   Me.IntegratedObjects = integratedObjects
   Me.IsPrivate = isPrivate
   Me.AuthorizedRoles = authorizedRoles
   Me.AuthorizedEditRoles = authorizedEditRoles
  End Sub
#End Region

#Region "Public Properties"
  Public Property ForumID() As Integer
   Get
    Return _forumID
   End Get
   Set(ByVal Value As Integer)
    _forumID = Value
   End Set
  End Property

  Public Property GroupID() As Integer
   Get
    Return _groupID
   End Get
   Set(ByVal Value As Integer)
    _groupID = Value
   End Set
  End Property

  Public Property IsActive() As Boolean
   Get
    Return _isActive
   End Get
   Set(ByVal Value As Boolean)
    _isActive = Value
   End Set
  End Property

  Public Property ParentID() As Integer
   Get
    Return _parentID
   End Get
   Set(ByVal Value As Integer)
    _parentID = Value
   End Set
  End Property

  Public Property Name() As String
   Get
    Return _name
   End Get
   Set(ByVal Value As String)
    _name = Value
   End Set
  End Property

  Public Property Description() As String
   Get
    Return _description
   End Get
   Set(ByVal Value As String)
    _description = Value
   End Set
  End Property

  Public Property CreatedDate() As DateTime
   Get
    Return _createdDate
   End Get
   Set(ByVal Value As DateTime)
    _createdDate = Value
   End Set
  End Property

  Public Property CreatedByUser() As Integer
   Get
    Return _createdByUser
   End Get
   Set(ByVal Value As Integer)
    _createdByUser = Value
   End Set
  End Property

  Public Property UpdatedByUser() As Integer
   Get
    Return _updatedByUser
   End Get
   Set(ByVal Value As Integer)
    _updatedByUser = Value
   End Set
  End Property

  Public Property UpdatedDate() As DateTime
   Get
    Return _updatedDate
   End Get
   Set(ByVal Value As DateTime)
    _updatedDate = Value
   End Set
  End Property

  Public Property IsModerated() As Boolean
   Get
    Return _isModerated
   End Get
   Set(ByVal Value As Boolean)
    _isModerated = Value
   End Set
  End Property

  Public Property DaysToView() As Integer
   Get
    Return _daysToView
   End Get
   Set(ByVal Value As Integer)
    _daysToView = Value
   End Set
  End Property

  Public Property SortOrder() As Integer
   Get
    Return _sortOrder
   End Get
   Set(ByVal Value As Integer)
    _sortOrder = Value
   End Set
  End Property

  Public Property TotalPosts() As Integer
   Get
    Return _totalPosts
   End Get
   Set(ByVal Value As Integer)
    _totalPosts = Value
   End Set
  End Property

  Public Property TotalThreads() As Integer
   Get
    Return _totalThreads
   End Get
   Set(ByVal Value As Integer)
    _totalThreads = Value
   End Set
  End Property

  Public Property EnablePostStatistics() As Boolean
   Get
    Return _enablePostStatistics
   End Get
   Set(ByVal Value As Boolean)
    _enablePostStatistics = Value
   End Set
  End Property

  Public Property EnableAutoDelete() As Boolean
   Get
    Return _enableAutoDelete
   End Get
   Set(ByVal Value As Boolean)
    _enableAutoDelete = Value
   End Set
  End Property

  Public Property AutoDeleteThreshold() As Integer
   Get
    Return _autoDeleteThreshold
   End Get
   Set(ByVal Value As Integer)
    _autoDeleteThreshold = Value
   End Set
  End Property

  Public Property MostRecentPostID() As Integer
   Get
    Return _mostRecentPostID
   End Get
   Set(ByVal Value As Integer)
    _mostRecentPostID = Value
   End Set
  End Property

  Public Property MostRecentThreadID() As Integer
   Get
    Return _mostRecentThreadID
   End Get
   Set(ByVal Value As Integer)
    _mostRecentThreadID = Value
   End Set
  End Property

  Public Property MostRecentPostAuthorID() As Integer
   Get
    Return _mostRecentPostAuthorID
   End Get
   Set(ByVal Value As Integer)
    _mostRecentPostAuthorID = Value
   End Set
  End Property

  Public Property MostRecentPostDate() As DateTime
   Get
    Return _mostRecentPostDate
   End Get
   Set(ByVal Value As DateTime)
    _mostRecentPostDate = Value
   End Set
  End Property

  Public Property PostsToModerate() As Integer
   Get
    Return _postsToModerate
   End Get
   Set(ByVal Value As Integer)
    _postsToModerate = Value
   End Set
  End Property

  Public Property ForumType() As Integer
   Get
    Return _forumType
   End Get
   Set(ByVal Value As Integer)
    _forumType = Value
   End Set
  End Property

  Public Property IsIntegrated() As Boolean
   Get
    Return _isIntegrated
   End Get
   Set(ByVal Value As Boolean)
    _isIntegrated = Value
   End Set
  End Property

  Public Property IntegratedModuleID() As Integer
   Get
    Return _integratedModuleID
   End Get
   Set(ByVal Value As Integer)
    _integratedModuleID = Value
   End Set
  End Property

  Public Property IntegratedObjects() As String
   Get
    Return _integratedObjects
   End Get
   Set(ByVal Value As String)
    _integratedObjects = Value
   End Set
  End Property

  Public Property IsPrivate() As Boolean
   Get
    Return _isPrivate
   End Get
   Set(ByVal Value As Boolean)
    _isPrivate = Value
   End Set
  End Property

  Public Property AuthorizedRoles() As String
   Get
    Return _authorizedRoles
   End Get
   Set(ByVal Value As String)
    _authorizedRoles = Value
   End Set
  End Property

  Public Property AuthorizedEditRoles() As String
   Get
    Return _authorizedEditRoles
   End Get
   Set(ByVal Value As String)
    _authorizedEditRoles = Value
   End Set
  End Property
#End Region

 End Class

#End Region

#Region "Forums_ForumsController"

 Public Class Forum_ForumsController

#Region "Public Methods"

  Public Function List(ByVal GroupID As Integer) As ArrayList

   Return CBO.FillCollection(DataProvider.Instance().Upgrade_ListForum_Forums(GroupID), GetType(Forum_ForumsInfo))

  End Function

#End Region

 End Class

#End Region

#End Region

#Region "Forum_Thread Upgrade"

#Region "Forum_ThreadsInfo"
 Public Class Forum_ThreadsInfo

#Region "Private Members"
  Dim _threadID As Integer
  Dim _forumID As Integer
  Dim _views As Integer
  Dim _lastPostedPostID As Integer
  Dim _replies As Integer
  Dim _isPinned As Boolean
  Dim _pinnedDate As DateTime
  Dim _image As String
  Dim _objectType As Integer
  Dim _objectID As String
#End Region

#Region "Constructors"
  Public Sub New()
  End Sub

  Public Sub New(ByVal threadID As Integer, ByVal forumID As Integer, ByVal views As Integer, ByVal lastPostedPostID As Integer, ByVal replies As Integer, ByVal isPinned As Boolean, ByVal pinnedDate As DateTime, ByVal image As String, ByVal objectTypeCode As Integer, ByVal objectID As String)
   Me.ThreadID = threadID
   Me.ForumID = forumID
   Me.Views = views
   Me.LastPostedPostID = lastPostedPostID
   Me.Replies = replies
   Me.IsPinned = isPinned
   Me.PinnedDate = pinnedDate
   Me.Image = image
   Me.ObjectType = objectTypeCode
   Me.ObjectID = objectID
  End Sub
#End Region

#Region "Public Properties"
  Public Property ThreadID() As Integer
   Get
    Return _threadID
   End Get
   Set(ByVal Value As Integer)
    _threadID = Value
   End Set
  End Property

  Public Property ForumID() As Integer
   Get
    Return _forumID
   End Get
   Set(ByVal Value As Integer)
    _forumID = Value
   End Set
  End Property

  Public Property Views() As Integer
   Get
    Return _views
   End Get
   Set(ByVal Value As Integer)
    _views = Value
   End Set
  End Property

  Public Property LastPostedPostID() As Integer
   Get
    Return _lastPostedPostID
   End Get
   Set(ByVal Value As Integer)
    _lastPostedPostID = Value
   End Set
  End Property

  Public Property Replies() As Integer
   Get
    Return _replies
   End Get
   Set(ByVal Value As Integer)
    _replies = Value
   End Set
  End Property

  Public Property IsPinned() As Boolean
   Get
    Return _isPinned
   End Get
   Set(ByVal Value As Boolean)
    _isPinned = Value
   End Set
  End Property

  Public Property PinnedDate() As DateTime
   Get
    Return _pinnedDate
   End Get
   Set(ByVal Value As DateTime)
    _pinnedDate = Value
   End Set
  End Property

  Public Property Image() As String
   Get
    Return _image
   End Get
   Set(ByVal Value As String)
    _image = Value
   End Set
  End Property

  Public Property ObjectType() As Integer
   Get
    Return _objectType
   End Get
   Set(ByVal Value As Integer)
    _objectType = Value
   End Set
  End Property

  Public Property ObjectID() As String
   Get
    Return _objectID
   End Get
   Set(ByVal Value As String)
    _objectID = Value
   End Set
  End Property
#End Region

 End Class

#End Region

#Region "Forum_ThreadsController"
 Public Class ForumThreadsController

#Region "Public Methods"
  Public Function List(ByVal ForumID As Integer) As ArrayList

   Return CBO.FillCollection(DataProvider.Instance().Upgrade_ListForum_Threads(ForumID), GetType(Forum_ThreadsInfo))

  End Function

#End Region

 End Class

#End Region

#End Region

#Region "Forum_Posts Upgrade"

#Region "Forum_PostsInfo"
 Public Class Forum_PostsInfo

#Region "Private Members"
  Dim _postID As Integer
  Dim _parentPostID As Integer
  Dim _userID As Integer
  Dim _remoteAddr As String
  Dim _notify As Boolean
  Dim _subject As String
  Dim _body As String
  Dim _createdByUser As Integer
  Dim _createdDate As DateTime
  Dim _threadID As Integer
  Dim _postLevel As Integer
  Dim _treeSortOrder As Integer
  Dim _flatSortOrder As Integer
  Dim _updatedDate As DateTime
  Dim _updatedByUser As Integer
  Dim _isApproved As Boolean
  Dim _isLocked As Boolean
  Dim _isClosed As Boolean
  Dim _mediaURL As String
  Dim _mediaNAV As String
  Dim _attachments As String
#End Region

#Region "Constructors"
  Public Sub New()
  End Sub

  Public Sub New(ByVal postID As Integer, ByVal parentPostID As Integer, ByVal userID As Integer, ByVal remoteAddr As String, ByVal notify As Boolean, ByVal subject As String, ByVal body As String, ByVal createdByUser As Integer, ByVal createdDate As DateTime, ByVal threadID As Integer, ByVal postLevel As Integer, ByVal treeSortOrder As Integer, ByVal flatSortOrder As Integer, ByVal updatedDate As DateTime, ByVal updatedByUser As Integer, ByVal isApproved As Boolean, ByVal isLocked As Boolean, ByVal isClosed As Boolean, ByVal mediaURL As String, ByVal mediaNAV As String, ByVal attachments As String)
   Me.PostID = postID
   Me.ParentPostID = parentPostID
   Me.UserID = userID
   Me.RemoteAddr = remoteAddr
   Me.Notify = notify
   Me.Subject = subject
   Me.Body = body
   Me.CreatedByUser = createdByUser
   Me.CreatedDate = createdDate
   Me.ThreadID = threadID
   Me.PostLevel = postLevel
   Me.TreeSortOrder = treeSortOrder
   Me.FlatSortOrder = flatSortOrder
   Me.UpdatedDate = updatedDate
   Me.UpdatedByUser = updatedByUser
   Me.IsApproved = isApproved
   Me.IsLocked = isLocked
   Me.IsClosed = isClosed
   Me.MediaURL = mediaURL
   Me.MediaNAV = mediaNAV
   Me.Attachments = attachments
  End Sub
#End Region

#Region "Public Properties"
  Public Property PostID() As Integer
   Get
    Return _postID
   End Get
   Set(ByVal Value As Integer)
    _postID = Value
   End Set
  End Property

  Public Property ParentPostID() As Integer
   Get
    Return _parentPostID
   End Get
   Set(ByVal Value As Integer)
    _parentPostID = Value
   End Set
  End Property

  Public Property UserID() As Integer
   Get
    Return _userID
   End Get
   Set(ByVal Value As Integer)
    _userID = Value
   End Set
  End Property

  Public Property RemoteAddr() As String
   Get
    Return _remoteAddr
   End Get
   Set(ByVal Value As String)
    _remoteAddr = Value
   End Set
  End Property

  Public Property Notify() As Boolean
   Get
    Return _notify
   End Get
   Set(ByVal Value As Boolean)
    _notify = Value
   End Set
  End Property

  Public Property Subject() As String
   Get
    Return _subject
   End Get
   Set(ByVal Value As String)
    _subject = Value
   End Set
  End Property

  Public Property Body() As String
   Get
    Return _body
   End Get
   Set(ByVal Value As String)
    _body = Value
   End Set
  End Property

  Public Property CreatedByUser() As Integer
   Get
    Return _createdByUser
   End Get
   Set(ByVal Value As Integer)
    _createdByUser = Value
   End Set
  End Property

  Public Property CreatedDate() As DateTime
   Get
    Return _createdDate
   End Get
   Set(ByVal Value As DateTime)
    _createdDate = Value
   End Set
  End Property

  Public Property ThreadID() As Integer
   Get
    Return _threadID
   End Get
   Set(ByVal Value As Integer)
    _threadID = Value
   End Set
  End Property

  Public Property PostLevel() As Integer
   Get
    Return _postLevel
   End Get
   Set(ByVal Value As Integer)
    _postLevel = Value
   End Set
  End Property

  Public Property TreeSortOrder() As Integer
   Get
    Return _treeSortOrder
   End Get
   Set(ByVal Value As Integer)
    _treeSortOrder = Value
   End Set
  End Property

  Public Property FlatSortOrder() As Integer
   Get
    Return _flatSortOrder
   End Get
   Set(ByVal Value As Integer)
    _flatSortOrder = Value
   End Set
  End Property

  Public Property UpdatedDate() As DateTime
   Get
    Return _updatedDate
   End Get
   Set(ByVal Value As DateTime)
    _updatedDate = Value
   End Set
  End Property

  Public Property UpdatedByUser() As Integer
   Get
    Return _updatedByUser
   End Get
   Set(ByVal Value As Integer)
    _updatedByUser = Value
   End Set
  End Property

  Public Property IsApproved() As Boolean
   Get
    Return _isApproved
   End Get
   Set(ByVal Value As Boolean)
    _isApproved = Value
   End Set
  End Property

  Public Property IsLocked() As Boolean
   Get
    Return _isLocked
   End Get
   Set(ByVal Value As Boolean)
    _isLocked = Value
   End Set
  End Property

  Public Property IsClosed() As Boolean
   Get
    Return _isClosed
   End Get
   Set(ByVal Value As Boolean)
    _isClosed = Value
   End Set
  End Property

  Public Property MediaURL() As String
   Get
    Return _mediaURL
   End Get
   Set(ByVal Value As String)
    _mediaURL = Value
   End Set
  End Property

  Public Property MediaNAV() As String
   Get
    Return _mediaNAV
   End Get
   Set(ByVal Value As String)
    _mediaNAV = Value
   End Set
  End Property

  Public Property Attachments() As String
   Get
    Return _attachments
   End Get
   Set(ByVal Value As String)
    _attachments = Value
   End Set
  End Property
#End Region

 End Class

#End Region

#Region "class Forum_PostsController"

 Public Class Forum_PostsController

#Region "Public Methods"
  Public Function List(ByVal ThreadID As Integer) As ArrayList

   Return CBO.FillCollection(DataProvider.Instance().Upgrade_ListForum_Posts(ThreadID), GetType(Forum_PostsInfo))

  End Function

#End Region

 End Class

#End Region

#End Region

#Region "Forum_Ratings Upgrade"

#Region "Forum_RatingsInfo"
 Public Class Forum_ThreadRatingInfo

#Region "Private Members"
  Dim _userID As Integer
  Dim _threadID As Integer
  Dim _rate As Integer
  Dim _comment As String
#End Region

#Region "Constructors"
  Public Sub New()
  End Sub

  Public Sub New(ByVal userID As Integer, ByVal threadID As Integer, ByVal rate As Integer, ByVal comment As String)
   Me.UserID = userID
   Me.ThreadID = threadID
   Me.Rate = rate
   Me.Comment = comment
  End Sub
#End Region

#Region "Public Properties"
  Public Property UserID() As Integer
   Get
    Return _userID
   End Get
   Set(ByVal Value As Integer)
    _userID = Value
   End Set
  End Property

  Public Property ThreadID() As Integer
   Get
    Return _threadID
   End Get
   Set(ByVal Value As Integer)
    _threadID = Value
   End Set
  End Property

  Public Property Rate() As Integer
   Get
    Return _rate
   End Get
   Set(ByVal Value As Integer)
    _rate = Value
   End Set
  End Property

  Public Property Comment() As String
   Get
    Return _comment
   End Get
   Set(ByVal Value As String)
    _comment = Value
   End Set
  End Property
#End Region

 End Class

#End Region

#Region "Forum_RatingsController"
 Public Class Forum_ThreadRatingController

#Region "Public Methods"

  Public Function List(ByVal ThreadID As Integer) As ArrayList

   Return CBO.FillCollection(DataProvider.Instance().Upgrade_ListForum_ThreadRating(ThreadID), GetType(Forum_ThreadRatingInfo))

  End Function

#End Region

 End Class

#End Region

#End Region

End Namespace
