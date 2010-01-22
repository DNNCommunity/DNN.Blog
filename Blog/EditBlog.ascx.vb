'
' DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2010
' by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
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
'-------------------------------------------------------------------------

Imports System
Imports DotNetNuke.Modules.Blog.Business
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Framework
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Security



Partial Class EditBlog
 Inherits BlogModuleBase

#Region " Private Members "
 Private m_oBlogController As New BlogController
 Private m_oParentBlog As BlogInfo
 Private m_oBlog As BlogInfo
 Private m_oBlogSettings As Hashtable
#End Region

#Region " Controls "
#End Region

#Region " Event Handlers "
 Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

  If Not (Request.Params("BlogID") Is Nothing) Then
   If Int32.Parse(Request.Params("BlogID")) > 0 Then
    m_oBlog = m_oBlogController.GetBlog(Int32.Parse(Request.Params("BlogID")))
    If Not Utility.HasBlogPermission(Me.UserId, m_oBlog.UserID, Me.ModuleId) Then
     Response.Redirect(NavigateURL())
    ElseIf m_oBlog.ParentBlogID > -1 Then
     m_oParentBlog = m_oBlogController.GetBlog(m_oBlog.ParentBlogID)
    End If
   End If
  End If

  If m_oBlog Is Nothing And Not (Request.Params("ParentBlogID") Is Nothing) And BlogSettings.AllowChildBlogs Then
   If Int32.Parse(Request.Params("ParentBlogID")) > 0 Then
    m_oParentBlog = m_oBlogController.GetBlog(Int32.Parse(Request.Params("ParentBlogID")))
   End If
  End If

  If Not m_oParentBlog Is Nothing Then
   If Not Utility.HasBlogPermission(Me.UserId, m_oParentBlog.UserID, Me.ModuleId) Then
    Response.Redirect(NavigateURL())
   Else
    If m_oBlog Is Nothing Then
     Me.ModuleConfiguration.ModuleTitle = Localization.GetString("msgCreateNewChildBlog", LocalResourceFile)
    Else
     Me.ModuleConfiguration.ModuleTitle = Localization.GetString("msgEditChildBlog", LocalResourceFile)
    End If
   End If
  Else
   If m_oBlog Is Nothing Then
    Me.ModuleConfiguration.ModuleTitle = Localization.GetString("msgCreateBlog", LocalResourceFile)
   Else
    Me.ModuleConfiguration.ModuleTitle = Localization.GetString("msgEditBlog", LocalResourceFile)
   End If
  End If

 End Sub

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  Try
   If Not Page.IsPostBack Then
    DotNetNuke.UI.Utilities.ClientAPI.AddButtonConfirm(cmdDelete, Localization.GetString("msgDeleteBlog", LocalResourceFile))
    DotNetNuke.UI.Utilities.ClientAPI.AddButtonConfirm(btnDeleteChildBlog, Localization.GetString("msgDeleteChildBlog", LocalResourceFile))

    Localization.LoadTimeZoneDropDownList(cboTimeZone, CType(Page, PageBase).PageCulture.Name, PortalSettings.TimeZoneOffset.ToString)
    Localization.LoadCultureDropDownList(cboCulture, CultureDropDownTypes.NativeName, CType(Page, PageBase).PageCulture.Name)

    txtTweetTemplate.Text = Localization.GetString("txtTweetTemplate", LocalResourceFile)
    lblChildBlogsOff.Visible = (Not BlogSettings.AllowChildBlogs)

    If Not m_oBlog Is Nothing Then
     'Load data
     txtTitle.Text = m_oBlog.Title
     txtDescription.Text = m_oBlog.Description
     chkPublic.Checked = m_oBlog.Public
     'DR-19/04/2009-BLG-9760
     'Load the Twitter settings
     chkEnableTwitterIntegration.Checked = m_oBlog.EnableTwitterIntegration
     txtTwitterUsername.Text = m_oBlog.TwitterUsername
     'Dim oSec As New PortalSecurity ' is not shown anyway
     'txtTwitterPassword.Text = oSec.Decrypt(m_oBlog.EncryptionKey, m_oBlog.TwitterPassword)
     'oSec = Nothing
     txtTweetTemplate.Text = m_oBlog.TweetTemplate

     If m_oBlog.AllowComments Then
      If m_oBlog.MustApproveComments Then
       rdoUsersComments.Items.FindByValue("RequireApproval").Selected = True
      Else
       rdoUsersComments.Items.FindByValue("Allow").Selected = True
      End If
     Else
      rdoUsersComments.Items.FindByValue("Disallow").Selected = True
     End If
     If m_oBlog.AllowAnonymous Then
      If m_oBlog.MustApproveAnonymous Then
       rdoAnonymousComments.Items.FindByValue("RequireApproval").Selected = True
      Else
       rdoAnonymousComments.Items.FindByValue("Allow").Selected = True
      End If
     Else
      rdoAnonymousComments.Items.FindByValue("Disallow").Selected = True
     End If
     If m_oBlog.AllowTrackbacks Then
      If m_oBlog.MustApproveTrackbacks Then
       rdoTrackbacks.Items.FindByValue("RequireApproval").Selected = True
      Else
       rdoTrackbacks.Items.FindByValue("Allow").Selected = True
      End If
     Else
      rdoTrackbacks.Items.FindByValue("Disallow").Selected = True
     End If
     chkEmailNotification.Checked = m_oBlog.EmailNotification
     chkAutoTrackbacks.Checked = m_oBlog.AutoTrackback
     chkCaptcha.Checked = m_oBlog.UseCaptcha

     chkSyndicate.Checked = m_oBlog.Syndicated
     If m_oBlog.ParentBlogID = -1 Then
      chkSyndicateIndependant.Visible = False
     Else
      chkSyndicateIndependant.Checked = m_oBlog.SyndicateIndependant
     End If
     If m_oBlog.SyndicationEmail Is Nothing Then
      txtSyndicationEmail.Text = New DotNetNuke.Entities.Users.UserMembership().Email
     Else
      txtSyndicationEmail.Text = m_oBlog.SyndicationEmail
     End If
     cmdDelete.Visible = True
     btnAddChildBlog.Enabled = BlogSettings.AllowChildBlogs
     If Not rdoUserName.SelectedItem Is Nothing Then rdoUserName.SelectedItem.Selected = False
     rdoUserName.Items.FindByValue(m_oBlog.ShowFullName.ToString()).Selected = True
     lstChildBlogs.Attributes.Add("onclick", "if (this.selectedIndex > -1) { " & btnEditChildBlog.ClientID & ".disabled = false; " & btnDeleteChildBlog.ClientID & ".disabled = false; }")

     If m_oParentBlog Is Nothing Then
      lstChildBlogs.DataTextField = "Title"
      lstChildBlogs.DataValueField = "BlogID"
      lstChildBlogs.DataSource = m_oBlogController.ListBlogs(Me.PortalId, m_oBlog.BlogID, True)
      lstChildBlogs.DataBind()
     Else
      pnlChildBlogs.Visible = False
     End If

     BindDateOptions(m_oBlog.TimeZone, m_oBlog.Culture, m_oBlog.DateFormat)

     lblMetaWeblogUrl.Text = "http://" & Request.Url.Host & ModulePath & "blogpost.ashx?tabid=" & TabId

    ElseIf Not m_oParentBlog Is Nothing Then
     If Not rdoUserName.SelectedItem Is Nothing Then rdoUserName.SelectedItem.Selected = False
     rdoUserName.Items.FindByValue(m_oParentBlog.ShowFullName.ToString()).Selected = True
     BindDateOptions(m_oParentBlog.TimeZone, m_oParentBlog.Culture, m_oParentBlog.DateFormat)

     pnlChildBlogs.Visible = False
    Else
     BindDateOptions(Now.Subtract(Date.UtcNow).Hours, System.Threading.Thread.CurrentThread.CurrentCulture.Name, "g")
    End If
    If Not Request.UrlReferrer Is Nothing Then
     ViewState("URLReferrer") = Request.UrlReferrer.ToString
    End If
   End If

  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCancel.Click
  Try
   If m_oParentBlog Is Nothing Then
    Response.Redirect(NavigateURL(), True)
   Else
    Response.Redirect(CType(ViewState("URLReferrer"), String), True)
   End If
  Catch exc As Exception    'Module failed to load
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 Private Sub cmdDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDelete.Click
  Try
   If Not m_oBlog Is Nothing Then
    m_oBlogController.DeleteBlog(m_oBlog.BlogID)
   End If
   Response.Redirect(NavigateURL(), True)
  Catch exc As Exception    'Module failed to load
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 Private Sub cboTimeZone_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboTimeZone.SelectedIndexChanged
  BindDateFormats()
 End Sub

 Private Sub cboCulture_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboCulture.SelectedIndexChanged
  BindDateFormats()
 End Sub

 Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
  If UpdateBlog() Then
   If Not m_oParentBlog Is Nothing Then
    Response.Redirect(EditUrl("BlogID", m_oParentBlog.BlogID.ToString(), "Edit_Blog"), True)
   Else
    Response.Redirect(Utility.AddTOQueryString(NavigateURL(), "BlogID", m_oBlog.BlogID.ToString()), True)
   End If
  End If
 End Sub

 Private Sub btnAddChildBlog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddChildBlog.Click
  If UpdateBlog() Then
   Response.Redirect(EditUrl("ParentBlogID", m_oBlog.BlogID.ToString(), "Edit_Blog"))
  End If
 End Sub

 Private Sub btnEditChildBlog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEditChildBlog.Click
  If Not lstChildBlogs.SelectedItem Is Nothing Then
   If UpdateBlog() Then
    Response.Redirect(EditUrl("BlogID", lstChildBlogs.SelectedValue, "Edit_Blog"))
   End If
  End If
 End Sub

 Private Sub btnDeleteChildBlog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDeleteChildBlog.Click
  If Not lstChildBlogs.SelectedItem Is Nothing Then
   m_oBlogController.DeleteBlog(CType(lstChildBlogs.SelectedValue, Integer))
   lstChildBlogs.Items.Remove(lstChildBlogs.SelectedItem)
  End If
 End Sub

 Private Sub cmdGenerateLinks_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdGenerateLinks.Click
  Utility.CreateAllEntryLinks(PortalId, m_oBlog.BlogID)
 End Sub

 Private Sub chkEnableTwitterIntegration_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkEnableTwitterIntegration.CheckedChanged
  'DR-19/04/2009-BLG-9760
  Dim Enable As Boolean = chkEnableTwitterIntegration.Checked

  If Enable Then
   txtTwitterUsername.Enabled = True
   txtTwitterPassword.Enabled = True
   txtTwitterUsername.Enabled = True
   txtTweetTemplate.Enabled = True
  Else
   txtTwitterUsername.Enabled = False
   txtTwitterPassword.Enabled = False
   txtTwitterUsername.Enabled = False
   txtTweetTemplate.Enabled = False
  End If

 End Sub

#End Region

#Region "Private Methods"
 Private Sub BindDateOptions(ByVal TimeZone As Integer, ByVal Culture As String, ByVal DateFormat As String)
  If Not cboCulture.Items.FindByValue(Culture) Is Nothing Then
   cboCulture.SelectedValue = Culture
  End If
  If Not cboTimeZone.Items.FindByValue(TimeZone.ToString()) Is Nothing Then
   cboTimeZone.SelectedValue = TimeZone.ToString
  End If
  BindDateFormats()
  If Not cboDateFormat.Items.FindByValue(DateFormat) Is Nothing Then
   cboDateFormat.SelectedValue = DateFormat
  End If
 End Sub

 Private Function UpdateBlog() As Boolean
  Try
   If Page.IsValid = True Then
    If m_oBlog Is Nothing Then
     m_oBlog = New BlogInfo
     m_oBlog = CType(CBO.InitializeObject(m_oBlog, GetType(BlogInfo)), BlogInfo)
     With m_oBlog
      .UserID = Me.UserId
      .PortalID = Me.PortalId
      If Not m_oParentBlog Is Nothing Then
       .ParentBlogID = m_oParentBlog.BlogID
      End If
     End With
    End If
    With m_oBlog
     'bind text values to object
     .Title = txtTitle.Text
     .Description = txtDescription.Text
     .Public = chkPublic.Checked
     .ShowFullName = CType(rdoUserName.SelectedItem.Value, Boolean)
     .Culture = cboCulture.SelectedItem.Value
     .DateFormat = cboDateFormat.SelectedItem.Value
     .TimeZone = CType(cboTimeZone.SelectedItem.Value, Integer)
     .Syndicated = chkSyndicate.Checked
     .SyndicateIndependant = chkSyndicateIndependant.Checked
     .SyndicationEmail = txtSyndicationEmail.Text
     .EmailNotification = chkEmailNotification.Checked
     .AutoTrackback = chkAutoTrackbacks.Checked
     .UseCaptcha = chkCaptcha.Checked
     Select Case rdoUsersComments.SelectedItem.Value
      Case "Allow"
       .AllowComments = True
       .MustApproveComments = False
      Case "RequireApproval"
       .AllowComments = True
       .MustApproveComments = True
      Case "Disallow"
       .AllowComments = False
       .MustApproveComments = True
      Case Else
       .AllowComments = True
       .MustApproveComments = True
     End Select
     Select Case rdoAnonymousComments.SelectedItem.Value
      Case "Allow"
       .AllowAnonymous = True
       .MustApproveAnonymous = False
      Case "RequireApproval"
       .AllowAnonymous = True
       .MustApproveAnonymous = True
      Case "Disallow"
       .AllowAnonymous = False
       .MustApproveAnonymous = True
      Case Else
       .AllowAnonymous = True
       .MustApproveAnonymous = True
     End Select
     Select Case rdoTrackbacks.SelectedItem.Value
      Case "Allow"
       .AllowTrackbacks = True
       .MustApproveTrackbacks = False
      Case "RequireApproval"
       .AllowTrackbacks = True
       .MustApproveTrackbacks = True
      Case "Disallow"
       .AllowTrackbacks = False
       .MustApproveTrackbacks = True
      Case Else
       .AllowTrackbacks = True
       .MustApproveTrackbacks = True
     End Select
     'DR-19/04/2009-BLG-9760
     .EnableTwitterIntegration = chkEnableTwitterIntegration.Checked
     .TwitterUsername = txtTwitterUsername.Text

     If txtTwitterPassword.Text.Length > 0 Then
      Dim oSec As New PortalSecurity
      .TwitterPassword = oSec.Encrypt(m_oBlog.EncryptionKey, txtTwitterPassword.Text)
      oSec = Nothing
     End If

     .TweetTemplate = txtTweetTemplate.Text

     If Null.IsNull(m_oBlog.BlogID) Then
      .BlogID = m_oBlogController.AddBlog(m_oBlog)
      If .Syndicated Then m_oBlog = m_oBlogController.GetBlog(.BlogID)
     Else
      m_oBlogController.UpdateBlog(m_oBlog)
     End If
    End With
    Return True
   End If
  Catch exc As Exception    'Module failed to load
   ProcessModuleLoadException(Me, exc)
  End Try
  Return False
 End Function

 Private Sub BindDateFormats()
  If Not cboCulture.SelectedItem Is Nothing Then
   Dim dt As Date = Date.UtcNow.AddMinutes(Integer.Parse(cboTimeZone.SelectedValue))
   Dim dtf As System.Globalization.DateTimeFormatInfo = New System.Globalization.CultureInfo(cboCulture.SelectedItem.Value, False).DateTimeFormat
   Dim i As Integer = cboDateFormat.SelectedIndex
   With cboDateFormat
    .Items.Clear()
    .Items.Add(New ListItem(dt.ToString("d", dtf), "d"))
    .Items.Add(New ListItem(dt.ToString("g", dtf), "g"))
    .Items.Add(New ListItem(dt.ToString("G", dtf), "G"))
    .Items.Add(New ListItem(dt.ToString("D", dtf), "D"))
    .Items.Add(New ListItem(dt.ToString("f", dtf), "f"))
    .Items.Add(New ListItem(dt.ToString("F", dtf), "F"))
    .Items.Add(New ListItem(dt.ToString("U", dtf), "U"))
    .Items.Add(New ListItem(dt.ToString("r", dtf), "r"))
    .Items.Add(New ListItem(dt.ToString("s", dtf), "s"))
    .Items.Add(New ListItem(dt.ToString("u", dtf), "u"))
    .SelectedIndex = i
   End With
  End If
 End Sub
#End Region

End Class


