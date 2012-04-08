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

''' <summary>
''' This class is used for managing taxonomy/folksonomy related terms via integration with the core.
''' </summary>
''' <remarks></remarks>
Public Class Terms

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="objThread"></param>
    ''' <param name="objContent"></param>
    ''' <remarks></remarks>
    Friend Sub ManageEntryTerms(ByVal objThread As EntryInfo, ByVal objContent As ContentItem)
        RemoveEntryTerms(objContent)

        For Each term As Entities.Content.Taxonomy.Term In objThread.Terms
            Util.GetTermController().AddTermToContent(term, objContent)
        Next
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="objContent"></param>
    ''' <remarks></remarks>
    Friend Sub RemoveEntryTerms(ByVal objContent As ContentItem)
        Util.GetTermController().RemoveTermsFromContent(objContent)
    End Sub

End Class