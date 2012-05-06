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
Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports DotNetNuke.Common
Imports DotNetNuke.Modules.Blog.Business
Imports DotNetNuke.Modules.Blog.Components.Controllers
Imports DotNetNuke.Modules.Blog.Components.Common
Imports DotNetNuke.Security
Imports DotNetNuke.Data
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.Log.EventLog
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Framework
Imports DotNetNuke.UI.Modules
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Modules.Blog.Components.Settings
Imports DotNetNuke.Modules.Blog.Components.Entities

Namespace Components.Business

    Public Class Utility

#Region "Various"

        Public Shared Function AddTOQueryString(ByVal URL As String, ByVal Key As String, ByVal Value As String) As String
            Dim RegExp As New System.Text.RegularExpressions.Regex(Key & "=.*?(&|$)")
            Dim Match As System.Text.RegularExpressions.Match = RegExp.Match(URL)
            If Match.Success Then
                Return RegExp.Replace(URL, Key & "=" & Value & Match.Groups(1).ToString)
            ElseIf URL.IndexOf("?") > 0 Then
                Return URL & "&" & Key & "=" & Value
            Else
                Return URL & "?" & Key & "=" & Value
            End If
        End Function

        Public Shared Function IsInteger(ByVal strValue As String) As Boolean
            If IsNumeric(strValue) Then
                If Int(Val(strValue)) = Val(strValue) Then
                    If Val(strValue) > -32769 And Val(strValue) < 32768 Then
                        Return True
                    End If
                End If
            End If
            Return False
        End Function

#End Region

#Region " HTML "
        Public Shared Function FormatText(ByVal strHTML As String) As String

            Dim strText As String = strHTML

            strText = Replace(strText, "<br>", ControlChars.Lf)
            strText = Replace(strText, "<BR>", ControlChars.Lf)

            Return strText

        End Function

        Public Shared Function FormatHTML(ByVal strText As String) As String

            Dim strHTML As String = strText

            strHTML = Replace(strHTML, Chr(13), "")
            strHTML = Replace(strHTML, ControlChars.Lf, "<br />")

            Return strHTML

        End Function

        '''-----------------------------------------------------------------------------
        ''' <history>
        ''' 	[DW] 	06/06/2008	Added to accomodate need to clean comment links
        '''                 by removing any target other than _blank and by ensuring 
        '''                 that the rel="nofollow" is added.
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Shared Function CleanCommentHtml(ByVal strContent As String, ByVal allowAnchors As Boolean, _
                                                ByVal allowImages As Boolean, ByVal allowFormatting As Boolean) As String

            Dim options As RegexOptions = RegexOptions.IgnoreCase Or RegexOptions.Singleline
            Dim regPreGetRel As String = "<a(?<firstpart>[^>]*[^>]*?)(?<rel>rel=""*nofollow""*)(?<secondpart>[^>]*)>"
            Dim regAddAtt As String = "(?<firstpart><a[^>]*)>"

            '*************************
            '* CLEAN HTML
            '*************************
            ' The following call to TruncateHTML will make use of the code in this procedure
            ' to remove all the HTML but anchor tags and image tags.  The second parameter is 
            ' the length.  When set to 0, there is no truncation.  The third parameter is set to
            ' false noting that we don't want to allow script tags in the comment HTML returned.
            strContent = CleanHTML(strContent, 0, False, allowFormatting, allowAnchors, allowImages)

            '*************************
            '* CONVERT WEB ADDRESSES 
            '* TO LINKS IF ANCHORS ALLOWED
            '*************************
            If allowAnchors Then
                strContent = Regex.Replace(strContent, "(\s|\(|,|:)(http(?:s{0,1})://([^\s]+(?:/|\b)))(\){0,1})", "$1<a href=""$2"">$3</a>$4", options)
                ' Decided not to include since the following since we don't know if, say, microsoft.com
                ' would appear inside the text of another link.  Left the regex for reference in case this 
                ' feature is needed in the future.  The best way to include this feature would be to use 
                ' a MatchExpression on a regex which found both the form above and the form below.  This would
                ' preclude finding the form below within the form above.
                'strContent = Regex.Replace(strContent, "(\s|\(|,|:)((?:[a-z0-9_-]+\.)*(?:com|edu|org|biz|net|gov|mil|info))\b", "<a href=""http://$2"">$2</a>", options)
            End If

            '*************************
            '* ADD REL=NOFOLLOW
            '*************************
            strContent = Regex.Replace(strContent, regPreGetRel, "$1$3", options)
            strContent = Regex.Replace(strContent, regAddAtt, "$1 rel=""nofollow"">", options)

            Return strContent

        End Function

        '''-----------------------------------------------------------------------------
        ''' <history>
        ''' 	[HP] 	10/13/2005	Changed BLG-2031
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Shared Function removeHtmlTags(ByVal inputString As String) As String
            Dim objPortalSecurity As New PortalSecurity
            Return objPortalSecurity.InputFilter(inputString, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup)
            'Return Regex.Replace(inputString, "</?(?i:script|embed|object|img|frameset|href|frame|iframe|meta|table|tr|th|td|p|link|style)(.|\\n)*?>", "")
        End Function

        Public Shared Function removeAllHtmltags(ByVal inputString As String) As String
            Return removeHtmlTags(Regex.Replace(inputString, "<[^>]*>", ""))
        End Function

        '''-----------------------------------------------------------------------------
        ''' <history>
        ''' 	[DW] 	05/23/08	Added EnsureBeginningHttp to check website URLs
        '''                         added in the comments section contained a beginning
        '''                         http protocol reference.
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Shared Function EnsureBeginningHttp(ByVal inputUrl As String) As String
            If inputUrl.StartsWith("http://") Or inputUrl.StartsWith("https://") Then
                ' We're fine, just return the URL
                Return inputUrl
            Else
                ' Otherwise, we need to add the http protocol
                If (inputUrl.Trim <> String.Empty) Then
                    Return "http://" & inputUrl
                Else
                    Return String.Empty
                End If
            End If
        End Function

        Public Shared Function CleanHTML(ByVal input As String, ByVal len As Integer) As String
            Return CleanHTML(input, len, True, False, True, True)
        End Function

        Public Shared Function CleanHTML(ByVal input As String, ByVal len As Integer, ByVal allowScripts As Boolean, ByVal allowFormatTags As Boolean, ByVal allowAnchors As Boolean, ByVal allowImages As Boolean) As String

            input = HttpUtility.HtmlDecode(input)

            ' Escape Images, Anchors and Scripts
            ' and Replace </div> and </p> with a custom
            ' escaped <br /> tag.
            EscapeInput(input, allowScripts, allowFormatTags, allowAnchors, allowImages)

            ' Next, we will strip the HTML
            input = HtmlUtils.StripTags(input, False)

            ' Now we'll unescape the custom sequence used to escape images and anchors

            input = input.Replace("&_lt_;", "<")
            input = input.Replace("&_gt_;", ">")

            ' Next, we replace the spaces in anchor and image tags
            ' with a unique sequence that we'll use to restore the spaces
            ' after we find our breaking point based on the next location of a space.
            EscapeSpaces(input, allowScripts)

            ' OK, now we're ready to truncate, but we'll do this to the closest space
            ' and we'll only truncate if the value of len is > 0

            If len > 0 Then

                ' Check first to make sure the new length is not shorter than the requested 
                ' length (len parameter)
                If (input.Length <= len) Then
                    UnEscapeInput(input)
                    Return input & "... "
                End If

                ' TRUNCATE
                Dim lenToNextSpace As Integer = input.Substring(len).IndexOf(" ")
                Dim lenToNextReturn As Integer = input.Substring(len).IndexOf(vbCrLf)
                Dim cutOffLenDiff As Integer

                ' Check to see which is the lowest, positive number
                If (lenToNextSpace > 0 And lenToNextReturn > 0) Then
                    'Both numbers are positive, so get the lowest of the two
                    If lenToNextSpace <= lenToNextReturn Then
                        cutOffLenDiff = lenToNextSpace
                    Else
                        cutOffLenDiff = lenToNextReturn
                    End If
                Else
                    ' One or both of the numbers are negative, so get the highest of the two
                    ' or -1 if they're both -1
                    If lenToNextSpace >= lenToNextReturn Then
                        cutOffLenDiff = lenToNextSpace
                    Else
                        cutOffLenDiff = lenToNextReturn
                    End If
                End If


                If (cutOffLenDiff < 0) Then cutOffLenDiff = input.Length

                If input.Length >= len + cutOffLenDiff Then
                    input = input.Substring(0, len + cutOffLenDiff) & "... "
                End If

            End If

            UnEscapeInput(input)

            Return input

        End Function

        Private Shared Sub EscapeInput(ByRef input As String, ByVal allowScripts As Boolean, ByVal allowFormatTags As Boolean, _
                                       ByVal allowAnchors As Boolean, ByVal allowImages As Boolean)

            ' Create the Regex expressions we'll be using
            Dim regParEnd As String = "(<\s*/\s*p\s*>|<\s*/\s*div\s*>)"
            Dim options As RegexOptions = RegexOptions.Singleline Or RegexOptions.IgnoreCase

            ' We want to keep anchors and images, so we escape the HTML characters < and >
            ' with special character strings that we'll replace later.
            If allowAnchors Then

                Dim regAnchorABeg As String
                If allowScripts Then
                    ' We check allowScripts becuase if this is allowed, then it's safe to allow style attributes on the 
                    ' anchor.  Currently, allowScripts is only allowed when CleanHTML is called from BlogView to 
                    ' truncate the entry summary.
                    regAnchorABeg = "<(a(\s+((href)|(class)|(rel)|(id)|(style)|(tabindex)|(accesskey)|(rev))="".*?"")*\s*/*)>"
                Else
                    regAnchorABeg = "<(a(\s+((href))="".*?"")*\s*/*)>"
                End If
                Dim regAnchorAEnd As String = "<\s*(/a)\s*>"
                input = Regex.Replace(input, regAnchorABeg, "&_lt_;$1&_gt_;", options)
                input = Regex.Replace(input, regAnchorAEnd, "&_lt_;$1&_gt_;", options)
            End If
            If allowScripts Then
                Dim regScriptBeg As String = "<\s*(script[^>]+/*)\s*>"
                Dim regScriptEnd As String = "<\s*(/script)\s*>"
                input = Regex.Replace(input, regScriptBeg, "&_lt_;$1&_gt_;", options)
                input = Regex.Replace(input, regScriptEnd, "&_lt_;$1&_gt_;", options)
            End If
            If allowFormatTags Then
                Dim regFormatBeg As String = "<(b|strong|blockquote|em|u|i)>"
                Dim regFormatEnd As String = "<(/(b|strong|blockquote|em|u|i))>"
                input = Regex.Replace(input, regFormatBeg, "&_lt_;$1&_gt_;", options)
                input = Regex.Replace(input, regFormatEnd, "&_lt_;$1&_gt_;", options)
            End If
            If allowImages Then
                Dim regImage As String
                If allowScripts Then
                    ' We check allowScripts becuase if this is allowed, then it's safe to allow style attributes on the 
                    ' image content.  Currently, allowScripts is only allowed when CleanHTML is called from BlogView to 
                    ' truncate the entry summary.
                    regImage = "<(img(\s+((src)|(style)|(alt)|(id)|(title)|(height)|(width)|(border)|(longdesc))="".*?"")*\s*/*)>"
                Else
                    regImage = "<(img(\s+((src)|(alt)|(id)|(title)|(height)|(width)|(border)|(longdesc))="".*?"")*\s*/*)>"
                End If
                input = Regex.Replace(input, regImage, "&_lt_;$1&_gt_;", options)
            End If
            ' We also want to replace </p> and </div> with <br /><br />
            input = Regex.Replace(input, regParEnd, "_p_br_", options)

        End Sub

        Private Shared Sub EscapeSpaces(ByRef input As String, ByVal allowScripts As Boolean)

            ' Next, we replace the spaces in anchor and image tags
            ' with a unique sequence that we'll use to restore the spaces
            ' after we find our breaking point based on the next location of a space.
            Dim regSpaces As String = "<(a(\s+((href)|(class)|(rel)|(id)|(style)|(tabindex)|(accesskey)|(rev))="".*?"")*\s*/*)>.*?</a>"
            Dim options As RegexOptions = RegexOptions.Singleline Or RegexOptions.IgnoreCase

            input = Regex.Replace(input, regSpaces, AddressOf TagMatch, options)
            regSpaces = "<(img(\s+((src)|(alt)|(id)|(class)|(title)|(height)|(width)|(border)|(style)|(longdesc))="".*?"")*\s*/*)>"
            input = Regex.Replace(input, regSpaces, AddressOf TagMatch, options)
            If allowScripts Then
                regSpaces = "<\s*script[^>]+?>"
                input = Regex.Replace(input, regSpaces, AddressOf TagMatch, options)
                regSpaces = "<\s*script.*?>.*?</script\s*>"
                input = Regex.Replace(input, regSpaces, AddressOf TagMatch, options)
            End If

        End Sub

        Private Shared Sub UnEscapeInput(ByRef input As String)

            'Place the spaces and the <br /> tags back in
            input = input.Replace("_!_", " ")
            input = input.Replace("_p_br_", "<br /><br />")

        End Sub

        Private Shared Function TagMatch(ByVal match As Match) As String
            Return match.ToString().Replace(" ", "_!_")
        End Function
#End Region

#Region "Dates"
        Public Shared Function FormatDate(ByVal [Date] As DateTime, Culture As String, ByVal DateFormat As String, ByVal TimeZone As TimeZoneInfo) As String
            Return FormatDate([Date], Culture, DateFormat, TimeZone, False)
        End Function
        Public Shared Function FormatDate(ByVal [Date] As DateTime, Culture As String, ByVal DateFormat As String, ByVal TimeZone As TimeZoneInfo, ByVal ToUniversal As Boolean) As String
            If String.IsNullOrEmpty(Culture) Then Culture = Threading.Thread.CurrentThread.CurrentCulture.Name
            Dim dtf As System.Globalization.DateTimeFormatInfo = New System.Globalization.CultureInfo(Culture, False).DateTimeFormat
            If ToUniversal = True Then
                Dim dto As New DateTimeOffset([Date])
                Return dto.ToUniversalTime.ToString(DateFormat, dtf)
            Else
                Dim dt As Date = AdjustedDate([Date], TimeZone)
                Return dt.ToString(DateFormat, dtf)
            End If
        End Function

        Public Shared Function AdjustedDate(ByVal [Date] As DateTime, ByVal TimeZone As TimeZoneInfo) As Date
            Dim dto As New DateTimeOffset([Date])
            Return TimeZoneInfo.ConvertTime(dto, TimeZone).DateTime
        End Function

        Public Shared Function ParseDate(ByVal DateString As String, ByVal Culture As String) As DateTime
            Dim dtf As System.Globalization.DateTimeFormatInfo = New System.Globalization.CultureInfo(Culture, False).DateTimeFormat
            Try
                Return Date.Parse(DateString, dtf)
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        Public Shared Function IsValidDate(ByVal DateString As String, ByVal Culture As String) As Boolean
            Dim dtf As System.Globalization.DateTimeFormatInfo = New System.Globalization.CultureInfo(Culture, False).DateTimeFormat
            Try
                Dim oDate As Date = Date.Parse(DateString, dtf)
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

#End Region

#Region "Blog Settings"

        Public Shared Sub UpgradeApplication(ByVal PortalID As Integer, ByVal DataVersion As String)
            Select Case DataVersion
                Case "03.05.00"
                    CreateAllEntryLinks(PortalID)
            End Select
        End Sub

        Public Shared Sub UpdateBlogModuleSetting(ByVal PortalID As Integer, ByVal TabID As Integer, ByVal Key As String, ByVal Value As String)
            Providers.Data.DataProvider.Instance().UpdateBlogModuleSetting(PortalID, TabID, Key, Value)
        End Sub

        Public Shared Function GetTabIDByPortalID(ByVal PortalID As String) As Integer
            Dim blogTabId As Integer = 0
            ' This is safe from SQL Injection because PortalID is retrieved in code and 
            ' not passed by the user.
            Dim SQL As String = "SELECT Min(tm.TabID) as TabID FROM {databaseOwner}{objectQualifier}Tabs t " + "" & Chr(9) & " JOIN {databaseOwner}{objectQualifier}TabModules tm ON tm.TabId = t.TabId " + "    JOIN {databaseOwner}{objectQualifier}Modules m on m.ModuleID = tm.ModuleID " + "    JOIN {databaseOwner}{objectQualifier}ModuleDefinitions md on md.ModuleDefID = m.ModuleDefID " + "    JOIN {databaseOwner}{objectQualifier}DesktopModules dm on dm.DesktopModuleID = md.DesktopModuleID " + "WHERE md.FriendlyName = 'View_Blog' " + "    AND m.PortalID = " + PortalID + "" & Chr(9) & " AND m.IsDeleted = 0 " + "" & Chr(9) & " AND t.IsDeleted = 0"

            Dim dr As IDataReader = DataProvider.Instance().ExecuteSQL(SQL)
            If dr.Read() Then
                blogTabId = dr.GetInt32(dr.GetOrdinal("TabID"))
            End If
            Return blogTabId
        End Function

#End Region

#Region " Trackback "
        Public Shared Function GetTrackbackRDF(ByVal URL As String, ByVal oEntry As EntryInfo) As String
            Dim HostURL As String = AddHTTP(GetDomainName(HttpContext.Current.Request))
            If Not HostURL.EndsWith("/") Then
                HostURL &= "/"
            End If
            Dim sRDF As String = Environment.NewLine
            sRDF += "<!--" + Environment.NewLine
            sRDF += "<rdf:RDF xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#""" + Environment.NewLine
            sRDF += "xmlns:dc=""http://purl.org/dc/elements/1.1/""" + Environment.NewLine
            sRDF += "xmlns:trackback=""http://madskills.com/public/xml/rss/module/trackback/"">" + Environment.NewLine
            sRDF += "<rdf:Description " + Environment.NewLine
            sRDF += "rdf:about=""" & URL & """" + Environment.NewLine
            sRDF += "dc:identifier=""" & URL & """" + Environment.NewLine
            sRDF += "dc:title=""" & oEntry.Title & """" + Environment.NewLine
            sRDF += "trackback:ping=""" & HostURL & "desktopmodules/Blog/Trackback.aspx?id=" & oEntry.EntryID & """ />" + Environment.NewLine
            sRDF += "</rdf:RDF>" + Environment.NewLine
            sRDF += "-->" + Environment.NewLine
            Return sRDF
        End Function

        Public Shared Function GetTrackbackLink(ByVal pageBody As String) As String
            Dim sPattern As String = "<rdf:\w+\s[^>]*?>(</rdf:rdf>)?"
            Dim anchors As Regex = New Regex(sPattern, RegexOptions.IgnoreCase)
            For Each match As Match In anchors.Matches(pageBody)
                Dim pattern As String = "trackback:ping=""(?<url>[^""]+)"""
                Dim anchor As Regex = New Regex(pattern, RegexOptions.IgnoreCase)
                Dim m As Match = anchor.Match(match.Value)
                If Not (m.Groups("url").Value = "") Then
                    Dim trackBacklink As Uri = New Uri(m.Groups("url").Value)
                    If trackBacklink.Scheme = Uri.UriSchemeHttp Then
                        Return trackBacklink.ToString
                    End If
                End If
            Next
            Return Nothing
        End Function

        Public Shared Sub AutoTrackback(ByVal content As EntryInfo, ByVal title As String)
            If Not (content Is Nothing) Then

                Dim anchors As Regex = New Regex("href\s*=\s*(?:(?:\""(?<url>[^\""]*)\"")|(?<url>[^\s]* ))")
                For Each match As Match In anchors.Matches(HttpUtility.HtmlDecode(content.Entry))
                    Dim url As String = match.Groups("url").Value
                    If url.StartsWith("http") Then
                        Try

                            Dim req As WebRequest = WebRequest.Create(url)
                            ' Replaed null with Nothing
                            ' Dim Null As Object
                            Dim wreq As HttpWebRequest = CType(req, HttpWebRequest)
                            If Not (wreq Is Nothing) Then
                                wreq.UserAgent = "Blog for DNN"
                                wreq.Referer = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)
                                wreq.Timeout = 60000
                            End If
                            Dim response As HttpWebResponse = CType(wreq.GetResponse, HttpWebResponse)
                            Dim s As Stream = response.GetResponseStream
                            Dim enc As String = response.ContentEncoding.Trim
                            If enc = "" Then enc = "us-ascii"
                            Dim encode As Encoding = System.Text.Encoding.GetEncoding(enc)
                            Dim sr As StreamReader = New StreamReader(s, encode)
                            Dim remoteBody As String = sr.ReadToEnd
                            Dim trackbackUrl As String = GetTrackbackLink(remoteBody)
                            If Not (trackbackUrl Is Nothing) Then
                                PingBackService.SendTrackBack(trackbackUrl, content, title)
                            End If
                            response.Close()
                        Catch e As Exception

                        End Try
                    End If
                Next
            End If
        End Sub
#End Region

#Region "SEO"

        Public Shared Sub SetPageMetaAndOpenGraph(ByVal defaultPage As CDefault, ByVal modContext As ModuleInstanceContext, ByVal title As String, ByVal content As String, ByVal keyWords As String, ByVal link As String)
            defaultPage.Title = title + " - " + modContext.PortalSettings.PortalName

            Dim meta As New HtmlMeta()
            meta.Attributes.Add("property", "og:title")
            meta.Attributes.Add("content", title)
            defaultPage.Header.Controls.Add(meta)

            content = (HttpUtility.HtmlDecode(content))
            Dim description As String = TruncateString(content, Constants.SeoDescriptionLimit, False)

            If description.Length > 0 Then
                defaultPage.Description = description

                meta = New HtmlMeta()
                meta.Attributes.Add("property", "og:description")
                meta.Attributes.Add("content", description)
                defaultPage.Header.Controls.Add(meta)
            End If

            meta = New HtmlMeta()
            meta.Attributes.Add("property", "og:type")
            meta.Attributes.Add("content", "article")
            defaultPage.Header.Controls.Add(meta)

            If keyWords.Length > 0 Then
                defaultPage.KeyWords = keyWords

                meta = New HtmlMeta()
                meta.Attributes.Add("property", "article:tag")
                meta.Attributes.Add("content", keyWords)
                defaultPage.Header.Controls.Add(meta)
            End If

            meta = New HtmlMeta()
            meta.Attributes.Add("property", "og:url")
            meta.Attributes.Add("content", link)
            defaultPage.Header.Controls.Add(meta)

            meta = New HtmlMeta()
            meta.Attributes.Add("property", "og:site_name")
            meta.Attributes.Add("content", modContext.PortalSettings.PortalName)
            defaultPage.Header.Controls.Add(meta)

            If modContext.PortalSettings.LogoFile.Trim().Length > 0 Then
                Dim url As String = "http://" + modContext.PortalAlias.HTTPAlias + "/Portals/" + modContext.PortalId.ToString() + "/" + modContext.PortalSettings.LogoFile
                meta = New HtmlMeta()
                meta.Attributes.Add("property", "og:image")
                meta.Attributes.Add("content", url)
                defaultPage.Header.Controls.Add(meta)
            End If
        End Sub

        Public Shared Function TruncateString(source As String, length As Integer, showElipse As Boolean) As String
            If source.Length > length Then
                source = source.Substring(0, length)
                If showElipse Then
                    source += "..."
                End If
            End If
            Return source
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="date"></param>
        ''' <returns></returns>
        Public Shared Function CalculateDateForDisplay([date] As DateTime) As String
            Dim utcDate As DateTime = [date].ToUniversalTime()
            Dim utcTimeDifference As TimeSpan = Services.SystemDateTime.SystemDateTime.GetCurrentTimeUtc() - utcDate

            If utcTimeDifference.TotalSeconds < 60 Then
                Return CInt(utcTimeDifference.TotalSeconds).ToString() + Localization.GetString("secondsago", Constants.SharedResourceFileName)
            End If
            If utcTimeDifference.TotalMinutes < 60 Then
                If utcTimeDifference.TotalMinutes < 2 Then
                    Return CInt(utcTimeDifference.TotalMinutes).ToString() + Localization.GetString("minuteago", Constants.SharedResourceFileName)
                End If
                Return CInt(utcTimeDifference.TotalMinutes).ToString() + Localization.GetString("minutesago", Constants.SharedResourceFileName)
            End If
            If utcTimeDifference.TotalHours < 24 Then
                If utcTimeDifference.TotalHours < 2 Then
                    Return CInt(utcTimeDifference.TotalHours).ToString() + Localization.GetString("hourago", Constants.SharedResourceFileName)
                End If
                Return CInt(utcTimeDifference.TotalHours).ToString() + Localization.GetString("hoursago", Constants.SharedResourceFileName)
            End If

            If utcTimeDifference.TotalDays < 7 Then
                If utcTimeDifference.TotalDays < 2 Then
                    Return CInt(utcTimeDifference.TotalDays).ToString() + Localization.GetString("dayago", Constants.SharedResourceFileName)
                End If
                Return CInt(utcTimeDifference.TotalDays).ToString() + Localization.GetString("daysago", Constants.SharedResourceFileName)
            End If

            If utcTimeDifference.TotalDays < 30 Then
                If utcTimeDifference.TotalDays < 14 Then
                    Return CInt((utcTimeDifference.TotalDays) / 7).ToString() + Localization.GetString("weekago", Constants.SharedResourceFileName)
                End If
                Return CInt((utcTimeDifference.TotalDays) / 7).ToString() + Localization.GetString("weeksago", Constants.SharedResourceFileName)
            End If

            If utcTimeDifference.TotalDays < 180 Then
                If utcTimeDifference.TotalDays < 60 Then
                    Return CInt((utcTimeDifference.TotalDays) / 30).ToString() + Localization.GetString("monthago", Constants.SharedResourceFileName)
                End If
                Return CInt((utcTimeDifference.TotalDays) / 30).ToString() + Localization.GetString("monthsago", Constants.SharedResourceFileName)
            End If

            'if (utcTimeDifference.TotalDays < 60)
            '{
            '    return 1 + Localization.GetString("monthago", Constants.SharedResourceFileName);
            '}

            ' anything else (this is the only time we have to personalize it to the user)
            Return [date].ToShortDateString()
        End Function

#End Region

#Region "Urls"

        Public Shared Function GetSEOLink(ByVal PortalId As Integer, ByVal TabID As Integer, ByVal ControlKey As String, ByVal Title As String, ByVal ParamArray AdditionalParameters() As String) As String
            If BlogSettings.GetBlogSettings(PortalId, TabID).ShowSeoFriendlyUrl Then
                Dim TabInfo As DotNetNuke.Entities.Tabs.TabInfo = (New DotNetNuke.Entities.Tabs.TabController).GetTab(TabID, PortalId, False)
                Dim Path As String = "~/default.aspx?tabid=" & TabInfo.TabID
                For Each p As String In AdditionalParameters
                    Path &= "&" & p
                Next
                If String.IsNullOrEmpty(Title) Then Title = "Default.aspx"
                Return DotNetNuke.Common.Globals.FriendlyUrl(TabInfo, Path, Title)
            Else
                Return NavigateURL(TabID, ControlKey, AdditionalParameters)
            End If
        End Function

        Public Shared Function GenerateEntryLink(ByVal PortalID As Integer, ByVal EntryID As Integer, ByVal TabID As Integer, Optional ByVal EntryTitle As String = Nothing) As String
            Dim sReturnURL As String

            If EntryTitle Is Nothing Then
                ' DW - 04/22/2008 - Added to allow the use of the new BlogNavigateURL method
                Dim EntryController As New EntryController
                Dim EntryInfo As EntryInfo = EntryController.GetEntry(EntryID, PortalID)
                If Not EntryInfo Is Nothing Then
                    EntryTitle = EntryInfo.Title
                End If
            End If

            ' Get Blog Module Settings
            Dim BlogModuleSettings As BlogSettings = BlogSettings.GetBlogSettings(PortalID, TabID)
            Dim SeoFriendlyUrl As Boolean = BlogModuleSettings.ShowSeoFriendlyUrl

            'Set the title to Default if there is no Title
            If EntryTitle Is Nothing Or EntryTitle = String.Empty Then
                EntryTitle = "Default"
            End If

            'Get the URL using the Globals.NavigateURL function
            sReturnURL = Utility.BlogNavigateURL(TabID, PortalID, EntryID, EntryTitle, SeoFriendlyUrl)

            If Left(sReturnURL, 4) <> "http" Then
                ' Rip Rowan 6/20/2008
                ' ensure that URL begins with host URL information
                ' kludgy, and maybe totally unnecessary, but it should prevent possible problems
                ' (code provided with sincere apologies to my CS201 professor)
                ' DW - 06/21/2008 - Changed to work when Friendly URLs are not checked at the host level
                Dim HostURL As String = HttpContext.Current.Request.Url.Scheme & "://" & HttpContext.Current.Request.Url.Host
                sReturnURL = HostURL + sReturnURL
            End If

            Return sReturnURL
        End Function

        Public Shared Function RewriteRefs(ByVal html As String) As String
            Dim strInput As String
            Dim HRefPattern As String
            Dim hRef As String
            HRefPattern = "<(a|link|img|script).[^>]*(href|src)=(\""|'|)(.[^\""\s]*)(\""|'|)[^>]*>"
            strInput = HttpUtility.HtmlDecode(html)
            Dim hrefMatches As MatchCollection
            hrefMatches = Regex.Matches(strInput, HRefPattern, RegexOptions.IgnoreCase)
            If hrefMatches.Count > 0 Then
                Try
                    For Each match As Match In hrefMatches
                        hRef = match.Groups(match.Groups.Count - 2).Value
                        If (Regex.IsMatch(match.Value.ToLower, "<img.[^>]*>")) Then
                            'strInput = strInput.Replace(hRef, "http://" & HttpContext.Current.Request.Url.Host & hRef)
                            strInput = strInput
                        End If

                        If (Regex.IsMatch(match.Value.ToLower, "<a.[^>]*>")) Then
                            If Not hRef.StartsWith("http") Then
                                strInput = strInput.Replace(hRef, "http://" & HttpContext.Current.Request.Url.Host & hRef)
                            End If
                        End If
                    Next
                Catch ex As Exception
                    LogException(ex)
                End Try
            End If
            Return strInput
        End Function

        Public Shared Function checkUriFormat(ByVal url As String) As String
            ' BLG-8225 DW 08/12/2008 - http://support.dotnetnuke.com/issue/ViewIssue.aspx?id=8225&PROJID=29
            If url.StartsWith("http") Then
                Return url
            Else
                Return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) & url
            End If
        End Function

        Public Shared Sub CreateAllEntryLinks(ByVal PortalID As Integer, Optional ByVal BlogId As Integer = -1, Optional ByVal TabID As Integer = -1)
            Dim m_EntryController As New EntryController
            Dim m_Entries As New List(Of EntryInfo)
            Dim TabIdFromPortalId As Integer = -1 'Needed only if TabID isn't passed in and TabID can't be retrieved
            ' from the PermaLink

            m_Entries = m_EntryController.ListAllEntriesByPortal(PortalID, True)
            For Each entry As EntryInfo In m_Entries
                If (BlogId = entry.BlogID Or BlogId = -1) And (TabID = -1 _
                                                               Or entry.PermaLink Is Nothing Or entry.PermaLink = String.Empty _
                                                               Or entry.PermaLink.ToLower().IndexOf("tabid=" & TabID) > 0 _
                                                               Or entry.PermaLink.ToLower().IndexOf("tabid/" & TabID) > 0) Then

                    Dim CurrentTabId As Integer
                    If TabID = -1 Then
                        ' Get the TabID from the PortalId
                        ' This is the case when the procedure is being called from the 
                        ' Blog settings page and we're trying to update all the 
                        ' Permalinks after making a change to the URL format.
                        ' In this case, the correct TabID is in the PermaLink, so
                        ' we'll extract it with some regex.
                        ' 
                        Dim match As Match
                        match = Regex.Match(entry.PermaLink, "tabid(?:/|=)(?<TabId>\d*)", RegexOptions.IgnoreCase)
                        If Not match.Groups("TabId").Value Is Nothing And match.Groups("TabId").Value <> String.Empty Then
                            CurrentTabId = Convert.ToInt32(match.Groups("TabId").Value)
                        Else
                            ' Otherwise, we'll try to get the TabID from the PortalID
                            If TabIdFromPortalId = -1 Then
                                'We haven't retrieved this yet, so we'll retreive it
                                TabIdFromPortalId = Utility.GetTabIDByPortalID(PortalID.ToString())
                            End If
                            CurrentTabId = TabIdFromPortalId
                        End If
                    Else
                        CurrentTabId = TabID
                    End If
                    entry.PermaLink = GenerateEntryLink(PortalID, entry.EntryID, CurrentTabId)
                    m_EntryController.UpdateEntry(entry, CurrentTabId, PortalID)
                End If
            Next
        End Sub

        Public Shared Function BlogNavigateURL(ByVal TabId As Integer, ByVal PortalID As Integer, ByVal EntryInfo As EntryInfo, ByVal ShowSEOFriendly As Boolean) As String
            Dim TabInfo As DotNetNuke.Entities.Tabs.TabInfo
            Dim TabController As New DotNetNuke.Entities.Tabs.TabController

            'Retrieve the TabInfo object from the TabId
            TabInfo = TabController.GetTab(TabId, PortalID, False)

            Return BlogNavigateURL(TabInfo, EntryInfo.EntryID.ToString(), EntryInfo.Title, ShowSEOFriendly)
        End Function

        Public Shared Function BlogNavigateURL(ByVal TabId As Integer, ByVal PortalID As Integer, ByVal EntryId As Integer, ByVal EntryTitle As String, ByVal ShowSEOFriendly As Boolean) As String
            Dim TabInfo As DotNetNuke.Entities.Tabs.TabInfo
            Dim TabController As New DotNetNuke.Entities.Tabs.TabController


            'Retrieve the TabInfo object from the TabId
            TabInfo = TabController.GetTab(TabId, PortalID, False)

            Return BlogNavigateURL(TabInfo, EntryId.ToString(), EntryTitle, ShowSEOFriendly)
        End Function

        Public Shared Function CreateFriendlySlug(ByVal pagename As String) As String
            'Set the PageName
            Dim options As RegexOptions = RegexOptions.IgnoreCase
            pagename = pagename.Replace("'", String.Empty)
            'Handle international characters
            pagename = Regex.Replace(pagename, "Ă|Ā|À|Á|Â|Ã|Ä|Å", "A")
            pagename = Regex.Replace(pagename, "ă|ā|à|á|â|ã|ä|å|ą", "a")
            pagename = Regex.Replace(pagename, "Æ", "AE")
            pagename = Regex.Replace(pagename, "æ", "ae")
            pagename = Regex.Replace(pagename, "ß", "ss")
            pagename = Regex.Replace(pagename, "Ç|Ć|Ĉ|Ċ|Č", "C")
            pagename = Regex.Replace(pagename, "ć|ĉ|ċ|č|ç", "c")
            pagename = Regex.Replace(pagename, "Ď|Đ", "D")
            pagename = Regex.Replace(pagename, "ď|đ", "d")
            pagename = Regex.Replace(pagename, "Ē|Ĕ|Ė|Ę|Ě|É|Ę|È|É|Ê|Ë", "E")
            pagename = Regex.Replace(pagename, "ē|ĕ|ė|ę|ě|ê|ë|è|é", "e")
            pagename = Regex.Replace(pagename, "Ĝ|Ğ|Ġ|Ģ|Ģ", "G")
            pagename = Regex.Replace(pagename, "ĝ|ğ|ġ|ģ|ģ", "g")
            pagename = Regex.Replace(pagename, "Ĥ|Ħ", "H")
            pagename = Regex.Replace(pagename, "ĥ|ħ", "h")
            pagename = Regex.Replace(pagename, "Ì|Í|Î|Ï|Ĩ|Ī|Ĭ|Į|İ|İ", "I")
            pagename = Regex.Replace(pagename, "ì|í|î|ï|ĩ|ī|ĭ|į", "i")
            pagename = Regex.Replace(pagename, "Ĳ", "IJ")
            pagename = Regex.Replace(pagename, "Ĵ", "J")
            pagename = Regex.Replace(pagename, "ĵ", "j")
            pagename = Regex.Replace(pagename, "Ķ", "K")
            pagename = Regex.Replace(pagename, "ķ", "k")
            pagename = Regex.Replace(pagename, "Ñ|Ñ", "N")
            pagename = Regex.Replace(pagename, "ñ", "n")
            pagename = Regex.Replace(pagename, "Ò|Ó|Ô|Õ|Ö|Ø|Ő", "O")
            pagename = Regex.Replace(pagename, "ò|ó|ô|õ|ö|ø|ő", "o")
            pagename = Regex.Replace(pagename, "Œ", "OE")
            pagename = Regex.Replace(pagename, "œ", "oe")
            pagename = Regex.Replace(pagename, "Ŕ|Ř|Ŗ|Ŕ", "R")
            pagename = Regex.Replace(pagename, "ř|ŗ|ŕ", "r")
            pagename = Regex.Replace(pagename, "Š|Ş|Ŝ|Ś", "S")
            pagename = Regex.Replace(pagename, "š|ş|ŝ|ś", "s")
            pagename = Regex.Replace(pagename, "Ť|Ţ", "T")
            pagename = Regex.Replace(pagename, "ť|ţ", "t")
            pagename = Regex.Replace(pagename, "Ų|Ű|Ů|Ŭ|Ū|Ũ|Ù|Ú|Û|Ü", "U")
            pagename = Regex.Replace(pagename, "ų|ű|ů|ŭ|ū|ũ|ú|û|ü|ù", "u")
            pagename = Regex.Replace(pagename, "Ŵ", "W")
            pagename = Regex.Replace(pagename, "ŵ", "w")
            pagename = Regex.Replace(pagename, "Ÿ|Ŷ|Ý", "Y")
            pagename = Regex.Replace(pagename, "ŷ|ÿ|ý", "y")
            pagename = Regex.Replace(pagename, "Ž|Ż|Ź", "Z")
            pagename = Regex.Replace(pagename, "ž|ż|ź", "z")

            pagename = Regex.Replace(pagename, "[^a-z0-9_-ĂăĀāÀÁÂÃÄÅàáâãäåąæÆßÇĆćĈĉĊċČčçĎďĐđĒēĔĕĖėĘęĚěÉêëĘÈÉÊËèéĜĝĞğĠġĢģĢģĤĥĦħÌÍÎÏĨĩĪīĬĭĮįİÌíîïìĲĴĵĶķÑÑÒÓÔÕÖŐØòóôõőöøñŒœŔřŘŗŖŕŔšŠşŞŝŜśŚťŤţŢųŲűŰůŮŭŬūŪũŨÙÚÛÜÙúûüùŵŴŸŷŶÝÿýžŽżŻźŹ]+", "-", options) & ".aspx"
            'For titles with ' - ', we replace --- with -
            pagename = pagename.Replace("---", "-")

            'Remove trailing dash if one exists.
            If (pagename.EndsWith("-.aspx")) Then
                pagename = pagename.Replace("-.aspx", ".aspx")
            End If

            Return pagename
        End Function

        Private Shared Function BlogNavigateURL(ByVal TabInfo As DotNetNuke.Entities.Tabs.TabInfo, ByVal EntryId As String, ByVal EntryTitle As String, ByVal ShowSEOFriendly As Boolean) As String
            Dim Path As String
            Dim sURL As String = String.Empty

            If ShowSEOFriendly Then
                Dim AppPath As String = HttpContext.Current.Request.ApplicationPath
                If (AppPath = "/") Then AppPath = String.Empty

                'Set the path
                'Path = AppPath + "/default.aspx?tabid=" & TabInfo.TabID & "&EntryId=" & EntryId

                Path = "~/" + DotNetNuke.Common.Globals.glbDefaultPage + "?tabid=" & TabInfo.TabID & "&EntryId=" & EntryId

                sURL = DotNetNuke.Common.Globals.FriendlyUrl(TabInfo, Path, CreateFriendlySlug(EntryTitle))
            Else
                sURL = NavigateURL(TabInfo.TabID, String.Empty, "EntryId=" & EntryId)
            End If

            Return sURL
        End Function

#End Region

#Region "Other"

        Private Shared Sub LogIssue(ByVal Issue As String, ByVal Description As String, ByVal UserId As Integer)
            ' PAD: Orphaned method?
            Dim oEventLog As EventLogController = Nothing
            Dim PortalSettings As DotNetNuke.Entities.Portals.PortalSettings
            Try
                oEventLog = New EventLogController
                PortalSettings = PortalController.GetCurrentPortalSettings

                oEventLog.AddLog(Issue, Description, PortalSettings, UserId, DotNetNuke.Services.Log.EventLog.EventLogController.EventLogType.ADMIN_ALERT)
            Catch ex As Exception
            Finally
                If oEventLog IsNot Nothing Then oEventLog = Nothing
            End Try
        End Sub

#End Region

    End Class

End Namespace