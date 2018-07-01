Imports System.Xml.Serialization

Namespace BlogML.Xml
 <Serializable>
 Public NotInheritable Class BlogMLAttachment

  <XmlAttribute("embedded")>
  Public Property Embedded As Boolean = False

  <XmlAttribute("url")>
  Public Property Url As String

  <XmlAttribute("path")>
  Public Property Path As String

  <XmlAttribute("mime-type")>
  Public Property MimeType As String

  <XmlText>
  Public Property Data As Byte()

 End Class
End Namespace
