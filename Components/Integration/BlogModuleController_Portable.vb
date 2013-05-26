Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services
Imports System.Linq
Imports System.Xml
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Entities.Posts
Imports DotNetNuke.Modules.Blog.Entities.Terms

Namespace Integration
 Partial Public Class BlogModuleController
  Implements IPortable

#Region " IPortable Methods "

  Public Function ExportModule(ByVal ModuleID As Integer) As String Implements IPortable.ExportModule

   Dim strXml As New StringBuilder
   Using sw As New IO.StringWriter(strXml)
    Using xml As New XmlTextWriter(sw)
     xml.WriteStartElement("dnnblog")
     Dim tabMods As ArrayList = (New ModuleController).GetAllTabsModulesByModuleID(ModuleID)
     If tabMods.Count > 0 Then
      Dim vs As ViewSettings = ViewSettings.GetViewSettings(CType(tabMods(0), ModuleInfo).TabModuleID)
      vs.Serialize(xml)
      If vs.BlogModuleId = -1 Then
       Dim ms As ModuleSettings = ModuleSettings.GetModuleSettings(ModuleID)
       ms.Serialize(xml)
       If ms.VocabularyId > -1 Then
        TermsController.WriteVocabulary(xml, "Categories", TermsController.GetTermsByVocabulary(ModuleID, ms.VocabularyId).Values.ToList())
       End If
       TermsController.WriteVocabulary(xml, "Tags", TermsController.GetTermsByModule(ModuleID, "").Where(Function(t) CBool(t.VocabularyId = 1)).ToList())
      End If
     End If
     For Each b As BlogInfo In BlogsController.GetBlogsByModule(ModuleID, "").Values
      b.WriteXml(xml)
     Next
     xml.WriteEndElement() ' dnnblog
    End Using
   End Using
   Return strXml.ToString

  End Function

  Public Sub ImportModule(ByVal ModuleID As Integer, ByVal Content As String, ByVal Version As String, ByVal UserID As Integer) Implements DotNetNuke.Entities.Modules.IPortable.ImportModule
   Try

    Dim xContent As XmlNode = DotNetNuke.Common.GetContent(Content, "dnnblog")

    Dim tabMods As ArrayList = (New ModuleController).GetAllTabsModulesByModuleID(ModuleID)
    If tabMods.Count > 0 Then
     Dim vs As ViewSettings = ViewSettings.GetViewSettings(CType(tabMods(0), ModuleInfo).TabModuleID)
     vs.FromXml(xContent.SelectSingleNode("ViewSettings"))
     vs.UpdateSettings()
     If vs.BlogModuleId = -1 Then
      Dim settings As ModuleSettings = ModuleSettings.GetModuleSettings(ModuleID)
      settings.FromXml(xContent.SelectSingleNode("Settings"))
      Dim vocabulary As List(Of TermInfo) = TermsController.FromXml(xContent.SelectSingleNode("Categories"))
      Dim categories As New Dictionary(Of String, TermInfo)
      If vocabulary.Count > 0 Then
       settings.VocabularyId = Integration.CreateNewVocabulary(CType(tabMods(0), ModuleInfo).PortalID).VocabularyId
       TermsController.AddVocabulary(settings.VocabularyId, vocabulary)
       categories = TermsController.GetTermsByVocabulary(ModuleID, settings.VocabularyId, "", True)
      End If
      settings.UpdateSettings()
      vocabulary = TermsController.FromXml(xContent.SelectSingleNode("Tags"))
      If vocabulary.Count > 0 Then
       TermsController.AddTags(ModuleID, vocabulary)
      End If
      Dim tags As Dictionary(Of String, TermInfo) = TermsController.GetTermsByVocabulary(ModuleID, 1, "")
      For Each xBlog As XmlNode In xContent.SelectNodes("Blog")
       Dim blog As New BlogInfo
       blog.FromXml(xBlog)
       blog.ModuleID = ModuleID
       blog.OwnerUserId = UserID
       blog.BlogID = BlogsController.AddBlog(blog, UserID)
       If blog.ImportedFiles.Count > 0 Then
        Dim blogDir As String = GetBlogDirectoryMapPath(blog.BlogID)
        IO.Directory.CreateDirectory(blogDir)
        For Each att As BlogML.Xml.BlogMLAttachment In blog.ImportedFiles
         If att.Embedded And att.Data IsNot Nothing Then
          Dim filename As String = att.Path
          If filename = "" Then filename = att.Url
          filename = filename.Replace("/", "\")
          If filename.IndexOf("\") > 0 Then filename = filename.Substring(filename.LastIndexOf("\") + 1)
          IO.File.WriteAllBytes(blogDir & filename, att.Data)
         End If
        Next
       End If
       For Each p As PostInfo In blog.ImportedPosts
        p.BlogID = blog.BlogID
        For Each tagName As String In p.ImportedTags
         If tags.ContainsKey(tagName) Then
          p.Terms.Add(tags(tagName))
         End If
        Next
        For Each catName As String In p.ImportedCategories
         If categories.ContainsKey(catName) Then
          p.Terms.Add(categories(catName))
         End If
        Next
        PostsController.AddPost(p, UserID)
        If p.ImportedFiles.Count > 0 Then
         Dim postDir As String = GetPostDirectoryMapPath(p)
         Dim postPath As String = GetPostDirectoryPath(p)
         IO.Directory.CreateDirectory(postDir)
         For Each att As BlogML.Xml.BlogMLAttachment In p.ImportedFiles
          If att.Embedded And att.Data IsNot Nothing Then
           Dim filename As String = att.Path
           If filename = "" Then filename = att.Url
           filename = filename.Replace("/", "\")
           If filename.IndexOf("\") > 0 Then filename = filename.Substring(filename.LastIndexOf("\") + 1)
           IO.File.WriteAllBytes(postDir & filename, att.Data)
           p.Content = p.Content.Replace(filename, postPath & filename)
           For Each l As String In p.ContentLocalizations.Locales
            p.ContentLocalizations(l) = p.ContentLocalizations(l).Replace(filename, postPath & filename)
           Next
           If Not String.IsNullOrEmpty(p.Summary) Then p.Summary = p.Summary.Replace(filename, postPath & filename)
           For Each l As String In p.SummaryLocalizations.Locales
            p.SummaryLocalizations(l) = p.SummaryLocalizations(l).Replace(filename, postPath & filename)
           Next
          End If
         Next
         PostsController.UpdatePost(p, UserID)
        End If
       Next
      Next
     End If
    End If

   Catch ex As Exception
    Exceptions.LogException(ex)
   End Try
  End Sub
#End Region

#Region " Private Methods "
#End Region

 End Class
End Namespace