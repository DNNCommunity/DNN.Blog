'
' DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2005
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

 Public Class EntryInfo

#Region "local property declarations"
  Private _UserID As Integer
  Private _Username As String
  Private _UserFullName As String
  Private _EntryID As Integer
  Private _BlogID As Integer
  Private _Title As String
  Private _Description As String
  Private _Entry As String
  Private _AddedDate As DateTime
  Private _Published As Boolean
  Private _AllowComments As Boolean
  Private _DisplayCopyright As Boolean
  Private _Copyright As String
  Private _PermaLink As String
  Private _CommentCount As Integer
  Private _SyndicationEmail As String
#End Region

#Region "Constructors"
  Public Sub New()
  End Sub

  Public Sub New(ByVal UserID As Integer, ByVal Username As String, ByVal UserFullName As String, ByVal EntryID As Integer, ByVal BlogID As Integer, ByVal Title As String, ByVal Description As String, ByVal Entry As String, ByVal AddedDate As DateTime, ByVal Published As Boolean, ByVal AllowComments As Boolean, ByVal AllowAnonymous As Boolean, ByVal BlogSyndicated As Boolean, ByVal BlogPublic As Boolean, ByVal CategoryID As Integer, ByVal CategoryTitle As String, ByVal CategorySyndicated As Boolean, ByVal CategoryPublic As Boolean, ByVal CommentCount As Integer)

   Me.UserID = UserID
   Me.UserName = Username
   Me.UserFullName = UserFullName
   Me.EntryID = EntryID
   Me.BlogID = BlogID
   Me.Title = Title
   Me.Description = Description
   Me.Entry = Entry
   Me.AddedDate = AddedDate
   Me.Published = Published
   Me.AllowComments = AllowComments
   Me.CommentCount = CommentCount
   Me.SyndicationEmail = SyndicationEmail
  End Sub
#End Region

#Region "Public Properties"


  Public Property EntryID() As Integer
   Get
    Return _EntryID
   End Get
   Set(ByVal Value As Integer)
    _EntryID = Value
   End Set
  End Property

  Public Property BlogID() As Integer
   Get
    Return _BlogID
   End Get
   Set(ByVal Value As Integer)
    _BlogID = Value
   End Set
  End Property

  Public Property Title() As String
   Get
    Return _Title
   End Get
   Set(ByVal Value As String)
    _Title = Value
   End Set
  End Property

  Public Property Description() As String
   Get
    Return _Description
   End Get
   Set(ByVal Value As String)
    _Description = Value
   End Set
  End Property

  Public Property Entry() As String
   Get
    Return _Entry
   End Get
   Set(ByVal Value As String)
    _Entry = Value
   End Set
  End Property

  Public Property AddedDate() As DateTime
   Get
    Return _AddedDate
   End Get
   Set(ByVal Value As DateTime)
    _AddedDate = Value
   End Set
  End Property

  Public Property UserID() As Integer
   Get
    Return _UserID
   End Get
   Set(ByVal Value As Integer)
    _UserID = Value
   End Set
  End Property

  Public Property UserName() As String
   Get
    Return _Username
   End Get

   Set(ByVal Value As String)
    _Username = Value
   End Set
  End Property

  Public Property UserFullName() As String
   Get
    Return _UserFullName
   End Get

   Set(ByVal Value As String)
    _UserFullName = Value
   End Set
  End Property

  Public Property Published() As Boolean
   Get
    Return _Published
   End Get
   Set(ByVal Value As Boolean)
    _Published = Value
   End Set
  End Property


  Public Property CommentCount() As Integer
   Get
    Return _CommentCount
   End Get
   Set(ByVal Value As Integer)
    _CommentCount = Value
   End Set
  End Property

  Public Property AllowComments() As Boolean
   Get
    Return _AllowComments
   End Get
   Set(ByVal Value As Boolean)
    _AllowComments = Value
   End Set
  End Property

  Public Property DisplayCopyright() As Boolean
   Get
    Return _DisplayCopyright
   End Get
   Set(ByVal Value As Boolean)
    _DisplayCopyright = Value
   End Set
  End Property

  Public Property Copyright() As String
   Get
    Return _Copyright
   End Get
   Set(ByVal Value As String)
    _Copyright = Value
   End Set
  End Property

  Public Property PermaLink() As String
   Get
    Return _PermaLink
   End Get
   Set(ByVal Value As String)
    _PermaLink = Value
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

#End Region

 End Class

End Namespace
