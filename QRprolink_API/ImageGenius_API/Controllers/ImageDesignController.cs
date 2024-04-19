using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;


namespace ImageGenius_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageDesignController : Controller
    {
        [HttpPost("imgconvert")]
        public IActionResult ImgConvert(string inputPath, string outputPath, int targetWidth, int targetHeight)
        {
            string pathExtension = Path.GetExtension(inputPath).ToLower();
            var supportedExtensions = new[] { ".bmp", ".jpg", ".jpeg", ".png", ".gif", ".webp", ".pbm", ".tiff", ".tga" };

            if (supportedExtensions.Contains(pathExtension))
            {
                using (var image = Image.Load(inputPath))
                {
                    image.Mutate(x => x
                        .Resize(new ResizeOptions
                        {
                            Mode = ResizeMode.BoxPad,
                            Size = new Size(targetWidth, targetHeight)
                        }));
                    if (pathExtension == ".webp") image.Save(outputPath, new SixLabors.ImageSharp.Formats.Webp.WebpEncoder());
                    if (pathExtension == ".bmp") image.Save(outputPath, new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder());
                    if (pathExtension == ".jpeg") image.Save(outputPath, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder());
                    if (pathExtension == ".jpg") image.Save(outputPath, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder());
                    if (pathExtension == ".png") image.Save(outputPath, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
                    if (pathExtension == ".gif") image.Save(outputPath, new SixLabors.ImageSharp.Formats.Gif.GifEncoder());
                    if (pathExtension == ".pbm") image.Save(outputPath, new SixLabors.ImageSharp.Formats.Pbm.PbmEncoder());
                    if (pathExtension == ".tiff") image.Save(outputPath, new SixLabors.ImageSharp.Formats.Tiff.TiffEncoder());
                    if (pathExtension == ".tga") image.Save(outputPath, new SixLabors.ImageSharp.Formats.Tga.TgaEncoder());
                }
                // Otomatik indirme için dosya yolunu belirtin
                return File(outputPath, "application/octet-stream", $"converted_image{pathExtension}");
            }
            else
            {
                return BadRequest("File type not supported");
            }
        }



    }
}
