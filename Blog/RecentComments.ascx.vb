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
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization

Partial Public Class RecentComments
  Inherits BlogModuleBase

  Private _settings As Settings.RecentCommentsSettings

  Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    _settings = DotNetNuke.Modules.Blog.Settings.RecentCommentsSettings.GetRecentCommentsSettings(TabModuleId)
    LoadRecentComments()
  End Sub

  Private Sub LoadRecentComments()
    Dim RecentComments As List(Of CommentInfo) = Nothing
    Dim oController As CommentController = Nothing
    Dim strTemplate As String
    Dim strBuilder As StringBuilder = Nothing
    Dim strRecentEntries As String = Nothing
    Try
      oController = New CommentController

      If Request.QueryString("BlogID") IsNot Nothing Then
        RecentComments = oController.ListCommentsByBlog(CInt(Request.QueryString("BlogID")), False, _settings.RecentCommentsMax)
      Else
        RecentComments = oController.ListCommentsByPortal(PortalId, False, _settings.RecentCommentsMax)
      End If

      If RecentComments IsNot Nothing AndAlso RecentComments.Count > 0 Then
        strBuilder = New StringBuilder
        For Each Comment As CommentInfo In RecentComments

          strTemplate = _settings.RecentCommentsTemplate
          strTemplate = strTemplate.Replace("[PERMALINK]", Utility.GenerateEntryLink(PortalId, Comment.EntryID, TabId, Nothing) & "#Comments")
          strTemplate = ProcessTemplate(Comment, strTemplate)

          strBuilder.Append(strTemplate)
        Next
      Else
        UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("MsgNoRecentComments", LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
      End If

      ' assign the content
      If strBuilder IsNot Nothing Then
        Me.RecentComments.Controls.Add(New LiteralControl(strBuilder.ToString))
      End If

    Catch ex As Exception
      ProcessModuleLoadException(Me, ex)
    Finally
      If RecentComments IsNot Nothing Then RecentComments = Nothing
      If oController IsNot Nothing Then oController = Nothing
      If strBuilder IsNot Nothing Then strBuilder = Nothing
    End Try
  End Sub

  Private Function ProcessTemplate(ByVal objComment As CommentInfo, ByVal strTemplate As String) _
      As String
    Dim TemplateManager As TemplateManager = Nothing
    Try
      TemplateManager = New TemplateManager(objComment)
      Return TemplateManager.ProcessTemplate(strTemplate)
    Catch ex As Exception
      ProcessModuleLoadException(Me, ex)
      Return Nothing
    Finally
      If TemplateManager IsNot Nothing Then TemplateManager = Nothing
    End Try
  End Function

End Class