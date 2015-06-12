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

Imports System
Imports System.Data
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Localization.Localization
Imports DotNetNuke.Services.Tokens

Imports DotNetNuke.Modules.Blog.Data
Imports DotNetNuke.Modules.Blog.Integration
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Entities.Posts
Imports DotNetNuke.Modules.Blog.Security.Permissions
Imports DotNetNuke.Modules.Blog.Common.Globals

Namespace Entities.Comments

 Partial Public Class CommentsController

  Public Shared Function GetComment(commentID As Int32, userId As Integer) As CommentInfo

   Return CType(CBO.FillObject(DataProvider.Instance().GetComment(commentID, userId), GetType(CommentInfo)), CommentInfo)

  End Function

  Public Shared Function GetCommentsByModule(moduleId As Int32, userID As Int32, pageIndex As Int32, pageSize As Int32, orderBy As String, ByRef totalRecords As Integer) As Dictionary(Of Integer, CommentInfo)

   If pageIndex < 0 Then
    pageIndex = 0
    pageSize = Integer.MaxValue
   End If

   Dim res As New Dictionary(Of Integer, CommentInfo)
   Using ir As IDataReader = DataProvider.Instance().GetCommentsByModuleId(moduleId, userID, pageIndex, pageSize, orderBy)
    res = DotNetNuke.Common.Utilities.CBO.FillDictionary(Of Integer, CommentInfo)("CommentId", ir, False)
    ir.NextResult()
    totalRecords = DotNetNuke.Common.Globals.GetTotalRecords(ir)
   End Using
   Return res

  End Function

  Public Shared Function AddComment(blog As BlogInfo, Post As PostInfo, ByRef comment As CommentInfo) As Integer

   AddComment(comment, PortalSettings.Current.UserId)
   If comment.Approved Then
    JournalController.AddOrUpdateCommentInJournal(blog, Post, comment, PortalSettings.Current.PortalId, PortalSettings.Current.ActiveTab.TabID, PortalSettings.Current.UserId, Post.PermaLink(PortalSettings.Current))
   Else
    Dim title As String = GetString("CommentPendingNotify.Subject", SharedResourceFileName)
    Dim summary As String = String.Format(GetString("CommentPendingNotify.Body", SharedResourceFileName), Post.PermaLink(PortalSettings.Current), comment.CommentID, Post.Title, comment.Comment)
    NotificationController.CommentPendingApproval(comment, blog, Post, PortalSettings.Current.PortalId, summary, title)
   End If
   Return comment.CommentID

  End Function

  Public Shared Sub UpdateComment(blog As BlogInfo, Post As PostInfo, comment As CommentInfo)

   UpdateComment(comment, PortalSettings.Current.UserId)
   NotificationController.RemoveCommentPendingNotification(blog.ModuleID, blog.BlogID, comment.ContentItemId, comment.CommentID)
   If comment.Approved Then
    JournalController.AddOrUpdateCommentInJournal(blog, Post, comment, PortalSettings.Current.PortalId, PortalSettings.Current.ActiveTab.TabID, PortalSettings.Current.UserId, Post.PermaLink(PortalSettings.Current))
   Else
    Dim title As String = GetString("CommentPendingNotify.Subject", SharedResourceFileName)
    Dim summary As String = String.Format(GetString("CommentPendingNotify.Body", SharedResourceFileName), Post.PermaLink(PortalSettings.Current), comment.CommentID, Post.Title, comment.Comment)
    NotificationController.CommentPendingApproval(comment, blog, Post, PortalSettings.Current.PortalId, summary, title)
   End If

  End Sub

  Public Shared Sub ApproveComment(moduleId As Integer, blogId As Integer, comment As CommentInfo)

   Data.DataProvider.Instance.ApproveComment(comment.CommentID)
   NotificationController.RemoveCommentPendingNotification(moduleId, blogId, comment.ContentItemId, comment.CommentID)

  End Sub

  Public Shared Sub DeleteComment(moduleId As Integer, blogId As Integer, comment As CommentInfo)

   DataProvider.Instance().DeleteComment(comment.CommentID)
   NotificationController.RemoveCommentPendingNotification(moduleId, blogId, comment.ContentItemId, comment.CommentID)

  End Sub

  Public Shared Function CommentKarma(callingModule As ModuleInfo, comment As CommentInfo, user As DotNetNuke.Entities.Users.UserInfo, karma As Integer) As Integer

   If user.UserID < 0 Then Return -1
   Dim ret As Integer = Data.DataProvider.Instance.AddCommentKarma(comment.CommentID, user.UserID, karma)
   If ret > -1 Then
    If karma = 2 Then ' reporting comment as inappropriate
     Dim title As String = String.Format(GetString("CommentReportedNotify", SharedResourceFileName), user.DisplayName)
     Dim post As PostInfo = PostsController.GetPost(comment.ContentItemId, callingModule.ModuleID, "")
     Dim summary As String = String.Format(GetString("CommentReportedNotify.Body", SharedResourceFileName), user.DisplayName, comment.DisplayName, comment.Comment, post.PermaLink(PortalSettings.Current), post.Title)
     NotificationController.ReportComment(comment, post.Blog, post, callingModule.PortalID, summary, title)
    End If
    Return ret
   End If
   Return 0

  End Function

 End Class
End Namespace

