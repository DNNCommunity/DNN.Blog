Imports System.Xml

Partial Public Class WLWManifest
 Inherits System.Web.UI.Page

#Region " Private Members "
 Private _portalId As Integer = -1
 Private _tabId As Integer = -1
#End Region

#Region " Event Handlers "
 Private Sub WLWManifest_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
  Globals.ReadValue(Me.Request.Params, "PortalId", _portalId)
 End Sub

 Protected Sub WLWManifest_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

  Response.Clear()
  Response.ClearContent()
  Response.ContentType = "application/xml"

  Using xtw As New XmlTextWriter(Response.OutputStream, System.Text.Encoding.UTF8)
   Dim bs As New Settings.BlogSettings(_portalId, -1)
   bs.WriteWLWManifest(xtw)
  End Using

  Response.End()

 End Sub
#End Region

End Class