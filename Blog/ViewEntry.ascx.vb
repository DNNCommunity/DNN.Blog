'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2012
' by DotNetNuke Corporation
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

Imports System
Imports DotNetNuke.Modules.Blog.Components.Business
Imports DotNetNuke.Modules.Blog.Components.Controllers
Imports DotNetNuke.Modules.Blog.Components.Common
Imports DotNetNuke.Entities.Content
Imports DotNetNuke.Web.Client.ClientResourceManagement
Imports DotNetNuke.Security
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Framework
Imports DotNetNuke.Modules.Blog.Components.Integration
Imports DotNetNuke.Modules.Blog.Components.Entities

Partial Public Class ViewEntry
 Inherits BlogModuleBase

#Region "Private Members"


 Private objBlog As BlogInfo
 Private objParentBlog As BlogInfo
 Private cntEntry As New EntryController
 'Private objEntry As EntryInfo

 Private ReadOnly Property VocabularyId() As Integer
  Get
   Return BlogSettings.VocabularyId
  End Get
 End Property

 Private ReadOnly Property EntryId() As Integer
  Get
   Dim entry As Integer = -1

   If Not (Request.Params("EntryID") Is Nothing) Then
    entry = Int32.Parse(Request.Params("EntryID"))
   End If

   Return entry
  End Get
 End Property

 Private ReadOnly Property Entry() As EntryInfo
  Get
   If EntryId > 0 Then
    Dim cntBlog As New BlogController
    Return cntEntry.GetEntry(EntryId, ModuleContext.PortalId)
   Else
    Return Nothing
   End If
  End Get
 End Property

#End Region

#Region "Event Handlers"

 Protected Overloads Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
  ScriptRegistration()
  OutputAdditionalFiles = True
  ModuleConfiguration.DisplayPrint = False

  If Not Entry Is Nothing Then
   Dim cntBlog As New BlogController
   objBlog = cntBlog.GetBlog(Entry.BlogID)
   Dim isOwner As Boolean = objBlog.UserID = ModuleContext.PortalSettings.UserId
   Dim objSecurity As New ModuleSecurity(ModuleContext.ModuleId, ModuleContext.TabId)

   If (Not objBlog.Public And Not objSecurity.CanAddEntry(isOwner, objBlog.AuthorMode)) Then
    Response.Redirect(NavigateURL("Unauthorized Access"), True)
   End If

   If objSecurity.CanAddEntry(isOwner, objBlog.AuthorMode) Then
    lnkEditEntry.Visible = True
    lnkEditEntry.NavigateUrl = Links.EditEntry(ModuleContext, objBlog.BlogID, Entry.EntryID)
   Else
    lnkEditEntry.Visible = False
   End If

   Dim keyCount As Integer = 1
   Dim count As Integer = keyCount
   Dim pageTitle As String = Entry.Title
   Dim keyWords As String = ""
   Dim pageDescription As String = Entry.Entry
   Dim pageUrl As String = Entry.PermaLink
   Dim pageAuthor As String = objBlog.UserFullName

   ' needs to be integrated w/ keyword limit constant
   For Each term As DotNetNuke.Entities.Content.Taxonomy.Term In Entry.Terms
    keyWords += "," + term.Name
    keyCount += 1
   Next

   Utility.SetPageMetaAndOpenGraph(CType(Page, CDefault), ModuleContext, pageTitle, pageDescription, keyWords, pageUrl)
  Else
   Response.Redirect(NavigateURL("Unauthorized Access"), True)
  End If
 End Sub

 Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  Try
   If Not Page.IsPostBack Then
    Dim objSecurity As ModuleSecurity = New ModuleSecurity(ModuleContext.ModuleId, ModuleContext.TabId)

    If Entry Is Nothing Then
     Response.Redirect(NavigateURL(), False)
     Exit Sub
    End If

    Dim isOwner As Boolean
    isOwner = objBlog.UserID = ModuleContext.PortalSettings.UserId

    If (Entry.Published = False) AndAlso (objSecurity.CanAddEntry(isOwner, objBlog.AuthorMode) = False) Then
     Response.Redirect(NavigateURL("Access Denied"), False)
    End If

    If Not objBlog Is Nothing Then
     ' Comment perms here

     If objBlog.ShowFullName Then
      hlAuthor.Text = Entry.UserFullName
      hlAuthorBio.Text = Entry.UserFullName
     Else
      hlAuthor.Text = Entry.UserName
      hlAuthorBio.Text = Entry.UserName
     End If
     hlAuthor.NavigateUrl = DotNetNuke.Common.Globals.UserProfileURL(Entry.UserID)
     pnlComments.Visible = objBlog.AllowComments

     hlAuthorBio.NavigateUrl = DotNetNuke.Common.Globals.UserProfileURL(Entry.UserID)
     imgAuthorLink.NavigateUrl = DotNetNuke.Common.Globals.UserProfileURL(Entry.UserID)

     Dim objAuthor As Entities.Users.UserInfo = DotNetNuke.Entities.Users.UserController.GetUserById(ModuleContext.PortalId, objBlog.UserID)
     dbiUser.ImageUrl = Control.ResolveUrl("~/profilepic.ashx?userid=" + objAuthor.UserID.ToString + "&w=" + "50" + "&h=" + "50")
     'litBio.Text = "<p>" + objAuthor.DisplayName + "</p>"

     lnkBlogs.NavigateUrl = NavigateURL()
     If objBlog.ParentBlogID > -1 Then
      Dim cntBlog As New BlogController
      objParentBlog = cntBlog.GetBlog(objBlog.ParentBlogID)
      lnkParentBlog.Text = objParentBlog.Title
      lnkParentBlog.NavigateUrl = NavigateURL(Me.TabId, "", "BlogID=" & objParentBlog.BlogID.ToString())
      lnkChildBlog.Visible = True
      lnkChildBlog.Text = objBlog.Title
      lnkChildBlog.NavigateUrl = NavigateURL(Me.TabId, "", "BlogID=" & objBlog.BlogID.ToString())
      imgParentChildSeparator.Visible = True
     Else
      lnkParentBlog.Text = objBlog.Title
      lnkParentBlog.NavigateUrl = NavigateURL(Me.TabId, "", "BlogID=" & objBlog.BlogID.ToString())
      lnkChildBlog.Visible = False
      imgParentChildSeparator.Visible = False
     End If
     If Entry.DisplayCopyright Then
      If Entry.Copyright.Length > 0 Then
       Me.BasePage.Copyright = Entry.Copyright
       lblCopyright.Text = (New PortalSecurity).InputFilter(Entry.Copyright, PortalSecurity.FilterFlag.NoScripting)
       lblCopyright.Visible = True
      End If
     End If
    End If

    If Not Entry Is Nothing Then
     ' Make sure content item and moduleid are proper here (because we integrated content items years after module was built)
     If (Entry.ContentItemId < 1) Then
      Dim cntEntry As New EntryController()
      Dim objEntry As EntryInfo = cntEntry.GetEntry(Entry.EntryID, ModuleContext.PortalId)

      objEntry.ModuleID = ModuleContext.ModuleId
      objEntry.TabID = ModuleContext.TabId

      Dim cntTaxonomy As New Content()
      Dim objContentItem As ContentItem = cntTaxonomy.CreateContentItem(objEntry, ModuleContext.TabId)
      objEntry.ContentItemId = objContentItem.ContentItemId

      cntEntry.UpdateEntry(objEntry, ModuleContext.TabId, ModuleContext.PortalId)
     End If

     'DW - 08/14/2008 - Added code to issue a 301 Redirect in cases where the URL of the page is not the same
     ' as that created by the new BlogNavigateURL function
     Dim requestedUrl As String = DirectCast(HttpContext.Current.Items()("UrlRewrite:OriginalUrl"), String)
     ' DW - 11/12/2008 - Replaced with Permalink
     Dim correctUrl As String = Entry.PermaLink
     If (BlogSettings.ShowSeoFriendlyUrl And Not requestedUrl Is Nothing And (Not requestedUrl.ToLower().EndsWith(correctUrl.ToLower()) And Not System.Web.HttpUtility.UrlDecode(requestedUrl.ToLower()).EndsWith(correctUrl.ToLower()))) Then
      'NOTE: We use EndsWith here because NavigateURL returns a relative URL to BlogNavigateURL
      '       when friendly URLs is not turned on for the portal.
      '301 Redirect to the correct format for the page
      Response.Status = "301 Moved Permanently"
      Response.AddHeader("Location", correctUrl)
      Response.AddHeader("X-Blog-Redirect-Reason", "No match for requested Url (" & requestedUrl & ").  Correct url is " & correctUrl)
      Response.End()
     End If

     lblBlogTitle.InnerHtml = Entry.Title ' Issue 22505
     'lblBlogTitle.NavigateUrl = m_oEntry.PermaLink
     lblTrackback.Text = Utility.GetTrackbackRDF(NavigateURL(), Entry)
     lblDateTime.Text = Utility.DateFromUtc(Entry.AddedDate, UITimeZone).ToString("f")

     If BlogSettings.SocialSharingMode = "0" Then
      Dim facebookContent As String = ""
      Dim googleContent As String = ""
      Dim twitterContent As String = ""
      Dim linkedInContent As String = ""

      If BlogSettings.FacebookAppId.Length > 0 Then
       facebookContent = "<li><div class='fb-like' data-send='false' data-width='46' data-show-faces='false' data-layout='button_count'></div></li>"
      End If

      If BlogSettings.EnablePlusOne Then
       googleContent = "<li><g:plusone annotation='none' size='medium'></g:plusone></li>"
      End If

      If BlogSettings.EnableTwitter Then
       twitterContent = "<li><a href='https://twitter.com/share' data-lang='en' data-count='none' class='twitter-share-button' data-size='small'" + "'></a></li>"
      End If

      If BlogSettings.EnableLinkedIn Then
       linkedInContent = "<li><script type='IN/Share'></script></li>"
      End If

      litSocialSharing.Text = "<ul class='qaSocialActions'>" + facebookContent + googleContent + twitterContent + linkedInContent + "</ul>"
     ElseIf BlogSettings.SocialSharingMode = "1" Then
      Dim addThisId As String = BlogSettings.AddThisId

     Else
      litSocialSharing.Visible = False
     End If

     litSummary.Text = Server.HtmlDecode(Entry.Description)

     If BlogSettings.ShowSummary Then
      litSummary.Visible = True
     Else
      litSummary.Visible = False
     End If

     litEntry.Text = Server.HtmlDecode(Entry.Entry)

     pnlComments.Visible = (objBlog.AllowComments Or Entry.AllowComments)
     If pnlComments.Visible Then
      BindCommentsList()
     End If
     pnlAddComment.Visible = (objBlog.AllowComments Or Entry.AllowComments) And (objBlog.AllowAnonymous Or Me.UserId > -1)

     If ModuleContext.PortalSettings.UserId > 0 Then
      litAddComment.Text = "<a href='#AddComment' id='linkAdd' class='dnnPrimaryAction'>" + Localization.GetString("AddComment", LocalResourceFile) + "</a>"
     Else
      If objBlog.AllowAnonymous Then
       litAddComment.Text = "<a href='#AddComment' id='linkAdd' class='dnnPrimaryAction'>" + Localization.GetString("AddComment", LocalResourceFile) + "</a>"
      Else
       Dim returnUrl As String = HttpContext.Current.Request.RawUrl
       If returnUrl.IndexOf("?returnurl=") <> -1 Then
        returnUrl = returnUrl.Substring(0, returnUrl.IndexOf("?returnurl="))
       End If

       returnUrl = HttpUtility.UrlEncode(returnUrl)
       litAddComment.Text = "<a href='" + DotNetNuke.Common.Globals.LoginURL(returnUrl, False) + "' class='dnnPrimaryAction'>" + Localization.GetString("AddComment", LocalResourceFile) + "</a>"
      End If
     End If

     'lnkPermaLink.NavigateUrl = m_oEntry.PermaLink

     'Antonio Chagoury
     'BLG(-4471): Fixed CDATA enconding in 
     'title and description for trackbacks
     lnkTrackBack.NavigateUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) & Me.ControlPath & "Trackback.aspx?id=" & Entry.EntryID & "&blogid=" & Entry.BlogID

     'pnlWebsite.Visible = BlogSettings.ShowWebsite
     'pnlCommentTitle.Visible = BlogSettings.ShowCommentTitle
     'pnlGravatar.Visible = BlogSettings.ShowGravatars
    End If

    cntEntry.UpdateEntryViewCount(Entry.EntryID)
   End If

   txtClientIP.Text = HttpContext.Current.Request.UserHostAddress.ToString

   Dim Categories As String = ""
   Dim i As Integer = 0
   Dim colCategories As List(Of TermInfo) = Entry.EntryTerms(VocabularyId)

   For Each objTerm As TermInfo In colCategories
    Categories += "<a href='" + ModuleContext.NavigateUrl(ModuleContext.TabId, "", False, "catid=" + objTerm.TermId.ToString()) + "'>" + objTerm.Name + "</a>"
    i += 1
    If i <= (colCategories.Count - 1) Then
     Categories += ", "
    End If
   Next

   litCategories.Text = Categories

   rptTags.DataSource = Entry.EntryTerms(1)
   rptTags.DataBind()

  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 Protected Sub lstComments_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataListItemEventArgs) Handles lstComments.ItemDataBound
  Dim imgUser As System.Web.UI.WebControls.Image = CType(e.Item.FindControl("imgUser"), System.Web.UI.WebControls.Image)
  Dim hlUser As System.Web.UI.WebControls.HyperLink = CType(e.Item.FindControl("hlUser"), System.Web.UI.WebControls.HyperLink)
  Dim hlCommentAuthor As System.Web.UI.WebControls.HyperLink = CType(e.Item.FindControl("hlCommentAuthor"), System.Web.UI.WebControls.HyperLink)
  'hlCommentAuthor
  Dim lnkEditComment As System.Web.UI.WebControls.ImageButton = CType(e.Item.FindControl("lnkEditComment"), System.Web.UI.WebControls.ImageButton)
  Dim lnkApproveComment As System.Web.UI.WebControls.ImageButton = CType(e.Item.FindControl("lnkApproveComment"), System.Web.UI.WebControls.ImageButton)
  Dim lblCommentDate As System.Web.UI.WebControls.Label = CType(e.Item.FindControl("lblCommentDate"), System.Web.UI.WebControls.Label)
  Dim divBlogBubble As System.Web.UI.WebControls.Panel = CType(e.Item.FindControl("divBlogBubble"), System.Web.UI.WebControls.Panel)
  Dim lnkDeleteComment As System.Web.UI.WebControls.ImageButton = CType(e.Item.FindControl("lnkDeleteComment"), System.Web.UI.WebControls.ImageButton)

  Dim objComment As CommentInfo = CType(e.Item.DataItem, CommentInfo)

  Dim objSecurity As ModuleSecurity = New ModuleSecurity(ModuleContext.ModuleId, ModuleContext.TabId)

  Dim isOwner As Boolean
  isOwner = objBlog.UserID = ModuleContext.PortalSettings.UserId

  lnkEditComment.Visible = objSecurity.CanAddEntry(isOwner, objBlog.AuthorMode)

  lnkDeleteComment.Visible = lnkEditComment.Visible

  Dim objUser As UserInfo = UserController.GetUserById(ModuleContext.PortalId, objComment.UserID)

  hlUser.NavigateUrl = DotNetNuke.Common.Globals.UserProfileURL(objUser.UserID)
  imgUser.ImageUrl = Control.ResolveUrl("~/profilepic.ashx?userid=" + objComment.UserID.ToString + "&w=" + "50" + "&h=" + "50")
  hlCommentAuthor.Text = objUser.DisplayName
  hlCommentAuthor.NavigateUrl = DotNetNuke.Common.Globals.UserProfileURL(objUser.UserID)

  'DW - 06/06/2008
  'Set a unique CSS class for the blog bubbles of the blog owner
  If objBlog.UserID = objComment.UserID Then
   divBlogBubble.CssClass = "BlogBubbleOwner"
  End If

  Dim n As DateTime = Utility.AdjustedDate(CType(e.Item.DataItem, CommentInfo).AddedDate, UITimeZone)
  Dim publishDate As DateTime = n
  Dim timeOffset As TimeSpan = UITimeZone.BaseUtcOffset
  publishDate = publishDate.Add(timeOffset)
  lblCommentDate.Text = Utility.CalculateDateForDisplay(publishDate)

  If Not objComment.Approved Then
   lnkApproveComment.Visible = True
  Else
   lnkApproveComment.Visible = False
  End If
 End Sub

 Protected Sub lstComments_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataListCommandEventArgs) Handles lstComments.ItemCommand
  Select Case e.CommandName.ToLower
   Case "editcomment"
    valComment.Enabled = False

    Dim oComment As CommentInfo = New CommentController().GetComment(Int32.Parse(CType(e.CommandArgument, String)))
    If Not oComment Is Nothing Then
     txtComment.Text = Utility.removeAllHtmltags(Server.HtmlDecode(oComment.Comment))
     ViewState("CommentID") = oComment.CommentID

     cmdAddComment.Text = Localization.GetString("msgUpdateComment", LocalResourceFile)
     cmdDeleteComment.Visible = True
    End If
   Case "approvecomment"
    Dim oComment As CommentInfo = New CommentController().GetComment(Int32.Parse(CType(e.CommandArgument, String)))
    oComment.Approved = True
    Dim objCtlComment As New CommentController
    objCtlComment.UpdateComment(oComment)
    BindCommentsList()
   Case "deletecomment"
    ' DR 01/21/2008 - Fix BLG-8849
    ' Added fast comment deletion.
    Dim oCommentController As New CommentController
    oCommentController.DeleteComment(Int32.Parse(CType(e.CommandArgument, String)))
    BindCommentsList()
  End Select
 End Sub

 Protected Sub cmdAddComment_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAddComment.Click
  Try
   valComment.Enabled = True

   If (txtComment.Text.Length > 0) And (ModuleContext.PortalSettings.UserId > 0) Then
    Dim objComment As New CommentInfo
    Dim cntComment As New CommentController

    If Not ViewState("CommentID") Is Nothing Then
     objComment = cntComment.GetComment(Int32.Parse(CType(ViewState("CommentID"), String)))
     ViewState("CommentID") = Nothing
    Else
     objComment = CType(CBO.InitializeObject(objComment, GetType(CommentInfo)), CommentInfo)
     objComment.EntryID = Entry.EntryID
     objComment.UserID = Me.UserId
    End If
    Dim objSec As New PortalSecurity

    objComment.Comment = objSec.InputFilter(txtComment.Text, PortalSecurity.FilterFlag.NoProfanity)

    Dim objSecurity As ModuleSecurity = New ModuleSecurity(ModuleContext.ModuleId, ModuleContext.TabId)

    Dim isOwner As Boolean
    isOwner = objBlog.UserID = ModuleContext.PortalSettings.UserId

    If objSecurity.CanAddEntry(isOwner, objBlog.AuthorMode) Then
     objComment.Approved = True
    End If
    If objComment.CommentID > -1 Then
     cntComment.UpdateComment(objComment)

     If objComment.Approved Then
      Dim cntJournal As New Components.Integration.Journal
      cntJournal.AddCommentToJournal(Entry, objComment, ModuleContext.PortalId, ModuleContext.TabId, objComment.UserID, Entry.PermaLink)
     End If
    Else
     objComment.CommentID = cntComment.AddComment(objComment)


                    If objComment.Approved Then
                        Dim cntJournal As New Journal
                        cntJournal.AddCommentToJournal(Entry, objComment, ModuleContext.PortalId, ModuleContext.TabId, ModuleContext.PortalSettings.UserId, Entry.PermaLink)

                        ' Notification of the comment being added (unless the blog owner is adding it)
                        If (objComment.UserID <> objBlog.UserID) Then
                            If objBlog.EmailNotification = True Then
                                Dim cntNotification As New Notifications


                            End If
                        End If
                    Else
                        ' Notification of comment requiring approval
                        Dim cntNotification As New Notifications

                    End If

     If objComment.Approved Then
      Dim cntJournal As New Components.Integration.Journal
      cntJournal.AddCommentToJournal(Entry, objComment, ModuleContext.PortalId, ModuleContext.TabId, ModuleContext.PortalSettings.UserId, Entry.PermaLink)
     End If

     If objBlog.EmailNotification = True Then
      sendMail(objBlog, objComment)
     End If
    End If
    BindCommentsList()
   End If
  Catch exc As Exception
   LogException(exc)
  End Try
 End Sub

 Protected Sub cmdDeleteComment_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDeleteComment.Click
  If Not ViewState("CommentID") Is Nothing Then
   Dim oCommentController As New CommentController
   oCommentController.DeleteComment(Int32.Parse(CType(ViewState("CommentID"), String)))
   ViewState("CommentID") = Nothing
   BindCommentsList()
  End If
 End Sub

 Protected Sub cmdPrint_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdPrint.Click
  Response.Clear()
  Dim strPrintPage As String
  Dim strAuthor As String

  ' Rip Rowan 2008.05.29
  ' Set author based on username / fullname section
  ' Fixed BLG-6964

  If objBlog.ShowFullName Then
   strAuthor = objBlog.UserFullName
  Else
   strAuthor = objBlog.UserName
  End If

  ' Rip Rowan 2008.05.29
  ' Changed title to entry.title & (Print Friendly View)
  ' Improves output when printed
  strPrintPage = "<html><head><title>" & Entry.Title & " (Print Friendly View)</title></head><body bgcolor=""white"">" & _
  "<table width=""100%""><tr><td align=""left"">" & _
  "<h1>" & Entry.Title & "</h1></td>" & _
  "<td align=""right""><h5>" & Localization.GetString("lblPostedBy.Text", LocalResourceFile) & strAuthor & " " & Entry.AddedDate.ToLocalTime().ToString() & "</h5></td></tr></table>" & _
  "<hr />"

  If Entry.Description <> String.Empty Then
   ' Rip Rowan 2008.05.29
   ' HTMLDecode the entry.description
   ' Fixed  BLG-7641
   strPrintPage += Server.HtmlDecode(Entry.Description) & Environment.NewLine & "<hr />"
  End If

  strPrintPage += Server.HtmlDecode(Entry.Entry) & Environment.NewLine & _
  "<hr />" & _
  (New PortalSecurity).InputFilter(Server.HtmlDecode(Entry.Copyright), PortalSecurity.FilterFlag.NoScripting) & Environment.NewLine & _
  "</body></html>"

  Response.Write(strPrintPage)
  Response.End()

 End Sub

 Protected Sub RptTagsItemDataBound(sender As Object, e As RepeaterItemEventArgs)
  Dim tagControl As Tags = DirectCast(e.Item.FindControl("dbaSingleTag"), Tags)
  Dim term As TermInfo = DirectCast(e.Item.DataItem, TermInfo)
  Dim colTerms As New List(Of TermInfo)
  colTerms.Add(term)

  tagControl.ModContext = ModuleContext
  tagControl.DataSource = colTerms
  'tagControl.CountMode = TagTimeFrame;	
  tagControl.DataBind()
 End Sub

#End Region

#Region "Private Methods"

 Private Sub ScriptRegistration()
  jQuery.RequestUIRegistration()
  ClientResourceManager.RegisterScript(Page, TemplateSourceDirectory + "/js/jquery.qatooltip.js")
  ClientResourceManager.RegisterScript(Page, "~/Resources/Shared/Scripts/jquery/jquery.hoverIntent.min.js")
  ClientResourceManager.RegisterScript(Page, TemplateSourceDirectory + "/js/jquery.qaplaceholder.js")

  If BlogSettings.EnableLinkedIn Then
   ClientResourceManager.RegisterScript(Page, "https://platform.linkedin.com/in.js")
  End If


 End Sub

 Private Sub BindCommentsList()
  Dim objCtlComments As New CommentController
  Dim list As ArrayList

  Dim objSecurity As ModuleSecurity = New ModuleSecurity(ModuleContext.ModuleId, ModuleContext.TabId)

  Dim isOwner As Boolean
  isOwner = objBlog.UserID = ModuleContext.PortalSettings.UserId


  list = objCtlComments.ListComments(Entry.EntryID, objSecurity.CanAddEntry(isOwner, objBlog.AuthorMode))
  lstComments.DataSource = list
  lstComments.DataBind()

  'lblCommentCount.Text = Localization.GetString("msgComments", LocalResourceFile) & lstComments.Items.Count & ")"

  'Antonio Chagoury
  'Using string format routing instead of appending ")"
  lblComments.Text = String.Format(Localization.GetString("lblComments", LocalResourceFile), lstComments.Items.Count)

  txtComment.Text = ""
  cmdAddComment.Text = Localization.GetString("msgAddComment", LocalResourceFile)
  cmdDeleteComment.Visible = False
 End Sub

 Private Sub sendMail(ByVal oBlog As BlogInfo, ByVal Comment As CommentInfo)
  Dim strSubject As String = Localization.GetString("msgMailSubject", LocalResourceFile)
  Dim strBody As String = Localization.GetString("msgMailBody", LocalResourceFile)
  ' replace [BLOG] with Blog.Title
  strSubject = strSubject.Replace("[BLOG]", oBlog.Title)
  strBody = strBody.Replace("[BLOG]", oBlog.Title)
  ' replace [TITLE] with Comment.Title
  strSubject = strSubject.Replace("[TITLE]", Comment.Title)
  strBody = strBody.Replace("[TITLE]", Comment.Title)
  ' replace [COMMENT] with Comment.Comment
  strSubject = strSubject.Replace("[COMMENT]", Comment.Comment)
  strBody = strBody.Replace("[COMMENT]", Comment.Comment)
  ' replace [DATE] with Comment.Date
  strSubject = strSubject.Replace("[DATE]", Now.ToShortDateString)
  strBody = strBody.Replace("[DATE]", Now.ToShortDateString)
  ' replace [USER] with Comment.UserName
  strSubject = strSubject.Replace("[USER]", Comment.UserName)
  strBody = strBody.Replace("[USER]", Comment.UserName)
  ' replace [URL] with NavigateUrl()
  strSubject = strSubject.Replace("[URL]", Entry.PermaLink)
  strBody = strBody.Replace("[URL]", Entry.PermaLink)

  Dim uc As New UserController
  Dim ui As UserInfo = uc.GetUser(PortalId, oBlog.UserID)

  DotNetNuke.Services.Mail.Mail.SendMail(PortalSettings.Email, ui.Email, "", strSubject, strBody, "", "html", "", "", "", "")
 End Sub

#End Region

#Region "Public Methods"

 Public Function FormatUserName(ByVal UserName As String) As String
  FormatUserName = String.Format(Localization.GetString("lblFormatUserName.Text", LocalResourceFile), UserName)
 End Function

#End Region

End Class