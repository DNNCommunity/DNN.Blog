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
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Modules.Blog.Business
Imports DotNetNuke.Services.Exceptions.Exceptions



Partial Class Blog
 Inherits BlogModuleBase
 Implements Entities.Modules.IActionable

#Region "Controls"
#End Region

#Region "Event Handlers"
 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  Try
   Dim objBlogs As New BlogController
   Dim objBlog As BlogInfo
   Dim m_PersonalBlogID As Integer = BlogSettings.PageBlogs

   If Not Page.IsPostBack Then

    ' 11/19/2008 Rip Rowan replaced deprecated code
    'If DotNetNuke.Security.PortalSecurity.HasEditPermissions(Me.ModuleId) Then
    If DotNetNuke.Security.PortalSecurity.HasNecessaryPermission(Security.SecurityAccessLevel.Edit, PortalSettings, ModuleConfiguration) Then
     objBlog = objBlogs.GetBlogByUserID(Me.PortalId, Me.UserId)
     If objBlog Is Nothing Then
      pnlBlog.Visible = True
      pnlExistingBlog.Visible = False
      lblLogin.Visible = False
      lnkBlog.Visible = True
     Else
      pnlBlog.Visible = False
      pnlExistingBlog.Visible = True
      ViewState("BlogID") = objBlog.BlogID
     End If
    Else
     If m_PersonalBlogID = -1 Then
      pnlBlog.Visible = True
     Else
      pnlBlog.Visible = False
     End If
     pnlExistingBlog.Visible = False
     lblLogin.Visible = True
     lnkBlog.Visible = False
    End If
   End If
  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub
#End Region

#Region "Private Methods"
 Private Sub lnkBlog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lnkBlog.Click
  Response.Redirect(EditUrl("BlogID", "-1", "Edit_Blog"))
 End Sub

 Private Sub lnkEditBlog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lnkEditBlog.Click
  Response.Redirect(EditUrl("BlogID", CType(ViewState("BlogID"), String), "Edit_Blog"))
 End Sub

 Private Sub lnkViewBlog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lnkViewBlog.Click
  Response.Redirect(NavigateURL(Me.TabId, "", "BlogID=" & CType(ViewState("BlogID"), String)))
 End Sub

 Private Sub lnkAddEntry_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lnkAddEntry.Click
  Response.Redirect(EditUrl("", "", "Edit_Entry"))
 End Sub

#End Region

#Region " Web Form Designer Generated Code "

 'This call is required by the Web Form Designer.
 <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

 End Sub

 'NOTE: The following placeholder declaration is required by the Web Form Designer.
 'Do not delete or move it.
 Private designerPlaceholderDeclaration As System.Object

 Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
  'CODEGEN: This method call is required by the Web Form Designer
  'Do not modify it using the code editor.
  InitializeComponent()
  Me.ModuleConfiguration.SupportedFeatures = 0
 End Sub

#End Region

#Region "public properties"
 Public ReadOnly Property ModuleActions() As Entities.Modules.Actions.ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
  Get
   Return MyActions
  End Get
 End Property
#End Region

End Class




