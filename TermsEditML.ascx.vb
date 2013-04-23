Imports DotNetNuke.Web.Client.ClientResourceManagement
Imports DotNetNuke.Services.Localization
Imports System.Globalization
Imports DotNetNuke.Modules.Blog.Entities.Terms

Public Class TermsEditML
 Inherits BlogModuleBase

 Public Property VocabularyId As Integer = 1
 Public Property ColumnHeaders As String = "[]"
 Public Property Columns As String = ""

 Private Sub Page_Init1(sender As Object, e As System.EventArgs) Handles Me.Init
  AddJavascriptFile("jquery.handsontable.js", 60)
  AddCssFile("jquery.handsontable.css")
  Me.Request.Params.ReadValue("VocabularyId", VocabularyId)
  If VocabularyId <> Settings.VocabularyId AndAlso VocabularyId <> 1 Then ' prevent users from editing another vocabulary
   VocabularyId = 1
  End If
  ColumnHeaders = "['" & (New CultureInfo(PortalSettings.DefaultLanguage)).EnglishName & "'"
  Columns = "[{data: 'DefaultName'}"
  For Each l As Locale In LocaleController.Instance.GetLocales(PortalId).Values
   If l.Code <> PortalSettings.DefaultLanguage Then
    ColumnHeaders &= ", '" & l.EnglishName & "'"
    Columns &= ", {data: 'LocNames." & l.Code & "'}"
   End If
  Next
  ColumnHeaders &= "]"
  Columns &= "]"
 End Sub

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

  If Not Security.IsEditor Then
   Throw New Exception("You do not have access to this resource. Please check your login status.")
  End If

 End Sub

 Private Sub cmdCancel_Click(sender As Object, e As System.EventArgs) Handles cmdCancel.Click
  Response.Redirect(EditUrl("Admin"), False)
 End Sub

 Private Sub cmdUpdate_Click(sender As Object, e As System.EventArgs) Handles cmdUpdate.Click
  Dim vocab As List(Of TermsController.TermML) = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of TermsController.TermML))(Storage.Value)
  Dim currentVocabulary As Dictionary(Of Integer, TermInfo) = TermsController.GetTermsByVocabulary(ModuleId, VocabularyId)

  For Each t As TermsController.TermML In vocab
   Dim crtTerm As Entities.Terms.TermInfo = currentVocabulary(t.TermID)
   If crtTerm IsNot Nothing Then
    If crtTerm.Name <> t.DefaultName Then
     crtTerm.Name = t.DefaultName
     DotNetNuke.Entities.Content.Common.Util.GetTermController().UpdateTerm(crtTerm)
    End If
   End If
   For Each l As String In t.LocNames.Keys
    Data.DataProvider.Instance.SetTermLocalization(t.TermID, l, t.LocNames(l), "")
   Next
  Next

  Response.Redirect(EditUrl("Admin"), False)
 End Sub
End Class