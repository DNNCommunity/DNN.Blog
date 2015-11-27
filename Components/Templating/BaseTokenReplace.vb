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

Imports DotNetNuke
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Entities.Profile
Imports DotNetNuke.Services.Localization
Imports System.Globalization
Imports System.Web
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Reflection
Imports DotNetNuke.UI.Utilities


Namespace Templating

 ''' <summary>
 ''' The BaseTokenReplace class provides the tokenization of tokens formatted  
 ''' [object:property] or [object:property|format|ifEmpty] or [custom:no] within a string
 ''' with the appropriate current property/custom values.
 ''' </summary>
 ''' <remarks></remarks>
 Public MustInherit Class BaseTokenReplace

#Region "Regexp Tokenizer"


  'Private Const ExpressionDefault As String = "(?:\[(?:(?<object>[^\]\[:]+):(?<property>[^\]\[\|]+))(?:\|(?:(?<format>[^\]\[]+)\|(?<ifEmpty>[^\]\[]+))|\|(?:(?<format>[^\|\]\[]+)))?\])|(?<text>\[[^\]\[]+\])|(?<text>[^\]\[]+)"
  Private Const ExpressionDefault As String = "\[(?<object>[^\]\[:\r\n]+):(?<property>[^\]\[\|\r\n]+)((\])|(\|(?<format>[^\]\[\r\n]+)((\])|(\|(?<ifEmpty>[^\]\[\r\n]+)))))"

  Private Const ExpressionObjectLess As String = "(?:\[(?:(?<object>[^\]\[:]+):(?<property>[^\]\[\|]+))(?:\|(?:(?<format>[^\]\[]+)\|(?<ifEmpty>[^\]\[]+))|\|(?:(?<format>[^\|\]\[]+)))?\])" _
              & "|(?:(?<object>\[)(?<property>[A-Z0-9._]+)(?:\|(?:(?<format>[^\]\[]+)\|(?<ifEmpty>[^\]\[]+))|\|(?:(?<format>[^\|\]\[]+)))?\])" _
              & "|(?<text>\[[^\]\[]+\])" _
              & "|(?<text>[^\]\[]+)"

  Private _UseObjectLessExpression As Boolean = False

  Protected Property UseObjectLessExpression() As Boolean
   Get
    Return _UseObjectLessExpression
   End Get
   Set(ByVal value As Boolean)
    _UseObjectLessExpression = value
   End Set
  End Property

  Private ReadOnly Property TokenReplaceCacheKey() As String
   Get
    If UseObjectLessExpression Then
     Return "Blog_TokenReplaceRegEx_Objectless"
    Else
     Return "Blog_TokenReplaceRegEx_Default"
    End If
   End Get
  End Property

  Private ReadOnly Property RegExpression() As String
   Get
    If UseObjectLessExpression Then
     Return ExpressionObjectLess
    Else
     Return ExpressionDefault
    End If
   End Get
  End Property

  Protected Const ObjectLessToken As String = "no_object"

  ''' <summary>
  ''' Gets the Regular expression for the token to be replaced
  ''' </summary>
  ''' <value>A regular Expression</value>   
  Protected ReadOnly Property TokenizerRegex() As Regex
   Get
    Dim tokenizer As Regex = CType(DataCache.GetCache(TokenReplaceCacheKey), Regex)
    If tokenizer Is Nothing Then
     tokenizer = New Regex(RegExpression, RegexOptions.Compiled)
     DataCache.SetCache(TokenReplaceCacheKey, tokenizer)
    End If
    Return tokenizer
   End Get
  End Property
#End Region

#Region "Fields & Properties"
  Private _Language As String
  Private _FormatProvider As CultureInfo

  ''' <summary>
  ''' Gets/sets the language to be used, e.g. for date format
  ''' </summary>
  ''' <value>A string, representing the locale</value>
  Public Property Language() As String
   Get
    Return _Language
   End Get
   Set(ByVal value As String)
    _Language = value
    _FormatProvider = New CultureInfo(_Language)
   End Set
  End Property

  ''' <summary>
  ''' Gets the Format provider as Culture info from stored language or current culture
  ''' </summary>
  ''' <value>An CultureInfo</value>
  Protected ReadOnly Property FormatProvider() As System.Globalization.CultureInfo
   Get
    If _FormatProvider Is Nothing Then
     _FormatProvider = System.Threading.Thread.CurrentThread.CurrentUICulture
    End If
    Return _FormatProvider
   End Get
  End Property
#End Region

  Protected Overridable Function ReplaceTokens(ByVal strSourceText As String) As String
   If strSourceText Is Nothing Then Return String.Empty
   Dim Result As New System.Text.StringBuilder
   Return TokenizerRegex.Replace(strSourceText, AddressOf ReplaceTokenMatch)
  End Function

  Private Function ReplaceTokenMatch(m As Match) As String

   Dim strObjectName As String = m.Result("${object}")
   If strObjectName.Length > 0 Then
    If strObjectName = "[" Then strObjectName = ObjectLessToken
    Dim strPropertyName As String = m.Result("${property}")
    Dim strFormat As String = m.Result("${format}")
    Dim strIfEmptyReplacment As String = m.Result("${ifEmpty}")
    Dim strConversion As String = replacedTokenValue(strObjectName, strPropertyName, strFormat)
    If strIfEmptyReplacment.Length > 0 AndAlso strConversion.Length = 0 Then strConversion = strIfEmptyReplacment
    Return strConversion
   Else
    Return m.Value
   End If

  End Function


  Protected MustOverride Function replacedTokenValue(ByVal strObjectName As String, ByVal strPropertyName As String, ByVal strFormat As String) As String

 End Class
End Namespace
