using System.Linq.Expressions;
using StainManager.Domain.Common;

namespace StainManager.Infrastructure.Extensions;

public static class Sorting
{
    public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query,
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
}