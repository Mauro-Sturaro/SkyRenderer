using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Threading.Tasks;

namespace SkyRenderer
{
    public interface IImageService
    {
        double RightAscension { get; set; }
        double Declination { get; set; }
        double ImageScale { get; set; }
        int Height { get; set; }
        int Width { get; set; }
        double RotationAngle { get; set; }
        Task<Image<Rgba32>> GetImageAsync();
    }

}