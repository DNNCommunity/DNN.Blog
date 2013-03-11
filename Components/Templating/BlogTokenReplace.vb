
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Services.Tokens
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Entities.Entries
Imports DotNetNuke.Modules.Blog.Entities.Terms

Namespace Templating
 Public Class BlogTokenReplace
  Inherits GenericTokenReplace

  Public Sub New(blogModule As BlogModuleBase, settings As Common.ModuleSettings)
   MyBase.new(Scope.DefaultSettings)

   Me.ModuleInfo = blogModule.ModuleConfiguration
   Me.UseObjectLessExpression = False
   Me.PropertySource("query") = blogModule
   Me.PropertySource("urls") = blogModule.ModuleUrls
   Me.PropertySource("settings") = settings
   If blogModule.Blog IsNot Nothing Then
    Me.PropertySource("blog") = blogModule.Blog
   End If
   If blogModule.Entry IsNot Nothing Then
    Me.PropertySource("entry") = blogModule.Entry
   End If
   If blogModule.Term IsNot Nothing Then
    Me.PropertySource("selectedterm") = blogModule.Term
   End If

  End Sub

  Public Sub New(blogModule As BlogModuleBase, settings As Common.ModuleSettings, blog As BlogInfo)
   MyBase.new(Scope.DefaultSettings)

   Me.ModuleInfo = blogModule.ModuleConfiguration
   Me.UseObjectLessExpression = False
   Me.PropertySource("query") = blogModule
   Me.PropertySource("urls") = blogModule.ModuleUrls
   Me.PropertySource("settings") = settings
   Me.PropertySource("blog") = blog
   If blogModule.Entry IsNot Nothing Then
    Me.PropertySource("entry") = blogModule.Entry
   End If
   If blogModule.Term IsNot Nothing Then
    Me.PropertySource("selectedterm") = blogModule.Term
   End If

  End Sub

  Public Sub New(blogModule As BlogModuleBase, settings As Common.ModuleSettings, entry As EntryInfo)
   MyBase.new(Scope.DefaultSettings)

   Me.ModuleInfo = blogModule.ModuleConfiguration
   Me.UseObjectLessExpression = False
   Me.PropertySource("query") = blogModule
   Me.PropertySource("urls") = blogModule.ModuleUrls
   Me.PropertySource("settings") = settings
   Me.PropertySource("entry") = entry
   Me.PropertySource("blog") = entry.Blog
   If blogModule.Term IsNot Nothing Then
    Me.PropertySource("selectedterm") = blogModule.Term
   End If

  End Sub

  Public Sub New(blogModule As BlogModuleBase, settings As Common.ModuleSettings, entry As EntryInfo, term As TermInfo)
   MyBase.new(Scope.DefaultSettings)

   Me.ModuleInfo = blogModule.ModuleConfiguration
   Me.UseObjectLessExpression = False
   Me.PropertySource("query") = blogModule
   Me.PropertySource("urls") = blogModule.ModuleUrls
   Me.PropertySource("settings") = settings
   If entry IsNot Nothing Then
    Me.PropertySource("entry") = entry
    Me.PropertySource("blog") = entry.Blog
   ElseIf blogModule.Blog IsNot Nothing Then
    Me.PropertySource("blog") = blogModule.Blog
   End If
   Me.PropertySource("term") = term
   If blogModule.Term IsNot Nothing Then
    Me.PropertySource("selectedterm") = blogModule.Term
   End If

  End Sub

 End Class
End Namespace
