Imports System.Xml.Serialization

Namespace BlogML.Xml
 <Serializable(), Obsolete("I don't think that we use this now that we are using Dictionary<K,V>")> _
 Public NotInheritable Class Meta
  Private m_type As String
  Private m_value As String

  <XmlAttribute("type")> _
  Public Property Type() As String
   Get
    Return Me.m_type
   End Get
   Set(value As String)
    Me.m_type = m_value
   End Set
  End Property

  <XmlAttribute("value")> _
  Public Property Value() As String
   Get
    Return Me.m_value
   End Get
   Set(value As String)
    Me.m_value = m_value
   End Set
  End Property
 End Class
End Namespace
