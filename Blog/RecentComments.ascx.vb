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
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports System.Reflection
Imports DotNetNuke.Security

Partial Public Class RecentComments
 Inherits BlogModuleBase
 'Implements Entities.Modules.IActionable

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
  LoadRecentComments()
 End Sub

 'Private Sub LoadRecentComments()
 '    Dim RecentComments As List(Of CommentInfo) = Nothing
 '    Dim oController As CommentController = Nothing
 '    Dim strTemplate As String
 '    Dim strBuilder As StringBuilder = Nothing
 '    Dim strRecentEntries As String = Nothing
 '    Try
 '        oController = New CommentController

 '        If Request.QueryString("BlogID") IsNot Nothing Then
 '            RecentComments = oController.ListCommentsByBlog(CInt(Request.QueryString("BlogID")), True, CType(Settings("RecentEntriesMax"), Integer))
 '        Else
 '            RecentComments = oController.ListCommentsByPortal(PortalId, True, CType(Settings("RecentEntriesMax"), Integer))
 '        End If

 '        If RecentComments IsNot Nothing AndAlso RecentComments.Count > 0 Then
 '            strBuilder = New StringBuilder
 '            For Each Comment As CommentInfo In RecentComments
 '                Dim objProperties As ArrayList = Common.Utilities.CBO.GetPropertyInfo(GetType(CommentInfo))
 '                Dim intProperty As Integer
 '                Dim objPropertyInfo As PropertyInfo

 '                strTemplate = CType(Settings("RecentCommentsTemplate"), String)
 '                If strTemplate = String.Empty Then strTemplate = Localization.GetString("DefaultRecentCommentsTemplate", BLOG_TEMPLATES_RESOURCE)

 '                For intProperty = 0 To objProperties.Count - 1
 '                    objPropertyInfo = CType(objProperties(intProperty), PropertyInfo)
 '                    Select Case objPropertyInfo.Name.ToLower
 '                        Case "comment"
 '                            strTemplate = strTemplate.Replace("{comment}", DotNetNuke.Common.Utilities.HtmlUtils.Shorten(DataBinder.Eval(Comment, objPropertyInfo.Name).ToString(), 35, "..."))
 '                        Case Else
 '                            strTemplate = strTemplate.Replace("{" & objPropertyInfo.Name.ToLower & "}", DataBinder.Eval(Comment, objPropertyInfo.Name).ToString())
 '                    End Select

 '                    strTemplate = strTemplate.Replace("{permalink}", Utility.GenerateEntryLink(PortalId, Comment.EntryID, TabId, Nothing) & "#Comments")

 '                Next intProperty
 '                strBuilder.Append(strTemplate)
 '            Next

 '        Else
 '            UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("MsgNoRecentComments", LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
 '        End If

 '        ' assign the content
 '        If strBuilder IsNot Nothing Then
 '            Dim lblContent As New Label
 '            lblContent.Text = strBuilder.ToString
 '            Me.RecentComments.Controls.Add(lblContent)
 '        End If

 '    Catch ex As Exception
 '        ProcessModuleLoadException(Me, ex)
 '    Finally
 '        If RecentComments IsNot Nothing Then RecentComments = Nothing
 '        If oController IsNot Nothing Then oController = Nothing
 '        If strBuilder IsNot Nothing Then strBuilder = Nothing
 '    End Try
 'End Sub

 Private Sub LoadRecentComments()
  Dim RecentComments As List(Of CommentInfo) = Nothing
  Dim oController As CommentController = Nothing
  Dim strTemplate As String
  Dim strBuilder As StringBuilder = Nothing
  Dim strRecentEntries As String = Nothing
  Try
   oController = New CommentController

   If Request.QueryString("BlogID") IsNot Nothing Then
    RecentComments = oController.ListCommentsByBlog(CInt(Request.QueryString("BlogID")), True, CType(Settings("RecentEntriesMax"), Integer))
   Else
    RecentComments = oController.ListCommentsByPortal(PortalId, True, CType(Settings("RecentEntriesMax"), Integer))
   End If

   If RecentComments IsNot Nothing AndAlso RecentComments.Count > 0 Then
    strBuilder = New StringBuilder
    For Each Comment As CommentInfo In RecentComments

     strTemplate = CType(Settings("RecentCommentsTemplate"), String)
     If strTemplate = String.Empty Then strTemplate = Localization.GetString("DefaultRecentCommentsTemplate", BLOG_TEMPLATES_RESOURCE)

     strTemplate = strTemplate.Replace("[PERMALINK]", Utility.GenerateEntryLink(PortalId, Comment.EntryID, TabId, Nothing) & "#Comments")

     strTemplate = ProcessTemplate(Comment, strTemplate)


     strBuilder.Append(strTemplate)
    Next
   Else
    UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("MsgNoRecentComments", LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
   End If

   ' assign the content
   If strBuilder IsNot Nothing Then
    Dim lblContent As New Label
    lblContent.Text = strBuilder.ToString
    Me.RecentComments.Controls.Add(lblContent)
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

#Region "Optional Interfaces"

 '''' -----------------------------------------------------------------------------
 '''' <summary>
 '''' Registers the module actions required for interfacing with the portal framework
 '''' </summary>
 '''' <value></value>
 '''' <returns></returns>
 '''' <remarks></remarks>
 '''' <history>
 '''' </history>
 '''' -----------------------------------------------------------------------------
 'Public ReadOnly Property ModuleActions() As Entities.Modules.Actions.ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
 '    Get
 '        Dim Actions As New Entities.Modules.Actions.ModuleActionCollection
 '        Actions.Add(GetNextActionID, Localization.GetString(Entities.Modules.Actions.ModuleActionType.AddContent, LocalResourceFile), Entities.Modules.Actions.ModuleActionType.AddContent, "", "", EditUrl(), False, Security.SecurityAccessLevel.Edit, True, False)
 '        Return Actions
 '    End Get
 'End Property

#End Region

End Class
