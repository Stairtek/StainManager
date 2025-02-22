namespace StainManager.Application.Species.Queries.GetSpeciesForManagement;

public record struct SpeciesManagementResponse(
    int Id,
    string Name,
    string Abbreviation,
    bool IsProduction,
    string? ThumbnailImageLocation);