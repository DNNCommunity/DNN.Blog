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
Imports DotNetNuke.UI.Modules

Namespace Common

 Public Class Links

  Const ControlKey_EditBlog As String = "Edit_Blog"
  Const ControlKey_EditEntry As String = "Edit_Entry"

  Public Shared Function ViewBlog(modContext As ModuleInstanceContext, blogId As Integer) As String
   Return modContext.NavigateUrl(modContext.TabId, "", False, "BlogID=" & blogId.ToString())
  End Function

  Public Shared Function ViewChildBlog(modContext As ModuleInstanceContext, blogId As Integer, parentId As Integer) As String
   Return modContext.NavigateUrl(modContext.TabId, "", False, "BlogID=" & blogId.ToString(), "ParentBlogID=" & parentId.ToString())
  End Function

  Public Shared Function EditBlog(modContext As ModuleInstanceContext, blogId As Integer) As String
   If blogId > 0 Then
    Return modContext.NavigateUrl(modContext.TabId, ControlKey_EditBlog, False, "BlogID=" & blogId.ToString(), "mid=" + modContext.ModuleId.ToString())
   Else
    Return modContext.NavigateUrl(modContext.TabId, ControlKey_EditBlog, False, "mid=" + modContext.ModuleId.ToString())
   End If
  End Function

  Public Shared Function ViewBlogsByDate(modContext As ModuleInstanceContext, newDate As String, dateType As String) As String
   Return modContext.NavigateUrl(modContext.TabId, "", False, "BlogDate=" & newDate, "DateType=" & dateType)
  End Function

#Region "Entries"

  Public Shared Function EditEntry(modContext As ModuleInstanceContext, blogId As Integer, entryId As Integer) As String
   If entryId > 0 Then
    Return modContext.NavigateUrl(modContext.TabId, ControlKey_EditEntry, False, "BlogID=" & blogId.ToString(), "mid=" + modContext.ModuleId.ToString(), "EntryId=" + entryId.ToString())
   Else
    Return modContext.NavigateUrl(modContext.TabId, ControlKey_EditEntry, False, "BlogID=" & blogId.ToString(), "mid=" + modContext.ModuleId.ToString())
   End If
  End Function

  Public Shared Function ViewEntriesByBlog(modContext As ModuleInstanceContext, blog As Integer, page As Integer) As String
   If page > 1 Then
    Return modContext.NavigateUrl(modContext.TabId, "", False, "BlogID=" + blog.ToString(), "page=" + page.ToString())
   Else
    Return modContext.NavigateUrl(modContext.TabId, "", False, "BlogID=" + blog.ToString())
   End If
  End Function

  Public Shared Function ViewEntriesByCategory(modContext As ModuleInstanceContext, category As Integer, page As Integer) As String
   If page > 1 Then
    Return modContext.NavigateUrl(modContext.TabId, "", False, "catid=" + category.ToString(), "page=" + page.ToString())
   Else
    Return modContext.NavigateUrl(modContext.TabId, "", False, "catid=" + category.ToString())
   End If
  End Function

  Public Shared Function ViewEntriesByTag(modContext As ModuleInstanceContext, tag As Integer, page As Integer) As String
   If page > 1 Then
    Return modContext.NavigateUrl(modContext.TabId, "", False, "tagid=" + tag.ToString(), "page=" + page.ToString())
   Else
    Return modContext.NavigateUrl(modContext.TabId, "", False, "tagid=" + tag.ToString())
   End If
  End Function

#End Region

#Region "RSS"

  Public Shared Function RSSAggregated(modContext As ModuleInstanceContext) As String
   Return modContext.NavigateUrl(modContext.TabId, "", False, "rssid=0")
  End Function

  Public Shared Function RSSByBlog(modContext As ModuleInstanceContext, blogId As Integer) As String
   Return modContext.NavigateUrl(modContext.TabId, "", False, "rssid=" & blogId.ToString())
  End Function

  Public Shared Function RssByCategory(modContext As ModuleInstanceContext, category As Integer) As String
   Return modContext.NavigateUrl(modContext.TabId, "", False, "rssid=0", "catid=" + category.ToString())
  End Function

  Public Shared Function RssByTag(modContext As ModuleInstanceContext, tag As Integer) As String
   Return modContext.NavigateUrl(modContext.TabId, "", False, "rssid=0", "tagid=" + tag.ToString())
  End Function

#End Region

 End Class

End Namespace