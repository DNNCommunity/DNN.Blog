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
Imports DotNetNuke.Modules.Blog.Data
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Modules.Blog.Entities
Imports System.Linq

Namespace Entities.Blogs

 Partial Public Class BlogsController

  Public Shared Function GetBlog(blogID As Int32, userId As Integer, locale As String) As BlogInfo

   Return CType(CBO.FillObject(DataProvider.Instance().GetBlog(blogID, userId, locale), GetType(BlogInfo)), BlogInfo)

  End Function

  Public Shared Function GetBlogsByModule(moduleID As Int32, locale As String) As Dictionary(Of Integer, BlogInfo)

   Return GetBlogsByModule(moduleID, -1, locale)

  End Function

  Public Shared Function GetBlogsByModule(moduleID As Int32, userId As Integer, locale As String) As Dictionary(Of Integer, BlogInfo)

   Dim res As Dictionary(Of Integer, BlogInfo) = DotNetNuke.Common.Utilities.CBO.FillDictionary(Of Integer, BlogInfo)("BlogID", DataProvider.Instance().GetBlogsByModule(moduleID, userId, locale))
   If userId > -1 Then
    For Each b As BlogInfo In res.Values
     If b.OwnerUserId = userId Then
      b.CanAdd = True
      b.CanEdit = True
      b.CanApprove = True
      b.IsOwner = True
     End If
    Next
   End If
   Return res

  End Function

  Public Shared Function GetBlogsByPortal(portalId As Int32, userId As Integer, locale As String) As Dictionary(Of Integer, BlogInfo)

   Dim res As Dictionary(Of Integer, BlogInfo) = DotNetNuke.Common.Utilities.CBO.FillDictionary(Of Integer, BlogInfo)("BlogID", DataProvider.Instance().GetBlogsByPortal(portalId, userId, locale))
   If userId > -1 Then
    For Each b As BlogInfo In res.Values
     If b.OwnerUserId = userId Then
      b.CanAdd = True
      b.CanEdit = True
      b.CanApprove = True
      b.IsOwner = True
     End If
    Next
   End If
   Return res

  End Function

  Public Shared Function GetBlogCalendar(moduleId As Integer, blogId As Integer, locale As String) As List(Of BlogCalendarInfo)
   Return DotNetNuke.Common.Utilities.CBO.FillCollection(Of BlogCalendarInfo)(DataProvider.Instance.GetBlogCalendar(moduleId, blogId, locale))
  End Function

 End Class

End Namespace