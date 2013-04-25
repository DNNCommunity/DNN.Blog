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
    Case "content"
     Return Me.Content.OutputHtml(strFormat)
    Case "hasimage"
     Return CBool(Me.Image <> "").ToString(formatProvider)
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

#Region " JSON Serialization "
  Public Sub WriteJSON(ByRef s As Stream)
   Dim ser As New DataContractJsonSerializer(GetType(PostInfo))
   ser.WriteObject(s, Me)
  End Sub
#End Region

 End Class
End Namespace


