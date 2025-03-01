namespace StainManager.Application.Textures;

public record struct TextureResponse(
    int Id,
    string Name,
    string? FullImageLocation,
    string? ThumbnailImageLocation);