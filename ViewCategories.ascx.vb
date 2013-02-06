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

Imports System.Linq
Imports DotNetNuke.Modules.Blog.Common
Imports DotNetNuke.Modules.Blog.Settings
Imports DotNetNuke.Modules.Blog.Entities
Imports Telerik.Web.UI
Imports TermController = DotNetNuke.Modules.Blog.Controllers.TermController

Partial Class ViewCategories
 Inherits BlogModuleBase

#Region "Private Members"

 Private _settings As CategoryViewSettings

 Private ReadOnly Property BlogCategories() As List(Of TermInfo)
  Get
   If Settings.VocabularyId > 0 Then
    Return TermController.GetTermsByContentType(ModuleContext.PortalId, Settings.VocabularyId)
   Else
    Return Nothing
   End If
  End Get
 End Property

#End Region

#Region "Event Handlers"

 Protected Overloads Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
  _settings = CategoryViewSettings.GetCategoryViewSettings(TabModuleId)
 End Sub

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
  If _settings.TreeSkin <> "" Then
   dtCategories.Skin = _settings.TreeSkin
  End If

  If Not Page.IsPostBack Then
   If Settings.VocabularyId > 1 Then
    Dim colCategories As List(Of TermInfo) = TermController.GetTermsByContentType(ModuleContext.PortalId, Settings.VocabularyId)

    dtCategories.DataSource = colCategories
    dtCategories.DataBind()

    If Request.Params("catid") IsNot Nothing Then
     Dim catId As Integer = Convert.ToInt32(Request.Params("catid"))
     Dim objNode As RadTreeNode = dtCategories.FindNodeByValue(catId.ToString())
     If objNode IsNot Nothing Then
      objNode.Selected = True
      objNode.ExpandParentNodes()
     End If
    End If
   End If
  End If
 End Sub

 Protected Sub RptTagsItemDataBound(sender As Object, e As RepeaterItemEventArgs)
  Dim tagControl As Tags = DirectCast(e.Item.FindControl("dbaSingleTag"), Tags)
  Dim term As TermInfo = DirectCast(e.Item.DataItem, TermInfo)
  Dim colTerms As New List(Of TermInfo)

  If term IsNot Nothing Then
   colTerms.Add(term)
  End If

  tagControl.ModContext = ModuleContext
  tagControl.DataSource = colTerms
  tagControl.CountMode = Constants.TagMode.ShowTotalUsage
  tagControl.DataBind()
 End Sub

 Protected Sub TvNodeItemDataBound(sender As Object, e As RadTreeNodeEventArgs)
  Dim categoryId As Integer = Convert.ToInt32(e.Node.Value)
  e.Node.NavigateUrl = ModuleContext.NavigateUrl(ModuleContext.TabId, "", False, "catid=" + categoryId.ToString())

  If BlogCategories IsNot Nothing Then
   Dim objTerm As TermInfo = (From c In BlogCategories Where c.TermId = categoryId Select c).SingleOrDefault()

   If objTerm IsNot Nothing Then
    e.Node.Text = e.Node.Text + " (" + objTerm.TotalTermUsage.ToString() + ")"
   Else
    e.Node.Text = e.Node.Text + " (0)"
   End If
  End If
 End Sub

#End Region

End Class