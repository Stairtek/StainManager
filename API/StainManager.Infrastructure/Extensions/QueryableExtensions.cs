using System.Linq.Expressions;
using System.Text.Json;
using StainManager.Domain.Common;
using StainManager.Infrastructure.Extensions.Filtering;

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

    public static IQueryable<T> ApplySorting<T>(
        this IQueryable<T> query,
        Sort? sort)
    {
        if (sort is null)
            return query;

        var propertyInfo = typeof(T).GetProperty(sort.Value.SortBy);
        if (propertyInfo is null)
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, propertyInfo);
        var lambda = Expression.Lambda(property, parameter);

        var methodName = sort.Value.Descending ? "OrderByDescending" : "OrderBy";
        var resultExpression = Expression.Call(
            typeof(Queryable), methodName,
            [typeof(T), propertyInfo.PropertyType],
            query.Expression,
            Expression.Quote(lambda));

        return query.Provider.CreateQuery<T>(resultExpression);
    }

    public static IQueryable<T> ApplyFilters<T>(
        this IQueryable<T> query,
        List<Filter>? filters)
    {
        if (filters == null || filters.Count == 0)
            return query;

        return filters
            .Select(FilterExpressionGenerator.GenerateExpression<T>)
            .Aggregate(
                query, 
                (current, filterExpression) => 
                    current.Where(filterExpression));
    }
}