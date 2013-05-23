Imports Satrabel.HttpModules.Provider
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Entities.Posts
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Modules.Blog.Entities.Terms


Public Class DNNBlogUrlRuleProvider
 Inherits UrlRuleProvider

 Public Overrides Function GetRules(PortalId As Integer) As List(Of UrlRule)

  Dim portal As PortalInfo = (New PortalController).GetPortal(PortalId)
  Dim rules As New List(Of UrlRule)
  Dim dicSecondaryLocales As List(Of Locale) = LocaleController.Instance.GetLocales(PortalId).Values.Where(Function(l) l.Code <> portal.DefaultLanguage).ToList
  Dim blogModules As New Dictionary(Of Integer, Integer) ' module id to tab id
  Try
   For Each blog As BlogInfo In BlogsController.GetBlogsByPortal(PortalId, -1, portal.DefaultLanguage).Values.Where(Function(b) b.Published = True)
    If Not blogModules.ContainsKey(blog.ModuleID) Then
     Dim tabMods As ArrayList = (New ModuleController).GetAllTabsModulesByModuleID(blog.ModuleID)
     If tabMods.Count > 0 Then
      blogModules.Add(blog.ModuleID, CType(tabMods(0), ModuleInfo).TabID)
     End If
    End If
    Dim removeTab As Boolean = CBool(blogModules(blog.ModuleID) = portal.HomeTabId)
    rules.Add(New UrlRule With {.Action = UrlRuleAction.Rewrite, .Parameters = "blog=" & blog.BlogID.ToString, .RuleType = UrlRuleType.Module, .Url = UrlRuleProvider.CleanupUrl(blog.Title), .TabId = blogModules(blog.ModuleID), .RemoveTab = removeTab})
    For Each loc As Locale In dicSecondaryLocales
     If Not String.IsNullOrEmpty(blog.TitleLocalizations(loc.Code)) Then
      rules.Add(New UrlRule With {.Action = UrlRuleAction.Rewrite, .CultureCode = loc.Code, .Parameters = "blog=" & blog.BlogID.ToString, .RuleType = UrlRuleType.Module, .Url = UrlRuleProvider.CleanupUrl(blog.TitleLocalizations(loc.Code)), .TabId = blogModules(blog.ModuleID), .RemoveTab = removeTab})
     End If
    Next
    Dim page As Integer = 0
    Dim totalRecords As Integer = 1
    Do While page * 10 < totalRecords
     For Each post As PostInfo In PostsController.GetPostsByBlog(blog.ModuleID, blog.BlogID, portal.DefaultLanguage, -1, page, 20, "PUBLISHEDONDATE DESC", totalRecords).Values
      rules.Add(New UrlRule With {.Action = UrlRuleAction.Rewrite, .Parameters = "post=" & post.ContentItemId.ToString, .RuleType = UrlRuleType.Module, .Url = UrlRuleProvider.CleanupUrl(post.Title)})
      For Each loc As Locale In dicSecondaryLocales
       If Not String.IsNullOrEmpty(post.TitleLocalizations(loc.Code)) Then
        rules.Add(New UrlRule With {.Action = UrlRuleAction.Rewrite, .CultureCode = loc.Code, .Parameters = "post=" & post.ContentItemId.ToString, .RuleType = UrlRuleType.Module, .Url = UrlRuleProvider.CleanupUrl(post.TitleLocalizations(loc.Code)), .TabId = blogModules(blog.ModuleID), .RemoveTab = removeTab})
       End If
      Next
     Next
     page += 1
    Loop
   Next
   For Each blogM As Integer In blogModules.Keys
    Dim removeTab As Boolean = CBool(blogModules(blogM) = portal.HomeTabId)
    For Each u As PostAuthor In PostsController.GetAuthors(blogM, -1).OrderBy(Function(t) t.Username)
     rules.Add(New UrlRule With {.Action = UrlRuleAction.Rewrite, .Parameters = "author=" & u.UserID.ToString, .RuleType = UrlRuleType.Module, .Url = UrlRuleProvider.CleanupUrl(u.DisplayName), .TabId = blogModules(blogM), .RemoveTab = removeTab})
    Next
    For Each t As TermInfo In TermsController.GetTermsByModule(blogM, portal.DefaultLanguage)
     rules.Add(New UrlRule With {.Action = UrlRuleAction.Rewrite, .Parameters = "term=" & t.TermId.ToString, .RuleType = UrlRuleType.Module, .Url = UrlRuleProvider.CleanupUrl(t.Name), .TabId = blogModules(blogM), .RemoveTab = removeTab})
     For Each loc As Locale In dicSecondaryLocales
      If Not String.IsNullOrEmpty(t.NameLocalizations(loc.Code)) Then
       rules.Add(New UrlRule With {.Action = UrlRuleAction.Rewrite, .CultureCode = loc.Code, .Parameters = "term=" & t.TermId.ToString, .RuleType = UrlRuleType.Module, .Url = UrlRuleProvider.CleanupUrl(t.NameLocalizations(loc.Code)), .TabId = blogModules(blogM), .RemoveTab = removeTab})
      End If
     Next
    Next
   Next
  Catch ex As Exception
  End Try

  Return rules

 End Function

End Class
