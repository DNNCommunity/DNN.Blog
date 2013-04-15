Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports System.Linq
Imports DotNetNuke.Modules.Blog.Entities.Posts
Imports DotNetNuke.Web.UI.WebControls
Imports DotNetNuke.Web.Client.ClientResourceManagement
Imports DotNetNuke.Modules.Blog.Common.Globals

Public Class Manage
 Inherits BlogModuleBase

 Private _totalPosts As Integer = -1

 Private Sub Page_Init1(sender As Object, e As System.EventArgs) Handles Me.Init
  AddBlogService()
 End Sub

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

  If Not Me.IsPostBack Then
   Me.DataBind()
  End If

 End Sub

 Private Sub cmdAdd_Click(sender As Object, e As System.EventArgs) Handles cmdAdd.Click
  Response.Redirect(EditUrl("BlogEdit"), False)
 End Sub

 Private Sub cmdReturn_Click(sender As Object, e As System.EventArgs) Handles cmdReturn.Click
  Response.Redirect(DotNetNuke.Common.NavigateURL(TabId), False)
 End Sub

 Public Overrides Sub DataBind()

  MyBase.DataBind()

  If Security.IsEditor Then
   dlBlogs.DataSource = BlogsController.GetBlogsByModule(Settings.ModuleId, UserId, Locale).Values
  Else
   dlBlogs.DataSource = BlogsController.GetBlogsByModule(Settings.ModuleId, UserId, Locale).Values.Where(Function(b)
                                                                                                          Return b.OwnerUserId = UserId
                                                                                                         End Function)
  End If
  dlBlogs.DataBind()
  If dlBlogs.Items.Count = 0 Then dlBlogs.Visible = False

 End Sub

 Public Sub GetPosts()

  grdPosts.DataSource = PostsController.GetPosts(Settings.ModuleId, -1, Locale, -1, "", Now, -1, grdPosts.CurrentPageIndex, grdPosts.PageSize, "PUBLISHEDONDATE DESC", _totalPosts, UserId, Security.UserIsAdmin).Values
  grdPosts.VirtualItemCount = _totalPosts

 End Sub

 Public Sub RebindPosts()
  GetPosts()
  grdPosts.Rebind()
 End Sub

End Class