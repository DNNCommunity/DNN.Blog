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
Namespace Rss

 ''' <summary>
 ''' Support class to allow encoded writing
 ''' </summary>
 ''' <remarks></remarks>
 ''' <history>
 '''		[pdonker]	12/14/2009	created
 ''' </history>
 Public Class StringWriterWithEncoding
  Inherits IO.StringWriter

  Private _encoding As System.Text.Encoding

  Public Sub New(builder As StringBuilder, encoding As System.Text.Encoding)
   MyBase.New(builder)
   _encoding = encoding
  End Sub

  Public Overrides ReadOnly Property Encoding() As System.Text.Encoding
   Get
    Return _encoding
   End Get
  End Property

 End Class

End Namespace