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
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Modules.Blog.Business
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization

Partial Class ViewCategories
    Inherits BlogModuleBase
    Implements Entities.Modules.IActionable


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then

            Dim CatController As New CategoryController
            Dim CatList As List(Of Business.CategoryInfo) = CatController.ListCategoriesSorted(PortalId)
            For Each Cat As CategoryInfo In CatList
                AddCategory(Cat, Cat.ParentID)
            Next

        End If
    End Sub

    Sub AddCategory(ByVal cat As CategoryInfo, ByVal parentid As Integer)

        Dim node As New TreeNode

        node.Text = cat.Category + " (" + cat.Cnt.ToString + ")"
        node.Value = cat.CatID.ToString

        Dim TabInfo As DotNetNuke.Entities.Tabs.TabInfo
        Dim TabController As New DotNetNuke.Entities.Tabs.TabController
        TabInfo = TabController.GetTab(Me.TabId, Me.PortalId, False)

        Dim path As String = "~/default.aspx?tabid=" & Me.TabId & "&CatID=" + cat.CatID.ToString
        Dim sURL As String = DotNetNuke.Common.Globals.FriendlyUrl(TabInfo, path, cat.Slug)

        node.NavigateUrl = sURL

        If parentid = 0 Then
            tvCategories.Nodes.Add(node)
        Else
            Dim n As New TreeNode
            For Each n In tvCategories.Nodes
                AddChildNode(node, n, parentid)
            Next
        End If

    End Sub

    Sub AddChildNode(ByVal newnode As TreeNode, ByVal n As TreeNode, ByVal parentid As Integer)

        If n.Value = parentid.ToString Then
            n.ChildNodes.Add(newnode)
        Else
            If n.ChildNodes.Count > 0 Then
                Dim cnode As TreeNode
                For Each cnode In n.ChildNodes
                    AddChildNode(newnode, cnode, parentid)
                Next
            End If
        End If
    End Sub


    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        Me.ModuleConfiguration.SupportedFeatures = 0
    End Sub

    Public ReadOnly Property ModuleActions() As Entities.Modules.Actions.ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
        Get
            Dim Actions As New Entities.Modules.Actions.ModuleActionCollection
            Actions.Add(GetNextActionID, Localization.GetString(Entities.Modules.Actions.ModuleActionType.EditContent, LocalResourceFile), _
            "", "", "", EditUrl(), False, DotNetNuke.Security.SecurityAccessLevel.Edit, True, False)
            Return Actions
        End Get
    End Property

End Class

