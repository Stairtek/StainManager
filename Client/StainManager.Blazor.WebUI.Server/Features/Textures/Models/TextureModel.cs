using MudBlazor;

namespace StainManager.Blazor.WebUI.Server.Features.Textures.Models;

public class TextureModel
{
    public int Id { get; set; }
    
    [Label("Name")]
    public string? Name { get; set; }
    
    public string? FullImageLocation { get; set; }
    
    public string? ThumbnailImageLocation { get; set; }
}