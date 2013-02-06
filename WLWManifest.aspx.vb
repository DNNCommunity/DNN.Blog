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

Imports System.Xml
Imports DotNetNuke.Modules.Blog.Components.Common
Imports DotNetNuke.Modules.Blog.Components.Settings

Partial Public Class WLWManifest
 Inherits System.Web.UI.Page

#Region " Private Members "
 Private _portalId As Integer = -1
 Private _tabId As Integer = -1
#End Region

#Region " Event Handlers "
 Private Sub WLWManifest_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
  Me.Request.Params.ReadValue("PortalId", _portalId)
 End Sub

 Protected Sub WLWManifest_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

  Response.Clear()
  Response.ClearContent()
  Response.ContentType = "application/xml"

  Using xtw As New XmlTextWriter(Response.OutputStream, System.Text.Encoding.UTF8)
   Dim bs As New BlogSettings(_portalId, -1)
   bs.WriteWLWManifest(xtw)
  End Using

  Response.End()

 End Sub
#End Region

End Class