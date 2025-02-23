using System.Linq.Expressions;
using Mapster;
using MudBlazor;
using StainManager.Domain.Common;

namespace StainManager.Infrastructure.Extensions.Filtering;

public static class FilterExpressionGenerator
{
    public static Expression<Func<T, bool>> GenerateExpression<T>(
        Filter filter)
    {
        if (filter.Title == null || filter.Value == null || filter.Operator == null)
            return x => true;

        var propertyInfo = typeof(T).GetProperty(filter.Title);
        if (propertyInfo == null)
            return x => true;

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, propertyInfo);

        if (propertyInfo.PropertyType == typeof(string))
            return GetStringExpression<T>(filter, parameter, property);
        
        // TODO: Add support for other types

        return x => true;
    }

    private static Expression<Func<T, bool>> GetStringExpression<T>(
        Filter filter,
        ParameterExpression parameter,
        MemberExpression property)
    {
        if (filter?.Value is null)
            return x => true;
        
        var value = Expression.Constant(filter.Value.ToString(), typeof(string));

        switch (filter.Operator)
        {
            case FilterOperator.String.Contains or FilterOperator.String.NotContains:
            {
                var method = typeof(string).GetMethod("Contains", [typeof(string)]);
                if (method is null)
                    return x => true;
            
                var containsMethodExpression = Expression.Call(property, method, value);
            
                if (filter.Operator is FilterOperator.String.Contains)
                    return Expression.Lambda<Func<T, bool>>(containsMethodExpression, parameter);
            
                var notContainsExpression = Expression.Not(containsMethodExpression);
                return Expression.Lambda<Func<T, bool>>(notContainsExpression, parameter);
            }
            case FilterOperator.String.StartsWith:
            {
                var method = typeof(string).GetMethod("StartsWith", [typeof(string)]);
                if (method is null)
                    return x => true;
            
                var startsWithMethodExp = Expression.Call(property, method, value);
                return Expression.Lambda<Func<T, bool>>(startsWithMethodExp, parameter);
            }
            case FilterOperator.String.EndsWith:
            {
                var method = typeof(string).GetMethod("EndsWith", [typeof(string)]);
                if (method is null)
                    return x => true;
            
                var endsWithMethodExp = Expression.Call(property, method, value);
                return Expression.Lambda<Func<T, bool>>(endsWithMethodExp, parameter);
            }
            case FilterOperator.String.Equal:
            {
                var equalExpression = Expression.Equal(property, value);
                return Expression.Lambda<Func<T, bool>>(equalExpression, parameter);
            }
            case FilterOperator.String.NotEqual:
            {
                var notEqualExpression = Expression.NotEqual(property, value);
                return Expression.Lambda<Func<T, bool>>(notEqualExpression, parameter);
            }
            case FilterOperator.String.Empty:
            {
                var emptyExpression = Expression.Equal(property, Expression.Constant(string.Empty));
                return Expression.Lambda<Func<T, bool>>(emptyExpression, parameter);
            }
            case FilterOperator.String.NotEmpty:
            {
                var notEmptyExpression = Expression.NotEqual(property, Expression.Constant(string.Empty));
                return Expression.Lambda<Func<T, bool>>(notEmptyExpression, parameter);
            }
            default:
                return x => true;
        }
    }
}