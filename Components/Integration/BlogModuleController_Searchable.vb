'
' DNN Connect - http://dnn-connect.org
' Copyright (c) 2014
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

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Search
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Entities.Posts

Namespace Integration
 Partial Public Class BlogModuleController
  Implements ISearchable

#Region " ISearchable Methods "
  Public Function GetSearchItems(ByVal ModInfo As DotNetNuke.Entities.Modules.ModuleInfo) As SearchItemInfoCollection Implements ISearchable.GetSearchItems

   Dim settings As New ViewSettings(ModInfo.TabModuleID, True)
   If settings.BlogModuleId <> -1 And settings.BlogModuleId <> ModInfo.ModuleID Then Return New SearchItemInfoCollection ' bail out if it's a slave module

   Dim SearchItemCollection As New SearchItemInfoCollection
   For Each b As BlogInfo In BlogsController.GetBlogsByModule(ModInfo.ModuleID, "").Values
    SearchItemCollection.Add(New SearchItemInfo With {.Author = b.OwnerUserId, .Content = b.Title & " - " & b.Description, .Description = b.Description, .ModuleId = ModInfo.ModuleID, .PubDate = b.FirstPublishDate, .Title = b.Title, .GUID = "Blog=" & b.BlogID.ToString, .SearchKey = b.BlogID.ToString})
    Dim page As Integer = 0
    Dim totalRecords As Integer = 1
    Do While page * 10 < totalRecords
     For Each p As PostInfo In PostsController.GetPostsByBlog(ModInfo.ModuleID, b.BlogID, "", -1, page, 20, "PUBLISHEDONDATE DESC", totalRecords).Values
      SearchItemCollection.Add(New SearchItemInfo With {.Author = p.CreatedByUserID, .Content = p.Title & vbCrLf & p.Summary & vbCrLf & p.Content, .Description = p.Summary, .ModuleId = ModInfo.ModuleID, .PubDate = p.PublishedOnDate, .Title = p.Title, .GUID = "Post=" & p.ContentItemId.ToString, .SearchKey = p.ContentItemId.ToString})
     Next
     page += 1
    Loop
   Next

   Return SearchItemCollection

  End Function
#End Region

 End Class
End Namespace