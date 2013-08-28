Imports System.Xml.Serialization

Namespace BlogML
 Public Enum BlogPostTypes As Short
  <System.Xml.Serialization.XmlEnumAttribute("normal")> _
  Normal = 1
  <System.Xml.Serialization.XmlEnumAttribute("article")> _
  Article = 2
 End Enum
End Namespace
