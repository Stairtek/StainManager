using StainManager.Domain.Species;

namespace StainManager.Application.Species.Commands.RestoreSpecies;

public class RestoreSpeciesCommand
    : ICommand
{
    public int Id { get; set; }
}

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