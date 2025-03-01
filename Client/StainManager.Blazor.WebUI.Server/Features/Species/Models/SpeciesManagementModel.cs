namespace StainManager.Blazor.WebUI.Server.Features.Species.Models;

public class SpeciesManagementModel
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Abbreviation { get; set; }

    public bool IsProduction { get; set; }

    public string? ThumbnailImageLocation { get; set; }
}