using StainManager.Domain.Species;

namespace StainManager.Application.Species.DeleteSpecies;

public class DeleteSpeciesCommandHandler(
    ISpeciesRepository speciesRepository)
    : ICommandHandler<DeleteSpeciesCommand>
{
    public async Task<Result> Handle(
        DeleteSpeciesCommand request,
        CancellationToken cancellationToken)
    {
        var result = await speciesRepository
            .DeleteSpeciesAsync(request.Id);
        
        return !result 
            ? Result.Fail("Species not found") 
            : Result.Ok();
    }
}