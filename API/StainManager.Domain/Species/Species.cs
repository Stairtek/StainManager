using StainManager.Domain.Common;

namespace StainManager.Domain.Species;

public class Species : Entity
{
    public required string Name { get; set; }

    public string? Abbreviation { get; set; }

    public bool IsProduction { get; set; }

    public string? FullImageLocation { get; set; }

    public string? ThumbnailImageLocation { get; set; }

    public string? ScientificName { get; set; }

    public string? CountryOfOrigin { get; set; }

    public string? JankaHardness { get; set; }
}