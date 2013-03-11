Imports DotNetNuke.Services.Tokens
Imports DotNetNuke.Services.Localization.Localization

Namespace Templating
 ''' <summary>
 ''' BaseCustomTokenReplace  allows to add multiple sources implementing <see cref="IPropertyAccess">IPropertyAccess</see>
 ''' </summary>
 ''' <remarks></remarks>
 Public MustInherit Class BaseCustomTokenReplace
  Inherits BaseTokenReplace

#Region " Private Fields "
  Private _AccessLevel As Scope
  Private _AccessingUser As DotNetNuke.Entities.Users.UserInfo
  Private _debugMessages As Boolean
#End Region

#Region "Protected Properties"

  ''' <summary>
  ''' Gets/sets the current Access Level controlling access to critical user settings
  ''' </summary>
  ''' <value>A TokenAccessLevel as defined above</value>
  Protected Property CurrentAccessLevel() As Scope
   Get
    Return _AccessLevel
   End Get
   Set(ByVal value As Scope)

    _AccessLevel = value
   End Set
  End Property

  Protected PropertySource As New System.Collections.Generic.Dictionary(Of String, IPropertyAccess)


#End Region

#Region "Public Properties"

  ''' <summary>
  ''' Gets/sets the user object representing the currently accessing user (permission)
  ''' </summary>
  ''' <value>UserInfo oject</value>
  Public Property AccessingUser() As DotNetNuke.Entities.Users.UserInfo
   Get
    Return _AccessingUser
   End Get
   Set(ByVal value As DotNetNuke.Entities.Users.UserInfo)
    _AccessingUser = value
   End Set
  End Property

  ''' <summary>
  ''' If DebugMessages are enabled, unknown Tokens are replaced with Error Messages 
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Property DebugMessages() As Boolean
   Get
    Return _debugMessages
   End Get
   Set(ByVal value As Boolean)
    _debugMessages = value
   End Set
  End Property

#End Region

#Region "Protected Methods"

  Protected Overrides Function replacedTokenValue(ByVal strObjectName As String, ByVal strPropertyName As String, ByVal strFormat As String) As String
   Dim PropertyNotFound As Boolean = False
   Dim result As String = String.Empty
   If PropertySource.ContainsKey(strObjectName.ToLower) Then
    result = PropertySource(strObjectName.ToLower).GetProperty(strPropertyName, strFormat, FormatProvider, AccessingUser, CurrentAccessLevel, PropertyNotFound)
   Else
    If DebugMessages Then
     Dim message As String = GetString("TokenReplaceUnknownObject", SharedResourceFile, FormatProvider.ToString())
     If message = String.Empty Then message = "Error accessing [{0}:{1}], {0} is an unknown datasource"
     result = String.Format(message, strObjectName, strPropertyName)
    End If
   End If
   If DebugMessages And PropertyNotFound Then
    Dim message As String
    If result = PropertyAccess.ContentLocked Then
     message = GetString("TokenReplaceRestrictedProperty", GlobalResourceFile, FormatProvider.ToString())
    Else
     message = GetString("TokenReplaceUnknownProperty", GlobalResourceFile, FormatProvider.ToString())
    End If

    If message = String.Empty Then message = "Error accessing [{0}:{1}], {1} is unknown for datasource {0}"
    result = String.Format(message, strObjectName, strPropertyName)
   End If
   Return result
  End Function

#End Region

#Region "Public Methods"

  ''' <summary>
  ''' Checks for present [Object:Property] tokens
  ''' </summary>
  ''' <param name="strSourceText">String with [Object:Property] tokens</param>
  ''' <returns></returns>
  ''' <history>
  '''    08/10/2007 [sleupold] created
  '''    10/19/2007 [sleupold] corrected to ignore unchanged text returned (issue DNN-6526)
  ''' </history>
  Public Function ContainsTokens(ByVal strSourceText As String) As Boolean
   If Not String.IsNullOrEmpty(strSourceText) Then
    For Each currentMatch As System.Text.RegularExpressions.Match In TokenizerRegex.Matches(strSourceText)
     If currentMatch.Result("${object}").Length > 0 Then
      Return True
     End If
    Next
   End If
   Return False
  End Function

  ''' <summary>
  ''' returns cacheability of the passed text regarding all contained tokens
  ''' </summary>
  ''' <param name="strSourcetext">the text to parse for tokens to replace</param>
  ''' <returns>cacheability level (not - safe - fully)</returns>
  ''' <remarks>always check cacheability before caching a module!</remarks>
  ''' <history>
  '''    10/19/2007 [sleupold] corrected to handle non-empty strings
  ''' </history>
  Public Function Cacheability(ByVal strSourcetext As String) As CacheLevel
   Dim IsSafe As CacheLevel = CacheLevel.fullyCacheable
   If Not (strSourcetext Is Nothing) AndAlso Not String.IsNullOrEmpty(strSourcetext) Then

    'initialize PropertyAccess classes
    Dim DummyResult As String = ReplaceTokens(strSourcetext)

    Dim Result As New System.Text.StringBuilder
    For Each currentMatch As System.Text.RegularExpressions.Match In TokenizerRegex.Matches(strSourcetext)

     Dim strObjectName As String = currentMatch.Result("${object}")
     If strObjectName.Length > 0 Then
      If strObjectName = "[" Then
       'nothing
      ElseIf Not PropertySource.ContainsKey(strObjectName.ToLower) Then
       'end if
      Else
       Dim c As CacheLevel = PropertySource(strObjectName.ToLower).Cacheability
       If c < IsSafe Then IsSafe = c
      End If
     End If
    Next
   End If
   Return IsSafe
  End Function

#End Region

 End Class

End Namespace