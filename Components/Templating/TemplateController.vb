'
' Bring2mind - http://www.bring2mind.net
' Copyright (c) 2012
' by Bring2mind
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

Imports System.IO

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Cache
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Localization.Localization

Namespace Templating
 Public Class Templating

#Region " Public Methods "
  Public Shared Function FormatBoolean(ByVal value As Boolean, ByVal format As String, ByVal formatProvider As System.Globalization.CultureInfo) As String
   If String.IsNullOrEmpty(format) Then
    Return value.ToString
   End If
   If format.Contains(";") Then
    If value Then
     Return Left(format, format.IndexOf(";"))
    Else
     Return Mid(format, format.IndexOf(";") + 2)
    End If
   End If
   Return DotNetNuke.Services.Tokens.PropertyAccess.Boolean2LocalizedYesNo(value, formatProvider)
  End Function
#End Region

#Region " Private Methods "
  Private Shared Function GetTemplateFileLookupDictionary(ByVal cacheItemArgs As CacheItemArgs) As Object
   Return New Dictionary(Of String, Boolean)
  End Function

  Private Shared Function GetTemplateFileLookupDictionary() As Dictionary(Of String, Boolean)
   Return CBO.GetCachedObject(Of Dictionary(Of String, Boolean))(New CacheItemArgs(DataCache.ResourceFileLookupDictionaryCacheKey, DataCache.ResourceFileLookupDictionaryTimeOut, DataCache.ResourceFileLookupDictionaryCachePriority), AddressOf GetTemplateFileLookupDictionary)
  End Function

  Private Shared Function GetTemplateFileCallBack(ByVal cacheItemArgs As CacheItemArgs) As Object

   Dim cacheKey As String = cacheItemArgs.CacheKey
   Dim Template As String = Nothing
   Dim TemplateFileExistsLookup As Dictionary(Of String, Boolean) = GetTemplateFileLookupDictionary()
   If (Not TemplateFileExistsLookup.ContainsKey(cacheKey)) OrElse TemplateFileExistsLookup(cacheKey) Then
    Dim filePath As String = Nothing
    If cacheKey.Contains(":\") AndAlso Path.IsPathRooted(cacheKey) Then
     If IO.File.Exists(cacheKey) Then
      filePath = cacheKey
     End If
    End If
    If filePath Is Nothing Then
     filePath = System.Web.Hosting.HostingEnvironment.MapPath(ApplicationPath + cacheKey)
    End If
    If IO.File.Exists(filePath) Then
     Template = Common.Globals.ReadFile(filePath)
     cacheItemArgs.CacheDependency = New DNNCacheDependency(filePath)
     TemplateFileExistsLookup(cacheKey) = True
    Else
     TemplateFileExistsLookup(cacheKey) = False
    End If
   End If
   Return Template

  End Function

  Public Shared Function GetTemplateFile(ByVal TemplateFile As String) As String
   Return CBO.GetCachedObject(Of String)(New CacheItemArgs(TemplateFile, DataCache.ResourceFilesCacheTimeOut, DataCache.ResourceFilesCachePriority), AddressOf GetTemplateFileCallBack)
  End Function
#End Region


 End Class
End Namespace
