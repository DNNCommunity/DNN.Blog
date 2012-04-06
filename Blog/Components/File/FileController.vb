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

Imports System.IO
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Modules.Blog.Business

Namespace File

 Public Class FileController

#Region "Shared methods"

  Public Shared Function getFileList(ByVal pModulPath As String, ByVal pEntry As EntryInfo) As ArrayList
   Dim myList As New ArrayList

   'Antonio Chagoury 4/09/2007
   'FIX: BLG-5425
   'This routine used to create a folder for each entry
   'everytime the edit page loaded - whether or not a folder was needed
   'Now loads the file list if indeed any files are available
   If Directory.Exists(getEntryDir(pModulPath, pEntry)) Then
    Dim fileList() As String = System.IO.Directory.GetFiles(getEntryDir(pModulPath, pEntry))
    For Each s As String In fileList
     myList.Add(System.IO.Path.GetFileName(s))
    Next
   End If
   Return myList

  End Function

  Public Shared Function getEntryDir(ByVal pModulPath As String, ByVal pEntry As EntryInfo) As String

   Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
   'Antonio Chagoury 4/09/2007
   'FIX: BLG-5425
   'This routine used to create a folder for each entry
   'everytime the edit page loaded - whether or not a folder was needed

   'If Not Directory.Exists(System.Web.HttpContext.Current.Request.MapPath(pModulPath) & "Files\" & pEntry.BlogID.ToString() & "\" & pEntry.EntryID.ToString()) Then
   '    Directory.CreateDirectory(System.Web.HttpContext.Current.Request.MapPath(pModulPath) & "Files\" & pEntry.BlogID.ToString() & "\" & pEntry.EntryID.ToString())
   'End If
   If pEntry Is Nothing Then
    getEntryDir = System.Web.HttpContext.Current.Request.MapPath(pModulPath) & "Files\AnonymousBlogAttachments\"
   Else
    getEntryDir = System.Web.HttpContext.Current.Request.MapPath(pModulPath) & "Files\" & pEntry.BlogID.ToString() & "\" & pEntry.EntryID.ToString() & "\"
   End If
  End Function

  Public Shared Function createFileDirectory(ByVal filePath As String) As String
   Dim newFolderPath As String = filePath.Substring(0, filePath.LastIndexOf("\"))
   ' Make sure the directory exists
   If Not Directory.Exists(newFolderPath) Then
    ' No problem, we'll just create it!
    Directory.CreateDirectory(newFolderPath)
   End If

   ' 11/19/2008 Rip Rowan
   ' Added return, nothing was being returned.  Should this just be a sub?
   Return newFolderPath

  End Function

  Public Shared Function getVirtualFileName(ByVal pModulPath As String, ByVal pFullPath As String) As String
   Dim strReturn As String
   strReturn = pFullPath.Replace(System.Web.HttpContext.Current.Request.MapPath(pModulPath), "")
   strReturn = strReturn.Replace("\", "/")
   strReturn = pModulPath & strReturn
   getVirtualFileName = ResolveUrl(strReturn)
  End Function
#End Region

 End Class

End Namespace