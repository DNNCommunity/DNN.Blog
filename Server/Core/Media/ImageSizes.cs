using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetNuke.Modules.Blog.Core.Media
{
  /// <summary>Fixed set of variants the image endpoint will generate; anything else is snapped to these.</summary>
  public static class ImageSizes
  {
    public static readonly int[] Widths = { 320, 480, 640, 800, 1024, 1200, 1600 };

    // Crop sizes are used for thumbnails and social images
    public static readonly int[][] CropSizes =
    {
      new[] { 100, 100 }, new[] { 150, 150 }, new[] { 300, 200 },
      new[] { 600, 315 }, new[] { 1200, 630 }, new[] { 1200, 675 }
    };

    public const int DefaultWidth = 800;

    /// <summary>Smallest allowed width that is at least the requested one; the largest otherwise. 0 means original.</summary>
    public static int SnapWidth(int requested)
    {
      if (requested <= 0) return 0;
      foreach (var w in Widths)
        if (w >= requested) return w;
      return Widths[Widths.Length - 1];
    }

    /// <summary>Nearest allowed crop size, or null if the request is not a crop.</summary>
    public static int[] SnapCrop(int width, int height)
    {
      if (width <= 0 || height <= 0) return null;
      return CropSizes
        .OrderBy(c => Math.Abs(c[0] - width) + Math.Abs(c[1] - height))
        .First();
    }

    /// <summary>The widths worth offering for an image of the given size.</summary>
    public static IList<int> WidthsFor(int originalWidth)
    {
      return Widths.Where(w => w <= originalWidth).ToList();
    }
  }
}
