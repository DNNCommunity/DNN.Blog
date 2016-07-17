Imports System.Xml.Serialization

Namespace BlogML.Xml
 <Serializable>
 Public NotInheritable Class BlogMLAuthor
  Inherits BlogMLNode

  <XmlAttribute("email")>
  Public Property Email As String

 End Class
End Namespace
