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

Imports CookComputing.XmlRpc

Namespace Blogger

 ''' <summary>
 ''' Interface for Blogger Methods that WLW uses
 ''' </summary>
 ''' <remarks></remarks>
 ''' <history>
 '''		[pdonker]	12/14/2009	created
 ''' </history>
 Public Interface IBlogger

  <XmlRpcMethod("blogger.deletePost", Description:="Deletes a post.")> _
  Function deletePost(ByVal appKey As String, ByVal postid As String, ByVal username As String, ByVal password As String, <XmlRpcParameter(Description:="Where applicable, this specifies whether the blog " + "should be republished after the post has been deleted.")> _
                               ByVal publish As Boolean) As Boolean

  <XmlRpcMethod("blogger.getUsersBlogs", Description:="Returns information on all the blogs a given user " + "is a member.")> _
  Function getUsersBlogs(ByVal appKey As String, ByVal username As String, ByVal password As String) As BlogInfoStruct()

 End Interface

 Public Structure BlogInfoStruct
  Public url As String
  Public blogName As String
  Public blogid As String
 End Structure
End Namespace