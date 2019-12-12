Imports System.IO
Imports System.Xml.Serialization
Imports System.Xml

Namespace BlogML.Xml
 Public Class BlogMLSerializer
  Private Shared ReadOnly syncRoot As New Object
  Private Shared m_serializer As XmlSerializer

  Public Shared ReadOnly Property Serializer As XmlSerializer
   Get
    SyncLock syncRoot
     If m_serializer Is Nothing Then
      m_serializer = New XmlSerializer(GetType(BlogMLBlog))
     End If
     Return m_serializer
    End SyncLock
   End Get
  End Property

  Public Shared ReadOnly Property Namespaces As XmlSerializerNamespaces
   Get
    Dim ns As New XmlSerializerNamespaces()
    ns.Add("dnn", "http://dnn-connect.org/blog/")
    Return ns
   End Get
  End Property

  Public Shared Function Deserialize(stream As Stream) As BlogMLBlog
   Return TryCast(Serializer.Deserialize(stream), BlogMLBlog)
  End Function

  Public Shared Function Deserialize(reader As TextReader) As BlogMLBlog
   Return TryCast(Serializer.Deserialize(reader), BlogMLBlog)
  End Function

  Public Shared Function Deserialize(reader As XmlReader) As BlogMLBlog
   Return TryCast(Serializer.Deserialize(reader), BlogMLBlog)
  End Function

  Public Shared Sub Serialize(stream As Stream, blog As BlogMLBlog)
   Serializer.Serialize(stream, blog, Namespaces)
  End Sub

  Public Shared Sub Serialize(writer As TextWriter, blog As BlogMLBlog)
   Serializer.Serialize(writer, blog, Namespaces)
  End Sub

  Public Shared Sub Serialize(writer As XmlWriter, blog As BlogMLBlog)
   Serializer.Serialize(writer, blog, Namespaces)
  End Sub
 End Class
End Namespace
