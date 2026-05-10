using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color = TCTOS.Abstractions.Data.Color;

namespace TCTOS.Operations;

public static class ColorModulator
{
    public static Image<Rgba32> ModulateImage(Image<Rgba32> image, Color color)
    {
        var clonedImage = image.Clone();
        clonedImage.Mutate(img => img.Grayscale());
        clonedImage.ProcessPixelRows(accessor =>
        {
            for (var y = 0; y < accessor.Height; y++)
            {
                var pixelRow = accessor.GetRowSpan(y);
                for (var x = 0; x < pixelRow.Length; x++)
                {
                    pixelRow[x].R = (byte)(pixelRow[x].R * (color.Red / 255));
                    pixelRow[x].G = (byte)(pixelRow[x].G * (color.Green / 255));
                    pixelRow[x].B = (byte)(pixelRow[x].B * (color.Blue / 255));
                }
            }
        });
        return clonedImage;
    }
}