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

Imports System
Imports System.Data
Imports System.IO
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json
Imports System.Text
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Tokens

Imports DotNetNuke.Modules.Blog.Common.Globals

Namespace Entities.Posts

 <Serializable(), XmlRoot("Post"), DataContract()>
 Partial Public Class PostInfo
  Implements IHydratable
  Implements IPropertyAccess
  Implements IXmlSerializable

#Region " ML Properties "
  Public Property ParentTabID As Integer = -1
#End Region

#Region " IHydratable Implementation "
  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Fill hydrates the object from a Datareader
  ''' </summary>
  ''' <remarks>The Fill method is used by the CBO method to hydrtae the object
  ''' rather than using the more expensive Refection  methods.</remarks>
  ''' <history>
  ''' 	[pdonker]	02/19/2013  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Overrides Sub Fill(dr As IDataReader) Implements IHydratable.Fill
   FillInternal(dr)
   BlogID = Convert.ToInt32(Null.SetNull(dr.Item("BlogID"), BlogID))
   Title = Convert.ToString(Null.SetNull(dr.Item("Title"), Title))
   Summary = Convert.ToString(Null.SetNull(dr.Item("Summary"), Summary))
   Image = Convert.ToString(Null.SetNull(dr.Item("Image"), Image))
   Published = Convert.ToBoolean(Null.SetNull(dr.Item("Published"), Published))
   PublishedOnDate = CDate(Null.SetNull(dr.Item("PublishedOnDate"), PublishedOnDate))
   AllowComments = Convert.ToBoolean(Null.SetNull(dr.Item("AllowComments"), AllowComments))
   DisplayCopyright = Convert.ToBoolean(Null.SetNull(dr.Item("DisplayCopyright"), DisplayCopyright))
   Copyright = Convert.ToString(Null.SetNull(dr.Item("Copyright"), Copyright))
   Locale = Convert.ToString(Null.SetNull(dr.Item("Locale"), Locale))
   ViewCount = Convert.ToInt32(Null.SetNull(dr.Item("ViewCount"), ViewCount))
   Username = Convert.ToString(Null.SetNull(dr.Item("Username"), Username))
   Email = Convert.ToString(Null.SetNull(dr.Item("Email"), Email))
   DisplayName = Convert.ToString(Null.SetNull(dr.Item("DisplayName"), DisplayName))
   AltLocale = Convert.ToString(Null.SetNull(dr.Item("AltLocale"), AltLocale))
   AltTitle = Convert.ToString(Null.SetNull(dr.Item("AltTitle"), AltTitle))
   AltSummary = Convert.ToString(Null.SetNull(dr.Item("AltSummary"), AltSummary))
   AltContent = Convert.ToString(Null.SetNull(dr.Item("AltContent"), AltContent))
   NrComments = Convert.ToInt32(Null.SetNull(dr.Item("NrComments"), NrComments))
   PublishedOnDate = Date.SpecifyKind(PublishedOnDate, DateTimeKind.Utc)
  End Sub
  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets and sets the Key ID
  ''' </summary>
  ''' <remarks>The KeyID property is part of the IHydratble interface.  It is used
  ''' as the key property when creating a Dictionary</remarks>
  ''' <history>
  ''' 	[pdonker]	02/19/2013  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Overrides Property KeyID() As Integer Implements IHydratable.KeyID
   Get
    Return ContentItemId
   End Get
   Set(value As Integer)
    ContentItemId = value
   End Set
  End Property
#End Region

#Region " IPropertyAccess Implementation "
  Public Function GetProperty(strPropertyName As String, strFormat As String, formatProvider As System.Globalization.CultureInfo, AccessingUser As DotNetNuke.Entities.Users.UserInfo, AccessLevel As DotNetNuke.Services.Tokens.Scope, ByRef PropertyNotFound As Boolean) As String Implements DotNetNuke.Services.Tokens.IPropertyAccess.GetProperty
   Dim OutputFormat As String = String.Empty
   Dim portalSettings As DotNetNuke.Entities.Portals.PortalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings()
   If strFormat = String.Empty Then
    OutputFormat = "D"
   Else
    OutputFormat = strFormat
   End If
   Select Case strPropertyName.ToLower
    Case "blogid"
     Return (Me.BlogID.ToString(OutputFormat, formatProvider))
    Case "title"
     Return PropertyAccess.FormatString(Me.Title, strFormat)
    Case "summary"
     Return Me.Summary.OutputHtml(strFormat)
    Case "image"
     Return PropertyAccess.FormatString(Me.Image, strFormat)
    Case "published"
     Return Me.Published.ToString
    Case "publishedyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.Published, formatProvider)
    Case "publishedondate"
     Return UtcToLocalTime(PublishedOnDate).ToString(OutputFormat, formatProvider)
    Case "allowcomments"
     Return Me.AllowComments.ToString
    Case "allowcommentsyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.AllowComments, formatProvider)
    Case "displaycopyright"
     Return Me.DisplayCopyright.ToString
    Case "displaycopyrightyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.DisplayCopyright, formatProvider)
    Case "copyright"
     Return PropertyAccess.FormatString(Me.Copyright, strFormat)
    Case "locale"
     Return PropertyAccess.FormatString(Me.Locale, strFormat)
    Case "nrcomments"
     Return (Me.NrComments.ToString(OutputFormat, formatProvider))
    Case "viewcount"
     Return (Me.ViewCount.ToString(OutputFormat, formatProvider))
    Case "username"
     Return PropertyAccess.FormatString(Me.Username, strFormat)
    Case "email"
     Return PropertyAccess.FormatString(Me.Email, strFormat)
    Case "displayname"
     Return PropertyAccess.FormatString(Me.DisplayName, strFormat)
    Case "altlocale"
     Return PropertyAccess.FormatString(Me.AltLocale, strFormat)
    Case "alttitle"
     Return PropertyAccess.FormatString(Me.AltTitle, strFormat)
    Case "altsummary"
     Return PropertyAccess.FormatString(Me.AltSummary, strFormat)
    Case "altcontent"
     Return PropertyAccess.FormatString(Me.AltContent, strFormat)
    Case "localizedtitle"
     Return PropertyAccess.FormatString(Me.LocalizedTitle, strFormat)
    Case "localizedsummary"
     Return Me.LocalizedSummary.OutputHtml(strFormat)
    Case "localizedcontent"
     Return Me.LocalizedContent.OutputHtml(strFormat)
    Case "contentitemid"
     Return (Me.ContentItemId.ToString(OutputFormat, formatProvider))
    Case "createdbyuserid"
     Return (Me.CreatedByUserID.ToString(OutputFormat, formatProvider))
    Case "createdondate"
     Return (Me.CreatedOnDate.ToString(OutputFormat, formatProvider))
    Case "lastmodifiedbyuserid"
     Return (Me.LastModifiedByUserID.ToString(OutputFormat, formatProvider))
    Case "lastmodifiedondate"
     Return (Me.LastModifiedOnDate.ToString(OutputFormat, formatProvider))
    Case "content", "contents"
     Return Me.Content.OutputHtml(strFormat)
    Case "hasimage"
     Return CBool(Me.Image <> "").ToString(formatProvider)
    Case "link", "permalink"
     Return PermaLink(DotNetNuke.Entities.Portals.PortalSettings.Current)
    Case "parenturl"
     Return PermaLink(ParentTabID)
    Case Else
     PropertyNotFound = True
   End Select

   Return Null.NullString
  End Function

  Public ReadOnly Property Cacheability() As DotNetNuke.Services.Tokens.CacheLevel Implements DotNetNuke.Services.Tokens.IPropertyAccess.Cacheability
   Get
    Return CacheLevel.fullyCacheable
   End Get
  End Property
#End Region

#Region " IXmlSerializable Implementation "
  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' GetSchema returns the XmlSchema for this class
  ''' </summary>
  ''' <remarks>GetSchema is implemented as a stub method as it is not required</remarks>
  ''' <history>
  ''' 	[pdonker]	05/03/2013  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Function GetSchema() As XmlSchema Implements IXmlSerializable.GetSchema
   Return Nothing
  End Function

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' ReadXml fills the object (de-serializes it) from the XmlReader passed
  ''' </summary>
  ''' <remarks></remarks>
  ''' <param name="reader">The XmlReader that contains the xml for the object</param>
  ''' <history>
  ''' 	[pdonker]	05/03/2013  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub ReadXml(reader As XmlReader) Implements IXmlSerializable.ReadXml
   ' not implemented
  End Sub

  Friend Property ImportedPostId As Integer = -1
  Friend Property ImportedFiles As List(Of String)
  Friend Property ImportedTags As List(Of String)
  Friend Property ImportedCategories As List(Of String)

  Public Sub FromXml(xml As XmlNode)
   If xml Is Nothing Then Exit Sub

   xml.ReadValue("PostId", ImportedPostId)
   xml.ReadValue("Title", Title)
   xml.ReadValue("TitleLocalizations", TitleLocalizations)
   xml.ReadValue("Summary", Summary)
   xml.ReadValue("SummaryLocalizations", SummaryLocalizations)
   xml.ReadValue("Content", Content)
   xml.ReadValue("ContentLocalizations", ContentLocalizations)
   xml.ReadValue("Image", Image)
   xml.ReadValue("Published", Published)
   xml.ReadValue("PublishedOnDate", PublishedOnDate)
   xml.ReadValue("AllowComments", AllowComments)
   xml.ReadValue("DisplayCopyright", DisplayCopyright)
   xml.ReadValue("Copyright", Copyright)
   xml.ReadValue("Locale", Locale)
   xml.ReadValue("Username", Username)
   xml.ReadValue("Email", Email)

   ImportedFiles = New List(Of String)
   For Each xFile As XmlNode In xml.SelectNodes("Files/File")
    ImportedFiles.Add(xFile.InnerText)
   Next
   ImportedTags = New List(Of String)
   For Each xTag As XmlNode In xml.SelectNodes("Tag")
    ImportedTags.Add(xTag.InnerText)
   Next
   ImportedCategories = New List(Of String)
   For Each xCategory As XmlNode In xml.SelectNodes("Category")
    ImportedCategories.Add(xCategory.InnerText)
   Next

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' WriteXml converts the object to Xml (serializes it) and writes it using the XmlWriter passed
  ''' </summary>
  ''' <remarks></remarks>
  ''' <param name="writer">The XmlWriter that contains the xml for the object</param>
  ''' <history>
  ''' 	[pdonker]	05/03/2013  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub WriteXml(writer As XmlWriter) Implements IXmlSerializable.WriteXml
   writer.WriteStartElement("Post")
   writer.WriteElementString("PostId", ContentItemId.ToString)
   writer.WriteElementString("Title", Title)
   writer.WriteElementString("TitleLocalizations", TitleLocalizations.ToString)
   writer.WriteElementString("Summary", Summary)
   writer.WriteElementString("SummaryLocalizations", SummaryLocalizations.ToString)
   writer.WriteElementString("Content", Content)
   writer.WriteElementString("ContentLocalizations", ContentLocalizations.ToString)
   writer.WriteElementString("Image", Image)
   writer.WriteElementString("Published", Published.ToString())
   writer.WriteElementString("PublishedOnDate", PublishedOnDate.ToString())
   writer.WriteElementString("AllowComments", AllowComments.ToString())
   writer.WriteElementString("DisplayCopyright", DisplayCopyright.ToString())
   writer.WriteElementString("Copyright", Copyright)
   writer.WriteElementString("Locale", Locale)
   writer.WriteElementString("Username", Username)
   writer.WriteElementString("Email", Email)
   writer.WriteStartElement("Files")
   ' pack files
   Dim postDir As String = GetPostDirectoryMapPath(BlogID, ContentItemId)
   Dim newSummary As String = Summary
   Dim newSummaryLocalized As LocalizedText = SummaryLocalizations
   Dim newContent As String = Content
   Dim newContentLocalized As LocalizedText = ContentLocalizations
   If IO.Directory.Exists(postDir) Then
    For Each f As String In IO.Directory.GetFiles(postDir)
     Dim fileName As String = IO.Path.GetFileName(f)
     writer.WriteElementString("File", fileName)
    Next
   End If
   writer.WriteEndElement() ' Files
   For Each t As Entities.Terms.TermInfo In Me.PostTags
    writer.WriteElementString("Tag", t.Name)
   Next
   For Each t As Entities.Terms.TermInfo In Me.PostCategories
    writer.WriteElementString("Category", t.Name)
   Next
   writer.WriteEndElement() ' Post
  End Sub
#End Region

#Region " ToXml Methods "
  Public Function ToXml() As String
   Return ToXml("Post")
  End Function

  Public Function ToXml(elementName As String) As String
   Dim xml As New StringBuilder
   xml.Append("<")
   xml.Append(elementName)
   AddAttribute(xml, "BlogID", BlogID.ToString())
   AddAttribute(xml, "Title", Title)
   AddAttribute(xml, "Summary", Summary)
   AddAttribute(xml, "Image", Image)
   AddAttribute(xml, "Published", Published.ToString())
   AddAttribute(xml, "PublishedOnDate", PublishedOnDate.ToUniversalTime.ToString("u"))
   AddAttribute(xml, "AllowComments", AllowComments.ToString())
   AddAttribute(xml, "DisplayCopyright", DisplayCopyright.ToString())
   AddAttribute(xml, "Copyright", Copyright)
   AddAttribute(xml, "Locale", Locale)
   AddAttribute(xml, "ViewCount", ViewCount.ToString())
   AddAttribute(xml, "Username", Username)
   AddAttribute(xml, "Email", Email)
   AddAttribute(xml, "DisplayName", DisplayName)
   xml.Append(" />")
   Return xml.ToString
  End Function

  Private Sub AddAttribute(ByRef xml As StringBuilder, attributeName As String, attributeValue As String)
   xml.Append(" " & attributeName)
   xml.Append("=""" & attributeValue & """")
  End Sub
#End Region

#Region " JSON Serialization "
  Public Sub WriteJSON(ByRef s As Stream)
   Dim ser As New DataContractJsonSerializer(GetType(PostInfo))
   ser.WriteObject(s, Me)
  End Sub
#End Region

 End Class
End Namespace


