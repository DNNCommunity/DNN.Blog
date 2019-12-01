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

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Entities.Posts
Imports DotNetNuke.Services.Search.Entities

Namespace Integration
 Partial Public Class BlogModuleController
  Inherits ModuleSearchBase

#Region " Search Implementation "
  Public Overrides Function GetModifiedSearchDocuments(moduleInfo As ModuleInfo, beginDate As Date) As IList(Of SearchDocument)

   Dim res As New List(Of SearchDocument)
   Dim settings As New ViewSettings(moduleInfo.TabModuleID, True)
   If settings.BlogModuleId <> -1 And settings.BlogModuleId <> moduleInfo.ModuleID Then Return res ' bail out if it's a slave module

   ' Blogs
   For Each b As BlogInfo In BlogsController.GetBlogsByModule(moduleInfo.ModuleID, "").Values
    If b.LastModifiedOnDate.ToUniversalTime() >= beginDate Then
     res.Add(New SearchDocument With {
                 .AuthorUserId = b.OwnerUserId,
                 .Body = b.Title & " - " & b.Description,
                 .Description = b.Description,
                 .ModifiedTimeUtc = b.LastModifiedOnDate.ToUniversalTime(),
                 .PortalId = moduleInfo.PortalID,
                 .QueryString = "Blog=" & b.BlogID.ToString(),
                 .Title = b.Title,
                 .UniqueKey = "Blog" & b.BlogID.ToString()})
    End If
   Next

   ' Posts
   Dim addedPrimaryPosts As New List(Of Integer)
   For Each p As PostInfo In PostsController.GetChangedPosts(moduleInfo.ModuleID, beginDate)
    If Not addedPrimaryPosts.Contains(p.ContentItemId) Then
     res.Add(New SearchDocument With {
       .AuthorUserId = p.CreatedByUserID,
       .Body = p.Summary & " " & HtmlUtils.Clean(p.Content, False),
       .Description = p.Summary,
       .ModifiedTimeUtc = p.LastModifiedOnDate.ToUniversalTime(),
       .PortalId = moduleInfo.PortalID,
       .QueryString = "Post=" & p.ContentItemId.ToString(),
       .Title = p.Title,
       .UniqueKey = "BlogPost" & p.ContentItemId.ToString()})
     addedPrimaryPosts.Add(p.ContentItemId)
    End If
    If p.AltLocale <> "" Then
     res.Add(New SearchDocument With {
       .AuthorUserId = p.CreatedByUserID,
       .Body = p.AltSummary & " " & HtmlUtils.Clean(p.AltContent, False),
       .CultureCode = p.AltLocale,
       .Description = p.AltSummary,
       .ModifiedTimeUtc = p.LastModifiedOnDate.ToUniversalTime(),
       .PortalId = moduleInfo.PortalID,
       .QueryString = "Post=" & p.ContentItemId.ToString(),
       .Title = p.AltTitle,
       .UniqueKey = "BlogPost" & p.ContentItemId.ToString()})
    End If
   Next
   Return res

  End Function
#End Region

 End Class
End Namespace
