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

Imports System
Imports System.Runtime.Serialization

Namespace Entities.Comments
 Partial Public Class CommentInfo

#Region " Private Members "
#End Region

#Region " Constructors "
  Public Sub New()
  End Sub
#End Region

#Region " Public Properties "
  <DataMember()>
  Public Property CommentID As Int32 = -1
  <DataMember()>
  Public Property ContentItemId As Int32 = -1
  <DataMember()>
  Public Property ParentId As Int32 = -1
  <DataMember()>
  Public Property Comment As String = ""
  <DataMember()>
  Public Property Approved As Boolean = False
  <DataMember()>
  Public Property Author As String = ""
  <DataMember()>
  Public Property Website As String = ""
  <DataMember()>
  Public Property Email As String = ""
  <DataMember()>
  Public Property CreatedByUserID As Int32 = -1
  <DataMember()>
  Public Property CreatedOnDate As Date = Date.MinValue
  <DataMember()>
  Public Property LastModifiedByUserID As Int32 = -1
  <DataMember()>
  Public Property LastModifiedOnDate As Date = Date.MinValue
  <DataMember()>
  Public Property Username As String = ""
  <DataMember()>
  Public Property DisplayName As String = ""
  <DataMember()>
  Public Property Likes As Int32 = 0
  <DataMember()>
  Public Property Dislikes As Int32 = 0
  <DataMember()>
  Public Property Reports As Int32 = 0
  <DataMember()>
  Public Property Liked As Int32 = 0
  <DataMember()>
  Public Property Disliked As Int32 = 0
  <DataMember()>
  Public Property Reported As Int32 = 0
#End Region

 End Class
End Namespace


