Imports System.Web.UI.WebControls
Imports DotNetNuke
Imports DotNetNuke.Modules.Blog.Common.Globals

Namespace Controls

 Public Class ShortTextEdit
  Inherits LocalizedTextEdit

#Region " Private Members "
  Private pnlMain As System.Web.UI.WebControls.Panel
  Private pnlContent As System.Web.UI.WebControls.Panel
  Private txtMain As System.Web.UI.WebControls.TextBox
  Private _required As Boolean = False
  Private _regex As String = ""
#End Region

#Region " Protected Methods "
  Protected Overrides Sub CreateChildControls()

   pnlMain = New System.Web.UI.WebControls.Panel
   Me.Controls.Add(pnlMain)
   txtMain = New TextBox
   txtMain.Width = TextBoxWidth
   txtMain.ID = Me.ID & "_txtMain"
   pnlMain.Controls.Add(txtMain)

   If SupportedLocales.Count > 1 Then
    pnlBox = New System.Web.UI.WebControls.Panel
    pnlBox.CssClass = "showHideBox"
    Me.Controls.Add(pnlBox)
    pnlContent = New System.Web.UI.WebControls.Panel
    pnlContent.CssClass = "showHideContent"
    pnlBox.Controls.Add(pnlContent)
    pnlMain.Controls.Add(New LiteralControl("&nbsp;"))
    Dim img As Image
    If UseFlags Then
     img = New Image
     With img
      .ImageUrl = DotNetNuke.Common.Globals.ResolveUrl("~/images/flags/" & DefaultLanguage & ".gif")
      .AlternateText = SupportedLocales(DefaultLanguage).Text
     End With
     pnlMain.Controls.Add(img)
    Else
     pnlMain.Controls.Add(New LiteralControl(SupportedLocales(DefaultLanguage).Text))
    End If

    Dim ib As New ImageButton
    pnlMain.Controls.Add(New LiteralControl("&nbsp;"))
    pnlMain.Controls.Add(ib)
    DotNetNuke.UI.Utilities.DNNClientAPI.EnableMinMax(ib, pnlContent, -1, Not StartMaximized, DotNetNuke.Common.ResolveUrl("~/images/min.gif"), DotNetNuke.Common.ResolveUrl("~/images/max.gif"), DotNetNuke.UI.Utilities.DNNClientAPI.MinMaxPersistanceType.None)

    For Each localeCode As String In SupportedLocales
     If Not localeCode = DefaultLanguage Then
      Dim tb As New TextBox
      tb.ID = Me.ID & "_txt" & localeCode
      tb.Width = Me.TextBoxWidth
      pnlContent.Controls.Add(tb)
      pnlContent.Controls.Add(New LiteralControl("&nbsp;"))
      If UseFlags Then
       Dim i As New Image
       i.ImageUrl = DotNetNuke.Common.Globals.ResolveUrl("~/images/flags/" & localeCode & ".gif")
       i.AlternateText = SupportedLocales(localeCode).Text
       pnlContent.Controls.Add(i)
      Else
       pnlContent.Controls.Add(New LiteralControl(SupportedLocales(localeCode).Text))
      End If
      If Regex <> "" Then
       Dim rv As New RegularExpressionValidator
       With rv
        .Display = ValidatorDisplay.Dynamic
        .ErrorMessage = "<br />" & DotNetNuke.Services.Localization.Localization.GetString("Expression.Error", SharedResourceFileName)
        .CssClass = "NormalRed"
        .ControlToValidate = tb.ID
        .ValidationExpression = Regex
       End With
       pnlContent.Controls.Add(rv)
      End If
      pnlContent.Controls.Add(New LiteralControl("<br />"))
     End If
    Next
   End If

   If Required Then

    Dim rv As New RequiredFieldValidator
    With rv
     .Display = ValidatorDisplay.Dynamic
     .ErrorMessage = "<br />" & DotNetNuke.Services.Localization.Localization.GetString("Required.Error", SharedResourceFileName)
     .CssClass = "NormalRed"
     .ControlToValidate = txtMain.ID
    End With
    pnlMain.Controls.Add(rv)

   End If

   If Regex <> "" Then

    Dim rv As New RegularExpressionValidator
    With rv
     .Display = ValidatorDisplay.Dynamic
     .ErrorMessage = "<br />" & DotNetNuke.Services.Localization.Localization.GetString("Expression.Error", SharedResourceFileName)
     .CssClass = "NormalRed"
     .ControlToValidate = txtMain.ID
     .ValidationExpression = Regex
    End With
    pnlMain.Controls.Add(rv)

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
  Public Property Required() As Boolean
   Get
    Return _required
   End Get
   Set(ByVal value As Boolean)
    _required = value
   End Set
  End Property

  Public Property Regex() As String
   Get
    Return _regex
   End Get
   Set(ByVal value As String)
    _regex = value
   End Set
  End Property
#End Region

 End Class
End Namespace
