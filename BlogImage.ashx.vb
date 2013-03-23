Imports System.Web
Imports System.Web.Services
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Entities.Entries
Imports DotNetNuke.Entities.Portals

Public Class BlogImage
 Implements System.Web.IHttpHandler

 Private Property BlogId As Integer = -1
 Private Property EntryId As Integer = -1
 Private Property ModuleId As Integer = -1
 Private Property TabId As Integer = -1
 Private Property Key As String = ""
 Private Property Width As Integer = -1
 Private Property Height As Integer = -1
 Private Property Crop As Boolean = False
 Private Property PortalSettings As PortalSettings = Nothing

 Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

  PortalSettings = PortalController.GetCurrentPortalSettings
  context.Request.Params.ReadValue("Blog", BlogId)
  context.Request.Params.ReadValue("Post", EntryId)
  context.Request.Params.ReadValue("Key", Key)
  context.Request.Params.ReadValue("ModuleId", ModuleId)
  context.Request.Params.ReadValue("TabId", TabId)
  context.Request.Params.ReadValue("w", Width)
  context.Request.Params.ReadValue("h", Height)
  context.Request.Params.ReadValue("c", Crop)

  Try
   Dim path As String = ""
   If EntryId > -1 Then ' we're looking for an entry's image
    path = GetPostDirectoryMapPath(BlogId, EntryId)
   ElseIf BlogId > -1 Then ' we're looking for a blog's image
    path = GetBlogDirectoryMapPath(BlogId)
   End If
   Dim files() As String = IO.Directory.GetFiles(path, String.Format("{0}-{1}-{2}-{3}.*", Key, Width, Height, Crop))
   If files.Length > 0 Then
    context.Response.WriteFile(files(0))
   Else
    files = IO.Directory.GetFiles(path, String.Format("{0}.*", Key))
    If files.Length > 0 Then
     Dim img As New Common.Image(files(0))
     If img.IsValidExtension Then
      Dim newImg As String = img.ResizeImage(Width, Height, Crop)
      context.Response.WriteFile(newImg)
      img.Dispose()
     End If
    End If
   End If
  Catch ex As Exception
  End Try

 End Sub

 ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
  Get
   Return False
  End Get
 End Property

End Class