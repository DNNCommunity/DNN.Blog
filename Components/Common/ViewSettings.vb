Imports System.Xml
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.Tokens

Namespace Common
 Public Class ViewSettings
  Implements IPropertyAccess

#Region " Private Members "
  Private _allSettings As Hashtable = Nothing
  Private _tabModuleId As Integer = -1
#End Region

#Region " Properties "
  Public Property Template As String = "[G]_default"
  Public Property ShowManagementPanel As Boolean = True
  Public Property ShowComments As Boolean = True
  Public Property BlogModuleId As Integer = -1
  Public Property ShowAllLocales As Boolean = True
  Public Property BlogId As Integer = -1
  Public Property TermId As Integer = -1
  Public Property AuthorId As Integer = -1
  Public Property TemplateSettings As New Dictionary(Of String, String)
  Private Property TemplateManager As Templating.TemplateManager
#End Region

#Region " Constructors "
  Public Sub New(tabModuleId As Integer)
   Me.New(tabModuleId, False)
  End Sub

  Public Sub New(tabModuleId As Integer, justLoadSettings As Boolean)

   _tabModuleId = tabModuleId
   _allSettings = (New DotNetNuke.Entities.Modules.ModuleController).GetTabModuleSettings(tabModuleId)
   _allSettings.ReadValue("Template", Template)
   _allSettings.ReadValue("ShowManagementPanel", ShowManagementPanel)
   _allSettings.ReadValue("ShowComments", ShowComments)
   _allSettings.ReadValue("BlogModuleId", BlogModuleId)
   _allSettings.ReadValue("ShowAllLocales", ShowAllLocales)
   _allSettings.ReadValue("BlogId", BlogId)
   _allSettings.ReadValue("TermId", TermId)
   _allSettings.ReadValue("AuthorId", AuthorId)
   If justLoadSettings Then Exit Sub

   ' Template Settings - first load defaults
   SetTemplate(Template)

  End Sub

  Public Shared Function GetViewSettings(tabModuleId As Integer) As ViewSettings
   Dim CacheKey As String = "TabModuleSettings" & tabModuleId.ToString
   Dim settings As ViewSettings = CType(DotNetNuke.Common.Utilities.DataCache.GetCache(CacheKey), ViewSettings)
   If settings Is Nothing Then
    settings = New ViewSettings(tabModuleId)
    DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey, settings)
   End If
   Return settings
  End Function
#End Region

#Region " Public Members "
  Public Overridable Sub UpdateSettings()
   UpdateSettings(_tabModuleId)
  End Sub

  Public Overridable Sub UpdateSettings(tabModuleId As Integer)

   Dim objModules As New DotNetNuke.Entities.Modules.ModuleController
   objModules.UpdateTabModuleSetting(tabModuleId, "Template", Template)
   objModules.UpdateTabModuleSetting(tabModuleId, "ShowManagementPanel", ShowManagementPanel.ToString)
   objModules.UpdateTabModuleSetting(tabModuleId, "ShowComments", ShowComments.ToString)
   objModules.UpdateTabModuleSetting(tabModuleId, "BlogModuleId", BlogModuleId.ToString)
   objModules.UpdateTabModuleSetting(tabModuleId, "ShowAllLocales", ShowAllLocales.ToString)
   objModules.UpdateTabModuleSetting(tabModuleId, "BlogId", BlogId.ToString)
   objModules.UpdateTabModuleSetting(tabModuleId, "TermId", TermId.ToString)
   objModules.UpdateTabModuleSetting(tabModuleId, "AuthorId", AuthorId.ToString)

   Dim CacheKey As String = "TabModuleSettings" & tabModuleId.ToString
   DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey, Me)

  End Sub

  Public Sub SaveTemplateSettings()
   Dim objModules As New DotNetNuke.Entities.Modules.ModuleController
   For Each key As String In TemplateSettings.Keys
    objModules.UpdateTabModuleSetting(_tabModuleId, "t_" & key, TemplateSettings(key))
   Next
  End Sub

  Public Sub SetTemplateSetting(key As String, value As String)
   If Not TemplateSettings.ContainsKey(key) Then
    TemplateSettings.Add(key, value)
   Else
    TemplateSettings(key) = value
   End If
  End Sub
#End Region

#Region " Private Methods "
  Private Sub SetTemplate(template As String)
   TemplateManager = New Templating.TemplateManager(DotNetNuke.Entities.Portals.PortalSettings.Current, template)
   TemplateSettings.Clear()
   For Each st As Templating.TemplateSetting In _TemplateManager.TemplateSettings.Settings
    TemplateSettings.Add(st.Key, st.DefaultValue)
   Next
   For Each key As String In _allSettings.Keys
    If key.StartsWith("t_") Then
     SetTemplateSetting(Mid(key, 3), CStr(_allSettings(key)))
    End If
   Next
  End Sub
#End Region

#Region " IPropertyAccess "
  Public ReadOnly Property Cacheability As DotNetNuke.Services.Tokens.CacheLevel Implements DotNetNuke.Services.Tokens.IPropertyAccess.Cacheability
   Get
    Return CacheLevel.fullyCacheable
   End Get
  End Property

  Public Function GetProperty(strPropertyName As String, strFormat As String, formatProvider As System.Globalization.CultureInfo, AccessingUser As DotNetNuke.Entities.Users.UserInfo, AccessLevel As DotNetNuke.Services.Tokens.Scope, ByRef PropertyNotFound As Boolean) As String Implements DotNetNuke.Services.Tokens.IPropertyAccess.GetProperty
   Dim OutputFormat As String = String.Empty
   If strFormat = String.Empty Then
    OutputFormat = "D"
   Else
    OutputFormat = strFormat
   End If
   Select Case strPropertyName.ToLower
    Case "tabmoduleid"
     Return (_tabModuleId.ToString(OutputFormat, formatProvider))
    Case "templatepath"
     Return PropertyAccess.FormatString(_TemplateManager.TemplatePath, strFormat)
    Case "templatemappath"
     Return PropertyAccess.FormatString(_TemplateManager.TemplateMapPath, strFormat)
    Case Else
     If TemplateSettings.ContainsKey(strPropertyName) Then
      Return PropertyAccess.FormatString(CStr(TemplateSettings(strPropertyName)), strFormat)
     End If
     If strPropertyName.StartsWith("t_") Then strPropertyName = Mid(strPropertyName, 3)
     If TemplateSettings.ContainsKey(strPropertyName) Then
      Return PropertyAccess.FormatString(CStr(TemplateSettings(strPropertyName)), strFormat)
     End If
     Return ""
   End Select
  End Function
#End Region

 End Class
End Namespace
