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

Imports DotNetNuke.Modules.Blog.Controllers
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Sitemap
Imports DotNetNuke.Modules.Blog.Entities

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
   Dim permaLink As SitemapUrl
   Dim urls As New List(Of SitemapUrl)
   Dim entries As List(Of EntryInfo)

   ' get all portal blog entries that are published (and at current date)
   entries = EntryController.GetAllEntriesByPortal(portalId, False, False)

   For Each objEntry As EntryInfo In entries
    permaLink = GetEntryUrl(objEntry)
    urls.Add(permaLink)
   Next

   Return urls
  End Function

  ''' <summary>
  ''' Creates a seo sitemap url, with priority, change frequency
  ''' </summary>
  ''' <param name="objEntry"></param>
  ''' <returns>A single sitemap url object.</returns>
  ''' <remarks></remarks>
  Private Function GetEntryUrl(ByVal objEntry As EntryInfo) As SitemapUrl
   Dim pageUrl As New SitemapUrl
   pageUrl.Url = objEntry.PermaLink
   'pageUrl.Priority = 
   pageUrl.LastModified = objEntry.AddedDate ' This is UTC

   If objEntry.AddedDate > DateAdd(DateInterval.Month, 18, DateTime.Now()) Then
    pageUrl.ChangeFrequency = SitemapChangeFrequency.Never
   ElseIf objEntry.AddedDate > DateAdd(DateInterval.Month, 6, DateTime.Now()) Then
    pageUrl.ChangeFrequency = SitemapChangeFrequency.Yearly
   ElseIf objEntry.AddedDate > DateAdd(DateInterval.Month, 1, DateTime.Now()) Then
    pageUrl.ChangeFrequency = SitemapChangeFrequency.Monthly
   ElseIf objEntry.AddedDate > DateAdd(DateInterval.Day, 6, DateTime.Now()) Then
    pageUrl.ChangeFrequency = SitemapChangeFrequency.Weekly
   ElseIf objEntry.AddedDate > DateAdd(DateInterval.Hour, 12, DateTime.Now()) Then
    pageUrl.ChangeFrequency = SitemapChangeFrequency.Daily
   Else
    pageUrl.ChangeFrequency = SitemapChangeFrequency.Hourly
   End If

   Return pageUrl
  End Function

 End Class

End Namespace