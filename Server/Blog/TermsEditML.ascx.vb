﻿'
' DNN Connect - http://dnn-connect.org
' Copyright (c) 2015
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

Imports System.Globalization
Imports DotNetNuke.Modules.Blog.Core.Common
Imports DotNetNuke.Modules.Blog.Core.Data
Imports DotNetNuke.Modules.Blog.Core.Entities.Terms
Imports DotNetNuke.Services.Localization

Public Class TermsEditML
  Inherits BlogModuleBase

  Public Property VocabularyId As Integer = 1
  Public Property ColumnHeaders As String = "[]"
  Public Property Columns As String = ""

  Private Sub Page_Init1(sender As Object, e As EventArgs) Handles Me.Init
    AddJavascriptFile("jquery.handsontable.js", "jquery.handsontable", "0.8.17", 60)
    AddCssFile("jquery.handsontable.css", "jquery.handsontable", "0.8.17")
    VocabularyId = Request.Params.ReadValue("VocabularyId", VocabularyId)
    If VocabularyId <> Settings.VocabularyId AndAlso VocabularyId <> 1 Then ' prevent users from editing another vocabulary
      VocabularyId = 1
    End If
    ColumnHeaders = "['" & (New CultureInfo(PortalSettings.DefaultLanguage)).EnglishName & "'"
    Columns = "[{data: 'DefaultName'}"
    For Each l As Locale In LocaleController.Instance.GetLocales(PortalId).Values
      If l.Code <> PortalSettings.DefaultLanguage Then
        ColumnHeaders &= ", '" & l.EnglishName & "'"
        Columns &= ", {data: 'LocNames." & l.Code & "'}"
      End If
    Next
    ColumnHeaders &= "]"
    Columns &= "]"
  End Sub

  Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

    If Not BlogContext.Security.IsEditor Then
      Throw New Exception("You do not have access to this resource. Please check your login status.")
    End If

  End Sub

  Private Sub cmdCancel_Click(sender As Object, e As EventArgs) Handles cmdCancel.Click
    Response.Redirect(EditUrl("Admin"), False)
  End Sub

  Private Sub cmdUpdate_Click(sender As Object, e As EventArgs) Handles cmdUpdate.Click
    Dim vocab As List(Of TermML) = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of TermML))(Storage.Value)
    Dim currentVocabulary As Dictionary(Of Integer, TermInfo) = TermsController.GetTermsByVocabulary(ModuleId, VocabularyId)

    For Each t As TermML In vocab
      Dim crtTerm As TermInfo = currentVocabulary(t.TermID)
      If crtTerm IsNot Nothing Then
        If crtTerm.Name <> t.DefaultName Then
          crtTerm.Name = t.DefaultName
          DotNetNuke.Entities.Content.Common.Util.GetTermController().UpdateTerm(crtTerm)
        End If
      End If
      For Each l As String In t.LocNames.Keys
        DataProvider.Instance.SetTermLocalization(t.TermID, l, t.LocNames(l), "")
      Next
    Next

    Response.Redirect(EditUrl("Admin"), False)
  End Sub
End Class