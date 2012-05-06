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
Namespace Components.Business


    Public Class ArchiveMonths

#Region "local property declarations"
        Private _AddedMonth As Integer
        Private _AddedYear As Integer
        Private _PostCount As Integer
#End Region

#Region "constructors"
        Public Sub New()
        End Sub
#End Region

#Region "Public Properties"

        Public Sub New(ByVal AddedMonth As Integer, ByVal AddedYear As Integer, ByVal PostCount As Integer)
            Me.AddedMonth = AddedMonth
            Me.AddedYear = AddedYear
            Me.PostCount = PostCount
        End Sub

        Public Property AddedMonth() As Integer
            Get
                Return _AddedMonth
            End Get
            Set(ByVal Value As Integer)
                _AddedMonth = Value
            End Set
        End Property

        Public Property AddedYear() As Integer
            Get
                Return _AddedYear
            End Get
            Set(ByVal Value As Integer)
                _AddedYear = Value
            End Set
        End Property

        Public ReadOnly Property MonthName() As String
            Get
                Return Microsoft.VisualBasic.DateAndTime.MonthName(_AddedMonth)
            End Get
        End Property

        Public ReadOnly Property AddedDate() As Date
            Get
                Return New Date(_AddedYear, _AddedMonth, Date.DaysInMonth(_AddedYear, _AddedMonth))
            End Get
        End Property

        Public Property PostCount() As Integer
            Get
                Return _PostCount
            End Get
            Set(ByVal value As Integer)
                _PostCount = value
            End Set
        End Property

#End Region

    End Class
End Namespace