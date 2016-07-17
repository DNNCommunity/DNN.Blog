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
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports DotNetNuke.Modules.Blog.BlogML.Xml
Imports DotNetNuke.Modules.Blog.Entities.Posts
Imports ICSharpCode.SharpZipLib.Zip

Public Class BlogImport
 Inherits BlogModuleBase

 Private Property CanImportCategories As Boolean = False

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

  If Not BlogContext.Security.IsBlogger Then
   Throw New Exception("You do not have access to this resource. Please check your login status.")
  End If

  If Settings.VocabularyId > -1 AndAlso (BlogContext.Security.IsEditor) Then CanImportCategories = True

  If Not Me.IsPostBack Then
   lblTargetName.Text = BlogContext.Blog.Title
   chkImportCategories.Enabled = CanImportCategories
  End If

 End Sub

 Protected Function GetText(type As String) As String
  Dim text As String = Null.NullString
  Dim pageName As String = wizBlogImport.ActiveStep.Title
  If type = "Title" Then
   text = Localization.GetString(pageName + ".Title", LocalResourceFile)
  ElseIf type = "Help" Then
   text = Localization.GetString(pageName + ".Help", LocalResourceFile)
  End If
  Return text
 End Function

 Private Sub wizBlogImport_ActiveStepChanged(sender As Object, e As System.EventArgs) Handles wizBlogImport.ActiveStepChanged

 End Sub

 Private Sub wizBlogImport_CancelButtonClick(sender As Object, e As System.EventArgs) Handles wizBlogImport.CancelButtonClick
  Response.Redirect(EditUrl("Manage"), False)
 End Sub

 Private Sub wizBlogImport_NextButtonClick(sender As Object, e As System.Web.UI.WebControls.WizardNavigationEventArgs) Handles wizBlogImport.NextButtonClick
  Select Case e.CurrentStepIndex
   Case 0 ' upload
    Dim strReport As New StringBuilder
    Dim file As HttpPostedFile = cmdBrowse.PostedFile
    If file.FileName <> "" Then
     If file.FileName.ToLower.EndsWith(".zip") Then
      Dim objZipInputStream As New ZipInputStream(file.InputStream)
      Dim objZipEntry As ZipEntry = objZipInputStream.GetNextEntry
      If objZipEntry.Name.ToLower.EndsWith(".xml") Then
       Using objFileStream As IO.FileStream = IO.File.Create(BlogContext.BlogMapPath & "import.resources")
        Dim intSize As Integer = 2048
        Dim arrData(2048) As Byte
        intSize = objZipInputStream.Read(arrData, 0, arrData.Length)
        While intSize > 0
         objFileStream.Write(arrData, 0, intSize)
         intSize = objZipInputStream.Read(arrData, 0, arrData.Length)
        End While
       End Using
      End If
      objZipInputStream.Close()
     Else
      file.SaveAs(BlogContext.BlogMapPath & "import.resources")
     End If
     strReport.AppendLine("Saved File")
     Dim blog As BlogMLBlog = Nothing
     Using strIn As New IO.StreamReader(BlogContext.BlogMapPath & "import.resources")
      Using xmlIn As New System.Xml.XmlTextReader(strIn)
       blog = BlogMLSerializer.Deserialize(xmlIn)
      End Using
     End Using
     If blog IsNot Nothing Then
      strReport.AppendLine("File is a valid BlogML file")
      strReport.AppendFormat("Original Title: {0}" & vbCrLf, blog.Title)
      strReport.AppendFormat("Total Posts: {0}" & vbCrLf, blog.Posts.Count)
      strReport.AppendFormat("Total Categories: {0}" & vbCrLf, blog.Categories.Count)
     End If
     txtAnalysis.Text = strReport.ToString
    Else ' no file uploaded

    End If
   Case 1 ' analysis and selecting options
    Dim strReport As New StringBuilder
    Dim b As BlogMLBlog = Nothing
    Using strIn As New IO.StreamReader(BlogContext.BlogMapPath & "import.resources")
     Using xmlIn As New System.Xml.XmlTextReader(strIn)
      b = BlogMLSerializer.Deserialize(xmlIn)
     End Using
    End Using
    If b IsNot Nothing Then
     If CanImportCategories AndAlso chkImportCategories.Checked Then
      For Each c As BlogML.Xml.BlogMLCategory In b.Categories
       ' check for and add category
      Next
     ElseIf chkImportMissingCategoriesAsKeywords.Checked Then
      For Each c As BlogML.Xml.BlogMLCategory In b.Categories
       ' check for and add tag
      Next
     End If
     For Each post As BlogML.Xml.BlogMLPost In b.Posts
      ' import post
      Dim newPost As New PostInfo
      With newPost
       .BlogID = BlogContext.BlogId
       '.AllowComments = Blog.AllowComments
       .ViewCount = 0
       .Title = post.Title
       .Content = post.Content.Text
       .Summary = post.Excerpt.Text
       .Published = post.Approved
       .PublishedOnDate = post.DateCreated
      End With
      If newPost.Title <> "" And newPost.Content <> "" Then
       newPost.ContentItemId = PostsController.AddPost(newPost, UserId)
       strReport.AppendFormat("Added {0}" & vbCrLf, post.Title)
       ' import resources
       If post.Attachments.Count > 0 Then
        Dim postDir As String = GetPostDirectoryMapPath(newPost)
        Dim postPath As String = GetPostDirectoryPath(newPost)
        IO.Directory.CreateDirectory(postDir)
        For Each att As BlogMLAttachment In post.Attachments
         If att.Embedded And att.Data IsNot Nothing Then
          Dim filename As String = att.Path
          If filename = "" Then filename = att.Url
          filename = filename.Replace("/", "\")
          If filename.IndexOf("\") > 0 Then filename = filename.Substring(filename.LastIndexOf("\") + 1)
          IO.File.WriteAllBytes(postDir & filename, att.Data)
          newPost.Content = newPost.Content.Replace(filename, postPath & filename)
          If Not String.IsNullOrEmpty(newPost.Summary) Then newPost.Summary = newPost.Summary.Replace(filename, postPath & filename)
         End If
        Next
       End If
       PostsController.UpdatePost(newPost, UserId)
      End If
     Next
     txtReport.Text = strReport.ToString
    End If
   Case 2 ' report
    Try
     IO.File.Delete(BlogContext.BlogMapPath & "import.resources")
    Catch ex As Exception
    End Try
    Response.Redirect(EditUrl("Manage"), False)
  End Select
 End Sub
End Class