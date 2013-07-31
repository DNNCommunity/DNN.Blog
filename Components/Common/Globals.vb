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

Imports System.Linq
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Localization.Localization
Imports DotNetNuke.Entities.Content.Taxonomy

Namespace Common

 Public Class Globals

#Region " Constants "
  Public Const SharedResourceFileName As String = "~/DesktopModules/Blog/App_LocalResources/SharedResources.resx"
  Public Const glbAppName As String = "Blog"
  Public Const glbImageHandlerPath As String = "~/DesktopModules/Blog/BlogImage.ashx"
  Public Const glbPermittedFileExtensions As String = ".jpg,.png,.gif,.bmp,"
  Public Const glbTemplatesPath As String = "~/DesktopModules/Blog/Templates/"
  Public Const glbServicesPath As String = "~/DesktopModules/Blog/API/"
  Public Const BloggerPermission As String = "BLOGGER"

  Public Enum SummaryType
   PlainTextIndependent = 0
   HtmlIndependent = 1
   HtmlPrecedesPost = 2
  End Enum

  Public Enum LocalizationType
   None = 0
   Loose = 1
   Strict = 2
  End Enum
#End Region

#Region " Dates "
  Public Shared Function UtcToLocalTime(utcTime As Date, TimeZone As TimeZoneInfo) As Date
   Return TimeZoneInfo.ConvertTimeFromUtc(utcTime, TimeZone)
  End Function

  Public Shared Function UtcToLocalTime(utcTime As Date) As Date
   Return Date.SpecifyKind(utcTime, DateTimeKind.Utc).ToLocalTime
  End Function

  Public Shared Function ParseDate(DateString As String, Culture As String) As DateTime
   Dim dtf As System.Globalization.DateTimeFormatInfo = New System.Globalization.CultureInfo(Culture, False).DateTimeFormat
   Try
    Return Date.Parse(DateString, dtf)
   Catch ex As Exception
    Return Nothing
   End Try
  End Function

  Public Shared Function IsValidDate(DateString As String, Culture As String) As Boolean
   Dim dtf As System.Globalization.DateTimeFormatInfo = New System.Globalization.CultureInfo(Culture, False).DateTimeFormat
   Try
    Dim oDate As Date = Date.Parse(DateString, dtf)
    Return True
   Catch ex As Exception
    Return False
   End Try
  End Function

  Public Shared Function GetLocalAddedTime(AddedDate As DateTime, PortalId As Integer, user As DotNetNuke.Entities.Users.UserInfo) As DateTime
   Return TimeZoneInfo.ConvertTimeToUtc(AddedDate, user.Profile.PreferredTimeZone)
  End Function
#End Region

#Region " Other "
  Public Shared Function GetBlogDirectoryMapPath(blogId As Integer) As String
   Return String.Format("{0}Blog\Files\{1}\", DotNetNuke.Entities.Portals.PortalSettings.Current.HomeDirectoryMapPath, blogId)
  End Function
  Public Shared Function GetBlogDirectoryPath(blogId As Integer) As String
   Return String.Format("{0}Blog/Files/{1}/", DotNetNuke.Entities.Portals.PortalSettings.Current.HomeDirectory, blogId)
  End Function
  Public Shared Function GetPostDirectoryMapPath(blogId As Integer, postId As Integer) As String
   Return String.Format("{0}Blog\Files\{1}\{2}\", DotNetNuke.Entities.Portals.PortalSettings.Current.HomeDirectoryMapPath, blogId, postId)
  End Function
  Public Shared Function GetPostDirectoryPath(blogId As Integer, postId As Integer) As String
   Return String.Format("{0}Blog/Files/{1}/{2}/", DotNetNuke.Entities.Portals.PortalSettings.Current.HomeDirectory, blogId, postId)
  End Function
  Public Shared Function GetPostDirectoryMapPath(post As Entities.Posts.PostInfo) As String
   Return GetPostDirectoryMapPath(post.BlogID, post.ContentItemId)
  End Function
  Public Shared Function GetPostDirectoryPath(post As Entities.Posts.PostInfo) As String
   Return GetPostDirectoryPath(post.BlogID, post.ContentItemId)
  End Function
  Public Shared Function GetTempPostDirectoryMapPath(blogId As Integer) As String
   Return String.Format("{0}Blog\Files\{1}\_temp_images\", DotNetNuke.Entities.Portals.PortalSettings.Current.HomeDirectoryMapPath, blogId)
  End Function
  Public Shared Function GetTempPostDirectoryPath(blogId As Integer) As String
   Return String.Format("{0}Blog/Files/{1}/_temp_images/", DotNetNuke.Entities.Portals.PortalSettings.Current.HomeDirectory, blogId)
  End Function

  Public Shared Function ManifestFilePath(tabId As Integer, moduleId As Integer) As String
   Return String.Format("~/DesktopModules/Blog/API/Modules/Manifest?TabId={0}&ModuleId={1}", tabId, moduleId)
  End Function

  Public Shared Function GetAString(Value As Object) As String
   If Value Is Nothing Then
    Return ""
   Else
    If Value Is DBNull.Value Then
     Return ""
    Else
     Return CType(Value, String)
    End If
   End If
  End Function

  Public Shared Function ReadFile(fileName As String) As String
   If Not IO.File.Exists(fileName) Then Return ""
   Using sr As New IO.StreamReader(fileName)
    Return sr.ReadToEnd
   End Using
  End Function

  Public Shared Sub WriteToFile(filePath As String, text As String)
   WriteToFile(filePath, text, False)
  End Sub
  Public Shared Sub WriteToFile(filePath As String, text As String, append As Boolean)
   Using sw As New IO.StreamWriter(filePath, append)
    sw.Write(text)
    sw.Flush()
   End Using
  End Sub

  Public Shared Function GetResource(resourceName As String) As String
   Dim res As String = ""
   Using stream As IO.Stream = System.Reflection.Assembly.GetExecutingAssembly.GetManifestResourceStream(resourceName)
    Using rdr As New IO.StreamReader(stream)
     res = rdr.ReadToEnd
    End Using
   End Using
   Return res
  End Function

  Public Shared Function FormatBoolean(value As Boolean, format As String) As String
   If String.IsNullOrEmpty(format) Then
    Return value.ToString
   End If
   If format.Contains(";") Then
    If value Then
     Return Left(format, format.IndexOf(";"))
    Else
     Return Mid(format, format.IndexOf(";") + 2)
    End If
   End If
   Return value.ToString
  End Function

  Public Shared Function GetSummary(body As String, autoGenerateLength As Integer, summaryModel As SummaryType) As String
   Dim res As String = RemoveHtmlTags(body).Substring(0, autoGenerateLength)
   If summaryModel = SummaryType.PlainTextIndependent Then
    Return res
   Else
    Return String.Format("<p>{0} ...</p>", res)
   End If
  End Function

  Public Shared Function RemoveHtmlTags(inputString As String) As String
   inputString = Regex.Replace(inputString, "<[^>]+>", "")
   Return (New DotNetNuke.Security.PortalSecurity).InputFilter(inputString, DotNetNuke.Security.PortalSecurity.FilterFlag.NoScripting Or DotNetNuke.Security.PortalSecurity.FilterFlag.NoMarkup)
  End Function

  Friend Shared Function GetPortalVocabularies(portalId As Integer) As List(Of Vocabulary)
   Dim cntVocab As IVocabularyController = DotNetNuke.Entities.Content.Common.Util.GetVocabularyController()
   Dim colVocabularies As IQueryable(Of Vocabulary) = cntVocab.GetVocabularies()
   Dim portalVocabularies As IQueryable(Of Vocabulary) = From v In colVocabularies Where v.ScopeTypeId = 2 And v.ScopeId = portalId
   Return portalVocabularies.ToList()
  End Function

  Public Shared Function GetSafePageName(pageName As String) As String
   Return Regex.Replace(Regex.Replace(pageName, "[^\w^\d]", "-").Trim("-"c), "-+", "-")
  End Function

  Public Shared Sub RemoveOldTimeStampedFiles(dir As IO.DirectoryInfo)
   Dim today As String = Date.Now.ToString("yyyy-MM-dd")
   Dim deleteList As New List(Of String)
   For Each f As IO.FileInfo In dir.GetFiles
    Dim m As Match = Regex.Match(f.Name, "^(\d\d\d\d-\d\d-\d\d)-")
    If m.Success Then
     If m.Groups(1).Value < today Then
      deleteList.Add(f.FullName)
     End If
    End If
   Next
   For Each f As String In deleteList
    Try
     IO.File.Delete(f)
    Catch ex As Exception
    End Try
   Next
  End Sub

  Public Shared Function GetRolesByGroup(portalId As Integer, roleGroupId As Integer) As List(Of DotNetNuke.Security.Roles.RoleInfo)
   Return DotNetNuke.Security.Roles.RoleProvider.Instance.GetRoles(portalId).Cast(Of DotNetNuke.Security.Roles.RoleInfo).Where(Function(r) r.RoleGroupID = roleGroupId).ToList
  End Function

  Public Shared Function GetRolesByPortal(portalId As Integer) As List(Of DotNetNuke.Security.Roles.RoleInfo)
   Return DotNetNuke.Security.Roles.RoleProvider.Instance.GetRoles(portalId).Cast(Of DotNetNuke.Security.Roles.RoleInfo).ToList
  End Function

  Public Shared Function SafeString(input As String, filter As DotNetNuke.Security.PortalSecurity.FilterFlag) As String
   Dim ps As New DotNetNuke.Security.PortalSecurity
   Return ps.InputFilter(input, filter)
  End Function

  Public Shared Function SafeString(input As String) As String
   Return SafeString(input, DotNetNuke.Security.PortalSecurity.FilterFlag.NoMarkup And DotNetNuke.Security.PortalSecurity.FilterFlag.NoProfanity And DotNetNuke.Security.PortalSecurity.FilterFlag.NoScripting And DotNetNuke.Security.PortalSecurity.FilterFlag.NoSQL)
  End Function

  Public Shared Function SafeHtml(input As String) As String
   Return SafeString(input, DotNetNuke.Security.PortalSecurity.FilterFlag.NoProfanity And DotNetNuke.Security.PortalSecurity.FilterFlag.NoScripting And DotNetNuke.Security.PortalSecurity.FilterFlag.NoSQL)
  End Function

  Public Shared Function SafeStringSimpleHtml(input As String) As String
   input = SafeString(input)
   input = Regex.Replace(input, "(?#Protocol)(?:(?:ht|f)tp(?:s?)\:\/\/|~\/|\/)?(?#Username:Password)(?:\w+:\w+@)?(?#Subdomains)(?:(?:[-\w]+\.)+(?#TopLevel Domains)(?:com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|travel|[a-z]{2}))(?#Port)(?::[\d]{1,5})?(?#Directories)(?:(?:(?:\/(?:[-\w~!$+|.,=]|%[a-f\d]{2})+)+|\/)+|\?|#)?(?#Query)(?:(?:\?(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=?(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)(?:&(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=?(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)*)*(?#Anchor)(?:#(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)?", AddressOf ReplaceLink)
   input = input.Replace(vbCrLf, "<br />")
   Return input
  End Function
  Public Shared Function ReplaceLink(m As Match) As String
   Dim link As String = m.Value
   Return String.Format("<a href=""{0}"">{0}</a>", link)
  End Function

  'Public Shared Sub WriteMultiLingualText(writer As System.Xml.XmlWriter, name As String, defaultText As String, localizedTexts As LocalizedText)
  ' writer.WriteStartElement(name)
  ' writer.WriteElementString("Default", defaultText)
  ' For Each locale As String In localizedTexts.Locales
  '  If Not String.IsNullOrEmpty(localizedTexts(locale)) Then
  '   writer.WriteStartElement("Text")
  '   writer.WriteAttributeString("Locale", locale)
  '   writer.WriteValue(localizedTexts(locale))
  '   writer.WriteEndElement() ' Text
  '  End If
  ' Next
  ' writer.WriteEndElement() ' name
  'End Sub

  'Public Shared Sub ReadMultiLingualText(reader As System.Xml.XmlReader, name As String, ByRef defaultText As String, ByRef localizedTexts As LocalizedText)
  ' reader.ReadStartElement(name) ' advance to name
  ' defaultText = readElement(reader, "Default")
  ' localizedTexts = New LocalizedText
  ' Do While reader.ReadToNextSibling("Text")
  '  Dim locale As String = readAttribute(reader, "Locale")
  '  Dim text As String = reader.ReadElementContentAsString
  '  localizedTexts.Add(locale, text)
  ' Loop
  'End Sub

  Public Shared Function readElement(reader As System.Xml.XmlReader, ElementName As String) As String
   If (Not reader.NodeType = System.Xml.XmlNodeType.Element) OrElse reader.Name <> ElementName Then
    reader.ReadToFollowing(ElementName)
   End If
   If reader.NodeType = System.Xml.XmlNodeType.Element Then
    Return reader.ReadElementContentAsString
   Else
    Return ""
   End If
  End Function

  Public Shared Function readAttribute(reader As System.Xml.XmlReader, attributeName As String) As String
   If (Not reader.NodeType = System.Xml.XmlNodeType.Attribute) OrElse reader.Name <> attributeName Then
    reader.ReadToFollowing(attributeName)
   End If
   If reader.NodeType = System.Xml.XmlNodeType.Attribute Then
    Return reader.ReadContentAsString
   Else
    Return ""
   End If
  End Function
#End Region

 End Class

End Namespace