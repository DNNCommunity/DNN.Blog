'
' DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2005
' by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
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
'-------------------------------------------------------------------------
Imports System
Imports DotNetNuke.Modules.Blog.Data
Imports DotNetNuke.Common.Utilities

Namespace Business

 Public Class NewBlogUgradeController

#Region "Public data access methods"


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
