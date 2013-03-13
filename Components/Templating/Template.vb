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

Namespace Templating
 Public Class Template

#Region " Private Members "
  Public Property FileName As String = "Template.html"
  Public Property Contents As String = ""
  Public Property ViewMapPath As String = ""
  Public Property Replacer As GenericTokenReplace = Nothing
#End Region

#Region " Events "
  Private Sub Template_GetData(ByVal DataSource As String, ByVal Parameters As Dictionary(Of String, String), ByRef Replacers As List(Of GenericTokenReplace), ByRef Arguments As List(Of String()), callingObject As Object)
   RaiseEvent GetData(DataSource, Parameters, Replacers, Arguments, callingObject)
  End Sub

  Public Event GetData(ByVal DataSource As String, ByVal Parameters As Dictionary(Of String, String), ByRef Replacers As List(Of GenericTokenReplace), ByRef Arguments As List(Of String()), callingObject As Object)
#End Region

#Region " Constructors "
  Public Sub New(ByVal ViewMapPath As String, ByVal Filename As String, ByVal Replacer As GenericTokenReplace, item As TemplateRepeaterItem)
   Me.ViewMapPath = ViewMapPath
   Me.FileName = Filename
   Me.Replacer = Replacer
   Me.Replacer.AddResources(ViewMapPath & Filename)
   If item IsNot Nothing Then Me.Replacer.AddPropertySource("item", item)
  End Sub

  Public Sub New(ByVal ViewMapPath As String, ByVal Filename As String, ByVal Replacer As GenericTokenReplace, item As TemplateRepeaterItem, ByVal Arguments As String())
   Me.ViewMapPath = ViewMapPath
   Me.FileName = Filename
   Me.Replacer = Replacer
   Me.Replacer.AddCustomParameters(Arguments)
   Me.Replacer.AddResources(ViewMapPath & Filename)
   If item IsNot Nothing Then Me.Replacer.AddPropertySource("item", item)
  End Sub
#End Region

#Region " Public Methods "
  Public Function ReplaceContents() As String

   Try
    Contents = Templating.GetTemplateFile(_ViewMapPath & _FileName)
    If String.IsNullOrEmpty(Contents) Then Return ""
    ' Expand subtemplates
    ' Simple conditional template e.g. [subtemplate|Widget.html|widget:isgood|True]
    Contents = Regex.Replace(_Contents, "(?i)\[subtemplate\|([^|\]]+)\|([^:|\]]+):([^|\]]+)\|?([^|\]]+)?\](?-i)", AddressOf ReplaceConditionalTemplate)
    ' e.g. [subtemplate|Flight.html|flights|pagesize=6]
    Contents = Regex.Replace(Contents, "(?i)\[subtemplate\|([^|\]]+)\|([^|\]]+)\|?([^|\]]+)?\](?-i)", AddressOf ReplaceSubtemplates)
    If Replacer IsNot Nothing Then
     Return Replacer.ReplaceTokens(Contents)
    Else
     Return Contents
    End If
   Catch ex As Exception
    Return String.Format("<p>Error: {0}</p><p>In: {1}</p>", ex.Message, ex.StackTrace)
   End Try

  End Function
#End Region

#Region " Private Methods "
  Private Function ReplaceConditionalTemplate(ByVal m As Match) As String

   Dim file As String = m.Groups(1).Value.ToLower
   Dim conditionObject As String = m.Groups(2).Value.ToLower
   Dim conditionProperty As String = m.Groups(3).Value.ToLower
   Dim shouldRender As String = Replacer.GetTokenValue(conditionObject, conditionProperty, "")
   If String.IsNullOrEmpty(shouldRender) Then Return ""
   shouldRender = shouldRender.ToLower
   Dim compareValue As String = ""
   If m.Groups(4) IsNot Nothing AndAlso Not String.IsNullOrEmpty(m.Groups(4).Value) Then
    compareValue = m.Groups(4).Value.ToLower
    If shouldRender <> compareValue Then
     Return ""
    End If
   Else
    Select Case shouldRender
     Case "false", "no", "0"
      Return ""
    End Select
   End If

   Dim t As New Template(ViewMapPath, file, _Replacer, Nothing)
   AddHandler t.GetData, AddressOf Template_GetData
   Return t.ReplaceContents

  End Function

  Private Function ReplaceSubtemplates(ByVal m As Match) As String

   Dim file As String = m.Groups(1).Value.ToLower
   Dim datasource As String = m.Groups(2).Value.ToLower
   Dim properties() As String = {}
   If m.Groups(3) IsNot Nothing AndAlso Not String.IsNullOrEmpty(m.Groups(3).Value) Then
    properties = m.Groups(3).Value.Split(","c)
   End If
   Dim params As New Dictionary(Of String, String)
   For Each p As String In properties
    If p.IndexOf("="c) > -1 Then
     params.Add(Left(p, p.IndexOf("="c)).ToLower, Mid(p, p.IndexOf("="c) + 2))
    Else
     params.Add(p.ToLower, "")
    End If
   Next

   Dim dataSrc As New List(Of GenericTokenReplace)
   Dim args As New List(Of String())
   RaiseEvent GetData(datasource, params, dataSrc, args, Replacer.PrimaryObject)

   Dim res As New StringBuilder
   Dim totalItems As Integer = dataSrc.Count
   If totalItems = 1 And args.Count > 0 Then
    For Each arg As String() In args
     Dim tri As New TemplateRepeaterItem(totalItems, 1)
     Dim t As New Template(ViewMapPath, file, dataSrc(0), tri, arg)
     AddHandler t.GetData, AddressOf Template_GetData
     res.Append(t.ReplaceContents)
    Next
   Else
    If args.Count > 0 Then
     Dim arg As String() = args(0)
     Dim i As Integer = 1
     For Each d As GenericTokenReplace In dataSrc
      Dim tri As New TemplateRepeaterItem(totalItems, i)
      Dim t As New Template(ViewMapPath, file, d, tri, arg)
      AddHandler t.GetData, AddressOf Template_GetData
      res.Append(t.ReplaceContents)
      i += 1
     Next
    Else
     Dim i As Integer = 1
     For Each d As GenericTokenReplace In dataSrc
      Dim tri As New TemplateRepeaterItem(totalItems, i)
      Dim t As New Template(ViewMapPath, file, d, tri)
      AddHandler t.GetData, AddressOf Template_GetData
      res.Append(t.ReplaceContents)
      i += 1
     Next
    End If
   End If
   Return res.ToString

  End Function
#End Region

 End Class
End Namespace
