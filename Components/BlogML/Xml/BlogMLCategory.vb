Imports System.Xml.Serialization

Namespace BlogML.Xml
 <Serializable>
 Public NotInheritable Class BlogMLCategory
  Inherits BlogMLNode

  <XmlAttribute("description")>
  Public Property Description As String

  <XmlAttribute("parentref")>
  Public Property ParentRef As String

 End Class
End Namespace
