Imports System.Linq

Imports DotNetNuke.Modules.Blog.Templating
Imports DotNetNuke.Modules.Blog.Entities.Comments

Namespace Controls
 Public Class Comments
  Inherits BlogModuleBase

#Region " Properties "
  Private Property AllComments As New List(Of CommentInfo)
  Private fulllocs As List(Of String) = {"pt-br", "zh-cn", "zh-tw"}.ToList
  Private twoletterlocs As List(Of String) = {"ar", "bg", "bs", "ca", "cy", "cz", "da", "de", "el", "en", "es", "fa", "fi", "fr", "he", "hr", "hu", "hy", "id", "it", "ja", "ko", "mk", "nl", "no", "pl", "pt", "ro", "ru", "sk", "sv", "th", "tr", "uk", "uz"}.ToList
#End Region

#Region " Event Handlers "
  Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

   LocalResourceFile = "~/DesktopModules/Blog/Controls/App_LocalResources/Comments.ascx.resx"
   AddJavascriptFile("jquery.timeago.js", 60)
   If fulllocs.Contains(BlogContext.Locale.ToLower) Then
    AddJavascriptFile("time-ago-locales/jquery.timeago." & BlogContext.Locale.ToLower & ".js", 60)
   ElseIf twoletterlocs.Contains(Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower) Then
    AddJavascriptFile("time-ago-locales/jquery.timeago." & Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower & ".js", 61)
   End If
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
