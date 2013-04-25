Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Services.Tokens
Imports DotNetNuke.Modules.Blog.Entities.Posts

Namespace Templating
 Public Class LazyLoadingUser
  Implements IPropertyAccess

#Region " Properties "
  Public Property PortalId As Integer = -1
  Public Property UserId As Integer = -1
  Public Property Username As String = ""

  Private _user As UserInfo
  Public Property User() As UserInfo
   Get
    If _user Is Nothing Then
     If UserId > -1 Then
      _user = UserController.GetUserById(PortalId, UserId)
     ElseIf Username <> "" Then
      _user = UserController.GetCachedUser(PortalId, Username)
     Else
      _user = New UserInfo
     End If
    End If
    Return _user
   End Get
   Set(ByVal value As UserInfo)
    _user = value
   End Set
  End Property

  Private _postAuthor As PostAuthor = Nothing
#End Region

#Region " Constructors "
  Public Sub New(portalId As Integer, userId As Integer)
   Me.PortalId = portalId
   Me.UserId = userId
  End Sub

  Public Sub New(portalId As Integer, userName As String)
   Me.PortalId = portalId
   Me.Username = userName
  End Sub

  Public Sub New(user As UserInfo)
   Me.User = user
   Me.PortalId = user.PortalID
   Me.UserId = user.UserID
  End Sub

  Public Sub New(user As PostAuthor)
   Me.User = user
   Me.PortalId = user.PortalID
   Me.UserId = user.UserID
   _postAuthor = user
  End Sub
#End Region

#Region " IPropertyAccess Implementation "
  Public Function GetProperty(strPropertyName As String, strFormat As String, formatProvider As System.Globalization.CultureInfo, AccessingUser As DotNetNuke.Entities.Users.UserInfo, AccessLevel As DotNetNuke.Services.Tokens.Scope, ByRef PropertyNotFound As Boolean) As String Implements DotNetNuke.Services.Tokens.IPropertyAccess.GetProperty
   Dim res As String = Null.NullString
   If _postAuthor IsNot Nothing Then
    res = _postAuthor.GetProperty(strPropertyName, strFormat, formatProvider, AccessingUser, AccessLevel, PropertyNotFound)
   Else
    res = User.GetProperty(strPropertyName, strFormat, formatProvider, AccessingUser, AccessLevel, PropertyNotFound)
   End If
   If PropertyNotFound Then
    res = User.Profile.GetPropertyValue(strPropertyName)
   End If
   Return res
  End Function

  Public ReadOnly Property Cacheability() As DotNetNuke.Services.Tokens.CacheLevel Implements DotNetNuke.Services.Tokens.IPropertyAccess.Cacheability
   Get
    Return CacheLevel.fullyCacheable
   End Get
  End Property
#End Region

 End Class
End Namespace