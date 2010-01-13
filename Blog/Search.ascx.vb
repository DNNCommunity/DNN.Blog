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



Partial Class Search
 Inherits BlogModuleBase

#Region " Private Members "
 Private BlogID As Integer = -1
 Private m_PersonalBlogID As Integer
#End Region

#Region "Controls"
#End Region

#Region "Event Handlers"
 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  Try
   If Not Request.Params("BlogID") Is Nothing Then
    BlogID = CType(Request.Params("BlogID"), Integer)
   End If
   If Not Page.IsPostBack Then
    Dim objBlogs As New BlogController
    Dim list As ArrayList

    m_PersonalBlogID = BlogSettings.PageBlogs
    If m_PersonalBlogID <> -1 Then
     BlogID = m_PersonalBlogID
    End If
    If m_PersonalBlogID <> -1 Then
     Dim m_oBlog As New BlogInfo
     m_oBlog = objBlogs.GetBlog(m_PersonalBlogID)
     list = New ArrayList
     list.Add(m_oBlog)
    Else
     list = objBlogs.ListBlogs(PortalId, -1, DotNetNuke.Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleId.ToString()))
    End If

    If list.Count < 2 Then
     cboBlogSelect.Visible = False
    Else
    cboBlogSelect.DataTextField = "Title"
    cboBlogSelect.DataValueField = "BlogID"
    cboBlogSelect.DataSource = list
    cboBlogSelect.DataBind()
    If m_PersonalBlogID = -1 Then
     cboBlogSelect.Items.Insert(0, New System.Web.UI.WebControls.ListItem(Localization.GetString("lstViewAllBlogs", LocalResourceFile), "-1"))
    End If
    If Not cboBlogSelect.Items.FindByValue(BlogID.ToString()) Is Nothing Then
     cboBlogSelect.Items.FindByValue(BlogID.ToString()).Selected = True
    End If
    End If

    If Not Request.Params("SearchType") Is Nothing Then
     If Request.Params("SearchType") = "Phrase" Then
      optSearchType.Items.FindByValue("Phrase").Selected = True
     Else
      optSearchType.Items.FindByValue("Keyword").Selected = True
     End If
    End If
    If Not Request.Params("Search") Is Nothing Then
     txtSearch.Text = Request.Params("Search")
    End If
    txtSearch.Attributes.Add("onkeypress", "if (event.keyCode == 13) {document.forms[0]." & btnSearch.ClientID & ".click();event.keyCode = 0; return false;}")
   End If

  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub
#End Region

#Region "Private Methods"
 Private Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
  StartSearch()
 End Sub

 Private Sub StartSearch()
  Dim sURL As String = Utility.AddTOQueryString(NavigateURL(), "Search", txtSearch.Text)
  sURL = Utility.AddTOQueryString(sURL, "SearchType", optSearchType.SelectedValue)
  If Not cboBlogSelect.SelectedItem Is Nothing Then
   If Int32.Parse(cboBlogSelect.SelectedItem.Value) > -1 Then sURL = sURL & "&BlogID=" & cboBlogSelect.SelectedItem.Value
  End If
  Response.Redirect(sURL)
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

End Class


