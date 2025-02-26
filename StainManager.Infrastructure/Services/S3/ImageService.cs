using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using StainManager.Application.Services;
using StainManager.Domain.Common;

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
        string imageContent,
        string fileName = "",
        string mediaType = "image/jpg")
    {
        try
        {
            var imageBytes = Convert.FromBase64String(imageContent);
            var fileExtension = GetFileExtensionFromMediaType(mediaType);
            var fileKey = $"{MainDirectory}/{directory}/{id}.{fileExtension}";
            
            var uploadResult = await UploadToS3Async(fileKey, imageBytes, fileName, mediaType);
            if (!uploadResult.Success)
                return Result.Fail<ImageUploadResult>("Failed to upload image");

            var thumbnailKey = $"{MainDirectory}/{directory}/{id}_thumbnail.{fileExtension}";
            var thumbnail = await ImageHelper.CreateThumbnail(imageBytes);
            
            var thumbnailUploadResult = await UploadToS3Async(
                thumbnailKey, 
                thumbnail.ToArray(), 
                fileName,
                mediaType);
            
            if (!thumbnailUploadResult.Success)
                return Result.Fail<ImageUploadResult>("Failed to upload thumbnail image");

            var result = new ImageUploadResult
            {
                FullImageURL = $"{FileKeyPrefixURL}{fileKey}",
                ThumbnailImageURL = $"{FileKeyPrefixURL}{thumbnailKey}"
            };

            return Result.Ok(result);
        }
        catch (ArgumentException argumentException)
        {
            return Result.Fail<ImageUploadResult>(argumentException.Message);        
        }
        catch (AmazonS3Exception s3Exception)
        {
            return Result.Fail<ImageUploadResult>($"S3 error: {s3Exception.Message}");
        }
        catch (Exception ex)
        {
            return Result.Fail<ImageUploadResult>($"Error uploading image: {ex.Message}");
        }
    }
    
    private static string GetFileExtensionFromMediaType(string mediaType)
    {
        return mediaType.ToLower() switch
        {
            "image/jpeg" => "jpg",
            "image/png" => "png",
            "image/gif" => "gif",
            "image/bmp" => "bmp",
            "image/tiff" => "tiff",
            _ => throw new ArgumentException("Unsupported media type", nameof(mediaType))
        };
    }
    
    private async Task<Result> UploadToS3Async(
        string fileKey, 
        byte[] fileBytes, 
        string originalFileName, 
        string contentType)
    {
        using var memoryStream = new MemoryStream(fileBytes);
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = BucketName,
            Key = fileKey,
            ContentType = contentType,
            InputStream = memoryStream,
        };
        
        if (!string.IsNullOrEmpty(originalFileName))
            putObjectRequest.Metadata.Add("OriginalFileName", originalFileName);

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
            var fileExtension = Path.GetExtension(tempImageFileKey);
            var newFileKey = $"{MainDirectory}/{directory}/{id}.{fileExtension}";
            
            if (isThumbnail)
                newFileKey = $"{MainDirectory}/{directory}/{id}_thumbnail.{fileExtension}";
            
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