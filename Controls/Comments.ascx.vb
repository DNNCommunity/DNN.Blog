Imports System.Linq

Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Modules.Blog.Entities.Comments
Imports DotNetNuke.Modules.Blog.Integration
Imports DotNetNuke.Security
Imports DotNetNuke.UI.Skins.Controls

Namespace Controls
 Public Class Comments
  Inherits BlogContextBase

#Region " Private Properties "
  Private Property SelectedCommentId As Integer = -1
  Private fulllocs As List(Of String) = {"pt-br", "zh-cn", "zh-tw"}.ToList
  Private twoletterlocs As List(Of String) = {"ar", "bg", "bs", "ca", "cy", "cz", "da", "de", "el", "en", "es", "fa", "fi", "fr", "he", "hr", "hu", "hy", "id", "it", "ja", "ko", "mk", "nl", "no", "pl", "pt", "ro", "ru", "sk", "sv", "th", "tr", "uk", "uz"}.ToList
#End Region

#Region " Event Handlers "
  Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init

   ViewState.ReadValue("SelectedCommentId", SelectedCommentId)

   CType(Me.Parent, BlogModuleBase).AddBlogService()

   If fulllocs.Contains(Locale.ToLower) Then
    Web.Client.ClientResourceManagement.ClientResourceManager.RegisterScript(Page, TemplateSourceDirectory + "/../js/time-ago-locales/jquery.timeago." & Locale.ToLower & ".js")
   ElseIf twoletterlocs.Contains(Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower) Then
    Web.Client.ClientResourceManagement.ClientResourceManager.RegisterScript(Page, TemplateSourceDirectory + "/../js/time-ago-locales/jquery.timeago." & Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower & ".js")
   End If
   Web.Client.ClientResourceManagement.ClientResourceManager.RegisterScript(Page, TemplateSourceDirectory + "/../js/jquery.timeago.js")

  End Sub

  Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
   LocalResourceFile = "~/DesktopModules/Blog/Controls/App_LocalResources/Comments.ascx.resx"
   If Not Me.IsPostBack Then
    BindCommentsList()
   End If
   pnlAddComment.Visible = Security.CanAddComment And Post IsNot Nothing
  End Sub

  Private Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
   ViewState("SelectedCommentId") = SelectedCommentId.ToString
  End Sub

  Protected Sub cmdAddComment_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAddComment.Click
   Try
    valComment.Enabled = True

    If Security.CanAddComment And (txtComment.Text.Length > 0) Then
     Dim objComment As New CommentInfo

     If SelectedCommentId > -1 Then
      objComment = CommentsController.GetComment(SelectedCommentId)
      SelectedCommentId = -1
     Else
      objComment = New CommentInfo
      objComment.ContentItemId = Post.ContentItemId
      objComment.CreatedByUserID = Me.UserId
     End If

     objComment.Comment = Server.HtmlEncode((New PortalSecurity).InputFilter(txtComment.Text, PortalSecurity.FilterFlag.NoProfanity))
     objComment.Approved = Security.CanApproveComment

     If objComment.CommentID > -1 Then
      CommentsController.UpdateComment(Blog, Post, objComment)
     Else
      objComment.CommentID = CommentsController.AddComment(Blog, Post, objComment)
      If Not objComment.Approved Then
       UI.Skins.Skin.AddModuleMessage(Me, LocalizeString("CommentPendingApproval"), ModuleMessage.ModuleMessageType.BlueInfo)
      End If
     End If
     txtComment.Text = String.Empty
     BindCommentsList()
    End If
   Catch exc As Exception
    LogException(exc)
   End Try
  End Sub

  Protected Sub lstComments_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataListCommandEventArgs) Handles lstComments.ItemCommand
   Select Case e.CommandName.ToLower
    Case "editcomment"
     valComment.Enabled = False
     Dim oComment As CommentInfo = CommentsController.GetComment(Int32.Parse(CType(e.CommandArgument, String)))
     If Not oComment Is Nothing Then
      txtComment.Text = Common.Globals.RemoveHtmlTags(Server.HtmlDecode(oComment.Comment))
      SelectedCommentId = oComment.CommentID
      cmdAddComment.Text = LocalizeString("msgUpdateComment")
      cmdDeleteComment.Visible = True
     End If
   End Select
  End Sub

  Protected Sub cmdDeleteComment_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDeleteComment.Click
   Dim comment As CommentInfo = CommentsController.GetComment(SelectedCommentId)
   If SelectedCommentId > -1 Then
    If Security.CanEditPost Or Security.CanApproveComment Or comment.CreatedByUserID = UserId Then
     CommentsController.DeleteComment(Settings.ModuleId, Blog.BlogID, comment)
    End If
    SelectedCommentId = -1
    BindCommentsList()
   End If
  End Sub
#End Region

#Region " Private Methods "
  Private Sub BindCommentsList()
   If Post Is Nothing Then
    pnlAddComment.Visible = False
    pnlComments.Visible = False
   Else
    lstComments.DataSource = CommentsController.GetCommentsByContentItem(Post.ContentItemId, Security.CanApprovePost)
    lstComments.DataBind()
    txtComment.Text = ""
    lblComments.Text = String.Format(LocalizeString("lblComments"), lstComments.Items.Count)
    cmdAddComment.Text = LocalizeString("msgAddComment")
    cmdDeleteComment.Visible = False
   End If
  End Sub
#End Region

 End Class
End Namespace
