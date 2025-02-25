using System.Net.Http.Headers;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Components.Forms;
using StainManager.Blazor.WebUI.Server.Features.Shared.Models;

namespace StainManager.Blazor.WebUI.Server.Features.Shared.Services;

public interface ITempImageService
{
    Task<ImageUploadResponse?> UploadTempImageAsync(IBrowserFile file);
}

public class TempImageService(
    HttpClient http,
    ILogger<TempImageService> logger) // Inject the IHttpContextAccessor service)
    : ITempImageService
{
    private const string BaseUrl = "TempImage";

    public async Task<ImageUploadResponse?> UploadTempImageAsync(
        IBrowserFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.OpenReadStream(10 * 1024 * 1024).CopyToAsync(memoryStream); // 10 MB max size
        var imageContent = Convert.ToBase64String(memoryStream.ToArray());

        var uploadTempImageCommand = new { ImageContent = imageContent };
        var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/")
        {
            Content = JsonContent.Create(uploadTempImageCommand)
        };
            
        var response = await http.SendAsync(request);

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<ImageUploadResponse?>();
        
        var errorMessage = await response.Content.ReadAsStringAsync();
        logger.LogError("Failed to upload image: {ErrorMessage}", errorMessage);
        return null;
    }
}