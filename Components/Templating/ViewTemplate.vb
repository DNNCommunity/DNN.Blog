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

Imports DotNetNuke.Services.Localization.Localization
Imports DotNetNuke.Services.Tokens
Imports DotNetNuke.Web.Client.ClientResourceManagement

Namespace Templating
 Public Class ViewTemplate
  Inherits UserControl

#Region " Properties "
  Public Property Template As Template
  Public Property TemplatePath As String = ""
  Public Property TemplateRelPath As String = ""
  Public Property TemplateMapPath As String = ""
  Public Property DefaultReplacer As GenericTokenReplace
  Public Property StartTemplate As String = "Template.html"
  Private Property ViewPath As String = ""
  Private Property ViewMapPath As String = ""
#End Region

#Region " Public Methods "
  Public Function GetContentsAsString() As String
   Dim sb As New StringBuilder
   Using tw As New IO.StringWriter(sb)
    Using w As New System.Web.UI.HtmlTextWriter(tw)
     Render(w)
     w.Flush()
    End Using
   End Using
   Return sb.ToString
  End Function
#End Region

#Region " Overrides "
  Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
   If Template IsNot Nothing Then
    writer.Write(Template.ReplaceContents)
   End If
  End Sub

  Public Overrides Sub DataBind()

   ViewMapPath = TemplateMapPath
   ViewPath = TemplatePath

   Dim dataSrc As New List(Of GenericTokenReplace)
   Dim args As New List(Of String())
   Dim params As New Dictionary(Of String, String)

   RaiseEvent GetData("", params, dataSrc, args, Nothing)
   If dataSrc.Count > 0 Then
    Template = New Template(ViewMapPath, TemplateRelPath, StartTemplate, dataSrc(0), Nothing)
   Else
    Template = New Template(ViewMapPath, TemplateRelPath, StartTemplate, DefaultReplacer, Nothing)
   End If
   AddHandler Template.GetData, AddressOf Template_GetData

  End Sub
#End Region

#Region " Events "
  Private Sub Template_GetData(ByVal DataSource As String, ByVal Parameters As Dictionary(Of String, String), ByRef Replacers As List(Of GenericTokenReplace), ByRef Arguments As List(Of String()), callingObject As Object)
   RaiseEvent GetData(DataSource, Parameters, Replacers, Arguments, callingObject)
  End Sub

  Public Event GetData(ByVal DataSource As String, ByVal Parameters As Dictionary(Of String, String), ByRef Replacers As List(Of GenericTokenReplace), ByRef Arguments As List(Of String()), callingObject As Object)
#End Region

#Region " Event Handlers "
  Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

   'hook in css files
   If IO.File.Exists(TemplateMapPath & "template.css") Then
    ClientResourceManager.RegisterStyleSheet(Me.Page, TemplatePath & "template.css")
   End If
   If IO.Directory.Exists(TemplateMapPath & "css") Then
    For Each f As IO.FileInfo In (New IO.DirectoryInfo(TemplateMapPath & "css")).GetFiles("*.css")
     ClientResourceManager.RegisterStyleSheet(Me.Page, TemplatePath & "css/" & f.Name)
    Next
   End If
   If IO.Directory.Exists(ViewMapPath & "css") Then
    For Each f As IO.FileInfo In (New IO.DirectoryInfo(ViewMapPath & "css")).GetFiles("*.css")
     ClientResourceManager.RegisterStyleSheet(Me.Page, ViewPath & "css/" & f.Name)
    Next
   End If
   'hook in js files
   If IO.File.Exists(TemplateMapPath & "template.js") Then
    ClientResourceManager.RegisterScript(Me.Page, TemplatePath & "template.js")
   End If
   If IO.Directory.Exists(TemplateMapPath & "js") Then
    For Each f As IO.FileInfo In (New IO.DirectoryInfo(TemplateMapPath & "js")).GetFiles("*.js")
     ClientResourceManager.RegisterScript(Me.Page, TemplatePath & "js/" & f.Name)
    Next
   End If
   If IO.Directory.Exists(ViewMapPath & "js") Then
    For Each f As IO.FileInfo In (New IO.DirectoryInfo(ViewMapPath & "js")).GetFiles("*.js")
     ClientResourceManager.RegisterScript(Me.Page, ViewPath & "js/" & f.Name)
    Next
   End If
   ' localized js files?
   Dim locale As String = Threading.Thread.CurrentThread.CurrentCulture.Name.ToLower
   If IO.Directory.Exists(ViewMapPath & "js\" & locale) Then
    For Each f As IO.FileInfo In (New IO.DirectoryInfo(ViewMapPath & "js\" & locale)).GetFiles("*.js")
     ClientResourceManager.RegisterScript(Me.Page, ViewPath & "js/" & locale & "/" & f.Name)
    Next
   Else ' check generic culture
    locale = Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower
    If IO.Directory.Exists(ViewMapPath & "js\" & locale) Then
     For Each f As IO.FileInfo In (New IO.DirectoryInfo(ViewMapPath & "js\" & locale)).GetFiles("*.js")
      ClientResourceManager.RegisterScript(Me.Page, ViewPath & "js/" & locale & "/" & f.Name)
     Next
    End If
   End If
   ' add js blocks
   If IO.Directory.Exists(ViewMapPath & "jsblocks") Then
    For Each f As IO.FileInfo In (New IO.DirectoryInfo(ViewMapPath & "jsblocks")).GetFiles("*.js")
     Dim t As New Template(ViewMapPath & "jsblocks\", ViewPath, f.Name, DefaultReplacer, Nothing)
     Dim s As String = t.ReplaceContents
     If Not s.StartsWith("<") Then
      s = String.Format("<script type=""text/javascript"">{0}//<![CDATA[{0}{1}//]]>{0}</script>", vbCrLf, s)
     End If
     Page.ClientScript.RegisterClientScriptBlock(GetType(String), f.Name, s)
    Next
   End If

  End Sub
#End Region

 End Class
End Namespace