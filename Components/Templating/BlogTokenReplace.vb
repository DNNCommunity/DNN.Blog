
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Services.Tokens
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Entities.Posts
Imports DotNetNuke.Modules.Blog.Entities.Terms
Imports DotNetNuke.Modules.Blog.Entities.Comments

Namespace Templating
 Public Class BlogTokenReplace
  Inherits GenericTokenReplace

  Public Sub New(actModule As DotNetNuke.Entities.Modules.ModuleInfo, security As Modules.Blog.Security.ContextSecurity, blog As BlogInfo, post As PostInfo, settings As Common.ModuleSettings)
   MyBase.new(Scope.DefaultSettings)

   Me.ModuleInfo = actModule
   Me.UseObjectLessExpression = False
   Me.PropertySource("security") = security
   Me.PropertySource("settings") = settings
   If blog IsNot Nothing Then
    Me.PropertySource("blog") = blog
    Me.PropertySource("owner") = New LazyLoadingUser(PortalSettings.PortalId, blog.Username)
   End If
   If post IsNot Nothing Then
    Me.PropertySource("post") = post
   End If

  End Sub

  Public Sub New(actModule As DotNetNuke.Entities.Modules.ModuleInfo, security As Modules.Blog.Security.ContextSecurity, blog As BlogInfo, Post As PostInfo, settings As Common.ModuleSettings, comment As CommentInfo)
   MyBase.new(Scope.DefaultSettings)

   Me.PrimaryObject = comment
   Me.ModuleInfo = actModule
   Me.UseObjectLessExpression = False
   Me.PropertySource("security") = security
   Me.PropertySource("settings") = settings
   If Post IsNot Nothing Then
    Me.PropertySource("post") = Post
    Me.PropertySource("author") = New LazyLoadingUser(PortalSettings.PortalId, Post.Username)
    Me.PropertySource("blog") = Post.Blog
    Me.PropertySource("owner") = New LazyLoadingUser(PortalSettings.PortalId, Post.Blog.Username)
   ElseIf blog IsNot Nothing Then
    Me.PropertySource("blog") = blog
    Me.PropertySource("owner") = New LazyLoadingUser(PortalSettings.PortalId, blog.Username)
   End If
   Me.PropertySource("comment") = comment
   Me.PropertySource("commenter") = New LazyLoadingUser(PortalSettings.PortalId, comment.Username)

  End Sub

  Public Sub New(blogModule As BlogContextBase, settings As Common.ModuleSettings)
   MyBase.new(Scope.DefaultSettings)

   Me.ModuleInfo = blogModule.ModuleConfiguration
   Me.UseObjectLessExpression = False
   Me.PropertySource("query") = blogModule
   Me.PropertySource("security") = blogModule.Security
   Me.PropertySource("urls") = blogModule.ModuleUrls
   Me.PropertySource("settings") = settings
   If blogModule.Blog IsNot Nothing Then
    Me.PropertySource("blog") = blogModule.Blog
    Me.PropertySource("owner") = New LazyLoadingUser(PortalSettings.PortalId, blogModule.Blog.Username)
   End If
   If blogModule.Post IsNot Nothing Then
    Me.PropertySource("post") = blogModule.Post
   End If
   If blogModule.Term IsNot Nothing Then
    Me.PropertySource("selectedterm") = blogModule.Term
   End If
   If blogModule.Author IsNot Nothing Then
    Me.PropertySource("author") = blogModule.Author
   End If

  End Sub

  Public Sub New(blogModule As BlogContextBase, settings As Common.ModuleSettings, blog As BlogInfo)
   MyBase.new(Scope.DefaultSettings)

   Me.PrimaryObject = blog
   Me.ModuleInfo = blogModule.ModuleConfiguration
   Me.UseObjectLessExpression = False
   Me.PropertySource("query") = blogModule
   Me.PropertySource("security") = blogModule.Security
   Me.PropertySource("urls") = blogModule.ModuleUrls
   Me.PropertySource("settings") = settings
   Me.PropertySource("blog") = blog
   Me.PropertySource("owner") = New LazyLoadingUser(PortalSettings.PortalId, blog.Username)
   If blogModule.Post IsNot Nothing Then
    Me.PropertySource("post") = blogModule.Post
    Me.PropertySource("author") = New LazyLoadingUser(PortalSettings.PortalId, blogModule.Post.Username)
   ElseIf blogModule.Author IsNot Nothing Then
    Me.PropertySource("author") = blogModule.Author
   End If
   If blogModule.Term IsNot Nothing Then
    Me.PropertySource("selectedterm") = blogModule.Term
   End If

  End Sub

  Public Sub New(blogModule As BlogContextBase, settings As Common.ModuleSettings, Post As PostInfo)
   MyBase.new(Scope.DefaultSettings)

   Me.PrimaryObject = Post
   Me.ModuleInfo = blogModule.ModuleConfiguration
   Me.UseObjectLessExpression = False
   Me.PropertySource("query") = blogModule
   Me.PropertySource("security") = blogModule.Security
   Me.PropertySource("urls") = blogModule.ModuleUrls
   Me.PropertySource("settings") = settings
   Me.PropertySource("post") = Post
   Me.PropertySource("author") = New LazyLoadingUser(PortalSettings.PortalId, Post.Username)
   Me.PropertySource("blog") = Post.Blog
   Me.PropertySource("owner") = New LazyLoadingUser(PortalSettings.PortalId, Post.Blog.Username)
   If blogModule.Term IsNot Nothing Then
    Me.PropertySource("selectedterm") = blogModule.Term
   End If

  End Sub

  Public Sub New(blogModule As BlogContextBase, settings As Common.ModuleSettings, Post As PostInfo, term As TermInfo)
   MyBase.new(Scope.DefaultSettings)

   Me.PrimaryObject = term
   Me.ModuleInfo = blogModule.ModuleConfiguration
   Me.UseObjectLessExpression = False
   Me.PropertySource("query") = blogModule
   Me.PropertySource("security") = blogModule.Security
   Me.PropertySource("urls") = blogModule.ModuleUrls
   Me.PropertySource("settings") = settings
   If Post IsNot Nothing Then
    Me.PropertySource("post") = Post
    Me.PropertySource("author") = New LazyLoadingUser(PortalSettings.PortalId, Post.Username)
    Me.PropertySource("blog") = Post.Blog
    Me.PropertySource("owner") = New LazyLoadingUser(PortalSettings.PortalId, Post.Blog.Username)
   ElseIf blogModule.Blog IsNot Nothing Then
    Me.PropertySource("blog") = blogModule.Blog
    Me.PropertySource("owner") = New LazyLoadingUser(PortalSettings.PortalId, blogModule.Blog.Username)
   End If
   Me.PropertySource("term") = term
   If blogModule.Term IsNot Nothing Then
    Me.PropertySource("selectedterm") = blogModule.Term
   End If

  End Sub

  Public Sub New(blogModule As BlogContextBase, settings As Common.ModuleSettings, Post As PostInfo, comment As CommentInfo)
   MyBase.new(Scope.DefaultSettings)

   Me.PrimaryObject = comment
   Me.ModuleInfo = blogModule.ModuleConfiguration
   Me.UseObjectLessExpression = False
   Me.PropertySource("query") = blogModule
   Me.PropertySource("security") = blogModule.Security
   Me.PropertySource("urls") = blogModule.ModuleUrls
   Me.PropertySource("settings") = settings
   If Post IsNot Nothing Then
    Me.PropertySource("post") = Post
    Me.PropertySource("author") = New LazyLoadingUser(PortalSettings.PortalId, Post.Username)
    Me.PropertySource("blog") = Post.Blog
    Me.PropertySource("owner") = New LazyLoadingUser(PortalSettings.PortalId, Post.Blog.Username)
   ElseIf blogModule.Blog IsNot Nothing Then
    Me.PropertySource("blog") = blogModule.Blog
    Me.PropertySource("owner") = New LazyLoadingUser(PortalSettings.PortalId, blogModule.Blog.Username)
   End If
   Me.PropertySource("comment") = comment
   Me.PropertySource("commenter") = New LazyLoadingUser(PortalSettings.PortalId, comment.Username)

  End Sub

 End Class
End Namespace
