using StainManager.Domain.Common;
using StainManager.Domain.Species;

namespace StainManager.Application.Species.Queries.GetSpeciesForManagement;

public class GetSpeciesForManagementQuery
    : IQuery<PaginatedList<SpeciesManagementResponse>>
{
    public bool IsActive { get; set; } = true;

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;
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
                request.IsActive,
                request.PageNumber,
                request.PageSize);
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