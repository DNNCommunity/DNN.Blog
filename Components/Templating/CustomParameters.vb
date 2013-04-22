Imports DotNetNuke.Services.Tokens

Namespace Templating
 Public Class CustomParameters
  Implements IPropertyAccess

#Region " Private Members "
  Private Property params As NameValueCollection
#End Region

#Region " Constructors "
  Public Sub New(ByVal ParamArray customParameters As String())
   params = New NameValueCollection
   For Each sParameter As String In customParameters
    params.Add(Left(sParameter.ToLower, sParameter.IndexOf("="c)), Mid(sParameter, sParameter.IndexOf("="c) + 2))
   Next
  End Sub
#End Region

#Region " IPropertyAccess "
  Public ReadOnly Property Cacheability() As DotNetNuke.Services.Tokens.CacheLevel Implements DotNetNuke.Services.Tokens.IPropertyAccess.Cacheability
   Get
    Return CacheLevel.notCacheable
   End Get
  End Property

  Public Function GetProperty(ByVal strPropertyName As String, ByVal strFormat As String, ByVal formatProvider As System.Globalization.CultureInfo, ByVal AccessingUser As DotNetNuke.Entities.Users.UserInfo, ByVal AccessLevel As DotNetNuke.Services.Tokens.Scope, ByRef PropertyNotFound As Boolean) As String Implements DotNetNuke.Services.Tokens.IPropertyAccess.GetProperty
   Dim OutputFormat As String = String.Empty
   If strFormat = String.Empty Then
    OutputFormat = "D"
   Else
    OutputFormat = strFormat
   End If
   If params(strPropertyName.ToLower) Is Nothing Then Return DotNetNuke.Common.Utilities.Null.NullString
   Dim value As String = params(strPropertyName.ToLower)
   Try
    If IsNumeric(value) Then
     Return CDbl(value).ToString(OutputFormat, formatProvider)
    End If
    If IsDate(value) Then
     Return CDate(value).ToString(OutputFormat, formatProvider)
    End If
    Return Common.Globals.FormatBoolean(CBool(value), strFormat)
   Catch ex As Exception
    Return value
   End Try
  End Function
#End Region

 End Class
End Namespace
