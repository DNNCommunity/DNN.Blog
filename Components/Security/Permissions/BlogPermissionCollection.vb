'
' Bring2mind - http://www.bring2mind.net
' Copyright (c) 2013
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

Imports System
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization
Imports DotNetNuke.Entities.Users

Namespace Security.Permissions
 <Serializable()> _
 Public Class BlogPermissionCollection
  Inherits List(Of BlogPermissionInfo)
  Implements IXmlSerializable

  Public Sub New(BlogPermissions As ArrayList)
   MyBase.New()
   Dim i As Integer
   For i = 0 To BlogPermissions.Count - 1
    Dim objBlogPermission As BlogPermissionInfo = CType(BlogPermissions(i), BlogPermissionInfo)
    Add(objBlogPermission)
   Next
  End Sub

  Public Sub New()
   MyBase.New()
  End Sub

  Default Public Overloads Property Item(index As Integer) As BlogPermissionInfo
   Get
    Return CType(MyBase.Item(index), BlogPermissionInfo)
   End Get
   Set(Value As BlogPermissionInfo)
    MyBase.Item(index) = Value
   End Set
  End Property

  Public Shadows Function ToString(PermissionKey As String) As String
   Dim res As String = ""
   For Each epi As BlogPermissionInfo In Me
    If epi.PermissionKey = PermissionKey Then
     If epi.RoleId = -1 Then
      res &= ";" & DotNetNuke.Common.Globals.glbRoleAllUsersName
     ElseIf epi.RoleId > -1 Then
      res &= ";" & epi.RoleName
     ElseIf epi.UserId > -1 Then
      res &= ";" & epi.UserId.ToString
     End If
    End If
   Next
   Return res & ";"
  End Function

  Public Function CurrentUserHasPermission(PermissionKey As String) As Boolean
   Dim u As UserInfo = UserController.GetCurrentUserInfo
   If u IsNot Nothing Then
    For Each epi As BlogPermissionInfo In Me
     If epi.PermissionKey Is Nothing Then
      Dim objP As PermissionInfo = PermissionsController.GetPermission(epi.PermissionId)
      epi.PermissionKey = objP.PermissionKey
     End If
     If epi.AllowAccess AndAlso epi.PermissionKey = PermissionKey Then
      If epi.RoleId = -1 Then Return True
      If epi.UserId = u.UserID Then Return True
      For Each role As String In u.Roles
       If epi.RoleName = role Then
        Return True
        Exit For
       End If
      Next
     End If
    Next
   End If
   Return False
  End Function

  Public Function ContainsPermissionKey(permissionKey As String) As Boolean
   For Each epi As BlogPermissionInfo In Me
    If epi.PermissionKey Is Nothing Then
     Dim objP As PermissionInfo = PermissionsController.GetPermission(epi.PermissionId)
     epi.PermissionKey = objP.PermissionKey
    End If
    If epi.AllowAccess AndAlso epi.PermissionKey = permissionKey Then
     Return True
     Exit For
    End If
   Next
   Return False
  End Function

  Public Function ContainsPermission(permissionKey As String, RoleId As Integer, UserId As Integer) As Boolean
   For Each epi As BlogPermissionInfo In Me
    If epi.PermissionKey Is Nothing Then
     Dim objP As PermissionInfo = PermissionsController.GetPermission(epi.PermissionId)
     epi.PermissionKey = objP.PermissionKey
    End If
    If epi.PermissionKey = permissionKey And epi.RoleId = RoleId And epi.UserId = UserId Then
     Return True
     Exit For
    End If
   Next
   Return False
  End Function

  Public Sub AddPermission(PortalId As Integer, BlogId As Integer, PermissionKey As String, RoleId As Integer, UserId As Integer)
   Dim objP As PermissionInfo = PermissionsController.GetPermissions(PermissionKey)
   Dim pi As New BlogPermissionInfo(objP)
   With pi
    .BlogId = BlogId
    .RoleId = RoleId
    .UserId = UserId
   End With
   Me.Add(pi)
  End Sub

#Region " IXmlSerializable Implementation "
  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' GetSchema returns the XmlSchema for this class
  ''' </summary>
  ''' <remarks>GetSchema is implemented as a stub method as it is not required</remarks>
  ''' <history>
  ''' 	[pdonker]	05/21/2008  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Function GetSchema() As XmlSchema Implements IXmlSerializable.GetSchema
   Return Nothing
  End Function

  Private Function readElement(reader As XmlReader, ElementName As String) As String
   If (Not reader.NodeType = XmlNodeType.Element) OrElse reader.Name <> ElementName Then
    reader.ReadToFollowing(ElementName)
   End If
   If reader.NodeType = XmlNodeType.Element Then
    Return reader.ReadElementContentAsString
   Else
    Return ""
   End If
  End Function

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' ReadXml fills the object (de-serializes it) from the XmlReader passed
  ''' </summary>
  ''' <remarks></remarks>
  ''' <param name="reader">The XmlReader that contains the xml for the object</param>
  ''' <history>
  ''' 	[pdonker]	05/21/2008  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub ReadXml(reader As XmlReader) Implements IXmlSerializable.ReadXml
   Try
    reader.ReadStartElement("Permissions")
    Do
     reader.ReadStartElement("Permission")
     Dim epi As New BlogPermissionInfo
     epi.ReadXml(reader)
     Me.Add(epi)
    Loop While reader.ReadToNextSibling("Permission")
   Catch ex As Exception
    ' log exception as DNN import routine does not do that
    DotNetNuke.Services.Exceptions.LogException(ex)
    ' re-raise exception to make sure import routine displays a visible error to the user
    Throw New Exception("An error occured during import of an Category", ex)
   End Try

  End Sub

  Public Sub ReadXml(xN As XmlNode)

   For Each xPermission As XmlNode In xN.ChildNodes
    Dim epi As New BlogPermissionInfo
    epi.ReadXml(xPermission)
    If epi.RoleName <> "" Then ' we don't import user permissions
     Me.Add(epi)
    End If
   Next

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' WriteXml converts the object to Xml (serializes it) and writes it using the XmlWriter passed
  ''' </summary>
  ''' <remarks></remarks>
  ''' <param name="writer">The XmlWriter that contains the xml for the object</param>
  ''' <history>
  ''' 	[pdonker]	05/21/2008  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub WriteXml(writer As XmlWriter) Implements IXmlSerializable.WriteXml
   writer.WriteStartElement("Permissions")
   For Each epi As BlogPermissionInfo In Me
    If epi.PermissionKey Is Nothing Then
     Dim pi As PermissionInfo = PermissionsController.GetPermission(epi.PermissionId)
     epi.PermissionKey = pi.PermissionKey
    End If
    epi.WriteXml(writer)
   Next
   writer.WriteEndElement()
  End Sub
#End Region

 End Class
End Namespace
