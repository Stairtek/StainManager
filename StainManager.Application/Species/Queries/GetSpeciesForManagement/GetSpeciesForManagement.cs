using StainManager.Domain.Common;
using StainManager.Domain.Species;

namespace StainManager.Application.Species.Queries.GetSpeciesForManagement;

public class GetSpeciesForManagementQuery
    : IQuery<PaginatedList<SpeciesManagementResponse>>
{
    public string? SearchQuery { get; set; }

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public bool IsActive { get; set; } = true;

    public Sort? Sort { get; set; }

    public List<Filter>? Filters { get; set; }
}

public class GetSpeciesForManagementQueryHandler(
    ISpeciesRepository speciesRepository)
    : IQueryHandler<GetSpeciesForManagementQuery, PaginatedList<SpeciesManagementResponse>>
{
    public async Task<Result<PaginatedList<SpeciesManagementResponse>>> Handle(
        GetSpeciesForManagementQuery request,
        CancellationToken cancellationToken)
    {
        var species = await speciesRepository
            .GetSpeciesForManagementAsync(
                request.SearchQuery,
                request.PageNumber,
                request.PageSize,
                request.IsActive,
                request.Sort,
                request.Filters);
        var constructor = typeof(PaginatedList<SpeciesManagementResponse>)
            .GetConstructor([typeof(List<SpeciesManagementResponse>), typeof(int), typeof(int), typeof(int)]);

        if (constructor is null)
            throw new InvalidOperationException("Constructor not found");

        TypeAdapterConfig<PaginatedList<Domain.Species.Species>, PaginatedList<SpeciesManagementResponse>>
            .NewConfig()
            .MapToConstructor(constructor);
        var response = species.Adapt<PaginatedList<SpeciesManagementResponse>>();

        return response;
    }
}