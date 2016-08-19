'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2010 by DotNetNuke Corp. 
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

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Controllers
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Entities.Host
Imports System.Linq
Imports DotNetNuke.Services.Tokens


Namespace Templating

 ''' <summary>
 ''' The TokenReplace class provides the option to replace tokens formatted 
 ''' [object:property] or [object:property|format] or [custom:no] within a string
 ''' with the appropriate current property/custom values.
 ''' Example for Newsletter: 'Dear [user:Displayname],' ==> 'Dear Superuser Account,'
 ''' Supported Token Sources: User, Host, Portal, Tab, Module, Membership, Profile, 
 '''                          Row, Date, Ticks, ArrayList (Custom), IDictionary
 ''' </summary>
 ''' <remarks></remarks>
 Public Class TokenReplace
  Inherits BaseCustomTokenReplace

#Region "Private Fields "

  Private _PortalSettings As PortalSettings
  Private _Hostsettings As Dictionary(Of String, String)
  Private _ModuleInfo As ModuleInfo
  Private _User As UserInfo
  Private _Tab As TabInfo
  Private _ModuleId As Integer = Integer.MinValue

#End Region

#Region "Public Properties "

  ''' <summary>
  ''' Gets the Host settings from Portal
  ''' </summary>
  ''' <value>A hashtable with all settings</value>
  Private ReadOnly Property HostSettings() As Dictionary(Of String, String)
   Get
    If _Hostsettings Is Nothing Then
     _Hostsettings = HostController.Instance.GetSettings(). _
         Where(Function(c) Not c.Value.IsSecure). _
         ToDictionary(Function(c) c.Key, Function(c) c.Value.Value)
    End If
    Return _Hostsettings
   End Get
  End Property

  ''' <summary>
  ''' Gets/sets the current ModuleID to be used for 'User:' token replacement
  ''' </summary>
  ''' <value>ModuleID (Integer)</value>
  Public Property ModuleId() As Integer
   Get
    Return _ModuleId
   End Get
   Set(ByVal value As Integer)

    _ModuleId = value
   End Set
  End Property

  ''' <summary>
  ''' Gets/sets the module settings object to use for 'Module:' token replacement
  ''' </summary>
  Public Property ModuleInfo() As ModuleInfo
   Get
    If ModuleId > Integer.MinValue AndAlso (_ModuleInfo Is Nothing OrElse _ModuleInfo.ModuleID <> ModuleId) Then
     Dim mc As New ModuleController
     If Not PortalSettings Is Nothing AndAlso Not PortalSettings.ActiveTab Is Nothing Then
      _ModuleInfo = mc.GetModule(ModuleId, PortalSettings.ActiveTab.TabID, False)
     Else
      _ModuleInfo = mc.GetModule(ModuleId)
     End If
    End If
    Return _ModuleInfo
   End Get
   Set(ByVal value As ModuleInfo)
    _ModuleInfo = value
   End Set
  End Property

  ''' <summary>
  ''' Gets/sets the portal settings object to use for 'Portal:' token replacement
  ''' </summary>
  ''' <value>PortalSettings oject</value>
  Public Property PortalSettings() As PortalSettings
   Get
    Return _PortalSettings
   End Get
   Set(ByVal value As PortalSettings)
    _PortalSettings = value
   End Set
  End Property

  ''' <summary>
  ''' Gets/sets the user object to use for 'User:' token replacement
  ''' </summary>
  ''' <value>UserInfo oject</value>
  Public Property User() As UserInfo
   Get
    Return _User
   End Get
   Set(ByVal value As UserInfo)
    _User = value
   End Set
  End Property

#End Region

#Region "Constructor"

  ''' <summary>
  ''' creates a new TokenReplace object for default context
  ''' </summary>
  ''' <history>
  ''' 08/10/2007 sLeupold  documented
  ''' </history>
  Public Sub New()
   Me.New(Scope.DefaultSettings, Nothing, Nothing, Nothing, Null.NullInteger)
  End Sub

  ''' <summary>
  ''' creates a new TokenReplace object for default context and the current module
  ''' </summary>
  ''' <param name="ModuleID">ID of the current module</param>
  ''' <history>
  ''' 10/19/2007 sLeupold  added
  ''' </history>
  Public Sub New(ByVal ModuleID As Integer)
   Me.New(Scope.DefaultSettings, Nothing, Nothing, Nothing, ModuleID)
  End Sub

  ''' <summary>
  ''' creates a new TokenReplace object for custom context
  ''' </summary>
  ''' <param name="AccessLevel">Security level granted by the calling object</param>
  ''' <history>
  ''' 08/10/2007 sLeupold  documented
  ''' </history>
  Public Sub New(ByVal AccessLevel As Scope)
   Me.New(AccessLevel, Nothing, Nothing, Nothing, Null.NullInteger)
  End Sub

  ''' <summary>
  ''' creates a new TokenReplace object for custom context
  ''' </summary>
  ''' <param name="AccessLevel">Security level granted by the calling object</param>
  ''' <param name="ModuleID">ID of the current module</param>
  ''' <history>
  ''' 08/10/2007 sLeupold  documented
  ''' 10/19/2007 sLeupold  added
  ''' </history>
  Public Sub New(ByVal AccessLevel As Scope, ByVal ModuleID As Integer)
   Me.New(AccessLevel, Nothing, Nothing, Nothing, ModuleID)
  End Sub


  ''' <summary>
  ''' creates a new TokenReplace object for custom context
  ''' </summary>
  ''' <param name="AccessLevel">Security level granted by the calling object</param>
  ''' <param name="Language">Locale to be used for formatting etc.</param>
  ''' <param name="PortalSettings">PortalSettings to be used</param>
  ''' <param name="User">user, for which the properties shall be returned</param>
  ''' <history>
  ''' 08/10/2007 sLeupold  documented
  ''' </history>
  Public Sub New(ByVal AccessLevel As Scope, ByVal Language As String, ByVal PortalSettings As PortalSettings, ByVal User As UserInfo)
   Me.New(AccessLevel, Language, PortalSettings, User, Null.NullInteger)
  End Sub

  ''' <summary>
  ''' creates a new TokenReplace object for custom context
  ''' </summary>
  ''' <param name="AccessLevel">Security level granted by the calling object</param>
  ''' <param name="Language">Locale to be used for formatting etc.</param>
  ''' <param name="PortalSettings">PortalSettings to be used</param>
  ''' <param name="User">user, for which the properties shall be returned</param>
  ''' <param name="ModuleID">ID of the current module</param>
  ''' <history>
  '''     08/10/2007    sleupold  documented
  '''     10/19/2007    sleupold  ModuleID added
  ''' </history>
  Public Sub New(ByVal AccessLevel As Scope, ByVal Language As String, ByVal PortalSettings As PortalSettings, ByVal User As UserInfo, ByVal ModuleID As Integer)
   CurrentAccessLevel = AccessLevel
   If AccessLevel <> Scope.NoSettings Then
    If PortalSettings Is Nothing Then
     If HttpContext.Current IsNot Nothing Then Me.PortalSettings = PortalController.Instance.GetCurrentPortalSettings
    Else
     Me.PortalSettings = PortalSettings
    End If
    If User Is Nothing Then
     If HttpContext.Current IsNot Nothing Then
      Me.User = CType(HttpContext.Current.Items("UserInfo"), UserInfo)
     Else
      Me.User = New UserInfo
     End If
     AccessingUser = Me.User
    Else
     Me.User = User
     If HttpContext.Current IsNot Nothing Then
      AccessingUser = CType(HttpContext.Current.Items("UserInfo"), UserInfo)
     Else
      AccessingUser = New UserInfo
     End If
    End If
    If String.IsNullOrEmpty(Language) Then
     Me.Language = Threading.Thread.CurrentThread.CurrentUICulture.Name
    Else
     Me.Language = Language
    End If
    If ModuleID <> Null.NullInteger Then
     Me.ModuleId = ModuleID
    End If
   End If
   PropertySource("date") = New DateTimePropertyAccess()
   PropertySource("datetime") = New DateTimePropertyAccess()
   PropertySource("ticks") = New TicksPropertyAccess()
   PropertySource("culture") = New CulturePropertyAccess()
  End Sub

#End Region

#Region "Public Replace Methods"

  ''' <summary>
  ''' Replaces tokens in strSourceText parameter with the property values
  ''' </summary>
  ''' <param name="strSourceText">String with [Object:Property] tokens</param>
  ''' <returns>string containing replaced values</returns>
  Public Function ReplaceEnvironmentTokens(ByVal strSourceText As String) As String
   Return ReplaceTokens(strSourceText)
  End Function

  ''' <summary>
  ''' Replaces tokens in strSourceText parameter with the property values
  ''' </summary>
  ''' <param name="strSourceText">String with [Object:Property] tokens</param>
  ''' <param name="row"></param>
  ''' <returns>string containing replaced values</returns>
  Public Function ReplaceEnvironmentTokens(ByVal strSourceText As String, ByVal row As DataRow) As String
   Dim rowProperties As New DataRowPropertyAccess(row)
   PropertySource("field") = rowProperties
   PropertySource("row") = rowProperties
   Return ReplaceTokens(strSourceText)
  End Function

  ''' <summary>
  ''' Replaces tokens in strSourceText parameter with the property values
  ''' </summary>
  ''' <param name="strSourceText">String with [Object:Property] tokens</param>
  ''' <param name="Custom"></param>
  ''' <param name="CustomCaption"></param>
  ''' <returns>string containing replaced values</returns>
  Public Function ReplaceEnvironmentTokens(ByVal strSourceText As String, ByVal Custom As ArrayList, ByVal CustomCaption As String) As String
   PropertySource(CustomCaption.ToLower) = New ArrayListPropertyAccess(Custom)
   Return ReplaceTokens(strSourceText)
  End Function

  ''' <summary>
  ''' Replaces tokens in strSourceText parameter with the property values
  ''' </summary>
  ''' <param name="strSourceText">String with [Object:Property] tokens</param>
  ''' <param name="Custom">NameValueList for replacing [custom:name] tokens, where 'custom' is specified in next param and name is either thekey or the index number in the string </param>
  ''' <param name="CustomCaption">Token name to be used inside token  [custom:name]</param>
  ''' <returns>string containing replaced values</returns>
  ''' <history>
  ''' 08/10/2007 sLeupold created
  ''' </history>
  Public Function ReplaceEnvironmentTokens(ByVal strSourceText As String, ByVal Custom As IDictionary, ByVal CustomCaption As String) As String
   PropertySource(CustomCaption.ToLower) = New DictionaryPropertyAccess(Custom)
   Return ReplaceTokens(strSourceText)
  End Function

  ''' <summary>
  ''' Replaces tokens in strSourceText parameter with the property values
  ''' </summary>
  ''' <param name="strSourceText">String with [Object:Property] tokens</param>
  ''' <param name="Custom">NameValueList for replacing [custom:name] tokens, where 'custom' is specified in next param and name is either thekey or the index number in the string </param>
  ''' <param name="CustomCaption">Token name to be used inside token  [custom:name]</param>
  ''' <param name="Row">DataRow, from which field values shall be used for replacement</param>
  ''' <returns>string containing replaced values</returns>
  ''' <history>
  ''' 08/10/2007 sLeupold created
  ''' </history>
  Public Function ReplaceEnvironmentTokens(ByVal strSourceText As String, ByVal Custom As ArrayList, ByVal CustomCaption As String, ByVal Row As System.Data.DataRow) As String
   Dim rowProperties As New DataRowPropertyAccess(Row)
   PropertySource("field") = rowProperties
   PropertySource("row") = rowProperties
   PropertySource(CustomCaption.ToLower) = New ArrayListPropertyAccess(Custom)
   Return ReplaceTokens(strSourceText)
  End Function

  ''' <summary>
  ''' Replaces tokens in strSourceText parameter with the property values, skipping environment objects
  ''' </summary>
  ''' <param name="strSourceText">String with [Object:Property] tokens</param>
  ''' <returns>string containing replaced values</returns>
  ''' <history>
  ''' 08/10/2007 sLeupold created
  ''' </history>
  Protected Overrides Function ReplaceTokens(ByVal strSourceText As String) As String
   InitializePropertySources()
   Return MyBase.ReplaceTokens(strSourceText)
  End Function

#End Region

#Region "Private methods"

  ''' <summary>
  ''' setup context by creating appropriate objects
  ''' </summary>
  ''' <history>
  ''' /08/10/2007 sCullmann created
  ''' </history>
  ''' <remarks >
  ''' security is not the purpose of the initialization, this is in the responsibility of each property access class
  ''' </remarks>
  Private Sub InitializePropertySources()

   'Cleanup, by default "" is returned for these objects and any property
   Dim DefaultPropertyAccess As IPropertyAccess = New EmptyPropertyAccess()
   PropertySource("portal") = DefaultPropertyAccess
   PropertySource("tab") = DefaultPropertyAccess
   PropertySource("host") = DefaultPropertyAccess
   PropertySource("module") = DefaultPropertyAccess
   PropertySource("user") = DefaultPropertyAccess
   PropertySource("membership") = DefaultPropertyAccess
   PropertySource("profile") = DefaultPropertyAccess

   'initialization
   If CurrentAccessLevel >= Scope.Configuration Then
    If PortalSettings IsNot Nothing Then
     PropertySource("portal") = PortalSettings
     PropertySource("tab") = PortalSettings.ActiveTab

    End If
    PropertySource("host") = New HostPropertyAccess()
    If Not (ModuleInfo Is Nothing) Then
     PropertySource("module") = ModuleInfo
    End If
   End If

   If CurrentAccessLevel >= Scope.DefaultSettings AndAlso Not (User Is Nothing OrElse User.UserID = -1) Then
    PropertySource("user") = User
    PropertySource("membership") = New MembershipPropertyAccess(User)
    PropertySource("profile") = New ProfilePropertyAccess(User)
   End If

  End Sub

#End Region

 End Class

End Namespace
