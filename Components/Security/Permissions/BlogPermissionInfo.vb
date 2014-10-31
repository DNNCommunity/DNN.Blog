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

Imports System
Imports System.Data
Imports System.IO
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json
Imports System.Text
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Tokens

Imports DotNetNuke.Modules.Blog.Security.Security

Namespace Security.Permissions
 Partial Public Class BlogPermissionInfo

#Region " Constructors "
  Public Sub New()
   _BlogId = Null.NullInteger
   _RoleId = glbRoleNothing
   _AllowAccess = False
   _RoleName = Null.NullString
   _UserId = glbUserNothing
   _Username = Null.NullString
   _DisplayName = Null.NullString
  End Sub 'New

  Public Sub New(pi As PermissionInfo)
   _BlogId = Null.NullInteger
   _RoleId = glbRoleNothing
   _AllowAccess = False
   _RoleName = Null.NullString
   _UserId = glbUserNothing
   _Username = Null.NullString
   _DisplayName = Null.NullString
   PermissionId = pi.PermissionId
   PermissionKey = pi.PermissionKey
  End Sub 'New
#End Region


#Region " Public Properties "
  Private _RoleName As String
  <DataMember()>
  Public Property RoleName() As String
   Get
    If String.IsNullOrEmpty(_RoleName) Then
     If RoleId = -1 Then
      _RoleName = DotNetNuke.Common.glbRoleAllUsersName
     End If
    End If
    Return _RoleName
   End Get
   Set(Value As String)
    _RoleName = Value
   End Set
  End Property

  Public Property PermissionKey As String
#End Region

#Region " Public Methods "
  Public Overloads Overrides Function Equals(obj As Object) As Boolean
   If obj Is Nothing Or Not Me.GetType() Is obj.GetType() Then
    Return False
   End If
   Dim perm As BlogPermissionInfo = CType(obj, BlogPermissionInfo)
   Return (Me.AllowAccess = perm.AllowAccess) And (Me.Expires > Now) And (Me.BlogId = perm.BlogId) And (Me.RoleId = perm.RoleId) And (Me.UserId = perm.UserId) And (Me.PermissionId = perm.PermissionId)
  End Function

  Public Function Clone() As BlogPermissionInfo
   Dim res As New BlogPermissionInfo
   With res
    .AllowAccess = AllowAccess
    .DisplayName = DisplayName
    .BlogId = BlogId
    .Expires = Expires
    .PermissionId = PermissionId
    .PermissionKey = PermissionKey
    .RoleId = RoleId
    .RoleName = RoleName
    .UserId = UserId
    .Username = Username
   End With
   Return res
  End Function

  Public Overloads Sub ReadXml(xN As XmlNode)

   Dim ht As New Hashtable
   For Each n As XmlNode In xN.ChildNodes
    ht.Add(n.Name, n.InnerText)
   Next
   ht.ReadValue("PermissionId", PermissionId)
   ht.ReadValue("AllowAccess", AllowAccess)
   ht.ReadValue("PermissionKey", PermissionKey)
   ht.ReadValue("RoleID", RoleId)
   ht.ReadValue("UserID", UserId)
   ht.ReadValue("RoleName", RoleName)
   ht.ReadValue("UserName", Username)

  End Sub
#End Region

 End Class
End Namespace


