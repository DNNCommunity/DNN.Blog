Imports System.Xml.Serialization

Namespace BlogML.Xml
 <Serializable>
 Public MustInherit Class BlogMLNode

  <XmlAttribute("id")>
  Public Property ID As String

  <XmlElement("title")>
  Public Property Title As String

  <XmlAttribute("date-created", DataType:="dateTime")>
  Public Property DateCreated As DateTime = Now

  <XmlAttribute("date-modified", DataType:="dateTime")>
  Public Property DateModified As DateTime = Now

  <XmlAttribute("approved")>
  Public Property Approved As Boolean = True

 End Class
End Namespace
