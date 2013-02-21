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
Imports DotNetNuke.Services.Tokens
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Users

Namespace Entities


 Public Class CommentInfo
  Implements IPropertyAccess

#Region "local property declarations"
  Private _commentID As Integer
  Private _EntryID As Integer
  Private _email As String
  Private _Title As String
  Private _comment As String
  Private _addedDate As DateTime
  Private _userID As Integer
  Private _userName As String
  Private _userFullName As String
  Private _Author As String
  Private _Approved As Boolean
  Private _Website As String
#End Region

#Region "Constructors"
  Public Sub New()
  End Sub

  Public Sub New(commentID As Integer, EntryID As Integer, email As String, comment As String, addedDate As DateTime, userID As Integer, userName As String, userFullName As String)
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
   Set(Value As Integer)
    _commentID = Value
   End Set
  End Property

  Public Property EntryID() As Integer
   Get
    Return _EntryID
   End Get
   Set(Value As Integer)
    _EntryID = Value
   End Set
  End Property

  Public Property Email() As String
   Get
    Return _email
   End Get
   Set(Value As String)
    _email = Value
   End Set
  End Property

  Public Property Title() As String
   Get
    Return _Title
   End Get
   Set(Value As String)
    _Title = Value
   End Set
  End Property

  Public Property Comment() As String
   Get
    Return _comment
   End Get
   Set(Value As String)
    _comment = Value
   End Set
  End Property

  Public Property AddedDate() As DateTime
   Get
    Return _addedDate
   End Get
   Set(Value As DateTime)
    _addedDate = Value
   End Set
  End Property

  Public Property UserID() As Integer
   Get
    Return _userID
   End Get
   Set(Value As Integer)
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
   Set(Value As String)
    _userName = Value
   End Set
  End Property

  Public Property UserFullName() As String
   Get
    Return _userFullName
   End Get
   Set(Value As String)
    _userFullName = Value
   End Set
  End Property

  Public Property Author() As String
   Get
    Return _Author
   End Get
   Set(Value As String)
    _Author = Value
   End Set
  End Property

  Public Property Approved() As Boolean
   Get
    Return _Approved
   End Get
   Set(Value As Boolean)
    _Approved = Value
   End Set
  End Property
  Public Property Website() As String
   Get
    Return _Website
   End Get
   Set(Value As String)
    _Website = Value
   End Set
  End Property
#End Region

  Public ReadOnly Property Cacheability() As CacheLevel Implements IPropertyAccess.Cacheability
   Get
    Return CacheLevel.fullyCacheable
   End Get
  End Property

  Public Function GetProperty(strPropertyName As String, strFormat As String, formatProvider As System.Globalization.CultureInfo, AccessingUser As UserInfo, AccessLevel As Scope, ByRef PropertyNotFound As Boolean) As String Implements IPropertyAccess.GetProperty

   Dim OutputFormat As String = String.Empty
   If strFormat = String.Empty Then
    OutputFormat = "D"
   Else
    OutputFormat = strFormat
   End If



   Select Case strPropertyName.ToLower
    Case "commentid"
     Return (Me.CommentID.ToString(OutputFormat, formatProvider))
    Case "email"
     Return PropertyAccess.FormatString(Me.UserName, strFormat)
    Case "entryid"
     Return (Me.EntryID.ToString(OutputFormat, formatProvider))
    Case "title"
     Return PropertyAccess.FormatString(Me.Title, strFormat)
    Case "comment"
     Return PropertyAccess.FormatString(HttpUtility.HtmlDecode(Me.Comment), strFormat)
    Case "addeddate"
     Return (Me.AddedDate.ToString(OutputFormat, formatProvider))
    Case "userid"
     Return (Me.UserID.ToString(OutputFormat, formatProvider))
    Case "username"
     Return PropertyAccess.FormatString(Me.UserName, strFormat)
    Case "userfullname"
     Return PropertyAccess.FormatString(Me.UserFullName, strFormat)
    Case "author"
     Return PropertyAccess.FormatString(Me.Author, strFormat)
    Case "approved"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.Approved, formatProvider)
    Case "website"
     Return PropertyAccess.FormatString(Me.Website, strFormat)
    Case Else
     PropertyNotFound = True
   End Select
   Return Null.NullString
  End Function

 End Class
End Namespace