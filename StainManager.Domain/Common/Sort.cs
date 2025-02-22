namespace StainManager.Domain.Common;

public record struct Sort(
    string SortBy,
    bool Descending
);