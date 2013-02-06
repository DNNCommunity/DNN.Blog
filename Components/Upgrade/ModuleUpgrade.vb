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
Option Strict On
Option Explicit On

Imports DotNetNuke.Modules.Blog.Controllers
Imports DotNetNuke.Entities.Content.Taxonomy
Imports DotNetNuke.Modules.Blog.Integration
Imports System.Linq
Imports DotNetNuke.Entities.Content
Imports DotNetNuke.Modules.Blog.Entities
Imports DotNetNuke.Services.Exceptions

Namespace Upgrade

 Public Class ModuleUpgrade

#Region "Category/Tag Migration"

  Friend Function MigrateTaxonomyFolksonomy() As String
   Try
    Dim message As String
    Dim colEntries As New List(Of EntryInfo)

    ' handle notification types
    Notifications.AddNotificationTypes()
    message = "Created Notification Types & Actions for the Blog Module." & vbCrLf & vbCrLf

    ' handle content item creation (for all blog entries in all portals)
    Dim colBlogs As New List(Of BlogInfo)
    colBlogs = ModuleUpgradeController.GetAllBlogsForUpgrade()

    If colBlogs IsNot Nothing Then
     If colBlogs.Count > 0 Then
      ' we want to loop through each portal, not blog so limit the results
      Dim disBlogs As List(Of Integer) = (From t In colBlogs Select t.PortalID).Distinct().ToList()

      For Each i As Integer In disBlogs
       CreateContentItems(i)
       message += "Created " + disBlogs.Count.ToString() + " Content Items for Portal: " + i.ToString() & vbCrLf & vbCrLf
      Next
     End If
    End If

    ' deal w/ category/tag migration
    colEntries = EntryController.RetrieveTaxonomyRelatedPosts()
    message = MigrateTaxonomy(colEntries, message)

    Return message
   Catch ex As Exception
    LogException(ex)
    Return ex.Message.ToString()
   End Try
  End Function

  Friend Function MigrateTaxonomy(ByVal colEntries As List(Of EntryInfo), ByVal message As String) As String
   Dim countContentItems As Integer = 0
   Dim countCategories As Integer = 0
   Dim countTags As Integer = 0

   If colEntries.Count > 0 Then
    ' loop through tags (old system), create in core (vocab = 1). KEEP collection of new tags available to query by name later
    Dim colOldTags As List(Of MigrateTagInfo)
    colOldTags = ModuleUpgradeController.GetAllTagsForUpgrade()

    Dim colNewTerms As New List(Of Term)

    If colOldTags IsNot Nothing Then
     For Each objTag As MigrateTagInfo In colOldTags
      Dim objTerm As New Term
      objTerm = Terms.CreateAndReturnTerm(objTag.Tag, 1)
      colNewTerms.Add(objTerm)

      ' update
      objTag.NewTermId = objTerm.TermId

      countTags += 1
     Next
    End If
    message += "Migrated " + countTags.ToString() + " tags. " & vbCrLf & vbCrLf

    ' loop through categories (old system), from table ordered by portal id, create under new vocabulary. KEEP both collections available
    Dim colOldCategories As List(Of MigrateCategoryInfo)
    colOldCategories = ModuleUpgradeController.GetAllCategoriesForUpgrade()

    Dim cntVocabulary As New VocabularyController
    'Dim colVocabs As IQueryable(Of Vocabulary) = cntVocabulary.GetVocabularies()

    Dim currentPortalId As Integer = -1
    Dim currentVocabId As Integer = -1
    Dim currentParentId As Integer = -1

    If colOldCategories IsNot Nothing Then
     For Each objCategory As MigrateCategoryInfo In colOldCategories
      If Not (objCategory.PortalId = currentPortalId) Then
       currentPortalId = objCategory.PortalId
       '' let's first see if there is an existing blog vocabulary for this portal
       'Dim objTempVocab As Vocabulary = colVocabs.Where(Function(s) s.ScopeId = currentPortalId And s.Name = "Blog").SingleOrDefault()

       'If objTempVocab IsNot Nothing Then
       '    currentVocabId = objTempVocab.VocabularyId
       'Else
       Dim cntScope As New ScopeTypeController
       Dim objScope As ScopeType = cntScope.GetScopeTypes().Where(Function(s) s.ScopeType = "Portal").SingleOrDefault()
       Dim objVocab As New Vocabulary

       objVocab.Name = "Blog Topics"
       objVocab.IsSystem = False
       objVocab.Weight = 0
       objVocab.Description = "Automatically generated for blog module."
       objVocab.ScopeId = currentPortalId
       objVocab.ScopeTypeId = objScope.ScopeTypeId
       'NOTE: CP - THis should be hierarchy, having a problem getting it to work.
       objVocab.Type = VocabularyType.Simple
       objVocab.VocabularyId = cntVocabulary.AddVocabulary(objVocab)

       currentVocabId = objVocab.VocabularyId

       Business.Utility.UpdateBlogModuleSetting(currentPortalId, -1, "VocabularyId", currentVocabId.ToString)
       'End If
      End If

      If objCategory.ParentId > 0 Then
       If Not (objCategory.ParentId = currentParentId) Then
        Dim tempParentId As Integer = objCategory.ParentId
        Dim objParentCategory As MigrateCategoryInfo = colOldCategories.Where(Function(t) t.CatId = tempParentId).FirstOrDefault
        Dim termController As ITermController = DotNetNuke.Entities.Content.Common.Util.GetTermController()
        Dim objParentTerm As Term = termController.GetTermsByVocabulary(currentVocabId).Where(Function(t) t.Name.ToLower = objParentCategory.Category.ToLower).FirstOrDefault

        If objParentTerm IsNot Nothing Then
         currentParentId = objParentTerm.TermId
        End If
       End If
      End If

      'Dim termController As ITermController = DotNetNuke.Entities.Content.Common.Util.GetTermController()
      'Dim existantTerm As Term
      'existantTerm = termController.GetTermsByVocabulary(currentVocabId).Where(Function(t) t.TermId = id).FirstOrDefault()



      Dim objTerm As New Term
      objTerm = Terms.CreateAndReturnTerm(objCategory.Category, currentVocabId, currentParentId)
      colNewTerms.Add(objTerm)

      ' update 
      objCategory.NewTermId = objTerm.TermId

      countCategories += 1
     Next
    End If
    message += "Migrated " + countCategories.ToString() + " categories. " & vbCrLf & vbCrLf

    ' At this point we have all categories and tags created in the core and also have the old categories/tags in collections associated w/ the new corresponding term id's

    ' loop through all entries that have a category or tag associated with them
    For Each objEntry As EntryInfo In colEntries
     Dim portalId As Integer = -1

     If objEntry.ContentItemId < 1 Then
      Dim cntTaxonomy As New Content()
      Dim objContentItem As ContentItem = cntTaxonomy.CreateContentItem(objEntry, objEntry.TabID)
      objEntry.ContentItemId = objContentItem.ContentItemId

      countContentItems += 1
     End If

     Dim entryTerms As New List(Of Term)

     ' handle tags
     Dim colTags As List(Of MigrateTagInfo) = ModuleUpgradeController.GetEntryTagsForUpgrade(objEntry.EntryID)
     For Each objOldTag As MigrateTagInfo In colTags
      Dim tagid As Integer = objOldTag.TagId
      Dim objMatchedTag As MigrateTagInfo = colOldTags.Where(Function(t) t.TagId = tagid).FirstOrDefault

      If objMatchedTag IsNot Nothing Then
       Dim objMatchedTerm As Term
       objMatchedTerm = Terms.GetTermById(objMatchedTag.NewTermId, 1)

       If objMatchedTerm IsNot Nothing Then
        entryTerms.Add(objMatchedTerm)
       End If

       portalId = objMatchedTag.PortalId
      End If
     Next

     ' handle categories
     Dim colCats As List(Of MigrateCategoryInfo) = ModuleUpgradeController.GetEntryCategoriesForUpgrade(objEntry.EntryID)
     For Each objOldCat As MigrateCategoryInfo In colCats
      Dim catid As Integer = objOldCat.CatId
      Dim objMatchedCategory As MigrateCategoryInfo = colOldCategories.Where(Function(t) t.CatId = catid).FirstOrDefault

      If objMatchedCategory IsNot Nothing Then
       Dim objMatchedTerm As Term
       objMatchedTerm = Terms.GetTermById(objMatchedCategory.NewTermId, currentVocabId)

       If objMatchedTerm IsNot Nothing Then
        entryTerms.Add(objMatchedTerm)
       End If

       portalId = objMatchedCategory.PortalId
      End If
     Next

     If portalId > -1 Then
      ' update content item
      objEntry.Terms.Clear()
      objEntry.Terms.AddRange(entryTerms)
      EntryController.UpdateEntry(objEntry, objEntry.TabID, portalId, currentVocabId)
     End If
    Next
    message += "Migrated " + countContentItems.ToString() + " content items. " & vbCrLf & vbCrLf
   End If

   Return message
  End Function

  Friend Sub CreateContentItems(ByVal portalId As Integer)
   Dim colEntries As List(Of EntryInfo) = EntryController.GetAllEntriesByPortal(portalId)

   For Each objEntry As EntryInfo In colEntries
    If objEntry.ContentItemId < 1 Then
     Dim cntTaxonomy As New Content()
     Dim objContentItem As ContentItem = cntTaxonomy.CreateContentItem(objEntry, objEntry.TabID)
     objEntry.ContentItemId = objContentItem.ContentItemId

     EntryController.UpdateEntry(objEntry, objEntry.TabID, portalId, 1)
    End If
   Next
  End Sub

#End Region

 End Class

End Namespace