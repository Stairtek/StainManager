using StainManager.Domain.Common;

namespace StainManager.Domain.Textures;

public class Texture : Entity
{
    public required string Name { get; set; }
    
    public string? FullImageLocation { get; set; }

    public string? ThumbnailImageLocation { get; set; }
}