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
   If BlogContext.Post IsNot Nothing AndAlso BlogContext.Security.CanViewComments AndAlso BlogContext.Post.AllowComments Then
    AllComments = CommentsController.GetCommentsByContentItem(BlogContext.Post.ContentItemId, BlogContext.Security.CanApproveComment)
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
    .TemplateMapPath = tmgr.TemplateMapPath
    .DefaultReplacer = New BlogTokenReplace(Me)
    .StartTemplate = "CommentsTemplate.html"
   End With
   vtContents.DataBind()

  End Sub
#End Region

 End Class
End Namespace
