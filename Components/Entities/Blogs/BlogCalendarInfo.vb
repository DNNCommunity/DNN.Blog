Imports System.Runtime.Serialization
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Tokens

Namespace Entities.Blogs
 Public Class BlogCalendarInfo
  Implements IHydratable
  Implements IPropertyAccess

#Region " Public Properties "
  <DataMember()>
  Public Property PostYear As Int32 = -1
  <DataMember()>
  Public Property PostMonth As Int32 = -1
  <DataMember()>
  Public Property PostCount As Int32 = -1
  <DataMember()>
  Public Property ViewCount As Int32 = -1

  Private _FirstDay As Date = Date.MinValue
  Public ReadOnly Property FirstDay As Date
   Get
    If _FirstDay = Date.MinValue Then
     _FirstDay = DateSerial(PostYear, PostMonth, 1)
    End If
    Return _FirstDay
   End Get
  End Property

  Private _FirstDayNextMonth As Date = Date.MinValue
  Public ReadOnly Property FirstDayNextMonth As Date
   Get
    If _FirstDayNextMonth = Date.MinValue Then
     _FirstDayNextMonth = DateSerial(PostYear, PostMonth, 1).AddMonths(1)
    End If
    Return _FirstDayNextMonth
   End Get
  End Property

  Private _LastDay As Date = Date.MinValue
  Public ReadOnly Property LastDay As Date
   Get
    If _LastDay = Date.MinValue Then
     _LastDay = DateSerial(PostYear, PostMonth, 1).AddMonths(1).AddDays(-1)
    End If
    Return _LastDay
   End Get
  End Property
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
   PostYear = Convert.ToInt32(Null.SetNull(dr.Item("PostYear"), PostYear))
   PostMonth = Convert.ToInt32(Null.SetNull(dr.Item("PostMonth"), PostMonth))
   PostCount = Convert.ToInt32(Null.SetNull(dr.Item("PostCount"), PostCount))
   ViewCount = Convert.ToInt32(Null.SetNull(dr.Item("ViewCount"), ViewCount))
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
  Public Function GetProperty(strPropertyName As String, strFormat As String, formatProvider As System.Globalization.CultureInfo, AccessingUser As DotNetNuke.Entities.Users.UserInfo, AccessLevel As DotNetNuke.Services.Tokens.Scope, ByRef PropertyNotFound As Boolean) As String Implements DotNetNuke.Services.Tokens.IPropertyAccess.GetProperty
   Dim OutputFormat As String = String.Empty
   Dim portalSettings As DotNetNuke.Entities.Portals.PortalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings()
   If strFormat = String.Empty Then
    OutputFormat = "D"
   Else
    OutputFormat = strFormat
   End If
   Select Case strPropertyName.ToLower
    Case "postyear"
     Return (Me.PostYear.ToString(OutputFormat, formatProvider))
    Case "postmonth"
     Return (Me.PostMonth.ToString(OutputFormat, formatProvider))
    Case "postcount"
     Return (Me.PostCount.ToString(OutputFormat, formatProvider))
    Case "viewcount"
     Return (Me.ViewCount.ToString(OutputFormat, formatProvider))
    Case "firstday"
     Return (Me.FirstDay.ToString(OutputFormat, formatProvider))
    Case "lastday"
     Return (Me.LastDay.ToString(OutputFormat, formatProvider))
    Case "firstdaynextmonth"
     Return (Me.FirstDayNextMonth.ToString(OutputFormat, formatProvider))
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

 End Class
End Namespace
