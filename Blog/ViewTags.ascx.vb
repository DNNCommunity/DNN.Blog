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

Imports DotNetNuke.Modules.Blog.Components.Controllers
Imports DotNetNuke.Modules.Blog.Components.Common
Imports DotNetNuke.Web.Client.ClientResourceManagement
Imports DotNetNuke.Framework
Imports DotNetNuke.Modules.Blog.Components.Settings
Imports DotNetNuke.Modules.Blog.Components.Entities
Imports Telerik.Web.UI

Partial Public Class ViewTags
    Inherits DotNetNuke.Entities.Modules.PortalModuleBase

    Private _settings As TagViewSettings
    Protected WithEvents rtcTags As DotNetNuke.Web.UI.WebControls.DnnTagCloud

#Region "Event Handlers"

    Protected Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        jQuery.RequestUIRegistration()
        ClientResourceManager.RegisterScript(Page, TemplateSourceDirectory + "/js/jquery.qatooltip.js")
        ClientResourceManager.RegisterScript(Page, "~/Resources/Shared/Scripts/jquery/jquery.hoverIntent.min.js")

        _settings = TagViewSettings.GetTagViewSettings(TabModuleId)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim TagDisplayMode As String
        TagDisplayMode = _settings.TagDisplayMode

        Dim cntTerm As New TermController
        Dim colTags As List(Of TermInfo)
        colTags = cntTerm.GetTermsByContentType(ModuleContext.PortalId, 1) ' the 1 ensures tags only

        If TagDisplayMode = "List" Then
            rptTags.DataSource = colTags
            rptTags.DataBind()
            pnlTagList.Visible = True
            pnlTagCloud.Visible = False
        Else
            rtcTags.DataSource = colTags
            rtcTags.DataBind()
            pnlTagList.Visible = False
            pnlTagCloud.Visible = True
        End If
    End Sub

    Protected Sub RptTagsItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        Dim tagControl As Tags = DirectCast(e.Item.FindControl("dbaSingleTag"), Tags)
        Dim term As TermInfo = DirectCast(e.Item.DataItem, TermInfo)
        Dim colTerms As New List(Of TermInfo)

        If term IsNot Nothing Then
            colTerms.Add(term)
        End If

        tagControl.ModContext = ModuleContext
        tagControl.DataSource = colTerms
        tagControl.CountMode = Constants.TagMode.ShowTotalUsage
        tagControl.DataBind()
    End Sub

    Protected Sub RtcCloudItemDataBound(sender As Object, e As RadTagCloudEventArgs)
        Dim term As TermInfo = DirectCast(e.Item.DataItem, TermInfo)
        Dim cloudLink As RadTagCloudItem = e.Item
        Dim link As String = DotNetNuke.Common.NavigateURL(ModuleContext.TabId, "", "tagid=" & term.TermId)

        cloudLink.NavigateUrl = link
    End Sub

#End Region

End Class