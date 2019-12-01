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
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports DotNetNuke.Modules.Blog.Entities.Posts
Imports DotNetNuke.Modules.Blog.Entities.Comments
Imports DotNetNuke.Modules.Blog.BlogML.Xml
Imports DotNetNuke.Modules.Blog.Entities.Terms
Imports System.Xml
Imports DotNetNuke.Modules.Blog.Services
Imports ICSharpCode.SharpZipLib.Zip

Namespace Entities.Blogs
 Partial Public Class BlogsController
  Inherits DnnApiController

  Public Class BlogDTO
   Public Property BlogId As Integer
  End Class

#Region " Private Members "
  Private Property Blog As BlogInfo = Nothing
  Private Property Settings As ModuleSettings = Nothing
#End Region

#Region " Service Methods "
  <HttpPost()>
  <BlogAuthorizeAttribute(Services.SecurityAccessLevel.Owner Or SecurityAccessLevel.Admin)>
  <ValidateAntiForgeryToken()>
  <ActionName("Export")>
  Public Function ExportBlog(postData As BlogDTO) As HttpResponseMessage
   SetContext(postData)

   Dim saveDir As New IO.DirectoryInfo(GetBlogDirectoryMapPath(Blog.BlogID))
   If Not saveDir.Exists Then saveDir.Create()
   RemoveOldTimeStampedFiles(saveDir)

   Dim newBlogML As New BlogMLBlog
   newBlogML.Title = Blog.Title
   newBlogML.SubTitle = Blog.Description
   newBlogML.Authors.Add(New BlogMLAuthor With {.Title = Blog.DisplayName})
   newBlogML.DateCreated = Blog.CreatedOnDate
   AddCategories(newBlogML)
   AddPosts(newBlogML)
   Dim blogMLFile As String = Date.Now.ToString("yyyy-MM-dd") & "-" & Guid.NewGuid.ToString("D")
   Dim objZipOutputStream As New ZipOutputStream(IO.File.Create(GetBlogDirectoryMapPath(Blog.BlogID) & blogMLFile & ".zip"))
   Dim objZipEntry As ZipEntry = New ZipEntry(blogMLFile & ".xml")
   objZipOutputStream.PutNextEntry(objZipEntry)
   objZipOutputStream.SetLevel(9)
   Using stream As XmlWriter = XmlWriter.Create(objZipOutputStream)
    BlogMLSerializer.Serialize(stream, newBlogML)
    stream.Flush()
   End Using
   objZipOutputStream.Finish()
   objZipOutputStream.Close()

   Return Request.CreateResponse(HttpStatusCode.OK, New With {.Result = GetBlogDirectoryPath(Blog.BlogID) & blogMLFile & ".zip"})
  End Function
#End Region

#Region " Private Methods "
  Private Sub SetContext(data As BlogDTO)
   Blog = BlogsController.GetBlog(data.BlogId, UserInfo.UserID, Threading.Thread.CurrentThread.CurrentCulture.Name)
   Settings = ModuleSettings.GetModuleSettings(ActiveModule.ModuleID)
  End Sub

  Private Sub AddCategories(ByRef TargetBlogML As BlogMLBlog)
   If Settings.VocabularyId > -1 Then
    For Each c As TermInfo In Entities.Terms.TermsController.GetTermsByVocabulary(ActiveModule.ModuleID, Settings.VocabularyId, Threading.Thread.CurrentThread.CurrentCulture.Name).Values
     Dim categoryML As New BlogMLCategory
     categoryML.Approved = True
     categoryML.DateCreated = c.CreatedOnDate
     categoryML.Description = ""
     categoryML.Title = c.Name
     categoryML.ID = c.TermId.ToString
     categoryML.ParentRef = c.ParentTermId.ToStringOrZero
     TargetBlogML.Categories.Add(categoryML)
    Next
   End If
  End Sub

  Private Sub AddPosts(ByRef TargetBlogML As BlogMLBlog)
   Dim totalRecs As Integer = 0
   Dim handledRecs As Integer = 0
   Dim page As Integer = 0
   Do
    For Each post As PostInfo In PostsController.GetPostsByBlog(ActiveModule.ModuleID, Blog.BlogID, Threading.Thread.CurrentThread.CurrentCulture.Name, -1, page, 10, "PUBLISHEDONDATE", totalRecs).Values
     handledRecs += 1
     TargetBlogML.Posts.Add(ConvertPost(post))
    Next
    page += 1
   Loop While totalRecs > handledRecs
  End Sub

  Private Function ConvertPost(post As PostInfo) As BlogMLPost

   Dim newPostML As New BlogMLPost
   newPostML.Approved = post.Published
   newPostML.Title = post.Title
   newPostML.Content = BlogMLContent.Create(HttpUtility.HtmlDecode(post.Content), BlogML.ContentTypes.Html)
   For Each t As TermInfo In post.PostCategories
    newPostML.Categories.Add(New BlogMLCategoryReference With {.Ref = t.TermId.ToString})
   Next
   newPostML.Authors.Add(post.DisplayName)
   newPostML.PostType = BlogML.BlogPostTypes.Normal
   newPostML.DateCreated = post.PublishedOnDate
   If Not String.IsNullOrEmpty(post.Summary) Then
    If Settings.SummaryModel = SummaryType.PlainTextIndependent Then
     newPostML.Excerpt = BlogMLContent.Create(post.Summary, BlogML.ContentTypes.Text)
    Else
     newPostML.Excerpt = BlogMLContent.Create(HttpUtility.HtmlDecode(post.Summary), BlogML.ContentTypes.Html)
    End If
    newPostML.HasExcerpt = True
   Else
    newPostML.HasExcerpt = False
   End If
   newPostML.ID = post.ContentItemId.ToString
   newPostML.PostUrl = post.PermaLink
   newPostML.Image = post.Image
   newPostML.AllowComments = post.AllowComments
   newPostML.DisplayCopyright = post.DisplayCopyright
   newPostML.Copyright = post.Copyright
   newPostML.Locale = post.Locale

   ' pack files
   Dim postDir As String = GetPostDirectoryMapPath(post.BlogID, post.ContentItemId)
   If IO.Directory.Exists(postDir) Then
    For Each f As String In IO.Directory.GetFiles(postDir)
     Dim fileName As String = IO.Path.GetFileName(f)
     Dim regexPattern As String = "&quot;([^\s]*)\/" & fileName & "&quot;"
     Dim options As RegexOptions = RegexOptions.Singleline Or RegexOptions.IgnoreCase
     If newPostML.HasExcerpt Then
      newPostML.Excerpt.Text = Regex.Replace(newPostML.Excerpt.Text, regexPattern, "&quot;" & fileName & "&quot;", options)
     End If
     newPostML.Content.Text = Regex.Replace(newPostML.Content.Text, regexPattern, "&quot;" & fileName & "&quot;", options)
     Dim att As New BlogMLAttachment With {.Embedded = True, .Path = fileName}
     Using fs As New IO.FileStream(f, IO.FileMode.Open)
      Dim fileData(CInt(fs.Length - 1)) As Byte
      If fs.Length > 0 Then
       fs.Read(fileData, 0, CInt(fs.Length))
       att.Data = fileData
       att.Embedded = True
      Else
       'Empty File
      End If
     End Using
     newPostML.Attachments.Add(att)
    Next
   End If

   For Each comment As CommentInfo In CommentsController.GetCommentsByContentItem(post.ContentItemId, False, UserInfo.UserID)
    Dim newComment As New BlogMLComment
    newComment.Approved = comment.Approved
    newComment.Content = New BlogMLContent()
    newComment.Content.Text = comment.Comment
    newComment.DateCreated = comment.CreatedOnDate
    newComment.Title = ""
    newComment.UserEMail = comment.Email
    newComment.UserName = comment.Username
    newComment.UserUrl = comment.Website
    newPostML.Comments.Add(newComment)
   Next

   Return newPostML

  End Function
#End Region

 End Class
End Namespace
