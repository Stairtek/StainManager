using MudBlazor;

namespace StainManager.Blazor.WebUI.Server.Features.Species.Models;

public class SpeciesModel
{
    public int Id { get; set; }
    
    [Label("Name")]
    public string? Name { get; set; }
    
    [Label("Name")]
    public string? Abbreviation { get; set; }
    
    [Label("Production")]
    public bool IsProduction { get; set; }
    
    public string? FullImageLocation { get; set; }
    
    public string? ThumbnailImageLocation { get; set; }
    
    [Label("Scientific Name")]
    public string? ScientificName { get; set; }
    
    [Label("Country of Origin")]
    public string? CountryOfOrigin { get; set; }
    
    [Label("Janka Hardness")]
    public string? JankaHardness { get; set; }
}