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
Imports DotNetNuke.Modules.Blog.Components.Common
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.Modules
Imports DotNetNuke.Modules.Blog.Components.Entities

''' <summary>
''' 
''' </summary>
<DefaultProperty("Terms"), Themeable(False)> _
<ToolboxData("<{0}:Tags runat=server> </{0}:Tags>")> _
Public Class Tags
    Inherits CompositeDataBoundControl

#Region "Private Members"

    Private _cmdSubscribe As LinkButton
    Private _htTags As Hashtable

    Private Shared ReadOnly EventSubmitKey As New Object()

    ''' <summary>
    ''' A collection of terms to be rendered by the control.
    ''' </summary>
    <Browsable(False)> _
    Private Property Terms() As List(Of TermInfo)
        Get
            Return m_Terms
        End Get
        Set(value As List(Of TermInfo))
            m_Terms = value
        End Set
    End Property
    Private m_Terms As List(Of TermInfo)

#End Region

#Region "Constructors"

    ''' <summary>
    ''' 
    ''' </summary>
    Public Sub New()

    End Sub

#End Region

#Region "Public Properties"

    ''' <summary>
    ''' This provides a full path to the shared resource file for localization. 
    ''' </summary>
    Friend ReadOnly Property SharedResourceFile() As String
        Get
            Return ResolveUrl("~/DesktopModules/Blog/App_LocalResources/SharedResources.resx")
        End Get
    End Property

    <Browsable(False)> _
    Public Property ModContext() As ModuleInstanceContext
        Get
            Return m_ModContext
        End Get
        Set(value As ModuleInstanceContext)
            m_ModContext = value
        End Set
    End Property
    Private m_ModContext As ModuleInstanceContext

    Public Property CountMode() As Constants.TagMode
        Get
            Return m_CountMode
        End Get
        Set(value As Constants.TagMode)
            m_CountMode = value
        End Set
    End Property
    Private m_CountMode As Constants.TagMode

#End Region

#Region "Event Handlers"

    Protected Overrides Function CreateChildControls(dataSource As IEnumerable, dataBinding As Boolean) As Integer
        Controls.Clear()

        Dim count As Integer = 0
        _htTags = New Hashtable()

        If dataSource IsNot Nothing Then
            Dim e As IEnumerator = dataSource.GetEnumerator()

            If TypeOf dataSource Is List(Of TermInfo) Then
                Terms = DirectCast(dataSource, List(Of TermInfo))

                For Each term As TermInfo In Terms
                    _cmdSubscribe = New LinkButton() With { _
                     .CausesValidation = False, _
                     .CssClass = "" _
                    }

                    '_cmdSubscribe.Command += SubscribeCommand()

                    '              Dim colUserSubs = Controller.GetUserSubscriptions(ModContext.PortalId, ModContext.PortalSettings.UserId)
                    '              Dim term1 = term
                    'Dim objSub = ( _
                    '	Where t.TermId = term1.TermId).SingleOrDefault()
                    '              If objSub Is Nothing Then
                    '                  _cmdSubscribe.CommandName = "subscribe"
                    '                  _cmdSubscribe.Text = Localization.GetString("Subscribe.Text", SharedResourceFile)
                    '                  _cmdSubscribe.CommandArgument = term.TermId.ToString()
                    '              Else
                    '                  _cmdSubscribe.Text = Localization.GetString("Unsubscribe.Text", SharedResourceFile)
                    '                  _cmdSubscribe.CommandName = "unsubscribe"
                    '                  _cmdSubscribe.CommandArgument = objSub.SubscriptionId.ToString()
                    '              End If

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

    ' ''' <summary>
    ' ''' The Vote event.
    ' ''' </summary>
    ' ''' <remarks>This is normally done behind the scenes by .net, implemented here for performance reasons.</remarks>
    '<Category("Action"), Description("Raised when the user clicks the subscribe button.")> _
    'Public Custom Event Subscribe As EventHandler
    '    AddHandler(ByVal value As EventHandler)
    '        Events.[AddHandler](EventSubmitKey, value)
    '    End AddHandler
    '    RemoveHandler(ByVal value As EventHandler)
    '        Events.[RemoveHandler](EventSubmitKey, value)
    '    End RemoveHandler
    'End Event

    ' ''' <summary>
    ' ''' 
    ' ''' </summary>
    ' ''' <param name="source"></param>
    ' ''' <param name="e"></param>
    'Protected Sub SubscribeCommand(source As Object, e As CommandEventArgs)
    '    RaiseEvent SubscribeClick(Me, e)
    '    Dim control = DirectCast(source, LinkButton)
    '    Dim action = e.CommandName
    '    Dim id = Convert.ToInt32(e.CommandArgument)

    '    Select Case action
    '        Case "subscribe"
    '            SubscribeUser(id, False)
    '            control.Text = Localization.GetString("Unsubscribe.Text", SharedResourceFile)
    '            Exit Select
    '        Case Else
    '            SubscribeUser(id, True)
    '            control.Text = Localization.GetString("Subscribe.Text", SharedResourceFile)
    '            Exit Select
    '    End Select
    'End Sub

    ''' <summary>
    ''' This method renders the entire user interface for this control.
    ''' </summary>
    ''' <param name="writer"></param>
    Protected Overrides Sub RenderContents(writer As HtmlTextWriter)
        If Terms IsNot Nothing Then
            For Each term As TermInfo In Terms
                Dim link As String = DotNetNuke.Common.NavigateURL(ModContext.TabId, "", "tagid=" & term.TermId)
                'Dim detaillink = Links.ViewTagDetail(ModContext, term.Name)
                ''var historylink = Links.ViewTagHistory(ModContext, term.Name);
                'Dim improvelink = Links.EditTag(ModContext, term.Name)

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
                'If ModContext.PortalSettings.UserId > 0 Then
                '    _cmdSubscribe = DirectCast(_htTags(term.TermId), LinkButton)
                '    ' <a />
                '    '''/ we register this here so that the tooltip is updated after the event action is taken.
                '    'AJAX.RegisterPostBackControl(_cmdSubscribe);
                '    _cmdSubscribe.RenderControl(writer)
                'End If
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

                'writer.AddAttribute(HtmlTextWriterAttribute.Href, link);
                'writer.RenderBeginTag(HtmlTextWriterTag.A);
                'writer.Write(Localization.GetString("browse.Text", SharedResourceFile));
                'writer.RenderEndTag();

                '' <a />
                'writer.AddAttribute(HtmlTextWriterAttribute.Href, detaillink)
                'writer.RenderBeginTag(HtmlTextWriterTag.A)
                'writer.Write(Localization.GetString("about.Text", SharedResourceFile))
                'writer.RenderEndTag()


                'writer.AddAttribute(HtmlTextWriterAttribute.Href, historylink);
                'writer.RenderBeginTag(HtmlTextWriterTag.A);
                'writer.Write(Localization.GetString("history.Text", SharedResourceFile));
                'writer.RenderEndTag();

                '' <span>
                'writer.AddAttribute(HtmlTextWriterAttribute.[Class], "tm-links")
                'writer.AddAttribute(HtmlTextWriterAttribute.Style, "float:right;")
                'writer.RenderBeginTag(HtmlTextWriterTag.Span)
                '' <a />
                'writer.AddAttribute(HtmlTextWriterAttribute.Href, improvelink)
                'writer.RenderBeginTag(HtmlTextWriterTag.A)
                'writer.Write(Localization.GetString("improve.Text", SharedResourceFile))
                'writer.RenderEndTag()
                '' '' </span>
                ''writer.RenderEndTag()

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

#Region "Private Methods"

    ' ''' <summary>
    ' ''' 
    ' ''' </summary>
    ' ''' <param name="id"></param>
    ' ''' <param name="remove"></param>
    'Private Sub SubscribeUser(id As Integer, remove As Boolean)
    '    If id <= 0 OrElse ModContext.PortalSettings.UserId <= 0 Then
    '        Return
    '    End If
    '    If remove Then
    '        Controller.DeleteSubscription(ModContext.PortalId, id)
    '    Else
    '        Dim objSub = New SubscriptionInfo() With { _
    '         .PortalId = ModContext.PortalId, _
    '         .UserId = ModContext.PortalSettings.UserId, _
    '         .TermId = id, _
    '         .CreatedOnDate = DateTime.Now, _
    '         .SubscriptionType = CInt(Constants.SubscriptionType.InstantTerm) _
    '        }
    '        Controller.AddSubscription(objSub)
    '    End If
    'End Sub

#End Region

End Class