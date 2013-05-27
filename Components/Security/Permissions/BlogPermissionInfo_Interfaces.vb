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

Namespace Security.Permissions

 <Serializable(), XmlRoot("BlogPermission"), DataContract()>
 Partial Public Class BlogPermissionInfo
  Implements IHydratable
  Implements IPropertyAccess
  Implements IXmlSerializable

#Region " IHydratable Implementation "
  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Fill hydrates the object from a Datareader
  ''' </summary>
  ''' <remarks>The Fill method is used by the CBO method to hydrtae the object
  ''' rather than using the more expensive Refection  methods.</remarks>
  ''' <history>
  ''' 	[pdonker]	02/08/2013  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub Fill(dr As IDataReader) Implements IHydratable.Fill

   AllowAccess = Convert.ToBoolean(Null.SetNull(dr.Item("AllowAccess"), AllowAccess))
   BlogId = Convert.ToInt32(Null.SetNull(dr.Item("BlogId"), BlogId))
   Expires = CDate(Null.SetNull(dr.Item("Expires"), Expires))
   PermissionId = Convert.ToInt32(Null.SetNull(dr.Item("PermissionId"), PermissionId))
   RoleId = Convert.ToInt32(Null.SetNull(dr.Item("RoleId"), RoleId))
   UserId = Convert.ToInt32(Null.SetNull(dr.Item("UserId"), UserId))
   Username = Convert.ToString(Null.SetNull(dr.Item("Username"), Username))
   DisplayName = Convert.ToString(Null.SetNull(dr.Item("DisplayName"), DisplayName))
   RoleName = Convert.ToString(Null.SetNull(dr.Item("RoleName"), RoleName))

  End Sub
  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets and sets the Key ID
  ''' </summary>
  ''' <remarks>The KeyID property is part of the IHydratble interface.  It is used
  ''' as the key property when creating a Dictionary</remarks>
  ''' <history>
  ''' 	[pdonker]	02/08/2013  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Property KeyID() As Integer Implements IHydratable.KeyID
   Get
    Return Nothing
   End Get
   Set(value As Integer)
   End Set
  End Property
#End Region

#Region " IPropertyAccess Implementation "
  Public Function GetProperty(strPropertyName As String, strFormat As String, formatProvider As System.Globalization.CultureInfo, AccessingUser As DotNetNuke.Entities.Users.UserInfo, AccessLevel As DotNetNuke.Services.Tokens.Scope, ByRef PropertyNotFound As Boolean) As String Implements DotNetNuke.Services.Tokens.IPropertyAccess.GetProperty
   Dim OutputFormat As String = String.Empty
   Dim portalSettings As DotNetNuke.Entities.Portals.PortalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings()
   If strFormat = String.Empty Then
    OutputFormat = "D"
   Else
    OutputFormat = strFormat
   End If
   Select Case strPropertyName.ToLower
    Case "allowaccess"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.AllowAccess, formatProvider)
    Case "blogid"
     Return (Me.BlogId.ToString(OutputFormat, formatProvider))
    Case "expires"
     Return (Me.Expires.ToString(OutputFormat, formatProvider))
    Case "permissionid"
     Return (Me.PermissionId.ToString(OutputFormat, formatProvider))
    Case "roleid"
     Return (Me.RoleId.ToString(OutputFormat, formatProvider))
    Case "userid"
     Return (Me.UserId.ToString(OutputFormat, formatProvider))
    Case "username"
     Return PropertyAccess.FormatString(Me.Username, strFormat)
    Case "displayname"
     Return PropertyAccess.FormatString(Me.DisplayName, strFormat)
    Case "rolename"
     Return PropertyAccess.FormatString(Me.RoleName, strFormat)
    Case Else
     PropertyNotFound = True
   End Select

   Return Null.NullString
  End Function

  Public ReadOnly Property Cacheability() As DotNetNuke.Services.Tokens.CacheLevel Implements DotNetNuke.Services.Tokens.IPropertyAccess.Cacheability
   Get
    Return CacheLevel.fullyCacheable
   End Get
  End Property
#End Region

#Region " IXmlSerializable Implementation "
  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' GetSchema returns the XmlSchema for this class
  ''' </summary>
  ''' <remarks>GetSchema is implemented as a stub method as it is not required</remarks>
  ''' <history>
  ''' 	[pdonker]	02/08/2013  Created
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
  ''' 	[pdonker]	02/08/2013  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Overloads Sub ReadXml(reader As XmlReader) Implements IXmlSerializable.ReadXml
   Try

    Boolean.TryParse(readElement(reader, "AllowAccess"), AllowAccess)
    If Not Int32.TryParse(readElement(reader, "BlogId"), BlogId) Then
     BlogId = Null.NullInteger
    End If
    Date.TryParse(readElement(reader, "Expires"), Expires)
    If Not Int32.TryParse(readElement(reader, "PermissionId"), PermissionId) Then
     PermissionId = Null.NullInteger
    End If
    If Not Int32.TryParse(readElement(reader, "RoleId"), RoleId) Then
     RoleId = Null.NullInteger
    End If
    If Not Int32.TryParse(readElement(reader, "UserId"), UserId) Then
     UserId = Null.NullInteger
    End If
    Username = readElement(reader, "Username")
    DisplayName = readElement(reader, "DisplayName")
    RoleName = readElement(reader, "RoleName")
   Catch ex As Exception
    ' log exception as DNN import routine does not do that
    DotNetNuke.Services.Exceptions.LogException(ex)
    ' re-raise exception to make sure import routine displays a visible error to the user
    Throw New Exception("An error occured during import of an BlogPermission", ex)
   End Try

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' WriteXml converts the object to Xml (serializes it) and writes it using the XmlWriter passed
  ''' </summary>
  ''' <remarks></remarks>
  ''' <param name="writer">The XmlWriter that contains the xml for the object</param>
  ''' <history>
  ''' 	[pdonker]	02/08/2013  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub WriteXml(writer As XmlWriter) Implements IXmlSerializable.WriteXml
   writer.WriteStartElement("BlogPermission")
   writer.WriteElementString("AllowAccess", AllowAccess.ToString())
   writer.WriteElementString("BlogId", BlogId.ToString())
   writer.WriteElementString("Expires", Expires.ToString())
   writer.WriteElementString("PermissionId", PermissionId.ToString())
   writer.WriteElementString("RoleId", RoleId.ToString())
   writer.WriteElementString("UserId", UserId.ToString())
   writer.WriteElementString("Username", Username)
   writer.WriteElementString("DisplayName", DisplayName)
   writer.WriteElementString("RoleName", RoleName)
   writer.WriteEndElement()
  End Sub
#End Region

#Region " ToXml Methods "
  Public Function ToXml() As String
   Return ToXml("BlogPermission")
  End Function

  Public Function ToXml(elementName As String) As String
   Dim xml As New StringBuilder
   xml.Append("<")
   xml.Append(elementName)
   AddAttribute(xml, "AllowAccess", AllowAccess.ToString())
   AddAttribute(xml, "BlogId", BlogId.ToString())
   AddAttribute(xml, "Expires", Expires.ToString())
   AddAttribute(xml, "PermissionId", PermissionId.ToString())
   AddAttribute(xml, "RoleId", RoleId.ToString())
   AddAttribute(xml, "UserId", UserId.ToString())
   AddAttribute(xml, "Username", Username)
   AddAttribute(xml, "DisplayName", DisplayName)
   AddAttribute(xml, "RoleName", RoleName)
   AddAttribute(xml, "AllowAccess", AllowAccess.ToString())
   AddAttribute(xml, "BlogId", BlogId.ToString())
   AddAttribute(xml, "Expires", Expires.ToUniversalTime.ToString("u"))
   AddAttribute(xml, "PermissionId", PermissionId.ToString())
   AddAttribute(xml, "RoleId", RoleId.ToString())
   AddAttribute(xml, "UserId", UserId.ToString())
   AddAttribute(xml, "Username", Username)
   AddAttribute(xml, "DisplayName", DisplayName)
   AddAttribute(xml, "RoleName", RoleName)
   xml.Append(" />")
   Return xml.ToString
  End Function

  Private Sub AddAttribute(ByRef xml As StringBuilder, attributeName As String, attributeValue As String)
   xml.Append(" " & attributeName)
   xml.Append("=""" & attributeValue & """")
  End Sub
#End Region

#Region " JSON Serialization "
  Public Sub WriteJSON(ByRef s As Stream)
   Dim ser As New DataContractJsonSerializer(GetType(BlogPermissionInfo))
   ser.WriteObject(s, Me)
  End Sub
#End Region

 End Class
End Namespace


