namespace StainManager.Blazor.WebUI.Server.Models;

public class ImageValidationResult
{
    public bool IsValid { get; private init; }

    public string ErrorMessage { get; private init; } = string.Empty;


    public static ImageValidationResult Valid()
    {
        return new ImageValidationResult()
        {
            IsValid = true
        };
    }
    
    public static ImageValidationResult Invalid(string errorMessage)
    {
        return new ImageValidationResult()
        {
            IsValid = false,
            ErrorMessage = errorMessage
        };
    }
}