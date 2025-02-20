namespace StainManager.Application.Species;

public record struct SpeciesResponse(
    int Id,
    string Name,
    string Abbreviation,
    bool IsProduction,
    string? FullImageLocation,
    string? ThumbnailImageLocation,
    string? ScientificName,
    string? CountryOfOrigin,
    string? JankaHardness);