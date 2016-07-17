'
' DNN Connect - http://dnn-connect.org
' Copyright (c) 2015
' by DNN Connect
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

Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Tokens
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Entities.Posts
Imports DotNetNuke.Modules.Blog.Entities.Terms
Imports DotNetNuke.Modules.Blog.Entities.Comments

Namespace Templating
 Public Class BlogTokenReplace
  Inherits GenericTokenReplace

  Public Sub New(moduleId As Integer)
   MyBase.new(Scope.DefaultSettings)

   Dim actModule As DotNetNuke.Entities.Modules.ModuleInfo = (New DotNetNuke.Entities.Modules.ModuleController).GetModule(moduleId)
   ModuleInfo = actModule
   UseObjectLessExpression = False

  End Sub

  Public Sub New(actModule As DotNetNuke.Entities.Modules.ModuleInfo, security As Modules.Blog.Security.ContextSecurity, blog As BlogInfo, post As PostInfo, settings As Common.ModuleSettings, viewSettings As ViewSettings)
   MyBase.new(Scope.DefaultSettings)

   ModuleInfo = actModule
   UseObjectLessExpression = False
   PropertySource("security") = security
   PropertySource("settings") = settings
   PropertySource("viewsettings") = viewSettings
   If blog IsNot Nothing Then
    PropertySource("blog") = blog
    PropertySource("owner") = New LazyLoadingUser(PortalSettings.PortalId, blog.OwnerUserId, blog.Username)
   End If
   If post IsNot Nothing Then
    PropertySource("post") = post
   End If

  End Sub

  Public Sub New(actModule As DotNetNuke.Entities.Modules.ModuleInfo, security As Modules.Blog.Security.ContextSecurity, blog As BlogInfo, Post As PostInfo, settings As Common.ModuleSettings, viewSettings As ViewSettings, comment As CommentInfo)
   MyBase.new(Scope.DefaultSettings)

   PrimaryObject = comment
   ModuleInfo = actModule
   UseObjectLessExpression = False
   PropertySource("security") = security
   PropertySource("settings") = settings
   PropertySource("viewsettings") = viewSettings
   If Post IsNot Nothing Then
    PropertySource("post") = Post
    PropertySource("author") = New LazyLoadingUser(PortalSettings.PortalId, Post.CreatedByUserID, Post.Username)
    PropertySource("blog") = Post.Blog
    PropertySource("owner") = New LazyLoadingUser(PortalSettings.PortalId, Post.Blog.OwnerUserId, Post.Blog.Username)
   ElseIf blog IsNot Nothing Then
    PropertySource("blog") = blog
    PropertySource("owner") = New LazyLoadingUser(PortalSettings.PortalId, blog.OwnerUserId, blog.Username)
   End If
   PropertySource("comment") = comment
   PropertySource("commenter") = New LazyLoadingUser(PortalSettings.PortalId, comment.CreatedByUserID, comment.Username)

  End Sub

  Public Sub New(blogModule As BlogModuleBase)
   MyBase.new(Scope.DefaultSettings)

   ModuleInfo = blogModule.ModuleConfiguration
   UseObjectLessExpression = False
   PropertySource("query") = blogModule.BlogContext
   PropertySource("security") = blogModule.BlogContext.Security
   PropertySource("urls") = blogModule.BlogContext.ModuleUrls
   PropertySource("settings") = blogModule.Settings
   PropertySource("viewsettings") = blogModule.ViewSettings
   If blogModule.BlogContext.Blog IsNot Nothing Then
    PropertySource("blog") = blogModule.BlogContext.Blog
    PropertySource("owner") = New LazyLoadingUser(PortalSettings.PortalId, blogModule.BlogContext.Blog.OwnerUserId, blogModule.BlogContext.Blog.Username)
   End If
   If blogModule.BlogContext.Post IsNot Nothing Then
    PropertySource("post") = blogModule.BlogContext.Post
    PropertySource("author") = New LazyLoadingUser(PortalSettings.PortalId, blogModule.BlogContext.Post.CreatedByUserID, blogModule.BlogContext.Post.Username)
   End If
   If blogModule.BlogContext.Term IsNot Nothing Then
    PropertySource("selectedterm") = blogModule.BlogContext.Term
   End If
   If blogModule.BlogContext.Author IsNot Nothing Then
    PropertySource("author") = blogModule.BlogContext.Author
   End If

  End Sub

  Public Sub New(blogModule As BlogModuleBase, blog As BlogInfo)
   MyBase.new(Scope.DefaultSettings)

   PrimaryObject = blog
   ModuleInfo = blogModule.ModuleConfiguration
   UseObjectLessExpression = False
   PropertySource("query") = blogModule.BlogContext
   PropertySource("security") = blogModule.BlogContext.Security
   PropertySource("urls") = blogModule.BlogContext.ModuleUrls
   PropertySource("settings") = blogModule.Settings
   PropertySource("viewsettings") = blogModule.ViewSettings
   PropertySource("blog") = blog
   PropertySource("owner") = New LazyLoadingUser(PortalSettings.PortalId, blog.OwnerUserId, blog.Username)
   If blogModule.BlogContext.Post IsNot Nothing Then
    PropertySource("post") = blogModule.BlogContext.Post
    PropertySource("author") = New LazyLoadingUser(PortalSettings.PortalId, blogModule.BlogContext.Post.CreatedByUserID, blogModule.BlogContext.Post.Username)
   ElseIf blogModule.BlogContext.Author IsNot Nothing Then
    PropertySource("author") = blogModule.BlogContext.Author
   End If
   If blogModule.BlogContext.Term IsNot Nothing Then
    PropertySource("selectedterm") = blogModule.BlogContext.Term
   End If

  End Sub

  Public Sub New(blogModule As BlogModuleBase, objBlogCalendar As BlogCalendarInfo)
   MyBase.new(Scope.DefaultSettings)

   PrimaryObject = objBlogCalendar
   ModuleInfo = blogModule.ModuleConfiguration
   UseObjectLessExpression = False
   PropertySource("query") = blogModule.BlogContext
   PropertySource("security") = blogModule.BlogContext.Security
   PropertySource("urls") = blogModule.BlogContext.ModuleUrls
   PropertySource("settings") = blogModule.Settings
   PropertySource("viewsettings") = blogModule.ViewSettings
   If blogModule.BlogContext.Blog IsNot Nothing Then
    PropertySource("blog") = blogModule.BlogContext.Blog
   End If
   PropertySource("calendar") = objBlogCalendar
   If blogModule.BlogContext.Post IsNot Nothing Then
    PropertySource("post") = blogModule.BlogContext.Post
    PropertySource("author") = New LazyLoadingUser(PortalSettings.PortalId, blogModule.BlogContext.Post.CreatedByUserID, blogModule.BlogContext.Post.Username)
   ElseIf blogModule.BlogContext.Author IsNot Nothing Then
    PropertySource("author") = blogModule.BlogContext.Author
   End If
   If blogModule.BlogContext.Term IsNot Nothing Then
    PropertySource("selectedterm") = blogModule.BlogContext.Term
   End If

  End Sub

  Public Sub New(blogModule As BlogModuleBase, objAuthor As LazyLoadingUser)
   MyBase.new(Scope.DefaultSettings)

   PrimaryObject = objAuthor
   ModuleInfo = blogModule.ModuleConfiguration
   UseObjectLessExpression = False
   PropertySource("query") = blogModule.BlogContext
   PropertySource("security") = blogModule.BlogContext.Security
   PropertySource("urls") = blogModule.BlogContext.ModuleUrls
   PropertySource("settings") = blogModule.Settings
   PropertySource("viewsettings") = blogModule.ViewSettings
   If blogModule.BlogContext.Blog IsNot Nothing Then
    PropertySource("blog") = blogModule.BlogContext.Blog
   End If
   PropertySource("author") = objAuthor
   If blogModule.BlogContext.Post IsNot Nothing Then
    PropertySource("post") = blogModule.BlogContext.Post
   End If
   If blogModule.BlogContext.Term IsNot Nothing Then
    PropertySource("selectedterm") = blogModule.BlogContext.Term
   End If

  End Sub

  Public Sub New(blogModule As BlogModuleBase, Post As PostInfo)
   MyBase.new(Scope.DefaultSettings)

   PrimaryObject = Post
   ModuleInfo = blogModule.ModuleConfiguration
   UseObjectLessExpression = False
   PropertySource("query") = blogModule.BlogContext
   PropertySource("security") = blogModule.BlogContext.Security
   PropertySource("urls") = blogModule.BlogContext.ModuleUrls
   PropertySource("settings") = blogModule.Settings
   PropertySource("viewsettings") = blogModule.ViewSettings
   PropertySource("post") = Post
   PropertySource("author") = New LazyLoadingUser(PortalSettings.PortalId, Post.CreatedByUserID, Post.Username)
   PropertySource("blog") = Post.Blog
   PropertySource("owner") = New LazyLoadingUser(PortalSettings.PortalId, Post.Blog.OwnerUserId, Post.Blog.Username)
   If blogModule.BlogContext.Term IsNot Nothing Then
    PropertySource("selectedterm") = blogModule.BlogContext.Term
   End If

  End Sub

  Public Sub New(blogModule As BlogModuleBase, Post As PostInfo, term As TermInfo)
   MyBase.new(Scope.DefaultSettings)

   PrimaryObject = term
   ModuleInfo = blogModule.ModuleConfiguration
   UseObjectLessExpression = False
   PropertySource("query") = blogModule.BlogContext
   PropertySource("security") = blogModule.BlogContext.Security
   PropertySource("urls") = blogModule.BlogContext.ModuleUrls
   PropertySource("settings") = blogModule.Settings
   PropertySource("viewsettings") = blogModule.ViewSettings
   If Post IsNot Nothing Then
    PropertySource("post") = Post
    PropertySource("author") = New LazyLoadingUser(PortalSettings.PortalId, Post.CreatedByUserID, Post.Username)
    PropertySource("blog") = Post.Blog
    PropertySource("owner") = New LazyLoadingUser(PortalSettings.PortalId, Post.Blog.OwnerUserId, Post.Blog.Username)
   ElseIf blogModule.BlogContext.Blog IsNot Nothing Then
    PropertySource("blog") = blogModule.BlogContext.Blog
    PropertySource("owner") = New LazyLoadingUser(PortalSettings.PortalId, blogModule.BlogContext.Blog.OwnerUserId, blogModule.BlogContext.Blog.Username)
   End If
   PropertySource("term") = term
   If blogModule.BlogContext.Term IsNot Nothing Then
    PropertySource("selectedterm") = blogModule.BlogContext.Term
   End If

  End Sub

  Public Sub New(blogModule As BlogModuleBase, Post As PostInfo, comment As CommentInfo)
   MyBase.new(Scope.DefaultSettings)

   PrimaryObject = comment
   ModuleInfo = blogModule.ModuleConfiguration
   UseObjectLessExpression = False
   PropertySource("query") = blogModule.BlogContext
   PropertySource("security") = blogModule.BlogContext.Security
   PropertySource("urls") = blogModule.BlogContext.ModuleUrls
   PropertySource("settings") = blogModule.Settings
   PropertySource("viewsettings") = blogModule.ViewSettings
   If Post IsNot Nothing Then
    PropertySource("post") = Post
    PropertySource("author") = New LazyLoadingUser(PortalSettings.PortalId, Post.CreatedByUserID, Post.Username)
    PropertySource("blog") = Post.Blog
    PropertySource("owner") = New LazyLoadingUser(PortalSettings.PortalId, Post.Blog.OwnerUserId, Post.Blog.Username)
   ElseIf blogModule.BlogContext.Blog IsNot Nothing Then
    PropertySource("blog") = blogModule.BlogContext.Blog
    PropertySource("owner") = New LazyLoadingUser(PortalSettings.PortalId, blogModule.BlogContext.Blog.OwnerUserId, blogModule.BlogContext.Blog.Username)
   End If
   PropertySource("comment") = comment
   PropertySource("commenter") = New LazyLoadingUser(PortalSettings.PortalId, comment.CreatedByUserID, comment.Username)

  End Sub

 End Class
End Namespace
