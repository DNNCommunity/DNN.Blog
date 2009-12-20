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

  End If
 End Sub

 Sub FillTree()

  tvCategories.Nodes.Clear()

  Dim CatController As New CategoryController
  Dim CatList As List(Of Business.CategoryInfo) = CatController.ListCategoriesSorted(PortalId)
  For Each Cat As Business.CategoryInfo In CatList
   AddCategory(Cat, Cat.ParentID)
  Next

  tvCategories.ExpandAll()

  ddlCategory.Items.Clear()
  ddlCategory.DataSource = CatList
  ddlCategory.DataBind()
  ' Need to localize next line

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


  Dim CatController As New CategoryController
  Dim cat As New CategoryInfo

  cat = CatController.GetCategory(CInt(tvCategories.SelectedNode.Value))
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

  Dim CatController As New CategoryController

  If tvCategories.SelectedNode Is Nothing Then
   CatController.AddCategory(tbCategory.Text, CInt(ddlCategory.SelectedValue), PortalId)
  Else
   CatController.UpdateCategory(CInt(tvCategories.SelectedValue), tbCategory.Text, CInt(ddlCategory.SelectedValue))
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

 End Sub

 Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDelete.Click
  Dim CatController As New CategoryController
  If tvCategories.SelectedNode.ChildNodes.Count > 0 Then
   Dim message As String = "Category has children and cannot be deleted.  Delete the child categories first."
   System.Web.HttpContext.Current.Response.Write("<SCRIPT LANGUAGE=""JavaScript"">" & vbCrLf)
   System.Web.HttpContext.Current.Response.Write("alert(""" & message & """)" & vbCrLf)
   System.Web.HttpContext.Current.Response.Write("</SCRIPT>")
  Else
   CatController.DeleteCategory(CInt(tvCategories.SelectedValue))
   FillTree()
   SetAddMode()
  End If
 End Sub
End Class

