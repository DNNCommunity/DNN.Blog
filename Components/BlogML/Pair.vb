Imports System.Collections.Generic
Imports System.Text


Namespace BlogML

 ''' <summary>
 ''' A serializable keyvalue pair class
 ''' </summary>
 Public Structure Pair(Of K, V)
  Public Key As K
  Public Value As V
  Public Sub New(key As K, value As V)
   Me.Key = key
   Me.Value = value
  End Sub
 End Structure

End Namespace
