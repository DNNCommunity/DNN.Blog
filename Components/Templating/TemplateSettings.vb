'
' DNN Connect - http://dnn-connect.org
' Copyright (c) 2015
' by DNN Connect
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'

Imports System.Xml.Serialization

Namespace Templating

 <Serializable()> _
 <XmlRoot("templateSettings")> _
 Public Class TemplateSettings

  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists"), XmlElement("templateSetting")> _
  Public Property Settings() As New List(Of TemplateSetting)

  Private _settingsFile As String = ""
  Public Sub New(templateMapPath As String)
   MyBase.New()
   _settingsFile = templateMapPath & "settings.xml"
   Dim x As New System.Xml.Serialization.XmlSerializer(GetType(TemplateSettings))
   If IO.File.Exists(_settingsFile) Then
    Using rdr As New IO.StreamReader(_settingsFile)
     Dim a As TemplateSettings = CType(x.Deserialize(rdr), TemplateSettings)
     Settings = a.Settings
    End Using
   End If
  End Sub
  Public Sub New()
  End Sub

 End Class
End Namespace
