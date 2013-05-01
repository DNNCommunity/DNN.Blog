Imports System.Linq
Imports DotNetNuke.Services.Localization.Localization
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports DotNetNuke.Modules.Blog.Entities.Blogs

Namespace Controls
 Public Class ManagementPanel
  Inherits BlogModuleBase

  Public Property RssLink As String = ""
  Public Property BlogSelectListHtml As String = ""
  Public Property NrBlogs As Integer = 0

  Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

   LocalResourceFile = "~/DesktopModules/Blog/Controls/App_LocalResources/ManagementPanel.ascx.resx"
   If Not Me.IsPostBack Then
    cmdManageBlogs.Visible = BlogContext.Security.IsBlogger Or BlogContext.Security.CanApprovePost
    cmdAdmin.Visible = BlogContext.Security.IsEditor
    cmdBlog.Visible = BlogContext.Security.CanAddPost
    cmdEditPost.Visible = (BlogContext.Post IsNot Nothing) And BlogContext.Security.CanEditPost
    cmdCopyModule.Visible = BlogContext.Security.IsEditor
    pnlCopyModule.Visible = BlogContext.Security.IsEditor
    wlwlink.Title = LocalizeString("WLW")
    wlwlink.Visible = BlogContext.Security.CanAddPost
    If BlogContext.Post IsNot Nothing Then
     cmdBlog.Text = LocalizeString("cmdEdit")
    End If
    If BlogContext.Security.IsEditor Then
     ddTemplate.Items.Clear()
     ddTemplate.Items.Add(New ListItem("Default [System]", "[G]_default"))
     For Each d As IO.DirectoryInfo In (New IO.DirectoryInfo(HttpContext.Current.Server.MapPath(DotNetNuke.Common.ResolveUrl(glbTemplatesPath)))).GetDirectories
      If d.Name <> "_default" Then
       ddTemplate.Items.Add(New ListItem(d.Name & " [System]", "[G]" & d.Name))
      End If
     Next
     For Each d As IO.DirectoryInfo In (New IO.DirectoryInfo(BlogContext.Settings.PortalTemplatesMapPath)).GetDirectories
      ddTemplate.Items.Add(New ListItem(d.Name & " [Local]", "[P]" & d.Name))
     Next
     For intItem As Integer = 0 To PortalSettings.ActiveTab.Panes.Count - 1
      ddPane.Items.Add(Convert.ToString(PortalSettings.ActiveTab.Panes(intItem)))
     Next intItem
     If Not ddPane.Items.FindByValue(DotNetNuke.Common.Globals.glbDefaultPane) Is Nothing Then
      ddPane.Items.FindByValue(DotNetNuke.Common.Globals.glbDefaultPane).Selected = True
     End If
     ddPosition.Items.Clear()
     ddPosition.Items.Add(New ListItem(GetString("Top", "admin/controlpanel/App_LocalResources/iconbar"), "0"))
     ddPosition.Items.Add(New ListItem(GetString("Bottom", "admin/controlpanel/App_LocalResources/iconbar"), "-1"))
    End If
   End If
   RssLink = ResolveUrl(glbServicesPath) & "RSS/Get"
   RssLink &= String.Format("?tabid={0}&moduleid={1}", TabId, ModuleId)
   If BlogContext.Blog IsNot Nothing Then RssLink &= String.Format("&blog={0}", BlogContext.BlogId)
   If BlogContext.Term IsNot Nothing Then RssLink &= String.Format("&term={0}", BlogContext.TermId)
   If Not String.IsNullOrEmpty(BlogContext.SearchString) Then
    RssLink &= String.Format("&search={0}&t={1}&c={2}", HttpUtility.UrlEncode(BlogContext.SearchString), BlogContext.SearchTitle, BlogContext.SearchContents)
   End If
   If BlogContext.ShowLocale <> "" Then RssLink &= String.Format("&language={0}", BlogContext.ShowLocale)
   If BlogContext.Security.CanAddPost Then
    BlogSelectListHtml = "<select id=""ddBlog"">"
    Dim blogList As IEnumerable(Of BlogInfo) = Nothing
    If BlogContext.Security.IsEditor Then
     blogList = BlogsController.GetBlogsByModule(BlogContext.Settings.ModuleId, UserId, BlogContext.Locale).Values
    Else
     blogList = BlogsController.GetBlogsByModule(BlogContext.Settings.ModuleId, UserId, BlogContext.Locale).Values.Where(Function(b)
                                                                                                                          Return b.OwnerUserId = UserId Or b.CanAdd Or (b.CanEdit And BlogContext.ContentItemId > -1)
                                                                                                                         End Function)
    End If
    NrBlogs = blogList.Count
    If NrBlogs = 0 Then
     cmdBlog.Visible = False
    End If
    For Each b As BlogInfo In blogList
     BlogSelectListHtml &= String.Format("<option value=""{0}"">{1}</option>", b.BlogID, b.LocalizedTitle)
    Next
    BlogSelectListHtml &= "</select>"
   End If

  End Sub

  Private Sub cmdAdmin_Click(sender As Object, e As System.EventArgs) Handles cmdAdmin.Click
   Response.Redirect(EditUrl("Admin"), False)
  End Sub

  Private Sub cmdManageBlogs_Click(sender As Object, e As System.EventArgs) Handles cmdManageBlogs.Click
   Response.Redirect(EditUrl("Manage"), False)
  End Sub

  Private Sub cmdBlog_Click(sender As Object, e As System.EventArgs) Handles cmdBlog.Click
   If BlogContext.BlogId <> -1 Then
    Response.Redirect(EditUrl("Blog", BlogContext.BlogId.ToString, "PostEdit"), False)
   Else
    Response.Redirect(EditUrl("PostEdit"), False)
   End If
  End Sub

  Private Sub cmdEditPost_Click(sender As Object, e As System.EventArgs) Handles cmdEditPost.Click
   Response.Redirect(EditUrl("Post", BlogContext.ContentItemId.ToString, "PostEdit"), False)
  End Sub

 End Class
End Namespace