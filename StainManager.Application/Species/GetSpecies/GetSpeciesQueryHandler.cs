using StainManager.Domain.Species;

namespace StainManager.Application.Species.GetSpecies;

public class GetSpeciesQueryHandler(
    ISpeciesRepository speciesRepository)
    : IQueryHandler<GetSpeciesQuery, List<SpeciesResponse>>
{
    public async Task<Result<List<SpeciesResponse>>> Handle(
        GetSpeciesQuery request,
        CancellationToken cancellationToken)
    {
        var species = await speciesRepository
            .GetAllSpeciesAsync(request.IsActive);
        var response = species.Adapt<List<SpeciesResponse>>();
        
        return response;
    }
}