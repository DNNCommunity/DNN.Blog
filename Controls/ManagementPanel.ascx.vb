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

  Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

   LocalResourceFile = "~/DesktopModules/Blog/Controls/App_LocalResources/ManagementPanel.ascx.resx"
   If Not IsPostBack Then
    cmdManageBlogs.Visible = BlogContext.Security.IsBlogger Or BlogContext.Security.CanApprovePost Or BlogContext.Security.CanEditPost
    cmdAdmin.Visible = BlogContext.Security.IsEditor
    cmdBlog.Visible = BlogContext.Security.CanAddPost
    wlwlink.Visible = BlogContext.Security.CanAddPost And Settings.AllowWLW
    cmdEditPost.Visible = (BlogContext.Post IsNot Nothing) And (BlogContext.Security.CanEditPost Or BlogContext.Security.CanEditThisPost(BlogContext.Post))
    cmdCopyModule.Visible = BlogContext.Security.IsEditor
    pnlCopyModule.Visible = BlogContext.Security.IsEditor
    wlwlink.Title = LocalizeString("WLW")
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
     For Each d As IO.DirectoryInfo In (New IO.DirectoryInfo(Settings.PortalTemplatesMapPath)).GetDirectories
      ddTemplate.Items.Add(New ListItem(d.Name & " [Local]", "[P]" & d.Name))
     Next
     Dim skinTemplatePath As String = Server.MapPath(DotNetNuke.UI.Skins.Skin.GetSkin(Page).SkinPath) & "Templates\Blog\"
     If IO.Directory.Exists(skinTemplatePath) Then
      For Each d As IO.DirectoryInfo In (New IO.DirectoryInfo(skinTemplatePath)).GetDirectories
       ddTemplate.Items.Add(New ListItem(d.Name & " [Skin]", "[S]" & d.Name))
      Next
     End If
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
   If Not String.IsNullOrEmpty(BlogContext.ShowLocale) Then RssLink &= String.Format("&language={0}", BlogContext.ShowLocale)
   If BlogContext.Security.CanAddPost Then
    BlogSelectListHtml = "<select id=""" & ClientID & "ddBlog"">"
    Dim blogList As IEnumerable(Of BlogInfo) = Nothing
    If BlogContext.Security.IsEditor Then
     blogList = BlogsController.GetBlogsByModule(Settings.ModuleId, UserId, BlogContext.Locale).Values
    Else
     blogList = BlogsController.GetBlogsByModule(Settings.ModuleId, UserId, BlogContext.Locale).Values.Where(Function(b) b.OwnerUserId = UserId Or b.CanAdd Or (b.CanEdit And BlogContext.ContentItemId > -1))
    End If
    NrBlogs = blogList.Count
    If NrBlogs = 0 Then
     cmdBlog.Visible = False
    End If
    For Each b As BlogInfo In blogList
     BlogSelectListHtml &= String.Format("<option value=""{0}"">{1}</option>", b.BlogID, UI.Utilities.ClientAPI.GetSafeJSString(b.LocalizedTitle))
    Next
    BlogSelectListHtml &= "</select>"
   End If
   If BlogContext.Security.IsBlogger Or BlogContext.Security.IsEditor Or BlogContext.Security.UserIsAdmin Then
    doclink.Visible = True
    doclink.HRef = ResolveUrl("~/DesktopModules/Blog/Manual/manual.html")
    doclink.Title = LocalizeString("Documentation")
   End If

  End Sub

  Private Sub cmdAdmin_Click(sender As Object, e As EventArgs) Handles cmdAdmin.Click
   Response.Redirect(EditUrl("Admin"), False)
  End Sub

  Private Sub cmdManageBlogs_Click(sender As Object, e As EventArgs) Handles cmdManageBlogs.Click
   Response.Redirect(EditUrl("Manage"), False)
  End Sub

  Private Sub cmdBlog_Click(sender As Object, e As EventArgs) Handles cmdBlog.Click
   If BlogContext.BlogId <> -1 Then
    Response.Redirect(EditUrl("Blog", BlogContext.BlogId.ToString, "PostEdit"), False)
   Else
    If BlogContext.Security.IsEditor Then
     Dim b1 As BlogInfo = BlogsController.GetBlogsByModule(Settings.ModuleId, UserId, BlogContext.Locale).Values.First
     Response.Redirect(EditUrl("Blog", b1.BlogID.ToString, "PostEdit"), False)
    Else
     Dim b1 As BlogInfo = BlogsController.GetBlogsByModule(Settings.ModuleId, UserId, BlogContext.Locale).Values.FirstOrDefault(Function(b) b.OwnerUserId = UserId Or b.CanAdd Or (b.CanEdit And BlogContext.ContentItemId > -1))
     Response.Redirect(EditUrl("Blog", b1.BlogID.ToString, "PostEdit"), False)
    End If
   End If
  End Sub

  Private Sub cmdEditPost_Click(sender As Object, e As EventArgs) Handles cmdEditPost.Click
   Response.Redirect(EditUrl("Post", BlogContext.ContentItemId.ToString, "PostEdit"), False)
  End Sub

 End Class
End Namespace