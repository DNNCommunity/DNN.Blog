Imports System.Web.UI.WebControls
Imports DotNetNuke
Imports DotNetNuke.Modules.Blog.Common.Globals

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

   Me.Controls.Clear()
   pnlMain = New System.Web.UI.WebControls.Table
   pnlMain.ID = Me.ID & "_mainTable"
   pnlMain.CssClass = CssPrefix & "default"
   Me.Controls.Add(pnlMain)
   Dim tr As New TableRow
   pnlMain.Rows.Add(tr)
   Dim td As New TableCell
   td.CssClass = CssPrefix & "defaulttext"
   tr.Cells.Add(td)

   txtMain = New TextBox
   If Me.TextBoxWidth <> Unit.Pixel(0) Then txtMain.Width = TextBoxWidth
   txtMain.ID = Me.ID & "_txtMain"
   td.Controls.Add(txtMain)

   If SupportedLocales.Count > 1 Then
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
    td.Controls.Add(New LiteralControl("&nbsp;"))
    td.Controls.Add(ib)

    ' Make the dropdown box
    pnlBox = New System.Web.UI.WebControls.Panel
    pnlBox.CssClass = CssPrefix & "minmaxbox"
    Me.Controls.Add(pnlBox)
    pnlContent = New System.Web.UI.WebControls.Table
    pnlContent.ID = Me.ID & "_contentTable"
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
      tb.ID = Me.ID & "_txt" & localeCode
      If Me.TextBoxWidth <> Unit.Pixel(0) Then tb.Width = Me.TextBoxWidth
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
        .ErrorMessage = "<br />" & DotNetNuke.Services.Localization.Localization.GetString("Expression.Error", SharedResourceFileName)
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
     .ErrorMessage = "<br />" & DotNetNuke.Services.Localization.Localization.GetString("Required.Error", SharedResourceFileName)
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
     .ErrorMessage = "<br />" & DotNetNuke.Services.Localization.Localization.GetString("Expression.Error", SharedResourceFileName)
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
   For Each localeCode As String In SupportedLocales
    If Not localeCode = DefaultLanguage Then
     Try
      Dim txtBox As TextBox = CType(pnlContent.FindControlByID(Me.ID & "_txt" & localeCode), TextBox)
      If LocalizedTexts.ContainsKey(localeCode) Then
       LocalizedTexts(localeCode) = txtBox.Text.Trim
      Else
       LocalizedTexts.Add(localeCode, txtBox.Text.Trim)
      End If
     Catch ex As Exception
     End Try
    End If
   Next
   JustUpdated = True

  End Sub

  Public Overrides Sub Rebind()
   Try
    txtMain.Text = DefaultText
   Catch ex As Exception
   End Try
   For Each localeCode As String In SupportedLocales
    If Not localeCode = DefaultLanguage Then
     Try
      CType(pnlContent.FindControlByID(Me.ID & "_txt" & localeCode), TextBox).Text = LocalizedTexts(localeCode)
     Catch ex As Exception
     End Try
    End If
   Next
  End Sub
#End Region

#Region " Properties "
  Public Property Required As Boolean = False
  Public Property Regex As String = ""
#End Region

 End Class
End Namespace
