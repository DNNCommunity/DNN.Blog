'
' Bring2mind - http://www.bring2mind.net
' Copyright (c) 2013
' by Bring2mind
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

Imports DotNetNuke.Modules.Blog.Entities.Posts
Imports DotNetNuke.Modules.Blog.Common.Globals

Namespace Common
 Public Class PostBodyAndSummary

  Public Property Body As String = ""
  Public Property BodyLocalizations As New LocalizedText
  Public Property Summary As String = ""
  Public Property SummaryLocalizations As New LocalizedText

#Region " Constructors "
  Public Sub New(contentEditor As Controls.LongTextEdit, summaryEditor As Controls.LongTextEdit, summaryModel As SummaryType, includeLocalizations As Boolean, autoGenerateSummaryIfEmpty As Boolean, autoGenerateLength As Integer)
   Me.Body = contentEditor.DefaultText
   Me.Summary = Trim(summaryEditor.DefaultText)
   If Summary = "&lt;p&gt;&amp;#160;&lt;/p&gt;" Then Summary = "" ' an empty editor in DNN returns this
   If includeLocalizations Then
    Me.BodyLocalizations = contentEditor.LocalizedTexts
    Me.SummaryLocalizations = summaryEditor.LocalizedTexts
   End If
   Select Case summaryModel
    Case SummaryType.HtmlIndependent
    Case SummaryType.HtmlPrecedesPost
     Body = Summary & Body
     For Each l As String In Me.SummaryLocalizations.Locales
      Me.BodyLocalizations(l) = Me.SummaryLocalizations(l) & Me.BodyLocalizations(l)
     Next
    Case SummaryType.PlainTextIndependent
     Me.Summary = RemoveHtmlTags(Me.Summary)
     For Each l As String In Me.SummaryLocalizations.Locales
      Me.SummaryLocalizations(l) = RemoveHtmlTags(Me.SummaryLocalizations(l))
     Next
   End Select
   If autoGenerateSummaryIfEmpty And autoGenerateLength > 0 Then
    If Summary = "" Then Summary = GetSummary(Body, autoGenerateLength, summaryModel)
    For Each l As String In Me.SummaryLocalizations.Locales
     If SummaryLocalizations(l) = "" Then SummaryLocalizations(l) = GetSummary(BodyLocalizations(l), autoGenerateLength, summaryModel)
    Next
   End If
  End Sub

  Public Sub New(Post As PostInfo, summaryModel As SummaryType, includeLocalizations As Boolean, autoGenerateSummaryIfEmpty As Boolean, autoGenerateLength As Integer)
   Body = HttpUtility.HtmlDecode(Post.Content)
   If Body Is Nothing Then Body = ""
   If summaryModel = SummaryType.PlainTextIndependent Then
    Summary = Post.Summary
   Else
    Summary = HttpUtility.HtmlDecode(Post.Summary)
   End If
   If summaryModel = SummaryType.HtmlPrecedesPost Then
    Body = Body.Substring(Summary.Length)
   End If
   If includeLocalizations Then
    For Each l As String In Post.TitleLocalizations.Locales
     Dim lBody As String = ""
     If Post.ContentLocalizations.ContainsKey(l) Then lBody = Post.ContentLocalizations(l)
     Dim lSummary As String = ""
     If Post.SummaryLocalizations.ContainsKey(l) Then lSummary = Post.SummaryLocalizations(l)
     BodyLocalizations.Add(l, lBody)
     If summaryModel = SummaryType.PlainTextIndependent Then
      SummaryLocalizations.Add(l, lSummary)
     Else
      SummaryLocalizations.Add(l, HttpUtility.HtmlDecode(lSummary))
     End If
     If summaryModel = SummaryType.HtmlPrecedesPost Then
      BodyLocalizations(l) = BodyLocalizations(l).Substring(SummaryLocalizations(l).Length)
     End If
    Next
   End If
   If autoGenerateSummaryIfEmpty And autoGenerateLength > 0 Then
    If Summary = "" Then Summary = GetSummary(Body, autoGenerateLength, summaryModel)
    For Each l As String In Me.SummaryLocalizations.Locales
     If SummaryLocalizations(l) = "" Then SummaryLocalizations(l) = GetSummary(BodyLocalizations(l), autoGenerateLength, summaryModel)
    Next
   End If
  End Sub

  Public Sub New(post As Services.WLW.MetaWeblog.Post, summaryModel As SummaryType, autoGenerateSummaryIfEmpty As Boolean, autoGenerateLength As Integer)
   Select Case summaryModel
    Case SummaryType.HtmlIndependent
     Body = post.description
    Case SummaryType.HtmlPrecedesPost
     Body = post.mt_text_more
     Summary = post.description
    Case Else ' plain text
     Body = post.description
     Summary = post.mt_excerpt
   End Select
   If autoGenerateSummaryIfEmpty And autoGenerateLength > 0 Then
    If Summary = "" Then Summary = GetSummary(Body, autoGenerateLength, summaryModel)
   End If
  End Sub
#End Region

#Region " Writing "
  Public Sub WriteToPost(ByRef Post As PostInfo, summaryModel As SummaryType, htmlEncode As Boolean, includeLocalizations As Boolean)
   If htmlEncode Then
    Body = HttpUtility.HtmlEncode(Body)
    If Not summaryModel = SummaryType.PlainTextIndependent Then
     Summary = HttpUtility.HtmlEncode(Summary)
    End If
    If includeLocalizations Then
     For Each l As String In Me.SummaryLocalizations.Locales
      Me.BodyLocalizations(l) = HttpUtility.HtmlEncode(Me.BodyLocalizations(l))
      Me.SummaryLocalizations(l) = HttpUtility.HtmlEncode(Me.SummaryLocalizations(l))
     Next
    End If
   End If
   Post.Content = Body
   Post.Summary = Summary
   If includeLocalizations Then
    Post.ContentLocalizations = BodyLocalizations
    Post.SummaryLocalizations = SummaryLocalizations
   End If
  End Sub

  Public Sub WriteToPost(ByRef post As Services.WLW.MetaWeblog.Post, summaryModel As SummaryType)
   Select Case summaryModel
    Case SummaryType.HtmlIndependent
     post.description = Body
    Case SummaryType.HtmlPrecedesPost
     post.mt_text_more = Body
     post.description = Summary
    Case Else ' plain text
     post.description = Body
     post.mt_excerpt = Summary
   End Select
  End Sub
#End Region

 End Class
End Namespace
