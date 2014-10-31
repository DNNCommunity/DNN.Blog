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
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Entities.Posts
Imports DotNetNuke.Modules.Blog.Entities.Comments
Imports DotNetNuke.Modules.Blog.Security
Imports DotNetNuke.Modules.Blog.Integration
Imports DotNetNuke.Modules.Blog.Services
Imports DotNetNuke.Modules.Blog.Templating
Imports System.Xml

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

  Private _Settings As ModuleSettings
  Private Property Settings() As ModuleSettings
   Get
    If _Settings Is Nothing Then
     _Settings = ModuleSettings.GetModuleSettings(ActiveModule.ModuleID)
    End If
    Return _Settings
   End Get
   Set(ByVal value As ModuleSettings)
    _Settings = value
   End Set
  End Property

  Private _viewSettings As ViewSettings
  Private Property ViewSettings() As ViewSettings
   Get
    If _viewSettings Is Nothing Then
     _viewSettings = ViewSettings.GetViewSettings(ActiveModule.TabModuleID)
    End If
    Return _viewSettings
   End Get
   Set(ByVal value As ViewSettings)
    _viewSettings = value
   End Set
  End Property

  Private _Security As ContextSecurity
  Private Property Security() As ContextSecurity
   Get
    If _Security Is Nothing Then
     _Security = New ContextSecurity(ActiveModule.ModuleID, ActiveModule.TabID, Blog, UserInfo)
    End If
    Return _Security
   End Get
   Set(ByVal value As ContextSecurity)
    _Security = value
   End Set
  End Property
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
   objComment.Comment = HttpUtility.HtmlEncode(SafeStringSimpleHtml(postData.Comment).Replace(vbCrLf, "<br />"))
   objComment.Approved = Security.CanAutoApproveComment Or Security.CanApproveComment Or Post.CreatedByUserID = UserInfo.UserID
   objComment.Author = SafeString(postData.Author)
   objComment.Email = SafeString(postData.Email)
   objComment.Website = SafeString(postData.Website)
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
   If Not Security.CanViewComments Then Return Request.CreateResponse(HttpStatusCode.OK, New With {.Result = ""})
   Dim ViewSettings As ViewSettings = ViewSettings.GetViewSettings(ActiveModule.TabModuleID)
   AllComments = CommentsController.GetCommentsByContentItem(Post.ContentItemId, Security.CanApproveComment, UserInfo.UserID)
   Dim vt As New ViewTemplate
   Dim tmgr As New TemplateManager(PortalSettings, ViewSettings.Template)
   With vt
    .TemplatePath = tmgr.TemplatePath
    .TemplateRelPath = tmgr.TemplateRelPath
    .TemplateMapPath = tmgr.TemplateMapPath
    .DefaultReplacer = New BlogTokenReplace(ActiveModule, Security, Blog, Post, Settings, ViewSettings)
    .StartTemplate = "CommentsTemplate.html"
   End With
   AddHandler vt.GetData, AddressOf TemplateGetData
   vt.DataBind()
   Return Request.CreateResponse(HttpStatusCode.OK, New With {.Result = vt.GetContentsAsString})
  End Function

  <HttpPost()>
  <AllowAnonymous()>
  <ActionName("Pingback")>
  Public Function Pingback() As HttpResponseMessage

   Dim BlogId As Integer = -1
   Dim PostId As Integer = -1
   HttpContext.Current.Request.Params.ReadValue("blogId", BlogId)
   HttpContext.Current.Request.Params.ReadValue("postId", PostId)
   Blog = BlogsController.GetBlog(BlogId, UserInfo.UserID, Threading.Thread.CurrentThread.CurrentCulture.Name)
   If Not Blog.EnablePingBackReceive Then
    Return Request.CreateResponse(HttpStatusCode.NotFound, New With {.Result = "This blog does not accept pingbacks"})
   End If
   Post = PostsController.GetPost(PostId, ActiveModule.ModuleID, "")
   If Post Is Nothing Then
    Return PingBackError(32, "The specified target URI does not exist.")
   End If

   Dim doc As XmlDocument = RetrieveXmlDocument(HttpContext.Current)
   Dim list As XmlNodeList = If(doc.SelectNodes("methodCall/params/param/value/string"), doc.SelectNodes("methodCall/params/param/value"))

   If list Is Nothing Then
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "Cannot parse the request"})
   End If

   Dim sourceUrl As String = SafeString(list(0).InnerText.Trim())
   Dim targetUrl As String = SafeString(list(1).InnerText.Trim())

   Dim containsHtml As Boolean = False
   Dim sourceHasLink As Boolean = False
   Dim title As String = sourceUrl

   Try
    CheckSourcePage(sourceUrl, targetUrl, sourceHasLink, title)
   Catch ex As Exception
   End Try

   If Not IsFirstPingBack(Post, sourceUrl) Then
    Return PingBackError(48, "The pingback has already been registered.")
   End If

   If Not sourceHasLink Then
    Return PingBackError(17, "The source URI does not contain a link to the target URI, and so cannot be used as a source.")
   End If

   If containsHtml Then
    ' spam
    Return Request.CreateResponse(HttpStatusCode.BadRequest, New With {.Result = "Cannot parse the request"})
   Else
    Dim objComment As New CommentInfo With {.ContentItemId = Post.ContentItemId, .Author = GetDomain(sourceUrl), .Website = sourceUrl}
    Dim comment As String = String.Format(DotNetNuke.Services.Localization.Localization.GetString("PingbackComment", SharedResourceFileName), objComment.Author, sourceUrl, title)
    objComment.Comment = HttpUtility.HtmlEncode(comment)
    objComment.Approved = Blog.AutoApprovePingBack
    objComment.CommentID = CommentsController.AddComment(Blog, Post, objComment)
    Return PingBackSuccess()
   End If

  End Function

  <HttpPost()>
  <AllowAnonymous()>
  <ActionName("Trackback")>
  Public Function Trackback() As HttpResponseMessage

   Dim BlogId As Integer = -1
   Dim PostId As Integer = -1
   HttpContext.Current.Request.Params.ReadValue("blogId", BlogId)
   HttpContext.Current.Request.Params.ReadValue("postId", PostId)
   Blog = BlogsController.GetBlog(BlogId, UserInfo.UserID, Threading.Thread.CurrentThread.CurrentCulture.Name)
   If Not Blog.EnableTrackBackReceive Then
    Return Request.CreateResponse(HttpStatusCode.NotFound, New With {.Result = "This blog does not accept trackbacks"})
   End If
   Post = PostsController.GetPost(PostId, ActiveModule.ModuleID, "")
   If Post Is Nothing Then
    Return TrackBackResponse("The source page does not link")
   End If

   Dim title As String = SafeString(HttpContext.Current.Request.Params("title"))
   Dim excerpt As String = SafeString(HttpContext.Current.Request.Params("excerpt"))
   Dim blogName As String = SafeString(HttpContext.Current.Request.Params("blog_name"))
   Dim sourceUrl As String = String.Empty
   If HttpContext.Current.Request.Params("url") IsNot Nothing Then
    sourceUrl = SafeString(HttpContext.Current.Request.Params("url").Split(","c)(0), DotNetNuke.Security.PortalSecurity.FilterFlag.NoSQL And DotNetNuke.Security.PortalSecurity.FilterFlag.NoScripting)
   End If

   Dim sourceHasLink As Boolean = False

   Try
    CheckSourcePage(sourceUrl, Post.PermaLink(PortalSettings), sourceHasLink, title)
   Catch ex As Exception
   End Try

   If Not IsFirstPingBack(Post, sourceUrl) Then
    Return TrackBackResponse("Trackback already registered")
   End If

   If Not sourceHasLink Then
    Return TrackBackResponse("The source page does not link")
   End If

   Dim objComment As New CommentInfo With {.ContentItemId = Post.ContentItemId, .Author = blogName, .Website = sourceUrl}
   Dim comment As String = String.Format(DotNetNuke.Services.Localization.Localization.GetString("TrackbackComment", SharedResourceFileName), blogName, sourceUrl, title)
   objComment.Comment = HttpUtility.HtmlEncode(comment)
   objComment.Approved = Blog.AutoApproveTrackBack
   objComment.CommentID = CommentsController.AddComment(Blog, Post, objComment)
   Return TrackBackResponse()

  End Function
#End Region

#Region " Private Methods "
  Private Sub SetContext(data As CommentDTO)
   Blog = BlogsController.GetBlog(data.BlogId, UserInfo.UserID, Threading.Thread.CurrentThread.CurrentCulture.Name)
   Comment = CommentsController.GetComment(data.CommentId, UserInfo.UserID)
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
       Replacers.Add(New BlogTokenReplace(ActiveModule, Security, Blog, Post, Settings, ViewSettings, c))
      Next
     Else
      For Each c As CommentInfo In AllComments.Where(Function(cmt) cmt.ParentId = -1).OrderBy(Function(cmt) cmt.CreatedOnDate)
       Replacers.Add(New BlogTokenReplace(ActiveModule, Security, Blog, Post, Settings, ViewSettings, c))
      Next
     End If

   End Select

  End Sub

  Private Shared Function RetrieveXmlDocument(context As HttpContext) As XmlDocument
   Dim xml As String = ParseRequest(context)
   If Not xml.Contains("<methodName>pingback.ping</methodName>") Then
    context.Response.StatusCode = 404
    context.Response.[End]()
   End If
   Dim doc As New XmlDocument()
   doc.LoadXml(xml)
   Return doc
  End Function

  Private Shared Function ParseRequest(context As HttpContext) As String
   Dim buffer(CInt(context.Request.InputStream.Length - 1)) As Byte
   context.Request.InputStream.Read(buffer, 0, buffer.Length)
   Return Encoding.[Default].GetString(buffer)
  End Function

  Private Shared Function IsFirstPingBack(post As PostInfo, sourceUrl As String) As Boolean
   For Each c As CommentInfo In CommentsController.GetCommentsByContentItem(post.ContentItemId, True, -1)
    If c.Website.ToString.Equals(sourceUrl, StringComparison.OrdinalIgnoreCase) Then Return False
   Next
   Return True
  End Function

  Private Shared Function TrackBackResponse() As HttpResponseMessage
   Return TrackBackResponse("0")
  End Function
  Private Shared Function TrackBackResponse(status As String) As HttpResponseMessage
   Dim reply As String = String.Format("<?xml version=""1.0"" encoding=""iso-8859-1""?><response><error>{0}</error></response>", status)
   Dim res As New HttpResponseMessage(HttpStatusCode.OK)
   res.Content = New StringContent(reply, System.Text.Encoding.UTF8, "application/xml")
   Return res
  End Function

  Private Shared Function PingBackSuccess() As HttpResponseMessage
   Dim Success As String = "<methodResponse><params><param><value><string>Thanks!</string></value></param></params></methodResponse>"
   Dim res As New HttpResponseMessage(HttpStatusCode.OK)
   res.Content = New StringContent(Success, System.Text.Encoding.UTF8, "application/xml")
   Return res
  End Function

  Private Shared Function PingBackError(code As Integer, message As String) As HttpResponseMessage
   Dim sb As New StringBuilder()
   sb.Append("<?xml version=""1.0""?>")
   sb.Append("<methodResponse>")
   sb.Append("<fault>")
   sb.Append("<value>")
   sb.Append("<struct>")
   sb.Append("<member>")
   sb.Append("<name>faultCode</name>")
   sb.AppendFormat("<value><int>{0}</int></value>", code)
   sb.Append("</member>")
   sb.Append("<member>")
   sb.Append("<name>faultString</name>")
   sb.AppendFormat("<value><string>{0}</string></value>", message)
   sb.Append("</member>")
   sb.Append("</struct>")
   sb.Append("</value>")
   sb.Append("</fault>")
   sb.Append("</methodResponse>")
   Dim res As New HttpResponseMessage(HttpStatusCode.OK)
   res.Content = New StringContent(sb.ToString, System.Text.Encoding.UTF8, "application/xml")
   Return res
  End Function

  Private Shared Function GetDomain(sourceUrl As String) As String
   Dim start As Integer = sourceUrl.IndexOf("://") + 3
   Dim [stop] As Integer = sourceUrl.IndexOf("/", start)
   Return sourceUrl.Substring(start, [stop] - start).Replace("www.", String.Empty)
  End Function

  Private Shared Sub CheckSourcePage(sourceUrl As String, targetUrl As String, ByRef sourceContainsLink As Boolean, ByRef title As String)

   Dim remoteFile As New WebPage(New Uri(sourceUrl))
   Dim html As String = remoteFile.GetFileAsString()
   Dim RegexTitle As New Regex("(?<=<title.*>)([\s\S]*)(?=</title>)", RegexOptions.IgnoreCase Or RegexOptions.Compiled)
   Dim titleMatch As Match = RegexTitle.Match(html)
   If titleMatch.Success Then title = SafeString(titleMatch.Value.Trim(CChar(vbCrLf)).Trim())
   html = html.ToUpperInvariant
   targetUrl = targetUrl.ToUpperInvariant
   sourceContainsLink = html.Contains("HREF=""" & targetUrl & """") OrElse html.Contains("HREF='" & targetUrl & "'")

  End Sub
#End Region

 End Class
End Namespace

