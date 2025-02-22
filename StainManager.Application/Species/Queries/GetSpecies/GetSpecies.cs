using StainManager.Domain.Species;

namespace StainManager.Application.Species.Queries.GetSpecies;

public class GetSpeciesQuery
    : IQuery<List<SpeciesResponse>>
{
    public bool IsActive { get; set; } = true;
}

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