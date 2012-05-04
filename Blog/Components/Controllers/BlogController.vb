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
Imports DotNetNuke.Modules.Blog.Data
Imports DotNetNuke.Common.Utilities

Namespace Business

 Public Class BlogController

  Public Function GetBlog(ByVal blogID As Integer) As BlogInfo
   Return CType(CBO.FillObject(DataProvider.Instance().GetBlog(blogID), GetType(BlogInfo)), BlogInfo)
  End Function

  Public Function GetBlogByUserID(ByVal PortalID As Integer, ByVal UserID As Integer) As BlogInfo
   Return CType(CBO.FillObject(DataProvider.Instance().GetBlogByUserID(PortalID, UserID), GetType(BlogInfo)), BlogInfo)
  End Function

  Public Function GetBlogByUserName(ByVal PortalID As Integer, ByVal UserName As String) As BlogInfo
   Return CType(CBO.FillObject(DataProvider.Instance().GetBlogByUserName(PortalID, UserName), GetType(BlogInfo)), BlogInfo)
  End Function

  Public Function GetBlogsByUserName(ByVal PortalID As Integer, ByVal UserName As String) As ArrayList
   Return CBO.FillCollection(DataProvider.Instance().GetBlogsByUserName(PortalID, UserName), GetType(BlogInfo))
  End Function

  Public Function ListBlogs(ByVal PortalID As Integer, ByVal ParentBlogID As Integer, ByVal ShowNonPublic As Boolean) As ArrayList
   Return CBO.FillCollection(DataProvider.Instance().ListBlogs(PortalID, ParentBlogID, ShowNonPublic), GetType(BlogInfo))
  End Function

  Public Function ListBlogsByPortal(ByVal PortalID As Integer, ByVal ShowNonPublic As Boolean) As ArrayList
   Return CBO.FillCollection(DataProvider.Instance().ListBlogsByPortal(PortalID, ShowNonPublic), GetType(BlogInfo))
  End Function

  Public Function ListBlogsRootByPortal(ByVal PortalID As Integer) As ArrayList
   Return CBO.FillCollection(DataProvider.Instance().ListBlogsRootByPortal(PortalID), GetType(BlogInfo))
  End Function

  Public Function AddBlog(ByVal objBlog As BlogInfo) As Integer
   With objBlog
                Return CType(DataProvider.Instance().AddBlog(.PortalID, .ParentBlogID, .UserID, .Title, .Description, .Public, .AllowComments, .AllowAnonymous, .ShowFullName, .Culture, .DateFormat, .TimeZone.Id, .Syndicated, .SyndicateIndependant, .SyndicationURL, .SyndicationEmail, .EmailNotification, .AllowTrackbacks, .AutoTrackback, .MustApproveComments, .MustApproveAnonymous, .MustApproveTrackbacks, .UseCaptcha, .EnableGhostWriter), Integer)
   End With
  End Function

  Public Sub UpdateBlog(ByVal objBlog As BlogInfo)
   With objBlog
                DataProvider.Instance().UpdateBlog(.PortalID, .BlogID, .ParentBlogID, .UserID, .Title, .Description, .Public, .AllowComments, .AllowAnonymous, .ShowFullName, .Culture, .DateFormat, .TimeZone.Id, .Syndicated, .SyndicateIndependant, .SyndicationURL, .SyndicationEmail, .EmailNotification, .AllowTrackbacks, .AutoTrackback, .MustApproveComments, .MustApproveAnonymous, .MustApproveTrackbacks, .UseCaptcha, .EnableGhostWriter)
   End With
  End Sub

  Public Sub DeleteBlog(ByVal blogID As Integer)
   DataProvider.Instance().DeleteBlog(blogID)
  End Sub

  Public Function GetBlogFromContext() As BlogInfo
   Dim ReturnBlog As BlogInfo = Nothing
   Dim Request As HttpRequest = HttpContext.Current.Request
   Dim PortalSettings As DotNetNuke.Entities.Portals.PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), DotNetNuke.Entities.Portals.PortalSettings)

   If Not (Request.Params("BlogID") Is Nothing) Then
    ReturnBlog = GetBlog(Int32.Parse(Request.Params("BlogID")))
   ElseIf Not Request.Params("Blog") Is Nothing Then
    ReturnBlog = GetBlogByUserName(PortalSettings.PortalId, Request.Params("Blog"))
   End If

   If ReturnBlog IsNot Nothing Then
    If ReturnBlog.PortalID <> PortalSettings.PortalId Then
     ReturnBlog = Nothing
    End If
   End If

   Return ReturnBlog
  End Function

#Region " 4.5.0 Upgrade"

        Public Function GetAllPublishedPortalBlogEntries(ByVal PortalID As Integer) As List(Of EntryInfo)

            Return Nothing
            'Return CBO.FillCollection(DataProvider.Instance().ListBlogsRootByPortal(PortalID), GetType(BlogInfo))

        End Function

#End Region

 End Class

End Namespace