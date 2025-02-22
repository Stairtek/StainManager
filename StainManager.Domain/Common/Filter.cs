namespace StainManager.Domain.Common;

public class Filter
{
    public string? Title { get; set; }

    public string? Operator { get; set; }

    public object? Value { get; set; }
}