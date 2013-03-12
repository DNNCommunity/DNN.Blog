Namespace Services.WLW
 Public Class BlogPostException
  Inherits Exception

  Private _resourceKey As String
  Public Sub New(resourceKey As String, defaultMessage As String)
   MyBase.New(defaultMessage)
   _resourceKey = resourceKey
  End Sub

  Public Property ResourceKey() As String
   Get
    Return _resourceKey
   End Get
   Set(Value As String)
    _resourceKey = Value
   End Set
  End Property

 End Class
End Namespace