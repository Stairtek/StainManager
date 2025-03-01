namespace StainManager.Domain.Common;

public class PaginatedList<T>(
    List<T> items,
    int pageNumber,
    int count,
    int pageSize)
{
    public List<T> Items { get; set; } = items;

    public int PageNumber { get; set; } = pageNumber;

    public int TotalPages { get; set; } = (int)Math.Ceiling(count / (double)pageSize);

    public int TotalCount { get; set; } = count;

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;
}