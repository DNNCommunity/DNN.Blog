'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2013
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
Imports DotNetNuke.Common
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Modules.Blog.Entities
Imports DotNetNuke.Modules.Blog.Entities.Posts

Namespace Integration

 Public Class FileController

#Region "Shared methods"

  Public Shared Function getFileList(pModulPath As String, pPost As PostInfo) As ArrayList

   Dim myList As New ArrayList
   If Directory.Exists(getPostDir(pModulPath, pPost)) Then
    Dim fileList() As String = System.IO.Directory.GetFiles(getPostDir(pModulPath, pPost))
    For Each s As String In fileList
     myList.Add(System.IO.Path.GetFileName(s))
    Next
   End If
   Return myList

  End Function

  Public Shared Function getPostDir(pModulPath As String, pPost As PostInfo) As String

   Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
   If pPost Is Nothing Then
    getPostDir = System.Web.HttpContext.Current.Request.MapPath(pModulPath) & "Files\AnonymousBlogAttachments\"
   Else
    getPostDir = System.Web.HttpContext.Current.Request.MapPath(pModulPath) & "Files\" & pPost.BlogID.ToString() & "\" & pPost.ContentItemId.ToString() & "\"
   End If

  End Function

  Public Shared Function createFileDirectory(filePath As String) As String

   Dim newFolderPath As String = filePath.Substring(0, filePath.LastIndexOf("\"))
   If Not Directory.Exists(newFolderPath) Then Directory.CreateDirectory(newFolderPath)
   Return newFolderPath

  End Function

  Public Shared Function getVirtualFileName(pModulPath As String, pFullPath As String) As String
   Dim strReturn As String
   strReturn = pFullPath.Replace(System.Web.HttpContext.Current.Request.MapPath(pModulPath), "")
   strReturn = strReturn.Replace("\", "/")
   strReturn = pModulPath & strReturn
   getVirtualFileName = ResolveUrl(strReturn)
  End Function
#End Region

 End Class

End Namespace