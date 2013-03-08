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

  Public Shared Function GetTermsByEntry(contentItemId As Int32, moduleId As Int32) As List(Of TermInfo)
   Return DotNetNuke.Common.Utilities.CBO.FillCollection(Of TermInfo)(Data.DataProvider.Instance.GetTermsByEntry(contentItemId, moduleId))
  End Function

  Public Shared Function GetTermsByModule(moduleId As Int32) As List(Of TermInfo)
   Return DotNetNuke.Common.Utilities.CBO.FillCollection(Of TermInfo)(Data.DataProvider.Instance.GetTermsByModule(moduleId))
  End Function

  Public Shared Function GetTermsByVocabulary(moduleId As Int32, vocabularyId As Int32) As List(Of TermInfo)
   Return DotNetNuke.Common.Utilities.CBO.FillCollection(Of TermInfo)(Data.DataProvider.Instance.GetTermsByVocabulary(moduleId, vocabularyId))
  End Function

  Public Shared Function GetTermList(moduleId As Integer, termList As String, vocabularyId As Integer, autoCreate As Boolean) As List(Of TermInfo)
   Dim termNames As String() = termList.Replace(";", ",").Trim(","c).Split(","c)
   Return GetTermList(moduleId, termNames.ToList, vocabularyId, autoCreate)
  End Function

  Public Shared Function GetTermList(moduleId As Integer, termList As List(Of String), vocabularyId As Integer, autoCreate As Boolean) As List(Of TermInfo)
   Dim vocab As List(Of TermInfo) = GetTermsByVocabulary(moduleId, vocabularyId)
   Dim res As New List(Of TermInfo)
   For Each termName As String In termList
    Dim name As String = termName
    Dim existantTerm As TermInfo = vocab.Where(Function(t) t.Name.ToLower() = name.ToLower()).FirstOrDefault()
    If existantTerm IsNot Nothing Then
     res.Add(existantTerm)
    ElseIf autoCreate Then
     Dim termId As Integer = DotNetNuke.Entities.Content.Common.Util.GetTermController().AddTerm(New Term(vocabularyId) With {.Name = name})
     res.Add(New TermInfo With {.Description = "", .Name = name, .ParentTermId = 0, .TermId = termId, .TotalPosts = 0, .Weight = 0})
    End If
   Next
   Return res
  End Function

 End Class
End Namespace