namespace StainManager.Blazor.WebUI.Server.Models;

public record struct Filter(
    string? Title,
    string? Operator,
    object? Value
);