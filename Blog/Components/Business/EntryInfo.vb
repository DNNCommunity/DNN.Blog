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

Namespace Business

  <Serializable(), XmlRoot("Entry")> _
  Public Class EntryInfo
    Implements IHydratable
    Implements IPropertyAccess
    Implements IXmlSerializable

#Region " Local Variables "
    Private _UserID As Integer
    Private _Username As String
    Private _UserFullName As String
    Private _EntryID As Integer
    Private _BlogID As Integer
    Private _Title As String
    Private _Description As String
    Private _Entry As String
    Private _AddedDate As DateTime
    Private _Published As Boolean = False
    Private _AllowComments As Boolean
    Private _DisplayCopyright As Boolean
    Private _Copyright As String
    Private _PermaLink As String
    Private _CommentCount As Integer
    Private _SyndicationEmail As String
#End Region

#Region " Constructors "
    Public Sub New()
    End Sub

    Public Sub New(ByVal UserID As Integer, ByVal Username As String, ByVal UserFullName As String, ByVal EntryID As Integer, ByVal BlogID As Integer, ByVal Title As String, ByVal Description As String, ByVal Entry As String, ByVal AddedDate As DateTime, ByVal Published As Boolean, ByVal AllowComments As Boolean, ByVal AllowAnonymous As Boolean, ByVal BlogSyndicated As Boolean, ByVal BlogPublic As Boolean, ByVal CategoryID As Integer, ByVal CategoryTitle As String, ByVal CategorySyndicated As Boolean, ByVal CategoryPublic As Boolean, ByVal CommentCount As Integer)

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
    End Sub
#End Region

#Region " Public Properties "
    Public Property EntryID() As Integer
      Get
        Return _EntryID
      End Get
      Set(ByVal Value As Integer)
        _EntryID = Value
      End Set
    End Property

    Public Property BlogID() As Integer
      Get
        Return _BlogID
      End Get
      Set(ByVal Value As Integer)
        _BlogID = Value
      End Set
    End Property

    Public Property Title() As String
      Get
        Return _Title
      End Get
      Set(ByVal Value As String)
        _Title = Value
      End Set
    End Property

    Public Property Description() As String
      Get
        Return _Description
      End Get
      Set(ByVal Value As String)
        _Description = Value
      End Set
    End Property

    Public Property Entry() As String
      Get
        Return _Entry
      End Get
      Set(ByVal Value As String)
        _Entry = Value
      End Set
    End Property

    Public Property AddedDate() As DateTime
      Get
        Return _AddedDate
      End Get
      Set(ByVal Value As DateTime)
        _AddedDate = Value
      End Set
    End Property

    Public Property UserID() As Integer
      Get
        Return _UserID
      End Get
      Set(ByVal Value As Integer)
        _UserID = Value
      End Set
    End Property

    Public Property UserName() As String
      Get
        Return _Username
      End Get

      Set(ByVal Value As String)
        _Username = Value
      End Set
    End Property

    Public Property UserFullName() As String
      Get
        Return _UserFullName
      End Get

      Set(ByVal Value As String)
        _UserFullName = Value
      End Set
    End Property

    Public Property Published() As Boolean
      Get
        Return _Published
      End Get
      Set(ByVal Value As Boolean)
        _Published = Value
      End Set
    End Property


    Public Property CommentCount() As Integer
      Get
        Return _CommentCount
      End Get
      Set(ByVal Value As Integer)
        _CommentCount = Value
      End Set
    End Property

    Public Property AllowComments() As Boolean
      Get
        Return _AllowComments
      End Get
      Set(ByVal Value As Boolean)
        _AllowComments = Value
      End Set
    End Property

    Public Property DisplayCopyright() As Boolean
      Get
        Return _DisplayCopyright
      End Get
      Set(ByVal Value As Boolean)
        _DisplayCopyright = Value
      End Set
    End Property

    Public Property Copyright() As String
      Get
        Return _Copyright
      End Get
      Set(ByVal Value As String)
        _Copyright = Value
      End Set
    End Property

    Public Property PermaLink() As String
      Get
        Return _PermaLink
      End Get
      Set(ByVal Value As String)
        _PermaLink = Value
      End Set
    End Property

    Public Property SyndicationEmail() As String
      Get
        Return _SyndicationEmail
      End Get
      Set(ByVal Value As String)
        _SyndicationEmail = Value
      End Set
    End Property
#End Region

#Region " IHydratable Implementation "
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Fill hydrates the object from a Datareader
    ''' </summary>
    ''' <remarks>The Fill method is used by the CBO method to hydrate the object
    ''' rather than using the more expensive Refection  methods.</remarks>
    ''' <history>
    ''' 	[pdonker]	11/07/2010  Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub Fill(ByVal dr As IDataReader) Implements IHydratable.Fill

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

    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Gets and sets the Key ID
    ''' </summary>
    ''' <remarks>The KeyID property is part of the IHydratble interface.  It is used
    ''' as the key property when creating a Dictionary</remarks>
    ''' <history>
    ''' 	[pdonker]	11/07/2010  Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Property KeyID() As Integer Implements IHydratable.KeyID
      Get
        Return EntryID
      End Get
      Set(ByVal value As Integer)
        EntryID = value
      End Set
    End Property
#End Region

#Region " IPropertyAccess Methods "
    Public ReadOnly Property Cacheability() As Services.Tokens.CacheLevel Implements Services.Tokens.IPropertyAccess.Cacheability
      Get
        Return CacheLevel.fullyCacheable
      End Get
    End Property

    Public Function GetProperty(ByVal strPropertyName As String, ByVal strFormat As String, ByVal formatProvider As System.Globalization.CultureInfo, ByVal AccessingUser As Entities.Users.UserInfo, ByVal AccessLevel As Services.Tokens.Scope, ByRef PropertyNotFound As Boolean) As String Implements Services.Tokens.IPropertyAccess.GetProperty
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
          'Return PropertyAccess.FormatString(Me.AddedDate.ToString, strFormat)
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
        Case Else
          PropertyNotFound = True
      End Select
      Return Null.NullString

    End Function

#End Region

#Region " IXmlSerializable Implementation "
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' GetSchema returns the XmlSchema for this class
    ''' </summary>
    ''' <remarks>GetSchema is implemented as a stub method as it is not required</remarks>
    ''' <history>
    ''' 	[pdonker]	11/07/2010  Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
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

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' ReadXml fills the object (de-serializes it) from the XmlReader passed
    ''' </summary>
    ''' <remarks></remarks>
    ''' <param name="reader">The XmlReader that contains the xml for the object</param>
    ''' <history>
    ''' 	[pdonker]	11/07/2010  Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
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
      Catch ex As Exception
        ' log exception as DNN import routine does not do that
        DotNetNuke.Services.Exceptions.LogException(ex)
        ' re-raise exception to make sure import routine displays a visible error to the user
        Throw New Exception("An error occured during import of an Entry", ex)
      End Try

    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' WriteXml converts the object to Xml (serializes it) and writes it using the XmlWriter passed
    ''' </summary>
    ''' <remarks></remarks>
    ''' <param name="writer">The XmlWriter that contains the xml for the object</param>
    ''' <history>
    ''' 	[pdonker]	11/07/2010  Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
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
      writer.WriteEndElement()
    End Sub
#End Region

  End Class

End Namespace