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
Imports System.Data
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Tokens
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Modules.Blog.Entities.Blogs

Namespace Entities.Entries

 Partial Public Class EntryInfo

  Public Property EntryCategories As List(Of TermInfo)
  Public Property EntryTags As List(Of TermInfo)

  Public Function PermaLink(portalSettings As DotNetNuke.Entities.Portals.PortalSettings) As String
   Return PermaLink(portalSettings.ActiveTab, portalSettings.PortalAlias.HTTPAlias)
  End Function

  Private _permaLink As String = ""
  Public Function PermaLink(tab As DotNetNuke.Entities.Tabs.TabInfo, portalAlias As String) As String
   If String.IsNullOrEmpty(_permaLink) Then
    ' _permaLink = DotNetNuke.Services.Url.FriendlyUrl.FriendlyUrlProvider.Instance.FriendlyUrl(tab, DotNetNuke.Common.NavigateURL(tab.TabID, "", New String() {"Post=" & ContentItemId.ToString}), Title, portalAlias)
    _permaLink = DotNetNuke.Common.NavigateURL(tab.TabID, "", New String() {"Post=" & ContentItemId.ToString})
   End If
   Return _permaLink
  End Function

  Private _blog As BlogInfo = Nothing
  Public Property Blog() As BlogInfo
   Get
    If _blog Is Nothing Then
     _blog = BlogsController.GetBlog(BlogID, -1)
    End If
    Return _blog
   End Get
   Set(ByVal value As BlogInfo)
    _blog = value
   End Set
  End Property
 End Class
End Namespace