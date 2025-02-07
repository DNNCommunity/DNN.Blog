'
' DNN Connect - http://dnn-connect.org
' Copyright (c) 2015
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
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Modules.Blog.Core.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Core.Entities.Posts
Imports DotNetNuke.Modules.Blog.Core.Entities.Terms


Namespace Controls
  Public Class ViewSettings
    Inherits ModuleSettingsBase

#Region " Properties "
    Private Property BlogModuleId As Integer = -1
    Private _viewSettings As Core.Common.ViewSettings
    Public Property ViewSettings() As Core.Common.ViewSettings
      Get
        If _viewSettings Is Nothing Then _viewSettings = Core.Common.ViewSettings.GetViewSettings(TabModuleId)
        Return _viewSettings
      End Get
      Set(ByVal value As Core.Common.ViewSettings)
        _viewSettings = value
      End Set
    End Property
#End Region

#Region " Page Events "
    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
      Try
        ctlCategories.ModuleConfiguration = ModuleConfiguration
        If Not IsPostBack Then
          If ViewSettings.BlogModuleId > -1 Then
            BlogModuleId = ViewSettings.BlogModuleId
          Else
            BlogModuleId = ModuleId
          End If
          ctlCategories.VocabularyId = Core.Common.ModuleSettings.GetModuleSettings(BlogModuleId).VocabularyId
        End If
      Catch ex As Exception
      End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
    End Sub

    Private Sub ddBlogModuleId_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddBlogModuleId.SelectedIndexChanged
      BlogModuleId = CInt(ddBlogModuleId.SelectedValue)
      ctlCategories.VocabularyId = Core.Common.ModuleSettings.GetModuleSettings(BlogModuleId).VocabularyId
      LoadDropdowns()
    End Sub
#End Region

#Region " Private Methods "
    Private Sub LoadDropdowns()
      ddBlogId.Items.Clear()
      ddBlogId.DataSource = BlogsController.GetBlogsByModule(BlogModuleId, UserId, Threading.Thread.CurrentThread.CurrentCulture.Name).Values.Where(Function(b)
                                                                                                                                                      Return b.Published = True
                                                                                                                                                    End Function).OrderBy(Function(b) b.Title)
      ddBlogId.DataBind()
      ddBlogId.Items.Insert(0, New ListItem(LocalizeString("All"), "-1"))

      ddAuthorId.DataSource = PostsController.GetAuthors(BlogModuleId, -1).OrderBy(Function(t) t.DisplayName)
      ddAuthorId.DataBind()
      ddAuthorId.Items.Insert(0, New ListItem(LocalizeString("All"), "-1"))
    End Sub
#End Region

#Region " Base Method Implementations "
    Public Overrides Sub LoadSettings()

      If Not IsPostBack Then

        ddTemplate.Items.Clear()
        ddTemplate.Items.Add(New ListItem("Default [System]", "[G]_default"))
        For Each d As IO.DirectoryInfo In (New IO.DirectoryInfo(HttpContext.Current.Server.MapPath(DotNetNuke.Common.ResolveUrl(Core.Common.Globals.glbTemplatesPath)))).GetDirectories
          If d.Name <> "_default" Then
            ddTemplate.Items.Add(New ListItem(d.Name & " [System]", "[G]" & d.Name))
          End If
        Next
        For Each d As IO.DirectoryInfo In (New IO.DirectoryInfo(Core.Common.ModuleSettings.GetModuleSettings(BlogModuleId).PortalTemplatesMapPath)).GetDirectories
          ddTemplate.Items.Add(New ListItem(d.Name & " [Local]", "[P]" & d.Name))
        Next
        Dim skinTemplatePath As String = Server.MapPath(DotNetNuke.UI.Skins.Skin.GetSkin(CType(Page, Framework.PageBase)).SkinPath) & "Templates\Blog\"
        If IO.Directory.Exists(skinTemplatePath) Then
          For Each d As IO.DirectoryInfo In (New IO.DirectoryInfo(skinTemplatePath)).GetDirectories
            ddTemplate.Items.Add(New ListItem(d.Name & " [Skin]", "[S]" & d.Name))
          Next
        End If

        ddBlogModuleId.Items.Clear()

        Dim listOfValidBlogModules As List(Of ModuleInfo) = New List(Of ModuleInfo)
        Dim tabController As DotNetNuke.Entities.Tabs.TabController = New DotNetNuke.Entities.Tabs.TabController()

        For Each blogModule As ModuleInfo In (New ModuleController).GetModulesByDefinition(PortalId, "DNNBlog.Blog")
          Dim blogPage As DotNetNuke.Entities.Tabs.TabInfo = tabController.GetTab(blogModule.TabID, PortalId, False)
          blogModule.ModuleTitle = String.Concat(blogPage.TabName, " - ", blogModule.ModuleTitle)

          Dim targetViewSetting As Core.Common.ViewSettings = Core.Common.ViewSettings.GetViewSettings(blogModule.TabModuleID)
          If CBool(targetViewSetting.BlogModuleId = -1) Then
            listOfValidBlogModules.Add(blogModule)
          End If
        Next

        ddBlogModuleId.DataSource = listOfValidBlogModules
        ddBlogModuleId.DataBind()

        Try
          ddBlogModuleId.Items.Remove(ddBlogModuleId.Items.FindByValue(ModuleId.ToString))
        Catch ex As Exception
        End Try
        ddBlogModuleId.Items.Insert(0, New ListItem(LocalizeString("NoParent"), "-1"))
        Try
          ddBlogModuleId.Items.FindByValue(ViewSettings.BlogModuleId.ToString).Selected = True
        Catch ex As Exception
        End Try

        LoadDropdowns()

        Try
          ddBlogId.Items.FindByValue(ViewSettings.BlogId.ToString).Selected = True
        Catch ex As Exception
        End Try

        ' New categories control
        Dim cId As Integer
        Dim SelectedCategories As New List(Of TermInfo)
        Dim catList As String = ViewSettings.Categories
        If Not String.IsNullOrEmpty(catList) Then
          For Each c As String In catList.Split(","c)
            If IsNumeric(c) Then
              cId = Integer.Parse(c)
              Dim cat As New TermInfo
              cat.TermId = cId
              SelectedCategories.Add(cat)
            End If
          Next
        End If
        ctlCategories.SelectedCategories = SelectedCategories

        Try
          ddAuthorId.Items.FindByValue(ViewSettings.AuthorId.ToString).Selected = True
        Catch ex As Exception
        End Try

      End If

      chkShowAllLocales.Checked = ViewSettings.ShowAllLocales
      chkModifyPageDetails.Checked = ViewSettings.ModifyPageDetails
      chkAddCanonicalTag.Checked = ViewSettings.AddCanonicalTag
      chkShowManagementPanel.Checked = ViewSettings.ShowManagementPanel
      chkShowManagementPanelViewMode.Checked = ViewSettings.ShowManagementPanelViewMode

      chkHideUnpublishedBlogsViewMode.Checked = ViewSettings.HideUnpublishedBlogsViewMode
      chkHideUnpublishedBlogsEditMode.Checked = ViewSettings.HideUnpublishedBlogsEditMode

      chkAllowComments.Checked = ViewSettings.AllowComments

      Try
        ddTemplate.Items.FindByValue(ViewSettings.Template).Selected = True
      Catch ex As Exception
      End Try

    End Sub

    Public Overrides Sub UpdateSettings()

      ViewSettings.BlogModuleId = CInt(ddBlogModuleId.SelectedValue)
      ViewSettings.ShowAllLocales = chkShowAllLocales.Checked
      ViewSettings.ModifyPageDetails = chkModifyPageDetails.Checked
      ViewSettings.AddCanonicalTag = chkAddCanonicalTag.Checked
      ViewSettings.ShowManagementPanel = chkShowManagementPanel.Checked
      ViewSettings.ShowManagementPanelViewMode = chkShowManagementPanelViewMode.Checked
      ViewSettings.HideUnpublishedBlogsViewMode = chkHideUnpublishedBlogsViewMode.Checked
      ViewSettings.HideUnpublishedBlogsEditMode = chkHideUnpublishedBlogsEditMode.Checked
      ViewSettings.AllowComments = chkAllowComments.Checked
      ViewSettings.BlogId = CInt(ddBlogId.SelectedValue)
      ViewSettings.Categories = ctlCategories.ToString
      ViewSettings.AuthorId = CInt(ddAuthorId.SelectedValue)
      ViewSettings.Template = ddTemplate.SelectedValue
      ViewSettings.UpdateSettings()

    End Sub
#End Region

  End Class
End Namespace