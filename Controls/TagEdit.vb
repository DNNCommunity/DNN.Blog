Imports System.ComponentModel
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports DotNetNuke.Modules.Blog.Entities.Terms
Imports DotNetNuke.Entities.Content.Taxonomy
Imports DotNetNuke.Web.Client.ClientResourceManagement

Namespace Controls
 Public Class TagEdit
  Inherits WebControl

#Region " Private Properties "
  Private Property ListClientID As String = ""
#End Region

#Region " Public Properties "
  Public Property Terms As List(Of TermInfo)
  Public Property ModuleConfiguration As DotNetNuke.Entities.Modules.ModuleInfo = Nothing
  Public Property VocabularyId As Integer = 1
  Public Property AllowSpaces As Boolean = False
  Public Property TagLimit As Integer = 0
#End Region

#Region " Event Handlers "
  Private Sub TagEdit_Init(sender As Object, e As System.EventArgs) Handles Me.Init
   DotNetNuke.Framework.jQuery.RegisterJQueryUI(Page)
   ClientResourceManager.RegisterScript(Page, ResolveUrl("~/DesktopModules/Blog/js/tag-it.min.js"))
   If Me.CssClass = "" Then Me.CssClass = "tagit-control"
   ClientResourceManager.RegisterStyleSheet(Page, ResolveUrl("~/DesktopModules/Blog/css/tagit.css"), Web.Client.FileOrder.Css.ModuleCss)
  End Sub

  Private Sub TagEdit_Load(sender As Object, e As System.EventArgs) Handles Me.Load
   ListClientID = Me.ClientID & "_TagEdit"
   If Me.Page.IsPostBack Then
    ' read return values
    Terms = New List(Of TermInfo)
    Dim vocab As Dictionary(Of String, TermInfo) = TermsController.GetTermsByVocabulary(ModuleConfiguration.ModuleID, VocabularyId, Threading.Thread.CurrentThread.CurrentCulture.Name)
    Dim tagList As String = ""
    Me.Page.Request.Params.ReadValue(ClientID, tagList)
    If Not String.IsNullOrEmpty(tagList) Then
     For Each t As String In tagList.Split(","c)
      If vocab.ContainsKey(t) Then
       Terms.Add(vocab(t))
      Else
       Terms.Add(New TermInfo(t))
      End If
     Next
    End If
   End If
   Dim pagescript As String = GetResource("DotNetNuke.Modules.Blog.TagEdit.JS.CodeBlock.txt")
   pagescript = pagescript.Replace("[ID]", ListClientID)
   pagescript = pagescript.Replace("[ModuleId]", ModuleConfiguration.ModuleID.ToString)
   pagescript = pagescript.Replace("[TabId]", ModuleConfiguration.TabID.ToString)
   pagescript = pagescript.Replace("[VocabularyId]", VocabularyId.ToString)
   pagescript = pagescript.Replace("[ClientID]", ClientID)
   pagescript = pagescript.Replace("[AllowSpaces]", AllowSpaces.ToString.ToLower)
   If TagLimit = 0 Then
    pagescript = pagescript.Replace("[TagLimit]", "null")
   Else
    pagescript = pagescript.Replace("[TagLimit]", TagLimit.ToString)
   End If
   If TypeOf (Me.Parent) Is DotNetNuke.Entities.Modules.PortalModuleBase Then
    pagescript = pagescript.Replace("[PlaceholderText]", DotNetNuke.Services.Localization.Localization.GetString(Me.ID & ".PlaceholderText", CType(Me.Parent, DotNetNuke.Entities.Modules.PortalModuleBase).LocalResourceFile))
   End If
   Me.Page.ClientScript.RegisterClientScriptBlock(GetType(String), ClientID, pagescript, True)
  End Sub

  Protected Overrides Sub RenderContents(writer As HtmlTextWriter)

   Using out As System.Xml.XmlWriter = System.Xml.XmlWriter.Create(writer, New System.Xml.XmlWriterSettings With {.OmitXmlDeclaration = True})

    out.WriteStartElement("ul")
    out.WriteAttributeString("id", ListClientID)
    If Width.Value <> 0 Then
     out.WriteAttributeString("width", Width.ToString)
    End If
    If Terms IsNot Nothing Then
     For Each term As TermInfo In Terms
      out.WriteStartElement("li")
      out.WriteValue(term.Name)
      out.WriteEndElement() ' li
     Next
    End If
    out.WriteEndElement() ' ul

   End Using

  End Sub
#End Region

#Region " Public Methods "
  Public Sub CreateMissingTerms()
   For Each t As TermInfo In Terms
    If t.TermId = -1 Then
     t.TermId = DotNetNuke.Entities.Content.Common.Util.GetTermController().AddTerm(New Term(VocabularyId) With {.Name = t.Name})
    End If
   Next
  End Sub
#End Region

 End Class
End Namespace
