Imports System.Xml.Serialization

Namespace BlogML.Xml
 <Serializable()> _
 Public NotInheritable Class BlogMLAuthorReference
  Private _ref As String

  <XmlAttribute("ref")> _
  Public Property Ref() As String
   Get
    Return Me._ref
   End Get
   Set(value As String)
    Me._ref = value
   End Set
  End Property
 End Class
End Namespace
