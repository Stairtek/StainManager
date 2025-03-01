using System.Text.Json;
using Mapster;
using MudBlazor;
using StainManager.Blazor.WebUI.Server.Common.Models;

namespace StainManager.Blazor.WebUI.Server.Common.Helpers;

public static class ManagementQueryHelpers
{
    public static string BuildQueryString<T>(
        string searchQuery,
        int pageNumber,
        int pageSize,
        bool showDeleted,
        SortDefinition<T>? sortDefinition = null,
        ICollection<IFilterDefinition<T>>? filterDefinitions = null)
        where T : class
    {
        var query = $"searchQuery={searchQuery}";
        query += $"&pageNumber={pageNumber}";
        query += $"&pageSize={pageSize}";
        query += $"&isActive={!showDeleted}";

        if (sortDefinition != null)
            query += $"&sort={JsonSerializer.Serialize(sortDefinition.Adapt<Sort>())}";

        if (filterDefinitions != null && filterDefinitions.Count != 0)
        {
            var filters = filterDefinitions.Adapt<List<Filter>>();
            query += $"&filters={JsonSerializer.Serialize(filters)}";
        }
        
        return query;
    }
}