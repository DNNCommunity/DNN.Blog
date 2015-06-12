'
' DNN Connect - http://dnn-connect.org
' Copyright (c) 2015
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

Imports System.Linq

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Content.Taxonomy
Imports System.Xml

Namespace Entities.Terms
 Partial Public Class TermsController

  Public Shared Function GetTerm(termId As Integer, moduleId As Integer, locale As String) As TermInfo
   Return CType(CBO.FillObject(Data.DataProvider.Instance().GetTerm(termId, moduleId, locale), GetType(TermInfo)), TermInfo)
  End Function

  Public Shared Function GetTermsByPost(contentItemId As Int32, moduleId As Int32, locale As String) As List(Of TermInfo)
   Return DotNetNuke.Common.Utilities.CBO.FillCollection(Of TermInfo)(Data.DataProvider.Instance.GetTermsByPost(contentItemId, moduleId, locale))
  End Function

  Public Shared Function GetTermsByModule(moduleId As Int32, locale As String) As List(Of TermInfo)
   Return DotNetNuke.Common.Utilities.CBO.FillCollection(Of TermInfo)(Data.DataProvider.Instance.GetTermsByModule(moduleId, locale))
  End Function

  Public Shared Function GetTermsByVocabulary(moduleId As Int32, vocabularyId As Int32, locale As String) As Dictionary(Of String, TermInfo)
   Return GetTermsByVocabulary(moduleId, vocabularyId, locale, False)
  End Function

  Public Shared Function GetTermsByVocabulary(moduleId As Int32, vocabularyId As Int32, locale As String, clearCache As Boolean) As Dictionary(Of String, TermInfo)
   Dim CacheKey As String = String.Format("BlogVocab-{0}-{1}-{2}", moduleId, vocabularyId, locale)
   Dim res As Dictionary(Of String, TermInfo) = CType(DotNetNuke.Common.Utilities.DataCache.GetCache(CacheKey), Dictionary(Of String, TermInfo))
   If res Is Nothing Or clearCache Then
    res = New Dictionary(Of String, TermInfo)(StringComparer.CurrentCultureIgnoreCase)
    DotNetNuke.Common.Utilities.CBO.FillDictionary(Of String, TermInfo)("Name", Data.DataProvider.Instance.GetTermsByVocabulary(moduleId, vocabularyId, locale), res)
    DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey, res)
   End If
   Return res
  End Function

  Public Shared Function GetTermsByVocabulary(moduleId As Int32, vocabularyId As Int32) As Dictionary(Of Integer, TermInfo)
   Dim res As New Dictionary(Of Integer, TermInfo)
   DotNetNuke.Common.Utilities.CBO.FillDictionary(Of Integer, TermInfo)("TermID", Data.DataProvider.Instance.GetTermsByVocabulary(moduleId, vocabularyId, ""), res)
   Return res
  End Function

  Public Shared Function GetTermList(moduleId As Integer, termList As String, vocabularyId As Integer, autoCreate As Boolean, locale As String) As List(Of TermInfo)
   If termList Is Nothing Then termList = ""
   Dim termNames As String() = termList.Replace(";", ",").Trim(","c).Split(","c)
   Return GetTermList(moduleId, termNames.ToList, vocabularyId, autoCreate, locale)
  End Function

  Public Shared Function GetTermList(moduleId As Integer, termList As List(Of String), vocabularyId As Integer, autoCreate As Boolean, locale As String) As List(Of TermInfo)
   Dim vocab As Dictionary(Of String, TermInfo) = GetTermsByVocabulary(moduleId, vocabularyId, locale, True)
   Dim res As New List(Of TermInfo)
   For Each termName As String In termList
    Dim name As String = termName.Trim
    Dim existantTerm As TermInfo = Nothing
    If vocab.ContainsKey(name) Then existantTerm = vocab(name)
    If existantTerm IsNot Nothing Then
     res.Add(existantTerm)
    ElseIf autoCreate And name <> "" Then
     Dim termId As Integer = DotNetNuke.Entities.Content.Common.Util.GetTermController().AddTerm(New Term(vocabularyId) With {.Name = name})
     res.Add(New TermInfo(name) With {.Description = "", .TermId = termId, .TotalPosts = 0, .Weight = 0})
    End If
   Next
   Return res
  End Function

#Region " (De)serialization "
  Public Shared Function FromXml(xml As XmlNode) As List(Of TermInfo)
   Dim res As New List(Of TermInfo)
   If xml Is Nothing Then Return res
   For Each xTerm As XmlNode In xml.SelectNodes("Term")
    Dim t As New TermInfo
    t.FromXml(xTerm)
    res.Add(t)
   Next
   Return res
  End Function

  Public Shared Sub WriteVocabulary(writer As XmlTextWriter, name As String, vocabulary As List(Of TermInfo))
   writer.WriteStartElement(name)
   For Each t As TermInfo In vocabulary.Where(Function(x) CBool(x.ParentTermId Is Nothing))
    t.WriteXml(writer, vocabulary)
   Next
   writer.WriteEndElement() ' Vocabulary
  End Sub

  Public Shared Sub AddTags(moduleId As Integer, tagList As List(Of TermInfo))
   Dim allTags As Dictionary(Of String, TermInfo) = GetTermsByVocabulary(moduleId, 1, "")
   For Each t As TermInfo In tagList
    If Not allTags.ContainsKey(t.Name) Then
     Dim newTerm As New Term(1) With {.Name = t.Name.Trim, .Description = t.Description}
     newTerm.TermId = DotNetNuke.Entities.Content.Common.Util.GetTermController().AddTerm(newTerm)
     For Each l As String In t.NameLocalizations.Locales
      Data.DataProvider.Instance.SetTermLocalization(newTerm.TermId, l, t.NameLocalizations(l), t.DescriptionLocalizations(l))
     Next
    End If
   Next
  End Sub

  Public Shared Sub AddVocabulary(vocabularyId As Integer, vocabulary As List(Of TermInfo))
   For Each t As TermInfo In vocabulary.Where(Function(x) CBool(x.ParentTermId Is Nothing))
    AddTerm(vocabularyId, vocabulary, 0, t)
   Next
  End Sub

  Private Shared Sub AddTerm(vocabularyId As Integer, vocabulary As List(Of TermInfo), parentId As Integer, term As TermInfo)
   Dim newTerm As New Term(vocabularyId) With {.Name = term.Name.Trim, .Description = term.Description, .ParentTermId = parentId}
   newTerm.TermId = DotNetNuke.Entities.Content.Common.Util.GetTermController().AddTerm(newTerm)
   For Each l As String In term.NameLocalizations.Locales
    Data.DataProvider.Instance.SetTermLocalization(newTerm.TermId, l, term.NameLocalizations(l), term.DescriptionLocalizations(l))
   Next
   For Each t As TermInfo In vocabulary.Where(Function(x) CBool(x.ParentTermId = term.TermId))
    AddTerm(vocabularyId, vocabulary, newTerm.TermId, t)
   Next
  End Sub
#End Region

#Region " UI Functions "
  Public Shared Function GetCategoryTreeAsJson(vocabulary As Dictionary(Of String, TermInfo)) As String
   Return GetCategoryTreeAsJson(vocabulary, New List(Of Integer))
  End Function

  Public Shared Function GetCategoryTreeAsJson(vocabulary As Dictionary(Of String, TermInfo), selectedIds As List(Of Integer)) As String
   Dim childTreeBuilder As New StringBuilder
   GetCategoryTree(childTreeBuilder, vocabulary, -1, selectedIds)
   Dim res As String = childTreeBuilder.ToString
   If res.Length > 0 Then
    res = res.Substring(14) ' cut off the first children declaration
   End If
   Return res
  End Function

  Private Shared Sub GetCategoryTree(out As StringBuilder, vocabulary As Dictionary(Of String, TermInfo), parentId As Integer, selectedIds As List(Of Integer))
   Dim selection As IEnumerable(Of TermInfo)
   If parentId = -1 Then
    selection = vocabulary.Values.Where(Function(t)
                                         Return CBool(t.ParentTermId Is Nothing)
                                        End Function)
   Else
    selection = vocabulary.Values.Where(Function(t)
                                         Return t.ParentTermId IsNot Nothing AndAlso CBool(t.ParentTermId = parentId)
                                        End Function)
   End If

   If selection.Count > 0 Then
    out.Append(", ""children"": [")
    Dim first As Boolean = True
    For Each cat As TermInfo In selection
     If Not first Then out.Append(",")
     out.Append("{")
     out.Append(String.Format("""title"": ""{0}"",", cat.LocalizedName))
     out.Append(String.Format("""key"": ""{0}"",", cat.TermId))
     out.Append("""icon"": false,")
     out.Append("""expand"": true,")
     out.Append("""isFolder"": true,")
     out.Append(String.Format("""select"": {0}", IIf(selectedIds.Contains(cat.TermId), "true", "false")))
     GetCategoryTree(out, vocabulary, cat.TermId, selectedIds)
     out.Append("}")
     first = False
    Next
    out.Append("]")
   End If
  End Sub
#End Region

 End Class
End Namespace