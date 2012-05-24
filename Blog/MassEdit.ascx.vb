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
Imports DotNetNuke.Modules.Blog.Components.Controllers
Imports DotNetNuke.Modules.Blog.Components.Common
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Modules.Blog.Components.Entities

Partial Class MassEdit
    Inherits BlogModuleBase

#Region " Private Members "

    Private m_oBlogController As New BlogController
    Private m_oBlog As BlogInfo
    Private m_dBlogDate As Date = Date.UtcNow
    Private m_dBlogDateType As String
    Private m_PersonalBlogID As Integer

#End Region

#Region "Event Handlers"

    Protected Overloads Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        m_oBlog = m_oBlogController.GetBlogFromContext()
        m_PersonalBlogID = BlogSettings.PageBlogs

        If m_PersonalBlogID <> -1 And m_oBlog Is Nothing Then
            Dim objBlog As New BlogController
            m_oBlog = objBlog.GetBlog(m_PersonalBlogID)
            'ModuleConfiguration.ModuleTitle = m_oBlog.Title
        End If
        MyActions.Add(GetNextActionID, Localization.GetString("msgModuleOptions", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "", EditUrl("", "", "Module_Options"), False, DotNetNuke.Security.SecurityAccessLevel.Admin, True, False)
    End Sub

    Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            Dim objEntries As New EntryController
            Dim list As List(Of EntryInfo)
            Dim currentpage As Integer

            If Not Page.IsPostBack Then
                If m_oBlog Is Nothing Then
     list = objEntries.GetEntriesByPortal(Me.PortalId, m_dBlogDate, m_dBlogDateType, 100000, 1, DotNetNuke.Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()), DotNetNuke.Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()))

                Else
                    Dim objSecurity As ModuleSecurity = New ModuleSecurity(ModuleContext.ModuleId, ModuleContext.TabId)

                    Dim isOwner As Boolean
                    isOwner = m_oBlog.UserID = ModuleContext.PortalSettings.UserId

                    list = objEntries.GetEntriesByBlog(m_oBlog.BlogID, m_dBlogDate, 100000, 1, objSecurity.CanAddEntry(isOwner, m_oBlog.AuthorMode), objSecurity.CanAddEntry(isOwner, m_oBlog.AuthorMode))
                End If

                Dim PageSize As Integer = 20 'Display 20 items per page

                'Get the currentpage index from the url parameter  
                If Request.QueryString("currentpage") IsNot Nothing Then
                    currentpage = CInt(Request.QueryString("currentpage"))
                Else
                    currentpage = 1
                End If

                Dim objPagedDataSource As New PagedDataSource
                objPagedDataSource.DataSource = list
                objPagedDataSource.PageSize = PageSize
                objPagedDataSource.CurrentPageIndex = currentpage - 1
                objPagedDataSource.AllowPaging = True

                With Pagecontrol
                    .TotalRecords = list.Count
                    .PageSize = PageSize
                    .CurrentPage = currentpage
                    .TabID = TabId
                    .QuerystringParams = "ctl/Mass_Edit/mid/" + Me.ModuleId.ToString + "/"
                End With

                rptEdit.DataSource = objPagedDataSource
                rptEdit.DataBind()

            End If

        Catch exc As Exception
            ProcessModuleLoadException(Me, exc)
        End Try
    End Sub

    Protected Sub rptEdit_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptEdit.ItemCommand
        If (e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem) Then

            If e.CommandName = "Edit" Then
                SetEdit(e)
            End If

            If e.CommandName = "Save" Then
                SaveItem(e)
            End If

            If e.CommandName = "Cancel" Then
                RptEditDisable()
            End If
        End If

    End Sub

    Protected Sub btnBack_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBack.Click
        Response.Redirect(NavigateURL(), False)
    End Sub

    Protected Sub rptEdit_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptEdit.ItemDataBound
        If (e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem) Then

            'Dim ti As String
            Dim eid As Integer = CType(CType(e.Item.DataItem, EntryInfo).EntryID, Integer)
            'TODO: CP
            'ti = TagController.GetTagsByEntry(eid)
            'Dim litTags As Literal = CType(e.Item.FindControl("litTags"), Literal)
            'Dim tbTags As TextBox = CType(e.Item.FindControl("tbTags"), TextBox)
            'litTags.Text = ti
            'tbTags.Text = ti
            'tbTags.Visible = False

            'Dim ci As List(Of CategoryInfo) = CategoryController.ListCatsByEntry(eid)
            'Dim cl As List(Of CategoryInfo) = CategoryController.ListCategoriesSorted(PortalId)

            'Dim dlCat As DropDownList = CType(e.Item.FindControl("ddlCat"), DropDownList)

            'dlCat.DataSource = cl

            'dlCat.DataBind()
            'dlCat.Items.Insert(0, New ListItem(" - Uncategorized - ", "-1"))

            'Dim litCat As Literal = CType(e.Item.FindControl("litCat"), Literal)
            'If ci.Count > 0 Then
            '    litCat.Text = ci(0).Category
            '    If Not dlCat.Items.FindByValue(ci(0).CatId.ToString) Is Nothing Then
            '        dlCat.Items.FindByValue(ci(0).CatId.ToString).Selected = True
            '    End If
            'End If
            'dlCat.Visible = False

            Dim litTitle As Literal = CType(e.Item.FindControl("litTitle"), Literal)
            If litTitle.Text.Length > 30 Then litTitle.Text = Left(litTitle.Text, 30) + "..."
            Dim tbTitle As TextBox = CType(e.Item.FindControl("tbTitle"), TextBox)
            tbTitle.Visible = False

            Dim cbPub As CheckBox = CType(e.Item.FindControl("cbPublished"), CheckBox)
            cbPub.Enabled = False
            Dim cbComments As CheckBox = CType(e.Item.FindControl("cbComments"), CheckBox)
            cbComments.Enabled = False

        End If

    End Sub

    Protected Sub SaveItem(ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs)
        Dim item As RepeaterItem = e.Item
        Dim tbTitle As TextBox = CType(item.FindControl("tbTitle"), TextBox)
        Dim tbTags As TextBox = CType(item.FindControl("tbTags"), TextBox)
        Dim dlCat As DropDownList = CType(item.FindControl("ddlCat"), DropDownList)
        Dim cbPub As CheckBox = CType(item.FindControl("cbPublished"), CheckBox)
        Dim cbComments As CheckBox = CType(item.FindControl("cbComments"), CheckBox)

        'TagController.UpdateTagsByEntry(CInt(e.CommandArgument), tbTags.Text)

        Dim litTags As Literal = CType(item.FindControl("litTags"), Literal)
        litTags.Text = tbTags.Text

        'CategoryController.UpdateCategoriesByEntry(CInt(e.CommandArgument), CInt(dlCat.SelectedValue))

        Dim litCat As Literal = CType(item.FindControl("litCat"), Literal)
        litCat.Text = dlCat.SelectedItem.Text

        Dim ec As New EntryController
        Dim ei As EntryInfo
        ei = ec.GetEntry(CInt(e.CommandArgument), PortalId)
        ei.Title = tbTitle.Text
        ei.AllowComments = cbComments.Checked
        ei.Published = cbPub.Checked
        ec.UpdateEntry(ei, ModuleContext.TabId, ModuleContext.PortalId)

        Dim litTitle As Literal = CType(item.FindControl("litTitle"), Literal)
        If tbTitle.Text.Length > 30 Then litTitle.Text = Left(tbTitle.Text, 30) + "..." Else litTitle.Text = tbTitle.Text

        RptEditDisable()

    End Sub

    Protected Sub SetEdit(ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs)
        Dim item As RepeaterItem = e.Item

        RptEditDisable()

        Dim litTags As Literal = CType(item.FindControl("litTags"), Literal)
        Dim tbTags As TextBox = CType(item.FindControl("tbTags"), TextBox)
        Dim litCat As Literal = CType(item.FindControl("litCat"), Literal)
        Dim dlCat As DropDownList = CType(item.FindControl("ddlCat"), DropDownList)
        Dim btnEditCmd As LinkButton = CType(item.FindControl("btnEditCmd"), LinkButton)
        Dim btnSaveCmd As LinkButton = CType(item.FindControl("btnSaveCmd"), LinkButton)
        Dim btnCancelCmd As LinkButton = CType(item.FindControl("btnCancelCmd"), LinkButton)
        Dim litTitle As Literal = CType(item.FindControl("litTitle"), Literal)
        Dim tbTitle As TextBox = CType(item.FindControl("tbTitle"), TextBox)
        Dim cbPub As CheckBox = CType(item.FindControl("cbPublished"), CheckBox)
        Dim cbComments As CheckBox = CType(item.FindControl("cbComments"), CheckBox)

        litTitle.Visible = False
        tbTitle.Visible = True
        litTags.Visible = False
        tbTags.Visible = True
        litCat.Visible = False
        dlCat.Visible = True
        btnEditCmd.Visible = False
        btnSaveCmd.Visible = True
        btnCancelCmd.Visible = True
        cbPub.Enabled = True
        cbComments.Enabled = True

    End Sub

#End Region

#Region " Private Methods "

    Private Sub RptEditDisable()
        Dim litTags As Literal
        Dim tbTags As TextBox
        Dim litCat As Literal
        Dim dlCat As DropDownList
        Dim btnEditCmd As LinkButton
        Dim btnSaveCmd As LinkButton
        Dim btnCancelCmd As LinkButton
        Dim litTitle As Literal
        Dim tbTitle As TextBox
        Dim cbPub As CheckBox
        Dim cbComments As CheckBox

        For Each item As RepeaterItem In rptEdit.Items
            litTitle = CType(item.FindControl("litTitle"), Literal)
            tbTitle = CType(item.FindControl("tbTitle"), TextBox)
            litTags = CType(item.FindControl("litTags"), Literal)
            tbTags = CType(item.FindControl("tbTags"), TextBox)
            litCat = CType(item.FindControl("litCat"), Literal)
            dlCat = CType(item.FindControl("ddlCat"), DropDownList)
            btnEditCmd = CType(item.FindControl("btnEditCmd"), LinkButton)
            btnSaveCmd = CType(item.FindControl("btnSaveCmd"), LinkButton)
            btnCancelCmd = CType(item.FindControl("btnCancelCmd"), LinkButton)
            cbPub = CType(item.FindControl("cbPublished"), CheckBox)
            cbComments = CType(item.FindControl("cbComments"), CheckBox)

            litTitle.Visible = True
            tbTitle.Visible = False
            litTags.Visible = True
            tbTags.Visible = False
            litCat.Visible = True
            dlCat.Visible = False
            btnEditCmd.Visible = True
            btnCancelCmd.Visible = False
            btnSaveCmd.Visible = False
            cbPub.Enabled = False
            cbComments.Enabled = False
        Next
    End Sub

#End Region

End Class