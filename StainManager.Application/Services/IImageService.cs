using StainManager.Domain.Common;

namespace StainManager.Application.Services;

public interface IImageService
{
    Task<Result<ImageUploadResult>> UploadImageAsync(
        string directory,
        Guid id,
        string imageContent,
        string fileName = "",
        string mediaType = "image/jpg");

    Task<Result<ImageMoveResult>> MoveImagesAsync(
        string? fullImageLocation,
        string? thumbnailImageLocation,
        string directory,
        int id);
}