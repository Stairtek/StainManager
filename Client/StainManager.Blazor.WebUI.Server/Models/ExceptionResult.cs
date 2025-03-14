namespace StainManager.Blazor.WebUI.Server.Models;

public class ExceptionResult
{
    public Guid Id { get; set; }
    
    public string Type { get; set; } = string.Empty;
    
    public string DisplayMessage { get; set; } = string.Empty;
    
    public string ErrorMessage { get; set; } = string.Empty;
    
    public string? StackTrace { get; set; } = string.Empty;

    public string? Source { get; set; } = string.Empty;
}