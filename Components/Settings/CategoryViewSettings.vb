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

 Public Class CategoryViewSettings

#Region "Private Members"

  Private _allSettings As Hashtable
  Private _tabModuleId As Integer = -1

  Private _CategoryDisplayMode As String = "Tree"
  Private _TreeSkin As String = "Default"

#End Region

#Region "Constructors"

  Public Sub New(ByVal TabModuleId As Integer)
   _tabModuleId = TabModuleId
   _allSettings = (New DotNetNuke.Entities.Modules.ModuleController).GetTabModuleSettings(_tabModuleId)

   _allSettings.ReadValue(Constants.SettingCategoryDisplayMode, CategoryDisplayMode)
   _allSettings.ReadValue(Constants.SettingCategoryTreeSkin, TreeSkin)
  End Sub

  Public Shared Function GetCategoryViewSettings(ByVal TabModuleId As Integer) As CategoryViewSettings
   Dim CacheKey As String = Constants.CategorySettingsCacheKey & TabModuleId.ToString
   Dim bs As CategoryViewSettings = CType(DotNetNuke.Common.Utilities.DataCache.GetCache(CacheKey), CategoryViewSettings)

   If bs Is Nothing Then
    Dim timeOut As Int32 = Common.Constants.CACHE_TIMEOUT * Convert.ToInt32(DotNetNuke.Entities.Host.Host.PerformanceSetting)
    bs = New CategoryViewSettings(TabModuleId)

    'Cache if timeout > 0 and settings are not null
    If timeOut > 0 And bs IsNot Nothing Then
     DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey, bs, TimeSpan.FromMinutes(timeOut))
    End If
   End If
   Return bs
  End Function

#End Region

#Region "Public Members"

  Public Overridable Sub UpdateSettings()
   Dim objModules As New DotNetNuke.Entities.Modules.ModuleController
   With objModules
    .UpdateTabModuleSetting(_tabModuleId, Constants.SettingCategoryDisplayMode, CategoryDisplayMode)
    .UpdateTabModuleSetting(_tabModuleId, Constants.SettingCategoryTreeSkin, TreeSkin)
   End With
   Dim CacheKey As String = Constants.CategorySettingsCacheKey & _tabModuleId.ToString
   DotNetNuke.Common.Utilities.DataCache.RemoveCache(CacheKey)
  End Sub

#End Region

#Region "Properties"

  Public Property CategoryDisplayMode() As String
   Get
    Return _CategoryDisplayMode
   End Get
   Set(ByVal Value As String)
    _CategoryDisplayMode = Value
   End Set
  End Property

  Public Property TreeSkin() As String
   Get
    Return _TreeSkin
   End Get
   Set(ByVal Value As String)
    _TreeSkin = Value
   End Set
  End Property

#End Region

 End Class

End Namespace