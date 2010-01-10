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
Imports DotNetNuke.Modules.Blog.Data
Imports DotNetNuke.Common.Utilities

Namespace Business
    Public Class CategoryController

        Public Sub PopulateTree(ByRef Tree As UI.WebControls.DnnTree, ByVal PortalId As Integer, ByVal RootCategoryID As Integer)

            Dim portalCats As IDictionary(Of Integer, CategoryInfo) = ListCategories(PortalId)
            For Each c As CategoryInfo In portalCats.Values
                If c.ParentID = RootCategoryID Then
                    Dim n As New UI.WebControls.TreeNode
                    With n
                        .Key = c.CatID.ToString
                        .Text = c.Category
                    End With
                    Tree.TreeNodes.Add(n)
                    PopulateNode(n, portalCats)
                End If
            Next

        End Sub

        Private Sub PopulateNode(ByRef Node As UI.WebControls.TreeNode, ByVal Cats As IDictionary(Of Integer, CategoryInfo))

            For Each c As CategoryInfo In Cats.Values
                If c.ParentID.ToString = Node.Key Then
                    Dim n As New UI.WebControls.TreeNode
                    With n
                        .Key = c.CatID.ToString
                        .Text = c.Category
                    End With
                    Node.TreeNodes.Add(n)
                    PopulateNode(n, Cats)
                End If
            Next

        End Sub

        Public Function PopulateFullCat(ByVal CatList As IDictionary(Of Integer, CategoryInfo)) As IDictionary(Of Integer, CategoryInfo)

            For Each c As CategoryInfo In CatList.Values
                c.FullCat = GetParentCategory(CatList, c.ParentID) & "\" & c.Category
            Next
            Return CatList

        End Function

        Public Function GetParentCategory(ByVal CatList As IDictionary(Of Integer, CategoryInfo), ByVal ParentId As Integer) As String
            If ParentId = 0 Then Return ""
            Return GetParentCategory(CatList, CatList(ParentId).ParentID) & "\" & CatList(ParentId).Category
        End Function

        Public Function ListCategories(ByVal PortalID As Integer) As IDictionary(Of Integer, CategoryInfo)

            Return CBO.FillDictionary(Of CategoryInfo)(DataProvider.Instance().ListCategories(PortalID))

        End Function

        Public Function ListCategoriesFullPath(ByVal PortalID As Integer) As IDictionary(Of Integer, CategoryInfo)

            Return PopulateFullCat(ListCategories(PortalID))

        End Function

        Public Function ListCategoriesSorted(ByVal PortalID As Integer) As List(Of CategoryInfo)

            Dim res As New List(Of CategoryInfo)
            For Each c As CategoryInfo In ListCategoriesFullPath(PortalID).Values
                res.Add(c)
            Next
            res.Sort()
            Return res

        End Function

        Public Function ListCatsByEntry(ByVal EntryID As Integer) As List(Of CategoryInfo)

            Dim CatList As List(Of CategoryInfo)
            CatList = CBO.FillCollection(Of CategoryInfo)(DataProvider.Instance().ListEntryCategories(EntryID))
            Return CatList

        End Function

        Public Function StringListCatsByEntry(ByVal EntryID As Integer) As String()

            Dim Cats As List(Of CategoryInfo) = ListCatsByEntry(EntryID)
            Dim CatString(Cats.Count - 1) As String
            Dim i As Integer = 0
            For Each c As CategoryInfo In Cats
                CatString(i) = c.Category
                i += 1
            Next
            Return CatString

        End Function

        Public Sub UpdateCategoriesByEntry(ByVal EntryID As Integer, ByVal Categories As List(Of Integer))

            Dim EntryCats As List(Of CategoryInfo) = ListCatsByEntry(EntryID)
            Dim Found As Boolean

            If Not EntryCats Is Nothing Then
                For Each i As CategoryInfo In EntryCats
                    Found = False
                    For Each cat As Integer In Categories
                        If i.CatID = cat Then
                            Found = True
                            Exit For
                        End If
                    Next
                    If Not Found Then
                        DataProvider.Instance().DeleteEntryCategories(EntryID, i.CatID)
                    End If
                Next
            End If

            For Each i As Integer In Categories
                Found = False
                If Not EntryCats Is Nothing Then
                    For Each cat As CategoryInfo In EntryCats
                        If i = cat.CatID Then
                            Found = True
                            Exit For
                        End If
                    Next
                End If
                If Not Found Then
                    DataProvider.Instance().AddEntryCategories(EntryID, i)
                End If
            Next


        End Sub

        Public Sub UpdateCategoriesByEntry(ByVal EntryID As Integer, ByVal CategoryID As Integer)

            Dim EntryCats As New List(Of Integer)
            EntryCats.Add(CategoryID)
            UpdateCategoriesByEntry(EntryID, EntryCats)

        End Sub

        Public Sub UpdateCategoriesByEntry(ByVal EntryID As Integer, ByVal Category() As String)

            Dim EntryCats As New List(Of Integer)
            Dim Cats As IDictionary(Of Integer, CategoryInfo)
            Dim PortalSettings As DotNetNuke.Entities.Portals.PortalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings
            Cats = ListCategories(PortalSettings.PortalId)
            For a As Integer = 0 To Category.Length - 1
                For Each c As CategoryInfo In Cats.Values
                    If c.Category = Category(a) Then
                        EntryCats.Add(c.CatID)
                    End If
                Next
            Next
            UpdateCategoriesByEntry(EntryID, EntryCats)

        End Sub

        Public Function GetCategory(ByVal CatID As Integer) As CategoryInfo
            Return CType(CBO.FillObject(DataProvider.Instance().GetCategory(CatID), GetType(CategoryInfo)), CategoryInfo)
        End Function

        Public Sub AddCategory(ByVal Category As String, ByVal ParentID As Integer, ByVal PortalID As Integer)
            'DataProvider.Instance().AddCategory(Category, ParentID, PortalID)

            Dim Slug As String
            Slug = Utility.CreateFriendlySlug(Category)
            DataProvider.Instance().AddCategory(Category, Slug, ParentID, PortalID)

        End Sub

        Public Sub UpdateCategory(ByVal CatID As Integer, ByVal Category As String, ByVal ParentID As Integer)
            Dim slug As String
            slug = Utility.CreateFriendlySlug(Category)
            DataProvider.Instance().UpdateCategory(CatID, Category, slug, ParentID)
        End Sub

        Public Sub DeleteCategory(ByVal CatID As Integer)
            DataProvider.Instance().DeleteCategory(CatID)
        End Sub

    End Class
End Namespace
