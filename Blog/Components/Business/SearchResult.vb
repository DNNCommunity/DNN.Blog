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

 Public Class SearchResult

#Region "local property declarations"
  Dim _EntryID As Integer
  Dim _blogID As Integer
  Dim _blogTitle As String
  Dim _entryTitle As String
  Dim _Summary As String
  Dim _addedDate As DateTime
  Dim _userID As Integer
  Dim _userName As String
  Dim _userFullName As String
  Dim _Published As Boolean = True
  Dim _PermaLink As String
  Dim _rank As Integer

#End Region

#Region "Constructors"
  Public Sub New()
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
    Return _blogID
   End Get
   Set(ByVal Value As Integer)
    _blogID = Value
   End Set
  End Property

  Public Property BlogTitle() As String
   Get
    Return _blogTitle
   End Get
   Set(ByVal Value As String)
    _blogTitle = Value
   End Set
  End Property

  Public Property EntryTitle() As String
   Get
    Return _entryTitle
   End Get
   Set(ByVal Value As String)
    _entryTitle = Value
   End Set
  End Property

  Public Property Summary() As String
   Get
    Return _Summary
   End Get
   Set(ByVal Value As String)
    _Summary = Value
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

  Public Property PermaLink() As String
   Get
    Return _PermaLink
   End Get
   Set(ByVal Value As String)
    _PermaLink = Value
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

  Public Property Rank() As Integer
   Get
    Return _rank
   End Get
   Set(ByVal Value As Integer)
    _rank = Value
   End Set
  End Property

#End Region

 End Class

End Namespace
