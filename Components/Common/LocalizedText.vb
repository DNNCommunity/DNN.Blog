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

Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Localization.Localization
Imports System.Runtime.Serialization

Namespace Common
 <Serializable()> _
 Public Class LocalizedText
  Implements IXmlSerializable

#Region " Properties "
  Private _texts As New Dictionary(Of String, String)
  Public ReadOnly Property Locales As List(Of String)
   Get
    Dim res As New List(Of String)
    For Each k As String In _texts.Keys
     res.Add(k)
    Next
    Return res
   End Get
  End Property
  Default Public Property Item(ByVal key As String) As String
   Get
    If ContainsKey(key) Then
     Return _texts.Item(key)
    Else
     Return ""
    End If
   End Get
   Set(ByVal value As String)
    _texts.Item(key) = value
   End Set
  End Property
#End Region

#Region " Public Methods "
  Public Function GetDictionary() As Dictionary(Of String, String)
   Return _texts
  End Function
  Public Function ContainsKey(ByVal key As String) As Boolean
   Return _texts.ContainsKey(key)
  End Function
  Public Sub Add(ByVal key As String, ByVal value As String)
   _texts.Add(key, value)
  End Sub
  Public Function Remove(ByVal key As String) As Boolean
   Return _texts.Remove(key)
  End Function
#End Region

#Region " Constructors "
  Public Sub New()
   MyBase.New()
  End Sub

  Public Sub New(ByVal ir As IDataReader, ByVal FieldName As String)
   MyBase.New()
   Do While ir.Read
    If ir.Item(FieldName) IsNot DBNull.Value Then
     _texts.Add(CStr(ir.Item("Locale")), CStr(ir.Item(FieldName)))
    End If
   Loop
   ir.Close()
   ir.Dispose()
  End Sub
#End Region

#Region " (De)Serialization "
  Public Function ToJSONArray(ByVal DefaultLocale As String, ByVal DefaultText As String) As String
   Dim res As String = ""
   For Each localeCode As String In _texts.Keys
    res &= ", """ & localeCode.Replace("-", "_") & """: """
    res &= CStr(_texts.Item(localeCode))
    res &= """"
   Next
   res &= ", """ & DefaultLocale.Replace("-", "_") & """: """
   res &= DefaultText
   res &= """"
   Return res
  End Function

  Public Overrides Function ToString() As String
   Dim res As New StringBuilder
   Dim xw As XmlWriter = XmlTextWriter.Create(res)
   xw.WriteStartElement("MLText")
   For Each l As String In _texts.Keys
    xw.WriteStartElement("Text")
    xw.WriteAttributeString("Locale", l)
    xw.WriteString(_texts.Item(l))
    xw.WriteEndElement()
   Next
   xw.WriteEndElement()
   xw.Flush()
   Return res.ToString
  End Function

  Public Sub Deserialize(ByVal xml As String)
   Dim str As New IO.StringReader(xml)
   Dim xr As XmlReader = XmlTextReader.Create(str)
   xr.MoveToContent()
   Do While xr.ReadToFollowing("Text")
    If xr.MoveToAttribute("Locale") Then
     Dim l As String = xr.ReadContentAsString
     xr.MoveToContent()
     Dim s As String = xr.ReadElementContentAsString
     _texts.Add(l, s)
    End If
   Loop
  End Sub

  Public Sub FromXml(xml As XmlNode)
   If xml Is Nothing Then Exit Sub

  End Sub

  Public Function ToConcatenatedString() As String
   Dim res As New StringBuilder
   For Each l As String In _texts.Keys
    res.Append(_texts.Item(l))
    res.Append(" ")
   Next
   Return res.ToString
  End Function
#End Region

#Region " IXmlSerializable Implementation "
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

  Private Function readAttribute(ByVal reader As XmlReader, ByVal AttributeName As String) As String
   If reader.HasAttributes Then
    reader.MoveToAttribute(AttributeName)
    Return reader.Value
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
  Public Sub ReadXml(ByVal reader As XmlReader) Implements IXmlSerializable.ReadXml
   Do While reader.Name = "Text"
    reader.ReadStartElement("Text")
    Dim loc As String = readAttribute(reader, "Locale")
    If loc <> "" Then
     Dim txt As String = reader.ReadElementContentAsString
     Me.Add(loc, txt)
    End If
    reader.ReadEndElement() ' Text
   Loop
  End Sub

  Public Sub ReadXml(ByVal xMLText As XmlNode)
   If xMLText Is Nothing Then Exit Sub
   For Each xText As XmlNode In xMLText.ChildNodes
    Dim locale As String = xText.Attributes("Locale").InnerText
    _texts.Add(locale, xText.InnerText)
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
  Public Sub WriteXml(ByVal writer As XmlWriter) Implements IXmlSerializable.WriteXml
   For Each locale As String In _texts.Keys
    writer.WriteStartElement("Text")
    writer.WriteAttributeString("Locale", locale)
    writer.WriteCData(_texts.Item(locale))
    writer.WriteEndElement()
   Next
  End Sub
#End Region

 End Class
End Namespace