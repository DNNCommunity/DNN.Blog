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
Imports System.Linq
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Entities.Terms

Namespace Entities.Posts

 Partial Public Class PostInfo

  Public ReadOnly Property PostCategories As List(Of TermInfo)
   Get
    Return Terms.Where(Function(t) t.VocabularyId <> 1).ToList
   End Get
  End Property

  Public ReadOnly Property PostTags As List(Of TermInfo)
   Get
    Return Terms.Where(Function(t) t.VocabularyId = 1).ToList
   End Get
  End Property

  Public Function PermaLink(strParentTabID As Integer) As String
   Dim oTabController As DotNetNuke.Entities.Tabs.TabController = New DotNetNuke.Entities.Tabs.TabController
   Dim oParentTab As DotNetNuke.Entities.Tabs.TabInfo = oTabController.GetTab(strParentTabID, DotNetNuke.Entities.Portals.PortalSettings.Current.PortalId, False)
   _permaLink = String.Empty
   Return PermaLink(oParentTab)
  End Function

  Public Function PermaLink() As String
   Return PermaLink(DotNetNuke.Entities.Portals.PortalSettings.Current.ActiveTab)
  End Function

  Public Function PermaLink(portalSettings As DotNetNuke.Entities.Portals.PortalSettings) As String
   Return PermaLink(portalSettings.ActiveTab)
  End Function

  Private _permaLink As String = ""
  Public Function PermaLink(tab As DotNetNuke.Entities.Tabs.TabInfo) As String
   If String.IsNullOrEmpty(_permaLink) Then
    _permaLink = ApplicationURL(tab.TabID)
    If Not String.IsNullOrEmpty(Locale) Then
     _permaLink &= "&language=" & Locale
    End If
    _permaLink &= "&Post=" & ContentItemId.ToString
    If DotNetNuke.Entities.Host.Host.UseFriendlyUrls Then
     _permaLink = FriendlyUrl(tab, _permaLink, GetSafePageName(LocalizedTitle))
    Else
     _permaLink = ResolveUrl(_permaLink)
    End If
   End If
   Return _permaLink
  End Function

  Private _terms As List(Of TermInfo)
  Public Shadows Property Terms() As List(Of TermInfo)
   Get
    If _terms Is Nothing Then
     _terms = TermsController.GetTermsByPost(ContentItemId, Blog.ModuleID, Threading.Thread.CurrentThread.CurrentCulture.Name)
    End If
    If _terms Is Nothing Then
     _terms = New List(Of TermInfo)
    End If
    Return _terms
   End Get
   Set(ByVal value As List(Of TermInfo))
    _terms = value
   End Set
  End Property

  Private _blog As BlogInfo = Nothing
  Public Property Blog() As BlogInfo
   Get
    If _blog Is Nothing Then
     _blog = BlogsController.GetBlog(BlogID, -1, Threading.Thread.CurrentThread.CurrentCulture.Name)
    End If
    Return _blog
   End Get
   Set(ByVal value As BlogInfo)
    _blog = value
   End Set
  End Property
 End Class
End Namespace