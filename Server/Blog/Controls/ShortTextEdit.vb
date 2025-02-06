'
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

Namespace Controls

  Public Class ShortTextEdit
    Inherits LocalizedTextEdit

#Region " Private Members "
    Private txtMain As System.Web.UI.WebControls.TextBox
    Private pnlMain As System.Web.UI.WebControls.Table
    Private pnlContent As System.Web.UI.WebControls.Table
#End Region

#Region " Protected Methods "
    Protected Overrides Sub CreateChildControls()

      Controls.Clear()
      pnlMain = New System.Web.UI.WebControls.Table
      pnlMain.ID = ID & "_mainTable"
      pnlMain.CssClass = CssPrefix & "default"
      Controls.Add(pnlMain)
      Dim tr As New TableRow
      pnlMain.Rows.Add(tr)
      Dim td As New TableCell
      td.CssClass = CssPrefix & "defaulttext"
      tr.Cells.Add(td)

      txtMain = New TextBox
      If TextBoxWidth <> Unit.Pixel(0) Then txtMain.Width = TextBoxWidth
      txtMain.ID = ID & "_txtMain"
      td.Controls.Add(txtMain)

      If SupportedLocales.Count > 1 And ShowTranslations Then
        td = New TableCell
        td.CssClass = CssPrefix & "defflag"
        tr.Cells.Add(td)
        Dim img As WebControls.Image
        If UseFlags Then
          img = New WebControls.Image
          With img
            .ImageUrl = DotNetNuke.Common.Globals.ResolveUrl("~/images/flags/" & DefaultLanguage & ".gif")
            .AlternateText = SupportedLocales(DefaultLanguage).Text
          End With
          td.Controls.Add(img)
        Else
          td.Controls.Add(New LiteralControl(SupportedLocales(DefaultLanguage).Text))
        End If
        Dim ib As New ImageButton
        ib.ID = ID & "_cmdMinmax"
        td.Controls.Add(New LiteralControl("&nbsp;"))
        td.Controls.Add(ib)

        ' Make the dropdown box
        pnlBox = New System.Web.UI.WebControls.Panel
        pnlBox.CssClass = CssPrefix & "minmaxbox"
        Controls.Add(pnlBox)
        pnlContent = New System.Web.UI.WebControls.Table
        pnlContent.ID = ID & "_contentTable"
        pnlContent.CssClass = CssPrefix & "localizations"
        pnlBox.Controls.Add(pnlContent)
        DotNetNuke.UI.Utilities.DNNClientAPI.EnableMinMax(ib, pnlContent, -1, Not StartMaximized, DotNetNuke.Common.ResolveUrl("~/images/min.gif"), DotNetNuke.Common.ResolveUrl("~/images/max.gif"), DotNetNuke.UI.Utilities.DNNClientAPI.MinMaxPersistanceType.None)

        For Each localeCode As String In SupportedLocales
          If Not localeCode = DefaultLanguage Then
            ' Add the row
            tr = New TableRow
            pnlContent.Rows.Add(tr)
            td = New TableCell
            td.CssClass = CssPrefix & "localization"
            tr.Cells.Add(td)

            Dim tb As New TextBox
            tb.ID = ID & "_txt" & localeCode
            If TextBoxWidth <> Unit.Pixel(0) Then tb.Width = TextBoxWidth
            td.Controls.Add(tb)
            td = New TableCell
            td.CssClass = CssPrefix & "flag"
            tr.Cells.Add(td)
            If UseFlags Then
              Dim i As New WebControls.Image
              i.ImageUrl = DotNetNuke.Common.ResolveUrl("~/images/flags/" & localeCode & ".gif")
              i.AlternateText = SupportedLocales(localeCode).Text
              td.Controls.Add(i)
            Else
              td.Controls.Add(New LiteralControl(SupportedLocales(localeCode).Text))
            End If

            If Regex <> "" Then
              ' Add the row
              tr = New TableRow
              pnlContent.Rows.Add(tr)
              td = New TableCell
              tr.Cells.Add(td)
              Dim rv As New RegularExpressionValidator
              With rv
                .Display = ValidatorDisplay.Dynamic
                .ErrorMessage = "<br />" & DotNetNuke.Services.Localization.Localization.GetString("Expression.Error", Core.Common.Globals.SharedResourceFileName)
                .CssClass = "NormalRed"
                .ControlToValidate = tb.ID
                .ValidationExpression = Regex
              End With
              td.Controls.Add(rv)
            End If

          End If
        Next
      End If

      If Required Then

        tr = New TableRow
        pnlMain.Rows.Add(tr)
        td = New TableCell
        tr.Cells.Add(td)
        Dim rv As New RequiredFieldValidator
        With rv
          .Display = ValidatorDisplay.Dynamic
          .ErrorMessage = String.Format("<div class=""dnnFormMessage dnnFormError"">{0}</div>", DotNetNuke.Services.Localization.Localization.GetString(RequiredResourceKey, Me))
          .CssClass = "NormalRed"
          .ControlToValidate = txtMain.ID
        End With
        td.Controls.Add(rv)

      End If

      If Regex <> "" Then

        tr = New TableRow
        pnlMain.Rows.Add(tr)
        td = New TableCell
        tr.Cells.Add(td)
        Dim rv As New RegularExpressionValidator
        With rv
          .Display = ValidatorDisplay.Dynamic
          .ErrorMessage = String.Format("<div class=""dnnFormMessage dnnFormError"">{0}</div>", DotNetNuke.Services.Localization.Localization.GetString(RegexResourceKey, Me))
          .CssClass = "NormalRed"
          .ControlToValidate = txtMain.ID
          .ValidationExpression = Regex
        End With
        td.Controls.Add(rv)

      End If

    End Sub
#End Region

#Region " Public Methods "
    Public Overrides Sub Update()

      If JustUpdated Then Exit Sub
      Try
        DefaultText = txtMain.Text
      Catch ex As Exception
      End Try
      If ShowTranslations Then
        For Each localeCode As String In SupportedLocales
          If Not localeCode = DefaultLanguage Then
            Try
              Dim txtBox As TextBox = CType(pnlContent.FindControlByID(ID & "_txt" & localeCode), TextBox)
              If LocalizedTexts.ContainsKey(localeCode) Then
                LocalizedTexts(localeCode) = txtBox.Text.Trim
              Else
                LocalizedTexts.Add(localeCode, txtBox.Text.Trim)
              End If
            Catch ex As Exception
            End Try
          End If
        Next
      End If
      JustUpdated = True

    End Sub

    Public Overrides Sub Rebind()
      Try
        txtMain.Text = DefaultText
      Catch ex As Exception
      End Try
      If ShowTranslations Then
        For Each localeCode As String In SupportedLocales
          If Not localeCode = DefaultLanguage Then
            Try
              CType(pnlContent.FindControlByID(ID & "_txt" & localeCode), TextBox).Text = LocalizedTexts(localeCode)
            Catch ex As Exception
            End Try
          End If
        Next
      End If
    End Sub
#End Region

#Region " Properties "
    Public Property Required As Boolean = False
    Public Property RequiredResourceKey As String = "Required.Error"
    Public Property Regex As String = ""
    Public Property RegexResourceKey As String = "Expression.Error"
#End Region

  End Class
End Namespace
