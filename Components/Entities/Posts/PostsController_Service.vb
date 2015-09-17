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

Imports System.Net
Imports System.Net.Http
Imports System.Web.Http
Imports DotNetNuke.Web.Api
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Services

Namespace Entities.Posts
 Partial Public Class PostsController
  Inherits DnnApiController

  Public Class PostDTO
   Public Property BlogId As Integer
   Public Property PostId As Integer
  End Class

#Region " Private Members "
  Private Property Blog As BlogInfo = Nothing
  Private Property Post As PostInfo = Nothing
#End Region

#Region " Service Methods "
  <HttpPost()>
  <BlogAuthorizeAttribute(Services.SecurityAccessLevel.ApprovePost)>
  <ValidateAntiForgeryToken()>
  <ActionName("Approve")>
  Public Function ApprovePost(postData As PostDTO) As HttpResponseMessage
   SetContext(postData)
   If Blog Is Nothing Or Post Is Nothing Then
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   End If
   PostsController.PublishPost(Post, True, UserInfo.UserID)
   Return Request.CreateResponse(HttpStatusCode.OK, New With {.Result = "success"})
  End Function

  <HttpPost()>
  <BlogAuthorizeAttribute(Services.SecurityAccessLevel.EditPost)>
  <ValidateAntiForgeryToken()>
  <ActionName("Delete")>
  Public Function DeletePost(postData As PostDTO) As HttpResponseMessage
   SetContext(postData)
   If Blog Is Nothing Or Post Is Nothing Then
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   End If
   PostsController.DeletePost(Post)
   Return Request.CreateResponse(HttpStatusCode.OK, New With {.Result = "success"})
  End Function

  <HttpPost()>
  <AllowAnonymous()>
  <ValidateAntiForgeryToken()>
  <ActionName("View")>
  Public Function ViewPost(postData As PostDTO) As HttpResponseMessage
   SetContext(postData)
   If Blog Is Nothing Or Post Is Nothing Then
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   End If
   Data.DataProvider.Instance.AddPostView(postData.PostId)
   Return Request.CreateResponse(HttpStatusCode.OK, New With {.Result = "success"})
  End Function
#End Region

#Region " Private Methods "
  Private Sub SetContext(data As PostDTO)
   Blog = BlogsController.GetBlog(data.BlogId, UserInfo.UserID, Threading.Thread.CurrentThread.CurrentCulture.Name)
   Post = PostsController.GetPost(data.PostId, ActiveModule.ModuleID, Threading.Thread.CurrentThread.CurrentCulture.Name)
  End Sub
#End Region

 End Class
End Namespace