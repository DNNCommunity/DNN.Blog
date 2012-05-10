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
Imports DotNetNuke.Modules.Blog.Providers.Data
Imports DotNetNuke.Modules.Blog.Components.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Modules.Blog.Components.Integration
Imports DotNetNuke.Modules.Blog.Components.Entities

Namespace Components.Controllers

    Public Class TermController

        Public Function GetTermsByContentType(ByVal portalId As Integer, ByVal vocabularyId As Integer) As List(Of TermInfo)
            DotNetNuke.Common.Requires.PropertyNotNegative("portalId", "", portalId)
            DotNetNuke.Common.Requires.PropertyNotNegative("vocabularyId", "", vocabularyId)

            Dim colTerms As List(Of TermInfo) = DirectCast(DataCache.GetCache(Constants.ModuleCacheKeyPrefix + Constants.VocabTermsCacheKey + Constants.VocabSuffixCacheKey + vocabularyId.ToString()), List(Of TermInfo))

            If colTerms Is Nothing Then
                Dim timeOut As Int32 = Common.Constants.CACHE_TIMEOUT * Convert.ToInt32(DotNetNuke.Entities.Host.Host.PerformanceSetting)

                colTerms = CBO.FillCollection(Of TermInfo)(DataProvider.Instance().GetTermsByContentType(portalId, Content.GetContentTypeID(), vocabularyId))

                If timeOut > 0 And colTerms IsNot Nothing Then
                    DataCache.SetCache(Constants.ModuleCacheKeyPrefix + Constants.VocabTermsCacheKey + Constants.VocabSuffixCacheKey + vocabularyId.ToString(), colTerms, TimeSpan.FromMinutes(timeOut))
                End If
            End If
            Return colTerms
        End Function

        Public Function GetTermsByContentItem(ByVal contentItemId As Integer, ByVal vocabularyId As Integer) As List(Of TermInfo)
            DotNetNuke.Common.Requires.PropertyNotNegative("contentItemId", "", contentItemId)

            Dim colTerms As List(Of TermInfo) = DirectCast(DataCache.GetCache(Constants.ModuleCacheKeyPrefix + Constants.ContentItemTermsCacheKey + contentItemId.ToString() + Constants.VocabularySuffixCacheKey + vocabularyId.ToString()), List(Of TermInfo))

            If colTerms Is Nothing Then
                Dim timeOut As Int32 = Common.Constants.CACHE_TIMEOUT * Convert.ToInt32(DotNetNuke.Entities.Host.Host.PerformanceSetting)

                colTerms = CBO.FillCollection(Of TermInfo)(DataProvider.Instance().GetTermsByContentItem(contentItemId, vocabularyId))

                If timeOut > 0 And colTerms IsNot Nothing Then
                    DataCache.SetCache(Constants.ModuleCacheKeyPrefix + Constants.ContentItemTermsCacheKey + contentItemId.ToString() + Constants.VocabularySuffixCacheKey + vocabularyId.ToString(), colTerms, TimeSpan.FromMinutes(timeOut))
                End If
            End If
            Return colTerms
        End Function

    End Class

End Namespace