using StainManager.Application.Common.RequestHandling;
using StainManager.Domain.Species;

namespace StainManager.Application.Species.UpdateSpecies;

public class UpdateSpeciesCommandHandler(
    ISpeciesRepository speciesRepository)
    : ICommandHandler<UpdateSpeciesCommand, SpeciesResponse?>
{
    public async Task<Result<SpeciesResponse?>> Handle(
        UpdateSpeciesCommand request,
        CancellationToken cancellationToken)
    {
        var updatedSpecies = request.Adapt<Domain.Species.Species>();
        
        var result = await speciesRepository.UpdateSpeciesAsync(updatedSpecies);
        
        return result?.Adapt<SpeciesResponse>() 
               ?? Result.Fail<SpeciesResponse?>("Species not found");
    }
}