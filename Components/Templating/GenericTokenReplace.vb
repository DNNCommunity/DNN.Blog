Imports DotNetNuke.Services.Tokens

Namespace Templating
 Public Class GenericTokenReplace
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