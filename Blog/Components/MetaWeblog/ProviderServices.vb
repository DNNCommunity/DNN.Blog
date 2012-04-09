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
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Web
Imports DotNetNuke.Data
Imports DotNetNuke.Modules.Blog.Business
Imports DotNetNuke.Services.Localization

Namespace MetaWeblog

    Public Class BlogPostServices

        Public Const IMPLEMENTED_BY_MODULE As String = "Implemented_By_Module"

        ''' <summary>
        ''' AuthorizeUser checks to make sure this user still has edit rights to this module.  If not, then 
        ''' we return false.
        ''' </summary>
        ''' <param name="blogId"></param>
        ''' <param name="modulesAuthorizedForUser"></param>
        Public Shared Sub AuthorizeUser(ByVal blogId As String, ByVal modulesAuthorizedForUser As ModuleInfoStruct())
            Dim isAuthorized As Boolean = False

            For Each mis As ModuleInfoStruct In modulesAuthorizedForUser
                If mis.BlogID = blogId Then
                    isAuthorized = True
                    Exit For
                End If
            Next

            If Not isAuthorized Then
                Throw New Exception(GetString("AuthenticationError", "The action requested is not authorized for the current user account."))
            End If

        End Sub

        Public Shared Function GetRedirectUrl(ByVal providerKey As String, ByVal TabId As Integer) As String
            Dim appPath As String = HttpContext.Current.Request.ApplicationPath
            If appPath = "/" Then
                appPath = String.Empty
            End If
            Dim returnUrl As String = appPath + "/DesktopModules/blog/blogpostredirect.aspx?tab=" + TabId.ToString
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

        Public Shared Sub ProcessItemImages(ByRef entry As EntryInfo, ByVal rootPath As String)
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
                        If System.IO.File.Exists(moveFromPath) AndAlso IsValidImageLocation(moveFromPath, rootPath) Then
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

                ' Check for any non-image files left behind
                Dim strWhiteList As String = "," & DotNetNuke.Entities.Host.HostSettings.GetHostSetting("FileExtensions").ToLower & ","
                For Each tempFile As Match In Regex.Matches(HttpUtility.HtmlDecode(entry.Entry), """([^""]*_temp_images/[^""]*)""")
                    Try
                        Dim moveFromPath As String = HttpContext.Current.Server.MapPath(tempFile.Groups(1).Value)
                        Dim strExtension As String = Path.GetExtension(moveFromPath).Replace(".", "")
                        If IsValidImageLocation(moveFromPath, rootPath) Then
                            If (Not String.IsNullOrEmpty(strExtension)) AndAlso strWhiteList.IndexOf("," & strExtension.ToLower & ",") > -1 Then
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
                            Else ' we have to delete the file
                                System.IO.File.Delete(moveFromPath)
                            End If
                        End If
                    Catch ex As Exception
                        ' We'll log the error and fail silently so we can attempt to save the other 
                        ' images.  
                        DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
                    End Try
                Next

                ' Clean up old files
                Try
                    Dim path As String = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings.HomeDirectoryMapPath & "Blog\Files\" & entry.BlogID.ToString & "\_temp_images"
                    ' we need to make sure the directory exists
                    If IO.Directory.Exists(path) Then
                        Dim colFiles As Array = New IO.DirectoryInfo(path).GetFiles()

                        If colFiles IsNot Nothing Then
                            For Each f As IO.FileInfo In (colFiles)
                                If f.CreationTime < Now.AddHours(-1) Then
                                    Try
                                        f.Delete()
                                    Catch ex1 As Exception
                                        DotNetNuke.Services.Exceptions.Exceptions.LogException(ex1)
                                    End Try
                                End If
                            Next
                        End If
                    End If
                Catch ex As Exception
                    DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
                End Try

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
            For Each match As Match In matches
                ' extract the src attribute from the image
                Dim regexSrc As String = regexInner
                input = match.Value
                Dim src As Match = Regex.Match(input, regexSrc, options)
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

        Private Shared Function IsValidImageLocation(ByVal path As String, ByVal rootpath As String) As Boolean
            Return path.StartsWith(rootpath)
        End Function

    End Class

End Namespace