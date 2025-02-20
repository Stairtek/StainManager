using StainManager.Domain.Species;

namespace StainManager.Application.Species.RestoreSpecies;

public class RestoreSpeciesCommandHandler(
    ISpeciesRepository speciesRepository)
    : ICommandHandler<RestoreSpeciesCommand>
{
    public async Task<Result> Handle(
        RestoreSpeciesCommand request,
        CancellationToken cancellationToken)
    {
        var result = await speciesRepository
            .RestoreSpeciesAsync(request.Id);
        
        return !result 
            ? Result.Fail("Species not found") 
            : Result.Ok();
    }
}