'
' DNN Connect - http://dnn-connect.org
' Copyright (c) 2015
' by DNN Connect
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

Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports System.Linq
Imports DotNetNuke.Modules.Blog.Entities.Posts

Public Class Manage
  Inherits BlogModuleBase

  Private _totalPosts As Integer = -1
  Private Const ASCENDING As String = " ASC"
  Private Const DESCENDING As String = " DESC"

  Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

    If Not BlogContext.Security.CanDoSomethingWithPosts Then
      Throw New Exception("You do not have access to this resource. Please check your login status.")
    End If
    cmdAdd.Visible = BlogContext.Security.IsBlogger
    blogsLink.Visible = BlogContext.Security.IsBlogger
    Blogs.Visible = BlogContext.Security.IsBlogger

    postsLink.Visible = BlogContext.Security.CanAddPost Or BlogContext.Security.CanEditPost
    Posts.Visible = BlogContext.Security.CanAddPost Or BlogContext.Security.CanEditPost

    If Not IsPostBack Then
      DataBind()
    End If

  End Sub

  Private Sub cmdAdd_Click(sender As Object, e As EventArgs) Handles cmdAdd.Click
    Response.Redirect(EditUrl("BlogEdit"), False)
  End Sub

  Private Sub cmdReturn_Click(sender As Object, e As EventArgs) Handles cmdReturn.Click
    Response.Redirect(DotNetNuke.Common.NavigateURL(TabId), False)
  End Sub

  Public Overrides Sub DataBind()

    MyBase.DataBind()

    If BlogContext.Security.IsEditor Then
      dlBlogs.DataSource = BlogsController.GetBlogsByModule(Settings.ModuleId, UserId, BlogContext.Locale).Values
    Else
      dlBlogs.DataSource = BlogsController.GetBlogsByModule(Settings.ModuleId, UserId, BlogContext.Locale).Values.Where(Function(b)
                                                                                                                          Return b.OwnerUserId = UserId
                                                                                                                        End Function)
    End If
    dlBlogs.DataBind()

    If dlBlogs.Items.Count = 0 Then dlBlogs.Visible = False

    GetPosts()

  End Sub

  Public Sub GetPosts()
    grdPosts.DataSource = PostsController.GetPosts(Settings.ModuleId, -1, BlogContext.Locale, -1, "", Nothing, -1, True, grdPosts.PageIndex, grdPosts.PageSize, SortOrder, _totalPosts, UserId, BlogContext.Security.UserIsAdmin).Values
    grdPosts.VirtualItemCount = _totalPosts
    grdPosts.DataBind()
  End Sub

  Public Property GridViewSortDirection As SortDirection
    Get
      If ViewState("sortDirection") Is Nothing Then ViewState("sortDirection") = SortDirection.Descending
      Return CType(ViewState("sortDirection"), SortDirection)
    End Get
    Set(ByVal value As SortDirection)
      ViewState("sortDirection") = value
    End Set
  End Property

  Public Property GridViewSortExpression As String
    Get
      If ViewState("sortExpression") Is Nothing Then ViewState("sortExpression") = "PUBLISHEDONDATE"
      Return ViewState("sortExpression").ToString()
    End Get
    Set(ByVal value As String)
      ViewState("sortExpression") = value
    End Set
  End Property

  Private ReadOnly Property SortOrder As String
    Get
      If GridViewSortDirection = SortDirection.Ascending Then
        Return String.Concat(GridViewSortExpression, " ASC")
      Else
        Return String.Concat(GridViewSortExpression, " DESC")
      End If
    End Get
  End Property

  Protected Sub GridView_Sorting(sender As Object, e As GridViewSortEventArgs) Handles grdPosts.Sorting
    Dim sortExpression As String = e.SortExpression

    If GridViewSortDirection = SortDirection.Ascending Then
      GridViewSortDirection = SortDirection.Descending
    Else
      GridViewSortDirection = SortDirection.Ascending
    End If

    GetPosts()
  End Sub

  Protected Sub GridView_Paging(sender As Object, e As GridViewPageEventArgs) Handles grdPosts.PageIndexChanging
    grdPosts.PageIndex = e.NewPageIndex
    GetPosts()
  End Sub

End Class