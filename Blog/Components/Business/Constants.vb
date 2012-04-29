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

Public Class Constants

    Public Enum TagMode
        ShowNoUsage = 0
        ShowDailyUsage = 1
        ShowWeeklyUsage = 2
        ShowMonthlyUsage = 3
        ShowTotalUsage = 4
    End Enum

    Public Const ContentTypeName As String = "DNN_Blog_Entry" ' change to DNN_Blog_Entry

    ''' <summary>
    ''' A recommended limit for a meta page title for SEO purposes.
    ''' </summary>
    Public Const SeoTitleLimit As Integer = 64

    ''' <summary>
    ''' A recommended limit for a meta page description for SEO purposes.
    ''' </summary>
    Public Const SeoDescriptionLimit As Integer = 150

    ''' <summary>
    ''' A recommended limit for meta page keywords for SEO purposes.
    ''' </summary>
    Public Const SeoKeywordsLimit As Integer = 15

    Public Const SharedResourceFileName As String = "~/DesktopModules/Blog/App_LocalResources/SharedResources.ascx.resx"

    Public Const ModuleCacheKeyPrefix As String = "DNN_Blog_"

    Public Const VocabTermsCacheKey As String = "Vocabulary_Terms_"

    Public Const VocabSuffixCacheKey As String = "Vocabulary_"

    Public Const VocabularySuffixCacheKey As String = "Vocabulary_"

    Public Const ContentItemTermsCacheKey As String = "ContentItem_Terms_"

End Class