Imports System.Xml.Serialization

Namespace BlogML.Xml
 <Serializable()> _
 Public NotInheritable Class BlogMLAuthor
  Inherits BlogMLNode
  Private m_email As String

  <XmlAttribute("email")> _
  Public Property Email() As String
   Get
    Return Me.m_email
   End Get
   Set(value As String)
    Me.m_email = value
   End Set
  End Property
 End Class
End Namespace
