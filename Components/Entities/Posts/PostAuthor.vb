Imports System.Runtime.Serialization
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Tokens

Namespace Entities.Posts
 Public Class PostAuthor
  Inherits UserInfo

  Implements IHydratable
  Implements IPropertyAccess

#Region " Public Properties "
  <DataMember()>
  Public Property NrPosts As Int32 = 0
  <DataMember()>
  Public Property NrViews As Int32 = 0
  <DataMember()>
  Public Property LastPublishDate As Date = Date.MinValue
  <DataMember()>
  Public Property FirstPublishDate As Date = Date.MinValue
#End Region

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
   PortalID = Convert.ToInt32(Null.SetNull(dr.Item("PortalID"), PortalID))
   IsSuperUser = Convert.ToBoolean(Null.SetNull(dr.Item("IsSuperUser"), IsSuperUser))
   UserID = Convert.ToInt32(Null.SetNull(dr.Item("UserID"), UserID))
   FirstName = Convert.ToString(Null.SetNull(dr.Item("FirstName"), FirstName))
   LastName = Convert.ToString(Null.SetNull(dr.Item("LastName"), LastName))
   DisplayName = Convert.ToString(Null.SetNull(dr.Item("DisplayName"), DisplayName))
   Username = Convert.ToString(Null.SetNull(dr.Item("Username"), Username))
   Email = Convert.ToString(Null.SetNull(dr.Item("Email"), Email))
   NrPosts = Convert.ToInt32(Null.SetNull(dr.Item("NrPosts"), NrPosts))
   NrViews = Convert.ToInt32(Null.SetNull(dr.Item("NrViews"), NrViews))
   LastPublishDate = CDate(Null.SetNull(dr.Item("LastPublishDate"), LastPublishDate))
   FirstPublishDate = CDate(Null.SetNull(dr.Item("FirstPublishDate"), FirstPublishDate))
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
    Return Nothing
   End Get
   Set(value As Integer)
   End Set
  End Property
#End Region

#Region " IPropertyAccess Implementation "
  Public Shadows Function GetProperty(strPropertyName As String, strFormat As String, formatProvider As System.Globalization.CultureInfo, AccessingUser As DotNetNuke.Entities.Users.UserInfo, AccessLevel As DotNetNuke.Services.Tokens.Scope, ByRef PropertyNotFound As Boolean) As String Implements DotNetNuke.Services.Tokens.IPropertyAccess.GetProperty
   Dim OutputFormat As String = String.Empty
   Dim portalSettings As DotNetNuke.Entities.Portals.PortalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings()
   If strFormat = String.Empty Then
    OutputFormat = "D"
   Else
    OutputFormat = strFormat
   End If
   Select Case strPropertyName.ToLower
    Case "nrposts"
     Return (Me.NrPosts.ToString(OutputFormat, formatProvider))
    Case "nrviews"
     Return (Me.NrViews.ToString(OutputFormat, formatProvider))
    Case "lastpublishdate"
     Return (Me.LastPublishDate.ToString(OutputFormat, formatProvider))
    Case "firstpublishdate"
     Return (Me.FirstPublishDate.ToString(OutputFormat, formatProvider))
    Case Else
     Return MyBase.GetProperty(strPropertyName, strFormat, formatProvider, AccessingUser, AccessLevel, PropertyNotFound)
   End Select
   Return Null.NullString
  End Function

  Public Shadows ReadOnly Property Cacheability() As DotNetNuke.Services.Tokens.CacheLevel Implements DotNetNuke.Services.Tokens.IPropertyAccess.Cacheability
   Get
    Return CacheLevel.fullyCacheable
   End Get
  End Property
#End Region

 End Class
End Namespace
