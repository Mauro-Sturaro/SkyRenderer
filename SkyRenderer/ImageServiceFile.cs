using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using System.Text;
using System.Threading.Tasks;
using SkyRenderer;
using Parquet.Schema;
using System.Reflection;
using System.IO;

namespace SkyRenderer
{

    /// <summary>
    /// Simple implementation that return a fixed image from a file.
    /// </summary>
    public class ImageServiceFile : IImageService
    {
        public double RightAscension { get; set; }
        public double Declination { get; set; }
        public double ImageScale { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public double RotationAngle { get; set; }

        /// <summary>
        /// Image file fullpath.
        /// </summary>
        public string FilePath { get; set; }

        public ImageServiceFile(string filepath)
        {
            FilePath = filepath;
        }
        public ImageServiceFile()
        {
            string location = Assembly.GetExecutingAssembly().Location;
            string? assemblyPath = Path.GetDirectoryName(location);
            FilePath = Path.Combine(assemblyPath!, "DefaultImage.jpg");
        }

        public Task<Image<Rgba32>> GetImageAsync()
        {
            var image = Image.Load<Rgba32>(FilePath);
            return Task.FromResult(image);
        }
    }
}
