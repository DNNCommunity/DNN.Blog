'
' DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2010
' by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
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
'-------------------------------------------------------------------------

Imports DotNetNuke.Entities.Modules

Public Enum RssViews
 None = 0
 ArchivEntries = 1
 BlogEntries = 2
 RecentEntries = 3
 SingleEntry = 4
End Enum

Public Class Globals
 Public Const glbSharedResourceFile As String = "DesktopModules/Blog/App_LocalResources/SharedResources"

 Public Shared Function RemoveMarkup(ByVal input As String) As String
  Return Regex.Replace(input, "<[^>]+>", "")
 End Function

#Region " Value Reading "
 Public Shared Sub ReadValue(ByRef ValueTable As Hashtable, ByVal ValueName As String, ByRef Variable As Integer)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Integer)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As Hashtable, ByVal ValueName As String, ByRef Variable As Long)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Long)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As Hashtable, ByVal ValueName As String, ByRef Variable As String)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), String)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As Hashtable, ByVal ValueName As String, ByRef Variable As Boolean)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Boolean)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As Hashtable, ByVal ValueName As String, ByRef Variable As Date)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Date)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As Hashtable, ByVal ValueName As String, ByRef Variable As TimeSpan)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = TimeSpan.Parse(CType(ValueTable.Item(ValueName), String))
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As NameValueCollection, ByVal ValueName As String, ByRef Variable As Integer)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Integer)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As NameValueCollection, ByVal ValueName As String, ByRef Variable As Long)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Long)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As NameValueCollection, ByVal ValueName As String, ByRef Variable As String)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), String)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As NameValueCollection, ByVal ValueName As String, ByRef Variable As Boolean)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Boolean)
   Catch ex As Exception
    Select Case ValueTable.Item(ValueName).ToLower
     Case "on", "yes"
      Variable = True
     Case Else
      Variable = False
    End Select
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As NameValueCollection, ByVal ValueName As String, ByRef Variable As Date)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Date)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As NameValueCollection, ByVal ValueName As String, ByRef Variable As TimeSpan)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = TimeSpan.Parse(CType(ValueTable.Item(ValueName), String))
   Catch ex As Exception
   End Try
  End If
 End Sub
#End Region

 Public Shared Sub AddModDef(ByVal PortalSettings As Entities.Portals.PortalSettings, ByVal ModuleDefID As Integer, ByVal TabID As Integer, ByVal paneName As String, ByVal position As Integer, ByVal title As String)

  Dim objModuleDefinition As Definitions.ModuleDefinitionInfo = (New Definitions.ModuleDefinitionController).GetModuleDefinition(ModuleDefID)
  Dim objTabPermissions As Security.Permissions.TabPermissionCollection = (New Security.Permissions.TabPermissionController).GetTabPermissionsCollectionByTabID(TabID, PortalSettings.PortalId)
  Dim objModule As New ModuleInfo
  objModule.Initialize(PortalSettings.PortalId)
  objModule.PortalID = PortalSettings.PortalId
  objModule.TabID = PortalSettings.ActiveTab.TabID
  objModule.ModuleOrder = position
  If title = "" Then
   objModule.ModuleTitle = objModuleDefinition.FriendlyName
  Else
   objModule.ModuleTitle = title
  End If
  objModule.PaneName = paneName
  objModule.ModuleDefID = objModuleDefinition.ModuleDefID
  objModule.InheritViewPermissions = True

  ' get the default module view permissions
  Dim arrSystemModuleViewPermissions As ArrayList = (New DotNetNuke.Security.Permissions.PermissionController).GetPermissionByCodeAndKey("SYSTEM_MODULE_DEFINITION", "VIEW")

  objModule.AllTabs = False
  objModule.Alignment = ""
  Dim objModules As New ModuleController
  objModules.AddModule(objModule)

 End Sub

End Class
