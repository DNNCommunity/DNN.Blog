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
Imports DotNetNuke.Modules.Blog.Templating

Namespace Controls
 Public Class TemplateSettings
  Inherits BlogModuleBase

  Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

   Dim tmgr As New TemplateManager(PortalSettings, ViewSettings.Template)
   divMessage.Visible = False
   If tmgr.TemplateSettings.Settings.Count = 0 Then
    divMessage.InnerText = LocalizeString("NoSettings")
    divMessage.Visible = True
   End If
   For Each ts As TemplateSetting In tmgr.TemplateSettings.Settings
    Dim tr As New TableRow
    Dim td As New TableCell
    td.CssClass = "SubHead"
    td.Width = Unit.Pixel(165)
    Dim dnnl As DotNetNuke.UI.UserControls.LabelControl = CType(Me.LoadControl(ResolveUrl("~/controls/LabelControl.ascx")), DotNetNuke.UI.UserControls.LabelControl)
    dnnl.Suffix = ":"
    dnnl.Text = GetString(ts.Key & ".Text", tmgr.SharedResourcesFile)
    dnnl.HelpText = GetString(ts.Key & ".Help", tmgr.SharedResourcesFile)
    dnnl.ControlName = "txt" & ts.Key
    td.Controls.Add(dnnl)
    tr.Cells.Add(td)
    td = New TableCell
    Select Case ts.ValueType.ToLower
     Case "dropdown"
      Dim dd As New DropDownList
      dd.ID = ts.Key
      For Each v As TemplateSettingValue In ts.Values
       Dim txt As String = GetString(v.Value, tmgr.SharedResourcesFile)
       If String.IsNullOrEmpty(txt) Then txt = v.Value
       dd.Items.Add(New ListItem(txt, v.Value))
      Next
      td.Controls.Add(dd)
     Case "integer"
      Dim tb As New TextBox
      tb.ID = ts.Key
      tb.Width = Unit.Pixel(50)
      td.Controls.Add(tb)
     Case "number"
      Dim tb As New TextBox
      tb.ID = ts.Key
      tb.Width = Unit.Pixel(50)
      td.Controls.Add(tb)
     Case "truefalse", "10"
      Dim chk As New CheckBox
      chk.ID = ts.Key
      td.Controls.Add(chk)
     Case Else
      Dim tb As New TextBox
      tb.ID = ts.Key
      tb.Width = Unit.Pixel(300)
      td.Controls.Add(tb)
    End Select
    tr.Cells.Add(td)
    tblSettings.Rows.Add(tr)
   Next

   If Not Me.IsPostBack Then
    DataBind()
   End If

  End Sub

  Public Overrides Sub DataBind()

   Dim tmgr As New TemplateManager(PortalSettings, ViewSettings.Template)
   Dim rowIndex As Integer = 0
   For Each ts As TemplateSetting In tmgr.TemplateSettings.Settings

    Dim tr As TableRow = tblSettings.Rows.Item(rowIndex)
    Dim value As String = ts.DefaultValue
    If ViewSettings.TemplateSettings.ContainsKey(ts.Key) Then value = ViewSettings.TemplateSettings(ts.Key)

    Select Case ts.ValueType.ToLower
     Case "dropdown"
      Dim dd As DropDownList = CType(tr.Cells(1).Controls(0), DropDownList)
      Try
       dd.Items.FindByValue(value).Selected = True
      Catch ex As Exception
      End Try
     Case "integer"
      Dim tb As TextBox = CType(tr.Cells(1).Controls(0), TextBox)
      tb.Text = value
     Case "number"
      Dim tb As TextBox = CType(tr.Cells(1).Controls(0), TextBox)
      tb.Text = value
     Case "truefalse", "10"
      Dim chk As CheckBox = CType(tr.Cells(1).Controls(0), CheckBox)
      Try
       chk.Checked = CBool(value)
      Catch ex As Exception
      End Try
     Case Else
      Dim tb As TextBox = CType(tr.Cells(1).Controls(0), TextBox)
      tb.Text = value
    End Select

    rowIndex += 1

   Next

  End Sub

  Private Sub cmdCancel_Click(sender As Object, e As System.EventArgs) Handles cmdCancel.Click
   Response.Redirect(DotNetNuke.Common.NavigateURL, False)
  End Sub

  Private Sub cmdUpdate_Click(sender As Object, e As System.EventArgs) Handles cmdUpdate.Click

   Dim tmgr As New TemplateManager(PortalSettings, ViewSettings.Template)
   Dim rowIndex As Integer = 0
   For Each ts As TemplateSetting In tmgr.TemplateSettings.Settings

    Dim tr As TableRow = tblSettings.Rows.Item(rowIndex)

    Select Case ts.ValueType.ToLower
     Case "dropdown"
      Dim dd As DropDownList = CType(tr.Cells(1).Controls(0), DropDownList)
      ViewSettings.SetTemplateSetting(ts.Key, dd.SelectedValue)
     Case "integer"
      Dim tb As TextBox = CType(tr.Cells(1).Controls(0), TextBox)
      ViewSettings.SetTemplateSetting(ts.Key, tb.Text)
     Case "number"
      Dim tb As TextBox = CType(tr.Cells(1).Controls(0), TextBox)
      ViewSettings.SetTemplateSetting(ts.Key, tb.Text)
     Case "truefalse"
      Dim chk As CheckBox = CType(tr.Cells(1).Controls(0), CheckBox)
      ViewSettings.SetTemplateSetting(ts.Key, chk.Checked.ToString.ToLower)
     Case "10"
      Dim chk As CheckBox = CType(tr.Cells(1).Controls(0), CheckBox)
      If chk.Checked Then
       ViewSettings.SetTemplateSetting(ts.Key, "1")
      Else
       ViewSettings.SetTemplateSetting(ts.Key, "0")
      End If
     Case Else
      Dim tb As TextBox = CType(tr.Cells(1).Controls(0), TextBox)
      ViewSettings.SetTemplateSetting(ts.Key, tb.Text)
    End Select

    rowIndex += 1

   Next

   ViewSettings.SaveTemplateSettings()

   Response.Redirect(DotNetNuke.Common.NavigateURL, False)
  End Sub
 End Class
End Namespace
