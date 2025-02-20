using StainManager.Application.Common.RequestHandling;
using StainManager.Domain.Common;
using StainManager.Domain.Species;

namespace StainManager.Application.Species.Commands.CreateSpecies;

public class CreateSpeciesCommand
    : ICommand<SpeciesResponse>
{
    public required string Name { get; set; }
    
    public required string Abbreviation { get; set; }
    
    public bool IsProduction { get; set; }
    
    public string? FullImageLocation { get; set; }
    
    public string? ThumbnailImageLocation { get; set; }
    
    public string? ScientificName { get; set; }
    
    public string? CountryOfOrigin { get; set; }
    
    public string? JankaHardness { get; set; }
}

public class CreateSpeciesCommandHandler(
    ISpeciesRepository speciesRepository)
    : ICommandHandler<CreateSpeciesCommand, SpeciesResponse>
{
    public async Task<Result<SpeciesResponse>> Handle(
        CreateSpeciesCommand request,
        CancellationToken cancellationToken)
    {
        var newSpecies = request.Adapt<Domain.Species.Species>();
        var species = await speciesRepository.CreateSpeciesAsync(newSpecies);
        
        return species.Adapt<SpeciesResponse>();
    }
}