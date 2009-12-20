Namespace MetaWeblog

 Partial Class blogpostredirect
  Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

  'This call is required by the Web Form Designer.
  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

  End Sub

  'NOTE: The following placeholder declaration is required by the Web Form Designer.
  'Do not delete or move it.
  Private designerPlaceholderDeclaration As System.Object
  Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
   'CODEGEN: This method call is required by the Web Form Designer
   'Do not modify it using the code editor.
   InitializeComponent()
  End Sub

#End Region

  Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
   Dim intendedUrl As String = String.Empty
   Dim providerKey As String = String.Empty
   Dim bStyleDetectionPost As Boolean = False

   ' Check to see if this is a style detection post.
   Dim sSQL As String = "SELECT TempInstallUrl FROM {databaseOwner}{objectQualifier}Blog_MetaWeblogData"
   Dim sURL As String = String.Empty
   Dim dr As IDataReader = DirectCast(DotNetNuke.Data.DataProvider.Instance().ExecuteSQL(sSQL), IDataReader)
   While dr.Read()
    sURL = dr("TempInstallUrl").ToString() + ""
   End While

   If sURL <> String.Empty Then    'Style Detection Post
    ' Delete the entry from the TempInstallUrl field in the database.
    sSQL = "UPDATE {databaseOwner}{objectQualifier}Blog_MetaWeblogData SET TempInstallUrl = ''"
    DotNetNuke.Data.DataProvider.Instance().ExecuteSQL(sSQL)

    Response.Redirect(sURL)

   Else                            'This is a regular post

    ' Retrieve the IntendedUrl from the QueryString
    If Not Request("IntendedUrl") Is Nothing Then
     intendedUrl = HttpUtility.UrlDecode(Request("IntendedUrl").ToString())
    End If
    ' Retrieve the providerKey from the QueryString
    If Not Request("key") Is Nothing Then
     providerKey = HttpUtility.UrlDecode(Request("key").ToString())
    End If

    If Not BlogPostServices.IsNullOrEmpty(intendedUrl) AndAlso Not BlogPostServices.IsNullOrEmpty(providerKey) Then
     ' Redirect to the intended URL using client side code
     'string rs = "<script type=\"text/javascript\">";
     'rs += "window.location.href = '" + Request["IntendedUrl"].ToString() + "';";
     'rs += "</script>";

     If Request.UserAgent.IndexOf("Windows Live Writer") < 0 Then
      Dim script As New HtmlGenericControl("script")
      script.Attributes.Add("type", "text/javascript")
      script.InnerText = "window.location.href = '" + HttpUtility.UrlDecode(intendedUrl) + "';"
      phBody.Controls.Add(script)
     End If

     Dim objBlogModuleProvider As New BlogModuleProvider

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