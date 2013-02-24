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
#End Region

#Region " Event Handlers "
  Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
   ViewState.ReadValue("SelectedCommentId", SelectedCommentId)
  End Sub

  Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

   If Not Me.IsPostBack Then
    BindCommentsList()
   End If

  End Sub

  Private Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
   ViewState("SelectedCommentId") = SelectedCommentId.ToString
  End Sub

  Protected Sub cmdAddComment_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAddComment.Click
   Try
    valComment.Enabled = True

    If (txtComment.Text.Length > 0) And (ModuleContext.PortalSettings.UserId > 0) Then
     Dim objComment As New CommentInfo

     If SelectedCommentId > -1 Then
      objComment = CommentsController.GetComment(SelectedCommentId)
      SelectedCommentId = -1
     Else
      objComment = New CommentInfo
      objComment.ContentItemId = Entry.ContentItemId
      objComment.UserID = Me.UserId
     End If

     objComment.Comment = Server.HtmlEncode((New PortalSecurity).InputFilter(txtComment.Text, PortalSecurity.FilterFlag.NoProfanity))
     objComment.Approved = Security.CanApproveComment

     If objComment.CommentID > -1 Then
      CommentsController.UpdateComment(objComment)
      If objComment.Approved Then
       JournalController.AddCommentToJournal(Entry, objComment, ModuleContext.PortalId, ModuleContext.TabId, objComment.UserID, Entry.PermaLink(PortalSettings))
      End If
     Else
      objComment.CommentID = CommentsController.AddComment(objComment)
      If objComment.Approved Then
       JournalController.AddCommentToJournal(Entry, objComment, ModuleContext.PortalId, ModuleContext.TabId, ModuleContext.PortalSettings.UserId, Entry.PermaLink(PortalSettings))
       If Not Security.IsOwner Then
        Dim title As String = Localization.GetString("CommentAddedNotify", Common.Constants.SharedResourceFileName)
        Dim summary As String = "<a target='_blank' href='" + Entry.PermaLink(PortalSettings) + "'>" + Entry.Title + "</a>"
        NotificationController.CommentAdded(objComment, Entry, Blog, ModuleContext.PortalId, summary, title)
       End If
      Else
       Dim title As String = Localization.GetString("CommentPendingNotify", Common.Constants.SharedResourceFileName)
       Dim summary As String = "<a target='_blank' href='" + Entry.PermaLink(PortalSettings) + "'>" + Entry.Title + "</a><br />" + objComment.Comment
       NotificationController.CommentPendingApproval(objComment, Blog, Entry, ModuleContext.PortalId, summary, title)
       UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("CommentPendingApproval", LocalResourceFile), ModuleMessage.ModuleMessageType.BlueInfo)
      End If
     End If
     txtComment.Text = String.Empty
     BindCommentsList()
    End If
   Catch exc As Exception
    LogException(exc)
   End Try
  End Sub

  Protected Sub lstComments_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataListItemEventArgs) Handles lstComments.ItemDataBound
   If (e.Item.ItemType = ListItemType.AlternatingItem) Or (e.Item.ItemType = ListItemType.Item) Then
    If (e.Item.DataItem Is Nothing) Or (Blog Is Nothing) Then
     Return
    End If

    Dim imgUser As System.Web.UI.WebControls.Image = CType(e.Item.FindControl("imgUser"), System.Web.UI.WebControls.Image)
    Dim hlUser As System.Web.UI.WebControls.HyperLink = CType(e.Item.FindControl("hlUser"), System.Web.UI.WebControls.HyperLink)
    Dim hlCommentAuthor As System.Web.UI.WebControls.HyperLink = CType(e.Item.FindControl("hlCommentAuthor"), System.Web.UI.WebControls.HyperLink)
    Dim lnkEditComment As System.Web.UI.WebControls.ImageButton = CType(e.Item.FindControl("lnkEditComment"), System.Web.UI.WebControls.ImageButton)
    Dim lnkApproveComment As System.Web.UI.WebControls.ImageButton = CType(e.Item.FindControl("lnkApproveComment"), System.Web.UI.WebControls.ImageButton)
    Dim lblCommentDate As System.Web.UI.WebControls.Label = CType(e.Item.FindControl("lblCommentDate"), System.Web.UI.WebControls.Label)
    Dim lnkDeleteComment As System.Web.UI.WebControls.ImageButton = CType(e.Item.FindControl("lnkDeleteComment"), System.Web.UI.WebControls.ImageButton)
    Dim litComment As System.Web.UI.WebControls.Literal = CType(e.Item.FindControl("txtCommentBody"), Literal)

    Dim objComment As CommentInfo = CType(e.Item.DataItem, CommentInfo)

    lnkEditComment.Visible = Security.CanApproveEntry Or Security.CanEditEntry
    lnkDeleteComment.Visible = lnkEditComment.Visible

    Dim objUser As UserInfo = UserController.GetUserById(ModuleContext.PortalId, objComment.UserID)

    If objUser IsNot Nothing Then
     hlUser.NavigateUrl = DotNetNuke.Common.Globals.UserProfileURL(objUser.UserID)
     imgUser.ImageUrl = Control.ResolveUrl("~/profilepic.ashx?userid=" + objComment.UserID.ToString + "&w=" + "50" + "&h=" + "50")
     hlCommentAuthor.Text = objUser.DisplayName
     hlCommentAuthor.NavigateUrl = DotNetNuke.Common.Globals.UserProfileURL(objUser.UserID)
    Else
     hlUser.NavigateUrl = DotNetNuke.Common.Globals.UserProfileURL(-1)
     imgUser.ImageUrl = Control.ResolveUrl("~/profilepic.ashx?userid=-1&w=" + "50" + "&h=" + "50")
     hlCommentAuthor.Text = Localization.GetString("Anonymous", LocalResourceFile)
     hlCommentAuthor.NavigateUrl = DotNetNuke.Common.Globals.UserProfileURL(-1)
    End If

    lblCommentDate.Text = Common.Globals.CalculateDateForDisplay(objComment.AddedDate)
    Dim comment As String = objComment.Comment
    Dim matches As MatchCollection = New Regex("(?<!["">])((http|https|ftp)\://.+?)(?=\s|$)").Matches(comment)

    For Each m As Match In matches
     comment = comment.Replace(m.Value, "<a rel=""nofollow"" href=""" + m.Value + """>" + m.Value + "</a>")
    Next
    comment.Replace(System.Environment.NewLine, "<br />")
    litComment.Text = comment

    If Not objComment.Approved Then
     lnkApproveComment.Visible = True
    Else
     lnkApproveComment.Visible = False
    End If
   End If
  End Sub

  Protected Sub lstComments_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataListCommandEventArgs) Handles lstComments.ItemCommand
   Select Case e.CommandName.ToLower
    Case "editcomment"
     valComment.Enabled = False
     Dim oComment As CommentInfo = CommentsController.GetComment(Int32.Parse(CType(e.CommandArgument, String)))
     If Not oComment Is Nothing Then
      txtComment.Text = Common.Globals.removeHtmlTags(Server.HtmlDecode(oComment.Comment))
      SelectedCommentId = oComment.CommentID
      cmdAddComment.Text = Localization.GetString("msgUpdateComment", LocalResourceFile)
      cmdDeleteComment.Visible = True
      NotificationController.RemoveCommentPendingNotification(Blog.BlogID, oComment.ContentItemId, oComment.CommentID)
     End If
    Case "approvecomment"
     Dim oComment As CommentInfo = CommentsController.GetComment(Int32.Parse(CType(e.CommandArgument, String)))
     oComment.Approved = True
     CommentsController.UpdateComment(oComment)
     NotificationController.RemoveCommentPendingNotification(Blog.BlogID, oComment.ContentItemId, oComment.CommentID)
     BindCommentsList()
    Case "deletecomment"
     CommentsController.DeleteComment(Int32.Parse(CType(e.CommandArgument, String)))
     NotificationController.RemoveCommentPendingNotification(Blog.BlogID, Entry.ContentItemId, Int32.Parse(CType(e.CommandArgument, String)))
     BindCommentsList()
   End Select
  End Sub

  Protected Sub cmdDeleteComment_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDeleteComment.Click
   If SelectedCommentId > -1 Then
    CommentsController.DeleteComment(SelectedCommentId)
    SelectedCommentId = -1
    BindCommentsList()
   End If
  End Sub
#End Region

#Region " Private Methods "
  Private Sub BindCommentsList()
   If Entry Is Nothing Then
    pnlAddComment.Visible = False
    pnlComments.Visible = False
   Else
    lstComments.DataSource = CommentsController.GetCommentsByContentItem(Entry.ContentItemId, Security.CanApproveEntry)
    lstComments.DataBind()
    lblComments.Text = String.Format(Localization.GetString("lblComments", LocalResourceFile), lstComments.Items.Count)
    txtComment.Text = ""
    cmdAddComment.Text = Localization.GetString("msgAddComment", LocalResourceFile)
    cmdDeleteComment.Visible = False
   End If
  End Sub
#End Region

 End Class
End Namespace
