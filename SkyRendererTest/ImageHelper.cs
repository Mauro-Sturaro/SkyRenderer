using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace SkyRendererTest
{
    /// <summary>
    /// Provides utility methods for image manipulation and conversion
    /// </summary>
    public static class ImageHelper
    {
        /// <summary>
        /// Adds a reticle (crosshair) to the center of the image
        /// </summary>
        /// <param name="image">The image to add the reticle to</param>
        public static void AddReticle(Image<Rgba32> image)
        {
            image.Mutate(x => x
                // Horizontal line
                .DrawLine(SixLabors.ImageSharp.Color.Red, 1f,
                    new SixLabors.ImageSharp.PointF(0, image.Height / 2),
                    new SixLabors.ImageSharp.PointF(image.Width, image.Height / 2))
                // Vertical line
                .DrawLine(SixLabors.ImageSharp.Color.Red, 1f,
                    new SixLabors.ImageSharp.PointF(image.Width / 2, 0),
                    new SixLabors.ImageSharp.PointF(image.Width / 2, image.Height))
                );
        }

        /// <summary>
        /// Converts an ImageSharp RGBA32 image to a System.Drawing Bitmap
        /// </summary>
        /// <param name="image">The ImageSharp image to convert</param>
        /// <returns>A System.Drawing Bitmap representation of the image</returns>
        public static System.Drawing.Bitmap ConvertToBitmap(Image<SixLabors.ImageSharp.PixelFormats.Rgba32> image)
        {
            System.Drawing.Bitmap bitmap;
            using (var memoryStream = new MemoryStream())
            {
                // Save ImageSharp image to memory stream as BMP
                image.Save(memoryStream, new BmpEncoder());
                memoryStream.Seek(0, SeekOrigin.Begin); // Reset stream for reading

                // Create Bitmap from memory stream
                bitmap = new System.Drawing.Bitmap(memoryStream);
            }
            return bitmap;
        }

        /// <summary>
        /// Converts an ImageSharp RGBA32 image to a JPEG System.Drawing.Image
        /// </summary>
        /// <param name="image">The ImageSharp image to convert</param>
        /// <returns>A System.Drawing.Image in JPEG format</returns>
        public static System.Drawing.Image ConvertToJpg(Image<SixLabors.ImageSharp.PixelFormats.Rgba32> image)
        {
            using (var memoryStream = new MemoryStream())
            {
                // Save ImageSharp image to memory stream as JPEG
                image.Save(memoryStream, new JpegEncoder());
                memoryStream.Seek(0, SeekOrigin.Begin); // Reset stream for reading

                // Create Image from memory stream
                return System.Drawing.Image.FromStream(memoryStream);
            }
        }
    }
}