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
Imports DotNetNuke.Modules.Blog.Components.Business
Imports DotNetNuke.Modules.Blog.Components.Controllers
Imports DotNetNuke.Modules.Blog.Components.Common
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Framework
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Modules.Blog.Components.Entities

Partial Public Class EditBlog
    Inherits BlogModuleBase

#Region "Private Members"

    Private m_oParentBlog As BlogInfo
    Private objBlog As BlogInfo
    Private m_oBlogSettings As Hashtable

#End Region

#Region " Controls "
#End Region

#Region "Event Handlers"

    Protected Overloads Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        jQuery.RequestDnnPluginsRegistration()

        Dim cntBlog As New BlogController
        Dim objSecurity As ModuleSecurity = New ModuleSecurity(ModuleContext.ModuleId, ModuleContext.TabId)

        If SpecificBlogId > 0 Then
            objBlog = cntBlog.GetBlog(SpecificBlogId)

            Dim isOwner As Boolean = (objBlog.UserID = ModuleContext.PortalSettings.UserId)

            If (objSecurity.CanEditBlog(isOwner) = False) Then
                Response.Redirect(NavigateURL("Access Denied"))
            ElseIf objBlog.ParentBlogID > -1 Then
                m_oParentBlog = cntBlog.GetBlog(objBlog.ParentBlogID)
            End If
        End If

        If objBlog Is Nothing And Not (Request.Params("ParentBlogID") Is Nothing) And BlogSettings.AllowChildBlogs Then
            If Int32.Parse(Request.Params("ParentBlogID")) > 0 Then
                m_oParentBlog = cntBlog.GetBlog(Int32.Parse(Request.Params("ParentBlogID")))
            End If
        End If

        If Not m_oParentBlog Is Nothing Then
            Dim isOwner As Boolean = (m_oParentBlog.UserID = ModuleContext.PortalSettings.UserId)

            If objSecurity.CanEditBlog(isOwner) Then
                If objBlog Is Nothing Then
                    Me.ModuleConfiguration.ModuleTitle = Localization.GetString("msgCreateNewChildBlog", LocalResourceFile)
                Else
                    Me.ModuleConfiguration.ModuleTitle = Localization.GetString("msgEditChildBlog", LocalResourceFile)
                End If
            Else
                Response.Redirect(NavigateURL("Access Denied"))
            End If
        Else
            If objBlog Is Nothing Then
                Me.ModuleConfiguration.ModuleTitle = Localization.GetString("msgCreateBlog", LocalResourceFile)
                Me.cmdGenerateLinks.Enabled = False
            Else
                Me.ModuleConfiguration.ModuleTitle = Localization.GetString("msgEditBlog", LocalResourceFile)
                Me.cmdGenerateLinks.Enabled = True
            End If
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            If Not Page.IsPostBack Then
                fsChildBlogs.Visible = BlogSettings.AllowChildBlogs
                dnnSitePanelChildBlogs.Visible = BlogSettings.AllowChildBlogs
                pnlComments.Visible = (BlogSettings.CommentMode = Constants.CommentMode.Default)

                If Not BlogSettings.AllowWLW Then
                    lblMetaWeblogUrl.Visible = False
                    lblMetaWeblogNotAvailable.Visible = True
                Else
                    lblMetaWeblogUrl.Visible = True
                    lblMetaWeblogNotAvailable.Visible = False
                End If

                If Not objBlog Is Nothing Then
                    txtTitle.Text = objBlog.Title
                    txtDescription.Text = objBlog.Description
                    chkPublic.Checked = objBlog.Public
                    rdoUserName.Items.FindByValue(objBlog.ShowFullName.ToString()).Selected = True
                    ddlAuthorMode.SelectedValue = objBlog.AuthorMode.ToString

                    chkAllowComments.Checked = objBlog.AllowComments
                    chkSyndicate.Checked = objBlog.Syndicated
                    If objBlog.SyndicationEmail Is Nothing Then
                        txtSyndicationEmail.Text = ModuleContext.PortalSettings.UserInfo.Email
                    Else
                        txtSyndicationEmail.Text = objBlog.SyndicationEmail
                    End If
                    cmdDelete.Visible = True
                    cmdAddChildBlog.Enabled = BlogSettings.AllowChildBlogs
                    If Not rdoUserName.SelectedItem Is Nothing Then rdoUserName.SelectedItem.Selected = False

                    lstChildBlogs.Attributes.Add("onclick", "if (this.selectedIndex > -1) { " & cmdEditChildBlog.ClientID & ".disabled = false; " & cmdDeleteChildBlog.ClientID & ".disabled = false; }")

                    If m_oParentBlog Is Nothing Then
                        Dim cntBlog As New BlogController

                        lstChildBlogs.DataTextField = "Title"
                        lstChildBlogs.DataValueField = "BlogID"
                        lstChildBlogs.DataSource = cntBlog.GetParentsChildBlogs(Me.PortalId, objBlog.BlogID, True)
                        lstChildBlogs.DataBind()
                    Else
                        dnnSitePanelChildBlogs.Visible = False
                    End If

                    lblMetaWeblogUrl.Text = "http://" & Request.Url.Host & ControlPath & "blogpost.ashx?tabid=" & TabId
                ElseIf Not m_oParentBlog Is Nothing Then
                    If Not rdoUserName.SelectedItem Is Nothing Then rdoUserName.SelectedItem.Selected = False
                    rdoUserName.Items.FindByValue(m_oParentBlog.ShowFullName.ToString()).Selected = True

                    dnnSitePanelChildBlogs.Visible = False
                End If

                If Not Request.UrlReferrer Is Nothing Then
                    ViewState("URLReferrer") = Request.UrlReferrer.ToString
                End If

                If m_oParentBlog Is Nothing Then
                    hlCancel.NavigateUrl = ModuleContext.NavigateUrl(ModuleContext.TabId, "", False, "")
                Else
                    hlCancel.NavigateUrl = Request.UrlReferrer.ToString
                End If
            End If
        Catch exc As Exception
            ProcessModuleLoadException(Me, exc)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub cmdDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDelete.Click
        Try
            If Not objBlog Is Nothing Then
                Dim cntBlog As New BlogController
                cntBlog.DeleteBlog(objBlog.BlogID, ModuleContext.PortalId)
            End If
            Response.Redirect(NavigateURL(), True)
        Catch exc As Exception
            ProcessModuleLoadException(Me, exc)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
        If UpdateBlog() Then
            If Not m_oParentBlog Is Nothing Then
                Response.Redirect(EditUrl("BlogID", m_oParentBlog.BlogID.ToString(), "Edit_Blog"), True)
            Else
                Response.Redirect(Utility.AddTOQueryString(NavigateURL(), "BlogID", objBlog.BlogID.ToString()), True)
            End If
        End If
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btnAddChildBlog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAddChildBlog.Click
        If UpdateBlog() Then
            Response.Redirect(EditUrl("ParentBlogID", objBlog.BlogID.ToString(), "Edit_Blog"))
        End If
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btnEditChildBlog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdEditChildBlog.Click
        If Not lstChildBlogs.SelectedItem Is Nothing Then
            If UpdateBlog() Then
                Response.Redirect(EditUrl("BlogID", lstChildBlogs.SelectedValue, "Edit_Blog"))
            End If
        End If
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btnDeleteChildBlog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDeleteChildBlog.Click
        If Not lstChildBlogs.SelectedItem Is Nothing Then
            Dim cntBlog As New BlogController

            cntBlog.DeleteBlog(CType(lstChildBlogs.SelectedValue, Integer), ModuleContext.PortalId)
            lstChildBlogs.Items.Remove(lstChildBlogs.SelectedItem)
        End If
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub cmdGenerateLinks_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdGenerateLinks.Click
        Utility.CreateAllEntryLinks(PortalId, objBlog.BlogID)
    End Sub

#End Region

#Region "Private Methods"

    Private Function UpdateBlog() As Boolean
        Try
            If Page.IsValid = True Then
                If objBlog Is Nothing Then
                    objBlog = New BlogInfo
                    objBlog = CType(CBO.InitializeObject(objBlog, GetType(BlogInfo)), BlogInfo)
                    With objBlog
                        .UserID = Me.UserId
                        .PortalID = Me.PortalId
                        If Not m_oParentBlog Is Nothing Then
                            .ParentBlogID = m_oParentBlog.BlogID
                        End If
                    End With
                End If
                With objBlog
                    .Title = txtTitle.Text
                    .Description = txtDescription.Text
                    .Public = chkPublic.Checked
                    .ShowFullName = CType(rdoUserName.SelectedItem.Value, Boolean)
                    .AuthorMode = Convert.ToInt32(ddlAuthorMode.SelectedItem.Value)

                    .AllowComments = chkAllowComments.Checked
                    .Syndicated = chkSyndicate.Checked
                    .SyndicationEmail = txtSyndicationEmail.Text

                    Dim cntBlog As New BlogController

                    If Null.IsNull(objBlog.BlogID) Then
                        .BlogID = cntBlog.AddBlog(objBlog)
                        If .Syndicated Then objBlog = cntBlog.GetBlog(.BlogID)
                    Else
                        cntBlog.UpdateBlog(objBlog)
                    End If
                End With
                Return True
            End If
        Catch exc As Exception
            ProcessModuleLoadException(Me, exc)
        End Try
        Return False
    End Function

#End Region

End Class