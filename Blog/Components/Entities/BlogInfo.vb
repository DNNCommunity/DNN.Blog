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
Imports System.Data
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Tokens
Imports DotNetNuke.Entities.Users

Namespace Business

    <Serializable(), XmlRoot("Blog")> _
    Public Class BlogInfo
        Implements IHydratable
        Implements IPropertyAccess
        Implements IXmlSerializable

#Region "Private Members"

        Private _User As UserInfo
        Private _userName As String
        Private _userFullName As String
        Private _EnableGhostWriter As Boolean = False

#End Region

#Region "Constructors"

        Public Sub New()
        End Sub

#End Region

#Region "Public Properties"

        Public Property PortalID() As Integer
        Public Property BlogID() As Integer
        Public Property ParentBlogID() As Integer
        Public Property UserID() As Integer
        Public Property Title() As String
        Public Property Description() As String
        Public Property [Public]() As Boolean
        Public Property AllowComments() As Boolean
        Public Property AllowAnonymous() As Boolean
        Public Property LastEntry() As DateTime
        Public Property Created() As DateTime
        Public Property ShowFullName() As Boolean
        Public Property ChildBlogCount() As Integer = 0
        Public Property Syndicated() As Boolean
        Public Property SyndicateIndependant() As Boolean
        Public Property SyndicationURL() As String
        Public Property SyndicationEmail() As String
        Public Property EmailNotification() As Boolean
        Public Property AllowTrackbacks() As Boolean
        Public Property AutoTrackback() As Boolean
        Public Property MustApproveComments() As Boolean
        Public Property MustApproveAnonymous() As Boolean
        Public Property MustApproveTrackbacks() As Boolean
        Public Property UseCaptcha() As Boolean
        Public Property BlogPostCount() As Integer

        Public Property UserName() As String
            Get
                If _userName <> String.Empty Then
                    Return _userName
                Else
                    Dim objUser As UserInfo = DotNetNuke.Entities.Users.UserController.GetUserById(PortalID, UserID)

                    If objUser IsNot Nothing Then
                        Return objUser.Username
                    Else
                        Return "Anonymous"
                    End If
                End If
            End Get
            Set(ByVal Value As String)
                _userName = Value
            End Set
        End Property

        Public Property UserFullName() As String
            Get
                If _userFullName <> String.Empty Then
                    Return _userFullName
                Else
                    Dim objUser As UserInfo = DotNetNuke.Entities.Users.UserController.GetUserById(PortalID, UserID)

                    If objUser IsNot Nothing Then
                        Return objUser.DisplayName
                    Else
                        Return "Anonymous User"
                    End If
                End If
            End Get
            Set(ByVal Value As String)
                _userFullName = Value
            End Set
        End Property

        'Public Property DateFormat() As String
        '    Get
        '        If _DateFormat Is Nothing Then
        '            Return "g"
        '        Else
        '            If _DateFormat.Length = 0 Then
        '                Return "g"
        '            Else
        '                Return _DateFormat
        '            End If
        '        End If
        '    End Get
        '    Set(ByVal Value As String)
        '        _DateFormat = Value
        '    End Set
        'End Property

        Public Property User() As UserInfo
            Get
                If _User Is Nothing Then
                    Try
                        _User = (New UserController).GetUser(_PortalID, _UserID)
                    Catch ex As Exception
                    End Try
                End If
                Return _User
            End Get
            Set(ByVal value As UserInfo)
                _User = value
            End Set
        End Property

        ''' <summary>
        ''' Determines if the blog permits ghost writing.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property EnableGhostWriter() As Boolean
            Get
                Return _EnableGhostWriter
            End Get
            Set(ByVal Value As Boolean)
                _EnableGhostWriter = Value
            End Set
        End Property

#End Region

#Region "IHydratable Implementation"

        ''' <summary>
        ''' Fill hydrates the object from a Datareader
        ''' </summary>
        ''' <remarks>The Fill method is used by the CBO method to hydrtae the object
        ''' rather than using the more expensive Refection  methods.</remarks>
        ''' <history>
        ''' 	[pdonker]	11/07/2010  Created
        ''' </history>
        Public Sub Fill(ByVal dr As IDataReader) Implements IHydratable.Fill
            AllowAnonymous = Convert.ToBoolean(Null.SetNull(dr.Item("AllowAnonymous"), AllowAnonymous))
            AllowComments = Convert.ToBoolean(Null.SetNull(dr.Item("AllowComments"), AllowComments))
            AllowTrackbacks = Convert.ToBoolean(Null.SetNull(dr.Item("AllowTrackbacks"), AllowTrackbacks))
            AutoTrackback = Convert.ToBoolean(Null.SetNull(dr.Item("AutoTrackback"), AutoTrackback))
            BlogID = Convert.ToInt32(Null.SetNull(dr.Item("BlogID"), BlogID))
            Created = Convert.ToDateTime(Null.SetNull(dr.Item("Created"), Created))
            Description = Convert.ToString(Null.SetNull(dr.Item("Description"), Description))
            EmailNotification = Convert.ToBoolean(Null.SetNull(dr.Item("EmailNotification"), EmailNotification))
            LastEntry = Convert.ToDateTime(Null.SetNull(dr.Item("LastEntry"), LastEntry))
            MustApproveAnonymous = Convert.ToBoolean(Null.SetNull(dr.Item("MustApproveAnonymous"), MustApproveAnonymous))
            MustApproveComments = Convert.ToBoolean(Null.SetNull(dr.Item("MustApproveComments"), MustApproveComments))
            MustApproveTrackbacks = Convert.ToBoolean(Null.SetNull(dr.Item("MustApproveTrackbacks"), MustApproveTrackbacks))
            ParentBlogID = Convert.ToInt32(Null.SetNull(dr.Item("ParentBlogID"), ParentBlogID))
            PortalID = Convert.ToInt32(Null.SetNull(dr.Item("PortalID"), PortalID))
            [Public] = Convert.ToBoolean(Null.SetNull(dr.Item("Public"), [Public]))
            ShowFullName = Convert.ToBoolean(Null.SetNull(dr.Item("ShowFullName"), ShowFullName))
            Syndicated = Convert.ToBoolean(Null.SetNull(dr.Item("Syndicated"), Syndicated))
            SyndicateIndependant = Convert.ToBoolean(Null.SetNull(dr.Item("SyndicateIndependant"), SyndicateIndependant))
            SyndicationEmail = Convert.ToString(Null.SetNull(dr.Item("SyndicationEmail"), SyndicationEmail))
            SyndicationURL = Convert.ToString(Null.SetNull(dr.Item("SyndicationURL"), SyndicationURL))
            Title = Convert.ToString(Null.SetNull(dr.Item("Title"), Title))
            UseCaptcha = Convert.ToBoolean(Null.SetNull(dr.Item("UseCaptcha"), UseCaptcha))
            BlogPostCount = Convert.ToInt32(Null.SetNull(dr.Item("BlogPostCount"), UserID))
            UserID = Convert.ToInt32(Null.SetNull(dr.Item("UserID"), UserID))
            EnableGhostWriter = Convert.ToBoolean(Null.SetNull(dr.Item("EnableGhostWriter"), EnableGhostWriter))
        End Sub

        ''' <summary>
        ''' Gets and sets the Key ID
        ''' </summary>
        ''' <remarks>The KeyID property is part of the IHydratble interface.  It is used
        ''' as the key property when creating a Dictionary</remarks>
        ''' <history>
        ''' 	[pdonker]	11/07/2010  Created
        ''' </history>
        Public Property KeyID() As Integer Implements IHydratable.KeyID
            Get
                Return BlogID
            End Get
            Set(ByVal value As Integer)
                BlogID = value
            End Set
        End Property

#End Region

#Region "IPropertyAccess Implementation"

        Public Function GetProperty(ByVal strPropertyName As String, ByVal strFormat As String, ByVal formatProvider As System.Globalization.CultureInfo, ByVal AccessingUser As DotNetNuke.Entities.Users.UserInfo, ByVal AccessLevel As DotNetNuke.Services.Tokens.Scope, ByRef PropertyNotFound As Boolean) As String Implements DotNetNuke.Services.Tokens.IPropertyAccess.GetProperty
            Dim OutputFormat As String = String.Empty
            Dim portalSettings As DotNetNuke.Entities.Portals.PortalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings()
            If strFormat = String.Empty Then
                OutputFormat = "D"
            Else
                OutputFormat = strFormat
            End If
            Select Case strPropertyName.ToLower
                Case "allowanonymous"
                    Return PropertyAccess.Boolean2LocalizedYesNo(Me.AllowAnonymous, formatProvider)
                Case "allowcomments"
                    Return PropertyAccess.Boolean2LocalizedYesNo(Me.AllowComments, formatProvider)
                Case "allowtrackbacks"
                    Return PropertyAccess.Boolean2LocalizedYesNo(Me.AllowTrackbacks, formatProvider)
                Case "autotrackback"
                    Return PropertyAccess.Boolean2LocalizedYesNo(Me.AutoTrackback, formatProvider)
                Case "blogid"
                    Return (Me.BlogID.ToString(OutputFormat, formatProvider))
                Case "created"
                    Return (Me.Created.ToString(OutputFormat, formatProvider))
                Case "description"
                    Return PropertyAccess.FormatString(Me.Description, strFormat)
                Case "emailnotification"
                    Return PropertyAccess.Boolean2LocalizedYesNo(Me.EmailNotification, formatProvider)
                Case "lastentry"
                    Return (Me.LastEntry.ToString(OutputFormat, formatProvider))
                Case "mustapproveanonymous"
                    Return PropertyAccess.Boolean2LocalizedYesNo(Me.MustApproveAnonymous, formatProvider)
                Case "mustapprovecomments"
                    Return PropertyAccess.Boolean2LocalizedYesNo(Me.MustApproveComments, formatProvider)
                Case "mustapprovetrackbacks"
                    Return PropertyAccess.Boolean2LocalizedYesNo(Me.MustApproveTrackbacks, formatProvider)
                Case "parentblogid"
                    Return (Me.ParentBlogID.ToString(OutputFormat, formatProvider))
                Case "portalid"
                    Return (Me.PortalID.ToString(OutputFormat, formatProvider))
                Case "public"
                    Return PropertyAccess.Boolean2LocalizedYesNo(Me.Public, formatProvider)
                Case "showfullname"
                    Return PropertyAccess.Boolean2LocalizedYesNo(Me.ShowFullName, formatProvider)
                Case "syndicated"
                    Return PropertyAccess.Boolean2LocalizedYesNo(Me.Syndicated, formatProvider)
                Case "syndicateindependant"
                    Return PropertyAccess.Boolean2LocalizedYesNo(Me.SyndicateIndependant, formatProvider)
                Case "syndicationemail"
                    Return PropertyAccess.FormatString(Me.SyndicationEmail, strFormat)
                Case "syndicationurl"
                    Return PropertyAccess.FormatString(Me.SyndicationURL, strFormat)
                Case "title"
                    Return PropertyAccess.FormatString(Me.Title, strFormat)
                Case "usecaptcha"
                    Return PropertyAccess.Boolean2LocalizedYesNo(Me.UseCaptcha, formatProvider)
                Case "userid"
                    Return (Me.UserID.ToString(OutputFormat, formatProvider))
                Case "enableghostwriter"
                    Return PropertyAccess.Boolean2LocalizedYesNo(Me.EnableGhostWriter, formatProvider)
                Case Else
                    PropertyNotFound = True
            End Select

            Return Null.NullString
        End Function

        Public ReadOnly Property Cacheability() As DotNetNuke.Services.Tokens.CacheLevel Implements DotNetNuke.Services.Tokens.IPropertyAccess.Cacheability
            Get
                Return CacheLevel.fullyCacheable
            End Get
        End Property
#End Region

#Region "IXmlSerializable Implementation"

        ''' <summary>
        ''' GetSchema returns the XmlSchema for this class
        ''' </summary>
        ''' <remarks>GetSchema is implemented as a stub method as it is not required</remarks>
        ''' <history>
        ''' 	[pdonker]	11/07/2010  Created
        ''' </history>
        Public Function GetSchema() As XmlSchema Implements IXmlSerializable.GetSchema
            Return Nothing
        End Function

        Private Function readElement(ByVal reader As XmlReader, ByVal ElementName As String) As String
            If (Not reader.NodeType = XmlNodeType.Element) OrElse reader.Name <> ElementName Then
                reader.ReadToFollowing(ElementName)
            End If
            If reader.NodeType = XmlNodeType.Element Then
                Return reader.ReadElementContentAsString
            Else
                Return ""
            End If
        End Function

        ''' <summary>
        ''' ReadXml fills the object (de-serializes it) from the XmlReader passed
        ''' </summary>
        ''' <remarks></remarks>
        ''' <param name="reader">The XmlReader that contains the xml for the object</param>
        ''' <history>
        ''' 	[pdonker]	11/07/2010  Created
        ''' </history>
        Public Sub ReadXml(ByVal reader As XmlReader) Implements IXmlSerializable.ReadXml
            Try

                Boolean.TryParse(readElement(reader, "AllowAnonymous"), AllowAnonymous)
                Boolean.TryParse(readElement(reader, "AllowComments"), AllowComments)
                Boolean.TryParse(readElement(reader, "AllowTrackbacks"), AllowTrackbacks)
                Boolean.TryParse(readElement(reader, "AutoTrackback"), AutoTrackback)
                If Not DateTime.TryParse(readElement(reader, "Created"), Created) Then
                    Created = DateTime.MinValue
                End If
                Description = readElement(reader, "Description")
                Boolean.TryParse(readElement(reader, "EmailNotification"), EmailNotification)
                If Not DateTime.TryParse(readElement(reader, "LastEntry"), LastEntry) Then
                    LastEntry = DateTime.MinValue
                End If
                Boolean.TryParse(readElement(reader, "MustApproveAnonymous"), MustApproveAnonymous)
                Boolean.TryParse(readElement(reader, "MustApproveComments"), MustApproveComments)
                Boolean.TryParse(readElement(reader, "MustApproveTrackbacks"), MustApproveTrackbacks)
                If Not Int32.TryParse(readElement(reader, "ParentBlogID"), ParentBlogID) Then
                    ParentBlogID = Null.NullInteger
                End If
                If Not Int32.TryParse(readElement(reader, "PortalID"), PortalID) Then
                    PortalID = Null.NullInteger
                End If
                Boolean.TryParse(readElement(reader, "Public"), [Public])
                Boolean.TryParse(readElement(reader, "ShowFullName"), ShowFullName)
                Boolean.TryParse(readElement(reader, "Syndicated"), Syndicated)
                Boolean.TryParse(readElement(reader, "SyndicateIndependant"), SyndicateIndependant)
                SyndicationEmail = readElement(reader, "SyndicationEmail")
                SyndicationURL = readElement(reader, "SyndicationURL")
                Title = readElement(reader, "Title")
                Boolean.TryParse(readElement(reader, "UseCaptcha"), UseCaptcha)
                If Not Int32.TryParse(readElement(reader, "UserID"), UserID) Then
                    UserID = Null.NullInteger
                End If
                Boolean.TryParse(readElement(reader, "EnableGhostWriter"), EnableGhostWriter)
            Catch ex As Exception
                ' log exception as DNN import routine does not do that
                DotNetNuke.Services.Exceptions.LogException(ex)
                ' re-raise exception to make sure import routine displays a visible error to the user
                Throw New Exception("An error occured during import of an Blog", ex)
            End Try

        End Sub

        ''' <summary>
        ''' WriteXml converts the object to Xml (serializes it) and writes it using the XmlWriter passed
        ''' </summary>
        ''' <remarks></remarks>
        ''' <param name="writer">The XmlWriter that contains the xml for the object</param>
        ''' <history>
        ''' 	[pdonker]	11/07/2010  Created
        ''' </history>
        Public Sub WriteXml(ByVal writer As XmlWriter) Implements IXmlSerializable.WriteXml
            writer.WriteStartElement("Blog")
            writer.WriteElementString("BlogID", BlogID.ToString())
            writer.WriteElementString("AllowAnonymous", AllowAnonymous.ToString())
            writer.WriteElementString("AllowComments", AllowComments.ToString())
            writer.WriteElementString("AllowTrackbacks", AllowTrackbacks.ToString())
            writer.WriteElementString("AutoTrackback", AutoTrackback.ToString())
            writer.WriteElementString("Created", Created.ToString())
            writer.WriteElementString("Description", Description)
            writer.WriteElementString("EmailNotification", EmailNotification.ToString())
            writer.WriteElementString("LastEntry", LastEntry.ToString())
            writer.WriteElementString("MustApproveAnonymous", MustApproveAnonymous.ToString())
            writer.WriteElementString("MustApproveComments", MustApproveComments.ToString())
            writer.WriteElementString("MustApproveTrackbacks", MustApproveTrackbacks.ToString())
            writer.WriteElementString("ParentBlogID", ParentBlogID.ToString())
            writer.WriteElementString("PortalID", PortalID.ToString())
            writer.WriteElementString("Public", [Public].ToString())
            writer.WriteElementString("ShowFullName", ShowFullName.ToString())
            writer.WriteElementString("Syndicated", Syndicated.ToString())
            writer.WriteElementString("SyndicateIndependant", SyndicateIndependant.ToString())
            writer.WriteElementString("SyndicationEmail", SyndicationEmail)
            writer.WriteElementString("SyndicationURL", SyndicationURL)
            writer.WriteElementString("Title", Title)
            writer.WriteElementString("UseCaptcha", UseCaptcha.ToString())
            writer.WriteElementString("UserID", UserID.ToString())
            writer.WriteElementString("EnableGhostWriter", EnableGhostWriter.ToString())
            writer.WriteEndElement()
        End Sub

#End Region

    End Class

End Namespace