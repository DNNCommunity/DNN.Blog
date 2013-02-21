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

Imports System.Xml
Imports DotNetNuke.Modules.Blog.Common

Partial Public Class WLWManifest
 Inherits System.Web.UI.Page

#Region " Private Members "
 Private _moduleId As Integer = -1
 Private _tabId As Integer = -1
#End Region

#Region " Event Handlers "
 Private Sub WLWManifest_Init(sender As Object, e As System.EventArgs) Handles Me.Init
  Me.Request.Params.ReadValue("ModuleId", _moduleId)
 End Sub

 Protected Sub WLWManifest_Load(sender As Object, e As System.EventArgs) Handles Me.Load

  Response.Clear()
  Response.ClearContent()
  Response.ContentType = "application/xml"

  Using output As New XmlTextWriter(Response.OutputStream, System.Text.Encoding.UTF8)
   Dim bs As ModuleSettings = ModuleSettings.GetModuleSettings(_moduleId)

   output.Formatting = Formatting.Indented
   output.WriteStartDocument()
   output.WriteStartElement("manifest")
   output.WriteAttributeString("xmlns", "http://schemas.microsoft.com/wlw/manifest/weblog")
   output.WriteStartElement("options")

   output.WriteElementString("clientType", "Metaweblog")
   output.WriteElementString("supportsMultipleCategories", bs.AllowMultipleCategories.ToYesNo)
   output.WriteElementString("supportsCategories", CBool(bs.VocabularyId <> -1).ToYesNo)
   output.WriteElementString("supportsCustomDate", "Yes")
   output.WriteElementString("supportsKeywords", "Yes")
   output.WriteElementString("supportsTrackbacks", "No")
   output.WriteElementString("supportsEmbeds", "No")
   output.WriteElementString("supportsAuthor", "No")
   output.WriteElementString("supportsExcerpt", "Yes")
   output.WriteElementString("supportsPassword", "No")
   output.WriteElementString("supportsPages", "No")
   output.WriteElementString("supportsPageParent", "No")
   output.WriteElementString("supportsPageOrder", "No")
   output.WriteElementString("supportsExtendedEntries", "Yes")
   output.WriteElementString("supportsCommentPolicy", "Yes")
   output.WriteElementString("supportsPingPolicy", "No")
   output.WriteElementString("supportsPostAsDraft", "Yes")
   output.WriteElementString("supportsFileUpload", "Yes")
   output.WriteElementString("supportsSlug", "No")
   output.WriteElementString("supportsHierarchicalCategories", "Yes")
   output.WriteElementString("supportsCategoriesInline", "Yes")
   output.WriteElementString("supportsNewCategories", bs.BloggersCanEditCategories.ToYesNo)
   output.WriteElementString("supportsNewCategoriesInline", "No")

   output.WriteEndElement() ' manifest
   output.WriteEndElement() ' options
   output.Flush()

  End Using

  Response.End()

 End Sub
#End Region

End Class