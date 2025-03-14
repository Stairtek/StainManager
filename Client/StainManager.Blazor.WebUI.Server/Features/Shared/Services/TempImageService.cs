using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Components.Forms;
using StainManager.Blazor.WebUI.Server.Features.Shared.Models;
using StainManager.Blazor.WebUI.Server.Models;

namespace StainManager.Blazor.WebUI.Server.Features.Shared.Services;

public interface ITempImageService
{
    ImageValidationResult ValidateImage(
        IBrowserFile? file);
    
    Task<ImageConfigurationModel?> GetImageFromBrowserFileAsync(
        IBrowserFile file);

    Task<Result<ImageUploadResponse>> UploadTempImageAsync(
        ImageConfigurationModel imageConfigurationModel);
}

public class TempImageService(
    IHttpClientFactory httpClientFactory,
    ILogger<TempImageService> logger)
    : ITempImageService
{
    private readonly HttpClient _http = httpClientFactory.CreateClient("StainManagerAPI");
    private const string BaseUrl = "TempImage";

    public ImageValidationResult ValidateImage(IBrowserFile? file)
    {
        if (file is null)
            return ImageValidationResult.Invalid("No file selected.");
        
        if (file.Size > 10 * 1024 * 1024) // 10 MB max size
            return ImageValidationResult.Invalid("File size exceeds the maximum limit of 10 MB.");
        
        var validImageTypes = new[] { "image/jpg", "image/jpeg", "image/png" };
        
        if (!validImageTypes.Contains(file.ContentType))
            return ImageValidationResult.Invalid("Invalid file type. Only JPG, JPEG, and PNG are allowed.");
        
        return ImageValidationResult.Valid();
    }

    public async Task<ImageConfigurationModel?> GetImageFromBrowserFileAsync(
        IBrowserFile file)
    {
        if (file.Size > 10 * 1024 * 1024) // 10 MB max size
        {
            logger.LogError("File size exceeds the maximum limit of 10 MB.");
            return null;
        }
        
        using var memoryStream = new MemoryStream();
        await file.OpenReadStream(10 * 1024 * 1024).CopyToAsync(memoryStream); // 10 MB max size
        var imageContent = Convert.ToBase64String(memoryStream.ToArray());

        var imageModel = new ImageConfigurationModel(
            imageContent,
            file.Name,
            file.ContentType);
        
        return imageModel;
    }

    public async Task<Result<ImageUploadResponse>> UploadTempImageAsync(
        ImageConfigurationModel imageConfigurationModel)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/")
        {
            Content = JsonContent.Create(imageConfigurationModel)
        };
            
        var response = await _http.SendAsync(request);
        var result = await response.Content.ReadFromJsonAsync<Result<ImageUploadResponse>>();

        if (result is not null) 
            return result;
        
        var responseContent = await response.Content.ReadAsStringAsync();
        
        logger.LogError($"Failed to upload image. Status code: {response.StatusCode}, Response: {responseContent}");
        return Result.Fail<ImageUploadResponse>("Failed to upload image.");
    }
}