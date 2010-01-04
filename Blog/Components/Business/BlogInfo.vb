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

Namespace Business

 Public Class BlogInfo

#Region "local property declarations"
  Private _ParentBlogID As Integer
  Private _PortalID As Integer
  Private _blogID As Integer
  Private _userID As Integer
  Private _userName As String
  Private _userFullName As String
  Private _title As String
  Private _description As String
  Private _public As Boolean
  Private _allowComments As Boolean
  Private _allowAnonymous As Boolean
  Private _lastEntry As DateTime
  Private _created As DateTime
  Private _ChildBlogCount As Integer = 0
  Private _culture As String
  Private _DateFormat As String
  Private _ShowFullName As Boolean
  Private _TimeZone As Integer
  Private _syndicated As Boolean
  Private _SyndicateIndependant As Boolean
  Private _SyndicationURL As String
  Private _SyndicationEmail As String
  Private _EmailNotification As Boolean
  Private _AllowTrackbacks As Boolean
  Private _AutoTrackback As Boolean
  Private _MustApproveComments As Boolean
  Private _MustApproveAnonymous As Boolean
  Private _MustApproveTrackbacks As Boolean
  Private _useCaptcha As Boolean
  Private _EncryptionKey As String = "BWECIB34CBI3FBIP3BFIPUHT"
  Private _EnableTwitterIntegration As Boolean = False
  Private _TwitterUsername As String
  Private _TwitterPassword As String
  Private _TweetTemplate As String
  Private _BlogPostCount As Integer
#End Region

#Region "Constructors"
  Public Sub New()
  End Sub
#End Region

#Region "Public Properties"
  Public Property PortalID() As Integer
   Get
    Return _PortalID
   End Get
   Set(ByVal Value As Integer)
    _PortalID = Value
   End Set
  End Property

  Public Property BlogID() As Integer
   Get
    Return _blogID
   End Get
   Set(ByVal Value As Integer)
    _blogID = Value
   End Set
  End Property

  Public Property ParentBlogID() As Integer
   Get
    Return _ParentBlogID
   End Get
   Set(ByVal Value As Integer)
    _ParentBlogID = Value
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

  Public Property UserName() As String
   Get
    Return _userName
   End Get
   Set(ByVal Value As String)
    _userName = Value
   End Set
  End Property

  Public Property UserFullName() As String
   Get
    Return _userFullName
   End Get
   Set(ByVal Value As String)
    _userFullName = Value
   End Set
  End Property

  Public Property Title() As String
   Get
    Return _title
   End Get
   Set(ByVal Value As String)
    _title = Value
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

  Public Property [Public]() As Boolean
   Get
    Return _public
   End Get
   Set(ByVal Value As Boolean)
    _public = Value
   End Set
  End Property

  Public Property AllowComments() As Boolean
   Get
    Return _allowComments
   End Get
   Set(ByVal Value As Boolean)
    _allowComments = Value
   End Set
  End Property

  Public Property AllowAnonymous() As Boolean
   Get
    Return _allowAnonymous
   End Get
   Set(ByVal Value As Boolean)
    _allowAnonymous = Value
   End Set
  End Property

  Public Property LastEntry() As DateTime
   Get
    Return _lastEntry
   End Get
   Set(ByVal Value As DateTime)
    _lastEntry = Value
   End Set
  End Property

  Public Property Created() As DateTime
   Get
    Return _created
   End Get
   Set(ByVal Value As DateTime)
    _created = Value
   End Set
  End Property

  Public Property Culture() As String
   Get
    If _culture Is Nothing Then
     Return "en-US"
    Else
     If _culture.Length = 0 Then
      Return "en-US"
     Else
      Return _culture
     End If

    End If
   End Get
   Set(ByVal Value As String)
    _culture = Value
   End Set
  End Property

  Public Property DateFormat() As String
   Get
    If _DateFormat Is Nothing Then
     Return "g"
    Else
     If _DateFormat.Length = 0 Then
      Return "g"
     Else
      Return _DateFormat
     End If
    End If
   End Get
   Set(ByVal Value As String)
    _DateFormat = Value
   End Set
  End Property

  Public Property ShowFullName() As Boolean
   Get
    Return _ShowFullName
   End Get
   Set(ByVal Value As Boolean)
    _ShowFullName = Value
   End Set
  End Property

  Public Property TimeZone() As Integer
   Get
    Return _TimeZone
   End Get
   Set(ByVal Value As Integer)
    _TimeZone = Value
   End Set
  End Property

  Public Property ChildBlogCount() As Integer
   Get
    Return _ChildBlogCount
   End Get
   Set(ByVal Value As Integer)
    _ChildBlogCount = Value
   End Set
  End Property

  Public Property Syndicated() As Boolean
   Get
    Return _syndicated
   End Get
   Set(ByVal Value As Boolean)
    _syndicated = Value
   End Set
  End Property

  Public Property SyndicateIndependant() As Boolean
   Get
    Return _SyndicateIndependant
   End Get
   Set(ByVal Value As Boolean)
    _SyndicateIndependant = Value
   End Set
  End Property

  Public Property SyndicationURL() As String
   Get
    Return _SyndicationURL
   End Get
   Set(ByVal Value As String)
    _SyndicationURL = Value
   End Set
  End Property

  Public Property SyndicationEmail() As String
   Get
    Return _SyndicationEmail
   End Get
   Set(ByVal Value As String)
    _SyndicationEmail = Value
   End Set
  End Property

  Public Property EmailNotification() As Boolean
   Get
    Return _EmailNotification
   End Get
   Set(ByVal Value As Boolean)
    _EmailNotification = Value
   End Set
  End Property

  Public Property AllowTrackbacks() As Boolean
   Get
    Return _AllowTrackbacks
   End Get
   Set(ByVal Value As Boolean)
    _AllowTrackbacks = Value
   End Set
  End Property

  Public Property AutoTrackback() As Boolean
   Get
    Return _AutoTrackback
   End Get
   Set(ByVal Value As Boolean)
    _AutoTrackback = Value
   End Set
  End Property

  Public Property MustApproveComments() As Boolean
   Get
    Return _MustApproveComments
   End Get
   Set(ByVal Value As Boolean)
    _MustApproveComments = Value
   End Set
  End Property

  Public Property MustApproveAnonymous() As Boolean
   Get
    Return _MustApproveAnonymous
   End Get
   Set(ByVal Value As Boolean)
    _MustApproveAnonymous = Value
   End Set
  End Property

  Public Property MustApproveTrackbacks() As Boolean
   Get
    Return _MustApproveTrackbacks
   End Get
   Set(ByVal Value As Boolean)
    _MustApproveTrackbacks = Value
   End Set
  End Property

  Public Property UseCaptcha() As Boolean
   Get
    Return _useCaptcha
   End Get
   Set(ByVal Value As Boolean)
    _useCaptcha = Value
   End Set
  End Property

  Public Property EnableTwitterIntegration() As Boolean
   Get
    Return _EnableTwitterIntegration
   End Get
   Set(ByVal value As Boolean)
    _EnableTwitterIntegration = value
   End Set
  End Property

  Public Property TwitterUsername() As String
   Get
    Return _TwitterUsername
   End Get
   Set(ByVal value As String)
    _TwitterUsername = value
   End Set
  End Property

  Public Property TwitterPassword() As String
   Get
    Return _TwitterPassword
   End Get
   Set(ByVal value As String)
    _TwitterPassword = value
   End Set
  End Property

  Public Property TweetTemplate() As String
   Get
    Return _TweetTemplate
   End Get
   Set(ByVal value As String)
    _TweetTemplate = value
   End Set
  End Property

  Public Property EncryptionKey() As String
   Get
    Return _EncryptionKey
   End Get
   Set(ByVal value As String)
    _EncryptionKey = value
   End Set
  End Property

  Public Property BlogPostCount() As Integer
   Get
    Return _BlogPostCount
   End Get
   Set(ByVal value As Integer)
    _BlogPostCount = value
   End Set
  End Property

#End Region

 End Class

End Namespace
