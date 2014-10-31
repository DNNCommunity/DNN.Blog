'
' DNN Connect - http://dnn-connect.org
' Copyright (c) 2014
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

Namespace Security.Permissions
 Public Class PermissionsController

  Public Shared Function GetPermissions() As PermissionCollection

   Dim CacheKey As String = "BlogPermissions"
   Dim bp As PermissionCollection = CType(DotNetNuke.Common.Utilities.DataCache.GetCache(CacheKey), PermissionCollection)
   If bp Is Nothing Then
    bp = New PermissionCollection
    DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey, bp)
   End If
   Return bp

  End Function

  Public Shared Function GetPermission(permissionId As Integer) As PermissionInfo
   Return GetPermissions.GetById(permissionId)
  End Function

 End Class
End Namespace