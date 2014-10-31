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
 Public MustInherit Class GenericTokenReplace
  Inherits TokenReplace

  Public Property PrimaryObject As Object = Nothing

  Public Sub New(accessLevel As DotNetNuke.Services.Tokens.Scope)
   MyBase.New(Scope.DefaultSettings)
  End Sub

  Public Sub New(accessLevel As DotNetNuke.Services.Tokens.Scope, moduleId As Integer)
   MyBase.New(Scope.DefaultSettings, moduleId)
  End Sub

  Public Shadows Function ReplaceTokens(strSourceText As String) As String
   strSourceText = strSourceText.Replace("\[", "{{").Replace("\]", "}}")
   Return MyBase.ReplaceTokens(strSourceText).Replace("{{", "[").Replace("}}", "]")
  End Function

  Public Shadows Function ReplaceTokens(strSourceText As String, ParamArray additionalParameters As String()) As String
   strSourceText = strSourceText.Replace("\[", "{{").Replace("\]", "}}")
   Me.PropertySource("custom") = New CustomParameters(additionalParameters)
   Return MyBase.ReplaceTokens(strSourceText).Replace("{{", "[").Replace("}}", "]")
  End Function

  Public Sub AddCustomParameters(ParamArray additionalParameters As String())
   Me.PropertySource("custom") = New CustomParameters(additionalParameters)
  End Sub

  Public Sub AddResources(TemplateRelPath As String)
   Me.PropertySource("resx") = New Resources(TemplateRelPath)
  End Sub

  Public Sub AddPropertySource(key As String, resource As IPropertyAccess)
   Me.PropertySource(key) = resource
  End Sub

  Public Function GetTokenValue(obj As String, prop As String, format As String) As String
   If Not Me.PropertySource.ContainsKey(obj) Then Return ""
   Dim bFound As Boolean = False
   Return Me.PropertySource(obj).GetProperty(prop, format, Threading.Thread.CurrentThread.CurrentCulture, Nothing, Scope.DefaultSettings, bFound)
  End Function

 End Class
End Namespace