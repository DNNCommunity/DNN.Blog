Imports DotNetNuke.Modules.Blog.Entities.Posts
Imports DotNetNuke.Modules.Blog.Common.Globals

Namespace Common
 Public Class PostBodyAndSummary

  Public Property Body As String = ""
  Public Property Summary As String = ""

#Region " Constructors "
  Public Sub New(body As String)
   Me.Body = body
  End Sub

  Public Sub New(Post As PostInfo, summaryModel As SummaryType)
   Body = HttpUtility.HtmlDecode(Post.Content)
   If summaryModel = SummaryType.PlainTextIndependent Then
    Summary = Post.Summary
   Else
    Summary = HttpUtility.HtmlDecode(Post.Summary)
   End If
   If summaryModel = SummaryType.HtmlPrecedesPost Then
    Body = Body.Substring(Summary.Length)
   End If
  End Sub

  Public Sub New(post As Services.WLW.MetaWeblog.Post, summaryModel As SummaryType)
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
  End Sub
#End Region

#Region " Writing "
  Public Sub WriteToPost(ByRef Post As PostInfo, summaryModel As SummaryType, htmlEncode As Boolean)
   If htmlEncode Then
    Body = HttpUtility.HtmlEncode(Body)
    If Not summaryModel = SummaryType.PlainTextIndependent Then
     Summary = HttpUtility.HtmlEncode(Summary)
    End If
   End If
   If Summary = "&lt;p&gt;&amp;#160;&lt;/p&gt;" Then Summary = "" ' an empty editor in DNN returns this
   If summaryModel = SummaryType.PlainTextIndependent Then Summary = RemoveHtmlTags(Summary)
   If summaryModel = SummaryType.HtmlPrecedesPost Then
    Body = Summary & Body
   End If
   Post.Content = Body
   Post.Summary = Summary
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
