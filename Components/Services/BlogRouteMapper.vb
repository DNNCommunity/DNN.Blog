'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2011
' by DotNetNuke Corporation
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
Option Strict On
Option Explicit On

Imports DotNetNuke.Web.Api

Namespace Services

 Public Class BlogRouteMapper
  Implements IServiceRouteMapper

  Public Sub RegisterRoutes(mapRouteManager As IMapRoute) Implements IServiceRouteMapper.RegisterRoutes
   mapRouteManager.MapHttpRoute("Blog", "Blogs", "Blogs/{action}", New With {.Controller = "Blogs"}, New String() {"DotNetNuke.Modules.Blog.Entities.Blogs"})
   mapRouteManager.MapHttpRoute("Blog", "Comments", "Comments/{action}", New With {.Controller = "Comments"}, New String() {"DotNetNuke.Modules.Blog.Entities.Comments"})
   mapRouteManager.MapHttpRoute("Blog", "Posts", "Posts/{action}", New With {.Controller = "Posts"}, New String() {"DotNetNuke.Modules.Blog.Entities.Posts"})
   mapRouteManager.MapHttpRoute("Blog", "Terms", "Terms/{action}", New With {.Controller = "Terms"}, New String() {"DotNetNuke.Modules.Blog.Entities.Terms"})
   mapRouteManager.MapHttpRoute("Blog", "Other", "{controller}/{action}", New String() {"DotNetNuke.Modules.Blog.Services", "DotNetNuke.Modules.Blog.Integration.Services"})
  End Sub

 End Class

End Namespace