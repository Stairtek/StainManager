using System.Globalization;
using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
            typeof(System.Linq.Queryable), methodName,
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

        foreach (var filter in filters)
        {
            if (filter.Title == null || filter.Value == null || filter.Operator == null)
                continue;

            var propertyInfo = typeof(T).GetProperty(filter.Title);
            if (propertyInfo == null)
                continue;

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyInfo);
            
            var objectValue = GetObjectValue(filter.Value);
            
            if (objectValue == null)
                continue;
            
            var value = Convert.ChangeType(objectValue, propertyInfo.PropertyType, CultureInfo.InvariantCulture);
            var constant = Expression.Constant(value);

            var comparison = Expression.Call(property, filter.Operator, null, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);

            query = query.Where(lambda);
        }

        return query;
    }

    private static object? GetObjectValue(object obj)
    {
        var typeOfObject = ((JsonElement)obj).ValueKind;

        return typeOfObject switch
        {
            JsonValueKind.Number => ((JsonElement)obj).GetInt32(),
            JsonValueKind.String => ((JsonElement)obj).GetString()!,
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => null
        };
    }
}