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

Namespace Business

 Public Class TagInfo


#Region "local property declarations"
  Private _TagID As Integer
  Private _Tag As String
  Private _Cnt As Integer
  Private _Slug As String
  Private _Active As Boolean
  Private _Weight As Decimal
#End Region

#Region "Constructors"
  Public Sub New()
  End Sub

  Public Sub New(ByVal TagID As Integer, ByVal Tag As String, ByVal Cnt As Integer, ByVal Slug As String, ByVal Active As Boolean, ByVal Weight As Decimal)

   Me.TagID = TagID
   Me.Tag = Tag
   Me.Cnt = Cnt
   Me.Slug = Slug
   Me.Active = Active
   Me.Weight = Weight

  End Sub
#End Region

#Region "Public Properties"


  Public Property TagID() As Integer
   Get
    Return _TagID
   End Get
   Set(ByVal Value As Integer)
    _TagID = Value
   End Set
  End Property

  Public Property Tag() As String
   Get
    Return _Tag
   End Get
   Set(ByVal Value As String)
    _Tag = Value
   End Set
  End Property


  Public Property Cnt() As Integer
   Get
    Return _Cnt
   End Get
   Set(ByVal Value As Integer)
    _Cnt = Value
   End Set
  End Property

  Public Property Slug() As String
   Get
    Return _Slug
   End Get
   Set(ByVal Value As String)
    _Slug = Value
   End Set
  End Property


  Public Property Active() As Boolean
   Get
    Return _Active
   End Get
   Set(ByVal Value As Boolean)
    _Active = Value
   End Set
  End Property


  Public Property Weight() As Decimal
   Get
    Return _Weight
   End Get
   Set(ByVal Value As Decimal)
    _Weight = Value
   End Set
  End Property

#End Region


 End Class

End Namespace