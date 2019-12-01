'
' DNN Connect - http://dnn-connect.org
' Copyright (c) 2015
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

Imports System.Runtime.Serialization

Namespace Entities.LegacyUrls
 Partial Public Class LegacyUrlInfo

#Region " Private Members "
#End Region

#Region " Constructors "
  Public Sub New()
  End Sub

  Public Sub New(url As String, contentItemId As Int32, entryId As Int32)
   Me.ContentItemId = contentItemId
   Me.EntryId = entryId
   Me.Url = url
  End Sub
#End Region

#Region " Public Properties "
  <DataMember()>
  Public Property ContentItemId As Int32 = -1
  <DataMember()>
  Public Property EntryId As Int32 = -1
  <DataMember()>
  Public Property Url As String = ""
#End Region

 End Class
End Namespace
