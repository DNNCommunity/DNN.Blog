Imports System.Linq
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports DotNetNuke.Modules.Blog.Entities.Terms

Namespace Common
 Module Extensions

#Region " Collection Read Extensions "
  <System.Runtime.CompilerServices.Extension()>
  Public Sub ReadValue(ByRef ValueTable As Hashtable, ValueName As String, ByRef Variable As Integer)
   If Not ValueTable.Item(ValueName) Is Nothing Then
    Try
     Variable = CType(ValueTable.Item(ValueName), Integer)
    Catch ex As Exception
    End Try
   End If
  End Sub

  <System.Runtime.CompilerServices.Extension()>
  Public Sub ReadValue(ByRef ValueTable As Hashtable, ValueName As String, ByRef Variable As Long)
   If Not ValueTable.Item(ValueName) Is Nothing Then
    Try
     Variable = CType(ValueTable.Item(ValueName), Long)
    Catch ex As Exception
    End Try
   End If
  End Sub

  <System.Runtime.CompilerServices.Extension()>
  Public Sub ReadValue(ByRef ValueTable As Hashtable, ValueName As String, ByRef Variable As String)
   If Not ValueTable.Item(ValueName) Is Nothing Then
    Try
     Variable = CType(ValueTable.Item(ValueName), String)
    Catch ex As Exception
    End Try
   End If
  End Sub

  <System.Runtime.CompilerServices.Extension()>
  Public Sub ReadValue(ByRef ValueTable As Hashtable, ValueName As String, ByRef Variable As Boolean)
   If Not ValueTable.Item(ValueName) Is Nothing Then
    Try
     Variable = CType(ValueTable.Item(ValueName), Boolean)
    Catch ex As Exception
    End Try
   End If
  End Sub

  <System.Runtime.CompilerServices.Extension()>
  Public Sub ReadValue(ByRef ValueTable As Hashtable, ValueName As String, ByRef Variable As Date)
   If Not ValueTable.Item(ValueName) Is Nothing Then
    Try
     Variable = CType(ValueTable.Item(ValueName), Date)
    Catch ex As Exception
    End Try
   End If
  End Sub

  <System.Runtime.CompilerServices.Extension()>
  Public Sub ReadValue(ByRef ValueTable As Hashtable, ValueName As String, ByRef Variable As SummaryType)
   If Not ValueTable.Item(ValueName) Is Nothing Then
    Try
     Variable = CType(CType(ValueTable.Item(ValueName), Integer), SummaryType)
    Catch ex As Exception
    End Try
   End If
  End Sub

  <System.Runtime.CompilerServices.Extension()>
  Public Sub ReadValue(ByRef ValueTable As Hashtable, ValueName As String, ByRef Variable As LocalizationType)
   If Not ValueTable.Item(ValueName) Is Nothing Then
    Try
     Variable = CType(CType(ValueTable.Item(ValueName), Integer), LocalizationType)
    Catch ex As Exception
    End Try
   End If
  End Sub

  <System.Runtime.CompilerServices.Extension()>
  Public Sub ReadValue(ByRef ValueTable As Hashtable, ValueName As String, ByRef Variable As TimeSpan)
   If Not ValueTable.Item(ValueName) Is Nothing Then
    Try
     Variable = TimeSpan.Parse(CType(ValueTable.Item(ValueName), String))
    Catch ex As Exception
    End Try
   End If
  End Sub

  <System.Runtime.CompilerServices.Extension()>
  Public Sub ReadValue(ByRef ValueTable As NameValueCollection, ValueName As String, ByRef Variable As Integer)
   If Not ValueTable.Item(ValueName) Is Nothing Then
    Try
     Variable = CType(ValueTable.Item(ValueName), Integer)
    Catch ex As Exception
    End Try
   End If
  End Sub

  <System.Runtime.CompilerServices.Extension()>
  Public Sub ReadValue(ByRef ValueTable As NameValueCollection, ValueName As String, ByRef Variable As Long)
   If Not ValueTable.Item(ValueName) Is Nothing Then
    Try
     Variable = CType(ValueTable.Item(ValueName), Long)
    Catch ex As Exception
    End Try
   End If
  End Sub

  <System.Runtime.CompilerServices.Extension()>
  Public Sub ReadValue(ByRef ValueTable As NameValueCollection, ValueName As String, ByRef Variable As String)
   If Not ValueTable.Item(ValueName) Is Nothing Then
    Try
     Variable = CType(ValueTable.Item(ValueName), String)
    Catch ex As Exception
    End Try
   End If
  End Sub

  <System.Runtime.CompilerServices.Extension()>
  Public Sub ReadValue(ByRef ValueTable As NameValueCollection, ValueName As String, ByRef Variable As Boolean)
   If Not ValueTable.Item(ValueName) Is Nothing Then
    Try
     Variable = CType(ValueTable.Item(ValueName), Boolean)
    Catch ex As Exception
     Select Case ValueTable.Item(ValueName).ToLower
      Case "on", "yes"
       Variable = True
      Case Else
       Variable = False
     End Select
    End Try
   End If
  End Sub

  <System.Runtime.CompilerServices.Extension()>
  Public Sub ReadValue(ByRef ValueTable As NameValueCollection, ValueName As String, ByRef Variable As Date)
   If Not ValueTable.Item(ValueName) Is Nothing Then
    Try
     Variable = CType(ValueTable.Item(ValueName), Date)
    Catch ex As Exception
    End Try
   End If
  End Sub

  <System.Runtime.CompilerServices.Extension()>
  Public Sub ReadValue(ByRef ValueTable As NameValueCollection, ValueName As String, ByRef Variable As TimeSpan)
   If Not ValueTable.Item(ValueName) Is Nothing Then
    Try
     Variable = TimeSpan.Parse(CType(ValueTable.Item(ValueName), String))
    Catch ex As Exception
    End Try
   End If
  End Sub

  <System.Runtime.CompilerServices.Extension()>
  Public Sub ReadValue(ValueTable As Dictionary(Of String, String), ValueName As String, ByRef Variable As Integer)
   If ValueTable.ContainsKey(ValueName) Then
    Try
     Variable = CType(ValueTable.Item(ValueName), Integer)
    Catch ex As Exception
    End Try
   End If
  End Sub

  <System.Runtime.CompilerServices.Extension()>
  Public Sub ReadValue(ValueTable As Dictionary(Of String, String), ValueName As String, ByRef Variable As String)
   If ValueTable.ContainsKey(ValueName) Then
    Try
     Variable = CType(ValueTable.Item(ValueName), String)
    Catch ex As Exception
    End Try
   End If
  End Sub

  <System.Runtime.CompilerServices.Extension()>
  Public Sub ReadValue(ValueTable As Dictionary(Of String, String), ValueName As String, ByRef Variable As Boolean)
   If ValueTable.ContainsKey(ValueName) Then
    Try
     Variable = CType(ValueTable.Item(ValueName), Boolean)
    Catch ex As Exception
    End Try
   End If
  End Sub

  <System.Runtime.CompilerServices.Extension()>
  Public Sub ReadValue(ValueTable As Dictionary(Of String, String), ValueName As String, ByRef Variable As Date)
   If ValueTable.ContainsKey(ValueName) Then
    Try
     Variable = CType(ValueTable.Item(ValueName), Date)
    Catch ex As Exception
    End Try
   End If
  End Sub

  <System.Runtime.CompilerServices.Extension()>
  Public Sub ReadValue(ValueTable As Dictionary(Of String, String), ValueName As String, ByRef Variable As TimeSpan)
   If ValueTable.ContainsKey(ValueName) Then
    Try
     Variable = TimeSpan.Parse(CType(ValueTable.Item(ValueName), String))
    Catch ex As Exception
    End Try
   End If
  End Sub

  <System.Runtime.CompilerServices.Extension()>
  Public Sub ReadValue(ByRef ValueTable As StateBag, ValueName As String, ByRef Variable As Integer)
   If Not ValueTable.Item(ValueName) Is Nothing Then
    Try
     Variable = CType(ValueTable.Item(ValueName), Integer)
    Catch ex As Exception
    End Try
   End If
  End Sub

  <System.Runtime.CompilerServices.Extension()>
  Public Sub ReadValue(ByRef ValueTable As StateBag, ValueName As String, ByRef Variable As Long)
   If Not ValueTable.Item(ValueName) Is Nothing Then
    Try
     Variable = CType(ValueTable.Item(ValueName), Long)
    Catch ex As Exception
    End Try
   End If
  End Sub

  <System.Runtime.CompilerServices.Extension()>
  Public Sub ReadValue(ByRef ValueTable As StateBag, ValueName As String, ByRef Variable As String)
   If Not ValueTable.Item(ValueName) Is Nothing Then
    Try
     Variable = CType(ValueTable.Item(ValueName), String)
    Catch ex As Exception
    End Try
   End If
  End Sub

  <System.Runtime.CompilerServices.Extension()>
  Public Sub ReadValue(ByRef ValueTable As StateBag, ValueName As String, ByRef Variable As Boolean)
   If Not ValueTable.Item(ValueName) Is Nothing Then
    Try
     Variable = CType(ValueTable.Item(ValueName), Boolean)
    Catch ex As Exception
    End Try
   End If
  End Sub

  <System.Runtime.CompilerServices.Extension()>
  Public Sub ReadValue(ByRef ValueTable As StateBag, ValueName As String, ByRef Variable As Date)
   If Not ValueTable.Item(ValueName) Is Nothing Then
    Try
     Variable = CType(ValueTable.Item(ValueName), Date)
    Catch ex As Exception
    End Try
   End If
  End Sub

  <System.Runtime.CompilerServices.Extension()>
  Public Sub ReadValue(ByRef ValueTable As StateBag, ValueName As String, ByRef Variable As TimeSpan)
   If Not ValueTable.Item(ValueName) Is Nothing Then
    Try
     Variable = TimeSpan.Parse(CType(ValueTable.Item(ValueName), String))
    Catch ex As Exception
    End Try
   End If
  End Sub
#End Region

#Region " Conversion Extensions "
  <System.Runtime.CompilerServices.Extension()>
  Public Function ToInt(var As Boolean) As Integer
   If var Then
    Return 1
   Else
    Return 0
   End If
  End Function

  <System.Runtime.CompilerServices.Extension()>
  Public Function ToYesNo(var As Boolean) As String
   If var Then
    Return "Yes"
   Else
    Return "No"
   End If
  End Function

  <System.Runtime.CompilerServices.Extension()>
  Public Function ToInt(var As String) As Integer
   If IsNumeric(var) Then
    Return Integer.Parse(var)
   Else
    Return -1
   End If
  End Function

  <System.Runtime.CompilerServices.Extension()>
  Public Function ToBool(var As Integer) As Boolean
   Return CBool(var > 0)
  End Function

  <System.Runtime.CompilerServices.Extension()>
  Public Function ToStringArray(terms As List(Of TermInfo)) As String()
   Return terms.Select(Function(x)
                        Return x.Name
                       End Function).ToArray
  End Function

  <System.Runtime.CompilerServices.Extension()>
  Public Function ToTermIDString(terms As List(Of TermInfo)) As String
   Return ToTermIDString(terms, ";")
  End Function

  <System.Runtime.CompilerServices.Extension()>
  Public Function ToTermIDString(terms As List(Of TermInfo), separator As String) As String
   Dim res As New List(Of String)
   For Each t As TermInfo In terms
    res.Add(t.TermId.ToString)
   Next
   Return String.Join(separator, res.ToArray)
  End Function
#End Region

 End Module
End Namespace
