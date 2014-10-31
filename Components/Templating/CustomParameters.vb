'
' DNN Connect - http://dnn-connect.org
' Copyright (c) 2014
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
