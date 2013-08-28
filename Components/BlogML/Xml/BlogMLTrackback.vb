Imports System.Xml.Serialization

Namespace BlogML.Xml
 <Serializable()> _
 Public NotInheritable Class BlogMLTrackback
  Inherits BlogMLNode
  Private m_url As String

  <XmlAttribute("url")> _
  Public Property Url() As String
   Get
    Return Me.m_url
   End Get
   Set(value As String)
    Me.m_url = value
   End Set
  End Property
 End Class
End Namespace
