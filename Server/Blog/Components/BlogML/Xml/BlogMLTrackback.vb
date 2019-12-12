Imports System.Xml.Serialization

Namespace BlogML.Xml
 <Serializable>
 Public NotInheritable Class BlogMLTrackback
  Inherits BlogMLNode

  <XmlAttribute("url")>
  Public Property Url As String

 End Class
End Namespace
