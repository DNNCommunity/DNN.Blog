'
' DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2005
' by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
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
'-------------------------------------------------------------------------

Imports System
Imports System.Web.UI
Imports System.Web
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke

Public Class BlogModuleBase
 Inherits PortalModuleBase

#Region "Public Constants"
 Public Const RSS_RECENT_ENTRIES As Integer = 0
 Public Const RSS_BLOG_ENTRIES As Integer = 1
 Public Const RSS_SINGLE_ENTRY As Integer = 2
 Public Const RSS_ARCHIV_VIEW As Integer = 3

 Public Const CONTROL_VIEW_VIEWBLOG As String = "ViewBlog.ascx"
 Public Const CONTROL_VIEW_VIEWENTRY As String = "ViewEntry.ascx"
 Public Const CONTROL_VIEW_BLOGFEED As String = "BlogFeed.ascx"
 Public Const ONLINE_HELP_URL As String = ""
#End Region

#Region "Public Enums"
#End Region

#Region "Public Member"
 Public MyActions As New Entities.Modules.Actions.ModuleActionCollection
 Public Shared RssView As RssViews
#End Region

#Region "Public Methods"
 Public Sub SetModuleConfiguration(ByVal config As Entities.Modules.ModuleInfo)
  ModuleConfiguration = config
 End Sub

#End Region

#Region "Public Properties"
 Public ReadOnly Property BasePage() As DotNetNuke.Framework.CDefault
  Get
   Try
    Return CType(Me.Page, DotNetNuke.Framework.CDefault)
   Catch
    Return Nothing
   End Try
  End Get
 End Property

 Public Property BlogSettings() As BlogSettings
  Get
   If _blogSettings Is Nothing Then
    _blogSettings = BlogSettings.GetBlogSettings(PortalId, TabId)
   End If
   Return _blogSettings
  End Get
  Set(ByVal value As BlogSettings)
   _blogSettings = value
  End Set
 End Property
#End Region

#Region " Private Members "
 Private _blogSettings As BlogSettings
#End Region
End Class
