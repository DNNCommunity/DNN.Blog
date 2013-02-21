'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2011
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
Option Strict On
Option Explicit On

Imports DotNetNuke.Entities.Content
Imports DotNetNuke.Entities.Content.Common
Imports DotNetNuke.Entities.Content.Taxonomy
Imports System.Linq
Imports DotNetNuke.Modules.Blog.Entities
Imports DotNetNuke.Modules.Blog.Entities.Entries

Namespace Integration

 ''' <summary>
 ''' This class is used for managing taxonomy/folksonomy related terms via integration with the core.
 ''' </summary>
 ''' <remarks></remarks>
 Public Class Terms

  ''' <summary>
  ''' 
  ''' </summary>
  ''' <param name="objThread"></param>
  ''' <param name="objContent"></param>
  ''' <remarks></remarks>
  Friend Sub ManageEntryTerms(objThread As EntryInfo, objContent As ContentItem)
   RemoveEntryTerms(objContent)

   For Each term As DotNetNuke.Entities.Content.Taxonomy.Term In objThread.Terms
    Util.GetTermController().AddTermToContent(term, objContent)
   Next
  End Sub

  ''' <summary>
  ''' This will remove all terms from a content item object (categories or tags)
  ''' </summary>
  ''' <param name="objContent"></param>
  ''' <remarks></remarks>
  Friend Sub RemoveEntryTerms(objContent As ContentItem)
   Util.GetTermController().RemoveTermsFromContent(objContent)
  End Sub

  ''' <summary>
  ''' This method will create a term under a specific vocabulary (meant for tags) in the core data store and return the newly created term.
  ''' </summary>
  ''' <param name="name"></param>
  ''' <param name="vocabularyId"></param>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Friend Shared Function CreateAndReturnTerm(name As String, vocabularyId As Integer) As Term
   Dim termController As ITermController = DotNetNuke.Entities.Content.Common.Util.GetTermController()
   Dim existantTerm As Term = termController.GetTermsByVocabulary(vocabularyId).Where(Function(t) t.Name.ToLower() = name.ToLower()).FirstOrDefault()
   If existantTerm IsNot Nothing Then
    Return existantTerm
   End If

   Dim termId As Integer = termController.AddTerm(New Term(vocabularyId) With { _
                                                     .Name = name _
                                                     })
   Return New Term() With { _
       .Name = name, _
       .TermId = termId _
       }
  End Function

  Friend Shared Function CreateAndReturnTerm(name As String, vocabularyId As Integer, parentId As Integer) As Term
   Dim termController As ITermController = DotNetNuke.Entities.Content.Common.Util.GetTermController()
   Dim colTerms As IQueryable(Of Term) = termController.GetTermsByVocabulary(vocabularyId).Where(Function(t) t.Name.ToLower() = name.ToLower())
   Dim existingTerm As Term = colTerms.Where(Function(s) s.VocabularyId = vocabularyId).SingleOrDefault

   If existingTerm IsNot Nothing Then
    If existingTerm.TermId > 0 Then
     If existingTerm.ParentTermId = parentId Then
      Return (existingTerm)
     End If
    End If
   End If

   Dim termId As Integer = termController.AddTerm(New Term(vocabularyId) With { _
                                                     .Name = name, _
                                                     .ParentTermId = parentId _
                                                     })
   Return New Term() With { _
       .Name = name, _
       .ParentTermId = parentId, _
       .TermId = termId _
       }
  End Function


  ''' <summary>
  ''' This method checks to see if a term exists under a specific vocabulary and returns it (if available)
  ''' </summary>
  ''' <param name="id"></param>
  ''' <param name="vocabularyId"></param>
  ''' <returns>The core 'Term'.</returns>
  Friend Shared Function GetTermById(id As Integer, vocabularyId As Integer) As Term
   Dim termController As ITermController = DotNetNuke.Entities.Content.Common.Util.GetTermController()
   Dim existantTerm As Term = termController.GetTermsByVocabulary(vocabularyId).Where(Function(t) t.TermId = id).FirstOrDefault()
   Return existantTerm
  End Function

  ''' <summary>
  ''' This method queries the core Vocabulary and returns only application level vocabularies available to the current portal.
  ''' </summary>
  ''' <param name="portalId"></param>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Friend Shared Function GetPortalVocabularies(portalId As Integer) As List(Of Vocabulary)
   Dim cntVocab As IVocabularyController = DotNetNuke.Entities.Content.Common.Util.GetVocabularyController()
   Dim colVocabularies As IQueryable(Of Vocabulary) = cntVocab.GetVocabularies()
   Dim portalVocabularies As IQueryable(Of Vocabulary) = From v In colVocabularies Where v.ScopeTypeId = 2 And v.ScopeId = portalId

   Return portalVocabularies.ToList()
  End Function

 End Class

End Namespace