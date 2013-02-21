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
Imports DotNetNuke.Modules.Blog.Business
Imports DotNetNuke.Modules.Blog.Controllers
Imports DotNetNuke.Modules.Blog.Common
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Framework
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Modules.Blog.Entities

Partial Public Class EditBlog
 Inherits BlogModuleBase

#Region "Private Members"
#End Region

#Region " Controls "
#End Region

#Region "Event Handlers"

 Protected Overloads Sub Page_Init(sender As System.Object, e As System.EventArgs) Handles MyBase.Init
  jQuery.RequestDnnPluginsRegistration()

  If Not Security.CanEdit Then
   Response.Redirect(NavigateURL("Access Denied"))
  End If

  If Blog Is Nothing Then
   Me.ModuleConfiguration.ModuleTitle = Localization.GetString("msgCreateBlog", LocalResourceFile)
   Me.cmdGenerateLinks.Enabled = False
  Else
   Me.ModuleConfiguration.ModuleTitle = Localization.GetString("msgEditBlog", LocalResourceFile)
   Me.cmdGenerateLinks.Enabled = True
  End If

 End Sub

 Protected Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

  Try

   If Not Page.IsPostBack Then
    pnlComments.Visible = (Settings.CommentMode = Constants.CommentMode.Default)

    If Not Settings.AllowWLW Then
     lblMetaWeblogUrl.Visible = False
     lblMetaWeblogNotAvailable.Visible = True
    Else
     lblMetaWeblogUrl.Visible = True
     lblMetaWeblogUrl.Text = "http://" & Request.Url.Host & ControlPath & "blogpost.ashx?tabid=" & TabId
     lblMetaWeblogNotAvailable.Visible = False
    End If

    If Not Blog Is Nothing Then
     txtTitle.Text = Blog.Title
     txtDescription.Text = Blog.Description
     chkPublic.Checked = Blog.Public
     rdoUserName.Items.Findue(Blog.ShowFullName.ToString()).Selected = True
     ddlAuthorMode.SelectedValue = Blog.AuthorMode.ToString

     chkAllowComments.Checked = Blog.AllowComments
     chkSyndicate.Checked = Blog.Syndicated
     If Blog.SyndicationEmail Is Nothing Then
      txtSyndicationEmail.Text = ModuleContext.PortalSettings.UserInfo.Email
     Else
      txtSyndicationEmail.Text = Blog.SyndicationEmail
     End If
     cmdDelete.Visible = True
     If Not rdoUserName.SelectedItem Is Nothing Then rdoUserName.SelectedItem.Selected = False

    End If

    If Not Request.UrlReferrer Is Nothing Then
     ViewState("URLReferrer") = Request.UrlReferrer.ToString
    End If

    hlCancel.NavigateUrl = ModuleContext.NavigateUrl(ModuleContext.TabId, "", False, "")
   End If
  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 ''' <summary>
 ''' 
 ''' </summary>
 ''' <param name="sender"></param>
 ''' <param name="e"></param>
 ''' <remarks></remarks>
 Protected Sub cmdDelete_Click(sender As Object, e As EventArgs) Handles cmdDelete.Click
  Try
   If Not Blog Is Nothing Then
    BlogController.DeleteBlog(Blog.BlogID, ModuleContext.PortalId)
   End If
   Response.Redirect(NavigateURL(), True)
  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 ''' <summary>
 ''' 
 ''' </summary>
 ''' <param name="sender"></param>
 ''' <param name="e"></param>
 ''' <remarks></remarks>
 Protected Sub cmdUpdate_Click(sender As Object, e As EventArgs) Handles cmdUpdate.Click
  If UpdateBlog() Then
   Response.Redirect(Utility.AddTOQueryString(NavigateURL(), "BlogID", Blog.BlogID.ToString()), True)
  End If
 End Sub

 ''' <summary>
 ''' 
 ''' </summary>
 ''' <param name="sender"></param>
 ''' <param name="e"></param>
 ''' <remarks></remarks>
 Protected Sub cmdGenerateLinks_Click(sender As System.Object, e As System.EventArgs) Handles cmdGenerateLinks.Click
  Utility.CreateAllEntryLinks(PortalId, Blog.BlogID)
 End Sub

#End Region

#Region "Private Methods"

 Private Function UpdateBlog() As Boolean
  Try
   If Page.IsValid = True Then
    If Blog Is Nothing Then
     Blog = New BlogInfo
     With Blog
      .UserID = Me.UserId
      .PortalID = Me.PortalId
     End With
    End If
    With Blog
     .Title = txtTitle.Text
     .Description = txtDescription.Text
     .Public = chkPublic.Checked
     .ShowFullName = CType(rdoUserName.SelectedItem.Value, Boolean)
     .AuthorMode = Convert.ToInt32(ddlAuthorMode.SelectedItem.Value)
     .AllowComments = chkAllowComments.Checked
     .Syndicated = chkSyndicate.Checked
     .SyndicationEmail = txtSyndicationEmail.Text
     If Null.IsNull(Blog.BlogID) Then
      .BlogID = BlogController.AddBlog(Blog)
      'If .Syndicated Then Blog = BlogController.GetBlog(.BlogID)
     Else
      BlogController.UpdateBlog(Blog)
     End If
    End With
    Return True
   End If
  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
  Return False
 End Function

#End Region

End Class