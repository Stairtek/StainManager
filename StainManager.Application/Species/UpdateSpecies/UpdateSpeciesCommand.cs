using StainManager.Application.Common.RequestHandling;

namespace StainManager.Application.Species.UpdateSpecies;

public class UpdateSpeciesCommand
    : ICommand<SpeciesResponse?>
{
    public int Id { get; set; }
    
    public required string Name { get; set; }
    
    public required string Abbreviation { get; set; }
    
    public bool IsProduction { get; set; }
    
    public string? FullImageLocation { get; set; }
    
    public string? ThumbnailImageLocation { get; set; }
    
    public string? ScientificName { get; set; }
    
    public string? CountryOfOrigin { get; set; }
    
    public string? JankaHardness { get; set; }
}