Imports System.Web
Imports System.Xml.Serialization

Namespace BlogML.Xml
 <Serializable()> _
 Public NotInheritable Class BlogMLContent
  Private ReadOnly Property Base64Encoded() As Boolean
   Get
    Return ContentType = ContentTypes.Base64
   End Get
  End Property

  Private ReadOnly Property HtmlEncoded() As Boolean
   Get
    Return ContentType = ContentTypes.Html OrElse ContentType = ContentTypes.Xhtml
   End Get
  End Property

  <XmlAttribute("type")> _
  Public Property ContentType() As ContentTypes
   Get
    Return m_ContentType
   End Get
   Set(value As ContentTypes)
    m_ContentType = Value
   End Set
  End Property
  Private m_ContentType As ContentTypes = ContentTypes.Text

  ' Encoded Text
  <XmlText()> _
  Public Property Text() As String
   Get
    Return m_Text
   End Get
   Set(value As String)
    m_Text = Value
   End Set
  End Property
  Private m_Text As String

  <XmlIgnore()> _
  Public ReadOnly Property UncodedText() As String
   Get
    If Base64Encoded Then
     Dim byteArray As Byte() = Convert.FromBase64String(Text)
     Return System.Text.Encoding.UTF8.GetString(byteArray)
    End If
    If HtmlEncoded Then
     Return HttpUtility.HtmlDecode(Text)
    End If
    Return Text
   End Get
  End Property

  Public Shared Function Create(unencodedText As String, contentType As ContentTypes) As BlogMLContent
   Dim content As New BlogMLContent() With {.ContentType = contentType}
   If content.Base64Encoded Then
    Dim byteArray As Byte() = System.Text.Encoding.UTF8.GetBytes(unencodedText)
    content.Text = Convert.ToBase64String(byteArray)
   ElseIf content.HtmlEncoded Then
    content.Text = HttpUtility.HtmlEncode(unencodedText)
   Else
    content.Text = unencodedText
   End If
   Return content
  End Function
 End Class
End Namespace
