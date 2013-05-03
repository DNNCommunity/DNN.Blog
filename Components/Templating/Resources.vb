Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.Tokens
Imports DotNetNuke.Services.Localization.Localization

Namespace Templating
 Public Class Resources
  Implements IPropertyAccess

  Private Property ResourcesPath As String = ""
  Private Property PrimaryResourceFile As String = ""
  Private Property SecondaryResourceFile As String = ""

#Region " Constructors "
  Public Sub New(ByVal TemplateRelPath As String)
   Me.PrimaryResourceFile = TemplateRelPath
   Me.ResourcesPath = TemplateRelPath.Substring(0, TemplateRelPath.LastIndexOf("/"))
   If Not PrimaryResourceFile.ToLower.EndsWith(".resx") Then
    Me.PrimaryResourceFile = Me.PrimaryResourceFile & ".resx"
   End If
   Me.SecondaryResourceFile = Me.ResourcesPath & "/resx/SharedResources.ascx.resx"
  End Sub
#End Region

#Region " IPropertyAccess Implementation "
  Public ReadOnly Property Cacheability() As DotNetNuke.Services.Tokens.CacheLevel Implements DotNetNuke.Services.Tokens.IPropertyAccess.Cacheability
   Get
    Return CacheLevel.fullyCacheable
   End Get
  End Property

  Public Function GetProperty(ByVal strPropertyName As String, ByVal strFormat As String, ByVal formatProvider As System.Globalization.CultureInfo, ByVal AccessingUser As DotNetNuke.Entities.Users.UserInfo, ByVal AccessLevel As DotNetNuke.Services.Tokens.Scope, ByRef PropertyNotFound As Boolean) As String Implements DotNetNuke.Services.Tokens.IPropertyAccess.GetProperty
   Dim OutputFormat As String = String.Empty
   If strFormat = String.Empty Then
    OutputFormat = "D"
   Else
    OutputFormat = strFormat
   End If
   Dim res As String = GetString(strPropertyName, PrimaryResourceFile)
   If String.IsNullOrEmpty(res) Then
    res = GetString(strPropertyName, SecondaryResourceFile)
   End If
   If res IsNot Nothing Then
    Return res
   End If
   Return Null.NullString
  End Function
#End Region

 End Class
End Namespace
