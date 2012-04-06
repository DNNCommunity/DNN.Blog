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

Namespace Business

 Public Class ArchiveDays

#Region "local property declarations"
  Dim _BlogID As Integer
  Dim _EntryID As Integer
  Dim _AddedDate As DateTime
  Dim _Title As String
  Dim _userName As String
#End Region

#Region "Constructors"
  Public Sub New()
  End Sub

  Public Sub New(ByVal BlogID As Integer, ByVal EntryID As Integer, ByVal AddedDate As DateTime, ByVal Title As String, ByVal UserName As String)
   Me.BlogID = BlogID
   Me.EntryID = EntryID
   Me.AddedDate = AddedDate
   Me.Title = Title
   Me.UserName = UserName
  End Sub
#End Region

#Region "Public Properties"
  Public Property BlogID() As Integer
   Get
    Return _BlogID
   End Get
   Set(ByVal Value As Integer)
    _BlogID = Value
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

  Public Property AddedDate() As DateTime
   Get
    Return _AddedDate
   End Get
   Set(ByVal Value As DateTime)
    _AddedDate = Value
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

  Public Property UserName() As String
   Get
    Return _userName
   End Get
   Set(ByVal Value As String)
    _userName = Value
   End Set
  End Property

  Public ReadOnly Property AddedDay() As Integer
   Get
    Return _AddedDate.Day
   End Get
  End Property

  Public ReadOnly Property AddedMonth() As Integer
   Get
    Return _AddedDate.Month
   End Get
  End Property

  Public ReadOnly Property AddedYear() As Integer
   Get
    Return _AddedDate.Year
   End Get
  End Property
#End Region

 End Class

End Namespace