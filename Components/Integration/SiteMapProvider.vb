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
Imports DotNetNuke.Services.Sitemap
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Modules.Blog.Entities.Posts

Namespace Integration
 Public Class BlogSiteMapProvider
  Inherits SitemapProvider

  Public Overrides Function GetUrls(portalId As Integer, ps As DotNetNuke.Entities.Portals.PortalSettings, version As String) As System.Collections.Generic.List(Of SitemapUrl)

   Dim SitemapUrls As New List(Of SitemapUrl)
   Dim moduleTabs As New Dictionary(Of Integer, List(Of DotNetNuke.Entities.Tabs.TabInfo))
   For Each blog As BlogInfo In BlogsController.GetBlogsByPortal(portalId, -1, "").Values
    If Not moduleTabs.ContainsKey(blog.ModuleID) Then
     Dim tabs As New List(Of DotNetNuke.Entities.Tabs.TabInfo)
     For Each t As DotNetNuke.Entities.Tabs.TabInfo In (New DotNetNuke.Entities.Tabs.TabController).GetTabsByModuleID(blog.ModuleID).Values
      For Each tp As TabPermissionInfo In t.TabPermissions
       If tp.RoleName = DotNetNuke.Common.Globals.glbRoleAllUsersName Then
        tabs.Add(t)
        Exit For
       End If
      Next
     Next
     moduleTabs.Add(blog.ModuleID, tabs)
    End If
    Dim totalRecs As Integer = 0
    For Each p As PostInfo In PostsController.GetPostsByBlog(blog.ModuleID, blog.BlogID, Threading.Thread.CurrentThread.CurrentCulture.Name, -1, -1, 0, Nothing, totalRecs).Values
     If p.Published Then
      For Each t As DotNetNuke.Entities.Tabs.TabInfo In moduleTabs(blog.ModuleID)
       Dim smu As New SitemapUrl With {.ChangeFrequency = SitemapChangeFrequency.Daily, .LastModified = p.LastModifiedOnDate, .Priority = 0.5, .Url = p.PermaLink(t)}
       SitemapUrls.Add(smu)
      Next
     End If
    Next
   Next
   Return SitemapUrls

  End Function

 End Class
End Namespace
