'
' DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2005
' by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
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
'-------------------------------------------------------------------------

Imports System
Imports DotNetNuke.Modules.Blog.Data
Imports DotNetNuke.Common.Utilities

Namespace Business

 Public Class SearchController

  Public Function SearchByKeywordByPortal(ByVal PortalID As Integer, ByVal SearchString As String, ByVal ShowNonPublic As Boolean, ByVal ShowNonPublished As Boolean) As ArrayList
   Return CBO.FillCollection(DataProvider.Instance().SearchByKeyWordByPortal(PortalID, SearchString, ShowNonPublic, ShowNonPublished), GetType(SearchResult))
  End Function

  Public Function SearchByKeywordByBlog(ByVal BlogID As Integer, ByVal SearchString As String, ByVal ShowNonPublic As Boolean, ByVal ShowNonPublished As Boolean) As ArrayList
   Return CBO.FillCollection(DataProvider.Instance().SearchByKeyWordByBlog(BlogID, SearchString, ShowNonPublic, ShowNonPublished), GetType(SearchResult))
  End Function

  Public Function SearchByPhraseByPortal(ByVal PortalID As Integer, ByVal SearchString As String, ByVal ShowNonPublic As Boolean, ByVal ShowNonPublished As Boolean) As ArrayList
   Return CBO.FillCollection(DataProvider.Instance().SearchByPhraseByPortal(PortalID, SearchString, ShowNonPublic, ShowNonPublished), GetType(SearchResult))
  End Function

  Public Function SearchByPhraseByBlog(ByVal BlogID As Integer, ByVal SearchString As String, ByVal ShowNonPublic As Boolean, ByVal ShowNonPublished As Boolean) As ArrayList
   Return CBO.FillCollection(DataProvider.Instance().SearchByPhraseByBlog(BlogID, SearchString, ShowNonPublic, ShowNonPublished), GetType(SearchResult))
  End Function

 End Class

End Namespace
