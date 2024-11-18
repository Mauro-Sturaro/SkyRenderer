using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Diagnostics;

namespace SkyRenderer
{
    /// <summary>
    /// Represents a synthesized astronomical image with stars.
    /// Handles star plotting with different profiles and color options.
    /// </summary>
    internal class AstroPicture
    {
        /// <summary>
        /// Image scale in arcseconds per pixel
        /// </summary>
        readonly double scale;

        /// <summary>
        /// Image width in pixels
        /// </summary>
        readonly int width;

        /// <summary>
        /// Image height in pixels
        /// </summary>
        readonly int height;

        /// <summary>
        /// Gets or sets whether to use Moffat profile for star rendering.
        /// When false, uses simple circular profile.
        /// </summary>
        public bool UseMoffatProfile { get; set; }

        /// <summary>
        /// Gets the bounding box of the sky region covered by this image
        /// </summary>
        public BoundingBox SearchBoundingBox { get; private set; }

        /// <summary>
        /// Gets or sets whether to render stars with color based on their B-V index
        /// </summary>
        public bool UseColor { get; set; }

        private readonly Image<Rgba32> image;
        private readonly CoordinateConverter coordinateConverter;

        /// <summary>
        /// Creates a new astronomical image with the specified parameters
        /// </summary>
        /// <param name="width">Image width in pixels</param>
        /// <param name="height">Image height in pixels</param>
        /// <param name="raCenter">Right Ascension of image center in degrees</param>
        /// <param name="decCenter">Declination of image center in degrees</param>
        /// <param name="scale">Image scale in arcseconds per pixel</param>
        /// <param name="rotationDegrees">Image rotation in degrees</param>
        public AstroPicture(int width, int height, double raCenter, double decCenter, double scale, double rotationDegrees = 0)
        {
            this.width = width;
            this.height = height;
            this.scale = scale;
            image = new Image<Rgba32>(width, height, new Color(new Rgba32(20, 20, 20)));

            SearchBoundingBox = SkyImageBounds.CalculateBounds(raCenter, decCenter, scale, width, height, rotationDegrees);
            coordinateConverter = new CoordinateConverter(raCenter, decCenter, width, height, scale, rotationDegrees);
        }

        /// <summary>
        /// Adds a star to the image at the specified coordinates
        /// </summary>
        /// <param name="ra">Right Ascension in degrees</param>
        /// <param name="dec">Declination in degrees</param>
        /// <param name="mag">Star magnitude</param>
        /// <param name="bv">B-V color index</param>
        public void AddStar(double ra, double dec, double mag, double bv)
        {
            (var starX, var starY) = coordinateConverter.ConvertRaDecToXY(ra, dec);

            if (double.IsNaN(starX) || double.IsNaN(starY) || starX < 0 || starX > width || starY < 0 || starY > height)
            {
                return;
            }
            var color = Color.White;
            if (UseColor)
                color = GetStarColor(bv);

            if (UseMoffatProfile)
                DrawStarMoffat((float)starX, (float)starY, (float)mag, color);
            else
                DrawStarSimple(mag, starX, starY, color);
        }

        /// <summary>
        /// Calculates star color based on B-V color index
        /// </summary>
        /// <param name="bv">B-V color index (typically ranges from -0.4 to +2.0)</param>
        /// <returns>RGB color representing the star's temperature</returns>
        private Color GetStarColor(double bv)
        {
            // B-V typically ranges from -0.4 (blue) to +2.0 (red)
            double t = (bv + 0.4) / 2.4; // normalize to 0-1
            t = Math.Max(0, Math.Min(1, t));   // clamp to 0-1

            // Balanced intensity values
            const int maxIntensity = 255;    // maximum brightness
            const int midIntensity = 220;    // average intensity for natural white
            const int minIntensity = 160;    // minimum to maintain visibility

            if (t < 0.4)
            {
                // Blue-white to white (t from 0 to 0.4)
                int blue = maxIntensity;
                int green = minIntensity + (int)((midIntensity - minIntensity) * (t / 0.4));
                int red = (int)(green * 0.9);  // slightly reduced red for blue stars
                return Color.FromRgb((byte)red, (byte)green, (byte)blue);
            }
            else
            {
                // White to yellow-red (t from 0.4 to 1.0)
                t = (t - 0.4) / 0.6;
                int red = maxIntensity;
                int green = midIntensity - (int)((midIntensity - minIntensity) * t);
                int blue = (int)(green * 0.85); // reduced blue for red stars
                return Color.FromRgb((byte)red, (byte)green, (byte)blue);
            }
        }

        /// <summary>
        /// Gets the generated image
        /// </summary>
        public Image<Rgba32> GetImage()
        {
            return image;
        }

        /// <summary>
        /// Draws a star using a simple circular profile
        /// </summary>
        private void DrawStarSimple(double magnitude, double centerX, double centerY, Color color)
        {
            var radius = GetStarRadius(magnitude, scale);
            var polygon = new EllipsePolygon((float)centerX, (float)centerY, (float)radius);
            image.Mutate(context => context.Fill(color, polygon));
        }

        public static readonly double SQRT_LOG2 = Math.Sqrt(Math.Log(2));

        /// <summary>
        /// Calculates the drawing radius for a star
        /// </summary>
        /// <param name="magnitude">Star magnitude</param>
        /// <param name="scale">Image scale in arcseconds per pixel</param>
        /// <returns>Radius in pixels</returns>
        private double GetStarRadius(double magnitude, double scale)
        {
            double baseFwhm = 125;
            double fwhm = baseFwhm / scale;

            double flux = Math.Pow(10, -0.3 * magnitude);
            double hlrBase = fwhm / (2 * SQRT_LOG2);
            double hlr = hlrBase * Math.Sqrt(flux);

            return hlr * 2.0 * Math.Pow(scale, 0.42);
        }

        /// <summary>
        /// Calculates the maximum magnitude that should be included in the image based on the scale
        /// </summary>
        /// <param name="scale">Image scale in arcseconds per pixel</param>
        /// <returns>Maximum magnitude to include</returns>
        public static double GetMaxMagnitude(double scale)
        {
            var maxMag = -1.8 * Math.Log(scale) + 17;
            Debug.WriteLine($"Max Magnitude = {maxMag}");
            return maxMag;
        }

        /// <summary>
        /// Draws a star using the Moffat profile for more realistic appearance
        /// </summary>
        private void DrawStarMoffat(float centerX, float centerY, float magnitude, Color color)
        {
            var modifiedMag = magnitude * 8 / 12 + 1.5; // empirical correction
            var modifiedScale = 0.56 * scale + 9.5; // empirical correction
            var starSize = GetStarRadius(modifiedMag, modifiedScale);

            // alpha: controls star width (typical values 2-5)
            float alpha = (float)starSize;
            // beta: controls brightness falloff (2.5 is typical for real stars)
            float beta = 2.5f;

            // Calculate influence radius (beyond which intensity is negligible)
            // Use 3*alpha as a reasonable approximation to limit calculations
            int radius = (int)(5 * alpha);

            // Determine area to process
            int startX = Math.Max(0, (int)(centerX - radius));
            int endX = Math.Min(image.Width - 1, (int)(centerX + radius));
            int startY = Math.Max(0, (int)(centerY - radius));
            int endY = Math.Min(image.Height - 1, (int)(centerY + radius));

            // Convert magnitude to amplitude (brighter stars have lower magnitude)
            float amplitude = (float)(starSize / 2.0);

            // Convert to Rgba32 to access components
            Rgba32 rgba = color.ToPixel<Rgba32>();

            // Draw only pixels within star's influence
            for (int y = startY; y <= endY; y++)
            {
                for (int x = startX; x <= endX; x++)
                {
                    float dx = x - centerX;
                    float dy = y - centerY;
                    float distance2 = dx * dx + dy * dy;
                    // Moffat formula: A * (1 + (r/α)²)^(-β)
                    float intensity = amplitude * (float)Math.Pow(1 + distance2 / (alpha * alpha), -beta);

                    // Calculate new RGB values by multiplying intensity with each color component
                    byte newR = (byte)Math.Min(255, (intensity * rgba.R));
                    byte newG = (byte)Math.Min(255, (intensity * rgba.G));
                    byte newB = (byte)Math.Min(255, (intensity * rgba.B));

                    // Combine with existing pixel
                    var currentPixel = image[x, y];
                    newR = (byte)Math.Min(255, currentPixel.R + newR);
                    newG = (byte)Math.Min(255, currentPixel.G + newG);
                    newB = (byte)Math.Min(255, currentPixel.B + newB);

                    image[x, y] = new Rgba32(newR, newG, newB, 255);
                }
            }
        }
    }
}