Imports CookComputing.XmlRpc

Namespace Blogger
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

