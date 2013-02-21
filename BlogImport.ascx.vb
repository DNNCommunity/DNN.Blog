Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Modules.Blog.BlogML.Xml
Imports DotNetNuke.Modules.Blog.Entities.Entries

Public Class BlogImport
 Inherits BlogModuleBase

 Private Property CanImportCategories As Boolean = False

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

  If Settings.VocabularyId > -1 AndAlso (Security.IsEditor Or Settings.BloggersCanEditCategories) Then CanImportCategories = True

  If Not Me.IsPostBack Then
   lblTargetName.Text = Blog.Title
   chkImportCategories.Enabled = CanImportCategories
  End If

 End Sub

 Protected Function GetText(type As String) As String
  Dim text As String = Null.NullString
  Dim pageName As String = wizBlogImport.ActiveStep.Title
  If type = "Title" Then
   text = Localization.GetString(pageName + ".Title", LocalResourceFile)
  ElseIf type = "Help" Then
   text = Localization.GetString(pageName + ".Help", LocalResourceFile)
  End If
  Return text
 End Function

 Private Sub wizBlogImport_ActiveStepChanged(sender As Object, e As System.EventArgs) Handles wizBlogImport.ActiveStepChanged

 End Sub

 Private Sub wizBlogImport_CancelButtonClick(sender As Object, e As System.EventArgs) Handles wizBlogImport.CancelButtonClick
  Response.Redirect(EditUrl("Manage"), False)
 End Sub

 Private Sub wizBlogImport_NextButtonClick(sender As Object, e As System.Web.UI.WebControls.WizardNavigationEventArgs) Handles wizBlogImport.NextButtonClick
  Select Case e.CurrentStepIndex
   Case 0 ' upload
    Dim strReport As New StringBuilder
    Dim file As HttpPostedFile = cmdBrowse.PostedFile
    If file.FileName <> "" Then
     file.SaveAs(BlogMapPath & "import.resources")
     strReport.AppendLine("Saved File")
     Dim blog As BlogMLBlog = Nothing
     Using strIn As New IO.StreamReader(BlogMapPath & "import.resources")
      Using xmlIn As New System.Xml.XmlTextReader(strIn)
       blog = BlogMLSerializer.Deserialize(xmlIn)
      End Using
     End Using
     If blog IsNot Nothing Then
      strReport.AppendLine("File is a valid BlogML file")
      strReport.AppendFormat("Original Title: {0}" & vbCrLf, blog.Title)
      strReport.AppendFormat("Total Posts: {0}" & vbCrLf, blog.Posts.Count)
      strReport.AppendFormat("Total Categories: {0}" & vbCrLf, blog.Categories.Count)
     End If
     txtAnalysis.Text = strReport.ToString
    Else ' no file uploaded

    End If
   Case 1 ' analysis and selecting options
    Dim strReport As New StringBuilder
    Dim b As BlogMLBlog = Nothing
    Using strIn As New IO.StreamReader(BlogMapPath & "import.resources")
     Using xmlIn As New System.Xml.XmlTextReader(strIn)
      b = BlogMLSerializer.Deserialize(xmlIn)
     End Using
    End Using
    If b IsNot Nothing Then
     If CanImportCategories AndAlso chkImportCategories.Checked Then
      For Each c As BlogML.Xml.BlogMLCategory In b.Categories
       ' check for and add category
      Next
     ElseIf chkImportMissingCategoriesAsKeywords.Checked Then
      For Each c As BlogML.Xml.BlogMLCategory In b.Categories
       ' check for and add tag
      Next
     End If
     For Each post As BlogML.Xml.BlogMLPost In b.Posts
      ' import post
      Dim entry As New EntryInfo
      With entry
       .BlogID = BlogId
       .AllowComments = Blog.AllowComments
       .Title = post.Title
       .Content = post.Content.Text
       .Summary = post.Excerpt.Text
       .Published = post.Approved
       .PublishedOnDate = post.DateCreated
      End With
      If entry.Title <> "" And entry.Content <> "" Then
       entry.ContentItemId = EntriesController.AddEntry(entry, UserId)
       strReport.AppendFormat("Added {0}" & vbCrLf, entry.Title)
      End If
     Next
     txtReport.Text = strReport.ToString
    End If

   Case 2 ' report
    Response.Redirect(EditUrl("Manage"), False)
  End Select
 End Sub
End Class