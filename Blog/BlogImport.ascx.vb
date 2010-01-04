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
Imports DotNetNuke.Services.Exceptions.Exceptions



Partial Public Class BlogImport
 Inherits BlogModuleBase

#Region " Private Members "
 Private itemId As Integer
#End Region

#Region " Event Handlers "
 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  Try
   If Not Page.IsPostBack Then

    Dim list As ArrayList
    lstBlogs.Visible = True
    Dim fgc As Forum_GroupsController = New Forum_GroupsController
    list = fgc.List(PortalId)
    lstBlogs.DataSource = list
    lstBlogs.DataBind()
   End If

  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub
#End Region

#Region " Private Methods "
 Private Sub lstBlogs_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataListItemEventArgs) Handles lstBlogs.ItemDataBound
  If Not e.Item.DataItem Is Nothing Then
   Dim lblCategory As System.Web.UI.WebControls.Label = CType(e.Item.FindControl("lblCategory"), System.Web.UI.WebControls.Label)
   Dim lblCategoryID As System.Web.UI.WebControls.Label = CType(e.Item.FindControl("lblCategoryID"), System.Web.UI.WebControls.Label)
   Dim ddlBlogs As System.Web.UI.WebControls.DropDownList = CType(e.Item.FindControl("ddlBlogs"), System.Web.UI.WebControls.DropDownList)
   Dim m_BlogController As New BlogController
   ddlBlogs.DataSource = m_BlogController.ListBlogsRootByPortal(PortalId)
   ddlBlogs.DataValueField = "BlogID"
   ddlBlogs.DataTextField = "Title"
   ddlBlogs.DataBind()
   lblCategory.Text = CType(e.Item.DataItem, Forum_GroupsInfo).Name
   lblCategoryID.Text = CType(e.Item.DataItem, Forum_GroupsInfo).GroupID.ToString()
   Dim lstForum As System.Web.UI.WebControls.DataList = CType(e.Item.FindControl("lstForum"), System.Web.UI.WebControls.DataList)
   AddHandler lstForum.ItemDataBound, AddressOf lstForum_ItemDataBound
   lstForum.Visible = True
   Dim list As New ArrayList
   Dim fbc As Forum_ForumsController = New Forum_ForumsController
   list = fbc.List(CType(e.Item.DataItem, Forum_GroupsInfo).GroupID)
   lstForum.DataSource = list
   lstForum.DataBind()
  End If
 End Sub

 Private Sub lstForum_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataListItemEventArgs)
  Dim lblForum As System.Web.UI.WebControls.Label = CType(e.Item.FindControl("lblForum"), System.Web.UI.WebControls.Label)
  If Not e.Item.DataItem Is Nothing Then
   lblForum.Text = CType(e.Item.DataItem, Forum_ForumsInfo).Name
  End If
 End Sub

 Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
  Try
   Response.Redirect(NavigateURL(), True)
  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub


 Private Sub cmdImport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdImport.Click
  Try
   For Each di As DataListItem In lstBlogs.Items
    Dim chkImport As System.Web.UI.WebControls.CheckBox = CType(di.FindControl("chkImport"), System.Web.UI.WebControls.CheckBox)
    If chkImport.Checked Then       ' Import 
     Dim lblGroupID As System.Web.UI.WebControls.Label = CType(di.FindControl("lblCategoryID"), System.Web.UI.WebControls.Label)
     Dim GroupID As Integer = CType(lblGroupID.Text, Integer)
     Dim ddlBlogs As System.Web.UI.WebControls.DropDownList = CType(di.FindControl("ddlBlogs"), System.Web.UI.WebControls.DropDownList)
     Dim BlogID As Integer = CType(ddlBlogs.SelectedValue, Integer)
     Dim m_CustomeUpgrade As CustomUpgrade = New CustomUpgrade
     m_CustomeUpgrade.ImportForumBlog(GroupID, BlogID)
     ' DW - 06/22/2008 - Moved here to accomodate changes to CreateAllEntryLinks.
     '                   It now takes BlogID as a parameter
     ' Generate all Permalinks
     If lstBlogs.Items.Count > 0 Then
      Utility.CreateAllEntryLinks(PortalId, BlogID, TabId)
     End If
    End If
   Next
  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
  Response.Redirect(NavigateURL(), True)
 End Sub

#End Region

End Class


