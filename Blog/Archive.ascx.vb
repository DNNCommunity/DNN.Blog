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

Imports System
Imports DotNetNuke.Web.Client.ClientResourceManagement
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Modules.Blog.Business
Imports DotNetNuke.Services.Exceptions.Exceptions

Partial Public Class Archive
    Inherits BlogModuleBase

#Region " Private Members "
    Private objBlog As BlogInfo
    Private objCtlBlog As New BlogController
    Private m_Culture As String
    Private m_BlogID As Integer = -1
    Private BlogDate As Date = Date.UtcNow
    Private m_PersonalBlogID As Integer
#End Region

#Region "Event Handlers"

    Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            ClientResourceManager.RegisterStyleSheet(Page, TemplateSourceDirectory + "/Archive.css", Web.Client.FileOrder.Css.ModuleCss)

            If Not Request.Params("BlogID") Is Nothing Then
                m_BlogID = CType(Request.Params("BlogID"), Integer)
            End If
            If Not Request.Params("BlogDate") Is Nothing Then
                BlogDate = CType(Request.Params("BlogDate"), Date)
            End If

            If Not Page.IsPostBack Then

                objBlog = objCtlBlog.GetBlogFromContext()
                If BlogSettings.PageBlogs <> -1 Then
                    m_PersonalBlogID = BlogSettings.PageBlogs
                    m_BlogID = m_PersonalBlogID
                Else
                    m_PersonalBlogID = -1
                End If

                Dim objCtlArchive As New ArchiveController
                Dim objArchiveDays As ArrayList
                Dim objArchiveDay As ArchiveDays
                Dim objArchiveMonths As List(Of ArchiveMonths)
                objArchiveDays = objCtlArchive.GetBlogDaysForMonth(Me.PortalId, m_BlogID, BlogDate)

                calMonth.SelectedDates.Clear()
                For Each objArchiveDay In objArchiveDays
                    objBlog = objCtlBlog.GetBlog(objArchiveDay.BlogID)
                    If Not objBlog Is Nothing Then
                        Dim strDate As String = Utility.FormatDate(objArchiveDay.AddedDate, objBlog.Culture, objBlog.DateFormat, objBlog.TimeZone)
                        Dim dDate As Date = Utility.ParseDate(strDate, objBlog.Culture)
                        calMonth.SelectedDates.Add(dDate)
                    End If
                Next
                calMonth.VisibleDate = BlogDate
                objArchiveMonths = objCtlArchive.GetBlogMonths(Me.PortalId, m_BlogID)

                'CP - Added to support archive list displayed as drop down instead of data list (new module setting for this).
                If BlogSettings.EnableArchiveDropDown Then
                    BindArchiveDropDown(objArchiveMonths)
                    ddlArchiveMonths.Visible = True
                    cmdGo.Visible = True
                    lstArchiveMonths.Visible = False
                Else
                    lstArchiveMonths.DataSource = objArchiveMonths
                    lstArchiveMonths.DataBind()
                End If
                'END Add
            Else

                '<BLG-7444 date="04/29/2008" by="dw">
                ' Since we are setting the SelectedDates above, if we have only one selected
                ' date, then this interferes with the ability for SelectionChanged to fire 
                ' properly.  To work around this, we clear the SelectedDate for postbackss.
                calMonth.SelectedDate = Nothing
                '</BLG-7444>

            End If

        Catch exc As Exception
            ProcessModuleLoadException(Me, exc)
        End Try

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>CP: Added</remarks>
    Protected Sub cmdGo_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdGo.Click
        Response.Redirect(ddlArchiveMonths.SelectedValue, True)
    End Sub

    Protected Sub calMonth_SelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles calMonth.SelectionChanged
        ' The Calendar Control supports multiple cultures, but only on the current culture
        ' This is kind of a hack to reset the culture back to the System Culture
        ' Utility.SetCulture(CType(Me.Page, PageBase).PageCulture.Name)

        ' BLG-4154
        ' Antonio Chagoury 9/1/2007
        ' Changed the way the URL is being constructed to support standard DNN NavigateURL
        ' Also added a URL parameter to specify the type of calendar search is requested
        Dim newDate As String = calMonth.SelectedDate.ToString("yyyy-MM-dd")
        Response.Redirect(NavigateURL(TabId, "", "BlogDate=" & newDate, "DateType=" & "day"))
        'Response.Redirect(Utility.AddTOQueryString(Utility.AddTOQueryString(Request.Url.ToString, "BlogDate", newDate), "DateType", "day"), True)
    End Sub

    Protected Sub lstArchiveMonths_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataListItemEventArgs) Handles lstArchiveMonths.ItemDataBound
        Dim lnkMonthYear As System.Web.UI.WebControls.HyperLink
        Dim lnkBlogRSS As System.Web.UI.WebControls.HyperLink

        lnkMonthYear = CType(e.Item.FindControl("lnkMonthYear"), System.Web.UI.WebControls.HyperLink)
        lnkBlogRSS = CType(e.Item.FindControl("lnkBlogRSS"), System.Web.UI.WebControls.HyperLink)
        If Me.ModuleConfiguration.DisplaySyndicate Then
            lnkBlogRSS.Visible = True
        Else
            lnkBlogRSS.Visible = False
        End If
        'DR-04/17/2009-BLG-9749
        If objBlog Is Nothing Then
            lnkMonthYear.Text = String.Format("{0} ({1})", Convert.ToDateTime(CType(e.Item.DataItem, ArchiveMonths).AddedDate).ToString("y"), CType(e.Item.DataItem, ArchiveMonths).PostCount)
        Else
            lnkMonthYear.Text = String.Format("{0} ({1})", Utility.FormatDate(CType(e.Item.DataItem, ArchiveMonths).AddedDate, objBlog.Culture, "y", TimeZoneInfo.Local), CType(e.Item.DataItem, ArchiveMonths).PostCount)
        End If
        If Not Request.Params("BlogId") Is Nothing Then
            Dim BlogId As Integer = Int32.Parse(Request.Params("BlogID"))
            ' BLG-4154
            ' Antonio Chagoury 9/1/2007
            ' Changed the way the URL is being constructed to support standard DNN NavigateURL
            lnkMonthYear.NavigateUrl = NavigateURL(TabId, "", "BlogId", BlogId.ToString(), "BlogDate=" & Format(CType(e.Item.DataItem, ArchiveMonths).AddedDate, "yyyy-MM-dd"), "DateType=" & "month")
            'lnkMonthYear.NavigateUrl = NavigateURL(Me.TabId, "", "&BlogId=" & BlogId.ToString() & "&BlogDate=" & Format(CType(e.Item.DataItem, ArchiveMonths).AddedDate, "yyyy-MM-dd"))
            If Me.ModuleConfiguration.DisplaySyndicate Then
                lnkBlogRSS.NavigateUrl = NavigateURL(Me.TabId, "", "&RssId=" & BlogId.ToString() & "&RssDate=" & Format(CType(e.Item.DataItem, ArchiveMonths).AddedDate, "yyyy-MM-dd"))
            End If
        Else
            ' BLG-4154
            ' Antonio Chagoury 9/1/2007
            ' Changed the way the URL is being constructed to support standard DNN NavigateURL
            lnkMonthYear.NavigateUrl = NavigateURL(TabId, "", "BlogDate=" & Format(CType(e.Item.DataItem, ArchiveMonths).AddedDate, "yyyy-MM-dd"), "DateType=" & "month")
            'lnkMonthYear.NavigateUrl = NavigateURL(Me.TabId, "", "&BlogDate=" & Format(CType(e.Item.DataItem, ArchiveMonths).AddedDate, "yyyy-MM-dd"))
            If Me.ModuleConfiguration.DisplaySyndicate Then
                lnkBlogRSS.NavigateUrl = NavigateURL(Me.TabId, "", "&RssDate=" & Format(CType(e.Item.DataItem, ArchiveMonths).AddedDate, "yyyy-MM-dd"))
            End If
        End If
    End Sub

    Protected Sub calMonth_VisibleMonthChanged(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.MonthChangedEventArgs) Handles calMonth.VisibleMonthChanged
        'Utility.SetCulture(CType(Me.Page, PageBase).PageCulture.Name)
        ' BLG-4154
        ' Antonio Chagoury 9/1/2007
        ' Changed the way the URL is being constructed to support standard DNN NavigateURL
        ' Also added a URL parameter to specify the type of calendar search is requested
        Try
            Dim newDate As String = (New Date(e.NewDate.Year, e.NewDate.Month, Date.DaysInMonth(e.NewDate.Year, e.NewDate.Month))).ToString("yyyy-MM-dd")
            Response.Redirect(NavigateURL(TabId, "", "BlogDate=" & newDate, "DateType=" & "month"))
            'Response.Redirect(Utility.AddTOQueryString(Request.Url.ToString, "BlogDate", newDate), True)
        Catch ex As Exception
            ProcessModuleLoadException(Me, ex)
        End Try

    End Sub

    Protected Sub calMonth_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles calMonth.Unload
        '<BLG-1809 Date="08/22/05" User="HP" Comment="">
        'Utility.SetCulture(CType(Me.Page, PageBase).PageCulture.Name)
        'Utility.SetCulture("")
        '</BLG-1809>
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="archivedMonths"></param>
    ''' <remarks>CP: Added new option via module settings.</remarks>
    Private Sub BindArchiveDropDown(ByVal archivedMonths As List(Of ArchiveMonths))
        For Each item As ArchiveMonths In archivedMonths
            Dim li As New ListItem

            li.Text = String.Format("{0} ({1})", Convert.ToDateTime(item.AddedDate).ToString("y"), item.PostCount)

            If Not Request.Params("BlogId") Is Nothing Then
                Dim BlogId As Integer = Int32.Parse(Request.Params("BlogID"))
                li.Value = NavigateURL(TabId, "", "BlogId", BlogId.ToString(), "BlogDate=" & Format(item.AddedDate, "yyyy-MM-dd"), "DateType=" & "month")
            Else
                li.Value = NavigateURL(TabId, "", "BlogDate=" & Format(item.AddedDate, "yyyy-MM-dd"), "DateType=" & "month")
            End If

            ddlArchiveMonths.Items.Add(li)
        Next

    End Sub


#End Region

End Class