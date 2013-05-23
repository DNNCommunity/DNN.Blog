Imports System
Imports DotNetNuke

Namespace Data

 Partial Public MustInherit Class DataProvider

#Region " Shared/Static Methods "

  ' singleton reference to the instantiated object 
  Private Shared objProvider As DataProvider = Nothing

  ' constructor
  Shared Sub New()
   CreateProvider()
  End Sub

  ' dynamically create provider
  Private Shared Sub CreateProvider()
   objProvider = CType(DotNetNuke.Framework.Reflection.CreateObject("data", "DotNetNuke.Modules.Blog.Data", ""), DataProvider)
  End Sub

  ' return the provider
  Public Shared Shadows Function Instance() As DataProvider
   Return objProvider
  End Function

#End Region

#Region " General Methods "
  Public MustOverride Function GetNull(Field As Object) As Object
#End Region

#Region " BlogPermission Methods "
  Public MustOverride Function GetBlogPermission(blogId As Int32, permissionId As Int32, roleId As Int32, userId As Int32) As IDataReader
  Public MustOverride Sub AddBlogPermission(allowAccess As Boolean, blogId As Int32, expires As Date, permissionId As Int32, roleId As Int32, userId As Int32)
  Public MustOverride Sub UpdateBlogPermission(allowAccess As Boolean, blogId As Int32, expires As Date, permissionId As Int32, roleId As Int32, userId As Int32)
  Public MustOverride Sub DeleteBlogPermission(blogId As Int32, permissionId As Int32, roleId As Int32, userId As Int32)
#End Region

#Region " Blog Methods "
	Public MustOverride Function AddBlog(autoApprovePingBack As Boolean, moduleID As Int32, autoApproveTrackBack As Boolean, copyright As String, description As String, enablePingBackReceive As Boolean, enablePingBackSend As Boolean, enableTrackBackReceive As Boolean, enableTrackBackSend As Boolean, fullLocalization As Boolean, image As String, includeAuthorInFeed As Boolean, includeImagesInFeed As Boolean, locale As String, mustApproveGhostPosts As Boolean, ownerUserId As Int32, publishAsOwner As Boolean, published As Boolean, syndicated As Boolean, syndicationEmail As String, title As String, createdByUser As Int32) As Integer	
	Public MustOverride Sub UpdateBlog(autoApprovePingBack As Boolean, moduleID As Int32, autoApproveTrackBack As Boolean, blogID As Int32, copyright As String, description As String, enablePingBackReceive As Boolean, enablePingBackSend As Boolean, enableTrackBackReceive As Boolean, enableTrackBackSend As Boolean, fullLocalization As Boolean, image As String, includeAuthorInFeed As Boolean, includeImagesInFeed As Boolean, locale As String, mustApproveGhostPosts As Boolean, ownerUserId As Int32, publishAsOwner As Boolean, published As Boolean, syndicated As Boolean, syndicationEmail As String, title As String, updatedByUser As Int32)	
  Public MustOverride Sub DeleteBlog(blogID As Int32)
#End Region

#Region " Comment Methods "
  Public MustOverride Function GetComment(commentID As Int32) As IDataReader
  Public MustOverride Function AddComment(approved As Boolean, author As String, comment As String, contentItemId As Int32, email As String, parentId As Int32, website As String, createdByUser As Int32) As Integer
  Public MustOverride Sub UpdateComment(approved As Boolean, author As String, comment As String, commentID As Int32, contentItemId As Int32, email As String, parentId As Int32, website As String, updatedByUser As Int32)
  Public MustOverride Sub DeleteComment(commentID As Int32)
#End Region

#Region " LegacyUrl Methods "
	Public MustOverride Sub AddLegacyUrl(contentItemId As Int32, entryId As Int32, url As String)
#End Region

#Region " Post Methods "
  Public MustOverride Function GetPost(contentItemId As Int32, moduleId As Int32, locale As String) As IDataReader
  Public MustOverride Function AddPost(allowComments As Boolean, blogID As Int32, content As String, copyright As String, displayCopyright As Boolean, image As String, locale As String, published As Boolean, publishedOnDate As Date, summary As String, termIds As String, title As String, viewCount As Int32, createdByUser As Int32) As Integer
  Public MustOverride Sub UpdatePost(allowComments As Boolean, blogID As Int32, content As String, contentItemId As Int32, copyright As String, displayCopyright As Boolean, image As String, locale As String, published As Boolean, publishedOnDate As Date, summary As String, termIds As String, title As String, viewCount As Int32, updatedByUser As Int32)
  Public MustOverride Sub DeletePost(contentItemId As Int32)
#End Region

 End Class

End Namespace