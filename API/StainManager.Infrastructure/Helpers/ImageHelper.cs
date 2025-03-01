using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace StainManager.Infrastructure.Helpers;

public static class ImageHelper
{
    public async static Task<MemoryStream> CreateThumbnail(byte[] imageBytes)
    {
        using var originalImage = Image.Load(imageBytes);
        var thumbnailImage = ResizeImage(originalImage, 100, 100); // Resize to 100x100 pixels

        var thumbnailStream = new MemoryStream();
        await thumbnailImage.SaveAsync(thumbnailStream, new JpegEncoder());
        thumbnailStream.Position = 0;
        
        return thumbnailStream;
    }
    
    private static Image ResizeImage(Image image, int width, int height)
    {
        image.Mutate(x => x.Resize(width, height));
        return image;
    }
}