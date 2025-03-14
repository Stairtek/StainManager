namespace StainManager.Blazor.WebUI.Server.Models;

public class ImageConfigurationModel(
    string imageContent,
    string fileName,
    string mediaType)
{
    public string ImageContent { get; } = imageContent;

    public string FileName { get; } = fileName;

    public string MediaType { get; } = mediaType.Replace(";base64", string.Empty);
}