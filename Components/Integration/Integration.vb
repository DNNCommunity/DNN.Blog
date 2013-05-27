'
' Bring2mind - http://www.bring2mind.net
' Copyright (c) 2013
' by Bring2mind
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
Imports DotNetNuke.Entities.Content.Taxonomy

Namespace Integration
 Public Class Integration

  Public Const ContentTypeName As String = "DNN_Blog_Post"
  Public Const JournalBlogTypeName As String = "blog"
  Public Const JournalCommentTypeName As String = "comment"
  Public Const NotificationPublishingTypeName As String = "DNN_Blog_Post_Publishing"
  Public Const NotificationCommentApprovalTypeName As String = "DNN_Blog_Post_CommentApproval"
  Public Const NotificationCommentReportedTypeName As String = "DNN_Blog_Post_CommentReported"
  Public Const NotificationCommentAddedTypeName As String = "DNN_Blog_Post_CommentAdded"

  Public Shared Function CreateNewVocabulary(portalId As Integer) As Vocabulary

   Dim name As String = "Blog Categories"
   Dim cntScope As New ScopeTypeController
   Dim cntVocabulary As New VocabularyController
   Dim i As Integer = 1
   Do While cntVocabulary.GetVocabularies.Where(Function(v) v.Name = name).Count > 0
    name = "Blog Categories " + i.ToString
   Loop
   Dim objScope As ScopeType = cntScope.GetScopeTypes().Where(Function(s) s.ScopeType = "Portal").SingleOrDefault()
   Dim objVocab As New Vocabulary
   objVocab.Name = name
   objVocab.IsSystem = False
   objVocab.Weight = 0
   objVocab.Description = "Automatically generated for blog module."
   objVocab.ScopeId = portalId
   objVocab.ScopeTypeId = objScope.ScopeTypeId
   objVocab.Type = VocabularyType.Hierarchy
   objVocab.VocabularyId = cntVocabulary.AddVocabulary(objVocab)
   Return objVocab

  End Function

 End Class
End Namespace
