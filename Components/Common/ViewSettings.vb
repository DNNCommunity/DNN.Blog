'
' DNN Connect - http://dnn-connect.org
' Copyright (c) 2015
' by DNN Connect
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'

Imports System.Xml
Imports DotNetNuke.Services.Tokens

Namespace Common
 <Serializable()>
 Public Class ViewSettings
  Implements IPropertyAccess

#Region " Private Members "
  Private _allSettings As Hashtable = Nothing
  Private _tabModuleId As Integer = -1
#End Region

#Region " Properties "
  Public Property Template As String = "[G]_default"
  Public Property BlogModuleId As Integer = -1
  Public Property ShowAllLocales As Boolean = True
  Public Property ModifyPageDetails As Boolean = False
  Public Property ShowManagementPanel As Boolean = False
  Public Property ShowManagementPanelViewMode As Boolean = True
  Public Property HideUnpublishedBlogsViewMode As Boolean = False
  Public Property HideUnpublishedBlogsEditMode As Boolean = False
  Public Property BlogId As Integer = -1
  Public Property Categories As String = ""
  Public Property AuthorId As Integer = -1
  Public Property TemplateSettings As New Dictionary(Of String, String)
  Private Property TemplateManager As Templating.TemplateManager
  Friend Property CanCache As Boolean = True
#End Region

#Region " ReadOnly Properties "
  Private _categoryList As List(Of Integer)
  Public ReadOnly Property CategoryList As List(Of Integer)
   Get
    If _categoryList Is Nothing Then
     _categoryList = New List(Of Integer)
     For Each c As String In Categories.Split(","c)
      If IsNumeric(c) Then
       _categoryList.Add(Integer.Parse(c))
      End If
     Next
    End If
    Return _categoryList
   End Get
  End Property
#End Region

#Region " Constructors "
  Public Sub New(tabModuleId As Integer)
   Me.New(tabModuleId, False)
  End Sub

  Public Sub New(tabModuleId As Integer, justLoadSettings As Boolean)

   _tabModuleId = tabModuleId
   _allSettings = (New DotNetNuke.Entities.Modules.ModuleController).GetTabModule(tabModuleId).TabModuleSettings
   _allSettings.ReadValue("Template", Template)
   _allSettings.ReadValue("BlogModuleId", BlogModuleId)
   _allSettings.ReadValue("ShowAllLocales", ShowAllLocales)
   _allSettings.ReadValue("ModifyPageDetails", ModifyPageDetails)
   _allSettings.ReadValue("ShowManagementPanel", ShowManagementPanel)
   _allSettings.ReadValue("ShowManagementPanelViewMode", ShowManagementPanelViewMode)
   _allSettings.ReadValue("HideUnpublishedBlogsViewMode", HideUnpublishedBlogsViewMode)
   _allSettings.ReadValue("HideUnpublishedBlogsEditMode", HideUnpublishedBlogsEditMode)
   _allSettings.ReadValue("BlogId", BlogId)
   _allSettings.ReadValue("Categories", Categories)
   _allSettings.ReadValue("AuthorId", AuthorId)
   If BlogModuleId > -1 And tabModuleId > -1 Then ' security check
    Dim parentModule As DotNetNuke.Entities.Modules.ModuleInfo = (New DotNetNuke.Entities.Modules.ModuleController).GetModule(BlogModuleId)
    Dim thisTabModule As DotNetNuke.Entities.Modules.ModuleInfo = (New DotNetNuke.Entities.Modules.ModuleController).GetTabModule(tabModuleId)
    If parentModule Is Nothing OrElse parentModule.PortalID <> thisTabModule.PortalID Then
     BlogModuleId = -1
     CanCache = False
    End If
   End If
   If justLoadSettings Then Exit Sub

   ' Template Settings - first load defaults
   SetTemplate(Template)

  End Sub

  Public Shared Function GetViewSettings(tabModuleId As Integer) As ViewSettings
   Dim CacheKey As String = "Blog_TabModuleSettings" & tabModuleId.ToString
   Dim settings As ViewSettings = CType(DotNetNuke.Common.Utilities.DataCache.GetCache(CacheKey), ViewSettings)
   If settings Is Nothing Then
    settings = New ViewSettings(tabModuleId)
    If settings.CanCache Then DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey, settings)
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
   objModules.UpdateTabModuleSetting(tabModuleId, "BlogModuleId", BlogModuleId.ToString)
   objModules.UpdateTabModuleSetting(tabModuleId, "ShowAllLocales", ShowAllLocales.ToString)
   objModules.UpdateTabModuleSetting(tabModuleId, "ModifyPageDetails", ModifyPageDetails.ToString)
   objModules.UpdateTabModuleSetting(tabModuleId, "ShowManagementPanel", ShowManagementPanel.ToString)
   objModules.UpdateTabModuleSetting(tabModuleId, "ShowManagementPanelViewMode", ShowManagementPanelViewMode.ToString)
   objModules.UpdateTabModuleSetting(tabModuleId, "HideUnpublishedBlogsViewMode", HideUnpublishedBlogsViewMode.ToString)
   objModules.UpdateTabModuleSetting(tabModuleId, "HideUnpublishedBlogsEditMode", HideUnpublishedBlogsEditMode.ToString)
   objModules.UpdateTabModuleSetting(tabModuleId, "BlogId", BlogId.ToString)
   objModules.UpdateTabModuleSetting(tabModuleId, "Categories", Categories.ToString)
   objModules.UpdateTabModuleSetting(tabModuleId, "AuthorId", AuthorId.ToString)
   _categoryList = Nothing

   Dim CacheKey As String = "Blog_TabModuleSettings" & tabModuleId.ToString
   DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey, Me)

  End Sub

  Public Sub SaveTemplateSettings()
   Dim objModules As New DotNetNuke.Entities.Modules.ModuleController
   For Each key As String In TemplateSettings.Keys
    objModules.UpdateTabModuleSetting(_tabModuleId, "t_" & key, TemplateSettings(key))
   Next
   Dim CacheKey As String = "Blog_TabModuleSettings" & _tabModuleId.ToString
   DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey, Me)
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
     Select Case strPropertyName.ToLower
      Case "termid", "categories" ' termid is for legacy purposes
       Return Categories
      Case "authorid"
       Return (AuthorId.ToString(OutputFormat, formatProvider))
      Case "blogid"
       Return (BlogId.ToString(OutputFormat, formatProvider))
      Case "blogmoduleid"
       Return (BlogModuleId.ToString(OutputFormat, formatProvider))
      Case Else
       Return ""
     End Select
   End Select
  End Function
#End Region

#Region " Serialization "
  Public Sub Serialize(writer As XmlWriter)
   writer.WriteStartElement("ViewSettings")
   writer.WriteElementString("Template", Template)
   writer.WriteElementString("BlogModuleId", BlogModuleId.ToString)
   writer.WriteElementString("ShowAllLocales", ShowAllLocales.ToString)
   writer.WriteElementString("ModifyPageDetails", ModifyPageDetails.ToString)
   writer.WriteElementString("ShowManagementPanel", ShowManagementPanel.ToString)
   writer.WriteElementString("ShowManagementPanelViewMode", ShowManagementPanelViewMode.ToString)
   writer.WriteElementString("HideUnpublishedBlogsViewMode", HideUnpublishedBlogsViewMode.ToString)
   writer.WriteElementString("HideUnpublishedBlogsEditMode", HideUnpublishedBlogsEditMode.ToString)
   writer.WriteElementString("BlogId", BlogId.ToString)
   writer.WriteElementString("AuthorId", AuthorId.ToString)
   writer.WriteEndElement() ' viewsettings
  End Sub

  Public Sub FromXml(xml As XmlNode)
   If xml Is Nothing Then Exit Sub
   xml.ReadValue("Template", Template)
   xml.ReadValue("BlogModuleId", BlogModuleId)
   xml.ReadValue("ShowAllLocales", ShowAllLocales)
   xml.ReadValue("ModifyPageDetails", ModifyPageDetails)
   xml.ReadValue("ShowManagementPanel", ShowManagementPanel)
   xml.ReadValue("ShowManagementPanelViewMode", ShowManagementPanelViewMode)
   xml.ReadValue("HideUnpublishedBlogsViewMode", HideUnpublishedBlogsViewMode)
   xml.ReadValue("HideUnpublishedBlogsEditMode", HideUnpublishedBlogsEditMode)
   xml.ReadValue("BlogId", BlogId)
   xml.ReadValue("AuthorId", AuthorId)
   _categoryList = Nothing
  End Sub
#End Region

 End Class
End Namespace
