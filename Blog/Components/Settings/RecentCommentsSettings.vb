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
 Public Class RecentCommentsSettings

#Region " Private Members "
  Private _allSettings As Hashtable
  Private _tabModuleId As Integer = -1
  Private _RecentCommentsTemplate As String = ""
  Private _RecentCommentsMax As Integer = 10
#End Region

#Region " Constructors "
  Public Sub New(ByVal TabModuleId As Integer)

   _tabModuleId = TabModuleId
   _allSettings = (New DotNetNuke.Entities.Modules.ModuleController).GetTabModuleSettings(_tabModuleId)
   Globals.ReadValue(_allSettings, "RecentCommentsTemplate", RecentCommentsTemplate)
   Globals.ReadValue(_allSettings, "RecentCommentsMax", RecentCommentsMax)

  End Sub

  Public Shared Function GetRecentCommentsSettings(ByVal TabModuleId As Integer) As RecentCommentsSettings
   Dim CacheKey As String = "RecentCommentsSettings" & TabModuleId.ToString
   Dim bs As RecentCommentsSettings = CType(DotNetNuke.Common.Utilities.DataCache.GetCache(CacheKey), RecentCommentsSettings)
   If bs Is Nothing Then
    bs = New RecentCommentsSettings(TabModuleId)
    DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey, bs)
   End If
   Return bs
  End Function
#End Region

#Region " Public Members "
  Public Overridable Sub UpdateSettings()

   Dim objModules As New DotNetNuke.Entities.Modules.ModuleController
   With objModules
    .UpdateTabModuleSetting(_tabModuleId, "RecentCommentsTemplate", RecentCommentsTemplate)
    .UpdateTabModuleSetting(_tabModuleId, "RecentCommentsMax", RecentCommentsMax.ToString)
   End With
   Dim CacheKey As String = "RecentCommentsSettings" & _tabModuleId.ToString
   DotNetNuke.Common.Utilities.DataCache.RemoveCache(CacheKey)

  End Sub
#End Region

#Region " Properties "
  Public Property RecentCommentsTemplate() As String
   Get
    If _RecentCommentsTemplate = "" Then
     _RecentCommentsTemplate = Localization.GetString("DefaultRecentCommentsTemplate", BlogModuleBase.BLOG_TEMPLATES_RESOURCE)
    End If
    Return _RecentCommentsTemplate
   End Get
   Set(ByVal value As String)
    _RecentCommentsTemplate = value
   End Set
  End Property

  Public Property RecentCommentsMax() As Integer
   Get
    Return _RecentCommentsMax
   End Get
   Set(ByVal value As Integer)
    _RecentCommentsMax = value
   End Set
  End Property
#End Region

 End Class

End Namespace