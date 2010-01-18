'
' Yedda Twitter C# Library (or more of an API wrapper) v0.1
' Written by Eran Sandler (eran AT yedda.com)
' http://devblog.yedda.com/index.php/twitter-c-library/
'
' The library is provided on a "AS IS" basis. Yedda is not repsonsible in any way 
' for whatever usage you do with it.
'
' Giving credit would be nice though :-)
'
' Get more cool dev information and other stuff at the Yedda Dev Blog:
' http://devblog.yedda.com
'
' Got a question about this library? About programming? C#? .NET? About anything else?
' Ask about it at Yedda (http://yedda.com) and get answers from real people.
'
'
' The original code was converted to VB.NET by Peter Donker on Jan 18, 2010
' I also needed to make an edit to get this to work
'

Imports System
Imports System.IO
Imports System.Net
Imports System.Xml
Imports System.Web
Imports System.Collections.Generic
Imports System.Text

Namespace Twitter
 Public Class Twitter

#Region " Enums "
  ''' <summary>
  ''' The output formats supported by Twitter. Not all of them can be used with all of the functions.
  ''' For more information about the output formats and the supported functions Check the 
  ''' Twitter documentation at: http://groups.google.com/group/twitter-development-talk/web/api-documentation
  ''' </summary>
  Public Enum OutputFormatType
   JSON
   XML
   RSS
   Atom
  End Enum

  ''' <summary>
  ''' The various object types supported at Twitter.
  ''' </summary>
  Public Enum ObjectType
   Statuses
   Account
   Users
  End Enum

  ''' <summary>
  ''' The various actions used at Twitter. Not all actions works on all object types.
  ''' For more information about the actions types and the supported functions Check the 
  ''' Twitter documentation at: http://groups.google.com/group/twitter-development-talk/web/api-documentation
  ''' </summary>
  Public Enum ActionType
   Public_Timeline
   User_Timeline
   Friends_Timeline
   Friends
   Followers
   Update
   Account_Settings
   Featured
   Show
  End Enum
#End Region

#Region " Private Members "
  Private m_source As String = Nothing
  Private m_twitterClient As String = Nothing
  Private m_twitterClientVersion As String = Nothing
  Private m_twitterClientUrl As String = Nothing
#End Region

#Region " Properties "
  ''' <summary>
  ''' Source is an additional parameters that will be used to fill the "From" field.
  ''' Currently you must talk to the developers of Twitter at:
  ''' http://groups.google.com/group/twitter-development-talk/
  ''' Otherwise, Twitter will simply ignore this parameter and set the "From" field to "web".
  ''' </summary>
  Public Property Source() As String
   Get
    Return m_source
   End Get
   Set(ByVal value As String)
    m_source = value
   End Set
  End Property

  ''' <summary>
  ''' Sets the name of the Twitter client.
  ''' According to the Twitter Fan Wiki at http://twitter.pbwiki.com/API-Docs and supported by
  ''' the Twitter developers, this will be used in the future (hopefully near) to set more information
  ''' in Twitter about the client posting the information as well as future usage in a clients directory.
  ''' </summary>
  Public Property TwitterClient() As String
   Get
    Return m_twitterClient
   End Get
   Set(ByVal value As String)
    m_twitterClient = value
   End Set
  End Property

  ''' <summary>
  ''' Sets the version of the Twitter client.
  ''' According to the Twitter Fan Wiki at http://twitter.pbwiki.com/API-Docs and supported by
  ''' the Twitter developers, this will be used in the future (hopefully near) to set more information
  ''' in Twitter about the client posting the information as well as future usage in a clients directory.
  ''' </summary>
  Public Property TwitterClientVersion() As String
   Get
    Return m_twitterClientVersion
   End Get
   Set(ByVal value As String)
    m_twitterClientVersion = value
   End Set
  End Property

  ''' <summary>
  ''' Sets the URL of the Twitter client.
  ''' Must be in the XML format documented in the "Request Headers" section at:
  ''' http://twitter.pbwiki.com/API-Docs.
  ''' According to the Twitter Fan Wiki at http://twitter.pbwiki.com/API-Docs and supported by
  ''' the Twitter developers, this will be used in the future (hopefully near) to set more information
  ''' in Twitter about the client posting the information as well as future usage in a clients directory.		
  ''' </summary>
  Public Property TwitterClientUrl() As String
   Get
    Return m_twitterClientUrl
   End Get
   Set(ByVal value As String)
    m_twitterClientUrl = value
   End Set
  End Property
#End Region

#Region " Protected Methods "
  Protected Const TwitterBaseUrlFormat As String = "http://twitter.com/{0}/{1}.{2}"

  Protected Function GetObjectTypeString(ByVal objectType As ObjectType) As String
   Return objectType.ToString().ToLower()
  End Function

  Protected Function GetActionTypeString(ByVal actionType As ActionType) As String
   Return actionType.ToString().ToLower()
  End Function

  Protected Function GetFormatTypeString(ByVal format As OutputFormatType) As String
   Return format.ToString().ToLower()
  End Function

  ''' <summary>
  ''' Executes an HTTP GET command and retrives the information.		
  ''' </summary>
  ''' <param name="url">The URL to perform the GET operation</param>
  ''' <param name="userName">The username to use with the request</param>
  ''' <param name="password">The password to use with the request</param>
  ''' <returns>The response of the request, or null if we got 404 or nothing.</returns>
  Protected Function ExecuteGetCommand(ByVal url As String, ByVal userName As String, ByVal password As String) As String
   Using client As New WebClient()
    If Not String.IsNullOrEmpty(userName) AndAlso Not String.IsNullOrEmpty(password) Then
     client.Credentials = New NetworkCredential(userName, password)
    End If

    Try
     Using stream As Stream = client.OpenRead(url)
      Using reader As New StreamReader(stream)
       Return reader.ReadToEnd()
      End Using
     End Using
    Catch ex As WebException
     '
     ' Handle HTTP 404 errors gracefully and return a null string to indicate there is no content.
     '
     If TypeOf ex.Response Is HttpWebResponse Then
      If TryCast(ex.Response, HttpWebResponse).StatusCode = HttpStatusCode.NotFound Then
       Return Nothing
      End If
     End If

     Throw ex
    End Try
   End Using

   Return Nothing
  End Function

  ''' <summary>
  ''' Executes an HTTP POST command and retrives the information.		
  ''' This function will automatically include a "source" parameter if the "Source" property is set.
  ''' </summary>
  ''' <param name="url">The URL to perform the POST operation</param>
  ''' <param name="userName">The username to use with the request</param>
  ''' <param name="password">The password to use with the request</param>
  ''' <param name="data">The data to post</param> 
  ''' <returns>The response of the request, or null if we got 404 or nothing.</returns>
  Protected Function ExecutePostCommand(ByVal url As String, ByVal userName As String, ByVal password As String, ByVal data As String) As String
   Dim request As WebRequest = WebRequest.Create(url)
   If Not String.IsNullOrEmpty(userName) AndAlso Not String.IsNullOrEmpty(password) Then
    request.Credentials = New NetworkCredential(userName, password)
    request.ContentType = "application/x-www-form-urlencoded"
    request.Method = "POST"

    If Not String.IsNullOrEmpty(TwitterClient) Then
     request.Headers.Add("X-Twitter-Client", TwitterClient)
    End If

    If Not String.IsNullOrEmpty(TwitterClientVersion) Then
     request.Headers.Add("X-Twitter-Version", TwitterClientVersion)
    End If

    If Not String.IsNullOrEmpty(TwitterClientUrl) Then
     request.Headers.Add("X-Twitter-URL", TwitterClientUrl)
    End If


    If Not String.IsNullOrEmpty(Source) Then
     data += "&source=" & HttpUtility.UrlEncode(Source)
    End If

    Dim bytes As Byte() = Encoding.UTF8.GetBytes(data)

    System.Net.ServicePointManager.Expect100Continue = False

    request.ContentLength = bytes.Length
    Using requestStream As Stream = request.GetRequestStream()
     requestStream.Write(bytes, 0, bytes.Length)

     Using response As WebResponse = request.GetResponse()
      Using reader As New StreamReader(response.GetResponseStream())
       Return reader.ReadToEnd()
      End Using
     End Using
    End Using
   End If

   Return Nothing
  End Function
#End Region

#Region " Public Timeline "

  Public Function GetPublicTimeline(ByVal format As OutputFormatType) As String
   Dim url As String = String.Format(TwitterBaseUrlFormat, GetObjectTypeString(ObjectType.Statuses), GetActionTypeString(ActionType.Public_Timeline), GetFormatTypeString(format))
   Return ExecuteGetCommand(url, Nothing, Nothing)
  End Function

  Public Function GetPublicTimelineAsJSON() As String
   Return GetPublicTimeline(OutputFormatType.JSON)
  End Function

  Public Function GetPublicTimelineAsXML(ByVal format As OutputFormatType) As XmlDocument
   If format = OutputFormatType.JSON Then
    Throw New ArgumentException("GetPublicTimelineAsXml supports only XML based formats (XML, RSS, Atom)", "format")
   End If

   Dim output As String = GetPublicTimeline(format)
   If Not String.IsNullOrEmpty(output) Then
    Dim xmlDocument As New XmlDocument()
    xmlDocument.LoadXml(output)

    Return xmlDocument
   End If

   Return Nothing
  End Function

  Public Function GetPublicTimelineAsXML() As XmlDocument
   Return GetPublicTimelineAsXML(OutputFormatType.XML)
  End Function

  Public Function GetPublicTimelineAsRSS() As XmlDocument
   Return GetPublicTimelineAsXML(OutputFormatType.RSS)
  End Function

  Public Function GetPublicTimelineAsAtom() As XmlDocument
   Return GetPublicTimelineAsXML(OutputFormatType.Atom)
  End Function

#End Region

#Region " User Timeline "

  Public Function GetUserTimeline(ByVal userName As String, ByVal password As String, ByVal IDorScreenName As String, ByVal format As OutputFormatType) As String
   Dim url As String = Nothing
   If String.IsNullOrEmpty(IDorScreenName) Then
    url = String.Format(TwitterBaseUrlFormat, GetObjectTypeString(ObjectType.Statuses), GetActionTypeString(ActionType.User_Timeline), GetFormatTypeString(format))
   Else
    url = String.Format(TwitterBaseUrlFormat, GetObjectTypeString(ObjectType.Statuses), GetActionTypeString(ActionType.User_Timeline) & "/" & IDorScreenName, GetFormatTypeString(format))
   End If

   Return ExecuteGetCommand(url, userName, password)
  End Function

  Public Function GetUserTimeline(ByVal userName As String, ByVal password As String, ByVal format As OutputFormatType) As String
   Return GetUserTimeline(userName, password, Nothing, format)
  End Function

  Public Function GetUserTimelineAsJSON(ByVal userName As String, ByVal password As String) As String
   Return GetUserTimeline(userName, password, OutputFormatType.JSON)
  End Function

  Public Function GetUserTimelineAsJSON(ByVal userName As String, ByVal password As String, ByVal IDorScreenName As String) As String
   Return GetUserTimeline(userName, password, IDorScreenName, OutputFormatType.JSON)
  End Function

  Public Function GetUserTimelineAsXML(ByVal userName As String, ByVal password As String, ByVal IDorScreenName As String, ByVal format As OutputFormatType) As XmlDocument
   If format = OutputFormatType.JSON Then
    Throw New ArgumentException("GetUserTimelineAsXML supports only XML based formats (XML, RSS, Atom)", "format")
   End If

   Dim output As String = GetUserTimeline(userName, password, IDorScreenName, format)
   If Not String.IsNullOrEmpty(output) Then
    Dim xmlDocument As New XmlDocument()
    xmlDocument.LoadXml(output)

    Return xmlDocument
   End If

   Return Nothing
  End Function

  Public Function GetUserTimelineAsXML(ByVal userName As String, ByVal password As String, ByVal format As OutputFormatType) As XmlDocument
   Return GetUserTimelineAsXML(userName, password, Nothing, format)
  End Function

  Public Function GetUserTimelineAsXML(ByVal userName As String, ByVal password As String, ByVal IDorScreenName As String) As XmlDocument
   Return GetUserTimelineAsXML(userName, password, IDorScreenName, OutputFormatType.XML)
  End Function

  Public Function GetUserTimelineAsXML(ByVal userName As String, ByVal password As String) As XmlDocument
   Return GetUserTimelineAsXML(userName, password, Nothing, OutputFormatType.RSS)
  End Function

  Public Function GetUserTimelineAsRSS(ByVal userName As String, ByVal password As String, ByVal IDorScreenName As String) As XmlDocument
   Return GetUserTimelineAsXML(userName, password, IDorScreenName, OutputFormatType.RSS)
  End Function

  Public Function GetUserTimelineAsRSS(ByVal userName As String, ByVal password As String) As XmlDocument
   Return GetUserTimelineAsXML(userName, password, OutputFormatType.RSS)
  End Function

  Public Function GetUserTimelineAsAtom(ByVal userName As String, ByVal password As String, ByVal IDorScreenName As String) As XmlDocument
   Return GetUserTimelineAsXML(userName, password, IDorScreenName, OutputFormatType.Atom)
  End Function

  Public Function GetUserTimelineAsAtom(ByVal userName As String, ByVal password As String) As XmlDocument
   Return GetUserTimelineAsXML(userName, password, OutputFormatType.Atom)
  End Function
#End Region

#Region " Friends Timeline "
  Public Function GetFriendsTimeline(ByVal userName As String, ByVal password As String, ByVal format As OutputFormatType) As String
   Dim url As String = String.Format(TwitterBaseUrlFormat, GetObjectTypeString(ObjectType.Statuses), GetActionTypeString(ActionType.Friends_Timeline), GetFormatTypeString(format))

   Return ExecuteGetCommand(url, userName, password)
  End Function

  Public Function GetFriendsTimelineAsJSON(ByVal userName As String, ByVal password As String) As String
   Return GetFriendsTimeline(userName, password, OutputFormatType.JSON)
  End Function

  Public Function GetFriendsTimelineAsXML(ByVal userName As String, ByVal password As String, ByVal format As OutputFormatType) As XmlDocument
   If format = OutputFormatType.JSON Then
    Throw New ArgumentException("GetFriendsTimelineAsXML supports only XML based formats (XML, RSS, Atom)", "format")
   End If

   Dim output As String = GetFriendsTimeline(userName, password, format)
   If Not String.IsNullOrEmpty(output) Then
    Dim xmlDocument As New XmlDocument()
    xmlDocument.LoadXml(output)

    Return xmlDocument
   End If

   Return Nothing
  End Function

  Public Function GetFriendsTimelineAsXML(ByVal userName As String, ByVal password As String) As XmlDocument
   Return GetFriendsTimelineAsXML(userName, password, OutputFormatType.XML)
  End Function

  Public Function GetFriendsTimelineAsRSS(ByVal userName As String, ByVal password As String) As XmlDocument
   Return GetFriendsTimelineAsXML(userName, password, OutputFormatType.RSS)
  End Function

  Public Function GetFriendsTimelineAsAtom(ByVal userName As String, ByVal password As String) As XmlDocument
   Return GetFriendsTimelineAsXML(userName, password, OutputFormatType.Atom)
  End Function

#End Region

#Region " Friends "

  Public Function GetFriends(ByVal userName As String, ByVal password As String, ByVal format As OutputFormatType) As String
   If format <> OutputFormatType.JSON AndAlso format <> OutputFormatType.XML Then
    Throw New ArgumentException("GetFriends support only XML and JSON output format", "format")
   End If

   Dim url As String = String.Format(TwitterBaseUrlFormat, GetObjectTypeString(ObjectType.Statuses), GetActionTypeString(ActionType.Friends), GetFormatTypeString(format))
   Return ExecuteGetCommand(url, userName, password)
  End Function

  Public Function GetFriends(ByVal userName As String, ByVal password As String, ByVal IDorScreenName As String, ByVal format As OutputFormatType) As String
   If format <> OutputFormatType.JSON AndAlso format <> OutputFormatType.XML Then
    Throw New ArgumentException("GetFriends support only XML and JSON output format", "format")
   End If

   Dim url As String = Nothing
   If String.IsNullOrEmpty(IDorScreenName) Then
    url = String.Format(TwitterBaseUrlFormat, GetObjectTypeString(ObjectType.Statuses), GetActionTypeString(ActionType.Friends), GetFormatTypeString(format))
   Else
    url = String.Format(TwitterBaseUrlFormat, GetObjectTypeString(ObjectType.Statuses), GetActionTypeString(ActionType.Friends) & "/" & IDorScreenName, GetFormatTypeString(format))
   End If

   Return ExecuteGetCommand(url, userName, password)
  End Function

  Public Function GetFriendsAsJSON(ByVal userName As String, ByVal password As String, ByVal IDorScreenName As String) As String
   Return GetFriends(userName, password, IDorScreenName, OutputFormatType.JSON)
  End Function

  Public Function GetFriendsAsJSON(ByVal userName As String, ByVal password As String) As String
   Return GetFriendsAsJSON(userName, password, Nothing)
  End Function

  Public Function GetFriendsAsXML(ByVal userName As String, ByVal password As String, ByVal IDorScreenName As String) As XmlDocument
   Dim output As String = GetFriends(userName, password, IDorScreenName, OutputFormatType.XML)
   If Not String.IsNullOrEmpty(output) Then
    Dim xmlDocument As New XmlDocument()
    xmlDocument.LoadXml(output)

    Return xmlDocument
   End If

   Return Nothing
  End Function

  Public Function GetFriendsAsXML(ByVal userName As String, ByVal password As String) As XmlDocument
   Return GetFriendsAsXML(userName, password, Nothing)
  End Function

#End Region

#Region " Followers "

  Public Function GetFollowers(ByVal userName As String, ByVal password As String, ByVal format As OutputFormatType) As String
   If format <> OutputFormatType.JSON AndAlso format <> OutputFormatType.XML Then
    Throw New ArgumentException("GetFollowers supports only XML and JSON output format", "format")
   End If

   Dim url As String = String.Format(TwitterBaseUrlFormat, GetObjectTypeString(ObjectType.Statuses), GetActionTypeString(ActionType.Followers), GetFormatTypeString(format))
   Return ExecuteGetCommand(url, userName, password)
  End Function

  Public Function GetFollowersAsJSON(ByVal userName As String, ByVal password As String) As String
   Return GetFollowers(userName, password, OutputFormatType.JSON)
  End Function

  Public Function GetFollowersAsXML(ByVal userName As String, ByVal password As String) As XmlDocument
   Dim output As String = GetFollowers(userName, password, OutputFormatType.XML)
   If Not String.IsNullOrEmpty(output) Then
    Dim xmlDocument As New XmlDocument()
    xmlDocument.LoadXml(output)

    Return xmlDocument
   End If

   Return Nothing
  End Function

#End Region

#Region " Update "

  Public Function Update(ByVal userName As String, ByVal password As String, ByVal status As String, ByVal format As OutputFormatType) As String
   If format <> OutputFormatType.JSON AndAlso format <> OutputFormatType.XML Then
    Throw New ArgumentException("Update support only XML and JSON output format", "format")
   End If

   Dim url As String = String.Format(TwitterBaseUrlFormat, GetObjectTypeString(ObjectType.Statuses), GetActionTypeString(ActionType.Update), GetFormatTypeString(format))
   Dim data As String = String.Format("status={0}", HttpUtility.UrlEncode(status))

   Return ExecutePostCommand(url, userName, password, data)
  End Function

  Public Function UpdateAsJSON(ByVal userName As String, ByVal password As String, ByVal text As String) As String
   Return Update(userName, password, text, OutputFormatType.JSON)
  End Function

  Public Function UpdateAsXML(ByVal userName As String, ByVal password As String, ByVal text As String) As XmlDocument
   Dim output As String = Update(userName, password, text, OutputFormatType.XML)
   If Not String.IsNullOrEmpty(output) Then
    Dim xmlDocument As New XmlDocument()
    xmlDocument.LoadXml(output)

    Return xmlDocument
   End If

   Return Nothing
  End Function

#End Region

#Region " Featured "

  Public Function GetFeatured(ByVal userName As String, ByVal password As String, ByVal format As OutputFormatType) As String
   If format <> OutputFormatType.JSON AndAlso format <> OutputFormatType.XML Then
    Throw New ArgumentException("GetFeatured supports only XML and JSON output format", "format")
   End If

   Dim url As String = String.Format(TwitterBaseUrlFormat, GetObjectTypeString(ObjectType.Statuses), GetActionTypeString(ActionType.Featured), GetFormatTypeString(format))
   Return ExecuteGetCommand(url, userName, password)
  End Function

  Public Function GetFeaturedAsJSON(ByVal userName As String, ByVal password As String) As String
   Return GetFeatured(userName, password, OutputFormatType.JSON)
  End Function

  Public Function GetFeaturedAsXML(ByVal userName As String, ByVal password As String) As XmlDocument
   Dim output As String = GetFeatured(userName, password, OutputFormatType.XML)
   If Not String.IsNullOrEmpty(output) Then
    Dim xmlDocument As New XmlDocument()
    xmlDocument.LoadXml(output)

    Return xmlDocument
   End If

   Return Nothing
  End Function

#End Region

#Region " Show "

  Public Function Show(ByVal userName As String, ByVal password As String, ByVal IDorScreenName As String, ByVal format As OutputFormatType) As String
   If format <> OutputFormatType.JSON AndAlso format <> OutputFormatType.XML Then
    Throw New ArgumentException("Show supports only XML and JSON output format", "format")
   End If

   Dim url As String = String.Format(TwitterBaseUrlFormat, GetObjectTypeString(ObjectType.Users), GetActionTypeString(ActionType.Show) & "/" & IDorScreenName, GetFormatTypeString(format))
   Return ExecuteGetCommand(url, userName, password)
  End Function

  Public Function ShowAsJSON(ByVal userName As String, ByVal password As String, ByVal IDorScreenName As String) As String
   Return Show(userName, password, IDorScreenName, OutputFormatType.JSON)
  End Function

  Public Function ShowAsXML(ByVal userName As String, ByVal password As String, ByVal IDorScreenName As String) As XmlDocument
   Dim output As String = Show(userName, password, IDorScreenName, OutputFormatType.XML)
   If Not String.IsNullOrEmpty(output) Then
    Dim xmlDocument As New XmlDocument()
    xmlDocument.LoadXml(output)

    Return xmlDocument
   End If

   Return Nothing
  End Function

#End Region

 End Class
End Namespace
