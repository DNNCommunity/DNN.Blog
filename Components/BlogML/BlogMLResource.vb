Imports System.IO
Imports System.Reflection
Imports System.Xml
Imports System.Xml.Schema

Namespace BlogML

 Public Class BlogMLResource
  ' TODO: Update to .NET 2.0
  Public Shared Function GetSchemaStream() As Stream
   Dim stream As Stream = GetType(BlogMLResource).Assembly.GetManifestResourceStream("BlogML.BlogML.xsd")
   If stream Is Nothing Then
    Throw New InvalidOperationException("Schema not found")
   End If
   Return stream
  End Function

  Public Shared Function GetSchema() As XmlSchema

   Dim schema As XmlSchema = XmlSchema.Read(GetSchemaStream(), New ValidationEventHandler(AddressOf ValidationEvent))
   Return schema

  End Function

  Private Shared Sub ValidationEvent(sender As [Object], e As ValidationEventArgs)
   Dim message As String = String.Format("Validation {0}: {1}", e.Severity, e.Message)
   Throw New InvalidOperationException(message)
  End Sub


  Public Shared Sub Validate(inputUri As String)
   Validate(inputUri, Nothing)
  End Sub


  Public Shared Sub Validate(inputUri As String, validationHandler As ValidationEventHandler)
   Dim reader As XmlTextReader = Nothing
   Try
    reader = New XmlTextReader(inputUri)
    Validate(reader, validationHandler)
   Finally
    If reader IsNot Nothing Then
     reader.Close()

    End If
   End Try
  End Sub

  Public Shared Sub Validate(reader As XmlTextReader)
   Validate(reader, Nothing)
  End Sub


  Public Shared Sub Validate(treader As XmlTextReader, validationHandler As ValidationEventHandler)
   Dim validator As XmlReaderSettings = Nothing
   Try
    validator = New XmlReaderSettings()
    Dim schema As XmlSchema = GetSchema()
    validator.Schemas.Add(schema)
    validator.ValidationType = ValidationType.Schema


    If validationHandler IsNot Nothing Then
     AddHandler validator.ValidationEventHandler, validationHandler
    Else
     AddHandler validator.ValidationEventHandler, New ValidationEventHandler(AddressOf ValidationEvent)
    End If

    Dim objXmlReader As XmlReader = XmlReader.Create(treader, validator)


    While objXmlReader.Read()
    End While
   Catch ex As Exception
    'Console.WriteLine(ex.ToString())
    Throw
   Finally
    If validationHandler IsNot Nothing Then
     RemoveHandler validator.ValidationEventHandler, validationHandler
    Else
     RemoveHandler validator.ValidationEventHandler, New ValidationEventHandler(AddressOf ValidationEvent)
    End If
   End Try
  End Sub
 End Class
End Namespace