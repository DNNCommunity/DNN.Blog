Imports System
Imports System.Data
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Tokens

Imports DotNetNuke.Modules.Blog.Data

Namespace Entities.Blogs

 Partial Public Class BlogsController

  Public Shared Function AddBlog(ByRef objBlog As BlogInfo, createdByUser As Integer) As Integer

  objBlog.BlogID = CType(DataProvider.Instance().AddBlog(objBlog.AutoApprovePingBack, objBlog.ModuleID, objBlog.AutoApproveTrackBack, objBlog.Copyright, objBlog.Description, objBlog.EnablePingBackReceive, objBlog.EnablePingBackSend, objBlog.EnableTrackBackReceive, objBlog.EnableTrackBackSend, objBlog.FullLocalization, objBlog.Image, objBlog.IncludeAuthorInFeed, objBlog.IncludeImagesInFeed, objBlog.Locale, objBlog.MustApproveGhostPosts, objBlog.OwnerUserId, objBlog.PublishAsOwner, objBlog.Published, objBlog.Syndicated, objBlog.SyndicationEmail, objBlog.Title, createdByUser), Integer)

   ' localization
   For Each l As String In objBlog.TitleLocalizations.Locales
    DataProvider.Instance().SetBlogLocalization(objBlog.BlogID, l, objBlog.TitleLocalizations(l), objBlog.DescriptionLocalizations(l))
   Next

   Return objBlog.BlogID

  End Function

  Public Shared Sub UpdateBlog(objBlog As BlogInfo, updatedByUser As Integer)

  DataProvider.Instance().UpdateBlog(objBlog.AutoApprovePingBack, objBlog.ModuleID, objBlog.AutoApproveTrackBack, objBlog.BlogID, objBlog.Copyright, objBlog.Description, objBlog.EnablePingBackReceive, objBlog.EnablePingBackSend, objBlog.EnableTrackBackReceive, objBlog.EnableTrackBackSend, objBlog.FullLocalization, objBlog.Image, objBlog.IncludeAuthorInFeed, objBlog.IncludeImagesInFeed, objBlog.Locale, objBlog.MustApproveGhostPosts, objBlog.OwnerUserId, objBlog.PublishAsOwner, objBlog.Published, objBlog.Syndicated, objBlog.SyndicationEmail, objBlog.Title, updatedByUser)

   ' localization
   For Each l As String In objBlog.TitleLocalizations.Locales
    DataProvider.Instance().SetBlogLocalization(objBlog.BlogID, l, objBlog.TitleLocalizations(l), objBlog.DescriptionLocalizations(l))
   Next

  End Sub

  Public Shared Sub DeleteBlog(blogID As Int32)

   DataProvider.Instance().DeleteBlog(blogID)

  End Sub

 End Class
End Namespace

