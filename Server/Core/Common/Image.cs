using System;
using System.Drawing;
// 
// DNN Connect - http://dnn-connect.org
// Copyright (c) 2015
// by DNN Connect
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
// 

using System.Drawing.Imaging;

namespace DotNetNuke.Modules.Blog.Core.Common
{
  public class Image
  {

    public const string glbJpegFileType = ".jpg";
    public const string glbPngFileType = ".png";
    public const string glbGifFileType = ".gif";

    private Bitmap _thisImage;
    private ImageFormat _imgFormat;
    private float _imgRatio;
    private string _extension = "";
    private string _newExtension = "";
    private string _originalFile = "";

    public bool IsValidExtension { get; set; } = true;
    public string MimeType { get; set; } = "image/jpeg";
    public int OriginalWidth { get; set; }
    public int OriginalHeight { get; set; }
    public double Scale { get; set; }
    public int NewHeight { get; set; }
    public int NewWidth { get; set; }

    public Image(string originalFilePath)
    {
      _originalFile = originalFilePath;
      _extension = System.IO.Path.GetExtension(originalFilePath).ToLower();
      if ((_extension ?? "") == glbGifFileType | (_extension ?? "") == glbJpegFileType | (_extension ?? "") == glbPngFileType)
      {
        _thisImage = new Bitmap(originalFilePath);
        _imgFormat = _thisImage.RawFormat;
        OriginalWidth = _thisImage.Width;
        OriginalHeight = _thisImage.Height;
        _imgRatio = Convert.ToSingle(OriginalHeight / (double)OriginalWidth);
        switch (_extension ?? "")
        {
          case glbGifFileType:
            {
              MimeType = "image/gif";
              break;
            }
          case glbPngFileType:
            {
              MimeType = "image/png";
              break;
            }
        }
      }
      else
      {
        IsValidExtension = false;
      }
    }

    public string ResizeImage(int maxWidth, int maxHeight, bool crop)
    {

      switch (_extension ?? "")
      {
        case var @case when @case == (glbGifFileType.ToLower() ?? ""):
          {
            _newExtension = glbGifFileType;
            return ResizeImage(maxWidth, maxHeight, crop, ImageFormat.Gif);
          }
        case var case1 when case1 == (glbPngFileType.ToLower() ?? ""):
          {
            _newExtension = glbPngFileType;
            return ResizeImage(maxWidth, maxHeight, crop, ImageFormat.Png); // jpg
          }

        default:
          {
            _newExtension = glbJpegFileType;
            return ResizeImage(maxWidth, maxHeight, crop, ImageFormat.Jpeg);
          }
      }

    }

    public string ResizeImage(int maxWidth, int maxHeight, bool crop, ImageFormat format)
    {

      try
      {

        Bitmap backBuffer = null;
        Graphics backBufferGraphics = null;

        if (maxHeight == -1)
        {

          NewHeight = (int)Math.Round(Math.Floor((double)(_imgRatio * maxWidth)));
          Scale = maxWidth / (double)OriginalWidth;
          backBuffer = new Bitmap(maxWidth, NewHeight, PixelFormat.Format32bppRgb);
          backBufferGraphics = Graphics.FromImage(backBuffer);
          backBufferGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
          backBufferGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
          backBufferGraphics.DrawImage(_thisImage, 0, 0, maxWidth, NewHeight);
        }

        else if (maxWidth == -1)
        {

          NewWidth = (int)Math.Round(Math.Floor((double)(maxHeight / _imgRatio)));
          Scale = maxHeight / (double)OriginalHeight;
          backBuffer = new Bitmap(NewWidth, maxHeight, PixelFormat.Format32bppRgb);
          backBufferGraphics = Graphics.FromImage(backBuffer);
          backBufferGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
          backBufferGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
          backBufferGraphics.DrawImage(_thisImage, 0, 0, NewWidth, maxHeight);
        }

        else if (crop)
        {

          bool WidthOverflow = false;
          Scale = maxWidth / (double)OriginalWidth;
          if (maxHeight / (double)OriginalHeight > Scale)
          {
            Scale = maxHeight / (double)OriginalHeight;
            WidthOverflow = true;
          }
          NewHeight = maxHeight;
          NewWidth = maxWidth;
          backBuffer = new Bitmap(NewWidth, NewHeight, PixelFormat.Format32bppRgb);
          backBufferGraphics = Graphics.FromImage(backBuffer);
          backBufferGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
          backBufferGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
          if (WidthOverflow)
          {
            int Overflow = Convert.ToInt32((Scale * OriginalWidth - maxWidth) / 2d);
            backBufferGraphics.DrawImage(_thisImage, -1 * Overflow, 0, Convert.ToInt32(OriginalWidth * Scale), NewHeight);
          }
          else
          {
            int Overflow = Convert.ToInt32((Scale * OriginalHeight - maxHeight) / 2d);
            backBufferGraphics.DrawImage(_thisImage, 0, -1 * Overflow, NewWidth, Convert.ToInt32(OriginalHeight * Scale));
          }
        }

        else
        {

          Scale = maxWidth / (double)OriginalWidth;
          if (maxHeight / (double)OriginalHeight < Scale)
          {
            Scale = maxHeight / (double)OriginalHeight;
          }
          if (Scale > 1d)
          {
            Scale = 1d;
            NewHeight = OriginalHeight;
            NewWidth = OriginalWidth;
          }
          else
          {
            NewHeight = Convert.ToInt32(OriginalHeight * Scale);
            NewWidth = Convert.ToInt32(OriginalWidth * Scale);
          }
          backBuffer = new Bitmap(NewWidth, NewHeight, PixelFormat.Format32bppRgb);
          backBufferGraphics = Graphics.FromImage(backBuffer);
          backBufferGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
          backBufferGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
          backBufferGraphics.DrawImage(_thisImage, 0, 0, NewWidth, NewHeight);

        }

        string newFilePath = _originalFile.Substring(0, _originalFile.LastIndexOf(".")) + string.Format("-{0}-{1}-{2}", maxWidth, maxHeight, crop) + _newExtension;
        backBuffer.Save(newFilePath, _imgFormat);
        return newFilePath;
      }

      catch (Exception ex)
      {

        throw new Exception(ex.Message);

      }

    }

    public void Dispose()
    {
      try
      {
        _thisImage.Dispose();
        _thisImage = null;
      }
      catch
      {
      }
    }


  }
}