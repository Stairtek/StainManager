namespace StainManager.Application.Textures.Queries.GetTexturesForManagement;

public record struct TextureManagementResponse(
    int Id,
    string Name,
    string? ThumbnailImageLocation);