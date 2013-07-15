'
' Bring2mind - http://www.bring2mind.net
' Copyright (c) 2013
' by Bring2mind
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

  Public Sub New(portalId As Integer, userId As Integer, userName As String)
   Me.PortalId = portalId
   Me.UserId = userId
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
   Select strPropertyName.ToLower
    Case "profilepic"
     If IsNumeric(strFormat) Then
      Return DotNetNuke.Common.ResolveUrl(String.Format("~/profilepic.ashx?userid={0}&w={1}&h={1}", UserId, strFormat))
     Else
      Return DotNetNuke.Common.ResolveUrl(String.Format("~/profilepic.ashx?userid={0}&w={1}&h={1}", UserId, 50))
     End If
    Case "profileurl"
     Return DotNetNuke.Common.Globals.UserProfileURL(UserId)
   End Select
   If _postAuthor IsNot Nothing Then
    Return _postAuthor.GetProperty(strPropertyName, strFormat, formatProvider, AccessingUser, AccessLevel, PropertyNotFound)
   Else
    Return User.GetProperty(strPropertyName, strFormat, formatProvider, AccessingUser, AccessLevel, PropertyNotFound)
   End If
   If PropertyNotFound Then
    Dim res As String = User.Profile.GetPropertyValue(strPropertyName)
    If Not String.IsNullOrEmpty(res) Then Return res
   End If
   PropertyNotFound = True
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