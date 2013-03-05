Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports System.Linq
Imports DotNetNuke.Modules.Blog.Entities.Entries
Imports DotNetNuke.Web.UI.WebControls

Public Class Manage
 Inherits BlogModuleBase

 Private _totalEntries As Integer = -1

 Private Sub Page_Init1(sender As Object, e As System.EventArgs) Handles Me.Init

  DotNetNuke.Framework.jQuery.RequestDnnPluginsRegistration()
  DotNetNuke.Framework.ServicesFramework.Instance.RequestAjaxScriptSupport()
  DotNetNuke.Framework.ServicesFramework.Instance.RequestAjaxAntiForgerySupport()
  Web.Client.ClientResourceManagement.ClientResourceManager.RegisterScript(Page, TemplateSourceDirectory + "/js/dotnetnuke.blog.js")

 End Sub

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

  If Not Me.IsPostBack Then
   Me.DataBind()
  End If

 End Sub

 Private Sub cmdAdd_Click(sender As Object, e As System.EventArgs) Handles cmdAdd.Click
  Response.Redirect(EditUrl("BlogEdit"), False)
 End Sub

 Private Sub cmdCancel_Click(sender As Object, e As System.EventArgs) Handles cmdCancel.Click
  Response.Redirect(DotNetNuke.Common.NavigateURL(TabId), False)
 End Sub

 Public Overrides Sub DataBind()
  MyBase.DataBind()

  If Security.IsEditor Then
   dlBlogs.DataSource = BlogsController.GetBlogsByModule(ModuleId, UserId).Values
  Else
   dlBlogs.DataSource = BlogsController.GetBlogsByModule(ModuleId, UserId).Values.Where(Function(b)
                                                                                         Return b.OwnerUserId = UserId
                                                                                        End Function)
  End If
  dlBlogs.DataBind()

 End Sub

 Public Sub GetEntries()

  grdEntries.DataSource = EntriesController.GetEntries(ModuleId, -1, -1, Now, -1, grdEntries.CurrentPageIndex, grdEntries.PageSize, "PUBLISHEDONDATE DESC", _totalEntries, UserId).Values
  grdEntries.VirtualItemCount = _totalEntries

 End Sub

 Public Sub RebindEntries()
  GetEntries()
  grdEntries.Rebind()
 End Sub

End Class