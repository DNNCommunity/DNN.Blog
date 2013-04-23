Imports DotNetNuke.Modules.Blog.Common
Imports System.Globalization
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json
Imports System.Linq
Imports System.Net
Imports System.Net.Http
Imports System.Web.Http
Imports DotNetNuke.Web.Api
Imports DotNetNuke.Modules.Blog
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Entities.Posts
Imports DotNetNuke.Modules.Blog.Entities.Comments
Imports DotNetNuke.Modules.Blog.Security
Imports DotNetNuke.Modules.Blog.Integration
Imports DotNetNuke.Modules.Blog.Services
Imports DotNetNuke.Modules.Blog.Templating

Namespace Entities.Comments
 Partial Public Class CommentsController
  Inherits DnnApiController

  Public Class CommentDTO
   Public Property BlogId As Integer
   Public Property CommentId As Integer
   Public Property Karma As Integer
  End Class

  Public Class FullCommentDTO
   Public Property BlogId As Integer
   Public Property PostId As Integer
   Public Property ParentId As Integer
   Public Property Comment As String
   Public Property Author As String
   Public Property Website As String
   Public Property Email As String
  End Class

#Region " Private Members "
  Private Property Blog As BlogInfo = Nothing
  Private Property Post As PostInfo = Nothing
  Private Property Comment As CommentInfo = Nothing
  Private Property AllComments As New List(Of CommentInfo)
  Private Property Settings As ModuleSettings = Nothing
  Private Property Security As ContextSecurity = Nothing
#End Region

#Region " Service Methods "
  <HttpPost()>
  <BlogAuthorizeAttribute(Services.SecurityAccessLevel.ApproveComment)>
  <ValidateAntiForgeryToken()>
  <ActionName("Approve")>
  Public Function ApproveComment(postData As CommentDTO) As HttpResponseMessage
   SetContext(postData)
   If Blog Is Nothing Or Comment Is Nothing Then
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   End If
   CommentsController.ApproveComment(ActiveModule.ModuleID, Blog.BlogID, Comment)
   Return Request.CreateResponse(HttpStatusCode.OK, New With {.Result = "success"})
  End Function

  <HttpPost()>
  <BlogAuthorizeAttribute(Services.SecurityAccessLevel.EditPost)>
  <ValidateAntiForgeryToken()>
  <ActionName("Delete")>
  Public Function DeleteComment(postData As CommentDTO) As HttpResponseMessage
   SetContext(postData)
   If Blog Is Nothing Or Comment Is Nothing Then
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   End If
   CommentsController.DeleteComment(ActiveModule.ModuleID, Blog.BlogID, Comment)
   Return Request.CreateResponse(HttpStatusCode.OK, New With {.Result = "success"})
  End Function

  <HttpPost()>
  <BlogAuthorizeAttribute(Services.SecurityAccessLevel.AddComment)>
  <ValidateAntiForgeryToken()>
  <ActionName("Karma")>
  Public Function ReportComment(postData As CommentDTO) As HttpResponseMessage
   SetContext(postData)
   If Blog Is Nothing Or Comment Is Nothing Then
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   End If
   If UserInfo.UserID < 0 Then Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   Dim ret As Integer = Data.DataProvider.Instance.AddCommentKarma(postData.CommentId, UserInfo.UserID, postData.Karma)
   If ret = -1 Then Return Request.CreateResponse(HttpStatusCode.OK, New With {.Result = "exists"})
   Return Request.CreateResponse(HttpStatusCode.OK, New With {.Result = "success"})
  End Function

  <HttpPost()>
  <BlogAuthorizeAttribute(Services.SecurityAccessLevel.AddComment)>
  <ValidateAntiForgeryToken()>
  <ActionName("Add")>
  Public Function AddComment(postData As FullCommentDTO) As HttpResponseMessage
   SetContext(postData)
   Post = PostsController.GetPost(postData.PostId, ActiveModule.ModuleID, "")
   If Blog Is Nothing Or Post Is Nothing Then
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   End If
   Dim objComment As New CommentInfo
   objComment.ContentItemId = Post.ContentItemId
   objComment.CreatedByUserID = UserInfo.UserID
   objComment.ParentId = postData.ParentId
   Dim ps As New DotNetNuke.Security.PortalSecurity
   objComment.Comment = HttpUtility.HtmlEncode(ps.InputFilter(postData.Comment, DotNetNuke.Security.PortalSecurity.FilterFlag.NoProfanity))
   Security = New ContextSecurity(ActiveModule.ModuleID, ActiveModule.TabID, Blog, UserInfo)
   objComment.Approved = Security.CanApproveComment
   objComment.Author = ps.InputFilter(postData.Author, DotNetNuke.Security.PortalSecurity.FilterFlag.NoMarkup And DotNetNuke.Security.PortalSecurity.FilterFlag.NoProfanity And DotNetNuke.Security.PortalSecurity.FilterFlag.NoScripting And DotNetNuke.Security.PortalSecurity.FilterFlag.NoSQL)
   objComment.Email = ps.InputFilter(postData.Email, DotNetNuke.Security.PortalSecurity.FilterFlag.NoMarkup And DotNetNuke.Security.PortalSecurity.FilterFlag.NoProfanity And DotNetNuke.Security.PortalSecurity.FilterFlag.NoScripting And DotNetNuke.Security.PortalSecurity.FilterFlag.NoSQL)
   objComment.Website = ps.InputFilter(postData.Website, DotNetNuke.Security.PortalSecurity.FilterFlag.NoMarkup And DotNetNuke.Security.PortalSecurity.FilterFlag.NoProfanity And DotNetNuke.Security.PortalSecurity.FilterFlag.NoScripting And DotNetNuke.Security.PortalSecurity.FilterFlag.NoSQL)
   objComment.CommentID = CommentsController.AddComment(Blog, Post, objComment)
   If Not objComment.Approved Then
    Return Request.CreateResponse(HttpStatusCode.OK, New With {.Result = "successnotapproved"})
   End If
   Return Request.CreateResponse(HttpStatusCode.OK, New With {.Result = "success"})
  End Function

  <HttpGet()>
  <BlogAuthorizeAttribute(Services.SecurityAccessLevel.ViewModule)>
  <ValidateAntiForgeryToken()>
  <ActionName("List")>
  Public Function ListComments() As HttpResponseMessage
   Dim BlogId As Integer = -1
   Dim PostId As Integer = -1
   HttpContext.Current.Request.Params.ReadValue("blogId", BlogId)
   HttpContext.Current.Request.Params.ReadValue("postId", PostId)
   Blog = BlogsController.GetBlog(BlogId, UserInfo.UserID, Threading.Thread.CurrentThread.CurrentCulture.Name)
   Post = PostsController.GetPost(PostId, ActiveModule.ModuleID, "")
   If Blog Is Nothing Or Post Is Nothing Then
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "error"})
   End If
   Security = New ContextSecurity(ActiveModule.ModuleID, ActiveModule.TabID, Blog, UserInfo)
   If Not Security.CanViewComments Then Return Request.CreateResponse(HttpStatusCode.OK, New With {.Result = ""})
   Dim ViewSettings As ViewSettings = ViewSettings.GetViewSettings(ActiveModule.TabModuleID)
   Settings = ModuleSettings.GetModuleSettings(ActiveModule.ModuleID)
   AllComments = CommentsController.GetCommentsByContentItem(Post.ContentItemId, Security.CanApproveComment)
   Dim vt As New ViewTemplate
   Dim tmgr As New TemplateManager(PortalSettings, ViewSettings.Template)
   With vt
    .TemplatePath = tmgr.TemplatePath
    .TemplateMapPath = tmgr.TemplateMapPath
    .DefaultReplacer = New BlogTokenReplace(ActiveModule, Security, Blog, Post, Settings)
    .StartTemplate = "CommentsTemplate.html"
   End With
   AddHandler vt.GetData, AddressOf TemplateGetData
   vt.DataBind()
   Return Request.CreateResponse(HttpStatusCode.OK, New With {.Result = vt.GetContentsAsString})
  End Function
#End Region

#Region " Private Methods "
  Private Sub SetContext(data As CommentDTO)
   Blog = BlogsController.GetBlog(data.BlogId, UserInfo.UserID, Threading.Thread.CurrentThread.CurrentCulture.Name)
   Comment = CommentsController.GetComment(data.CommentId)
  End Sub

  Private Sub SetContext(data As FullCommentDTO)
   Blog = BlogsController.GetBlog(data.BlogId, UserInfo.UserID, Threading.Thread.CurrentThread.CurrentCulture.Name)
  End Sub

  Private Sub TemplateGetData(ByVal DataSource As String, ByVal Parameters As Dictionary(Of String, String), ByRef Replacers As System.Collections.Generic.List(Of GenericTokenReplace), ByRef Arguments As System.Collections.Generic.List(Of String()), callingObject As Object)

   Select Case DataSource.ToLower

    Case "comments"

     If callingObject IsNot Nothing AndAlso TypeOf callingObject Is CommentInfo Then
      Dim parent As Integer = CType(callingObject, CommentInfo).CommentID
      For Each c As CommentInfo In AllComments.Where(Function(cmt) cmt.ParentId = parent).OrderBy(Function(cmt) cmt.CreatedOnDate)
       Replacers.Add(New BlogTokenReplace(ActiveModule, Security, Blog, Post, Settings, c))
      Next
     Else
      For Each c As CommentInfo In AllComments.Where(Function(cmt) cmt.ParentId = -1).OrderBy(Function(cmt) cmt.CreatedOnDate)
       Replacers.Add(New BlogTokenReplace(ActiveModule, Security, Blog, Post, Settings, c))
      Next
     End If

   End Select

  End Sub
#End Region

 End Class
End Namespace

