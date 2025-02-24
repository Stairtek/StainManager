using Amazon.S3.Model;

namespace StainManager.Application.Services;

public interface IImageService
{
    Task<PutObjectResponse> UploadImageAsync(
        string directory,
        Guid id,
        string imageContent);

    Task<GetObjectResponse> GetImageAsync(
        string directory,
        Guid id);

    Task<DeleteObjectResponse> DeleteImageAsync(
        string directory,
        int id);
}