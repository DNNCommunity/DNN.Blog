Imports System
Imports System.Runtime.Serialization

Namespace Entities.Posts
  Partial Public Class PostInfo
  Inherits DotNetNuke.Entities.Content.ContentItem

#Region " Private Members "
#End Region

#Region " Constructors "
  Public Sub New()
  End Sub
#End Region

#Region " Public Properties "
  <DataMember()>
  Public Property BlogID As Int32 = -1
  <DataMember()>
  Public Property Title As String = ""
  <DataMember()>
  Public Property Summary As String = ""
  <DataMember()>
  Public Property Image As String = ""
  <DataMember()>
  Public Property Published As Boolean = True
  <DataMember()>
  Public Property PublishedOnDate As Date = Date.Now.ToUniversalTime
  <DataMember()>
  Public Property AllowComments As Boolean = True
  <DataMember()>
  Public Property DisplayCopyright As Boolean = False
  <DataMember()>
  Public Property Copyright As String = ""
  <DataMember()>
  Public Property Locale As String = ""
  <DataMember()>
  Public Property Username As String = ""
  <DataMember()>
  Public Property Email As String = ""
  <DataMember()>
  Public Property DisplayName As String = ""
  <DataMember()>
  Public Property NrComments As Int32 = 0
  <DataMember()>
  Public Property ViewCount As Int32 = 0
#End Region

#Region " ML Properties "
  <DataMember()>
  Public Property AltLocale As String = ""
  <DataMember()>
  Public Property AltTitle As String = ""
  <DataMember()>
  Public Property AltSummary As String = ""
  <DataMember()>
  Public Property AltContent As String = ""

  Public ReadOnly Property LocalizedTitle As String
   Get
    Return CStr(IIf(String.IsNullOrEmpty(AltTitle), Title, AltTitle))
   End Get
  End Property

  Public ReadOnly Property LocalizedSummary As String
   Get
    Return CStr(IIf(String.IsNullOrEmpty(AltSummary), Summary, AltSummary))
   End Get
  End Property

  Public ReadOnly Property LocalizedContent As String
   Get
    Return CStr(IIf(String.IsNullOrEmpty(AltContent), Content, AltContent))
   End Get
  End Property

  ''' <summary>
  ''' ML text type to handle the title of the post
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Property TitleLocalizations() As LocalizedText
   Get
    If _titleLocalizations Is Nothing Then
     If ContentItemId = -1 Then
      _titleLocalizations = New LocalizedText
     Else
      _titleLocalizations = New LocalizedText(Data.DataProvider.Instance().GetPostLocalizations(ContentItemId), "Title")
     End If
    End If
    Return _titleLocalizations
   End Get
   Set(ByVal value As LocalizedText)
    _titleLocalizations = value
   End Set
  End Property
  Private _titleLocalizations As LocalizedText

  ''' <summary>
  ''' ML text type to handle the summary of the post
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Property SummaryLocalizations() As LocalizedText
   Get
    If _summaryLocalizations Is Nothing Then
     If ContentItemId = -1 Then
      _summaryLocalizations = New LocalizedText
     Else
      _summaryLocalizations = New LocalizedText(Data.DataProvider.Instance().GetPostLocalizations(ContentItemId), "Summary")
     End If
    End If
    Return _summaryLocalizations
   End Get
   Set(ByVal value As LocalizedText)
    _summaryLocalizations = value
   End Set
  End Property
  Private _summaryLocalizations As LocalizedText

  ''' <summary>
  ''' ML text type to handle the content of the post
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Property ContentLocalizations() As LocalizedText
   Get
    If _contentLocalizations Is Nothing Then
     If BlogID = -1 Then
      _contentLocalizations = New LocalizedText
     Else
      _contentLocalizations = New LocalizedText(Data.DataProvider.Instance().GetPostLocalizations(ContentItemId), "Content")
     End If
    End If
    Return _contentLocalizations
   End Get
   Set(ByVal value As LocalizedText)
    _contentLocalizations = value
   End Set
  End Property
  Private _contentLocalizations As LocalizedText
#End Region

 End Class
End Namespace


