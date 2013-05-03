Imports System.ComponentModel
Imports System.Linq
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports DotNetNuke.Modules.Blog.Entities.Terms
Imports DotNetNuke.Entities.Content.Taxonomy
Imports DotNetNuke.Web.Client.ClientResourceManagement

Namespace Controls
 Public Class CategorySelect
  Inherits WebControl

#Region " Private Properties "
  Private Property MainControlId As String = ""
  Private Property StorageControlId As String = ""
  Protected WithEvents Storage As HiddenField
#End Region

#Region " Public Properties "
  Public Property SelectedCategories As New List(Of TermInfo)
  Public Property ModuleConfiguration As DotNetNuke.Entities.Modules.ModuleInfo = Nothing
  Public Property VocabularyId As Integer = -1
  Private _Vocabulary As Dictionary(Of String, TermInfo)
  Public Property Vocabulary() As Dictionary(Of String, TermInfo)
   Get
    If _Vocabulary Is Nothing Then
     _Vocabulary = TermsController.GetTermsByVocabulary(ModuleConfiguration.ModuleID, VocabularyId, Threading.Thread.CurrentThread.CurrentCulture.Name)
     If _Vocabulary Is Nothing Then
      _Vocabulary = New Dictionary(Of String, TermInfo)
     End If
    End If
    Return _Vocabulary
   End Get
   Set(ByVal value As Dictionary(Of String, TermInfo))
    _Vocabulary = value
   End Set
  End Property
#End Region

#Region " Event Handlers "
  Private Sub TagEdit_Init(sender As Object, e As System.EventArgs) Handles Me.Init
   ClientResourceManager.RegisterScript(Page, ResolveUrl("~/DesktopModules/Blog/js/jquery.dynatree.min.js"))
   If Me.CssClass = "" Then Me.CssClass = "category-control"
   ClientResourceManager.RegisterStyleSheet(Page, ResolveUrl("~/DesktopModules/Blog/css/dynatree.css"), Web.Client.FileOrder.Css.ModuleCss)
   StorageControlId = Me.ClientID & "_Storage"
   Storage = New HiddenField With {.ID = StorageControlId}
  End Sub

  Private Sub TagEdit_Load(sender As Object, e As System.EventArgs) Handles Me.Load

   MainControlId = Me.ClientID & "_CategorySelect"
   If Me.Page.IsPostBack Then
    ' read return values
    Dim catList As String = ""
    Me.Page.Request.Params.ReadValue(Storage.ClientID, catList)
    catList = catList.Trim(","c)
    SelectedCategories = New List(Of TermInfo)
    If Not String.IsNullOrEmpty(catList) Then
     For Each c As String In catList.Split(","c)
      If IsNumeric(c) Then
       Dim cId As Integer = Integer.Parse(c)
       Dim cat As TermInfo = Vocabulary.Values.FirstOrDefault(Function(t)
                                                               Return CBool(t.TermId = cId)
                                                              End Function)
       If cat IsNot Nothing Then SelectedCategories.Add(cat)
      End If
     Next
    End If
   End If

  End Sub

  Protected Overrides Sub RenderContents(writer As HtmlTextWriter)

   writer.Write(String.Format("<div id=""{0}"" class=""category-container""></div>", MainControlId))
   Storage.RenderControl(writer)

  End Sub

  Private Sub CategorySelect_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender

   ' get the node tree
   Dim selectedIds As New List(Of Integer)
   For Each c As TermInfo In SelectedCategories
    selectedIds.Add(c.TermId)
   Next

   Dim pagescript As String = GetResource("DotNetNuke.Modules.Blog.CategorySelect.JS.CodeBlock.txt")
   pagescript = pagescript.Replace("[ID]", MainControlId)
   pagescript = pagescript.Replace("[Children]", TermsController.GetCategoryTreeAsJson(Vocabulary, selectedIds))
   pagescript = pagescript.Replace("[CatIdList]", String.Join(",", selectedIds))
   pagescript = pagescript.Replace("[StorageControlId]", StorageControlId)
   Me.Page.ClientScript.RegisterClientScriptBlock(GetType(String), ClientID, pagescript, True)

  End Sub
#End Region

 End Class
End Namespace
