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

Imports System
Imports System.Data
Imports System.Runtime.Serialization
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Tokens
Imports DotNetNuke.Entities.Users

Namespace Entities.Blogs

 Partial Public Class BlogInfo

  Private _permissions As Security.Permissions.BlogPermissionCollection
  Public Property Permissions() As Security.Permissions.BlogPermissionCollection
   Get
    If _permissions Is Nothing Then
     _permissions = Security.Permissions.BlogPermissionsController.GetBlogPermissionsCollection(BlogID)
    End If
    Return _permissions
   End Get
   Set(value As Security.Permissions.BlogPermissionCollection)
    _permissions = value
   End Set
  End Property

  <DataMember()>
  Public Property CanEdit As Boolean = False
  <DataMember()>
  Public Property CanAdd As Boolean = False
  <DataMember()>
  Public Property CanApprove As Boolean = False
  <DataMember()>
  Public Property IsOwner As Boolean = False

  Public Function PermaLink(strParentTabID As Integer) As String
   Dim oTabController As DotNetNuke.Entities.Tabs.TabController = New DotNetNuke.Entities.Tabs.TabController
   Dim oParentTab As DotNetNuke.Entities.Tabs.TabInfo = oTabController.GetTab(strParentTabID, DotNetNuke.Entities.Portals.PortalSettings.Current.PortalId, False)
   _permaLink = String.Empty
   Return PermaLink(oParentTab)
  End Function

  Public Function PermaLink(portalSettings As DotNetNuke.Entities.Portals.PortalSettings) As String
   Return PermaLink(portalSettings.ActiveTab)
  End Function

  Private _permaLink As String = ""
  Public Function PermaLink(tab As DotNetNuke.Entities.Tabs.TabInfo) As String
   If String.IsNullOrEmpty(_permaLink) Then
    _permaLink = ApplicationURL(tab.TabID) & "&Blog=" & BlogID.ToString
    If DotNetNuke.Entities.Host.Host.UseFriendlyUrls Then
     _permaLink = FriendlyUrl(tab, _permaLink, GetSafePageName(LocalizedTitle))
    Else
     _permaLink = ResolveUrl(_permaLink)
    End If
   End If
   Return _permaLink
  End Function

 End Class

End Namespace