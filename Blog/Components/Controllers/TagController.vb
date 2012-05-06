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
Imports DotNetNuke.Modules.Blog.Components.Business
Imports DotNetNuke.Modules.Blog.Data
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Modules.Blog.Components.Entities

Namespace Components.Controllers


    Public Class TagController

        Public Shared Function GetTag(ByVal TagID As Integer) As TagInfo
            Return CType(CBO.FillObject(DataProvider.Instance().GetTag(TagID), GetType(TagInfo)), TagInfo)
        End Function

        Public Shared Function GetTagsByEntry(ByVal EntryID As Integer) As String
            Dim TagList As ArrayList
            Dim TagString As String = ""
            TagList = CBO.FillCollection(DataProvider.Instance().ListTagsByEntry(EntryID), GetType(TagInfo))
            For Each tag As TagInfo In TagList
                If TagString.Length > 0 Then
                    TagString = TagString + ","
                End If
                TagString = TagString + tag.Tag
            Next

            Return TagString

        End Function


        Public Shared Function ListTagsByEntry(ByVal EntryID As Integer) As ArrayList

            Return CBO.FillCollection(DataProvider.Instance().ListTagsByEntry(EntryID), GetType(TagInfo))

        End Function


        Public Shared Sub UpdateTagsByEntry(ByVal EntryID As Integer, ByVal TagList As String)

            Dim TagArray() As String
            Dim TagInfos As ArrayList = CBO.FillCollection(DataProvider.Instance().ListTagsByEntry(EntryID), GetType(TagInfo))
            Dim found As Boolean

            TagList = Trim(TagList)
            TagArray = Split(TagList, ",")

            If Not TagList = "" Then
                For i As Integer = 0 To TagArray.Length - 1
                    TagArray(i) = Trim(TagArray(i))
                Next i

                ' Find all the new tags & add them
                For Each t As String In TagArray
                    found = False
                    For Each o As TagInfo In TagInfos
                        If o.Tag = t Then
                            found = True
                            Exit For
                        End If
                    Next
                    If Not found And Not t = "" Then
                        AddEntryTag(EntryID, t)
                    End If
                Next

            End If


            ' Find existing tags that need to be dropped & delete them
            For Each t As TagInfo In TagInfos
                found = False
                For Each o As String In TagArray
                    If o = t.Tag Then
                        found = True
                        Exit For
                    End If
                Next
                If Not found Then
                    DataProvider.Instance().DeleteEntryTag(EntryID, t.Tag)
                End If
            Next

        End Sub

        Public Shared Sub AddEntryTag(ByVal EntryId As Integer, ByVal Tag As String)

            AddEntryTag(EntryId, Tag, Utility.CreateFriendlySlug(Tag))

        End Sub

        Public Shared Sub AddEntryTag(ByVal EntryId As Integer, ByVal Tag As String, ByVal Slug As String)

            Data.DataProvider.Instance().AddEntryTag(EntryId, Tag, Slug)

        End Sub

        Public Shared Function ListTags(ByVal PortalID As Integer) As ArrayList

            Return CBO.FillCollection(DataProvider.Instance().ListTagsCnt(PortalID), GetType(TagInfo))

        End Function

        Public Shared Function ListWeightedTags(ByVal PortalID As Integer) As ArrayList

            Dim TagList As ArrayList
            Dim tag As TagInfo
            Dim count, total As Integer
            Dim stddev, mean, sumsqrs, factor As Double

            TagList = CBO.FillCollection(DataProvider.Instance().ListTagsAlpha(PortalID), GetType(TagInfo))

            count = TagList.Count

            For Each tag In TagList
                total = total + tag.Cnt
            Next

            mean = total / count

            For Each tag In TagList
                sumsqrs = sumsqrs + ((tag.Cnt - mean) ^ 2)
            Next

            stddev = Math.Sqrt(sumsqrs / count)

            For Each tag In TagList

                factor = (tag.Cnt - mean) / (stddev)
                If factor <= (-2 * stddev) Then
                    tag.Weight = 1
                End If
                If (-2 * stddev) < factor And factor <= (-1 * stddev) Then
                    tag.Weight = 2
                End If
                If (-1 * stddev < factor) And factor <= (-0.5 * stddev) Then
                    tag.Weight = 3
                End If
                If (-0.5 * stddev) < factor And factor < (0.5 * stddev) Then
                    tag.Weight = 4
                End If
                If (0.5 * stddev <= factor) And factor < stddev Then
                    tag.Weight = 5
                End If
                If stddev <= factor And factor < (2 * stddev) Then
                    tag.Weight = 6
                End If
                If factor >= (2 * stddev) Then
                    tag.Weight = 7
                End If

            Next

            Return TagList
        End Function

    End Class

End Namespace