using MudBlazor;

namespace StainManager.Blazor.WebUI.Server.Infrastructure;

public interface IGlobalExceptionHandler
{
    Task<T?> ExecuteAsync<T>(
        Func<Task<T>> operation, 
        string errorMessage = "An unexpected error occurred",
        bool reportToSentry = true);
    
    Task ExecuteAsync(
        Func<Task> operation, 
        string errorMessage = "An unexpected error occurred",
        bool reportToSentry = true);

    void Execute(
        Action operation,
        string errorMessage = "An unexpected error occurred",
        bool reportToSentry = true);
}

public class GlobalExceptionHandler(
    ISnackbar snackbar,
    ILogger<GlobalExceptionHandler> logger,
    ISentryHandler sentryHandler)
    : IGlobalExceptionHandler
{
    public async Task<T?> ExecuteAsync<T>(
        Func<Task<T>> operation,
        string errorMessage = "An unexpected error occurred",
        bool reportToSentry = true)
    {
        try
        {
            return await operation();
        }
        catch (Exception error)
        {
            await HandleException(error, errorMessage, reportToSentry);
            return default;
        }
    }
    
    private async Task HandleException(Exception exception, string errorMessage, bool reportToSentry)
    {
        logger.LogError(exception, errorMessage);

        if (reportToSentry)
            await sentryHandler.CaptureExceptionAsync(exception, errorMessage, Guid.NewGuid());

        // Display user-friendly message
        var displayMessage = $"{errorMessage}: {exception.Message}";
        snackbar.Add(displayMessage, Severity.Error, config => 
        {
            config.ShowCloseIcon = true;
            config.VisibleStateDuration = 8000; // 8 seconds
        });
    }
    
    public async Task ExecuteAsync(
        Func<Task> operation,
        string errorMessage = "An unexpected error occurred",
        bool reportToSentry = true)
    {
        try
        {
            await operation();
        }
        catch (Exception error)
        {
            await HandleException(error, errorMessage, reportToSentry);
        }
    }
    
    public void Execute(
        Action operation,
        string errorMessage = "An unexpected error occurred",
        bool reportToSentry = true)
    {
        try
        {
            operation();
        }
        catch (Exception error)
        {
            HandleException(error, errorMessage, reportToSentry).Wait();
        }
    }
}