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

Imports System.Drawing.Imaging
Imports System.Drawing

Namespace Common
  Public Class Image

    Public Const glbJpegFileType As String = ".jpg"
    Public Const glbPngFileType As String = ".png"
    Public Const glbGifFileType As String = ".gif"

    Private _thisImage As Bitmap
    Private _imgFormat As ImageFormat
    Private _imgRatio As Single
    Private _extension As String = ""
    Private _newExtension As String = ""
    Private _originalFile As String = ""

    Public Property IsValidExtension As Boolean = True
    Public Property MimeType As String = "image/jpeg"
    Public Property OriginalWidth As Integer
    Public Property OriginalHeight As Integer
    Public Property Scale As Double
    Public Property NewHeight As Integer
    Public Property NewWidth As Integer

    Public Sub New(originalFilePath As String)
      _originalFile = originalFilePath
      _extension = IO.Path.GetExtension(originalFilePath).ToLower
      If _extension = glbGifFileType Or _extension = glbJpegFileType Or _extension = glbPngFileType Then
        _thisImage = New Bitmap(originalFilePath)
        _imgFormat = _thisImage.RawFormat
        OriginalWidth = _thisImage.Width
        OriginalHeight = _thisImage.Height
        _imgRatio = Convert.ToSingle(OriginalHeight / OriginalWidth)
        Select Case _extension
          Case glbGifFileType
            MimeType = "image/gif"
          Case glbPngFileType
            MimeType = "image/png"
        End Select
      Else
        IsValidExtension = False
      End If
    End Sub

    Public Function ResizeImage(maxWidth As Integer, maxHeight As Integer, crop As Boolean) As String

      Select Case _extension
        Case glbGifFileType.ToLower
          _newExtension = glbGifFileType
          Return ResizeImage(maxWidth, maxHeight, crop, ImageFormat.Gif)
        Case glbPngFileType.ToLower
          _newExtension = glbPngFileType
          Return ResizeImage(maxWidth, maxHeight, crop, ImageFormat.Png)
        Case Else ' jpg
          _newExtension = glbJpegFileType
          Return ResizeImage(maxWidth, maxHeight, crop, ImageFormat.Jpeg)
      End Select

    End Function

    Public Function ResizeImage(maxWidth As Integer, maxHeight As Integer, crop As Boolean, format As ImageFormat) As String

      Try

        Dim backBuffer As Bitmap = Nothing
        Dim backBufferGraphics As Graphics = Nothing

        If maxHeight = -1 Then

          NewHeight = CInt(Math.Floor(_imgRatio * maxWidth))
          Scale = maxWidth / OriginalWidth
          backBuffer = New Bitmap(maxWidth, NewHeight, Drawing.Imaging.PixelFormat.Format32bppRgb)
          backBufferGraphics = Graphics.FromImage(backBuffer)
          backBufferGraphics.InterpolationMode = Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
          backBufferGraphics.SmoothingMode = Drawing.Drawing2D.SmoothingMode.AntiAlias
          backBufferGraphics.DrawImage(_thisImage, 0, 0, maxWidth, NewHeight)

        ElseIf maxWidth = -1 Then

          NewWidth = CInt(Math.Floor(maxHeight / _imgRatio))
          Scale = maxHeight / OriginalHeight
          backBuffer = New Bitmap(NewWidth, maxHeight, Drawing.Imaging.PixelFormat.Format32bppRgb)
          backBufferGraphics = Graphics.FromImage(backBuffer)
          backBufferGraphics.InterpolationMode = Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
          backBufferGraphics.SmoothingMode = Drawing.Drawing2D.SmoothingMode.AntiAlias
          backBufferGraphics.DrawImage(_thisImage, 0, 0, NewWidth, maxHeight)

        ElseIf crop Then

          Dim WidthOverflow As Boolean = False
          Scale = maxWidth / OriginalWidth
          If (maxHeight / OriginalHeight) > Scale Then
            Scale = (maxHeight / OriginalHeight)
            WidthOverflow = True
          End If
          NewHeight = maxHeight
          NewWidth = maxWidth
          backBuffer = New Bitmap(NewWidth, NewHeight, Drawing.Imaging.PixelFormat.Format32bppRgb)
          backBufferGraphics = Graphics.FromImage(backBuffer)
          backBufferGraphics.InterpolationMode = Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
          backBufferGraphics.SmoothingMode = Drawing.Drawing2D.SmoothingMode.AntiAlias
          If WidthOverflow Then
            Dim Overflow As Integer = Convert.ToInt32(((Scale * OriginalWidth) - maxWidth) / 2)
            backBufferGraphics.DrawImage(_thisImage, -1 * Overflow, 0, Convert.ToInt32(OriginalWidth * Scale), NewHeight)
          Else
            Dim Overflow As Integer = Convert.ToInt32(((Scale * OriginalHeight) - maxHeight) / 2)
            backBufferGraphics.DrawImage(_thisImage, 0, -1 * Overflow, NewWidth, Convert.ToInt32(OriginalHeight * Scale))
          End If

        Else

          Scale = maxWidth / OriginalWidth
          If (maxHeight / OriginalHeight) < Scale Then
            Scale = (maxHeight / OriginalHeight)
          End If
          If Scale > 1 Then
            Scale = 1
            NewHeight = OriginalHeight
            NewWidth = OriginalWidth
          Else
            NewHeight = Convert.ToInt32(OriginalHeight * Scale)
            NewWidth = Convert.ToInt32(OriginalWidth * Scale)
          End If
          backBuffer = New Bitmap(NewWidth, NewHeight, Drawing.Imaging.PixelFormat.Format32bppRgb)
          backBufferGraphics = Graphics.FromImage(backBuffer)
          backBufferGraphics.InterpolationMode = Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
          backBufferGraphics.SmoothingMode = Drawing.Drawing2D.SmoothingMode.AntiAlias
          backBufferGraphics.DrawImage(_thisImage, 0, 0, NewWidth, NewHeight)

        End If

        Dim newFilePath As String = _originalFile.Substring(0, _originalFile.LastIndexOf(".")) & String.Format("-{0}-{1}-{2}", maxWidth, maxHeight, crop) & _newExtension
        backBuffer.Save(newFilePath, _imgFormat)
        Return newFilePath

      Catch ex As Exception

        Throw New Exception(ex.Message)

      End Try

    End Function

    Public Sub Dispose()
      Try
        _thisImage.Dispose()
        _thisImage = Nothing
      Catch
      End Try
    End Sub


  End Class
End Namespace
