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

Imports System.Linq

Imports DotNetNuke.Modules.Blog.Templating
Imports DotNetNuke.Modules.Blog.Entities.Comments

Namespace Controls
 Public Class Comments
  Inherits BlogModuleBase

#Region " Properties "
  Private Property AllComments As New List(Of CommentInfo)
#End Region

#Region " Event Handlers "
  Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

   LocalResourceFile = "~/DesktopModules/Blog/Controls/App_LocalResources/Comments.ascx.resx"
   AddJavascriptFile("jquery.timeago.js", 59)
   ' localized js files?
   Dim locale As String = Threading.Thread.CurrentThread.CurrentCulture.Name.ToLower
   If IO.Directory.Exists(BlogModuleMapPath & "js\" & locale) Then
    For Each f As IO.FileInfo In (New IO.DirectoryInfo(BlogModuleMapPath & "js\" & locale)).GetFiles("*.js")
     AddJavascriptFile(locale & "/" & f.Name, 60)
    Next
   Else ' check generic culture
    locale = Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower
    If IO.Directory.Exists(BlogModuleMapPath & "js\" & locale) Then
     For Each f As IO.FileInfo In (New IO.DirectoryInfo(BlogModuleMapPath & "js\" & locale)).GetFiles("*.js")
      AddJavascriptFile(locale & "/" & f.Name, 60)
     Next
    End If
   End If
   If BlogContext.Post IsNot Nothing AndAlso BlogContext.Security.CanViewComments AndAlso BlogContext.Post.AllowComments Then
    AllComments = CommentsController.GetCommentsByContentItem(BlogContext.Post.ContentItemId, BlogContext.Security.CanApproveComment, UserId)
    DataBind()
   End If

  End Sub
#End Region

#Region " Template Data Retrieval "
  Private Sub vtContents_GetData(ByVal DataSource As String, ByVal Parameters As Dictionary(Of String, String), ByRef Replacers As System.Collections.Generic.List(Of GenericTokenReplace), ByRef Arguments As System.Collections.Generic.List(Of String()), callingObject As Object) Handles vtContents.GetData

   Select Case DataSource.ToLower

    Case "comments"

     If callingObject IsNot Nothing AndAlso TypeOf callingObject Is CommentInfo Then
      Dim parent As Integer = CType(callingObject, CommentInfo).CommentID
      For Each c As CommentInfo In AllComments.Where(Function(cmt) cmt.ParentId = parent).OrderBy(Function(cmt) cmt.CreatedOnDate)
       Replacers.Add(New BlogTokenReplace(Me, BlogContext.Post, c))
      Next
     Else
      For Each c As CommentInfo In AllComments.Where(Function(cmt) cmt.ParentId = -1).OrderBy(Function(cmt) cmt.CreatedOnDate)
       Replacers.Add(New BlogTokenReplace(Me, BlogContext.Post, c))
      Next
     End If

   End Select

  End Sub
#End Region

#Region " Overrides "
  Public Overrides Sub DataBind()

   Dim tmgr As New TemplateManager(PortalSettings, ViewSettings.Template)
   With vtContents
    .TemplatePath = tmgr.TemplatePath
    .TemplateRelPath = tmgr.TemplateRelPath
    .TemplateMapPath = tmgr.TemplateMapPath
    .DefaultReplacer = New BlogTokenReplace(Me)
    .StartTemplate = "CommentsTemplate.html"
   End With
   vtContents.DataBind()

  End Sub
#End Region

 End Class
End Namespace
