using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace DotNetNuke.Modules.Blog.Core.Media
{
  public class ImageFormatInfo
  {
    public string Extension { get; set; }
    public string ContentType { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
  }

  public interface IImageResizer
  {
    /// <summary>Validates the stream is a supported image and returns its display size (EXIF orientation applied). Null if not valid.</summary>
    ImageFormatInfo Inspect(Stream stream);

    /// <summary>Writes a resized copy. For crop the image is scaled to cover and center-cropped.</summary>
    void Resize(string sourcePath, string destPath, int width, int height, bool crop);
  }

  public static class ImageFormats
  {
    public const long MaxPixels = 100L * 1000 * 1000;

    /// <summary>Detects jpg/png/gif from the file signature. The client supplied name and type are never trusted.</summary>
    public static ImageFormatInfo DetectFromHeader(byte[] h)
    {
      if (h == null || h.Length < 4) return null;
      if (h[0] == 0xFF && h[1] == 0xD8 && h[2] == 0xFF)
        return new ImageFormatInfo { Extension = ".jpg", ContentType = "image/jpeg" };
      if (h[0] == 0x89 && h[1] == 0x50 && h[2] == 0x4E && h[3] == 0x47)
        return new ImageFormatInfo { Extension = ".png", ContentType = "image/png" };
      if (h[0] == 0x47 && h[1] == 0x49 && h[2] == 0x46 && h[3] == 0x38)
        return new ImageFormatInfo { Extension = ".gif", ContentType = "image/gif" };
      return null;
    }

    public static string ContentTypeFromExtension(string ext)
    {
      switch ((ext ?? "").ToLowerInvariant())
      {
        case ".png": return "image/png";
        case ".gif": return "image/gif";
        default: return "image/jpeg";
      }
    }
  }

  /// <summary>Resizer on System.Drawing so no extra dependency is needed. Output is jpg/png/gif only.</summary>
  public class SystemDrawingImageResizer : IImageResizer
  {
    private const int OrientationTag = 0x0112;

    public ImageFormatInfo Inspect(Stream stream)
    {
      var header = new byte[8];
      var read = stream.Read(header, 0, header.Length);
      stream.Position = 0;
      var info = ImageFormats.DetectFromHeader(read < 4 ? null : header);
      if (info == null) return null;
      try
      {
        using (var img = Image.FromStream(stream, false, false))
        {
          long pixels = (long)img.Width * img.Height;
          if (img.Width < 1 || img.Height < 1 || pixels > ImageFormats.MaxPixels) return null;
          var swap = NeedsSwap(GetOrientation(img));
          info.Width = swap ? img.Height : img.Width;
          info.Height = swap ? img.Width : img.Height;
        }
      }
      catch (ArgumentException)
      {
        return null; // GDI+ could not decode it
      }
      finally
      {
        stream.Position = 0;
      }
      return info;
    }

    public void Resize(string sourcePath, string destPath, int width, int height, bool crop)
    {
      using (var src = Image.FromFile(sourcePath))
      {
        var format = src.RawFormat;
        var orientation = GetOrientation(src);
        using (var oriented = new Bitmap(src))
        {
          ApplyOrientation(oriented, orientation);
          int sw = oriented.Width, sh = oriented.Height;
          int tw, th;
          Rectangle srcRect;
          if (crop && width > 0 && height > 0)
          {
            tw = width;
            th = height;
            double scale = Math.Max(tw / (double)sw, th / (double)sh);
            int cw = Math.Min(sw, (int)Math.Round(tw / scale));
            int ch = Math.Min(sh, (int)Math.Round(th / scale));
            srcRect = new Rectangle((sw - cw) / 2, (sh - ch) / 2, cw, ch);
          }
          else
          {
            tw = Math.Min(width > 0 ? width : sw, sw);
            th = (int)Math.Max(1, Math.Round(sh * (tw / (double)sw)));
            srcRect = new Rectangle(0, 0, sw, sh);
          }

          using (var dest = new Bitmap(tw, th, PixelFormat.Format32bppArgb))
          {
            dest.SetResolution(96, 96);
            using (var g = Graphics.FromImage(dest))
            {
              g.CompositingQuality = CompositingQuality.HighQuality;
              g.InterpolationMode = InterpolationMode.HighQualityBicubic;
              g.SmoothingMode = SmoothingMode.HighQuality;
              g.PixelOffsetMode = PixelOffsetMode.HighQuality;
              if (!IsAlphaFormat(format)) g.Clear(Color.White);
              g.DrawImage(oriented, new Rectangle(0, 0, tw, th), srcRect, GraphicsUnit.Pixel);
            }
            Directory.CreateDirectory(Path.GetDirectoryName(destPath));
            Save(dest, format, destPath);
          }
        }
      }
    }

    private static bool IsAlphaFormat(ImageFormat f)
    {
      return f.Equals(ImageFormat.Png) || f.Equals(ImageFormat.Gif);
    }

    private static void Save(Bitmap bmp, ImageFormat format, string path)
    {
      if (format.Equals(ImageFormat.Png))
      {
        bmp.Save(path, ImageFormat.Png);
      }
      else if (format.Equals(ImageFormat.Gif))
      {
        bmp.Save(path, ImageFormat.Gif);
      }
      else
      {
        var codec = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
        using (var p = new EncoderParameters(1))
        {
          p.Param[0] = new EncoderParameter(Encoder.Quality, 85L);
          bmp.Save(path, codec, p);
        }
      }
    }

    private static int GetOrientation(Image img)
    {
      var item = img.PropertyItems.FirstOrDefault(p => p.Id == OrientationTag);
      return item == null || item.Value == null || item.Value.Length < 2 ? 1 : BitConverter.ToUInt16(item.Value, 0);
    }

    private static bool NeedsSwap(int orientation)
    {
      return orientation >= 5 && orientation <= 8;
    }

    private static void ApplyOrientation(Bitmap bmp, int orientation)
    {
      switch (orientation)
      {
        case 2: bmp.RotateFlip(RotateFlipType.RotateNoneFlipX); break;
        case 3: bmp.RotateFlip(RotateFlipType.Rotate180FlipNone); break;
        case 4: bmp.RotateFlip(RotateFlipType.Rotate180FlipX); break;
        case 5: bmp.RotateFlip(RotateFlipType.Rotate90FlipX); break;
        case 6: bmp.RotateFlip(RotateFlipType.Rotate90FlipNone); break;
        case 7: bmp.RotateFlip(RotateFlipType.Rotate270FlipX); break;
        case 8: bmp.RotateFlip(RotateFlipType.Rotate270FlipNone); break;
      }
    }
  }
}
