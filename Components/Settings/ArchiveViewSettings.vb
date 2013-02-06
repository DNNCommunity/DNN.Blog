'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2012
' by DotNetNuke Corporation
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
Imports DotNetNuke.Modules.Blog.Components.Common

Namespace Components.Settings

 Public Class ArchiveViewSettings

#Region "Private Members"

  Private _allSettings As Hashtable
  Private _tabModuleId As Integer = -1

  Private _ArchiveDisplayMode As String = "Calendar"
  Private _ListDisplayMode As String = "DropDown"
  Private _EnableArchiveCss As Boolean = True
  Private _CalendarMode As String = "ASPNET"
  Private _CalendarSkin As String = "Default"

#End Region

#Region "Constructors"

  Public Sub New(ByVal TabModuleId As Integer)
   _tabModuleId = TabModuleId
   _allSettings = (New DotNetNuke.Entities.Modules.ModuleController).GetTabModuleSettings(_tabModuleId)

   _allSettings.ReadValue(Constants.SettingArchiveDisplayMode, ArchiveDisplayMode)
   _allSettings.ReadValue(Constants.SettingListDisplayMode, ListDisplayMode)
   _allSettings.ReadValue(Constants.SettingEnableArchiveCss, EnableArchiveCss)
   _allSettings.ReadValue(Constants.SettingEnableArchiveCss, EnableArchiveCss)
   _allSettings.ReadValue(Constants.SettingEnableArchiveCss, EnableArchiveCss)
  End Sub

  Public Shared Function GetArchiveViewSettings(ByVal TabModuleId As Integer) As ArchiveViewSettings
   Dim CacheKey As String = Constants.ArchiveSettingsCacheKey & TabModuleId.ToString
   Dim bs As ArchiveViewSettings = CType(DotNetNuke.Common.Utilities.DataCache.GetCache(CacheKey), ArchiveViewSettings)
   If bs Is Nothing Then
    bs = New ArchiveViewSettings(TabModuleId)
    DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey, bs)
   End If
   Return bs
  End Function

#End Region

#Region "Public Members"

  Public Overridable Sub UpdateSettings()
   Dim objModules As New DotNetNuke.Entities.Modules.ModuleController
   With objModules
    .UpdateTabModuleSetting(_tabModuleId, Constants.SettingArchiveDisplayMode, ArchiveDisplayMode)
    .UpdateTabModuleSetting(_tabModuleId, Constants.SettingListDisplayMode, ListDisplayMode)
    .UpdateTabModuleSetting(_tabModuleId, Constants.SettingEnableArchiveCss, EnableArchiveCss.ToString)
   End With
   Dim CacheKey As String = Constants.ArchiveSettingsCacheKey & _tabModuleId.ToString
   DotNetNuke.Common.Utilities.DataCache.RemoveCache(CacheKey)
  End Sub

#End Region

#Region "Properties"

  Public Property ArchiveDisplayMode() As String
   Get
    Return _ArchiveDisplayMode
   End Get
   Set(ByVal Value As String)
    _ArchiveDisplayMode = Value
   End Set
  End Property

  Public Property ListDisplayMode() As String
   Get
    Return _ListDisplayMode
   End Get
   Set(ByVal value As String)
    _ListDisplayMode = value
   End Set
  End Property

  Public Property EnableArchiveCss() As Boolean
   Get
    Return _EnableArchiveCss
   End Get
   Set(ByVal value As Boolean)
    _EnableArchiveCss = value
   End Set
  End Property

  Public Property CalendarMode() As String
   Get
    Return _CalendarMode
   End Get
   Set(ByVal value As String)
    _CalendarMode = value
   End Set
  End Property

  Public Property CalendarSkin() As String
   Get
    Return _CalendarSkin
   End Get
   Set(ByVal value As String)
    _CalendarSkin = value
   End Set
  End Property

#End Region

 End Class

End Namespace