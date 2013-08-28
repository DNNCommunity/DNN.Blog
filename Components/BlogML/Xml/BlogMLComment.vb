Imports System.Xml.Serialization

Namespace BlogML.Xml
 <Serializable()> _
 Public NotInheritable Class BlogMLComment
  Inherits BlogMLNode
  Private m_userName As String
  Private m_userEmail As String
  Private m_userUrl As String
  Private m_content As New BlogMLContent()

  <XmlAttribute("user-name")> _
  Public Property UserName() As String
   Get
    Return Me.m_userName
   End Get
   Set(value As String)
    Me.m_userName = value
   End Set
  End Property

  <XmlAttribute("user-url")> _
  Public Property UserUrl() As String
   Get
    Return Me.m_userUrl
   End Get
   Set(value As String)
    Me.m_userUrl = value
   End Set
  End Property

  <XmlAttribute("user-email")> _
  Public Property UserEMail() As String
   Get
    Return Me.m_userEmail
   End Get
   Set(value As String)
    Me.m_userEmail = value
   End Set
  End Property

  <XmlElement("content")> _
  Public Property Content() As BlogMLContent
   Get
    Return Me.m_content
   End Get
   Set(value As BlogMLContent)
    Me.m_content = value
   End Set
  End Property
 End Class
End Namespace
