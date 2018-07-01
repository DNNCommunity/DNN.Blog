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

Imports System.IO
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Tokens

Namespace Entities.Comments

 <Serializable(), XmlRoot("Comment"), DataContract()>
 Partial Public Class CommentInfo
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
  ''' 	[pdonker]	02/24/2013  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub Fill(dr As IDataReader) Implements IHydratable.Fill
   CommentID = Convert.ToInt32(Null.SetNull(dr.Item("CommentID"), CommentID))
   ContentItemId = Convert.ToInt32(Null.SetNull(dr.Item("ContentItemId"), ContentItemId))
   ParentId = Convert.ToInt32(Null.SetNull(dr.Item("ParentId"), ParentId))
   Comment = Convert.ToString(Null.SetNull(dr.Item("Comment"), Comment))
   Approved = Convert.ToBoolean(Null.SetNull(dr.Item("Approved"), Approved))
   Author = Convert.ToString(Null.SetNull(dr.Item("Author"), Author))
   Website = Convert.ToString(Null.SetNull(dr.Item("Website"), Website))
   Email = Convert.ToString(Null.SetNull(dr.Item("Email"), Email))
   CreatedByUserID = Convert.ToInt32(Null.SetNull(dr.Item("CreatedByUserID"), CreatedByUserID))
   CreatedOnDate = CDate(Null.SetNull(dr.Item("CreatedOnDate"), CreatedOnDate))
   LastModifiedByUserID = Convert.ToInt32(Null.SetNull(dr.Item("LastModifiedByUserID"), LastModifiedByUserID))
   LastModifiedOnDate = CDate(Null.SetNull(dr.Item("LastModifiedOnDate"), LastModifiedOnDate))
   Username = Convert.ToString(Null.SetNull(dr.Item("Username"), Username))
   DisplayName = Convert.ToString(Null.SetNull(dr.Item("DisplayName"), DisplayName))
   Likes = Convert.ToInt32(Null.SetNull(dr.Item("Likes"), Likes))
   Dislikes = Convert.ToInt32(Null.SetNull(dr.Item("Dislikes"), Dislikes))
   Reports = Convert.ToInt32(Null.SetNull(dr.Item("Reports"), Reports))
   Liked = Convert.ToInt32(Null.SetNull(dr.Item("Liked"), Likes))
   Disliked = Convert.ToInt32(Null.SetNull(dr.Item("Disliked"), Dislikes))
   Reported = Convert.ToInt32(Null.SetNull(dr.Item("Reported"), Reports))
  End Sub
  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets and sets the Key ID
  ''' </summary>
  ''' <remarks>The KeyID property is part of the IHydratble interface.  It is used
  ''' as the key property when creating a Dictionary</remarks>
  ''' <history>
  ''' 	[pdonker]	02/24/2013  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Property KeyID() As Integer Implements IHydratable.KeyID
   Get
    Return CommentID
   End Get
   Set(value As Integer)
    CommentID = value
   End Set
  End Property
#End Region

#Region " IPropertyAccess Implementation "
  Public Function GetProperty(strPropertyName As String, strFormat As String, formatProvider As System.Globalization.CultureInfo, AccessingUser As DotNetNuke.Entities.Users.UserInfo, AccessLevel As DotNetNuke.Services.Tokens.Scope, ByRef PropertyNotFound As Boolean) As String Implements DotNetNuke.Services.Tokens.IPropertyAccess.GetProperty
   Dim OutputFormat As String = String.Empty
   Dim portalSettings As DotNetNuke.Entities.Portals.PortalSettings = DotNetNuke.Entities.Portals.PortalController.Instance.GetCurrentPortalSettings
   If strFormat = String.Empty Then
    OutputFormat = "D"
   Else
    OutputFormat = strFormat
   End If
   Select Case strPropertyName.ToLower
    Case "commentid"
     Return (CommentID.ToString(OutputFormat, formatProvider))
    Case "contentitemid"
     Return (ContentItemId.ToString(OutputFormat, formatProvider))
    Case "parentid"
     Return (ParentId.ToString(OutputFormat, formatProvider))
    Case "comment"
     Return Comment.OutputHtml(strFormat)
    Case "approved"
     Return Approved.ToString
    Case "approvedyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Approved, formatProvider)
    Case "author"
     Return PropertyAccess.FormatString(Author, strFormat)
    Case "website"
     Return PropertyAccess.FormatString(Website, strFormat)
    Case "email"
     Return PropertyAccess.FormatString(Email, strFormat)
    Case "createdbyuserid"
     Return (CreatedByUserID.ToString(OutputFormat, formatProvider))
    Case "createdondate"
     Return (CreatedOnDate.ToString(OutputFormat, formatProvider))
    Case "createdondateutc"
     Return (CreatedOnDate.ToUniversalTime.ToString(OutputFormat, formatProvider))
    Case "lastmodifiedbyuserid"
     Return (LastModifiedByUserID.ToString(OutputFormat, formatProvider))
    Case "lastmodifiedondate"
     Return (LastModifiedOnDate.ToString(OutputFormat, formatProvider))
    Case "lastmodifiedondateutc"
     Return (LastModifiedOnDate.ToUniversalTime.ToString(OutputFormat, formatProvider))
    Case "username"
     Return PropertyAccess.FormatString(Username, strFormat)
    Case "displayname"
     Return PropertyAccess.FormatString(DisplayName, strFormat)
    Case "likes"
     Return (Likes.ToString(OutputFormat, formatProvider))
    Case "dislikes"
     Return (Dislikes.ToString(OutputFormat, formatProvider))
    Case "reports"
     Return (Reports.ToString(OutputFormat, formatProvider))
    Case "liked"
     Return Liked.ToBool.ToString
    Case "likedyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Liked.ToBool, formatProvider)
    Case "disliked"
     Return Disliked.ToBool.ToString
    Case "dislikedyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Disliked.ToBool, formatProvider)
    Case "reported"
     Return Reported.ToBool.ToString
    Case "reportedyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Reported.ToBool, formatProvider)
    Case "karmaed"
     Return (Reported + Liked + Disliked).ToBool.ToString
    Case "karmaedyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo((Reported + Liked + Disliked).ToBool, formatProvider)
    Case "posturl"
     Dim _Link As String = DotNetNuke.Common.ApplicationURL(DotNetNuke.Entities.Portals.PortalSettings.Current.ActiveTab.TabID) & "&Post=" & ContentItemId.ToString
     Return DotNetNuke.Common.FriendlyUrl(DotNetNuke.Entities.Portals.PortalSettings.Current.ActiveTab, _Link)
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
  ''' 	[pdonker]	02/24/2013  Created
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
  ''' 	[pdonker]	02/24/2013  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub ReadXml(reader As XmlReader) Implements IXmlSerializable.ReadXml
   Try

    If Not Int32.TryParse(readElement(reader, "CommentID"), CommentID) Then
     CommentID = Null.NullInteger
    End If
    If Not Int32.TryParse(readElement(reader, "ContentItemId"), ContentItemId) Then
     ContentItemId = Null.NullInteger
    End If
    If Not Int32.TryParse(readElement(reader, "ParentId"), ParentId) Then
     ParentId = Null.NullInteger
    End If
    Comment = readElement(reader, "Comment")
    Boolean.TryParse(readElement(reader, "Approved"), Approved)
    Author = readElement(reader, "Author")
    Website = readElement(reader, "Website")
    Email = readElement(reader, "Email")
    If Not Int32.TryParse(readElement(reader, "CreatedByUserID"), CreatedByUserID) Then
     CreatedByUserID = Null.NullInteger
    End If
    Date.TryParse(readElement(reader, "CreatedOnDate"), CreatedOnDate)
    If Not Int32.TryParse(readElement(reader, "LastModifiedByUserID"), LastModifiedByUserID) Then
     LastModifiedByUserID = Null.NullInteger
    End If
    Date.TryParse(readElement(reader, "LastModifiedOnDate"), LastModifiedOnDate)
    Username = readElement(reader, "Username")
    DisplayName = readElement(reader, "DisplayName")
    If Not Int32.TryParse(readElement(reader, "Likes"), Likes) Then
     Likes = Null.NullInteger
    End If
    If Not Int32.TryParse(readElement(reader, "Dislikes"), Dislikes) Then
     Dislikes = Null.NullInteger
    End If
    If Not Int32.TryParse(readElement(reader, "Reports"), Reports) Then
     Reports = Null.NullInteger
    End If
   Catch ex As Exception
    ' log exception as DNN import routine does not do that
    DotNetNuke.Services.Exceptions.LogException(ex)
    ' re-raise exception to make sure import routine displays a visible error to the user
    Throw New Exception("An error occured during import of an Comment", ex)
   End Try

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' WriteXml converts the object to Xml (serializes it) and writes it using the XmlWriter passed
  ''' </summary>
  ''' <remarks></remarks>
  ''' <param name="writer">The XmlWriter that contains the xml for the object</param>
  ''' <history>
  ''' 	[pdonker]	02/24/2013  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub WriteXml(writer As XmlWriter) Implements IXmlSerializable.WriteXml
   writer.WriteStartElement("Comment")
   writer.WriteElementString("CommentID", CommentID.ToString())
   writer.WriteElementString("ContentItemId", ContentItemId.ToString())
   writer.WriteElementString("ParentId", ParentId.ToString())
   writer.WriteElementString("Comment", Comment)
   writer.WriteElementString("Approved", Approved.ToString())
   writer.WriteElementString("Author", Author)
   writer.WriteElementString("Website", Website)
   writer.WriteElementString("Email", Email)
   writer.WriteElementString("CreatedByUserID", CreatedByUserID.ToString())
   writer.WriteElementString("CreatedOnDate", CreatedOnDate.ToString())
   writer.WriteElementString("LastModifiedByUserID", LastModifiedByUserID.ToString())
   writer.WriteElementString("LastModifiedOnDate", LastModifiedOnDate.ToString())
   writer.WriteElementString("Username", Username)
   writer.WriteElementString("DisplayName", DisplayName)
   writer.WriteElementString("Likes", Likes.ToString())
   writer.WriteElementString("Dislikes", Dislikes.ToString())
   writer.WriteElementString("Reports", Reports.ToString())
   writer.WriteEndElement()
  End Sub
#End Region

#Region " ToXml Methods "
  Public Function ToXml() As String
   Return ToXml("Comment")
  End Function

  Public Function ToXml(elementName As String) As String
   Dim xml As New StringBuilder
   xml.Append("<")
   xml.Append(elementName)
   AddAttribute(xml, "CommentID", CommentID.ToString())
   AddAttribute(xml, "ContentItemId", ContentItemId.ToString())
   AddAttribute(xml, "ParentId", ParentId.ToString())
   AddAttribute(xml, "Comment", Comment)
   AddAttribute(xml, "Approved", Approved.ToString())
   AddAttribute(xml, "Author", Author)
   AddAttribute(xml, "Website", Website)
   AddAttribute(xml, "Email", Email)
   AddAttribute(xml, "CreatedByUserID", CreatedByUserID.ToString())
   AddAttribute(xml, "CreatedOnDate", CreatedOnDate.ToUniversalTime.ToString("u"))
   AddAttribute(xml, "LastModifiedByUserID", LastModifiedByUserID.ToString())
   AddAttribute(xml, "LastModifiedOnDate", LastModifiedOnDate.ToUniversalTime.ToString("u"))
   AddAttribute(xml, "Username", Username)
   AddAttribute(xml, "DisplayName", DisplayName)
   AddAttribute(xml, "Likes", Likes.ToString())
   AddAttribute(xml, "Dislikes", Dislikes.ToString())
   AddAttribute(xml, "Reports", Reports.ToString())
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
   Dim ser As New DataContractJsonSerializer(GetType(CommentInfo))
   ser.WriteObject(s, Me)
  End Sub
#End Region

 End Class
End Namespace


