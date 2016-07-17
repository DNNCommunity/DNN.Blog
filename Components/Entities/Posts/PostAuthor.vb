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

Imports DotNetNuke.Common.Globals

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
  Public Property ParentTabID As Integer = -1
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
     Return (NrPosts.ToString(OutputFormat, formatProvider))
    Case "nrviews"
     Return (NrViews.ToString(OutputFormat, formatProvider))
    Case "lastpublishdate"
     Return (LastPublishDate.ToString(OutputFormat, formatProvider))
    Case "firstpublishdate"
     Return (FirstPublishDate.ToString(OutputFormat, formatProvider))
    Case "parenturl"
     Return PermaLink(ParentTabID)
    Case Else
     Return MyBase.GetProperty(strPropertyName, strFormat, formatProvider, AccessingUser, AccessLevel, PropertyNotFound)
   End Select
  End Function

  Public Shadows ReadOnly Property Cacheability() As DotNetNuke.Services.Tokens.CacheLevel Implements DotNetNuke.Services.Tokens.IPropertyAccess.Cacheability
   Get
    Return CacheLevel.fullyCacheable
   End Get
  End Property
#End Region

#Region " Public Methods "
  Public Function PermaLink(portalSettings As DotNetNuke.Entities.Portals.PortalSettings) As String
   Return PermaLink(portalSettings.ActiveTab)
  End Function
  Public Function PermaLink(strParentTabID As Integer) As String
   Dim oTabController As DotNetNuke.Entities.Tabs.TabController = New DotNetNuke.Entities.Tabs.TabController
   Dim oParentTab As DotNetNuke.Entities.Tabs.TabInfo = oTabController.GetTab(strParentTabID, DotNetNuke.Entities.Portals.PortalSettings.Current.PortalId, False)
   _permaLink = String.Empty
   Return PermaLink(oParentTab)
  End Function

  Private _permaLink As String = ""
  Public Function PermaLink(tab As DotNetNuke.Entities.Tabs.TabInfo) As String
   If String.IsNullOrEmpty(_permaLink) Then
    _permaLink = ApplicationURL(tab.TabID) & "&author=" & UserID.ToString
    If DotNetNuke.Entities.Host.Host.UseFriendlyUrls Then
     _permaLink = FriendlyUrl(tab, _permaLink, "")
    Else
     _permaLink = ResolveUrl(_permaLink)
    End If
   End If
   Return _permaLink
  End Function

#End Region
 End Class
End Namespace
