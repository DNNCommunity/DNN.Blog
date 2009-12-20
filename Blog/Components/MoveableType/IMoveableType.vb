Imports CookComputing.XmlRpc

Namespace MoveableType
 Public Interface IMoveableType

  <XmlRpcMethod("mt.getPostCategories", Description:="Retrieves the categories for an existing post using the MoveableType " + "API. Returns the metaWeblog struct.")> _
  Function getPostCategories(ByVal postid As String, ByVal username As String, ByVal password As String) As Category()

  <XmlRpcMethod("mt.setPostCategories", Description:="Sets the categories for an existing post using the MoveableType " + "API. Returns the metaWeblog struct.")> _
  Function setPostCategories(ByVal postid As String, ByVal username As String, ByVal password As String, ByVal categories As Category()) As Boolean

 End Interface

 Public Structure Category
  Public categoryId As String
  <XmlRpcMissingMapping(MappingAction.Ignore)> _
  Public categoryName As String
  <XmlRpcMissingMapping(MappingAction.Ignore)> _
  Public isPrimary As Boolean
 End Structure

End Namespace
