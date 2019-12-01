Imports System.Xml.Serialization

Namespace BlogML
 Public Enum ContentTypes As Short
  <XmlEnumAttribute("html")> _
  Html = 1
  <XmlEnumAttribute("xhtml")> _
  Xhtml = 2
  <XmlEnumAttribute("text")> _
  Text = 3
  <XmlEnumAttribute("base64")> _
  Base64 = 4
 End Enum
End Namespace