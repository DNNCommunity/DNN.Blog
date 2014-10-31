'
' DNN Connect - http://dnn-connect.org
' Copyright (c) 2014
' by DNN Connect
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

Imports DotNetNuke.Web.Api
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports System.Web.Http
Imports System.Net.Http
Imports System.Net
Imports System.Linq

Namespace Entities.Terms
 Partial Public Class TermsController
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
    res.Add(New TermML With {.TermID = t.TermId, .DefaultName = t.Name, .LocNames = t.NameLocalizations.GetDictionary})
   Next
   Return Request.CreateResponse(HttpStatusCode.OK, res)
  End Function
#End Region

  Public Structure TermML
   Public TermID As Integer
   Public DefaultName As String
   Public LocNames As Dictionary(Of String, String)
  End Structure

 End Class
End Namespace