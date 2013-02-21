Imports System.Xml.Serialization

Namespace BlogML.Xml
 <Serializable()> _
 Public NotInheritable Class BlogMLAttachment
  Private m_embedded As Boolean = False
  Private m_url As String
  Private m_data As Byte()
  Private m_path As String
  Private m_mimeType As String

  <XmlAttribute("embedded")> _
  Public Property Embedded() As Boolean
   Get
    Return Me.m_embedded
   End Get
   Set(value As Boolean)
    Me.m_embedded = value
   End Set
  End Property

  <XmlAttribute("url")> _
  Public Property Url() As String
   Get
    Return Me.m_url
   End Get
   Set(value As String)
    Me.m_url = value
   End Set
  End Property

  <XmlAttribute("path")> _
  Public Property Path() As String
   Get
    Return Me.m_path
   End Get
   Set(value As String)
    Me.m_path = value
   End Set
  End Property

  <XmlAttribute("mime-type")> _
  Public Property MimeType() As String
   Get
    Return Me.m_mimeType
   End Get
   Set(value As String)
    Me.m_mimeType = value
   End Set
  End Property

  <XmlText()> _
  Public Property Data() As Byte()
   Get
    Return Me.m_data
   End Get
   Set(value As Byte())
    Me.m_data = value
   End Set
  End Property
 End Class
End Namespace
