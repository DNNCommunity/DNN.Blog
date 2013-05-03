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