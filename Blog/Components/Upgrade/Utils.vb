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
Option Strict On
Option Explicit On

Imports DotNetNuke.Modules.Blog.Providers.Data
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Modules.Blog.Components.Entities

Namespace Components.Upgrade

#Region "5.0.0"

    Public Class MigrateCategoryInfo

        Public Sub New()
        End Sub

        Private _NewTermId As Int32 = -1
        Private _CatId As Int32 = -1
        Private _Category As String = "New Category"
        Private _Cnt As Int32 = 0
        Private _FullCat As String = "New Category"
        Private _ParentId As Int32 = 0
        Private _PortalId As Int32 = -1
        Private _Slug As String = "Default.aspx"

        Public Property NewTermId() As Int32
            Get
                Return _NewTermId
            End Get
            Set(ByVal Value As Int32)
                _NewTermId = Value
            End Set
        End Property

#Region " Public Properties "

        Public Property CatId() As Int32
            Get
                Return _CatId
            End Get
            Set(ByVal Value As Int32)
                _CatId = Value
            End Set
        End Property

        Public Property Category() As String
            Get
                Return _Category
            End Get
            Set(ByVal Value As String)
                _Category = Value
            End Set
        End Property

        Public Property Cnt() As Int32
            Get
                Return _Cnt
            End Get
            Set(ByVal Value As Int32)
                _Cnt = Value
            End Set
        End Property

        Public Property FullCat() As String
            Get
                Return _FullCat
            End Get
            Set(ByVal Value As String)
                _FullCat = Value
            End Set
        End Property

        Public Property ParentId() As Int32
            Get
                Return _ParentId
            End Get
            Set(ByVal Value As Int32)
                _ParentId = Value
            End Set
        End Property

        Public Property PortalId() As Int32
            Get
                Return _PortalId
            End Get
            Set(ByVal Value As Int32)
                _PortalId = Value
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

#End Region

    End Class

    Public Class MigrateTagInfo

        Public Sub New()
        End Sub

        Private _NewTermId As Int32 = -1
        Private _TagId As Int32 = -1
        Private _Active As Boolean = True
        Private _Cnt As Int32 = 0
        Private _PortalId As Int32 = -1
        Private _Slug As String = "Default.aspx"
        Private _Tag As String = "New tag"
        Private _Weight As Decimal = 0

        Public Property NewTermId() As Int32
            Get
                Return _NewTermId
            End Get
            Set(ByVal Value As Int32)
                _NewTermId = Value
            End Set
        End Property

        Public Property TagId() As Int32
            Get
                Return _TagId
            End Get
            Set(ByVal Value As Int32)
                _TagId = Value
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

        Public Property Cnt() As Int32
            Get
                Return _Cnt
            End Get
            Set(ByVal Value As Int32)
                _Cnt = Value
            End Set
        End Property

        Public Property PortalId() As Int32
            Get
                Return _PortalId
            End Get
            Set(ByVal Value As Int32)
                _PortalId = Value
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

        Public Property Tag() As String
            Get
                Return _Tag
            End Get
            Set(ByVal Value As String)
                _Tag = Value
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

    End Class

    Public Class ModuleUpgradeController

        Public Shared Function GetEntryCategoriesForUpgrade(ByVal EntryID As Integer) As List(Of MigrateCategoryInfo)
            Dim colCategories As List(Of MigrateCategoryInfo)
            colCategories = CBO.FillCollection(Of MigrateCategoryInfo)(DataProvider.Instance().GetCategoriesByEntry(EntryID))
            Return colCategories
        End Function

        Public Shared Function GetEntryTagsForUpgrade(ByVal EntryID As Integer) As List(Of MigrateTagInfo)
            Dim colTags As List(Of MigrateTagInfo)
            colTags = CBO.FillCollection(Of MigrateTagInfo)(DataProvider.Instance().GetTagsByEntry(EntryID))
            Return colTags
        End Function

        Public Shared Function GetAllCategoriesForUpgrade() As List(Of MigrateCategoryInfo)
            Return CBO.FillCollection(Of MigrateCategoryInfo)(DataProvider.Instance().GetAllCategoriesForUpgrade())
        End Function

        Public Shared Function GetAllTagsForUpgrade() As List(Of MigrateTagInfo)
            Return CBO.FillCollection(Of MigrateTagInfo)(DataProvider.Instance().GetAllTagsForUpgrade())
        End Function

        Public Shared Function GetAllBlogsForUpgrade() As List(Of BlogInfo)
            Return CBO.FillCollection(Of BlogInfo)(DataProvider.Instance().GetAllBlogsForUpgrade())
        End Function

    End Class

#End Region

End Namespace