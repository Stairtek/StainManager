using StainManager.Domain.Species;

namespace StainManager.Application.Species.CreateSpecies;

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