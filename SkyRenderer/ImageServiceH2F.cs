using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace SkyRenderer
{
    /// <summary>
    /// Service to fetch astronomical images using the Hips2Fits service from CDS.
    /// Retrieves Digital Sky Survey (DSS) color images based on sky coordinates.
    /// </summary>
    public class ImageServiceH2F : IImageService
    {
        /// <summary>
        /// Image width in pixels
        /// </summary>
        public int Width { get; set; } = 800;

        /// <summary>
        /// Image height in pixels
        /// </summary>
        public int Height { get; set; } = 600;

        /// <summary>
        /// Image scale in arcseconds per pixel
        /// </summary>
        public double ImageScale { get; set; }

        /// <summary>
        /// Image rotation angle in degrees
        /// </summary>
        public double RotationAngle { get; set; } = 0.0;

        /// <summary>
        /// Right Ascension of image center in degrees (J2000)
        /// </summary>
        public double RightAscension { get; set; } = 83.6287;

        /// <summary>
        /// Declination of image center in degrees (J2000)
        /// </summary>
        public double Declination { get; set; } = 22.0147;

        // Service configuration
        private const string baseUrl = "https://alasky.cds.unistra.fr/hips-image-services/hips2fits";
        private const string Hips = "CDS/P/DSS2/color";
        private const string Projection = "TAN";
        private const string CoordSys = "icrs";

        // HttpClient is intended to be instantiated once per application
        private static readonly HttpClient httpClient = new();

        /// <summary>
        /// Retrieves an astronomical image from the Hips2Fits service
        /// </summary>
        /// <returns>Image in RGBA32 format</returns>
        /// <exception cref="Exception">Thrown when image download fails</exception>
        public async Task<Image<Rgba32>> GetImageAsync()
        {
            try
            {
                var fov = Width * ImageScale / 3600.0;

                var builder = new UriBuilder(baseUrl);
                var query = HttpUtility.ParseQueryString(string.Empty);
                query["hips"] = Hips;
                query["width"] = Width.ToString(CultureInfo.InvariantCulture);
                query["height"] = Height.ToString(CultureInfo.InvariantCulture);
                query["fov"] = fov.ToString(CultureInfo.InvariantCulture);
                query["projection"] = Projection;
                query["coordsys"] = CoordSys;
                query["rotation_angle"] = RotationAngle.ToString(CultureInfo.InvariantCulture);
                query["ra"] = RightAscension.ToString("0.0000", CultureInfo.InvariantCulture);
                query["dec"] = Declination.ToString("0.0000", CultureInfo.InvariantCulture);
                query["format"] = "jpg";

                builder.Query = query.ToString();
                var requestUrl = builder.ToString();

                Debug.WriteLine($"ImageServiceH2F - Request URL: {requestUrl}");

                var imageBytes = await httpClient.GetByteArrayAsync(requestUrl);
                Debug.WriteLine("Hips2fits image download complete.");

                using var memStream = new MemoryStream(imageBytes);
                return await Image.LoadAsync<Rgba32>(memStream);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.GetType().Name} - {ex.Message}");
                throw;
            }
        }
    }
}