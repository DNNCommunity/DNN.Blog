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
