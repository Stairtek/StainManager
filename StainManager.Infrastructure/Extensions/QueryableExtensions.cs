using StainManager.Domain.Common;

namespace StainManager.Infrastructure.Extensions;

public static class QueryableExtensions
{
    public static async Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(
        this IQueryable<TDestination> queryable,
        int pageNumber,
        int pageSize)
        where TDestination : class
    {
        return await queryable.CreateAsync(pageNumber, pageSize);
    }
}