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