using StainManager.Application.Services;
using StainManager.Domain.Common;
using StainManager.Domain.Species;

namespace StainManager.Application.Species.Commands.UpdateSpecies;

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

public class UpdateSpeciesCommandHandler(
    ISpeciesRepository speciesRepository,
    IImageService imageService)
    : ICommandHandler<UpdateSpeciesCommand, SpeciesResponse?>
{
    public async Task<Result<SpeciesResponse?>> Handle(
        UpdateSpeciesCommand request,
        CancellationToken cancellationToken)
    {
        var updatedSpecies = request.Adapt<Domain.Species.Species>();
        
        var hasTempImages =
            updatedSpecies.FullImageLocation?.Contains("temp") == true || 
            updatedSpecies.ThumbnailImageLocation?.Contains("temp") == true;

        if (hasTempImages)
        {
            var moveImagesResult = await imageService.MoveImagesAsync(
                updatedSpecies.FullImageLocation,
                updatedSpecies.ThumbnailImageLocation,
                "species",
                updatedSpecies.Id);
        
            if (moveImagesResult.Failure)
                return Result.Fail<SpeciesResponse?>(moveImagesResult.Error);
            
            updatedSpecies.FullImageLocation = moveImagesResult.Value?.FullImageLocation;
            updatedSpecies.ThumbnailImageLocation = moveImagesResult.Value?.ThumbnailImageLocation;
        }

        var result = await speciesRepository.UpdateSpeciesAsync(updatedSpecies);

        return result.Adapt<SpeciesResponse>();
    }
}