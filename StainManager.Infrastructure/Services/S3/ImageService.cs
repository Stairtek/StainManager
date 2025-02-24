using Amazon.S3;
using Amazon.S3.Model;
using StainManager.Application.Services;

namespace StainManager.Infrastructure.Services.S3;

public class ImageService(
    IAmazonS3 s3Client)
    : IImageService
{
    private const string BucketName = "mcampbell-aws";

    public async Task<PutObjectResponse> UploadImageAsync(
        string directory,
        Guid id,
        string imageContent)
    {
        var imageBytes = Convert.FromBase64String(imageContent);
        using var memoryStream = new MemoryStream(imageBytes);

        var putObjectRequest = new PutObjectRequest
        {
            BucketName = BucketName,
            Key = $"stain-manager/{directory}/{id}.jpg",
            ContentType = "image/jpeg",
            InputStream = memoryStream,
            Metadata =
            {
                ["x-amz-meta-originalname"] = "image.jpg",
                ["x-amz-meta-extension"] = ".jpg"
            }
        };

        return await s3Client.PutObjectAsync(putObjectRequest);
    }

    public Task<GetObjectResponse> GetImageAsync(
        string directory,
        Guid id)
    {
        var getObjectRequest = new GetObjectRequest
        {
            BucketName = BucketName,
            Key = $"stain-manager/{directory}/{id}"
        };

        return s3Client.GetObjectAsync(getObjectRequest);
    }

    public Task<DeleteObjectResponse> DeleteImageAsync(
        string directory,
        int id)
    {
        var deleteObjectRequest = new DeleteObjectRequest
        {
            BucketName = BucketName,
            Key = $"stain-manager/{directory}/{id}"
        };

        return s3Client.DeleteObjectAsync(deleteObjectRequest);
    }
}