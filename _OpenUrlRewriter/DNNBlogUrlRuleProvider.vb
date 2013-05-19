
Imports Satrabel.HttpModules.Provider
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Entities.Posts
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Portals


Public Class DNNBlogUrlRuleProvider
 Inherits UrlRuleProvider

 Public Overrides Function GetRules(PortalId As Integer) As List(Of UrlRule)

  Dim portal As PortalInfo = (New PortalController).GetPortal(PortalId)
  Dim rules As New List(Of UrlRule)
  Dim dicSecondaryLocales As List(Of Locale) = LocaleController.Instance.GetLocales(PortalId).Values.Where(Function(l) l.Code <> portal.DefaultLanguage).ToList
  Try
   For Each blog As BlogInfo In BlogsController.GetBlogsByPortal(PortalId, -1, portal.DefaultLanguage).Values.Where(Function(b) b.Published = True)
    rules.Add(New UrlRule With {.Action = UrlRuleAction.Rewrite, .Parameters = "Blog=" & blog.BlogID.ToString, .RuleType = UrlRuleType.Module, .Url = UrlRuleProvider.CleanupUrl(blog.Title)})
    For Each loc As Locale In dicSecondaryLocales
     If Not String.IsNullOrEmpty(blog.TitleLocalizations(loc.Code)) Then
      rules.Add(New UrlRule With {.Action = UrlRuleAction.Rewrite, .CultureCode = loc.Code, .Parameters = "blog=" & blog.BlogID.ToString, .RuleType = UrlRuleType.Module, .Url = UrlRuleProvider.CleanupUrl(blog.TitleLocalizations(loc.Code))})
     End If
    Next
    Dim page As Integer = 0
    Dim totalRecords As Integer = 1
    Do While page * 10 < totalRecords
     For Each post As PostInfo In PostsController.GetPostsByBlog(blog.ModuleID, blog.BlogID, portal.DefaultLanguage, -1, page, 20, "PUBLISHEDONDATE DESC", totalRecords).Values
      rules.Add(New UrlRule With {.Action = UrlRuleAction.Rewrite, .Parameters = "Post=" & post.ContentItemId.ToString, .RuleType = UrlRuleType.Module, .Url = UrlRuleProvider.CleanupUrl(post.Title)})
      For Each loc As Locale In dicSecondaryLocales
       If Not String.IsNullOrEmpty(post.TitleLocalizations(loc.Code)) Then
        rules.Add(New UrlRule With {.Action = UrlRuleAction.Rewrite, .CultureCode = loc.Code, .Parameters = "post=" & post.ContentItemId.ToString, .RuleType = UrlRuleType.Module, .Url = UrlRuleProvider.CleanupUrl(post.TitleLocalizations(loc.Code))})
       End If
      Next
     Next
     page += 1
    Loop
    'For Each u As PostAuthor In PostsController.GetAuthors(blog.ModuleID, blog.BlogID).OrderBy(Function(t) t.Username)
    ' rules.Add(New UrlRule With {.Action = UrlRuleAction.Rewrite, .Parameters = "author=" & u.UserID.ToString, .RuleType = UrlRuleType.Module, .Url = UrlRuleProvider.CleanupUrl(u.DisplayName)})
    'Next
   Next
  Catch ex As Exception
  End Try

  Return rules

 End Function

End Class
