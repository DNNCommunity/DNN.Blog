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
Imports DotNetNuke.Modules.Blog.Providers.Data
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Modules.Blog.Components.Entities
Imports System.Linq

Namespace Components.Controllers

    Public Class BlogController

#Region "linq"

        Public Function GetUsersParentBlogByName(ByVal portalId As Integer, ByVal username As String) As BlogInfo
            Dim colPortalBlogs As List(Of BlogInfo) = GetAllPortalBlogs(portalId)
            Return colPortalBlogs.Where(Function(t) t.UserName = username AndAlso t.ParentBlogID < 1).FirstOrDefault
        End Function

        Public Function GetUsersParentBlogById(ByVal portalId As Integer, ByVal userId As Integer) As BlogInfo
            Dim colPortalBlogs As List(Of BlogInfo) = GetAllPortalBlogs(portalId)
            Return colPortalBlogs.Where(Function(t) t.UserID = userId AndAlso t.ParentBlogID < 1).FirstOrDefault
        End Function

        Public Function GetUsersBlogs(ByVal portalId As Integer, ByVal username As String) As List(Of BlogInfo)
            Dim colPortalBlogs As List(Of BlogInfo) = GetAllPortalBlogs(portalId)
            Return colPortalBlogs.Where(Function(t) t.UserName = username).ToList
        End Function

        Public Function GetParentsChildBlogs(ByVal portalId As Integer, ByVal parentBlogId As Integer, ByVal ShowNonPublic As Boolean) As List(Of BlogInfo)
            Dim colPortalBlogs As List(Of BlogInfo) = GetAllPortalBlogs(portalId)
            If ShowNonPublic Then
                Return colPortalBlogs.Where(Function(t) t.PortalID = portalId AndAlso t.ParentBlogID = parentBlogId).OrderBy(Function(t) t.Title).ToList
            Else
                Return colPortalBlogs.Where(Function(t) t.PortalID = portalId AndAlso t.ParentBlogID = parentBlogId AndAlso t.Public = True).OrderBy(Function(t) t.Title).ToList
            End If
        End Function

        Public Function GetPortalBlogs(ByVal portalId As Integer, ByVal ShowNonPublic As Boolean) As List(Of BlogInfo)
            Dim colPortalBlogs As List(Of BlogInfo) = GetAllPortalBlogs(portalId)
            If ShowNonPublic Then
                Return colPortalBlogs
            Else
                Return colPortalBlogs.Where(Function(t) t.PortalID = portalId AndAlso t.Public = True).ToList
            End If
        End Function

        Public Function GetPortalParentBlogs(ByVal portalId As Integer) As List(Of BlogInfo)
            Dim colPortalBlogs As List(Of BlogInfo) = GetAllPortalBlogs(portalId)
            Return colPortalBlogs.Where(Function(t) t.ParentBlogID = -1).ToList
        End Function

#End Region

        Public Function GetBlog(ByVal blogId As Integer) As BlogInfo
            Return CType(CBO.FillObject(DataProvider.Instance().GetBlog(blogId), GetType(BlogInfo)), BlogInfo)
        End Function

        Public Function AddBlog(ByVal objBlog As BlogInfo) As Integer
            With objBlog
                Dim strCacheKey As String = Common.Constants.ModuleCacheKeyPrefix + Common.Constants.PortalBlogsCacheKey & CStr(objBlog.PortalID)
                DataCache.RemoveCache(strCacheKey)

                Return CType(DataProvider.Instance().AddBlog(.PortalID, .ParentBlogID, .UserID, .Title, .Description, .Public, .AllowComments, False, .ShowFullName, .Syndicated, True, .SyndicationURL, .SyndicationEmail, True, False, False, True, True, True, False, .AuthorMode), Integer)
            End With
        End Function

        Public Sub UpdateBlog(ByVal objBlog As BlogInfo)
            With objBlog
                DataProvider.Instance().UpdateBlog(.PortalID, .BlogID, .ParentBlogID, .UserID, .Title, .Description, .Public, .AllowComments, False, .ShowFullName, .Syndicated, True, .SyndicationURL, .SyndicationEmail, True, False, False, True, True, True, False, .AuthorMode)
            End With

            Dim strCacheKey As String = Common.Constants.ModuleCacheKeyPrefix + Common.Constants.PortalBlogsCacheKey & CStr(objBlog.PortalID)
            DataCache.RemoveCache(strCacheKey)
        End Sub

        Public Sub DeleteBlog(ByVal blogID As Integer, ByVal portalId As Integer)
            DataProvider.Instance().DeleteBlog(blogID, portalId)

            Dim strCacheKey As String = Common.Constants.ModuleCacheKeyPrefix + Common.Constants.PortalBlogsCacheKey & CStr(portalId)
            DataCache.RemoveCache(strCacheKey)
        End Sub

        Public Function GetBlogFromContext() As BlogInfo
            Dim ReturnBlog As BlogInfo = Nothing
            Dim Request As HttpRequest = HttpContext.Current.Request
            Dim PortalSettings As DotNetNuke.Entities.Portals.PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), DotNetNuke.Entities.Portals.PortalSettings)

            If Not (Request.Params("BlogID") Is Nothing) Then
                ReturnBlog = GetBlog(Int32.Parse(Request.Params("BlogID")))
            ElseIf Not Request.Params("Blog") Is Nothing Then
                ReturnBlog = GetUsersParentBlogByName(PortalSettings.PortalId, Request.Params("Blog"))
            End If

            If ReturnBlog IsNot Nothing Then
                If ReturnBlog.PortalID <> PortalSettings.PortalId Then
                    ReturnBlog = Nothing
                End If
            End If

            Return ReturnBlog
        End Function

#Region "Private Methods"

        ''' <summary>
        ''' Returns a list of all blogs in a portal.
        ''' </summary>
        ''' <param name="portalId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetAllPortalBlogs(ByVal portalId As Integer) As List(Of BlogInfo)
            Dim strCacheKey As String = Common.Constants.ModuleCacheKeyPrefix + Common.Constants.PortalBlogsCacheKey & CStr(portalId)
            Dim colBlogs As List(Of BlogInfo) = CType(DataCache.GetCache(strCacheKey), List(Of BlogInfo))

            If colBlogs Is Nothing Then
                Dim timeOut As Int32 = Common.Constants.CACHE_TIMEOUT * Convert.ToInt32(DotNetNuke.Entities.Host.Host.PerformanceSetting)

                colBlogs = CBO.FillCollection(Of BlogInfo)(DataProvider.Instance().GetBlogsByPortal(portalId))

                'Cache Forum if timeout > 0 and Forum is not null
                If timeOut > 0 And colBlogs IsNot Nothing Then
                    DataCache.SetCache(strCacheKey, colBlogs, TimeSpan.FromMinutes(timeOut))
                End If
            End If
            Return colBlogs
        End Function

#End Region

#Region " 4.5.0 Upgrade"

        Public Function GetAllPublishedPortalBlogEntries(ByVal PortalID As Integer) As List(Of EntryInfo)

            Return Nothing
            'Return CBO.FillCollection(DataProvider.Instance().ListBlogsRootByPortal(PortalID), GetType(BlogInfo))

        End Function

#End Region

    End Class

End Namespace