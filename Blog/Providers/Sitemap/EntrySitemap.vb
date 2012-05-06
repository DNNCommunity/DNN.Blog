'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2011
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
Option Strict On
Option Explicit On

Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Sitemap

Namespace Providers.Sitemap


    ''' <summary>
    ''' This is the seo sitemap provider for the core forum module. 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class EntrySitemap
        Inherits SitemapProvider

        ''' <summary>
        ''' Builds a collection of sitemap url's which are then used by the Sitemap Provider for the Forum module to generate pages for the SEO sitemap. 
        ''' </summary>
        ''' <param name="portalId"></param>
        ''' <param name="ps"></param>
        ''' <param name="version"></param>
        ''' <returns></returns>
        ''' <remarks>Only public forums are included in the seo sitemap.</remarks>
        Public Overrides Function GetUrls(ByVal portalId As Integer, ByVal ps As PortalSettings, ByVal version As String) As List(Of SitemapUrl)
            Dim threadURL As SitemapUrl
            Dim urls As New List(Of SitemapUrl)
            'Dim cntThread As New ThreadController()

            'Dim entries As List(Of ThreadInfo) = cntThread.GetSitemapThreads(portalId)

            'For Each objThread As ThreadInfo In entries
            '    threadURL = GetThreadUrl(objThread)
            '    urls.Add(threadURL)
            'Next
            Return urls
        End Function

        ' ''' <summary>
        ' ''' Creates a seo sitemap url, with priority, change frequency
        ' ''' </summary>
        ' ''' <param name="objThread"></param>
        ' ''' <returns>A single sitemap url object.</returns>
        ' ''' <remarks></remarks>
        'Private Function GetThreadUrl(ByVal objThread As ThreadInfo) As SitemapUrl
        '    Dim pageUrl As New SitemapUrl
        '    pageUrl.Url = Forum.Utilities.Links.ContainerViewThreadLink(objThread.TabID, objThread.ForumID, objThread.ThreadID)
        '    pageUrl.Priority = objThread.ContainingForum.SitemapPriority
        '    pageUrl.LastModified = objThread.LastApprovedPost.CreatedDate

        '    If objThread.LastApprovedPost.CreatedDate > DateAdd(DateInterval.Month, 18, DateTime.Now()) Then
        '        pageUrl.ChangeFrequency = SitemapChangeFrequency.Never
        '    ElseIf objThread.LastApprovedPost.CreatedDate > DateAdd(DateInterval.Month, 6, DateTime.Now()) Then
        '        pageUrl.ChangeFrequency = SitemapChangeFrequency.Yearly
        '    ElseIf objThread.LastApprovedPost.CreatedDate > DateAdd(DateInterval.Month, 1, DateTime.Now()) Then
        '        pageUrl.ChangeFrequency = SitemapChangeFrequency.Monthly
        '    ElseIf objThread.LastApprovedPost.CreatedDate > DateAdd(DateInterval.Day, 6, DateTime.Now()) Then
        '        pageUrl.ChangeFrequency = SitemapChangeFrequency.Weekly
        '    ElseIf objThread.LastApprovedPost.CreatedDate > DateAdd(DateInterval.Hour, 12, DateTime.Now()) Then
        '        pageUrl.ChangeFrequency = SitemapChangeFrequency.Daily
        '    Else
        '        pageUrl.ChangeFrequency = SitemapChangeFrequency.Hourly
        '    End If

        '    Return pageUrl
        'End Function

    End Class
End Namespace