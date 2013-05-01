Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports System.Linq
Imports DotNetNuke.Modules.Blog.Entities.Posts
Imports DotNetNuke.Web.UI.WebControls
Imports DotNetNuke.Web.Client.ClientResourceManagement
Imports DotNetNuke.Modules.Blog.Common.Globals

Public Class Manage
 Inherits BlogModuleBase

 Private _totalPosts As Integer = -1

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

  If Not BlogContext.security.CanDoSomethingWithPosts Then
   Throw New Exception("You do not have access to this resource. Please check your login status.")
  End If

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

  If BlogContext.security.IsEditor Then
   dlBlogs.DataSource = BlogsController.GetBlogsByModule(BlogContext.Settings.ModuleId, UserId, BlogContext.Locale).Values
  Else
   dlBlogs.DataSource = BlogsController.GetBlogsByModule(BlogContext.Settings.ModuleId, UserId, BlogContext.Locale).Values.Where(Function(b)
                                                                                                                                  Return b.OwnerUserId = UserId
                                                                                                                                 End Function)
  End If
  dlBlogs.DataBind()
  If dlBlogs.Items.Count = 0 Then dlBlogs.Visible = False

 End Sub

 Public Sub GetPosts()

  grdPosts.DataSource = PostsController.GetPosts(BlogContext.Settings.ModuleId, -1, BlogContext.Locale, -1, "", Now, -1, grdPosts.CurrentPageIndex, grdPosts.PageSize, "PUBLISHEDONDATE DESC", _totalPosts, UserId, BlogContext.Security.UserIsAdmin).Values
  grdPosts.VirtualItemCount = _totalPosts

 End Sub

 Public Sub RebindPosts()
  GetPosts()
  grdPosts.Rebind()
 End Sub

End Class