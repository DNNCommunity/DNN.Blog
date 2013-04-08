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
Imports System.Linq

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Content.Taxonomy

Namespace Entities.Terms
 Public Class TermsController

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
   Dim res As New Dictionary(Of String, TermInfo)(StringComparer.CurrentCultureIgnoreCase)
   DotNetNuke.Common.Utilities.CBO.FillDictionary(Of String, TermInfo)("Name", Data.DataProvider.Instance.GetTermsByVocabulary(moduleId, vocabularyId, locale), res)
   Return res
  End Function

  Public Shared Function GetTermList(moduleId As Integer, termList As String, vocabularyId As Integer, autoCreate As Boolean, locale As String) As List(Of TermInfo)
   Dim termNames As String() = termList.Replace(";", ",").Trim(","c).Split(","c)
   Return GetTermList(moduleId, termNames.ToList, vocabularyId, autoCreate, locale)
  End Function

  Public Shared Function GetTermList(moduleId As Integer, termList As List(Of String), vocabularyId As Integer, autoCreate As Boolean, locale As String) As List(Of TermInfo)
   Dim vocab As Dictionary(Of String, TermInfo) = GetTermsByVocabulary(moduleId, vocabularyId, locale)
   Dim res As New List(Of TermInfo)
   For Each termName As String In termList
    Dim name As String = termName
    Dim existantTerm As TermInfo = Nothing
    If vocab.ContainsKey(name) Then existantTerm = vocab(name)
    If existantTerm IsNot Nothing Then
     res.Add(existantTerm)
    ElseIf autoCreate And name <> "" Then
     Dim termId As Integer = DotNetNuke.Entities.Content.Common.Util.GetTermController().AddTerm(New Term(vocabularyId) With {.Name = name})
     res.Add(New TermInfo(name) With {.Description = "", .ParentTermId = 0, .TermId = termId, .TotalPosts = 0, .Weight = 0})
    End If
   Next
   Return res
  End Function

#Region " UI Functions "
  Public Shared Function GetCategoryTreeAsJson(vocabulary As Dictionary(Of String, TermInfo)) As String
   Return GetCategoryTreeAsJson(vocabulary, New List(Of Integer))
  End Function

  Public Shared Function GetCategoryTreeAsJson(vocabulary As Dictionary(Of String, TermInfo), selectedIds As List(Of Integer)) As String
   Dim childTreeBuilder As New StringBuilder
   GetCategoryTree(childTreeBuilder, vocabulary, -1, selectedIds)
   Return childTreeBuilder.ToString
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
    out.Append(", children: [")
    Dim first As Boolean = True
    For Each cat As TermInfo In selection
     If Not first Then out.Append(",")
     out.Append("{")
     out.Append(String.Format("title: '{0}',", cat.Name))
     out.Append(String.Format("key: '{0}',", cat.TermId))
     out.Append("icon: false,")
     out.Append("expand: true,")
     out.Append("isFolder: true,")
     out.Append(String.Format("select: {0}", IIf(selectedIds.Contains(cat.TermId), "true", "false")))
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