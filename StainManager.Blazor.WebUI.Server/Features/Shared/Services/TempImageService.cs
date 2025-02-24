using System.Net.Http.Headers;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Components.Forms;

namespace StainManager.Blazor.WebUI.Server.Features.Shared.Services;

public interface ITempImageService
{
    Task<HttpResponseMessage> UploadTempImageAsync(IBrowserFile file);
}

public class TempImageService(
    HttpClient http,
    ILogger<TempImageService> logger,
    IAntiforgery antiforgery, // Inject the IAntiforgery service
    IHttpContextAccessor httpContextAccessor) // Inject the IHttpContextAccessor service)
    : ITempImageService
{
    private const string BaseUrl = "TempImage";

    public async Task<HttpResponseMessage> UploadTempImageAsync(
        IBrowserFile file)
    {
        try
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
            logger.LogInformation("Temp image uploaded successfully");
            
            return response;
        }
        catch (Exception error)
        {
            logger.LogError(error, "Error uploading temp image");
            throw;
        }
    }
}