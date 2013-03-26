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
   Dim colTerms As IQueryable(Of String) = (From t In DotNetNuke.Entities.Content.Common.Util.GetTermController().GetTermsByVocabulary(vocab)
        Where t.Name.ToLower().Contains(searchString.ToLower())
        Where (t.Name.IndexOfAny(DisallowedCharacters.ToCharArray()) = -1)
        Select t.Name)
   Return Request.CreateResponse(HttpStatusCode.OK, colTerms)
  End Function
#End Region

 End Class
End Namespace
