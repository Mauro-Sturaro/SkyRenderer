using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace SkyRenderer
{
    /// <summary>
    /// Service for generating synthetic star field images using Tycho-2 catalog data
    /// </summary>
    public class ImageServiceTycho : IImageService
    {
        public double RightAscension { get; set; }
        public double Declination { get; set; }
        public double ImageScale { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public double RotationAngle { get; set; }

        // Extended rendering options
        /// <summary>
        /// Gets or sets whether to use Moffat profile for star rendering
        /// </summary>
        public bool UseMoffatProfile { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to render stars with color based on their B-V index
        /// </summary>
        public bool UseColor { get; set; } = true;

        /// <summary>
        /// Gets or sets the path to the Tycho-2 catalog data file
        /// </summary>
        public static string DataFile { get; set; }

        static ImageServiceTycho()
        {
            string location = Assembly.GetExecutingAssembly().Location;
            string? assemblyPath = Path.GetDirectoryName(location);
            DataFile = Path.Combine(assemblyPath!, "stars.parquet");
        }

        /// <summary>
        /// Generates a synthetic star field image based on the current settings
        /// </summary>
        /// <returns>An RGBA32 image of the star field</returns>
        public async Task<Image<Rgba32>> GetImageAsync()
        {
            var sw = Stopwatch.StartNew();

            var pic = new AstroPicture(Width, Height, RightAscension, Declination, ImageScale, RotationAngle);
            pic.UseMoffatProfile = UseMoffatProfile;
            pic.UseColor = UseColor;
            await LoadStarData(pic);
            var image = pic.GetImage();
            AddNoise(image);    // Add noise to enable plate solving with ASTAP

            sw.Stop();
            Trace.WriteLine($"Synthetic Image generated in {sw.ElapsedMilliseconds} ms.");

            return image;
        }

        /// <summary>
        /// Loads star data from the Tycho-2 catalog and adds them to the image
        /// </summary>
        private async Task LoadStarData(AstroPicture pic)
        {
            var fileReader = new TychoParquetReader(DataFile);

            var minRA = pic.SearchBoundingBox.RaMin;
            var maxRA = pic.SearchBoundingBox.RaMax;
            var minDEC = pic.SearchBoundingBox.DecMin;
            var maxDEC = pic.SearchBoundingBox.DecMax;
            var crossesRA360 = pic.SearchBoundingBox.CrossesRA360;

            var maxMagnitude = AstroPicture.GetMaxMagnitude(ImageScale);

            Debug.WriteLine("Begin star retrieval");
            Debug.WriteLine($"Coordinate range RA:[{minRA}; {maxRA}](cross={crossesRA360}), DEC:[{minDEC}; {maxDEC}]");

            int totalStars = 0;
            int validStars = 0;
            await foreach (var record in fileReader.ReadFilteredData(minRA, maxRA, minDEC, maxDEC))
            {
                totalStars++;

                if (record.Vmag <= maxMagnitude)
                {
                    validStars++;
                    pic.AddStar(record.RAdeg, record.DEdeg, record.Vmag, record.BV);
                }
            }
            Debug.WriteLine($"Star retrieval complete. Found {totalStars} stars, {validStars} inside magnitude limit.");
        }

        /// <summary>
        /// Adds random noise to the image
        /// </summary>
        /// <param name="image">The image to add noise to</param>
        /// <param name="noisePercentage">Noise intensity as a percentage (default 1.5%)</param>
        /// <param name="seed">Optional seed for reproducible noise</param>
        private static void AddNoise(Image<Rgba32> image, double noisePercentage = 1.5, int? seed = null)
        {
            Random random = seed.HasValue ? new Random(seed.Value) : new Random();

            // Convert percentage to 0-255 range
            int maxNoise = (int)(255 * (noisePercentage / 100.0));

            // Process each pixel
            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    var row = accessor.GetRowSpan(y);

                    for (int x = 0; x < row.Length; x++)
                    {
                        ref Rgba32 pixel = ref row[x];

                        // Generate noise for each RGB channel
                        int noiseR = random.Next(-maxNoise, maxNoise);
                        int noiseG = random.Next(-maxNoise, maxNoise);
                        int noiseB = random.Next(-maxNoise, maxNoise);

                        // Apply noise while keeping values in 0-255 range
                        pixel.R = (byte)Math.Clamp(pixel.R + noiseR, 0, 255);
                        pixel.G = (byte)Math.Clamp(pixel.G + noiseG, 0, 255);
                        pixel.B = (byte)Math.Clamp(pixel.B + noiseB, 0, 255);
                        // Alpha channel remains unchanged
                    }
                }
            });
        }
    }
}