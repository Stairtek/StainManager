namespace StainManager.Blazor.WebUI.Server.Features.Shared.Models;

public class ImageUploadResponse
{
    public string FullImageURL { get; set; } = string.Empty;
    
    public string ThumbnailImageURL { get; set; } = string.Empty;
}