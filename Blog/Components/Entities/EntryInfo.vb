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
Imports DotNetNuke.Modules.Blog.Components.Controllers
Imports DotNetNuke.Modules.Blog.Components.Business
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Tokens
Imports DotNetNuke.Entities.Users

Namespace Components.Entities

    <Serializable(), XmlRoot("Entry")> _
    Public Class EntryInfo
        Inherits DotNetNuke.Entities.Content.ContentItem
        Implements IHydratable
        Implements IPropertyAccess
        Implements IXmlSerializable

#Region "Constructors"

        Public Sub New()
        End Sub

        Public Sub New(ByVal UserID As Integer, ByVal Username As String, ByVal UserFullName As String, ByVal EntryID As Integer, ByVal BlogID As Integer, ByVal Title As String, ByVal Description As String, ByVal Entry As String, ByVal AddedDate As DateTime, ByVal Published As Boolean, ByVal AllowComments As Boolean, ByVal AllowAnonymous As Boolean, ByVal BlogSyndicated As Boolean, ByVal BlogPublic As Boolean, ByVal CategoryID As Integer, ByVal CategoryTitle As String, ByVal CategorySyndicated As Boolean, ByVal CategoryPublic As Boolean, ByVal CommentCount As Integer, ByVal TotalRecords As Integer)
            Me.UserID = UserID
            Me.UserName = Username
            Me.UserFullName = UserFullName
            Me.EntryID = EntryID
            Me.BlogID = BlogID
            Me.Title = Title
            Me.Description = Description
            Me.Entry = Entry
            Me.AddedDate = AddedDate
            Me.Published = Published
            Me.AllowComments = AllowComments
            Me.CommentCount = CommentCount
            Me.SyndicationEmail = SyndicationEmail
            Me.TotalRecords = TotalRecords
        End Sub

#End Region

#Region "Public Properties"

        Public Property EntryID() As Integer = -1
        Public Property BlogID() As Integer
        Public Property Title() As String
        Public Property Description() As String
        Public Property Entry() As String
        Public Property AddedDate() As DateTime
        Public Property UserID() As Integer
        Public Property UserName() As String
        Public Property UserFullName() As String
        Public Property Published() As Boolean
        Public Property CommentCount() As Integer
        Public Property AllowComments() As Boolean
        Public Property DisplayCopyright() As Boolean
        Public Property Copyright() As String
        Public Property PermaLink() As String
        Public Property SyndicationEmail() As String
        Public Property CreatedUserId() As Integer
        Public Property ViewCount() As Integer
        Public Property TotalRecords() As Integer

#End Region

#Region "Public Methods"

        Public Function EntryTerms(vocabularyId As Integer) As List(Of TermInfo)
            Dim cntTerms As New TermController()
            Return cntTerms.GetTermsByContentItem(ContentItemId, vocabularyId)
        End Function

#End Region

#Region "IHydratable Implementation"

        ''' <summary>
        ''' Fill hydrates the object from a Datareader
        ''' </summary>
        ''' <remarks>The Fill method is used by the CBO method to hydrate the object
        ''' rather than using the more expensive Refection  methods.</remarks>
        ''' <history>
        ''' 	[pdonker]	11/07/2010  Created
        ''' </history>
        Public Overrides Sub Fill(ByVal dr As IDataReader)
            'Call the base classes fill method to populate base class proeprties
            MyBase.FillInternal(dr)

            AddedDate = Convert.ToDateTime(Null.SetNull(dr.Item("AddedDate"), AddedDate))
            AllowComments = Convert.ToBoolean(Null.SetNull(dr.Item("AllowComments"), AllowComments))
            BlogID = Convert.ToInt32(Null.SetNull(dr.Item("BlogID"), BlogID))
            Copyright = Convert.ToString(Null.SetNull(dr.Item("Copyright"), Copyright))
            Description = Convert.ToString(Null.SetNull(dr.Item("Description"), Description))
            DisplayCopyright = Convert.ToBoolean(Null.SetNull(dr.Item("DisplayCopyright"), DisplayCopyright))
            Entry = Convert.ToString(Null.SetNull(dr.Item("Entry"), Entry))
            EntryID = Convert.ToInt32(Null.SetNull(dr.Item("EntryID"), EntryID))
            PermaLink = Convert.ToString(Null.SetNull(dr.Item("PermaLink"), PermaLink))
            Published = Convert.ToBoolean(Null.SetNull(dr.Item("Published"), Published))
            Title = Convert.ToString(Null.SetNull(dr.Item("Title"), Title))
            UserID = Convert.ToInt32(Null.SetNull(dr.Item("UserID"), UserID))
            UserName = Convert.ToString(Null.SetNull(dr.Item("UserName"), UserName))
            UserFullName = Convert.ToString(Null.SetNull(dr.Item("UserFullName"), UserFullName))
            CommentCount = Convert.ToInt32(Null.SetNull(dr.Item("CommentCount"), CommentCount))
            SyndicationEmail = Convert.ToString(Null.SetNull(dr.Item("SyndicationEmail"), SyndicationEmail))
            CreatedUserId = Convert.ToInt32(Null.SetNull(dr.Item("CreatedUserId"), CreatedUserId))
            ViewCount = Convert.ToInt32(Null.SetNull(dr.Item("ViewCount"), ViewCount))
            TotalRecords = Convert.ToInt32(Null.SetNull(dr.Item("TotalRecords"), CommentCount))
        End Sub

        ''' <summary>
        ''' Gets and sets the Key ID
        ''' </summary>
        ''' <remarks>The KeyID property is part of the IHydratble interface.  It is used
        ''' as the key property when creating a Dictionary</remarks>
        ''' <history>
        ''' 	[pdonker]	11/07/2010  Created
        ''' </history>
        Public Overrides Property KeyID() As Integer
            Get
                Return EntryID
            End Get
            Set(ByVal value As Integer)
                EntryID = value
            End Set
        End Property

#End Region

#Region "IPropertyAccess Methods"

        Public ReadOnly Property Cacheability() As CacheLevel Implements IPropertyAccess.Cacheability
            Get
                Return CacheLevel.fullyCacheable
            End Get
        End Property

        Public Function GetProperty(ByVal strPropertyName As String, ByVal strFormat As String, ByVal formatProvider As System.Globalization.CultureInfo, ByVal AccessingUser As UserInfo, ByVal AccessLevel As Scope, ByRef PropertyNotFound As Boolean) As String Implements IPropertyAccess.GetProperty
            Dim OutputFormat As String = String.Empty
            If strFormat = String.Empty Then
                OutputFormat = "D"
            Else
                OutputFormat = strFormat
            End If
            Select Case strPropertyName.ToLower
                Case "userid"
                    Return (Me.UserID.ToString(OutputFormat, formatProvider))
                Case "username"
                    Return PropertyAccess.FormatString(Me.UserName, strFormat)
                Case "userfullname"
                    Return PropertyAccess.FormatString(Me.UserFullName, strFormat)
                Case "entryid"
                    Return (Me.EntryID.ToString(OutputFormat, formatProvider))
                Case "blogid"
                    Return (Me.BlogID.ToString(OutputFormat, formatProvider))
                Case "title"
                    Return PropertyAccess.FormatString(Me.Title, strFormat)
                Case "description"
                    If String.IsNullOrEmpty(Me.Description) Then
                        Dim desc As String = Utility.removeAllHtmltags(HttpUtility.HtmlDecode(Me.Entry))
                        desc = Left(desc, 1024)
                        Return PropertyAccess.FormatString(desc, strFormat)
                    Else
                        Return PropertyAccess.FormatString(HttpUtility.HtmlDecode(Me.Description), strFormat)
                    End If
                Case "entry"
                    Return PropertyAccess.FormatString(HttpUtility.HtmlDecode(Me.Entry), strFormat)
                Case "addeddate"
                    Return (Me.AddedDate.ToString(OutputFormat, formatProvider))
                Case "published"
                    Return PropertyAccess.Boolean2LocalizedYesNo(Me.Published, formatProvider)
                Case "allowcomments"
                    Return PropertyAccess.Boolean2LocalizedYesNo(Me.AllowComments, formatProvider)
                Case "displaycopyright"
                    Return PropertyAccess.Boolean2LocalizedYesNo(Me.DisplayCopyright, formatProvider)
                Case "copyright"
                    Return PropertyAccess.FormatString(Me.Copyright, strFormat)
                Case "permalink"
                    Return PropertyAccess.FormatString(Me.PermaLink, strFormat)
                Case "commentcount"
                    Return (Me.CommentCount.ToString(OutputFormat, formatProvider))
                Case "syndicationemail"
                    Return PropertyAccess.FormatString(Me.SyndicationEmail, strFormat)
                Case "createduserid"
                    Return (Me.CreatedUserId.ToString(OutputFormat, formatProvider))
                Case "viewcount"
                    Return (Me.ViewCount.ToString(OutputFormat, formatProvider))
                Case "totalrecords"
                    Return (Me.TotalRecords.ToString(OutputFormat, formatProvider))
                Case Else
                    PropertyNotFound = True
            End Select
            Return Null.NullString
        End Function

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
                If Not DateTime.TryParse(readElement(reader, "AddedDate"), AddedDate) Then
                    AddedDate = DateTime.MinValue
                End If
                Boolean.TryParse(readElement(reader, "AllowComments"), AllowComments)
                If Not Int32.TryParse(readElement(reader, "BlogID"), BlogID) Then
                    BlogID = Null.NullInteger
                End If
                Copyright = readElement(reader, "Copyright")
                Description = readElement(reader, "Description")
                Boolean.TryParse(readElement(reader, "DisplayCopyright"), DisplayCopyright)
                Entry = readElement(reader, "Entry")
                PermaLink = readElement(reader, "PermaLink")
                Boolean.TryParse(readElement(reader, "Published"), Published)
                Title = readElement(reader, "Title")
                If Not Int32.TryParse(readElement(reader, "CreatedUserId"), CreatedUserId) Then
                    CreatedUserId = Null.NullInteger
                End If
                If Not Int32.TryParse(readElement(reader, "ViewCount"), ViewCount) Then
                    ViewCount = 0
                End If
            Catch ex As Exception
                ' log exception as DNN import routine does not do that
                DotNetNuke.Services.Exceptions.LogException(ex)
                ' re-raise exception to make sure import routine displays a visible error to the user
                Throw New Exception("An error occured during import of an Entry", ex)
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
            writer.WriteStartElement("Entry")
            writer.WriteElementString("EntryID", EntryID.ToString())
            writer.WriteElementString("AddedDate", AddedDate.ToString())
            writer.WriteElementString("AllowComments", AllowComments.ToString())
            writer.WriteElementString("BlogID", BlogID.ToString())
            writer.WriteElementString("Copyright", Copyright)
            writer.WriteElementString("Description", Description)
            writer.WriteElementString("DisplayCopyright", DisplayCopyright.ToString())
            writer.WriteElementString("Entry", Entry)
            writer.WriteElementString("PermaLink", PermaLink)
            writer.WriteElementString("Published", Published.ToString())
            writer.WriteElementString("Title", Title)
            writer.WriteElementString("CreatedUserId", CreatedUserId.ToString())
            writer.WriteElementString("ViewCount", ViewCount.ToString())
            writer.WriteEndElement()
        End Sub

#End Region

    End Class
End Namespace