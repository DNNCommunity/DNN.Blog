Imports System
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Modules.Blog.Business
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization




Partial Public Class ViewTags
 Inherits BlogModuleBase

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
  Dim TagController As New TagController
  Dim TagList As ArrayList
  Dim tag As TagInfo
  Dim TagDisplayMode As String
  If CType(Settings("TagDisplayMode"), String) Is Nothing Then
   TagDisplayMode = "Cloud"
  Else
   TagDisplayMode = CType(Settings("TagDisplayMode"), String)
  End If

  If TagDisplayMode = "List" Then
   TagList = TagController.ListTags(PortalId)
   For Each tag In TagList
    Dim a As New HtmlAnchor()
    a.HRef = NavigateURL(Me.TabId, "", "tagid=" + tag.TagID.ToString)
    a.InnerText = tag.Tag + " (" + tag.Cnt.ToString + ")"
    a.Title = tag.Tag
    phTags.Controls.Add(a)
    phTags.Controls.Add(New LiteralControl("<br />"))
   Next
  Else
   TagList = TagController.ListWeightedTags(PortalId)
   For Each tag In TagList
    Dim a As New HtmlAnchor()
    a.HRef = NavigateURL(Me.TabId, "", "tagid=" + tag.TagID.ToString)
    a.InnerText = tag.Tag
    a.Title = tag.Tag
    a.Attributes("class") = "TagCloud" + tag.Weight.ToString
    phTags.Controls.Add(a)
    phTags.Controls.Add(New LiteralControl(" "))
   Next
  End If
 End Sub

 Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
  Me.ModuleConfiguration.SupportedFeatures = 0
 End Sub

End Class

