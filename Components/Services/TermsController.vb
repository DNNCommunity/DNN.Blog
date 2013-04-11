Imports DotNetNuke.Web.Api
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports System.Web.Http
Imports System.Net.Http
Imports System.Net
Imports System.Linq

Namespace Services
 Public Class TermsController
  Inherits DnnApiController

  Private Const DisallowedCharacters As String = "%?*&;:'\\"

#Region " Service Methods "
  <HttpGet()>
  <DnnModuleAuthorize(accesslevel:=DotNetNuke.Security.SecurityAccessLevel.View)>
  <ActionName("Search")>
  Public Function Search() As HttpResponseMessage
   Dim queryString As NameValueCollection = HttpUtility.ParseQueryString(Me.Request.RequestUri.Query)
   Dim searchString As String = queryString("term")
   Dim vocab As Integer = Integer.Parse(queryString("vocab"))
   Dim colTerms As IEnumerable(Of String) = Entities.Terms.TermsController.GetTermsByVocabulary(ActiveModule.ModuleID, vocab, Threading.Thread.CurrentThread.CurrentCulture.Name).Values.Where(Function(t) t.LocalizedName.IndexOfAny(DisallowedCharacters.ToCharArray()) = -1 And t.LocalizedName.ToLower().Contains(searchString.ToLower())).Select(Function(t) t.LocalizedName)
   Return Request.CreateResponse(HttpStatusCode.OK, colTerms)
  End Function

  <HttpGet()>
  <DnnModuleAuthorize(accesslevel:=DotNetNuke.Security.SecurityAccessLevel.View)>
  <ActionName("VocabularyML")>
  Public Function GetVocabularyML(vocabularyId As Integer) As HttpResponseMessage
   Dim res As New List(Of TermML)
   For Each t As Entities.Terms.TermInfo In Entities.Terms.TermsController.GetTermsByVocabulary(ActiveModule.ModuleID, vocabularyId, "").Values
    res.Add(New TermML With {.TermID = t.TermId, .DefaultName = t.Name, .LocNames = t.NameLocalizations})
   Next
   Return Request.CreateResponse(HttpStatusCode.OK, res)
  End Function
#End Region

  Public Structure TermML
   Public TermID As Integer
   Public DefaultName As String
   Public LocNames As LocalizedText
  End Structure

 End Class
End Namespace
