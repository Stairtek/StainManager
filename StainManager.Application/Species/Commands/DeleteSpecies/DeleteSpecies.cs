using StainManager.Application.Common.RequestHandling;
using StainManager.Domain.Common;
using StainManager.Domain.Species;

namespace StainManager.Application.Species.Commands.DeleteSpecies;

public class DeleteSpeciesCommand
    : ICommand
{
    public int Id { get; set; }
}

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