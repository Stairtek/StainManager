namespace StainManager.Blazor.WebUI.Server.Common.Models;

public class PaginatedList<T>
{
    public List<T> Items { get; init; }

    public int PageNumber { get; init; }

    public int TotalPages { get; init; }

    public int TotalCount { get; init; }

    public bool HasPreviousPage { get; init; }

    public bool HasNextPage { get; init; }
}