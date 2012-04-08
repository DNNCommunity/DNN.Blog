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

Imports DotNetNuke.Modules.Blog.Business
Imports DotNetNuke.Entities.Content
Imports DotNetNuke.Entities.Content.Common
Imports System.Linq
Imports DotNetNuke.Common.Utilities

''' <summary>
''' This class handles all core content item integration methods. This is abstracted to create a centralized spot within the module to manage it's own content items.
''' </summary>
''' <remarks></remarks>
Public Class Content

#Region "Private Members"

    Private Const ContentTypeName As String = "DNN_Blog"

#End Region

    ''' <summary>
    ''' Creates a content item in the data store (via core API). Also, associates any 'terms'. 
    ''' </summary>
    ''' <param name="objEntry"></param>
    ''' <param name="tabId"></param>
    ''' <returns></returns>
    ''' <remarks>Once created, a thread is immediately updated (thread = content item). Will handlescontent type check too.</remarks>
    Friend Function CreateContentItem(ByVal objEntry As EntryInfo, ByVal tabId As Integer) As ContentItem
        Dim typeController As IContentTypeController = New ContentTypeController
        Dim colContentTypes As IQueryable(Of ContentType) = (From t In typeController.GetContentTypes() Where t.ContentType = ContentTypeName Select t)
        Dim ContentTypeID As Integer

        If colContentTypes.Count > 0 Then
            ContentTypeID = colContentTypes(0).ContentTypeId
        Else
            ContentTypeID = CreateContentType()
        End If

        Dim objContent As New ContentItem
        objContent.Content = objEntry.Entry
        objContent.ContentTypeId = ContentTypeID
        objContent.Indexed = False
        objContent.ContentKey = "EntryId=" + objEntry.EntryID.ToString()
        objContent.ModuleID = objEntry.ModuleID
        objContent.TabID = objEntry.TabID

        objContent.ContentItemId = Util.GetContentController.AddContentItem(objContent)

        ' we need to update the thread here so it has the new content item id
        Dim cntEntry As New EntryController()
        'cntThread.UpdateThread(objThread.ThreadID, objContent.ContentItemId, objThread.SitemapInclude)

        ' Update Terms
        Dim cntTerm As New Terms()
        cntTerm.ManageEntryTerms(objEntry, objContent)

        Return objContent
    End Function

    ''' <summary>
    ''' Updates a content item in the data store (via core API). Also updates associated 'terms'. 
    ''' </summary>
    ''' <param name="objEntry"></param>
    ''' <param name="tabId"></param>
    ''' <remarks></remarks>
    Friend Sub UpdateContentItem(ByVal objEntry As EntryInfo, ByVal tabId As Integer)
        Dim objContent As New ContentItem
        objContent = Util.GetContentController().GetContentItem(objEntry.ContentItemId)

        If objContent Is Nothing Then
            Return
        End If
        objContent.Content = objEntry.Entry
        objContent.TabID = tabId
        objContent.ModuleID = objEntry.ModuleID

        Util.GetContentController().UpdateContentItem(objContent)

        ' Update Terms
        Dim cntTerm As New Terms()
        cntTerm.ManageEntryTerms(objEntry, objContent)
    End Sub

    ''' <summary>
    ''' Deletes a content item from the data store (via core API).
    ''' </summary>
    ''' <param name="objEntry"></param>
    Friend Shared Sub DeleteContentItem(ByVal contentItemId As Integer)
        If contentItemId <= Null.NullInteger Then
            Return
        End If
        Dim objContent As ContentItem = Util.GetContentController().GetContentItem(contentItemId)
        If objContent Is Nothing Then
            Return
        End If

        ' remove any metadata/terms associated first (perhaps we should just rely on ContentItem cascade delete here?)
        Dim cntTerms As New Terms()
        cntTerms.RemoveEntryTerms(objContent)

        Util.GetContentController().DeleteContentItem(objContent)
    End Sub

#Region "Private Methods"

    ''' <summary>
    ''' This will create our content item type. 
    ''' </summary>
    ''' <returns>ContentTypeID, the primary key integer.</returns>
    ''' <remarks></remarks>
    Private Shared Function CreateContentType() As Integer
        Dim cntType As New ContentTypeController()
        Dim objContentType As ContentType = New ContentType() With {.ContentType = ContentTypeName}

        Return cntType.AddContentType(objContentType)
    End Function

#End Region

End Class