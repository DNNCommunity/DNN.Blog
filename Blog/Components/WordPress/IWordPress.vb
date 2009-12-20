Imports CookComputing.XmlRpc

Namespace WordPress
 Public Interface IWordPress

  <XmlRpcMethod("wp.getCategories", Description:="Retrieves the categories for a blog using the Wordpress API. Returns an array of category Infos.")> _
  Function getCategories(ByVal blogid As String, ByVal username As String, ByVal password As String) As CategoryInfo()

 End Interface

 Public Structure CategoryInfo
  Public categoryId As Integer
  Public parentId As Integer
  Public description As String
  Public categoryName As String
  Public htmlUrl As String
  Public rssUrl As String
 End Structure

End Namespace
