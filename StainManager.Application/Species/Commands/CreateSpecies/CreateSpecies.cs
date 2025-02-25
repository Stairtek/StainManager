using StainManager.Application.Services;
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
    ISpeciesRepository speciesRepository,
    IImageService imageService)
    : ICommandHandler<CreateSpeciesCommand, SpeciesResponse>
{
    public async Task<Result<SpeciesResponse>> Handle(
        CreateSpeciesCommand request,
        CancellationToken cancellationToken)
    {
        var newSpecies = request.Adapt<Domain.Species.Species>();
        var species = await speciesRepository.CreateSpeciesAsync(newSpecies);
        
        if (species.FullImageLocation is not null && 
            species.ThumbnailImageLocation is not null)
        {
            var moveFullImageResult = await imageService.MoveTempImageAsync(
                species.FullImageLocation,
                "species",
                species.Id);
                
            if (moveFullImageResult.Failure)
                return Result.Fail<SpeciesResponse>(moveFullImageResult.Error);
            
            var moveThumbnailImageResult = await imageService.MoveTempImageAsync(
                species.ThumbnailImageLocation,
                "species",
                species.Id);
            
            if (moveThumbnailImageResult.Failure)
                return Result.Fail<SpeciesResponse>(moveThumbnailImageResult.Error);
            
            species.FullImageLocation = moveFullImageResult.Value;
            species.ThumbnailImageLocation = moveThumbnailImageResult.Value;
            
            var updateSpeciesResult = await speciesRepository.UpdateSpeciesImageLocationsAsync(
                species.Id,
                species.FullImageLocation,
                species.ThumbnailImageLocation);
            
            if (updateSpeciesResult is false)
                return Result.Fail<SpeciesResponse>("Failed to update species image locations");
        }
        
        return species.Adapt<SpeciesResponse>();
    }
}