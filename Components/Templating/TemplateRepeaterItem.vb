'
' Bring2mind - http://www.bring2mind.net
' Copyright (c) 2012
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
Imports DotNetNuke.Services.Tokens
Imports DotNetNuke.Services.Localization.Localization

Namespace Templating
 Public Class TemplateRepeaterItem
  Implements IPropertyAccess

#Region " Properties "
  Public Property IsFirst As Boolean
  Public Property IsLast As Boolean
  Public Property IsEven As Boolean
  Public Property IsOdd As Boolean
  Public Property Index As Integer
#End Region

#Region " Constructors "
  Public Sub New(total As Integer, index As Integer)
   Me.Index = index
   IsFirst = CBool(index = 1)
   IsLast = CBool(index = total)
   IsEven = CBool(index Mod 2 = 0)
   IsOdd = Not IsEven
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
     Select strPropertyName.ToLower
    Case "isfirst", "first"
     Return Common.Globals.FormatBoolean(Me.IsFirst, strFormat)
    Case "islast", "last"
     Return Common.Globals.FormatBoolean(Me.IsLast, strFormat)
    Case "iseven", "even"
     Return Common.Globals.FormatBoolean(Me.IsEven, strFormat)
    Case "isodd", "odd"
     Return Common.Globals.FormatBoolean(Me.IsOdd, strFormat)
    Case "index"
     Return (Me.Index.ToString(OutputFormat, formatProvider))
   End Select
   Return Null.NullString
  End Function
#End Region

 End Class
End Namespace
