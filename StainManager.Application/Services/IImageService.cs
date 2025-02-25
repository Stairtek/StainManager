using Amazon.S3.Model;
using StainManager.Domain.Common;

namespace StainManager.Application.Services;

public interface IImageService
{
    Task<Result<ImageUploadResult>> UploadImageAsync(
        string directory,
        Guid id,
        string imageContent);

    Task<Result<string>> MoveTempImageAsync(
        string tempImageFileKey,
        string directory,
        int id);
}