'
' DNN Connect - http://dnn-connect.org
' Copyright (c) 2015
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

Imports System.Runtime.Serialization

Namespace Security.Permissions
 Partial Public Class BlogPermissionInfo

#Region " Private Members "
#End Region

#Region " Public Properties "
  <DataMember()>
  Public Property AllowAccess() As Boolean
  <DataMember()>
  Public Property BlogId() As Int32
  <DataMember()>
  Public Property Expires() As Date
  <DataMember()>
  Public Property PermissionId() As Int32
  <DataMember()>
  Public Property RoleId() As Int32
  <DataMember()>
  Public Property UserId() As Int32
  <DataMember()>
  Public Property Username() As String
  <DataMember()>
  Public Property DisplayName() As String
  '<DataMember()>
  'Public Property RoleName() As String
#End Region

 End Class
End Namespace


