namespace StainManager.Application.Textures.Queries.GetTexturesSummary;

public class TextureSummaryResponse
{
    public int Id { get; set; }
    
    public required string Name { get; set; }
    
    public int SortOrder { get; set; }
}