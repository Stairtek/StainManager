namespace StainManager.Blazor.WebUI.Server.Models;

public class StandardDataGridSettings(string title, string objectName)
{
    public string Title { get; init; } = title;
    
    public string ObjectName { get; init; } = objectName;
    
    public bool DisplaySearch { get; set; } = true;
    
    public bool DisplayShowDeletedButton { get; set; } = true;
    
    public bool DisplayRefreshButtons { get; set; } = true;
    
    public bool DisplayAddButton { get; set; } = true;
}