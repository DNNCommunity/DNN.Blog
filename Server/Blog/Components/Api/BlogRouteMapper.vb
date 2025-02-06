Imports DotNetNuke.Web.Api

Namespace Api
  Public Class BlogRouteMapper
    Implements IServiceRouteMapper

    Public Enum ServiceControllers
      Blogs
      Comments
      Posts
      Terms
    End Enum

    Public Const ServicePath As String = "~/DesktopModules/Blog/API/"

    Public Sub RegisterRoutes(mapRouteManager As IMapRoute) Implements IServiceRouteMapper.RegisterRoutes
      mapRouteManager.MapHttpRoute("Blog", "Blogs", "Blogs/{action}", New With {.Controller = "Blogs"}, New String() {"DotNetNuke.Modules.Blog.Api"})
      mapRouteManager.MapHttpRoute("Blog", "Comments", "Comments/{action}", New With {.Controller = "Comments"}, New String() {"DotNetNuke.Modules.Blog.Api"})
      mapRouteManager.MapHttpRoute("Blog", "Posts", "Posts/{action}", New With {.Controller = "Posts"}, New String() {"DotNetNuke.Modules.Blog.Api"})
      mapRouteManager.MapHttpRoute("Blog", "Terms", "Terms/{action}", New With {.Controller = "Terms"}, New String() {"DotNetNuke.Modules.Blog.Api"})
      mapRouteManager.MapHttpRoute("Blog", "Other", "{controller}/{action}", New String() {"DotNetNuke.Modules.Blog.Services", "DotNetNuke.Modules.Blog.Api"})
    End Sub

    Public Shared Function GetRoute(controller As ServiceControllers, method As String) As String
      Select Case controller
        Case ServiceControllers.Blogs
          Return GetRoute("Blogs", method)
        Case ServiceControllers.Comments
          Return GetRoute("Comments", method)
        Case ServiceControllers.Posts
          Return GetRoute("Posts", method)
        Case Else
          Return GetRoute("Terms", method)
      End Select
    End Function

    Public Shared Function GetRoute(controller As String, method As String) As String
      Return HttpContext.Current.Request.Url.Scheme & "://" & HttpContext.Current.Request.Url.Host & DotNetNuke.Common.ResolveUrl(ServicePath & controller & "/" & method)
    End Function

  End Class
End Namespace