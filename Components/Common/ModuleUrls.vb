Imports DotNetNuke.Services.Tokens

Namespace Common
 Public Class ModuleUrls
  Implements IPropertyAccess

#Region " Properties "
  Public Property TabId As Integer = -1
  Public Property BlogId As Integer = -1
  Public Property ContentItemId As Integer = -1
  Public Property TermId As Integer = -1
  Public Property AuthorId As Integer = -1
  Private Property Cache As New Dictionary(Of String, String)
#End Region

#Region " Constructors "
  Public Sub New(tabId As Integer, blogId As Integer, contentItemId As Integer, termId As Integer, authorId As Integer)
   Me.TabId = tabId
   Me.BlogId = blogId
   Me.ContentItemId = contentItemId
   Me.TermId = termId
   Me.AuthorId = authorId
  End Sub
#End Region

#Region " Public Methods "
  Public Function GetUrl(includeBlog As Boolean, includePost As Boolean, includeTerm As Boolean, includeAuthor As Boolean, includeEnding As Boolean) As String
   Dim urlType As String = ""
   If includeBlog Then urlType = "blog"
   If includePost Then urlType &= "post"
   If includeTerm Then urlType &= "term"
   If includeAuthor Then urlType &= "author"
   Dim cacheKey As String = String.Format("{0}:{1}", urlType, includeEnding.ToString.ToLower)
   If Cache.ContainsKey(cacheKey) Then Return Cache(cacheKey)
   BuildUrl(urlType)
   Return Cache(cacheKey)
  End Function
#End Region

#Region " Private Methods "
  Private Sub BuildUrl(urlType As String)
   Dim params As New List(Of String)
   Select Case urlType
    Case "blog"
     If BlogId > -1 Then params.Add("Blog=" & BlogId.ToString)
    Case "post"
     If ContentItemId > -1 Then params.Add("Post=" & ContentItemId.ToString)
    Case "term"
     If TermId > -1 Then params.Add("Term=" & TermId.ToString)
    Case "author"
     If AuthorId > -1 Then params.Add("Author=" & AuthorId.ToString)

    Case "blogpost"
     If BlogId > -1 Then params.Add("Blog=" & BlogId.ToString)
     If ContentItemId > -1 Then params.Add("Post=" & ContentItemId.ToString)
    Case "blogterm"
     If BlogId > -1 Then params.Add("Blog=" & BlogId.ToString)
     If TermId > -1 Then params.Add("Term=" & TermId.ToString)
    Case "blogauthor"
     If BlogId > -1 Then params.Add("Blog=" & BlogId.ToString)
     If AuthorId > -1 Then params.Add("Author=" & AuthorId.ToString)
    Case "postterm"
     If ContentItemId > -1 Then params.Add("Post=" & ContentItemId.ToString)
     If TermId > -1 Then params.Add("Term=" & TermId.ToString)
    Case "postauthor"
     If ContentItemId > -1 Then params.Add("Post=" & ContentItemId.ToString)
     If AuthorId > -1 Then params.Add("Author=" & AuthorId.ToString)
    Case "termauthor"
     If TermId > -1 Then params.Add("Term=" & TermId.ToString)
     If AuthorId > -1 Then params.Add("Author=" & AuthorId.ToString)

    Case "blogpostterm"
     If BlogId > -1 Then params.Add("Blog=" & BlogId.ToString)
     If ContentItemId > -1 Then params.Add("Post=" & ContentItemId.ToString)
     If TermId > -1 Then params.Add("Term=" & TermId.ToString)
    Case "blogpostauthor"
     If BlogId > -1 Then params.Add("Blog=" & BlogId.ToString)
     If ContentItemId > -1 Then params.Add("Post=" & ContentItemId.ToString)
     If AuthorId > -1 Then params.Add("Author=" & AuthorId.ToString)
    Case "posttermauthor"
     If ContentItemId > -1 Then params.Add("Post=" & ContentItemId.ToString)
     If TermId > -1 Then params.Add("Term=" & TermId.ToString)
     If AuthorId > -1 Then params.Add("Author=" & AuthorId.ToString)

    Case "blogposttermauthor", "all"
     If BlogId > -1 Then params.Add("Blog=" & BlogId.ToString)
     If ContentItemId > -1 Then params.Add("Post=" & ContentItemId.ToString)
     If TermId > -1 Then params.Add("Term=" & TermId.ToString)
     If AuthorId > -1 Then params.Add("Author=" & AuthorId.ToString)
   End Select
   Dim BaseUrl As String = DotNetNuke.Common.NavigateURL(TabId, "", params.ToArray)
   Dim BaseUrlPlusEnding As String
   If BaseUrl.Contains("?") Then
    BaseUrlPlusEnding = BaseUrl & "&"
   Else
    BaseUrlPlusEnding = BaseUrl & "?"
   End If
   Cache.Add(String.Format("{0}:false", urlType), BaseUrl)
   Cache.Add(String.Format("{0}:true", urlType), BaseUrlPlusEnding)
  End Sub
#End Region

#Region " IPropertyAccess Implementation "
  Public Function GetProperty(strPropertyName As String, strFormat As String, formatProvider As System.Globalization.CultureInfo, AccessingUser As DotNetNuke.Entities.Users.UserInfo, AccessLevel As DotNetNuke.Services.Tokens.Scope, ByRef PropertyNotFound As Boolean) As String Implements DotNetNuke.Services.Tokens.IPropertyAccess.GetProperty
   Dim OutputFormat As String = String.Empty
   Dim portalSettings As DotNetNuke.Entities.Portals.PortalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings()
   If strFormat = String.Empty Then
    OutputFormat = "D"
   Else
    OutputFormat = strFormat
   End If
   strFormat = strFormat.ToLower
   If strFormat <> "true" AndAlso strFormat <> "false" Then strFormat = "false"
   strPropertyName = strPropertyName.ToLower
   Dim cacheKey As String = String.Format("{0}:{1}", strPropertyName, strFormat)
   If Cache.ContainsKey(cacheKey) Then Return Cache(cacheKey)
   BuildUrl(strPropertyName)
   Return Cache(cacheKey)
  End Function

  Public ReadOnly Property Cacheability() As DotNetNuke.Services.Tokens.CacheLevel Implements DotNetNuke.Services.Tokens.IPropertyAccess.Cacheability
   Get
    Return CacheLevel.fullyCacheable
   End Get
  End Property
#End Region

 End Class
End Namespace
