Imports System.Xml.Serialization

Namespace BlogML.Xml
 <Serializable, Obsolete("I don't think that we use this now that we are using Dictionary<K,V>")>
 Public NotInheritable Class Meta

  <XmlAttribute("type")>
  Public Property Type As String

  <XmlAttribute("value")>
  Public Property Value As String

 End Class
End Namespace
