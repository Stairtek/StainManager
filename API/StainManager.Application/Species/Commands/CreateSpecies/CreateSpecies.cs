using StainManager.Application.Services;
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

        if (species.FullImageLocation is null || species.ThumbnailImageLocation is null) 
            return species.Adapt<SpeciesResponse>();
        
        var moveImagesResult = await imageService.MoveImagesAsync(
            species.FullImageLocation,
            species.ThumbnailImageLocation,
            "species",
            species.Id);
        
        if (moveImagesResult.Failure)
            return Result.Fail<SpeciesResponse>(moveImagesResult.Error, moveImagesResult.HandledError);
        
        species.FullImageLocation = moveImagesResult.Value?.FullImageLocation;
        species.ThumbnailImageLocation = moveImagesResult.Value?.ThumbnailImageLocation;
            
        var updateSpeciesResult = await speciesRepository.UpdateSpeciesImageLocationsAsync(
            species.Id,
            species.FullImageLocation,
            species.ThumbnailImageLocation);
            
        if (updateSpeciesResult is false)
            return Result.Fail<SpeciesResponse>("Failed to update species image locations", true);

        return species.Adapt<SpeciesResponse>();
    }
}