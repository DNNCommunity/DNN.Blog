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

Imports System.Web.Services
Imports System.ComponentModel
Imports System.Web.Script.Services
Imports System.Linq
Imports DotNetNuke.Common.Utilities

<System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://www.dotnetnuke.com/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
 Public Class BlogTerms
    Inherits System.Web.Services.WebService

    Public Const DisallowedCharacters As String = "%?*&;:'\\"


    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function SearchTags(searchTerm As String) As String
        If String.IsNullOrEmpty(searchTerm) Then
            Return ""
        End If

        Dim colTerms As IQueryable(Of String) = (From t In Entities.Content.Common.Util.GetTermController().GetTermsByVocabulary(1) _
                Where t.Name.ToLower().Contains(searchTerm.ToLower())
                Where (t.Name.IndexOfAny(DisallowedCharacters.ToCharArray()) = -1)
                Select t.Name)

        'Dim terms As IQueryable = Entities.Content.Common.Util.GetTermController().GetTermsByVocabulary(1).Where(Function(t) t.Name.ToLower().Contains(searchTerm.ToLower())).Where(Function(t) t.Name.IndexOfAny(Constants.DisallowedCharacters.ToCharArray()) = -1).[Select](Function(term) term.Name)
        Return colTerms.ToJson()
    End Function

End Class