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
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Content.Taxonomy
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Tokens

Namespace Entities.Terms
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
#End Region

#Region " IHydratable Implementation "
  Public Overrides Sub Fill(dr As System.Data.IDataReader)
   MyBase.Fill(dr)
   MyBase.FillInternal(dr)

   TotalPosts = Null.SetNullInteger(dr("TotalPosts"))
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
     Return Me.ParentTermId.ToString
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

 End Class
End Namespace