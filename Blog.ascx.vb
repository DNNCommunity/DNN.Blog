Imports System.Linq
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Modules.Blog.Templating
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Entities.Entries
Imports DotNetNuke.Modules.Blog.Entities.Terms

Public Class Blog
 Inherits BlogModuleBase
 Implements IActionable

#Region " Private Members "
 Private _urlParameters As New List(Of String)
 Private _pageSize As Integer = -1
 Private _totalRecords As Integer = 0
 Private _reqPage As Integer = 0
 Private _usePaging As Boolean = False
 Private _endDate As Date = Date.Now
 Private _search As String = ""
#End Region

#Region " Event Handlers "
 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

  DotNetNuke.Framework.jQuery.RequestRegistration()
  DotNetNuke.Framework.jQuery.RequestUIRegistration()
  Me.Request.Params.ReadValue("Page", _reqPage)
  Me.Request.Params.ReadValue("EndDate", _endDate)
  Me.Request.Params.ReadValue("search", _search)
  DataBind()

 End Sub
#End Region

#Region " Template Data Retrieval "
 Private Sub vtContents_GetData(ByVal DataSource As String, ByVal Parameters As Dictionary(Of String, String), ByRef Replacers As System.Collections.Generic.List(Of GenericTokenReplace), ByRef Arguments As System.Collections.Generic.List(Of String())) Handles vtContents.GetData

  Select Case DataSource.ToLower

   Case "blogs"

    Dim blogList As IEnumerable(Of BlogInfo) = BlogsController.GetBlogsByModule(Settings.ModuleId, UserId).Values.Where(Function(b)
                                                                                                                         Return b.Published = True
                                                                                                                        End Function).OrderBy(Function(b) b.Title)
    Parameters.ReadValue("pagesize", _pageSize)
    If _pageSize > 0 Then
     _usePaging = True
     Dim startRec As Integer = ((_reqPage - 1) * _pageSize) + 1
     Dim endRec As Integer = _reqPage * _pageSize
     Dim i As Integer = 1
     For Each b As BlogInfo In blogList
      If i >= startRec And i <= endRec Then
       Replacers.Add(New BlogTokenReplace(Me, Settings, b))
      End If
      i += 1
     Next
    Else
     For Each b As BlogInfo In blogList
      Replacers.Add(New BlogTokenReplace(Me, Settings, b))
     Next
    End If

   Case "entries"

    Parameters.ReadValue("pagesize", _pageSize)
    If _pageSize < 1 Then _pageSize = 10 ' we will not list "all entries"
    Dim entryList As IEnumerable(Of EntryInfo)
    If Not String.IsNullOrEmpty(_search) Then
     Dim searchTitle As Boolean = False
     Dim searchContents As Boolean = False
     Request.Params.ReadValue("t", searchTitle)
     Request.Params.ReadValue("c", searchContents)
     If Term Is Nothing Then
      entryList = EntriesController.SearchEntries(Settings.ModuleId, BlogId, _search, searchTitle, searchContents, 1, _endDate, -1, _reqPage, _pageSize, "PUBLISHEDONDATE DESC", _totalRecords, UserId).Values
     Else
      entryList = EntriesController.SearchEntriesByTerm(Settings.ModuleId, BlogId, TermId, _search, searchTitle, searchContents, 1, _endDate, -1, _reqPage, _pageSize, "PUBLISHEDONDATE DESC", _totalRecords, UserId).Values
     End If
    ElseIf Term Is Nothing Then
     entryList = EntriesController.GetEntries(Settings.ModuleId, BlogId, 1, _endDate, -1, _reqPage, _pageSize, "PUBLISHEDONDATE DESC", _totalRecords, UserId).Values
    Else
     entryList = EntriesController.GetEntriesByTerm(Settings.ModuleId, BlogId, TermId, 1, _endDate, -1, _reqPage, _pageSize, "PUBLISHEDONDATE DESC", _totalRecords, UserId).Values
    End If
    _usePaging = True
    For Each e As EntryInfo In entryList
     Replacers.Add(New BlogTokenReplace(Me, Settings, e))
    Next

   Case "terms"

    If Entry IsNot Nothing Then
     For Each t As TermInfo In Entry.Terms
      Replacers.Add(New BlogTokenReplace(Me, Settings, Entry, t))
     Next
    Else
     For Each t As TermInfo In TermsController.GetTermsByModule(Settings.ModuleId)
      Replacers.Add(New BlogTokenReplace(Me, Settings, Nothing, t))
     Next
    End If
    _usePaging = False

   Case "keywords", "tags"

    If Entry IsNot Nothing Then
     For Each t As TermInfo In Entry.EntryTags
      Replacers.Add(New BlogTokenReplace(Me, Settings, Entry, t))
     Next
    Else
     For Each t As TermInfo In TermsController.GetTermsByModule(Settings.ModuleId).Where(Function(x) x.VocabularyId = 1).ToList
      Replacers.Add(New BlogTokenReplace(Me, Settings, Nothing, t))
     Next
    End If
    _usePaging = False

   Case "categories"

    If Entry IsNot Nothing Then
     For Each t As TermInfo In Entry.EntryCategories
      Replacers.Add(New BlogTokenReplace(Me, Settings, Entry, t))
     Next
    Else
     For Each t As TermInfo In TermsController.GetTermsByModule(Settings.ModuleId).Where(Function(x) x.VocabularyId <> 1).ToList
      Replacers.Add(New BlogTokenReplace(Me, Settings, Nothing, t))
     Next
    End If
    _usePaging = False

  End Select

 End Sub
#End Region

#Region " Overrides "
 Public Overrides Sub DataBind()

  Dim tmgr As New TemplateManager(PortalSettings, ViewSettings.Template)
  With vtContents
   .TemplatePath = tmgr.TemplatePath
   .TemplateMapPath = tmgr.TemplateMapPath
   .DefaultReplacer = New BlogTokenReplace(Me, Settings)
  End With
  vtContents.DataBind()

  ctlComments.ClonePropertiesFrom(Me)
  If Blog IsNot Nothing Then
   ctlComments.AllowAnonymousComments = Blog.AllowAnonymousComments
  End If

  ctlManagement.Visible = ViewSettings.ShowManagementPanel
  ctlManagement.ClonePropertiesFrom(Me)

 End Sub
#End Region

#Region " IActionable "
 Public ReadOnly Property ModuleActions As Actions.ModuleActionCollection Implements IActionable.ModuleActions
  Get
   Dim MyActions As New Actions.ModuleActionCollection
   If IsEditable Or Security.IsBlogger Then
    MyActions.Add(GetNextActionID, Localization.GetString(ModuleActionType.EditContent, LocalResourceFile), ModuleActionType.EditContent, "", "", EditUrl("Manage"), False, DotNetNuke.Security.SecurityAccessLevel.View, True, False)
   End If
   Return MyActions
  End Get
 End Property
#End Region

End Class