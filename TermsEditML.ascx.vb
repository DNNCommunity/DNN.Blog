Imports DotNetNuke.Web.Client.ClientResourceManagement

Public Class TermsEditML
 Inherits BlogModuleBase

 Public Property VocabularyId As Integer = 1

 Private Sub Page_Init1(sender As Object, e As System.EventArgs) Handles Me.Init
  AddBlogService()
  ClientResourceManager.RegisterScript(Page, ResolveUrl("~/DesktopModules/Blog/js/jquery.handsontable.js"))
  ClientResourceManager.RegisterStyleSheet(Page, ResolveUrl("~/DesktopModules/Blog/css/jquery.handsontable.css"), Web.Client.FileOrder.Css.ModuleCss)
  Me.Request.Params.ReadValue("VocabularyId", VocabularyId)
  If VocabularyId <> Settings.VocabularyId AndAlso VocabularyId <> 1 Then ' prevent users from editing another vocabulary
   VocabularyId = 1
  End If
 End Sub

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

 End Sub

End Class