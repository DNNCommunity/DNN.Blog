'
' DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2010
' by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
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
'-------------------------------------------------------------------------

Imports System
Imports System.Data
Imports System.Drawing
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Data
Imports DotNetNuke.Modules.Blog.Business
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Framework.Providers
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Services.Localization

Namespace MetaWeblog

 Public Class BlogPostServices
  'Private Shared _providerConfiguration As ProviderConfiguration = ProviderConfiguration.GetProviderConfiguration("data")
  'Private Shared _connectionString As String
  'Private Shared _providerPath As String
  'Private Shared _objectQualifier As String
  'Private Shared _databaseOwner As String

  'Public Shared ReadOnly Property DatabaseOwner() As String
  '    Get
  '        If BlogPostServices.IsNullOrEmpty(_databaseOwner) Then
  '            ' Read the configuration specific information for this provider
  '            Dim objProvider As Provider = DirectCast(_providerConfiguration.Providers(_providerConfiguration.DefaultProvider), Provider)

  '            _databaseOwner = objProvider.Attributes("databaseOwner")
  '            If (_databaseOwner <> "") AndAlso Not _databaseOwner.EndsWith(".") Then
  '                _databaseOwner += "."

  '            End If
  '        End If
  '        Return _databaseOwner
  '    End Get
  'End Property

  'Public Shared ReadOnly Property ObjectQualifier() As String
  '    Get
  '        If BlogPostServices.IsNullOrEmpty(_objectQualifier) Then
  '            ' Read the configuration specific information for this provider
  '            Dim objProvider As Provider = DirectCast(_providerConfiguration.Providers(_providerConfiguration.DefaultProvider), Provider)

  '            _objectQualifier = objProvider.Attributes("objectQualifier")
  '            If (_objectQualifier <> "") AndAlso Not _objectQualifier.EndsWith("_") Then
  '                _objectQualifier += "_"

  '            End If
  '        End If
  '        Return _objectQualifier
  '    End Get
  'End Property

  Public Const IMPLEMENTED_BY_MODULE As String = "Implemented_By_Module"

  Public Shared Function GetRedirectUrl(ByVal providerKey As String, ByVal TabId As Integer) As String
   Dim appPath As String = HttpContext.Current.Request.ApplicationPath
   If appPath = "/" Then
    appPath = String.Empty
   End If
   Dim returnUrl As String = appPath + "/DesktopModules/blog/blogpostredirect.aspx?IntendedUrl=" + DotNetNuke.Common.NavigateURL(TabId) + "&key=" + HttpUtility.UrlEncode(providerKey)
   Return returnUrl
  End Function

  Public Shared Function GetString(ByVal key As String, ByVal defaultValue As String) As String
   Return GetString(key, defaultValue, "/DesktopModules/blog/App_LocalResources/blogpost")
  End Function
  Public Shared Function GetString(ByVal key As String, ByVal defaultValue As String, ByVal localizationFilePath As String) As String
   Dim retValue As String = Localization.GetString(key, localizationFilePath)
   If (retValue Is Nothing) Then
    Return defaultValue
   Else
    Return retValue
   End If
  End Function

  Public Shared Function ContainsHtml(ByVal content As String) As Boolean
   Dim retValue As Boolean = False
   Dim regexPattern As String = "<[div|table|span|tr|p|a|h\d|td|ul|li|blockquote|font]+[^>]*>"
   Dim options As RegexOptions = RegexOptions.Singleline

   Dim matches As MatchCollection = Regex.Matches(content, regexPattern, options)

   If matches.Count > 0 Then
    retValue = True
   End If
   Return retValue
  End Function
  Public Shared Function GetFriendlyNameFromModuleDefinition(ByVal moduleDefinition As String) As String
   Dim friendlyName As String

   Dim SQL As String = "Blog_MetaWeblog_Get_DesktopModule_FriendlyName"
   Dim methodParams As Object() = {moduleDefinition}
   friendlyName = DirectCast(DataProvider.Instance().ExecuteScalar(SQL, methodParams), String)

   Return friendlyName
  End Function

  Public Shared Sub ProcessItemImages(ByRef entry As EntryInfo)
   Dim imageUrls As New ArrayList

   Try

    ' When the newMediaObject method of the MetaWeblog API is called, we don't 
    ' have enough information to specify the blog entry id for the image path.
    ' So, we place the images in a folder named _temp_images until the 
    ' EditItem method is called (which calls this procedure).  EditItem contains
    ' the item parameter which has an itemId corresponding to the Entry Id of the 
    ' blog post.  We'll replace _temp_images with this EntryId and we'll 
    ' move the images to the right folder.  In order to move the images to the right
    ' folder, we'll use Regex to find the images in the post.

    Dim regexSrc As String = "<img[^>]+?_temp_images/.*?>"
    Dim regexHref As String = "<a[^>]+?(?:png|jpg|jpeg|gif)[^>]+?>[^<]*?<img[^>]+?src=""[^""]+?""[^>]+>[^<]*?</a>"
    ' Note that the inner Regex patterns required a group named 'src'
    Dim regexInnerSrc As String = "src=""(?<src>[^""]+?)"""
    Dim regexInnerHref As String = "href=""(?<src>[^""]+?)"""

    Dim options As RegexOptions = RegexOptions.IgnoreCase Or RegexOptions.Singleline
    Dim input As String = HttpUtility.HtmlDecode(entry.Description + entry.Entry)

    If Not BlogPostServices.IsNullOrEmpty(input) Then
     FindImageMatch(input, regexSrc, regexInnerSrc, options, imageUrls)
     FindImageMatch(input, regexHref, regexInnerHref, options, imageUrls)
    End If

    ' OK, now that we have the matches stored in the imageUrls Arraylist, we'll use this to 
    ' move the images to the right location.  

    For Each url As String In imageUrls
     Try
      Dim moveFromPath As String = HttpContext.Current.Server.MapPath(url)
      Dim moveToPath As String = moveFromPath.Replace("_temp_images", entry.EntryID.ToString())
      Dim moveToFolderPath As String = moveToPath.Substring(0, moveToPath.LastIndexOf("\"))
      ' Make sure the directory exists
      If Not Directory.Exists(moveToFolderPath) Then
       ' No problem, we'll just create it!
       Directory.CreateDirectory(moveToFolderPath)
      End If
      ' File may already have been moved.  We'll check first to see.
      ' Files will haev already been moved in the case where an entry is
      ' reposted from Windows Live Writer.
      If System.IO.File.Exists(moveFromPath) Then
       'Check to see if we need to overwrite an existing image
       If System.IO.File.Exists(moveToPath) Then
        System.IO.File.Delete(moveToPath)
       End If
       System.IO.File.Move(moveFromPath, moveToPath)
      End If
     Catch ex As Exception
      ' We'll log the error and fail silently so we can attempt to save the other 
      ' images.  
      DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
     End Try
    Next

    ' Finally, we'll update the URLs
    If Not BlogPostServices.IsNullOrEmpty(entry.Entry) Then
     entry.Entry = entry.Entry.Replace("_temp_images", entry.EntryID.ToString())
    End If
    If Not BlogPostServices.IsNullOrEmpty(entry.Description) Then
     entry.Description = entry.Description.Replace("_temp_images", entry.EntryID.ToString())
    End If
    'Yes!  We made it!!  'The images should be tucked in bed
   Catch ex As Exception
    DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
   End Try
  End Sub
  Private Shared Sub FindImageMatch(ByVal input As String, ByVal sRegex As String, ByVal regexInner As String, ByVal options As RegexOptions, ByVal imageUrls As ArrayList)
   Dim matches As MatchCollection = Regex.Matches(input, sRegex, options)
   For Each match As match In matches
    ' extract the src attribute from the image
    Dim regexSrc As String = regexInner
    input = match.Value
    Dim src As match = Regex.Match(input, regexSrc, options)
    If Not src Is Nothing AndAlso src.Groups("src").Captures.Count > 0 Then
     ' We have an image Url
     imageUrls.Add(src.Groups("src").Captures(0).Value)
    End If
   Next
  End Sub
  Friend Shared Function IsNullOrEmpty(ByVal value As String) As Boolean
   Dim retValue As Boolean
   If value Is Nothing OrElse value = String.Empty Then
    retValue = True
   Else
    retValue = False
   End If
   Return retValue
  End Function
 End Class
End Namespace
