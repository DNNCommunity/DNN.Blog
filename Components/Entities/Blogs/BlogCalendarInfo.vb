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

Imports System.Runtime.Serialization
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Tokens

Namespace Entities.Blogs
 Public Class BlogCalendarInfo
  Implements IHydratable
  Implements IPropertyAccess

#Region " ML Properties "
  Public Property ParentTabID As Integer = -1
#End Region

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
   Dim portalSettings As DotNetNuke.Entities.Portals.PortalSettings = DotNetNuke.Entities.Portals.PortalController.Instance.GetCurrentPortalSettings
   If strFormat = String.Empty Then
    OutputFormat = "D"
   Else
    OutputFormat = strFormat
   End If
   Select Case strPropertyName.ToLower
    Case "postyear"
     Return (PostYear.ToString(OutputFormat, formatProvider))
    Case "postmonth"
     Return (PostMonth.ToString(OutputFormat, formatProvider))
    Case "postcount"
     Return (PostCount.ToString(OutputFormat, formatProvider))
    Case "viewcount"
     Return (ViewCount.ToString(OutputFormat, formatProvider))
    Case "firstday"
     Return (FirstDay.ToString(OutputFormat, formatProvider))
    Case "lastday"
     Return (LastDay.ToString(OutputFormat, formatProvider))
    Case "firstdaynextmonth"
     Return (FirstDayNextMonth.ToString(OutputFormat, formatProvider))
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

  Public Function PermaLink(strParentTabID As Integer) As String
   Dim oTabController As DotNetNuke.Entities.Tabs.TabController = New DotNetNuke.Entities.Tabs.TabController
   Dim oParentTab As DotNetNuke.Entities.Tabs.TabInfo = oTabController.GetTab(strParentTabID, DotNetNuke.Entities.Portals.PortalSettings.Current.PortalId, False)
   _permaLink = String.Empty
   Return PermaLink(oParentTab)
  End Function

  Public Function PermaLink(portalSettings As DotNetNuke.Entities.Portals.PortalSettings) As String
   Return PermaLink(portalSettings.ActiveTab)
  End Function

  Private _permaLink As String = ""
  Public Function PermaLink(tab As DotNetNuke.Entities.Tabs.TabInfo) As String
   If String.IsNullOrEmpty(_permaLink) Then
    _permaLink = DotNetNuke.Common.Globals.ApplicationURL(tab.TabID) & "&end=" & FirstDayNextMonth.ToString
    _permaLink = DotNetNuke.Common.Globals.FriendlyUrl(tab, _permaLink)
   End If
   Return _permaLink
  End Function

 End Class
End Namespace
