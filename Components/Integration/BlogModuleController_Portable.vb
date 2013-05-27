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

  Private Const LogFilePattern As String = "{0}BlogImport_{1}.resources"

#Region " Post Import Logic "
  Public Shared Sub CheckupOnImportedFiles(moduleId As Integer)
   Dim CacheKey As String = "CheckupOnImportedFiles" & moduleId.ToString
   If DotNetNuke.Common.Utilities.DataCache.GetCache(CacheKey) Is Nothing Then
    Dim logFile As String = String.Format(LogFilePattern, DotNetNuke.Common.HostMapPath, moduleId)
    If IO.File.Exists(logFile) Then
     Using sr As New IO.StreamReader(logFile)
      Dim line As String
      Dim currentBlog As Integer = -1
      Do
       line = sr.ReadLine()
       If Not String.IsNullOrEmpty(line) Then
        Dim t As String = line.Substring(0, 1)
        Dim oldId As Integer = Integer.Parse(line.Substring(1, line.IndexOf("-") - 1))
        Dim newId As Integer = Integer.Parse(line.Substring(line.IndexOf("-") + 1))
        If t = "M" Then
         Data.DataProvider.Instance.UpdateModuleWiring(DotNetNuke.Entities.Portals.PortalSettings.Current.PortalId, oldId, newId)
        ElseIf t = "B" Then
         Dim d As New IO.DirectoryInfo(String.Format("{0}Blog\Files\{1}\", DotNetNuke.Entities.Portals.PortalSettings.Current.HomeDirectoryMapPath, oldId))
         If d.Exists Then
          d.MoveTo(String.Format("{0}Blog\Files\{1}\", DotNetNuke.Entities.Portals.PortalSettings.Current.HomeDirectoryMapPath, newId))
         End If
         currentBlog = newId
        ElseIf t = "P" Then
         Dim d As New IO.DirectoryInfo(String.Format("{0}Blog\Files\{1}\{2}\", DotNetNuke.Entities.Portals.PortalSettings.Current.HomeDirectoryMapPath, currentBlog, oldId))
         If d.Exists Then
          d.MoveTo(String.Format("{0}Blog\Files\{1}\{2}\", DotNetNuke.Entities.Portals.PortalSettings.Current.HomeDirectoryMapPath, currentBlog, newId))
         End If
         If d.GetFiles("*.*").Count = 0 Then
         Else
          Dim post As PostInfo = PostsController.GetPost(newId, moduleId, "")
          If post IsNot Nothing Then
           Dim postPath As String = GetPostDirectoryPath(post)
           For Each f As IO.FileInfo In d.GetFiles("*.*")
            Dim filename As String = f.Name
            Dim reg As String = "\&quot;([^\&]*)" & filename & "\&quot;"
            Dim repl As String = "&quot;" & postPath & filename & "&quot;"
            post.Content = Regex.Replace(post.Content, reg, repl)
            For Each l As String In post.ContentLocalizations.Locales
             post.ContentLocalizations(l) = Regex.Replace(post.ContentLocalizations(l), reg, repl)
            Next
            If Not String.IsNullOrEmpty(post.Summary) Then post.Summary = Regex.Replace(post.Summary, reg, repl)
            For Each l As String In post.SummaryLocalizations.Locales
             post.SummaryLocalizations(l) = Regex.Replace(post.SummaryLocalizations(l), reg, repl)
            Next
           Next
           PostsController.UpdatePost(post, -1)
          End If
         End If
        End If
       End If
      Loop Until line Is Nothing
      sr.ReadLine()
     End Using
     IO.File.Delete(logFile)
    End If
    DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey, True)
   End If
  End Sub
#End Region

#Region " IPortable Methods "
  Public Function ExportModule(ByVal ModuleID As Integer) As String Implements IPortable.ExportModule

   Dim strXml As New StringBuilder
   Using sw As New IO.StringWriter(strXml)
    Using xml As New XmlTextWriter(sw)
     xml.WriteStartElement("dnnblog")
     xml.WriteElementString("ModuleId", ModuleID.ToString)
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
    Dim importReport As New StringBuilder
    Dim oldModuleId As Integer = -1
    xContent.ReadValue("ModuleId", oldModuleId)
    importReport.AppendFormat("M{0}-{1}" & vbCrLf, oldModuleId, ModuleID)

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
       importReport.AppendFormat("B{0}-{1}" & vbCrLf, blog.ImportedBlogId, blog.BlogID)
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
        importReport.AppendFormat("P{0}-{1}" & vbCrLf, p.ImportedPostId, p.ContentItemId)
       Next
      Next
     End If
    End If

    Dim importLogFile As String = String.Format(LogFilePattern, DotNetNuke.Common.HostMapPath, ModuleID)
    Common.Globals.WriteToFile(importLogFile, importReport.ToString)

   Catch ex As Exception
    Exceptions.LogException(ex)
   End Try
  End Sub
#End Region

#Region " Private Methods "
#End Region

 End Class
End Namespace