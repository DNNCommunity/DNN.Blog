'
' DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2010
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

Imports DotNetNuke.Security
Imports DotNetNuke.Data


Namespace MetaWeblog

  Partial Class blogpostredirect
    Inherits System.Web.UI.Page

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

      Dim intendedUrl As String = String.Empty
      Dim bStyleDetectionPost As Boolean = False

      ' Check to see if this is a style detection post.
      Dim sSQL As String = "SELECT TempInstallUrl FROM {databaseOwner}{objectQualifier}Blog_MetaWeblogData"
      Dim sURL As String = String.Empty
      Dim dr As IDataReader = DirectCast(DataProvider.Instance().ExecuteSQL(sSQL), IDataReader)
      While dr.Read()
        sURL = dr("TempInstallUrl").ToString() + ""
      End While

      If sURL <> String.Empty Then    'Style Detection Post

        ' Delete the entry from the TempInstallUrl field in the database.
        sSQL = "UPDATE {databaseOwner}{objectQualifier}Blog_MetaWeblogData SET TempInstallUrl = ''"
        DataProvider.Instance().ExecuteSQL(sSQL)
        Response.Redirect(sURL, False)

      Else                            'This is a regular post

        ' Retrieve the IntendedUrl from the QueryString
        If Not Request("tab") Is Nothing Then
          Dim tab As Integer
          If Integer.TryParse(Request("tab"), tab) Then
            intendedUrl = DotNetNuke.Common.Globals.NavigateURL(tab)
          End If
        End If

        If Not BlogPostServices.IsNullOrEmpty(intendedUrl) Then

          If Request.UserAgent.IndexOf("Windows Live Writer") < 0 Then
            'Dim script As New HtmlGenericControl("script")
            'script.Attributes.Add("type", "text/javascript")
            'script.InnerHtml = "window.location.href ='" & intendedUrl & "';"
            'phBody.Controls.Add(script)

            Response.Redirect(intendedUrl)
          End If

          Dim objBlogModuleProvider As New BlogModuleProvider(CInt(Request.Params("PortalId")))
          Dim link As New HtmlGenericControl("link")
          link.Attributes.Add("rel", "wlwmanifest")
          link.Attributes.Add("type", "application/wlwmanifest+xml")
          link.Attributes.Add("href", DotNetNuke.Common.Globals.ApplicationPath & objBlogModuleProvider.ManifestFilePath)
          link.Visible = True

          phHead.Controls.Add(link)

        End If

      End If

    End Sub

  End Class

End Namespace