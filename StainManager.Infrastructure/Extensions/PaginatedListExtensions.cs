using StainManager.Domain.Common;

namespace StainManager.Infrastructure.Extensions;

public static class PaginatedListExtensions
{
    public static async Task<PaginatedList<T>> CreateAsync<T>(
        this IQueryable<T> source,
        int pageNumber,
        int pageSize)
        where T : class
    {
        var count = await source.CountAsync();
        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<T>(items, pageNumber, count, pageSize);
    }
}