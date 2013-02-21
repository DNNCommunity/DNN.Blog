Imports DotNetNuke.Services.Tokens

Namespace Templating
 Public Class GenericTokenReplace
  Inherits TokenReplace

  Public Sub New(ByVal AccessLevel As DotNetNuke.Services.Tokens.Scope)
   MyBase.New(Scope.DefaultSettings)
  End Sub

  Public Shadows Function ReplaceTokens(ByVal strSourceText As String) As String
   strSourceText = strSourceText.Replace("\[", "{{").Replace("\]", "}}")
   Return MyBase.ReplaceTokens(strSourceText).Replace("{{", "[").Replace("}}", "]")
  End Function

  Public Shadows Function ReplaceTokens(ByVal strSourceText As String, ByVal ParamArray additionalParameters As String()) As String
   strSourceText = strSourceText.Replace("\[", "{{").Replace("\]", "}}")
   Me.PropertySource("custom") = New CustomParameters(additionalParameters)
   Return MyBase.ReplaceTokens(strSourceText).Replace("{{", "[").Replace("}}", "]")
  End Function

  Public Sub AddCustomParameters(ByVal ParamArray additionalParameters As String())
   Me.PropertySource("custom") = New CustomParameters(additionalParameters)
  End Sub

  Public Sub AddResources(ByVal TemplateFileMapPath As String)
   Me.PropertySource("resx") = New Resources(TemplateFileMapPath)
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