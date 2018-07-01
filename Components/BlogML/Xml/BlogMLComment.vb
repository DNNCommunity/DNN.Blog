Imports System.Xml.Serialization

Namespace BlogML.Xml
 <Serializable>
 Public NotInheritable Class BlogMLComment
  Inherits BlogMLNode
  Private m_userName As String
  Private m_userEmail As String
  Private m_userUrl As String
  Private m_content As New BlogMLContent

  <XmlAttribute("user-name")>
  Public Property UserName As String

  <XmlAttribute("user-url")>
  Public Property UserUrl As String

  <XmlAttribute("user-email")>
  Public Property UserEMail As String

  <XmlElement("content")>
  Public Property Content As BlogMLContent

 End Class
End Namespace
