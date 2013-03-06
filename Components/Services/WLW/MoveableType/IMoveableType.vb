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

Namespace Services.WLW.MoveableType

 ''' <summary>
 ''' Interface for Moveable Type methods that WLW uses
 ''' </summary>
 ''' <remarks></remarks>
 ''' <history>
 '''		[pdonker]	12/14/2009	created
 ''' </history>
 Public Interface IMoveableType

  <XmlRpcMethod("mt.getPostCategories", Description:="Retrieves the categories for an existing post using the MoveableType " + "API. Returns the metaWeblog struct.")> _
  Function getPostCategories(postid As String, username As String, password As String) As Category()

  <XmlRpcMethod("mt.setPostCategories", Description:="Sets the categories for an existing post using the MoveableType " + "API. Returns the metaWeblog struct.")> _
  Function setPostCategories(postid As String, username As String, password As String, categories As Category()) As Boolean

 End Interface

 ''' <summary>
 ''' MT specific category structure
 ''' </summary>
 ''' <remarks></remarks>
 ''' <history>
 '''		[pdonker]	12/14/2009	created
 ''' </history>
 Public Structure Category
  Public categoryId As String
  <XmlRpcMissingMapping(MappingAction.Ignore)> _
  Public categoryName As String
  <XmlRpcMissingMapping(MappingAction.Ignore)> _
  Public isPrimary As Boolean
 End Structure

End Namespace