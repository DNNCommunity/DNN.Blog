Namespace Common
 Public Class DynatreeItem
  Public Property title As String = ""
  Public Property key As String = ""
  Public Property icon As Boolean = False
  Public Property expand As Boolean = True
  Public Property isFolder As Boolean = True
  Public Property [select] As Boolean = False
  Public Property children As New List(Of DynatreeItem)
 End Class
End Namespace
