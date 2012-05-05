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
Imports DotNetNuke.Services.Localization

Namespace Settings

 ''' <summary>
 ''' This class abstracts all settings for the module and makes sure they're (a) defaulted and (b) hard typed 
 ''' throughout the application.
 ''' </summary>
 ''' <remarks></remarks>
 ''' <history>
 '''		[pdonker]	12/30/2009	created
 ''' </history>
 Public Class RecentEntriesSettings

#Region " Private Members "
  Private _allSettings As Hashtable
  Private _tabModuleId As Integer = -1
  Private _RecentEntriesTemplate As String = ""
  Private _RecentEntriesMax As Integer = 10
#End Region

#Region " Constructors "
  Public Sub New(ByVal TabModuleId As Integer)

   _tabModuleId = TabModuleId
   _allSettings = (New DotNetNuke.Entities.Modules.ModuleController).GetTabModuleSettings(_tabModuleId)
   Globals.ReadValue(_allSettings, "RecentEntriesTemplate", RecentEntriesTemplate)
   Globals.ReadValue(_allSettings, "RecentEntriesMax", RecentEntriesMax)

  End Sub

  Public Shared Function GetRecentEntriesSettings(ByVal TabModuleId As Integer) As RecentEntriesSettings
   Dim CacheKey As String = "RecentEntriesSettings" & TabModuleId.ToString
   Dim bs As RecentEntriesSettings = CType(DotNetNuke.Common.Utilities.DataCache.GetCache(CacheKey), RecentEntriesSettings)
   If bs Is Nothing Then
    bs = New RecentEntriesSettings(TabModuleId)
    DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey, bs)
   End If
   Return bs
  End Function
#End Region

#Region " Public Members "
  Public Overridable Sub UpdateSettings()

   Dim objModules As New DotNetNuke.Entities.Modules.ModuleController
   With objModules
    .UpdateTabModuleSetting(_tabModuleId, "RecentEntriesTemplate", RecentEntriesTemplate)
    .UpdateTabModuleSetting(_tabModuleId, "RecentEntriesMax", RecentEntriesMax.ToString)
   End With
   Dim CacheKey As String = "RecentEntriesSettings" & _tabModuleId.ToString
   DotNetNuke.Common.Utilities.DataCache.RemoveCache(CacheKey)

  End Sub
#End Region

#Region " Properties "
  Public Property RecentEntriesTemplate() As String
   Get
    If _RecentEntriesTemplate = "" Then
     _RecentEntriesTemplate = Localization.GetString("DefaultRecentEntriesTemplate", BlogModuleBase.BLOG_TEMPLATES_RESOURCE)
    End If
    Return _RecentEntriesTemplate
   End Get
   Set(ByVal value As String)
    _RecentEntriesTemplate = value
   End Set
  End Property

  Public Property RecentEntriesMax() As Integer
   Get
    Return _RecentEntriesMax
   End Get
   Set(ByVal value As Integer)
    _RecentEntriesMax = value
   End Set
  End Property
#End Region

 End Class

End Namespace