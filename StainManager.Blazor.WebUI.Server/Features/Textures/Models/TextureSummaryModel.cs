namespace StainManager.Blazor.WebUI.Server.Features.Textures.Models;

public class TextureSummaryModel
{
    public int Id { get; set; }
    
    public required string Name { get; set; }
    
    public int SortOrder { get; set; }
}