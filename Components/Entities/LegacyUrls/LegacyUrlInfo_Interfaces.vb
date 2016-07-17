'
' DNN Connect - http://dnn-connect.org
' Copyright (c) 2015
' by DNN Connect
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


Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules

Namespace Entities.LegacyUrls
 Partial Public Class LegacyUrlInfo
  Implements IHydratable

#Region " IHydratable Implementation "
  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Fill hydrates the object from a Datareader
  ''' </summary>
  ''' <remarks>The Fill method is used by the CBO method to hydrtae the object
  ''' rather than using the more expensive Refection methods.</remarks>
  ''' <history>
  ''' [pdonker] 05/23/2013 Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub Fill(dr As IDataReader) Implements IHydratable.Fill

   ContentItemId = Convert.ToInt32(Null.SetNull(dr.Item("ContentItemId"), ContentItemId))
   EntryId = Convert.ToInt32(Null.SetNull(dr.Item("EntryId"), EntryId))
   Url = Convert.ToString(Null.SetNull(dr.Item("Url"), Url))

  End Sub
  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets and sets the Key ID
  ''' </summary>
  ''' <remarks>The KeyID property is part of the IHydratble interface. It is used
  ''' as the key property when creating a Dictionary</remarks>
  ''' <history>
  ''' [pdonker] 05/23/2013 Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Property KeyID() As Integer Implements IHydratable.KeyID
   Get
    Return Nothing
   End Get
   Set(value As Integer)
   End Set
  End Property
#End Region

 End Class
End Namespace

