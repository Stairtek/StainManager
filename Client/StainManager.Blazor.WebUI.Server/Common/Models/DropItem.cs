namespace StainManager.Blazor.WebUI.Server.Common.Models;

public class DropItem
{
    public int Id { get; set; }
    
    public required string Name { get; set; }
    
    public int SortOrder { get; set; }
    
    public string Zone { get; set; } = "1";
}