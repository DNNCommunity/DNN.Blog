'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2012
' by DotNetNuke Corporation
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

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Web.UI.WebControls
Imports System.Web.UI
Imports DotNetNuke.Modules.Blog.Common
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.Modules
Imports DotNetNuke.Modules.Blog.Entities

<DefaultProperty("Terms"), Themeable(False)> _
<ToolboxData("<{0}:Tags runat=server> </{0}:Tags>")> _
Public Class Tags
 Inherits CompositeDataBoundControl

#Region " Private Members "
 Private _cmdSubscribe As LinkButton
 Private _htTags As Hashtable
 Private Shared ReadOnly EventSubmitKey As New Object()

 ''' <summary>
 ''' A collection of terms to be rendered by the control.
 ''' </summary>
 <Browsable(False)>
 Private Property Terms() As List(Of TermInfo)
#End Region

#Region " Constructors "
#End Region

#Region " Public Properties "
 ''' <summary>
 ''' This provides a full path to the shared resource file for localization. 
 ''' </summary>
 Friend ReadOnly Property SharedResourceFile() As String
  Get
   Return ResolveUrl("~/DesktopModules/Blog/App_LocalResources/SharedResources.resx")
  End Get
 End Property

 <Browsable(False)>
 Public Property ModContext() As ModuleInstanceContext
 Public Property CountMode() As Common.Constants.TagMode
#End Region

#Region " Event Handlers "
 Protected Overrides Function CreateChildControls(dataSource As IEnumerable, dataBinding As Boolean) As Integer
  Controls.Clear()

  Dim count As Integer = 0
  _htTags = New Hashtable()

  If dataSource IsNot Nothing Then
   Dim e As IEnumerator = dataSource.GetEnumerator()

   If TypeOf dataSource Is List(Of TermInfo) Then
    Terms = DirectCast(dataSource, List(Of TermInfo))

    For Each term As TermInfo In Terms
     _cmdSubscribe = New LinkButton() With {.CausesValidation = False, .CssClass = ""}
     count += 1
     If Not _htTags.ContainsKey(term.TermId) Then
      _htTags.Add(term.TermId, _cmdSubscribe)
      Controls.Add(_cmdSubscribe)
     End If
    Next

   End If
  End If

  Return count
 End Function

 ''' <summary>
 ''' This method renders the entire user interface for this control.
 ''' </summary>
 ''' <param name="writer"></param>
 Protected Overrides Sub RenderContents(writer As HtmlTextWriter)
  If Terms IsNot Nothing Then
   For Each term As TermInfo In Terms
    Dim link As String = DotNetNuke.Common.NavigateURL(ModContext.TabId, "", "tagid=" & term.TermId)

    ' <div>
    writer.AddAttribute(HtmlTextWriterAttribute.[Class], "qaTooltip")
    writer.RenderBeginTag(HtmlTextWriterTag.Div)
    ' <a />
    writer.AddAttribute(HtmlTextWriterAttribute.[Class], "tag")
    writer.AddAttribute(HtmlTextWriterAttribute.Href, link)
    writer.RenderBeginTag(HtmlTextWriterTag.A)
    writer.Write(term.Name)
    writer.RenderEndTag()
    ' <div>
    writer.AddAttribute(HtmlTextWriterAttribute.[Class], "tag-menu dnnClear")
    writer.AddAttribute(HtmlTextWriterAttribute.Style, "display:none;")
    writer.RenderBeginTag(HtmlTextWriterTag.Div)
    ' <div>
    writer.RenderBeginTag(HtmlTextWriterTag.Div)
    ' <div>
    writer.AddAttribute(HtmlTextWriterAttribute.[Class], "tm-heading")
    writer.RenderBeginTag(HtmlTextWriterTag.Div)
    ' <span>
    writer.AddAttribute(HtmlTextWriterAttribute.[Class], "tm-sub-info")
    writer.RenderBeginTag(HtmlTextWriterTag.Span)
    writer.Write(Localization.GetString("Tag", SharedResourceFile))
    writer.Write(term.Name)
    ' <span>
    writer.AddAttribute(HtmlTextWriterAttribute.[Class], "tm-sub-links")
    writer.AddAttribute(HtmlTextWriterAttribute.Style, "float:right;")
    writer.RenderBeginTag(HtmlTextWriterTag.Span)
    ' </span>
    writer.RenderEndTag()
    ' </span>
    writer.RenderEndTag()
    ' </div>
    writer.RenderEndTag()
    ' <div />
    writer.AddAttribute(HtmlTextWriterAttribute.[Class], "tm-description")
    writer.RenderBeginTag(HtmlTextWriterTag.Div)
    writer.Write(term.Description)
    writer.RenderEndTag()
    ' </div>
    writer.RenderEndTag()
    ' <span>
    writer.AddAttribute(HtmlTextWriterAttribute.[Class], "tm-links")
    writer.RenderBeginTag(HtmlTextWriterTag.Span)
    ' </span>
    writer.RenderEndTag()
    ' </div>
    writer.RenderEndTag()

    If CountMode <> Constants.TagMode.ShowNoUsage Then
     ' <span>
     writer.AddAttribute(HtmlTextWriterAttribute.[Class], "percentRight")
     writer.RenderBeginTag(HtmlTextWriterTag.Span)

     Select Case CountMode
      Case Constants.TagMode.ShowDailyUsage
       writer.Write(term.DayTermUsage.ToString + Localization.GetString("posts", SharedResourceFile))
       Exit Select
      Case Constants.TagMode.ShowWeeklyUsage
       writer.Write(term.WeekTermUsage.ToString + Localization.GetString("posts", SharedResourceFile))
       Exit Select
      Case Constants.TagMode.ShowMonthlyUsage
       writer.Write(term.MonthTermUsage.ToString + Localization.GetString("posts", SharedResourceFile))
       Exit Select
      Case Constants.TagMode.ShowTotalUsage
       writer.Write(term.TotalTermUsage.ToString + Localization.GetString("posts", SharedResourceFile))
       Exit Select
     End Select

     ' </span>
     writer.RenderEndTag()
    End If

    ' </div>
    writer.RenderEndTag()
   Next
  End If
 End Sub
#End Region

#Region " Private Methods "
#End Region

End Class