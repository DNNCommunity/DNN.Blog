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

Imports System
Imports System.Data
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Tokens

Namespace Business
 ''' <summary>
 ''' Blog module's own implementation of a category.
 ''' </summary>
 ''' <remarks></remarks>
 ''' <history>
 '''		[pdonker]	12/14/2009	included DNN interfaces, most importantly IHydratable
 '''		[pdonker]	01/09/2010	included new column for SEO purposes
 ''' </history>
 <Serializable(), XmlRoot("Category")> _
 Public Class CategoryInfo
  Implements IHydratable
  Implements IPropertyAccess
  Implements IXmlSerializable
  Implements IComparable

#Region " Private Members "
  Private _CatId As Int32 = -1
  Private _Category As String = "New Category"
  Private _Cnt As Int32 = 0
  Private _FullCat As String = "New Category"
  Private _ParentId As Int32 = 0
  Private _PortalId As Int32 = -1
  Private _Slug As String = "Default.aspx"
#End Region

#Region " Constructors "
  Public Sub New()
  End Sub

  Public Sub New(ByVal CatId As Int32, ByVal Category As String, ByVal Cnt As Int32, ByVal FullCat As String, ByVal ParentId As Int32, ByVal PortalId As Int32, ByVal Slug As String)
   Me.Category = Category
   Me.CatId = CatId
   Me.Cnt = Cnt
   Me.FullCat = FullCat
   Me.ParentId = ParentId
   Me.PortalId = PortalId
   Me.Slug = Slug
  End Sub
#End Region

#Region " Public Properties "

  Public Property CatId() As Int32
   Get
    Return _CatId
   End Get
   Set(ByVal Value As Int32)
    _CatId = Value
   End Set
  End Property

  Public Property Category() As String
   Get
    Return _Category
   End Get
   Set(ByVal Value As String)
    _Category = Value
   End Set
  End Property

  Public Property Cnt() As Int32
   Get
    Return _Cnt
   End Get
   Set(ByVal Value As Int32)
    _Cnt = Value
   End Set
  End Property

  Public Property FullCat() As String
   Get
    Return _FullCat
   End Get
   Set(ByVal Value As String)
    _FullCat = Value
   End Set
  End Property

  Public Property ParentId() As Int32
   Get
    Return _ParentId
   End Get
   Set(ByVal Value As Int32)
    _ParentId = Value
   End Set
  End Property

  Public Property PortalId() As Int32
   Get
    Return _PortalId
   End Get
   Set(ByVal Value As Int32)
    _PortalId = Value
   End Set
  End Property

  Public Property Slug() As String
   Get
    Return _Slug
   End Get
   Set(ByVal Value As String)
    _Slug = Value
   End Set
  End Property

#End Region

#Region " IHydratable Implementation "
  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Fill hydrates the object from a Datareader
  ''' </summary>
  ''' <remarks>The Fill method is used by the CBO method to hydrtae the object
  ''' rather than using the more expensive Refection  methods.</remarks>
  ''' <history>
  ''' 	[]	12/13/2009  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub Fill(ByVal dr As IDataReader) Implements IHydratable.Fill

   CatId = Convert.ToInt32(Null.SetNull(dr.Item("CatId"), CatId))
   Category = Convert.ToString(Null.SetNull(dr.Item("Category"), Category))
   ParentId = Convert.ToInt32(Null.SetNull(dr.Item("ParentId"), ParentId))
   PortalId = Convert.ToInt32(Null.SetNull(dr.Item("PortalId"), PortalId))
   Slug = Convert.ToString(Null.SetNull(dr.Item("Slug"), Slug))
   Try
    Cnt = Convert.ToInt32(Null.SetNull(dr.Item("Cnt"), Cnt))
    FullCat = Convert.ToString(Null.SetNull(dr.Item("FullCat"), FullCat))
   Catch ex As Exception
   End Try

  End Sub
  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets and sets the Key ID
  ''' </summary>
  ''' <remarks>The KeyID property is part of the IHydratble interface.  It is used
  ''' as the key property when creating a Dictionary</remarks>
  ''' <history>
  ''' 	[]	12/13/2009  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Property KeyID() As Integer Implements IHydratable.KeyID
   Get
    Return CatId
   End Get
   Set(ByVal value As Integer)
    CatId = value
   End Set
  End Property
#End Region

#Region " IPropertyAccess Implementation "
  Public Function GetProperty(ByVal strPropertyName As String, ByVal strFormat As String, ByVal formatProvider As System.Globalization.CultureInfo, ByVal AccessingUser As DotNetNuke.Entities.Users.UserInfo, ByVal AccessLevel As DotNetNuke.Services.Tokens.Scope, ByRef PropertyNotFound As Boolean) As String Implements DotNetNuke.Services.Tokens.IPropertyAccess.GetProperty
   Dim OutputFormat As String = String.Empty
   Dim portalSettings As DotNetNuke.Entities.Portals.PortalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings()
   If strFormat = String.Empty Then
    OutputFormat = "D"
   Else
    OutputFormat = strFormat
   End If
   Select Case strPropertyName.ToLower
    Case "category"
     Return PropertyAccess.FormatString(Me.Category, strFormat)
    Case "catid"
     Return (Me.CatId.ToString(OutputFormat, formatProvider))
    Case "cnt"
     Return (Me.Cnt.ToString(OutputFormat, formatProvider))
    Case "fullcat"
     Return PropertyAccess.FormatString(Me.FullCat, strFormat)
    Case "parentid"
     Return (Me.ParentId.ToString(OutputFormat, formatProvider))
    Case "portalid"
     Return (Me.PortalId.ToString(OutputFormat, formatProvider))
    Case "slug"
     Return PropertyAccess.FormatString(Me.Slug, strFormat)
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
  ''' 	[]	12/13/2009  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Function GetSchema() As XmlSchema Implements IXmlSerializable.GetSchema
   Return Nothing
  End Function

  Private Function readElement(ByVal reader As XmlReader, ByVal ElementName As String) As String
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
  ''' 	[]	12/13/2009  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub ReadXml(ByVal reader As XmlReader) Implements IXmlSerializable.ReadXml
   Try

    Category = readElement(reader, "Category")
    If Not Int32.TryParse(readElement(reader, "Cnt"), Cnt) Then
     Cnt = Null.NullInteger
    End If
    FullCat = readElement(reader, "FullCat")
    If Not Int32.TryParse(readElement(reader, "ParentId"), ParentId) Then
     ParentId = Null.NullInteger
    End If
    If Not Int32.TryParse(readElement(reader, "PortalId"), PortalId) Then
     PortalId = Null.NullInteger
    End If
    Slug = readElement(reader, "Slug")
   Catch ex As Exception
    ' log exception as DNN import routine does not do that
    DotNetNuke.Services.Exceptions.LogException(ex)
    ' re-raise exception to make sure import routine displays a visible error to the user
    Throw New Exception("An error occured during import of an Category", ex)
   End Try

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' WriteXml converts the object to Xml (serializes it) and writes it using the XmlWriter passed
  ''' </summary>
  ''' <remarks></remarks>
  ''' <param name="writer">The XmlWriter that contains the xml for the object</param>
  ''' <history>
  ''' 	[]	12/13/2009  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub WriteXml(ByVal writer As XmlWriter) Implements IXmlSerializable.WriteXml
   writer.WriteStartElement("Category")
   writer.WriteElementString("CatId", CatId.ToString())
   writer.WriteElementString("Category", Category)
   writer.WriteElementString("Cnt", Cnt.ToString())
   writer.WriteElementString("FullCat", FullCat)
   writer.WriteElementString("ParentId", ParentId.ToString())
   writer.WriteElementString("PortalId", PortalId.ToString())
   writer.WriteElementString("Slug", Slug)
   writer.WriteEndElement()
  End Sub
#End Region

#Region " IComparable Implementation "
  Public Function CompareTo(ByVal obj As Object) As Integer Implements System.IComparable.CompareTo
   If Not TypeOf obj Is CategoryInfo Then
    Throw New Exception("Object is not CategoryInfo")
   End If
   Dim Compare As CategoryInfo = CType(obj, CategoryInfo)
   Dim result As Integer = Me.FullCat.CompareTo(Compare.FullCat)

   If result = 0 Then
    result = Me.FullCat.CompareTo(Compare.FullCat)
   End If

   Return result
  End Function
#End Region

 End Class
End Namespace
