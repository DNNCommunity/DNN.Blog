Imports System.Xml.Serialization

Namespace BlogML.Xml
 <Serializable()> _
 Public NotInheritable Class BlogMLCategory
  Inherits BlogMLNode
  Private m_description As String
  Private m_parentRef As String

  <XmlAttribute("description")> _
  Public Property Description() As String
   Get
    Return Me.m_description
   End Get
   Set(value As String)
    Me.m_description = value
   End Set
  End Property

  <XmlAttribute("parentref")> _
  Public Property ParentRef() As String
   Get
    Return Me.m_parentRef
   End Get
   Set(value As String)
    Me.m_parentRef = value
   End Set
  End Property
 End Class
End Namespace
