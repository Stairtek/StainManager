using Microsoft.JSInterop;
using StainManager.Blazor.WebUI.Server.Models;

namespace StainManager.Blazor.WebUI.Server.Infrastructure;

public interface ISentryHandler
{
    Task CaptureExceptionAsync(
        Exception exception,
        string errorMessage,
        Guid? correlationId = null);
    
    Task CaptureExceptionAsync(
        ExceptionResult? exceptionResult);

    Task CaptureExceptionAsync(
        string? errorMessage,
        Guid? correlationId = null);
}

public class SentryHandler(
    IHub sentryHub,
    IJSRuntime jsRuntime,
    ILogger<SentryHandler> logger)
    : ISentryHandler
{
    public async Task CaptureExceptionAsync(
        Exception exception,
        string errorMessage,
        Guid? correlationId = null)
    {
        try
        {
            correlationId ??= Guid.NewGuid();
            
            sentryHub.ConfigureScope(scope =>
            {
                scope.SetTag("correlation_id", correlationId.Value.ToString());
            });
                
            sentryHub.CaptureException(exception);
                
            await jsRuntime.InvokeVoidAsync(
                "sentryHelpers.captureException", 
                new
                {
                    correlationId,
                    name = exception.GetType().Name,
                    message = exception.Message,
                    stack = exception.StackTrace,
                    source = exception.Source,
                    errorMessage
                });
        }
        catch (Exception sentryError)
        {
            logger.LogError(sentryError, "Error reporting to Sentry");
        }
    }

    public async Task CaptureExceptionAsync(
        ExceptionResult? exceptionResult)
    {
        try
        {
            if (exceptionResult is null)
                return;
            
            sentryHub.ConfigureScope(scope =>
            {
                scope.SetTag("correlation_id", exceptionResult.Id.ToString());
            });
                
            await jsRuntime.InvokeVoidAsync(
                "sentryHelpers.captureException", 
                new 
                {
                    correlationId = exceptionResult.Id,
                    name = exceptionResult.Type,
                    message = exceptionResult.DisplayMessage,
                    stack = exceptionResult.StackTrace,
                    source = exceptionResult.Source,
                    errorMessage = exceptionResult.ErrorMessage
                });
        }
        catch (Exception sentryError)
        {
            logger.LogError(sentryError, "Error reporting to Sentry");
        }
    }

    public async Task CaptureExceptionAsync(
        string? errorMessage,
        Guid? correlationId = null)
    {
        try
        {
            correlationId ??= Guid.NewGuid();

            sentryHub.ConfigureScope(scope =>
            {
                scope.SetTag("correlation_id", correlationId.Value.ToString());
            });
            
            sentryHub.CaptureMessage(errorMessage, SentryLevel.Error);
            
            await jsRuntime.InvokeVoidAsync(
                "sentryHelpers.captureException", 
                new
                {
                    correlationId,
                    name = "GeneralException",
                    message = errorMessage,
                    errorMessage
                });
        }
        catch (Exception sentryError)
        {
            logger.LogError(sentryError, "Error reporting to Sentry");
        }
    }
}