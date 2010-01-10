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
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization

Partial Public Class ViewTags
 'Inherits BlogModuleBase
 Inherits DotNetNuke.Entities.Modules.PortalModuleBase

 Private _settings As Settings.TagViewSettings

 Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
  Me.ModuleConfiguration.SupportedFeatures = 0
  _settings = DotNetNuke.Modules.Blog.Settings.TagViewSettings.GetTagViewSettings(TabModuleId)
 End Sub

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
  Dim TagController As New TagController
  Dim TagList As ArrayList
  Dim tag As TagInfo
  Dim TagDisplayMode As String
  TagDisplayMode = _settings.TagDisplayMode

  If TagDisplayMode = "List" Then
   TagList = TagController.ListTags(PortalId)
   For Each tag In TagList
    Dim a As New HtmlAnchor()
    a.HRef = Utility.GetSEOLink(PortalId, TabId, "", tag.Slug, "tagid=" + tag.TagId.ToString)
    a.InnerText = tag.Tag + " (" + tag.Cnt.ToString + ")"
    a.Title = tag.Tag
    phTags.Controls.Add(a)
    phTags.Controls.Add(New LiteralControl("<br />"))
   Next
  Else
   TagList = TagController.ListWeightedTags(PortalId)
   For Each tag In TagList
    Dim a As New HtmlAnchor()
    a.HRef = Utility.GetSEOLink(PortalId, TabId, "", tag.Slug, "tagid=" + tag.TagId.ToString)
    a.InnerText = tag.Tag
    a.Title = tag.Tag
    a.Attributes("class") = "TagCloud" + tag.Weight.ToString
    phTags.Controls.Add(a)
    phTags.Controls.Add(New LiteralControl(" "))
   Next
  End If
 End Sub


End Class

