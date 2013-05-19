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

Namespace Entities.Blogs

 <Serializable(), XmlRoot("Blog"), DataContract()>
 Partial Public Class BlogInfo
  Implements IHydratable
  Implements IPropertyAccess
  Implements IXmlSerializable

#Region " IHydratable Implementation "
  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Fill hydrates the object from a Datareader
  ''' </summary>
  ''' <remarks>The Fill method is used by the CBO method to hydrtae the object
  ''' rather than using the more expensive Refection  methods.</remarks>
  ''' <history>
  ''' 	[pdonker]	02/16/2013  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub Fill(dr As IDataReader) Implements IHydratable.Fill
   BlogID = Convert.ToInt32(Null.SetNull(dr.Item("BlogID"), BlogID))
   ModuleID = Convert.ToInt32(Null.SetNull(dr.Item("ModuleID"), ModuleID))
   Title = Convert.ToString(Null.SetNull(dr.Item("Title"), Title))
   Description = Convert.ToString(Null.SetNull(dr.Item("Description"), Description))
   Image = Convert.ToString(Null.SetNull(dr.Item("Image"), Image))
   Locale = Convert.ToString(Null.SetNull(dr.Item("Locale"), Locale))
   FullLocalization = Convert.ToBoolean(Null.SetNull(dr.Item("FullLocalization"), FullLocalization))
   Published = Convert.ToBoolean(Null.SetNull(dr.Item("Published"), Published))
   IncludeImagesInFeed = Convert.ToBoolean(Null.SetNull(dr.Item("IncludeImagesInFeed"), IncludeImagesInFeed))
   IncludeAuthorInFeed = Convert.ToBoolean(Null.SetNull(dr.Item("IncludeAuthorInFeed"), IncludeAuthorInFeed))
   Syndicated = Convert.ToBoolean(Null.SetNull(dr.Item("Syndicated"), Syndicated))
   SyndicationEmail = Convert.ToString(Null.SetNull(dr.Item("SyndicationEmail"), SyndicationEmail))
   Copyright = Convert.ToString(Null.SetNull(dr.Item("Copyright"), Copyright))
   MustApproveGhostPosts = Convert.ToBoolean(Null.SetNull(dr.Item("MustApproveGhostPosts"), MustApproveGhostPosts))
   PublishAsOwner = Convert.ToBoolean(Null.SetNull(dr.Item("PublishAsOwner"), PublishAsOwner))
   EnablePingBackSend = Convert.ToBoolean(Null.SetNull(dr.Item("EnablePingBackSend"), EnablePingBackSend))
   EnablePingBackReceive = Convert.ToBoolean(Null.SetNull(dr.Item("EnablePingBackReceive"), EnablePingBackReceive))
   AutoApprovePingBack = Convert.ToBoolean(Null.SetNull(dr.Item("AutoApprovePingBack"), AutoApprovePingBack))
   EnableTrackBackSend = Convert.ToBoolean(Null.SetNull(dr.Item("EnableTrackBackSend"), EnableTrackBackSend))
   EnableTrackBackReceive = Convert.ToBoolean(Null.SetNull(dr.Item("EnableTrackBackReceive"), EnableTrackBackReceive))
   AutoApproveTrackBack = Convert.ToBoolean(Null.SetNull(dr.Item("AutoApproveTrackBack"), AutoApproveTrackBack))
   OwnerUserId = Convert.ToInt32(Null.SetNull(dr.Item("OwnerUserId"), OwnerUserId))
   CreatedByUserID = Convert.ToInt32(Null.SetNull(dr.Item("CreatedByUserID"), CreatedByUserID))
   CreatedOnDate = CDate(Null.SetNull(dr.Item("CreatedOnDate"), CreatedOnDate))
   LastModifiedByUserID = Convert.ToInt32(Null.SetNull(dr.Item("LastModifiedByUserID"), LastModifiedByUserID))
   LastModifiedOnDate = CDate(Null.SetNull(dr.Item("LastModifiedOnDate"), LastModifiedOnDate))
   DisplayName = Convert.ToString(Null.SetNull(dr.Item("DisplayName"), DisplayName))
   Email = Convert.ToString(Null.SetNull(dr.Item("Email"), Email))
   Username = Convert.ToString(Null.SetNull(dr.Item("Username"), Username))
   NrPosts = Convert.ToInt32(Null.SetNull(dr.Item("NrPosts"), NrPosts))
   LastPublishDate = CDate(Null.SetNull(dr.Item("LastPublishDate"), LastPublishDate))
   NrViews = Convert.ToInt32(Null.SetNull(dr.Item("NrViews"), NrViews))
   FirstPublishDate = CDate(Null.SetNull(dr.Item("FirstPublishDate"), FirstPublishDate))
   AltLocale = Convert.ToString(Null.SetNull(dr.Item("AltLocale"), AltLocale))
   AltTitle = Convert.ToString(Null.SetNull(dr.Item("AltTitle"), AltTitle))
   AltDescription = Convert.ToString(Null.SetNull(dr.Item("AltDescription"), AltDescription))
   CanEdit = Convert.ToBoolean(Null.SetNull(dr.Item("CanEdit"), CanEdit))
   CanAdd = Convert.ToBoolean(Null.SetNull(dr.Item("CanAdd"), CanAdd))
   CanApprove = Convert.ToBoolean(Null.SetNull(dr.Item("CanApprove"), CanApprove))

  End Sub
  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets and sets the Key ID
  ''' </summary>
  ''' <remarks>The KeyID property is part of the IHydratble interface.  It is used
  ''' as the key property when creating a Dictionary</remarks>
  ''' <history>
  ''' 	[pdonker]	02/16/2013  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Property KeyID() As Integer Implements IHydratable.KeyID
   Get
    Return BlogID
   End Get
   Set(value As Integer)
    BlogID = value
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
    Case "moduleid"
     Return (Me.ModuleID.ToString(OutputFormat, formatProvider))
    Case "title"
     Return PropertyAccess.FormatString(Me.Title, strFormat)
    Case "description"
     Return PropertyAccess.FormatString(Me.Description, strFormat)
    Case "image"
     Return PropertyAccess.FormatString(Me.Image, strFormat)
    Case "hasimage"
     Return CBool(Me.Image <> "").ToString(formatProvider)
    Case "locale"
     Return PropertyAccess.FormatString(Me.Locale, strFormat)
    Case "fulllocalization"
     Return Me.FullLocalization.ToString
    Case "fulllocalizationyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.FullLocalization, formatProvider)
    Case "published"
     Return Me.Published.ToString
    Case "publishedyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.Published, formatProvider)
    Case "includeimagesinfeed"
     Return Me.IncludeImagesInFeed.ToString
    Case "includeimagesinfeedyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.IncludeImagesInFeed, formatProvider)
    Case "includeauthorinfeed"
     Return Me.IncludeAuthorInFeed.ToString
    Case "includeauthorinfeedyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.IncludeAuthorInFeed, formatProvider)
    Case "syndicated"
     Return Me.Syndicated.ToString
    Case "syndicatedyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.Syndicated, formatProvider)
    Case "syndicationemail"
     Return PropertyAccess.FormatString(Me.SyndicationEmail, strFormat)
    Case "copyright"
     Return PropertyAccess.FormatString(Me.Copyright, strFormat)
    Case "mustapproveghostposts"
     Return Me.MustApproveGhostPosts.ToString
    Case "mustapproveghostpostsyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.MustApproveGhostPosts, formatProvider)
    Case "publishasowner"
     Return Me.PublishAsOwner.ToString
    Case "publishasowneryesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.PublishAsOwner, formatProvider)
    Case "enablepingbacksend"
     Return Me.EnablePingBackSend.ToString
    Case "enablepingbacksendyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.EnablePingBackSend, formatProvider)
    Case "enablepingbackreceive"
     Return Me.EnablePingBackReceive.ToString
    Case "enablepingbackreceiveyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.EnablePingBackReceive, formatProvider)
    Case "autoapprovepingback"
     Return Me.AutoApprovePingBack.ToString
    Case "autoapprovepingbackyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.AutoApprovePingBack, formatProvider)
    Case "enabletrackbacksend"
     Return Me.EnableTrackBackSend.ToString
    Case "enabletrackbacksendyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.EnableTrackBackSend, formatProvider)
    Case "enabletrackbackreceive"
     Return Me.EnableTrackBackReceive.ToString
    Case "enabletrackbackreceiveyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.EnableTrackBackReceive, formatProvider)
    Case "autoapprovetrackback"
     Return Me.AutoApproveTrackBack.ToString
    Case "autoapprovetrackbackyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.AutoApproveTrackBack, formatProvider)
    Case "owneruserid"
     Return (Me.OwnerUserId.ToString(OutputFormat, formatProvider))
    Case "createdbyuserid"
     Return (Me.CreatedByUserID.ToString(OutputFormat, formatProvider))
    Case "createdondate"
     Return (Me.CreatedOnDate.ToString(OutputFormat, formatProvider))
    Case "lastmodifiedbyuserid"
     Return (Me.LastModifiedByUserID.ToString(OutputFormat, formatProvider))
    Case "lastmodifiedondate"
     Return (Me.LastModifiedOnDate.ToString(OutputFormat, formatProvider))
    Case "displayname"
     Return PropertyAccess.FormatString(Me.DisplayName, strFormat)
    Case "email"
     Return PropertyAccess.FormatString(Me.Email, strFormat)
    Case "username"
     Return PropertyAccess.FormatString(Me.Username, strFormat)
    Case "nrposts"
     Return (Me.NrPosts.ToString(OutputFormat, formatProvider))
    Case "lastpublishdate"
     Return (Me.LastPublishDate.ToString(OutputFormat, formatProvider))
    Case "nrviews"
     Return (Me.NrViews.ToString(OutputFormat, formatProvider))
    Case "firstpublishdate"
     Return (Me.FirstPublishDate.ToString(OutputFormat, formatProvider))
    Case "altlocale"
     Return PropertyAccess.FormatString(Me.AltLocale, strFormat)
    Case "alttitle"
     Return PropertyAccess.FormatString(Me.AltTitle, strFormat)
    Case "altdescription"
     Return PropertyAccess.FormatString(Me.AltDescription, strFormat)
    Case "localizedtitle"
     Return PropertyAccess.FormatString(Me.LocalizedTitle, strFormat)
    Case "localizeddescription"
     Return PropertyAccess.FormatString(Me.LocalizedDescription, strFormat)
    Case "link", "permalink"
     Return PermaLink(DotNetNuke.Entities.Portals.PortalSettings.Current)
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
  ''' 	[pdonker]	02/16/2013  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Function GetSchema() As XmlSchema Implements IXmlSerializable.GetSchema
   Return Nothing
  End Function

  Private Function readElement(reader As XmlReader, ElementName As String) As String
   If (Not reader.NodeType = XmlNodeType.Element) OrElse reader.Name <> ElementName Then
    reader.ReadToFollowing(ElementName)
   End If
   If reader.NodeType = XmlNodeType.Element Then
    Return reader.ReadElementContentAsString
   Else
    Return ""
   End If
  End Function

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' ReadXml fills the object (de-serializes it) from the XmlReader passed
  ''' </summary>
  ''' <remarks></remarks>
  ''' <param name="reader">The XmlReader that contains the xml for the object</param>
  ''' <history>
  ''' 	[pdonker]	02/16/2013  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub ReadXml(reader As XmlReader) Implements IXmlSerializable.ReadXml
   Try

    ReadMultiLingualText(reader, "Title", Title, TitleLocalizations)
    ReadMultiLingualText(reader, "Description", Description, DescriptionLocalizations)
    Image = readElement(reader, "Image")
    Locale = readElement(reader, "Locale")
    Boolean.TryParse(readElement(reader, "FullLocalization"), FullLocalization)
    Boolean.TryParse(readElement(reader, "Published"), Published)
    Boolean.TryParse(readElement(reader, "IncludeImagesInFeed"), IncludeImagesInFeed)
    Boolean.TryParse(readElement(reader, "IncludeAuthorInFeed"), IncludeAuthorInFeed)
    Boolean.TryParse(readElement(reader, "Syndicated"), Syndicated)
    SyndicationEmail = readElement(reader, "SyndicationEmail")
    Copyright = readElement(reader, "Copyright")
    Boolean.TryParse(readElement(reader, "MustApproveGhostPosts"), MustApproveGhostPosts)
    Boolean.TryParse(readElement(reader, "PublishAsOwner"), PublishAsOwner)
    Boolean.TryParse(readElement(reader, "EnablePingBackSend"), EnablePingBackSend)
    Boolean.TryParse(readElement(reader, "EnablePingBackReceive"), EnablePingBackReceive)
    Boolean.TryParse(readElement(reader, "AutoApprovePingBack"), AutoApprovePingBack)
    Boolean.TryParse(readElement(reader, "EnableTrackBackSend"), EnableTrackBackSend)
    Boolean.TryParse(readElement(reader, "EnableTrackBackReceive"), EnableTrackBackReceive)
    Boolean.TryParse(readElement(reader, "AutoApproveTrackBack"), AutoApproveTrackBack)
    Username = readElement(reader, "Username")
    Email = readElement(reader, "Email")
    ImportedPosts = New List(Of Posts.PostInfo)
    reader.ReadStartElement("Posts") ' advance to content
    If reader.ReadState <> ReadState.EndOfFile And reader.NodeType <> XmlNodeType.None And reader.LocalName <> "" Then
     Do
      reader.ReadStartElement("Post")
      Dim post As New Posts.PostInfo
      post.ReadXml(reader)
      ImportedPosts.Add(post)
     Loop While reader.ReadToNextSibling("Post")
    End If
    reader.ReadStartElement("Files") ' advance to files
    ImportedFiles = New List(Of BlogML.Xml.BlogMLAttachment)
    If reader.ReadState <> ReadState.EndOfFile And reader.NodeType <> XmlNodeType.None And reader.LocalName <> "" Then
     Do
      reader.ReadStartElement("File")
      Dim f As New BlogML.Xml.BlogMLAttachment
      f.Path = readElement(reader, "Path")
      Dim bufferSize As Integer = 1000
      Dim buffer(bufferSize) As Byte
      Dim readBytes As Integer = 0
      Do
       readBytes = reader.ReadElementContentAsBase64(buffer, 0, bufferSize)
      Loop While bufferSize <= readBytes
      f.Data = buffer
      ImportedFiles.Add(f)
     Loop While reader.ReadToNextSibling("File")
    End If
   Catch ex As Exception
    ' log exception as DNN import routine does not do that
    DotNetNuke.Services.Exceptions.LogException(ex)
    ' re-raise exception to make sure import routine displays a visible error to the user
    Throw New Exception("An error occured during import of an Blog", ex)
   End Try

  End Sub
  Friend Property ImportedPosts As List(Of Posts.PostInfo)
  Friend Property ImportedFiles As List(Of BlogML.Xml.BlogMLAttachment)

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' WriteXml converts the object to Xml (serializes it) and writes it using the XmlWriter passed
  ''' </summary>
  ''' <remarks></remarks>
  ''' <param name="writer">The XmlWriter that contains the xml for the object</param>
  ''' <history>
  ''' 	[pdonker]	02/16/2013  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub WriteXml(writer As XmlWriter) Implements IXmlSerializable.WriteXml
   writer.WriteStartElement("Blog")
   WriteMultiLingualText(writer, "Title", Title, TitleLocalizations)
   WriteMultiLingualText(writer, "Description", Description, DescriptionLocalizations)
   writer.WriteElementString("Image", Image)
   writer.WriteElementString("Locale", Locale)
   writer.WriteElementString("FullLocalization", FullLocalization.ToString())
   writer.WriteElementString("Published", Published.ToString())
   writer.WriteElementString("IncludeImagesInFeed", IncludeImagesInFeed.ToString())
   writer.WriteElementString("IncludeAuthorInFeed", IncludeAuthorInFeed.ToString())
   writer.WriteElementString("Syndicated", Syndicated.ToString())
   writer.WriteElementString("SyndicationEmail", SyndicationEmail)
   writer.WriteElementString("Copyright", Copyright)
   writer.WriteElementString("MustApproveGhostPosts", MustApproveGhostPosts.ToString())
   writer.WriteElementString("PublishAsOwner", PublishAsOwner.ToString())
   writer.WriteElementString("EnablePingBackSend", EnablePingBackSend.ToString())
   writer.WriteElementString("EnablePingBackReceive", EnablePingBackReceive.ToString())
   writer.WriteElementString("AutoApprovePingBack", AutoApprovePingBack.ToString())
   writer.WriteElementString("EnableTrackBackSend", EnableTrackBackSend.ToString())
   writer.WriteElementString("EnableTrackBackReceive", EnableTrackBackReceive.ToString())
   writer.WriteElementString("AutoApproveTrackBack", AutoApproveTrackBack.ToString())
   writer.WriteElementString("Username", Username)
   writer.WriteElementString("Email", Email)
   writer.WriteStartElement("Posts")
   Dim page As Integer = 0
   Dim totalRecords As Integer = 1
   Do While page * 10 < totalRecords
    For Each p As Posts.PostInfo In Posts.PostsController.GetPostsByBlog(ModuleID, BlogID, "", -1, page, 20, "PUBLISHEDONDATE DESC", totalRecords).Values
     p.WriteXml(writer)
    Next
    page += 1
   Loop
   writer.WriteEndElement() ' Posts
   writer.WriteStartElement("Files")
   ' pack files
   Dim postDir As String = GetBlogDirectoryMapPath(BlogID)
   If IO.Directory.Exists(postDir) Then
    For Each f As String In IO.Directory.GetFiles(postDir)
     Dim fileName As String = IO.Path.GetFileName(f)
     Dim att As New BlogML.Xml.BlogMLAttachment With {.Embedded = True, .Path = fileName}
     Using fs As New IO.FileStream(f, IO.FileMode.Open)
      Dim fileData(CInt(fs.Length - 1)) As Byte
      If fs.Length > 0 Then
       fs.Read(fileData, 0, CInt(fs.Length - 1))
       att.Data = fileData
      Else
       'Empty File
      End If
     End Using
     att.WriteAttachmentToXml(writer)
    Next
   End If
   writer.WriteEndElement() ' Files
   writer.WriteEndElement() ' Blog
  End Sub
#End Region

 End Class
End Namespace


