namespace StainManager.Blazor.WebUI.Server.Common.Models;

public record struct Filter(
    string? Title,
    string? Operator,
    object? Value
);