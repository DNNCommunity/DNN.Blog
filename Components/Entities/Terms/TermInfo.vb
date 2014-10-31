'
' DNN Connect - http://dnn-connect.org
' Copyright (c) 2014
' by DNN Connect
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

Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Content.Taxonomy
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Tokens
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports System.Linq
Imports System.Xml

Namespace Entities.Terms
 <Serializable()>
 Public Class TermInfo
  Inherits Term
  Implements IHydratable
  Implements IPropertyAccess

#Region " Constructors "
  Public Sub New()
  End Sub
  Public Sub New(name As String)
   Me.Description = ""
   Me.Name = name
   Me.ParentTermId = 0
   Me.TermId = -1
  End Sub
#End Region

#Region " Public Properties "
  Public Property TotalPosts() As Integer
#End Region

#Region " ML Properties "
  Public Property ParentTabID As Integer = -1
  Public Property AltLocale As String = ""
  Public Property AltName As String = ""
  Public Property AltDescription As String = ""

  Public ReadOnly Property LocalizedName As String
   Get
    Return CStr(IIf(String.IsNullOrEmpty(AltName), Name, AltName))
   End Get
  End Property

  Public ReadOnly Property LocalizedDescription As String
   Get
    Return CStr(IIf(String.IsNullOrEmpty(AltDescription), Description, AltDescription))
   End Get
  End Property

  ''' <summary>
  ''' ML text type to handle the name of the term
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Property NameLocalizations() As LocalizedText
   Get
    If _nameLocalizations Is Nothing Then
     If TermId = -1 Then
      _nameLocalizations = New LocalizedText
     Else
      _nameLocalizations = New LocalizedText(Data.DataProvider.Instance().GetTermLocalizations(TermId), "Name")
     End If
    End If
    Return _nameLocalizations
   End Get
   Set(ByVal value As LocalizedText)
    _nameLocalizations = value
   End Set
  End Property
  Private _nameLocalizations As LocalizedText

  ''' <summary>
  ''' ML text type to handle the description of the term
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Property DescriptionLocalizations() As LocalizedText
   Get
    If _descriptionLocalizations Is Nothing Then
     If TermId = -1 Then
      _descriptionLocalizations = New LocalizedText
     Else
      _descriptionLocalizations = New LocalizedText(Data.DataProvider.Instance().GetTermLocalizations(TermId), "Description")
     End If
    End If
    Return _descriptionLocalizations
   End Get
   Set(ByVal value As LocalizedText)
    _descriptionLocalizations = value
   End Set
  End Property
  Private _descriptionLocalizations As LocalizedText
#End Region

#Region " IHydratable Implementation "
  Public Overrides Sub Fill(dr As System.Data.IDataReader)
   MyBase.Fill(dr)
   MyBase.FillInternal(dr)

   TotalPosts = Null.SetNullInteger(dr("TotalPosts"))
   AltLocale = Convert.ToString(Null.SetNull(dr.Item("AltLocale"), AltLocale))
   AltName = Convert.ToString(Null.SetNull(dr.Item("AltName"), AltName))
   AltDescription = Convert.ToString(Null.SetNull(dr.Item("AltDescription"), AltDescription))
  End Sub
#End Region

#Region " IPropertyAccess Implementation "
  Public Function GetProperty(strPropertyName As String, strFormat As String, formatProvider As System.Globalization.CultureInfo, AccessingUser As DotNetNuke.Entities.Users.UserInfo, AccessLevel As DotNetNuke.Services.Tokens.Scope, ByRef PropertyNotFound As Boolean) As String Implements DotNetNuke.Services.Tokens.IPropertyAccess.GetProperty
   Dim OutputFormat As String = String.Empty
   Dim portalSettings As DotNetNuke.Entities.Portals.PortalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings()

   Dim ParentModule As New DotNetNuke.Entities.Modules.ModuleInfo
   'PropertySource(strObjectName.ToLower).GetProperty(strPropertyName, strFormat, formatProvider, AccessingUser, CurrentAccessLevel, PropertyNotFound)

   If strFormat = String.Empty Then
    OutputFormat = "D"
   Else
    OutputFormat = strFormat
   End If
   Select Case strPropertyName.ToLower
    Case "description"
     Return PropertyAccess.FormatString(Me.Description, strFormat)
    Case "isheirarchical"
     Return Me.IsHeirarchical.ToString
    Case "isheirarchicalyesno"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.IsHeirarchical, formatProvider)
    Case "left"
     Return (Me.Left.ToString(OutputFormat, formatProvider))
    Case "title", "name"
     Return PropertyAccess.FormatString(Me.Name, strFormat)
    Case "parenttermid"
     Return Me.ParentTermId.ToStringOrZero
    Case "right"
     Return (Me.Right.ToString(OutputFormat, formatProvider))
    Case "termid"
     Return (Me.TermId.ToString(OutputFormat, formatProvider))
    Case "vocabularyid"
     Return (Me.VocabularyId.ToString(OutputFormat, formatProvider))
    Case "weight"
     Return (Me.Weight.ToString(OutputFormat, formatProvider))
    Case "createdbyuserid"
     Return (Me.CreatedByUserID.ToString(OutputFormat, formatProvider))
    Case "createdondate"
     Return (Me.CreatedOnDate.ToString(OutputFormat, formatProvider))
    Case "lastmodifiedbyuserid"
     Return (Me.LastModifiedByUserID.ToString(OutputFormat, formatProvider))
    Case "lastmodifiedondate"
     Return (Me.LastModifiedOnDate.ToString(OutputFormat, formatProvider))
    Case "totalposts"
     Return (Me.TotalPosts.ToString(OutputFormat, formatProvider))
    Case "altlocale"
     Return PropertyAccess.FormatString(Me.AltLocale, strFormat)
    Case "altname"
     Return PropertyAccess.FormatString(Me.AltName, strFormat)
    Case "altdescription"
     Return PropertyAccess.FormatString(Me.AltDescription, strFormat)
    Case "localizedname"
     Return PropertyAccess.FormatString(Me.LocalizedName, strFormat)
    Case "localizeddescription"
     Return PropertyAccess.FormatString(Me.LocalizedDescription, strFormat)
    Case "link", "permalink"
     Return PermaLink(DotNetNuke.Entities.Portals.PortalSettings.Current)
    Case "parenturl"
     Return PermaLink(ParentTabID)
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

#Region " (De)serialization "
  Friend Property ImportedChildTerms As List(Of TermInfo)
  Public Sub FromXml(xml As XmlNode)
   If xml Is Nothing Then Exit Sub
   xml.ReadValue("Name", Name)
   xml.ReadValue("NameLocalizations", NameLocalizations)
   xml.ReadValue("Description", Description)
   xml.ReadValue("DescriptionLocalizations", DescriptionLocalizations)
   For Each xTerm As XmlNode In xml.SelectNodes("Term")
    Dim t As New TermInfo
    t.FromXml(xTerm)
    ImportedChildTerms.Add(t)
   Next
  End Sub

  Public Sub WriteXml(writer As XmlWriter, vocabulary As List(Of TermInfo))
   writer.WriteStartElement("Term")
   writer.WriteElementString("Name", Name)
   writer.WriteElementString("NameLocalizations", NameLocalizations.ToString)
   writer.WriteElementString("Description", Description)
   writer.WriteElementString("DescriptionLocalizations", DescriptionLocalizations.ToString)
   For Each t As TermInfo In vocabulary.Where(Function(x) CBool(x.ParentTermId IsNot Nothing AndAlso x.ParentTermId = TermId))
    t.WriteXml(writer, vocabulary)
   Next
   writer.WriteEndElement() ' Term
  End Sub
#End Region

#Region " Public Methods "
  Public Function PermaLink(portalSettings As DotNetNuke.Entities.Portals.PortalSettings) As String
   Return PermaLink(portalSettings.ActiveTab)
  End Function
  Public Function PermaLink(strParentTabID As Integer) As String
   Dim oTabController As DotNetNuke.Entities.Tabs.TabController = New DotNetNuke.Entities.Tabs.TabController
   Dim oParentTab As DotNetNuke.Entities.Tabs.TabInfo = oTabController.GetTab(strParentTabID, DotNetNuke.Entities.Portals.PortalSettings.Current.PortalId, False)
   _permaLink = String.Empty
   Return PermaLink(oParentTab)
  End Function

  Private _permaLink As String = ""
  Public Function PermaLink(tab As DotNetNuke.Entities.Tabs.TabInfo) As String
   If String.IsNullOrEmpty(_permaLink) Then
    _permaLink = ApplicationURL(tab.TabID) & "&Term=" & TermId.ToString
    If DotNetNuke.Entities.Host.Host.UseFriendlyUrls Then
     _permaLink = FriendlyUrl(tab, _permaLink, GetSafePageName(LocalizedName))
    Else
     _permaLink = ResolveUrl(_permaLink)
    End If
   End If
   Return _permaLink
  End Function

  Public Function FlatClone() As TermInfo
   Dim res As New TermInfo
   With res
    .Name = Me.Name
    .NameLocalizations = Me.NameLocalizations
    .Name = Me.Description
    .DescriptionLocalizations = Me.DescriptionLocalizations
    .Weight = Me.Weight
   End With
   Return res
  End Function
#End Region

 End Class
End Namespace