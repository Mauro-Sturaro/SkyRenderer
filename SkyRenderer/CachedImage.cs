using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Threading.Tasks;

namespace SkyRenderer
{
    /// <summary>
    /// Provides caching functionality for astronomical images.
    /// Stores the last generated image to avoid unnecessary regeneration when parameters haven't changed.
    /// </summary>
    public class CachedImage : IImageService
    {
        public double RightAscension { get; set; }
        public double Declination { get; set; }
        public double ImageScale { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public double RotationAngle { get; set; }

        private Image<Rgba32>? image;
        private IImageService? baseSvc;

        /// <summary>
        /// Creates a new cached image service wrapping another image service
        /// </summary>
        /// <param name="imageService">The base image service to cache results from</param>
        public CachedImage(IImageService imageService)
        {
            RightAscension = imageService.RightAscension;
            Declination = imageService.Declination;
            ImageScale = imageService.ImageScale;
            Height = imageService.Height;
            Width = imageService.Width;
            RotationAngle = imageService.RotationAngle;
            baseSvc = imageService;
        }

        /// <summary>
        /// Checks if the cached image is valid for the given parameters
        /// </summary>
        /// <param name="imageService">The image service to compare parameters with</param>
        /// <returns>true if the cached image matches all parameters, false otherwise</returns>
        public bool IsValid(IImageService imageService)
        {
            if (imageService == null || image == null)
                return false;

            return imageService.RightAscension == RightAscension
                && imageService.Declination == Declination
                && imageService.ImageScale == ImageScale
                && imageService.Height == Height
                && imageService.Width == Width
                && imageService.RotationAngle == RotationAngle;
        }

        /// <summary>
        /// Gets the cached image if valid, or generates a new one using the base service
        /// </summary>
        /// <returns>The astronomical image</returns>
        public async Task<Image<Rgba32>> GetImageAsync()
        {
            if (image != null)
                return image;
            else
            {
                image = await baseSvc!.GetImageAsync();
                baseSvc = null; // Release the base service after first use
                return image;
            }
        }
    }
}