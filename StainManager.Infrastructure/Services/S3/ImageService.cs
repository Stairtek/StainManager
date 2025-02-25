using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

using StainManager.Application.Common.Models;
using StainManager.Application.Services;
using StainManager.Domain.Common;
using StainManager.Infrastructure.Helpers;

namespace StainManager.Infrastructure.Services.S3;

public class ImageService(
    IAmazonS3 s3Client)
    : IImageService
{
    private static readonly RegionEndpoint BucketRegion = RegionEndpoint.USEast2;
    private const string BucketName = "mcampbell-aws";
    private const string MainDirectory = "stain-manager";

    private static string FileKeyPrefixURL
        => $"https://{BucketName}.s3.{BucketRegion.SystemName}.{BucketRegion.PartitionDnsSuffix}/";

    public async Task<Result<ImageUploadResult>> UploadImageAsync(
        string directory,
        Guid id,
        string imageContent)
    {
        var imageBytes = Convert.FromBase64String(imageContent);
        var fileKey = $"{MainDirectory}/{directory}/{id}.jpg";
        
        var uploadResult = await UploadToS3Async(fileKey, imageBytes, "image/jpeg");
        if (!uploadResult.Success)
            return Result.Fail<ImageUploadResult>("Failed to upload image");

        var thumbnailKey = $"{MainDirectory}/{directory}/{id}_thumbnail.jpg";
        var thumbnail = await ImageHelper.CreateThumbnail(imageBytes);
        
        var thumbnailUploadResult = await UploadToS3Async(
            thumbnailKey, 
            thumbnail.ToArray(), 
            "image/jpeg");
        
        if (!thumbnailUploadResult.Success)
            return Result.Fail<ImageUploadResult>("Failed to upload thumbnail image");

        var result = new ImageUploadResult
        {
            FullImageURL = $"{FileKeyPrefixURL}{fileKey}",
            ThumbnailImageURL = $"{FileKeyPrefixURL}{thumbnailKey}"
        };

        return Result.Ok(result);
    }
    
    private async Task<Result> UploadToS3Async(string fileKey, byte[] fileBytes, string contentType)
    {
        using var memoryStream = new MemoryStream(fileBytes);
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = BucketName,
            Key = fileKey,
            ContentType = contentType,
            InputStream = memoryStream
        };

        var response = await s3Client.PutObjectAsync(putObjectRequest);
        return response.HttpStatusCode == System.Net.HttpStatusCode.OK
            ? Result.Ok()
            : Result.Fail("Failed to upload to S3");
    }

    public async Task<Result<ImageMoveResult>> MoveImagesAsync(
        string? fullImageLocation,
        string? thumbnailImageLocation,
        string directory,
        int id)
    {
        if (fullImageLocation is null || thumbnailImageLocation is null)
            return Result.Fail<ImageMoveResult>("Image Locations cannot be null");
        
        var moveFullImageResult = await MoveTempImageAsync(
            fullImageLocation,
            directory,
            id);
        
        if (moveFullImageResult.Failure)
            return Result.Fail<ImageMoveResult>(moveFullImageResult.Error);
        
        var moveThumbnailImageResult = await MoveTempImageAsync(
            thumbnailImageLocation,
            directory,
            id,
            true);

        if (moveThumbnailImageResult.Failure)
            return Result.Fail<ImageMoveResult>(moveThumbnailImageResult.Error);
        
        var result = new ImageMoveResult
        {
            FullImageLocation = moveFullImageResult.Value ?? "",
            ThumbnailImageLocation = moveThumbnailImageResult.Value ?? ""
        };
        
        return Result.Ok(result);
    }

    private async Task<Result<string>> MoveTempImageAsync(
        string? tempImageURL,
        string directory,
        int id,
        bool isThumbnail = false)
    {
        try
        {
            if (tempImageURL is null)
                return Result.Fail<string>("Temporary image URL is null");
            
            var tempImageFileKey = tempImageURL.Replace(FileKeyPrefixURL, string.Empty);
            var newFileKey = $"{MainDirectory}/{directory}/{id}.jpg";
            
            if (isThumbnail)
                newFileKey = $"{MainDirectory}/{directory}/{id}_thumbnail.jpg";
            
            var copyRequest = new CopyObjectRequest
            {
                SourceBucket = BucketName,
                SourceKey = tempImageFileKey,
                DestinationBucket = BucketName,
                DestinationKey = newFileKey
            };
            
            var copyResponse = await s3Client.CopyObjectAsync(copyRequest);
            
            if (copyResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
                return Result.Fail<string>("Failed to copy image to new location");
            
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = BucketName,
                Key = tempImageFileKey
            };
            
            var deleteResponse = await s3Client.DeleteObjectAsync(deleteRequest);
            
            if (deleteResponse.HttpStatusCode != System.Net.HttpStatusCode.NoContent)
                return Result.Fail<string>("Failed to delete temporary image");
            
            return Result.Ok($"{FileKeyPrefixURL}{newFileKey}");
        }
        catch (Exception ex)
        {
            return Result.Fail<string>($"Error moving image: {ex.Message}");
        }
    }
}