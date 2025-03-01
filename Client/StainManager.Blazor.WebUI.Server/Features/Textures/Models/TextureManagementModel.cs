namespace StainManager.Blazor.WebUI.Server.Features.Textures.Models;

public class TextureManagementModel
{
    public int Id { get; init; }

    public string? Name { get; init; }

    public string? ThumbnailImageLocation { get; init; }
    
    public int SortOrder { get; init; }
}