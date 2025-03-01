using StainManager.Domain.Common;
using StainManager.Domain.Species;

namespace StainManager.Application.Species.Queries.GetSpeciesById;

public class GetSpeciesByIdQuery
    : IQuery<SpeciesResponse>
{
    public int Id { get; set; }
}

public class GetSpeciesByIdQueryHandler(
    ISpeciesRepository speciesRepository)
    : IQueryHandler<GetSpeciesByIdQuery, SpeciesResponse>
{
    public async Task<Result<SpeciesResponse>> Handle(
        GetSpeciesByIdQuery request,
        CancellationToken cancellationToken)
    {
        var species = await speciesRepository
            .GetSpeciesByIdAsync(request.Id);
        
        if (species is null)
            return Result.Fail<SpeciesResponse>($"Species with ID {request.Id} not found.", true);
        
        var response = species.Adapt<SpeciesResponse>();

        return response;
    }
}