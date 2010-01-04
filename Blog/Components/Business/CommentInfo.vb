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

 Public Class CommentInfo

#Region "local property declarations"
  Dim _commentID As Integer
  Dim _EntryID As Integer
  Dim _email As String
  Dim _Title As String
  Dim _comment As String
  Dim _addedDate As DateTime
  Dim _userID As Integer
  Dim _userName As String
  Dim _userFullName As String
  Dim _Author As String
  Dim _Approved As Boolean
  Dim _Website As String
#End Region

#Region "Constructors"
  Public Sub New()
  End Sub

  Public Sub New(ByVal commentID As Integer, ByVal EntryID As Integer, ByVal email As String, ByVal comment As String, ByVal addedDate As DateTime, ByVal userID As Integer, ByVal userName As String, ByVal userFullName As String)
   Me.CommentID = commentID
   Me.EntryID = EntryID
   Me.Email = email
   Me.Comment = comment
   Me.AddedDate = addedDate
  End Sub
#End Region

#Region "Public Properties"
  Public Property CommentID() As Integer
   Get
    Return _commentID
   End Get
   Set(ByVal Value As Integer)
    _commentID = Value
   End Set
  End Property

  Public Property EntryID() As Integer
   Get
    Return _EntryID
   End Get
   Set(ByVal Value As Integer)
    _EntryID = Value
   End Set
  End Property

  Public Property Email() As String
   Get
    Return _email
   End Get
   Set(ByVal Value As String)
    _email = Value
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

  Public Property Comment() As String
   Get
    Return _comment
   End Get
   Set(ByVal Value As String)
    _comment = Value
   End Set
  End Property

  Public Property AddedDate() As DateTime
   Get
    Return _addedDate
   End Get
   Set(ByVal Value As DateTime)
    _addedDate = Value
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
    Select Case _userID
     Case -1
      Return "Anonymous"
     Case -2
      Return "TrackBack"
     Case Else
      Return _userName
    End Select
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

  Public Property Author() As String
   Get
    Return _Author
   End Get
   Set(ByVal Value As String)
    _Author = Value
   End Set
  End Property

  Public Property Approved() As Boolean
   Get
    Return _Approved
   End Get
   Set(ByVal Value As Boolean)
    _Approved = Value
   End Set
  End Property
  Public Property Website() As String
   Get
    Return _Website
   End Get
   Set(ByVal Value As String)
    _Website = Value
   End Set
  End Property
#End Region

 End Class

End Namespace
