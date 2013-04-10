Imports System.Web.UI.WebControls
Imports DotNetNuke
Imports DotNetNuke.Modules.Blog.Common

Namespace Controls

 Public Class LongTextEdit
  Inherits LocalizedTextEdit

#Region " Private Members "
  Private _showRichTextBox As Boolean = False
  Private pnlMain As System.Web.UI.WebControls.Table
  Private pnlContent As System.Web.UI.WebControls.Table
#End Region

#Region " Protected Methods "
  Protected Overrides Sub CreateChildControls()

   Me.Controls.Clear()
   pnlMain = New System.Web.UI.WebControls.Table
   pnlMain.ID = Me.ID & "_mainTable"
   Me.Controls.Add(pnlMain)
   Dim tr As New TableRow
   pnlMain.Rows.Add(tr)
   Dim td As New TableCell
   tr.Cells.Add(td)

   If Me.ShowRichTextBox Then
    Dim t As New DotNetNuke.Web.UI.WebControls.DnnTextBox
    With t
     .ID = Me.ID & "_txtMain"
     .Width = Me.TextBoxWidth
     .Height = Me.TextBoxHeight
     .EnableViewState = True
    End With
    td.Controls.Add(t)
   Else
    Dim txtMain As New TextBox
    With txtMain
     .ID = Me.ID & "_txtMain"
     .Width = Me.TextBoxWidth
     .Height = Me.TextBoxHeight
     .TextMode = TextBoxMode.MultiLine
    End With
    td.Controls.Add(txtMain)
   End If

   If SupportedLocales.Count > 1 Then
    ' Create max/min cell
    td = New TableCell
    td.VerticalAlign = VerticalAlign.Top
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
    pnlBox.CssClass = "showHideBox"
    Me.Controls.Add(pnlBox)
    pnlContent = New System.Web.UI.WebControls.Table
    pnlContent.ID = Me.ID & "_contentTable"
    pnlContent.CssClass = "showHideContent"
    pnlBox.Controls.Add(pnlContent)
    DotNetNuke.UI.Utilities.DNNClientAPI.EnableMinMax(ib, pnlContent, -1, Not StartMaximized, DotNetNuke.Common.ResolveUrl("~/images/min.gif"), DotNetNuke.Common.ResolveUrl("~/images/max.gif"), DotNetNuke.UI.Utilities.DNNClientAPI.MinMaxPersistanceType.None)

    For Each localeCode As String In SupportedLocales
     If Not localeCode = DefaultLanguage Then
      ' Add the row
      tr = New TableRow
      pnlContent.Rows.Add(tr)
      td = New TableCell
      tr.Cells.Add(td)
      If Me.ShowRichTextBox Then
       Dim t As New DotNetNuke.Web.UI.WebControls.DnnTextBox
       With t
        .ID = Me.ID & "_txt" & localeCode
        .Width = Me.TextBoxWidth
        .Height = Me.TextBoxHeight
        .EnableViewState = True
       End With
       td.Controls.Add(t)
      Else
       Dim tb As New TextBox
       With tb
        .ID = Me.ID & "_txt" & localeCode
        .Width = Me.TextBoxWidth
        .Height = Me.TextBoxHeight
        .TextMode = TextBoxMode.MultiLine
       End With
       td.Controls.Add(tb)
      End If
      td = New TableCell
      td.VerticalAlign = VerticalAlign.Top
      tr.Cells.Add(td)
      If UseFlags Then
       Dim i As New WebControls.Image
       i.ImageUrl = DotNetNuke.Common.ResolveUrl("~/images/flags/" & localeCode & ".gif")
       i.AlternateText = SupportedLocales(localeCode).Text
       td.Controls.Add(i)
      Else
       td.Controls.Add(New LiteralControl(SupportedLocales(localeCode).Text))
      End If
     End If
    Next
   End If

  End Sub
#End Region

#Region " Public Methods "
  Public Overrides Sub Update()

   If JustUpdated Then Exit Sub
   Try
    If Me.ShowRichTextBox Then
     Dim txtMain As DotNetNuke.Web.UI.WebControls.DnnTextBox = CType(pnlMain.FindControlByID(Me.ID & "_txtMain"), DotNetNuke.Web.UI.WebControls.DnnTextBox)
     Dim returnText As String = CStr(txtMain.Text)
     If returnText <> "<p>&#160;</p>" Then
      DefaultText = returnText
     End If
    Else
     Dim txtMain As TextBox = CType(pnlMain.FindControlByID(Me.ID & "_txtMain"), TextBox)
     DefaultText = txtMain.Text
    End If
   Catch ex As Exception
   End Try
   For Each localeCode As String In SupportedLocales
    If Not localeCode = DefaultLanguage Then
     Try
      If Me.ShowRichTextBox Then
       Dim txtBox As DotNetNuke.Web.UI.WebControls.DnnTextBox = CType(pnlContent.FindControlByID(Me.ID & "_txt" & localeCode), DotNetNuke.Web.UI.WebControls.DnnTextBox)
       If LocalizedTexts.ContainsKey(localeCode) Then
        LocalizedTexts(localeCode) = CStr(txtBox.Text)
       Else
        LocalizedTexts.Add(localeCode, CStr(txtBox.Text))
       End If
      Else
       Dim txtBox As TextBox = CType(pnlContent.FindControlByID(Me.ID & "_txt" & localeCode), TextBox)
       If LocalizedTexts.ContainsKey(localeCode) Then
        LocalizedTexts(localeCode) = txtBox.Text.Trim
       Else
        LocalizedTexts.Add(localeCode, txtBox.Text.Trim)
       End If
      End If
     Catch ex As Exception
     End Try
    End If
   Next
   JustUpdated = True

  End Sub

  Public Overrides Sub Rebind()
   Try
    If Me.ShowRichTextBox Then
     Dim txtMain As DotNetNuke.Web.UI.WebControls.DnnTextBox = CType(pnlMain.FindControlByID(Me.ID & "_txtMain"), DotNetNuke.Web.UI.WebControls.DnnTextBox)
     txtMain.Text = DefaultText
    Else
     Dim txtMain As TextBox = CType(pnlMain.FindControlByID(Me.ID & "_txtMain"), TextBox)
     txtMain.Text = DefaultText
    End If
   Catch ex As Exception
   End Try
   For Each localeCode As String In SupportedLocales
    If Not localeCode = DefaultLanguage Then
     If Me.ShowRichTextBox Then
      CType(pnlContent.FindControlByID(Me.ID & "_txt" & localeCode), DotNetNuke.Web.UI.WebControls.DnnTextBox).Text = LocalizedTexts(localeCode)
     Else
      CType(pnlContent.FindControlByID(Me.ID & "_txt" & localeCode), TextBox).Text = LocalizedTexts(localeCode)
     End If
    End If
   Next
  End Sub
#End Region

#Region " Properties "
  Public Property ShowRichTextBox() As Boolean
   Get
    Return _showRichTextBox
   End Get
   Set(ByVal value As Boolean)
    _showRichTextBox = value
   End Set
  End Property
#End Region

 End Class
End Namespace
