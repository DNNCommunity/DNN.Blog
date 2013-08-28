Imports System.Xml.Serialization

Namespace BlogML.Xml
 <Serializable()> _
 Public MustInherit Class BlogMLNode
  Private m_id As String
  Private m_title As String
  Private m_dateCreated As DateTime = DateTime.Now
  Private m_dateModified As DateTime = DateTime.Now
  Private m_approved As Boolean = True

  <XmlAttribute("id")> _
  Public Property ID() As String
   Get
    Return Me.m_id
   End Get
   Set(value As String)
    Me.m_id = value
   End Set
  End Property

  <XmlElement("title")> _
  Public Property Title() As String
   Get
    Return Me.m_title
   End Get
   Set(value As String)
    Me.m_title = value
   End Set
  End Property

  <XmlAttribute("date-created", DataType:="dateTime")> _
  Public Property DateCreated() As DateTime
   Get
    Return Me.m_dateCreated
   End Get
   Set(value As DateTime)
    Me.m_dateCreated = value
   End Set
  End Property

  <XmlAttribute("date-modified", DataType:="dateTime")> _
  Public Property DateModified() As DateTime
   Get
    Return Me.m_dateModified
   End Get
   Set(value As DateTime)
    Me.m_dateModified = value
   End Set
  End Property

  <XmlAttribute("approved")> _
  Public Property Approved() As Boolean
   Get
    Return Me.m_approved
   End Get
   Set(value As Boolean)
    Me.m_approved = value
   End Set
  End Property
 End Class
End Namespace
