'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2011
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
Option Strict On
Option Explicit On

Imports DotNetNuke.Modules.Blog.Components.Entities
Imports DotNetNuke.Services.Journal

Namespace Components.Integration

    Public Class Journal

        ' get journal type id

        Public Sub AddItemToJournal(ByVal objEntry As EntryInfo, ByVal portalId As Integer, ByVal journalUserId As Integer, ByVal url As String)
            Dim jc As New JournalController
            Dim objectKey As String = Components.Common.Constants.ContentTypeName + String.Format("{0}:{1}", objEntry.BlogID.ToString(), objEntry.EntryID.ToString())
            Dim ji As JournalItem = jc.Journal_GetByKey(portalId, objectKey)
            If Not ji Is Nothing Then
                jc.Journal_DeleteByKey(portalId, objectKey)
            End If

            ji = New JournalItem

            ji.PortalId = portalId
            ji.ProfileId = journalUserId
            ji.UserId = journalUserId
            ji.ContentItemId = objEntry.ContentItemId
            ji.Title = objEntry.Title
            ji.ItemData = New ItemData()
            ji.ItemData.Url = url
            ji.Summary = objEntry.Description
            ji.Body = Nothing
            ji.JournalTypeId = 7 ' NOTE: CP - COMEBACK (Get by name, like content type)
            ji.ObjectKey = objectKey
            ji.SecuritySet = "E,"

            jc.Journal_Save(ji, -1)
        End Sub

    End Class

End Namespace