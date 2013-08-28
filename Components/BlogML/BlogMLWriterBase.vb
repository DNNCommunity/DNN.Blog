Imports System.Xml
Imports System.Text.RegularExpressions
Imports System.IO
Imports DotNetNuke.Modules.Blog.BlogML.Xml

Namespace BlogML

 Public MustInherit Class BlogMLWriterBase
  Private Const BlogMLNamespace As String = "http://www.blogml.com/2006/09/BlogML"

  Protected Property Writer() As XmlWriter
   Get
    Return m_Writer
   End Get
   Private Set(value As XmlWriter)
    m_Writer = Value
   End Set
  End Property
  Private m_Writer As XmlWriter

  Public Sub Write(writer__1 As XmlWriter)
   Writer = writer__1

   'Write the XML delcaration. 
   writer__1.WriteStartDocument()

   Try
    InternalWriteBlog()
   Finally
    Writer = Nothing
   End Try
  End Sub

  Protected MustOverride Sub InternalWriteBlog()

#Region "WriteStartBlog"


  Protected Sub WriteStartBlog(title As String, subTitle As String, rootUrl As String)
   WriteStartBlog(title, ContentTypes.Text, subTitle, ContentTypes.Text, rootUrl, DateTime.Now)
  End Sub

  Protected Sub WriteStartBlog(title As String, subTitle As String, rootUrl As String, dateCreated As DateTime)
   WriteStartBlog(title, ContentTypes.Text, subTitle, ContentTypes.Text, rootUrl, dateCreated)
  End Sub

  Protected Sub WriteStartBlog(title As String, titleContentType As ContentTypes, subTitle As String, subTitleContentType As ContentTypes, rootUrl As String, dateCreated As DateTime)
   'WriteStartElement("blog");
   Writer.WriteStartElement("blog", BlogMLNamespace)
   ' fixes bug in Work Item 2004
   WriteAttributeStringRequired("root-url", rootUrl)
   WriteAttributeString("date-created", FormatDateTime(dateCreated))

   ' Write the default namespace, identified as xmlns with no prefix
   Writer.WriteAttributeString("xmlns", Nothing, Nothing, "http://www.blogml.com/2006/09/BlogML")
   Writer.WriteAttributeString("xmlns", "xs", Nothing, "http://www.w3.org/2001/XMLSchema")

   WriteContent("title", BlogMLContent.Create(title, titleContentType))
   WriteContent("sub-title", BlogMLContent.Create(subTitle, subTitleContentType))
  End Sub

#End Region

  Protected Sub WriteStartElement(tag As String)
   Writer.WriteStartElement(tag)
  End Sub


  Protected Sub WriteEndElement()
   Writer.WriteEndElement()
  End Sub


#Region "Extended Properties"

  Protected Sub WriteStartExtendedProperties()
   WriteStartElement("extended-properties")
  End Sub

  Protected Sub WriteExtendedProperty(name As String, value As String)
   WriteStartElement("property")
   WriteAttributeString("name", name)
   WriteAttributeString("value", value)
   WriteEndElement()
  End Sub

#End Region


  Protected Sub WriteAuthor(id As String, name As String, email As String, dateCreated As DateTime, dateModified As DateTime, approved As Boolean)
   WriteStartElement("author")
   WriteNodeAttributes(id, dateCreated, dateModified, approved)
   WriteAttributeString("email", email)
   WriteContent("title", BlogMLContent.Create(name, ContentTypes.Text))
   WriteEndElement()
  End Sub


  Protected Sub WriteCategory(id As String, title As String, dateCreated As DateTime, dateModified As DateTime, approved As Boolean, description As String, _
   parentRef As String)
   WriteCategory(id, title, ContentTypes.Text, dateCreated, dateModified, approved, _
    description, parentRef)
  End Sub


  Protected Sub WriteCategory(id As String, title As String, titleContentType As ContentTypes, dateCreated As DateTime, dateModified As DateTime, approved As Boolean, _
   description As String, parentRef As String)
   WriteStartElement("category")
   WriteNodeAttributes(id, dateCreated, dateModified, approved)
   WriteAttributeString("description", description)
   WriteAttributeString("parentref", parentRef)
   WriteContent("title", BlogMLContent.Create(title, titleContentType))
   WriteEndElement()
  End Sub

  Protected Sub WriteAuthorReference(refId As String)
   WriteStartElement("author")
   WriteAttributeStringRequired("ref", refId)
   WriteEndElement()
  End Sub

  Protected Sub WriteCategoryReference(refId As String)
   WriteStartElement("category")
   WriteAttributeStringRequired("ref", refId)
   WriteEndElement()
  End Sub

  Protected Sub WriteStartAuthors()
   WriteStartElement("authors")
  End Sub

  Protected Sub WriteStartCategories()
   WriteStartElement("categories")
  End Sub


  Protected Sub WriteStartPosts()
   WriteStartElement("posts")
  End Sub


  Protected Sub WriteStartTrackbacks()
   WriteStartElement("trackbacks")
  End Sub


  Protected Sub WriteStartAttachments()
   WriteStartElement("attachments")
  End Sub


  Protected Sub WriteNodeAttributes(id As String, dateCreated As DateTime, dateModified As DateTime, approved As Boolean)
   WriteAttributeString("id", id)
   WriteAttributeString("date-created", FormatDateTime(dateCreated))
   WriteAttributeString("date-modified", FormatDateTime(dateModified))
   WriteAttributeString("approved", If(approved, "true", "false"))
  End Sub


  Protected Function FormatDateTime([date] As DateTime) As String
   Return [date].ToUniversalTime().ToString("s")
  End Function

  Protected Sub WriteStartPost(id As String, title As String, dateCreated As DateTime, dateModified As DateTime, approved As Boolean, content As String, _
   postUrl As String)
   WriteStartPost(id, title, ContentTypes.Text, dateCreated, dateModified, approved, _
    content, ContentTypes.Text, postUrl, 0, False, Nothing, _
    ContentTypes.Text, BlogPostTypes.Normal, String.Empty)
  End Sub

  Protected Sub WriteStartPost(id As String, title As String, dateCreated As DateTime, dateModified As DateTime, approved As Boolean, content As String, _
   postUrl As String, views As UInt32, blogpostType As BlogPostTypes, postName As String)
   WriteStartPost(id, title, ContentTypes.Text, dateCreated, dateModified, approved, _
    content, ContentTypes.Text, postUrl, views, False, Nothing, _
    ContentTypes.Text, blogpostType, postName)
  End Sub

  Protected Sub WriteStartPost(id As String, title As String, dateCreated As DateTime, dateModified As DateTime, approved As Boolean, content As String, _
   postUrl As String, views As UInt32, excerpt As String, blogpostType As BlogPostTypes, postName As String)
   WriteStartPost(id, title, ContentTypes.Text, dateCreated, dateModified, approved, _
    content, ContentTypes.Text, postUrl, views, True, excerpt, _
    ContentTypes.Text, blogpostType, postName)
  End Sub

  Protected Sub WriteStartPost(id As String, title As BlogMLContent, dateCreated As DateTime, dateModified As DateTime, approved As Boolean, content As BlogMLContent, _
   postUrl As String, views As UInt32, hasexcerpt As Boolean, excerpt As BlogMLContent, blogpostType As BlogPostTypes, postName As String)
   WriteStartElement("post")
   WriteNodeAttributes(id, dateCreated, dateModified, approved)
   WriteAttributeString("post-url", postUrl)
   WriteAttributeStringRequired("type", blogpostType.ToString().ToLower())
   WriteAttributeStringRequired("hasexcerpt", hasexcerpt.ToString().ToLower())
   WriteAttributeStringRequired("views", views.ToString())
   WriteContent("title", title)
   WriteContent("content", content)
   If postName IsNot Nothing Then
    WriteContent("post-name", BlogMLContent.Create(postName, ContentTypes.Text))
   End If
   If hasexcerpt Then
    WriteContent("excerpt", excerpt)
   End If
  End Sub

  Protected Sub WriteStartPost(id As String, title As String, titleContentType As ContentTypes, dateCreated As DateTime, dateModified As DateTime, approved As Boolean, _
   content As String, postContentType As ContentTypes, postUrl As String, views As UInt32, hasexcerpt As Boolean, excerpt As String, _
   excerptContentType As ContentTypes, blogpostType As BlogPostTypes, postName As String)
   WriteStartElement("post")
   WriteNodeAttributes(id, dateCreated, dateModified, approved)
   WriteAttributeString("post-url", postUrl)
   WriteAttributeStringRequired("type", blogpostType.ToString().ToLower())
   WriteAttributeStringRequired("hasexcerpt", hasexcerpt.ToString().ToLower())
   WriteAttributeStringRequired("views", views.ToString())
   WriteContent("title", BlogMLContent.Create(title, titleContentType))
   WriteContent("content", BlogMLContent.Create(content, postContentType))
   If postName IsNot Nothing Then
    WriteContent("post-name", BlogMLContent.Create(postName, ContentTypes.Text))
   End If
   If hasexcerpt Then
    WriteContent("excerpt", BlogMLContent.Create(excerpt, excerptContentType))
   End If
  End Sub

  Protected Sub WriteStartComments()
   WriteStartElement("comments")
  End Sub


  Protected Sub WriteComment(id As String, title As String, dateCreated As DateTime, dateModified As DateTime, approved As Boolean, userName As String, _
   userEmail As String, userUrl As String, content As String)
   WriteComment(id, title, ContentTypes.Text, dateCreated, dateModified, approved, _
    userName, userEmail, userUrl, content, ContentTypes.Text)
  End Sub

  Protected Sub WriteComment(id As String, title As BlogMLContent, dateCreated As DateTime, dateModified As DateTime, approved As Boolean, userName As String, _
   userEmail As String, userUrl As String, content As BlogMLContent)
   WriteStartElement("comment")
   WriteNodeAttributes(id, dateCreated, dateModified, approved)
   WriteAttributeStringRequired("user-name", If(userName, ""))
   WriteAttributeString("user-url", If(userUrl, ""))
   WriteAttributeString("user-email", If(userEmail, ""))
   WriteContent("title", title)
   WriteContent("content", content)
   WriteEndElement()
  End Sub


  Protected Sub WriteComment(id As String, title As String, titleContentType As ContentTypes, dateCreated As DateTime, dateModified As DateTime, approved As Boolean, _
   userName As String, userEmail As String, userUrl As String, content As String, commentContentType As ContentTypes)
   WriteStartElement("comment")
   WriteNodeAttributes(id, dateCreated, dateModified, approved)
   WriteAttributeStringRequired("user-name", If(userName, ""))
   WriteAttributeString("user-url", If(userUrl, ""))
   WriteAttributeString("user-email", If(userEmail, ""))
   WriteContent("title", BlogMLContent.Create(title, titleContentType))
   WriteContent("content", BlogMLContent.Create(content, commentContentType))
   WriteEndElement()
  End Sub


  Protected Sub WriteTrackback(id As String, title As String, dateCreated As DateTime, dateModified As DateTime, approved As Boolean, url As String)
   WriteTrackback(id, title, ContentTypes.Text, dateCreated, dateModified, approved, _
    url)
  End Sub


  Protected Sub WriteTrackback(id As String, title As String, titleContentType As ContentTypes, dateCreated As DateTime, dateModified As DateTime, approved As Boolean, _
   url As String)
   WriteStartElement("trackback")
   WriteNodeAttributes(id, dateCreated, dateModified, approved)
   WriteAttributeStringRequired("url", url)
   WriteContent("title", BlogMLContent.Create(title, titleContentType))
   WriteEndElement()
  End Sub


  Protected Sub WriteAttributeStringRequired(name As String, value As String)
   If String.IsNullOrEmpty(value) Then
    Throw New ArgumentNullException("value", name)
   End If
   Writer.WriteAttributeString(name, value)
  End Sub


  Protected Sub WriteAttributeString(name As String, value As String)
   If Not String.IsNullOrEmpty(value) Then
    Writer.WriteAttributeString(name, value)
   End If
  End Sub

  Protected Sub WriteContent(elementName As String, content As BlogMLContent)
   WriteStartElement(elementName)
   Dim contentType As String = (If([Enum].GetName(GetType(ContentTypes), content.ContentType), "text")).ToLowerInvariant()
   WriteAttributeString("type", contentType)
   Writer.WriteCData(If(content.Text, String.Empty))
   WriteEndElement()
  End Sub

  Protected Sub WriteAttachment(externalUri As String, mimeType As String, fullUrl As String)
   WriteAttachment(fullUrl, 0, mimeType, externalUri, False, Nothing)
  End Sub

  Protected Sub WriteAttachment(embeddedUrl As String, mimeType As String, inputStream As Stream)
   Using reader As New BinaryReader(inputStream)
    reader.BaseStream.Position = 0
    Dim data As Byte() = reader.ReadBytes(CInt(inputStream.Length))
    WriteAttachment(embeddedUrl, data.Length, mimeType, Nothing, True, data)
   End Using
  End Sub

  Protected Sub WriteAttachment(embeddedUrl As String, size As Double, mimeType As String, externalUri As String, embedded As Boolean, data As Byte())
   WriteStartElement("attachment")

   Try

    WriteAttributeStringRequired("url", embeddedUrl)

    If size > 0 Then
     WriteAttributeStringRequired("size", size.ToString())
    End If

    If mimeType IsNot Nothing Then
     WriteAttributeStringRequired("mime-type", mimeType)
    End If

    If Not String.IsNullOrEmpty(externalUri) Then
     WriteAttributeStringRequired("external-uri", externalUri)
    End If

    WriteAttributeString("embedded", If(embedded, "true", "false"))

    If embedded Then
     Writer.WriteBase64(data, 0, data.Length)
    End If
   Finally
    WriteEndElement()
   End Try

  End Sub


  Friend Sub CopyStream(src As Stream, dst As Stream)
   Dim buf(4095) As Byte
   While True
    Dim bytesRead As Integer = src.Read(buf, 0, buf.Length)

    'Read returns 0 when reached end of stream.
    If bytesRead = 0 Then
     Exit While
    End If

    dst.Write(buf, 0, bytesRead)
   End While
  End Sub


  Public NotInheritable Class SgmlUtil
   Public Shared Function IsRootUrlOf(rootUrl As String, url As String) As Boolean

    If rootUrl Is Nothing Then
     Throw New ArgumentNullException("rootUrl")
    End If

    If url Is Nothing Then
     Throw New ArgumentNullException("url")
    End If

    rootUrl = rootUrl.Trim().ToLower()
    url = url.Trim().ToLower()
    ' is it a full path
    If url.StartsWith("http://") Then
     Return url.StartsWith(rootUrl)
    End If

    ' it's local
    Return True
   End Function

   Public Shared Function StripRootUrlPath(rootUrl As String, url As String) As String
    If url.StartsWith(rootUrl) Then
     url = url.Remove(0, rootUrl.Length)
    End If

    If url.StartsWith("/") Then
     url.TrimStart("/"c)
    End If

    Return url
   End Function

   Public Shared Function CleanAttachmentUrls(content As String, oldPath As String, newPath As String) As String
    oldPath = Regex.Escape(oldPath)

    content = Regex.Replace(content, oldPath, newPath, RegexOptions.CultureInvariant Or RegexOptions.IgnoreCase Or RegexOptions.Multiline Or RegexOptions.ExplicitCapture)
    Return content
   End Function

   Public Shared Function GetAttributeValues(content As String, tag As String, attribute As String) As String()
    Dim srcrx As Regex = CreateAttributeRegex(attribute)
    Dim matches As MatchCollection = CreateTagRegex(tag).Matches(content)
    Dim sources(matches.Count - 1) As String
    For i As Integer = 0 To sources.Length - 1
     Dim m As Match = srcrx.Match(matches(i).Value)
     sources(i) = m.Groups("Value").Value
    Next
    Return sources
   End Function

   Public Shared Function CreateTagRegex(name As String) As Regex

    Dim pattern As String = "<\s*{0}[^>]+>"

    pattern = String.Format(pattern, name)
    Return New Regex(pattern, RegexOptions.CultureInvariant Or RegexOptions.IgnoreCase Or RegexOptions.Multiline Or RegexOptions.ExplicitCapture)
   End Function

   Public Shared Function CreateAttributeRegex(name As String) As Regex
    Dim pattern As String = "{0}\s*=\s*['""]?\s*(?<Value>[^'"" ]+)"
    pattern = String.Format(pattern, name)
    Return New Regex(pattern, RegexOptions.CultureInvariant Or RegexOptions.IgnoreCase Or RegexOptions.Multiline Or RegexOptions.ExplicitCapture)
   End Function

  End Class

 End Class

End Namespace
