Namespace Common
 Public Class BlogPostException
  Inherits Exception

  Private _resourceKey As String
  Public Sub New(ByVal resourceKey As String, ByVal defaultMessage As String)
   MyBase.New(defaultMessage)
   _resourceKey = resourceKey
  End Sub

  Public Property ResourceKey() As String
   Get
    Return _resourceKey
   End Get
   Set(ByVal Value As String)
    _resourceKey = Value
   End Set
  End Property

 End Class
End Namespace