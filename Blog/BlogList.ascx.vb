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
Imports DotNetNuke.Services.Exceptions.Exceptions
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Modules.Blog.Components.Entities

Partial Class BlogList
    Inherits BlogModuleBase

#Region "Private Members"

    Private m_iSelectedBlogID As Integer = -1
    Private m_iParentBlogID As Integer = -1
    Private m_oBlog As BlogInfo
    Private m_PersonalBlogID As Integer
    Private m_oBlogController As New BlogController
    Private lstBlogChildren As System.Web.UI.WebControls.DataList
    Private lnkBlogIcon As System.Web.UI.WebControls.HyperLink
    Private lnkBlog As System.Web.UI.WebControls.HyperLink
    Private m_BlogTitleStringTemplate As String
    Private lblFooter As System.Web.UI.WebControls.Label

#End Region

#Region "Event Handlers"

    Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            Dim list As List(Of BlogInfo)
            'DR-04/17/2009-BLG-9754
            m_BlogTitleStringTemplate = Localization.GetString("BlogTitleStringTemplate", LocalResourceFile)

            If Not Request.Params("BlogID") Is Nothing Then
                m_iSelectedBlogID = Int32.Parse(Request.Params("BlogID"))
            End If
            If Not Request.Params("ParentBlogID") Is Nothing Then
                m_iParentBlogID = Int32.Parse(Request.Params("ParentBlogID"))
            End If

            m_PersonalBlogID = BlogSettings.PageBlogs

            If Not Page.IsPostBack Then
                ' new settings to determine view mode (display)

                list = m_oBlogController.GetParentsChildBlogs(PortalId, m_PersonalBlogID, DotNetNuke.Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()))
                lstBlogs.DataSource = list
                lstBlogs.DataBind()

                ' if no Entries are shown, show the Footer
                lstBlogs.ShowFooter = (lstBlogs.Items.Count = 0)
                lstBlogs.ShowHeader = Not lstBlogs.ShowFooter

                If lstBlogs.ShowFooter Then
                    lblFooter = CType(lstBlogs.Controls(lstBlogs.Controls.Count - 1).FindControl("lblFooter"), System.Web.UI.WebControls.Label)
                    If m_PersonalBlogID = -1 Then       ' General Blog Page
                        lblFooter.Text = Localization.GetString("msgNoBlogsInPortal", LocalResourceFile)
                    Else
                        lblFooter.Text = Localization.GetString("msgNoCategriesInBlog", LocalResourceFile)
                    End If
                End If
            End If
        Catch exc As Exception
            ProcessModuleLoadException(Me, exc)
        End Try
    End Sub

    Protected Sub lstBlogs_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataListItemEventArgs) Handles lstBlogs.ItemDataBound
        Dim lnkBlogLink As HyperLink = CType(e.Item.FindControl("lnkBlogLink"), HyperLink)
        Dim lnkBlogLinkIcon As Image = CType(e.Item.FindControl("lnkBlogLinkIcon"), Image)
        Dim lnkBlog As HyperLink = CType(e.Item.FindControl("lnkBlog"), HyperLink)
        Dim lnkBlogRSS As HyperLink = CType(e.Item.FindControl("lnkBlogRSS"), HyperLink)

        If Not e.Item.DataItem Is Nothing Then
            lnkBlogLink.NavigateUrl = NavigateURL(Me.TabId, "", "&BlogID=" & CType(e.Item.DataItem, BlogInfo).BlogID.ToString())
            lnkBlog.NavigateUrl = NavigateURL(Me.TabId, "", "&BlogID=" & CType(e.Item.DataItem, BlogInfo).BlogID.ToString())
            'DR-04/17/2009-BLG-9754
            lnkBlog.Text = String.Format(m_BlogTitleStringTemplate, CType(e.Item.DataItem, BlogInfo).Title, CType(e.Item.DataItem, BlogInfo).BlogPostCount)

            If m_PersonalBlogID = -1 Then
                If CType(e.Item.DataItem, BlogInfo).Syndicated Then
                    lnkBlogRSS.NavigateUrl = NavigateURL(Me.TabId, "", "rssid=" & CType(e.Item.DataItem, BlogInfo).BlogID.ToString())
                    lnkBlogRSS.Visible = True
                End If
            Else
                If CType(e.Item.DataItem, BlogInfo).Syndicated Then
                    lnkBlogRSS.NavigateUrl = NavigateURL(Me.TabId, "", "rssid=" & CType(e.Item.DataItem, BlogInfo).BlogID.ToString())
                    lnkBlogRSS.Visible = True
                End If
            End If
            If CType(e.Item.DataItem, BlogInfo).BlogID = m_iSelectedBlogID Or CType(e.Item.DataItem, BlogInfo).BlogID = m_iParentBlogID Then
                Dim trBlogChildren As System.Web.UI.WebControls.TableRow = CType(e.Item.FindControl("trBlogChildren"), System.Web.UI.WebControls.TableRow)
                Dim tdBlogChildren As System.Web.UI.WebControls.TableCell = CType(e.Item.FindControl("tdBlogChildren"), System.Web.UI.WebControls.TableCell)
                lstBlogChildren = CType(e.Item.FindControl("lstBlogChildren"), System.Web.UI.WebControls.DataList)
                AddHandler lstBlogChildren.ItemDataBound, AddressOf lstBlogChildren_ItemDataBound
                lstBlogChildren.DataSource = m_oBlogController.GetParentsChildBlogs(Me.PortalId, CType(e.Item.DataItem, BlogInfo).BlogID, (CType(e.Item.DataItem, BlogInfo).UserID = Me.UserId Or DotNetNuke.Security.PortalSecurity.IsInRole(Me.PortalSettings.AdministratorRoleId.ToString())))
                lstBlogChildren.DataBind()
                If lstBlogChildren.Items.Count > 0 Then
                    trBlogChildren.Visible = True
                    tdBlogChildren.Controls.Add(lstBlogChildren)
                    lnkBlogLinkIcon.ImageUrl = "~/desktopmodules/Blog/Images/folder_opened.gif"
                    lstBlogChildren.Visible = True
                End If
            Else
                If CType(e.Item.DataItem, BlogInfo).ChildBlogCount > 0 Then
                    lnkBlogLinkIcon.ImageUrl = "~/desktopmodules/Blog/Images/folder_closed.gif"
                End If
            End If
        ElseIf Not lnkBlog Is Nothing Then
            lnkBlog.NavigateUrl = NavigateURL()
        End If
    End Sub

    Protected Sub lstBlogChildren_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataListItemEventArgs)
        Dim lnkChildIcon As System.Web.UI.WebControls.HyperLink = CType(e.Item.FindControl("lnkChildIcon"), System.Web.UI.WebControls.HyperLink)
        Dim lnkChildBlog As System.Web.UI.WebControls.HyperLink = CType(e.Item.FindControl("lnkChildBlog"), System.Web.UI.WebControls.HyperLink)
        Dim lnkChildBlogRSS As System.Web.UI.WebControls.HyperLink = CType(e.Item.FindControl("lnkChildBlogRSS"), System.Web.UI.WebControls.HyperLink)

        lnkChildIcon.NavigateUrl = NavigateURL(Me.TabId, "", "&BlogID=" & CType(e.Item.DataItem, BlogInfo).BlogID.ToString() & "&ParentBlogID=" & CType(e.Item.DataItem, BlogInfo).ParentBlogID.ToString())
        lnkChildBlog.NavigateUrl = NavigateURL(Me.TabId, "", "&BlogID=" & CType(e.Item.DataItem, BlogInfo).BlogID.ToString() & "&ParentBlogID=" & CType(e.Item.DataItem, BlogInfo).ParentBlogID.ToString())

        If CType(e.Item.DataItem, BlogInfo).Syndicated Then
            lnkChildBlogRSS.NavigateUrl = NavigateURL(Me.TabId, "", "rssid=" & CType(e.Item.DataItem, BlogInfo).BlogID.ToString())
            lnkChildBlogRSS.Visible = True
        End If

        'DR-04/17/2009-BLG-9754
        lnkChildBlog.Text = String.Format(m_BlogTitleStringTemplate, CType(e.Item.DataItem, BlogInfo).Title, CType(e.Item.DataItem, BlogInfo).BlogPostCount)
    End Sub

#End Region

End Class