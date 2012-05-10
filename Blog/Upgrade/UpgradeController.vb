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

Imports DotNetNuke.Modules.Blog.Providers.Data
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Modules.Blog.Components.Entities

Namespace Upgrade

 Public Class UgradeController

#Region "Public data access methods"
  Public Shared Function ListCatsByEntry(ByVal EntryID As Integer) As List(Of CategoryInfo)
   Dim CatList As List(Of CategoryInfo)
   CatList = CBO.FillCollection(Of CategoryInfo)(DataProvider.Instance().GetCategoriesByEntry(EntryID))
   Return CatList
  End Function

  Public Shared Function GetAllCategoriesForUpgrade() As List(Of MigrateCategoryInfo)
   Return CBO.FillCollection(Of MigrateCategoryInfo)(DataProvider.Instance().GetAllCategoriesForUpgrade())
  End Function

  Public Shared Function GetAllTagsForUpgrade() As List(Of MigrateTagInfo)
   Return CBO.FillCollection(Of MigrateTagInfo)(DataProvider.Instance().GetAllTagsForUpgrade())
  End Function

  Public Function Upgrade_ListBlogs(ByVal PortalID As Integer, ByVal ParentBlogID As Integer, ByVal ShowNonPublic As Boolean) As ArrayList

   Return CBO.FillCollection(DataProvider.Instance().Upgrade_ListBlogs(PortalID, ParentBlogID, ShowNonPublic), GetType(BlogInfo))

  End Function

  Public Sub Upgrade_DeleteBlog(ByVal blogID As Integer)

   DataProvider.Instance().Upgrade_DeleteBlog(blogID)

  End Sub

  Public Function Upgrade_ListEntriesByBlog(ByVal BlogID As Integer, ByVal BlogDate As Date, Optional ByVal ShowNonPublic As Boolean = False, Optional ByVal ShowNonPublished As Boolean = False) As ArrayList

   Return CBO.FillCollection(DataProvider.Instance().Upgrade_ListEntriesByBlog(BlogID, BlogDate, ShowNonPublic, ShowNonPublished), GetType(EntryInfo))

  End Function

  Public Sub Upgrade_DeleteEntry(ByVal EntryID As Integer)

   DataProvider.Instance().Upgrade_DeleteEntry(EntryID)

  End Sub

  Public Function Upgrade_ListComments(ByVal EntryID As Integer) As ArrayList

   Return CBO.FillCollection(DataProvider.Instance().Upgrade_ListComments(EntryID), GetType(CommentInfo))

  End Function

  Public Sub Upgrade_DeleteComment(ByVal commentID As Integer)

   DataProvider.Instance().Upgrade_DeleteComment(commentID)

  End Sub

  Public Sub UpgradeModuleDefId(ByVal ModuleDefId As Integer, ByVal ModuleId As Integer)
   DataProvider.Instance.Upgrade_UpdateModuleDefId(ModuleDefId, ModuleId)
  End Sub

  Public Function Upgrade_GetBlogModuleSettings() As IDataReader
   Return DataProvider.Instance.Upgrade_GetBlogModuleSettings()
  End Function

#End Region

 End Class

End Namespace