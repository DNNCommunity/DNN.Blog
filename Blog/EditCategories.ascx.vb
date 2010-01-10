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


Partial Class EditCategories
 Inherits BlogModuleBase

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
  If Not Page.IsPostBack Then

   FillTree()
   btnDelete.Attributes.Add("onclick", "return confirm('" & String.Format(Localization.GetString("msgEnsureDeleteCategory", Me.LocalResourceFile), btnDelete.CommandName) & "');")
   SetAddMode()

  End If
 End Sub

 Sub FillTree()

  tvCategories.Nodes.Clear()

  Dim CatList As List(Of Business.CategoryInfo) = CategoryController.ListCategoriesSorted(PortalId)
  For Each Cat As Business.CategoryInfo In CatList
   AddCategory(Cat, Cat.ParentID)
  Next

  tvCategories.ExpandAll()

  ddlCategory.Items.Clear()
  ddlCategory.DataSource = CatList
  ddlCategory.DataBind()
  ddlCategory.Items.Insert(0, New ListItem(Localization.GetString("strNone", Me.LocalResourceFile), "0"))

  tbCategory.Text = ""

 End Sub

 Sub AddCategory(ByVal cat As CategoryInfo, ByVal parentid As Integer)

  Dim node As New TreeNode

  node.Text = cat.Category + " (" + cat.Cnt.ToString + ")"
  node.Value = cat.CatID.ToString

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


 Protected Sub tvCategories_SelectedNodeChanged(ByVal sender As Object, ByVal e As EventArgs) Handles tvCategories.SelectedNodeChanged

  Dim cat As New CategoryInfo

  cat = CategoryController.GetCategory(CInt(tvCategories.SelectedNode.Value))
  tbCategory.Text = cat.Category
  ddlCategory.SelectedValue = cat.ParentID.ToString

  SetEditMode()

 End Sub


 Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click

  SetAddMode()
  If Not tvCategories.SelectedNode Is Nothing Then
   tvCategories.SelectedNode.Selected = False
  End If
  If Not ddlCategory.SelectedItem Is Nothing Then
   ddlCategory.SelectedItem.Selected = False
  End If

 End Sub

 Protected Sub btnAddEdit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddEdit.Click

  If tvCategories.SelectedNode Is Nothing Then
   CategoryController.AddCategory(tbCategory.Text, CInt(ddlCategory.SelectedValue), PortalId)
  Else
   CategoryController.UpdateCategory(CInt(tvCategories.SelectedValue), tbCategory.Text, CInt(ddlCategory.SelectedValue))
  End If

  FillTree()
  SetAddMode()

 End Sub

 Protected Sub SetEditMode()

  btnAddEdit.Text = Localization.GetString("btnEdit", Me.LocalResourceFile)
  btnDelete.Visible = True
  btnCancel.Visible = True

 End Sub


 Protected Sub SetAddMode()

  tbCategory.Text = ""
  btnAddEdit.Text = Localization.GetString("btnAdd", Me.LocalResourceFile)
  btnDelete.Visible = False
  btnCancel.Visible = False

 End Sub

 Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDelete.Click

  If tvCategories.SelectedNode.ChildNodes.Count > 0 Then
   Dim message As String = "Category has children and cannot be deleted.  Delete the child categories first."
   System.Web.HttpContext.Current.Response.Write("<SCRIPT LANGUAGE=""JavaScript"">" & vbCrLf)
   System.Web.HttpContext.Current.Response.Write("alert(""" & message & """)" & vbCrLf)
   System.Web.HttpContext.Current.Response.Write("</SCRIPT>")
  Else
   CategoryController.DeleteCategory(CInt(tvCategories.SelectedValue))
   FillTree()
   SetAddMode()
  End If

 End Sub

 Private Sub btnBack_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBack.Click

  Response.Redirect(NavigateURL(), False)

 End Sub
End Class

